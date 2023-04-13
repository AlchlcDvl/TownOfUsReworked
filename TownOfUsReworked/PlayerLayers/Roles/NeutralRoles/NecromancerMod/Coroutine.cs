using System;
using System.Collections;
using Reactor.Utilities.Extensions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Patches;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Objectifiers;
using AmongUs.GameOptions;
using TownOfUsReworked.Data;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.NecromancerMod
{
    public static class Coroutine
    {
        public static IEnumerator NecromancerResurrect(DeadBody target, Necromancer role)
        {
            var parentId = target.ParentId;
            var player = Utils.PlayerById(parentId);
            var position = target.TruePosition;

            if (!Utils.PlayerById(parentId).Is(SubFaction.None))
            {
                Utils.Flash(Color.red, $"{player.Data.PlayerName} cannot be resurrected!");
                yield break;
            }

            if (PlayerControl.LocalPlayer.PlayerId == parentId)
                Utils.Flash(Colors.Reanimated, "You are being ressurected!");

            Utils.Spread(role.Player, player);
            role.Resurrecting = true;

            if (CustomGameOptions.NecromancerTargetBody)
                target?.gameObject.Destroy();

            var startTime = DateTime.UtcNow;

            while (true)
            {
                var now = DateTime.UtcNow;
                var seconds = (now - startTime).TotalSeconds;

                if (seconds < CustomGameOptions.NecroResurrectDuration)
                    yield return null;
                else
                    break;

                if (MeetingHud.Instance)
                {
                    role.Resurrecting = false;
                    yield break;
                }
            }

            var targetRole = Role.GetRole(player);
            targetRole.DeathReason = DeathReasonEnum.Revived;
            targetRole.KilledBy = " By " + role.PlayerName;
            player.Revive();
            RoleGen.Convert(parentId, role.Player.PlayerId, SubFaction.Reanimated, false);

            if (player.Data.IsImpostor())
                RoleManager.Instance.SetRole(player, RoleTypes.Impostor);
            else
                RoleManager.Instance.SetRole(player, RoleTypes.Crewmate);

            Murder.KilledPlayers.Remove(Murder.KilledPlayers.Find(x => x.PlayerId == player.PlayerId));
            player.NetTransform.SnapTo(new Vector2(position.x, position.y + 0.3636f));

            if (SubmergedCompatibility.IsSubmerged() && PlayerControl.LocalPlayer == player)
                SubmergedCompatibility.ChangeFloor(player.transform.position.y > -7);

            target?.gameObject.Destroy();

            if (player.Is(ObjectifierEnum.Lovers) && CustomGameOptions.BothLoversDie)
            {
                var lover = Objectifier.GetObjectifier<Lovers>(player).OtherLover;
                lover.Revive();
                Murder.KilledPlayers.Remove(Murder.KilledPlayers.Find(x => x.PlayerId == lover.PlayerId));
                Utils.BodyById(lover.PlayerId).gameObject.Destroy();
                var loverRole = Role.GetRole(lover);
                loverRole.DeathReason = DeathReasonEnum.Revived;
                loverRole.KilledBy = " By " + role.PlayerName;
                RoleGen.Convert(lover.PlayerId, role.Player.PlayerId, SubFaction.Reanimated, false);
            }

            if (player == PlayerControl.LocalPlayer)
            {
                if (MapBehaviour.Instance)
                    MapBehaviour.Instance.Close();

                if (Minigame.Instance)
                    Minigame.Instance.Close();
            }
        }
    }
}