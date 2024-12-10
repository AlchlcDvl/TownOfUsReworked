namespace TownOfUsReworked.PlayerLayers.Dispositions;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Traitor : Disposition
{
    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool TraitorKnows { get; set; } = true;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool TraitorColourSwap { get; set; } = false;

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
                return ClientOptions.CustomDispColors ? CustomColorManager.Traitor : CustomColorManager.Disposition;
        }
    }
    public override string Name => "Traitor";
    public override string Symbol => "♣";
    public override LayerEnum Type => LayerEnum.Traitor;
    public override Func<string> Description => () => !Turned ? "- Finish your tasks to join either the <#FF1919FF>Intruders</color> or the <#008000FF>Syndicate</color>" : "";
    public override bool Hidden => !TraitorKnows && !Turned && !Dead;

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

    public override void UponTaskComplete(uint taskId)
    {
        if (TasksDone && Local)
        {
            GetFactionChoice(out var syndicate, out var intruder);
            CallRpc(CustomRPC.Misc, MiscRPC.ChangeRoles, this, false, syndicate, intruder);
            TurnTraitor(syndicate, intruder);
        }
    }

    public void TurnBetrayer()
    {
        var role = Player.GetRole();
        Betrayed = true;

        if (role.Type != LayerEnum.Betrayer)
            new Betrayer() { Objectives = role.Objectives }.RoleUpdate(role, Player);
    }

    public static void GetFactionChoice(out bool turnSyndicate, out bool turnIntruder)
    {
        turnIntruder = false;
        turnSyndicate = false;

        var intAlive = AllPlayers().Count(x => x.Is(Faction.Intruder) && !x.HasDied());
        var synAlive = AllPlayers().Count(x => x.Is(Faction.Syndicate) && !x.HasDied());

        if (intAlive > 0 && synAlive > 0)
        {
            var random = URandom.RandomRangeInt(0, 100);

            if (intAlive == synAlive)
            {
                turnIntruder = random >= 50;
                turnSyndicate = random > 50;
            }
            else if (intAlive > synAlive)
            {
                turnIntruder = random >= 75;
                turnSyndicate = random < 75;
            }
            else if (intAlive < synAlive)
            {
                turnIntruder = random >= 25;
                turnSyndicate = random < 25;
            }
        }
        else if (intAlive > 0 && synAlive == 0)
            turnIntruder = true;
        else if (synAlive > 0 && intAlive == 0)
            turnSyndicate = true;
    }

    public void TurnTraitor(bool turnSyndicate, bool turnIntruder)
    {
        var traitorRole = Player.GetRole();

        if (turnIntruder)
        {
            traitorRole.Faction = Faction.Intruder;
            traitorRole.Objectives = () => Role.IntrudersWinCon;
        }
        else if (turnSyndicate)
        {
            traitorRole.Faction = Faction.Syndicate;
            traitorRole.Objectives = () => Role.SyndicateWinCon;
        }

        Side = traitorRole.Faction;
        Turned = true;

        foreach (var snitch in GetLayers<Snitch>())
        {
            if (Snitch.SnitchSeesTraitor)
            {
                if (snitch.TasksLeft <= Snitch.SnitchTasksRemaining && Local)
                    Role.LocalRole.AllArrows.Add(snitch.PlayerId, new(Player, CustomColorManager.Snitch));
                else if (snitch.TasksDone && snitch.Local)
                    snitch.Player.GetRole().AllArrows.Add(Player.PlayerId, new(snitch.Player, CustomColorManager.Snitch));
            }
        }

        foreach (var revealer in GetLayers<Revealer>())
        {
            if (revealer.Revealed && Revealer.RevealerRevealsTraitor && Local)
                Role.LocalRole.AllArrows.Add(revealer.PlayerId, new(Player, revealer.Color));
        }

        if (CustomPlayer.Local.Is(LayerEnum.Mystic) && !Local)
            Flash(CustomColorManager.Mystic);

        if (Local || CustomPlayer.Local.Is(traitorRole.Faction))
            Flash(CustomColorManager.Traitor);

        if (Local)
            traitorRole.UpdateButtons();
    }
}