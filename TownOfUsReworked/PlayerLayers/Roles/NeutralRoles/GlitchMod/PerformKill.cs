using System;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Patches;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.GlitchMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static Sprite HackSprite => TownOfUsReworked.HackSprite;
        public static Sprite MimicSprite => TownOfUsReworked.MimicSprite;

        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Glitch);

            if (!flag)
                return true;

            if (!PlayerControl.LocalPlayer.CanMove)
                return false;

            if (PlayerControl.LocalPlayer.Data.IsDead)
                return false;

            var role = Role.GetRole<Glitch>(PlayerControl.LocalPlayer);
            var target = role.ClosestPlayer;

            if (__instance == role.GlitchButton)
            {
                if (!__instance.isActiveAndEnabled)
                    return false;

                if (role.GlitchButton.graphic.sprite == HackSprite)
                {
                    if (target == null)
                        return false;

                    role.HackTarget = target;
                    role.GlitchButton.graphic.sprite = HackSprite;
                    role.GlitchButton.SetTarget(null);
                    DestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(null);

                    if (role.HackTimer() < 5f)
                        role.LastMimic = DateTime.UtcNow.AddSeconds(5 - CustomGameOptions.MimicCooldown);

                    try
                    {
                        AudioClip SampleSFX = TownOfUsReworked.loadAudioClipFromResources("TownOfUsReworked.Resources.Sample.raw");
                        SoundManager.Instance.PlaySound(SampleSFX, false, 0.4f);
                    } catch {}
                }
                else
                {
                    if (__instance.isCoolingDown)
                        return false;
                        
                    if (role.MimicTimer() != 0)
                        return false;

                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.Morph,
                        SendOption.Reliable, -1);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(role.MimicTarget.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    role.TimeRemaining = CustomGameOptions.MorphlingDuration;
                    Utils.Morph(role.Player, role.MimicTarget);

                    try
                    {
                        AudioClip MorphSFX = TownOfUsReworked.loadAudioClipFromResources("TownOfUsReworked.Resources.Morph.raw");
                        SoundManager.Instance.PlaySound(MorphSFX, false, 0.4f);
                    } catch {}
                }

                return false;
            }

            return true;
        }
    }
}
