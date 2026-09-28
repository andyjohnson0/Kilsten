using Android.Views;
using Android.Util;

using AndroidX.AppCompat.App;


namespace Kilsten.Android;

/// <summary>
/// Extension methods for <see cref="Activity"/>.
/// </summary>
public static class ActivityExt
{
    /// <summary>
    /// Gets the activity's content view - the view set by <c>SetContentView()</c>.
    /// </summary>
    /// <param name="self">Activity</param>
    /// <returns>The content view, or null if there is none</returns>
    public static View? GetContentView(this Activity self)
    {
        ArgumentNullException.ThrowIfNull(self);

        if (self.FindViewById(global::Android.Resource.Id.Content) is ViewGroup vg)
            return vg.GetChildAt(0);
        else
            return null;
    }


    /// <summary>
    /// Enters immersive mode, hiding the system bars until the user swipes from the edge of the screen.
    /// </summary>
    /// <param name="self">Activity</param>
    public static void EnterImmersiveMode(this AppCompatActivity self)
    {
        ArgumentNullException.ThrowIfNull(self);

        self.Window!.EnterImmersiveMode();
    }


    /// <summary>
    /// Gets the display that the activity is shown on.
    /// </summary>
    /// <param name="self">Activity</param>
    /// <returns>The display, or null if the activity is not associated with a display</returns>
    public static Display? GetDisplay(this AppCompatActivity self)
    {
        if (OperatingSystem.IsAndroidVersionAtLeast(30))
        {
            // For API level 30 and above
            return self.Display;
        }
        else
        {
            // For API levels below 30
            return self.WindowManager!.DefaultDisplay;
        }
    }


    /// <summary>
    /// Gets the size of the display that the activity is shown on.
    /// </summary>
    /// <param name="self">Activity</param>
    /// <returns>Display size, in pixels</returns>
    /// <exception cref="InvalidOperationException">The activity is not associated with a display.</exception>
    public static Size GetDisplaySize(this AppCompatActivity self)
    {
        var displayMetrics = new DisplayMetrics();
#pragma warning disable CA1422 
        var display = self.GetDisplay() ?? throw new InvalidOperationException("The activity is not associated with a display.");
        display.GetMetrics(displayMetrics);  // Obsoleted on 30 and later
#pragma warning restore CA1422
        return new Size(displayMetrics.WidthPixels, displayMetrics.HeightPixels);
    }


    /// <summary>
    /// Determines whether the app is probably running on a tablet, taken to be a device with a screen diagonal of
    /// 6.5 inches or more.
    /// </summary>
    /// <param name="self">Activity</param>
    /// <returns>True if the device is probably a tablet, otherwise false</returns>
    public static bool IsTablet(this AppCompatActivity self)
    {
        var metrics = self.Resources!.DisplayMetrics!;
        var widthInches = metrics.WidthPixels / metrics.Xdpi;
        var heightInches = metrics.HeightPixels / metrics.Ydpi;
        var diagonalInches = Math.Sqrt(Math.Pow(widthInches, 2) + Math.Pow(heightInches, 2));
        return diagonalInches >= 6.5;
    }
}
