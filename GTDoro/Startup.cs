using GTDoro.DAL;
using GTDoro.Models;
using Microsoft.Owin;
using Owin;
using System;
using System.Linq;

[assembly: OwinStartupAttribute(typeof(GTDoro.Startup))]
namespace GTDoro
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

        }

        static void Main(string[] args)
        {
        } 
    }
}
