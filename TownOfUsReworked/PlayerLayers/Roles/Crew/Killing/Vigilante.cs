namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu2.LayerSubOptions)]
public class Vigilante : Crew
{
    [ToggleOption(MultiMenu2.LayerSubOptions)]
    public static bool MisfireKillsInno { get; set; } = true;

    [ToggleOption(MultiMenu2.LayerSubOptions)]
    public static bool VigiKillAgain { get; set; } = true;

    [ToggleOption(MultiMenu2.LayerSubOptions)]
    public static bool RoundOneNoShot { get; set; } = true;

    [StringOption(MultiMenu2.LayerSubOptions)]
    public static VigiOptions HowDoesVigilanteDie { get; set; } = VigiOptions.Immediate;

    [StringOption(MultiMenu2.LayerSubOptions)]
    public static VigiNotif HowIsVigilanteNotified { get; set; } = VigiNotif.Never;

    [NumberOption(MultiMenu2.LayerSubOptions, 1, 15, 1)]
    public static int MaxBullets { get; set; } = 5;

    [NumberOption(MultiMenu2.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static float ShootCd { get; set; } = 25f;

    public bool KilledInno { get; set; }
    public bool PreMeetingDie { get; set; }
    public bool PostMeetingDie { get; set; }
    public bool InnoMessage { get; set; }
    public CustomButton ShootButton { get; set; }
    public bool RoundOne { get; set; }

    public override UColor Color => ClientOptions.CustomCrewColors ? CustomColorManager.Vigilante : CustomColorManager.Crew;
    public override string Name => "Vigilante";
    public override LayerEnum Type => LayerEnum.Vigilante;
    public override Func<string> StartText => () => "Shoot The <color=#FF0000FF>Evildoers</color>";
    public override Func<string> Description => () => "- You can shoot players\n- If you shoot someone you're not supposed to, you will die to guilt";
    public override AttackEnum AttackVal => AttackEnum.Basic;

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.CrewKill;
        ShootButton = CreateButton(this, "SHOOT", new SpriteName("Shoot"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClick)Shoot, new Cooldown(ShootCd),
            (PlayerBodyExclusion)Exception, MaxBullets, (UsableFunc)Usable);
        RoundOne = RoundOneNoShot;
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        base.OnMeetingStart(__instance);
        RoundOne = false;

        if (PreMeetingDie)
            RpcMurderPlayer(Player);
        else if (InnoMessage)
            Run("<color=#FFFF00FF>〖 How Dare You 〗</color>", "You killed an innocent an innocent crew! You have put your gun away out of guilt.");
    }

    public bool Exception(PlayerControl player) => (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) && SubFaction != SubFaction.None) ||
        Player.IsLinkedTo(player);

    public bool Usable() => !KilledInno && !RoundOne;

    public void Shoot()
    {
        var target = ShootButton.TargetPlayer;
        var flag4 = target.Is(Faction.Intruder) || target.GetAlignment() is Alignment.NeutralKill or Alignment.NeutralNeo or Alignment.NeutralPros or Alignment.NeutralHarb or
            Alignment.NeutralApoc || target.Is(Faction.Syndicate) || target.Is(LayerEnum.Troll) || (target.Is(LayerEnum.Jester) && Jester.VigiKillsJester) || Player.IsFramed() ||
            (target.Is(LayerEnum.Executioner) && Executioner.VigiKillsExecutioner) || (target.Is(LayerEnum.Cannibal) && Cannibal.VigiKillsCannibal) || target.IsFramed() ||
            (target.Is(Alignment.NeutralBen) && CustomGameOptions.VigiKillsNB) || Player.NotOnTheSameSide() || target.NotOnTheSameSide() || Player.Is(LayerEnum.Corrupted) ||
            (target.Is(LayerEnum.Actor) && Actor.VigiKillsActor) || (target.Is(LayerEnum.BountyHunter) && BountyHunter.VigiKillsBH);
        var cooldown = Interact(Player, target, flag4);

        if (cooldown != CooldownType.Fail)
        {
            if (flag4 && !Player.IsFramed())
                KilledInno = false;
            else
            {
                if (MisfireKillsInno)
                    RpcMurderPlayer(Player, target);

                if (Local && HowIsVigilanteNotified == VigiNotif.Flash && HowDoesVigilanteDie != VigiOptions.Immediate)
                    Flash(Color);

                KilledInno = !VigiKillAgain;
                InnoMessage = HowIsVigilanteNotified == VigiNotif.Message && HowDoesVigilanteDie != VigiOptions.Immediate;
                PreMeetingDie = HowDoesVigilanteDie == VigiOptions.PreMeeting;
                PostMeetingDie = HowDoesVigilanteDie == VigiOptions.PostMeeting;

                if (HowDoesVigilanteDie == VigiOptions.Immediate)
                    RpcMurderPlayer(Player);
            }
        }

        ShootButton.StartCooldown(cooldown);
    }
}