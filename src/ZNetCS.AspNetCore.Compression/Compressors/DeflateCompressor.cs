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
        #region Public Properties

        /// <inheritdoc />
        public override string ContentCoding => "deflate";

        #endregion

        #region Methods

        /// <inheritdoc />
        protected override Stream CreateCompressionStream(Stream compressedDestination)
        {
            return new DeflateStream(compressedDestination, CompressionMode.Compress, leaveOpen: true);
        }

        #endregion
    }
}