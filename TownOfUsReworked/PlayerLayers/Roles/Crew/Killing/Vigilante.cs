namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Vigilante : Crew
{
    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool MisfireKillsInno { get; set; } = true;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool VigiKillAgain { get; set; } = true;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool RoundOneNoShot { get; set; } = true;

    [StringOption(MultiMenu.LayerSubOptions)]
    public static VigiOptions HowDoesVigilanteDie { get; set; } = VigiOptions.Immediate;

    [StringOption(MultiMenu.LayerSubOptions)]
    public static VigiNotif HowIsVigilanteNotified { get; set; } = VigiNotif.Never;

    [NumberOption(MultiMenu.LayerSubOptions, 0, 15, 1, ZeroIsInfinity = true)]
    public static Number MaxBullets { get; set; } = new(5);

    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number ShootCd { get; set; } = new(25);

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
        ShootButton ??= CreateButton(this, "SHOOT", new SpriteName("Shoot"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClick)Shoot, new Cooldown(ShootCd),
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
        var target = ShootButton.GetTarget<PlayerControl>();
        var flag4 = target.Is(Faction.Intruder) || target.GetAlignment() is Alignment.NeutralKill or Alignment.NeutralNeo or Alignment.NeutralPros or Alignment.NeutralHarb or
            Alignment.NeutralApoc || target.Is(Faction.Syndicate) || target.Is(LayerEnum.Troll) || Player.NotOnTheSameSide() || target.NotOnTheSameSide() || Player.IsFramed() ||
            (target.Is(Alignment.NeutralEvil) && NeutralEvilSettings.VigilanteKillsEvils) || Player.Is(LayerEnum.Corrupted) || target.IsFramed() || (target.Is(Alignment.NeutralBen) &&
            NeutralBenignSettings.VigilanteKillsBenigns);
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