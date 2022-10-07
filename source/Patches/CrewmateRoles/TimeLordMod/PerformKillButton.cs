using HarmonyLib;
using Hazel;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.TimeLordMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKillButton

    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.TimeLord);
            if (!flag) return true;
            var role = Role.GetRole<TimeLord>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var flag2 = (role.TimeLordRewindTimer() == 0f) & !RecordRewind.rewinding;
            if (!flag2) return false;
            if (!__instance.enabled) return false;
            if (!role.ButtonUsable) return false;

            role.UsesLeft--;

            StartStop.StartRewind(role);
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte) CustomRPC.Rewind, SendOption.Reliable, -1);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            try {
                AudioClip RewindSFX = TownOfUs.loadAudioClipFromResources("TownOfUs.Resources.Rewind.raw");
                SoundManager.Instance.PlaySound(RewindSFX, false, 0.4f);
            } catch {
            }
            return false;
        }
    }
}