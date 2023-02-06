using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.CamouflagerMod;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.ConsigliereMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class Update
    {
        public static string NameText(PlayerControl player, string str = "")
        {
            if (CamouflageUnCamouflage.IsCamoed)
            {
                if (MeetingHud.Instance && !CustomGameOptions.MeetingColourblind)
                    return player.name + str;

                return "";
            }

            return player.name + str;
        }

        [HarmonyPriority(Priority.Last)]
        private static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1 || PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null ||
                !PlayerControl.LocalPlayer.Is(RoleEnum.Consigliere))
                return;

            var consig = Role.GetRole<Consigliere>(PlayerControl.LocalPlayer);

            foreach (var playerId in consig.Investigated)
            {
                var player = Utils.PlayerById(playerId);
                var role = Role.GetRole(player);
                player.nameText().transform.localPosition = new Vector3(0f, 0.15f, -0.5f);
                player.nameText().color = CustomGameOptions.ConsigInfo == ConsigInfo.Role ? role.Color : role.FactionColor;
                player.nameText().text = NameText(player, CustomGameOptions.ConsigInfo == ConsigInfo.Role ? $"\n {role.Name}" : $"\n {role.FactionName}");
            }
        }
    }
}