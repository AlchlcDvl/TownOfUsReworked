using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.ChameleonMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformSwoop
    {
        public static bool Prefix(KillButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Chameleon))
                return false;
            
            if (!PlayerControl.LocalPlayer.CanMove)
                return false;

            var role = Role.GetRole<Chameleon>(PlayerControl.LocalPlayer);

            if (__instance == role.SwoopButton)
            {
                if (!__instance.isActiveAndEnabled)
                    return false;

                if (role.SwoopTimer() != 0f)
                    return false;

                role.TimeRemaining = CustomGameOptions.SwoopDuration;
                role.RegenTask();
                role.Invis();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                writer.Write((byte)ActionsRPC.Swoop);
                writer.Write(role.Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                return false;
            }

            return false;
        }
    }
}