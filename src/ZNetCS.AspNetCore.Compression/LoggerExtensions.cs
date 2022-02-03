// -----------------------------------------------------------------------
// <copyright file="LoggerExtensions.cs" company="Marcin Smółka">
// Copyright (c) Marcin Smółka. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ZNetCS.AspNetCore.Compression;

#region Usings

using System;
using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.Logging;

#endregion

[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:Elements should be documented", Justification = "Internal")]
internal static class LoggerExtensions
{
    #region Static Fields

    private static readonly Action<ILogger, string, Exception?> CompressingDefinition =
        LoggerMessage.Define<string?>(LogLevel.Debug, new EventId(100, "Compressing"), "Compressing response using {ContentCoding} compressor");

    private static readonly Action<ILogger, Exception?> CompressingFinishedDefinition =
        LoggerMessage.Define(LogLevel.Debug, new EventId(101, "CompressingFinished"), "Finished compressing request");

    private static readonly Action<ILogger, string, Exception?> CompressionPathCheckDefinition =
        LoggerMessage.Define<string?>(LogLevel.Debug, new EventId(105, "CompressionPathCheck"), "Checking response for compression: {Path}");

    private static readonly Action<ILogger, string, Exception?> DecompressingDefinition =
        LoggerMessage.Define<string?>(LogLevel.Debug, new EventId(102, "Decompressing"), "Decompressing response using {ContentCoding} decompressor");

    private static readonly Action<ILogger, Exception?> DecompressingFinishedDefinition =
        LoggerMessage.Define(LogLevel.Debug, new EventId(103, "DecompressingFinished"), "Finished decompressing request");

    private static readonly Action<ILogger, string, Exception?> DecompressionPathCheckDefinition =
        LoggerMessage.Define<string?>(LogLevel.Debug, new EventId(106, "DecompressionPathCheck"), "Checking request for decompression: {Path}");

    private static readonly Action<ILogger, Exception?> FinishedDefinition =
        LoggerMessage.Define(LogLevel.Debug, new EventId(109, "Finished"), "Finished handling request");

    private static readonly Action<ILogger, Exception?> NoCompressionBodyDefinition =
        LoggerMessage.Define(LogLevel.Debug, new EventId(107, "NoCompressionBody"), "Continue response without writing any body");

    private static readonly Action<ILogger, Exception?> NoCompressionDefinition =
        LoggerMessage.Define(LogLevel.Debug, new EventId(108, "NoCompression"), "Continue response without compression");

    #endregion

    #region Public Methods

    public static void Compressing(this ILogger logger, string contentCoding)
    {
        CompressingDefinition(logger, contentCoding, null);
    }

    public static void CompressingFinished(this ILogger logger)
    {
        CompressingFinishedDefinition(logger, null);
    }

    public static void CompressionPathCheck(this ILogger logger, string path)
    {
        CompressionPathCheckDefinition(logger, path, null);
    }

    public static void Decompressing(this ILogger logger, string contentCoding)
    {
        DecompressingDefinition(logger, contentCoding, null);
    }

    public static void DecompressingFinished(this ILogger logger)
    {
        DecompressingFinishedDefinition(logger, null);
    }

    public static void DecompressionPathCheck(this ILogger logger, string path)
    {
        DecompressionPathCheckDefinition(logger, path, null);
    }

    public static void Finished(this ILogger logger)
    {
        FinishedDefinition(logger, null);
    }

    public static void NoCompression(this ILogger logger)
    {
        NoCompressionDefinition(logger, null);
    }

    public static void NoCompressionBody(this ILogger logger)
    {
        NoCompressionBodyDefinition(logger, null);
    }

    #endregion
}