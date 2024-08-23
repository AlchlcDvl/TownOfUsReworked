namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu2.LayerSubOptions)]
public class Actor : Neutral
{
    [ToggleOption(MultiMenu2.LayerSubOptions)]
    public static bool ActorCanPickRole { get; set; } = false;

    [ToggleOption(MultiMenu2.LayerSubOptions)]
    public static bool ActorButton { get; set; } = true;

    [ToggleOption(MultiMenu2.LayerSubOptions)]
    public static bool ActorVent { get; set; } = false;

    [ToggleOption(MultiMenu2.LayerSubOptions)]
    public static bool ActSwitchVent { get; set; } = false;

    [NumberOption(MultiMenu2.LayerSubOptions, 1, 5, 1)]
    public static int ActorRoleCount { get; set; } = 3;

    [ToggleOption(MultiMenu2.LayerSubOptions)]
    public static bool VigiKillsActor { get; set; } = false;

    public bool Guessed { get; set; }
    public List<Role> PretendRoles { get; set; }
    public bool Failed => !Ability.GetAssassins().Any(x => x.Alive);
    public CustomButton PretendButton { get; set; }
    public int Rounds { get; set; }
    public bool TargetFailed => !Targeted && Rounds > 0;
    public bool Targeted { get; set; }

    public override UColor Color => ClientOptions.CustomNeutColors ? CustomColorManager.Actor : CustomColorManager.Neutral;
    public override string Name => "Actor";
    public override LayerEnum Type => LayerEnum.Actor;
    public override Func<string> StartText => () => "Play Pretend With The Others";
    public override Func<string> Description => () => !Targeted ? "- You can select a player whose role you can pretend to be" : "- Upon being guessed, you will kill your guesser";
    public override AttackEnum AttackVal => AttackEnum.Unstoppable;

    public override void Init()
    {
        BaseStart();
        Objectives = () => Guessed ? "- You have successfully fooled the crew" : (!Targeted ? "- Find a set of roles you must pretend to be" : ("- Get guessed as one of your target roles\n" +
            $"- Your target roles are {PretendListString()}"));
        Alignment = Alignment.NeutralEvil;
        PretendButton = CreateButton(this, new SpriteName("Pretend"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClick)PickRole, "PRETEND", (UsableFunc)Usable);
        PretendRoles = [];
    }

    public string PretendListString()
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
        CallRpc(CustomRPC.Misc, MiscRPC.SetTarget, this, PretendRoles);
    }

    public void TurnRole(Role role)
    {
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
            LayerEnum.Amnesiac => new Amnesiac(),
            LayerEnum.Arsonist => new Arsonist(),
            LayerEnum.Betrayer => new Betrayer(),
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

            // Whatever remains
            LayerEnum.Actor or _ => new Amnesiac(),
        };

        newRole.Start<Role>(Player).RoleUpdate(this);
    }

    public bool Usable() => !Targeted;

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);

        if ((TargetFailed || (Targeted && Failed)) && !Dead)
        {
            var role = (TargetFailed ? AllRoles : PretendRoles).Random(x => x.Type != Type);
            CallRpc(CustomRPC.Misc, MiscRPC.ChangeRoles, this, role);
            TurnRole(role);
        }
    }

    public void FillRoles(PlayerControl target)
    {
        if (!target.Is(LayerEnum.Actor))
            PretendRoles.Add(target.GetRole());

        var targets = CustomPlayer.AllPlayers.Where(x => x != Player && x != target && !x.Is(LayerEnum.Actor)).ToList();
        targets.Shuffle();

        foreach (var player in targets)
        {
            if (PretendRoles.Count >= ActorRoleCount)
                break;

            PretendRoles.Add(player.GetRole());
        }

        PretendRoles.Shuffle();
        Targeted = true;
    }
}