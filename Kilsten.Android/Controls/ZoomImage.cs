using Android.Content;
using Android.Util;


namespace Kilsten.Android.Controls;

/// <summary>
/// <see cref="ImageView"/> for use as the child of a <see cref="ZoomLayout"/>, raising <see cref="LaidOut"/> after
/// each layout pass.
/// </summary>
public class ZoomImage : ImageView
{
    /// <summary>
    /// Initialises a new <see cref="ZoomImage"/>.
    /// </summary>
    /// <param name="context">Context</param>
    public ZoomImage(Context context)
        : base(context)
    {
    }

    /// <summary>
    /// Initialises a new <see cref="ZoomImage"/>.
    /// </summary>
    /// <param name="context">Context</param>
    /// <param name="attrs">Attributes</param>
    public ZoomImage(Context context, IAttributeSet attrs)
        : base(context, attrs)
    {
    }

    /// <summary>
    /// Initialises a new <see cref="ZoomImage"/>.
    /// </summary>
    /// <param name="context">Context</param>
    /// <param name="attrs">Attributes</param>
    /// <param name="defStyle">Default style</param>
    public ZoomImage(Context context, IAttributeSet attrs, int defStyle)
        : base(context, attrs, defStyle)
    {
    }

    /// <summary>
    /// Initialises a new <see cref="ZoomImage"/>.
    /// </summary>
    /// <param name="context">Context</param>
    /// <param name="attrs">Attributes</param>
    /// <param name="defStyle">Default style</param>
    /// <param name="defStyleRes">Default style resource ID</param>
    public ZoomImage(Context context, IAttributeSet attrs, int defStyle, int defStyleRes)
        : base(context, attrs, defStyle, defStyleRes)
    {
    }


    /// <summary>
    /// Raised after the view has been laid out.
    /// </summary>
    public event EventHandler? LaidOut;

    /// <inheritdoc/>
    protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
    {
        base.OnLayout(changed, left, top, right, bottom);

        this.LaidOut?.Invoke(this, EventArgs.Empty);
    }
}
