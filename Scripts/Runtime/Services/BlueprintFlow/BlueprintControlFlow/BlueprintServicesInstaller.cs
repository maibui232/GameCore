namespace GameCore.Services.BlueprintFlow.BlueprintControlFlow
{
    using GameCore.Extensions.VContainer;
    using GameCore.Extensions.VContainer.Installer;
    using GameCore.Services.BlueprintFlow.APIHandler;
    using GameCore.Services.BlueprintFlow.Signals;
    using VContainer;

    public class BlueprintServicesInstaller : Installer<BlueprintServicesInstaller>
    {
        public override void InstallBinding(IContainerBuilder builder)
        {
            builder.Register<PreProcessBlueprintMobile>(Lifetime.Scoped);
            builder.Register<FetchBlueprintInfo>(Lifetime.Scoped);
            builder.Register<BlueprintDownloader>(Lifetime.Scoped);
            builder.Register<BlueprintReaderManager>(Lifetime.Scoped);
            builder.Register<BlueprintConfig>(Lifetime.Scoped);

            // builder.BindAllTypeDriveFrom<IGenericBlueprintReader>();

            builder.RegisterMessage<LoadBlueprintDataSucceedMessage>();
            builder.RegisterMessage<LoadBlueprintDataProgressMessage>();
            builder.RegisterMessage<ReadBlueprintProgressMessage>();
        }
    }
}