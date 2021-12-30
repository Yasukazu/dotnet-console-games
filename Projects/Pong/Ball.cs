using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public record Offsets(int x, int y);
public class Ball // : ScreenDrawItem
{
	public char DispChar {get{return 'O';}}
	public float X {get; private set;}
	public float Y {get; private set;}
	public float dX {get; private set;}
	public float dY {get; private set;}
	public float Stride => (float)Math.Sqrt(dX * dX + dY * dY);
	public Offsets offsets {get {
		return new Offsets(XOffset.Value, YOffset.Value);
	}}
	public Slider XOffset{get; init;}
	public Slider YOffset{get; init;}
	Random random = new();

	/// <summary>
	/// itself has dX and dY
	/// </summary>
	/// <param name="x_range"></param>
	/// <param name="y_range"></param>
	/// <param name="rotate"></param>
	/// <param name="start_from"></param>
	/// <param name="degree">random dx and dy are set if 0</param>
    public Ball(Range x_range, Range y_range, bool rotate, StartFrom start_from = StartFrom.Center, int degree = 0) {
        float dx, dy;
        if (degree == 0) {
            float randomFloat = (float)random.NextDouble() * 2f;
            dx = Math.Max(randomFloat, 1f - randomFloat);
            dy = 1f - dx;
        } else {
            dx = (float)Math.Sin(degree);
            dy = (float)Math.Cos(degree);
        }
		var k = (float)Math.Sqrt((dx*dx + dy*dy) * (x_range.End.Value - x_range.Start.Value)
		 * (y_range.End.Value - y_range.Start.Value)) / 10;
		dx *= k;
		dy *= k;
        if (rotate) {
            (dY, dX) = (dx, dy);
            XOffset = new Slider(y_range);
            YOffset = new Slider(x_range);
        } else {
            (dX, dY) = (dx, dy);
            XOffset = new Slider(x_range);
            YOffset = new Slider(y_range);
        }
        X = XOffset.Value;
        Y = YOffset.Value;
    }


	/// <summary>
	/// Change self value with dx or dy
	/// </summary>
	/// <returns>true if 1 of offsets.value is changed </returns>
	public bool Move() {
		if (XOffset.Value == XOffset.Min && dX < 0f ||
			XOffset.Value == XOffset.Max && dX > 0f)
			dX = -dX;
		if (YOffset.Value == YOffset.Min && dY < 0f ||
			YOffset.Value == YOffset.Max && dY > 0f)
			dY = -dY;
		X += dX;
		Y += dY;
		var old_offsets = new Offsets(XOffset.Value, YOffset.Value);
		XOffset.Set((int)Math.Round(X));
		YOffset.Set((int)Math.Round(Y));
		return (new Offsets(XOffset.Value, YOffset.Value)) != old_offsets;
	}
}
