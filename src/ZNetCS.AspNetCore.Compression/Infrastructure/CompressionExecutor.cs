// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompressionExecutor.cs" company="Marcin Smółka zNET Computer Solutions">
//   Copyright (c) Marcin Smółka zNET Computer Solutions. All rights reserved.
// </copyright>
// <summary>
//   The compression executor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZNetCS.AspNetCore.Compression.Infrastructure
{
    #region Usings

    #region Usings

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Primitives;
    using Microsoft.Net.Http.Headers;

    #endregion

    #endregion

    /// <summary>
    /// The compression executor.
    /// </summary>
    public class CompressionExecutor
    {
        #region Fields

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CompressionExecutor"/> class.
        /// </summary>
        /// <param name="loggerFactory">
        /// The logger factory.
        /// </param>
        public CompressionExecutor(ILoggerFactory loggerFactory) => this.logger = loggerFactory.CreateLogger<CompressionExecutor>();

        #endregion

        #region Public Methods

        /// <summary>
        /// Checks if compression is allowed for given media types.
        /// </summary>
        /// <param name="context">
        /// The <see cref="HttpContext"/> context.
        /// </param>
        /// <param name="allowedMediaTypes">
        /// Allowed content types that can be compressed.
        /// </param>
        public bool CanCompress(HttpContext context, IEnumerable<MediaTypeHeaderValue> allowedMediaTypes)
        {
            if (allowedMediaTypes == null)
            {
                return false;
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            // check if content type is allowed to be compressed
            return MediaTypeHeaderValue.TryParse(context.Response.ContentType, out MediaTypeHeaderValue contentType)
                   && allowedMediaTypes.Any(m => contentType.Equals(m) || contentType.IsSubsetOf(m));
        }

        /// <summary>
        /// Checks if compression is allowed for given request path.
        /// </summary>
        /// <param name="context">
        /// The <see cref="HttpContext"/> context.
        /// </param>
        /// <param name="ignoredPaths">
        /// Collection of paths t be ignored.
        /// </param>
        public bool CanCompress(HttpContext context, IEnumerable<string> ignoredPaths)
        {
            if (ignoredPaths == null)
            {
                return true;
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            // check if request path should be ignored
            return !context.Request.Path.HasValue || !ignoredPaths.Any(i => context.Request.Path.Value.StartsWith(i, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Checks if context can be compressed based on content encoding and defined compressor list.
        /// </summary>
        /// <param name="context">
        /// The <see cref="HttpContext"/> context.
        /// </param>
        /// <param name="compressors">
        /// The collection of available compressors.
        /// </param>
        public bool CanCompress(HttpContext context, IEnumerable<ICompressor> compressors)
        {
            if (compressors == null)
            {
                return false;
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            // check if there is available compressor
            var acceptEncodings = GetAcceptEncodings(context).Select(a => a.Value.ToString());
            return compressors.Any(c => acceptEncodings.Contains(c.ContentCoding, StringComparer.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Executes asynchronously compression process and updates response based on result.
        /// </summary>
        /// <param name="context">
        /// The <see cref="HttpContext"/> context.
        /// </param>
        /// <param name="bufferedStream">
        /// The stream to be compressed and send to response body.
        /// </param>
        /// <param name="compressors">
        /// The collection of available compressors.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token.
        /// </param>
        public async Task ExecuteAsync(HttpContext context, Stream bufferedStream, ICollection<ICompressor> compressors, CancellationToken cancellationToken)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            ICompressor compressor = FindCompressor(context, compressors);

            if (compressor != null)
            {
                this.logger.LogDebug($"Compressing response using {compressor.ContentCoding} compressor.");

                // we need to wrap to be able to count length after compression, body is unreadable
                // also all headers needs to be set before data is starting to be copied to output body
                // without wrapper we cannot apply headers after compression
                using (var compressionStream = new MemoryStream())
                {
                    await compressor.CompressAsync(bufferedStream, compressionStream, cancellationToken);

                    // stream is compressed, so set proper length.
                    context.Response.ContentLength = compressionStream.Length;

                    compressionStream.Seek(0, SeekOrigin.Begin);

                    var contentEncodings = context.Response.Headers.GetCommaSeparatedValues(HeaderNames.ContentEncoding) ?? Array.Empty<string>();

                    // lets add new content coding on the end of list
                    contentEncodings = contentEncodings.Concat(new[] { compressor.ContentCoding }).ToArray();
                    context.Response.Headers[HeaderNames.ContentEncoding] = new StringValues(contentEncodings);

                    // the good practice is to add vary header
                    var vary = context.Response.Headers.GetCommaSeparatedValues(HeaderNames.Vary) ?? Array.Empty<string>();
                    if (!vary.Contains(HeaderNames.AcceptEncoding))
                    {
                        vary = vary.Concat(new[] { HeaderNames.AcceptEncoding }).ToArray();
                        context.Response.Headers[HeaderNames.Vary] = new StringValues(vary);
                    }

                    // remove content md5, because content changes and header is already deprecated.
                    context.Response.Headers.Remove(HeaderNames.ContentMD5);

                    await compressionStream.CopyToAsync(context.Response.Body, Consts.DefaultBufferSize, cancellationToken);
                }

                this.logger.LogDebug("Finished compressing request.");
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Finds best suitable compressor.
        /// </summary>
        /// <param name="context">
        /// The <see cref="HttpContext"/> context.
        /// </param>
        /// <param name="compressors">
        /// The collection of available compressors.
        /// </param>
        private static ICompressor FindCompressor(HttpContext context, ICollection<ICompressor> compressors)
        {
            var acceptEncodings = GetAcceptEncodings(context);

            ICompressor compressor = null;

            foreach (StringWithQualityHeaderValue ae in acceptEncodings)
            {
                if ((compressor = compressors.FirstOrDefault(c => c.ContentCoding.Equals(ae.Value.ToString(), StringComparison.InvariantCultureIgnoreCase))) != null)
                {
                    break;
                }
            }

            return compressor;
        }

        /// <summary>
        /// Gets available content encodings for response.
        /// </summary>
        /// <param name="context">
        /// The <see cref="HttpContext"/> context.
        /// </param>
        private static IEnumerable<StringWithQualityHeaderValue> GetAcceptEncodings(HttpContext context)
        {
            var accpetEncodings = context.Request.Headers.GetCommaSeparatedValues(HeaderNames.AcceptEncoding) ?? Array.Empty<string>();

            // See https://tools.ietf.org/html/rfc7231#section-5.3.4
            // 3. If the representation's content-coding is one of the
            // content-codings listed in the Accept-Encoding field, then it is
            // acceptable unless it is accompanied by a qvalue of 0. (As
            // defined in Section 5.3.1, a qvalue of 0 means "not acceptable".)
            // 4. If multiple content-codings are acceptable, then the acceptable
            // content-coding with the highest non-zero qvalue is preferred.
            return accpetEncodings
                .Select(a => StringWithQualityHeaderValue.Parse(a))
                .Where(e => (e.Quality == null) || (e.Quality > 0))
                .OrderByDescending(e => e.Quality ?? 1);
        }

        #endregion
    }
}