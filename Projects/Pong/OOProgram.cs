<<<<<<< HEAD
﻿using System.Runtime.CompilerServices;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using CommandLineParser; // Original source code: https://github.com/wertrain/command-line-parser-cs (Version 0.1)
=======
﻿global using System;
global using System.Linq;
global using System.Collections;
global using System.Collections.Generic;
global using System.Diagnostics;
global using System.Threading;

global using CommandLineParser; // Original source code: https://github.com/wertrain/command-line-parser-cs (Version 0.1)
>>>>>>> tmp

// ConsoleTraceListener myWriter = new GonsoleTraceListener();
// Trace.Listeners.Add(myWriter);
Debug.Write("myWriter is added to Trace.Listeners.  OOProgram start.");
var _rotation = 90; // Rotation.Horizontal;
var clargs = Environment.GetCommandLineArgs();
var pArgs = clargs[1..];
<<<<<<< HEAD
ParserResult<Options> parseResult = Parser.Parse<Options>(pArgs);
=======
var parseResult = Parser.Parse<Options>(pArgs);
/*
>>>>>>> tmp
var speed_ratio = 1;
var screen_width = 32;
var screen_height = 12;
var paddle_width = 8;
var delay = 100;
var oppo_delay = 300;
var ball_delay = 100;
var ball_angle = 0;
if (parseResult.Tag == ParserResultType.Parsed){
	if(parseResult.Value.speed > 0)
		speed_ratio = parseResult.Value.speed;
	if(parseResult.Value.width > 0)
		screen_width = parseResult.Value.width;
	if(parseResult.Value.height > 0)
		screen_height = parseResult.Value.height;
	if(parseResult.Value.paddle > 0)
		paddle_width = parseResult.Value.paddle;
	_rotation = parseResult.Value.rotation;
	if(parseResult.Value.delay > 0)
		delay = parseResult.Value.delay;
	if(parseResult.Value.oppo_delay > 0)
		oppo_delay = parseResult.Value.oppo_delay;
	if(parseResult.Value.ball_delay > 0)
		ball_delay = parseResult.Value.ball_delay;
	if(parseResult.Value.ball_angle != 0)
<<<<<<< HEAD
		ball_angle = parseResult.Value.ball_angle;
}else {
	Environment.Exit(-1);
}
=======
		ball_delay = parseResult.Value.ball_angle;
} */
// Make a new Options instance
Options opt = parseResult.Value;

