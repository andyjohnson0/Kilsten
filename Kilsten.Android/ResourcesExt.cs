namespace Kilsten.Android;

/// <summary>
/// Extension methods for <see cref="global::Android.Content.Res.Resources"/>.
/// </summary>
public static class ResourcesExt
{
    /// <summary>
    /// Formats a string as a question, using the localised question format.
    /// </summary>
    /// <param name="self">Resources</param>
    /// <param name="str">String to make into a question</param>
    /// <returns>Question string</returns>
    public static string? MakeQuestion(
        this global::Android.Content.Res.Resources self,
        string str)
    {
        ArgumentNullException.ThrowIfNull(self);
        ArgumentNullException.ThrowIfNull(str);

        return self.GetString(Resource.String._kilsten_questionFmt, str!);
    }


    /// <summary>
    /// Formats a string resource as a question, using the localised question format.
    /// </summary>
    /// <param name="self">Resources</param>
    /// <param name="id">String resource ID</param>
    /// <returns>Question string</returns>
    public static string? MakeQuestion(
        this global::Android.Content.Res.Resources self,
        int id)
    {
        ArgumentNullException.ThrowIfNull(self);

        var str = self.GetString(id);
        return self.MakeQuestion(str);
    }


    /// <summary>
    /// Formats a quantity string resource as a question, using the localised question format.
    /// </summary>
    /// <param name="self">Resources</param>
    /// <param name="id">Plurals resource ID</param>
    /// <param name="quantity">Quantity</param>
    /// <returns>Question string</returns>
    public static string? MakeQuestionQuantityString(
        this global::Android.Content.Res.Resources self,
        int id, 
        int quantity)
    {
        ArgumentNullException.ThrowIfNull(self);

        var str = self.GetQuantityString(id, quantity);
        return self.MakeQuestion(str);
    }


    /// <summary>
    /// Formats a quantity string resource as a question, using the localised question format.
    /// </summary>
    /// <param name="self">Resources</param>
    /// <param name="id">Plurals resource ID</param>
    /// <param name="quantity">Quantity</param>
    /// <param name="formatArgs">Format arguments</param>
    /// <returns>Question string</returns>
    public static string? MakeQuestionQuantityString(
        this global::Android.Content.Res.Resources self,
        int id, 
        int quantity,
        params Java.Lang.Object[] formatArgs)
    {
        ArgumentNullException.ThrowIfNull(self);

        var str = self.GetQuantityString(id, quantity, formatArgs);
        return self.MakeQuestion(str);
    }
}
