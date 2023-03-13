using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using System;
using System.Linq;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Patches;
using TownOfUsReworked.PlayerLayers.Roles.AllRoles;
using Random = UnityEngine.Random;
using TownOfUsReworked.Objects;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.RetributionistMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class StopRevive
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Retributionist))
                return;

            var ret = Role.GetRole<Retributionist>(PlayerControl.LocalPlayer);

            if (ret.RevivedRole == null)
                return;

            if (ret.RevivedRole.RoleType == RoleEnum.Operative)
            {
                if (ret.BuggedPlayers.Count == 0)
                    HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "No one triggered your bugs.");
                else if (ret.BuggedPlayers.Count < CustomGameOptions.MinAmountOfPlayersInBug)
                    HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "Not enough players triggered your bugs.");
                else
                {
                    string message = "Roles caught in your bugs:\n";

                    foreach (RoleEnum role in ret.BuggedPlayers.OrderBy(x => Guid.NewGuid()))
                        message += $" {role},";

                    message.Remove(message.Length - 1, 1);
                    
                    //Ensures only the Retributionist-Operative sees this
                    if (HudManager.Instance)
                        HudManager.Instance.Chat.AddChat(ret.Player, message);
                }
            }
            else if (ret.RevivedRole.RoleType == RoleEnum.Coroner)
            {
                var matches = Murder.KilledPlayers.ToArray();
                DeadPlayer killer = null;

                if (matches.Length > 0)
                {
                    var random = Random.RandomRangeInt(0, matches.Length);
                    killer = matches[0];
                }

                if (killer == null)
                    return;

                if (ret.Player.Data.IsDead)
                    return;

                var br = new BodyReport
                {
                    Killer = Utils.PlayerById(killer.KillerId),
                    Body = Utils.PlayerById(killer.PlayerId),
                    KillAge = (float)(DateTime.UtcNow - killer.KillTime).TotalMilliseconds
                };

                var reportMsg = BodyReport.ParseBodyReport(br);

                if (string.IsNullOrWhiteSpace(reportMsg))
                    return;

                //Send the message through chat only visible to the Retributionist-Coroner
                if (HudManager.Instance)
                    HudManager.Instance.Chat.AddChat(ret.Player, reportMsg);
            }
            else if (ret.RevivedRole.RoleType == RoleEnum.Detective)
                EndGame.Reset();
            else if (ret.RevivedRole.RoleType == RoleEnum.Sheriff)
                ret.Interrogated.Clear();
            else if (ret.RevivedRole.RoleType == RoleEnum.Inspector)
                ret.Inspected.Clear();

            ret.RevivedRole = null;
        }
    }
}