namespace GameCore.Services.BlueprintFlow.BlueprintControlFlow
{
    using System.Linq;
    using GameCore.Extensions.VContainer;
    using GameCore.Extensions.VContainer.Installer;
    using GameCore.Services.BlueprintFlow.APIHandler;
    using GameCore.Services.BlueprintFlow.BlueprintReader;
    using GameCore.Services.BlueprintFlow.Message;
    using UnityEngine;
    using VContainer;

    public class BlueprintServicesInstaller : Installer<BlueprintServicesInstaller>
    {
        public override void InstallBinding(IContainerBuilder builder)
        {
            builder.Register<PreProcessBlueprintMobile>(Lifetime.Singleton);
            builder.Register<FetchBlueprintInfo>(Lifetime.Singleton);
            builder.Register<BlueprintDownloader>(Lifetime.Singleton);
            builder.Register<BlueprintReaderManager>(Lifetime.Singleton);
            builder.Register<BlueprintConfig>(Lifetime.Singleton);

            builder.RegisterAllDerivedType<IGenericBlueprintReader>(Lifetime.Singleton);
            
            builder.RegisterMessage<LoadBlueprintDataSucceedMessage>();
            builder.RegisterMessage<LoadBlueprintDataProgressMessage>();
            builder.RegisterMessage<BlueprintProgressMessage>();
        }
    }
}