// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestsBase.cs" company="Marcin Smółka zNET Computer Solutions">
//   Copyright (c) Marcin Smółka zNET Computer Solutions. All rights reserved.
// </copyright>
// <summary>
//   The base tests class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZNetCS.AspNetCore.CompressionTest
{
    #region Usings

    using System;
    using System.IO;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;

    using ZNetCS.AspNetCore.Compression;
    using ZNetCS.AspNetCore.Compression.DependencyInjection;

    #endregion

    /// <summary>
    /// The base tests class.
    /// </summary>
    public abstract class TestsBase
    {
        #region Public Methods

        /// <summary>
        /// The create builder.
        /// </summary>
        /// <param name="configure">
        /// The <see cref="CompressionMiddleware"/> configuration delegate.
        /// </param>
        public IWebHostBuilder CreateBuilder(Action<CompressionOptions> configure = null)
        {
            if (configure == null)
            {
                configure = options => { };
            }

            IWebHostBuilder builder = new WebHostBuilder()
                .ConfigureServices(s => s.AddCompression(configure))
                .Configure(
                    app =>
                    {
                        app.UseCompression();
                        app.Run(
                            async c =>
                            {
                                c.Response.ContentType = "text/plain";
                                c.Response.ContentLength = Helpers.ResponseText.Length;
                                await c.Response.WriteAsync(Helpers.ResponseText);
                            });
                    });

            return builder;
        }

        /// <summary>
        /// The create decompression builder.
        /// </summary>
        /// <param name="configure">
        /// The <see cref="CompressionMiddleware"/> configuration delegate.
        /// </param>
        public IWebHostBuilder CreateDecompressionBuilder(Action<CompressionOptions> configure = null)
        {
            if (configure == null)
            {
                configure = options => { };
            }

            IWebHostBuilder builder = new WebHostBuilder()
                .ConfigureServices(s => s.AddCompression(configure))
                .Configure(
                    app =>
                    {
                        app.UseCompression();
                        app.Run(
                            async c =>
                            {
                                string text;

                                using (var reader = new StreamReader(c.Request.Body))
                                {
                                    text = reader.ReadToEnd();
                                }

                                c.Response.ContentType = "text/plain";
                                c.Response.ContentLength = text.Length;
                                await c.Response.WriteAsync(text);
                            });
                    });

            return builder;
        }

        #endregion
    }
}