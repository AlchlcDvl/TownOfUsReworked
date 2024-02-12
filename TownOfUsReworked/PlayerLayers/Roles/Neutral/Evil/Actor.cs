namespace TownOfUsReworked.PlayerLayers.Roles;

public class Actor : Neutral
{
    public bool Guessed { get; set; }
    public List<Role> PretendRoles { get; set; }
    public bool Failed => !Ability.GetAssassins().Any(x => !x.IsDead && !x.Disconnected);
    public CustomButton PretendButton { get; set; }
    public int Rounds { get; set; }
    public bool TargetFailed => !Targeted && Rounds > 0;
    public bool Targeted { get; set; }

    public override UColor Color => ClientGameOptions.CustomNeutColors ? CustomColorManager.Actor : CustomColorManager.Neutral;
    public override string Name => "Actor";
    public override LayerEnum Type => LayerEnum.Actor;
    public override Func<string> StartText => () => "Play Pretend With The Others";
    public override Func<string> Description => () => !Targeted ? "- You can select a player whose role you can pretend to be" : "- Upon being guessed, you will kill your guesser";
    public override AttackEnum AttackVal => AttackEnum.Unstoppable;

    public Actor() : base() {}

    public override PlayerLayer Start(PlayerControl player)
    {
        SetPlayer(player);
        BaseStart();
        Objectives = () => Guessed ? "- You have successfully fooled the crew" : (!Targeted ? "- Find a set of roles you must pretend to be" : ("- Get guessed as one of your " +
            $"target roles\n- Your target roles are {PretendListString()}"));
        Alignment = Alignment.NeutralEvil;
        PretendButton = new(this, "Pretend", AbilityTypes.Alive, "ActionSecondary", PickRole);
        PretendRoles = new();
        return this;
    }

    private string PretendListString()
    {
        var text = $"{PretendRoles[0].Name}, ";
        var pos = 0;

        foreach (var target in PretendRoles.Skip(1))
        {
            pos++;
            text += pos == PretendRoles.Count ? $"and {target.Name}" : $"{target.Name}, ";
        }

        return text;
    }

    public void PickRole()
    {
        FillRoles(PretendButton.TargetPlayer);
        CallRpc(CustomRPC.Target, TargetRPC.SetActPretendList, this, PretendRoles);
    }

    public void TurnRole(Role role)
    {
        var target = role.Player.GetTarget();
        var leader = role.Player.GetLeader();

        Role newRole = role.Type switch
        {
            LayerEnum.Anarchist => new Anarchist(),
            LayerEnum.Arsonist => new Arsonist(),
            LayerEnum.Blackmailer => new Blackmailer(),
            LayerEnum.Bomber => new Bomber(),
            LayerEnum.Camouflager => new Camouflager(),
            LayerEnum.Cannibal => new Cannibal(),
            LayerEnum.Enforcer => new Enforcer(),
            LayerEnum.Concealer => new Concealer(),
            LayerEnum.Consigliere => new Consigliere(),
            LayerEnum.Consort => new Consort(),
            LayerEnum.Crewmate => new Crewmate(),
            LayerEnum.Cryomaniac => new Cryomaniac(),
            LayerEnum.Detective => new Detective(),
            LayerEnum.Disguiser => new Disguiser(),
            LayerEnum.Dracula => new Dracula(),
            LayerEnum.Escort => new Escort(),
            LayerEnum.Executioner => new Executioner() { TargetPlayer = target },
            LayerEnum.Framer => new Framer(),
            LayerEnum.Glitch => new Glitch(),
            LayerEnum.Godfather => new Godfather(),
            LayerEnum.PromotedGodfather => new PromotedGodfather(),
            LayerEnum.Grenadier => new Grenadier(),
            LayerEnum.GuardianAngel => new GuardianAngel() { TargetPlayer = target },
            LayerEnum.Impostor => new Impostor(),
            LayerEnum.Jackal => new Jackal(),
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
            LayerEnum.PromotedRebel => new PromotedRebel(),
            LayerEnum.Sidekick => new Sidekick() { Rebel = (Rebel)leader },
            LayerEnum.Shapeshifter => new Shapeshifter(),
            LayerEnum.Murderer => new Murderer(),
            LayerEnum.Survivor => new Survivor(),
            LayerEnum.Plaguebearer => new Plaguebearer(),
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
            LayerEnum.BountyHunter => new BountyHunter() { TargetPlayer = target },
            LayerEnum.Guesser => new Guesser() { TargetPlayer = target },
            LayerEnum.Necromancer => new Necromancer(),
            LayerEnum.Whisperer => new Whisperer(),
            LayerEnum.Betrayer => new Betrayer(),
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
            LayerEnum.Monarch => new Monarch(),
            LayerEnum.Retributionist => new Retributionist(),
            LayerEnum.Bastion => new Bastion(),
            _ => new Crewmate(),
        };

        newRole.Start<Role>(Player).RoleUpdate(this);
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        PretendButton.Update2("PRETEND", !Targeted);

        if ((TargetFailed || (Targeted && Failed)) && !IsDead)
        {
            var role = PretendRoles.Random();
            CallRpc(CustomRPC.Change, TurnRPC.TurnRole, this, role);
            TurnRole(role);
        }
    }

    public void FillRoles(PlayerControl target)
    {
        PretendRoles.Add(target.GetRole());
        var targets = CustomPlayer.AllPlayers.Where(x => x != Player && x != target).ToList();
        targets.Shuffle();

        foreach (var player in targets)
        {
            if (PretendRoles.Count >= CustomGameOptions.ActorRoleCount)
                break;

            PretendRoles.Add(player.GetRole());
        }

        PretendRoles.Shuffle();
        PretendRoles = PretendRoles.GetRange(0, CustomGameOptions.ActorRoleCount);
        Targeted = true;
    }
}