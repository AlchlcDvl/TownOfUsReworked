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
            if (GameStates.IsLobby)
                __instance.ReportButton.gameObject.SetActive(false);

            if (PlayerControl.AllPlayerControls.Count <= 1 || PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null || (GameStates.IsInGame && GameStates.IsHnS) ||
                GameStates.IsEnded)
                return;

            var role = Role.GetRole(PlayerControl.LocalPlayer);

            if (role == null)
                return;

            if (Utils.CanVent(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer.Data))
            {
                Sprite Vent = __instance.ImpostorVentButton.graphic.sprite;

                if (PlayerControl.LocalPlayer.IsBlocked())
                    Vent = TownOfUsReworked.Blocked;
                else if (PlayerControl.LocalPlayer.Is(Faction.Intruder))
                    Vent = TownOfUsReworked.IntruderVent;
                else if (PlayerControl.LocalPlayer.Is(Faction.Syndicate))
                    Vent = TownOfUsReworked.SyndicateVent;
                else if (PlayerControl.LocalPlayer.Is(Faction.Crew))
                    Vent = TownOfUsReworked.CrewVent;
                else if (PlayerControl.LocalPlayer.Is(Faction.Neutral))
                    Vent = TownOfUsReworked.NeutralVent;

                __instance.ImpostorVentButton.graphic.sprite = Vent;
                __instance.ImpostorVentButton.buttonLabelText.SetOutlineColor(role == null ? Color.red : role.FactionColor);

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Revealer) || PlayerControl.LocalPlayer.Is(RoleEnum.Phantom))
                    __instance.ImpostorVentButton.gameObject.SetActive(PlayerControl.LocalPlayer.inVent);
            }

            Sprite Report = __instance.ReportButton.graphic.sprite;

            if (PlayerControl.LocalPlayer.IsBlocked())
                Report = TownOfUsReworked.Blocked;
            else
                Report = TownOfUsReworked.Report;
            
            __instance.ReportButton.graphic.sprite = Report;
            __instance.ReportButton.buttonLabelText.SetOutlineColor(role == null ? Color.red : role.FactionColor);

            if (PlayerControl.LocalPlayer.IsBlocked() || Utils.GetClosestDeadPlayer(PlayerControl.LocalPlayer) == null)
                __instance.ReportButton.SetDisabled();
            else
                __instance.ReportButton.SetEnabled();

            Sprite Use = __instance.ReportButton.graphic.sprite;

            if (PlayerControl.LocalPlayer.IsBlocked())
                Use = TownOfUsReworked.Blocked;
            else
                Use = TownOfUsReworked.Use;
            
            __instance.UseButton.graphic.sprite = Use;
            __instance.UseButton.buttonLabelText.SetOutlineColor(role == null ? Color.red : role.FactionColor);

            __instance.KillButton.gameObject.SetActive(false);

            if (role.IsBlocked && Minigame.Instance)
                Minigame.Instance.Close();

            if (role.IsBlocked && MapBehaviour.Instance)
                MapBehaviour.Instance.Close();
        }
    }
}