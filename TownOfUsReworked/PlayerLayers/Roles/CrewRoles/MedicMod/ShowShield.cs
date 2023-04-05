using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using UnityEngine;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class ShowShield
    {
        public static void Postfix()
        {
            if (ConstantVariables.IsLobby || ConstantVariables.IsEnded)
                return;

            foreach (var medic in Role.GetRoles<Medic>(RoleEnum.Medic))
            {
                var exPlayer = medic.ExShielded;

                if (exPlayer != null)
                {
                    Utils.LogSomething(exPlayer.name + " is ex-Shielded and unvisored");
                    exPlayer.MyRend().material.SetColor("_VisorColor", Palette.VisorColor);
                    exPlayer.MyRend().material.SetFloat("_Outline", 0f);
                    medic.ExShielded = null;
                    continue;
                }

                var player = medic.ShieldedPlayer;

                if (player == null)
                    continue;

                if (player.Data.IsDead || medic.Player.Data.IsDead || medic.Player.Data.Disconnected)
                {
                    StopKill.BreakShield(medic.Player.PlayerId, player.PlayerId, true);
                    continue;
                }

                var showShielded = CustomGameOptions.ShowShielded;

                if (showShielded == ShieldOptions.Everyone || (PlayerControl.LocalPlayer.PlayerId == player.PlayerId && (showShielded == ShieldOptions.Self ||
                    showShielded == ShieldOptions.SelfAndMedic)) || (PlayerControl.LocalPlayer == medic.Player && (showShielded == ShieldOptions.Medic ||
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