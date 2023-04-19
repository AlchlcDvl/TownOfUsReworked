using System;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Custom;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Vigilante : CrewRole
    {
        public PlayerControl ClosestPlayer;
        public DateTime LastKilled;
        public bool KilledInno;
        public bool PreMeetingDie;
        public bool PostMeetingDie;
        public bool InnoMessage;
        public CustomButton ShootButton;
        public int UsesLeft;
        public bool ButtonUsable => UsesLeft > 0;
        public bool FirstRound;

        public Vigilante(PlayerControl player) : base(player)
        {
            Name = "Vigilante";
            StartText = "Shoot The <color=#FF0000FF>Evildoers</color>";
            AbilitiesText = "- You can shoot players\n- You you shoot someone you are not supposed to, you will die to guilt";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Vigilante : Colors.Crew;
            RoleType = RoleEnum.Vigilante;
            RoleAlignment = RoleAlignment.CrewKill;
            AlignmentName = CK;
            InspectorResults = InspectorResults.UsesGuns;
            UsesLeft = CustomGameOptions.VigiBulletCount;
            Type = LayerEnum.Vigilante;
            ShootButton = new(this, AssetManager.Shoot, AbilityTypes.Direct, "ActionSecondary", Shoot, true);
        }

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastKilled;
            var num = Player.GetModifiedCooldown(CustomGameOptions.VigiKillCd) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public override void OnMeetingStart(MeetingHud __instance)
        {
            base.OnMeetingStart(__instance);

            foreach (var vigi in GetRoles<Vigilante>(RoleEnum.Vigilante))
            {
                if (vigi.PreMeetingDie)
                    Utils.RpcMurderPlayer(vigi.Player, vigi.Player);
                else if (vigi.Player == PlayerControl.LocalPlayer && vigi.InnoMessage)
                    HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "You an innocent an innocent crew! You have put your gun away out of guilt.");
            }
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            ShootButton.Update("SHOOT", KillTimer(), CustomGameOptions.VigiKillCd, UsesLeft, ButtonUsable, ButtonUsable && !KilledInno && !FirstRound);
        }

        public void Shoot()
        {
            if (Utils.IsTooFar(Player, ClosestPlayer) || KillTimer() != 0f || KilledInno)
                return;

            var flag4 = ClosestPlayer.Is(Faction.Intruder) || ClosestPlayer.Is(RoleAlignment.NeutralKill) || ClosestPlayer.Is(Faction.Syndicate) || (ClosestPlayer.Is(RoleEnum.Jester) &&
                CustomGameOptions.VigiKillsJester) || (ClosestPlayer.Is(RoleEnum.Executioner) && CustomGameOptions.VigiKillsExecutioner) || (ClosestPlayer.Is(RoleEnum.Cannibal) &&
                CustomGameOptions.VigiKillsCannibal) || (ClosestPlayer.Is(RoleAlignment.NeutralBen) && CustomGameOptions.VigiKillsNB) || ClosestPlayer.Is(RoleAlignment.NeutralNeo) ||
                ClosestPlayer.Is(RoleAlignment.NeutralPros) || ClosestPlayer.IsFramed() || Player.IsFramed() || Player.NotOnTheSameSide() || ClosestPlayer.NotOnTheSameSide() ||
                Player.Is(ObjectifierEnum.Corrupted) || ClosestPlayer.Is(RoleEnum.Troll) || (ClosestPlayer.Is(RoleEnum.BountyHunter) && CustomGameOptions.VigiKillsBH) ||
                (ClosestPlayer.Is(RoleEnum.Actor) && CustomGameOptions.VigiKillsActor);
            var interact = Utils.Interact(Player, ClosestPlayer, flag4);

            if (interact[3])
            {
                if (flag4 && !Player.IsFramed())
                {
                    KilledInno = false;
                    LastKilled = DateTime.UtcNow;
                }
                else
                {
                    if (CustomGameOptions.MisfireKillsInno || ClosestPlayer.IsFramed())
                        Utils.RpcMurderPlayer(Player, ClosestPlayer);

                    if (Player == PlayerControl.LocalPlayer && CustomGameOptions.VigiNotifOptions == VigiNotif.Flash && CustomGameOptions.VigiOptions != VigiOptions.Immediate)
                        Utils.Flash(Color, "Your target was innocent!");
                    else if (CustomGameOptions.VigiNotifOptions == VigiNotif.Message && CustomGameOptions.VigiOptions != VigiOptions.Immediate)
                        InnoMessage = true;

                    LastKilled = DateTime.UtcNow;
                    KilledInno = !CustomGameOptions.VigiKillAgain;

                    if (CustomGameOptions.VigiOptions == VigiOptions.Immediate)
                        Utils.RpcMurderPlayer(Player, Player);
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