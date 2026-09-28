using Android.Content;
using Android.Util;


namespace Kilsten.Android;

/// <summary>
/// Extension methods for <see cref="Context"/>.
/// </summary>
public static class ContextExt
{
    /// <summary>
    /// Converts a density-independent pixel (dp) value to pixels, rounded to the nearest pixel.
    /// </summary>
    /// <param name="self">Context</param>
    /// <param name="dp">Value, in dp</param>
    /// <returns>Value, in pixels</returns>
    public static float DpToPixel(
        this Context self,
        float dp)
    {
        ArgumentNullException.ThrowIfNull(self);

        var dim = TypedValue.ApplyDimension(ComplexUnitType.Dip, dp, self.Resources!.DisplayMetrics);
        return (float)Math.Round(dim);
    }



    /// <summary>
    /// Converts a scale-independent pixel (sp) value to pixels, rounded to the nearest pixel.
    /// </summary>
    /// <param name="self">Context</param>
    /// <param name="sp">Value, in sp</param>
    /// <returns>Value, in pixels</returns>
    public static float SpToPixel(
        this Context self,
        float sp)
    {
        ArgumentNullException.ThrowIfNull(self);

        var dim = TypedValue.ApplyDimension(ComplexUnitType.Sp, sp, self.Resources!.DisplayMetrics);
        return (float)Math.Round(dim);
    }



    /// <summary>
    /// Determines whether the current Do Not Disturb settings allow alarm sounds and vibration.
    /// </summary>
    /// <param name="self">Context</param>
    /// <returns>Whether alarm sounds are allowed, and whether vibration is allowed</returns>
    public static (bool allowSound, bool allowVibration) GetDndAlarmSettings(
        this Context self)
    {
        ArgumentNullException.ThrowIfNull(self);

        var notificationManager = (NotificationManager)self.GetSystemService(Context.NotificationService)!;
        if (notificationManager.CurrentInterruptionFilter == InterruptionFilter.All)
        {
            // DND is off
            return (allowSound: true, allowVibration: true);
        }
        else if (notificationManager.CurrentInterruptionFilter == InterruptionFilter.Priority)
        {
            // DND is on with priority only
            var zenPolicy = notificationManager.NotificationPolicy!.PriorityCategories;
            bool allowSound = !OperatingSystem.IsAndroidVersionAtLeast(28) || (zenPolicy & NotificationPriorityCategory.Alarms) != 0;  // Could also include Reminders category?
            bool allowVibration = notificationManager.NotificationPolicy.PriorityCallSenders == NotificationPrioritySenders.Any ||
                                    notificationManager.NotificationPolicy.PriorityMessageSenders == NotificationPrioritySenders.Any;
            return (allowSound, allowVibration);
        }
        else if (notificationManager.CurrentInterruptionFilter == InterruptionFilter.Alarms)
        {
            // Only alarms are allowed
            return (allowSound: true, allowVibration: true);
        }
        else if (notificationManager.CurrentInterruptionFilter == InterruptionFilter.None)
        {
            // DND is on with no interruptions allowed
            return (allowSound: false, allowVibration: false);
        }
        else
        {
            // Catch-all
            return (allowSound: false, allowVibration: false);
        }
    }
}
