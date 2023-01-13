using System.Linq;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.SheriffMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class HUDInterrogate
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1)
                return;

            if (PlayerControl.LocalPlayer == null)
                return;

            if (PlayerControl.LocalPlayer.Data == null)
                return;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Sheriff))
                return;

            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;
            var investigateButton = DestroyableSingleton<HudManager>.Instance.KillButton;
            var role = Role.GetRole<Sheriff>(PlayerControl.LocalPlayer);

            investigateButton.gameObject.SetActive(!MeetingHud.Instance && !LobbyBehaviour.Instance && !isDead);
            investigateButton.SetCoolDown(role.InterrogateTimer(), CustomGameOptions.InterrogateCd);
            var notInvestigated = PlayerControl.AllPlayerControls.ToArray().Where(x => !role.Interrogated.Contains(x.PlayerId)).ToList();
            Utils.SetTarget(ref role.ClosestPlayer, investigateButton, float.NaN, notInvestigated);
        }
    }
}
