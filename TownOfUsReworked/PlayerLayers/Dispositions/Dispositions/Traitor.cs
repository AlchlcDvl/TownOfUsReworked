namespace TownOfUsReworked.PlayerLayers.Dispositions;

[LayerHeaderOption(LayerEnum.Traitor)]
public sealed class Traitor : Disposition
{
    [ToggleOption]
    private static bool TraitorKnows = true;

    [ToggleOption]
    public static bool TraitorColourSwap = false;

    private bool Turned { get; set; }
    private bool Betrayed { get; set; }
    public Faction Side { get; private set; }

    protected override UColor MainColor => Turned
        ? (Side switch
        {
            Faction.Intruder => CustomColorManager.Intruder,
            Faction.Syndicate => CustomColorManager.Syndicate,
            _ => CustomColorManager.Fanatic
        })
        : CustomColorManager.Fanatic;
    public override string Symbol => "♣";
    public override LayerEnum Type => LayerEnum.Traitor;
    public override Func<string> Description => () => !Turned ? "- Finish your tasks to join either the <#FF1919FF>Intruders</color> or the <#008000FF>Syndicate</color>" : "";
    public override bool Hidden => !TraitorKnows && !Turned && !Dead;

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

    public override void UponTaskComplete(uint taskId)
    {
        if (!TasksDone || !Local)
            return;

        GetFactionChoice(out var syndicate, out var intruder);
        CallRpc(CustomRPC.Misc, MiscRPC.ChangeRoles, this, false, syndicate, intruder);
        TurnTraitor(syndicate, intruder);
    }

    public void TurnBetrayer()
    {
        var role = Player.GetRole();
        Betrayed = true;

        if (role.Type != LayerEnum.Betrayer)
            new Betrayer() { Objectives = role.Objectives }.RoleUpdate(role);
    }

    private static void GetFactionChoice(out bool turnSyndicate, out bool turnIntruder)
    {
        turnIntruder = false;
        turnSyndicate = false;

        var intAlive = AllPlayers().Count(x => x.Is(Faction.Intruder) && !x.HasDied());
        var synAlive = AllPlayers().Count(x => x.Is(Faction.Syndicate) && !x.HasDied());

        switch (intAlive)
        {
            case > 0 when synAlive > 0:
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

                break;
            }
            case > 0 when synAlive == 0:
            {
                turnIntruder = true;
                break;
            }
            default:
            {
                if (synAlive > 0 && intAlive == 0)
                    turnSyndicate = true;

                break;
            }
        }
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
        var local = CustomPlayer.Local.GetRole();

        if (Snitch.SnitchSeesTraitor)
        {
            foreach (var snitch in GetLayers<Snitch>())
            {
                if (snitch.TasksLeft <= Snitch.SnitchTasksRemaining && Local)
                    local.AllArrows.Add(snitch.PlayerId, new(Player, snitch.Player, snitch.Color));
                else if (snitch.TasksDone && snitch.Local)
                    snitch.Player.GetRole().AllArrows.Add(Player.PlayerId, new(snitch.Player, Player, snitch.Color));
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

        if (Local || CustomPlayer.Local.Is(traitorRole.Faction))
            Flash(CustomColorManager.Traitor);

        if (Local)
            traitorRole.UpdateButtons();
    }
}