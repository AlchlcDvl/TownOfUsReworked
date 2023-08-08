namespace TownOfUsReworked.PlayerLayers.Roles;

public class Amnesiac : Neutral
{
    public Dictionary<byte, CustomArrow> BodyArrows { get; set; }
    public CustomButton RememberButton { get; set; }

    public override Color32 Color => ClientGameOptions.CustomNeutColors ? Colors.Amnesiac : Colors.Intruder;
    public override string Name => "Amnesiac";
    public override LayerEnum Type => LayerEnum.Amnesiac;
    public override Func<string> StartText => () => "You Forgor <i>:skull:</i>";
    public override Func<string> Description => () => "- You can copy over a player's role should you find their body" + (CustomGameOptions.RememberArrows ? ("\n- When someone dies, " +
        "you get an arrow pointing to their body") : "") + "\n- If there are less than 6 players alive, you will become a <color=#80FF00FF>Thief</color>";
    public override InspectorResults InspectorResults => InspectorResults.LeadsTheGroup;

    public Amnesiac(PlayerControl player) : base(player)
    {
        RoleAlignment = RoleAlignment.NeutralBen;
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

    public void TurnThief()
    {
        var newRole = new Thief(Player);
        newRole.RoleUpdate(this);

        if (Local)
            Flash(Colors.Thief);

        if (CustomPlayer.Local.Is(LayerEnum.Seer))
            Flash(Colors.Seer);
    }

    public void Remember()
    {
        if (IsTooFar(Player, RememberButton.TargetBody))
            return;

        var player = PlayerByBody(RememberButton.TargetBody);
        Spread(Player, player);
        CallRpc(CustomRPC.Action, ActionsRPC.Morph, this, player);
        Remember(this, player);
    }

    public static void Remember(Amnesiac amneRole, PlayerControl other)
    {
        var role = GetRole(other);
        var amnesiac = amneRole.Player;
        var target = other.GetTarget();
        var leader = other.GetLeader();
        var actor = other.GetActorList();

        if (CustomPlayer.Local == amnesiac || CustomPlayer.Local == other)
        {
            Flash(Colors.Amnesiac);
            role.OnLobby();
            amneRole.OnLobby();
            ButtonUtils.ResetCustomTimers();
        }

        Role newRole = role.Type switch
        {
            LayerEnum.Anarchist => new Anarchist(amnesiac),
            LayerEnum.Arsonist => new Arsonist(amnesiac) { Doused = ((Arsonist)role).Doused },
            LayerEnum.Blackmailer => new Blackmailer(amnesiac) { BlackmailedPlayer = ((Blackmailer)role).BlackmailedPlayer },
            LayerEnum.Bomber => new Bomber(amnesiac),
            LayerEnum.Camouflager => new Camouflager(amnesiac),
            LayerEnum.Cannibal => new Cannibal(amnesiac) { EatNeed = ((Cannibal)role).EatNeed },
            LayerEnum.Enforcer => new Enforcer(amnesiac),
            LayerEnum.Concealer => new Concealer(amnesiac),
            LayerEnum.Consigliere => new Consigliere(amnesiac) { Investigated = ((Consigliere)role).Investigated },
            LayerEnum.Consort => new Consort(amnesiac),
            LayerEnum.Crewmate => new Crewmate(amnesiac),
            LayerEnum.Cryomaniac => new Cryomaniac(amnesiac) { Doused = ((Cryomaniac)role).Doused },
            LayerEnum.Detective => new Detective(amnesiac),
            LayerEnum.Disguiser => new Disguiser(amnesiac),
            LayerEnum.Dracula => new Dracula(amnesiac) { Converted = ((Dracula)role).Converted },
            LayerEnum.Escort => new Escort(amnesiac),
            LayerEnum.Executioner => new Executioner(amnesiac) { TargetPlayer = target },
            LayerEnum.Framer => new Framer(amnesiac) { Framed = ((Framer)role).Framed },
            LayerEnum.Glitch => new Glitch(amnesiac),
            LayerEnum.Godfather => new Godfather(amnesiac),
            LayerEnum.PromotedGodfather => new PromotedGodfather(amnesiac)
            {
                Investigated = ((PromotedGodfather)role).Investigated,
                BlackmailedPlayer = ((PromotedGodfather)role).BlackmailedPlayer
            },
            LayerEnum.Grenadier => new Grenadier(amnesiac),
            LayerEnum.GuardianAngel => new GuardianAngel(amnesiac) { TargetPlayer = target },
            LayerEnum.Impostor => new Impostor(amnesiac),
            LayerEnum.Jackal => new Jackal(amnesiac)
            {
                Recruited = ((Jackal)role).Recruited,
                EvilRecruit = ((Jackal)role).EvilRecruit,
                GoodRecruit = ((Jackal)role).GoodRecruit,
                BackupRecruit = ((Jackal)role).BackupRecruit
            },
            LayerEnum.Jester => new Jester(amnesiac),
            LayerEnum.Juggernaut => new Juggernaut(amnesiac) { JuggKills = ((Juggernaut)role).JuggKills },
            LayerEnum.Sheriff => new Sheriff(amnesiac),
            LayerEnum.Mafioso => new Mafioso(amnesiac) { Godfather = (Godfather)leader },
            LayerEnum.Miner => new Miner(amnesiac),
            LayerEnum.Morphling => new Morphling(amnesiac),
            LayerEnum.Medic => new Medic(amnesiac),
            LayerEnum.Medium => new Medium(amnesiac),
            LayerEnum.Shifter => new Shifter(amnesiac),
            LayerEnum.Rebel => new Rebel(amnesiac),
            LayerEnum.PromotedRebel => new PromotedRebel(amnesiac) { Framed = ((PromotedRebel)role).Framed },
            LayerEnum.Sidekick => new Sidekick(amnesiac) { Rebel = (Rebel)leader },
            LayerEnum.Shapeshifter => new Shapeshifter(amnesiac),
            LayerEnum.Murderer => new Murderer(amnesiac),
            LayerEnum.Survivor => new Survivor(amnesiac) { UsesLeft = ((Survivor)role).UsesLeft },
            LayerEnum.Plaguebearer => new Plaguebearer(amnesiac) { Infected = ((Plaguebearer)role).Infected },
            LayerEnum.Pestilence => new Pestilence(amnesiac),
            LayerEnum.SerialKiller => new SerialKiller(amnesiac),
            LayerEnum.Werewolf => new Werewolf(amnesiac),
            LayerEnum.Janitor => new Janitor(amnesiac),
            LayerEnum.Poisoner => new Poisoner(amnesiac),
            LayerEnum.Teleporter => new Teleporter(amnesiac) { TeleportPoint = ((Teleporter)role).TeleportPoint },
            LayerEnum.Troll => new Troll(amnesiac),
            LayerEnum.Thief => new Thief(amnesiac),
            LayerEnum.VampireHunter => new VampireHunter(amnesiac),
            LayerEnum.Warper => new Warper(amnesiac),
            LayerEnum.Wraith => new Wraith(amnesiac),
            LayerEnum.Mystic => new Mystic(amnesiac),
            LayerEnum.Dictator => new Dictator(amnesiac),
            LayerEnum.Seer => new Seer(amnesiac),
            LayerEnum.Actor => new Actor(amnesiac) { TargetRole = actor },
            LayerEnum.BountyHunter => new BountyHunter(amnesiac) { TargetPlayer = target },
            LayerEnum.Guesser => new Guesser(amnesiac) { TargetPlayer = target },
            LayerEnum.Necromancer => new Necromancer(amnesiac)
            {
                Resurrected = ((Necromancer)role).Resurrected,
                KillCount = ((Necromancer)role).KillCount,
                ResurrectedCount = ((Necromancer)role).ResurrectedCount
            },
            LayerEnum.Whisperer => new Whisperer(amnesiac)
            {
                Persuaded = ((Whisperer)role).Persuaded,
                WhisperCount = ((Whisperer)role).WhisperCount,
                WhisperConversion = ((Whisperer)role).WhisperConversion
            },
            LayerEnum.Betrayer => new Betrayer(amnesiac) { Faction = role.Faction },
            LayerEnum.Ambusher => new Ambusher(amnesiac),
            LayerEnum.Crusader => new Crusader(amnesiac),
            LayerEnum.Altruist => new Altruist(amnesiac) { UsesLeft = ((Altruist)role).UsesLeft },
            LayerEnum.Engineer => new Engineer(amnesiac) { UsesLeft = ((Engineer)role).UsesLeft },
            LayerEnum.Inspector => new Inspector(amnesiac) { Inspected = ((Inspector)role).Inspected },
            LayerEnum.Tracker => new Tracker(amnesiac)
            {
                TrackerArrows = ((Tracker)role).TrackerArrows,
                UsesLeft = ((Tracker)role).UsesLeft
            },
            LayerEnum.Stalker => new Stalker(amnesiac) { StalkerArrows = ((Stalker)role).StalkerArrows },
            LayerEnum.Transporter => new Transporter(amnesiac) { UsesLeft = ((Transporter)role).UsesLeft },
            LayerEnum.Mayor => new Mayor(amnesiac) { Revealed = ((Mayor)role).Revealed },
            LayerEnum.Operative => new Operative(amnesiac) { UsesLeft = ((Operative)role).UsesLeft },
            LayerEnum.Veteran => new Veteran(amnesiac) { UsesLeft = ((Veteran)role).UsesLeft },
            LayerEnum.Vigilante => new Vigilante(amnesiac) { UsesLeft = ((Vigilante)role).UsesLeft },
            LayerEnum.Chameleon => new Chameleon(amnesiac) { UsesLeft = ((Chameleon)role).UsesLeft },
            LayerEnum.Coroner => new Coroner(amnesiac)
            {
                ReferenceBodies = ((Coroner)role).ReferenceBodies,
                Reported = ((Coroner)role).Reported
            },
            LayerEnum.Monarch => new Monarch(amnesiac)
            {
                UsesLeft = ((Monarch)role).UsesLeft,
                ToBeKnighted = ((Monarch)role).ToBeKnighted,
                Knighted = ((Monarch)role).Knighted
            },
            LayerEnum.Retributionist => new Retributionist(amnesiac)
            {
                TrackerArrows = ((Retributionist)role).TrackerArrows,
                Inspected = ((Retributionist)role).Inspected,
                Selected = ((Retributionist)role).Selected,
                UsesLeft = ((Retributionist)role).UsesLeft,
                Reported = ((Retributionist)role).Reported,
                ReferenceBodies = ((Retributionist)role).ReferenceBodies
            },
            _ => new Amnesiac(amnesiac),
        };

        newRole.RoleUpdate(amneRole);

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

        amnesiac.Data.SetImpostor(amnesiac.Is(Faction.Intruder) || (amnesiac.Is(Faction.Syndicate) && CustomGameOptions.AltImps));

        if (amnesiac.Is(Faction.Intruder) || amnesiac.Is(Faction.Syndicate) || (amnesiac.Is(Faction.Neutral) && CustomGameOptions.SnitchSeesNeutrals))
        {
            foreach (var snitch in Ability.GetAbilities<Snitch>(LayerEnum.Snitch))
            {
                if (snitch.TasksLeft <= CustomGameOptions.SnitchTasksRemaining && CustomPlayer.Local == amnesiac)
                    LocalRole.AllArrows.Add(snitch.PlayerId, new(amnesiac, Colors.Snitch));
                else if (snitch.TasksDone && CustomPlayer.Local == snitch.Player)
                    GetRole(snitch.Player).AllArrows.Add(amnesiac.PlayerId, new(snitch.Player, Colors.Snitch));
            }

            foreach (var revealer in GetRoles<Revealer>(LayerEnum.Revealer))
            {
                if (revealer.Revealed && CustomPlayer.Local == amnesiac)
                    LocalRole.AllArrows.Add(revealer.PlayerId, new(amnesiac, Colors.Revealer));
            }
        }

        if (CustomPlayer.Local == amnesiac)
            ButtonUtils.ResetCustomTimers();
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        RememberButton.Update("REMEMBER");

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

        if (CustomPlayer.AllPlayers.Count <= 6 && !IsDead)
        {
            CallRpc(CustomRPC.Change, TurnRPC.TurnThief, this);
            TurnThief();
        }
    }
}