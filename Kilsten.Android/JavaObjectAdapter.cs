namespace Kilsten.Android;

/// <summary>
/// Wraps a .NET object so that it can be passed where a <see cref="Java.Lang.Object"/> is required, such as
/// <see cref="global::Android.Views.View.Tag"/>.
/// </summary>
/// <typeparam name="T">Wrapped object type</typeparam>
/// <param name="obj">Object to wrap</param>
public class JavaObjectAdapter<T>(T obj) : Java.Lang.Object
{
    /// <summary>
    /// The wrapped object.
    /// </summary>
    public T Value { get; private set; } = obj;
}
