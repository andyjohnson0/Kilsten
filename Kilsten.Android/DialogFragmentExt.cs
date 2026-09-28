using Android.Views;


namespace Kilsten.Android;

/// <summary>
/// Extension methods for <see cref="AndroidX.Fragment.App.DialogFragment"/>.
/// </summary>
public static class DialogFragmentExt
{
    /// <summary>
    /// Enters immersive mode for the fragment's dialog, hiding the system bars.
    /// Call from <c>OnStart()</c> and <c>OnResume()</c> in the DialogFragment subclass.
    /// </summary>
    /// <param name="self">Dialog fragment</param>
    public static void EnterImmersiveMode(this AndroidX.Fragment.App.DialogFragment self)
    {
        ArgumentNullException.ThrowIfNull(self);

        var window = self?.Dialog?.Window;
        if (window == null)
        {
            return;
        }

        // Prevent system UI from being triggered
        window.SetFlags(WindowManagerFlags.NotFocusable, WindowManagerFlags.NotFocusable);

        window.EnterImmersiveMode();

        // Clear the not focusable flag so the dialog behaves normally
        window.ClearFlags(WindowManagerFlags.NotFocusable);
    }
}
