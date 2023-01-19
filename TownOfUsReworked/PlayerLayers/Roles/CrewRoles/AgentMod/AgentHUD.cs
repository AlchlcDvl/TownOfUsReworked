using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.AgentMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class AgentHUD
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1 || PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null || !PlayerControl.LocalPlayer.Is(RoleEnum.Agent))
                return;

            var isDead = PlayerControl.LocalPlayer.Data.IsDead;

            if (isDead)
                return;

            var role = Role.GetRole<Agent>(PlayerControl.LocalPlayer);

            if (role.IsBlocked)
            {
                __instance.UseButton.gameObject.SetActive(false);
                __instance.ReportButton.gameObject.SetActive(false);
                __instance.MapButton.gameObject.SetActive(false);

                if (MapBehaviour.Instance)
                {
                    MapBehaviour.Instance.Close();
                    __instance.UseButton.gameObject.SetActive(false);
                    __instance.ReportButton.gameObject.SetActive(false);
                    __instance.MapButton.gameObject.SetActive(false);
                }

                if (Minigame.Instance)
                    Minigame.Instance.Close();
            }
            else
            {
                __instance.UseButton.gameObject.SetActive(true);
                __instance.ReportButton.gameObject.SetActive(true);
                __instance.MapButton.gameObject.SetActive(true);
            }
        }
    }
}