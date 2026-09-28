using System.Diagnostics.CodeAnalysis;

using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;


namespace Kilsten.Android.Controls;

/// <summary>
/// <see cref="FrameLayout"/> that lets the user pinch-zoom and pan a single child view, typically a
/// <see cref="ZoomImage"/> or <see cref="ZoomFrame"/>.
/// </summary>
public class ZoomLayout : FrameLayout
{
    /// <summary>
    /// Initialises a new <see cref="ZoomLayout"/>.
    /// </summary>
    /// <param name="context">Context</param>
    public ZoomLayout(Context context)
        : base(context)
    {
        Configure(context, null);
    }

    /// <summary>
    /// Initialises a new <see cref="ZoomLayout"/>.
    /// </summary>
    /// <param name="context">Context</param>
    /// <param name="attrs">Attributes</param>
    public ZoomLayout(Context context, IAttributeSet attrs)
        : base(context, attrs)
    {
        Configure(context, attrs);
    }

    /// <summary>
    /// Initialises a new <see cref="ZoomLayout"/>.
    /// </summary>
    /// <param name="context">Context</param>
    /// <param name="attrs">Attributes</param>
    /// <param name="defStyle">Default style</param>
    public ZoomLayout(Context context, IAttributeSet attrs, int defStyle)
        : base(context, attrs, defStyle)
    {
        Configure(context, attrs);
    }

    /// <summary>
    /// Initialises a new <see cref="ZoomLayout"/>.
    /// </summary>
    /// <param name="context">Context</param>
    /// <param name="attrs">Attributes</param>
    /// <param name="defStyle">Default style</param>
    /// <param name="defStyleRes">Default style resource ID</param>
    public ZoomLayout(Context context, IAttributeSet attrs, int defStyle, int defStyleRes)
        : base(context, attrs, defStyle, defStyleRes)
    {
        Configure(context, attrs);
    }


    /// <summary>
    /// Raised after the view has been laid out.
    /// </summary>
    public event EventHandler? LaidOut;

    /// <summary>
    /// Raised when the user taps the view.
    /// </summary>
    public event EventHandler<MotionEvent>? SingleTap;

    /// <summary>
    /// Raised when the user long-presses the view.
    /// </summary>
    public event EventHandler<MotionEvent>? LongPress;

    /// <summary>
    /// Raised when the zoom level changes.
    /// </summary>
    public event EventHandler<ZoomLevelChangedEventArgs>? ZoomLevelChanged;

    /// <summary>
    /// Raised when the user flings the view.
    /// </summary>
    public event EventHandler<FlingEventArgs>? Fling;


    /// <summary>
    /// Event arguments for <see cref="ZoomLevelChanged"/>.
    /// </summary>
    public class ZoomLevelChangedEventArgs : EventArgs
    {
        /// <summary>The new zoom level</summary>
        public float ZoomLevel { get; set; }
    }

    /// <summary>
    /// Event arguments for <see cref="Fling"/>.
    /// </summary>
    public class FlingEventArgs
    {
        /// <summary>The down event that started the fling, if available</summary>
        public MotionEvent? E1 { get; set; }
        /// <summary>The move event that triggered the fling</summary>
        public required MotionEvent E2 { get; set; }
        /// <summary>Horizontal velocity, in pixels per second</summary>
        public float VelocityX { get; set; }
        /// <summary>Vertical velocity, in pixels per second</summary>
        public float VelocityY { get; set; }
    }


    /// <summary>
    /// The child's current scale, limited to between <see cref="MinZoomLevel"/> and <see cref="MaxZoomLevel"/>.
    /// </summary>
    public float ZoomLevel
    {
        get => zoom;
        set
        {
            zoom = LimitZoom(value);
            ApplyTransform();
            OnZoomLevelChanged();
        }
    }

    /// <summary>
    /// The minimum zoom level. Default is 1.
    /// </summary>
    public float MinZoomLevel
    {
        get => minZoom;
        set
        {
            minZoom = value;
            EnforceZoomLimits();
        }
    }

    /// <summary>
    /// The maximum zoom level. Default is 4.
    /// </summary>
    public float MaxZoomLevel
    {
        get => maxZoom;
        set
        {
            maxZoom = value;
            EnforceZoomLimits();
        }
    }


    /// <summary>
    /// Scales the child to fit the view, within the zoom limits, and centres it. Raises <see cref="ZoomLevelChanged"/>.
    /// </summary>
    public void ResetZoom()
    {
        if (!TryGetLaidOutChild(out var child))
            return;

        zoom = LimitZoom(Math.Min((float)Width / child.Width, (float)Height / child.Height));
        offset = new PointF((Width - child.Width) / 2f, (Height - child.Height) / 2f);
        ApplyTransform();
        OnZoomLevelChanged();
    }


