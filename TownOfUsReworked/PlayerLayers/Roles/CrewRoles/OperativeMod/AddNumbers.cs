using HarmonyLib;
using TownOfUsReworked.Classes;
using UnityEngine;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.OperativeMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public static class AddNumbers
    {
        public static void GenNumber(Operative role, PlayerVoteArea voteArea)
        {
            var targetId = voteArea.TargetPlayerId;
            var nameText = Object.Instantiate(voteArea.NameText, voteArea.transform);
            nameText.transform.localPosition = new Vector3(-1.211f, -0.18f, -0.1f);
            nameText.text = GameData.Instance.GetPlayerById(targetId).DefaultOutfit.ColorId.ToString();
            role.PlayerNumbers[targetId] = nameText;
        }

        public static void Postfix(MeetingHud __instance)
        {
            foreach (var role2 in Role.GetRoles<Operative>(RoleEnum.Operative))
                role2.PlayerNumbers.Clear();

            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Operative))
                return;

            var role = Role.GetRole<Operative>(PlayerControl.LocalPlayer);

            foreach (var voteArea in __instance.playerStates)
                GenNumber(role, voteArea);
        }
    }
}