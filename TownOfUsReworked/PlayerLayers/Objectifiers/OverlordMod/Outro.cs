using System.Linq;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Objectifiers.Objectifiers;

namespace TownOfUsReworked.PlayerLayers.Objectifiers.OverlordMod
{
    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
    public static class Outro
    {
        public static void Postfix(EndGameManager __instance)
        {
            var obj = Objectifier.AllObjectifiers.FirstOrDefault(x => x.ObjectifierType == ObjectifierEnum.Overlord && ((Overlord)x).OverlordWins);

            if (obj == null)
                return;

            PoolablePlayer[] array = Object.FindObjectsOfType<PoolablePlayer>();

            foreach (var player in array)
                player.NameText().text = obj.ColorString + player.NameText().text + "</color>";

            __instance.BackgroundBar.material.color = obj.Color;
            var text = Object.Instantiate(__instance.WinText);
            text.text = "Overlord Wins!";
            text.color = obj.Color;
            var pos = __instance.WinText.transform.localPosition;
            pos.y = 1.5f;
            text.transform.position = pos;
            text.text = $"<size=4>{text.text}</size>";
            
            try
            {
                SoundManager.Instance.PlaySound(TownOfUsReworked.WerewolfWin, false, 1f);
            } catch {}
        }
    }
}