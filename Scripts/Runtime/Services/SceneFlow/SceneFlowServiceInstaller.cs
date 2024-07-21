namespace GameCore.Services.SceneFlow
{
    using GameCore.Extensions.VContainer.Installer;
    using VContainer;

    public class SceneFlowServiceInstaller : Installer<SceneFlowServiceInstaller>
    {
        public override void InstallBinding(IContainerBuilder builder)
        {
            builder.Register<SceneFlowService>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}