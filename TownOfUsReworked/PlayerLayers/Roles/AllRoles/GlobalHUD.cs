using HarmonyLib;
using TownOfUsReworked.Classes;
using UnityEngine;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Roles.AllRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class GlobalHUD
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1 || PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null)
                return;

            __instance.KillButton.gameObject.SetActive(false);
            __instance.ReportButton.gameObject.SetActive(GameStates.IsRoaming);

            if (Utils.CanVent(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer.Data) && GameStates.IsInGame)
            {
                Sprite Vent;
                var role = Role.GetRole(PlayerControl.LocalPlayer);

                if (PlayerControl.LocalPlayer.Is(Faction.Intruder))
                    Vent = TownOfUsReworked.IntruderVent;
                else if (PlayerControl.LocalPlayer.Is(Faction.Syndicate))
                    Vent = TownOfUsReworked.SyndicateVent;
                else if (PlayerControl.LocalPlayer.Is(Faction.Crew))
                    Vent = TownOfUsReworked.CrewVent;
                else if (PlayerControl.LocalPlayer.Is(Faction.Neutral))
                    Vent = TownOfUsReworked.NeutralVent;
                else
                    Vent = __instance.ImpostorVentButton.graphic.sprite;

                __instance.ImpostorVentButton.graphic.sprite = Vent;
            }
        }
    }
}