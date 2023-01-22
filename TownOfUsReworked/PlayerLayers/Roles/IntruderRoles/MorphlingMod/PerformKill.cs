using System;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.MorphlingMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static Sprite SampleSprite => TownOfUsReworked.SampleSprite;
        public static Sprite MorphSprite => TownOfUsReworked.MorphSprite;

        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Morphling);

            if (!flag)
                return true;

            if (!PlayerControl.LocalPlayer.CanMove)
                return false;

            if (PlayerControl.LocalPlayer.Data.IsDead)
                return false;

            var role = Role.GetRole<Morphling>(PlayerControl.LocalPlayer);
            var target = role.ClosestPlayer;

            if (__instance == role.MorphButton)
            {
                if (!__instance.isActiveAndEnabled)
                    return false;

                if (role.MorphButton.graphic.sprite == SampleSprite)
                {
                    if (target == null)
                        return false;

                    role.SampledPlayer = target;
                    role.MorphButton.graphic.sprite = MorphSprite;
                    role.MorphButton.SetTarget(null);
                    DestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(null);

                    if (role.MorphTimer() < 5f)
                        role.LastMorphed = DateTime.UtcNow.AddSeconds(5 - CustomGameOptions.MorphlingCd);
                        
                    try
                    {
                        SoundManager.Instance.PlaySound(TownOfUsReworked.SampleSound, false, 1f);
                    } catch {}
                }
                else
                {
                    if (__instance.isCoolingDown)
                        return false;
                        
                    if (role.MorphTimer() != 0)
                        return false;

                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                    writer.Write((byte)ActionsRPC.Morph);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(role.SampledPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    role.TimeRemaining = CustomGameOptions.MorphlingDuration;
                    role.MorphedPlayer = role.SampledPlayer;
                    Utils.Morph(role.Player, role.SampledPlayer);
                    
                    try
                    {
                        SoundManager.Instance.PlaySound(TownOfUsReworked.MorphSound, false, 1f);
                    } catch {}
                }

                return false;
            }

            return false;
        }
    }
}
