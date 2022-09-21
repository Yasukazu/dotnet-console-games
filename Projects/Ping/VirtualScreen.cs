namespace ping;

using pong;

  public enum ScreenItem {
    None, Wall, Ball, Paddle
  }

public class VirtualScreen {
  pong.Screen screen;

  public Border Border { get; init;}
  readonly int _w; // width
  readonly int _h; // height
  public BitArray[] BitImage {get; init;}

  public float Width {
    get => _w;
  }
  public float Height {
    get => _h;
  }
  PointF lastBallPos;
  BallO ball;

  IntervalEvent intervalEvent {get; init;}

  public VirtualScreen (IntervalEvent intervalEvent, BallO ball, int width = 0, int height = 0) {
    this.ball = ball;
    (_w, _h) = pong.OnScreen.init(width, height);
    if (_w < 4) {
      throw new ApplicationException("Screen width is not enough.");
    }
    if (_h < 4) {
      throw new ApplicationException("Screen height is not enough.");
    }
    screen = new Screen(_w, _h);
    Border = new Border(new PointF(0, 0), new SizeF(_w, _h));
    ball.moveTo(screen.dim.x / 2, screen.dim.y / 2);
    lastBallPos = new(ball.x, ball.y);
    BitImage = new BitArray[_h]; 
    for (int i = 0; i < _h; ++i) {
      BitImage[i] = new BitArray(_w);
    }
    this.intervalEvent = intervalEvent;
    // intervalEvent.Step += new EventHandler(ball.step);
    intervalEvent.Step += step;
  }

  void step(object? sender, System.EventArgs e) {
    Debug.Print("VirtualScreen.step();");
    var ballPos = ball.step();
    var ballXi = (int)Math.Round(ballPos.X);
    if (ballXi <= 0 || ballXi >= BitImage[0].Length - 1) {

    }
    if (lastBallPos != ball.getPoint()) {
      // clear old ball image
      var oldLine = (int)Math.Round(ball.y);
      var bb = BitImage[oldLine];
      for(int i = 0; i < bb.Length; ++i) {
        bb[i] = false;
      }
      clearLine(oldLine);
      draw();
      lastBallPos = new(ball.x, ball.y);
    }
  }

  void drawLine() {
    // TODO:
  }

  void clearLine(int n) {
    Console.SetCursorPosition(0, n);
    var cc = new Char[_w];
    Array.Fill(cc, ' ');
    cc[0] = cc[_w - 1] = WallChar;
    Console.Write(new string(cc));
  }



  public void SetBallPos(float x, float y) {
    // TODO: BitImage

  }

  public void draw() {
    Char[] cc = new Char[_w];

    //TODO: PlayerPaddle draw
    //TODO: EnemyPaddle draw
    for (int i = 0; i < _h; ++i) {
        BitArray bb = (BitArray)BitImage[i].Clone();
        bb[0] = bb[_w - 1] = false;
        int anyTrue = 0;
        int j = 0;
        for(j = 1; j < bb.Length - 1; ++j) {
          if (bb[j] == true) {
            anyTrue = j;
            break;
          }
        }
        if (anyTrue > 0) {
          Array.Fill(cc, ' ');
          cc[0] = cc[_w - 1] = WallChar;
          cc[anyTrue] = BallChar;
          Console.SetCursorPosition(0, j);
          Console.Write(new string(cc));
          break;
        }
      }
  }

  public readonly Char SpaceChar = ' ';
  public readonly Char WallChar = '|';
  public readonly Char PaddleChar = '+';
  public readonly Char BallChar = 'O';
  public void DrawWalls() { 
    for (int i = 0; i < Height; ++i) {
      BitImage[i][0] = BitImage[i][_w - 1] = true;
    }
    Char[] cc = new Char[_w];
    Array.Fill(cc, ' ');
    cc[0] = cc[_w - 1] = WallChar;
    var s = new string(cc);
    for (int n = 0; n < _h; ++n) {
      Console.SetCursorPosition(0, n);
      Console.Write(s);
    }
  }
}

public class Border {
  public RectangleF area {get; init;}

  public Border(PointF point, SizeF size) {
    area = new RectangleF(point + new SizeF(1, 1), size - new SizeF(2, 2));
  }
    /// check the ball is inside the border  
  bool IsInside(BallO ball) {
    if (ball.x >= area.Left && ball.x < area.Right &&
        ball.y >= area.Top && ball.y < area.Bottom) {
            return true;
        }
    else {
        return false;
    }
  }

}