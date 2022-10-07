using System;
using HarmonyLib;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.MysticMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MeetingHud_Start
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Mystic) || PlayerControl.LocalPlayer.Data.IsDead)
                return;
            
            var matches = Murder.KilledPlayers.ToArray();

            foreach (var match in matches)
            {
                if (match != null)
                {
                    var br = new BodyReport
                    {
                        Killer = Utils.PlayerById(match.KillerId),
                        Reporter = PlayerControl.LocalPlayer,
                        Body = Utils.PlayerById(match.PlayerId),
                        KillAge = (float) (DateTime.UtcNow - match.KillTime).TotalMilliseconds
                    };
                    var reportMsg = BodyReport.ParseBodyReport(br);
                    if (!string.IsNullOrWhiteSpace(reportMsg))
                    {
                        if (DestroyableSingleton<HudManager>.Instance) {
                            // Send the message through chat only visible to the Mystic
                            DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, reportMsg);
                        }
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdReportDeadBody))]
    public class DeadInfo
    {
        private static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] GameData.PlayerInfo info)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Mystic)) return;

            Role.GetRole<Mystic>(PlayerControl.LocalPlayer).Reported.Add(info.PlayerId);
        }
    }
}