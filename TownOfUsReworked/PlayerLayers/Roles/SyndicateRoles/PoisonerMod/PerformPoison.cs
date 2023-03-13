using System;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.PoisonerMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public class PerformPoison
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Poisoner))
                return true;

            if (!Utils.ButtonUsable(__instance))
                return false;

            var role = Role.GetRole<Poisoner>(PlayerControl.LocalPlayer);
            
            if (__instance == role.PoisonButton)
            {
                if (role.PoisonTimer() != 0f)
                    return false;
                
                if (role.Poisoned)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;
                
                var interact = Utils.Interact(role.Player, role.ClosestPlayer, false, true);

                if (interact[3] == true)
                {
                    role.PoisonedPlayer = role.ClosestPlayer;
                    role.TimeRemaining = CustomGameOptions.PoisonDuration;
                    var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer2.Write((byte)ActionsRPC.Poison);
                    writer2.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer2.Write(role.PoisonedPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer2);
                    role.Poison();
                }

                if (interact[0] == true && Role.SyndicateHasChaosDrive)
                    role.LastPoisoned = DateTime.UtcNow;
                else if (interact[1] == true)
                    role.LastPoisoned.AddSeconds(CustomGameOptions.ProtectKCReset);
                else if (interact[2] == true)
                    role.LastPoisoned.AddSeconds(CustomGameOptions.VestKCReset);

                return false;
            }

            return true;
        }
    }
}