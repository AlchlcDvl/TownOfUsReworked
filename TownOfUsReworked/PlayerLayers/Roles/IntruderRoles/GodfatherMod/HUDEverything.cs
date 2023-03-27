using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using UnityEngine;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.DisguiserMod;
using System.Linq;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Modules;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.GodfatherMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDEverything
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Godfather))
                return;

            var role = Role.GetRole<Godfather>(PlayerControl.LocalPlayer);

            if (role.DeclareButton == null)
                role.DeclareButton = CustomButtons.InstantiateButton();

            var Imp = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Intruder) && !x.Is(RoleEnum.Consort)).ToList();
            role.DeclareButton.UpdateButton(role, "PROMOTE", 0, 1, AssetManager.Promote, AbilityTypes.Direct, "Secondary", Imp, !role.HasDeclared);

            if (!role.WasMafioso || role.FormerRole == null || role.FormerRole?.RoleType == RoleEnum.Impostor)
                return;

            var formerRole = role.FormerRole?.RoleType;

            if (formerRole == RoleEnum.Blackmailer)
            {
                if (role.BlackmailButton == null)
                    role.BlackmailButton = CustomButtons.InstantiateButton();

                var notBlackmailed = PlayerControl.AllPlayerControls.ToArray().Where(player => role.BlackmailedPlayer != player).ToList();
                role.BlackmailButton.UpdateButton(role, role.Blackmailed ? "BLACKMAILED" : "BLACKMAIL", role.BlackmailTimer(), CustomGameOptions.BlackmailCd, role.Blackmailed ?
                    AssetManager.BlackmailLetter : AssetManager.Blackmail, AbilityTypes.Direct, "Secondary", notBlackmailed);
            }
            else if (formerRole == RoleEnum.Consigliere)
            {
                if (role.InvestigateButton == null)
                    role.InvestigateButton = CustomButtons.InstantiateButton();

                var notInvestigated = PlayerControl.AllPlayerControls.ToArray().Where(x => !role.Investigated.Contains(x.PlayerId) && !(x.Is(Faction.Intruder) &&
                    CustomGameOptions.FactionSeeRoles)).ToList();
                role.InvestigateButton.UpdateButton(role, "INVESTIGATE", role.ConsigliereTimer(), CustomGameOptions.ConsigCd, AssetManager.Placeholder, AbilityTypes.Direct, "Secondary",
                    notInvestigated);
            }
            else if (formerRole == RoleEnum.Camouflager)
            {
                if (role.CamouflageButton == null)
                    role.CamouflageButton = CustomButtons.InstantiateButton();

                role.CamouflageButton.UpdateButton(role, "CAMOUFLAGE", role.CamouflageTimer(), CustomGameOptions.CamouflagerCd, AssetManager.Camouflage, AbilityTypes.Effect, "Secondary",
                    null, true, !role.Camouflaged, role.Camouflaged, role.CamoTimeRemaining, CustomGameOptions.CamouflagerDuration);
            }
            else if (formerRole == RoleEnum.Disguiser)
            {
                if (role.DisguiseButton == null)
                    role.DisguiseButton = CustomButtons.InstantiateButton();

                var targets = PlayerControl.AllPlayerControls.ToArray().ToList();

                if (CustomGameOptions.DisguiseTarget == DisguiserTargets.Intruders)
                    targets = targets.Where(x => x.Is(Faction.Intruder)).ToList();
                else if (CustomGameOptions.DisguiseTarget == DisguiserTargets.NonIntruders)
                    targets = targets.Where(x => !x.Is(Faction.Intruder)).ToList();

                if (role.MeasuredPlayer != null && targets.Contains(role.MeasuredPlayer))
                    targets.Remove(role.MeasuredPlayer);

                role.DisguiseButton.UpdateButton(role, "DISGUISE", role.DisguiseTimer(), CustomGameOptions.DisguiseCooldown, AssetManager.Disguise, AbilityTypes.Direct, "Secondary",
                    targets, role.MeasuredPlayer != null, true, role.DelayActive || role.Disguised, role.DelayActive ? role.DisguiserTimeRemaining2 : role.DisguiserTimeRemaining,
                    role.DelayActive ? CustomGameOptions.TimeToDisguise : CustomGameOptions.DisguiseDuration);

                if (role.MeasureButton == null)
                    role.MeasureButton = CustomButtons.InstantiateButton();

                var notMeasured = PlayerControl.AllPlayerControls.ToArray().Where(x => role.MeasuredPlayer != x).ToList();
                role.MeasureButton.UpdateButton(role, "MEASURE", role.MeasureTimer(), CustomGameOptions.MeasureCooldown, AssetManager.Measure, AbilityTypes.Direct, "Tertiary",
                    notMeasured);
            }
            else if (formerRole == RoleEnum.Grenadier)
            {
                if (role.FlashButton == null)
                    role.FlashButton = CustomButtons.InstantiateButton();

                var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
                var dummyActive = (bool)system?.dummy.IsActive;
                var sabActive = (bool)system?.specials.ToArray().Any(s => s.IsActive);
                role.FlashButton.UpdateButton(role, "FLASH", role.FlashTimer(), CustomGameOptions.GrenadeCd, AssetManager.Flash, AbilityTypes.Effect, "Secondary", null, true,
                    !sabActive && !dummyActive && !role.Flashed, role.Flashed, role.FlashTimeRemaining, CustomGameOptions.GrenadeDuration);
            }
            else if (formerRole == RoleEnum.Janitor)
            {
                if (role.CleanButton == null)
                    role.CleanButton = CustomButtons.InstantiateButton();

                role.CleanButton.UpdateButton(role, "CLEAN", role.CleanTimer(), CustomGameOptions.JanitorCleanCd, AssetManager.Clean, AbilityTypes.Dead, "Secondary");
            }
            else if (formerRole == RoleEnum.Miner)
            {
                if (role.MineButton == null)
                    role.MineButton = CustomButtons.InstantiateButton();

                var hits = Physics2D.OverlapBoxAll(PlayerControl.LocalPlayer.transform.position, Utils.GetSize(), 0);
                hits = hits.ToArray().Where(c => (c.name.Contains("Vent") || !c.isTrigger) && c.gameObject.layer != 8 && c.gameObject.layer != 5).ToArray();
                role.CanPlace = hits.Count == 0 && PlayerControl.LocalPlayer.moveable && !SubmergedCompatibility.GetPlayerElevator(PlayerControl.LocalPlayer).Item1;
                role.MineButton.UpdateButton(role, "MINE", role.MineTimer(), CustomGameOptions.MineCd, AssetManager.Mine, AbilityTypes.Effect, "Secondary", null, true, role.CanPlace);
            }
            else if (formerRole == RoleEnum.Morphling)
            {
                if (role.MorphButton == null)
                    role.MorphButton = CustomButtons.InstantiateButton();

                role.MorphButton.UpdateButton(role, "MORPH", role.MorphTimer(), CustomGameOptions.MorphlingCd, AssetManager.Morph, AbilityTypes.Effect, "Secondary", role.Morphed,
                    role.MorphTimeRemaining, CustomGameOptions.MorphlingDuration, role.SampledPlayer != null, !role.Morphed);

                if (role.SampleButton == null)
                    role.SampleButton = CustomButtons.InstantiateButton();

                var notSampled = PlayerControl.AllPlayerControls.ToArray().Where(x => role.SampledPlayer?.PlayerId != x.PlayerId).ToList();
                role.SampleButton.UpdateButton(role, "SAMPLE", role.SampleTimer(), CustomGameOptions.SampleCooldown, AssetManager.Sample, AbilityTypes.Direct, "Tertiary", notSampled);
            }
            else if (formerRole == RoleEnum.Teleporter)
            {
                if (role.TeleportButton == null)
                    role.TeleportButton = CustomButtons.InstantiateButton();

                role.TeleportButton.UpdateButton(role, "TELEPORT", role.TeleportTimer(), CustomGameOptions.TeleportCd, AssetManager.Teleport, AbilityTypes.Effect, "Secondary", null,
                    role.TeleportPoint != new Vector3(0, 0, 0));

                if (role.MarkButton == null)
                    role.MarkButton = CustomButtons.InstantiateButton();

                var hits = Physics2D.OverlapBoxAll(PlayerControl.LocalPlayer.transform.position, Utils.GetSize(), 0);
                hits = hits.ToArray().Where(c => (c.name.Contains("Vent") || !c.isTrigger) && c.gameObject.layer != 8 && c.gameObject.layer != 5).ToArray();
                role.CanMark = hits.Count == 0 && PlayerControl.LocalPlayer.moveable && !SubmergedCompatibility.GetPlayerElevator(PlayerControl.LocalPlayer).Item1 && role.TeleportPoint !=
                    PlayerControl.LocalPlayer.transform.position;
                role.MarkButton.UpdateButton(role, "MARK", role.MarkTimer(), CustomGameOptions.MarkCooldown, AssetManager.Mark, AbilityTypes.Effect, "Tertiary", null, true,
                    role.CanMark);
            }
            else if (formerRole == RoleEnum.TimeMaster)
            {
                if (role.FreezeButton == null)
                    role.FreezeButton = CustomButtons.InstantiateButton();

                role.FreezeButton.UpdateButton(role, "TIME FREEZE", role.FreezeTimer(), CustomGameOptions.FreezeCooldown, AssetManager.TimeFreeze, AbilityTypes.Effect, "Secondary",
                    null, true, !role.Frozen, role.Frozen, role.FreezeTimeRemaining, CustomGameOptions.FreezeDuration);
            }
            else if (formerRole == RoleEnum.Undertaker)
            {
                if (role.DragButton == null)
                    role.DragButton = CustomButtons.InstantiateButton();

                role.DragButton.UpdateButton(role, "DRAG", role.DragTimer(), CustomGameOptions.DragCd, AssetManager.Drag, AbilityTypes.Dead, "Secondary", role.CurrentlyDragging == null);

                if (role.DropButton == null)
                    role.DropButton = CustomButtons.InstantiateButton();

                role.DropButton.UpdateButton(role, "DROP", 0, 1, AssetManager.Drop, AbilityTypes.Dead, "Secondary", role.CurrentlyDragging != null);
            }
            else if (formerRole == RoleEnum.Wraith)
            {
                if (role.InvisButton == null)
                    role.InvisButton = CustomButtons.InstantiateButton();

                role.InvisButton.UpdateButton(role, "INVIS", role.InvisTimer(), CustomGameOptions.InvisCd, AssetManager.Invis, AbilityTypes.Effect, "Secondary", null, true,
                    !role.IsInvis, role.IsInvis, role.InvisTimeRemaining, CustomGameOptions.InvisDuration);
            }
        }
    }
}