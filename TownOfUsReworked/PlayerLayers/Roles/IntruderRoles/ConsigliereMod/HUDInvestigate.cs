using System.Linq;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.ConsigliereMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class HUDInvestigate
    {
        public static void Postfix(PlayerControl __instance)
        {
            UpdateInvButton(__instance);
        }

        public static void UpdateInvButton(PlayerControl __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1)
                return;

            if (PlayerControl.LocalPlayer == null)
                return;

            if (PlayerControl.LocalPlayer.Data == null)
                return;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Consigliere))
                return;

            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;
            var role = Role.GetRole<Consigliere>(PlayerControl.LocalPlayer);

            if (isDead)
                return;
            
            role.InvestigateButton.gameObject.SetActive(!MeetingHud.Instance);
            role.InvestigateButton.SetCoolDown(role.ConsigliereTimer(), CustomGameOptions.ConsigCd);
            var notInvestigated = PlayerControl.AllPlayerControls.ToArray().Where(x => !role.Investigated.Contains(x.PlayerId)).ToList();
            Utils.SetTarget(ref role.ClosestPlayer, role.InvestigateButton, float.NaN, notInvestigated);
        }
    }
}
