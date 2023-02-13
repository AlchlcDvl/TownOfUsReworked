using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles.AllRoles
{
    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
    internal class Outro
    {
        public static void Postfix(EndGameManager __instance)
        {
            if (!Role.NobodyWins)
                return;

            __instance.BackgroundBar.material.color = Colors.Stalemate;
            var text = Object.Instantiate(__instance.WinText);
            text.text = "Stalemate!";
            text.color = Colors.Stalemate;
            var pos = __instance.WinText.transform.localPosition;
            pos.y = 1.5f;
            text.transform.position = pos;
            text.text = $"<size=4>{text.text}</size>";
        }
    }
}