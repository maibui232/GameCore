namespace GameCore.Extensions.VContainer.Installer
{
    using global::VContainer;
    using UnityEngine;

    public abstract class MonoInstaller : MonoBehaviour, IInstaller
    {
        public abstract void InstallBinding(IContainerBuilder builder);
    }
}