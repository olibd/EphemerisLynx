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
using NBitcoin;

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
            //Initializing NBitcoin's RNG
            RandomUtils.Random = new LynxSecureRandom();

            //Register dependencies
            CreatableTypes()
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();
            Mvx.RegisterSingleton<ITokenCryptoService<IToken>>(() => new TokenCryptoService<IToken>(Mvx.Resolve<IECCCryptoService>()));

            Mvx.RegisterSingleton(() => _dataService);

            string dbfile = _dataService.GetDatabaseFile();

            Mvx.RegisterType<IMapper<Certificate>>(() => new ExternalElementMapper<Certificate>(dbfile));
            Mvx.RegisterType<IMapper<Attribute>>(() => new AttributeMapper(dbfile, Mvx.Resolve<IMapper<Certificate>>()));
            Mvx.RegisterType<IMapper<ID>>(() => new IDMapper(dbfile, Mvx.Resolve<IMapper<Attribute>>()));

            //Configure the the eth node
            Mvx.GetSingleton<ILynxConfigurationService>().ConfigureEthNode("0xcddd77d4b7a28faea9e1ed225e559656527c60b9", "http://jmon.tech:8545");

            //Register the dummy ContentService as a singleton, temp solution
            Mvx.RegisterSingleton<IContentService>(() => new DummyContentService());
            RegisterAppStart<ViewModels.MainViewModel>();

            Mvx.RegisterType<IRequester>(() => new Requester(Mvx.Resolve<ITokenCryptoService<IToken>>(), Mvx.Resolve<IAccountService>(), Mvx.Resolve<ID>(), Mvx.Resolve<IIDFacade>(), Mvx.Resolve<IAttributeFacade>(), Mvx.Resolve<ICertificateFacade>()));
            Mvx.RegisterType<IReceiver>(() => new Receiver(Mvx.Resolve<ITokenCryptoService<IToken>>(), Mvx.Resolve<IAccountService>(), Mvx.Resolve<ID>(), Mvx.Resolve<IIDFacade>(), Mvx.Resolve<ICertificateFacade>()));
        }
    }
}
