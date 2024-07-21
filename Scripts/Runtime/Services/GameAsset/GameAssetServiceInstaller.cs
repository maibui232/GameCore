namespace GameCore.Services.GameAsset
{
    using GameCore.Extensions.VContainer.Installer;
    using VContainer;

    public class GameAssetServiceInstaller : Installer<GameAssetServiceInstaller>
    {
        public override void InstallBinding(IContainerBuilder builder)
        {
            builder.Register<GameAssetService>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}