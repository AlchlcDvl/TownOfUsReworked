using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Linq;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.PlayerLayers.Objectifiers;
using TownOfUsReworked.PlayerLayers.Modifiers;
using TownOfUsReworked.PlayerLayers.Abilities;
using AmongUs.GameOptions;
using TMPro;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch]
    public static class Summary
    {
        public static class AdditionalTempData
        {
            public static readonly List<PlayerRoleInfo> PlayerRoles = new();

            public static void Clear() => PlayerRoles.Clear();

            public class PlayerRoleInfo
            {
                public string PlayerName { get; set; }
                public string Role { get; set; }
            }
        }

        [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
        public static class EndGameSummary
        {
            public static void Postfix()
            {
                AdditionalTempData.Clear();

                //There's a better way of doing this e.g. switch statement or dictionary. But this works for now.
                //AD says - Done.
                foreach (var playerControl in PlayerControl.AllPlayerControls)
                {
                    var summary = "";
                    const string endString = "</color>";
                    var TotalTasks = playerControl.Data.Tasks.ToArray().Length;
                    var playerTasksDone = playerControl.Data.Tasks.ToArray().Count(x => x.Complete);

                    var info = playerControl.AllPlayerInfo();

                    if (info[0] != null)
                    {
                        var role = info[0] as Role;

                        if (role.RoleHistory.Count != 0)
                        {
                            role.RoleHistory.Reverse();

                            foreach (var role2 in role.RoleHistory)
                                summary += $"{role2.ColorString}{role2.Name}{endString} → ";
                        }

                        summary += $"{role.ColorString}{role.Name}{endString}";
                    }

                    if (playerControl.IsRecruit())
                        summary += " <color=#575657FF>$</color>";

                    if (playerControl.IsPersuaded())
                        summary += " <color=#F995FCFF>Λ</color>";

                    if (playerControl.IsResurrected())
                        summary += " <color=#E6108AFF>Σ</color>";

                    if (playerControl.IsBitten())
                        summary += " <color=#7B8968FF>γ</color>";

                    if (info[3] != null)
                    {
                        var objectifier = info[3] as Objectifier;
                        summary += $" {objectifier.GetColoredSymbol()}";
                    }

                    if (info[1] != null)
                    {
                        var modifier = info[1] as Modifier;
                        summary += $" ({modifier.ColorString}{modifier.Name}{endString})";
                    }

                    if (info[2] != null)
                    {
                        var ability = info[2] as Ability;
                        summary += $" [{ability.ColorString}{ability.Name}{endString}]";
                    }

                    if (playerControl.IsGATarget())
                        summary += " <color=#FFFFFFFF>★</color>";

                    if (playerControl.IsExeTarget())
                        summary += " <color=#CCCCCCFF>§</color>";

                    if (playerControl.IsBHTarget())
                        summary += " <color=#B51E39FF>Θ</color>";

                    if (playerControl.IsGuessTarget())
                        summary += " <color=#EEE5BEFF>π</color>";

                    if (playerControl.CanDoTasks())
                        summary += " {" + playerTasksDone + "/" + TotalTasks + "}";

                    summary += " | " + playerControl.DeathReason();
                    AdditionalTempData.PlayerRoles.Add(new AdditionalTempData.PlayerRoleInfo() { PlayerName = playerControl.Data.PlayerName, Role = summary });
                }
            }
        }

        [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.SetEverythingUp))]
        public static class ShipStatusSetUpPatch
        {
            public static void Postfix(EndGameManager __instance)
            {
                if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek)
                    return;

                var bonusText = Object.Instantiate(__instance.WinText.gameObject);
                bonusText.transform.position = new Vector3(__instance.WinText.transform.position.x, __instance.WinText.transform.position.y - 0.8f, __instance.WinText.transform.position.z);
                bonusText.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
                var textRenderer = bonusText.GetComponent<TMP_Text>();
                textRenderer.text = "";

                var position = Camera.main.ViewportToWorldPoint(new Vector3(0f, 1f, Camera.main.nearClipPlane));
                var roleSummary = Object.Instantiate(__instance.WinText.gameObject);
                roleSummary.transform.position = new Vector3(__instance.Navigation.ExitButton.transform.position.x + 0.1f, position.y - 0.1f, -14f);
                roleSummary.transform.localScale = new Vector3(1f, 1f, 1f);

                var roleSummaryText = new StringBuilder();
                var winnersText = new StringBuilder();
                var losersText = new StringBuilder();

                var winnerCount = 0;
                var loserCount = 0;

                roleSummaryText.AppendLine("<size=125%><u><b>End Game Summary</b></u>:</size>");
                roleSummaryText.AppendLine();
                winnersText.AppendLine("<size=105%><b>Winners</b></size>");
                losersText.AppendLine("<size=105%><b>Losers</b></size>");

                foreach (var data in AdditionalTempData.PlayerRoles)
                {
                    var role = string.Join(" ", data.Role);
                    var dataString = $"<size=75%>{data.PlayerName} - {role}</size>";

                    if (data.PlayerName.IsWinner())
                    {
                        winnersText.AppendLine(dataString);
                        winnerCount++;
                    }
                    else
                    {
                        losersText.AppendLine(dataString);
                        loserCount++;
                    }
                }

                if (winnerCount == 0)
                    winnersText.AppendLine("<size=75%>No One Won</size>");

                if (loserCount == 0)
                    losersText.AppendLine("<size=75%>No One Lost</size>");

                roleSummaryText.Append(winnersText);
                roleSummaryText.AppendLine();
                roleSummaryText.Append(losersText);

                var roleSummaryTextMesh = roleSummary.GetComponent<TMP_Text>();
                roleSummaryTextMesh.alignment = TextAlignmentOptions.TopLeft;
                roleSummaryTextMesh.color = Color.white;
                roleSummaryTextMesh.fontSizeMin = 1.5f;
                roleSummaryTextMesh.fontSizeMax = 1.5f;
                roleSummaryTextMesh.fontSize = 1.5f;

                var roleSummaryTextMeshRectTransform = roleSummaryTextMesh.GetComponent<RectTransform>();
                roleSummaryTextMeshRectTransform.anchoredPosition = new Vector2(position.x + 3.5f, position.y - 0.1f);
                roleSummaryTextMesh.text = roleSummaryText.ToString();
                AdditionalTempData.Clear();
            }
        }

        private static bool IsWinner(this string playerName)
        {
            foreach (var win in TempData.winners)
            {
                if (win.PlayerName == playerName)
                    return true;
            }

            return false;
        }

        private static string DeathReason(this PlayerControl player)
        {
            if (player == null)
                return "";

            var role = Role.GetRole(player);

            if (role == null)
                return " ERROR";

            var die = $"{role.DeathReason}";
            var killedBy = "";

            if (role.DeathReason != DeathReasonEnum.Alive && role.DeathReason != DeathReasonEnum.Ejected && role.DeathReason != DeathReasonEnum.Suicide)
                killedBy = role.KilledBy;

            return die + killedBy;
        }
    }
}