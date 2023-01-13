using System.Linq;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Objectifiers.Objectifiers;

namespace TownOfUsReworked.PlayerLayers.Objectifiers.RivalsMod
{
    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
    public static class Outro
    {
        public static void Postfix(EndGameManager __instance)
        {
            var obj = Objectifier.AllObjectifiers.FirstOrDefault(x => x.ObjectifierType == ObjectifierEnum.Rivals && ((Rivals)x).RivalWins);

            if (obj == null)
                return;

            PoolablePlayer[] array = Object.FindObjectsOfType<PoolablePlayer>();

            foreach (var player in array)
                player.NameText().text = obj.ColorString + player.NameText().text + "</color>";

            __instance.BackgroundBar.material.color = obj.Color;
            var text = Object.Instantiate(__instance.WinText);
            text.text = "Rival Wins!";
            text.color = obj.Color;
            var pos = __instance.WinText.transform.localPosition;
            pos.y = 1.5f;
            text.transform.position = pos;
            text.text = $"<size=4>{text.text}</size>";
        }
    }
}