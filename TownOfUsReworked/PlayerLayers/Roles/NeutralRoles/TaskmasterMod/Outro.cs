using System.Linq;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.PlayerLayers.Objectifiers.Objectifiers;
using TownOfUsReworked.PlayerLayers.Objectifiers;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.TaskmasterMod
{
    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
    public static class Outro
    {
        public static void Postfix(EndGameManager __instance)
        {
            if (Role.GetRoles(RoleEnum.Cannibal).Any(x => ((Cannibal)x).EatNeed == 0))
                return;

            if (Objectifier.GetObjectifiers(ObjectifierEnum.Phantom).Any(x => ((Phantom)x).CompletedTasks))
                return;
            
            var role = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Taskmaster && ((Taskmaster)x).WinTasksDone);

            if (role == null)
                return;

            PoolablePlayer[] array = Object.FindObjectsOfType<PoolablePlayer>();
            array[0].NameText().text = role.ColorString + array[0].NameText().text + "</color>";
            __instance.BackgroundBar.material.color = role.Color;
            var text = Object.Instantiate(__instance.WinText);
            text.text = "Taskmaster Wins!";
            text.color = role.Color;
            var pos = __instance.WinText.transform.localPosition;
            pos.y = 1.5f;
            text.transform.position = pos;
            text.text = $"<size=4>{text.text}</size>";
            //SoundManager.Instance.PlaySound(TownOfUsReworked.TaskmasterWin, false, 0.3f);
        }
    }
}