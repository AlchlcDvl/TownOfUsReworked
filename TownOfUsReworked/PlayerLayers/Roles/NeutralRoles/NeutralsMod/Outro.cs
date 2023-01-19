using System.Linq;
using HarmonyLib;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.NeutralsMod
{
    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
    public class Outro
    {
        public static void Postfix(EndGameManager __instance)
        {
            var role = Role.AllRoles.FirstOrDefault(x => (x.Faction == Faction.Neutral && Role.NeutralsWin));

            if (role == null)
                return;

            PoolablePlayer[] array = Object.FindObjectsOfType<PoolablePlayer>();

            foreach (var player in array)
                player.NameText().text = player.name.GetEndGameName();

            __instance.BackgroundBar.material.color = Colors.Neutral;
            var text = Object.Instantiate(__instance.WinText);
            text.text = "Neutrals Win!";
            text.color = Colors.Neutral;
            var pos = __instance.WinText.transform.localPosition;
            pos.y = 1.5f;
            text.transform.position = pos;
            text.text = $"<size=4>{text.text}</size>";
            
            try
            {
                SoundManager.Instance.PlaySound(TownOfUsReworked.NeutralsWin, false, 1f);
            } catch {}
        }
    }
}