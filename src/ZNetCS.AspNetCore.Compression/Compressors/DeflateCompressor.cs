// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeflateCompressor.cs" company="Marcin Smółka zNET Computer Solutions">
//   Copyright (c) Marcin Smółka zNET Computer Solutions. All rights reserved.
// </copyright>
// <summary>
//   Deflate compressor implementation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZNetCS.AspNetCore.Compression.Compressors
{
    #region Usings

    using System.IO;
    using System.IO.Compression;

    #endregion

    /// <summary>
    /// Deflate compressor implementation.
    /// </summary>
    public class DeflateCompressor : CompressorBase
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DeflateCompressor"/> class.
        /// </summary>
        public DeflateCompressor() : this(CompressionLevel.Optimal)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeflateCompressor"/> class.
        /// </summary>
        /// <param name="compressionLevel">
        /// The compression level.
        /// </param>
        public DeflateCompressor(CompressionLevel compressionLevel) => this.CompressionLevel = compressionLevel;

        #endregion

        #region Public Properties

        /// <inheritdoc/>
        public override CompressionLevel CompressionLevel { get; }

        /// <inheritdoc/>
        public override string ContentCoding => "deflate";

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override Stream CreateCompressionStream(Stream compressedDestination) => new DeflateStream(compressedDestination, this.CompressionLevel, true);

        #endregion
    }
}