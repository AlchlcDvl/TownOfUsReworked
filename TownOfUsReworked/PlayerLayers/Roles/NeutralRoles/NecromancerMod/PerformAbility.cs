using HarmonyLib;
using Hazel;
using Reactor.Utilities;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using System;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.NecromancerMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public class PerformRevive
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Necromancer))
                return true;

            var role = Role.GetRole<Necromancer>(PlayerControl.LocalPlayer);

            if (__instance == role.ResurrectButton)
            {
                if (Utils.IsTooFar(role.Player, role.CurrentTarget))
                    return false;

                if (!Utils.ButtonUsable(__instance))
                    return false;

                var playerId = role.CurrentTarget.ParentId;
                var player = Utils.PlayerById(playerId);
                Utils.Spread(role.Player, player);
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.NecromancerResurrect);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(playerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);        
                Coroutines.Start(Coroutine.NecromancerResurrect(role.CurrentTarget, role));
                role.ResurrectedCount++;
                role.TimeRemaining = CustomGameOptions.NecroResurrectDuration;
                role.Resurrect();

                if (CustomGameOptions.KillResurrectCooldownsLinked)
                    role.LastKilled = DateTime.UtcNow;

                return false;
            }
            else if (__instance == role.KillButton)
            {
                if (!Utils.ButtonUsable(__instance))
                    return false;

                if (role.KillTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer, true);

                if (interact[3] == true)
                    role.KillCount++;
                
                if (interact[0] == true)
                {
                    role.LastKilled = DateTime.UtcNow;

                    if (CustomGameOptions.KillResurrectCooldownsLinked)
                        role.LastResurrected = DateTime.UtcNow;
                }
                else if (interact[1] == true)
                {
                    role.LastKilled.AddSeconds(CustomGameOptions.ProtectKCReset);

                    if (CustomGameOptions.KillResurrectCooldownsLinked)
                        role.LastResurrected.AddSeconds(CustomGameOptions.ProtectKCReset);
                }
                else if (interact[2] == true)
                {
                    role.LastKilled.AddSeconds(CustomGameOptions.VestKCReset);

                    if (CustomGameOptions.KillResurrectCooldownsLinked)
                        role.LastResurrected.AddSeconds(CustomGameOptions.VestKCReset);
                }

                return false;
            }

            return true;
        }
    }
}