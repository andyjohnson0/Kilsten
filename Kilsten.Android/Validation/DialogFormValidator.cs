using Android.Content;
using Android.Views;
using AlertDialog = AndroidX.AppCompat.App.AlertDialog;


namespace Kilsten.Android.Validation;

/// <summary>
/// <see cref="FormValidator"/> for a form in an <see cref="AlertDialog"/>. Validates the form when the positive
/// button is clicked, and only dismisses the dialog if it is valid.
/// </summary>
public class DialogFormValidator : FormValidator
{
    /// <summary>
    /// Initialises a new <see cref="DialogFormValidator"/>.
    /// </summary>
    /// <param name="dialog">Dialog containing the form. It must have a positive button.</param>
    /// <param name="dialogFormView">View in the dialog that contains the form elements</param>
    public DialogFormValidator(
        AlertDialog dialog,
        View dialogFormView)
    : base(dialogFormView)
    {
        this.dialog = dialog;
        var positiveButton = dialog.GetButton((int)DialogButtonType.Positive);
        if (positiveButton != null)
            positiveButton.Click += DialogPositiveButton_Click;
        else
            dialog.ShowEvent += Dialog_Show;
    }

    private readonly AlertDialog dialog;


    private void Dialog_Show(object? sender, EventArgs e)
    {
        dialog.ShowEvent -= Dialog_Show;
        dialog.GetButton((int)DialogButtonType.Positive)!.Click += DialogPositiveButton_Click;
    }



    /// <summary>
    /// Creates and shows a dialog from a builder, and attaches a validator to it.
    /// </summary>
    /// <param name="adb">Dialog builder. The dialog must have a positive button.</param>
    /// <param name="dialogFormView">View in the dialog that contains the form elements</param>
    /// <returns>The validator</returns>
    public static DialogFormValidator CreateFrom(
        AlertDialog.Builder adb,
        View dialogFormView)
    {
        var dialog = adb.Create()!;
        dialog.Show();
        return new DialogFormValidator(dialog, dialogFormView);
    }


    /// <summary>
    /// The dialog being validated.
    /// </summary>
    public AlertDialog Dialog => dialog;


    /// <summary>
    /// Whether to show error messages on invalid elements when the positive button is clicked. Default is true.
    /// </summary>
    public bool ShowFeedback { get; set; } = true;


    /// <summary>
    /// Raised when the positive button is clicked and the form is valid, just before the dialog is dismissed. Use it
    /// to read the validated values.
    /// </summary>
    public event EventHandler? Valid;


    /// <summary>
    /// Raised when the positive button is clicked and the form is not valid. The dialog stays open.
    /// </summary>
    public event EventHandler? Invalid;


    private void DialogPositiveButton_Click(object? sender, EventArgs e)
    {
        if (this.ValidateAll(this.ShowFeedback))
        {
            this.Valid?.Invoke(this, EventArgs.Empty);
            dialog.Dismiss();
        }
        else
        {
            this.Invalid?.Invoke(this, EventArgs.Empty);
        }
    }
}
