using System;
using System.Threading.Tasks;

using Castle.DynamicProxy;
using Common;
using JetBrains.Annotations;

namespace Interception
{
    /// <summary>
    /// Represents the interceptor that log method calls.
    /// </summary>
    public class LoggingInterceptor : AsyncAdaptedInterceptor
    {
        private readonly ILog _log;

        public LoggingInterceptor([NotNull] ILog log)
        {
            AssertArg.NotNull(log, nameof(log));

            _log = log;
        }

        public override void InterceptSynchronous(IInvocation invocation)
        {
            var message = new MethodCallMessage(invocation);

            _log.Debug(message.Calling());

            try
            {
                invocation.Proceed();

                var hasReturnValue = invocation.MethodInvocationTarget.ReturnType != typeof(void);

                var successMessage = hasReturnValue
                    ? message.SuccessfullyCompleted(invocation.ReturnValue)
                    : message.SuccessfullyCompleted();

                _log.Debug(successMessage);
            }
            catch (Exception ex)
            {
                _log.Error(message.Fault(ex), ex);

                throw;
            }
        }

        public override void InterceptAsynchronous(IInvocation invocation)
        {
            InterceptAsynchronousInternal<Task>(
                invocation,
                async (task, message) =>
                {
                    await task.ConfigureAwait(false);

                    return message.SuccessfullyCompleted();
                });
        }

        public override void InterceptAsynchronous<TResult>(IInvocation invocation)
        {
            InterceptAsynchronousInternal<Task<TResult>>(
                invocation,
                async (task, message) =>
                {
                    var result = await task.ConfigureAwait(false);

                    return message.SuccessfullyCompleted(result);
                });
        }

        private async void InterceptAsynchronousInternal<TTask>(
            IInvocation invocation,
            Func<TTask, MethodCallMessage, Task<string>> tryWork)
            where TTask : Task
        {
            var message = new MethodCallMessage(invocation);

            _log.Debug(message.Calling());

            try
            {
                invocation.Proceed();

                var task = (TTask)invocation.ReturnValue;

                var logMessage = await tryWork(task, message);

                _log.Debug(logMessage);
            }
            catch (OperationCanceledException)
            {
                _log.Info(message.Cancelled());
            }
            catch (Exception ex)
            {
                _log.Error(message.Fault(ex), ex);

                throw;
            }
        }

        private class MethodCallMessage
        {
            private readonly Guid _id = Guid.NewGuid();

            private readonly string _methodName;

            public MethodCallMessage(IInvocation invocation)
            {
                _methodName = $"{invocation.TargetType.Name}.{invocation.MethodInvocationTarget.Name}";
            }

            public string Calling() => ToString(nameof(Calling));
            public string Cancelled() => ToString(nameof(Cancelled));
            public string Fault(Exception ex) => ToString($"{nameof(Fault)} with {ex.GetType().Name}");
            public string SuccessfullyCompleted() => ToString(nameof(SuccessfullyCompleted));

            public string SuccessfullyCompleted<TResult>(TResult result) =>
                $"{ToString(nameof(SuccessfullyCompleted))} Result='{result}'";

            private string ToString(string state) =>
                $"Call: ID='{_id}' Method='{_methodName}' State='{state}'";
        }
    }
}