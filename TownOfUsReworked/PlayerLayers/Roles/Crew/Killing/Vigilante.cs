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

    public bool KilledInno { get; private set; }
    private bool KeepKilling { get; set; }
    private CustomButton ShootButton { get; set; }
    private bool RoundOne { get; set; }

    protected override UColor MainColor => CustomColorManager.Vigilante;
    public override LayerEnum Type => LayerEnum.Vigilante;
    public override Func<string> StartText { get; } = () => "Shoot The <#FF0000FF>Evildoers</color>";
    public override Func<string> Description => () => "- You can shoot players\n- If you shoot someone you're not supposed to, you will die to guilt";
    public override AttackEnum AttackVal => AttackEnum.Basic;

    public override void Init()
    {
        base.Init();
        ShootButton ??= new(this, "SHOOT", new SpriteName("Shoot"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Shoot, new Cooldown(ShootCd), (PlayerBodyExclusion)Exception,
            MaxBullets, (UsableFunc)Usable);
        RoundOne = RoundOneNoShot;
        KeepKilling = true;
    }

    public override void BeforeMeeting()
    {
        RoundOne = false;

        if (KilledInno && HowDoesVigilanteDie == VigiOptions.PreMeeting)
            Player.RpcSuicide();
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        base.OnMeetingStart(__instance);

        if (KilledInno && HowIsVigilanteNotified == VigiNotif.Message && HowDoesVigilanteDie != VigiOptions.Immediate)
            Run("<#FFFF00FF>〖 How Dare You 〗</color>", "You killed an innocent an innocent crew! You have put your gun away out of guilt.");
    }

    private bool Exception(PlayerControl player) => Player.IsBuddyWith(player, Faction);

    private bool Usable() => KeepKilling && !RoundOne;

    private void Shoot(PlayerControl target)
    {
        var targetRole = target.GetRole();
        var toKill = targetRole.Alignment is Alignment.Neophyte or Alignment.Proselyte || targetRole.Faction.IsFactionedEvil() || targetRole is Troll || target.IsFramed() || Player.IsFramed() ||
            (targetRole.Alignment == Alignment.Evil && OutcastEvilSettings.VigilanteKillsEvils) || Player.Is<Corrupted>() || target.Is<Corrupted>() || (targetRole.Alignment == Alignment.Benign &&
            OutcastBenignSettings.VigilanteKillsBenigns);
        var cooldown = Interact(Player, target, toKill);

        if (cooldown != CooldownType.Fail)
        {
            if (toKill && !Player.IsFramed() && !target.IsFramed())
                KilledInno = false;
            else
            {
                if (MisfireKillsInno)
                    Player.RpcMurderPlayer(target);

                if (Local && HowIsVigilanteNotified == VigiNotif.Flash && HowDoesVigilanteDie != VigiOptions.Immediate)
                    Flash(Color);

                KeepKilling = VigiKillAgain;
                KilledInno = true;

                if (HowDoesVigilanteDie == VigiOptions.Immediate)
                    Player.RpcSuicide();
            }

            Play("Shoot");
        }

        ShootButton.StartCooldown(cooldown);
    }
}