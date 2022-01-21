using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static System.ConsoleColor;
/* using Konsole.Internal;
using Konsole.Drawing;
using Konsole.Forms;
using Konsole.Menus;
using Konsole.Platform; */
using Konsole;

// void Wait() => Console.ReadKey(true); // declared but never used.
Console.Clear();
Console.Write("Hit Esc key to exit:");
// IConsole _gameBox = Window.OpenBox(0, 1, 8, 12);
var gameBox = new SafeBox(0, 1, 8, 12);
gameBox.PrintAt(gameBox.WindowWidth - 1, gameBox.WindowHeight - 1, 'o');
gameBox.PrintAt(gameBox.WindowWidth, gameBox.WindowHeight, 'o'); // invalid
gameBox.PrintAt(gameBox.WindowWidth + 1, gameBox.WindowHeight + 1, 'o'); // invalid

var nyse = Window.OpenBox("NYSE", 20, 12, new BoxStyle() { 
    ThickNess = LineThickNess.Single, 
    Title = new Colors(White, Red) 
});

Console.WriteLine("line two");

// create another inline Box window at the current cursor position
var ftse100 = Window.OpenBox("FTSE 100", 20, 12, new BoxStyle() { 
    ThickNess = LineThickNess.Double, 
    Title = new Colors(White, Blue) 
});
Console.Write("line three");

public class SafeBox {
    public int WindowHeight => box.WindowHeight;
    public int WindowWidth => box.WindowWidth;
    public IConsole box {get; init;}
    public SafeBox(int atX, int atY, int withX, int withY){
        box = Window.OpenBox("", atX, atY, withX, withY);
    }
    public void PrintAt(int x, int y, char c){
        if(x < 0)
            throw new LeftOutBoxException();
        else if( x >= box.WindowWidth)
            throw new RightOutBoxException();
         else if( y < 0 )
            throw new UpOutBoxException();
           else if( y >= box.WindowHeight)
            throw new DownOutBoxException();
        box.PrintAt(x, y, c);
    }
}

public class OutOfBoxException : ArgumentOutOfRangeException {}
public class LeftOutBoxException : OutOfBoxException {}
public class RightOutBoxException : OutOfBoxException {}
public class UpOutBoxException : OutOfBoxException {}
public class DownOutBoxException : OutOfBoxException {}