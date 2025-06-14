namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Sovereign : Crew
{
    public bool Revealed;

    public override Alignment Alignment => Alignment.Sovereign;

    /// <summary>
    /// Performs an action to be done when revealing.
    /// </summary>
    public virtual void OnReveal() {}
}