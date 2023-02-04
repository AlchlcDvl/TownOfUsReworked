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
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Disguiser))
                return false;

            var role = Role.GetRole<Disguiser>(PlayerControl.LocalPlayer);

            if (__instance == role.DisguiseButton)
            {
                if (!__instance.isActiveAndEnabled)
                    return false;
                
                if (role.DisguiseTimer() > 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;
                
                var interact = Utils.Interact(role.Player, role.ClosestPlayer, Role.GetRoleValue(RoleEnum.Pestilence));
                
                if (interact[3] == true)
                {
                    if (role.DisguiseButton.graphic.sprite == MeasureSprite)
                    {
                        role.MeasuredPlayer = role.ClosestPlayer;
                        role.DisguiseButton.graphic.sprite = DisguiseSprite;

                        if (role.DisguiseTimer() < 5f)
                            role.LastDisguised = DateTime.UtcNow.AddSeconds(5 - CustomGameOptions.DisguiseCooldown);
                            
                        try
                        {
                            //SoundManager.Instance.PlaySound(TownOfUsReworked.SampleSound, false, 1f);
                        } catch {}

                        return false;
                    }
                    else if (role.DisguiseButton.graphic.sprite == DisguiseSprite)
                    {
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                        writer.Write((byte)ActionsRPC.Disguise);
                        writer.Write(PlayerControl.LocalPlayer.PlayerId);
                        writer.Write(role.MeasuredPlayer.PlayerId);
                        writer.Write(role.ClosestPlayer.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        role.TimeRemaining = CustomGameOptions.DisguiseDuration;
                        role.Disguise();
                        
                        try
                        {
                            //SoundManager.Instance.PlaySound(TownOfUsReworked.MorphSound, false, 1f);
                        } catch {}

                        return false;
                    }
                }
                else if (interact[1] == true)
                    role.LastDisguised.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }
            else if (__instance == role.KillButton)
            {
                if (!__instance.isActiveAndEnabled)
                    return false;

                if (role.KillTimer() > 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer, Role.GetRoleValue(RoleEnum.Pestilence), true);

                if (interact[3] == true && interact[0] == true)
                    role.LastKilled = DateTime.UtcNow;
                else if (interact[0] == true)
                    role.LastKilled = DateTime.UtcNow;
                else if (interact[1] == true)
                    role.LastKilled.AddSeconds(CustomGameOptions.ProtectKCReset);
                else if (interact[2] == true)
                    role.LastKilled.AddSeconds(CustomGameOptions.VestKCReset);

                return false;
            }

            return false;
        }
    }
}