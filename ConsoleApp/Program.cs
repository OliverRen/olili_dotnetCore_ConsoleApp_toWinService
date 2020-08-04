using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                ServiceBase[] services = new ServiceBase[] { new Service1(), };
                ServiceBase.Run(services);
                return;
            }
            
            var currDomain = AppDomain.CurrentDomain;
            var path = Path.Combine(currDomain.BaseDirectory, currDomain.FriendlyName);
            switch (args[0])
            {
                case "-cmd":
                {
                    StartMain();
                    return;
                }
            }
        }

        static void StartMain()
        {
            Console.WriteLine("Hello World!");

            // 使用轻量级手动事件复位
            ManualResetEventSlim exit = new ManualResetEventSlim(false);
            // 注册事件以拦截 ctrl+C,ctrl+Break 的关闭窗口
            Console.CancelKeyPress += (sender, e) =>
             {
                 var key = e.SpecialKey;
                 e.Cancel = false;
                 exit.Set();
             };
            // 注册事件以在appdomain关闭的时候回调
            // 这里操作系统等待的事件并不长,依然又可能来不及关闭所有的资源,但是给了一个机会去运行在wait后的代码
            AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
            {
                exit.Set();
            };

            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
            services.AddLogging(build => build.SetMinimumLevel(LogLevel.Trace));

            var container=services.BuildServiceProvider();
            var loggerFactory=container.GetRequiredService<ILoggerFactory>();
            var logger = container.GetService<ILogger<Program>>();
            logger.LogDebug("start");

            Task.Run(() =>
            {
                var timer = new Timer((_) =>
                  {
                      var msg = $"{DateTime.Now.ToLongTimeString()}";
                      logger.LogDebug(msg);
                      Console.WriteLine(msg);
                  }, null, 0, 1000);
            }).ConfigureAwait(false);

            // do a lot back thread job
            // add some threads
            // open some resources
            // start listen sockets

            // 主线程等待
            exit.Wait();

            // 手动重置后程序结束

            // close back thread
            // close resources
            // close net listen

            Console.WriteLine("Bye Bye");
        }
    }
}
