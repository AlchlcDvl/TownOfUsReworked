namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Actor : Evil
{
    [ToggleOption]
    public static bool ActorCanPickRole = false;

    [ToggleOption]
    public static bool ActorButton = true;

    [ToggleOption]
    public static bool ActorVent = false;

    [ToggleOption]
    public static bool ActSwitchVent = false;

    [NumberOption(1, 5, 1)]
    public static Number ActorRoleCount = 3;

    public bool Guessed { get; set; }
    public List<Role> PretendRoles { get; } = [];
    public bool Failed => !Ability.GetAssassins().Any(x => x.Alive);
    public CustomButton PretendButton { get; set; }
    public int Rounds { get; set; }
    public bool TargetFailed => !Targeted && Rounds > 0;
    public bool Targeted { get; set; }

    public override UColor Color => ClientOptions.CustomNeutColors ? CustomColorManager.Actor : FactionColor;
    public override LayerEnum Type => LayerEnum.Actor;
    public override Func<string> StartText => () => "Play Pretend With The Others";
    public override Func<string> Description => () => !Targeted ? "- You can select a player whose role you can pretend to be" : "- Upon being guessed, you will kill your guesser";
    public override AttackEnum AttackVal => AttackEnum.Unstoppable;
    public override bool HasWon => Guessed;
    public override WinLose EndState => WinLose.ActorWins;

    public override void Init()
    {
        base.Init();
        Objectives = () => Guessed ? "- You have successfully fooled the crew" : (!Targeted ? "- Find a set of roles you must pretend to be" : ("- Get guessed as one of your target roles\n" +
            $"- Your target roles are {PretendListString()}"));
        PretendButton ??= new(this, new SpriteName("Pretend"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)PickRole, "PRETEND", (UsableFunc)Usable);
        PretendRoles.Clear();
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

    public void PickRole(PlayerControl target)
    {
        FillRoles(target);
        CallRpc(CustomRPC.Misc, MiscRPC.SetTarget, this, PretendRoles);
    }

    public void TurnRole(LayerEnum role)
    {
        Role newRole = role switch
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
            LayerEnum.Veteran => new Veteran(),
            LayerEnum.Vigilante => new Vigilante(),

            // Neutral roles
            LayerEnum.Amnesiac => new Amnesiac(),
            LayerEnum.Arsonist => new Arsonist(),
            LayerEnum.Betrayer => new Betrayer(),
            LayerEnum.BountyHunter => new BountyHunter(),
            LayerEnum.Cannibal => new Cannibal(),
            LayerEnum.Cryomaniac => new Cryomaniac(),
            LayerEnum.Dracula => new Dracula(),
            LayerEnum.Executioner => new Executioner(),
            LayerEnum.Glitch => new Glitch(),
            LayerEnum.GuardianAngel => new GuardianAngel(),
            LayerEnum.Guesser => new Guesser(),
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
            LayerEnum.Grenadier => new Grenadier(),
            LayerEnum.Impostor => new Impostor(),
            LayerEnum.Janitor => new Janitor(),
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
            LayerEnum.Shapeshifter => new Shapeshifter(),
            LayerEnum.Silencer => new Silencer(),
            LayerEnum.Spellslinger => new Spellslinger(),
            LayerEnum.Stalker => new Stalker(),
            LayerEnum.Timekeeper => new Timekeeper(),
            LayerEnum.Warper => new Warper(),

            // Whatever remains
            LayerEnum.Actor or _ => new Amnesiac(),
        };

        newRole.RoleUpdate(this);
    }

    public bool Usable() => !Targeted;

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);

        if ((TargetFailed || (Targeted && Failed)) && !Dead)
        {
            var allRoles = GetLayers<Role>();
            var target = GetValuesFromTo(LayerEnum.Altruist, LayerEnum.Warper, x => x is not LayerEnum.Phantom or LayerEnum.Revealer or LayerEnum.Banshee or LayerEnum.Ghoul or LayerEnum.Actor or
                LayerEnum.PromotedGodfather or LayerEnum.PromotedRebel or LayerEnum.Sidekick or LayerEnum.Mafioso or LayerEnum.Betrayer or LayerEnum.Amnesiac && !allRoles.Any(y => y.Type == x))
                    .Random(LayerEnum.Amnesiac);

            if (target == LayerEnum.Amnesiac)
            {
                var targetList = (TargetFailed ? allRoles : PretendRoles).Where(x => x.Type is not LayerEnum.Phantom or LayerEnum.Revealer or LayerEnum.Banshee or LayerEnum.Ghoul or
                    LayerEnum.Actor or LayerEnum.PromotedGodfather or LayerEnum.PromotedRebel or LayerEnum.Sidekick or LayerEnum.Mafioso);
                target = (targetList.Random(x => x.Dead) ?? targetList.Random())?.Type ?? LayerEnum.Amnesiac;
            }

            CallRpc(CustomRPC.Misc, MiscRPC.ChangeRoles, this, target);
            TurnRole(target);
        }
    }

    public void FillRoles(PlayerControl target)
    {
        if (!target.Is<Actor>())
            PretendRoles.Add(target.GetRole());

        var targets = AllPlayers().Where(x => x != Player && x != target && !x.Is<Actor>()).Select(x => x.GetRole());
        var count = (int)ActorRoleCount;

        if (targets.Count() < count)
            count = targets.Count();

        while (PretendRoles.Count < count)
            PretendRoles.Add(targets.Random(x => !PretendRoles.Contains(x)));

        PretendRoles.Shuffle();
        Targeted = true;
    }
}