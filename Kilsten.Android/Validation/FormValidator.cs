using System.Globalization;
using System.Text;

using Android.Content;
using Android.Views;


namespace Kilsten.Android.Validation;

/// <summary>
/// Validates the <see cref="EditText"/> elements in a view against length, numeric range and custom rules, showing
/// error messages on invalid elements.
/// </summary>
public class FormValidator
{
    /// <summary>
    /// Initialises a new <see cref="FormValidator"/>.
    /// </summary>
    /// <param name="formView">View containing the form elements</param>
    public FormValidator(
        View formView)
    {
        this.formView = formView;
    }

    private readonly View formView;
    private readonly List<ValidationContext> validationContexts = [];


    /// <summary>
    /// Validates all form elements that have rules.
    /// </summary>
    /// <param name="showFeedback">True to show error messages on invalid elements, false to validate silently</param>
    /// <returns>True if the form is valid, otherwise false</returns>
    public bool ValidateAll(bool showFeedback = true)
    {
        var isValid = true;

        foreach (var valCtx in validationContexts)
        {
            if (!this.Validate(valCtx.EditTextResId, showFeedback))
                isValid = false;
        }

        return isValid;
    }



    /// <summary>
    /// Validates a single form element. An element with no rules is valid.
    /// </summary>
    /// <param name="editTextResId">ID of the EditText element</param>
    /// <param name="showFeedback">True to show error messages on invalid elements, false to validate silently</param>
    /// <returns>True if the form element is valid, otherwise false</returns>
    public bool Validate(
        int editTextResId,
        bool showFeedback = true)
    {
        var isValid = true;

        var i = validationContexts.FindIndex(vi => vi.EditTextResId == editTextResId);
        if (i == -1)
        {
            // No context for this edittext so we assume it is valid.
            return isValid;
        }

        var valCtx = validationContexts[i];
        var editText = formView.FindViewById<EditText>(valCtx.EditTextResId);
        if (editText == null)
            return false;
        var str = editText.Text!.Trim();
        var messages = new List<string>();

        // Value check
        var v = valCtx.GetValueValid(str);
        if (!v.isValid)
        {
            messages.Add(v.message);
        }

        // Length check
        if (!valCtx.GetLengthValid(str))
        {
            var message = valCtx.GetLengthCriteria(this.formView.Context!);
            if (message != null)
                messages.Add(message);
        }

        // Numeric value check
        if (!valCtx.GetNumericValueValid(str))
        {
            var message = valCtx.GetNumericValueCriteria(this.formView.Context!);
            if (message != null)
                messages.Add(message);
        }

        //
        if (messages.Count > 0)
            isValid = false;

        // 
        if (showFeedback)
        {
            if (messages.Count > 0)
            {
                // Show error
                var sb = new StringBuilder();
                messages.ForEach(m => sb.AppendLine(m));
                editText.Error = sb.ToString();
            }
            else
            {
                // Clear error
                editText.SetError((string?)null, null);
            }
        }

        return isValid;
    }


    /// <summary>
    /// Sets a custom validation rule.
    /// </summary>
    /// <param name="editTextResId">ID of the EditText element</param>
    /// <param name="validator">
    /// Function that takes the element's trimmed text and returns whether it is valid, and an error message if not
    /// </param>
    /// <returns>This validator, so that calls can be chained</returns>
    public FormValidator SetValueValidator(
        int editTextResId,
        Func<string, (bool, string)> validator)
    {
        var valCtx = GetOrCreateValidationContext(editTextResId);
        valCtx.ValueValidator = validator;
        return this;
    }


    #region Length validation

