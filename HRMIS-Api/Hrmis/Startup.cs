using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;

[assembly: OwinStartup(typeof(Hrmis.Startup))]

namespace Hrmis
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            string root = AppDomain.CurrentDomain.BaseDirectory;
            var physicalFileSystem = new PhysicalFileSystem(Path.Combine(root, "wwwroot"));
            var options = new FileServerOptions
            {
                RequestPath = PathString.Empty,
                EnableDefaultFiles = true,
                FileSystem = physicalFileSystem
            };
            options.StaticFileOptions.FileSystem = physicalFileSystem;
            options.StaticFileOptions.ServeUnknownFileTypes = false;
            app.UseFileServer(options);

        }
    }
}
