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
            Mvx.RegisterSingleton(() => new BlockchainFacadeSetup());
            RegisterAppStart<ViewModels.MainViewModel>();
        }
    }
}
