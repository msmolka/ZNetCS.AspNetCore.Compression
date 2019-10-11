// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BrotliDecompressor.cs" company="Marcin Smółka zNET Computer Solutions">
//   Copyright (c) Marcin Smółka zNET Computer Solutions. All rights reserved.
// </copyright>
// <summary>
//   Brotli decompressor implementation.
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
    /// Brotli decompressor implementation.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "OK")]
    public class BrotliDecompressor : DecompressorBase
    {
        #region Public Properties

        /// <inheritdoc/>
        public override string ContentCoding => "br";

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override Stream CreateDecompressionStream(Stream compressedSource) => new BrotliStream(compressedSource, CompressionMode.Decompress, true);

        #endregion
    }
}
#endif