using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using System;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.BlackmailerMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public class PerformBlackmail
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Blackmailer))
                return true;

            var role = Role.GetRole<Blackmailer>(PlayerControl.LocalPlayer);

            if (__instance == role.BlackmailButton)
            {
                if (role.BlackmailTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestBlackmail))
                    return false;
                
                var interact = Utils.Interact(role.Player, role.ClosestBlackmail);

                if (interact[3] == true)
                {
                    role.BlackmailedPlayer = role.ClosestBlackmail;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.Blackmail);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(role.ClosestBlackmail.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }

                if (interact[0] == true)
                    role.LastBlackmailed = DateTime.UtcNow;
                else if (interact[1] == true)
                    role.LastBlackmailed.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }

            return true;
        }
    }
}