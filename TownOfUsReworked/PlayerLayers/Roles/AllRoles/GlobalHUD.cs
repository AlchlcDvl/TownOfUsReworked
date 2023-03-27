using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using UnityEngine;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;
using TownOfUsReworked.Modules;

namespace TownOfUsReworked.PlayerLayers.Roles.AllRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class GlobalHUD
    {
        public static void Postfix(HudManager __instance)
        {
            var local = PlayerControl.LocalPlayer;

            if (PlayerControl.AllPlayerControls.Count <= 1 || local == null || local.Data == null || (ConstantVariables.IsInGame && ConstantVariables.IsHnS) || ConstantVariables.IsEnded)
                return;

            __instance.KillButton.SetTarget(null);
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
            }

            Sprite Report;

            if (local.IsBlocked())
                Report = AssetManager.Blocked;
            else
                Report = AssetManager.Report;

            __instance.ReportButton.graphic.sprite = Report;
            __instance.ReportButton.buttonLabelText.SetOutlineColor(role.FactionColor);
            __instance.ReportButton.buttonLabelText.fontSharedMaterial = __instance.SabotageButton.buttonLabelText.fontSharedMaterial;

            var closestDead = CustomButtons.GetClosestDeadPlayer(local, CustomGameOptions.ReportDistance);

            if (closestDead == null || CustomButtons.CannotUse(local))
                __instance.ReportButton.SetDisabled();
            else
                __instance.ReportButton.SetEnabled();

            Sprite Use;

            if (local.IsBlocked())
                Use = AssetManager.Blocked;
            else
                Use = AssetManager.Use;

            __instance.UseButton.graphic.sprite = Use;
            __instance.UseButton.buttonLabelText.SetOutlineColor(role.FactionColor);
            __instance.UseButton.buttonLabelText.fontSharedMaterial = __instance.SabotageButton.buttonLabelText.fontSharedMaterial;

            Sprite Pet;

            if (local.IsBlocked())
                Pet = AssetManager.Blocked;
            else
                Pet = AssetManager.Pet;

            __instance.PetButton.graphic.sprite = Pet;
            __instance.PetButton.buttonLabelText.SetOutlineColor(role.FactionColor);
            __instance.PetButton.buttonLabelText.fontSharedMaterial = __instance.SabotageButton.buttonLabelText.fontSharedMaterial;

            Sprite Sabotage;

            if (local.IsBlocked())
                Sabotage = AssetManager.Blocked;
            else if (local.Is(Faction.Syndicate))
                Sabotage = AssetManager.SyndicateSabotage;
            else
                Sabotage = AssetManager.Sabotage;

            __instance.SabotageButton.graphic.sprite = Sabotage;
            __instance.SabotageButton.buttonLabelText.SetOutlineColor(role.FactionColor);

            if (CustomButtons.CannotUse(local))
                __instance.SabotageButton.SetDisabled();
            else
                __instance.SabotageButton.SetEnabled();

            if (role.IsBlocked && Minigame.Instance)
                Minigame.Instance.Close();

            if (role.IsBlocked && MapBehaviour.Instance)
                MapBehaviour.Instance.Close();

            if (local.Data.IsDead)
            {
                var ghostRole = (local.Is(RoleEnum.Revealer) && !Role.GetRole<Revealer>(local).Caught) || (local.Is(RoleEnum.Ghoul) && !Role.GetRole<Ghoul>(local).Caught) ||
                    (local.Is(RoleEnum.Banshee) && !Role.GetRole<Banshee>(local).Caught) || (local.Is(RoleEnum.Phantom) && !Role.GetRole<Phantom>(local).Caught);

                if (!ghostRole)
                {
                    if (role.SpectateButton == null)
                        role.SpectateButton = CustomButtons.InstantiateButton();

                    role.SpectateButton.UpdateButton(role, "SPECTATE", 0, 1, AssetManager.Placeholder, AbilityTypes.Effect, "ActionSecondary", null, !ghostRole, !ghostRole, false, 0, 1,
                        false, 0, !ghostRole);

                    if (role.ZoomButton == null)
                        role.ZoomButton = CustomButtons.InstantiateButton();

                    role.ZoomButton.UpdateButton(role, "SPECTATE", 0, 1, role.Zooming ? AssetManager.Minus : AssetManager.Plus, AbilityTypes.Effect, "Secondary", null, !ghostRole, !ghostRole,
                        false, 0, 1, false, 0, !ghostRole);
                }
            }
            else if (local.CanInteractWithBodyInVent())
            {
                if (role.PullButton == null)
                    role.PullButton = CustomButtons.InstantiateButton();

                role.PullButton.UpdateButton(role, "PULL BODY", 0, 1, AssetManager.Placeholder, AbilityTypes.Vent, "Quarternary", null, local.CanInteractWithBodyInVent());
            }
        }
    }
}