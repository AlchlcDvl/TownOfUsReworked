using HarmonyLib;
using System.Linq;
using Hazel;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.BansheeMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class RepickBanshee
    {
        public static void Postfix()
        {
            if (PlayerControl.AllPlayerControls.Count <= 1 || PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null)
                return;

            var toChooseFrom = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Syndicate) && x.Data.IsDead && !x.Data.Disconnected).ToList();

            if (toChooseFrom.Count == 0)
                return;

            var rand = Random.RandomRangeInt(0, toChooseFrom.Count);
            var pc = toChooseFrom[rand];
            SetBanshee.WillBeBanshee = pc;
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetBanshee, SendOption.Reliable);
            writer.Write(pc.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }
    }
}