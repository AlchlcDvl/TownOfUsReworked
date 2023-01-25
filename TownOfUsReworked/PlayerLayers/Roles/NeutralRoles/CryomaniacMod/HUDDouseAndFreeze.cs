using System.Linq;
using HarmonyLib;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.CryomaniacMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDDouseAndFreeze
    {
        public static Sprite IgniteSprite => TownOfUsReworked.Placeholder;
        
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1 || PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null || !PlayerControl.LocalPlayer.Is(RoleEnum.Cryomaniac))
                return;

            var role = Role.GetRole<Cryomaniac>(PlayerControl.LocalPlayer);

            foreach (var playerId in role.DousedPlayers)
            {
                var player = Utils.PlayerById(playerId);
                var data = player?.Data;

                if (data == null || data.Disconnected || data.IsDead || PlayerControl.LocalPlayer.Data.IsDead)
                    continue;

                player.myRend().material.SetColor("_VisorColor", role.Color);
                player.nameText().color = Color.black;
            }

            if (role.FreezeButton == null)
            {
                role.FreezeButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.FreezeButton.graphic.enabled = true;
                role.FreezeButton.GetComponent<AspectPosition>().DistanceFromEdge = TownOfUsReworked.BelowVentPosition;
                role.FreezeButton.gameObject.SetActive(false);
            }

            role.FreezeButton.GetComponent<AspectPosition>().Update();
            role.FreezeButton.graphic.sprite = IgniteSprite;

            role.FreezeButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance && !LobbyBehaviour.Instance);
            __instance.KillButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance && !LobbyBehaviour.Instance);
            role.FreezeButton.SetCoolDown(0f, 1f);
            __instance.KillButton.SetCoolDown(role.DouseTimer(), CustomGameOptions.DouseCd);

            var notDoused = PlayerControl.AllPlayerControls.ToArray().Where(player => !role.DousedPlayers.Contains(player.PlayerId)).ToList();

            Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, notDoused);

            if (!role.FreezeButton.isCoolingDown && role.FreezeButton.isActiveAndEnabled && !role.FreezeUsed)
            {
                role.FreezeButton.graphic.color = Palette.EnabledColor;
                role.FreezeButton.graphic.material.SetFloat("_Desat", 0f);
            }
            else
            {
                role.FreezeButton.graphic.color = Palette.DisabledClear;
                role.FreezeButton.graphic.material.SetFloat("_Desat", 1f);
            }
        }
    }
}
