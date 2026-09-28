using Android.Graphics;


namespace Kilsten.Android;

/// <summary>
/// Arrowhead styles for <see cref="CanvasExt.DrawArrow(Canvas, PointF, PointF, float, float, ArrowHeadStyle, Paint)"/>.
/// </summary>
public enum ArrowHeadStyle
{
    /// <summary>No arrowhead - just a line</summary>
    None = 0,
    /// <summary>Open - a V shape</summary>
    Open = 1,
    /// <summary>Filled triangle</summary>
    Filled = 2,
    /// <summary>Unfilled triangle</summary>
    Hollow = 3
}


/// <summary>
/// Extension methods for <see cref="Canvas"/>.
/// </summary>
public static class CanvasExt
{
    /// <summary>
    /// Draws a line with an arrowhead at its end point.
    /// </summary>
    /// <param name="self">Canvas</param>
    /// <param name="start">Start point</param>
    /// <param name="stop">End point</param>
    /// <param name="radius">Length of a side of the arrowhead, from point to base</param>
    /// <param name="angle">Angle of the arrowhead's point, in degrees</param>
    /// <param name="arrowHeadStyle">Arrowhead style</param>
    /// <param name="paint">Paint to draw with</param>
    public static void DrawArrow(
        this Canvas self,
        PointF start,
        PointF stop,
        float radius,
        float angle,
        ArrowHeadStyle arrowHeadStyle,
        Paint paint)
    {
        self.DrawArrow(start.X, start.Y, stop.X, stop.Y, radius, angle, arrowHeadStyle, paint);
    }



    /// <summary>
    /// Draws a line with an arrowhead at its end point.
    /// </summary>
    /// <param name="self">Canvas</param>
    /// <param name="startX">X coordinate of the start point</param>
    /// <param name="startY">Y coordinate of the start point</param>
    /// <param name="stopX">X coordinate of the end point</param>
    /// <param name="stopY">Y coordinate of the end point</param>
    /// <param name="radius">Length of a side of the arrowhead, from point to base</param>
    /// <param name="angle">Angle of the arrowhead's point, in degrees</param>
    /// <param name="arrowHeadStyle">Arrowhead style</param>
    /// <param name="paint">Paint to draw with</param>
    public static void DrawArrow(
        this Canvas self,
        float startX,
        float startY,
        float stopX,
        float stopY,
        float radius,
        float angle,
        ArrowHeadStyle arrowHeadStyle,
        Paint paint)
    {
        ArgumentNullException.ThrowIfNull(self);

        if (arrowHeadStyle == ArrowHeadStyle.None)
        {
            self.DrawLine(startX, startY, stopX, stopY, paint);
            return;
        }

        // Points
        var tip = new PointF(stopX, stopY);
        var (base1, base2) = CalculateArrowHeadBasePoints(startX, startY, stopX, stopY, radius, angle);

        // 
        if (arrowHeadStyle == ArrowHeadStyle.Filled || arrowHeadStyle == ArrowHeadStyle.Hollow)
        {
            // Clone the supplied paint object so that we can safely mutate the copy.
            paint = new Paint(paint);
            if (arrowHeadStyle == ArrowHeadStyle.Filled)
                paint.SetStyle(Paint.Style.Fill);
            else
                paint.SetStyle(Paint.Style.Stroke);

            // Draw the arrowhead as a closed shape
            // We do this first so that we can use the start and end points of the base to determine where the
            // line meets it. This is necessary because extending the line to the tip would result in a blunted
            // tip in the case of wide stroke widths.
            var path = new global::Android.Graphics.Path();
            path.SetFillType(global::Android.Graphics.Path.FillType.EvenOdd!);
            path.MoveTo(base1.X, base1.Y);  // base
            path.LineTo(tip.X, tip.Y);  // tip
            path.LineTo(base2.X, base2.Y);  // base
            path.Close();
            self.DrawPath(path, paint);

        }
        else  // ArrowStyle.Open
        {
            // Draw the arrowhead as an open shape
            self.DrawLine(base1.X, base1.Y, tip.X, tip.Y, paint);
            self.DrawLine(tip.X, tip.Y, base2.X, base2.Y, paint);

            // Reset the base points so that when we draw the line (below) it goes to a point inside the hollow head
            // but just short of the tip, We don't extend the line to the tip because this would result in a blunted
            // tip in the case of wide stroke widths.
            (base1, base2) = CalculateArrowHeadBasePoints(startX, startY, stopX, stopY, paint.StrokeWidth * 2f, angle);
        }

        // Draw the line to a point on the base
        var pStop = new PointF(
            base1.X + ((base2.X - base1.X) / 2f),
            base1.Y + ((base2.Y - base1.Y) / 2f));
        self.DrawLine(startX, startY, pStop.X, pStop.Y, paint);
    }



    private static (PointF base1, PointF base2) CalculateArrowHeadBasePoints(
        float startX,
        float startY,
        float stopX,
        float stopY,
        float radius,
        float angle)
    {
        // Angle calculations
        float anglerad = MathF.PI * angle / 180f;
        float lineangle = MathF.Atan2(stopY - startY, stopX - startX);

        var base1 = new PointF(
            stopX - radius * MathF.Cos(lineangle - (anglerad / 2f)),
            stopY - radius * MathF.Sin(lineangle - (anglerad / 2f)));
        var base2 = new PointF(
            stopX - radius * MathF.Cos(lineangle + (anglerad / 2f)),
            stopY - radius * MathF.Sin(lineangle + (anglerad / 2f)));

        return (base1, base2);
    }
}
