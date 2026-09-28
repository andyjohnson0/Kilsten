using Android.Content;
using Android.Content.Res;
using Android.Views;
using AndroidX.Core.Content.Resources;


namespace Kilsten.WearOS.Controls;

/// <summary>
/// Alert dialog with a title, message, and round Wear OS style image buttons. Create one with <see cref="Builder"/>.
/// </summary>
public class WearableAlertDialog : AndroidX.AppCompat.App.AppCompatDialog
{
    /// <summary>
    /// Initialises a new <see cref="WearableAlertDialog"/>.
    /// </summary>
    /// <param name="context">Context</param>
    public WearableAlertDialog(Context context)
        : base(context)
    {
    }

    /// <summary>
    /// Initialises a new <see cref="WearableAlertDialog"/>.
    /// </summary>
    /// <param name="context">Context</param>
    /// <param name="themeResId">Theme resource ID</param>
    public WearableAlertDialog(Context context, int themeResId)
        : base(context, themeResId)
    {
    }


    /// <summary>
    /// Builds a <see cref="WearableAlertDialog"/>.
    /// </summary>
    public class Builder
    {
        /// <summary>
        /// Initialises a new <see cref="WearableAlertDialog.Builder"/>.
        /// </summary>
        /// <param name="context">Context</param>
        public Builder(Context context)
        {
            this.context = context;
            this.themeResId = 0;
        }

        /// <summary>
        /// Initialises a new <see cref="WearableAlertDialog.Builder"/>.
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="themeResId">Theme resource ID</param>
        public Builder(Context context, int themeResId)
        {
            this.context = context;
            this.themeResId = themeResId;
        }

        private readonly Context context;
        private readonly int themeResId;


        private bool cancellable = true;
        private int titleStringResId = 0;
        private string? title = null;
        private int messageStringResId = 0;
        private string? message = null;
        private ButtonOptions? positiveButton = null;
        private ButtonOptions? negativeButton = null;

        private sealed record ButtonOptions(
            int StringResId,
            int DrawableResId,
            int BackgroundColourResId,
            EventHandler<DialogClickEventArgs>? Handler);


        /// <summary>
        /// Creates the dialog. Call <c>Show()</c> on the result to display it.
        /// </summary>
        /// <returns>The dialog</returns>
        public WearableAlertDialog Create()
        {
            var dlg = (this.themeResId != 0) ? new WearableAlertDialog(this.context, this.themeResId) : new WearableAlertDialog(this.context);

            dlg.RequestWindowFeature((int)WindowFeatures.NoTitle);
            if (this.cancellable)
                dlg.RequestWindowFeature((int)WindowFeatures.SwipeToDismiss);
            dlg.SetCancelable(this.cancellable);
            dlg.SetContentView(Resource.Layout._kilsten_wear_wearable_alert_dialog);

            // Title
            dlg.FindViewById<TextView>(Resource.Id._kilsten_wear_titleTv)!.SetTextIfSpecified(this.titleStringResId, this.title);

            // Message
            dlg.FindViewById<TextView>(Resource.Id._kilsten_wear_messageTv)!.SetTextIfSpecified(this.messageStringResId, this.message);

            // **** DEBUGGING NOTE ****
            // If you get a resource not found exception in the call to SetImageResource() in ConfigureButton()
            // then you've passed a string resource instead of a drawable resource to SetPositiveButton().
            // Similarly for the resources.GetString() call where you may have passed a drawable resource
            // for the string resource parameter.
            // Its best to called Builder.SetPositiveButton() and builder.SetNegativeButton() using named
            // arguments to guard against this.

            // Positive button
            ConfigureButton(dlg, Resource.Id._kilsten_wear_positiveBtn, this.positiveButton, DialogButtonType.Positive);

            // Negative button
            ConfigureButton(dlg, Resource.Id._kilsten_wear_negativeBtn, this.negativeButton, DialogButtonType.Negative);

            return dlg;
        }


