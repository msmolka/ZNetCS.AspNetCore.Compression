// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GZipDecompressor.cs" company="Marcin Smółka">
//   Copyright (c) Marcin Smółka. All rights reserved.
// </copyright>
// <summary>
//   GZIP decompressor implementation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZNetCS.AspNetCore.Compression.Compressors;

#region Usings

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;

#endregion

/// <summary>
/// GZIP decompressor implementation.
/// </summary>
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "OK")]
public class GZipDecompressor : DecompressorBase
{
    #region Public Properties

    /// <inheritdoc/>
    public override string ContentCoding => "gzip";

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override Stream CreateDecompressionStream(Stream compressedSource) => new GZipStream(compressedSource, CompressionMode.Decompress, true);

    #endregion
}