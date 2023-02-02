using System;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.DisguiserMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformAbility
    {
        public static Sprite MeasureSprite => TownOfUsReworked.MeasureSprite;
        public static Sprite DisguiseSprite => TownOfUsReworked.DisguiseSprite;

        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Disguiser);

            if (!flag)
                return true;

            if (!PlayerControl.LocalPlayer.CanMove)
                return false;

            if (PlayerControl.LocalPlayer.Data.IsDead)
                return false;

            var role = Role.GetRole<Disguiser>(PlayerControl.LocalPlayer);
            var target = role.ClosestPlayer;

            if (__instance == role.DisguiseButton)
            {
                if (!__instance.isActiveAndEnabled)
                    return false;

                if (role.DisguiseButton.graphic.sprite == MeasureSprite)
                {
                    if (target == null)
                        return false;

                    role.MeasuredPlayer = target;
                    role.DisguiseButton.graphic.sprite = DisguiseSprite;
                    role.DisguiseButton.SetTarget(null);
                    DestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(null);

                    if (role.DisguiseTimer() < 5f)
                        role.LastDisguised = DateTime.UtcNow.AddSeconds(5 - CustomGameOptions.DisguiseCooldown);
                        
                    try
                    {
                        //SoundManager.Instance.PlaySound(TownOfUsReworked.SampleSound, false, 1f);
                    } catch {}
                }
                else
                {
                    if (__instance.isCoolingDown)
                        return false;

                    if (role.DisguiseTimer() != 0)
                        return false;

                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                    writer.Write((byte)ActionsRPC.Disguise);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(role.MeasuredPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    role.TimeRemaining = CustomGameOptions.DisguiseDuration;
                    role.DisguisedPlayer = role.MeasuredPlayer;
                    Utils.Morph(role.Player, role.MeasuredPlayer);
                    
                    try
                    {
                        //SoundManager.Instance.PlaySound(TownOfUsReworked.MorphSound, false, 1f);
                    } catch {}
                }

                return false;
            }

            return false;
        }
    }
}
