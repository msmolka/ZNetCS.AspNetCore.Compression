// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DecompressorBase.cs" company="Marcin Smółka">
//   Copyright (c) Marcin Smółka. All rights reserved.
// </copyright>
// <summary>
//   The base decompressor class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZNetCS.AspNetCore.Compression.Compressors;

#region Usings

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

#endregion

/// <summary>
/// The base decompressor class.
/// </summary>
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "OK")]
public abstract class DecompressorBase : IDecompressor
{
    #region Public Properties

    /// <inheritdoc />
    public abstract string ContentCoding { get; }

    #endregion

    #region Implemented Interfaces

    #region IDecompressor

    /// <inheritdoc />
    public virtual async Task DecompressAsync(Stream inputStream, Stream outputStream, CancellationToken cancellationToken)
    {
        await using Stream decompressionSource = this.CreateDecompressionStream(inputStream);
        await decompressionSource.CopyToAsync(outputStream, Consts.DefaultBufferSize, cancellationToken);
    }

    #endregion

    #endregion

    #region Methods

    /// <summary>
    /// Creates decompression stream that will be used during decompression process.
    /// </summary>
    /// <param name="compressedSource">
    /// The compressed source to be used for decompression.
    /// </param>
    protected abstract Stream CreateDecompressionStream(Stream compressedSource);

    #endregion
}