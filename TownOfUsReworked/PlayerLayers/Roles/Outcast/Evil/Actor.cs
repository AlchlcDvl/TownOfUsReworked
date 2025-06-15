namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(Layer.Actor)]
public sealed class Actor : Evil
{
    [ToggleOption]
    public static bool ActorCanPickRole = false;

    [ToggleOption]
    public static bool ActorButton = true;

    [ToggleOption]
    private static bool ActorVent = false;

    [ToggleOption]
    private static bool ActSwitchVent = false;

    [NumberOption(1, 5, 1)]
    private static Number ActorRoleCount = 3;

    private static bool Failed => !GetLayers<Assassin>().Any(x => x.Alive);

    private bool TargetFailed => !Targeted && Rounds > 0;

    public bool Guessed;
    public readonly List<Layer> PretendRoles = [];

    private CustomButton PretendButton;
    private int Rounds;
    private bool Targeted;

    protected override UColor MainColor => CustomColorManager.Actor;
    public override Layer Type => Layer.Actor;
    public override string StartText => "Play Pretend With The Others";
    public override string Description => !Targeted ? "- You can select a player whose role you can pretend to be" : "- Upon being guessed, you will kill your guesser";
    public override Attack Attack => Attack.Unstoppable;
    public override bool HasWon => Guessed;
    public override bool CanVent => base.CanVent && ActorVent;
    public override bool CanSwitchVents => ActSwitchVent;
    protected override WinLose EndState => WinLose.ActorWins;

    public override void Init()
    {
        base.Init();
        Objectives = () => Guessed ? "- You have successfully fooled the crew" : (!Targeted ? "- Find a set of roles you must pretend to be" : ("- Get guessed as one of your target roles\n" +
            $"- Your target roles are {PretendListString()}"));
        PretendButton ??= new(this, new SpriteName("Pretend"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)PickRole, "PRETEND", (UsableFunc)Usable);
        PretendRoles.Clear();
    }

    public override void Reset(bool meeting, bool start)
    {
        if (meeting && !Targeted)
            Rounds++;
    }

    public string PretendListString()
    {
        var text = "";

        foreach (var (i, target) in PretendRoles.Indexed())
            text += i == PretendRoles.Count ? $"and {LayerDictionary[target].Name}" : $"{LayerDictionary[target].Name}, ";

        return text;
    }

    private void PickRole(PlayerControl target)
    {
        FillRoles(target);
        CallRpc(ReworkedRpc.Misc, MiscRpc.SetTarget, this, PretendRoles);
    }

