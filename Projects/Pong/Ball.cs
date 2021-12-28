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
		return new Offsets(XOffset.Value + 1, YOffset.Value + 1);
	}}
	public Slider XOffset{get; init;}
	public Slider YOffset{get; init;}
	Random random = new();
	public Ball(Range x_range, Range y_range, bool rotate){
	float randomFloat = (float)random.NextDouble() * 2f;
	float dx = Math.Max(randomFloat, 1f - randomFloat);
	float dy = 1f - dx;
	float x = x_range.End.Value / 2f;
	float y = y_range.End.Value / 2f;
	if (random.Next(2) == 0)
		dx = -dx;
	if (random.Next(2) == 0)
		dy = -dy;
	if (rotate) {
	//	(Y, X) = (x, y);
		(dY, dX) = (dx, dy);
	}
	else {
		//(X, Y) = (x, y);
		(dX, dY) = (dx, dy);
	}
		XOffset = new Slider(0..(x_range.End.Value - 1), (int)x);
		YOffset = new Slider(0..(y_range.End.Value - 1), (int)y);
	}

	public bool Move() {
		if (XOffset.Value == 0 && dX < 0f ||
			XOffset.Value == XOffset.Max && dX > 0f)
			dX = -dX;
		if (YOffset.Value == 0 && dY < 0f ||
			YOffset.Value == YOffset.Max && dY > 0f)
			dY = -dY;
		X += dX;
		Y += dY;
		var old_offsets = new Offsets(XOffset.Value, YOffset.Value);
		XOffset.set((int)X);
		YOffset.set((int)Y);
		return (new Offsets(XOffset.Value, YOffset.Value)) != old_offsets;
	}
	/* public BitArray GetImage(){
		BitArray buff = new BitArray(XOffset.Max + 1);
		return
	}*/
}
