using Castle.DynamicProxy;
using Serilog;

namespace KSW.Interception
{
    public class LoggingInterceptor : IAsyncInterceptor
    {
        public void InterceptSynchronous(IInvocation invocation)
        {
            Log.Information($"{invocation.Method.Name}方法执行前");
            try
            {
                // 同步方法拦截逻辑
                invocation.Proceed();
            }
            catch (Exception ex)
            {
                Log.Error($"{invocation.Method.Name}方法执行异常:{ex.Message}");
                // 处理异常
                throw ex;
            }
            Log.Information($"{invocation.Method.Name}方法执行后");
        }

        public void InterceptAsynchronous(IInvocation invocation)
        {
            Log.Information($"{invocation.Method.Name}方法执行前");
            invocation.Proceed();
            var task = (Task)invocation.ReturnValue;
            invocation.ReturnValue = InterceptAsync(task, invocation);
            Log.Information($"{invocation.Method.Name}方法执行后");
        }

        public void InterceptAsynchronous<TResult>(IInvocation invocation)
        {
            Log.Information($"{invocation.Method.Name}方法执行前");
            invocation.Proceed();
            var task = (Task<TResult>)invocation.ReturnValue;
            invocation.ReturnValue = InterceptAsync(task, invocation);
            Log.Information($"{invocation.Method.Name}方法执行后");
        }

        private async Task InterceptAsync(Task task, IInvocation invocation)
        {
            try
            {
                await task.ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Log.Error($"{invocation.Method.Name}方法执行异常:{ex.Message}");
                throw;
            }
        }

        private async Task<TResult> InterceptAsync<TResult>(Task<TResult> task, IInvocation invocation)
        {
            try
            {
                var result = await task.ConfigureAwait(false);
                return result;
            }
            catch (Exception ex)
            {
                Log.Error($"{invocation.Method.Name}方法执行异常:{ex.Message}");
                throw;
            }
        }
    }
}
