using Android.Content;


namespace Kilsten.Android;

/// <summary>
/// Extension methods for <see cref="ISharedPreferences"/> and <see cref="ISharedPreferencesEditor"/>, for storing
/// <see cref="DateTime"/> and <see cref="TimeSpan"/> values.
/// </summary>
public static class SharedPreferencesExt
{
    /// <summary>
    /// Gets a <see cref="DateTime"/> value stored by <see cref="PutDateTime"/>.
    /// </summary>
    /// <param name="self">Preferences</param>
    /// <param name="key">Preference name</param>
    /// <param name="defValue">Value to return if the preference does not exist</param>
    /// <returns>The preference value if it exists, otherwise <paramref name="defValue"/></returns>
    public static DateTime GetDateTime(
        this ISharedPreferences self, 
        string key, 
        DateTime defValue)
    {
        if (self.Contains(key) && self.Contains(key + "_kind"))
        {
            var mSec = self.GetLong(key, 0L);
            var dt = DateTime.UnixEpoch.AddMilliseconds(mSec);  // UTC as serialised
            var kind = (DateTimeKind)self.GetInt(key + "_kind", 0);
            if (kind != DateTimeKind.Utc)
                dt = DateTime.SpecifyKind(dt.ToLocalTime(), kind);
            return dt;
        }
        else
        {
            return defValue;
        }
    }


    /// <summary>
    /// Sets a <see cref="DateTime"/> value in the editor, to be written when <c>Commit()</c> or <c>Apply()</c> is called.
    /// </summary>
    /// <param name="self">Preferences editor</param>
    /// <param name="key">Preference name</param>
    /// <param name="value">Value to set</param>
    /// <returns>The same editor, so that calls can be chained</returns>
    public static ISharedPreferencesEditor PutDateTime(
        this ISharedPreferencesEditor self, 
        string key, 
        DateTime value)
    {
        var mSec = ((DateTimeOffset)value.ToUniversalTime()).ToUnixTimeMilliseconds();
        self.PutLong(key, mSec);
        self.PutInt(key + "_kind", (int)value.Kind);
        return self;
    }


    /// <summary>
    /// Gets a <see cref="TimeSpan"/> value stored by <see cref="PutTimeSpan"/>.
    /// </summary>
    /// <param name="self">Preferences</param>
    /// <param name="key">Preference name</param>
    /// <param name="defValue">Value to return if the preference does not exist</param>
    /// <returns>The preference value if it exists, otherwise <paramref name="defValue"/></returns>
    public static TimeSpan GetTimeSpan(
        this ISharedPreferences self, 
        string key, 
        TimeSpan defValue)
    {
        if (self.Contains(key + "_whole") && self.Contains(key + "_fract"))
        {
            var mSec1 = self.GetLong(key + "_whole", 0L);
            var mSec2 = self.GetFloat(key + "_fract", 0F);
            var ts = TimeSpan.FromMilliseconds((double)mSec1 + (double)mSec2);
            return ts;
        }
        else
        {
            return defValue;
        }
    }


    /// <summary>
    /// Sets a <see cref="TimeSpan"/> value in the editor, to be written when <c>Commit()</c> or <c>Apply()</c> is called.
    /// </summary>
    /// <param name="self">Preferences editor</param>
    /// <param name="key">Preference name</param>
    /// <param name="value">Value to set</param>
    /// <returns>The same editor, so that calls can be chained</returns>
    public static ISharedPreferencesEditor PutTimeSpan(
        this ISharedPreferencesEditor self, 
        string key, 
        TimeSpan value)
    {
        var mSec1 = (long)value.TotalMilliseconds;            // Whole part
        var mSec2 = (float)(value.TotalMilliseconds - mSec1); // Fractional part
        self.PutLong(key + "_whole", mSec1);
        self.PutFloat(key + "_fract", mSec2);
        return self;
    }
}
