namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Vigilante)]
public sealed class Vigilante : CKilling
{
    [ToggleOption]
    private static bool MisfireKillsInno = true;

    [ToggleOption]
    private static bool VigiKillAgain = true;

    [ToggleOption]
    private static bool RoundOneNoShot = true;

    [StringOption<VigiOptions>]
    public static VigiOptions HowDoesVigilanteDie = VigiOptions.Immediate;

    [StringOption<VigiNotif>]
    private static VigiNotif HowIsVigilanteNotified = VigiNotif.Never;

    [NumberOption(0, 15, 1, zeroIsInf: true)]
    public static Number MaxBullets = 5;

    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number ShootCd = 25;

    private bool KilledInno { get; set; }
    private bool PreMeetingDie { get; set; }
    public bool PostMeetingDie { get; private set; }
    private bool InnoMessage { get; set; }
    private CustomButton ShootButton { get; set; }
    private bool RoundOne { get; set; }

    protected override UColor MainColor => CustomColorManager.Vigilante;
    public override LayerEnum Type => LayerEnum.Vigilante;
    public override Func<string> StartText { get; } = () => "Shoot The <#FF0000FF>Evildoers</color>";
    public override Func<string> Description => () => "- You can shoot players\n- If you shoot someone you're not supposed to, you will die to guilt";
    public override AttackEnum AttackVal => AttackEnum.Basic;

    protected override void Init()
    {
        base.Init();
        ShootButton ??= new(this, "SHOOT", new SpriteName("Shoot"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Shoot, new Cooldown(ShootCd), (PlayerBodyExclusion)Exception,
            MaxBullets, (UsableFunc)Usable);
        RoundOne = RoundOneNoShot;
    }

    public override void BeforeMeeting()
    {
        RoundOne = false;

        if (PreMeetingDie)
            Player.RpcSuicide();
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        base.OnMeetingStart(__instance);

        if (InnoMessage)
            Run("<#FFFF00FF>〖 How Dare You 〗</color>", "You killed an innocent an innocent crew! You have put your gun away out of guilt.");
    }

    private bool Exception(PlayerControl player) => (player.Is(Faction) && Faction.IsFactionedEvil()) || Player.IsLinkedTo(player);

    private bool Usable() => !KilledInno && !RoundOne;

    private void Shoot(PlayerControl target)
    {
        var flag4 = target.GetAlignment() is Alignment.Neophyte or Alignment.Proselyte || target.GetFaction() is not (Faction.Crew or Faction.Outcast) || target.Is<Troll>() || target.IsFramed() ||
            Player.IsFramed() || (target.Is(Alignment.Evil) && OutcastEvilSettings.VigilanteKillsEvils) || Player.Is<Corrupted>() || (target.Is(Alignment.Benign) &&
            OutcastBenignSettings.VigilanteKillsBenigns);
        var cooldown = Interact(Player, target, flag4);

        if (cooldown != CooldownType.Fail)
        {
            if (flag4 && !Player.IsFramed())
                KilledInno = false;
            else
            {
                if (MisfireKillsInno)
                    Player.RpcMurderPlayer(target);

                if (Local && HowIsVigilanteNotified == VigiNotif.Flash && HowDoesVigilanteDie != VigiOptions.Immediate)
                    Flash(Color);

                KilledInno = !VigiKillAgain;
                InnoMessage = HowIsVigilanteNotified == VigiNotif.Message && HowDoesVigilanteDie != VigiOptions.Immediate;
                PreMeetingDie = HowDoesVigilanteDie == VigiOptions.PreMeeting;
                PostMeetingDie = HowDoesVigilanteDie == VigiOptions.PostMeeting;

                if (HowDoesVigilanteDie == VigiOptions.Immediate)
                    Player.RpcSuicide();
            }
        }

        ShootButton.StartCooldown(cooldown);
    }
}