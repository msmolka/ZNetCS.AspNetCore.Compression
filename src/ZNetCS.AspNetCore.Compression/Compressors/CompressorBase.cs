// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompressorBase.cs" company="Marcin Smółka zNET Computer Solutions">
//   Copyright (c) Marcin Smółka zNET Computer Solutions. All rights reserved.
// </copyright>
// <summary>
//   The base compressor class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZNetCS.AspNetCore.Compression.Compressors
{
    #region Usings

    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    #endregion

    /// <summary>
    /// The base compressor class.
    /// </summary>
    public abstract class CompressorBase : ICompressor
    {
        #region Public Properties

        /// <inheritdoc />
        public abstract string ContentCoding { get; }

        #endregion

        #region Implemented Interfaces

        #region ICompressor

        /// <inheritdoc />
        public virtual async Task CompressAsync(Stream inputStream, Stream outputStream, CancellationToken cancellationToken)
        {
            using (Stream compressedStream = this.CreateCompressionStream(outputStream))
            {
                await inputStream.CopyToAsync(compressedStream, Consts.DefaultBufferSize, cancellationToken);
            }
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Creates compression stream that will be used during compression process.
        /// </summary>
        /// <param name="compressedDestination">
        /// The compressed destination used as compression output.
        /// </param>
        protected abstract Stream CreateCompressionStream(Stream compressedDestination);

        #endregion
    }
}