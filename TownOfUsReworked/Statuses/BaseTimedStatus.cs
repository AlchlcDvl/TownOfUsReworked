namespace TownOfUsReworked.Statuses;

/// <summary>
/// Base class for statuses that have a certain duration to last.
/// </summary>
public abstract class BaseTimedStatus : BaseStatus
{
    /// <summary>
    /// Gets the duration of the timed status.
    /// </summary>
    public abstract float Duration { get; }

    /// <summary>
    /// Gets a value indicating whether or not the status is removed once the timer ends.
    /// </summary>
    public virtual bool RemoveOnTimerEnd => true;

    /// <summary>
    /// Gets a value indicating whether or not the status is automatically started once assigned.
    /// </summary>
    public virtual bool AutoStart => true;

    /// <summary>
    /// Gets or sets the timer for the status.
    /// </summary>
    public float Timer { get; set; }

    /// <summary>
    /// Gets a value indicating whether or not the timed status is currently active and running.
    /// </summary>
    public bool Active => Timer > 0;

    /// <summary>
    /// The event that's invoked once the timer's ended.
    /// </summary>
    public virtual void OnTimerEnd()
    {
        if (RemoveOnTimerEnd)
            Handler.RemoveStatus(this);
    }

    /// <inheritdoc/>
    public override void OnAdd()
    {
        if (AutoStart)
            Timer = Duration;
    }
}