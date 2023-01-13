using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.Patches;

using Hazel;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.RebelMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton)
                return true;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Rebel))
                return true;

            var role = Role.GetRole<Rebel>(PlayerControl.LocalPlayer);

            if (!PlayerControl.LocalPlayer.CanMove || role.ClosestSyndicate == null)
                return false;

            if (!__instance.enabled)
                return false;

            var maxDistance = GameOptionsData.KillDistances[CustomGameOptions.InteractionDistance];

            if (Vector2.Distance(role.ClosestSyndicate.GetTruePosition(), PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance)
                return false;

            if (role.ClosestSyndicate == null)
                return false;

            if (role.ClosestSyndicate.IsInfected())
            {
                foreach (var pb in Role.GetRoles(RoleEnum.Plaguebearer))
                    ((Plaguebearer)pb).RpcSpreadInfection(role.ClosestSyndicate, role.Player);
            }

            unchecked
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Sidekick,
                    SendOption.Reliable, -1);
                writer.Write(role.Player.PlayerId);
                writer.Write(role.ClosestSyndicate.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }

            Sidekick(role, role.ClosestSyndicate);
            return false;
        }

        public static void Sidekick(Rebel reb, PlayerControl target)
        {
            reb.HasDeclared = true;
            var formerRole = Role.GetRole(target);
            var sidekick = new Sidekick(target);
            sidekick.FormerRole = formerRole;
            sidekick.RoleHistory.Add(formerRole);
            sidekick.RoleHistory.AddRange(formerRole.RoleHistory);
        }
    }
}
