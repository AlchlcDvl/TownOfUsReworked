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
            if (PlayerControl.AllPlayerControls.Count <= 1 || PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null || !PlayerControl.LocalPlayer.Is(RoleEnum.Glitch))
                return;

            var role = Role.GetRole<Glitch>(PlayerControl.LocalPlayer);

            if (AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started)
            {
                if (role != null)
                {
                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Glitch))
                        return;
                        //role.Update(__instance);
                }
            }
        }
    }
}