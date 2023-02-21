using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Linq;
using TownOfUsReworked.Patches;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using AmongUs.GameOptions;

namespace TownOfUsReworked.PlayerLayers.Objectifiers.TraitorMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class Betray
    {
        public static void Postfix(HudManager __instance)
        {
            
        }
    }
}