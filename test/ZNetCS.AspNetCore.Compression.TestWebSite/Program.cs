// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Marcin Smółka zNET Computer Solutions">
//   Copyright (c) Marcin Smółka zNET Computer Solutions. All rights reserved.
// </copyright>
// <summary>
//   The program.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZNetCS.AspNetCore.Compression.TestWebSite
{
    #region Usings

    using System.IO;

    using Microsoft.AspNetCore.Hosting;

    #endregion

    /// <summary>
    /// The program.
    /// </summary>
    public class Program
    {
        #region Public Methods

        /// <summary>
        /// The main application entry.
        /// </summary>
        /// <param name="args">
        /// The startup arguments.
        /// </param>
        public static void Main(string[] args)
        {
            IWebHost host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }

        #endregion
    }
}