using HarmonyLib;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.AgentMod
{
	[HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
	public class AddNumbers
    {
		public static void GenNumber(Agent role, PlayerVoteArea voteArea)
        {
			var targetId = voteArea.TargetPlayerId;
			var nameText = Object.Instantiate(voteArea.NameText, voteArea.transform);
			nameText.transform.localPosition = new Vector3(-1.211f, -0.18f, -0.1f);
			nameText.text = GameData.Instance.GetPlayerById(targetId).DefaultOutfit.ColorId.ToString();
			role.PlayerNumbers[targetId] = nameText;
		}

		public static void Postfix(MeetingHud __instance)
        {
			if (PlayerControl.LocalPlayer.Data.IsDead)
                return;

			if (!PlayerControl.LocalPlayer.Is(RoleEnum.Agent))
                return;

			foreach (var role in Role.GetRoles(RoleEnum.Agent))
            {
				var agent = (Agent)role;
				agent.PlayerNumbers.Clear();
			}

			var spyRole = Role.GetRole<Agent>(PlayerControl.LocalPlayer);

			foreach (var voteArea in __instance.playerStates)
				GenNumber(spyRole, voteArea);
		}
	}
}