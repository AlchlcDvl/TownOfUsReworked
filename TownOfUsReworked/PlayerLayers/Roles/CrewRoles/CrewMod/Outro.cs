using System.Linq;
using HarmonyLib;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Enums;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.CrewMod
{
    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
    public class Outro
    {
        public static void Postfix(EndGameManager __instance)
        {
            var role = Role.AllRoles.FirstOrDefault(x => x.Faction == Faction.Crew && Role.CrewWin);

            if (role == null)
                return;

            PoolablePlayer[] array = Object.FindObjectsOfType<PoolablePlayer>();

            foreach (var player in array)
                player.NameText().text = role.ColorString + player.NameText().text + "</color>";

            __instance.BackgroundBar.material.color = role.FactionColor;
            var text = Object.Instantiate(__instance.WinText);
            text.text = "Crew Wins!";
            text.color = role.FactionColor;
            var pos = __instance.WinText.transform.localPosition;
            pos.y = 1.5f;
            text.transform.position = pos;
            text.text = $"<size=4>{text.text}</size>";
            
            SoundManager.Instance.StopAllSound();

            try
            {
                SoundManager.Instance.PlaySound(TownOfUsReworked.CrewWin, false, 1f);
            } catch {}
        }
    }
}