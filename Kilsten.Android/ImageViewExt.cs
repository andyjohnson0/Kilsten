using Android.Graphics;


namespace Kilsten.Android;

/// <summary>
/// Extension methods for <see cref="ImageView"/>.
/// </summary>
public static class ImageViewExt
{
    /// <summary>
    /// Sets the view's bitmap, and runs an action once the view has been updated to show it.
    /// </summary>
    /// <param name="self">Image view</param>
    /// <param name="bitmap">Bitmap to show</param>
    /// <param name="onBitmapLoaded">Action to run once the bitmap is shown</param>
    public static void SetImageBitmap(
        this ImageView self,
        Bitmap bitmap,
        Action onBitmapLoaded)
    {
        self.SetImageBitmap(bitmap);
        self.Post(onBitmapLoaded);
    }
}
