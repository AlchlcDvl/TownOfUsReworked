using HarmonyLib;
using TMPro;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.Patches;

namespace TownOfUsReworked.PlayerLayers
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.RpcEndGame))]
    internal class Outro
    {
        public static void Postfix(EndGameManager __instance)
        {
            TextMeshPro text;
            Vector3 pos;

            if (Role.NobodyWins)
            {
                text = Object.Instantiate(__instance.WinText);
                __instance.BackgroundBar.material.color = Colors.Stalemate;
                text.color = Colors.Stalemate;
                text.text = "Stalemate!";
                pos = __instance.WinText.transform.localPosition;
                pos.y = 1.5f;
                text.transform.position = pos;
                text.text = $"<size=4>{text.text}</size>";
                return;
            }
        }
    }
}