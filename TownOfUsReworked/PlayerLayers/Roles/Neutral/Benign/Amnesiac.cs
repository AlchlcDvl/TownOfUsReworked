
namespace TownOfUsReworked.PlayerLayers.Roles;

public class Amnesiac : Neutral
{
    public Dictionary<byte, CustomArrow> BodyArrows { get; set; }
    public CustomButton RememberButton { get; set; }

    public override Color Color => ClientGameOptions.CustomNeutColors ? Colors.Amnesiac : Colors.Neutral;
    public override string Name => "Amnesiac";
    public override LayerEnum Type => LayerEnum.Amnesiac;
    public override Func<string> StartText => () => "You Forgor <i>:skull:</i>";
    public override Func<string> Description => () => "- You can copy over a player's role should you find their body" + (CustomGameOptions.RememberArrows ? ("\n- When someone dies, " +
        "you get an arrow pointing to their body") : "") + "\n- If there are less than 6 players alive, you will become a <color=#80FF00FF>Thief</color>";

    public Amnesiac(PlayerControl player) : base(player)
    {
        Alignment = Alignment.NeutralBen;
        Objectives = () => "- Find a dead body, remember their role and then fulfill the win condition for that role";
        BodyArrows = new();
        RememberButton = new(this, "Remember", AbilityTypes.Dead, "ActionSecondary", Remember);
    }

    public void DestroyArrow(byte targetPlayerId)
    {
        BodyArrows.FirstOrDefault(x => x.Key == targetPlayerId).Value?.Destroy();
        BodyArrows.Remove(targetPlayerId);
    }

    public override void OnLobby()
    {
        base.OnLobby();
        BodyArrows.Values.ToList().DestroyAll();
        BodyArrows.Clear();
    }

    public void TurnThief() => new Thief(Player).RoleUpdate(this);

