// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Startup.cs" company="Marcin Smółka zNET Computer Solutions">
//   Copyright (c) Marcin Smółka zNET Computer Solutions. All rights reserved.
// </copyright>
// <summary>
//   The startup.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZNetCS.AspNetCore.Compression.TestWebSite
{
    #region Usings

    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Net.Http.Headers;

    using ZNetCS.AspNetCore.Compression.Compressors;
    using ZNetCS.AspNetCore.Compression.DependencyInjection;

    #endregion

    /// <summary>
    /// The startup.
    /// </summary>
    public class Startup
    {
        #region Public Methods

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">
        /// The application builder.
        /// </param>
        /// <param name="env">
        /// The environment.
        /// </param>
        /// <param name="loggerFactory">
        /// The logger factory.
        /// </param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();
            loggerFactory.AddDebug();

            app.UseCompression(
                new CompressionOptions
                {
                    AllowedMediaTypes = new List<MediaTypeHeaderValue>
                    {
                        MediaTypeHeaderValue.Parse("*/*")
                    },
                    IgnoredPaths = new List<string>
                    {
                        "/css/",
                        "/images/",
                        "/js/",
                        "/lib/"
                    },
                    MinimumCompressionThreshold = 860,
                    Compressors = new List<ICompressor> { new GZipCompressor(), new DeflateCompressor() },
                    Decompressors = new List<IDecompressor> { new GZipDecompressor(), new DeflateDecompressor() }
                });

            app.Run(
                async context =>
                {
                    await context.Response.WriteAsync("0123456789abcdefghijklmnopgrstuvwxyzABCDEFGHIJKLMNOPGRSTUVWXYZ0123456789abcdefghijklmnopgrstuvwxyzABCDEFGHIJKLMNOPGRSTUVWXYZ");

                    context.Response.ContentType = "text/plain";
                    context.Response.ContentLength = 124;
                });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940.
        /// </summary>
        /// <param name="services">
        /// The service collection.
        /// </param>        
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "OK")]
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCompression();
        }

        #endregion
    }
}