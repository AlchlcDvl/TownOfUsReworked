using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.DisguiserMod;
using System.Linq;
using AmongUs.GameOptions;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.GodfatherMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDEverything
    {
        public static Sprite Promote => TownOfUsReworked.Placeholder;
        public static Sprite Blackmail => TownOfUsReworked.BlackmailSprite;
        public static Sprite Camouflage => TownOfUsReworked.Camouflage;
        public static Sprite MeasureSprite => TownOfUsReworked.MeasureSprite;
        public static Sprite DisguiseSprite => TownOfUsReworked.DisguiseSprite;
        public static Sprite FlashSprite => TownOfUsReworked.FlashSprite;
        public static Sprite Clean => TownOfUsReworked.JanitorClean;
        public static Sprite MineSprite => TownOfUsReworked.MineSprite;
        public static Sprite SampleSprite => TownOfUsReworked.SampleSprite;
        public static Sprite MorphSprite => TownOfUsReworked.MorphSprite;
        public static Sprite MarkSprite => TownOfUsReworked.MarkSprite;
        public static Sprite TeleportSprite => TownOfUsReworked.TeleportSprite;
        public static Sprite Freeze => TownOfUsReworked.TimeFreezeSprite;
        public static Sprite Drag => TownOfUsReworked.DragSprite;
        public static Sprite InvisSprite => TownOfUsReworked.InvisSprite;
        public static Sprite Drop => TownOfUsReworked.DropSprite;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Godfather))
                return;

            var role = Role.GetRole<Godfather>(PlayerControl.LocalPlayer);

            if (role.KillButton == null)
            {
                role.KillButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.KillButton.graphic.enabled = true;
                role.KillButton.gameObject.SetActive(false);
            }

            var notImp = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Intruder)).ToList();
            role.KillButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));
            role.KillButton.SetCoolDown(role.KillTimer(), CustomGameOptions.IntKillCooldown);
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

                var Imp = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Intruder)).ToList();

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

            if (role.FormerRole == null || !role.WasMafioso || role.FormerRole?.RoleType == RoleEnum.Impostor)
                return;

            var formerRole = role.FormerRole.RoleType;

            if (formerRole == RoleEnum.Blackmailer)
            {
                if (role.BlackmailButton == null)
                {
                    role.BlackmailButton = Object.Instantiate(__instance.KillButton, __instance.UseButton.transform.parent);
                    role.BlackmailButton.graphic.enabled = true;
                    role.BlackmailButton.graphic.sprite = Blackmail;
                    role.BlackmailButton.gameObject.SetActive(false);
                }

                role.BlackmailButton.SetCoolDown(role.BlackmailTimer(), CustomGameOptions.BlackmailCd);
                role.BlackmailButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));
                var notBlackmailed = PlayerControl.AllPlayerControls.ToArray().Where(player => role.Blackmailed?.PlayerId != player.PlayerId).ToList();
                Utils.SetTarget(ref role.ClosestPlayer, role.BlackmailButton, notBlackmailed);

                var renderer = role.BlackmailButton.graphic;
                
                if (role.ClosestPlayer != null && !role.BlackmailButton.isCoolingDown)
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
            else if (formerRole == RoleEnum.Consigliere)
            {
                if (role.CamouflageButton == null)
                {
                    role.CamouflageButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                    role.CamouflageButton.graphic.sprite = Camouflage;
                    role.CamouflageButton.graphic.enabled = true;
                    role.CamouflageButton.gameObject.SetActive(false);
                }

                if (role.CamoEnabled)
                    role.CamouflageButton.SetCoolDown(role.CamoTimeRemaining, CustomGameOptions.CamouflagerDuration);
                else
                    role.CamouflageButton.SetCoolDown(role.CamouflageTimer(), CustomGameOptions.CamouflagerCd);
            
                var renderer = role.CamouflageButton.graphic;

                if (!role.Camouflaged && !role.CamouflageButton.isCoolingDown)
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
            else if (formerRole == RoleEnum.Disguiser)
            {
                if (role.DisguiseButton == null)
                {
                    role.DisguiseButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                    role.DisguiseButton.graphic.enabled = true;
                    role.DisguiseButton.graphic.sprite = MeasureSprite;
                    role.DisguiseButton.gameObject.SetActive(false);

                    if (role.DisguiseButton.graphic.sprite != MeasureSprite && role.DisguiseButton.graphic.sprite != DisguiseSprite)
                        role.DisguiseButton.graphic.sprite = MeasureSprite;
                }

                var targets = PlayerControl.AllPlayerControls.ToArray().ToList();

                if (CustomGameOptions.DisguiseTarget == DisguiserTargets.Intruders)
                    targets = targets.Where(x => x.Is(Faction.Intruder)).ToList();
                else if (CustomGameOptions.DisguiseTarget == DisguiserTargets.NonIntruders)
                    targets = targets.Where(x => !x.Is(Faction.Intruder)).ToList();

                role.DisguiseButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));

                if (role.DisguiseButton.graphic.sprite == MeasureSprite)
                    role.DisguiseButton.SetCoolDown(0f, 1f);
                else
                {
                    if (role.Disguised)
                        role.DisguiseButton.SetCoolDown(role.DisguiseTimeRemaining, CustomGameOptions.DisguiseDuration);
                    else
                        role.DisguiseButton.SetCoolDown(role.DisguiseTimer(), CustomGameOptions.DisguiseCooldown);
                }

                var renderer = role.DisguiseButton.graphic;
                
                if (role.ClosestPlayer != null && !role.DisguiseButton.isCoolingDown && !role.Disguised)
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
            else if (formerRole == RoleEnum.Grenadier)
            {
                if (role.FlashButton == null)
                {
                    role.FlashButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                    role.FlashButton.graphic.enabled = true;
                    role.FlashButton.graphic.sprite = FlashSprite;
                    role.FlashButton.gameObject.SetActive(false);
                }

                role.FlashButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));

                if (role.Flashed)
                    role.FlashButton.SetCoolDown(role.FlashTimeRemaining, CustomGameOptions.GrenadeDuration);
                else 
                    role.FlashButton.SetCoolDown(role.FlashTimer(), CustomGameOptions.GrenadeCd);

                var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
                var specials = system.specials.ToArray();
                var dummyActive = system.dummy.IsActive;
                var sabActive = specials.Any(s => s.IsActive);
                var renderer = role.FlashButton.graphic;

                if (sabActive && !dummyActive && !role.FlashButton.isCoolingDown)
                {
                    renderer.color = Palette.DisabledClear;
                    renderer.material.SetFloat("_Desat", 1f);
                }
                else
                {
                    renderer.color = Palette.EnabledColor;
                    renderer.material.SetFloat("_Desat", 0f);
                }

                if (CustomGameOptions.GrenadierIndicators)
                {
                    foreach (var player in PlayerControl.AllPlayerControls)
                    {
                        if (player != PlayerControl.LocalPlayer && !player.Is(Faction.Intruder))
                        {
                            var tempColour = player.nameText().color;
                            var data = player?.Data;

                            if (data == null || data.Disconnected || data.IsDead || PlayerControl.LocalPlayer.Data.IsDead)
                                continue;

                            if (role.flashedPlayers.Contains(player))
                            {
                                player.myRend().material.SetColor("_VisorColor", Color.black);
                                player.nameText().color = Color.black;
                            }
                            else
                            {
                                player.myRend().material.SetColor("_VisorColor", Palette.VisorColor);
                                player.nameText().color = tempColour;
                            }
                        }
                    }
                }
            }
            else if (formerRole == RoleEnum.Janitor)
            {
                if (role.CleanButton == null)
                {
                    role.CleanButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                    role.CleanButton.graphic.enabled = true;
                    role.CleanButton.graphic.sprite = Clean;
                    role.CleanButton.gameObject.SetActive(false);
                }

                role.CleanButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));

                var data = PlayerControl.LocalPlayer.Data;
                var isDead = data.IsDead;
                var truePosition = PlayerControl.LocalPlayer.GetTruePosition();
                var maxDistance = GameOptionsData.KillDistances[CustomGameOptions.InteractionDistance];
                var flag = GameStates.IsInGame && !GameStates.IsMeeting && PlayerControl.LocalPlayer.CanMove;
                var allocs = Physics2D.OverlapCircleAll(truePosition, maxDistance, LayerMask.GetMask(new[] {"Players", "Ghost"}));
                var killButton = role.CleanButton;
                DeadBody closestBody = null;
                var closestDistance = float.MaxValue;

                foreach (var collider2D in allocs)
                {
                    if (!flag || isDead || collider2D.tag != "DeadBody")
                        continue;

                    var component = collider2D.GetComponent<DeadBody>();

                    if (!(Vector2.Distance(truePosition, component.TruePosition) <= maxDistance))
                        continue;

                    var distance = Vector2.Distance(truePosition, component.TruePosition);

                    if (!(distance < closestDistance))
                        continue;

                    closestBody = component;
                    closestDistance = distance;
                }

                KillButtonTarget.SetTarget(killButton, closestBody, role);
                role.CleanButton.SetCoolDown(role.CleanTimer(), CustomGameOptions.JanitorCleanCd);
                var renderer = role.CleanButton.graphic;

                if (role.CurrentTarget != null && !role.CleanButton.isCoolingDown)
                {
                    renderer.color = Palette.DisabledClear;
                    renderer.material.SetFloat("_Desat", 1f);
                }
                else
                {
                    renderer.color = Palette.EnabledColor;
                    renderer.material.SetFloat("_Desat", 0f);
                }
            }
            else if (formerRole == RoleEnum.Miner)
            {
                if (role.MineButton == null)
                {
                    role.MineButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                    role.MineButton.graphic.enabled = true;
                    role.MineButton.gameObject.SetActive(false);
                    role.MineButton.graphic.sprite = MineSprite;
                }

                role.MineButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));
                role.MineButton.SetCoolDown(role.MineTimer(), CustomGameOptions.MineCd);
                var hits = Physics2D.OverlapBoxAll(PlayerControl.LocalPlayer.transform.position, role.VentSize, 0);
                hits = hits.ToArray().Where(c => (c.name.Contains("Vent") || !c.isTrigger) && c.gameObject.layer != 8 && c.gameObject.layer != 5).ToArray();
                
                if (hits.Count == 0 && PlayerControl.LocalPlayer.moveable == true && !role.MineButton.isCoolingDown)
                {
                    role.MineButton.graphic.color = Palette.EnabledColor;
                    role.MineButton.graphic.material.SetFloat("_Desat", 0f);
                    role.CanPlace = true;
                }
                else
                {
                    role.MineButton.graphic.color = Palette.DisabledClear;
                    role.MineButton.graphic.material.SetFloat("_Desat", 1f);
                    role.CanPlace = false;
                }
            }
            else if (formerRole == RoleEnum.Morphling)
            {
                if (role.MorphButton == null)
                {
                    role.MorphButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                    role.MorphButton.graphic.enabled = true;
                    role.MorphButton.graphic.sprite = SampleSprite;
                    role.MorphButton.gameObject.SetActive(false);
                }

                if (role.MorphButton.graphic.sprite != SampleSprite && role.MorphButton.graphic.sprite != MorphSprite)
                    role.MorphButton.graphic.sprite = SampleSprite;

                role.MorphButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));
                
                if (role.MorphButton.graphic.sprite == SampleSprite)
                {
                    role.MorphButton.SetCoolDown(0f, 1f);
                    Utils.SetTarget(ref role.ClosestPlayer, role.MorphButton);
                    var renderer = role.MorphButton.graphic;

                    if (role.ClosestPlayer != null && !role.MorphButton.isCoolingDown)
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
                else
                {
                    if (role.Morphed)
                        role.MorphButton.SetCoolDown(role.MorphTimeRemaining, CustomGameOptions.MorphlingDuration);
                    else 
                        role.MorphButton.SetCoolDown(role.MorphTimer(), CustomGameOptions.MorphlingCd);

                    var renderer = role.MorphButton.graphic;
                    role.MorphButton.SetTarget(null);

                    if (!role.Morphed && !role.MorphButton.isCoolingDown)
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
            else if (formerRole == RoleEnum.Teleporter)
            {
                if (role.TeleportButton == null)
                {
                    role.TeleportButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                    role.TeleportButton.graphic.enabled = true;
                    role.TeleportButton.graphic.sprite = MarkSprite;
                    role.TeleportButton.gameObject.SetActive(false);
                }

                if (role.TeleportButton.graphic.sprite != MarkSprite && role.TeleportButton.graphic.sprite != TeleportSprite)
                    role.TeleportButton.graphic.sprite = MarkSprite;

                role.TeleportButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));

                if (role.TeleportButton.graphic.sprite == MarkSprite)
                    role.TeleportButton.SetCoolDown(0f, 1f);
                else
                    role.TeleportButton.SetCoolDown(role.TeleportTimer(), CustomGameOptions.TeleportCd);
        
                var renderer = role.TeleportButton.graphic;
                
                if (!role.TeleportButton.isCoolingDown)
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
            else if (formerRole == RoleEnum.Teleporter)
            {
                if (role.FreezeButton == null)
                {
                    role.FreezeButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                    role.FreezeButton.graphic.enabled = true;
                    role.FreezeButton.graphic.sprite = Freeze;
                    role.FreezeButton.gameObject.SetActive(false);
                }

                role.FreezeButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance && !LobbyBehaviour.Instance);

                if (role.FreezeEnabled)
                    role.FreezeButton.SetCoolDown(role.FreezeTimeRemaining, CustomGameOptions.FreezeDuration);
                else
                    role.FreezeButton.SetCoolDown(role.FreezeTimer(), CustomGameOptions.FreezeCooldown);
                
                var renderer = role.FreezeButton.graphic;

                if (!role.Frozen && !role.FreezeButton.isCoolingDown)
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
            else if (formerRole == RoleEnum.Undertaker)
            {
                if (role.DragDropButton == null)
                {
                    role.DragDropButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                    role.DragDropButton.graphic.enabled = true;
                    role.DragDropButton.graphic.sprite = Drag;
                    role.DragDropButton.gameObject.SetActive(false);
                }

                if ((role.DragDropButton.graphic.sprite != Drag && role.DragDropButton.graphic.sprite != Drop) || (role.DragDropButton.graphic.sprite == Drop &&
                    role.CurrentlyDragging == null))
                    role.DragDropButton.graphic.sprite = Drag;

                role.DragDropButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));

                if (role.DragDropButton.graphic.sprite == Drag)
                {
                    var data = PlayerControl.LocalPlayer.Data;
                    var isDead = data.IsDead;
                    var truePosition = PlayerControl.LocalPlayer.GetTruePosition();
                    var maxDistance = GameOptionsData.KillDistances[CustomGameOptions.InteractionDistance];
                    var flag = GameStates.IsInGame && !GameStates.IsMeeting && PlayerControl.LocalPlayer.CanMove;
                    var allocs = Physics2D.OverlapCircleAll(truePosition, maxDistance, LayerMask.GetMask(new[] {"Players", "Ghost"}));
                    DeadBody closestBody = null;
                    var closestDistance = float.MaxValue;

                    foreach (var collider2D in allocs)
                    {
                        if (!flag || isDead || collider2D.tag != "DeadBody")
                            continue;

                        var component = collider2D.GetComponent<DeadBody>();

                        if (!(Vector2.Distance(truePosition, component.TruePosition) <= maxDistance))
                            continue;

                        var distance = Vector2.Distance(truePosition, component.TruePosition);

                        if (!(distance < closestDistance))
                            continue;

                        closestBody = component;
                        closestDistance = distance;
                    }

                    KillButtonTarget.SetTarget(role.DragDropButton, closestBody, role);
                }

                if (role.DragDropButton.graphic.sprite == Drag)
                {
                    role.DragDropButton.SetCoolDown(role.DragTimer(), CustomGameOptions.DragCd);

                    var renderer1 = role.DragDropButton.graphic;

                    if (role.CurrentTarget != null && !role.DragDropButton.isCoolingDown)
                    {
                        renderer1.color = Palette.EnabledColor;
                        renderer1.material.SetFloat("_Desat", 0f);
                    }
                    else
                    {
                        renderer1.color = Palette.DisabledClear;
                        renderer1.material.SetFloat("_Desat", 1f);
                    }
                }
                else
                {
                    role.DragDropButton.SetCoolDown(0f, 1f);
                    role.DragDropButton.SetTarget(null);
                    role.DragDropButton.graphic.color = Palette.EnabledColor;
                    role.DragDropButton.graphic.material.SetFloat("_Desat", 0f);
                }
            }
            else if (formerRole == RoleEnum.Wraith)
            {
                if (role.InvisButton == null)
                {
                    role.InvisButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                    role.InvisButton.graphic.enabled = true;
                    role.InvisButton.graphic.sprite = InvisSprite;
                    role.InvisButton.gameObject.SetActive(false);
                }

                role.InvisButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));

                if (role.IsInvis)
                    role.InvisButton.SetCoolDown(role.InvisTimeRemaining, CustomGameOptions.InvisDuration);
                else
                    role.InvisButton.SetCoolDown(role.InvisTimer(), CustomGameOptions.InvisCd);

                var renderer = role.InvisButton.graphic;

                if (!role.IsInvis && !role.InvisButton.isCoolingDown)
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