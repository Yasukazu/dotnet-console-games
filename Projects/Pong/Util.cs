using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public enum Rotation {
	Horizontal, Vertical
}

interface ScreenDrawItem {
	BitArray GetImage();
	char DispChar{get;}
	// void Draw(int lineNum, BitArray image, char dispChar);
}
public enum PaddleSide {Home = 0, Away = -1}
public enum PaddleLine {Self = 0, Opponent = -1}


public class Player {
    public int score {get; set;}
    public Paddle paddle {get;}
}

public record Dim2(int W, int H);
public record Cood2(int X, int Y);
interface OnScreen {
	public static Cood2 dim;

	static (int, int) init(int x = 0, int y = 0) {
		var W = Console.WindowWidth;
		var H = Console.WindowHeight;
		(int _x, int _y) = (x > 0 ? x : W, y > 0 ? y : H);
		(int mx, int my) = (_x % 8, _y % 8);
		dim = new(_x - mx, _y - my);
		return (dim.X, dim.Y);
	}
}
public enum ScreenChar {O = '\u25CB',
 C = ' ', 
 B = '\u25A0', // Black square
}
interface Cood2Listable {
	List<Cood2> Cood2List();
}
interface HasDispChar {
	char DispChar();
}

interface Movable {
	void move_to(int x, int y); // move to (x, y) and redraw
	void move_by(int x, int y);
}

public enum Direction {V, H}
public enum HPos {Start, End}

interface IRender {
	char[][] render();
}

interface KeyManipulate { // key manipulate-able
	bool manipulate(System.ConsoleKey key);
}

interface IDrawOnScreen : OnScreen {
	void draw(in char [][] buffer);
}

public class EscKeyPressedException : Exception
{
    public EscKeyPressedException()
    {
    }

    public EscKeyPressedException(string message)
        : base(message)
    {
    }

    public EscKeyPressedException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
public class NestedRange {
	public Range inner {
        get{ return (offset.Value..(offset.Value + width)); }
     }
    public Slider offset{get; init;}
    public int width {get; init;}
	public Range outer {get; init;}
	public NestedRange(Range r1, Range r2) {
            var (_inner, _outer) = 
        (r1.End.Value - r1.Start.Value < r2.End.Value - r2.Start.Value) ?
            (r1, r2) : (r2, r1); // automatically makes smaller range as inner.

		if (_inner.Start.Value < _outer.Start.Value || _inner.End.Value > _outer.End.Value)
            throw new ArgumentOutOfRangeException("Inner range out of outer range!");
		// inner = _inner;
        offset = new Slider(0..(_outer.End.Value - _inner.End.Value));
        width = _inner.End.Value - _inner.Start.Value;
		outer = _outer;
	}

	public NestedRange() {
		outer = (0..1);
        offset = new Slider(0..1);
		width = 0;
	}
	public int shift(int d) {
		return offset.Move(d);
	}

	public char[] render(char element){
		var cc = new char[outer.End.Value - outer.Start.Value];
		var nn = cc.AsSpan()[inner];
		for(int i = 0; i < nn.Length; ++i)
			nn[i] = element;
		return cc;
	}
	public BitArray ToBitArray(){
		var all = new BitArray(outer.End.Value - outer.Start.Value, false);
		for (int i = 0; i < width; ++i)
			all[i + offset.Value] = true;
		return all;
	}
	
} 

public enum StartFrom {Min, Center, Max}
public class Slider
{
    public int Value {get{return value;}
	 set{
		Set(value);
	}}
	int value;
    public int Min {get {return start;} }
    public int Max {get {return end - 1;} }
	int end{get{
		return range.End.Value;
	} }
	int start {get{
		return range.Start.Value;
	} }
	Range range{get; init;}

