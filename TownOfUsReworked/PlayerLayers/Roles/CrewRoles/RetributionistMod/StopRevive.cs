using HarmonyLib;
using TownOfUsReworked.Classes;
using System;
using System.Linq;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Patches;
using TownOfUsReworked.PlayerLayers.Roles.AllRoles;
using Random = UnityEngine.Random;
using TownOfUsReworked.Objects;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.RetributionistMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public static class StopRevive
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Retributionist) || PlayerControl.LocalPlayer.Data.IsDead)
                return;

            var ret = Role.GetRole<Retributionist>(PlayerControl.LocalPlayer);

            if (ret.RevivedRole == null)
                return;

            if (ret.RevivedRole.RoleType == RoleEnum.Operative)
            {
                if (ret.BuggedPlayers.Count == 0)
                    return;

                var message = "The following role(s) triggered your bug:\n";

                if (ret.BuggedPlayers.Count > 1)
                {
                    var position = 0;

                    foreach (var role in ret.BuggedPlayers.OrderBy(_ => Guid.NewGuid()))
                    {
                        if (position < ret.BuggedPlayers.Count - 1)
                            message += $" {role},";
                        else
                            message += $" and {role}.";

                        position++;
                    }
                }
                else
                    message += $" {ret.BuggedPlayers[0]}";

                //Ensures only the Retributionist-Operative sees this
                if (HudManager.Instance)
                    HudManager.Instance.Chat.AddChat(ret.Player, message);
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

            ret.RevivedRole = null;
        }
    }
}