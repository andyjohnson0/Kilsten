using Java.Util.Concurrent;

using Android.Content;

using AndroidX.Wear.Activity;
using AndroidX.Wear.Remote.Interactions;


namespace Kilsten.WearOS;

/// <summary>
/// Opens URLs on the phone paired with the watch.
/// </summary>
public static class ExternalUrl
{
    private static readonly Lazy<IExecutorService> remoteActivityExecutor = new(() => Executors.NewSingleThreadExecutor()!);


    /// <summary>
    /// Opens a URL in the browser on the paired phone. If the phone accepts the request, shows the "open on phone"
    /// confirmation animation.
    /// The app's manifest must declare <c>android.support.wearable.activity.ConfirmationActivity</c>.
    /// </summary>
    /// <param name="context">Context</param>
    /// <param name="message">Message to show with the animation, e.g. "Continue on phone"</param>
    /// <param name="url">URL to open</param>
    /// <returns>True if the phone accepted the request, otherwise false - for example, if no phone is connected</returns>
    public static async Task<bool> OpenAsync(
        Context context,
        string message,
        global::Android.Net.Uri url)
    {
        try
        {
            // Open in browser on paired device.
            var intent = new Intent(Intent.ActionView)
                .AddCategory(Intent.CategoryBrowsable)
                .SetData(url);
            var remoteActivityHelper = new RemoteActivityHelper(context, remoteActivityExecutor.Value);
            await remoteActivityHelper.StartRemoteActivity(intent, null).AsTask(remoteActivityExecutor.Value);

            // Show "continue on phone" animation.
            // Note that the following needs to be manually added to the application manifest:
            //     <activity android:name="android.support.wearable.activity.ConfirmationActivity" />
            intent = new Intent(context, Java.Lang.Class.FromType(typeof(ConfirmationActivity)));
            intent.PutExtra(ConfirmationActivity.ExtraAnimationType, ConfirmationActivity.OpenOnPhoneAnimation);
            intent.PutExtra(ConfirmationActivity.ExtraMessage, message);
            intent.PutExtra(ConfirmationActivity.ExtraAnimationDurationMillis, 2500);
            context.StartActivity(intent);

            return true;
        }
        catch (Exception)
        {
            // Do nothing
            return false;
        }
    }
}
