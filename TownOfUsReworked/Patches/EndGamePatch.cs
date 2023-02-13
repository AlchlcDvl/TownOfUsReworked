using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Linq;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.PlayerLayers.Objectifiers;
using TownOfUsReworked.PlayerLayers.Modifiers;
using TownOfUsReworked.PlayerLayers.Abilities;
using AmongUs.GameOptions;

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
                var endString = "</color>";
                var modifierString = "";
                var abilityString = "";
                var roleString = "";
                var TotalTasks = playerControl.Data.Tasks.ToArray().Count();
                var playerTasksDone = playerControl.Data.Tasks.ToArray().Count(x => x.Complete);

                var role = Role.GetRole(playerControl);

                if (role != null)
                {
                    if (role.RoleHistory.Count != 0)
                    {
                        role.RoleHistory.Reverse();

                        foreach (var role2 in role.RoleHistory)
                        {
                            colorString = role2.ColorString;
                            roleName = role2.Name;
                            roleString = colorString + roleName + endString + " → ";

                            summary += roleString;
                        }
                    }

                    colorString = role.ColorString;
                    roleName = role.Name;
                    roleString = colorString + roleName + endString;

                    summary += roleString;
                }

                if (playerControl.IsRecruit())
                    summary += " <color=#575657FF>$</color>";

                if (playerControl.IsPersuaded())
                    summary += " <color=#F995FCFF>Λ</color>";

                if (playerControl.IsResurrected())
                    summary += " <color=#E6108AFF>Σ</color>";

                var objectifier = Objectifier.GetObjectifier(playerControl);

                if (objectifier != null)
                    summary += $" {objectifier.GetColoredSymbol()}";

                var modifier = Modifier.GetModifier(playerControl);

                if (modifier != null)
                {
                    colorString = " (" + modifier.ColorString;
                    modifierName = modifier.Name;

                    modifierString = colorString + modifierName + endString + ")";

                    summary += modifierString;
                }

                var ability = Ability.GetAbility(playerControl);

                if (ability != null)
                {
                    colorString = " [" + ability.ColorString;
                    abilityName = ability.Name;

                    abilityString = colorString + abilityName + endString + "]";

                    summary += abilityString;
                }

                if (playerControl.IsGATarget())
                    summary += " <color=#FFFFFFFF>★</color>";

                if (playerControl.IsExeTarget())
                    summary += " <color=#CCCCCCFF>§</color>";

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
            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek)
                return;

            GameObject bonusText = UnityEngine.Object.Instantiate(__instance.WinText.gameObject);
            bonusText.transform.position = new Vector3(__instance.WinText.transform.position.x, __instance.WinText.transform.position.y - 0.8f, __instance.WinText.transform.position.z);
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

            var winnerCount = 0;
            var loserCount = 0;

            roleSummaryText.AppendLine("<size=125%><u><b>End Game Summary</b></u>:</size>");
            roleSummaryText.AppendLine(" ");
            winnersText.AppendLine("<size=105%><b>Winners</b></size> -");
            losersText.AppendLine("<size=105%><b>Losers</b></size> -");

            foreach (var data in AdditionalTempData.playerRoles)
            {
                var role = string.Join(" ", data.Role);
                var dataString = $"<size=75%>{data.PlayerName} - {role}</size>";

                if (data.PlayerName.IsWinner())
                {
                    winnersText.AppendLine(dataString);
                    winnerCount += 1;
                }
                else
                {
                    losersText.AppendLine(dataString);
                    loserCount += 1;
                }
            }

            if (winnerCount == 0)
                winnersText.AppendLine("<size=75%>No One Won</size>");

            if (loserCount == 0)
                winnersText.AppendLine("<size=75%>No One Lost</size>");

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