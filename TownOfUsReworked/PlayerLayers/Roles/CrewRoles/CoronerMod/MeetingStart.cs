using System;
using System.Linq;
using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Objects;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.CoronerMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdReportDeadBody))]
    public static class MeetingStart
    {
        public static void Postfix([HarmonyArgument(0)] GameData.PlayerInfo info)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Coroner) || PlayerControl.LocalPlayer.Data.IsDead)
                return;

            if (info == null)
                return;

            var matches = Murder.KilledPlayers.Where(x => x.PlayerId == info.PlayerId).ToArray();
            DeadPlayer killer = null;

            if (matches.Length > 0)
                killer = matches[0];

            if (killer == null)
                return;

            Role.GetRole<Coroner>(PlayerControl.LocalPlayer).Reported.Add(info.PlayerId);

            var br = new BodyReport
            {
                Killer = Utils.PlayerById(killer.KillerId),
                Body = Utils.PlayerById(killer.PlayerId),
                KillAge = (float) (DateTime.UtcNow - killer.KillTime).TotalMilliseconds
            };

            var reportMsg = BodyReport.ParseBodyReport(br);

            if (string.IsNullOrWhiteSpace(reportMsg))
                return;

            //Only Coroner can see this
            if (HudManager.Instance)
                HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, reportMsg);
        }
    }
}