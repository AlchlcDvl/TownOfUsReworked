using HarmonyLib;
using UnityEngine;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
    public static class PingTracker_Update
    {
        [HarmonyPostfix]
        public static void Postfix(PingTracker __instance)
        {
            var position = __instance.GetComponent<AspectPosition>();
            position.DistanceFromEdge = new Vector3(3.6f, 0.1f, 0);
            position.AdjustPosition();

            __instance.text.text = "<color=#00FF00FF>TownOfUs</color><color=#FF00FFFF>Reworked</color>\n<color=#0000FFFF>v1.0.0-dev8_test</color>\n" +
                $"Ping: {AmongUsClient.Instance.Ping}ms\n" +
                (!MeetingHud.Instance ?
                    "<color=#FF00FFFF>Reworked By: Alcoholic Devil</color>\n" +
                    "<color=#FFFFFFFF>With Help (And Code) From: Oper,</color>\n" +
                    "<color=#FFFFFFFF>xxOm3ga77xx, Discussions, Detective,</color>\n" +
                    "<color=#FFFFFFFF>& -H</color>\n" : "") +
                (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started ?
                    "<color=#FF0000FF>Made By: Slushiegoose</color>\n" +
                    "<color=#6AA84FFF>Continued By: Polus.gg</color>\n" +
                    "<color=#00FFFFFF>Reactivated By: Donners, Term, -H &</color>\n" +
                    "<color=#00FFFFFF>MyDragonBreath</color>" : "");
        }
    }
}
