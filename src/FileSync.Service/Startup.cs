﻿using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using FileSync.Common;
using FileSync.Common.Filesystem;

namespace FileSync.Service
{
    sealed class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var currentDirectory = new SystemFilepath(Directory.GetCurrentDirectory());

            services
                .AddSingleton<IDirectoryFactory>(new DirectoryFactory(currentDirectory))
                .AddSingleton<IFileHasher, FileHasher>()
                .AddSingleton<IDirectoryListingService, DirectoryListingService>()
                .AddSingleton<IFileContentService, FileContentService>()
                .AddControllers(options => options.SuppressAsyncSuffixInActionNames = false)
                .AddJsonOptions(options => options.JsonSerializerOptions.IgnoreNullValues = true);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app
                .UseRouting()
                .UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
