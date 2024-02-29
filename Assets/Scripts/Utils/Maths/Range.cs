
/// <summary>
/// Range [Start, End)
/// </summary>
[System.Serializable]
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

/// <summary>
/// Range [Start, End)
/// </summary>
[System.Serializable]
public struct Range
{
    public float Start;
    public float End;
    public Range(float start, float end)
    {
        Start = start;
        End = end;
    }
    public bool Contains(float value) => Start <= value && value < End;
    public float Random() => UnityEngine.Random.Range(Start, End);

}