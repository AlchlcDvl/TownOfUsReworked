namespace TownOfUsReworked.PlayerLayers.Objectifiers;

public class Traitor : Objectifier
{
    public bool Turned { get; set; }
    public bool Betrayed { get; set; }
    public Faction Side { get; set; }
    public bool Betray => ((Side == Faction.Intruder && LastImp) || (Side == Faction.Syndicate && LastSyn)) && !IsDead && Turned && !Betrayed;

    public override Color Color
    {
        get
        {
            if (Turned)
            {
                if (Side == Faction.Syndicate)
                    return Colors.Syndicate;
                else if (Side == Faction.Intruder)
                    return Colors.Intruder;
                else
                    return ClientGameOptions.CustomObjColors ? Colors.Traitor : Colors.Objectifier;
            }
            else
                return ClientGameOptions.CustomObjColors ? Colors.Traitor : Colors.Objectifier;
        }
    }
    public override string Name => "Traitor";
    public override string Symbol => "♣";
    public override LayerEnum Type => LayerEnum.Traitor;
    public override Func<string> Description => () => !Turned ? "- Finish your tasks to switch sides to either <color=#FF0000FF>Intruders</color> or the <color=#008000FF>Syndicate</color>" :
        "";
    public override bool Hidden => !CustomGameOptions.TraitorKnows && !Turned && !IsDead;

    public Traitor(PlayerControl player) : base(player) => Side = Faction.Crew;

    public void TurnBetrayer()
    {
        var role = Role.GetRole(Player);
        Betrayed = true;

        if (role.Type == LayerEnum.Betrayer)
            return;

        var betrayer = new Betrayer(Player) { Objectives = role.Objectives };
        betrayer.RoleUpdate(role);
    }

    public static void GetFactionChoice(out bool turnSyndicate, out bool turnIntruder)
    {
        turnIntruder = false;
        turnSyndicate = false;

        var IntrudersAlive = CustomPlayer.AllPlayers.Count(x => x.Is(Faction.Intruder) && !x.HasDied());
        var SyndicateAlive = CustomPlayer.AllPlayers.Count(x => x.Is(Faction.Syndicate) && !x.HasDied());

        if (IntrudersAlive > 0 && SyndicateAlive > 0)
        {
            var random = URandom.RandomRangeInt(0, 100);

            if (IntrudersAlive == SyndicateAlive)
            {
                turnIntruder = random < 50;
                turnSyndicate = random >= 50;
            }
            else if (IntrudersAlive > SyndicateAlive)
            {
                turnIntruder = random < 25;
                turnSyndicate = random >= 25;
            }
            else if (IntrudersAlive < SyndicateAlive)
            {
                turnIntruder = random < 75;
                turnSyndicate = random >= 75;
            }
        }
        else if (IntrudersAlive > 0 && SyndicateAlive == 0)
            turnIntruder = true;
        else if (SyndicateAlive > 0 && IntrudersAlive == 0)
            turnSyndicate = true;
    }

    public void TurnTraitor(bool turnSyndicate, bool turnIntruder)
    {
        var traitorRole = Role.GetRole(Player);

        if (turnIntruder)
        {
            traitorRole.Faction = Faction.Intruder;
            traitorRole.IsIntTraitor = true;
            traitorRole.FactionColor = Colors.Intruder;
            traitorRole.Objectives = () => Role.IntrudersWinCon;
        }
        else if (turnSyndicate)
        {
            traitorRole.Faction = Faction.Syndicate;
            traitorRole.IsSynTraitor = true;
            traitorRole.FactionColor = Colors.Syndicate;
            traitorRole.Objectives = () => Role.SyndicateWinCon;
        }

        Side = traitorRole.Faction;
        Turned = true;
        traitorRole.Alignment = traitorRole.Alignment.GetNewAlignment(traitorRole.Faction);

        foreach (var snitch in Ability.GetAbilities<Snitch>(LayerEnum.Snitch))
        {
            if (CustomGameOptions.SnitchSeesTraitor)
            {
                if (snitch.TasksLeft <= CustomGameOptions.SnitchTasksRemaining && CustomPlayer.Local == Player)
                    Role.LocalRole.AllArrows.Add(snitch.PlayerId, new(Player, Colors.Snitch));
                else if (snitch.TasksDone && CustomPlayer.Local == snitch.Player)
                    Role.GetRole(snitch.Player).AllArrows.Add(Player.PlayerId, new(snitch.Player, Colors.Snitch));
            }
        }

        foreach (var revealer in Role.GetRoles<Revealer>(LayerEnum.Revealer))
        {
            if (revealer.Revealed && CustomGameOptions.RevealerRevealsTraitor && Local)
                Role.LocalRole.AllArrows.Add(revealer.PlayerId, new(Player, revealer.Color));
        }

        if (CustomPlayer.Local.Is(LayerEnum.Mystic) && !Local)
            Flash(Colors.Mystic);

        if (Local || CustomPlayer.Local.Is(traitorRole.Faction))
            Flash(Colors.Traitor);
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);

        if (Betray && Turned)
        {
            CallRpc(CustomRPC.Change, TurnRPC.TurnTraitorBetrayer, this);
            TurnBetrayer();
        }
    }
}