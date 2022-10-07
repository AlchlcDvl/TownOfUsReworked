using HarmonyLib;
using Hazel;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.WraithMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Wraith);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Wraith>(PlayerControl.LocalPlayer);
            if (__instance == role.InvisButton)
            {
                if (__instance.isCoolingDown) return false;
                if (!__instance.isActiveAndEnabled) return false;
                if (role.InvisTimer() != 0) return false;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte) CustomRPC.Invis, SendOption.Reliable, -1);
                var position = PlayerControl.LocalPlayer.transform.position;
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                role.TimeRemaining = CustomGameOptions.InvisDuration;
                role.Invis();
                try {
                    AudioClip InvisSFX = TownOfUs.loadAudioClipFromResources("TownOfUs.Resources.Morph.raw");
                    SoundManager.Instance.PlaySound(InvisSFX, false, 0.4f);
                } catch {
                }
                return false;
            }

            return true;
        }
    }
}