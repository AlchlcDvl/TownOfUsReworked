using HarmonyLib;
using Hazel;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.ChameleonMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformSwoop
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Chameleon))
                return true;

            var role = Role.GetRole<Chameleon>(PlayerControl.LocalPlayer);

            if (__instance == role.SwoopButton)
            {
                if (role.SwoopTimer() != 0f)
                    return false;

                role.TimeRemaining = CustomGameOptions.SwoopDuration;
                role.Invis();
                role.UsesLeft--;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.Swoop);
                writer.Write(role.Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                return false;
            }

            return true;
        }
    }
}