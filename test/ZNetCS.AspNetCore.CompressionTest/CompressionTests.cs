﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompressionTests.cs" company="Marcin Smółka zNET Computer Solutions">
//   Copyright (c) Marcin Smółka zNET Computer Solutions. All rights reserved.
// </copyright>
// <summary>
//   The compression tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZNetCS.AspNetCore.CompressionTest
{
    #region Usings

    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Net.Http.Headers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using ZNetCS.AspNetCore.Compression;
    using ZNetCS.AspNetCore.Compression.Compressors;
    using ZNetCS.AspNetCore.Compression.DependencyInjection;

    #endregion

    /// <summary>
    /// The compression tests.
    /// </summary>
    [TestClass]
    public class CompressionTests : TestsBase
    {
        #region Public Methods

        /// <summary>
        /// The compression threshold test.
        /// </summary>
        [TestMethod]
        public async Task CompressionMediaTest()
        {
            // Arrange
            using (var server = new TestServer(
                this.CreateBuilder(options =>
                {
                    options.MinimumCompressionThreshold = 0;
                    options.AllowedMediaTypes = new List<MediaTypeHeaderValue> { MediaTypeHeaderValue.Parse("text/html") };
                })))
            {
                // Act
                RequestBuilder request = server.CreateRequest("/");
                request.AddHeader(HeaderNames.AcceptEncoding, "gzip");

                HttpResponseMessage response = await request.SendAsync("PUT");

                // Assert
                response.EnsureSuccessStatusCode();

                string responseText = await response.Content.ReadAsStringAsync();

                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "StatusCode != OK");
                Assert.AreEqual(Helpers.ResponseText, responseText, "Response Text not equal");
                Assert.AreEqual(124, response.Content.Headers.ContentLength, "Content-Length != 124");
                Assert.AreEqual(false, response.Content.Headers.ContentEncoding.Any(), "Content-Encoding != null");
                Assert.AreEqual(false, response.Headers.Vary.Contains(HeaderNames.AcceptEncoding), "Vary != Accept-Encoding");
            }
        }

        /// <summary>
        /// The compression with no content response.
        /// </summary>
        [TestMethod]
        public async Task CompressionNoContentResponseTest()
        {
            IWebHostBuilder builder = new WebHostBuilder()
                .ConfigureServices(s => s.AddCompression(options => { options.MinimumCompressionThreshold = 100; }))
                .Configure(
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

            using (var server = new TestServer(builder))
            {
                // Act
                RequestBuilder request = server.CreateRequest("/");
                request.AddHeader(HeaderNames.AcceptEncoding, "gzip");

                HttpResponseMessage response = await request.SendAsync("PUT");

                // Assert
                response.EnsureSuccessStatusCode();

                string responseText = await response.Content.ReadAsStringAsync();

                Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode, "StatusCode != NoContent");
                Assert.AreEqual(string.Empty, responseText, "Response Text not empty");
                Assert.AreEqual(0, response.Content.Headers.ContentLength, "Content-Length != 0");
            }
        }

        /// <summary>
        /// The compression threshold test.
        /// </summary>
        [TestMethod]
        public async Task CompressionThresholdTest()
        {
            // Arrange
            using (var server = new TestServer(this.CreateBuilder()))
            {
                // Act
                RequestBuilder request = server.CreateRequest("/");
                request.AddHeader(HeaderNames.AcceptEncoding, "gzip");

                HttpResponseMessage response = await request.SendAsync("PUT");

                // Assert
                response.EnsureSuccessStatusCode();

                string responseText = await response.Content.ReadAsStringAsync();

                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "StatusCode != OK");
                Assert.AreEqual(Helpers.ResponseText, responseText, "Response Text not equal");
                Assert.AreEqual(124, response.Content.Headers.ContentLength, "Content-Length != 124");
                Assert.AreEqual(false, response.Content.Headers.ContentEncoding.Any(), "Content-Encoding != null");
                Assert.AreEqual(false, response.Headers.Vary.Contains(HeaderNames.AcceptEncoding), "Vary != Accept-Encoding");
            }
        }

        /// <summary>
        /// The compression threshold zero deflate fastest test.
        /// </summary>
        [TestMethod]
        public async Task CompressionThresholdZeroDeflateFastestTest()
        {
            // Arrange
            using (
                var server =
                    new TestServer(
                        this.CreateBuilder(options =>
                        {
                            options.MinimumCompressionThreshold = 0;
                            options.Compressors = new List<ICompressor> { new DeflateCompressor(CompressionLevel.Fastest) };
                        })))
            {
                // Act
                RequestBuilder request = server.CreateRequest("/");
                request.AddHeader(HeaderNames.AcceptEncoding, "deflate");

                HttpResponseMessage response = await request.SendAsync("PUT");

                // Assert
                response.EnsureSuccessStatusCode();

                Stream stream = await response.Content.ReadAsStreamAsync();
                string responseText;

                using (var decompression = new DeflateStream(stream, CompressionMode.Decompress))
                {
                    using (var ms = new MemoryStream())
                    {
                        await decompression.CopyToAsync(ms);
                        responseText = Encoding.UTF8.GetString(ms.ToArray());
                    }
                }

                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "StatusCode != OK");
                Assert.AreEqual(Helpers.ResponseText, responseText, "Response Text not equal");
                Assert.AreEqual(67, response.Content.Headers.ContentLength, "Content-Length != 67");
                Assert.AreEqual(true, response.Content.Headers.ContentEncoding.Any(), "Content-Encoding == null");
                Assert.AreEqual("deflate", response.Content.Headers.ContentEncoding.ToString(), "Content-Encoding != deflate");
                Assert.AreEqual(true, response.Headers.Vary.Contains(HeaderNames.AcceptEncoding), "Vary != Accept-Encoding");
            }
        }

        /// <summary>
        /// The compression threshold zero deflate test.
        /// </summary>
        [TestMethod]
        public async Task CompressionThresholdZeroDeflateTest()
        {
            // Arrange
            using (var server = new TestServer(this.CreateBuilder(options => { options.MinimumCompressionThreshold = 0; })))
            {
                // Act
                RequestBuilder request = server.CreateRequest("/");
                request.AddHeader(HeaderNames.AcceptEncoding, "deflate");

                HttpResponseMessage response = await request.SendAsync("PUT");

                // Assert
                response.EnsureSuccessStatusCode();

                Stream stream = await response.Content.ReadAsStreamAsync();
                string responseText;

                using (var decompression = new DeflateStream(stream, CompressionMode.Decompress))
                {
                    using (var ms = new MemoryStream())
                    {
                        await decompression.CopyToAsync(ms);
                        responseText = Encoding.UTF8.GetString(ms.ToArray());
                    }
                }

                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "StatusCode != OK");
                Assert.AreEqual(Helpers.ResponseText, responseText, "Response Text not equal");
                Assert.AreEqual(67, response.Content.Headers.ContentLength, "Content-Length != 67");
                Assert.AreEqual(true, response.Content.Headers.ContentEncoding.Any(), "Content-Encoding == null");
                Assert.AreEqual("deflate", response.Content.Headers.ContentEncoding.ToString(), "Content-Encoding != deflate");
                Assert.AreEqual(true, response.Headers.Vary.Contains(HeaderNames.AcceptEncoding), "Vary != Accept-Encoding");
            }
        }

        /// <summary>
        /// The compression threshold zero GZIP Fastest test.
        /// </summary>
        [TestMethod]
        public async Task CompressionThresholdZeroGZipFastestTest()
        {
            // Arrange
            using (
                var server =
                    new TestServer(
                        this.CreateBuilder(options =>
                        {
                            options.MinimumCompressionThreshold = 0;
                            options.Compressors = new List<ICompressor> { new GZipCompressor(CompressionLevel.Fastest) };
                        })))
            {
                // Act
                RequestBuilder request = server.CreateRequest("/");
                request.AddHeader(HeaderNames.AcceptEncoding, "gzip");

                HttpResponseMessage response = await request.SendAsync("PUT");

                // Assert
                response.EnsureSuccessStatusCode();

                Stream stream = await response.Content.ReadAsStreamAsync();
                string responseText;

                using (var decompression = new GZipStream(stream, CompressionMode.Decompress))
                {
                    using (var ms = new MemoryStream())
                    {
                        await decompression.CopyToAsync(ms);
                        responseText = Encoding.UTF8.GetString(ms.ToArray());
                    }
                }

                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "StatusCode != OK");
                Assert.AreEqual(Helpers.ResponseText, responseText, "Response Text not equal");
                Assert.AreEqual(85, response.Content.Headers.ContentLength, "Content-Length != 85");
                Assert.AreEqual(true, response.Content.Headers.ContentEncoding.Any(), "Content-Encoding == null");
                Assert.AreEqual("gzip", response.Content.Headers.ContentEncoding.ToString(), "Content-Encoding != gzip");
                Assert.AreEqual(true, response.Headers.Vary.Contains(HeaderNames.AcceptEncoding), "Vary != Accept-Encoding");
            }
        }

        /// <summary>
        /// The compression threshold zero GZIP no compression test.
        /// </summary>
        [TestMethod]
        public async Task CompressionThresholdZeroGZipNoCompressionTest()
        {
            // Arrange
            using (
                var server =
                    new TestServer(
                        this.CreateBuilder(options =>
                        {
                            options.MinimumCompressionThreshold = 0;
                            options.Compressors = new List<ICompressor> { new GZipCompressor(CompressionLevel.NoCompression) };
                        })))
            {
                // Act
                RequestBuilder request = server.CreateRequest("/");
                request.AddHeader(HeaderNames.AcceptEncoding, "gzip");

                HttpResponseMessage response = await request.SendAsync("PUT");

                // Assert
                response.EnsureSuccessStatusCode();

                Stream stream = await response.Content.ReadAsStreamAsync();
                string responseText;

                using (var decompression = new GZipStream(stream, CompressionMode.Decompress))
                {
                    using (var ms = new MemoryStream())
                    {
                        await decompression.CopyToAsync(ms);
                        responseText = Encoding.UTF8.GetString(ms.ToArray());
                    }
                }

                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "StatusCode != OK");
                Assert.AreEqual(Helpers.ResponseText, responseText, "Response Text not equal");
                Assert.AreEqual(147, response.Content.Headers.ContentLength, "Content-Length != 147");
                Assert.AreEqual(true, response.Content.Headers.ContentEncoding.Any(), "Content-Encoding == null");
                Assert.AreEqual("gzip", response.Content.Headers.ContentEncoding.ToString(), "Content-Encoding != gzip");
                Assert.AreEqual(true, response.Headers.Vary.Contains(HeaderNames.AcceptEncoding), "Vary != Accept-Encoding");
            }
        }

        /// <summary>
        /// The compression threshold zero GZIP test.
        /// </summary>
        [TestMethod]
        public async Task CompressionThresholdZeroGZipTest()
        {
            // Arrange
            using (var server = new TestServer(this.CreateBuilder(options => { options.MinimumCompressionThreshold = 0; })))
            {
                // Act
                RequestBuilder request = server.CreateRequest("/");
                request.AddHeader(HeaderNames.AcceptEncoding, "gzip");

                HttpResponseMessage response = await request.SendAsync("PUT");

                // Assert
                response.EnsureSuccessStatusCode();

                Stream stream = await response.Content.ReadAsStreamAsync();
                string responseText;

                using (var decompression = new GZipStream(stream, CompressionMode.Decompress))
                {
                    using (var ms = new MemoryStream())
                    {
                        await decompression.CopyToAsync(ms);
                        responseText = Encoding.UTF8.GetString(ms.ToArray());
                    }
                }

                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "StatusCode != OK");
                Assert.AreEqual(Helpers.ResponseText, responseText, "Response Text not equal");
                Assert.AreEqual(85, response.Content.Headers.ContentLength, "Content-Length != 85");
                Assert.AreEqual(true, response.Content.Headers.ContentEncoding.Any(), "Content-Encoding == null");
                Assert.AreEqual("gzip", response.Content.Headers.ContentEncoding.ToString(), "Content-Encoding != gzip");
                Assert.AreEqual(true, response.Headers.Vary.Contains(HeaderNames.AcceptEncoding), "Vary != Accept-Encoding");
            }
        }

        #endregion
    }
}