using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.Patches;
using Hazel;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.GodfatherMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton)
                return true;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Godfather))
                return true;

            var role = Role.GetRole<Godfather>(PlayerControl.LocalPlayer);

            if (!PlayerControl.LocalPlayer.CanMove | role.ClosestIntruder == null)
                return false;

            if (!__instance.enabled)
                return false;

            var maxDistance = GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance];

            if (Vector2.Distance(role.ClosestIntruder.GetTruePosition(), PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance)
                return false;

            if (role.ClosestIntruder == null)
                return false;

            if (role.ClosestIntruder.IsInfected() | role.Player.IsInfected())
            {
                foreach (var pb in Role.GetRoles(RoleEnum.Plaguebearer))
                    ((Plaguebearer)pb).RpcSpreadInfection(role.ClosestIntruder, role.Player);
            }

            unchecked
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Declare,
                    SendOption.Reliable, -1);
                writer.Write(role.Player.PlayerId);
                writer.Write(role.ClosestIntruder.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }

            Declare(role, role.ClosestIntruder);
            return false;
        }

        public static void Declare(Godfather gf, PlayerControl target)
        {
            gf.HasDeclared = true;
            var formerRole = Role.GetRole(target);
            var mafioso = new Mafioso(target);
            mafioso.FormerRole = formerRole;
            
            Role.RoleDictionary.Remove(target.PlayerId);
            Role.RoleDictionary.Add(target.PlayerId, mafioso);
        }
    }
}
