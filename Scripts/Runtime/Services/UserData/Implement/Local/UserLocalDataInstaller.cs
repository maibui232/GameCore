namespace GameCore.Services.UserData.Implement.Local
{
    using GameCore.Extensions.VContainer.Installer;
    using VContainer;

    public class UserLocalDataInstaller : Installer<UserLocalDataInstaller>
    {
        public override void InstallBinding(IContainerBuilder builder)
        {
            builder.Register<UserUserDataService>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}