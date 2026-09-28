using Android.Graphics;


namespace Kilsten.Android;

/// <summary>
/// Extension methods for <see cref="Bitmap"/>.
/// </summary>
public static class BitmapExt
{
    /// <summary>
    /// Creates a copy of the bitmap resized to fit within a maximum width and height, preserving its aspect ratio.
    /// </summary>
    /// <param name="self">Bitmap</param>
    /// <param name="maxWidthPx">Maximum width, in pixels</param>
    /// <param name="maxHeightPx">Maximum height, in pixels</param>
    /// <returns>Resized bitmap</returns>
    public static Bitmap Resize(
        this Bitmap self,
        int maxWidthPx,
        int maxHeightPx)
    {
        var ratioX = (double)maxWidthPx / self.Width;
        var ratioY = (double)maxHeightPx / self.Height;
        var ratio = Math.Min(ratioX, ratioY);

        var newWidthPx = Math.Max(1, (int)(self.Width * ratio));
        var newHeightPx = Math.Max(1, (int)(self.Height * ratio));

        return Bitmap.CreateScaledBitmap(self, newWidthPx, newHeightPx, false);
    }
}
