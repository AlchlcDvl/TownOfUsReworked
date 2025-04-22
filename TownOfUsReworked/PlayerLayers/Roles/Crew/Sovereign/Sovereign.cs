namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Sovereign : Crew
{
    public bool Revealed { get; set; }

    protected override void Init()
    {
        base.Init();
        Alignment = Alignment.Sovereign;
    }

    /// <summary>
    /// Performs an action to be done when revealing.
    /// </summary>
    public virtual void OnReveal() {}
}