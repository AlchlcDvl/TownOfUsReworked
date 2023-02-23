﻿using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using System;
using TownOfUsReworked.CustomOptions;
using Hazel;
using TownOfUsReworked.Objects;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.BomberMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformAbility
    {
        public static bool Prefix(KillButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Bomber))
                return false;

            if (!Utils.ButtonUsable(__instance))
                return false;

            var role = Role.GetRole<Bomber>(PlayerControl.LocalPlayer);

            if (__instance == role.BombButton)
            {
                if (role.BombTimer() != 0f)
                    return false;

                role.Bombs.Add(BombExtentions.CreateBomb(PlayerControl.LocalPlayer.GetTruePosition()));
                role.LastPlaced = DateTime.UtcNow;

                if (CustomGameOptions.BombCooldownsLinked)
                {
                    role.LastPlaced = DateTime.UtcNow;
                    role.LastKilled = DateTime.UtcNow;
                }

                return false;
            }
            else if (__instance == role.DetonateButton)
            {
                if (role.DetonateTimer() != 0f)
                    return false;

                if (role.Bombs.Count <= 0)
                    return false;
                
                role.Bombs.DetonateBombs(role.PlayerName);
                role.LastDetonated = DateTime.UtcNow;

                if (CustomGameOptions.BombCooldownsLinked)
                {
                    role.LastPlaced = DateTime.UtcNow;
                    role.LastKilled = DateTime.UtcNow;
                }

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.Detonate);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                return false;
            }
            else if (__instance == role.KillButton)
            {
                if (role.KillTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer, Role.GetRoleValue(RoleEnum.Pestilence));

                if (interact[3] == true && interact[0] == true)
                {
                    role.LastKilled = DateTime.UtcNow;

                    if (CustomGameOptions.BombCooldownsLinked)
                    {
                        role.LastPlaced = DateTime.UtcNow;
                        role.LastDetonated = DateTime.UtcNow;
                    }
                }
                else if (interact[0] == true)
                {
                    role.LastKilled = DateTime.UtcNow;

                    if (CustomGameOptions.BombCooldownsLinked)
                    {
                        role.LastPlaced = DateTime.UtcNow;
                        role.LastDetonated = DateTime.UtcNow;
                    }
                }
                else if (interact[1] == true)
                {
                    role.LastKilled.AddSeconds(CustomGameOptions.ProtectKCReset);

                    if (CustomGameOptions.BombCooldownsLinked)
                    {
                        role.LastPlaced.AddSeconds(CustomGameOptions.ProtectKCReset);
                        role.LastDetonated.AddSeconds(CustomGameOptions.ProtectKCReset);
                    }
                }
                else if (interact[2] == true)
                {
                    role.LastKilled.AddSeconds(CustomGameOptions.VestKCReset);

                    if (CustomGameOptions.BombCooldownsLinked)
                    {
                        role.LastPlaced.AddSeconds(CustomGameOptions.VestKCReset);
                        role.LastDetonated.AddSeconds(CustomGameOptions.VestKCReset);
                    }
                }
                
                return false;
            }

            return false;
        }
    }
}
