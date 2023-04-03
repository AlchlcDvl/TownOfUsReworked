using System;
using System.Collections.Generic;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Extensions;
using Hazel;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Framer : SyndicateRole
    {
        public AbilityButton FrameButton;
        public List<byte> Framed = new();
        public DateTime LastFramed;
        public PlayerControl ClosestFrame;

        public Framer(PlayerControl player) : base(player)
        {
            Name = "Framer";
            StartText = "Make Everyone Suspicious";
            AbilitiesText = "- You can frame players.\n- Framed players will die very easily to <color=#FFFF00FF>Vigilantes</color> and <color=#073763FF>Assassins</color>.\n- Framed " +
                "players will appear to have the wrong results to investigative roles till you are dead.";
            Type = RoleEnum.Framer;
            RoleAlignment = RoleAlignment.SyndicateDisruption;
            AlignmentName = SD;
            Color = CustomGameOptions.CustomSynColors ? Colors.Framer : Colors.Syndicate;
            Framed = new();
        }

        public float FrameTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastFramed;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.FrameCooldown, CustomButtons.GetUnderdogChange(Player)) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        public void Frame(PlayerControl player)
        {
            if (player.Is(Faction.Syndicate) || Framed.Contains(player.PlayerId))
                return;

            Framed.Add(player.PlayerId);
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.Frame);
            writer.Write(Player.PlayerId);
            writer.Write(player.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }
    }
}