
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

        if (CustomPlayer.Local == Player || CustomPlayer.Local == other)
        {
            Flash(Colors.Amnesiac);
            role.OnLobby();
            OnLobby();
            ButtonUtils.ResetCustomTimers();
        }

        Role newRole = role.Type switch
        {
            LayerEnum.Anarchist => new Anarchist(Player),
            LayerEnum.Arsonist => new Arsonist(Player) { Doused = ((Arsonist)role).Doused },
            LayerEnum.Blackmailer => new Blackmailer(Player) { BlackmailedPlayer = ((Blackmailer)role).BlackmailedPlayer },
            LayerEnum.Bomber => new Bomber(Player),
            LayerEnum.Camouflager => new Camouflager(Player),
            LayerEnum.Cannibal => new Cannibal(Player) { EatNeed = ((Cannibal)role).EatNeed },
            LayerEnum.Enforcer => new Enforcer(Player),
            LayerEnum.Concealer => new Concealer(Player),
            LayerEnum.Consigliere => new Consigliere(Player),
            LayerEnum.Consort => new Consort(Player),
            LayerEnum.Crewmate => new Crewmate(Player),
            LayerEnum.Cryomaniac => new Cryomaniac(Player) { Doused = ((Cryomaniac)role).Doused },
            LayerEnum.Detective => new Detective(Player),
            LayerEnum.Disguiser => new Disguiser(Player),
            LayerEnum.Dracula => new Dracula(Player) { Converted = ((Dracula)role).Converted },
            LayerEnum.Escort => new Escort(Player),
            LayerEnum.Executioner => new Executioner(Player) { TargetPlayer = target },
            LayerEnum.Framer => new Framer(Player) { Framed = ((Framer)role).Framed },
            LayerEnum.Glitch => new Glitch(Player),
            LayerEnum.Godfather => new Godfather(Player),
            LayerEnum.PromotedGodfather => new PromotedGodfather(Player) { BlackmailedPlayer = ((PromotedGodfather)role).BlackmailedPlayer },
            LayerEnum.Grenadier => new Grenadier(Player),
            LayerEnum.GuardianAngel => new GuardianAngel(Player) { TargetPlayer = target },
            LayerEnum.Impostor => new Impostor(Player),
            LayerEnum.Bastion => new Bastion(Player) { BombedIDs = ((Bastion)role).BombedIDs },
            LayerEnum.Jackal => new Jackal(Player)
            {
                Recruited = ((Jackal)role).Recruited,
                EvilRecruit = ((Jackal)role).EvilRecruit,
                GoodRecruit = ((Jackal)role).GoodRecruit,
                BackupRecruit = ((Jackal)role).BackupRecruit
            },
            LayerEnum.Jester => new Jester(Player),
            LayerEnum.Juggernaut => new Juggernaut(Player),
            LayerEnum.Sheriff => new Sheriff(Player),
            LayerEnum.Mafioso => new Mafioso(Player) { Godfather = (Godfather)leader },
            LayerEnum.Miner => new Miner(Player),
            LayerEnum.Morphling => new Morphling(Player),
            LayerEnum.Medic => new Medic(Player),
            LayerEnum.Medium => new Medium(Player),
            LayerEnum.Shifter => new Shifter(Player),
            LayerEnum.Rebel => new Rebel(Player),
            LayerEnum.PromotedRebel => new PromotedRebel(Player) { Framed = ((PromotedRebel)role).Framed },
            LayerEnum.Sidekick => new Sidekick(Player) { Rebel = (Rebel)leader },
            LayerEnum.Shapeshifter => new Shapeshifter(Player),
            LayerEnum.Murderer => new Murderer(Player),
            LayerEnum.Survivor => new Survivor(Player),
            LayerEnum.Plaguebearer => new Plaguebearer(Player) { Infected = ((Plaguebearer)role).Infected },
            LayerEnum.Pestilence => new Pestilence(Player),
            LayerEnum.SerialKiller => new SerialKiller(Player),
            LayerEnum.Werewolf => new Werewolf(Player),
            LayerEnum.Janitor => new Janitor(Player),
            LayerEnum.Poisoner => new Poisoner(Player),
            LayerEnum.Teleporter => new Teleporter(Player),
            LayerEnum.Troll => new Troll(Player),
            LayerEnum.Thief => new Thief(Player),
            LayerEnum.VampireHunter => new VampireHunter(Player),
            LayerEnum.Warper => new Warper(Player),
            LayerEnum.Wraith => new Wraith(Player),
            LayerEnum.Mystic => new Mystic(Player),
            LayerEnum.Dictator => new Dictator(Player),
            LayerEnum.Seer => new Seer(Player),
            LayerEnum.Actor => new Actor(Player) { PretendRoles = actor },
            LayerEnum.BountyHunter => new BountyHunter(Player) { TargetPlayer = target },
            LayerEnum.Guesser => new Guesser(Player) { TargetPlayer = target },
            LayerEnum.Necromancer => new Necromancer(Player) { Resurrected = ((Necromancer)role).Resurrected },
            LayerEnum.Whisperer => new Whisperer(Player) { Persuaded = ((Whisperer)role).Persuaded },
            LayerEnum.Betrayer => new Betrayer(Player) { Faction = role.Faction },
            LayerEnum.Ambusher => new Ambusher(Player),
            LayerEnum.Crusader => new Crusader(Player),
            LayerEnum.Altruist => new Altruist(Player),
            LayerEnum.Engineer => new Engineer(Player),
            LayerEnum.Tracker => new Tracker(Player),
            LayerEnum.Stalker => new Stalker(Player),
            LayerEnum.Transporter => new Transporter(Player),
            LayerEnum.Mayor => new Mayor(Player),
            LayerEnum.Operative => new Operative(Player),
            LayerEnum.Veteran => new Veteran(Player),
            LayerEnum.Vigilante => new Vigilante(Player),
            LayerEnum.Chameleon => new Chameleon(Player),
            LayerEnum.Coroner => new Coroner(Player),
            LayerEnum.Monarch => new Monarch(Player)
            {
                ToBeKnighted = ((Monarch)role).ToBeKnighted,
                Knighted = ((Monarch)role).Knighted
            },
            LayerEnum.Retributionist => new Retributionist(Player)
            {
                Selected = ((Retributionist)role).Selected,
                ShieldedPlayer = ((Retributionist)role).ShieldedPlayer,
                BombedIDs = ((Retributionist)role).BombedIDs
            },
            _ => new Amnesiac(Player),
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

        Player.Data.SetImpostor(Player.Is(Faction.Intruder) || (Player.Is(Faction.Syndicate) && CustomGameOptions.AltImps));

        if (Player.Is(Faction.Intruder) || Player.Is(Faction.Syndicate) || (Player.Is(Faction.Neutral) && CustomGameOptions.SnitchSeesNeutrals))
        {
            foreach (var snitch in PlayerLayer.GetLayers<Snitch>())
            {
                if (snitch.TasksLeft <= CustomGameOptions.SnitchTasksRemaining && CustomPlayer.Local == Player)
                    LocalRole.AllArrows.Add(snitch.PlayerId, new(Player, Colors.Snitch));
                else if (snitch.TasksDone && CustomPlayer.Local == snitch.Player)
                    GetRole(snitch.Player).AllArrows.Add(Player.PlayerId, new(snitch.Player, Colors.Snitch));
            }

            foreach (var revealer in GetLayers<Revealer>())
            {
                if (revealer.Revealed && CustomPlayer.Local == Player)
                    LocalRole.AllArrows.Add(revealer.PlayerId, new(Player, Colors.Revealer));
            }
        }

        if (CustomPlayer.Local == Player || CustomPlayer.Local == other)
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