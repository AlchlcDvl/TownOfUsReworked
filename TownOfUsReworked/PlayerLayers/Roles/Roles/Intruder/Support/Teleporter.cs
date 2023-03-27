using Reactor.Utilities;
using System;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Teleporter : IntruderRole
    {
        public AbilityButton TeleportButton;
        public DateTime LastTeleport;
        public DateTime LastMarked;
        public AbilityButton MarkButton;
        public bool CanMark;
        public Vector3 TeleportPoint = new(0, 0, 0);

        public Teleporter(PlayerControl player) : base(player)
        {
            Name = "Teleporter";
            StartText = "Get Away From Kills With Ease";
            AbilitiesText = "Teleport to get away from bodies";
            Color = CustomGameOptions.CustomIntColors ? Colors.Teleporter : Colors.Intruder;
            RoleType = RoleEnum.Teleporter;
        }

        public float TeleportTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastTeleport;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.TeleportCd, CustomButtons.GetUnderdogChange(Player)) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public float MarkTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastMarked;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.MarkCooldown, CustomButtons.GetUnderdogChange(Player)) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public static void Teleport(PlayerControl teleporter)
        {
            teleporter.MyPhysics.ResetMoveState();
            var teleporterRole = GetRole<Teleporter>(teleporter);
            var position = teleporterRole.TeleportPoint;
            teleporter.NetTransform.SnapTo(new Vector2(position.x, position.y));

            if (SubmergedCompatibility.IsSubmerged() && PlayerControl.LocalPlayer.PlayerId == teleporter.PlayerId)
            {
                SubmergedCompatibility.ChangeFloor(teleporter.GetTruePosition().y > -7);
                SubmergedCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
            }

            if (PlayerControl.LocalPlayer.PlayerId == teleporter.PlayerId)
            {
                Utils.Flash(Colors.Teleporter, "You have teleported to a different location!");

                if (Minigame.Instance)
                    Minigame.Instance.Close();
            }

            teleporter.moveable = true;
            teleporter.Collider.enabled = true;
            teleporter.NetTransform.enabled = true;
        }
    }
}