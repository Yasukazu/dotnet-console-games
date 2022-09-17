namespace pong {

enum DrawDirection {Normal, Rotating}
public enum CharCode {ESC = '\x1b', SPC = '\x20', VBAR = '|', HBAR = '-', DOT = '.'}
public record struct Dimention ( int x, int y);
public record Dim2(int W, int H);
public record Cood2(int X, int Y);
public interface OnScreen {
	public static Cood2 dim = new(0, 0);

	public static (int, int) init(int x = 0, int y = 0, int align = 8) {
		if (x < 0 || y < 0) {
			throw new ArgumentException("Both args must not be minus.");
		}
		var W = Console.WindowWidth;
		var H = Console.WindowHeight;
		if (W == 0 || H == 0) {
			throw new InsufficientMemoryException("Console area is zero.");
		}
		(int _x, int _y) = (x > 0 ? Math.Min(x, W) : W, y > 0 ? Math.Min(y, H) : H);
		(int mx, int my) = (_x % align, _y % align);
		dim = new(_x - mx, _y - my);
		return (dim.X, dim.Y);
	}
}

public class Screen {
	public Dictionary<System.ConsoleKey, Func<int>> KeyManipDict = new Dictionary<ConsoleKey, Func<int>>();
	public const char BlankChar = (char)CharCode.SPC;
	public Action<int, BitArray, char> DrawImage;
	public Action<int, BitArray, char> RedrawImage; // (Line, this_array, new_array, c='+')
	public virtual bool isRotated {get; init;} // 90 degree
	protected int w {get; init;}
	protected int h {get; init;}
	public Dimention dim {get; init;}
	public int EndOfLines {get{return Lines.Length - 1;}}
	public BitArray[] Lines {get; private set;} // [h][w]
	// Gonsole console;
	Action<int, int> setCursorPosition;
	public Screen(int x = 80, int y = 24, bool rotate = false) {
		(w, h) = OnScreen.init(x, y);
		dim = new(w, h);
		isRotated = rotate;
		// console = isRotated ? new VGonsole() : HGonsole();
		setCursorPosition = isRotated ? (x, y) => Console.SetCursorPosition(y, x) : (x, y) => Console.SetCursorPosition(x, y);
		Lines = new BitArray[isRotated ? w : h];
        // for(int i = 0; i < (rotate ? w :h); ++i) buffer[i] = new BitArray(rotate ? h :w);
        RedrawImage = isRotated ? (line, new_buff, c) =>
        {
            var ad = Lines[line].ToAddedDeleted(new_buff);
            VPutCasBitArray(line, c, ad.Added);
            VPutCasBitArray(line, BlankChar, ad.Deleted);
        }
        : (line, new_buff, c) =>
        {
            var ad = Lines[line].ToAddedDeleted(new_buff);
            HPutCasBitArray(line, c, ad.Added);
            HPutCasBitArray(line, BlankChar, ad.Deleted);
        };
        DrawImage = isRotated ? (line, buff, c) =>
            VPutCasBitArray(line, c, buff)
        : (line, buff, c) =>
            HPutCasBitArray(line, c, buff);

	}

	public BitArray [] new_buffer() {
		var old_buffer = Lines;
		Lines = new BitArray[isRotated ? w : h];
		// for(int i = 0; i < h; ++i) buffer[i] = new char[w];
		return old_buffer;
	}
	/* public void drawImage(int n, BitArray image, char c){
		PutCasBitArray(this.isRotated, n, c, image);
		// Lines[n] = image;
	} */

	/// <summary>Breaks image</summary>
	public void redrawImage(int n, BitArray image, char c, char b = BlankChar){
		Debug.Assert(Lines[n] != null);
        var ad = Lines[n].ToAddedDeleted(image);
		drawImage(n, ad.Added, c);
		drawImage(n, ad.Deleted, b);
		/*
        if(this.isRotated ) {
            VPutCasBitArray(n, c, ad.Added);
            VPutCasBitArray(n, b, ad.Deleted);
        }
		else {
            HPutCasBitArray(n, c, ad.Added);
            HPutCasBitArray(n, b, ad.Deleted);
        } */
		// Lines[n] = image;
	}

	public static void PutCasBitArray(Boolean rot, int line, char c, BitArray bb) {
		if(rot)
			VPutCasBitArray(line, c, bb);
		else
			HPutCasBitArray(line, c, bb);
	}
	public void drawImage(int line, BitArray bb, char c) {
		// Debug.Write(bb.renderImage());
		for(int i = 0; i < bb.Length; ++i)
            if (bb[i]) {
				// if(isRotated) SetCursorPosition(i, line); else
                SetCursorPosition(i, line);
                Console.Write(c);
            }
    }

	public static void VPutCasBitArray(int x, char c, BitArray bb) {
		Debug.Write(bb.renderImage());
		for(int i = 0; i < bb.Length; ++i)
            if (bb[i])
            {
                Console.SetCursorPosition(x, i);
                Console.Write(c);
            }
    }

	// public void HDrawImage(Side side, BitArray image){ HPutCasBitArray(SideToLine(side), SideToChar(side), image); }
	public static void HPutCasBitArray(int y, char c, BitArray bb) {
        for (int i = 0; i < bb.Length; ++i)
            if (bb[i])
            {
                Console.SetCursorPosition(i, y);
                Console.Write(c);
            }
    }

    public void SetCursorPosition(int x, int y){
		if(isRotated)
			(y, x) = (x, y);
		else {
			y = h - 1 -y;
		}
        Console.SetCursorPosition(x, y);
    }
}
} // end pong

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
