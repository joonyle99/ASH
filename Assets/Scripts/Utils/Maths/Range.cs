
/// <summary>
/// Range [Start, End)
/// </summary>
struct RangeInt
{
    public int Start;
    public int End;
    public RangeInt(int start, int end)
    {
        Start = start;
        End = end;
    }
    public bool Contains(int index) => Start <= index && index < End;
}