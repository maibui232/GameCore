namespace GameCore.Services.ScreenFlow
{
    using GameCore.Extensions.VContainer.Installer;
    using VContainer;

    public class ScreenFlowServiceInstaller : Installer<ScreenFlowServiceInstaller>
    {
        public override void InstallBinding(IContainerBuilder builder)
        {
            builder.Register<ScreenFlowService>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}