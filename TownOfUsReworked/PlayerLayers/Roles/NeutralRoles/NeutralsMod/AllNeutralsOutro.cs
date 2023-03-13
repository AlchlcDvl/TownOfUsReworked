using System.Linq;
using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Enums;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.NeutralsMod
{
    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
    public class AllNeutralsOutro
    {
        public static void Postfix(EndGameManager __instance)
        {
            var role = Role.AllRoles.FirstOrDefault(x => x.Faction == Faction.Neutral && Role.AllNeutralsWin);

            if (role == null)
                return;

            PoolablePlayer[] array = Object.FindObjectsOfType<PoolablePlayer>();

            foreach (var player in array)
                player.NameText().text = Utils.GetEndGameName(player.NameText().text);

            __instance.BackgroundBar.material.color = Colors.Neutral;
            var text = Object.Instantiate(__instance.WinText);
            text.text = "Neutrals Win!";
            text.color = Colors.Neutral;
            var pos = __instance.WinText.transform.localPosition;
            pos.y = 1.5f;
            text.transform.position = pos;
            text.text = $"<size=4>{text.text}</size>";
        }
    }
}