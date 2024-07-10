namespace GameCore.Services.Message
{
    using System;
    using System.Collections.Generic;
    using MessagePipe;
    using VContainer;

    public interface IMessageService
    {
        void Subscribe<TMessage>(Action              callback);
        void Subscribe<TMessage>(Action<TMessage>    callback);
        void TrySubscribe<TMessage>(Action           callback);
        void TrySubscribe<TMessage>(Action<TMessage> callback);

        void Unsubscribe<TMessage>(Action              callback);
        void Unsubscribe<TMessage>(Action<TMessage>    callback);
        void TryUnsubscribe<TMessage>(Action           callback);
        void TryUnsubscribe<TMessage>(Action<TMessage> callback);

        void Publish<TMessage>(TMessage    message);
        void TryPublish<TMessage>(TMessage message);
    }

    public class MessageService : IMessageService, IDisposable
    {
        private readonly Dictionary<(Type, Delegate), IDisposable> delegateToDisposable = new();

#region Inject

        private readonly IObjectResolver resolver;

#endregion

        public MessageService(IObjectResolver resolver)
        {
            this.resolver = resolver;
        }

        public void Dispose()
        {
            this.resolver?.Dispose();
            foreach (var (_, disposable) in this.delegateToDisposable) disposable.Dispose();

            this.delegateToDisposable.Clear();
        }

        public void Subscribe<TMessage>(Action callback)
        {
            this.InternalSubscribe<TMessage>(callback);
        }

        public void Subscribe<TMessage>(Action<TMessage> callback)
        {
            this.InternalSubscribe(callback);
        }

        public void TrySubscribe<TMessage>(Action callback)
        {
            try
            {
                this.InternalSubscribe<TMessage>(callback);
            }
            catch (Exception)
            {
                // ignore
            }
        }

        public void TrySubscribe<TMessage>(Action<TMessage> callback)
        {
            try
            {
                this.InternalSubscribe(callback);
            }
            catch (Exception)
            {
                // ignore
            }
        }

        public void Unsubscribe<TMessage>(Action callback)
        {
            this.InternalUnsubscribe<TMessage>(callback);
        }

        public void Unsubscribe<TMessage>(Action<TMessage> callback)
        {
            this.InternalUnsubscribe(callback);
        }

        public void TryUnsubscribe<TMessage>(Action callback)
        {
            try
            {
                this.InternalUnsubscribe<TMessage>(callback);
            }
            catch (Exception)
            {
                // ignore
            }
        }

        public void TryUnsubscribe<TMessage>(Action<TMessage> callback)
        {
            try
            {
                this.InternalUnsubscribe(callback);
            }
            catch (Exception)
            {
                // ignore
            }
        }

        public void Publish<TMessage>(TMessage message)
        {
            this.InternalPublish(message);
        }

        public void TryPublish<TMessage>(TMessage message)
        {
            try
            {
                this.InternalPublish(message);
            }
            catch (Exception)
            {
                // ignore
            }
        }

        private void InternalPublish<TMessage>(TMessage message)
        {
            if (this.resolver.TryResolve<IPublisher<TMessage>>(out var publisher))
                publisher.Publish(message);
            else
                throw new Exception($"Don't has message broker: {typeof(TMessage)}");
        }

        private void InternalSubscribe<TMessage>(Action callback)
        {
            if (callback == null) throw new ArgumentNullException();
            var key = (typeof(TMessage), callback);

            if (this.delegateToDisposable.TryGetValue(key, out _))
                throw new Exception($"Subscribe message multiple time: {callback}");

            if (this.resolver.TryResolve<ISubscriber<TMessage>>(out var subscriber))
            {
                var disposable = subscriber.Subscribe(_ => callback.Invoke());
                this.delegateToDisposable.Add(key, disposable);
            }
            else
            {
                throw new Exception($"Don't has message broker: {typeof(TMessage)}");
            }
        }

        private void InternalSubscribe<TMessage>(Action<TMessage> callback)
        {
            if (callback == null) throw new ArgumentNullException();
            var key = (typeof(TMessage), callback);

            if (this.delegateToDisposable.TryGetValue(key, out _))
                throw new Exception($"Subscribe message multiple time: {callback}");

            if (this.resolver.TryResolve<ISubscriber<TMessage>>(out var subscriber))
            {
                var disposable = subscriber.Subscribe(callback.Invoke);
                this.delegateToDisposable.Add(key, disposable);
            }
            else
            {
                throw new Exception($"Don't has message broker: {typeof(TMessage)}");
            }
        }

        private void InternalUnsubscribe<TMessage>(Action callback)
        {
            if (callback == null) throw new ArgumentNullException();
            var key = (typeof(TMessage), callback);

            if (!this.delegateToDisposable.Remove(key, out var disposable))
                throw new Exception($"Don's has subscribe for {callback}");

            disposable.Dispose();
        }

        private void InternalUnsubscribe<TMessage>(Action<TMessage> callback)
        {
            if (callback == null) throw new ArgumentNullException();
            var key = (typeof(TMessage), callback);

            if (!this.delegateToDisposable.Remove(key, out var disposable))
                throw new Exception($"Don's has subscribe for {callback}");

            disposable.Dispose();
        }
    }
}