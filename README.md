# Kilsten

Kilsten is a pair of .NET for Android class libraries: reusable components, helpers and
extension methods for Android™ and Wear OS™ apps written in C#.

- **Kilsten.Android** - views, form validation and extension methods for Android apps.
- **Kilsten.WearOS** - Wear OS style dialogs and buttons, and phone hand-off for watch apps.

The two libraries are independent of each other. A phone app, a watch app, or a phone
app with a companion watch app can reference either or both.

Android and Wear OS are trademarks of Google LLC. This project is not associated with or
endorsed by Google LLC.


## Kilsten.Android

The extension methods and general helpers are in the root `Kilsten.Android` namespace, so a
single `using Kilsten.Android;` makes them all available. Controls and form validation are
in sub-namespaces.


### Extension methods (`Kilsten.Android`)

| Type | Extensions |
| --- | --- |
| `Activity` | Get the content view, enter immersive mode, get the display and its size, detect a tablet |
| `DialogFragment` | Enter immersive mode |
| `Bitmap` | Resize to fit within a maximum width and height, preserving aspect ratio |
| `Canvas` | Draw a line with an open, filled or hollow arrowhead |
| `Context` | Convert dp and sp values to pixels, check whether Do Not Disturb allows alarm sounds and vibration |
| `ImageView` | Set a bitmap and run an action once it has loaded |
| `Resources` | Format a string or quantity string as a question |
| `ISharedPreferences` | Store and retrieve `DateTime` and `TimeSpan` values |
| `TextView` | Set text containing inline drawables, tinted to match the text colour |


### Other helpers (`Kilsten.Android`)

- **JavaObjectAdapter&lt;T&gt;** - wraps a .NET object so that it can be passed where a
  `Java.Lang.Object` is required, such as a view's `Tag`.


### Controls (`Kilsten.Android.Controls`)

- **ZoomLayout** - a `FrameLayout` that provides pinch-zooming and panning of a single
  child view. Raises `SingleTap`, `LongPress`, `Fling` and `ZoomLevelChanged` events, and
  supports `minZoomLevel` and `maxZoomLevel` layout attributes. `ResetZoom()` fits the
  child to the view, and `EnsureVisible()` pans to bring a point in the child into view.
- **ZoomImage** and **ZoomFrame** - an `ImageView` and a square `FrameLayout` suitable for
  use as the child of a `ZoomLayout`.
- **SimplePagerAdapter** - a `ViewPager` adapter that inflates each page from a layout
  resource, with a `PageCreated` event for initialising pages.


### Form validation (`Kilsten.Android.Validation`)

- **FormValidator** - validates the `EditText` elements in a view, with a fluent API for
  minimum, maximum and exact length, numeric ranges (for fields where a number is required or
  optional), and custom validation functions.
  Invalid elements show their error messages inline.
- **DialogFormValidator** - attaches a `FormValidator` to an `AlertDialog`, so the dialog
  is only dismissed when its positive button is pressed with valid input.


## Kilsten.WearOS

As in Kilsten.Android, general helpers are in the root `Kilsten.WearOS` namespace, and controls
are in a sub-namespace.


### Helpers (`Kilsten.WearOS`)

- **ExternalUrl.OpenAsync()** - opens a URL in the browser on the paired phone. If the phone
  accepts the request, shows the standard "open on phone" confirmation animation on the watch.
  Returns whether the request succeeded, so the app can report a failure, for example when no
  phone is connected.


### Controls (`Kilsten.WearOS.Controls`)

- **WearableAlertDialog** - an alert dialog with round Wear OS style image buttons,
  created with a builder in the style of Android's `AlertDialog.Builder`. Supports
  swipe-to-dismiss when cancelable.
- **WearableTaskDialog** - a dialog for long-running tasks, showing a progress indicator
  and an optional cancel button that can be set to appear after a delay. The cancel handler is
  also called if the dialog is cancelled with the back button. Can keep the
  screen on while it is shown.
- **WearableButton** - an image button that greys out its image when disabled.

The library also provides the round button background, close and done icons, and the
colours `kilsten_wear_wearable_systemAccent`, `kilsten_wear_wearable_systemGrey` and
`kilsten_wear_wearable_systemDarkGrey`.


## Requirements

- .NET 10 with the Android workload (`net10.0-android`)
- Android 6.0 (API level 23) or later
- AndroidX packages, which are referenced by the libraries and restored automatically


## Getting started

Clone the repository and add a project reference from your app to `Kilsten.Android`,
`Kilsten.WearOS`, or both:

```xml
<ItemGroup>
  <ProjectReference Include="..\Kilsten\Kilsten.Android\Kilsten.Android.csproj" />
</ItemGroup>
```

Resources that are internal to the libraries are named with a `_kilsten_` prefix, so that
they don't clash with resources in your app.

To use `ExternalUrl.OpenAsync()`, add the confirmation activity to your watch app's manifest:

```xml
<activity android:name="android.support.wearable.activity.ConfirmationActivity" />
```


## Examples

Show a Wear OS alert dialog with a confirm and a cancel button (with
`using Kilsten.WearOS.Controls;`):

```csharp
new WearableAlertDialog.Builder(this)
    .SetTitle("Delete item")
    .SetMessage("This can't be undone.")
    .SetPositiveButton(handler: (sender, e) => DeleteItem())
    .SetNegativeButton()
    .Create()
    .Show();
```

Show a progress dialog while a task runs, with a cancel button that appears after five
seconds (with `using Kilsten.WearOS.Controls;`):

```csharp
var dialog = new WearableTaskDialog.Builder(this)
    .SetTitle("Syncing")
    .SetCancelButton(handler: (sender, e) => CancelSync(), delaySecs: 5)
    .SetKeepScreenOn(true)
    .Create();
dialog.Show();

await SyncAsync();
dialog.Dismiss();
```

Validate the fields of a dialog before accepting it (with `using Kilsten.Android.Validation;`,
and AndroidX's `AlertDialog`). The dialog must have a positive button:

```csharp
var builder = new AlertDialog.Builder(this)
    .SetView(formView)
    .SetPositiveButton("OK", (sender, e) => { })
    .SetNegativeButton("Cancel", (sender, e) => { });

var validator = DialogFormValidator.CreateFrom(builder, formView);
validator
    .SetLengthRange(Resource.Id.nameEditText, 1, 40)
    .SetNumValueRange(Resource.Id.ageEditText, 0, 150);
validator.Valid += (sender, e) => SaveForm(formView);
```


## History

Kilsten grew out of code written between 2019 and 2022 for a number of personal Android and
Wear OS apps, and was previously known as AndroidExt and then Infill. In 2026 it was
reviewed, some bugs were fixed, and the code was updated to current C# and .NET conventions.

"Kilsten" is a Swedish word that, among other meanings, can refer to small pieces of rock
used to fill gaps in a stone wall.


## AI Statement

This code was written by the author, who is Human. A codeing agent (Claude Code) was briefly used,
in 2026, to perform some mechanical refactoring and documentation updates, but the author reviewed
and approved all changes. The functional code is human-written.


## Licence

Kilsten is released under the MIT licence - see [LICENSE.md](LICENSE.md).


## Author

Andrew Johnson - [andy@andyjohnson.uk](mailto:andy@andyjohnson.uk) - [https://andyjohnson.uk](https://andyjohnson.uk)
