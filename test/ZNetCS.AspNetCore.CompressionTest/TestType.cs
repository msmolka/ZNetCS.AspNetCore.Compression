// -----------------------------------------------------------------------
// <copyright file="TestType.cs" company="Marcin Smółka">
// Copyright (c) Marcin Smółka. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ZNetCS.AspNetCore.CompressionTest;

/// <summary>
/// Test type.
/// </summary>
internal enum TestType
{
    /// <summary>
    /// The compression.
    /// </summary>
    Compression = 0,

    /// <summary>
    /// The decompression.
    /// </summary>
    Decompression = 1,

    /// <summary>
    /// No content.
    /// </summary>
    NoContent = 2,

    /// <summary>
    /// No decompression.
    /// </summary>
    NoDecompression = 3
}