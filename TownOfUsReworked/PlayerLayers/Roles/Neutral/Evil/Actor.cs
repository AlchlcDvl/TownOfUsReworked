namespace TownOfUsReworked.PlayerLayers.Roles;

public class Actor : Neutral
{
    public bool Guessed { get; set; }
    public InspectorResults PretendRoles => TargetRole.InspectorResults;
    public Role TargetRole { get; set; }
    public bool Failed => !Ability.GetAssassins().Any(x => !x.IsDead && !x.Disconnected);
    public CustomButton RoleButton { get; set; }
    public int Rounds { get; set; }
    public bool TargetFailed => TargetRole == null && Rounds > 0;

    public override Color32 Color => ClientGameOptions.CustomNeutColors ? Colors.Actor : Colors.Neutral;
    public override string Name => "Actor";
    public override LayerEnum Type => LayerEnum.Actor;
    public override Func<string> StartText => () => "Play Pretend With The Others";
    public override Func<string> Description => () => TargetRole == null ? "- You can select a player whose role you can pretend to be" : "- Upon being guessed, you kill your guesser";
    public override InspectorResults InspectorResults => TargetRole == null ? InspectorResults.Manipulative : TargetRole.InspectorResults;

    public Actor(PlayerControl player) : base(player)
    {
        Objectives = () => Guessed ? "- You have successfully fooled the crew" : (TargetRole == null ? "- Find a set of roles you must pretend to be" : ("- Get guessed as one of your" +
            $" target roles\n- Your target roles belong to the {PretendRoles} role list"));
        RoleAlignment = RoleAlignment.NeutralEvil;
        RoleButton = new(this, "ActorRole", AbilityTypes.Direct, "ActionSecondary", PickRole);
    }

    public void PickRole()
    {
        if (IsTooFar(Player, RoleButton.TargetPlayer))
            return;

        TargetRole = GetRole(RoleButton.TargetPlayer);
        CallRpc(CustomRPC.Target, TargetRPC.SetActPretendList, this, TargetRole);
    }

    public void TurnRole()
    {
        Role newRole;

        if (TargetRole == null)
            newRole = new Crewmate(Player);
        else
        {
            var target = TargetRole.Player.GetTarget();
            var leader = TargetRole.Player.GetLeader();

            newRole = TargetRole.Type switch
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
                LayerEnum.Inspector => new Inspector(Player),
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
                _ => new Crewmate(Player),
            };
        }

        newRole.RoleUpdate(this);

        if (Local)
            Flash(newRole.Color);

        if (CustomPlayer.Local.Is(LayerEnum.Seer))
            Flash(Colors.Seer);
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        RoleButton.Update("PRETEND", true, TargetRole == null);

        if ((TargetFailed || (TargetRole != null && Failed)) && !IsDead)
        {
            CallRpc(CustomRPC.Change, TurnRPC.TurnRole, this);
            TurnRole();
        }
    }
}