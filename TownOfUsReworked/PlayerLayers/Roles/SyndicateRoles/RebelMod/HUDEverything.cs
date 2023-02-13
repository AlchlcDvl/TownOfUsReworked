using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using System.Linq;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.RebelMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDEverything
    {
        public static Sprite Promote => TownOfUsReworked.Placeholder;
        public static Sprite Conceal => TownOfUsReworked.Placeholder;
        public static Sprite Kill => TownOfUsReworked.SyndicateKill;
        public static Sprite PoisonSprite => TownOfUsReworked.PoisonSprite;
        public static Sprite PoisonedSprite => TownOfUsReworked.PoisonedSprite;
        public static Sprite Gaze => TownOfUsReworked.Placeholder;
        public static Sprite Shapeshift => TownOfUsReworked.Placeholder;
        public static Sprite Warp => TownOfUsReworked.WarpSprite;
        public static Sprite FrameSprite => TownOfUsReworked.Placeholder;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Rebel))
                return;

            var role = Role.GetRole<Rebel>(PlayerControl.LocalPlayer);

            if (role.KillButton == null)
            {
                role.KillButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.KillButton.graphic.enabled = true;
                role.KillButton.graphic.sprite = Kill;
                role.KillButton.gameObject.SetActive(false);
            }

            var notImp = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Syndicate)).ToList();
            role.KillButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance) && Role.SyndicateHasChaosDrive);
            role.KillButton.SetCoolDown(role.KillTimer(), CustomGameOptions.ChaosDriveKillCooldown);
            Utils.SetTarget(ref role.ClosestPlayer, role.KillButton, notImp);
            var renderer2 = role.KillButton.graphic;
            
            if (role.ClosestPlayer != null && !role.KillButton.isCoolingDown)
            {
                renderer2.color = Palette.EnabledColor;
                renderer2.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer2.color = Palette.DisabledClear;
                renderer2.material.SetFloat("_Desat", 1f);
            }

            if (!role.HasDeclared)
            {
                if (role.DeclareButton == null)
                {
                    role.DeclareButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                    role.DeclareButton.graphic.enabled = true;
                    role.DeclareButton.graphic.sprite = Promote;
                    role.DeclareButton.gameObject.SetActive(false);
                }

                var Imp = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Syndicate)).ToList();
                role.DeclareButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance) && !role.HasDeclared);
                Utils.SetTarget(ref role.ClosestPlayer, role.DeclareButton, Imp);

                var renderer = role.DeclareButton.graphic;
                
                if (role.ClosestPlayer != null && !role.DeclareButton.isCoolingDown)
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

            if (role.FormerRole == null || !role.WasSidekick || role.FormerRole?.RoleType == RoleEnum.Anarchist)
                return;

            var formerRole = role.FormerRole.RoleType;

            if (formerRole == RoleEnum.Concealer)
            {
                if (role.ConcealButton == null)
                {
                    role.ConcealButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                    role.ConcealButton.graphic.enabled = true;
                    role.ConcealButton.graphic.sprite = Conceal;
                    role.ConcealButton.gameObject.SetActive(false);
                }

                role.ConcealButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));

                if (role.ConcealEnabled)
                    role.ConcealButton.SetCoolDown(role.ConcealTimeRemaining, CustomGameOptions.ConcealDuration);
                else 
                    role.ConcealButton.SetCoolDown(role.ConcealTimer(), CustomGameOptions.ConcealCooldown);

                var renderer = role.ConcealButton.graphic;
                
                if (!role.ConcealButton.isCoolingDown && !role.Concealed)
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
            else if (formerRole == RoleEnum.Framer)
            {
                if (role.FrameButton == null)
                {
                    role.FrameButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                    role.FrameButton.graphic.enabled = true;
                    role.FrameButton.graphic.sprite = FrameSprite;
                    role.FrameButton.gameObject.SetActive(false);
                }

                role.FrameButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));
                var notFramed = PlayerControl.AllPlayerControls.ToArray().Where(x => !role.Framed.Contains(x.PlayerId) && !x.Is(Faction.Syndicate)).ToList();
                Utils.SetTarget(ref role.ClosestPlayer, role.FrameButton, notFramed);
                role.FrameButton.SetCoolDown(role.FrameTimer(), CustomGameOptions.FrameCooldown);
                var renderer = role.FrameButton.graphic;
                
                if (role.ClosestPlayer != null && !role.FrameButton.isCoolingDown)
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
            else if (formerRole == RoleEnum.Gorgon)
            {
                if (role.GazeButton == null)
                {
                    role.GazeButton = Object.Instantiate(__instance.KillButton, HudManager.Instance.transform);
                    role.GazeButton.graphic.enabled = true;
                    role.GazeButton.graphic.sprite = Gaze;
                    role.GazeButton.gameObject.SetActive(false);
                }

                role.GazeButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance && !LobbyBehaviour.Instance);
                var notImpostor = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Syndicate)).ToList();
                Utils.SetTarget(ref role.ClosestPlayer, role.GazeButton, notImpostor);
                role.GazeButton.SetCoolDown(role.GazeTimer(), CustomGameOptions.GazeCooldown);
                var renderer = role.GazeButton.graphic;
                
                if (role.ClosestPlayer != null && !role.GazeButton.isCoolingDown)
                {
                    renderer.color = Palette.EnabledColor;
                    renderer.material.SetFloat("_Desat", 0f);
                }
                else
                {
                    renderer.color = Palette.DisabledClear;
                    renderer.material.SetFloat("_Desat", 1.0f);
                }
            }
            else if (formerRole == RoleEnum.Shapeshifter)
            {
                if (role.ShapeshiftButton == null)
                {
                    role.ShapeshiftButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                    role.ShapeshiftButton.graphic.enabled = true;
                    role.ShapeshiftButton.graphic.sprite = Shapeshift;
                    role.ShapeshiftButton.gameObject.SetActive(false);
                }

                role.ShapeshiftButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));

                if (role.ShapeshiftEnabled)
                    role.ShapeshiftButton.SetCoolDown(role.ShapeshiftTimeRemaining, CustomGameOptions.ShapeshiftDuration);
                else
                    role.ShapeshiftButton.SetCoolDown(role.ShapeshiftTimer(), CustomGameOptions.ShapeshiftCooldown);
                
                var renderer = role.ShapeshiftButton.graphic;

                if (!role.ShapeshiftButton.isCoolingDown && !role.Shapeshifted)
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
            else if (formerRole == RoleEnum.Warper)
            {
                if (role.WarpButton == null)
                {
                    role.WarpButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                    role.WarpButton.graphic.enabled = true;
                    role.WarpButton.graphic.sprite = Warp;
                    role.WarpButton.gameObject.SetActive(false);
                }

                role.WarpButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));
                role.WarpButton.SetCoolDown(role.WarpTimer(), CustomGameOptions.WarpCooldown);

                var renderer = role.WarpButton.graphic;

                if (!role.WarpButton.isCoolingDown)
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
            else if (formerRole == RoleEnum.Poisoner)
            {
                if (role.PoisonButton == null)
                {
                    role.PoisonButton = Object.Instantiate(__instance.KillButton, HudManager.Instance.transform);
                    role.PoisonButton.graphic.enabled = true;
                    role.PoisonButton.graphic.sprite = PoisonSprite;
                    role.PoisonButton.gameObject.SetActive(false);
                }

                role.PoisonButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));
                var notSyn = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Syndicate) && x != role.PoisonedPlayer).ToList();
                Utils.SetTarget(ref role.ClosestPlayer, role.PoisonButton, notSyn);

                if (role.Poisoned)
                {
                    role.PoisonButton.graphic.sprite = PoisonedSprite;
                    role.Poison();
                    role.PoisonButton.SetCoolDown(role.PoisonTimeRemaining, CustomGameOptions.PoisonDuration);
                }
                else
                {
                    role.PoisonButton.graphic.sprite = PoisonSprite;

                    if (role.PoisonedPlayer && role.PoisonedPlayer != PlayerControl.LocalPlayer)
                        role.PoisonKill();

                    role.PoisonButton.SetCoolDown(role.PoisonTimer(), CustomGameOptions.PoisonCd);
                    role.PoisonedPlayer = PlayerControl.LocalPlayer; //Only do this to stop repeatedly trying to re-kill poisoned player. null didn't work for some reason
                }

                var renderer = role.PoisonButton.graphic;

                if (role.ClosestPlayer != null && !role.PoisonButton.isCoolingDown && !role.Poisoned)
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
}