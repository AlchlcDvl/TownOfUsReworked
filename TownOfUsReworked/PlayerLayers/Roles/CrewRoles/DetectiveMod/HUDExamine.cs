using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.DetectiveMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class HUDExamine
    {
        public static void Postfix(PlayerControl __instance)
        {
            UpdateExamineButton(__instance);
        }

        public static void UpdateExamineButton(PlayerControl __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1)
                return;

            if (PlayerControl.LocalPlayer == null)
                return;

            if (PlayerControl.LocalPlayer.Data == null)
                return;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Detective))
                return;

            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;
            var examineButton = DestroyableSingleton<HudManager>.Instance.KillButton;
            var role = Role.GetRole<Detective>(PlayerControl.LocalPlayer);

            examineButton.gameObject.SetActive(!MeetingHud.Instance && !LobbyBehaviour.Instance && !isDead);
            examineButton.SetCoolDown(role.ExamineTimer(), CustomGameOptions.ExamineCd);
            Utils.SetTarget(ref role.ClosestPlayer, examineButton, float.NaN);

            var renderer = examineButton.graphic;
            
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