	/// <summary>
	/// if given parameter is (1..3), a Slider travels from 1 to 2.
	/// </summary>
    public Slider(Range _range, StartFrom start_from = StartFrom.Center) {
        // if (ma < 0) throw new ArgumentOutOfRangeException("Max must not minus!");
        if (start < 0 || start >= _range.End.Value)
			throw new ArgumentOutOfRangeException($"start value({start}) is not in [0..{Max}]. ");           
        range = _range; // end = ma;
			value = start_from switch {
		StartFrom.Min => range.Start.Value,
		StartFrom.Center => (range.Start.Value + range.End.Value) / 2,
		StartFrom.Max => range.End.Value - 1,
		_ => throw new ArgumentException("")};
    }

    public bool Inc(){
        if (value == Max)
			return false;
        value += 1;
        return true;
    }
	public int Add(int n) {
		int i = 0;
		while(n-- > 0) {
			if (!Inc())
				break;
			++i;
		}
		return i;
	}
    public bool Dec(){
        if (value == start) 
			return false;
        value -= 1;
        return true;
    }

	public int Sub(int n) {
		int i = 0;
		while(n-- > 0) {
			if (!Dec())
				break;
			--i;
		}
		return i;
	}

	public int Move(int n) {
		if (n > 0) 
			return Add(n);
		else if (n < 0)
			return Sub(-n);
		return 0;
	}
    public bool Set(int nv) {
        if (nv == value)
            return false;
        if (nv <= start)
			value = start;
		else if (nv > Max)
			value = Max;
		else
        	value = nv;
        return true;
    }

	public void Center() {
		Set((Max - Min) / 2);
	}
}

static class RangeExtention
{
    public static bool Contains(this Range range, int value)
    {
        var start = range.Start.IsFromEnd ? (int.MaxValue - range.Start.Value) : range.Start.Value;
        var end = range.End.IsFromEnd ? (int.MaxValue - range.End.Value) : range.End.Value;
        if (start > end)
            throw new ArgumentOutOfRangeException(nameof(range));
        return start <= value && value < end;
    }
}
record AddedDeleted(BitArray Added, BitArray Deleted);
static class BitArrayExtention {
	// <summary>Breaks this_one</summary>
	public static AddedDeleted ToAddedDeleted (this BitArray this_one, BitArray new_one) {
		Debug.Assert(new_one != null);
		Debug.Assert(this_one != null);
		BitArray clone = this_one.Clone() as BitArray; // Clone old image1
		clone.Xor(new_one);
		return new AddedDeleted(Deleted: this_one.And(clone), Added: clone.And(new_one));
		// return new Tuple(new_one.And(this_one), clone.And(this_one));
	}
	public static void Shift(this BitArray this_one, int d) {
		if (d < 0)
			this_one.RightShift(d);
		else if (d > 0)
			this_one.LeftShift(d);
	}

	public static int ClampShift(this BitArray this_one, int n) {
		Debug.Write($"ClampSHift:{this_one.renderImage()}");
		int i = 0;
		if (n < 0) {
			n = -n;
			for(; i < n; ++i)
				if (!this_one.ClampRightShift1())
					break;
			return -i;
		}
		else if (n > 0) {
			for(; i < n; ++i)
				if (!this_one.ClampLeftShift1())
					break;
			return i;
		}
		return 0;
	}
	public static bool ClampRightShift1(this BitArray this_one) {
		Debug.Write($"ClampRightShift1:{this_one.renderImage()}");
		if (this_one[0])
			return false;
		this_one.RightShift(1);
		return true;
	}
	public static bool ClampLeftShift1(this BitArray this_one) {
		Debug.Write($"ClampLeftShift1:{this_one.renderImage()}");
		if (this_one[^1])
			return false;
		this_one.LeftShift(1);
		return true;
	}
	public static string renderImage(this BitArray image){
	char[] cc = new char[image.Length];
	for(int i=0; i < cc.Length; ++i)
		cc[i] = image[i] ? '+' : '_';
	return new String(cc);
	}
}

record ImageLineChar(BitArray Image, int Line, char Char);