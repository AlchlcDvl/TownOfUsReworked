using HarmonyLib;
using Hazel;
using TownOfUs.Roles;

namespace TownOfUs.ImpostorRoles.TimeMasterMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.TimeMaster);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<TimeMaster>(PlayerControl.LocalPlayer);
            var target = DestroyableSingleton<HudManager>.Instance.KillButton.currentTarget;
            if (__instance == role.FreezeButton)
            {
                if (__instance.isCoolingDown) return false;
                if (!__instance.isActiveAndEnabled) return false;
                if (role.FreezeTimer() != 0) return false;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.Freeze, SendOption.Reliable, -1);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                role.TimeRemaining = CustomGameOptions.FreezeDuration;
                Utils.TimeFreeze();

                return false;
            }

            return true;
        }
    }
}