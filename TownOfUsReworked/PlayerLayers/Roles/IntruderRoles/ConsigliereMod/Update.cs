using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.CamouflagerMod;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.ConsigliereMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class Update
    {
        public static string NameText(PlayerControl player, string str = "", bool meeting = false)
        {
            if (CamouflageUnCamouflage.IsCamoed)
            {
                if (meeting && !CustomGameOptions.MeetingColourblind)
                    return player.name + str;

                return "";
            }

            return player.name + str;
        }

        [HarmonyPriority(Priority.Last)]
        private static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1)
                return;

            if (PlayerControl.LocalPlayer == null)
                return;

            if (PlayerControl.LocalPlayer.Data == null)
                return;

            var consig = Role.GetRole<Consigliere>(PlayerControl.LocalPlayer);

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Seer))
                return;

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (!consig.Investigated.Contains(player.PlayerId))
                    continue;

                var roleType = Utils.GetRole(player);
                var role = Role.GetRole(player);
                player.nameText().transform.localPosition = new Vector3(0f, 2f, -0.5f);
                player.nameText().color = CustomGameOptions.ConsigInfo == ConsigInfo.Role ? role.Color : role.FactionColor;
                player.nameText().text = NameText(player, CustomGameOptions.ConsigInfo == ConsigInfo.Role ? $"\n ({role.Name})" : $"\n ({role.FactionName})", true);
            }
        }
    }
}