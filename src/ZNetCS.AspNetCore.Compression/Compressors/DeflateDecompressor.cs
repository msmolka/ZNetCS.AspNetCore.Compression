// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeflateDecompressor.cs" company="Marcin Smółka zNET Computer Solutions">
//   Copyright (c) Marcin Smółka zNET Computer Solutions. All rights reserved.
// </copyright>
// <summary>
//   Deflate decompressor implementation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZNetCS.AspNetCore.Compression.Compressors
{
    #region Usings

    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.IO.Compression;

    #endregion

    /// <summary>
    /// Deflate decompressor implementation.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "OK")]
    public class DeflateDecompressor : DecompressorBase
    {
        #region Public Properties

        /// <inheritdoc />
        public override string ContentCoding => "deflate";

        #endregion

        #region Methods

        /// <inheritdoc />
        protected override Stream CreateDecompressionStream(Stream compressedSource)
        {
            return new DeflateStream(compressedSource, CompressionMode.Decompress, leaveOpen: true);
        }

        #endregion
    }
}