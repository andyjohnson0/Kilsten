using Android.Content;
using Android.Content.Res;
using Android.Util;
using Android.Views;


namespace Kilsten.WearOS.Controls;

/// <summary>
/// Dialog shown while a task runs, with a title, a progress indicator and an optional cancel button. Create one with
/// <see cref="Builder"/>, and dismiss it when the task finishes.
/// </summary>
public class WearableTaskDialog : AndroidX.AppCompat.App.AppCompatDialog
{
    /// <summary>
    /// Initialises a new <see cref="WearableTaskDialog"/>.
    /// </summary>
    /// <param name="context">Activity</param>
    public WearableTaskDialog(Activity context)
        : base(context)
    {
        this.context = context;
    }

    /// <summary>
    /// Initialises a new <see cref="WearableTaskDialog"/>.
    /// </summary>
    /// <param name="context">Activity</param>
    /// <param name="themeResId">Theme resource ID</param>
    public WearableTaskDialog(Activity context, int themeResId)
        : base(context, themeResId)
    {
        this.context = context;
    }


    private readonly Activity context;

    private System.Threading.Timer? cancelButtonVisibleTimer = null;
    private EventHandler<DialogClickEventArgs>? cancelHandler = null;


    private void StartCancelButtonVisibleTimer(int durationSecs)
    {
        cancelButtonVisibleTimer = new System.Threading.Timer((o) =>
        {
            this.context.RunOnUiThread(() =>
            {
                this.FindViewById<ImageButton>(Resource.Id._kilsten_wear_cancelBtn)!.Visibility = ViewStates.Visible;
            });
        },
        null, TimeSpan.FromSeconds(durationSecs), TimeSpan.Zero);
    }


    private void StopCancelButtonVisibleTimer()
    {
        if (cancelButtonVisibleTimer != null)
        {
            cancelButtonVisibleTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
            cancelButtonVisibleTimer.Dispose();
            cancelButtonVisibleTimer = null;
        }
    }

    /// <inheritdoc/>
    protected override void OnStop()
    {
        StopCancelButtonVisibleTimer();
        base.OnStop();
    }

    /// <summary>
    /// Cancels the dialog, for example when the back button is pressed, and calls the cancel button's handler.
    /// </summary>
    public override void Cancel()
    {
        base.Cancel();
        cancelHandler?.Invoke(this, new DialogClickEventArgs((int)DialogButtonType.Negative));
    }


    /// <summary>
    /// Builds a <see cref="WearableTaskDialog"/>.
    /// </summary>
    public class Builder
    {
        /// <summary>
        /// Initialises a new <see cref="WearableTaskDialog.Builder"/>.
        /// </summary>
        /// <param name="context">Activity</param>
        public Builder(Activity context)
        {
            this.context = context;
            this.themeResId = 0;
        }

        /// <summary>
        /// Initialises a new <see cref="WearableTaskDialog.Builder"/>.
        /// </summary>
        /// <param name="context">Activity</param>
        /// <param name="themeResId">Theme resource ID</param>
        public Builder(Activity context, int themeResId)
        {
            this.context = context;
            this.themeResId = themeResId;
        }

        private readonly Activity context;
        private readonly int themeResId;

        private int titleStringResId = 0;
        private string? title = null;
        private bool cancelButtonEnabled = false;
        private int cancelButtonDrawableResId = 0;
        private EventHandler<DialogClickEventArgs>? cancelButtonHandler = null;
        private int progressBarTint = -1;  // colour as int
        private int cancelButtonForegroundTint = -1;  // colour as int
        private int cancelButtonBackgroundTint = -1;  // colour as int
        private int cancelButtonVisibleDelaySecs = 0;
        private bool keepScreenOn = false;