    public void TurnRole(Layer role)
    {
        Role newRole = role switch
        {
            // Crew roles
            Layer.Altruist => new Altruist(),
            Layer.Bastion => new Bastion(),
            Layer.Chameleon => new Chameleon(),
            Layer.Coroner => new Coroner(),
            Layer.Crewmate => new Crewmate(),
            Layer.Detective => new Detective(),
            Layer.Dictator => new Dictator(),
            Layer.Engineer => new Engineer(),
            Layer.Escort => new Escort(),
            Layer.Mayor or Layer.Democrat => new Democrat(),
            Layer.Medic => new Medic(),
            Layer.Medium => new Medium(),
            Layer.Monarch => new Monarch(),
            Layer.Mystic => new Mystic(),
            Layer.Operative => new Operative(),
            Layer.Retributionist => new Retributionist(),
            Layer.Sheriff => new Sheriff(),
            Layer.Seer => new Seer(),
            Layer.Shifter => new Shifter(),
            Layer.Tracker => new Tracker(),
            Layer.Transporter => new Transporter(),
            Layer.Trapper => new Trapper(),
            Layer.Veteran => new Veteran(),
            Layer.Vigilante => new Vigilante(),

            // Outcast roles
            Layer.Amnesiac => new Amnesiac(),
            Layer.Arsonist => new Arsonist(),
            Layer.Betrayer => new Betrayer(),
            Layer.BountyHunter => new BountyHunter(),
            Layer.Cannibal => new Cannibal(),
            Layer.Cryomaniac => new Cryomaniac(),
            Layer.Dracula => new Dracula(),
            Layer.Executioner => new Executioner(),
            Layer.Glitch => new Glitch(),
            Layer.GuardianAngel => new GuardianAngel(),
            Layer.Guesser => new Guesser(),
            Layer.Jackal => new Jackal(),
            Layer.Jester => new Jester(),
            Layer.Juggernaut => new Juggernaut(),
            Layer.Murderer => new Murderer(),
            Layer.Necromancer => new Necromancer(),
            Layer.SerialKiller => new SerialKiller(),
            Layer.Survivor => new Survivor(),
            Layer.Thief => new Thief(),
            Layer.Troll => new Troll(),
            Layer.Werewolf => new Werewolf(),
            Layer.Whisperer => new Whisperer(),

            // Intruder roles
            Layer.Ambusher => new Ambusher(),
            Layer.Blackmailer => new Blackmailer(),
            Layer.Camouflager => new Camouflager(),
            Layer.Consigliere => new Consigliere(),
            Layer.Consort => new Consort(),
            Layer.Disguiser => new Disguiser(),
            Layer.Enforcer => new Enforcer(),
            Layer.Godfather => new Godfather(),
            Layer.Grenadier => new Grenadier(),
            Layer.Impostor => new Impostor(),
            Layer.Janitor => new Janitor(),
            Layer.Miner => new Miner(),
            Layer.Morphling => new Morphling(),
            Layer.Teleporter => new Teleporter(),
            Layer.Wraith => new Wraith(),

            // Syndicate roles
            Layer.Anarchist => new Anarchist(),
            Layer.Bomber => new Bomber(),
            Layer.Collider => new Collider(),
            Layer.Concealer => new Concealer(),
            Layer.Crusader => new Crusader(),
            Layer.Drunkard => new Drunkard(),
            Layer.Framer => new Framer(),
            Layer.Poisoner => new Poisoner(),
            Layer.Rebel => new Rebel(),
            Layer.Shapeshifter => new Shapeshifter(),
            Layer.Silencer => new Silencer(),
            Layer.Spellslinger => new Spellslinger(),
            Layer.Stalker => new Stalker(),
            Layer.Timekeeper => new Timekeeper(),
            Layer.Warper => new Warper(),

            // Apocalypse roles
            Layer.Cultist or Layer.Void => new Cultist(),
            Layer.Plaguebearer or Layer.Pestilence => new Plaguebearer(),

            // Whatever remains
            Layer.Actor or _ => new Amnesiac(),
        };

        newRole.RoleUpdate(this);
    }

    private bool Usable() => !Targeted;

    public override void UpdateHud(HudManager __instance)
    {
        if ((!TargetFailed && (!Targeted || !Failed)) || Dead)
            return;

        var allRoles = GetLayers<Role>().Select(x => x.Type);
        var target = GetValuesFromTo(Layer.Altruist, Layer.Warper, x => x is not (Layer.Phantom or Layer.Revealer or Layer.Banshee or Layer.Ghoul or Layer.Actor
            or Layer.Sidekick or Layer.Mafioso or Layer.Betrayer or Layer.Amnesiac) && allRoles.All(y => y != x)).Random(Layer.Amnesiac);

        if (target == Layer.Amnesiac)
        {
            var targetList = (TargetFailed ? allRoles : PretendRoles).Where(x => x is not (Layer.Phantom or Layer.Revealer or Layer.Banshee or Layer.Ghoul or
                Layer.Actor or Layer.Sidekick or Layer.Mafioso));
            target = targetList.Random();
        }

        CallRpc(ReworkedRpc.Misc, MiscRpc.ChangeRoles, this, target);
        TurnRole(target);
    }

    public void FillRoles(PlayerControl target)
    {
        if (!target.Is<Actor>())
            PretendRoles.Add(target.GetRole().Type);

        var targets = AllPlayers().Where(x => x != Player && x != target && !x.Is<Actor>()).Select(x => x.GetRole().Type);
        var count = (int)ActorRoleCount;

        if (targets.Count() < count)
            count = targets.Count();

        while (PretendRoles.Count < count)
            PretendRoles.Add(targets.Random(x => !PretendRoles.Contains(x)));

        PretendRoles.Shuffle();
        Targeted = true;
    }
}