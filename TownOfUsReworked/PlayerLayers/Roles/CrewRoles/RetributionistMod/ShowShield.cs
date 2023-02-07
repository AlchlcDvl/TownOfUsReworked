using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using System.Linq;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.RetributionistMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class ShowShield
    {
        public static Color ProtectedColor = Color.cyan;

        public static void Postfix(HudManager __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Retributionist))
            {
                var ret = (Retributionist)role;

                if (ret.RevivedRole?.RoleType != RoleEnum.Medic)
                    continue;

                var exPlayer = ret.exShielded;

                if (exPlayer != null)
                {
                    System.Console.WriteLine(exPlayer.name + " is ex-Shielded and unvisored");
                    exPlayer.myRend().material.SetColor("_VisorColor", Palette.VisorColor);
                    exPlayer.myRend().material.SetFloat("_Outline", 0f);
                    ret.exShielded = null;
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
                    player.myRend().material.SetColor("_VisorColor", ProtectedColor);
                    player.myRend().material.SetFloat("_Outline", 1f);
                    player.myRend().material.SetColor("_OutlineColor", ProtectedColor);

                    if (!MeetingHud.Instance)
                        continue;

                    SpriteRenderer icon = MeetingHud.Instance.playerStates.FirstOrDefault(v => v.TargetPlayerId == player.PlayerId).GAIcon;
                    icon.gameObject.SetActive(true);
                    icon.enabled = true;
                    icon.sprite = TownOfUsReworked.MedicSprite;
                    icon.transform.localScale = Vector2.one / 4;
                }
            }
        }
    }
}