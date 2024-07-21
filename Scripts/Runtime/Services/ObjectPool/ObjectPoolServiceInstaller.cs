namespace GameCore.Services.ObjectPool
{
    using GameCore.Extensions.VContainer.Installer;
    using VContainer;

    public class ObjectPoolServiceInstaller : Installer<ObjectPoolServiceInstaller>
    {
        public override void InstallBinding(IContainerBuilder builder)
        {
            builder.Register<ObjectPoolService>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}