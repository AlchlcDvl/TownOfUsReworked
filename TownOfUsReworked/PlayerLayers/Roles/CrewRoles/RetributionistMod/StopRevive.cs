using HarmonyLib;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using System;
using System.Linq;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Patches;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.RetributionistMod.RevivedRoles.Coroner;
using TownOfUsReworked.PlayerLayers.Roles.AllRoles;
using Random = UnityEngine.Random;

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

            if (ret.RevivedRole?.RoleType == RoleEnum.Operative)
            {
                if (ret.BuggedPlayers.Count == 0)
                    DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "No one triggered your bugs.");
                else if (ret.BuggedPlayers.Count < CustomGameOptions.MinAmountOfPlayersInBug)
                    DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "Not enough players triggered your bugs.");
                else
                {
                    string message = "Roles caught in your bugs:\n";

                    foreach (RoleEnum role in ret.BuggedPlayers.OrderBy(x => Guid.NewGuid()))
                        message += $" {role},";

                    message.Remove(message.Length - 1, 1);
                    
                    //Ensures only the Retributionist-Operative sees this
                    if (DestroyableSingleton<HudManager>.Instance)
                        DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, message);
                }
            }
            else if (ret.RevivedRole?.RoleType == RoleEnum.Coroner)
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
                    Reporter = PlayerControl.LocalPlayer,
                    Body = Utils.PlayerById(killer.PlayerId),
                    KillAge = (float) (DateTime.UtcNow - killer.KillTime).TotalMilliseconds
                };

                var reportMsg = BodyReport.ParseBodyReport(br);

                if (string.IsNullOrWhiteSpace(reportMsg))
                    return;

                //Send the message through chat only visible to the Retributionist-Coroner
                if (DestroyableSingleton<HudManager>.Instance)
                    DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, reportMsg);
            }
            else if (ret.RevivedRole.RoleType == RoleEnum.Detective)
                EndGame.Reset();

            ret.RevivedRole = null;
        }
    }
}