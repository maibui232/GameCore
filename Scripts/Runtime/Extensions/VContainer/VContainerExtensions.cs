namespace GameCore.Extensions.VContainer
{
    using System.Collections.Generic;
    using GameCore.Services.ScreenFlow;
    using GameCore.Services.ScreenFlow.Base;
    using GameCore.Services.ScreenFlow.Base.Screen;
    using global::VContainer;
    using MessagePipe;

    public static class VContainerExtensions
    {
        private static readonly List<Registration> Registrations = new();
        private static readonly MessagePipeOptions Options       = new();

        public static IContainerBuilder ContainerBuilder { get; set; }

        public static IContainerBuilder RegisterMessage<TMessage>(this IContainerBuilder builder)
        {
            return builder.RegisterMessageBroker<TMessage>(Options);
        }

        public static void InitScreenManually<TPresenter>(this IContainerBuilder builder) where TPresenter : IScreenPresenter
        {
            builder.RegisterBuildCallback(resolver => resolver.Resolve<IScreenFlowService>().InitScreenManually<TPresenter>());
        }

        public static void InitScreenManually<TPresenter, TModel>(this IContainerBuilder builder, TModel model) where TPresenter : IScreenPresenter<TModel>
        {
            builder.RegisterBuildCallback(resolver => resolver.Resolve<IScreenFlowService>().InitScreenManually<TPresenter, TModel>(model));
        }
    }
}