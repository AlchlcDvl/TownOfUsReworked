using System.Linq;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Objectifiers.TaskmasterMod
{
    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
    public static class Outro
    {
        public static void Postfix(EndGameManager __instance)
        {
            var objectifier = Objectifier.AllObjectifiers.FirstOrDefault(x => x.ObjectifierType == ObjectifierEnum.Taskmaster && ((Taskmaster)x).WinTasksDone);

            if (objectifier == null)
                return;

            PoolablePlayer[] array = Object.FindObjectsOfType<PoolablePlayer>();

            foreach (var player in array)
                player.NameText().text = Utils.GetEndGameName(player.NameText().text);

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