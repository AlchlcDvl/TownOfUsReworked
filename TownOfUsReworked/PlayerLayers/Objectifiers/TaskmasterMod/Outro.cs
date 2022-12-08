using System.Linq;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.PlayerLayers.Objectifiers.Objectifiers;
using TownOfUsReworked.PlayerLayers.Roles;

namespace TownOfUsReworked.PlayerLayers.Objectifiers.TaskmasterMod
{
    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
    public static class Outro
    {
        public static void Postfix(EndGameManager __instance)
        {
            if (Role.GetRoles(RoleEnum.Cannibal).Any(x => ((Cannibal)x).EatNeed == 0))
                return;

            if (Role.GetRoles(RoleEnum.Troll).Any(x => ((Troll)x).Killed))
                return;

            if (Objectifier.GetObjectifiers(ObjectifierEnum.Phantom).Any(x => ((Phantom)x).CompletedTasks))
                return;
            
            var objectifier = Objectifier.AllObjectifiers.FirstOrDefault(x => x.ObjectifierType == ObjectifierEnum.Taskmaster && ((Taskmaster)x).WinTasksDone);

            if (objectifier == null)
                return;

            PoolablePlayer[] array = Object.FindObjectsOfType<PoolablePlayer>();
            array[0].NameText().text = objectifier.ColorString + array[0].NameText().text + "</color>";
            __instance.BackgroundBar.material.color = objectifier.Color;
            var text = Object.Instantiate(__instance.WinText);
            text.text = "Taskmaster Wins!";
            text.color = objectifier.Color;
            var pos = __instance.WinText.transform.localPosition;
            pos.y = 1.5f;
            text.transform.position = pos;
            text.text = $"<size=4>{text.text}</size>";
            //SoundManager.Instance.PlaySound(TownOfUsReworked.TaskmasterWin, false, 0.3f);
        }
    }
}