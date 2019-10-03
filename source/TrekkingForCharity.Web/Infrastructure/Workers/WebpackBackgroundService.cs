using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace TrekkingForCharity.Web.Infrastructure.Workers
{
    public class WebpackBackgroundService : BackgroundService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private Process _process;

        public WebpackBackgroundService(IWebHostEnvironment webHostEnvironment)
        {
            this._webHostEnvironment =
                webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!this._webHostEnvironment.IsDevelopment())
            {
                return Task.FromResult(0);
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = "node",
                Arguments = "node_modules/webpack/bin/webpack.js --config webpack.dev.js --watch",
                CreateNoWindow = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = this._webHostEnvironment.ContentRootPath,
            };
            var tcs = new TaskCompletionSource<int>();
            this._process = new Process
            {
                StartInfo = startInfo,
                EnableRaisingEvents = true,
            };
            this._process.ErrorDataReceived += (sender, args) => { Debug.Write(args.Data); };
            this._process.OutputDataReceived += (sender, args) => { Debug.Write(args.Data); };
            this._process.Exited += (sender, args) =>
            {
                tcs.SetResult(this._process.ExitCode);
                this._process.Dispose();
            };
            this._process.Start();
            this._process.BeginOutputReadLine();
            this._process.BeginErrorReadLine();
            return tcs.Task;
        }

        public override void Dispose()
        {
            base.Dispose();
            if (this._process.HasExited == false)
            {
                this._process.Kill();
            }
        }
    }
}