>>>>>>> tmp
Rotation rot = _rotation switch {
	0 => Rotation.Horizontal, 90 => Rotation.Vertical,
	_ => throw new ArgumentException("Rotation must be one of {0, 90}.")
};
(opt.width, opt.height) = OnScreen.init(opt.width, opt.height);
var game = new Game(opt); // speed_ratio, screen_w, screen_h, paddle_width, rot, delay, oppo_delay, ball_delay, ball_angle);
game.Run();
public class Game {
	public Ball Ball;
	public PaddleScreen screen;
	volatile public SelfPaddle selfPadl;
	volatile public OpponentPaddle oppoPadl;
	public Paddle[] Paddles = new Paddle[2]; // {selfPadl, oppoPadl};
	public BitArray SelfOutputImage, OpponentOutputImage;
	// public int PaddleWidth {get; init;}
	public Dictionary<System.ConsoleKey, Func<int>> manipDict = new();	
	public Rotation rotation {get; init;}
	TimeSpan delay;
	Stopwatch opponentStopwatch = new();
	Stopwatch ballStopwatch = new();
	TimeSpan opponentInputDelay;
	TimeSpan ballDelay;
<<<<<<< HEAD
	int[] Points = {3, 3}; // self, opponent
	Queue<Action> DrawQueue = new();
	int newBallDelay = 800;
	public Game(int speed_ratio, int screen_w, int screen_h, int paddleWidth, Rotation rot, 
		int refresh_delay, int opponent_delay, int ball_delay, int ball_angle){

		screen = new(screen_w, screen_h, rot == Rotation.Vertical ? true : false);
		if (paddleWidth >= screen.SideToSide / 2)
			paddleWidth = screen.SideToSide / 2;
		selfPadl = new(range: screen.PaddleRange, width: paddleWidth, manipDict);
		oppoPadl = new(range: screen.PaddleRange, width: paddleWidth);
		Paddles[0] = selfPadl;
		Paddles[1] = oppoPadl;
		var ballSpec = screen.BallSpec;
		screen.Ball = Ball = new(ballSpec.xrange, ballSpec.yrange, StartFrom.Center, ball_angle); 
		/* ballSpec.rot switch {
			Rotation.Horizontal => ball_angle,
			Rotation.Vertical => 90 - ball_angle,
			_  => throw new ArgumentException($"{ballSpec.rot} is not supported as ball angle!")}); */
=======
	public Game(Options opt) 
/*(int speed_ratio, int screen_w, int screen_h, int paddleWidth, 
Rotation rot, int refresh_delay, int opponent_delay, int ball_delay, 
int ball_angle)*/
{

Rotation rot = opt.rotation switch {
	0 => Rotation.Horizontal, 90 => Rotation.Vertical,
	_ => throw new ArgumentException("Rotation must be one of {0, 90}.")
};
		screen = new(opt.width, opt.height, rot == Rotation.Vertical ? true : false);
		if (opt.paddle > screen.SideToSide / 2)
			opt.paddle = screen.SideToSide / 2;
		selfPadl = new(range: screen.PaddleRange, width: opt.paddle, manipDict);
		oppoPadl = new(range: screen.PaddleRange, width: opt.paddle);
		screen.Paddles[0] = selfPadl;
		screen.Paddles[1] = oppoPadl;
		var ballSpec = screen.BallRanges;
		Ball = new(ballSpec[0], ballSpec[1], StartFrom.Center, opt.ball_angle);
		screen.Ball = Ball;
>>>>>>> tmp
		if (rot == Rotation.Vertical){
			manipDict[ConsoleKey.UpArrow] = ()=>{ return selfPadl.Shift(-opt.speed); };
			manipDict[ConsoleKey.DownArrow] = ()=>{ return selfPadl.Shift(opt.speed); };
		}else{
			manipDict[ConsoleKey.LeftArrow] = ()=>{ return selfPadl.Shift(-opt.speed); };
			manipDict[ConsoleKey.RightArrow] = ()=>{ return selfPadl.Shift(opt.speed); };
		}
		delay = TimeSpan.FromMilliseconds(opt.delay);
		opponentInputDelay = TimeSpan.FromMilliseconds(opt.oppo_delay);
		// Ball dX dY compensation
		var ball_stride = Ball.Stride;
		ballDelay = TimeSpan.FromMilliseconds(Math.Round(opt.ball_delay / Ball.Stride));
	// pdl = new VPaddle(screen.w, paddle_width); // NestedRange(0..(width / 3), 0..width);
	Console.CancelKeyPress += delegate {
		Console.Clear();
		Console.CursorVisible = true;
	};
	Console.CursorVisible = false; // hide cursor
	Console.Clear();
	Debug.WriteLine($"screen.isRotated={screen.isRotated}");
	Debug.WriteLine($"selfPadl range: 0..{selfPadl.Offset.Max + selfPadl.Width + 1}");
	Debug.Write($"screen.SideToSide={screen.SideToSide}, ");
	Debug.WriteLine($"screen.HomeToAway={screen.HomeToAway}");
	Debug.WriteLine($"screen.EndOfLines={screen.EndOfLines}");
		screen.drawWalls();
		screen.draw(selfPadl);
		screen.draw(oppoPadl);

	}

	public void Run(){
		opponentStopwatch.Restart();
		ballStopwatch.Restart();
	while(Points.Min() > 0) {
		int react;
		if (Console.KeyAvailable) {
			System.ConsoleKey key = Console.ReadKey(true).Key;
			if (key == ConsoleKey.Escape)
				goto exit;
			if (selfPadl.ManipDict.ContainsKey(key)) {
				react = selfPadl.ReactKey(key);
				if(react != 0){
					screen.draw(selfPadl);
				}
			}
			// else if (pdl.manipDict.ContainsKey(key)) moved = pdl.manipDict[key]() != 0;
			while(Console.KeyAvailable) // clear over input
				Console.ReadKey(true);
		}
		if(ballStopwatch.IsRunning && ballStopwatch.Elapsed > ballDelay){
			var old_dy = Ball.dY;
			var isBallMoved = screen.drawBall(); // screen.Ball.Move();
            if (isBallMoved) {
				bool doReset = false;
				var offsets = Ball.offsets;
                if (old_dy < 0 && offsets.y <= Ball.YOffset.Min) {
                    var selfPadlStart = selfPadl.Offset.Value;
                    var selfPadlEnd = selfPadlStart + selfPadl.Width + 0;
                    if (!(selfPadlStart..selfPadlEnd).Contains(offsets.x)) {
						--Points[0];
						doReset = true;
                    }
                }
                else if (old_dy > 0 && offsets.y >= Ball.YOffset.Max) {
                    var PadlStart = oppoPadl.Offset.Value;
                    var PadlEnd = PadlStart + oppoPadl.Width;
                    if (!(PadlStart..PadlEnd).Contains(offsets.x)) {
						--Points[1];
						doReset = true;
                    }
                }
				if (doReset) {
					ballStopwatch.Stop();
					Task.Run(()=> {
						screen.resetBall();
						Array.ForEach(Paddles, (p)=> {
							p.Reset();
							screen.draw(p);});
						Task.Delay(newBallDelay).Wait();
						ballStopwatch.Restart();
					});
					continue;
				}
            }

		}
		if(ballStopwatch.IsRunning && opponentStopwatch.IsRunning && opponentStopwatch.Elapsed > opponentInputDelay){
			var diff = screen.Ball.offsets.x - (oppoPadl.Offset.Value + oppoPadl.Width / 2);
			if (Math.Abs(diff) > 1){ // know when diff. is 2
				opponentStopwatch.Stop();
				Task.Run(()=> {
					Task.Delay(opponentInputDelay).Wait();
					DrawQueue.Enqueue( () => {
						oppoPadl.Shift(diff < 0 ? -1 : 1);
						screen.draw(oppoPadl);
						opponentStopwatch.Restart();
					});
				});
			}
		}
		while (DrawQueue.Count > 0)
			DrawQueue.Dequeue()();

		// Thread.Sleep(delay);
		using(var task = Task.Delay(delay)) {
			task.Wait();
		}
	}
	exit:
    screen.SetCursorPosition(0, 0);

	string msg;
	switch ((Points[0], Points[1])){
		case (> 0, <= 0):
		 msg = "You win";
		 break;
		case (<= 0, > 0):
		 msg = "You lose";
		 break;
		default:
		 msg = "Even-even";
		 break;
	}

    Console.Write($"{msg}. Hit any key:");
    // Console.ReadKey();
	Console.CursorVisible = true;
	}
}





public class GonsoleTraceListener : ConsoleTraceListener {
	public override void Write(string s){
		var (x,y) = Console.GetCursorPosition();
		Console.SetCursorPosition(0, 0);
		Trace.WriteLine(s);
		// Console.ReadKey();
		Console.SetCursorPosition(x,y);
	}

}