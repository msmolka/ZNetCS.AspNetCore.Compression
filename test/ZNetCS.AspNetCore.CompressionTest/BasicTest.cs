// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BasicTest.cs" company="Marcin Smółka">
//   Copyright (c) Marcin Smółka. All rights reserved.
// </copyright>
// <summary>
//   The basic tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZNetCS.AspNetCore.CompressionTest;

#region Usings

using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.TestHost;
using Microsoft.Net.Http.Headers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ZNetCS.AspNetCore.Compression;
using ZNetCS.AspNetCore.Compression.Compressors;

#endregion

/// <summary>
/// The basic tests.
/// </summary>
[TestClass]
public class BasicTest
{
    #region Public Methods

    /// <summary>
    /// The basic middleware connection test.
    /// </summary>
    [TestMethod]
    public async Task BasicMiddlewareConnectionTest()
    {
        using var factory = new TestWebApplicationFactory();

        // Act
        HttpResponseMessage response = await factory.Server.CreateRequest("/").SendAsync("PUT");

        // Assert
        response.EnsureSuccessStatusCode();

        string responseText = await response.Content.ReadAsStringAsync();

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "StatusCode != OK");
        Assert.AreEqual(Helpers.ResponseText, responseText, "Response Text not equal");
        Assert.AreEqual(Helpers.ResponseText.Length, response.Content.Headers.ContentLength, $"Content-Length != {Helpers.ResponseText.Length}");
    }

    /// <summary>
    /// The compression threshold zero GZIP ignore test.
    /// </summary>
    [TestMethod]
    public async Task IgnoredCompressionThresholdZeroGZipTest()
    {
        using var factory = new TestWebApplicationFactory(
            options =>
            {
                options.MinimumCompressionThreshold = 0;
                options.Compressors = new List<ICompressor> { new GZipCompressor() };
            });

        // Act
        RequestBuilder request = factory.Server.CreateRequest("/lib/ignore");
        request.AddHeader(HeaderNames.AcceptEncoding, "gzip");

        HttpResponseMessage response = await request.SendAsync("PUT");

        // Assert
        response.EnsureSuccessStatusCode();

        string responseText = await response.Content.ReadAsStringAsync();

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "StatusCode != OK");
        Assert.AreEqual(Helpers.ResponseText, responseText, "Response Text not equal");
        Assert.AreEqual(Helpers.ResponseText.Length, response.Content.Headers.ContentLength, $"Content-Length != {Helpers.ResponseText.Length}");
        Assert.AreEqual(false, response.Content.Headers.ContentEncoding.Any(), "Content-Encoding == null");
    }

    /// <summary>
    /// The basic middleware connection test.
    /// </summary>
    [TestMethod]
    public async Task IgnoredMiddlewareConnectionTest()
    {
        // Arrange
        using var factory = new TestWebApplicationFactory(options => { options.MinimumCompressionThreshold = 0; });

        // Act
        HttpResponseMessage response = await factory.Server.CreateRequest("/lib/test.js").SendAsync("PUT");

        // Assert
        response.EnsureSuccessStatusCode();

        string responseText = await response.Content.ReadAsStringAsync();

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "StatusCode != OK");
        Assert.AreEqual(Helpers.ResponseText, responseText, "Response Text not equal");
        Assert.AreEqual(Helpers.ResponseText.Length, response.Content.Headers.ContentLength, $"Content-Length != {Helpers.ResponseText.Length}");
    }

    #endregion
}