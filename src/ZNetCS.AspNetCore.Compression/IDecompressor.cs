// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDecompressor.cs" company="Marcin Smółka zNET Computer Solutions">
//   Copyright (c) Marcin Smółka zNET Computer Solutions. All rights reserved.
// </copyright>
// <summary>
//   The decompressor interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZNetCS.AspNetCore.Compression
{
    #region Usings

    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    #endregion

    /// <summary>
    /// The decompressor interface.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "OK")]
    public interface IDecompressor : IContentCoding
    {
        #region Public Methods

        /// <summary>
        /// Asynchronously decompresses the <paramref name="inputStream"/> stream into
        /// <paramref name="outputStream"/> one.
        /// </summary>
        /// <param name="inputStream">
        /// The compressed source stream to be decompressed.
        /// </param>
        /// <param name="outputStream">
        /// The destination stream to be decompressed.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token, to support decompression cancellation.
        /// </param>
        Task DecompressAsync(Stream inputStream, Stream outputStream, CancellationToken cancellationToken);

        #endregion
    }
}