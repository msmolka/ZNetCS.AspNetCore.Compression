// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GZipCompressor.cs" company="Marcin Smółka zNET Computer Solutions">
//   Copyright (c) Marcin Smółka zNET Computer Solutions. All rights reserved.
// </copyright>
// <summary>
//   GZIP compressor implementation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZNetCS.AspNetCore.Compression.Compressors
{
    #region Usings

    using System.IO;
    using System.IO.Compression;

    #endregion

    /// <summary>
    /// GZIP compressor implementation.
    /// </summary>
    public class GZipCompressor : CompressorBase
    {
        #region Public Properties

        /// <inheritdoc />
        public override string ContentCoding => "gzip";

        #endregion

        #region Methods

        /// <inheritdoc />
        protected override Stream CreateCompressionStream(Stream compressedDestination)
        {
            return new GZipStream(compressedDestination, CompressionMode.Compress, leaveOpen: true);
        }

        #endregion
    }
}