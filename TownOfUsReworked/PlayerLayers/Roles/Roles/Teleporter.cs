using Reactor.Utilities;
using System;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Patches;
using Il2CppSystem.Collections.Generic;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Teleporter : Role
    {
        public KillButton _teleportButton;
        public DateTime LastTeleport;
        public Vector3 TeleportPoint;

        public Teleporter(PlayerControl player) : base(player)
        {
            Name = "Teleport";
            ImpostorText = () => "Get Away From Kills With Ease";
            TaskText = () => "Teleport to get away from bodies";
            Color = CustomGameOptions.CustomImpColors ? Colors.Teleporter : Colors.Intruder;
            SubFaction = SubFaction.None;
            RoleType = RoleEnum.Teleporter;
            Faction = Faction.Intruders;
            Results = InspResults.TransWarpTeleTask;
            AddToRoleHistory(RoleType);
        }

        public KillButton TeleportButton
        {
            get => _teleportButton;
            set
            {
                _teleportButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public float TeleportTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastTeleport;
            var num = CustomGameOptions.TeleportCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public static void Teleport(PlayerControl teleporter)
        {
            teleporter.MyPhysics.ResetMoveState();
            var teleporterRole = Role.GetRole<Teleporter>(teleporter);
            var position = teleporterRole.TeleportPoint;
            teleporter.NetTransform.SnapTo(new Vector2(position.x, position.y));

            if (SubmergedCompatibility.isSubmerged())
            {
                if (PlayerControl.LocalPlayer.PlayerId == teleporter.PlayerId)
                {
                    SubmergedCompatibility.ChangeFloor(teleporter.GetTruePosition().y > -7);
                    SubmergedCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
                }
            }

            if (PlayerControl.LocalPlayer.PlayerId == teleporter.PlayerId)
            {
                Coroutines.Start(Utils.FlashCoroutine(new Color(0.6f, 0.1f, 0.2f, 1f)));

                if (Minigame.Instance)
                    Minigame.Instance.Close();
            }

            teleporter.moveable = true;
            teleporter.Collider.enabled = true;
            teleporter.NetTransform.enabled = true;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var intTeam = new List<PlayerControl>();

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Is(Faction.Intruders))
                    intTeam.Add(player);
            }
            __instance.teamToShow = intTeam;
        }
    }
}