    /// <summary>
    /// If a point in the child is not visible, pans to centre it. The zoom level is not changed.
    /// </summary>
    /// <param name="childViewPt">Point, in the child's coordinates</param>
    public void EnsureVisible(
        PointF childViewPt)
    {
        if (!TryGetLaidOutChild(out var child))
            return;

        var onScreen = ChildToLayout(child, childViewPt);
        if (onScreen.X >= 0 && onScreen.X <= Width && onScreen.Y >= 0 && onScreen.Y <= Height)
            return;

        offset = new PointF(offset.X + (Width / 2f) - onScreen.X, offset.Y + (Height / 2f) - onScreen.Y);
        ApplyTransform();
    }


    /// <inheritdoc/>
    protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
    {
        base.OnLayout(changed, left, top, right, bottom);
        LaidOut?.Invoke(this, EventArgs.Empty);
    }


    /// <inheritdoc/>
    public override bool OnInterceptTouchEvent(MotionEvent? motionEvent)
    {
        // Keep ancestors, such as scrolling containers, from taking over a gesture that starts here.
        var action = motionEvent?.ActionMasked;
        if (action == MotionEventActions.Down)
            Parent?.RequestDisallowInterceptTouchEvent(true);
        else if (action == MotionEventActions.Up || action == MotionEventActions.Cancel)
            Parent?.RequestDisallowInterceptTouchEvent(false);

        return false;
    }


    /// <inheritdoc/>
    public override bool OnTouchEvent(MotionEvent? motionEvent)
    {
        if (motionEvent == null)
            return false;

        scaleDetector.OnTouchEvent(motionEvent);
        gestureDetector.OnTouchEvent(motionEvent);

        switch (motionEvent.ActionMasked)
        {
            case MotionEventActions.Down:
                StartPan(motionEvent);
                ForwardToChild(motionEvent);
                break;
            case MotionEventActions.PointerDown:
                gesture = Gesture.Pinch;
                break;
            case MotionEventActions.Move:
                ContinuePan(motionEvent);
                break;
            case MotionEventActions.PointerUp:
                gesture = Gesture.Idle;
                break;
            case MotionEventActions.Up:
                gesture = Gesture.Idle;
                ForwardToChild(motionEvent);
                break;
        }

        if (gesture != Gesture.Idle)
            ApplyTransform();
#if DEBUG
        else
            Invalidate();
#endif

        return true;
    }


    // ----------------------------------------------------------------------------------------------------------
    // Implementation

    private enum Gesture
    {
        Idle,
        Pan,
        Pinch
    }

    private const float ZoomChangeNotifyThreshold = 0.01f;

    private ScaleGestureDetector scaleDetector;
    private GestureDetector gestureDetector;

    private float zoom = 1f;
    private float minZoom = 1f;
    private float maxZoom = 4f;
    private PointF offset = new(0f, 0f);

    private Gesture gesture = Gesture.Idle;
    private PointF panAnchor = new(0f, 0f);


    [MemberNotNull(nameof(scaleDetector), nameof(gestureDetector))]
    private void Configure(Context context, IAttributeSet? attrs)
    {
        scaleDetector = new ScaleGestureDetector(context, new PinchListener(this));
        gestureDetector = new GestureDetector(context, new TapListener(this));

        if (attrs == null)
            return;

        var values = context.ObtainStyledAttributes(attrs, Resource.Styleable.ZoomLayout);
        MinZoomLevel = values.GetFloat(Resource.Styleable.ZoomLayout_minZoomLevel, MinZoomLevel);
        MaxZoomLevel = values.GetFloat(Resource.Styleable.ZoomLayout_maxZoomLevel, MaxZoomLevel);
#if DEBUG
        showDebugInfo = values.GetBoolean(Resource.Styleable.ZoomLayout_debugInfoShow, showDebugInfo);
        debugColour = values.GetColor(Resource.Styleable.ZoomLayout_debugInfoColor, debugColour);
#endif
    }


    private float LimitZoom(float value) => Math.Min(Math.Max(value, minZoom), maxZoom);


    private void EnforceZoomLimits()
    {
        var limited = LimitZoom(zoom);
        if (limited == zoom)
            return;

        zoom = limited;
        if (GetChildAt(0) != null)
        {
            offset = new PointF(0f, 0f);
            ApplyTransform();
        }
        OnZoomLevelChanged();
    }


    private void OnZoomLevelChanged()
    {
        ZoomLevelChanged?.Invoke(this, new ZoomLevelChangedEventArgs() { ZoomLevel = zoom });
    }


    private bool TryGetLaidOutChild([NotNullWhen(true)] out View? child)
    {
        child = GetChildAt(0);
        return child != null && child.Width > 0 && child.Height > 0;
    }


    private void ApplyTransform()
    {
        // The child is scaled about its pivot point (its centre, by default), and then translated.
        var child = GetChildAt(0);
        if (child == null)
            return;

        child.ScaleX = zoom;
        child.ScaleY = zoom;
        child.TranslationX = offset.X;
        child.TranslationY = offset.Y;

#if DEBUG
        Invalidate();
#endif
    }


    private PointF ChildToLayout(View child, PointF point)
    {
        return new PointF(
            offset.X + child.PivotX + ((point.X - child.PivotX) * zoom),
            offset.Y + child.PivotY + ((point.Y - child.PivotY) * zoom));
    }


