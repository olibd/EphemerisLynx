using Lynx.Core.Communications.Packets.Interfaces;
using Lynx.Core.Crypto;
using Lynx.Core.Crypto.Interfaces;
using Lynx.Core.Mappers.IDSubsystem.SQLiteMappers;
using Lynx.Core.Mappers.IDSubsystem.Strategies;
using Lynx.Core.Facade.Interfaces;
using Lynx.Core.Facade;
using Lynx.Core.Models.IDSubsystem;
using MvvmCross.Platform;
using MvvmCross.Platform.IoC;
using Attribute = Lynx.Core.Models.IDSubsystem.Attribute;
using Lynx.Core.PeerVerification.Interfaces;
using Lynx.Core.PeerVerification;
using Lynx.Core.Interfaces;

namespace Lynx.Core
{
    /// <summary>
    /// App. This class bootstraps the app at startup.
    /// </summary>
    public class App : MvvmCross.Core.ViewModels.MvxApplication
    {
        private IPlatformSpecificDataService _dataService;
        public App(IPlatformSpecificDataService dataService)
        {
            _dataService = dataService;
        }

        public override void Initialize()
        {
            //Register dependencies
            CreatableTypes()
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();
            Mvx.RegisterSingleton<ID>(() => new ID());
            Mvx.RegisterSingleton<ITokenCryptoService<IToken>>(() => new TokenCryptoService<IToken>(Mvx.Resolve<IECCCryptoService>()));

            Mvx.RegisterSingleton(() => _dataService);

            string dbfile = _dataService.GetDatabaseFile();

            Mvx.RegisterType<IMapper<Certificate>>(() => new ExternalElementMapper<Certificate>(dbfile));
            Mvx.RegisterType<IMapper<Attribute>>(() => new AttributeMapper(dbfile, Mvx.Resolve<IMapper<Certificate>>()));
            Mvx.RegisterType<IMapper<ID>>(() => new IDMapper(dbfile, Mvx.Resolve<IMapper<Attribute>>()));

            //Configure the the eth node
            Mvx.GetSingleton<ILynxConfigurationService>().ConfigureEthNode(null, null, "0x6d10E8ca4d6EC1De78D11D12fCAF89256f6FD13E", "http://cd9e5e83.ngrok.io");

            //Register the dummy ContentService as a singleton, temp solution
            Mvx.RegisterSingleton<IContentService>(() => new DummyContentService());
            RegisterAppStart<ViewModels.MainViewModel>();

            Mvx.RegisterType<IRequester>(() => new Requester(Mvx.Resolve<ITokenCryptoService<IToken>>(), Mvx.Resolve<IAccountService>(), Mvx.Resolve<ID>(), Mvx.Resolve<IIDFacade>(), Mvx.Resolve<IAttributeFacade>(), Mvx.Resolve<ICertificateFacade>()));
            Mvx.RegisterType<IVerifier>(() => new Verifier(Mvx.Resolve<ITokenCryptoService<IToken>>(), Mvx.Resolve<IAccountService>(), Mvx.Resolve<ID>(), Mvx.Resolve<IIDFacade>(), Mvx.Resolve<ICertificateFacade>()));
        }
    }
}
