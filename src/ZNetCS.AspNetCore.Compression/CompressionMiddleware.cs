// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompressionMiddleware.cs" company="Marcin Smółka">
//   Copyright (c) Marcin Smółka. All rights reserved.
// </copyright>
// <summary>
//   The compression middleware.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZNetCS.AspNetCore.Compression;

#region Usings

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using ZNetCS.AspNetCore.Compression.Infrastructure;

#endregion

/// <summary>
/// The compression middleware.
/// </summary>
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Public API")]
public class CompressionMiddleware
{
    #region Fields

    /// <summary>
    /// The logger.
    /// </summary>
    private readonly ILogger logger;

    /// <summary>
    /// The next delegate.
    /// </summary>
    private readonly RequestDelegate next;

    /// <summary>
    /// The options.
    /// </summary>
    private readonly CompressionOptions options;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="CompressionMiddleware"/> class.
    /// </summary>
    /// <param name="next">
    /// The <see cref="RequestDelegate"/> representing the next middleware in the pipeline.
    /// </param>
    /// <param name="loggerFactory">
    /// The logger factory.
    /// </param>
    /// <param name="options">
    /// The <see cref="CompressionOptions"/> representing the options for the <see cref="CompressionMiddleware"/>.
    /// </param>
    public CompressionMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IOptions<CompressionOptions> options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        this.next = next;
        this.logger = loggerFactory.CreateLogger<CompressionMiddleware>();
        this.options = options.Value;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Invokes middleware.
    /// </summary>
    /// <param name="context">
    /// The <see cref="HttpContext"/> context.
    /// </param>
    public async Task Invoke(HttpContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        CancellationToken cancellationToken = context.RequestAborted;
        var decompressionExecutor = context.RequestServices.GetRequiredService<DecompressionExecutor>();

        // first decompress incoming request
        this.logger.DecompressionPathCheck(context.Request.Path);
        if (decompressionExecutor.CanDecompress(context, this.options.Decompressors))
        {
            await decompressionExecutor.ExecuteAsync(context, this.options.Decompressors!, cancellationToken);
        }

        this.logger.CompressionPathCheck(context.Request.Path);
        var compressionExecutor = context.RequestServices.GetRequiredService<CompressionExecutor>();

        // check we are supporting accepted encodings and request path is not ignored
        if (compressionExecutor.CanCompress(context, this.options.IgnoredPaths) && compressionExecutor.CanCompress(context, this.options.Compressors))
        {
            await using var bufferedStream = new MemoryStream();
            Stream bodyStream = context.Response.Body;
            context.Response.Body = bufferedStream;

            try
            {
                await this.next.Invoke(context);
            }
            finally
            {
                context.Response.Body = bodyStream;
            }

            bufferedStream.Seek(0, SeekOrigin.Begin);

            if ((bufferedStream.Length <= 0) || !context.Response.Body.CanWrite)
            {
                // there is no content so there is no need to compress - this prevents errors on no content result.
                this.logger.NoCompressionBody();
                return;
            }

            // skip compression for small requests, and not allowed media types
            if ((bufferedStream.Length < this.options.MinimumCompressionThreshold) || !compressionExecutor.CanCompress(context, this.options.AllowedMediaTypes))
            {
                // simply copy buffed value to output stream
                this.logger.NoCompression();
                await bufferedStream.CopyToAsync(context.Response.Body, Consts.DefaultBufferSize, cancellationToken);
            }
            else
            {
                // compress buffered stream directly to output body
                await compressionExecutor.ExecuteAsync(context, bufferedStream, this.options.Compressors!, cancellationToken);
            }
        }
        else
        {
            this.logger.NoCompression();
            await this.next.Invoke(context);
        }

        this.logger.Finished();
    }

    #endregion
}