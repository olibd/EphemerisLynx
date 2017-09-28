using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lynx.API.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Hangfire;
using Hangfire.MemoryStorage;
using Lynx.Core.Crypto.Interfaces;
using Lynx.Core.Communications.Packets.Interfaces;
using Lynx.Core.Mappers.IDSubsystem.Strategies;
using Lynx.Core.Models.IDSubsystem;
using Attribute = Lynx.Core.Models.IDSubsystem.Attribute;
using Lynx.Core.Facade.Interfaces;
using Lynx.Core.PeerVerification.Interfaces;
using Lynx.Core.Mappers.IDSubsystem.SQLiteMappers;
using Lynx.Core.Crypto;
using Lynx.Core.Facade;
using Lynx.Core.PeerVerification;
using Nethereum.Web3;
using Lynx.Core.Interfaces;

namespace Lynx.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.  policy => policy.RequireClaim("EmployeeNumber")
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ClientContext>(opt => opt.UseInMemoryDatabase("ClientsList"));
            services.AddDbContext<SessionContext>(opt => opt.UseInMemoryDatabase("Sessions"));
            //TODO: temporary for testing purposes, hangfire is deployed in memory
            var inMemory = GlobalConfiguration.Configuration.UseMemoryStorage();
            services.AddHangfire(x => x.UseStorage(inMemory));
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHangfireServer();
            app.UseHangfireDashboard();
            app.UseMvc();
        }

        private void RegisterLynxCoreDependencies(IServiceCollection services)
        {
            services.AddTransient<IECCCryptoService, SECP256K1CryptoService>();
            var sp = services.BuildServiceProvider();
            services.AddSingleton<ITokenCryptoService<IToken>>(new TokenCryptoService<IToken>(sp.GetService<IECCCryptoService>()));

            string dbfile = _dataService.GetDatabaseFile();

            services.AddSingleton<IMapper<Certificate>>(new ExternalElementMapper<Certificate>(dbfile));
            sp = services.BuildServiceProvider();
            services.AddSingleton<IMapper<Attribute>>(new AttributeMapper(dbfile, sp.GetService<IMapper<Certificate>>()));
            sp = services.BuildServiceProvider();
            services.AddSingleton<IMapper<ID>>(new IDMapper(dbfile, sp.GetService<IMapper<Attribute>>()));

            //Register the dummy ContentService as a singleton, temp solution
            services.AddSingleton<IContentService>(new DummyContentService());

            Web3 web3 = new Web3("http://jmon.tech:8545");
            services.AddSingleton<Web3>(web3);
        }
    }
}
