// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompressionTest.cs" company="Marcin Smółka">
//   Copyright (c) Marcin Smółka. All rights reserved.
// </copyright>
// <summary>
//   The compression tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZNetCS.AspNetCore.CompressionTest;

#region Usings

using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.TestHost;
using Microsoft.Net.Http.Headers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ZNetCS.AspNetCore.Compression;
using ZNetCS.AspNetCore.Compression.Compressors;

#endregion

/// <summary>
/// The compression tests.
/// </summary>
[TestClass]
public class CompressionTest
{
    #region Public Methods

    /// <summary>
    /// The compression threshold test.
    /// </summary>
    [TestMethod]
    public async Task CompressionMediaTest()
    {
        // Arrange
        using var factory = new TestWebApplicationFactory(
            options =>
            {
                options.MinimumCompressionThreshold = 0;
                options.AllowedMediaTypes = new List<MediaTypeHeaderValue> { MediaTypeHeaderValue.Parse("text/html") };
            });

        // Act
        RequestBuilder request = factory.Server.CreateRequest("/");
        request.AddHeader(HeaderNames.AcceptEncoding, "gzip");

        HttpResponseMessage response = await request.SendAsync("PUT");

        // Assert
        response.EnsureSuccessStatusCode();

        string responseText = await response.Content.ReadAsStringAsync();

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "StatusCode != OK");
        Assert.AreEqual(Helpers.ResponseText, responseText, "Response Text not equal");
        Assert.AreEqual(124, response.Content.Headers.ContentLength, "Content-Length != 124");
        Assert.IsFalse(response.Content.Headers.ContentEncoding.Any(), "Content-Encoding != null");
        Assert.DoesNotContain(HeaderNames.AcceptEncoding, response.Headers.Vary, "Vary != Accept-Encoding");
    }

    /// <summary>
    /// The compression with no content response.
    /// </summary>
    [TestMethod]
    public async Task CompressionNoContentResponseTest()
    {
        using var factory = new TestWebApplicationFactory(options => { options.MinimumCompressionThreshold = 100; }, TestType.NoContent);

        // Act
        RequestBuilder request = factory.Server.CreateRequest("/");
        request.AddHeader(HeaderNames.AcceptEncoding, "gzip");

        HttpResponseMessage response = await request.SendAsync("PUT");

        // Assert
        response.EnsureSuccessStatusCode();

        string responseText = await response.Content.ReadAsStringAsync();

        Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode, "StatusCode != NoContent");
        Assert.AreEqual(string.Empty, responseText, "Response Text not empty");
        Assert.AreEqual(0, response.Content.Headers.ContentLength, "Content-Length != 0");
    }

    /// <summary>
    /// The compression threshold test.
    /// </summary>
    [TestMethod]
    public async Task CompressionThresholdTest()
    {
        // Arrange
        using var factory = new TestWebApplicationFactory();

        // Act
        RequestBuilder request = factory.Server.CreateRequest("/");
        request.AddHeader(HeaderNames.AcceptEncoding, "gzip");

        HttpResponseMessage response = await request.SendAsync("PUT");

        // Assert
        response.EnsureSuccessStatusCode();

        string responseText = await response.Content.ReadAsStringAsync();

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "StatusCode != OK");
        Assert.AreEqual(Helpers.ResponseText, responseText, "Response Text not equal");
        Assert.AreEqual(124, response.Content.Headers.ContentLength, "Content-Length != 124");
        Assert.IsFalse(response.Content.Headers.ContentEncoding.Any(), "Content-Encoding != null");
        Assert.DoesNotContain(HeaderNames.AcceptEncoding, response.Headers.Vary, "Vary != Accept-Encoding");
    }

    /// <summary>
    /// The compression threshold zero deflate fastest test.
    /// </summary>
    [TestMethod]
    public async Task CompressionThresholdZeroDeflateFastestTest()
    {
        // Arrange
        using var factory = new TestWebApplicationFactory(
            options =>
            {
                options.MinimumCompressionThreshold = 0;
                options.Compressors = new List<ICompressor> { new DeflateCompressor(CompressionLevel.Fastest) };
            });

        // Act
        RequestBuilder request = factory.Server.CreateRequest("/");
        request.AddHeader(HeaderNames.AcceptEncoding, "deflate");

        HttpResponseMessage response = await request.SendAsync("PUT");

        // Assert
        response.EnsureSuccessStatusCode();

        Stream stream = await response.Content.ReadAsStreamAsync();
        string responseText;

        await using (var decompression = new DeflateStream(stream, CompressionMode.Decompress))
        {
            await using var ms = new MemoryStream();
            await decompression.CopyToAsync(ms);
            responseText = Encoding.UTF8.GetString(ms.ToArray());
        }

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "StatusCode != OK");
        Assert.AreEqual(Helpers.ResponseText, responseText, "Response Text not equal");
        Assert.IsTrue(response.Content.Headers.ContentEncoding.Any(), "Content-Encoding == null");
        Assert.AreEqual("deflate", response.Content.Headers.ContentEncoding.ToString(), "Content-Encoding != deflate");
        Assert.Contains(HeaderNames.AcceptEncoding, response.Headers.Vary, "Vary != Accept-Encoding");
    }

    /// <summary>
    /// The compression threshold zero deflate test.
    /// </summary>
    [TestMethod]
    public async Task CompressionThresholdZeroDeflateTest()
    {
        // Arrange
        using var factory = new TestWebApplicationFactory(options => { options.MinimumCompressionThreshold = 0; });

        // Act
        RequestBuilder request = factory.Server.CreateRequest("/");
        request.AddHeader(HeaderNames.AcceptEncoding, "deflate");

        HttpResponseMessage response = await request.SendAsync("PUT");

        // Assert
        response.EnsureSuccessStatusCode();

        Stream stream = await response.Content.ReadAsStreamAsync();
        string responseText;

        await using (var decompression = new DeflateStream(stream, CompressionMode.Decompress))
        {
            await using var ms = new MemoryStream();
            await decompression.CopyToAsync(ms);
            responseText = Encoding.UTF8.GetString(ms.ToArray());
        }

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "StatusCode != OK");
        Assert.AreEqual(Helpers.ResponseText, responseText, "Response Text not equal");
        Assert.AreEqual(67, response.Content.Headers.ContentLength, "Content-Length != 67");
        Assert.IsTrue(response.Content.Headers.ContentEncoding.Any(), "Content-Encoding == null");
        Assert.AreEqual("deflate", response.Content.Headers.ContentEncoding.ToString(), "Content-Encoding != deflate");
        Assert.Contains(HeaderNames.AcceptEncoding, response.Headers.Vary, "Vary != Accept-Encoding");
    }

    /// <summary>
    /// The compression threshold zero GZIP Fastest test.
    /// </summary>
    [TestMethod]
    public async Task CompressionThresholdZeroGZipFastestTest()
    {
        // Arrange
        using var factory = new TestWebApplicationFactory(
            options =>
            {
                options.MinimumCompressionThreshold = 0;
                options.Compressors = new List<ICompressor> { new GZipCompressor(CompressionLevel.Fastest) };
            });

        // Act
        RequestBuilder request = factory.Server.CreateRequest("/");
        request.AddHeader(HeaderNames.AcceptEncoding, "gzip");

        HttpResponseMessage response = await request.SendAsync("PUT");

        // Assert
        response.EnsureSuccessStatusCode();

        Stream stream = await response.Content.ReadAsStreamAsync();
        string responseText;

        await using (var decompression = new GZipStream(stream, CompressionMode.Decompress))
        {
            await using var ms = new MemoryStream();
            await decompression.CopyToAsync(ms);
            responseText = Encoding.UTF8.GetString(ms.ToArray());
        }

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "StatusCode != OK");
        Assert.AreEqual(Helpers.ResponseText, responseText, "Response Text not equal");
        Assert.IsTrue(response.Content.Headers.ContentEncoding.Any(), "Content-Encoding == null");
        Assert.AreEqual("gzip", response.Content.Headers.ContentEncoding.ToString(), "Content-Encoding != gzip");
        Assert.Contains(HeaderNames.AcceptEncoding, response.Headers.Vary, "Vary != Accept-Encoding");
    }

    /// <summary>
    /// The compression threshold zero GZIP no compression test.
    /// </summary>
    [TestMethod]
    public async Task CompressionThresholdZeroGZipNoCompressionTest()
    {
        // Arrange
        using var factory = new TestWebApplicationFactory(
            options =>
            {
                options.MinimumCompressionThreshold = 0;
                options.Compressors = new List<ICompressor> { new GZipCompressor(CompressionLevel.NoCompression) };
            });

        // Act
        RequestBuilder request = factory.Server.CreateRequest("/");
        request.AddHeader(HeaderNames.AcceptEncoding, "gzip");

        HttpResponseMessage response = await request.SendAsync("PUT");

        // Assert
        response.EnsureSuccessStatusCode();

        Stream stream = await response.Content.ReadAsStreamAsync();
        string responseText;

        await using (var decompression = new GZipStream(stream, CompressionMode.Decompress))
        {
            await using var ms = new MemoryStream();
            await decompression.CopyToAsync(ms);
            responseText = Encoding.UTF8.GetString(ms.ToArray());
        }

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "StatusCode != OK");
        Assert.AreEqual(Helpers.ResponseText, responseText, "Response Text not equal");
        Assert.AreEqual(147, response.Content.Headers.ContentLength, "Content-Length != 147");
        Assert.IsTrue(response.Content.Headers.ContentEncoding.Any(), "Content-Encoding == null");
        Assert.AreEqual("gzip", response.Content.Headers.ContentEncoding.ToString(), "Content-Encoding != gzip");
        Assert.Contains(HeaderNames.AcceptEncoding, response.Headers.Vary, "Vary != Accept-Encoding");
    }

    /// <summary>
    /// The compression threshold zero GZIP test.
    /// </summary>
    [TestMethod]
    public async Task CompressionThresholdZeroGZipTest()
    {
        // Arrange
        using var factory = new TestWebApplicationFactory(options => { options.MinimumCompressionThreshold = 0; });

        // Act
        RequestBuilder request = factory.Server.CreateRequest("/");
        request.AddHeader(HeaderNames.AcceptEncoding, "gzip");

        HttpResponseMessage response = await request.SendAsync("PUT");

        // Assert
        response.EnsureSuccessStatusCode();

        Stream stream = await response.Content.ReadAsStreamAsync();
        string responseText;

        await using (var decompression = new GZipStream(stream, CompressionMode.Decompress))
        {
            await using var ms = new MemoryStream();
            await decompression.CopyToAsync(ms);
            responseText = Encoding.UTF8.GetString(ms.ToArray());
        }

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "StatusCode != OK");
        Assert.AreEqual(Helpers.ResponseText, responseText, "Response Text not equal");
        Assert.AreEqual(85, response.Content.Headers.ContentLength, "Content-Length != 85");
        Assert.IsTrue(response.Content.Headers.ContentEncoding.Any(), "Content-Encoding == null");
        Assert.AreEqual("gzip", response.Content.Headers.ContentEncoding.ToString(), "Content-Encoding != gzip");
        Assert.Contains(HeaderNames.AcceptEncoding, response.Headers.Vary, "Vary != Accept-Encoding");
    }

    /// <summary>
    /// The compression threshold zero Brotli Fastest test.
    /// </summary>
    [TestMethod]
    public async Task CompressionThresholdZeroBrotliFastestTest()
    {
        // Arrange
        using var factory = new TestWebApplicationFactory(
            options =>
            {
                options.MinimumCompressionThreshold = 0;
                options.Compressors = new List<ICompressor> { new BrotliCompressor(CompressionLevel.Fastest) };
            });

        // Act
        RequestBuilder request = factory.Server.CreateRequest("/");
        request.AddHeader(HeaderNames.AcceptEncoding, "br");

        HttpResponseMessage response = await request.SendAsync("PUT");

        // Assert
        response.EnsureSuccessStatusCode();

        Stream stream = await response.Content.ReadAsStreamAsync();
        string responseText;

        await using (var decompression = new BrotliStream(stream, CompressionMode.Decompress))
        {
            await using var ms = new MemoryStream();
            await decompression.CopyToAsync(ms);
            responseText = Encoding.UTF8.GetString(ms.ToArray());
        }

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "StatusCode != OK");
        Assert.AreEqual(Helpers.ResponseText, responseText, "Response Text not equal");
        Assert.AreEqual(88, response.Content.Headers.ContentLength, "Content-Length != 88");
        Assert.IsTrue(response.Content.Headers.ContentEncoding.Any(), "Content-Encoding == null");
        Assert.AreEqual("br", response.Content.Headers.ContentEncoding.ToString(), "Content-Encoding != br");
        Assert.Contains(HeaderNames.AcceptEncoding, response.Headers.Vary, "Vary != Accept-Encoding");
    }

    /// <summary>
    /// The compression threshold zero Brotli no compression test.
    /// </summary>
    [TestMethod]
    public async Task CompressionThresholdZeroBrotliNoCompressionTest()
    {
        // Arrange
        using var factory = new TestWebApplicationFactory(
            options =>
            {
                options.MinimumCompressionThreshold = 0;
                options.Compressors = new List<ICompressor> { new BrotliCompressor(CompressionLevel.NoCompression) };
            });

        // Act
        RequestBuilder request = factory.Server.CreateRequest("/");
        request.AddHeader(HeaderNames.AcceptEncoding, "br");

        HttpResponseMessage response = await request.SendAsync("PUT");

        // Assert
        response.EnsureSuccessStatusCode();

        Stream stream = await response.Content.ReadAsStreamAsync();
        string responseText;

        await using (var decompression = new BrotliStream(stream, CompressionMode.Decompress))
        {
            await using var ms = new MemoryStream();
            await decompression.CopyToAsync(ms);
            responseText = Encoding.UTF8.GetString(ms.ToArray());
        }

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "StatusCode != OK");
        Assert.AreEqual(Helpers.ResponseText, responseText, "Response Text not equal");
        Assert.AreEqual(128, response.Content.Headers.ContentLength, "Content-Length != 128");
        Assert.IsTrue(response.Content.Headers.ContentEncoding.Any(), "Content-Encoding == null");
        Assert.AreEqual("br", response.Content.Headers.ContentEncoding.ToString(), "Content-Encoding != br");
        Assert.Contains(HeaderNames.AcceptEncoding, response.Headers.Vary, "Vary != Accept-Encoding");
    }

    /// <summary>
    /// The compression threshold zero Brotli test.
    /// </summary>
    [TestMethod]
    public async Task CompressionThresholdZeroBrotliTest()
    {
        // Arrange
        using var factory = new TestWebApplicationFactory(options => { options.MinimumCompressionThreshold = 0; });

        // Act
        RequestBuilder request = factory.Server.CreateRequest("/");
        request.AddHeader(HeaderNames.AcceptEncoding, "br");

        HttpResponseMessage response = await request.SendAsync("PUT");

        // Assert
        response.EnsureSuccessStatusCode();

        Stream stream = await response.Content.ReadAsStreamAsync();
        string responseText;

        await using (var decompression = new BrotliStream(stream, CompressionMode.Decompress))
        {
            await using var ms = new MemoryStream();
            await decompression.CopyToAsync(ms);
            responseText = Encoding.UTF8.GetString(ms.ToArray());
        }

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "StatusCode != OK");
        Assert.AreEqual(Helpers.ResponseText, responseText, "Response Text not equal");
        Assert.IsTrue(response.Content.Headers.ContentEncoding.Any(), "Content-Encoding == null");
        Assert.AreEqual("br", response.Content.Headers.ContentEncoding.ToString(), "Content-Encoding != br");
        Assert.Contains(HeaderNames.AcceptEncoding, response.Headers.Vary, "Vary != Accept-Encoding");
    }

    #endregion
}