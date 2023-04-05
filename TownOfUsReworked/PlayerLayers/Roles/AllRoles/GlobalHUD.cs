using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using UnityEngine;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.AllRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class GlobalHUD
    {
        public static void Postfix(HudManager __instance)
        {
            var local = PlayerControl.LocalPlayer;

            if (ConstantVariables.Inactive)
                return;

            __instance.KillButton.SetTarget(null);
            __instance.KillButton.gameObject.SetActive(false);
            __instance.AbilityButton.gameObject.SetActive(false);
            var role = Role.GetRole(local);

            if (role == null)
                return;

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                Janitor.DragBody(player);
                Godfather.DragBody(player);
            }

            foreach (var layer in PlayerLayer.GetLayers(local))
                layer.UpdateHud(__instance);

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

            if (local.Is(ModifierEnum.Coward))
                __instance.ReportButton.gameObject.SetActive(false);

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

            if (local.IsBlocked() && Minigame.Instance)
                Minigame.Instance.Close();

            if (local.IsBlocked() && MapBehaviour.Instance)
                MapBehaviour.Instance.Close();
        }
    }
}