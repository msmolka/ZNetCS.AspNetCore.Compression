// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompressionOptions.cs" company="Marcin Smółka">
//   Copyright (c) Marcin Smółka. All rights reserved.
// </copyright>
// <summary>
//   The compression option used for middleware compression and decompression process.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZNetCS.AspNetCore.Compression;

#region Usings

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Microsoft.Net.Http.Headers;

using ZNetCS.AspNetCore.Compression.Compressors;

#endregion

/// <summary>
/// The compression option used for middleware compression and decompression process.
/// </summary>
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "Public API")]
public class CompressionOptions
{
    #region Public Properties

    /// <summary>
    /// Gets or sets the compression allowed media types.
    /// </summary>
    [SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Options")]
    public ICollection<MediaTypeHeaderValue> AllowedMediaTypes { get; set; }
        = new List<MediaTypeHeaderValue>
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

    /// <summary>
    /// Gets or sets the collection of compressors.
    /// </summary>
    [SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Options")]
    public ICollection<ICompressor>? Compressors { get; set; }
        = new List<ICompressor> { new BrotliCompressor(), new GZipCompressor(), new DeflateCompressor() };

    /// <summary>
    /// Gets or sets the collection of decompressors.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "OK")]
    [SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Options")]
    public ICollection<IDecompressor>? Decompressors { get; set; }
        = new List<IDecompressor> { new BrotliDecompressor(), new GZipDecompressor(), new DeflateDecompressor() };

    /// <summary>
    /// Gets or sets the paths to be ignored for compression.
    /// </summary>
    [SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Options")]
    public ICollection<string>? IgnoredPaths { get; set; }
        = new List<string>
        {
            "/css/",
            "/images/",
            "/js/",
            "/lib/"
        };

    /// <summary>
    /// Gets or sets the minimum compression threshold.
    /// </summary>
    public int MinimumCompressionThreshold { get; set; } = 860;

    #endregion
}