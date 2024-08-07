namespace GameCore.Extensions.VContainer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using GameCore.Services.ScreenFlow;
    using GameCore.Services.ScreenFlow.Base.Screen;
    using GameExtensions.Reflection;
    using global::VContainer;
    using MessagePipe;
    using UnityEngine;

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

        public static T InstantiateConcrete<T>(this IObjectResolver resolver)
        {
            var ctors = typeof(T).GetConstructors();
            switch (ctors.Length)
            {
                case > 1:
                    throw new Exception($"{typeof(T).Name} need to implement only 1 constructor");
                case 0:
                    return Activator.CreateInstance<T>();
            }

            var ctor      = ctors[0];
            var arguments = ctor.GetParameters().Select(param => resolver.Resolve(param.ParameterType)).ToArray();
            var instance  = Activator.CreateInstance(typeof(T), arguments);

            return (T)instance;
        }

        public static void RegisterAllDerivedType<T>(this IContainerBuilder builder, Lifetime lifetime)
        {
            var allType = AppDomain.CurrentDomain.GetAllTypeFromDerived(typeof(T));
            foreach (var type in allType)
            {
                builder.Register(type, lifetime);
            }
        }
    }
}