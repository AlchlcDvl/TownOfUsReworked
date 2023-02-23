using System;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.ArsonistMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformAbility
    {
        public static bool Prefix(KillButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Arsonist))
                return false;

            var role = Role.GetRole<Arsonist>(PlayerControl.LocalPlayer);

            if (!Utils.ButtonUsable(__instance))
                return false;

            if (__instance == role.IgniteButton && role.DousedAlive > 0)
            {
                if (Utils.IsTooFar(role.Player, role.ClosestPlayerIgnite))
                    return false;
                
                if (role.IgniteTimer() != 0f)
                    return false;

                if (!role.DousedPlayers.Contains(role.ClosestPlayerIgnite.PlayerId))
                    return false;
                
                var interact = Utils.Interact(role.Player, role.ClosestPlayerIgnite, Role.GetRoleValue(RoleEnum.Pestilence), true);

                if (interact[3] == true)
                {
                    role.LastIgnited = DateTime.UtcNow;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.Ignite);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    role.Ignite();
                    
                    try
                    {
                        //SoundManager.Instance.PlaySound(TownOfUsReworked.IgniteSound, false, 1f);
                    } catch {}

                    if (CustomGameOptions.ArsoCooldownsLinked)
                        role.LastDoused = DateTime.UtcNow;
                }

                if (interact[0])
                {
                    role.LastIgnited = DateTime.UtcNow;

                    if (CustomGameOptions.ArsoCooldownsLinked)
                        role.LastDoused = DateTime.UtcNow;
                }
                else if (interact[1] == true)
                {
                    role.LastIgnited.AddSeconds(CustomGameOptions.ProtectKCReset);

                    if (CustomGameOptions.ArsoCooldownsLinked)
                        role.LastDoused.AddSeconds(CustomGameOptions.ProtectKCReset);
                }
                else if (interact[2] == true)
                {
                    role.LastIgnited.AddSeconds(CustomGameOptions.VestKCReset);

                    if (CustomGameOptions.ArsoCooldownsLinked)
                        role.LastDoused.AddSeconds(CustomGameOptions.VestKCReset);
                }

                return false;
            }
            else if (__instance == role.DouseButton)
            {
                if (Utils.IsTooFar(role.Player, role.ClosestPlayerDouse))
                    return false;
                
                if (role.DouseTimer() != 0f)
                    return false;

                if (role.DousedPlayers.Contains(role.ClosestPlayerDouse.PlayerId))
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayerIgnite, Role.GetRoleValue(RoleEnum.Pestilence));

                if (interact[3] == true)
                {
                    var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer2.Write((byte)ActionsRPC.Douse);
                    writer2.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer2.Write(role.ClosestPlayerDouse.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer2);
                    role.DousedPlayers.Add(role.ClosestPlayerDouse.PlayerId);
                }

                if (interact[0])
                {
                    role.LastDoused = DateTime.UtcNow;

                    if (CustomGameOptions.ArsoCooldownsLinked)
                        role.LastIgnited = DateTime.UtcNow;
                }
                else if (interact[1] == true)
                {
                    role.LastDoused.AddSeconds(CustomGameOptions.ProtectKCReset);

                    if (CustomGameOptions.ArsoCooldownsLinked)
                        role.LastIgnited.AddSeconds(CustomGameOptions.ProtectKCReset);
                }

                return false;
            }

            return false;
        }
    }
}