        private void ConfigureButton(
            WearableAlertDialog dlg,
            int buttonResId,
            ButtonOptions? options,
            DialogButtonType buttonType)
        {
            var btn = dlg.FindViewById<ImageButton>(buttonResId)!;
            btn.Visibility = options != null ? ViewStates.Visible : ViewStates.Gone;
            if (options == null)
                return;

            if (options.StringResId != 0)
            {
                var str = this.context.Resources!.GetString(options.StringResId);
                btn.ContentDescription = str;
                if (OperatingSystem.IsAndroidVersionAtLeast(26))
                    btn.TooltipText = str;
            }
            if (options.DrawableResId != 0)
            {
                btn.SetImageResource(options.DrawableResId);
            }
            if (options.BackgroundColourResId != 0)
            {
                var tintColour = new global::Android.Graphics.Color(ResourcesCompat.GetColor(context.Resources!, options.BackgroundColourResId, context.Theme));
                btn.BackgroundTintList = ColorStateList.ValueOf(tintColour);
            }
            btn.Click += (o, e) =>
            {
                dlg.Dismiss();
                options.Handler?.Invoke(o, new DialogClickEventArgs((int)buttonType));
            };
        }


        /// <summary>
        /// Sets whether the dialog can be cancelled by swiping or pressing back. Default is true.
        /// </summary>
        /// <param name="cancellable">True if the dialog can be cancelled</param>
        /// <returns>This builder, so that calls can be chained</returns>
        public Builder SetCancelable(bool cancellable)
        {
            this.cancellable = cancellable;
            return this;
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
        /// Sets the dialog message.
        /// </summary>
        /// <param name="messageStringResId">Message string resource ID</param>
        /// <returns>This builder, so that calls can be chained</returns>
        public Builder SetMessage(int messageStringResId)
        {
            this.messageStringResId = messageStringResId;
            return this;
        }

        /// <summary>
        /// Sets the dialog message.
        /// </summary>
        /// <param name="message">Message</param>
        /// <returns>This builder, so that calls can be chained</returns>
        public Builder SetMessage(string message)
        {
            this.message = message;
            return this;
        }


        /// <summary>
        /// Shows the positive (confirm) button, which dismisses the dialog and then calls the handler.
        /// All parameters are optional, and the resource IDs are easy to mix up, so call this using named arguments.
        /// </summary>
        /// <param name="stringResId">String resource ID for the button's description and tooltip, or 0 for the default</param>
        /// <param name="drawableResId">Drawable resource ID for the button image, or 0 for the default</param>
        /// <param name="backgroundColourResId">Colour resource ID for the button background, or 0 for the default</param>
        /// <param name="handler">Click handler, or null for none</param>
        /// <returns>This builder, so that calls can be chained</returns>
        public Builder SetPositiveButton(
            int stringResId = 0,
            int drawableResId = 0,
            int backgroundColourResId = 0,
            EventHandler<DialogClickEventArgs>? handler = null)
        {
            this.positiveButton = new ButtonOptions(stringResId, drawableResId, backgroundColourResId, handler);
            return this;
        }


        /// <summary>
        /// Shows the negative (cancel) button, which dismisses the dialog and then calls the handler.
        /// All parameters are optional, and the resource IDs are easy to mix up, so call this using named arguments.
        /// </summary>
        /// <param name="stringResId">String resource ID for the button's description and tooltip, or 0 for the default</param>
        /// <param name="drawableResId">Drawable resource ID for the button image, or 0 for the default</param>
        /// <param name="backgroundColourResId">Colour resource ID for the button background, or 0 for the default</param>
        /// <param name="handler">Click handler, or null for none</param>
        /// <returns>This builder, so that calls can be chained</returns>
        public Builder SetNegativeButton(
            int stringResId = 0,
            int drawableResId = 0, 
            int backgroundColourResId = 0,
            EventHandler<DialogClickEventArgs>? handler = null)
        {
            this.negativeButton = new ButtonOptions(stringResId, drawableResId, backgroundColourResId, handler);
            return this;
        }
    }
}
