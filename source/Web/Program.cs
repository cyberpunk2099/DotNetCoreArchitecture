using DotNetCore.AspNetCore;
using Microsoft.Extensions.Hosting;

namespace DotNetCoreArchitecture.Web
{
    public static class Program
    {
        public static void Main()
        {
            Host.CreateDefaultBuilder().Run<Startup>();
        }
    }
}
