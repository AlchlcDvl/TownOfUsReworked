using HarmonyLib;
using Hazel;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Data;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Functions;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.TimeLordMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformRewind
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.TimeLord))
                return true;

            var role = Role.GetRole<TimeLord>(PlayerControl.LocalPlayer);

            if (__instance == role.RewindButton)
            {
                if (role.TimeLordRewindTimer() != 0f && !RecordRewind.rewinding)
                    return false;

                role.UsesLeft--;
                StartStop.StartRewind(role);
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.Rewind);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                return false;
            }

            return true;
        }
    }
}