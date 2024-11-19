namespace TownOfUsReworked.PlayerLayers.Dispositions;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Fanatic : Disposition
{
    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool FanaticKnows { get; set; } = true;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool FanaticColourSwap { get; set; } = false;

    private bool Turned { get; set; }
    private bool Betrayed { get; set; }
    public Faction Side { get; set; }
    private bool Betray => ((Side == Faction.Intruder && LastImp()) || (Side == Faction.Syndicate && LastSyn())) && !Dead && Turned && !Betrayed;

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
            else
                return ClientOptions.CustomDispColors ? CustomColorManager.Fanatic : CustomColorManager.Disposition;
        }
    }
    public override string Name => "Fanatic";
    public override string Symbol => "♠";
    public override LayerEnum Type => LayerEnum.Fanatic;
    public override Func<string> Description => () => !Turned ? "- Get attacked by either an <color=#FF1919FF>Intruder</color> or a <color=#008000FF>Syndicate</color> to join their side" :
        "";
    public override bool Hidden => !FanaticKnows && !Turned && !Dead;

    public override void Init()
    {
        base.Init();
        Side = Faction.Crew;
    }

    public override void UpdatePlayer()
    {
        if (Betray && Turned)
            TurnBetrayer();
    }

    public void TurnFanatic(Faction faction)
    {
        var fanaticRole = Player.GetRole();
        fanaticRole.Faction = faction;
        Turned = true;

        if (CustomPlayer.Local.Is(LayerEnum.Mystic) || CustomPlayer.Local.Is(faction))
            Flash(CustomColorManager.Mystic);

        Side = faction;
        fanaticRole.Objectives = faction switch
        {
            Faction.Syndicate => () => Role.SyndicateWinCon,
            Faction.Intruder => () => Role.IntrudersWinCon,
            _ => () => ""
        };
        fanaticRole.Alignment = fanaticRole.Alignment.GetNewAlignment(fanaticRole.Faction);

        foreach (var snitch in GetLayers<Snitch>())
        {
            if (Snitch.SnitchSeesFanatic)
            {
                if (snitch.TasksLeft <= Snitch.SnitchTasksRemaining && Local)
                    Role.LocalRole.AllArrows.Add(snitch.PlayerId, new(Player, CustomColorManager.Snitch));
                else if (snitch.TasksDone && snitch.Local)
                    snitch.Player.GetRole().AllArrows.Add(PlayerId, new(snitch.Player, CustomColorManager.Snitch));
            }
        }

        foreach (var revealer in GetLayers<Revealer>())
        {
            if (revealer.Revealed && Revealer.RevealerRevealsTraitor && Local)
                Role.LocalRole.AllArrows.Add(revealer.PlayerId, new(Player, revealer.Color));
        }

        if (CustomPlayer.Local.Is(LayerEnum.Mystic) && !Local)
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
            new Betrayer() { Objectives = role.Objectives }.RoleUpdate(role, Player);
    }
}