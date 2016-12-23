// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DecompressionExecutor.cs" company="Marcin Smółka zNET Computer Solutions">
//   Copyright (c) Marcin Smółka zNET Computer Solutions. All rights reserved.
// </copyright>
// <summary>
//   The decompression excecutor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZNetCS.AspNetCore.Compression.Infrastructure
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Primitives;
    using Microsoft.Net.Http.Headers;

    #endregion

    /// <summary>
    /// The decompression executor.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "OK")]
    public class DecompressionExecutor
    {
        #region Fields

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DecompressionExecutor"/> class.
        /// </summary>
        /// <param name="loggerFactory">
        /// The logger factory.
        /// </param>
        public DecompressionExecutor(ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger<DecompressionExecutor>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Checks if context can be decompressed based on content encoding and defined decompressor list.
        /// </summary>
        /// <param name="context">
        /// The <see cref="HttpContext"/> context.
        /// </param>
        /// <param name="decompressors">
        /// The collection of available decompresssors.
        /// </param>
        public bool CanDecompress(HttpContext context, IEnumerable<IDecompressor> decompressors)
        {
            if (decompressors == null)
            {
                return false;
            }

            var contentEncodings = context.Request.Headers.GetCommaSeparatedValues(HeaderNames.ContentEncoding) ?? new string[0];

            string ce = contentEncodings.LastOrDefault();
            return decompressors.Any(d => d.ContentCoding.Equals(ce, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Executes asynchronously decompression process and updates request based on result.
        /// </summary>
        /// <param name="context">
        /// The <see cref="HttpContext"/> context.
        /// </param>
        /// <param name="decompressors">
        /// The collection of available decompresssors.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token.
        /// </param>
        public async Task ExecuteAsync(HttpContext context, IEnumerable<IDecompressor> decompressors, CancellationToken cancellationToken)
        {
            // If one or more encodings have been applied to a representation, the
            // sender that applied the encodings MUST generate a Content-Encoding
            // header field that lists the content codings in the order in which
            // they were applied. Additional information about the encoding
            // parameters can be provided by other header fields not defined by this
            // specification.
            var contentEncodings = context.Request.Headers.GetCommaSeparatedValues(HeaderNames.ContentEncoding) ?? new string[0];

            // We can only decompress last encoding on list, because encodings where applied in order, so they have to be decomposed in reverse order
            string contentEncoding = contentEncodings.LastOrDefault();

            IDecompressor decompressor = decompressors.FirstOrDefault(c => c.ContentCoding.Equals(contentEncoding, StringComparison.OrdinalIgnoreCase));

            if (decompressor != null)
            {
                this.logger.LogDebug($"Decompressing request using {decompressor.ContentCoding} decompressor.");

                Stream decompressed = new MemoryStream();

                using (Stream requestBody = context.Request.Body)
                {
                    // decompress here
                    await decompressor.DecompressAsync(requestBody, decompressed, cancellationToken);

                    // move to beggining of stream
                    decompressed.Seek(0, SeekOrigin.Begin);

                    // stream is decompressed, so set proper length.
                    context.Request.ContentLength = decompressed.Length;

                    // remove encoding already processed from list, the last one
                    contentEncodings = contentEncodings.Take(contentEncodings.Length - 1).ToArray();

                    // update content-encoding header because it is no longer valid
                    if (contentEncodings.Any())
                    {
                        context.Request.Headers[HeaderNames.ContentEncoding] = new StringValues(contentEncodings);
                    }
                    else
                    {
                        context.Request.Headers.Remove(HeaderNames.ContentEncoding);
                    }

                    // reasign new decompressed stream
                    context.Request.Body = decompressed;
                }

                this.logger.LogDebug("Finished decompressing request.");
            }
        }

        #endregion
    }
}