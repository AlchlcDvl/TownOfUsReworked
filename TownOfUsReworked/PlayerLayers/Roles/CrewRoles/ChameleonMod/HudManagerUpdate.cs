using HarmonyLib;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.ChameleonMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1)
                return;

            if (PlayerControl.LocalPlayer == null)
                return;

            if (PlayerControl.LocalPlayer.Data == null)
                return;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Chameleon))
                return;

            var swoopButton = __instance.KillButton;
            var role = Role.GetRole<Chameleon>(PlayerControl.LocalPlayer);

            swoopButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled) && !MeetingHud.Instance &&
                !PlayerControl.LocalPlayer.Data.IsDead && GameStates.IsInGame);

            if (role.IsSwooped)
                swoopButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.SwoopDuration);
            else
                swoopButton.SetCoolDown(role.SwoopTimer(), CustomGameOptions.SwoopCooldown);

            var renderer = swoopButton.graphic;

            if (role.IsSwooped || !swoopButton.isCoolingDown)
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