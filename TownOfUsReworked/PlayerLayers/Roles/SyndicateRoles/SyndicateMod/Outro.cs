using System.Linq;
using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Enums;
using UnityEngine;
using Reactor.Utilities.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.SyndicateMod
{
    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
    public static class Outro
    {
        public static void Postfix(EndGameManager __instance)
        {
            var role = Role.AllRoles.FirstOrDefault(x => x.Faction == Faction.Syndicate && Role.SyndicateWin);

            if (role == null)
                return;

            PoolablePlayer[] array = Object.FindObjectsOfType<PoolablePlayer>();

            foreach (var player in array)
                player.NameText().text = Utils.GetEndGameName(player.NameText().text);

            __instance.BackgroundBar.material.color = role.FactionColor;
            var text = Object.Instantiate(__instance.WinText);
            text.text = "Syndicate Wins!";
            text.color = role.FactionColor;
            var pos = __instance.WinText.transform.localPosition;
            pos.y = 1.5f;
            text.transform.position = pos;
            text.text = $"<size=4>{text.text}</size>";
        }
    }
}