using System.Linq;
using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Enums;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.UndeadMod
{
    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
    public static class Outro
    {
        public static void Postfix(EndGameManager __instance)
        {
            var role = Role.AllRoles.Find(x => x.SubFaction == SubFaction.Undead && Role.UndeadWin);

            if (role == null)
                return;

            foreach (var player in Object.FindObjectsOfType<PoolablePlayer>())
                player.NameText().text = Utils.GetEndGameName(player.NameText().text);

            __instance.BackgroundBar.material.color = Colors.Undead;
            var text = Object.Instantiate(__instance.WinText);
            text.text = "The Undead Win!";
            text.color = Colors.Undead;
            var pos = __instance.WinText.transform.localPosition;
            pos.y = 1.5f;
            text.transform.position = pos;
            text.text = $"<size=4>{text.text}</size>";
        }
    }
}