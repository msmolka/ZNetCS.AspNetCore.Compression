// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompressionOptions.cs" company="Marcin Smółka zNET Computer Solutions">
//   Copyright (c) Marcin Smółka zNET Computer Solutions. All rights reserved.
// </copyright>
// <summary>
//   The compression option used for middleware compression and decompression process.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZNetCS.AspNetCore.Compression
{
    #region Usings

    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Microsoft.Net.Http.Headers;

    using ZNetCS.AspNetCore.Compression.Compressors;

    #endregion

    /// <summary>
    /// The compression option used for middleware compression and decompression process.
    /// </summary>
    public class CompressionOptions
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CompressionOptions"/> class.
        /// </summary>
        public CompressionOptions()
        {
            this.AllowedMediaTypes = new List<MediaTypeHeaderValue>
            {
                MediaTypeHeaderValue.Parse("text/*"),
                MediaTypeHeaderValue.Parse("message/*"),
                MediaTypeHeaderValue.Parse("application/x-javascript"),
                MediaTypeHeaderValue.Parse("application/javascript"),
                MediaTypeHeaderValue.Parse("application/json"),
                MediaTypeHeaderValue.Parse("application/xml"),
                MediaTypeHeaderValue.Parse("application/atom+xml"),
                MediaTypeHeaderValue.Parse("application/xaml+xml")
            };

            this.IgnoredPaths = new List<string>
            {
                "/css/",
                "/images/",
                "/js/",
                "/lib/"
            };

            this.MinimumCompressionThreshold = 860;

            this.Compressors = new List<ICompressor> { new GZipCompressor(), new DeflateCompressor() };
            this.Decompressors = new List<IDecompressor> { new GZipDecompressor(), new DeflateDecompressor() };
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the compression allowed media types.
        /// </summary>
        public ICollection<MediaTypeHeaderValue> AllowedMediaTypes { get; set; }

        /// <summary>
        /// Gets or sets the collection of compressors.
        /// </summary>
        public ICollection<ICompressor> Compressors { get; set; }

        /// <summary>
        /// Gets or sets the collection of decompressors.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "OK")]
        public ICollection<IDecompressor> Decompressors { get; set; }

        /// <summary>
        /// Gets or sets the paths to be ignored for compression.
        /// </summary>
        public ICollection<string> IgnoredPaths { get; set; }

        /// <summary>
        /// Gets or sets the minimum compression threshold.
        /// </summary>
        public int MinimumCompressionThreshold { get; set; }

        #endregion
    }
}