namespace GameCore.Extensions.VContainer
{
    using System;
    using System.Collections.Generic;
    using GameCore.Extensions.VContainer.Installer;
    using global::VContainer;
    using UnityEngine;

    [Serializable]
    public class ScopeConfig
    {
        [SerializeField] private List<MonoInstaller> monoInstallers;

        public void InstallBinding(IContainerBuilder builder)
        {
            foreach (var installer in this.monoInstallers)
            {
                installer.InstallBinding(builder);
            }
        }
    }
}