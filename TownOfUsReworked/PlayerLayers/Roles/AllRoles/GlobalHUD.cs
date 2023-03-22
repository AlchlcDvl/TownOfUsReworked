using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.AllRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class GlobalHUD
    {
        public static void Postfix(HudManager __instance)
        {
            if (GameStates.IsLobby)
            {
                __instance.ReportButton.gameObject.SetActive(false);

                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    if (GameSettings.SettingsPage >= 7)
                        GameSettings.SettingsPage = 0;
                    else
                        GameSettings.SettingsPage++;
                }

                return;
            }
            
            var local = PlayerControl.LocalPlayer;

            if (PlayerControl.AllPlayerControls.Count <= 1 || local == null || local.Data == null || (GameStates.IsInGame && GameStates.IsHnS) || GameStates.IsEnded)
                return;

            __instance.KillButton.gameObject.SetActive(false);
            __instance.AbilityButton.gameObject.SetActive(false);

            var role = Role.GetRole(local);

            if (role == null)
                return;

            if (local.Data.IsDead)
                role.IsBlocked = false;

            if (Utils.CanVent(local, local.Data))
            {
                var Vent = __instance.ImpostorVentButton.graphic.sprite;

                if (local.IsBlocked())
                    Vent = AssetManager.Blocked;
                else if (local.Is(Faction.Intruder))
                    Vent = AssetManager.IntruderVent;
                else if (local.Is(Faction.Syndicate))
                    Vent = AssetManager.SyndicateVent;
                else if (local.Is(Faction.Crew))
                    Vent = AssetManager.CrewVent;
                else if (local.Is(Faction.Neutral))
                    Vent = AssetManager.NeutralVent;

                __instance.ImpostorVentButton.graphic.sprite = Vent;
                __instance.ImpostorVentButton.buttonLabelText.SetOutlineColor(role.FactionColor);
                __instance.ImpostorVentButton.buttonLabelText.fontSharedMaterial = __instance.SabotageButton.buttonLabelText.fontSharedMaterial;

                if (local.Is(RoleEnum.Revealer) || local.Is(RoleEnum.Phantom) || local.Is(RoleEnum.Banshee) || local.Is(RoleEnum.Ghoul))
                    __instance.ImpostorVentButton.gameObject.SetActive(local.inVent);
            }

            var Report = __instance.ReportButton.graphic.sprite;

            if (local.IsBlocked())
                Report = AssetManager.Blocked;
            else
                Report = AssetManager.Report;

            __instance.ReportButton.graphic.sprite = Report;
            __instance.ReportButton.buttonLabelText.SetOutlineColor(role.FactionColor);
            __instance.ReportButton.buttonLabelText.fontSharedMaterial = __instance.SabotageButton.buttonLabelText.fontSharedMaterial;

            var closestDead = Utils.GetClosestDeadPlayer(local, CustomGameOptions.ReportDistance);

            if (closestDead == null || Utils.CannotUse(local))
                __instance.ReportButton.SetDisabled();
            else
                __instance.ReportButton.SetEnabled();

            var Use = __instance.UseButton.graphic.sprite;

            if (local.IsBlocked())
                Use = AssetManager.Blocked;
            else
                Use = AssetManager.Use;

            __instance.UseButton.graphic.sprite = Use;
            __instance.UseButton.buttonLabelText.SetOutlineColor(role.FactionColor);
            __instance.UseButton.buttonLabelText.fontSharedMaterial = __instance.SabotageButton.buttonLabelText.fontSharedMaterial;

            var Pet = __instance.PetButton.graphic.sprite;

            if (local.IsBlocked())
                Pet = AssetManager.Blocked;
            else
                Pet = AssetManager.Pet;

            __instance.PetButton.graphic.sprite = Pet;
            __instance.PetButton.buttonLabelText.SetOutlineColor(role.FactionColor);
            __instance.PetButton.buttonLabelText.fontSharedMaterial = __instance.SabotageButton.buttonLabelText.fontSharedMaterial;

            var Sabotage = __instance.SabotageButton.graphic.sprite;

            if (local.IsBlocked())
                Sabotage = AssetManager.Blocked;
            else if (local.Is(Faction.Intruder))
                Sabotage = AssetManager.Sabotage;
            else if (local.Is(Faction.Syndicate))
                Sabotage = AssetManager.SyndicateSabotage;

            __instance.SabotageButton.graphic.sprite = Sabotage;
            __instance.SabotageButton.buttonLabelText.SetOutlineColor(role.FactionColor);

            if (Utils.CannotUse(local))
                __instance.SabotageButton.SetDisabled();
            else
                __instance.SabotageButton.SetEnabled();

            if (role.IsBlocked && Minigame.Instance)
                Minigame.Instance.Close();

            if (role.IsBlocked && MapBehaviour.Instance)
                MapBehaviour.Instance.Close();
            
            if (local.Data.IsDead)
            {
                if (role.SpectateButton = null)
                    role.SpectateButton = Utils.InstantiateButton();
                
                role.SpectateButton.UpdateButton(role, "SPECTATE", 0, 1, AssetManager.Placeholder, AbilityTypes.Effect, "ActionSecondary", null, true, true, false, 0, 1, false, 0, true);
            }
        }
    }
}