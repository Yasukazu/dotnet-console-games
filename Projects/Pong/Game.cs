public class Game {
	public Ball Ball;
	public PaddleScreen screen;
	volatile public SelfPaddle selfPadl;
	volatile public OpponentPaddle oppoPadl;
	// public Paddle[] Paddles = new Paddle[2]; // {selfPadl, oppoPadl};
	public BitArray SelfOutputImage, OpponentOutputImage;
	// public int PaddleWidth {get; init;}
	public Dictionary<System.ConsoleKey, Func<int>> manipDict = new();	
	public Rotation rotation {get; init;}
	TimeSpan delay;
	Stopwatch opponentStopwatch = new();
	Stopwatch ballStopwatch = new();
	TimeSpan opponentInputDelay;
	TimeSpan ballDelay;
	int[] Points = {3, 3}; // self, opponent
	Queue<Action> DrawQueue = new();
	int newBallDelay = 800;
	public Options Opts{get;init;}

	public Game(Options opt) {

		Opts = opt;
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

	public Points Run(){
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
						Array.ForEach(screen.Paddles, (p)=> {
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
						oppoPadl.Shift(diff < 0 ? -Opts.oppo_speed : Opts.oppo_speed);
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
	return new Points(self: Points[0], opponent: Points[1]);

	}
}

public class Points{
	public int Self{get; init;} 
	public int Opponent	{get; init;}
	public Points(int self, int opponent) {
		Self = self;
		Opponent = opponent;
	}
	Points() {
		Self = 0;
		Opponent = 0;
	}
	public static void SSaveXML(Points[] ppoints, string save_to_xml) {
        System.Xml.Serialization.XmlSerializer serializer = new (typeof(Points[]));
        using(System.IO.StreamWriter writer = new(save_to_xml, false, new System.Text.UTF8Encoding(false))){
            serializer.Serialize(writer, ppoints);
        }
    }
	public static Points[] LLoadXML(string load_from_xml) {
        System.Xml.Serialization.XmlSerializer serializer = new (typeof(Points[]));
		Points[] ppoints;
        using(System.IO.StreamReader reader = new(load_from_xml)){
			ppoints = (Points[])serializer.Deserialize(reader);
        }
		return ppoints;
    }
}