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

            Mvx.RegisterType<IMapper<Certificate>>(() => new ExternalElementMapper<Certificate>(":memory:"));
            Mvx.RegisterType<IMapper<Attribute>>(() => new AttributeMapper(":memory:", Mvx.Resolve<IMapper<Certificate>>()));
            Mvx.RegisterType<IMapper<ID>>(() => new IDMapper(":memory:", Mvx.Resolve<IMapper<Attribute>>()));
            //Register the BlockchainFacadeSetup (this will likely get reworked) as a Singleton
            Mvx.RegisterSingleton<BlockchainFacadeSetup>(() => new BlockchainFacadeSetup());
            //Register the dummy ContentService as a singleton, temp solution
            Mvx.RegisterSingleton<IContentService>(() => new DummyContentService());
            RegisterAppStart<ViewModels.MainViewModel>();
        }
    }
}
