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
                    _ => ClientOptions.CustomObjColors ? CustomColorManager.Fanatic : CustomColorManager.Disposition
                };
            }
            else
                return ClientOptions.CustomObjColors ? CustomColorManager.Traitor : CustomColorManager.Disposition;
        }
    }
    public override string Name => "Traitor";
    public override string Symbol => "♣";
    public override LayerEnum Type => LayerEnum.Traitor;
    public override Func<string> Description => () => !Turned ? "- Finish your tasks to join either the <color=#FF1919FF>Intruders</color> or the <color=#008000FF>Syndicate</color>" : "";
    public override bool Hidden => !TraitorKnows && !Turned && !Dead;

    public override void Init()
    {
        base.Init();
        Side = Faction.Crew;
    }

    public void TurnBetrayer()
    {
        var role = Player.GetRole();
        Betrayed = true;

        if (role.Type != LayerEnum.Betrayer)
            new Betrayer() { Objectives = role.Objectives }.Start<Role>(Player).RoleUpdate(role);
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
                turnIntruder = random < 50;
                turnSyndicate = random >= 50;
            }
            else if (intAlive > synAlive)
            {
                turnIntruder = random < 25;
                turnSyndicate = random >= 25;
            }
            else if (intAlive < synAlive)
            {
                turnIntruder = random < 75;
                turnSyndicate = random >= 75;
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
            traitorRole.FactionColor = CustomColorManager.Intruder;
            traitorRole.Objectives = () => Role.IntrudersWinCon;
        }
        else if (turnSyndicate)
        {
            traitorRole.Faction = Faction.Syndicate;
            traitorRole.FactionColor = CustomColorManager.Syndicate;
            traitorRole.Objectives = () => Role.SyndicateWinCon;
        }

        Side = traitorRole.Faction;
        Turned = true;
        traitorRole.Alignment = traitorRole.Alignment.GetNewAlignment(traitorRole.Faction);

        foreach (var snitch in GetLayers<Snitch>())
        {
            if (Snitch.SnitchSeesTraitor)
            {
                if (snitch.TasksLeft <= Snitch.SnitchTasksRemaining && CustomPlayer.Local == Player)
                    Role.LocalRole.AllArrows.Add(snitch.PlayerId, new(Player, CustomColorManager.Snitch));
                else if (snitch.TasksDone && CustomPlayer.Local == snitch.Player)
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

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);

        if (Betray && Turned)
        {
            CallRpc(CustomRPC.Misc, MiscRPC.ChangeRoles, this, true);
            TurnBetrayer();
        }
    }

    public override void UponTaskComplete(uint taskId)
    {
        base.UponTaskComplete(taskId);

        if (TasksDone)
        {
            GetFactionChoice(out var syndicate, out var intruder);
            CallRpc(CustomRPC.Misc, MiscRPC.ChangeRoles, this, false, syndicate, intruder);
            TurnTraitor(syndicate, intruder);
        }
    }
}