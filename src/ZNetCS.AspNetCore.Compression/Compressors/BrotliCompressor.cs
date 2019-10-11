// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BrotliCompressor.cs" company="Marcin Smółka zNET Computer Solutions">
//   Copyright (c) Marcin Smółka zNET Computer Solutions. All rights reserved.
// </copyright>
// <summary>
//   Brotli compressor implementation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#if NETCOREAPP3_0
namespace ZNetCS.AspNetCore.Compression.Compressors
{
    #region Usings

    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.IO.Compression;

    #endregion

    /// <summary>
    /// Brotli compressor implementation.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    public class BrotliCompressor : CompressorBase
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BrotliCompressor"/> class.
        /// </summary>
        public BrotliCompressor() : this(CompressionLevel.Optimal)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BrotliCompressor"/> class.
        /// </summary>
        /// <param name="compressionLevel">
        /// The compression level.
        /// </param>
        public BrotliCompressor(CompressionLevel compressionLevel) => this.CompressionLevel = compressionLevel;

        #endregion

        #region Public Properties

        /// <inheritdoc/>
        public override CompressionLevel CompressionLevel { get; }

        /// <inheritdoc/>
        public override string ContentCoding => "br";

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override Stream CreateCompressionStream(Stream compressedDestination) => new BrotliStream(compressedDestination, this.CompressionLevel, leaveOpen: true);

        #endregion
    }
}
#endif