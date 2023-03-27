using System;
using System.Collections;
using Reactor.Utilities.Extensions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Patches;
using UnityEngine;
using Object = UnityEngine.Object;
using TownOfUsReworked.PlayerLayers.Objectifiers;
using AmongUs.GameOptions;
using Reactor.Utilities;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;

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

            role.Resurrecting = true;

            if (CustomGameOptions.NecromancerTargetBody)
            {
                foreach (var deadBody in Object.FindObjectsOfType<DeadBody>())
                {
                    if (deadBody.ParentId == parentId)
                        deadBody.gameObject.Destroy();
                }
            }

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

            foreach (var deadBody in Object.FindObjectsOfType<DeadBody>())
            {
                if (deadBody.ParentId == role.Player.PlayerId)
                    deadBody.gameObject.Destroy();
            }

            var targetRole = Role.GetRole(player);
            targetRole.DeathReason = DeathReasonEnum.Revived;
            targetRole.KilledBy = " By " + role.PlayerName;
            targetRole.SubFaction = SubFaction.Reanimated;
            targetRole.IsResurrected = true;
            role.Resurrected.Add(parentId);

            foreach (var poisoner in Role.GetRoles(RoleEnum.Poisoner))
            {
                var poisonerRole = (Poisoner)poisoner;

                if (poisonerRole.PoisonedPlayer == player)
                    poisonerRole.PoisonedPlayer = poisonerRole.Player;
            }

            player.Revive();

            if (player.Data.IsImpostor())
                RoleManager.Instance.SetRole(player, RoleTypes.Impostor);
            else
                RoleManager.Instance.SetRole(player, RoleTypes.Crewmate);

            Murder.KilledPlayers.Remove(Murder.KilledPlayers.Find(x => x.PlayerId == player.PlayerId));
            player.NetTransform.SnapTo(new Vector2(position.x, position.y + 0.3636f));

            if (PlayerControl.LocalPlayer == player)
            {
                try
                {
                    //SoundManager.Instance.PlaySound(TownOfUsReworked.ReviveSound, false, 1f);
                } catch {}
            }

            if (SubmergedCompatibility.IsSubmerged() && PlayerControl.LocalPlayer.PlayerId == player.PlayerId)
                SubmergedCompatibility.ChangeFloor(player.transform.position.y > -7);

            if (target != null)
                Object.Destroy(target.gameObject);

            if (player.Is(ObjectifierEnum.Lovers) && CustomGameOptions.BothLoversDie)
            {
                var lover = Objectifier.GetObjectifier<Lovers>(player).OtherLover;

                lover.Revive();
                Murder.KilledPlayers.Remove(Murder.KilledPlayers.Find(x => x.PlayerId == lover.PlayerId));

                foreach (DeadBody deadBody in Object.FindObjectsOfType<DeadBody>())
                {
                    if (deadBody.ParentId == lover.PlayerId)
                        deadBody.gameObject.Destroy();
                }

                var loverRole = Role.GetRole(lover);
                loverRole.DeathReason = DeathReasonEnum.Revived;
                loverRole.KilledBy = " By " + role.PlayerName;
                loverRole.SubFaction = SubFaction.Reanimated;
                loverRole.IsResurrected = true;
                role.Resurrected.Add(lover.PlayerId);
            }

            if (Minigame.Instance)
                Minigame.Instance.Close();

            Utils.Spread(role.Player, player);

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Mystic))
                Utils.Flash(Colors.Mystic, "Someone has changed their allegience!");
        }
    }
}