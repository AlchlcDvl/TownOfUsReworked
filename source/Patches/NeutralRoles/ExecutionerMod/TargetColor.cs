using HarmonyLib;
using Hazel;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;
using System.Collections.Generic;
using TownOfUs.Roles.Modifiers;

namespace TownOfUs.NeutralRoles.ExecutionerMod
{
    public enum OnTargetDead
    {
        Crew,
        Amnesiac,
        Survivor,
        Jester
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class TargetColor
    {
        private static void UpdateMeeting(MeetingHud __instance, Executioner role)
        {
            foreach (var player in __instance.playerStates)
                if (player.TargetPlayerId == role.target.PlayerId)
                    player.NameText.color = Color.black;
        }

        private static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Executioner)) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;

            var role = Role.GetRole<Executioner>(PlayerControl.LocalPlayer);

            if (MeetingHud.Instance != null) UpdateMeeting(MeetingHud.Instance, role);

            if (!CustomGameOptions.ExeKnowsTargetRole) role.target.nameText().color = Color.black;

            if (!role.target.Data.IsDead && !role.target.Data.Disconnected) return;
            if (role.TargetVotedOut) return;

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte)CustomRPC.ExecutionerToJester, SendOption.Reliable, -1);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);

            ExeToJes(PlayerControl.LocalPlayer);
        }

        public static void ExeToJes(PlayerControl player)
        {
            player.myTasks.RemoveAt(0);
            Role.RoleDictionary.Remove(player.PlayerId);

            if (CustomGameOptions.OnTargetDead == OnTargetDead.Jester)
            {
                var jester = new Jester(player);
                var task = new GameObject("JesterTask").AddComponent<ImportantTextTask>();
                task.transform.SetParent(player.transform, false);
                task.Text =
                    $"{jester.ColorString}Role: {jester.Name}\nYour target was killed. Now you have to get voted out!\nFake Tasks:";
                player.myTasks.Insert(0, task);
            }
            else if (CustomGameOptions.OnTargetDead == OnTargetDead.Amnesiac)
            {
                var amnesiac = new Amnesiac(player);
                var task = new GameObject("AmnesiacTask").AddComponent<ImportantTextTask>();
                task.transform.SetParent(player.transform, false);
                task.Text =
                    $"{amnesiac.ColorString}Role: {amnesiac.Name}\nYour target was killed. Now remember a new role!";
                player.myTasks.Insert(0, task);
            }
            else if (CustomGameOptions.OnTargetDead == OnTargetDead.Survivor)
            {
                var surv = new Survivor(player);
                var task = new GameObject("SurvivorTask").AddComponent<ImportantTextTask>();
                task.transform.SetParent(player.transform, false);
                task.Text =
                    $"{surv.ColorString}Role: {surv.Name}\nYour target was killed. Now you just need to live!";
                player.myTasks.Insert(0, task);
            }
            else
            {
                new Crewmate(player);
            }
        }
    }
}