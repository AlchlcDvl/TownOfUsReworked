using HarmonyLib;
using Hazel;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Data;
using UnityEngine;
using System.Collections;
using Reactor.Utilities;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.PlayerLayers.Objectifiers;
using System.Linq;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.Patches
{
    //Thanks to twix for the location code
    [HarmonyPatch]
    public static class GameAnnouncements
    {
        private static string Location;
        private static bool GivingAnnouncements;

        [HarmonyPatch(typeof(RoomTracker), nameof(RoomTracker.FixedUpdate))]
        public static class Recordlocation
        {
            public static void Postfix(RoomTracker __instance)
            {
                if (__instance.text.transform.localPosition.y != -3.25f)
                    Location = __instance.text.text;
                else
                {
                    var name = PlayerControl.LocalPlayer.name;
                    Location = $"a hallway or somewhere outside, {name} where is the body?";
                }
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdReportDeadBody))]
        public static class Sendchat
        {
            public static void Postfix([HarmonyArgument(0)] GameData.PlayerInfo target) => Coroutines.Start(Announcements(target));

            private static IEnumerator Announcements(GameData.PlayerInfo target)
            {
                yield return new WaitForSeconds(5f);

                GivingAnnouncements = true;
                var extraTime = 0f;

                if (CustomGameOptions.GameAnnouncements)
                {
                    PlayerControl check = null;

                    if (target != null)
                    {
                        var player = target.Object;
                        check = player;
                        var report = $"{player.name} was found dead last round.";
                        HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, report);
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SendChat, SendOption.Reliable);
                        writer.Write(report);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);

                        yield return new WaitForSeconds(2f);

                        extraTime += 2;

                        if (CustomGameOptions.LocationReports)
                            report = $"Their body was found in {Location}.";
                        else
                            report = "It is unknown where they died.";

                        HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, report);
                        var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SendChat, SendOption.Reliable);
                        writer2.Write(report);
                        AmongUsClient.Instance.FinishRpcImmediately(writer2);

                        yield return new WaitForSeconds(2f);

                        extraTime += 2;
                        var killer = Utils.PlayerById(Murder.KilledPlayers.Find(x => x.PlayerId == player.PlayerId).KillerId);
                        var flag = killer.Is(RoleEnum.Altruist) || killer.Is(RoleEnum.Arsonist) || killer.Is(RoleEnum.Amnesiac) || killer.Is(RoleEnum.Executioner) ||
                            killer.Is(RoleEnum.Engineer) || killer.Is(RoleEnum.Escort) || killer.Is(RoleEnum.Impostor) || killer.Is(RoleEnum.Inspector) || killer.Is(RoleEnum.Operative) ||
                            killer.Is(RoleEnum.Eraser);
                        var a_an = flag ? "an" : "a";
                        var killerRole = Role.GetRole(killer);

                        if (CustomGameOptions.KillerReports == RoleFactionReports.Role)
                            report = $"They were killed by {a_an} {killerRole.Name}.";
                        else if (CustomGameOptions.KillerReports == RoleFactionReports.Faction)
                            report = $"They were killed by a member of the {killerRole.FactionName}.";
                        else
                            report = "They were killed by an unknown assailant.";

                        HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, report);
                        var writer3 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SendChat, SendOption.Reliable);
                        writer3.Write(report);
                        AmongUsClient.Instance.FinishRpcImmediately(writer3);

                        yield return new WaitForSeconds(2f);

                        extraTime += 2;
                        var flag2 = player.Is(RoleEnum.Altruist) || player.Is(RoleEnum.Arsonist) || player.Is(RoleEnum.Amnesiac) || player.Is(RoleEnum.Executioner) ||
                            player.Is(RoleEnum.Engineer) || player.Is(RoleEnum.Escort) || player.Is(RoleEnum.Impostor) || player.Is(RoleEnum.Inspector) || player.Is(RoleEnum.Operative) ||
                            player.Is(RoleEnum.Eraser);
                        var a_an2 = flag2 ? "an" : "a";
                        var role = Role.GetRole(player);

                        if (CustomGameOptions.RoleFactionReports == RoleFactionReports.Role)
                            report = $"They were {a_an2} {role.Name}.";
                        else if (CustomGameOptions.RoleFactionReports == RoleFactionReports.Faction)
                            report = $"They were from the {role.FactionName} faction.";
                        else
                            report = "It is unknown what they were.";

                        HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, report);
                        var writer4 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SendChat, SendOption.Reliable);
                        writer4.Write(report);
                        AmongUsClient.Instance.FinishRpcImmediately(writer4);
                    }
                    else
                    {
                        HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "A meeting has been called.");
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SendChat, SendOption.Reliable);
                        writer.Write("A meeting has been called.");
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }

                    yield return new WaitForSeconds(2f);
                    extraTime += 2;

                    foreach (var player in Utils.RecentlyKilled)
                    {
                        if (player != check)
                        {
                            var report = $"{player.name} was found dead last round.";
                            HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, report);
                            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SendChat, SendOption.Reliable);
                            writer.Write(report);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);

                            yield return new WaitForSeconds(2f);

                            extraTime += 2;

                            report = "It is unknown where they died.";
                            HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, report);
                            var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SendChat, SendOption.Reliable);
                            writer2.Write(report);
                            AmongUsClient.Instance.FinishRpcImmediately(writer2);

                            yield return new WaitForSeconds(2f);

                            extraTime += 2;

                            var killer = Utils.PlayerById(Murder.KilledPlayers.Find(x => x.PlayerId == player.PlayerId).KillerId);
                            var killerRole = Role.GetRole(killer);

                            if (Role.Cleaned.Contains(player))
                                report = "They were killed by an unknown assailant.";
                            else if (CustomGameOptions.KillerReports == RoleFactionReports.Role)
                                report = $"They were killed by a(n) {killerRole.Name}.";
                            else if (CustomGameOptions.KillerReports == RoleFactionReports.Faction)
                                report = $"They were killed by a member of the {killerRole.FactionName}.";
                            else
                                report = "They were killed by an unknown assailant.";

                            HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, report);
                            var writer3 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SendChat, SendOption.Reliable);
                            writer3.Write(report);
                            AmongUsClient.Instance.FinishRpcImmediately(writer3);

                            yield return new WaitForSeconds(2f);

                            extraTime += 2;

                            var role = Role.GetRole(player);

                            if (Role.Cleaned.Contains(player))
                                report = $"We could not determine what {player.name} was.";
                            else if (CustomGameOptions.RoleFactionReports == RoleFactionReports.Role)
                                report = $"They were a(n) {role.Name}.";
                            else if (CustomGameOptions.RoleFactionReports == RoleFactionReports.Faction)
                                report = $"They were from the {role.FactionName} faction.";
                            else
                                report = $"We could not determine what {player.name} was.";

                            HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, report);
                            var writer4 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SendChat, SendOption.Reliable);
                            writer4.Write(report);
                            AmongUsClient.Instance.FinishRpcImmediately(writer4);

                            yield return new WaitForSeconds(2f);

                            extraTime += 2;
                        }
                    }
                }

                var message = "";

                if (Role.ChaosDriveMeetingTimerCount < CustomGameOptions.ChaosDriveMeetingCount - 1)
                    message = $"{CustomGameOptions.ChaosDriveMeetingCount - Role.ChaosDriveMeetingTimerCount} meetings remain till the Syndicate gets their hands on the Chaos Drive!";
                else if (Role.ChaosDriveMeetingTimerCount == CustomGameOptions.ChaosDriveMeetingCount - 1)
                    message = "This is the last meeting before the Syndicate gets their hands on the Chaos Drive!";
                else if (Role.ChaosDriveMeetingTimerCount == CustomGameOptions.ChaosDriveMeetingCount)
                    message = "The Syndicate now possesses the Chaos Drive!";
                else
                    message = "The SYndicate possesses the Chaod Drive.";

                HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, message);
                var writer5 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SendChat, SendOption.Reliable);
                writer5.Write(message);
                AmongUsClient.Instance.FinishRpcImmediately(writer5);

                yield return new WaitForSeconds(2f);

                extraTime += 2;

                if (Objectifier.GetObjectifiers<Overlord>(ObjectifierEnum.Overlord).Any(x => x.IsAlive))
                {
                    if (MeetingPatches.MeetingCount == CustomGameOptions.OverlordMeetingWinCount - 1)
                        message = "This is the last meeting to find and kill the Overlord. Should you fail, you will all lose!";
                    else if (MeetingPatches.MeetingCount < CustomGameOptions.OverlordMeetingWinCount - 1)
                        message = "There seems to be an Overlord bent on dominating the mission! Kill them before they are successful!";

                    if (message != "")
                    {
                        HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, message);
                        var writer6 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SendChat, SendOption.Reliable);
                        writer6.Write(message);
                        AmongUsClient.Instance.FinishRpcImmediately(writer6);
                    }

                    yield return new WaitForSeconds(2f);

                    extraTime += 2;
                }
                else if (Objectifier.GetObjectifiers<Overlord>(ObjectifierEnum.Overlord).All(x => !x.IsAlive) && Objectifier.GetObjectifiers<Overlord>(ObjectifierEnum.Overlord).Count > 0)
                {
                    message = "All Overlords are dead!";
                    HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, message);
                    var writer6 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SendChat, SendOption.Reliable);
                    writer6.Write(message);
                    AmongUsClient.Instance.FinishRpcImmediately(writer6);

                    yield return new WaitForSeconds(2f);

                    extraTime += 2;
                }

                MeetingHud.Instance.discussionTimer += extraTime;
                Utils.RecentlyKilled.Clear();
                Role.Cleaned.Clear();
                GivingAnnouncements = false;
            }
        }

        [HarmonyPatch(typeof(TextBoxTMP), nameof(TextBoxTMP.SetText))]
        public static class StopChatting
        {
            public static bool Prefix() => PlayerControl.LocalPlayer.Data.IsDead || !GivingAnnouncements;
        }
    }
}