using HarmonyLib;
using InnerNet;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.GlitchMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class Update
    {
        private static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1)
                return;

            if (PlayerControl.LocalPlayer == null)
                return;

            if (PlayerControl.LocalPlayer.Data == null)
                return;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Glitch))
                return;

            var role = Role.GetRole<Glitch>(PlayerControl.LocalPlayer);

            if (AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started)
            {
                if (role != null)
                {
                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Glitch))
                        role.Update(__instance);
                }
            }
        }
    }
}