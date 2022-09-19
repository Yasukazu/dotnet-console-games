// See https://aka.ms/new-console-template for more information
// Ping main
global using System;
global using System.Collections;
global using System.Collections.Generic;
global using System.Diagnostics;
// global using System.Threading.Tasks;
global using System.Drawing;

using ping;
using pong;

// int w, h;
// (w, h) = pong.OnScreen.init();
Console.Clear();
IntervalEvent intervalEvent = new();
BallO ball = new ();
ball.speedUp(13.5f, -24.4f);
var screen = new VirtualScreen(intervalEvent, ball);
screen.draw();
intervalEvent.step();
// screen.BallMoveTo(ball.moveTo(screen.Width / 2, screen.Height / 2));
screen.draw();
// Debug.Print($"Ball2={ball.ToString()}");
ball.moveBy(1.1f, 2.3f);
screen.draw();
// Debug.Print($"Ball2={ball.ToString()}");
namespace ping {
public record struct Dim2f(float x, float y);
public class IntervalEvent {
  /// Event handler
  public event EventHandler Step = delegate {};
  public void step() {
    if (Step != null) {
      Step(this, EventArgs.Empty);
    }
  }
}
public class VirtualScreen {
  RectangleF window;
  pong.Screen screen;

  public RectangleF Window { get => window;}
  readonly int _w; // width
  readonly int _h; // height
  enum ScreenItem {
    None, Wall, Ball, Paddle
  }
  BitArray[] bitImage;

  public float Width {
    get => window.Width;
  }
  public float Height {
    get => window.Height;
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
    ball.moveTo(screen.dim.x / 2, screen.dim.y / 2);
    lastBallPos = new(ball.x, ball.y);
    window = new RectangleF(new PointF(0, 0), new SizeF(_w, _h));
    bitImage = new BitArray[_h];
    for (int i = 0; i < _h; ++i) {
      bitImage[i] = new BitArray(_w);
    }
    this.intervalEvent = intervalEvent;
    intervalEvent.Step += new EventHandler(ball.step);
    intervalEvent.Step += new EventHandler(step);
  }

  void step(object sender, System.EventArgs e) {
    Debug.Print("VirtualScreen.step();");
    if (lastBallPos != ball.getPoint()) {
      // clear old ball image
      var oldLine = (int)Math.Round(ball.y);
      var bb = bitImage[oldLine];
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
        BitArray bb = (BitArray)bitImage[i].Clone();
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
      bitImage[i][0] = bitImage[i][_w - 1] = true;
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
}