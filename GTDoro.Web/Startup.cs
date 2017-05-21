using GTDoro.Core.DAL;
using GTDoro.Core.Models;
using Microsoft.Owin;
using Owin;
using System;
using System.Linq;

[assembly: OwinStartupAttribute(typeof(GTDoro.Web.Startup))]
namespace GTDoro.Web
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
