namespace TownOfUsReworked.PlayerLayers.Roles;

public class Vigilante : Crew
{
    public bool KilledInno { get; set; }
    public bool PreMeetingDie { get; set; }
    public bool PostMeetingDie { get; set; }
    public bool InnoMessage { get; set; }
    public CustomButton ShootButton { get; set; }
    public bool RoundOne { get; set; }

    public override UColor Color => ClientGameOptions.CustomCrewColors ? CustomColorManager.Vigilante : CustomColorManager.Crew;
    public override string Name => "Vigilante";
    public override LayerEnum Type => LayerEnum.Vigilante;
    public override Func<string> StartText => () => "Shoot The <color=#FF0000FF>Evildoers</color>";
    public override Func<string> Description => () => "- You can shoot players\n- If you shoot someone you're not supposed to, you will die to guilt";
    public override AttackEnum AttackVal => AttackEnum.Basic;

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.CrewKill;
        ShootButton = CreateButton(this, "SHOOT", new SpriteName("Shoot"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClick)Shoot, new Cooldown(CustomGameOptions.ShootCd),
            (PlayerBodyExclusion)Exception, CustomGameOptions.MaxBullets, (UsableFunc)Usable);
        RoundOne = CustomGameOptions.RoundOneNoShot;
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
            Alignment.NeutralApoc || target.Is(Faction.Syndicate) || target.Is(LayerEnum.Troll) || (target.Is(LayerEnum.Jester) && CustomGameOptions.VigiKillsJester) ||
            (target.Is(LayerEnum.Executioner) && CustomGameOptions.VigiKillsExecutioner) || (target.Is(LayerEnum.Cannibal) && CustomGameOptions.VigiKillsCannibal) || target.IsFramed() ||
            (target.Is(Alignment.NeutralBen) && CustomGameOptions.VigiKillsNB) || Player.IsFramed() || Player.NotOnTheSameSide() || target.NotOnTheSameSide() ||
            Player.Is(LayerEnum.Corrupted) || (target.Is(LayerEnum.Actor) && CustomGameOptions.VigiKillsActor) || (target.Is(LayerEnum.BountyHunter) && CustomGameOptions.VigiKillsBH);
        var cooldown = Interact(Player, target, flag4);

        if (cooldown != CooldownType.Fail)
        {
            if (flag4 && !Player.IsFramed())
                KilledInno = false;
            else
            {
                if (CustomGameOptions.MisfireKillsInno)
                    RpcMurderPlayer(Player, target);

                if (Local && CustomGameOptions.VigiNotifOptions == VigiNotif.Flash && CustomGameOptions.VigiOptions != VigiOptions.Immediate)
                    Flash(Color);

                KilledInno = !CustomGameOptions.VigiKillAgain;
                InnoMessage = CustomGameOptions.VigiNotifOptions == VigiNotif.Message && CustomGameOptions.VigiOptions != VigiOptions.Immediate;
                PreMeetingDie = CustomGameOptions.VigiOptions == VigiOptions.PreMeeting;
                PostMeetingDie = CustomGameOptions.VigiOptions == VigiOptions.PostMeeting;

                if (CustomGameOptions.VigiOptions == VigiOptions.Immediate)
                    RpcMurderPlayer(Player);
            }
        }

        ShootButton.StartCooldown(cooldown);
    }
}