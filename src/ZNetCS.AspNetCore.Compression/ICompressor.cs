// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICompressor.cs" company="Marcin Smółka zNET Computer Solutions">
//   Copyright (c) Marcin Smółka zNET Computer Solutions. All rights reserved.
// </copyright>
// <summary>
//   The compressor interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZNetCS.AspNetCore.Compression
{
    #region Usings

    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    #endregion

    /// <summary>
    /// The compressor interface.
    /// </summary>
    public interface ICompressor : IContentCoding
    {
        #region Public Methods

        /// <summary>
        /// Asynchronously compresses the <paramref name="inputStream"/> stream into <paramref name="outputStream"/> one.
        /// </summary>
        /// <param name="inputStream">
        /// The source stream to be compressed.
        /// </param>
        /// <param name="outputStream">
        /// The destination stream to be compressed.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token, to support compression cancellation.
        /// </param>
        Task CompressAsync(Stream inputStream, Stream outputStream, CancellationToken cancellationToken);

        #endregion
    }
}