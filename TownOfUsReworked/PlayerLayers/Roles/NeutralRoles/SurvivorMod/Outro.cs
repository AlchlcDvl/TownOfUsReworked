using System.Linq;
using HarmonyLib;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.SurvivorMod
{
    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
    public class Outro
    {
        public static void Postfix(EndGameManager __instance)
        {
            var role = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Survivor && Role.NBOnlyWin);

            if (role == null)
                return;

            PoolablePlayer[] array = Object.FindObjectsOfType<PoolablePlayer>();

            foreach (var player in array)
                player.NameText().text = role.ColorString + player.NameText().text + "</color>";

            __instance.BackgroundBar.material.color = Colors.Neutral;
            var text = Object.Instantiate(__instance.WinText);
            text.text = "Neutral Benigns Win!";
            text.color = Colors.Neutral;
            var pos = __instance.WinText.transform.localPosition;
            pos.y = 1.5f;
            text.transform.position = pos;
            text.text = $"<size=4>{text.text}</size>";
            SoundManager.Instance.PlaySound(TownOfUsReworked.NBWin, false, 1f);
        }
    }
}