using System.Linq;
using HarmonyLib;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.GorgonMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDGaze
    {
        public static Sprite Gaze => TownOfUsReworked.Placeholder;

        public static void Postfix(HudManager __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Gorgon))
                return;

            if (PlayerControl.AllPlayerControls.Count <= 1)
                return;

            if (PlayerControl.LocalPlayer == null)
                return;

            if (PlayerControl.LocalPlayer.Data == null)
                return;

            var role = Role.GetRole<Gorgon>(PlayerControl.LocalPlayer);

            if (role.GazeButton == null)
            {
                role.GazeButton = Object.Instantiate(__instance.KillButton, HudManager.Instance.transform);
                role.GazeButton.graphic.enabled = true;
                role.GazeButton.graphic.sprite = Gaze;
            }

            role.GazeButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance && !LobbyBehaviour.Instance);
            var notImpostor = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Syndicate)).ToList();
            Utils.SetTarget(ref role.ClosestPlayer, role.GazeButton, float.NaN, notImpostor);
            
            if (role.ClosestPlayer != null)
            {
                role.GazeButton.graphic.color = Palette.EnabledColor;
                role.GazeButton.graphic.material.SetFloat("_Desat", 0f);
            }
            else
            {
                role.GazeButton.graphic.color = Palette.DisabledClear;
                role.GazeButton.graphic.material.SetFloat("_Desat", 1.0f);
            }
        }
    }
}