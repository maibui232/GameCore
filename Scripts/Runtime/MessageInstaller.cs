namespace GameCore
{
    using GameCore.Extensions.VContainer;
    using GameCore.Extensions.VContainer.Installer;
    using GameCore.Services.SceneFlow;
    using GameCore.Services.ScreenFlow;
    using MessagePipe;
    using VContainer;

    public class MessageInstaller : Installer<MessageInstaller>
    {
        public override void InstallBinding(IContainerBuilder builder)
        {
            builder.RegisterMessagePipe();

            // Scene Flow
            builder.RegisterMessage<OpenSingleSceneMessage>();
            builder.RegisterMessage<OpenAdditiveSceneMessage>();
            builder.RegisterMessage<ReleaseSceneMessage>();

            // Screen Flow
            builder.RegisterMessage<ScreenInitializeMessage>();
            builder.RegisterMessage<ScreenOpenedMessage>();
            builder.RegisterMessage<ScreenClosedMessage>();
            builder.RegisterMessage<ScreenDestroyedMessage>();
        }
    }
}