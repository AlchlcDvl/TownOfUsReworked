namespace TownOfUsReworked.PlayerLayers.Roles;

public class Vigilante : Crew
{
    public DateTime LastKilled { get; set; }
    public bool KilledInno { get; set; }
    public bool PreMeetingDie { get; set; }
    public bool PostMeetingDie { get; set; }
    public bool InnoMessage { get; set; }
    public CustomButton ShootButton { get; set; }
    public int UsesLeft { get; set; }
    public bool ButtonUsable => UsesLeft > 0;
    public bool RoundOne { get; set; }
    public float Timer => ButtonUtils.Timer(Player, LastKilled, CustomGameOptions.VigiKillCd);

    public override Color32 Color => ClientGameOptions.CustomCrewColors ? Colors.Vigilante : Colors.Crew;
    public override string Name => "Vigilante";
    public override LayerEnum Type => LayerEnum.Vigilante;
    public override Func<string> StartText => () => "Shoot The <color=#FF0000FF>Evildoers</color>";
    public override Func<string> Description => () => "- You can shoot players\n- You you shoot someone you are not supposed to, you will die to guilt";
    public override InspectorResults InspectorResults => InspectorResults.IsCold;

    public Vigilante(PlayerControl player) : base(player)
    {
        RoleAlignment = RoleAlignment.CrewKill;
        UsesLeft = CustomGameOptions.VigiBulletCount;
        ShootButton = new(this, "Shoot", AbilityTypes.Direct, "ActionSecondary", Shoot, true);
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        base.OnMeetingStart(__instance);

        if (PreMeetingDie)
            RpcMurderPlayer(Player, Player);
        else if (InnoMessage)
            HUD.Chat.AddChat(CustomPlayer.Local, "You killed an innocent an innocent crew! You have put your gun away out of guilt.");
    }

    public bool Exception(PlayerControl player) => player.Is(Faction) || (player.Is(SubFaction) && SubFaction != SubFaction.None) || Player.IsLinkedTo(player);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        ShootButton.Update("SHOOT", Timer, CustomGameOptions.VigiKillCd, UsesLeft, ButtonUsable, ButtonUsable && !KilledInno && !RoundOne);
    }

    public void Shoot()
    {
        if (IsTooFar(Player, ShootButton.TargetPlayer) || Timer != 0f || KilledInno || RoundOne)
            return;

        var target = ShootButton.TargetPlayer;
        var flag4 = target.Is(Faction.Intruder) || target.Is(RoleAlignment.NeutralKill) || target.Is(Faction.Syndicate) || target.Is(LayerEnum.Troll) || (target.Is(LayerEnum.Jester) &&
            CustomGameOptions.VigiKillsJester) || (target.Is(LayerEnum.Executioner) && CustomGameOptions.VigiKillsExecutioner) || (target.Is(LayerEnum.Cannibal) &&
            CustomGameOptions.VigiKillsCannibal) || (target.Is(RoleAlignment.NeutralBen) && CustomGameOptions.VigiKillsNB) || target.Is(RoleAlignment.NeutralNeo) ||
            target.Is(RoleAlignment.NeutralPros) || target.IsFramed() || Player.IsFramed() || Player.NotOnTheSameSide() || target.NotOnTheSameSide() ||
            Player.Is(LayerEnum.Corrupted) || (target.Is(LayerEnum.BountyHunter) && CustomGameOptions.VigiKillsBH) || (target.Is(LayerEnum.Actor) &&
            CustomGameOptions.VigiKillsActor) || target.Is(RoleAlignment.NeutralHarb);
        var interact = Interact(Player, target, flag4);

        if (interact[3])
        {
            if (flag4 && !Player.IsFramed())
            {
                KilledInno = false;
                LastKilled = DateTime.UtcNow;
            }
            else
            {
                if (CustomGameOptions.MisfireKillsInno)
                    RpcMurderPlayer(Player, target);

                if (Local && CustomGameOptions.VigiNotifOptions == VigiNotif.Flash && CustomGameOptions.VigiOptions != VigiOptions.Immediate)
                    Flash(Color);
                else if (CustomGameOptions.VigiNotifOptions == VigiNotif.Message && CustomGameOptions.VigiOptions != VigiOptions.Immediate)
                    InnoMessage = true;

                LastKilled = DateTime.UtcNow;
                KilledInno = !CustomGameOptions.VigiKillAgain;

                if (CustomGameOptions.VigiOptions == VigiOptions.Immediate)
                    RpcMurderPlayer(Player, Player);
                else if (CustomGameOptions.VigiOptions == VigiOptions.PreMeeting)
                    PreMeetingDie = true;
                else if (CustomGameOptions.VigiOptions == VigiOptions.PostMeeting)
                    PostMeetingDie = true;
            }

            UsesLeft--;
        }
        else if (interact[0])
            LastKilled = DateTime.UtcNow;
        else if (interact[1])
            LastKilled.AddSeconds(CustomGameOptions.ProtectKCReset);
        else if (interact[2])
            LastKilled.AddSeconds(CustomGameOptions.VestKCReset);
    }
}