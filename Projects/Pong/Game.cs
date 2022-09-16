// using System.Threading;
using System.Timers;
using System.Threading;
using System.Threading.Tasks;
namespace pong;
public class Game {
	public Ball Ball;
	public PaddleScreen screen;
	volatile public SelfPaddle selfPadl;
	volatile public OpponentPaddle oppoPadl;
	// public Paddle[] Paddles = new Paddle[2]; // {selfPadl, oppoPadl};
	// public BitArray SelfOutputImage, OpponentOutputImage;
	// public int PaddleWidth {get; init;}
	public Dictionary<System.ConsoleKey, Func<int>> manipDict = new();	
	public Rotation rotation {get; init;}
	TimeSpan delay => TimeSpan.FromMilliseconds(Opts.delay);
	Stopwatch opponentStopwatch = new();
	Stopwatch ballStopwatch = new();
	System.Timers.Timer ballTimer;
	// bool ballIsRunning;
	TimeSpan opponentInputDelay { get{ 
		var ak = Opts.ball_delay * Math.Cos(Ball.Angle);
		// var ms = Math.Abs(Math.Round(Opts.oppo_delay / ak));
		return TimeSpan.FromMilliseconds( Math.Abs(Math.Round(ak)));
		}
	}

	TimeSpan ballDelay => TimeSpan.FromMilliseconds(Math.Round((float)Opts.ball_delay / Ball.Stride));
	public Points score = new(3, 3);
	// int[] Points = {3, 3}; // self, opponent
	Queue<Action> DrawQueue = new();
	int newBallDelay = 800;
	public Options Opts{get;init;}

	public float BallSpeed => (float)Math.Sqrt(Math.Pow(Ball.dX, 2) + Math.Pow(Ball.dY, 2)) / Opts.ball_delay;

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
		ballTimer = new (Opts.ball_delay);//o => do_ball(o), null, 0, Opts.ball_delay);
		ballTimer.AutoReset = true;
	}

	public void Run(Points? given_score = null){
		if(given_score != null)
			score = given_score;
		CancellationTokenSource tokenSource = new();
		var ctoken = tokenSource.Token;
		opponentStopwatch.Start(); // ballStopwatch.Start();
		ballTimer.Elapsed += do_ball;
		ballTimer.Enabled = true; // ballIsRunning = true;
	while(score.Min > 0) {
		int react;
		if (Console.KeyAvailable) {
			System.ConsoleKey key = Console.ReadKey(true).Key;
			if (key == ConsoleKey.Escape){
				tokenSource.Cancel();
				goto exit;
			}
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
		if(ballTimer.Enabled && opponentStopwatch.IsRunning && opponentStopwatch.Elapsed > opponentInputDelay){
			var diff = screen.Ball.offsets.x - (oppoPadl.Offset.Value + oppoPadl.Width / 2);
			if (Math.Abs(diff) > Opts.oppo_about){ // know when diff. is 2
				opponentStopwatch.Stop();
				Task.Run(()=> {
					Task.Delay(opponentInputDelay).Wait();
					if(!ctoken.IsCancellationRequested){
						oppoPadl.Shift(diff < 0 ? -Opts.oppo_speed : Opts.oppo_speed);
						DrawQueue.Enqueue( () => screen.draw(oppoPadl) );
					}
					opponentStopwatch.Restart();
				}, ctoken);
				continue;
			}
			opponentStopwatch.Restart();
		}
		while (DrawQueue.Count > 0){
			var action = DrawQueue.Dequeue();
			//Debug.Assert(action != null);
			if(action != null)
				action();
		}
		// Thread.Sleep(delay);
		using(var task = Task.Delay(delay)) {
			task.Wait();
		}
	}
	exit:
	tokenSource.Cancel();
	ballTimer.Enabled = false;
	return; // new Points(self: Points[0], opponent: Points[1]);
        void do_ball(object obj, ElapsedEventArgs eeas) {
			// if(!ballIsRunning) return;
            // if(ballStopwatch.IsRunning && ballStopwatch.Elapsed > ballDelay){
            var old_dy = Ball.dY;
            var ballMoved = screen.doBall(DrawQueue); // screen.Ball.Move();
            if (ballMoved.x != 0 || ballMoved.y != 0) {
                bool doReset = false;
                var offsets = Ball.offsets;
                if (old_dy < 0 && offsets.y <= Ball.YOffset.Min) {
                    var selfPadlStart = selfPadl.Offset.Value;
                    var selfPadlEnd = selfPadlStart + selfPadl.Width + 0;
                    if (!(selfPadlStart..selfPadlEnd).Contains(offsets.x)) {
                        --score.Self; // Points[0];
                        doReset = true;
                    }
                }
                else if (old_dy > 0 && offsets.y >= Ball.YOffset.Max) {
                    var PadlStart = oppoPadl.Offset.Value;
                    var PadlEnd = PadlStart + oppoPadl.Width;
                    if (!(PadlStart..PadlEnd).Contains(offsets.x)) {
                        --score.Opponent; // Points[1];
                        doReset = true;
                    }
                }
                if (doReset) {
					ballTimer.Enabled = false; // stop timer first
                    screen.HideBall(DrawQueue); // erase ball before waiting Task
                    Task.Run(() => {
                        Task.Delay(newBallDelay).Wait(); // wait first
						if(!ctoken.IsCancellationRequested){
	                        screen.ResetBall(DrawQueue);
	                        Array.ForEach(screen.Paddles, (p) => {
	                            p.Reset();
	                            DrawQueue.Enqueue(() => screen.draw(p));
	                        });
						}
						// ballTimer.Change(0, Opts.ball_delay);
						ballTimer.Enabled = true; // ballIsRunning = true;
                    }, ctoken);
                }
            }
        }
    }

}

public record Points{
	public int Self{get; set;} 
	public int Opponent	{get; set;}
	public Points(int self, int opponent) {
		Self = self;
		Opponent = opponent;
	}
	Points() {
		Self = 0;
		Opponent = 0;
	}
	public int Min => Self < Opponent ? Self : Opponent;
	public static void SSaveXML(Points[] ppoints, string save_to_xml) {
        System.Xml.Serialization.XmlSerializer serializer = new (typeof(Points[]));
        using(System.IO.StreamWriter writer = new(save_to_xml, false, new System.Text.UTF8Encoding(false))){
            serializer.Serialize(writer, ppoints);
        }
    }
	public static Points[]? LLoadXML(string load_from_xml) {
        System.Xml.Serialization.XmlSerializer serializer = new (typeof(Points[]));
        using(System.IO.StreamReader reader = new(load_from_xml)){
			var obj = serializer.Deserialize(reader);
			if(obj != null)
				return (Points[])obj;
        }
		return null;
    }
}