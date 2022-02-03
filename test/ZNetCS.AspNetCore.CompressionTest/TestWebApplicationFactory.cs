// -----------------------------------------------------------------------
// <copyright file="TestWebApplicationFactory.cs" company="Marcin Smółka">
// Copyright (c) Marcin Smółka. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ZNetCS.AspNetCore.CompressionTest;

#region Usings

using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using ZNetCS.AspNetCore.Compression;

#endregion

/// <inheritdoc/>
internal class TestWebApplicationFactory : WebApplicationFactory<Startup>
{
    #region Fields

    private readonly TestType testType;
    private readonly Action<CompressionOptions> configure;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="TestWebApplicationFactory"/> class.
    /// </summary>
    /// <param name="configure">The option configure.</param>
    /// <param name="testType">True if testing compression.</param>
    public TestWebApplicationFactory(Action<CompressionOptions>? configure = null, TestType testType = TestType.Compression)
    {
        configure ??= _ => { };

        this.configure = configure;
        this.testType = testType;
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseStartup<Startup>();
        builder.UseContentRoot(GetPath() ?? string.Empty);

        switch (this.testType)
        {
            case TestType.Compression:

                builder.ConfigureServices(s => s.AddCompression(this.configure));
                builder.Configure(
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
                break;
            case TestType.Decompression:

                builder.ConfigureServices(s => s.AddCompression(this.configure));
                builder.Configure(
                    app =>
                    {
                        app.UseCompression();
                        app.Run(
                            async c =>
                            {
                                string text;

                                using (var reader = new StreamReader(c.Request.Body))
                                {
                                    text = await reader.ReadToEndAsync();
                                }

                                c.Response.ContentType = "text/plain";
                                c.Response.ContentLength = text.Length;
                                await c.Response.WriteAsync(text);
                            });
                    });
                break;
            case TestType.NoContent:

                builder.ConfigureServices(s => s.AddCompression(this.configure));
                builder.Configure(
                    app =>
                    {
                        app.UseCompression();
                        app.Run(
                            c =>
                            {
                                c.Response.ContentType = "text/plain";
                                c.Response.StatusCode = (int)HttpStatusCode.NoContent;

                                return Task.CompletedTask;
                            });
                    });
                break;

            case TestType.NoDecompression:

                builder.ConfigureServices(s => s.AddCompression());
                builder.Configure(
                    app =>
                    {
                        app.UseCompression();
                        app.Run(
                            async c =>
                            {
                                await using var ms = new MemoryStream();
                                await c.Request.Body.CopyToAsync(ms);

                                c.Response.ContentType = "text/plain";
                                c.Response.ContentLength = ms.Length;

                                ms.Seek(0, SeekOrigin.Begin);

                                await ms.CopyToAsync(c.Response.Body);
                            });
                    });
                break;
        }
    }

    /// <inheritdoc/>
    protected override IHostBuilder CreateHostBuilder() => Host.CreateDefaultBuilder().ConfigureWebHostDefaults(_ => { });

    /// <summary>
    /// Get root path for test web server.
    /// </summary>
    private static string? GetPath()
    {
        string path = Path.GetDirectoryName(typeof(Startup).GetTypeInfo().Assembly.Location)!;

        // ReSharper disable PossibleNullReferenceException
        DirectoryInfo? di = new DirectoryInfo(path).Parent?.Parent?.Parent;

        return di?.FullName;
    }

    #endregion
}