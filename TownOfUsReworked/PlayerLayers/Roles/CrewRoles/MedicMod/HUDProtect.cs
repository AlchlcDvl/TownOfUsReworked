using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class HUDProtect
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1)
                return;

            if (PlayerControl.LocalPlayer == null)
                return;

            if (PlayerControl.LocalPlayer.Data == null)
                return;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Medic))
                return;

            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;
            var protectButton = DestroyableSingleton<HudManager>.Instance.KillButton;
            var role = Role.GetRole<Medic>(PlayerControl.LocalPlayer);

            protectButton.gameObject.SetActive(!MeetingHud.Instance && !LobbyBehaviour.Instance && !isDead && !role.UsedAbility);
            protectButton.SetCoolDown(0f, 1f);
            Utils.SetTarget(ref role.ClosestPlayer, protectButton);
        }
    }
}
