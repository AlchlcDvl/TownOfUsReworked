using HarmonyLib;
using Hazel;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using System;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.CrusaderMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformCrusade
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Crusader))
                return true;

            var role = Role.GetRole<Crusader>(PlayerControl.LocalPlayer);

            if (__instance == role.CrusadeButton)
            {
                if (role.CrusadeTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestCrusade))
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestCrusade);

                if (interact[3])
                {
                    role.TimeRemaining = CustomGameOptions.AmbushDuration;
                    role.CrusadedPlayer = role.ClosestCrusade;
                    role.Crusade();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.Crusade);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(role.CrusadedPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
                else if (interact[0])
                    role.LastCrusaded = DateTime.UtcNow;
                else if (interact[1])
                    role.LastCrusaded.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }

            return true;
        }
    }
}