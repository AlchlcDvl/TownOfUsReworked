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

    public Actor(PlayerControl player) : base(player)
    {
        Objectives = () => Guessed ? "- You have successfully fooled the crew" : (!Targeted ? "- Find a set of roles you must pretend to be" : ("- Get guessed as one of your " +
            $"target roles\n- Your target roles are {PretendListString()}"));
        Alignment = Alignment.NeutralEvil;
        PretendButton = new(this, "Pretend", AbilityTypes.Alive, "ActionSecondary", PickRole);
        PretendRoles = new();
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
            LayerEnum.Anarchist => new Anarchist(Player),
            LayerEnum.Arsonist => new Arsonist(Player),
            LayerEnum.Blackmailer => new Blackmailer(Player),
            LayerEnum.Bomber => new Bomber(Player),
            LayerEnum.Camouflager => new Camouflager(Player),
            LayerEnum.Cannibal => new Cannibal(Player),
            LayerEnum.Enforcer => new Enforcer(Player),
            LayerEnum.Concealer => new Concealer(Player),
            LayerEnum.Consigliere => new Consigliere(Player),
            LayerEnum.Consort => new Consort(Player),
            LayerEnum.Crewmate => new Crewmate(Player),
            LayerEnum.Cryomaniac => new Cryomaniac(Player),
            LayerEnum.Detective => new Detective(Player),
            LayerEnum.Disguiser => new Disguiser(Player),
            LayerEnum.Dracula => new Dracula(Player),
            LayerEnum.Escort => new Escort(Player),
            LayerEnum.Executioner => new Executioner(Player) { TargetPlayer = target },
            LayerEnum.Framer => new Framer(Player),
            LayerEnum.Glitch => new Glitch(Player),
            LayerEnum.Godfather => new Godfather(Player),
            LayerEnum.PromotedGodfather => new PromotedGodfather(Player),
            LayerEnum.Grenadier => new Grenadier(Player),
            LayerEnum.GuardianAngel => new GuardianAngel(Player) { TargetPlayer = target },
            LayerEnum.Impostor => new Impostor(Player),
            LayerEnum.Jackal => new Jackal(Player),
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
            LayerEnum.PromotedRebel => new PromotedRebel(Player),
            LayerEnum.Sidekick => new Sidekick(Player) { Rebel = (Rebel)leader },
            LayerEnum.Shapeshifter => new Shapeshifter(Player),
            LayerEnum.Murderer => new Murderer(Player),
            LayerEnum.Survivor => new Survivor(Player),
            LayerEnum.Plaguebearer => new Plaguebearer(Player),
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
            LayerEnum.BountyHunter => new BountyHunter(Player) { TargetPlayer = target },
            LayerEnum.Guesser => new Guesser(Player) { TargetPlayer = target },
            LayerEnum.Necromancer => new Necromancer(Player),
            LayerEnum.Whisperer => new Whisperer(Player),
            LayerEnum.Betrayer => new Betrayer(Player),
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
            LayerEnum.Monarch => new Monarch(Player),
            LayerEnum.Retributionist => new Retributionist(Player),
            LayerEnum.Bastion => new Bastion(Player),
            _ => new Crewmate(Player),
        };

        newRole.RoleUpdate(this);
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
        PretendRoles.Add(GetRole(target));
        var targets = CustomPlayer.AllPlayers.Where(x => x != Player && x != target).ToList();
        targets.Shuffle();

        foreach (var player in targets)
        {
            if (PretendRoles.Count >= CustomGameOptions.ActorRoleCount)
                break;

            PretendRoles.Add(GetRole(player));
        }

        PretendRoles.Shuffle();
        PretendRoles = PretendRoles.GetRange(0, CustomGameOptions.ActorRoleCount);
        Targeted = true;
    }
}