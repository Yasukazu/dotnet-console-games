// See https://aka.ms/new-console-template for more information
// Ping main
global using System;
global using System.Collections;
global using System.Collections.Generic;
global using System.Diagnostics;
global using System.Threading.Tasks;
global using System.IO;
global using System.Drawing;

using ping;
using pong;

int w, h;
Console.Clear();
(w, h) = pong.OnScreen.init();
var vScreen0 = new VirtualScreen();
var vScreen = new VirtualScreen(80, 16);
vScreen.DrawWalls();
var screen = new pong.Screen(80, 16, false);
BallO b2 = new ();
Debug.Print($"Ball2={b2.ToString()}");
b2.moveBy(1.1f, 2.3f);
b2.speedUp(0.1f, -2.4f);
Debug.Print($"Ball2={b2.ToString()}");

public class VirtualScreen {
    RectangleF window;
    pong.Screen screen;

    public RectangleF Window { get => window;}
  int _w;
  int _h;
  public float Width {
    get => window.Width;
  }
  public float Height {
    get => window.Height;
  }
  public VirtualScreen () {
    (_w, _h) = pong.OnScreen.init();
    screen = new Screen(_w, _h);
    window = new RectangleF(new PointF(0, 0), new SizeF(_w, _h));
  }

  public VirtualScreen (int width, int height) {
    (_w, _h) = pong.OnScreen.init();
    _w = Math.Min(_w, width);
    _h = Math.Min(_h, height);
    screen = new Screen(_w, _h);
    window = new RectangleF(new PointF(0, 0), new SizeF(_w, _h));
  }

  public static Char WallChar = '|';
  public void DrawWalls() { 
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