namespace GameCore.Extensions.VContainer.Installer
{
    using System;
    using global::VContainer;

    public interface IInstaller
    {
        void InstallBinding(IContainerBuilder builder);
    }

    public abstract class Installer<TDerived> : IInstaller where TDerived : IInstaller
    {
        public abstract void InstallBinding(IContainerBuilder builder);

        public static void Install(IContainerBuilder builder)
        {
            Activator.CreateInstance<TDerived>().InstallBinding(builder);
        }
    }
}