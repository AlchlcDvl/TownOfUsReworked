using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.GuardianAngelMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class ShowProtect
    {
        public static void Postfix(HudManager __instance)
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
                        player.myRend().material.SetColor("_VisorColor", new Color32(255, 217, 0, 255));
                        player.myRend().material.SetFloat("_Outline", 1f);
                        player.myRend().material.SetColor("_OutlineColor", new Color32(255, 217, 0, 255));
                    }
                    else
                    {
                        player.myRend().material.SetColor("_VisorColor", Palette.VisorColor);
                        player.myRend().material.SetFloat("_Outline", 0f);
                    }
                }
                else if (ga.TargetPlayer.IsShielded())
                {
                    var showShielded = CustomGameOptions.ShowShielded;

                    if (showShielded == ShieldOptions.Everyone || (PlayerControl.LocalPlayer == player && (showShielded == ShieldOptions.Self || showShielded == ShieldOptions.SelfAndMedic))
                        || (PlayerControl.LocalPlayer.Is(RoleEnum.Medic) && (showShielded == ShieldOptions.Medic || showShielded == ShieldOptions.SelfAndMedic)))
                    {
                        player.myRend().material.SetColor("_VisorColor", new Color32(0, 255, 255, 255));
                        player.myRend().material.SetFloat("_Outline", 1f);
                        player.myRend().material.SetColor("_OutlineColor", new Color32(0, 255, 255, 255));
                    }
                    else
                    {
                        player.myRend().material.SetColor("_VisorColor", Palette.VisorColor);
                        player.myRend().material.SetFloat("_Outline", 0f);
                    }
                }
                else
                {
                    player.myRend().material.SetColor("_VisorColor", Palette.VisorColor);
                    player.myRend().material.SetFloat("_Outline", 0f);
                }
            }
        }
    }
}