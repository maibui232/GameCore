namespace GameCore.Services.UserData.Implement.Local
{
    using GameCore.Extensions.VContainer;
    using GameCore.Extensions.VContainer.Installer;
    using GameCore.Services.UserData.Interface;
    using VContainer;

    public class UserLocalDataInstaller : Installer<UserLocalDataInstaller>
    {
        public override void InstallBinding(IContainerBuilder builder)
        {
            builder.RegisterAllDerivedType<IUserData>(Lifetime.Singleton);
            builder.Register<UserUserDataService>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}