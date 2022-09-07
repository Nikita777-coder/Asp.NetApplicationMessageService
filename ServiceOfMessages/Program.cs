using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace ServiceOfMessages
{
    /// <summary>
    ///  ласс дл€ запуска программы.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// “очка запуска проекта.
        /// </summary>
        /// <param name="args">параметры, передаваемые с консоли</param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// —оздание конструктора хостов.
        /// </summary>
        /// <param name="args">параметры, передаваемые с консоли</param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
