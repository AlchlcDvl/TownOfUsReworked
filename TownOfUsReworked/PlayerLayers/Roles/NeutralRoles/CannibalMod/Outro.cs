using System.Linq;
using HarmonyLib;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Enums;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using Reactor.Utilities.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.CannibalMod
{
    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
    public static class Outro
    {
        public static void Postfix(EndGameManager __instance)
        {
            var role = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Cannibal && ((Cannibal)x).CannibalWin);

            if (role == null)
                return;

            PoolablePlayer[] array = Object.FindObjectsOfType<PoolablePlayer>();

            foreach (var player in array)
                player.NameText().text = "<color=#" + Color.white.ToHtmlStringRGBA() + ">" + player.NameText().text + "</color>";

            __instance.BackgroundBar.material.color = role.Color;
            var text = Object.Instantiate(__instance.WinText);
            text.text = "Cannibal Wins!";
            text.color = role.Color;
            var pos = __instance.WinText.transform.localPosition;
            pos.y = 1.5f;
            text.transform.position = pos;
            text.text = $"<size=4>{text.text}</size>";
        }
    }
}