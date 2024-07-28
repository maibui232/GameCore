namespace GameCore.Services.SceneFlow
{
    using GameCore.Extensions.VContainer;
    using GameCore.Extensions.VContainer.Installer;
    using VContainer;

    public class SceneFlowServiceInstaller : Installer<SceneFlowServiceInstaller>
    {
        public override void InstallBinding(IContainerBuilder builder)
        {
            builder.RegisterMessage<OpenSingleSceneMessage>();
            builder.RegisterMessage<OpenAdditiveSceneMessage>();
            builder.RegisterMessage<ReleaseSceneMessage>();
            builder.Register<SceneFlowService>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}