using HarmonyLib;
using TownOfUsReworked.Classes;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.PlayerLayers.Abilities.Abilities;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.PlayerLayers.Roles.AllRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class GlobalHUD
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1 || PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null)
                return;

            var role = Role.GetRole(PlayerControl.LocalPlayer);
            __instance.KillButton.gameObject.SetActive(false);

            if (Utils.CanVent(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer.Data) && GameStates.IsInGame)
            {
                if ((role?.AbilityButtons?.Count == 0 || role?.AbilityButtons?.Count == 1) && !PlayerControl.LocalPlayer.Is(ObjectifierEnum.Corrupted) &&
                    !PlayerControl.LocalPlayer.Is(AbilityEnum.ButtonBarry))
                    __instance.ImpostorVentButton.transform.localPosition = __instance.AbilityButton.transform.localPosition;
                else if ((role?.AbilityButtons?.Count == 2) && !PlayerControl.LocalPlayer.Is(ObjectifierEnum.Corrupted) && !PlayerControl.LocalPlayer.Is(AbilityEnum.ButtonBarry) &&
                    !(PlayerControl.LocalPlayer.Is(Faction.Intruder) && CustomGameOptions.IntrudersCanSabotage) && !(PlayerControl.LocalPlayer.Is(Faction.Syndicate) &&
                    CustomGameOptions.AltImps))
                    __instance.ImpostorVentButton.transform.localPosition = __instance.SabotageButton.transform.localPosition;

                Sprite Vent;

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