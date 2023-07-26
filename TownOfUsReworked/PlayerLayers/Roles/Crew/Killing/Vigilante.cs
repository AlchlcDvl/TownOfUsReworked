namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Vigilante : Crew
    {
        public DateTime LastKilled;
        public bool KilledInno;
        public bool PreMeetingDie;
        public bool PostMeetingDie;
        public bool InnoMessage;
        public CustomButton ShootButton;
        public int UsesLeft;
        public bool ButtonUsable => UsesLeft > 0;
        public bool RoundOne;

        public override Color32 Color => ClientGameOptions.CustomCrewColors ? Colors.Vigilante : Colors.Crew;
        public override string Name => "Vigilante";
        public override LayerEnum Type => LayerEnum.Vigilante;
        public override RoleEnum RoleType => RoleEnum.Vigilante;
        public override Func<string> StartText => () => "Shoot The <color=#FF0000FF>Evildoers</color>";
        public override Func<string> AbilitiesText => () => "- You can shoot players\n- You you shoot someone you are not supposed to, you will die to guilt";
        public override InspectorResults InspectorResults => InspectorResults.IsCold;

        public Vigilante(PlayerControl player) : base(player)
        {
            RoleAlignment = RoleAlignment.CrewKill;
            UsesLeft = CustomGameOptions.VigiBulletCount;
            ShootButton = new(this, "Shoot", AbilityTypes.Direct, "ActionSecondary", Shoot, true);
        }

        public float KillTimer()
        {
            var timespan = DateTime.UtcNow - LastKilled;
            var num = Player.GetModifiedCooldown(CustomGameOptions.VigiKillCd) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
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
            ShootButton.Update("SHOOT", KillTimer(), CustomGameOptions.VigiKillCd, UsesLeft, ButtonUsable, ButtonUsable && !KilledInno && !RoundOne);
        }

        public void Shoot()
        {
            if (IsTooFar(Player, ShootButton.TargetPlayer) || KillTimer() != 0f || KilledInno || RoundOne)
                return;

            var target = ShootButton.TargetPlayer;
            var flag4 = target.Is(Faction.Intruder) || target.Is(RoleAlignment.NeutralKill) || target.Is(Faction.Syndicate) || target.Is(RoleEnum.Troll) || (target.Is(RoleEnum.Jester) &&
                CustomGameOptions.VigiKillsJester) || (target.Is(RoleEnum.Executioner) && CustomGameOptions.VigiKillsExecutioner) || (target.Is(RoleEnum.Cannibal) &&
                CustomGameOptions.VigiKillsCannibal) || (target.Is(RoleAlignment.NeutralBen) && CustomGameOptions.VigiKillsNB) || target.Is(RoleAlignment.NeutralNeo) ||
                target.Is(RoleAlignment.NeutralPros) || target.IsFramed() || Player.IsFramed() || Player.NotOnTheSameSide() || target.NotOnTheSameSide() ||
                Player.Is(ObjectifierEnum.Corrupted) || (target.Is(RoleEnum.BountyHunter) && CustomGameOptions.VigiKillsBH) || (target.Is(RoleEnum.Actor) &&
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
}