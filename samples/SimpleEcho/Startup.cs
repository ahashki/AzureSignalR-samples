﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Azure.SignalR.Samples.SimpleEcho
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSignalR().AddAzureSignalR();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime)
        {
            app.UseAzureSignalR(routes =>
            {
                routes.MapHub<EchoHub>("/echo");
            });
            applicationLifetime.ApplicationStarted.Register(LaunchClient);

            async void LaunchClient()
            {
                try
                {
                    var task = new Client().Run();
                    if (await Task.WhenAny(task, Task.Delay(TimeSpan.FromMinutes(1))) != task)
                    {
                        throw new TimeoutException();
                    }
                }
                finally
                {
                    applicationLifetime.StopApplication();
                }
            }
        }
    }
}
