namespace TownOfUsReworked.PlayerLayers.Roles;

public class Amnesiac : Neutral
{
    public Dictionary<byte, CustomArrow> BodyArrows { get; set; }
    public CustomButton RememberButton { get; set; }

    public override UColor Color => ClientGameOptions.CustomNeutColors ? CustomColorManager.Amnesiac : CustomColorManager.Neutral;
    public override string Name => "Amnesiac";
    public override LayerEnum Type => LayerEnum.Amnesiac;
    public override Func<string> StartText => () => "You Forgor <i>:skull:</i>";
    public override Func<string> Description => () => "- You can copy over a player's role should you find their body" + (CustomGameOptions.RememberArrows ? ("\n- When someone dies, you" +
        " get an arrow pointing to their body") : "") + "\n- If there are less than 6 players alive, you will become a <color=#80FF00FF>Thief</color>";

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.NeutralBen;
        Objectives = () => "- Find a dead body, remember their role and then fulfill the win condition for that role";
        BodyArrows = [];
        RememberButton = CreateButton(this, new SpriteName("Remember"), AbilityTypes.Dead, KeybindType.ActionSecondary, (OnClick)Remember, "REMEMBER");
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
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, player);
        Remember(player);
    }

    public override void ReadRPC(MessageReader reader) => Remember(reader.ReadPlayer());

    public void Remember(PlayerControl other)
    {
        var role = other.GetRole();
        var player = Player;

        if (CustomPlayer.Local == player || CustomPlayer.Local == other)
        {
            Flash(Color);
            role.OnLobby();
            OnLobby();
            ButtonUtils.Reset();
        }

        Role newRole = role.Type switch
        {
            // Crew roles
            LayerEnum.Altruist => new Altruist(),
            LayerEnum.Bastion => new Bastion(),
            LayerEnum.Chameleon => new Chameleon(),
            LayerEnum.Coroner => new Coroner(),
            LayerEnum.Crewmate => new Crewmate(),
            LayerEnum.Detective => new Detective(),
            LayerEnum.Dictator => new Dictator(),
            LayerEnum.Engineer => new Engineer(),
            LayerEnum.Escort => new Escort(),
            LayerEnum.Mayor => new Mayor(),
            LayerEnum.Medic => new Medic(),
            LayerEnum.Medium => new Medium(),
            LayerEnum.Monarch => new Monarch(),
            LayerEnum.Mystic => new Mystic(),
            LayerEnum.Operative => new Operative(),
            LayerEnum.Retributionist => new Retributionist(),
            LayerEnum.Sheriff => new Sheriff(),
            LayerEnum.Seer => new Seer(),
            LayerEnum.Shifter => new Shifter(),
            LayerEnum.Tracker => new Tracker(),
            LayerEnum.Transporter => new Transporter(),
            LayerEnum.Trapper => new Trapper(),
            LayerEnum.VampireHunter => new VampireHunter(),
            LayerEnum.Veteran => new Veteran(),
            LayerEnum.Vigilante => new Vigilante(),

            // Neutral roles
            LayerEnum.Actor => new Actor() { PretendRoles = ((Actor)role).PretendRoles },
            LayerEnum.Arsonist => new Arsonist(),
            LayerEnum.Betrayer => new Betrayer() { Faction = role.Faction },
            LayerEnum.BountyHunter => new BountyHunter() { TargetPlayer = ((BountyHunter)role).TargetPlayer },
            LayerEnum.Cannibal => new Cannibal(),
            LayerEnum.Cryomaniac => new Cryomaniac(),
            LayerEnum.Dracula => new Dracula(),
            LayerEnum.Executioner => new Executioner() { TargetPlayer = ((Executioner)role).TargetPlayer },
            LayerEnum.Glitch => new Glitch(),
            LayerEnum.GuardianAngel => new GuardianAngel() { TargetPlayer = ((GuardianAngel)role).TargetPlayer },
            LayerEnum.Guesser => new Guesser() { TargetPlayer = ((Guesser)role).TargetPlayer },
            LayerEnum.Jackal => new Jackal(),
            LayerEnum.Jester => new Jester(),
            LayerEnum.Juggernaut => new Juggernaut(),
            LayerEnum.Murderer => new Murderer(),
            LayerEnum.Necromancer => new Necromancer(),
            LayerEnum.Plaguebearer or LayerEnum.Pestilence => new Plaguebearer(),
            LayerEnum.SerialKiller => new SerialKiller(),
            LayerEnum.Survivor => new Survivor(),
            LayerEnum.Thief => new Thief(),
            LayerEnum.Troll => new Troll(),
            LayerEnum.Werewolf => new Werewolf(),
            LayerEnum.Whisperer => new Whisperer(),

            // Intruder roles
            LayerEnum.Ambusher => new Ambusher(),
            LayerEnum.Blackmailer => new Blackmailer(),
            LayerEnum.Camouflager => new Camouflager(),
            LayerEnum.Consigliere => new Consigliere(),
            LayerEnum.Consort => new Consort(),
            LayerEnum.Disguiser => new Disguiser(),
            LayerEnum.Enforcer => new Enforcer(),
            LayerEnum.Godfather => new Godfather(),
            LayerEnum.PromotedGodfather => new PromotedGodfather() { FormerRole = ((PromotedGodfather)role).FormerRole },
            LayerEnum.Grenadier => new Grenadier(),
            LayerEnum.Impostor => new Impostor(),
            LayerEnum.Janitor => new Janitor(),
            LayerEnum.Mafioso => new Mafioso() { Godfather = ((Mafioso)role).Godfather },
            LayerEnum.Miner => new Miner(),
            LayerEnum.Morphling => new Morphling(),
            LayerEnum.Teleporter => new Teleporter(),
            LayerEnum.Wraith => new Wraith(),

            // Syndicate roles
            LayerEnum.Anarchist => new Anarchist(),
            LayerEnum.Bomber => new Bomber(),
            LayerEnum.Collider => new Collider(),
            LayerEnum.Concealer => new Concealer(),
            LayerEnum.Crusader => new Crusader(),
            LayerEnum.Drunkard => new Drunkard(),
            LayerEnum.Framer => new Framer(),
            LayerEnum.Poisoner => new Poisoner(),
            LayerEnum.Rebel => new Rebel(),
            LayerEnum.PromotedRebel => new PromotedRebel() { FormerRole = ((PromotedRebel)role).FormerRole },
            LayerEnum.Shapeshifter => new Shapeshifter(),
            LayerEnum.Sidekick => new Sidekick() { Rebel = ((Sidekick)role).Rebel },
            LayerEnum.Silencer => new Silencer(),
            LayerEnum.Spellslinger => new Spellslinger(),
            LayerEnum.Stalker => new Stalker(),
            LayerEnum.Timekeeper => new Timekeeper(),
            LayerEnum.Warper => new Warper(),

            // Whatever else
            LayerEnum.Amnesiac or _ => new Amnesiac(),
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

        if (player.Is(Faction.Intruder) || player.Is(Faction.Syndicate) || (player.Is(Faction.Neutral) && (CustomGameOptions.SnitchSeesNeutrals || CustomGameOptions.RevealerRevealsNeutrals)))
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

        if (CustomGameOptions.RememberArrows && !Dead)
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
        else if (BodyArrows.Count > 0 || CustomPlayer.AllPlayers.Count(x => !x.HasDied()) <= 4)
            OnLobby();

        if (CustomGameOptions.AmneToThief && CustomPlayer.AllPlayers.Count(x => !x.HasDied()) <= 4 && !Dead)
        {
            CallRpc(CustomRPC.Misc, MiscRPC.ChangeRoles, this);
            TurnThief();
        }
    }
}