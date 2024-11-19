namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Actor : Neutral
{
    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool ActorCanPickRole { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool ActorButton { get; set; } = true;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool ActorVent { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool ActSwitchVent { get; set; } = false;

    [NumberOption(MultiMenu.LayerSubOptions, 1, 5, 1)]
    public static Number ActorRoleCount { get; set; } = new(3);

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
        base.Init();
        Objectives = () => Guessed ? "- You have successfully fooled the crew" : (!Targeted ? "- Find a set of roles you must pretend to be" : ("- Get guessed as one of your target roles\n" +
            $"- Your target roles are {PretendListString()}"));
        Alignment = Alignment.NeutralEvil;
        PretendButton ??= new(this, new SpriteName("Pretend"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClick)PickRole, "PRETEND", (UsableFunc)Usable);
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
        FillRoles(PretendButton.GetTarget<PlayerControl>());
        CallRpc(CustomRPC.Misc, MiscRPC.SetTarget, this, PretendRoles);
    }

    public void TurnRole(Role role)
    {
        Role newRole = role switch
        {
            // Crew roles
            Altruist => new Altruist(),
            Bastion => new Bastion(),
            Chameleon => new Chameleon(),
            Coroner => new Coroner(),
            Crewmate => new Crewmate(),
            Detective => new Detective(),
            Dictator => new Dictator(),
            Engineer => new Engineer(),
            Escort => new Escort(),
            Mayor => new Mayor(),
            Medic => new Medic(),
            Medium => new Medium(),
            Monarch => new Monarch(),
            Mystic => new Mystic(),
            Operative => new Operative(),
            Retributionist => new Retributionist(),
            Sheriff => new Sheriff(),
            Seer => new Seer(),
            Shifter => new Shifter(),
            Tracker => new Tracker(),
            Transporter => new Transporter(),
            Trapper => new Trapper(),
            VampireHunter => new VampireHunter(),
            Veteran => new Veteran(),
            Vigilante => new Vigilante(),

            // Neutral roles
            Amnesiac => new Amnesiac(),
            Arsonist => new Arsonist(),
            Betrayer => new Betrayer(),
            BountyHunter bh => new BountyHunter() { TargetPlayer = bh.TargetPlayer },
            Cannibal => new Cannibal(),
            Cryomaniac => new Cryomaniac(),
            Dracula => new Dracula(),
            Executioner exe => new Executioner() { TargetPlayer = exe.TargetPlayer },
            Glitch => new Glitch(),
            GuardianAngel ga => new GuardianAngel() { TargetPlayer = ga.TargetPlayer },
            Guesser guesser => new Guesser() { TargetPlayer = guesser.TargetPlayer },
            Jackal => new Jackal(),
            Jester => new Jester(),
            Juggernaut => new Juggernaut(),
            Murderer => new Murderer(),
            Necromancer => new Necromancer(),
            Plaguebearer or Pestilence => new Plaguebearer(),
            SerialKiller => new SerialKiller(),
            Survivor => new Survivor(),
            Thief => new Thief(),
            Troll => new Troll(),
            Werewolf => new Werewolf(),
            Whisperer => new Whisperer(),

            // Intruder roles
            Ambusher => new Ambusher(),
            Blackmailer => new Blackmailer(),
            Camouflager => new Camouflager(),
            Consigliere => new Consigliere(),
            Consort => new Consort(),
            Disguiser => new Disguiser(),
            Enforcer => new Enforcer(),
            Godfather => new Godfather(),
            PromotedGodfather gf => new PromotedGodfather() { FormerRole = gf.FormerRole },
            Grenadier => new Grenadier(),
            Impostor => new Impostor(),
            Janitor => new Janitor(),
            Mafioso mafioso => new Mafioso() { Godfather = mafioso.Godfather },
            Miner => new Miner(),
            Morphling => new Morphling(),
            Teleporter => new Teleporter(),
            Wraith => new Wraith(),

            // Syndicate roles
            Anarchist => new Anarchist(),
            Bomber => new Bomber(),
            Collider => new Collider(),
            Concealer => new Concealer(),
            Crusader => new Crusader(),
            Drunkard => new Drunkard(),
            Framer => new Framer(),
            Poisoner => new Poisoner(),
            Rebel => new Rebel(),
            PromotedRebel reb => new PromotedRebel() { FormerRole = reb.FormerRole },
            Shapeshifter => new Shapeshifter(),
            Sidekick sidekick => new Sidekick() { Rebel = sidekick.Rebel },
            Silencer => new Silencer(),
            Spellslinger => new Spellslinger(),
            Stalker => new Stalker(),
            Timekeeper => new Timekeeper(),
            Warper => new Warper(),

            // Whatever remains
            Actor or _ => new Amnesiac(),
        };

        newRole.RoleUpdate(this, Player);
    }

    public bool Usable() => !Targeted;

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);

        if ((TargetFailed || (Targeted && Failed)) && !Dead)
        {
            var targetList = TargetFailed ? AllRoles() : PretendRoles;
            var role = targetList.Random(x => x.Type != Type && x.Dead) ?? targetList.Random(x => x.Type != Type);
            CallRpc(CustomRPC.Misc, MiscRPC.ChangeRoles, this, role);
            TurnRole(role);
        }
    }

    public void FillRoles(PlayerControl target)
    {
        if (!target.Is(LayerEnum.Actor))
            PretendRoles.Add(target.GetRole());

        var targets = AllPlayers().Where(x => x != Player && x != target && !x.Is(LayerEnum.Actor));

        while (PretendRoles.Count < ActorRoleCount)
            PretendRoles.Add(targets.Random().GetRole());

        PretendRoles.Shuffle();
        Targeted = true;
    }
}