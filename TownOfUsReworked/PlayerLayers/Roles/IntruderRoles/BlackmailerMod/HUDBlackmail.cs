using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using UnityEngine;
using System.Linq;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.BlackmailerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDBlackmail
    {
        public static Sprite Blackmail => TownOfUsReworked.BlackmailSprite;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.CannotUseButton(PlayerControl.LocalPlayer, RoleEnum.Blackmailer))
                return;

            var role = Role.GetRole<Blackmailer>(PlayerControl.LocalPlayer);

            if (role.BlackmailButton == null)
            {
                role.BlackmailButton = Object.Instantiate(__instance.KillButton, __instance.UseButton.transform.parent);
                role.BlackmailButton.graphic.enabled = true;
                role.BlackmailButton.GetComponent<AspectPosition>().DistanceFromEdge = TownOfUsReworked.BelowVentPosition;
                role.BlackmailButton.gameObject.SetActive(false);
            }

            if (role.KillButton == null)
            {
                role.KillButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.KillButton.graphic.enabled = true;
            }

            var notImp = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Intruder)).ToList();

            if (role.IsRecruit)
                notImp = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(SubFaction.Cabal)).ToList();

            role.BlackmailButton.GetComponent<AspectPosition>().Update();
            role.BlackmailButton.graphic.sprite = Blackmail;
            role.BlackmailButton.SetCoolDown(role.BlackmailTimer(), CustomGameOptions.BlackmailCd);
            role.BlackmailButton.gameObject.SetActive(Utils.SetActive(PlayerControl.LocalPlayer));
            role.KillButton.gameObject.SetActive(Utils.SetActive(PlayerControl.LocalPlayer));
            role.KillButton.SetCoolDown(role.KillTimer(), CustomGameOptions.IntKillCooldown);
            var notBlackmailed = PlayerControl.AllPlayerControls.ToArray().Where(player => role.Blackmailed?.PlayerId != player.PlayerId).ToList();
            Utils.SetTarget(ref role.ClosestPlayer, role.BlackmailButton, notBlackmailed);
            Utils.SetTarget(ref role.ClosestPlayer, role.KillButton, notImp);

            if (role.Blackmailed != null && !role.Blackmailed.Data.IsDead && !role.Blackmailed.Data.Disconnected)
            {
                role.Blackmailed.myRend().material.SetFloat("_Outline", 1f);
                role.Blackmailed.myRend().material.SetColor("_OutlineColor", role.Color);

                if (role.Blackmailed.GetCustomOutfitType() != CustomPlayerOutfitType.Camouflage && role.Blackmailed.GetCustomOutfitType() !=
                    CustomPlayerOutfitType.Invis)
                    role.Blackmailed.nameText().color = role.Color;
                else
                    role.Blackmailed.nameText().color = Color.clear;
            }

            var imps = PlayerControl.AllPlayerControls.ToArray().Where(player => player.Data.IsImpostor() && player != role.Blackmailed).ToList();

            foreach (var imp in imps)
            {
                if ((imp.GetCustomOutfitType() == CustomPlayerOutfitType.Camouflage || imp.GetCustomOutfitType() == CustomPlayerOutfitType.Invis) && imp.nameText().color == role.Color)
                    imp.nameText().color = Color.clear;
                else if (imp.GetCustomOutfitType() != CustomPlayerOutfitType.Camouflage && imp.GetCustomOutfitType() != CustomPlayerOutfitType.Invis && imp.nameText().color == Color.clear)
                    imp.nameText().color = role.Color;
            }
        }
    }
}