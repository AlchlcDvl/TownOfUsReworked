using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Data;
using TownOfUsReworked.Custom;
using Hazel;
using System.Collections.Generic;
using System;
using System.Linq;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Monarch : CrewRole
    {
        public bool RoundOne;
        public CustomButton KnightButton;
        public List<byte> ToBeKnighted = new();
        public List<byte> Knighted = new();
        public DateTime LastKnighted;
        public int UsesLeft;
        public bool ButtonUsable => UsesLeft > 0;

        public Monarch(PlayerControl player) : base(player)
        {
            Name = "Monarch";
            StartText = "Knight Those Who You Trust";
            AbilitiesText = $"- You can knight players\n- Knighted players will have their votes count {CustomGameOptions.KnightVoteCount + 1} times";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Monarch : Colors.Crew;
            RoleType = RoleEnum.Monarch;
            RoleAlignment = RoleAlignment.CrewSov;
            AlignmentName = CSv;
            InspectorResults = InspectorResults.NewLens;
            Type = LayerEnum.Monarch;
            Knighted = new();
            ToBeKnighted = new();
            UsesLeft = CustomGameOptions.KnightCount;
            KnightButton = new(this, "Knight", AbilityTypes.Direct, "ActionSecondary", Knight, true);
        }

        public float KnightTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastKnighted;
            var num = Player.GetModifiedCooldown(CustomGameOptions.KnightingCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Knight()
        {
            if (Utils.IsTooFar(Player, KnightButton.TargetPlayer) || KnightTimer() != 0f || !ButtonUsable || RoundOne)
                return;

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.Knight);
            writer.Write(Player.PlayerId);
            writer.Write(KnightButton.TargetPlayer.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            ToBeKnighted.Add(KnightButton.TargetPlayer.PlayerId);
            UsesLeft--;
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            var notKnighted = PlayerControl.AllPlayerControls.ToArray().Where(x => !ToBeKnighted.Contains(x.PlayerId) && !Knighted.Contains(x.PlayerId) && !x.IsKnighted()).ToList();
            KnightButton.Update("KNIGHT", KnightTimer(), CustomGameOptions.KnightingCooldown, UsesLeft, notKnighted, ButtonUsable, !RoundOne && ButtonUsable);
        }
    }
}