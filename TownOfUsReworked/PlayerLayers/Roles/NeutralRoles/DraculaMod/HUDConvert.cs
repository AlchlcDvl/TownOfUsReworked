using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;


namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.DraculaMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class HUDShift
    {
        public static void Postfix(PlayerControl __instance)
        {
            UpdateConvertButton(__instance);
        }

        public static void UpdateConvertButton(PlayerControl __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1)
                return;

            if (PlayerControl.LocalPlayer == null)
                return;

            if (PlayerControl.LocalPlayer.Data == null)
                return;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Dracula))
                return;

            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;
            var maxDistance = GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance];
            var biteButton = DestroyableSingleton<HudManager>.Instance.KillButton;

            var role = Role.GetRole<Dracula>(PlayerControl.LocalPlayer);

            if (isDead)
                biteButton.gameObject.SetActive(false);
            else
            {
                biteButton.gameObject.SetActive(!MeetingHud.Instance);
                biteButton.SetCoolDown(role.ConvertTimer(), CustomGameOptions.BiteCd);

                Utils.SetTarget(ref role.ClosestPlayer, biteButton);
            }
        }
    }
}
