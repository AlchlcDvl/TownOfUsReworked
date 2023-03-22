using System;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.CryomaniacMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public class PerformAbility
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Cryomaniac))
                return true;

            if (!Utils.ButtonUsable(__instance))
                return false;

            var role = Role.GetRole<Cryomaniac>(PlayerControl.LocalPlayer);

            if (__instance == role.FreezeButton)
            {
                if (role.DousedAlive <= 0)
                    return false;

                if (role.FreezeUsed)
                    return false;

                role.FreezeUsed = true;
                return false;
            }
            else if (__instance == role.DouseButton)
            {
                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                if (role.DouseTimer() != 0f)
                    return false;

                if (role.DousedPlayers.Contains(role.ClosestPlayer.PlayerId))
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer);

                if (interact[3] == true)
                {
                    var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer2.Write((byte)ActionsRPC.FreezeDouse);
                    writer2.Write(role.Player.PlayerId);
                    writer2.Write(role.ClosestPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer2);
                    role.DousedPlayers.Add(role.ClosestPlayer.PlayerId);
                }

                if (interact[0])
                    role.LastDoused = DateTime.UtcNow;
                else if (interact[1] == true)
                    role.LastDoused.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }

            return true;
        }
    }
}
