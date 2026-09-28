using Android.Views;
using AndroidX.Core.View;


namespace Kilsten.Android;

/// <summary>
/// Extension methods for <see cref="Window"/>.
/// </summary>
internal static class WindowExt
{
    /// <summary>
    /// Enters immersive mode, hiding the system bars until the user swipes from the edge of the screen.
    /// </summary>
    /// <param name="self">Window</param>
    public static void EnterImmersiveMode(this Window self)
    {
        if (OperatingSystem.IsAndroidVersionAtLeast(30))  // SDK 30+ / Android 11
        {
            // https://developer.android.com/develop/ui/views/layout/immersive
            var windowInsetsController = WindowCompat.GetInsetsController(self, self.DecorView)!;
            windowInsetsController.SystemBarsBehavior = WindowInsetsControllerCompat.BehaviorShowTransientBarsBySwipe;
            windowInsetsController.Hide(WindowInsetsCompat.Type.SystemBars());
        }
        else
        {
#pragma warning disable CS0618 // Type or member is obsolete
            // See https://developer.android.com/training/system-ui/immersive
            var flags = SystemUiFlags.Immersive | SystemUiFlags.Fullscreen | SystemUiFlags.HideNavigation |
                        SystemUiFlags.LayoutFullscreen | SystemUiFlags.LayoutHideNavigation | SystemUiFlags.LayoutStable;
            self.DecorView.SystemUiVisibility = (StatusBarVisibility)(int)flags;
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }
}
