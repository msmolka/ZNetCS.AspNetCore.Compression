// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DecompressionTest.cs" company="Marcin Smółka">
//   Copyright (c) Marcin Smółka. All rights reserved.
// </copyright>
// <summary>
//   The decompression tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZNetCS.AspNetCore.CompressionTest;

#region Usings

using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Net.Http.Headers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

/// <summary>
/// The decompression tests.
/// </summary>
[TestClass]
public class DecompressionTest
{
    #region Public Methods

    /// <summary>
    /// The decompression deflate test.
    /// </summary>
    [TestMethod]
    public async Task DecompressionDeflateTest()
    {
        // Arrange
        using var factory = new TestWebApplicationFactory(options => { options.MinimumCompressionThreshold = 0; }, TestType.Decompression);

        // Act
        byte[] compressedBytes;
        await using (var dataStream = new MemoryStream())
        {
            await using (var zipStream = new DeflateStream(dataStream, CompressionMode.Compress))
            {
                await using var writer = new StreamWriter(zipStream);
                await writer.WriteAsync(Helpers.ResponseText);
            }

            compressedBytes = dataStream.ToArray();
        }

        HttpResponseMessage response;
        string responseText;

        using (HttpClient client = factory.CreateClient())
        {
            client.DefaultRequestHeaders.Add(HeaderNames.AcceptEncoding, "gzip");

            var content = new ByteArrayContent(compressedBytes);
            content.Headers.Add(HeaderNames.ContentEncoding, "deflate");
            content.Headers.Add(HeaderNames.ContentLength, HeaderUtilities.FormatNonNegativeInt64(compressedBytes.Length));

            response = await client.PutAsync("/", content);

            // Assert
            response.EnsureSuccessStatusCode();

            Stream stream = await response.Content.ReadAsStreamAsync();

            await using var decompression = new GZipStream(stream, CompressionMode.Decompress);
            await using var ms = new MemoryStream();
            await decompression.CopyToAsync(ms);
            responseText = Encoding.UTF8.GetString(ms.ToArray());
        }

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "StatusCode != OK");
        Assert.AreEqual(Helpers.ResponseText, responseText, "Response Text not equal");
        Assert.AreEqual(85, response.Content.Headers.ContentLength, "Content-Length != 85");
        Assert.IsTrue(response.Content.Headers.ContentEncoding.Any(), "Content-Encoding == null");
        Assert.AreEqual("gzip", response.Content.Headers.ContentEncoding.ToString(), "Content-Encoding != gzip");
    }

    /// <summary>
    /// The decompression GZIP no compression test.
    /// </summary>
    [TestMethod]
    public async Task DecompressionGZipNoCompressionTest()
    {
        // Arrange
        using var factory = new TestWebApplicationFactory(testType: TestType.Decompression);

        // Act
        byte[] compressedBytes;
        await using (var dataStream = new MemoryStream())
        {
            await using (var zipStream = new GZipStream(dataStream, CompressionMode.Compress))
            {
                await using var writer = new StreamWriter(zipStream);
                await writer.WriteAsync(Helpers.ResponseText);
            }

            compressedBytes = dataStream.ToArray();
        }

        HttpResponseMessage response;
        string responseText;

        using (HttpClient client = factory.CreateClient())
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
        Assert.IsFalse(response.Content.Headers.ContentEncoding.Any(), "Content-Encoding != null");
    }

    /// <summary>
    /// The decompression GZIP test.
    /// </summary>
    [TestMethod]
    public async Task DecompressionGZipTest()
    {
        // Arrange
        using var factory = new TestWebApplicationFactory(options => { options.MinimumCompressionThreshold = 0; }, TestType.Decompression);

        // Act
        byte[] compressedBytes;
        await using (var dataStream = new MemoryStream())
        {
            await using (var zipStream = new GZipStream(dataStream, CompressionMode.Compress))
            {
                await using var writer = new StreamWriter(zipStream);
                await writer.WriteAsync(Helpers.ResponseText);
            }

            compressedBytes = dataStream.ToArray();
        }

        HttpResponseMessage response;
        string responseText;

        using (HttpClient client = factory.CreateClient())
        {
            client.DefaultRequestHeaders.Add(HeaderNames.AcceptEncoding, "gzip");

            var content = new ByteArrayContent(compressedBytes);
            content.Headers.Add(HeaderNames.ContentEncoding, "gzip");
            content.Headers.Add(HeaderNames.ContentLength, HeaderUtilities.FormatNonNegativeInt64(compressedBytes.Length));

            response = await client.PutAsync("/", content);

            // Assert
            response.EnsureSuccessStatusCode();

            Stream stream = await response.Content.ReadAsStreamAsync();

            await using var decompression = new GZipStream(stream, CompressionMode.Decompress);
            await using var ms = new MemoryStream();
            await decompression.CopyToAsync(ms);
            responseText = Encoding.UTF8.GetString(ms.ToArray());
        }

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "StatusCode != OK");
        Assert.AreEqual(Helpers.ResponseText, responseText, "Response Text not equal");
        Assert.AreEqual(85, response.Content.Headers.ContentLength, "Content-Length != 85");
        Assert.IsTrue(response.Content.Headers.ContentEncoding.Any(), "Content-Encoding == null");
        Assert.AreEqual("gzip", response.Content.Headers.ContentEncoding.ToString(), "Content-Encoding != gzip");
    }

    /// <summary>
    /// No decompression GZIP test.
    /// </summary>
    [TestMethod]
    public async Task NoDecompressionGZipTest()
    {
        // Arrange
        using var factory = new TestWebApplicationFactory(testType: TestType.NoDecompression);

        // Act
        byte[] compressedBytes;
        await using (var dataStream = new MemoryStream())
        {
            await using (var zipStream = new GZipStream(dataStream, CompressionMode.Compress))
            {
                await using var writer = new StreamWriter(zipStream);
                await writer.WriteAsync(Helpers.ResponseText);
            }

            compressedBytes = dataStream.ToArray();
        }

        HttpResponseMessage response;
        byte[] responseBytes;

        using (HttpClient client = factory.CreateClient())
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
        Assert.IsTrue(compressedBytes.SequenceEqual(responseBytes), "Response bytes not equal");
        Assert.AreEqual(compressedBytes.Length, response.Content.Headers.ContentLength, $"Content-Length != {compressedBytes.Length}");
        Assert.IsFalse(response.Content.Headers.ContentEncoding.Any(), "Content-Encoding != null");
    }

    /// <summary>
    /// The decompression Brotli no compression test.
    /// </summary>
    [TestMethod]
    public async Task DecompressionBrotliNoCompressionTest()
    {
        // Arrange
        using var factory = new TestWebApplicationFactory(testType: TestType.Decompression);

        // Act
        byte[] compressedBytes;
        await using (var dataStream = new MemoryStream())
        {
            await using (var zipStream = new BrotliStream(dataStream, CompressionMode.Compress))
            {
                await using var writer = new StreamWriter(zipStream);
                await writer.WriteAsync(Helpers.ResponseText);
            }

            compressedBytes = dataStream.ToArray();
        }

        HttpResponseMessage response;
        string responseText;

        using (HttpClient client = factory.CreateClient())
        {
            client.DefaultRequestHeaders.Add(HeaderNames.AcceptEncoding, "br");

            var content = new ByteArrayContent(compressedBytes);
            content.Headers.Add(HeaderNames.ContentEncoding, "br");
            content.Headers.Add(HeaderNames.ContentLength, HeaderUtilities.FormatNonNegativeInt64(compressedBytes.Length));

            response = await client.PutAsync("/", content);

            // Assert
            response.EnsureSuccessStatusCode();
            responseText = await response.Content.ReadAsStringAsync();
        }

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "StatusCode != OK");
        Assert.AreEqual(Helpers.ResponseText, responseText, "Response Text not equal");
        Assert.AreEqual(124, response.Content.Headers.ContentLength, "Content-Length != 124");
        Assert.IsFalse(response.Content.Headers.ContentEncoding.Any(), "Content-Encoding != null");
    }

    /// <summary>
    /// The decompression Brotli test.
    /// </summary>
    [TestMethod]
    public async Task DecompressionBrotliTest()
    {
        // Arrange
        using var factory = new TestWebApplicationFactory(options => { options.MinimumCompressionThreshold = 0; }, TestType.Decompression);

        // Act
        byte[] compressedBytes;
        await using (var dataStream = new MemoryStream())
        {
            await using (var zipStream = new BrotliStream(dataStream, CompressionMode.Compress))
            {
                await using var writer = new StreamWriter(zipStream);
                await writer.WriteAsync(Helpers.ResponseText);
            }

            compressedBytes = dataStream.ToArray();
        }

        HttpResponseMessage response;
        string responseText;

        using (HttpClient client = factory.CreateClient())
        {
            client.DefaultRequestHeaders.Add(HeaderNames.AcceptEncoding, "br");

            var content = new ByteArrayContent(compressedBytes);
            content.Headers.Add(HeaderNames.ContentEncoding, "br");
            content.Headers.Add(HeaderNames.ContentLength, HeaderUtilities.FormatNonNegativeInt64(compressedBytes.Length));

            response = await client.PutAsync("/", content);

            // Assert
            response.EnsureSuccessStatusCode();

            Stream stream = await response.Content.ReadAsStreamAsync();

            await using var decompression = new BrotliStream(stream, CompressionMode.Decompress);
            await using var ms = new MemoryStream();
            await decompression.CopyToAsync(ms);
            responseText = Encoding.UTF8.GetString(ms.ToArray());
        }

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "StatusCode != OK");
        Assert.AreEqual(Helpers.ResponseText, responseText, "Response Text not equal");
        Assert.IsTrue(response.Content.Headers.ContentEncoding.Any(), "Content-Encoding == null");
        Assert.AreEqual("br", response.Content.Headers.ContentEncoding.ToString(), "Content-Encoding != br");
    }

    #endregion
}