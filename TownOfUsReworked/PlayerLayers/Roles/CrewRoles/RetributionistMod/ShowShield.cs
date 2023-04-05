using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.RetributionistMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class ShowShield
    {
        public static void Postfix()
        {
            if (ConstantVariables.IsLobby || ConstantVariables.IsEnded)
                return;

            foreach (var ret in Role.GetRoles<Retributionist>(RoleEnum.Retributionist))
            {
                if (ret.RevivedRole?.RoleType != RoleEnum.Medic)
                    continue;

                var exPlayer = ret.ExShielded;

                if (exPlayer != null)
                {
                    Utils.LogSomething(exPlayer.name + " is ex-Shielded and unvisored");
                    exPlayer.MyRend().material.SetColor("_VisorColor", Palette.VisorColor);
                    exPlayer.MyRend().material.SetFloat("_Outline", 0f);
                    ret.ExShielded = null;
                    continue;
                }

                var player = ret.ShieldedPlayer;

                if (player == null)
                    continue;

                if (player.Data.IsDead || ret.Player.Data.IsDead || ret.Player.Data.Disconnected)
                {
                    StopKill.BreakShield(ret.Player.PlayerId, player.PlayerId, true);
                    continue;
                }

                var showShielded = CustomGameOptions.ShowShielded;

                if (showShielded == ShieldOptions.Everyone || (PlayerControl.LocalPlayer.PlayerId == player.PlayerId && (showShielded == ShieldOptions.Self ||
                    showShielded == ShieldOptions.SelfAndMedic)) || (PlayerControl.LocalPlayer.Is(RoleEnum.Medic) && (showShielded == ShieldOptions.Medic ||
                    showShielded == ShieldOptions.SelfAndMedic)))
                {
                    player.MyRend().material.SetColor("_VisorColor", Color.cyan);
                    player.MyRend().material.SetFloat("_Outline", 1f);
                    player.MyRend().material.SetColor("_OutlineColor", Color.cyan);
                }
            }
        }
    }
}