// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Marcin Smółka zNET Computer Solutions" file="ServiceCollectionExtensions.cs">
//   Copyright (c) Marcin Smółka zNET Computer Solutions. All rights reserved.
// </copyright>
// <summary>
//   The service collection extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    #region Usings

    using System;

    using Microsoft.Extensions.DependencyInjection.Extensions;
    using ZNetCS.AspNetCore.Compression;
    using ZNetCS.AspNetCore.Compression.Infrastructure;

    #endregion

    /// <summary>
    /// The service collection extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        #region Public Methods

        /// <summary>
        /// Adds compression executor services to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">
        /// The <see cref="IServiceCollection"/> to add services to.
        /// </param>
        /// <returns>
        /// The <see cref="IServiceCollection"/> so that additional calls can be chained.
        /// </returns>
        public static IServiceCollection AddCompression(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddOptions();

            services.TryAddSingleton<CompressionExecutor>();
            services.TryAddSingleton<DecompressionExecutor>();

            return services;
        }

        /// <summary>
        /// Adds compression executor services to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">
        /// The <see cref="IServiceCollection"/> to add services to.
        /// </param>
        /// <param name="configure">
        /// The <see cref="CompressionMiddleware"/> configuration delegate.
        /// </param>
        /// <returns>
        /// The <see cref="IServiceCollection"/> so that additional calls can be chained.
        /// </returns>
        public static IServiceCollection AddCompression(this IServiceCollection services, Action<CompressionOptions> configure)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddOptions();

            services.Configure(configure);

            services.TryAddSingleton<CompressionExecutor>();
            services.TryAddSingleton<DecompressionExecutor>();

            return services;
        }

        #endregion
    }
}