using Castle.DynamicProxy;

namespace Interception
{
    public abstract class AsyncAdaptedInterceptor : IInterceptor, IAsyncInterceptor
    {
        private readonly AsyncDeterminationInterceptor _interceptor;

        protected AsyncAdaptedInterceptor()
        {
            _interceptor = new AsyncDeterminationInterceptor(this);
        }

        public void Intercept(IInvocation invocation) => _interceptor.Intercept(invocation);

        public abstract void InterceptSynchronous(IInvocation invocation);

        public abstract void InterceptAsynchronous(IInvocation invocation);

        public abstract void InterceptAsynchronous<TResult>(IInvocation invocation);
    }
}