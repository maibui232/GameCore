namespace GameCore.Services.Message
{
    using GameCore.Extensions.VContainer.Installer;
    using MessagePipe;
    using VContainer;

    public class MessageServiceInstaller : Installer<MessageServiceInstaller>
    {
        public override void InstallBinding(IContainerBuilder builder)
        {
            builder.RegisterMessagePipe();
            builder.Register<MessageService>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}