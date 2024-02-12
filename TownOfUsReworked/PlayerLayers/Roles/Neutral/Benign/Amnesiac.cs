namespace TownOfUsReworked.PlayerLayers.Roles;

public class Amnesiac : Neutral
{
    public Dictionary<byte, CustomArrow> BodyArrows { get; set; }
    public CustomButton RememberButton { get; set; }

    public override UColor Color => ClientGameOptions.CustomNeutColors ? CustomColorManager.Amnesiac : CustomColorManager.Neutral;
    public override string Name => "Amnesiac";
    public override LayerEnum Type => LayerEnum.Amnesiac;
    public override Func<string> StartText => () => "You Forgor <i>:skull:</i>";
    public override Func<string> Description => () => "- You can copy over a player's role should you find their body" + (CustomGameOptions.RememberArrows ? ("\n- When someone dies, " +
        "you get an arrow pointing to their body") : "") + "\n- If there are less than 6 players alive, you will become a <color=#80FF00FF>Thief</color>";

    public Amnesiac() : base() {}

    public override PlayerLayer Start(PlayerControl player)
    {
        SetPlayer(player);
        BaseStart();
        Alignment = Alignment.NeutralBen;
        Objectives = () => "- Find a dead body, remember their role and then fulfill the win condition for that role";
        BodyArrows = new();
        RememberButton = new(this, "Remember", AbilityTypes.Dead, "ActionSecondary", Remember);
        return this;
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

    public void TurnThief() => new Thief().Start<Thief>(Player).RoleUpdate(this);

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
        var role = other.GetRole();
        var target = other.GetTarget();
        var leader = other.GetLeader();
        var actor = other.GetActorList();
        var player = Player;

        if (CustomPlayer.Local == player || CustomPlayer.Local == other)
        {
            Flash(CustomColorManager.Amnesiac);
            role.OnLobby();
            OnLobby();
            ButtonUtils.Reset();
        }

        Role newRole = role.Type switch
        {
            LayerEnum.Anarchist => new Anarchist(),
            LayerEnum.Arsonist => new Arsonist() { Doused = ((Arsonist)role).Doused },
            LayerEnum.Blackmailer => new Blackmailer() { BlackmailedPlayer = ((Blackmailer)role).BlackmailedPlayer },
            LayerEnum.Bomber => new Bomber(),
            LayerEnum.Camouflager => new Camouflager(),
            LayerEnum.Cannibal => new Cannibal() { EatNeed = ((Cannibal)role).EatNeed },
            LayerEnum.Enforcer => new Enforcer(),
            LayerEnum.Concealer => new Concealer(),
            LayerEnum.Consigliere => new Consigliere(),
            LayerEnum.Consort => new Consort(),
            LayerEnum.Crewmate => new Crewmate(),
            LayerEnum.Cryomaniac => new Cryomaniac() { Doused = ((Cryomaniac)role).Doused },
            LayerEnum.Detective => new Detective(),
            LayerEnum.Disguiser => new Disguiser(),
            LayerEnum.Dracula => new Dracula() { Converted = ((Dracula)role).Converted },
            LayerEnum.Escort => new Escort(),
            LayerEnum.Executioner => new Executioner() { TargetPlayer = target },
            LayerEnum.Framer => new Framer() { Framed = ((Framer)role).Framed },
            LayerEnum.Glitch => new Glitch(),
            LayerEnum.Godfather => new Godfather(),
            LayerEnum.PromotedGodfather => new PromotedGodfather() { BlackmailedPlayer = ((PromotedGodfather)role).BlackmailedPlayer },
            LayerEnum.Grenadier => new Grenadier(),
            LayerEnum.GuardianAngel => new GuardianAngel() { TargetPlayer = target },
            LayerEnum.Impostor => new Impostor(),
            LayerEnum.Bastion => new Bastion() { BombedIDs = ((Bastion)role).BombedIDs },
            LayerEnum.Jackal => new Jackal()
            {
                Recruited = ((Jackal)role).Recruited,
                EvilRecruit = ((Jackal)role).EvilRecruit,
                GoodRecruit = ((Jackal)role).GoodRecruit,
                BackupRecruit = ((Jackal)role).BackupRecruit
            },
            LayerEnum.Jester => new Jester(),
            LayerEnum.Juggernaut => new Juggernaut(),
            LayerEnum.Sheriff => new Sheriff(),
            LayerEnum.Mafioso => new Mafioso() { Godfather = (Godfather)leader },
            LayerEnum.Miner => new Miner(),
            LayerEnum.Morphling => new Morphling(),
            LayerEnum.Medic => new Medic(),
            LayerEnum.Medium => new Medium(),
            LayerEnum.Shifter => new Shifter(),
            LayerEnum.Rebel => new Rebel(),
            LayerEnum.PromotedRebel => new PromotedRebel() { Framed = ((PromotedRebel)role).Framed },
            LayerEnum.Sidekick => new Sidekick() { Rebel = (Rebel)leader },
            LayerEnum.Shapeshifter => new Shapeshifter(),
            LayerEnum.Murderer => new Murderer(),
            LayerEnum.Survivor => new Survivor(),
            LayerEnum.Plaguebearer => new Plaguebearer() { Infected = ((Plaguebearer)role).Infected },
            LayerEnum.Pestilence => new Pestilence(),
            LayerEnum.SerialKiller => new SerialKiller(),
            LayerEnum.Werewolf => new Werewolf(),
            LayerEnum.Janitor => new Janitor(),
            LayerEnum.Poisoner => new Poisoner(),
            LayerEnum.Teleporter => new Teleporter(),
            LayerEnum.Troll => new Troll(),
            LayerEnum.Thief => new Thief(),
            LayerEnum.VampireHunter => new VampireHunter(),
            LayerEnum.Warper => new Warper(),
            LayerEnum.Wraith => new Wraith(),
            LayerEnum.Mystic => new Mystic(),
            LayerEnum.Dictator => new Dictator(),
            LayerEnum.Seer => new Seer(),
            LayerEnum.Actor => new Actor() { PretendRoles = actor },
            LayerEnum.BountyHunter => new BountyHunter() { TargetPlayer = target },
            LayerEnum.Guesser => new Guesser() { TargetPlayer = target },
            LayerEnum.Necromancer => new Necromancer() { Resurrected = ((Necromancer)role).Resurrected },
            LayerEnum.Whisperer => new Whisperer() { Persuaded = ((Whisperer)role).Persuaded },
            LayerEnum.Betrayer => new Betrayer() { Faction = role.Faction },
            LayerEnum.Ambusher => new Ambusher(),
            LayerEnum.Crusader => new Crusader(),
            LayerEnum.Altruist => new Altruist(),
            LayerEnum.Engineer => new Engineer(),
            LayerEnum.Tracker => new Tracker(),
            LayerEnum.Stalker => new Stalker(),
            LayerEnum.Transporter => new Transporter(),
            LayerEnum.Mayor => new Mayor(),
            LayerEnum.Operative => new Operative(),
            LayerEnum.Veteran => new Veteran(),
            LayerEnum.Vigilante => new Vigilante(),
            LayerEnum.Chameleon => new Chameleon(),
            LayerEnum.Coroner => new Coroner(),
            LayerEnum.Monarch => new Monarch()
            {
                ToBeKnighted = ((Monarch)role).ToBeKnighted,
                Knighted = ((Monarch)role).Knighted
            },
            LayerEnum.Retributionist => new Retributionist()
            {
                Selected = ((Retributionist)role).Selected,
                ShieldedPlayer = ((Retributionist)role).ShieldedPlayer,
                BombedIDs = ((Retributionist)role).BombedIDs
            },
            _ => new Amnesiac(),
        };

        newRole.Start<Role>(player).RoleUpdate(this, Faction == Faction.Neutral);

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
                    LocalRole.AllArrows.Add(snitch.PlayerId, new(player, CustomColorManager.Snitch));
                else if (snitch.TasksDone && CustomPlayer.Local == snitch.Player)
                    snitch.Player.GetRole().AllArrows.Add(player.PlayerId, new(snitch.Player, CustomColorManager.Snitch));
            }

            foreach (var revealer in GetLayers<Revealer>())
            {
                if (revealer.Revealed && CustomPlayer.Local == player)
                    LocalRole.AllArrows.Add(revealer.PlayerId, new(player, CustomColorManager.Revealer));
            }
        }

        if (CustomPlayer.Local == player || CustomPlayer.Local == other)
            ButtonUtils.Reset();
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