namespace GameCore
{
    using GameCore.Extensions.VContainer;
    using GameCore.Extensions.VContainer.Installer;
    using GameCore.Services.Audio;
    using GameCore.Services.GameAsset;
    using GameCore.Services.LocalData;
    using GameCore.Services.Message;
    using GameCore.Services.ObjectPool;
    using GameCore.Services.SceneFlow;
    using GameCore.Services.ScreenFlow;
    using VContainer;

    public class GameCoreInstaller : Installer<GameCoreInstaller>
    {
        public override void InstallBinding(IContainerBuilder builder)
        {
            VContainerExtensions.ContainerBuilder = builder;
            MessageInstaller.Install(builder);

            builder.Register<MessageService>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<GameAssetService>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<LocalDataService>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<ObjectPoolService>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<AudioService>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<SceneFlowService>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<ScreenFlowService>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}