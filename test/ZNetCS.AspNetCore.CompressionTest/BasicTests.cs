// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BasicTests.cs" company="Marcin Smółka zNET Computer Solutions">
//   Copyright (c) Marcin Smółka zNET Computer Solutions. All rights reserved.
// </copyright>
// <summary>
//   The basic tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZNetCS.AspNetCore.CompressionTest
{
    #region Usings

    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.TestHost;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using ZNetCS.AspNetCore.Compression;

    #endregion

    /// <summary>
    /// The basic tests.
    /// </summary>
    [TestClass]
    public class BasicTests : TestsBase
    {
        #region Public Methods

        /// <summary>
        /// The basic middleware connection test.
        /// </summary>
        [TestMethod]
        public async Task BasicMiddlewareConnectionTest()
        {
            // Arrange
            using (var server = new TestServer(this.CreateBuilder()))
            {
                // Act
                HttpResponseMessage response = await server.CreateRequest("/").SendAsync("PUT");

                // Assert
                response.EnsureSuccessStatusCode();

                string responseText = await response.Content.ReadAsStringAsync();

                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "StatusCode != OK");
                Assert.AreEqual(Helpers.ResponseText, responseText, "Response Text not equal");
                Assert.AreEqual(Helpers.ResponseText.Length, response.Content.Headers.ContentLength, $"Content-Length != {Helpers.ResponseText.Length}");
            }
        }

        /// <summary>
        /// The basic middleware connection test.
        /// </summary>
        [TestMethod]
        public async Task IgnoredMiddlewareConnectionTest()
        {
            // Arrange
            using (var server = new TestServer(this.CreateBuilder(new CompressionOptions { MinimumCompressionThreshold = 0 })))
            {
                // Act
                HttpResponseMessage response = await server.CreateRequest("/lib/test.js").SendAsync("PUT");

                // Assert
                response.EnsureSuccessStatusCode();

                string responseText = await response.Content.ReadAsStringAsync();

                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "StatusCode != OK");
                Assert.AreEqual(Helpers.ResponseText, responseText, "Response Text not equal");
                Assert.AreEqual(Helpers.ResponseText.Length, response.Content.Headers.ContentLength, $"Content-Length != {Helpers.ResponseText.Length}");
            }
        }

        #endregion
    }
}