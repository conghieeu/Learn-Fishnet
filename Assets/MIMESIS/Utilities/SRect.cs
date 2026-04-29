using System;

public struct SRect
{
	public int Left;

	public int Right;

	public int Top;

	public int Bottom;

	public int Width => Right - Left;

	public int Height => Bottom - Top;

	public SRect(SPoint a, SPoint b)
	{
		Left = Math.Min(a.X, b.X);
		Top = Math.Min(a.Y, b.Y);
		Right = Math.Max(a.X, b.X);
		Bottom = Math.Max(a.Y, b.Y);
	}

	public bool Contains(SPoint point)
	{
		if (Left <= point.X && point.X < Right && Top <= point.Y)
		{
			return point.Y < Bottom;
		}
		return false;
	}
}
