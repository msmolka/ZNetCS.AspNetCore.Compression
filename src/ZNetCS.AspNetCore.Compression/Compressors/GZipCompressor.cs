// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GZipCompressor.cs" company="Marcin Smółka">
//   Copyright (c) Marcin Smółka. All rights reserved.
// </copyright>
// <summary>
//   GZIP compressor implementation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZNetCS.AspNetCore.Compression.Compressors;

#region Usings

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;

#endregion

/// <summary>
/// GZIP compressor implementation.
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API")]
public class GZipCompressor : CompressorBase
{
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="GZipCompressor"/> class.
    /// </summary>
    public GZipCompressor() : this(CompressionLevel.Optimal)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GZipCompressor"/> class.
    /// </summary>
    /// <param name="compressionLevel">
    /// The compression level.
    /// </param>
    public GZipCompressor(CompressionLevel compressionLevel) => this.CompressionLevel = compressionLevel;

    #endregion

    #region Public Properties

    /// <inheritdoc/>
    public override CompressionLevel CompressionLevel { get; }

    /// <inheritdoc/>
    public override string ContentCoding => "gzip";

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override Stream CreateCompressionStream(Stream compressedDestination) => new GZipStream(compressedDestination, this.CompressionLevel, true);

    #endregion
}