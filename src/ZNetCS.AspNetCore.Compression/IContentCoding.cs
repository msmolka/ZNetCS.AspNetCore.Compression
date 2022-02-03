// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IContentCoding.cs" company="Marcin Smółka">
//   Copyright (c) Marcin Smółka. All rights reserved.
// </copyright>
// <summary>
//   The content coding interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZNetCS.AspNetCore.Compression;

/// <summary>
/// The content coding interface.
/// </summary>
public interface IContentCoding
{
    #region Public Properties

    /// <summary>
    /// Gets the supported content coding.
    /// </summary>
    string ContentCoding { get; }

    #endregion
}