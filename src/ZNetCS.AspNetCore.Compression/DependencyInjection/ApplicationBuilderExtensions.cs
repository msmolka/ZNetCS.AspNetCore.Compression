// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationBuilderExtensions.cs" company="Marcin Smółka zNET Computer Solutions">
//   Copyright (c) Marcin Smółka zNET Computer Solutions. All rights reserved.
// </copyright>
// <summary>
//   Extension methods for adding the <see cref="CompressionMiddleware" /> to an application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZNetCS.AspNetCore.Compression.DependencyInjection
{
    #region Usings

    using System;
    using System.Diagnostics.CodeAnalysis;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Options;

    #endregion

    /// <summary>
    /// Extension methods for adding the <see cref="CompressionMiddleware"/> to an application.
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        #region Public Methods

        /// <summary>
        /// Adds the <see cref="CompressionMiddleware"/> to automatically set compressors and decompressors for
        /// requests and responses based on information provided by the client.
        /// </summary>
        /// <param name="app">
        /// The <see cref="IApplicationBuilder"/>.
        /// </param>
        /// <returns>
        /// The <see cref="IApplicationBuilder"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1625:ElementDocumentationMustNotBeCopiedAndPasted", Justification = "OK")]
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "OK")]
        public static IApplicationBuilder UseCompression(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<CompressionMiddleware>();
        }

        /// <summary>
        /// Adds the <see cref="CompressionMiddleware"/> to automatically  set compressors and decompressors for
        /// requests and responses based on information provided by the client.
        /// </summary>
        /// <param name="app">
        /// The <see cref="IApplicationBuilder"/>.
        /// </param>
        /// <param name="options">
        /// The <see cref="CompressionOptions"/> to configure the middleware with.
        /// </param>
        /// <returns>
        /// The <see cref="IApplicationBuilder"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1625:ElementDocumentationMustNotBeCopiedAndPasted", Justification = "OK")]
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "OK")]
        public static IApplicationBuilder UseCompression(
            this IApplicationBuilder app,
            CompressionOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return app.UseMiddleware<CompressionMiddleware>(Options.Create(options));
        }

        #endregion
    }
}