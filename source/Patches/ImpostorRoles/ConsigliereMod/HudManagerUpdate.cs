using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.ConsigliereMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudManagerUpdate
    {
        public static Sprite RevealSprite => TownOfUs.Placeholder;
        
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Consigliere)) return;
            var role = Role.GetRole<Consigliere>(PlayerControl.LocalPlayer);

            if (role.RevealButton == null)
            {
                role.RevealButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.RevealButton.graphic.enabled = true;
                role.RevealButton.GetComponent<AspectPosition>().DistanceFromEdge = TownOfUs.ButtonPosition;
                role.RevealButton.gameObject.SetActive(false);
            }
            role.RevealButton.GetComponent<AspectPosition>().Update();
            role.RevealButton.graphic.sprite = RevealSprite;

            role.RevealButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);
            __instance.KillButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);
            role.RevealButton.SetCoolDown(role.ConsigliereTimer(), CustomGameOptions.ConsigCd);

            if (role.ClosestPlayer != null)
            {
                Utils.SetTarget(ref role.ClosestPlayer, role.RevealButton, GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance]);
            }
        }
    }
}