        /// <summary>
        /// Creates the dialog. Call <c>Show()</c> on the result to display it.
        /// </summary>
        /// <returns>The dialog</returns>
        public WearableTaskDialog Create()
        {
            var dlg = (this.themeResId != 0) ? new WearableTaskDialog(this.context, this.themeResId) : new WearableTaskDialog(this.context);
            dlg.RequestWindowFeature((int)WindowFeatures.NoTitle);
            dlg.SetContentView(Resource.Layout._kilsten_wear_wearable_task_dialog);
            // Title
            dlg.FindViewById<TextView>(Resource.Id._kilsten_wear_titleTv)!.SetTextIfSpecified(this.titleStringResId, this.title);

            // Pregress bar
            var pb = dlg.FindViewById<ProgressBar>(Resource.Id._kilsten_wear_progressPb)!;
            var displayMetrics = new DisplayMetrics();
#pragma warning disable CA1422 // Validate platform compatibility
            this.context.WindowManager!.DefaultDisplay!.GetMetrics(displayMetrics);
#pragma warning restore CA1422 // Validate platform compatibility
            var h = (int)(displayMetrics.HeightPixels * 0.50);
            var w = (int)(displayMetrics.WidthPixels * 0.50);
            pb.LayoutParameters = new FrameLayout.LayoutParams(w, h, GravityFlags.CenterHorizontal | GravityFlags.CenterVertical);
            if (this.progressBarTint != -1)
            {
                var colour = new global::Android.Graphics.Color(this.progressBarTint);
                pb.IndeterminateTintList= ColorStateList.ValueOf(colour);
            }

            // Cancel button
            var btn = dlg.FindViewById<ImageButton>(Resource.Id._kilsten_wear_cancelBtn)!;
            btn.Visibility = cancelButtonEnabled && cancelButtonVisibleDelaySecs == 0 ? ViewStates.Visible : ViewStates.Invisible;
            if (this.cancelButtonDrawableResId != 0)
            {
                btn.SetImageResource(this.cancelButtonDrawableResId);
            }
            if (this.cancelButtonForegroundTint != -1)
            {
                var colour = new global::Android.Graphics.Color(this.cancelButtonForegroundTint);
                btn.SetColorFilter(colour);
            }
            if (this.cancelButtonBackgroundTint != -1)
            {
                var colour = new global::Android.Graphics.Color(this.cancelButtonBackgroundTint);
                btn.BackgroundTintList = ColorStateList.ValueOf(colour);
            }

            dlg.cancelHandler = this.cancelButtonHandler;
            btn.Click += (o, e) =>
            {
                dlg.StopCancelButtonVisibleTimer();
                dlg.Dismiss();
                dlg.cancelHandler?.Invoke(dlg, new DialogClickEventArgs((int)DialogButtonType.Negative));
            };
            if (cancelButtonVisibleDelaySecs != 0)
            {
                dlg.StartCancelButtonVisibleTimer(cancelButtonVisibleDelaySecs);
            }

            // Keep screen on.
            if (this.keepScreenOn)
            {
                dlg.Window!.AddFlags(WindowManagerFlags.KeepScreenOn);
            }

            return dlg;
        }


        /// <summary>
        /// Sets the dialog title.
        /// </summary>
        /// <param name="titleStringResId">Title string resource ID</param>
        /// <returns>This builder, so that calls can be chained</returns>
        public Builder SetTitle(int titleStringResId)
        {
            this.titleStringResId = titleStringResId;
            return this;
        }

        /// <summary>
        /// Sets the dialog title.
        /// </summary>
        /// <param name="title">Title</param>
        /// <returns>This builder, so that calls can be chained</returns>
        public Builder SetTitle(string title)
        {
            this.title = title;
            return this;
        }


        /// <summary>
        /// Sets the colour of the progress indicator.
        /// </summary>
        /// <param name="colour">Colour (ARGB)</param>
        /// <returns>This builder, so that calls can be chained</returns>
        public Builder SetProgressBarTint(
            int colour)
        {
            this.progressBarTint = colour;
            return this;
        }


        /// <summary>
        /// Shows a cancel button, which dismisses the dialog and then calls the handler. The handler is also called if
        /// the dialog is cancelled in another way, such as with the back button.
        /// </summary>
        /// <param name="drawableResId">Drawable resource ID for the button image, or 0 for the default</param>
        /// <param name="handler">Click handler, or null for none</param>
        /// <param name="delaySecs">Delay, in seconds, before the button appears, or 0 to show it straight away</param>
        /// <returns>This builder, so that calls can be chained</returns>
        public Builder SetCancelButton(
            int drawableResId = 0,
            EventHandler<DialogClickEventArgs>? handler = null,
            int delaySecs = 0)
        {
            this.cancelButtonEnabled = true;
            this.cancelButtonDrawableResId = drawableResId;
            this.cancelButtonHandler = handler;
            this.cancelButtonVisibleDelaySecs = delaySecs;
            return this;
        }


        /// <summary>
        /// Sets the colour of the cancel button image.
        /// </summary>
        /// <param name="colour">Colour (ARGB)</param>
        /// <returns>This builder, so that calls can be chained</returns>
        public Builder SetCancelButtonForegroundTint(
            int colour)
        {
            this.cancelButtonForegroundTint = colour;
            return this;
        }

        /// <summary>
        /// Sets the colour of the cancel button background.
        /// </summary>
        /// <param name="colour">Colour (ARGB)</param>
        /// <returns>This builder, so that calls can be chained</returns>
        public Builder SetCancelButtonBackgroundTint(
            int colour)
        {
            this.cancelButtonBackgroundTint = colour;
            return this;
        }


        /// <summary>
        /// Sets whether the screen is kept on while the dialog is shown. Default is false.
        /// </summary>
        /// <param name="keepScreenOn">True to keep the screen on</param>
        /// <returns>This builder, so that calls can be chained</returns>
        public Builder SetKeepScreenOn(bool keepScreenOn)
        {
            this.keepScreenOn = keepScreenOn;
            return this;
        }
    }
}
