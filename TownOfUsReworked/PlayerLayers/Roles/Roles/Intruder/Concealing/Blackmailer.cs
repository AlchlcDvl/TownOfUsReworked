using System;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;
using TownOfUsReworked.Custom;
using Hazel;
using TownOfUsReworked.Classes;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Blackmailer : IntruderRole
    {
        public CustomButton BlackmailButton;
        public PlayerControl BlackmailedPlayer;
        public PlayerControl ClosestBlackmail;
        public DateTime LastBlackmailed;

        public Blackmailer(PlayerControl player) : base(player)
        {
            Name = "Blackmailer";
            StartText = "You Know Their Secrets";
            AbilitiesText = "- You can blackmail players to ensure they cannot speak in the next meeting\n- Everyone will be alerted at the start of the meeting that someone has been" +
                $" blackmailed{(CustomGameOptions.WhispersNotPrivate ? "\n- You can read whispers during meetings." : "")}\n{AbilitiesText}";
            Color = CustomGameOptions.CustomIntColors ? Colors.Blackmailer : Colors.Intruder;
            RoleType = RoleEnum.Blackmailer;
            RoleAlignment = RoleAlignment.IntruderConceal;
            AlignmentName = IC;
            InspectorResults = InspectorResults.HasInformation;
            BlackmailedPlayer = null;
            Type = LayerEnum.Blackmailer;
            BlackmailButton = new(this, AssetManager.Blackmail, AbilityTypes.Direct, "Secondary", Blackmail);
        }

        public float BlackmailTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastBlackmailed;
            var num = Player.GetModifiedCooldown(CustomGameOptions.BlackmailCd) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Blackmail()
        {
            if (BlackmailTimer() != 0f || Utils.IsTooFar(Player, ClosestBlackmail) || ClosestBlackmail == BlackmailedPlayer)
                return;

            var interact = Utils.Interact(Player, ClosestBlackmail);

            if (interact[3])
            {
                BlackmailedPlayer = ClosestBlackmail;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.Blackmail);
                writer.Write(Player.PlayerId);
                writer.Write(ClosestBlackmail.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }

            if (interact[0])
                LastBlackmailed = DateTime.UtcNow;
            else if (interact[1])
                LastBlackmailed.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            var notBlackmailed = PlayerControl.AllPlayerControls.ToArray().Where(player => BlackmailedPlayer != player).ToList();
            BlackmailButton.Update("BLACKMAIL", BlackmailTimer(), CustomGameOptions.BlackmailCd, notBlackmailed);
        }
    }
}