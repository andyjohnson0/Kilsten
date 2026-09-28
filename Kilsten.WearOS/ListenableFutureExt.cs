using Google.Common.Util.Concurrent;

using Java.Util.Concurrent;


namespace Kilsten.WearOS;

/// <summary>
/// Extension methods for <see cref="IListenableFuture"/>.
/// </summary>
internal static class ListenableFutureExt
{
    /// <summary>
    /// Gets a task that completes when the future completes, with the future's result, its failure, or its
    /// cancellation.
    /// </summary>
    /// <param name="self">Future</param>
    /// <param name="executor">Executor to run the completion listener on</param>
    /// <returns>Task representing the future</returns>
    public static Task<Java.Lang.Object?> AsTask(this IListenableFuture self, IExecutor executor)
    {
        var tcs = new TaskCompletionSource<Java.Lang.Object?>(TaskCreationOptions.RunContinuationsAsynchronously);

        self.AddListener(new Java.Lang.Runnable(() =>
        {
            try
            {
                tcs.TrySetResult(self.Get());
            }
            catch (ExecutionException ex)
            {
                tcs.TrySetException(ex.Cause ?? ex);
            }
            catch (CancellationException)
            {
                tcs.TrySetCanceled();
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
        }), executor);

        return tcs.Task;
    }
}
