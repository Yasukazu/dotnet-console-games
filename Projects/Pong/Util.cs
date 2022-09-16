namespace pong;

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


record ImageLineChar(BitArray Image, int Line, char Char);

public class GonsoleTraceListener : ConsoleTraceListener {
	public override void Write(string s){
		var (x,y) = Console.GetCursorPosition();
		Console.SetCursorPosition(0, 0);
		Trace.WriteLine(s);
		// Console.ReadKey();
		Console.SetCursorPosition(x,y);
	}

}