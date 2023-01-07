using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Linq;
using Reactor.Utilities.Extensions;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.PlayerLayers.Objectifiers;
using TownOfUsReworked.PlayerLayers.Modifiers;
using TownOfUsReworked.PlayerLayers.Abilities;

namespace TownOfUsReworked.Patches
{
    static class AdditionalTempData
    {
        public static List<PlayerRoleInfo> playerRoles = new List<PlayerRoleInfo>();

        public static void clear()
        {
            playerRoles.Clear();
        }

        internal class PlayerRoleInfo
        {
            public string PlayerName { get; set; }
            public string Role { get; set; }
        }
    }

    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
    public class OnGameEndPatch
    {
        public static void Postfix(AmongUsClient __instance, [HarmonyArgument(0)] EndGameResult endGameResult)
        {
            AdditionalTempData.clear();

            //There's a better way of doing this e.g. switch statement or dictionary. But this works for now.
            //AD says - Done.
            foreach (var playerControl in PlayerControl.AllPlayerControls)
            {
                var summary = "";
                var colorString = "";
                var roleName = "";
                var modifierName = "";
                var abilityName = "";
                var endString = "";
                var modifierString = "";
                var abilityString = "";
                var totalString = "";
                var TotalTasks = playerControl.Data.Tasks.ToArray().Count();
                var playerTasksDone = playerControl.Data.Tasks.ToArray().Count(x => x.Complete);
                
                foreach (var role in Role.RoleHistory.Where(x => x.Key == playerControl.PlayerId))
                {
                    var role2 = Role.GetRoleValue(role.Value);
                    totalString = "";

                    if (role2 != null)
                    {
                        colorString = "<color=#" + role2.Color.ToHtmlStringRGBA() + ">";
                        roleName = role2.Name;
                        endString = "</color> → ";

                        totalString = colorString + roleName + endString;
                    }

                    summary += totalString;
                }

                if (summary.Length != 0)
                    summary = summary.Remove(summary.Length - 3);

                var modifier = Modifier.GetModifier(playerControl);

                if (modifier != null)
                {
                    colorString = " (<color=#" + modifier.Color.ToHtmlStringRGBA() + ">";
                    modifierName = modifier.Name;
                    endString = "</color>)";

                    modifierString = colorString + modifierName + endString;

                    summary += modifierString;
                }

                var ability = Ability.GetAbility(playerControl);

                if (ability != null)
                {
                    colorString = " [<color=#" + ability.Color.ToHtmlStringRGBA() + ">";
                    abilityName = ability.Name;
                    endString = "</color>]";

                    abilityString = colorString + abilityName + endString;

                    summary += abilityString;
                }

                var objectifier = Objectifier.GetObjectifier(playerControl);

                if (objectifier != null)
                    summary += $" {objectifier.GetColoredSymbol()}";

                if (playerControl.IsGATarget())
                    summary += " <color=#FFFFFFFF>★</color>";

                if (playerControl.IsExeTarget())
                    summary += " <color=#CCCCCCFF>§</color>";

                if (playerControl.IsRecruit())
                    summary += " <color=#575657FF>$</color>";
                
                if (playerControl.CanDoTasks())
                    summary += " {" + playerTasksDone + "/" + TotalTasks + "}";
                
                summary += " | " + playerControl.DeathReason();

                AdditionalTempData.playerRoles.Add(new AdditionalTempData.PlayerRoleInfo() {PlayerName = playerControl.Data.PlayerName, Role = summary});
            }
        }
    }

    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.SetEverythingUp))]
    public class ShipStatusSetUpPatch
    {
        public static void Postfix(EndGameManager __instance)
        {
            GameObject bonusText = UnityEngine.Object.Instantiate(__instance.WinText.gameObject);
            bonusText.transform.position = new Vector3(__instance.WinText.transform.position.x, __instance.WinText.transform.position.y - 0.8f,
                __instance.WinText.transform.position.z);
            bonusText.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
            TMPro.TMP_Text textRenderer = bonusText.GetComponent<TMPro.TMP_Text>();
            textRenderer.text = "";

            var position = Camera.main.ViewportToWorldPoint(new Vector3(0f, 1f, Camera.main.nearClipPlane));
            GameObject roleSummary = UnityEngine.Object.Instantiate(__instance.WinText.gameObject);
            roleSummary.transform.position = new Vector3(__instance.Navigation.ExitButton.transform.position.x + 0.1f, position.y - 0.1f, -14f); 
            roleSummary.transform.localScale = new Vector3(1f, 1f, 1f);

            var roleSummaryText = new StringBuilder();
            var winnersText = new StringBuilder();
            var losersText = new StringBuilder();
            var winners = TempData.winners;

            roleSummaryText.AppendLine("<size=120%><u><b>End Game Summary</b></u>:</size>");
            roleSummaryText.AppendLine(" ");
            winnersText.AppendLine("<size=105%><b>Winners</b></size> -");
            losersText.AppendLine("<size=105%><b>Losers</b></size> -");

            foreach (var data in AdditionalTempData.playerRoles)
            {
                var role = string.Join(" ", data.Role);
                var dataString = $"<size=95%>{data.PlayerName} - {role}</size>";

                if (data.PlayerName.IsWinner())
                    winnersText.AppendLine(dataString);
                else
                    losersText.AppendLine(dataString);
            }

            winnersText.AppendLine(" ");
            
            TMPro.TMP_Text roleSummaryTextMesh = roleSummary.GetComponent<TMPro.TMP_Text>();
            roleSummaryTextMesh.alignment = TMPro.TextAlignmentOptions.TopLeft;
            roleSummaryTextMesh.color = Color.white;
            roleSummaryTextMesh.fontSizeMin = 1.5f;
            roleSummaryTextMesh.fontSizeMax = 1.5f;
            roleSummaryTextMesh.fontSize = 1.5f;
             
            var roleSummaryTextMeshRectTransform = roleSummaryTextMesh.GetComponent<RectTransform>();
            roleSummaryTextMeshRectTransform.anchoredPosition = new Vector2(position.x + 3.5f, position.y - 0.1f);
            roleSummaryTextMesh.text = roleSummaryText.ToString() + winnersText.ToString() + losersText.ToString();
            AdditionalTempData.clear();
        }
    }
}