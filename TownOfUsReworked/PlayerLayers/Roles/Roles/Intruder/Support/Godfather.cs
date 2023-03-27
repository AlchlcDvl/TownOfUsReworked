using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using Il2CppSystem.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using Reactor.Utilities;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Functions;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Godfather : IntruderRole
    {
        public Godfather(PlayerControl player) : base(player)
        {
            Name = "Godfather";
            RoleType = RoleEnum.Godfather;
            StartText = "Promote Your Fellow <color=#FF0000FF>Intruders</color> To Do Better";
            AbilitiesText = "- You can promote a fellow <color=#FF0000FF>Intruder</color> into becoming your successor.\n- Promoting an <color=#FF0000FF>Intruder</color> turns them " +
                "into a <color=#6400FFFF>Mafioso</color>.\n- If you die, the <color=#6400FFFF>Mafioso</color> become the new <color=#404C08FF>Godfather</color>\nand inherits better " +
                "abilities of their former role.";
            Color = CustomGameOptions.CustomIntColors ? Colors.Godfather : Colors.Intruder;
            RoleAlignment = RoleAlignment.IntruderSupport;
            AlignmentName = IS;
        }

        //Godfather Stuff
        public PlayerControl ClosestIntruder;
        public bool HasDeclared;
        public bool WasMafioso;
        public Role FormerRole;
        public AbilityButton DeclareButton;
        public DateTime LastDeclared;
        public DeadBody CurrentTarget;
        public PlayerControl ClosestTarget;
        public Vent ClosestVent;

        //Blackmailer Stuff
        public AbilityButton BlackmailButton;
        public PlayerControl BlackmailedPlayer;
        public DateTime LastBlackmailed;
        public bool Blackmailed => BlackmailedPlayer != null;
        public PlayerControl ClosestBlackmail;

        public float BlackmailTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastBlackmailed;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.BlackmailCd, CustomButtons.GetUnderdogChange(Player), CustomGameOptions.MafiosoAbilityCooldownDecrease) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        //Camouflager Stuff
        public AbilityButton CamouflageButton;
        public bool CamoEnabled;
        public DateTime LastCamouflaged;
        public float CamoTimeRemaining;
        public bool Camouflaged => CamoTimeRemaining > 0f;

        public void Camouflage()
        {
            CamoEnabled = true;
            CamoTimeRemaining -= Time.deltaTime;
            Utils.Camouflage();

            if (MeetingHud.Instance)
                CamoTimeRemaining = 0f;
        }

        public void UnCamouflage()
        {
            CamoEnabled = false;
            LastCamouflaged = DateTime.UtcNow;
            Utils.DefaultOutfitAll();
        }

        public float CamouflageTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastCamouflaged;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.CamouflagerCd, CustomButtons.GetUnderdogChange(Player), CustomGameOptions.MafiosoAbilityCooldownDecrease) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        //Grenadier Stuff
        public AbilityButton FlashButton;
        public bool FlashEnabled;
        public DateTime LastFlashed;
        public float FlashTimeRemaining;
        private static List<PlayerControl> ClosestPlayers = new();
        private static readonly Color NormalVision = new Color32(212, 212, 212, 0);
        private static readonly Color DimVision = new Color32(212, 212, 212, 51);
        private static readonly Color BlindVision = new Color32(212, 212, 212, 255);
        public List<PlayerControl> FlashedPlayers;
        public bool Flashed => FlashTimeRemaining > 0f;

        public float FlashTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastFlashed;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.GrenadeCd, CustomButtons.GetUnderdogChange(Player), CustomGameOptions.MafiosoAbilityCooldownDecrease) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Flash()
        {
            if (!FlashEnabled)
            {
                ClosestPlayers = Utils.GetClosestPlayers(Player.GetTruePosition(), CustomGameOptions.FlashRadius);
                FlashedPlayers = ClosestPlayers;
            }

            FlashEnabled = true;
            FlashTimeRemaining -= Time.deltaTime;

            if (MeetingHud.Instance)
                FlashTimeRemaining = 0f;

            //To stop the scenario where the flash and sabotage are called at the same time.
            var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
            var dummyActive = system.dummy.IsActive;
            var sabActive = system.specials.ToArray().Any(s => s.IsActive);

            if (sabActive || dummyActive)
                return;

            foreach (var player in ClosestPlayers)
            {
                if (PlayerControl.LocalPlayer.PlayerId == player.PlayerId)
                {
                    HudManager.Instance.FullScreen.enabled = true;
                    HudManager.Instance.FullScreen.gameObject.active = true;

                    if (FlashTimeRemaining > CustomGameOptions.GrenadeDuration - 0.5f)
                    {
                        float fade = (FlashTimeRemaining - CustomGameOptions.GrenadeDuration) * (-2f);

                        if (ShouldPlayerBeBlinded(player))
                            HudManager.Instance.FullScreen.color = Color32.Lerp(NormalVision, BlindVision, fade);
                        else if (ShouldPlayerBeDimmed(player))
                            HudManager.Instance.FullScreen.color = Color32.Lerp(NormalVision, DimVision, fade);
                        else
                            HudManager.Instance.FullScreen.color = NormalVision;
                    }
                    else if (FlashTimeRemaining <= (CustomGameOptions.GrenadeDuration - 0.5f) && FlashTimeRemaining >= 0.5f)
                    {
                        HudManager.Instance.FullScreen.enabled = true;
                        HudManager.Instance.FullScreen.gameObject.active = true;

                        if (ShouldPlayerBeBlinded(player))
                            HudManager.Instance.FullScreen.color = BlindVision;
                        else if (ShouldPlayerBeDimmed(player))
                            HudManager.Instance.FullScreen.color = DimVision;
                        else
                            HudManager.Instance.FullScreen.color = NormalVision;
                    }
                    else if (FlashTimeRemaining < 0.5f)
                    {
                        float fade2 = (FlashTimeRemaining * -2.0f) + 1.0f;

                        if (ShouldPlayerBeBlinded(player))
                            HudManager.Instance.FullScreen.color = Color32.Lerp(BlindVision, NormalVision, fade2);
                        else if (ShouldPlayerBeDimmed(player))
                            HudManager.Instance.FullScreen.color = Color32.Lerp(DimVision, NormalVision, fade2);
                        else
                            HudManager.Instance.FullScreen.color = NormalVision;
                    }
                    else
                    {
                        HudManager.Instance.FullScreen.color = NormalVision;
                        FlashTimeRemaining = 0f;
                    }
                }
            }
        }

        private static bool ShouldPlayerBeDimmed(PlayerControl player) => (player.Is(Faction.Intruder) || player.Data.IsDead) && !MeetingHud.Instance;

        private static bool ShouldPlayerBeBlinded(PlayerControl player) => !(player.Is(Faction.Intruder) || player.Data.IsDead || MeetingHud.Instance);

        public void UnFlash()
        {
            FlashEnabled = false;
            LastFlashed = DateTime.UtcNow;
            HudManager.Instance.FullScreen.enabled = true;
            HudManager.Instance.FullScreen.color = NormalVision;
            FlashedPlayers.Clear();
        }

        //Janitor Stuff
        public AbilityButton CleanButton;
        public DateTime LastCleaned;

        public float CleanTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastCleaned;
            var num = CustomButtons.GetModifiedCooldown(Utils.LastImp() && CustomGameOptions.SoloBoost ? (CustomGameOptions.JanitorCleanCd - CustomGameOptions.UnderdogKillBonus) :
                CustomGameOptions.JanitorCleanCd, CustomButtons.GetUnderdogChange(Player), CustomGameOptions.MafiosoAbilityCooldownDecrease) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        //Undertaker Stuff
        public AbilityButton DragButton;
        public AbilityButton DropButton;
        public DateTime LastDragged;
        public DeadBody CurrentlyDragging;

        public float DragTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastDragged;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.DragCd, CustomButtons.GetUnderdogChange(Player)) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        //Disguiser Stuff
        public AbilityButton DisguiseButton;
        public DateTime LastDisguised;
        public PlayerControl MeasuredPlayer;
        public float DisguiserTimeRemaining;
        public bool Disguised => DisguiserTimeRemaining > 0f;
        public float DisguiserTimeRemaining2;
        public bool DelayActive => DisguiserTimeRemaining2 > 0f;
        public bool DisguiserEnabled;
        public PlayerControl DisguisedPlayer;
        public PlayerControl MeasureTarget;
        public AbilityButton MeasureButton;
        public DateTime LastMeasured;

        public void Disguise()
        {
            DisguiserTimeRemaining -= Time.deltaTime;
            Utils.Morph(DisguisedPlayer, MeasuredPlayer);

            if (Player.Data.IsDead || DisguisedPlayer.Data.IsDead || DisguisedPlayer.Data.Disconnected || MeetingHud.Instance)
                DisguiserTimeRemaining = 0f;
        }

        public void Delay()
        {
            DisguiserTimeRemaining2 -= Time.deltaTime;

            if (Player.Data.IsDead || DisguisedPlayer.Data.IsDead || DisguisedPlayer.Data.Disconnected || MeetingHud.Instance)
                DisguiserTimeRemaining2 = 0f;
        }

        public void UnDisguise()
        {
            Utils.DefaultOutfit(DisguisedPlayer);
            LastDisguised = DateTime.UtcNow;

            if (CustomGameOptions.DisgCooldownsLinked)
                LastMeasured = DateTime.UtcNow;
        }

        public float DisguiseTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastDisguised;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.DisguiseCooldown, CustomButtons.GetUnderdogChange(Player), CustomGameOptions.MafiosoAbilityCooldownDecrease) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public float MeasureTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastMeasured;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.MeasureCooldown, CustomButtons.GetUnderdogChange(Player), CustomGameOptions.MafiosoAbilityCooldownDecrease) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        //Morphling Stuff
        public AbilityButton MorphButton;
        public DateTime LastMorphed;
        public PlayerControl MorphedPlayer;
        public PlayerControl SampledPlayer;
        public float MorphTimeRemaining;
        public bool Morphed => MorphTimeRemaining > 0f;
        public bool MorphEnabled;
        public DateTime LastSampled;
        public AbilityButton SampleButton;

        public void Morph()
        {
            MorphEnabled = true;
            MorphTimeRemaining -= Time.deltaTime;
            Utils.Morph(Player, MorphedPlayer);

            if (Player.Data.IsDead || MeetingHud.Instance)
                MorphTimeRemaining = 0f;
        }

        public void Unmorph()
        {
            MorphEnabled = false;
            MorphedPlayer = null;
            Utils.DefaultOutfit(Player);
            LastMorphed = DateTime.UtcNow;

            if (CustomGameOptions.MorphCooldownsLinked)
                LastSampled = DateTime.UtcNow;
        }

        public float MorphTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastMorphed;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.MorphlingCd, CustomButtons.GetUnderdogChange(Player), CustomGameOptions.MafiosoAbilityCooldownDecrease) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public float SampleTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastSampled;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.SampleCooldown, CustomButtons.GetUnderdogChange(Player), CustomGameOptions.MafiosoAbilityCooldownDecrease) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        //Wraith Stuff
        public AbilityButton InvisButton;
        public bool InvisEnabled;
        public DateTime LastInvis;
        public float InvisTimeRemaining;
        public bool IsInvis => InvisTimeRemaining > 0f;

        public float InvisTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastInvis;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.InvisCd, CustomButtons.GetUnderdogChange(Player), CustomGameOptions.MafiosoAbilityCooldownDecrease) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        public void Invis()
        {
            InvisEnabled = true;
            InvisTimeRemaining -= Time.deltaTime;

            if (Player.Data.IsDead || MeetingHud.Instance)
                InvisTimeRemaining = 0f;

            var color = new Color32(0, 0, 0, 0);

            if (PlayerControl.LocalPlayer.Is(Faction) || PlayerControl.LocalPlayer.Data.IsDead)
                color.a = 26;

            if (Player.GetCustomOutfitType() != CustomPlayerOutfitType.Invis)
            {
                Player.SetOutfit(CustomPlayerOutfitType.Invis, new GameData.PlayerOutfit()
                {
                    ColorId = Player.CurrentOutfit.ColorId,
                    HatId = "",
                    SkinId = "",
                    VisorId = "",
                    PlayerName = " "
                });

                Player.MyRend().color = color;
                Player.NameText().color = new Color32(0, 0, 0, 0);
                Player.cosmetics.colorBlindText.color = new Color32(0, 0, 0, 0);
            }
        }

        public void Uninvis()
        {
            InvisEnabled = false;
            LastInvis = DateTime.UtcNow;
            Utils.DefaultOutfit(Player);
            Player.MyRend().color = new Color32(255, 255, 255, 255);
        }

        //Consigliere Stuff
        public List<byte> Investigated = new();
        public AbilityButton InvestigateButton;
        public DateTime LastInvestigated;

        public float ConsigliereTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastInvestigated;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.ConsigCd, CustomButtons.GetUnderdogChange(Player), CustomGameOptions.MafiosoAbilityCooldownDecrease) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        //Miner Stuff
        public readonly List<Vent> Vents = new();
        public AbilityButton MineButton;
        public DateTime LastMined;
        public bool CanPlace;

        public float MineTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastMined;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.MineCd, CustomButtons.GetUnderdogChange(Player), CustomGameOptions.MafiosoAbilityCooldownDecrease) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        //Teleporter Stuff
        public AbilityButton TeleportButton;
        public DateTime LastTeleport;
        public Vector3 TeleportPoint = new(0, 0, 0);
        public DateTime LastMarked;
        public AbilityButton MarkButton;
        public bool CanMark;

        public float MarkTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastMarked;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.MarkCooldown, CustomButtons.GetUnderdogChange(Player), CustomGameOptions.MafiosoAbilityCooldownDecrease) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public float TeleportTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastTeleport;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.TeleportCd, CustomButtons.GetUnderdogChange(Player), CustomGameOptions.MafiosoAbilityCooldownDecrease) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public static void Teleport(PlayerControl godfather)
        {
            godfather.MyPhysics.ResetMoveState();
            var teleporterRole = GetRole<Godfather>(godfather);
            var position = teleporterRole.TeleportPoint;
            godfather.NetTransform.SnapTo(new Vector2(position.x, position.y));

            if (SubmergedCompatibility.IsSubmerged() && PlayerControl.LocalPlayer.PlayerId == godfather.PlayerId)
            {
                SubmergedCompatibility.ChangeFloor(godfather.GetTruePosition().y > -7);
                SubmergedCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
            }

            if (PlayerControl.LocalPlayer.PlayerId == godfather.PlayerId)
            {
                Utils.Flash(Colors.Teleporter, "You have moved to a different location!");

                if (Minigame.Instance)
                    Minigame.Instance.Close();
            }

            godfather.moveable = true;
            godfather.Collider.enabled = true;
            godfather.NetTransform.enabled = true;
        }

        //Time Master Stuff
        public AbilityButton FreezeButton;
        public bool FreezeEnabled;
        public float FreezeTimeRemaining;
        public DateTime LastFrozen;
        public bool Frozen => FreezeTimeRemaining > 0f;

        public float FreezeTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastFrozen;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.FreezeCooldown, CustomButtons.GetUnderdogChange(Player), CustomGameOptions.MafiosoAbilityCooldownDecrease) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void TimeFreeze()
        {
            FreezeEnabled = true;
            FreezeTimeRemaining -= Time.deltaTime;

            if (MeetingHud.Instance)
                FreezeTimeRemaining = 0f;
        }

        public void Unfreeze()
        {
            FreezeEnabled = false;
            LastFrozen = DateTime.UtcNow;
            Freeze.FreezeFunctions.UnfreezeAll();
        }

        //Ambusher Stuff
        public bool AmbushEnabled;
        public DateTime LastAmbushed;
        public float AmbushTimeRemaining;
        public bool OnAmbush => AmbushTimeRemaining > 0f;
        public PlayerControl AmbushedPlayer;
        public PlayerControl ClosestAmbush;
        public AbilityButton AmbushButton;

        public float AmbushTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastAmbushed;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.AmbushCooldown) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        public void Ambush()
        {
            AmbushEnabled = true;
            AmbushTimeRemaining -= Time.deltaTime;

            if (Player.Data.IsDead || AmbushedPlayer.Data.IsDead || AmbushedPlayer.Data.Disconnected || MeetingHud.Instance)
                AmbushTimeRemaining = 0f;
        }

        public void UnAmbush()
        {
            AmbushEnabled = false;
            LastAmbushed = DateTime.UtcNow;
            AmbushedPlayer = null;
        }
    }
}