    public void Remember()
    {
        var player = PlayerByBody(RememberButton.TargetBody);
        Spread(Player, player);
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction2, this, player);
        Remember(player);
    }

    public override void ReadRPC(MessageReader reader) => Remember(reader.ReadPlayer());

    public void Remember(PlayerControl other)
    {
        var role = GetRole(other);
        var target = other.GetTarget();
        var leader = other.GetLeader();
        var actor = other.GetActorList();
        var player = Player;

        if (CustomPlayer.Local == player || CustomPlayer.Local == other)
        {
            Flash(Colors.Amnesiac);
            role.OnLobby();
            OnLobby();
            ButtonUtils.ResetCustomTimers();
        }

        Role newRole = role.Type switch
        {
            LayerEnum.Anarchist => new Anarchist(player),
            LayerEnum.Arsonist => new Arsonist(player) { Doused = ((Arsonist)role).Doused },
            LayerEnum.Blackmailer => new Blackmailer(player) { BlackmailedPlayer = ((Blackmailer)role).BlackmailedPlayer },
            LayerEnum.Bomber => new Bomber(player),
            LayerEnum.Camouflager => new Camouflager(player),
            LayerEnum.Cannibal => new Cannibal(player) { EatNeed = ((Cannibal)role).EatNeed },
            LayerEnum.Enforcer => new Enforcer(player),
            LayerEnum.Concealer => new Concealer(player),
            LayerEnum.Consigliere => new Consigliere(player),
            LayerEnum.Consort => new Consort(player),
            LayerEnum.Crewmate => new Crewmate(player),
            LayerEnum.Cryomaniac => new Cryomaniac(player) { Doused = ((Cryomaniac)role).Doused },
            LayerEnum.Detective => new Detective(player),
            LayerEnum.Disguiser => new Disguiser(player),
            LayerEnum.Dracula => new Dracula(player) { Converted = ((Dracula)role).Converted },
            LayerEnum.Escort => new Escort(player),
            LayerEnum.Executioner => new Executioner(player) { TargetPlayer = target },
            LayerEnum.Framer => new Framer(player) { Framed = ((Framer)role).Framed },
            LayerEnum.Glitch => new Glitch(player),
            LayerEnum.Godfather => new Godfather(player),
            LayerEnum.PromotedGodfather => new PromotedGodfather(player) { BlackmailedPlayer = ((PromotedGodfather)role).BlackmailedPlayer },
            LayerEnum.Grenadier => new Grenadier(player),
            LayerEnum.GuardianAngel => new GuardianAngel(player) { TargetPlayer = target },
            LayerEnum.Impostor => new Impostor(player),
            LayerEnum.Bastion => new Bastion(player) { BombedIDs = ((Bastion)role).BombedIDs },
            LayerEnum.Jackal => new Jackal(player)
            {
                Recruited = ((Jackal)role).Recruited,
                EvilRecruit = ((Jackal)role).EvilRecruit,
                GoodRecruit = ((Jackal)role).GoodRecruit,
                BackupRecruit = ((Jackal)role).BackupRecruit
            },
            LayerEnum.Jester => new Jester(player),
            LayerEnum.Juggernaut => new Juggernaut(player),
            LayerEnum.Sheriff => new Sheriff(player),
            LayerEnum.Mafioso => new Mafioso(player) { Godfather = (Godfather)leader },
            LayerEnum.Miner => new Miner(player),
            LayerEnum.Morphling => new Morphling(player),
            LayerEnum.Medic => new Medic(player),
            LayerEnum.Medium => new Medium(player),
            LayerEnum.Shifter => new Shifter(player),
            LayerEnum.Rebel => new Rebel(player),
            LayerEnum.PromotedRebel => new PromotedRebel(player) { Framed = ((PromotedRebel)role).Framed },
            LayerEnum.Sidekick => new Sidekick(player) { Rebel = (Rebel)leader },
            LayerEnum.Shapeshifter => new Shapeshifter(player),
            LayerEnum.Murderer => new Murderer(player),
            LayerEnum.Survivor => new Survivor(player),
            LayerEnum.Plaguebearer => new Plaguebearer(player) { Infected = ((Plaguebearer)role).Infected },
            LayerEnum.Pestilence => new Pestilence(player),
            LayerEnum.SerialKiller => new SerialKiller(player),
            LayerEnum.Werewolf => new Werewolf(player),
            LayerEnum.Janitor => new Janitor(player),
            LayerEnum.Poisoner => new Poisoner(player),
            LayerEnum.Teleporter => new Teleporter(player),
            LayerEnum.Troll => new Troll(player),
            LayerEnum.Thief => new Thief(player),
            LayerEnum.VampireHunter => new VampireHunter(player),
            LayerEnum.Warper => new Warper(player),
            LayerEnum.Wraith => new Wraith(player),
            LayerEnum.Mystic => new Mystic(player),
            LayerEnum.Dictator => new Dictator(player),
            LayerEnum.Seer => new Seer(player),
            LayerEnum.Actor => new Actor(player) { PretendRoles = actor },
            LayerEnum.BountyHunter => new BountyHunter(player) { TargetPlayer = target },
            LayerEnum.Guesser => new Guesser(player) { TargetPlayer = target },
            LayerEnum.Necromancer => new Necromancer(player) { Resurrected = ((Necromancer)role).Resurrected },
            LayerEnum.Whisperer => new Whisperer(player) { Persuaded = ((Whisperer)role).Persuaded },
            LayerEnum.Betrayer => new Betrayer(player) { Faction = role.Faction },
            LayerEnum.Ambusher => new Ambusher(player),
            LayerEnum.Crusader => new Crusader(player),
            LayerEnum.Altruist => new Altruist(player),
            LayerEnum.Engineer => new Engineer(player),
            LayerEnum.Tracker => new Tracker(player),
            LayerEnum.Stalker => new Stalker(player),
            LayerEnum.Transporter => new Transporter(player),
            LayerEnum.Mayor => new Mayor(player),
            LayerEnum.Operative => new Operative(player),
            LayerEnum.Veteran => new Veteran(player),
            LayerEnum.Vigilante => new Vigilante(player),
            LayerEnum.Chameleon => new Chameleon(player),
            LayerEnum.Coroner => new Coroner(player),
            LayerEnum.Monarch => new Monarch(player)
            {
                ToBeKnighted = ((Monarch)role).ToBeKnighted,
                Knighted = ((Monarch)role).Knighted
            },
            LayerEnum.Retributionist => new Retributionist(player)
            {
                Selected = ((Retributionist)role).Selected,
                ShieldedPlayer = ((Retributionist)role).ShieldedPlayer,
                BombedIDs = ((Retributionist)role).BombedIDs
            },
            _ => new Amnesiac(player),
        };

        newRole.RoleUpdate(this, Faction == Faction.Neutral);

        if (other.Is(LayerEnum.Dracula))
            ((Dracula)role).Converted.Clear();
        else if (other.Is(LayerEnum.Whisperer))
            ((Whisperer)role).Persuaded.Clear();
        else if (other.Is(LayerEnum.Necromancer))
            ((Necromancer)role).Resurrected.Clear();
        else if (other.Is(LayerEnum.Jackal))
        {
            ((Jackal)role).Recruited.Clear();
            ((Jackal)role).EvilRecruit = null;
            ((Jackal)role).GoodRecruit = null;
            ((Jackal)role).BackupRecruit = null;
        }

        if (player.Is(Faction.Intruder) || player.Is(Faction.Syndicate) || (player.Is(Faction.Neutral) && CustomGameOptions.SnitchSeesNeutrals))
        {
            foreach (var snitch in GetLayers<Snitch>())
            {
                if (snitch.TasksLeft <= CustomGameOptions.SnitchTasksRemaining && CustomPlayer.Local == player)
                    LocalRole.AllArrows.Add(snitch.PlayerId, new(player, Colors.Snitch));
                else if (snitch.TasksDone && CustomPlayer.Local == snitch.Player)
                    GetRole(snitch.Player).AllArrows.Add(player.PlayerId, new(snitch.Player, Colors.Snitch));
            }

            foreach (var revealer in GetLayers<Revealer>())
            {
                if (revealer.Revealed && CustomPlayer.Local == player)
                    LocalRole.AllArrows.Add(revealer.PlayerId, new(player, Colors.Revealer));
            }
        }

        if (CustomPlayer.Local == player || CustomPlayer.Local == other)
            ButtonUtils.ResetCustomTimers();
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        RememberButton.Update2("REMEMBER");

        if (CustomGameOptions.RememberArrows && !CustomPlayer.LocalCustom.IsDead)
        {
            var validBodies = AllBodies.Where(x => KilledPlayers.Any(y => y.PlayerId == x.ParentId && y.KillTime.AddSeconds(CustomGameOptions.RememberArrowDelay) < DateTime.UtcNow));

            foreach (var bodyArrow in BodyArrows.Keys)
            {
                if (!validBodies.Any(x => x.ParentId == bodyArrow))
                    DestroyArrow(bodyArrow);
            }

            foreach (var body in validBodies)
            {
                if (!BodyArrows.ContainsKey(body.ParentId))
                    BodyArrows.Add(body.ParentId, new(Player, Color));

                BodyArrows[body.ParentId]?.Update(body.TruePosition);
            }
        }
        else if (BodyArrows.Count != 0 || CustomPlayer.AllPlayers.Count <= 4)
            OnLobby();

        if (CustomPlayer.AllPlayers.Count <= 4 && !IsDead)
        {
            CallRpc(CustomRPC.Change, TurnRPC.TurnThief, this);
            TurnThief();
        }
    }
}