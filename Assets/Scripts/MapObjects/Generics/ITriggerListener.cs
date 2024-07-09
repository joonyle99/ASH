public interface ITriggerListener
{
    public void OnEnterReported(TriggerActivator activator, TriggerReporter reporter) { }
    public void OnExitReported(TriggerActivator activator, TriggerReporter reporter) { }
    public void OnStayReported(TriggerActivator activator, TriggerReporter reporter) { }
}
