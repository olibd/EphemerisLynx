using Lynx.Core.Facade.Interfaces;
using Lynx.Core.Facade;
using Lynx.Core.Models.IDSubsystem;
using MvvmCross.Platform;
using MvvmCross.Platform.IoC;

namespace Lynx.Core
{
    public class App : MvvmCross.Core.ViewModels.MvxApplication
    {
        public override void Initialize()
        {
            CreatableTypes()
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();
            //Register the ID as a Singleton
            Mvx.RegisterSingleton<ID>(() => new ID());
            //Register the BlockchainFacadeSetup (this will likely get reworked) as a Singleton
            Mvx.RegisterSingleton<BlockchainFacadeSetup>(() => new BlockchainFacadeSetup());
            //Register the dummy ContentService as a singleton, temp solution
            Mvx.RegisterSingleton<IContentService>(() => new DummyContentService());
            RegisterAppStart<ViewModels.MainViewModel>();
        }
    }
}
