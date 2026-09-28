using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Util;


namespace Kilsten.WearOS.Controls;

/// <summary>
/// Image button that greys out its image while it is disabled.
/// </summary>
public class WearableButton : AndroidX.AppCompat.Widget.AppCompatImageButton
{
    /// <summary>
    /// Initialises a new <see cref="WearableButton"/>.
    /// </summary>
    /// <param name="context">Context</param>
    public WearableButton(Context context)
        : base(context)
    {
        CaptureEnabledTint();
    }


    /// <summary>
    /// Initialises a new <see cref="WearableButton"/>.
    /// </summary>
    /// <param name="context">Context</param>
    /// <param name="attrs">Attributes</param>
    public WearableButton(Context context, IAttributeSet attrs)
        : base(context, attrs)
    {
        CaptureEnabledTint();
    }


    /// <summary>
    /// Initialises a new <see cref="WearableButton"/>.
    /// </summary>
    /// <param name="context">Context</param>
    /// <param name="attrs">Attributes</param>
    /// <param name="defStyle">Default style</param>
    public WearableButton(Context context, IAttributeSet attrs, int defStyle)
        : base(context, attrs, defStyle)
    {
        CaptureEnabledTint();
    }


    private static readonly ColorStateList DisabledTint = ColorStateList.ValueOf(Color.Gray);

    private bool tintCaptured = false;
    private ColorStateList? enabledTint;
    private PorterDuff.Mode? enabledTintMode;


    /// <inheritdoc/>
    public override bool Enabled
    {
        get => base.Enabled;
        set
        {
            base.Enabled = value;
            UpdateImageTint();
        }
    }


    private void CaptureEnabledTint()
    {
        enabledTint = ImageTintList;
        enabledTintMode = ImageTintMode;
        tintCaptured = true;
        UpdateImageTint();
    }


    private void UpdateImageTint()
    {
        // Enabled may be set by the base class constructor, before the button's own tint has been captured.
        if (!tintCaptured)
            return;

        if (Enabled)
        {
            ImageTintList = enabledTint;
            ImageTintMode = enabledTintMode;
        }
        else
        {
            // Tinting with SrcIn keeps the image's shape (its alpha channel) but replaces its colours.
            ImageTintList = DisabledTint;
            ImageTintMode = PorterDuff.Mode.SrcIn;
        }
    }
}
