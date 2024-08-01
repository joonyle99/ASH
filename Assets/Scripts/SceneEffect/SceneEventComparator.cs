using System.Collections.Generic;

public class SceneEventComparator : IComparer<SceneEffectEvent>
{
    public int Compare(SceneEffectEvent first, SceneEffectEvent second)
    {
        if (first.Priority <= second.Priority)
            return -1;
        return 1;
    }
}
