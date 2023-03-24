using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.DrunkardMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformConfuse
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Drunkard))
                return true;

            var role = Role.GetRole<Drunkard>(PlayerControl.LocalPlayer);

            if (__instance == role.ConfuseButton)
            {
                if (role.DrunkTimer() != 0f)
                    return false;

                if (role.Confused)
                    return false;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.Confuse);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Reverse.ConfuseFunctions.ConfuseAll();
                role.TimeRemaining = CustomGameOptions.ConfuseDuration;
                role.Confuse();
                return false;
            }

            return true;
        }
    }
}