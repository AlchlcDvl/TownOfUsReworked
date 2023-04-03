using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.GuardianAngelMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class ShowProtect
    {
        public static void Postfix()
        {
            foreach (var role in Role.GetRoles(RoleEnum.GuardianAngel))
            {
                var ga = (GuardianAngel)role;
                var player = ga.TargetPlayer;

                if (player == null)
                    continue;

                if (ga.Protecting)
                {
                    var showProtected = CustomGameOptions.ShowProtect;

                    if (showProtected == ProtectOptions.Everyone || (PlayerControl.LocalPlayer == player && (showProtected == ProtectOptions.Self || showProtected ==
                        ProtectOptions.SelfAndGA)) || (PlayerControl.LocalPlayer.Is(RoleEnum.GuardianAngel) && (showProtected == ProtectOptions.GA || showProtected ==
                        ProtectOptions.SelfAndGA)))
                    {
                        player.MyRend().material.SetColor("_VisorColor", new Color32(255, 217, 0, 255));
                        player.MyRend().material.SetFloat("_Outline", 1f);
                        player.MyRend().material.SetColor("_OutlineColor", new Color32(255, 217, 0, 255));
                    }
                    else
                    {
                        player.MyRend().material.SetColor("_VisorColor", Palette.VisorColor);
                        player.MyRend().material.SetFloat("_Outline", 0f);
                    }
                }
                else if (ga.TargetPlayer.IsShielded())
                {
                    var showShielded = CustomGameOptions.ShowShielded;

                    if (showShielded == ShieldOptions.Everyone || (PlayerControl.LocalPlayer == player && (showShielded == ShieldOptions.Self || showShielded == ShieldOptions.SelfAndMedic))
                        || (PlayerControl.LocalPlayer.Is(RoleEnum.Medic) && (showShielded == ShieldOptions.Medic || showShielded == ShieldOptions.SelfAndMedic)))
                    {
                        player.MyRend().material.SetColor("_VisorColor", new Color32(0, 255, 255, 255));
                        player.MyRend().material.SetFloat("_Outline", 1f);
                        player.MyRend().material.SetColor("_OutlineColor", new Color32(0, 255, 255, 255));
                    }
                    else
                    {
                        player.MyRend().material.SetColor("_VisorColor", Palette.VisorColor);
                        player.MyRend().material.SetFloat("_Outline", 0f);
                    }
                }
                else
                {
                    player.MyRend().material.SetColor("_VisorColor", Palette.VisorColor);
                    player.MyRend().material.SetFloat("_Outline", 0f);
                }
            }
        }
    }
}