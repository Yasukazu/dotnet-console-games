using System.Net.Security;
using System.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
#nullable enable
public record BallSpec(Range xrange, Range yrange, Rotation rot);
public class PaddleScreen : Screen {
	public BallSpec BallSpec => new BallSpec(1..SideToSide, 1..HomeToAway, isRotated ? Rotation.Vertical : Rotation.Horizontal);
	public int HomeToAway {get{
		return isRotated ? w - 1 : h - 1;
	}}
	public int SideToSide {get{
		return isRotated ? h - 1: w - 1;
	}}
	public Range PaddleRange {get{
		return new Range(0, SideToSide + 1);}} // 0 <= Paddle < PaddleRange
	public int AwayLineNum {get;init;}
	public const int HomeLineNum = 0;
	// public Wall[] Walls = new Wall[2];
	// public int[] WallLocations = new int[2];
	enum WallSide {Left, Right};
	SideWall[] SideWalls = new SideWall[2];
	record SideWall(WallSide Side, Wall wall);
	// public Paddle[] Paddles = new Paddle[2]; // 0: self, 1: opponent
	/// <summary>
	/// Ball travel spec.
	/// </summary>
	/// <value>(x, y)</value>
	public Range[] BallRanges {get{
		//var rr = new Range[2];
		//rr[0] = (1..SideToSide);
		//rr[1] = (1..HomeToAway);
		return new Range[] {1..SideToSide, 1..HomeToAway};
	}}
	public Ball Ball;
	public Paddle[] Paddles = new Paddle[2];
	List<ScreenDrawItem> DrawItems = new();
	// IConsole Console;
	public PaddleScreen(IConsole console, bool rotate) : base(console, rotate) {
		AwayLineNum = this.EndOfLines; // Lines.Length - 1;
		// for (int i = 0; i < Walls.Length; ++i) Walls[i] = new Wall(1..EndOfLines);
		// WallLocations = {0, EndOfLines - 1};
		SideWalls[0] = new SideWall(WallSide.Left, new Wall(1..EndOfLines));
		SideWalls[1] = new SideWall(WallSide.Right, new Wall(1..EndOfLines));
		Ball = new(0..SideToSide, 0..HomeToAway, 0);
		// Console = console;
	}
	public void draw(Paddle padl, bool replace_buffer = true) {
		var side = padl.Side;
		int n = (side == PaddleSide.Home) ? 0 : AwayLineNum;
		var image = padl.GetImage();
		Debug.WriteLine($"Pad bitimage Length:{image.Length}, Width: {padl.Width}, Offset Max: {padl.Offset.Max}.");
		if(Lines[n] == null)
			drawImage(n, image, padl.DispChar);
		else
			redrawImage(n, image, padl.DispChar);
		if(replace_buffer)
		if(replace_buffer)
		if(replace_buffer)
			Lines[n] = image;
	}
	public bool drawBall() {
		var offsets = Ball.offsets;
		if (Ball.Move()){
			var new_offsets = Ball.offsets;
			Console.PrintAt(offsets.x, offsets.y, (char)CharCode.SPC); 
			Console.PrintAt(new_offsets.x, new_offsets.y, Ball.DispChar);
			return true;
		}
		return false;
	}
	public bool doBall(Queue<Action> drawQueue) {
		var offsets = Ball.offsets;
		if (Ball.Move()){
			var new_offsets = Ball.offsets;
			drawQueue.Enqueue(()=> {
				Console.PrintAt(offsets.x, offsets.y, (char)CharCode.SPC);
				Console.PrintAt(new_offsets.x, new_offsets.y, Ball.DispChar);
			});
			return true;
		}
		return false;
	}
	public void HideBall(Queue<Action> drawQueue) {
		var offsets = Ball.offsets;
		drawQueue.Enqueue(()=>{
			Console.PrintAt(offsets.x, offsets.y, (char)CharCode.SPC);
		});
	}
	public void ResetBall(Queue<Action> drawQueue) {
		Ball.Reset();
		var offsets = Ball.offsets;
		drawQueue.Enqueue(()=>{
			Console.PrintAt(offsets.x, offsets.y, Ball.DispChar);
		});
	}

	public void resetBall() {
		var offsets = Ball.offsets;
		Console.PrintAt(offsets.x, offsets.y, (char)CharCode.SPC);
		Ball.Reset();
		offsets = Ball.offsets;
		Console.PrintAt(offsets.x, offsets.y, Ball.DispChar);
	}
	public void drawWalls() {
		char c = isRotated ? '-' : '|';
		void drawVLine(int fromLeft) {
			Debug.WriteLine($"Walls y: from 1 to {HomeToAway}.");
			for (int y = 1; y <= HomeToAway; ++y){
				Console.PrintAt(fromLeft, y, c);
			}
		}
		drawVLine(0);
		Debug.WriteLine($"Walls x: 0 and {SideToSide}.");
		drawVLine(SideToSide);
	}

	public class Wall : ScreenDrawItem {
		DrawDirection drawDirection {get{return DrawDirection.Rotating;}}
		public char DispChar {get { return '.';}}
		public Range range{get; init;}
		bool isRotating{get; init;}
		public Wall(Range _range) {
			range = _range;
		}
		public BitArray GetImage() {
			var buff = new BitArray(range.End.Value);
			for (int i = 1; i < range.End.Value; ++i)
				buff[i] = true;
			return buff;
		}
	}
}