    /// <summary>
    /// Sets the minimum length rule.
    /// </summary>
    /// <param name="editTextResId">ID of the EditText element</param>
    /// <param name="value">Minimum length</param>
    /// <returns>This validator, so that calls can be chained</returns>
    public FormValidator SetLengthMin(
        int editTextResId,
        int value)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value);

        var valCtx = GetOrCreateValidationContext(editTextResId);
        valCtx.LengthMin = value;
        return this;
    }


    /// <summary>
    /// Sets the maximum length rule.
    /// </summary>
    /// <param name="editTextResId">ID of the EditText element</param>
    /// <param name="value">Maximum length</param>
    /// <returns>This validator, so that calls can be chained</returns>
    public FormValidator SetLengthMax(
        int editTextResId,
        int value)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value);

        var valCtx = GetOrCreateValidationContext(editTextResId);
        valCtx.LengthMax = value;
        return this;
    }


    /// <summary>
    /// Sets an exact length rule.
    /// </summary>
    /// <param name="editTextResId">ID of the EditText element</param>
    /// <param name="value">Required length</param>
    /// <returns>This validator, so that calls can be chained</returns>
    public FormValidator SetLengthFixed(
        int editTextResId,
        int value)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value);

        var valCtx = GetOrCreateValidationContext(editTextResId);
        valCtx.LengthMax = valCtx.LengthMin = value;
        return this;
    }


    /// <summary>
    /// Sets minimum and maximum length rules.
    /// </summary>
    /// <param name="editTextResId">ID of the EditText element</param>
    /// <param name="valueMin">Minimum length</param>
    /// <param name="valueMax">Maximum length</param>
    /// <returns>This validator, so that calls can be chained</returns>
    public FormValidator SetLengthRange(
        int editTextResId,
        int valueMin,
        int valueMax)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(valueMin);
        ArgumentOutOfRangeException.ThrowIfNegative(valueMax);
        ArgumentOutOfRangeException.ThrowIfLessThan(valueMax, valueMin);

        var valCtx = GetOrCreateValidationContext(editTextResId);
        valCtx.LengthMin = valueMin;
        valCtx.LengthMax = valueMax;
        return this;
    }

    #endregion Length validation


    #region Numeric value validation

    /// <summary>
    /// Sets a rule that the element must contain a number no less than a minimum.
    /// </summary>
    /// <param name="editTextResId">ID of the EditText element</param>
    /// <param name="value">Minimum value</param>
    /// <param name="required">True if the element must contain a number, or false to also allow it to be empty. Applies to
    /// all numeric rules for the element.</param>
    /// <returns>This validator, so that calls can be chained</returns>
    public FormValidator SetNumValueMin(
        int editTextResId,
        float value,
        bool required = true)
    {
        var valCtx = GetOrCreateValidationContext(editTextResId);
        valCtx.NumValueMin = value;
        valCtx.NumValueRequired = required;
        return this;
    }


    /// <summary>
    /// Sets a rule that the element must contain a number no greater than a maximum.
    /// </summary>
    /// <param name="editTextResId">ID of the EditText element</param>
    /// <param name="value">Maximum value</param>
    /// <param name="required">True if the element must contain a number, or false to also allow it to be empty. Applies to
    /// all numeric rules for the element.</param>
    /// <returns>This validator, so that calls can be chained</returns>
    public FormValidator SetNumValueMax(
        int editTextResId,
        float value,
        bool required = true)
    {
        var valCtx = GetOrCreateValidationContext(editTextResId);
        valCtx.NumValueMax = value;
        valCtx.NumValueRequired = required;
        return this;
    }


    /// <summary>
    /// Sets a rule that the element must contain a number within a range, inclusive.
    /// </summary>
    /// <param name="editTextResId">ID of the EditText element</param>
    /// <param name="valueMin">Minimum value</param>
    /// <param name="valueMax">Maximum value</param>
    /// <param name="required">True if the element must contain a number, or false to also allow it to be empty. Applies to
    /// all numeric rules for the element.</param>
    /// <returns>This validator, so that calls can be chained</returns>
    public FormValidator SetNumValueRange(
        int editTextResId,
        float valueMin,
        float valueMax,
        bool required = true)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(valueMax, valueMin);

        var valCtx = GetOrCreateValidationContext(editTextResId);
        valCtx.NumValueMin = valueMin;
        valCtx.NumValueMax = valueMax;
        valCtx.NumValueRequired = required;
        return this;
    }

    #endregion Numeric value validation



    private ValidationContext GetOrCreateValidationContext(int editTextResId)
    {
        var i = validationContexts.FindIndex(vi => vi.EditTextResId == editTextResId);
        if (i != -1)
        {
            return validationContexts[i];
        }
        else
        {
            var valCtx = new ValidationContext(editTextResId);
            validationContexts.Add(valCtx);
            return valCtx;
        }
    }


    private class ValidationContext
    {
        public ValidationContext(int editTextResId)
        {
            this.EditTextResId = editTextResId;
        }

        public int EditTextResId { get; private set; } = 0;


        public Func<string, (bool valid, string message)>? ValueValidator { get; set; } = null;

        public (bool isValid, string message) GetValueValid(string str) => this.ValueValidator?.Invoke(str) ?? (true, "");


        #region Length validation

        public int? LengthMin { get; set; } = null;

        public int? LengthMax { get; set; } = null;

        public bool GetLengthValid(string str)
        {
            var tooShort = this.LengthMin.HasValue && str.Length < this.LengthMin;
            var tooLong = this.LengthMax.HasValue && str.Length > this.LengthMax;
            return !(tooShort || tooLong);
        }

        public string? GetLengthCriteria(Context context)
        {
            if (this.LengthMin.HasValue && this.LengthMax.HasValue)
            {
                if (this.LengthMin.Value == this.LengthMax.Value)
                {
                    return context.Resources!.GetString(Resource.String._kilsten_validationLengthFixedMsg, 
                                                       this.LengthMin.Value, 
                                                       context.Resources!.GetQuantityString(Resource.Plurals._kilsten_character, this.LengthMin.Value)!);
                }
                else
                {
                    return context.Resources!.GetString(Resource.String._kilsten_validationLengthRangeMsg,  this.LengthMin.Value, this.LengthMax.Value)!;
                }
            }
            if (this.LengthMin.HasValue)
            {
                return context.Resources!.GetString(Resource.String._kilsten_validationLengthMinimumMsg, this.LengthMin.Value);
            }
            else if (this.LengthMax.HasValue)
            {
                return context.Resources!.GetString(Resource.String._kilsten_validationLengthMaximumMsg, this.LengthMax.Value);
            }
            else
            {
                return null;
            }
        }

        #endregion Length validation


        #region Numeric value validation

        public float? NumValueMin { get; set; } = null;

        public float? NumValueMax { get; set; } = null;

        public bool NumValueRequired { get; set; } = true;

        public bool GetNumericValueValid(string str)
        {
            if (str.Length == 0 && !this.NumValueRequired)
                return true;

            var isNumeric = float.TryParse(str, NumberStyles.Float, CultureInfo.CurrentCulture, out var numVal) ||
                            float.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out numVal);
            if (isNumeric)
            {
                // Numeric, so check the range
                var tooSmall = this.NumValueMin.HasValue && numVal < this.NumValueMin;
                var tooLarge = this.NumValueMax.HasValue && numVal > this.NumValueMax;
                return !(tooSmall || tooLarge);
            }
            else
            {
                return false;
            }
        }

        public string? GetNumericValueCriteria(Context context)
        {
            if (this.NumValueMin.HasValue && this.NumValueMax.HasValue)
            {
                return context.Resources!.GetString(Resource.String._kilsten_validationValueRangeMsg, FormatNumber(this.NumValueMin.Value), FormatNumber(this.NumValueMax.Value));
            }
            if (this.NumValueMin.HasValue)
            {
                return context.Resources!.GetString(Resource.String._kilsten_validationValueMinimumMsg, FormatNumber(this.NumValueMin.Value));
            }
            else if (this.NumValueMax.HasValue)
            {
                return context.Resources!.GetString(Resource.String._kilsten_validationValueMaximumMsg, FormatNumber(this.NumValueMax.Value));
            }
            else
            {
                return null;
            }
        }

        private static Java.Lang.String FormatNumber(float value) => new(value.ToString(CultureInfo.CurrentCulture));

        #endregion Numeric value validation
    }
}
