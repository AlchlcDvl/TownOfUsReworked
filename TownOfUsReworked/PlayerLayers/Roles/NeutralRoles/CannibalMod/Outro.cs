using System.Linq;
using HarmonyLib;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Objectifiers;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.PlayerLayers.Objectifiers.Objectifiers;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.CannibalMod
{
    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
    public class Outro
    {
        public static void Postfix(EndGameManager __instance)
        {
            if (Objectifier.GetObjectifiers(ObjectifierEnum.Taskmaster).Any(x => ((Taskmaster)x).WinTasksDone))
                return;

            if (Role.GetRoles(RoleEnum.Troll).Any(x => ((Troll)x).Killed))
                return;

            if (Objectifier.GetObjectifiers(ObjectifierEnum.Phantom).Any(x => ((Phantom)x).CompletedTasks))
                return;

            var role = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Cannibal && ((Cannibal) x).CannibalWin);
            if (role == null)
                return;

            PoolablePlayer[] array = Object.FindObjectsOfType<PoolablePlayer>();

            foreach (var player in array)
                player.NameText().text = role.ColorString + player.NameText().text + "</color>";

            __instance.BackgroundBar.material.color = role.Color;
            var text = Object.Instantiate(__instance.WinText);
            text.text = "Cannibal Wins!";
            text.color = role.Color;
            var pos = __instance.WinText.transform.localPosition;
            pos.y = 1.5f;
            text.transform.position = pos;
            text.text = $"<size=4>{text.text}</size>";
        }
    }
}