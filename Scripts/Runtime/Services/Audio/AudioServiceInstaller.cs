namespace GameCore.Services.Audio
{
    using GameCore.Extensions.VContainer.Installer;
    using VContainer;

    public class AudioServiceInstaller : Installer<AudioServiceInstaller>
    {
        public override void InstallBinding(IContainerBuilder builder)
        {
            builder.Register<AudioService>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}