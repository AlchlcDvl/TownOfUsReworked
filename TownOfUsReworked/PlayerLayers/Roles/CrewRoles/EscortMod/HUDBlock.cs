using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.EscortMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDBlock
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1)
                return;

            if (PlayerControl.LocalPlayer == null)
                return;

            if (PlayerControl.LocalPlayer.Data == null)
                return;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Escort))
                return;

            var isDead = PlayerControl.LocalPlayer.Data.IsDead;
            var roleblockButton = __instance.KillButton;
            var role = Role.GetRole<Escort>(PlayerControl.LocalPlayer);
            
            roleblockButton.gameObject.SetActive(!MeetingHud.Instance && !isDead && !LobbyBehaviour.Instance);
            roleblockButton.SetCoolDown(role.RoleblockTimer(), CustomGameOptions.ExamineCd);
            Utils.SetTarget(ref role.ClosestPlayer, roleblockButton);

            var renderer = roleblockButton.graphic;
            
            if (role.ClosestPlayer != null)
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
            }
        }
    }
}