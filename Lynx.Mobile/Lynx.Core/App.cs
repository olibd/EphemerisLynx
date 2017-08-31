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

namespace Lynx.Core
{
    public class App : MvvmCross.Core.ViewModels.MvxApplication
    {
        public override void Initialize()
        {
            //Register dependencies
            CreatableTypes()
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();
            Mvx.RegisterSingleton<ID>(() => new ID());
            Mvx.RegisterSingleton<ITokenCryptoService<IHandshakeToken>>(() => new TokenCryptoService<IHandshakeToken>(Mvx.Resolve<IECCCryptoService>()));

            Mvx.RegisterType<IMapper<Certificate>>(() => new ExternalElementMapper<Certificate>(":memory:"));
            Mvx.RegisterType<IMapper<Attribute>>(() => new AttributeMapper(":memory:", Mvx.Resolve<IMapper<Certificate>>()));
            Mvx.RegisterType<IMapper<ID>>(() => new IDMapper(":memory:", Mvx.Resolve<IMapper<Attribute>>()));

            //Configure the the eth node
            Mvx.GetSingleton<ILynxConfigurationService>().ConfigureEthNode(null, null, "0x731a10897d267e19b34503ad902d0a29173ba4b1", "http://jmon.tech");

            //Register the dummy ContentService as a singleton, temp solution
            Mvx.RegisterSingleton<IContentService>(() => new DummyContentService());
            RegisterAppStart<ViewModels.MainViewModel>();
        }
    }
}
