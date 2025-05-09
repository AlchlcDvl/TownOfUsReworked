namespace TownOfUsReworked.Statuses;

/// <summary>
/// Base class for status effects on a player. Based on the modifier system from MiraAPI
/// </summary>
public abstract class BaseStatus
{
    public static readonly List<BaseStatus> AllStatuses = [];

    /// <summary>
    /// Gets the value indicating whether or not multiple statuses of the same type can be applied on the same player.
    /// </summary>
    public virtual bool AllowMultiple => false;

    /// <summary>
    /// Gets or sets the player.
    /// </summary>
    public PlayerControl Player { get; private set; }

    /// <summary>
    /// Gets or sets the handler for this status.
    /// </summary>
    protected StatusHandler Handler { get; private set; }

    /// <summary>
    /// Starts the status by attaching it to the player and invoking the OnAdd event.
    /// </summary>
    /// <param name="player">The player to attach to.</param>
    public void Start(PlayerControl player)
    {
        Player = player;
        Handler = player.GetComponent<StatusHandler>();
        Handler.AddStatus(this);
        AllStatuses.Add(this);
    }

    /// <summary>
    /// Ends the status by detaching from the player and invoking the OnRemove event.
    /// </summary>
    public void End()
    {
        Handler.RemoveStatus(this);
        Player = null;
        AllStatuses.Remove(this);
    }

    /// <summary>
    /// Performs an action when attached to the player.
    /// </summary>
    public virtual void OnAdd() {}

    /// <summary>
    /// Performs an action when detached from the player.
    /// </summary>
    public virtual void OnRemove() {}

    /// <summary>
    /// Performs an action when updating frames. Runs for every player.
    /// </summary>
    public virtual void OnPlayerUpdate() {}

    /// <summary>
    /// Performs an action when updating frames. Runs only if the local player has this status.
    /// </summary>
    public virtual void OnLocalUpdate() {}

    /// <summary>
    /// Null check.
    /// </summary>
    public static implicit operator bool(BaseStatus exists) => exists is not null;
}