namespace GameCore.Extensions.VContainer
{
    using System;
    using global::VContainer;

    public interface ISubContainer
    {
        void Configuring(IContainerBuilder builder);
    }

    public abstract class SubContainer<TDerived> : ISubContainer where TDerived : ISubContainer
    {
        public abstract void Configuring(IContainerBuilder builder);

        public static void Configure(IContainerBuilder builder)
        {
            Activator.CreateInstance<TDerived>().Configuring(builder);
        }
    }
}