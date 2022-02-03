// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationBuilderExtensions.cs" company="Marcin Smółka">
//   Copyright (c) Marcin Smółka. All rights reserved.
// </copyright>
// <summary>
//   Extension methods for adding the <see cref="CompressionMiddleware" /> to an application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.Builder;

#region Usings

using System;
using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using ZNetCS.AspNetCore.Compression;

#endregion

/// <summary>
/// Extension methods for adding the <see cref="CompressionMiddleware"/> to an application.
/// </summary>
public static class ApplicationBuilderExtensions
{
    #region Public Methods

    /// <summary>
    /// Adds the <see cref="CompressionMiddleware"/> to automatically set compressors and decompressors for
    /// requests and responses based on information provided by the client.
    /// </summary>
    /// <param name="app">
    /// The <see cref="IApplicationBuilder"/> to use compression on.
    /// </param>
    /// <returns>
    /// The <see cref="IApplicationBuilder"/> so that additional calls can be chained.
    /// </returns>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "OK")]
    public static IApplicationBuilder UseCompression(this IApplicationBuilder app)
    {
        if (app == null)
        {
            throw new ArgumentNullException(nameof(app));
        }

        return app.UseMiddleware<CompressionMiddleware>();
    }

    #endregion
}