namespace TownOfUsReworked.PlayerLayers.Dispositions;

[HeaderOption(MultiMenu.LayerSubOptions)]
public sealed class Fanatic : Disposition
{
    [ToggleOption]
    private static bool FanaticKnows = true;

    [ToggleOption]
    public static bool FanaticColourSwap = false;

    private bool Turned { get; set; }
    private bool Betrayed { get; set; }
    public Faction Side { get; private set; }

    public override UColor Color
    {
        get
        {
            if (Turned)
            {
                return Side switch
                {
                    Faction.Intruder => CustomColorManager.Intruder,
                    Faction.Syndicate => CustomColorManager.Syndicate,
                    _ => ClientOptions.CustomDispColors ? CustomColorManager.Fanatic : CustomColorManager.Disposition
                };
            }

            return ClientOptions.CustomDispColors ? CustomColorManager.Fanatic : CustomColorManager.Disposition;
        }
    }
    public override string Symbol => "♠";
    public override LayerEnum Type => LayerEnum.Fanatic;
    public override Func<string> Description => () => !Turned ? "- Get attacked by either an <#FF1919FF>Intruder</color> or a <#008000FF>Syndicate</color> to join their side" : "";
    public override bool Hidden => !FanaticKnows && !Turned && !Dead;

    protected override void Init()
    {
        base.Init();
        Side = Faction.Crew;
    }

    public override void UpdatePlayer()
    {
        if (!Dead && Turned && !Betrayed && Last(Side))
            TurnBetrayer();
    }

    public void TurnFanatic(Faction faction)
    {
        var fanaticRole = Player.GetRole();
        fanaticRole.Faction = faction;
        Turned = true;

        if (CustomPlayer.Local.Is<Mystic>() || CustomPlayer.Local.Is(faction))
            Flash(CustomColorManager.Mystic);

        Side = faction;
        fanaticRole.Objectives = faction switch
        {
            Faction.Syndicate => () => Role.SyndicateWinCon,
            Faction.Intruder => () => Role.IntrudersWinCon,
            _ => () => ""
        };

        var local = CustomPlayer.Local.GetRole();

        if (Snitch.SnitchSeesFanatic)
        {
            foreach (var snitch in GetLayers<Snitch>())
            {
                if (snitch.TasksLeft <= Snitch.SnitchTasksRemaining && Local)
                    local.AllArrows.Add(snitch.PlayerId, new(Player, snitch.Player, snitch.Color));
                else if (snitch.TasksDone && snitch.Local)
                    snitch.Player.GetRole().AllArrows.Add(PlayerId, new(snitch.Player, Player, snitch.Color));
            }
        }

        if (Revealer.RevealerRevealsTraitor && Local)
        {
            foreach (var revealer in GetLayers<Revealer>())
            {
                if (revealer.Revealed)
                    local.AllArrows.Add(revealer.PlayerId, new(Player, revealer.Player, revealer.Color));
            }
        }

        if (CustomPlayer.Local.Is<Mystic>() && !Local)
            Flash(CustomColorManager.Mystic);

        if (Local || CustomPlayer.Local.Is(faction))
            Flash(CustomColorManager.Fanatic);

        if (Local)
            fanaticRole.UpdateButtons();
    }

    public void TurnBetrayer()
    {
        var role = Player.GetRole();
        Betrayed = true;

        if (role.Type != LayerEnum.Betrayer)
            new Betrayer { Objectives = role.Objectives }.RoleUpdate(role);
    }
}