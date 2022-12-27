using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.TimeLordMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKillButton
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton)
                return true;

            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.TimeLord);

            if (!flag)
                return true;

            var role = Role.GetRole<TimeLord>(PlayerControl.LocalPlayer);

            if (!PlayerControl.LocalPlayer.CanMove)
                return false;

            if (PlayerControl.LocalPlayer.Data.IsDead)
                return false;

            var flag2 = (role.TimeLordRewindTimer() == 0f) & !RecordRewind.rewinding;

            if (!flag2)
                return false;

            if (!__instance.enabled)
                return false;

            if (!role.ButtonUsable)
                return false;

            role.UsesLeft--;
            StartStop.StartRewind(role);

            unchecked
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.Rewind,
                    SendOption.Reliable, -1);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
            
            try
            {
                SoundManager.Instance.PlaySound(TownOfUsReworked.RewindSound, false, 1f);
            } catch {}
            
            return false;
        }
    }
}