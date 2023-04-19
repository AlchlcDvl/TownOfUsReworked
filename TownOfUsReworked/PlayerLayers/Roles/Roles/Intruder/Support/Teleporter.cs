using TownOfUsReworked.Data;
using System;
using UnityEngine;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Custom;
using Hazel;
using TownOfUsReworked.Classes;
using Reactor.Networking.Extensions;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Teleporter : IntruderRole
    {
        public CustomButton TeleportButton;
        public DateTime LastTeleport;
        public DateTime LastMarked;
        public CustomButton MarkButton;
        public bool CanMark;
        public Vector3 TeleportPoint = new(0, 0, 0);

        public Teleporter(PlayerControl player) : base(player)
        {
            Name = "Teleporter";
            StartText = "X Marks The Spot";
            AbilitiesText = $"- You can mark a spot to teleport to later\n{AbilitiesText}";
            Color = CustomGameOptions.CustomIntColors ? Colors.Teleporter : Colors.Intruder;
            RoleType = RoleEnum.Teleporter;
            AlignmentName = IS;
            Type = LayerEnum.Teleporter;
            TeleportPoint = new(0, 0, 0);
            MarkButton = new(this, AssetManager.Mark, AbilityTypes.Effect, "Secondary", Mark);
            TeleportButton = new(this, AssetManager.Teleport, AbilityTypes.Effect, "Secondary", Teleport);
        }

        public float TeleportTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastTeleport;
            var num = Player.GetModifiedCooldown(CustomGameOptions.TeleportCd) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public float MarkTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastMarked;
            var num = Player.GetModifiedCooldown(CustomGameOptions.MarkCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Mark()
        {
            if (!CanMark || MarkTimer() != 0f || TeleportPoint == Player.transform.position)
                return;

            TeleportPoint = Player.transform.position;
            LastMarked = DateTime.UtcNow;

            if (CustomGameOptions.TeleCooldownsLinked)
                LastTeleport = DateTime.UtcNow;
        }

        public void Teleport()
        {
            if (TeleportTimer() != 0f || TeleportPoint == Player.transform.position)
                return;

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.Teleport);
            writer.Write(Player.PlayerId);
            writer.Write(TeleportPoint);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            LastTeleport = DateTime.UtcNow;
            Utils.Teleport(Player, TeleportPoint);

            if (CustomGameOptions.TeleCooldownsLinked)
                LastMarked = DateTime.UtcNow;
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            var hits = Physics2D.OverlapBoxAll(Player.transform.position, Utils.GetSize(), 0);
            hits = hits.ToArray().Where(c => (c.name.Contains("Vent") || !c.isTrigger) && c.gameObject.layer != 8 && c.gameObject.layer != 5).ToArray();
            CanMark = hits.Count == 0 && Player.moveable && !SubmergedCompatibility.GetPlayerElevator(Player).Item1 && TeleportPoint != Player.transform.position;
            MarkButton.Update("MARK", MarkTimer(), CustomGameOptions.MarkCooldown, CanMark);
            TeleportButton.Update("TELEPORT", TeleportTimer(), CustomGameOptions.TeleportCd, true, TeleportPoint != new Vector3(0, 0, 0));
        }
    }
}