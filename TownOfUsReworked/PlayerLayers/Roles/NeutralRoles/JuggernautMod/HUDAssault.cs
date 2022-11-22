using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.JuggernautMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDAssault
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1)
                return;

            if (PlayerControl.LocalPlayer == null)
                return;

            if (PlayerControl.LocalPlayer.Data == null)
                return;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Juggernaut))
                return;

            var isDead = PlayerControl.LocalPlayer.Data.IsDead;

            if (isDead)
                return;

            var role = Role.GetRole<Juggernaut>(PlayerControl.LocalPlayer);
            __instance.KillButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);
            __instance.KillButton.SetCoolDown(role.KillTimer(), CustomGameOptions.JuggKillCooldown + 5.0f - CustomGameOptions.JuggKillBonus *
                role.JuggKills);

            Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton);
        }
    }
}