namespace TownOfUsReworked.PlayerLayers.Dispositions;

public abstract class FactionChanger : Disposition
{
    public bool Turned { get; set; }
    public Faction Side { get; private set; }
    protected bool Betrayed { get; set; }

    public abstract bool SheriffSwap { get; }
    public abstract bool RevealerReveals { get; }
    public abstract bool SnitchReveals { get; }

    public override void Init() => Side = Handler.CurrentFaction;

    public override void UpdatePlayer()
    {
        if (!Dead && Turned && !Betrayed && Last(Side))
            TurnBetrayer();
    }

    public void TurnFaction(Faction faction)
    {
        if (faction == Faction.Crew)
            return;

        Side = faction;
        Turned = true;
        var local = LayerHandler.Handlers[LocalPlayer.PlayerId];

        if (SnitchReveals)
        {
            foreach (var snitch in GetLayers<Snitch>())
            {
                if (snitch.TasksLeft <= Snitch.SnitchTasksRemaining && Local)
                    local.AllArrows.Add(snitch.PlayerId, new(Player, snitch.Player, snitch.Color));
                else if (snitch.TasksDone && snitch.Local)
                    LayerHandler.Handlers[snitch.PlayerId].AllArrows.Add(PlayerId, new(snitch.Player, Player, snitch.Color));
            }
        }

        if (RevealerReveals && Local)
        {
            foreach (var revealer in GetLayers<Revealer>())
            {
                if (revealer.Revealed)
                    local.AllArrows.Add(revealer.PlayerId, new(Player, revealer.Player, revealer.Color));
            }
        }

        if (LocalPlayer.Is<Mystic>() && !Local)
            Flash(CustomColorManager.Mystic);

        if (Local || LocalPlayer.Is(faction))
            Flash(CustomColorManager.Fanatic);
    }

    public void TurnBetrayer()
    {
        var role = Player.GetRole();
        Betrayed = true;

        if (role.Type != LayerEnum.Betrayer)
            new Betrayer { Objectives = role.Objectives }.RoleUpdate(role);
    }
}