    private void StartPan(MotionEvent motionEvent)
    {
        gesture = Gesture.Pan;
        panAnchor = new PointF(motionEvent.GetX() - offset.X, motionEvent.GetY() - offset.Y);
    }


    private void ContinuePan(MotionEvent motionEvent)
    {
        // Panning follows the first finger, including while pinching.
        if (gesture != Gesture.Idle)
            offset = new PointF(motionEvent.GetX() - panAnchor.X, motionEvent.GetY() - panAnchor.Y);
    }


    private void ForwardToChild(MotionEvent motionEvent)
    {
        // Let the child see the start and end of each gesture, in its own coordinates, so that it can detect clicks.
        var child = GetChildAt(0);
        if (child == null)
            return;

        var childEvent = MotionEvent.Obtain(motionEvent)!;
        childEvent.OffsetLocation(-child.Left, -child.Top);
        var layoutToChild = new Matrix();
        child.Matrix!.Invert(layoutToChild);
        childEvent.Transform(layoutToChild);

        child.DispatchTouchEvent(childEvent);
        childEvent.Recycle();
    }


    private sealed class PinchListener(ZoomLayout owner) : ScaleGestureDetector.SimpleOnScaleGestureListener
    {
        public override bool OnScale(ScaleGestureDetector detector)
        {
            if (!owner.TryGetLaidOutChild(out _))
                return false;

            var previous = owner.zoom;
            owner.zoom = owner.LimitZoom(owner.zoom * detector.ScaleFactor);
            if (Math.Abs(owner.zoom - previous) > ZoomChangeNotifyThreshold)
                owner.OnZoomLevelChanged();

            return true;
        }
    }


    // Implements only IOnGestureListener, not SimpleOnGestureListener: GestureDetector enables double-tap detection
    // for listeners that also implement IOnDoubleTapListener, which would suppress SingleTap for a second quick tap.
    private sealed class TapListener(ZoomLayout owner) : Java.Lang.Object, GestureDetector.IOnGestureListener
    {
        public bool OnDown(MotionEvent e) => true;

        public void OnShowPress(MotionEvent e)
        {
        }

        public bool OnSingleTapUp(MotionEvent e)
        {
            owner.SingleTap?.Invoke(owner, e);
            return true;
        }

        public bool OnScroll(MotionEvent? e1, MotionEvent e2, float distanceX, float distanceY) => true;

        public void OnLongPress(MotionEvent e)
        {
            owner.gesture = Gesture.Idle;
            owner.LongPress?.Invoke(owner, e);
        }

        public bool OnFling(MotionEvent? e1, MotionEvent e2, float velocityX, float velocityY)
        {
            if (!owner.TryGetLaidOutChild(out _))
                return false;

            owner.Fling?.Invoke(owner, new FlingEventArgs() { E1 = e1, E2 = e2, VelocityX = velocityX, VelocityY = velocityY });
            return true;
        }
    }


#if DEBUG

    private bool showDebugInfo = false;
    private Color debugColour = Color.White;
    private Paint? debugStrokePaint = null;
    private Paint? debugTextPaint = null;

    /// <inheritdoc/>
    protected override void OnDraw(Canvas canvas)
    {
        base.OnDraw(canvas);

        if (!showDebugInfo)
            return;

        debugStrokePaint ??= new Paint(PaintFlags.AntiAlias) { Color = debugColour, StrokeWidth = 3f };
        debugStrokePaint.SetStyle(Paint.Style.Stroke);
        debugTextPaint ??= new Paint(PaintFlags.AntiAlias) { Color = debugColour, TextSize = 40f };

        var bounds = new Rect();
        debugTextPaint.GetTextBounds("0", 0, 1, bounds);
        var margin = bounds.Height();
        var right = canvas.Width - 1;
        var bottom = canvas.Height - 1;

        var status = $"G={gesture} Z={zoom:N1} T=({(int)offset.X},{(int)offset.Y})";
        debugTextPaint.GetTextBounds(status, 0, status.Length, bounds);
        canvas.DrawText(status, (canvas.Width - bounds.Width()) / 2, margin + bounds.Height(), debugTextPaint);

        DrawDebugCorner(canvas, 0, 0, margin, margin, bounds);
        DrawDebugCorner(canvas, right, 0, -margin, margin, bounds);
        DrawDebugCorner(canvas, 0, bottom, margin, -margin, bounds);
        DrawDebugCorner(canvas, right, bottom, -margin, -margin, bounds);
    }

    private void DrawDebugCorner(Canvas canvas, int x, int y, int dx, int dy, Rect bounds)
    {
        var label = $"({x},{y})";
        debugTextPaint!.GetTextBounds(label, 0, label.Length, bounds);
        var textX = dx > 0 ? x + dx : x + dx - bounds.Width();
        var textY = dy > 0 ? y + dy + bounds.Height() : y + dy;
        canvas.DrawText(label, textX, textY, debugTextPaint);
        canvas.DrawLine(x, y, x + dx, y + dy, debugStrokePaint!);
    }

#endif
}
