[System.Serializable]
public class SceneEffectEvent
{
    public bool Enabled
    {
        get => _enabled;
        set
        {
            if (value != _enabled)
            {
                if (value)
                    OnEnter();
                else
                    OnExit();

                _enabled = value;
            }
        }
    }
    private bool _enabled = false;
    public enum MergePolicy
    {
        OverrideWithRecent, PlayTogether
    }
    public enum EventPriority
    {
        MovingObjects, MajorEvent
    }
    public EventPriority Priority { get; protected set; }
    public MergePolicy  MergePolicyWithSamePriority { get; protected set; }

    public virtual void OnEnter() { }
    public virtual void OnUpdate() { }
    public virtual void OnExit() { }
}