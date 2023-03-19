using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using ServerLauncher.Services;

namespace ServerLauncher
{
    public class Launcher
    {
        public Launcher()
        {
            Initialize();
        }

        private void Initialize()
        {
            ConfigureService();
            CreateSessionService();
        }

        private void SetConfiguration()
        {

        }

        private void CreateSessionService()
        {
            Assembly a = Assembly.Load("Session");
            Type myType = a.GetType("Session.Program");
            MethodInfo myMethod = myType.GetMethod("Main");
            object obj = Activator.CreateInstance(myType);

            myMethod.Invoke(obj, new string[1]);
        }
        
        public void ConfigureService()
        {
            var builder = WebApplication.CreateBuilder();

            builder.Services.AddGrpc();

            var app = builder.Build();

            app.MapGrpcService<GreeterService>();
            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

            app.RunAsync();
        }
    }
}
