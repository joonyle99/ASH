using UnityEngine;

[System.Serializable]
public class FollowObjects : SceneEffectEvent
{
    private Transform[] _targets;

    public FollowObjects(EventPriority priority, MergePolicy mergePolicyWithSamePriority, params Transform[] targets)
    {
        Priority = priority;
        MergePolicyWithSamePriority = mergePolicyWithSamePriority;

        _targets = targets;
    }
    public override void OnEnter()
    {
        //string str = "\n";
        //foreach (var target in _targets)
        //    str += target.name + "\n";
        //Debug.Log($"<color=red><b>Add Follow Targets</b></color>: {str}");
        SceneEffectManager.Instance.Camera.AddFollowTargets(_targets);
    }
    public override void OnExit()
    {
        //string str = "\n";
        //foreach (var target in _targets)
        //    str += target.name + "\n";
        //Debug.Log($"<color=blue><b>Remove Follow Targets</b></color>: {str}");
        SceneEffectManager.Instance.Camera.RemoveFollowTargets(_targets);
    }
}