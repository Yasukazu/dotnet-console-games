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
	public static AddedDeleted ToAddedDeleted (BitArray this_one, BitArray new_one) {
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
