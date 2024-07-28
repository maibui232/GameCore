namespace GameCore.Services.ScreenFlow
{
    using GameCore.Extensions.VContainer;
    using GameCore.Extensions.VContainer.Installer;
    using VContainer;

    public class ScreenFlowServiceInstaller : Installer<ScreenFlowServiceInstaller>
    {
        public override void InstallBinding(IContainerBuilder builder)
        {
            builder.RegisterMessage<ScreenInitializeMessage>();
            builder.RegisterMessage<ScreenOpenedMessage>();
            builder.RegisterMessage<ScreenClosedMessage>();
            builder.RegisterMessage<ScreenDestroyedMessage>();
            builder.Register<ScreenFlowService>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}