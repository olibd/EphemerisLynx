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
using Lynx.Core;
using Hangfire.Console;

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
            services.AddHangfire(config =>
            {
                config.UseStorage(inMemory);
                config.UseConsole();
            });
            RegisterLynxCoreDependencies(services);
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            GlobalConfiguration.Configuration
                               .UseActivator(new HangfireActivator(serviceProvider));

            app.UseHangfireServer();
            app.UseHangfireDashboard();
            app.UseMvc();
        }

        private void RegisterLynxCoreDependencies(IServiceCollection services)
        {
            services.AddTransient<IECCCryptoService, SECP256K1CryptoService>();
            var sp = services.BuildServiceProvider();
            services.AddSingleton<ITokenCryptoService<IToken>>(new TokenCryptoService<IToken>(sp.GetService<IECCCryptoService>()));

            string dbfile = "idDatabase.db";

            services.AddSingleton<IMapper<Certificate>>(new ExternalElementMapper<Certificate>(dbfile));
            sp = services.BuildServiceProvider();
            services.AddSingleton<IMapper<Attribute>>(new AttributeMapper(dbfile, sp.GetService<IMapper<Certificate>>()));
            sp = services.BuildServiceProvider();
            services.AddSingleton<IMapper<ID>>(new IDMapper(dbfile, sp.GetService<IMapper<Attribute>>()));

            //Register the dummy ContentService as a singleton, temp solution
            services.AddSingleton<IContentService>(new DummyContentService());

            Web3 web3 = new Web3("http://4bf33ea8.ngrok.io");
            services.AddSingleton<Web3>(web3);

            SeedDatabases(services);
        }

        //TODO: TEMPORARY, FOR TESTING PURPOSES
        private void SeedDatabases(IServiceCollection services)
        {
            var sp = services.BuildServiceProvider();
            SeedID(sp).Wait();
        }

        private async Task SeedID(IServiceProvider sp)
        {
            AccountService accountService = new AccountService("9e6a6bf412ce4e3a91a33c7c0f6d94b3127b8d4f5ed336210a672fe595bf1769");
            IContentService contServ = sp.GetService<IContentService>();
            ICertificateFacade certFacade = new CertificateFacade(sp.GetService<Web3>(), contServ, accountService);
            IAttributeFacade attrFacade = new AttributeFacade(sp.GetService<Web3>(), certFacade, contServ, accountService);
            IIDFacade idFacade = new IDFacade("0x455E342dEdc41bc3C82eb3C4E830bF172100B1d9", sp.GetService<Web3>(), attrFacade, accountService);

            //create some dummy attributes
            Attribute name = new Attribute()
            {
                Hash = "hash" + "Ephemeris",
                Location = "Location" + "Ephemeris",
                Content = new StringContent("Ephemeris"),
                Description = "name"
            };

            Attribute address = new Attribute()
            {
                Location = "Location" + "2",
                Hash = "2",
                Content = new StringContent("31 rue des Pommmiers"),
                Description = "address"
            };

            Attribute phone = new Attribute()
            {
                Location = "Location" + "3",
                Hash = "3",
                Content = new StringContent("555-555-5555"),
                Description = "phone"
            };


            ID id = new ID();
            id.AddAttribute(name);
            id.AddAttribute(address);
            id.AddAttribute(phone);

            try
            {
                id = await idFacade.DeployAsync(id);
            }
            catch (Exception e)
            {
                var o = e;
            }

            await sp.GetService<IMapper<ID>>().SaveAsync(id);
        }
    }
}
