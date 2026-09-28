using System.Globalization;

using Android.Content;
using Android.Text.Style;
using Android.Text;


namespace Kilsten.Android;

/// <summary>
/// Extension methods for <see cref="TextView"/>.
/// </summary>
public static class TextViewExt
{
    /// <summary>
    /// Sets text containing placeholders <c>{0}</c>, <c>{1}</c>, etc, and replaces each placeholder with the
    /// corresponding drawable, tinted to match the text colour.
    /// </summary>
    /// <param name="self">Text view</param>
    /// <param name="context">Context used to load the drawables</param>
    /// <param name="formatStr">Text containing placeholders</param>
    /// <param name="drawableIds">Drawable resource IDs, one for each placeholder</param>
    public static void SetTextWithDrawables(
        this TextView self,
        Context context,
        string formatStr,
        params int[] drawableIds)
    {
        ArgumentNullException.ThrowIfNull(self);

        self.Text = formatStr;
        for(int i = 0; i < drawableIds.Length; i++)
        {
            var placeholder = "{" + i.ToString(CultureInfo.InvariantCulture) + "}";
            var drawable = context.GetDrawable(drawableIds[i]);
            if (drawable != null)
            {
                drawable.SetTint(self.CurrentTextColor);
                drawable.SetBounds(0, 0, drawable.IntrinsicWidth, drawable.IntrinsicHeight);
                var imageSpan = new ImageSpan(drawable, SpanAlign.Bottom);
                var idx = self.Text.IndexOf(placeholder);
                if (idx != -1)
                {
                    var span = new SpannableString(self.TextFormatted);
                    span.SetSpan(imageSpan, idx, idx + placeholder.Length, SpanTypes.InclusiveInclusive);
                    self.SetText(span, TextView.BufferType.Spannable);
                }
            }
        }
    }

}
