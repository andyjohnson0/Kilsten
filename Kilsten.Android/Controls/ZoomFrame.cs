using Android.Content;
using Android.Util;
using Android.Views;


namespace Kilsten.Android.Controls;

/// <summary>
/// Square <see cref="FrameLayout"/>, with its height set to match its width, for use as the child of a
/// <see cref="ZoomLayout"/>.
/// </summary>
public class ZoomFrame : FrameLayout
{
    /// <summary>
    /// Initialises a new <see cref="ZoomFrame"/>.
    /// </summary>
    /// <param name="context">Context</param>
    public ZoomFrame(Context context)
        : base(context)
    {
    }

    /// <summary>
    /// Initialises a new <see cref="ZoomFrame"/>.
    /// </summary>
    /// <param name="context">Context</param>
    /// <param name="attrs">Attributes</param>
    public ZoomFrame(Context context, IAttributeSet attrs)
        : base(context, attrs)
    {
    }

    /// <summary>
    /// Initialises a new <see cref="ZoomFrame"/>.
    /// </summary>
    /// <param name="context">Context</param>
    /// <param name="attrs">Attributes</param>
    /// <param name="defStyle">Default style</param>
    public ZoomFrame(Context context, IAttributeSet attrs, int defStyle)
        : base(context, attrs, defStyle)
    {
    }

    /// <summary>
    /// Initialises a new <see cref="ZoomFrame"/>.
    /// </summary>
    /// <param name="context">Context</param>
    /// <param name="attrs">Attributes</param>
    /// <param name="defStyle">Default style</param>
    /// <param name="defStyleRes">Default style resource ID</param>
    public ZoomFrame(Context context, IAttributeSet attrs, int defStyle, int defStyleRes)
        : base(context, attrs, defStyle, defStyleRes)
    {
    }


    /// <inheritdoc/>
    protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
    {
        // Get the measured width from the width spec
        int width = MeasureSpec.GetSize(widthMeasureSpec);

        // Set the height measure spec to be equal to the width
        int newHeightMeasureSpec = MeasureSpec.MakeMeasureSpec(width, MeasureSpecMode.Exactly);

        // Call the base method with the modified height measure spec
        base.OnMeasure(widthMeasureSpec, newHeightMeasureSpec);

        // Set the measured dimensions
        SetMeasuredDimension(width, width);
    }

    /// <inheritdoc/>
    protected override void OnLayout(bool changed, int l, int t, int r, int b)
    {
        base.OnLayout(changed, l, t, r, b);

        // Ensure the layout params height is set correctly
        if (LayoutParameters != null && LayoutParameters.Height != Width)
        {
            LayoutParameters.Height = Width;
            RequestLayout();
        }
    }
}
