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
	float dX;
	float dY;
	public Offsets offsets {get {
		return new Offsets(XOffset.Value, YOffset.Value);
	}}
	public Slider XOffset{get; init;}
	public Slider YOffset{get; init;}
	Random random = new();
	public Ball(Range x_range, Range y_range, bool rotate, StartFrom start_from = StartFrom.Center){
	float randomFloat = (float)random.NextDouble() * 2f;
	float dx = Math.Max(randomFloat, 1f - randomFloat);
	float dy = 1f - dx;
	/* X = start_from switch {
		StartFrom.Min => x_range.Start.Value,
		StartFrom.Center => (x_range.Start.Value + x_range.End.Value) / 2f,
		StartFrom.Max => x_range.End.Value - 1};
	Y = start_from switch {
		StartFrom.Min => y_range.Start.Value,
		StartFrom.Center => (y_range.Start.Value + y_range.End.Value) / 2f,
		StartFrom.Max => y_range.End.Value - 1}; */
	// if (random.Next(2) == 0) dx = -dx;
	// if (random.Next(2) == 0) dy = -dy;
	if (rotate) {
		(dY, dX) = (dx, dy);
		XOffset = new Slider(y_range);
		YOffset = new Slider(x_range);
	}
	else {
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
		XOffset.Set((int)X);
		YOffset.Set((int)Y);
		return (new Offsets(XOffset.Value, YOffset.Value)) != old_offsets;
	}
}
