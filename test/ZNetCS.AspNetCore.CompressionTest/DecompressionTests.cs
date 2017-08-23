// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DecompressionTests.cs" company="Marcin Smółka zNET Computer Solutions">
//   Copyright (c) Marcin Smółka zNET Computer Solutions. All rights reserved.
// </copyright>
// <summary>
//   The decompression tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZNetCS.AspNetCore.CompressionTest
{
    #region Usings

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
    using ZNetCS.AspNetCore.Compression.DependencyInjection;

    #endregion

    /// <summary>
    /// The decompression tests.
    /// </summary>
    [TestClass]
    public class DecompressionTests : TestsBase
    {
        #region Public Methods

        /// <summary>
        /// The decompression deflate test.
        /// </summary>
        [TestMethod]
        public async Task DecompressionDeflateTest()
        {
            // Arrange
            using (var server = new TestServer(this.CreateDecompressionBuilder(new CompressionOptions { MinimumCompressionThreshold = 0 })))
            {
                // Act
                byte[] compressedBytes;
                using (var dataStream = new MemoryStream())
                {
                    using (var zipStream = new DeflateStream(dataStream, CompressionMode.Compress))
                    {
                        using (var writer = new StreamWriter(zipStream))
                        {
                            writer.Write(Helpers.ResponseText);
                        }
                    }

                    compressedBytes = dataStream.ToArray();
                }

                HttpResponseMessage response;
                string responseText;

                using (HttpClient client = server.CreateClient())
                {
                    client.DefaultRequestHeaders.Add(HeaderNames.AcceptEncoding, "gzip");

                    var content = new ByteArrayContent(compressedBytes);
                    content.Headers.Add(HeaderNames.ContentEncoding, "deflate");
                    content.Headers.Add(HeaderNames.ContentLength, HeaderUtilities.FormatNonNegativeInt64(compressedBytes.Length));

                    response = await client.PutAsync("/", content);

                    // Assert
                    response.EnsureSuccessStatusCode();

                    Stream stream = await response.Content.ReadAsStreamAsync();

                    using (var decompression = new GZipStream(stream, CompressionMode.Decompress))
                    {
                        using (var ms = new MemoryStream())
                        {
                            await decompression.CopyToAsync(ms);
                            responseText = Encoding.UTF8.GetString(ms.ToArray());
                        }
                    }
                }

                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "StatusCode != OK");
                Assert.AreEqual(Helpers.ResponseText, responseText, "Response Text not equal");
                Assert.AreEqual(85, response.Content.Headers.ContentLength, "Content-Length != 85");
                Assert.AreEqual(true, response.Content.Headers.ContentEncoding.Any(), "Content-Encoding == null");
                Assert.AreEqual("gzip", response.Content.Headers.ContentEncoding.ToString(), "Content-Encoding != gzip");
            }
        }

        /// <summary>
        /// The decompression GZIP no compression test.
        /// </summary>
        [TestMethod]
        public async Task DecompressionGZipNoCompressionTest()
        {
            // Arrange
            using (var server = new TestServer(this.CreateDecompressionBuilder()))
            {
                // Act
                byte[] compressedBytes;
                using (var dataStream = new MemoryStream())
                {
                    using (var zipStream = new GZipStream(dataStream, CompressionMode.Compress))
                    {
                        using (var writer = new StreamWriter(zipStream))
                        {
                            writer.Write(Helpers.ResponseText);
                        }
                    }

                    compressedBytes = dataStream.ToArray();
                }

                HttpResponseMessage response;
                string responseText;

                using (HttpClient client = server.CreateClient())
                {
                    client.DefaultRequestHeaders.Add(HeaderNames.AcceptEncoding, "gzip");

                    var content = new ByteArrayContent(compressedBytes);
                    content.Headers.Add(HeaderNames.ContentEncoding, "gzip");
                    content.Headers.Add(HeaderNames.ContentLength, HeaderUtilities.FormatNonNegativeInt64(compressedBytes.Length));

                    response = await client.PutAsync("/", content);

                    // Assert
                    response.EnsureSuccessStatusCode();
                    responseText = await response.Content.ReadAsStringAsync();
                }

                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "StatusCode != OK");
                Assert.AreEqual(Helpers.ResponseText, responseText, "Response Text not equal");
                Assert.AreEqual(124, response.Content.Headers.ContentLength, "Content-Length != 124");
                Assert.AreEqual(false, response.Content.Headers.ContentEncoding.Any(), "Content-Encoding != null");
            }
        }

        /// <summary>
        /// The decompression GZIP test.
        /// </summary>
        [TestMethod]
        public async Task DecompressionGZipTest()
        {
            // Arrange
            using (var server = new TestServer(this.CreateDecompressionBuilder(new CompressionOptions { MinimumCompressionThreshold = 0 })))
            {
                // Act
                byte[] compressedBytes;
                using (var dataStream = new MemoryStream())
                {
                    using (var zipStream = new GZipStream(dataStream, CompressionMode.Compress))
                    {
                        using (var writer = new StreamWriter(zipStream))
                        {
                            writer.Write(Helpers.ResponseText);
                        }
                    }

                    compressedBytes = dataStream.ToArray();
                }

                HttpResponseMessage response;
                string responseText;

                using (HttpClient client = server.CreateClient())
                {
                    client.DefaultRequestHeaders.Add(HeaderNames.AcceptEncoding, "gzip");

                    var content = new ByteArrayContent(compressedBytes);
                    content.Headers.Add(HeaderNames.ContentEncoding, "gzip");
                    content.Headers.Add(HeaderNames.ContentLength, HeaderUtilities.FormatNonNegativeInt64(compressedBytes.Length));

                    response = await client.PutAsync("/", content);

                    // Assert
                    response.EnsureSuccessStatusCode();

                    Stream stream = await response.Content.ReadAsStreamAsync();

                    using (var decompression = new GZipStream(stream, CompressionMode.Decompress))
                    {
                        using (var ms = new MemoryStream())
                        {
                            await decompression.CopyToAsync(ms);
                            responseText = Encoding.UTF8.GetString(ms.ToArray());
                        }
                    }
                }

                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "StatusCode != OK");
                Assert.AreEqual(Helpers.ResponseText, responseText, "Response Text not equal");
                Assert.AreEqual(85, response.Content.Headers.ContentLength, "Content-Length != 85");
                Assert.AreEqual(true, response.Content.Headers.ContentEncoding.Any(), "Content-Encoding == null");
                Assert.AreEqual("gzip", response.Content.Headers.ContentEncoding.ToString(), "Content-Encoding != gzip");
            }
        }

        /// <summary>
        /// No decompression GZIP test.
        /// </summary>
        [TestMethod]
        public async Task NoDecompressionGZipTest()
        {
            // Arrange
            IWebHostBuilder builder = new WebHostBuilder()
                .ConfigureServices(s => s.AddCompression())
                .Configure(
                    app =>
                    {
                        app.UseCompression();
                        app.Run(
                            async c =>
                            {
                                using (var ms = new MemoryStream())
                                {
                                    await c.Request.Body.CopyToAsync(ms);

                                    c.Response.ContentType = "text/plain";
                                    c.Response.ContentLength = ms.Length;

                                    ms.Seek(0, SeekOrigin.Begin);

                                    await ms.CopyToAsync(c.Response.Body);
                                }
                            });
                    });

            using (var server = new TestServer(builder))
            {
                // Act
                byte[] compressedBytes;
                using (var dataStream = new MemoryStream())
                {
                    using (var zipStream = new GZipStream(dataStream, CompressionMode.Compress))
                    {
                        using (var writer = new StreamWriter(zipStream))
                        {
                            writer.Write(Helpers.ResponseText);
                        }
                    }

                    compressedBytes = dataStream.ToArray();
                }

                HttpResponseMessage response;
                byte[] responseBytes;

                using (HttpClient client = server.CreateClient())
                {
                    client.DefaultRequestHeaders.Add(HeaderNames.AcceptEncoding, "gzip");

                    var content = new ByteArrayContent(compressedBytes);
                    content.Headers.Add(HeaderNames.ContentLength, HeaderUtilities.FormatNonNegativeInt64(compressedBytes.Length));

                    response = await client.PutAsync("/", content);

                    // Assert
                    response.EnsureSuccessStatusCode();
                    responseBytes = await response.Content.ReadAsByteArrayAsync();
                }

                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "StatusCode != OK");
                Assert.AreEqual(true, compressedBytes.SequenceEqual(responseBytes), "Response bytes not equal");
                Assert.AreEqual(compressedBytes.Length, response.Content.Headers.ContentLength, $"Content-Length != {compressedBytes.Length}");
                Assert.AreEqual(true, !response.Content.Headers.ContentEncoding.Any(), "Content-Encoding != null");
            }
        }

        #endregion
    }
}