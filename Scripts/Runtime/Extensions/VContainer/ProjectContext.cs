namespace GameCore.Extensions.VContainer
{
    using global::VContainer;
    using global::VContainer.Unity;
    using UnityEngine;

    public class ProjectContext : LifetimeScope
    {
        [SerializeField] private ScopeConfig scopeConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            this.scopeConfig.InstallBinding(builder);
        }
    }
}