/*using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.LocalPlayer))]
    public class DevTag
    {
        public static void Prefix(PlayerControl __instance, PlayerControl role)
        {
            string rname = PlayerControl.LocalPlayer.Data.PlayerName;
            string fontSize = "1.5";
            var role2 = PlayerControl.LocalPlayer;
            var roleVal = Role.GetRole(role2);
            var roleColor = roleVal.Color;
            string dev = $"<size={fontSize}>{roleColor}" + "Dev" + "</size>";
            string name = dev + "\r\n" + rname;

            if (role.FriendCode == "goingledge#5497")
            {
                PlayerControl.LocalPlayer.RpcSetName("<color=#" + roleColor + $">{name}</color>");
            }
        }
    }
}*/