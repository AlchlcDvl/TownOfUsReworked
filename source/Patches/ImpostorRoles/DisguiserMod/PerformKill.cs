using System;
using HarmonyLib;
using Hazel;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.DisguiserMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static Sprite MeasureSprite => TownOfUs.MeasureSprite;
        public static Sprite DisguiseSprite => TownOfUs.DisguiseSprite;

        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Disguiser);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Disguiser>(PlayerControl.LocalPlayer);
            var target = role.ClosestPlayer;
            if (__instance == role.DisguiseButton)
            {
                if (!__instance.isActiveAndEnabled) return false;
                if (role.DisguiseButton.graphic.sprite == MeasureSprite)
                {
                    if (target == null) return false;
                    role.MeasuredPlayer = target;
                    role.DisguiseButton.graphic.sprite = DisguiseSprite;
                    role.DisguiseButton.SetTarget(null);
                    DestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(null);
                    if (role.DisguiseTimer() < 5f)
                        role.LastDisguised = DateTime.UtcNow.AddSeconds(5 - CustomGameOptions.DisguiseCooldown);
                    try {
                        AudioClip SampleSFX = TownOfUs.loadAudioClipFromResources("TownOfUs.Resources.Sample.raw");
                        SoundManager.Instance.PlaySound(SampleSFX, false, 0.4f);
                    } catch {
                    }
                }
                else
                {
                    if (__instance.isCoolingDown) return false;
                    if (role.DisguiseTimer() != 0) return false;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.Disguise, SendOption.Reliable, -1);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(role.MeasuredPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    role.TimeRemaining = CustomGameOptions.DisguiseDuration;
                    role.DisguisedPlayer = role.MeasuredPlayer;
                    Utils.Morph(role.Player, role.MeasuredPlayer, true);
                    try {
                        AudioClip MorphSFX = TownOfUs.loadAudioClipFromResources("TownOfUs.Resources.Morph.raw");
                        SoundManager.Instance.PlaySound(MorphSFX, false, 0.4f);
                    } catch {
                    }
                }

                return false;
            }

            return true;
        }
    }
}
