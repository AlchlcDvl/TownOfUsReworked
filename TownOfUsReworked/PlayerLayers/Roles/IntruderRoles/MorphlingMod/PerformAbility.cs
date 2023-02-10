using System;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Classes;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.MorphlingMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformAbility
    {
        public static Sprite SampleSprite => TownOfUsReworked.SampleSprite;
        public static Sprite MorphSprite => TownOfUsReworked.MorphSprite;

        public static bool Prefix(KillButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Morphling))
                return false;

            var role = Role.GetRole<Morphling>(PlayerControl.LocalPlayer);

            if (__instance == role.MorphButton)
            {
                if (!__instance.isActiveAndEnabled)
                    return false;

                if (role.MorphButton.graphic.sprite == SampleSprite)
                {
                    if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                        return false;
                
                    var interact = Utils.Interact(role.Player, role.ClosestPlayer, Role.GetRoleValue(RoleEnum.Pestilence));

                    if (interact[3] == true)
                    {
                        role.SampledPlayer = role.ClosestPlayer;
                        role.MorphButton.graphic.sprite = MorphSprite;
                        role.MorphButton.SetTarget(null);

                        if (role.MorphTimer() < 5f)
                            role.LastMorphed = DateTime.UtcNow.AddSeconds(5 - CustomGameOptions.MorphlingCd);
                    }
                    else if (interact[1] == true)
                        role.LastMorphed.AddSeconds(CustomGameOptions.ProtectKCReset);
                        
                    try
                    {
                        //SoundManager.Instance.PlaySound(TownOfUsReworked.SampleSound, false, 1f);
                    } catch {}
                }
                else
                {
                    if (role.MorphTimer() != 0f)
                        return false;

                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                    writer.Write((byte)ActionsRPC.Morph);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(role.SampledPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    role.TimeRemaining = CustomGameOptions.MorphlingDuration;
                    role.MorphedPlayer = role.SampledPlayer;
                    role.Morph();
                    
                    try
                    {
                        //SoundManager.Instance.PlaySound(TownOfUsReworked.MorphSound, false, 1f);
                    } catch {}
                }

                return false;
            }
            else if (__instance == role.KillButton)
            {
                if (role.KillTimer() != 0f)
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
