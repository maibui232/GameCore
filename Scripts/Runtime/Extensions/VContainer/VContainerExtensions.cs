namespace GameCore.Extensions.VContainer
{
    using System;
    using System.Collections.Generic;
    using global::VContainer;
    using MessagePipe;

    public static class VContainerExtensions
    {
        private static readonly List<Registration> Registrations = new();
        private static readonly MessagePipeOptions Options       = new();

        public static T InstantiateConcrete<T>(this IContainerBuilder builder, IObjectResolver resolver)
        {
            return (T)builder.Register<T>(Lifetime.Scoped).Build().SpawnInstance(resolver);
        }

        public static object InstantiateConcrete(this IContainerBuilder builder, IObjectResolver resolver, Type type)
        {
            return builder.Register(type, Lifetime.Scoped).Build().SpawnInstance(resolver);
        }

        public static IContainerBuilder RegisterMessage<TMessage>(this IContainerBuilder builder)
        {
            return builder.RegisterMessageBroker<TMessage>(Options);
        }
    }
}