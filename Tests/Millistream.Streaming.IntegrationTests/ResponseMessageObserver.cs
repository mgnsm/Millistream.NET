using System;

namespace Millistream.Streaming.IntegrationTests
{
    internal class ResponseMessageObserver : IObserver<ResponseMessage>
    {
        private readonly Action<ResponseMessage> _onNext;
        private readonly Action<Exception> _onError;
        private readonly Action _onCompleted;

        internal ResponseMessageObserver(Action<ResponseMessage> onNext, Action<Exception> onError, Action onCompleted)
        {
            _onNext = onNext;
            _onError = onError;
            _onCompleted = onCompleted;
        }

        public void OnCompleted() => _onCompleted?.Invoke();

        public void OnError(Exception error) => _onError?.Invoke(error);

        public void OnNext(ResponseMessage value) => _onNext?.Invoke(value);
    }
}
