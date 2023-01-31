using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using Hazel;
using TownOfUsReworked.Lobby.CustomOption;
using AmongUs.GameOptions;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.RebelMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformAbility
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton)
                return true;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Rebel))
                return true;

            var role = Role.GetRole<Rebel>(PlayerControl.LocalPlayer);

            if (!PlayerControl.LocalPlayer.CanMove || role.ClosestPlayer == null)
                return false;

            if (!__instance.enabled)
                return false;

            var maxDistance = GameOptionsData.KillDistances[CustomGameOptions.InteractionDistance];

            if (Vector2.Distance(role.ClosestPlayer.GetTruePosition(), PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance)
                return false;

            if (role.ClosestPlayer == null)
                return false;

            if (role.ClosestPlayer.IsInfected())
            {
                foreach (var pb in Role.GetRoles(RoleEnum.Plaguebearer))
                    ((Plaguebearer)pb).RpcSpreadInfection(role.ClosestPlayer, role.Player);
            }

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
            writer.Write((byte)ActionsRPC.Sidekick);
            writer.Write(role.Player.PlayerId);
            writer.Write(role.ClosestPlayer.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            Sidekick(role, role.ClosestPlayer);
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
