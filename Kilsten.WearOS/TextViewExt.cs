namespace Kilsten.WearOS;

/// <summary>
/// Extension methods for <see cref="TextView"/>.
/// </summary>
internal static class TextViewExt
{
    /// <summary>
    /// Sets the text from a string resource if one is specified, otherwise from a string if one is specified.
    /// </summary>
    /// <param name="self">Text view</param>
    /// <param name="stringResId">String resource ID, or 0 for none</param>
    /// <param name="text">Text, or null for none</param>
    public static void SetTextIfSpecified(this TextView self, int stringResId, string? text)
    {
        if (stringResId != 0)
            self.SetText(stringResId);
        else if (text != null)
            self.Text = text;
    }
}
