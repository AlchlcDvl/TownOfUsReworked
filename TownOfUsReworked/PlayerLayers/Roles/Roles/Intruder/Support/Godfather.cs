using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using Il2CppSystem.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using TownOfUsReworked.PlayerLayers.Modifiers;
using Reactor.Utilities;
using TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.TimeMasterMod;

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
            RoleDescription = "You are the Godfather! You are the leader of the Intruders. You can promote a fellow Intruder into becoming your Mafioso." +
                " When you die, the Mafioso will become the new Godfather and will inherit stronger variations of their former role!";
            //IntroSound = "GodfatherIntro";
        }

        //Godfather Stuff
        public PlayerControl ClosestIntruder;
        public bool HasDeclared = false;
        public bool WasMafioso = false;
        public Role FormerRole = null;
        public AbilityButton DeclareButton;
        public DateTime LastDeclared;
        public DeadBody CurrentTarget;
        public PlayerControl ClosestTarget;

        public bool TryGetModifiedAppearance(out VisualAppearance appearance)
        {
            if (Disguised)
            {
                appearance = MeasuredPlayer.GetDefaultAppearance();
                var modifier = Modifier.GetModifier(MeasuredPlayer);

                if (modifier is IVisualAlteration alteration)
                    alteration.TryGetModifiedAppearance(out appearance);
                
                return true;
            }
            else if (Morphed)
            {
                appearance = MorphedPlayer.GetDefaultAppearance();
                var modifier = Modifier.GetModifier(MorphedPlayer);

                if (modifier is IVisualAlteration alteration)
                    alteration.TryGetModifiedAppearance(out appearance);

                return true;
            }

            appearance = Player.GetDefaultAppearance();
            return false;
        }

        //Blackmailer Stuff
        public AbilityButton BlackmailButton;
        public PlayerControl BlackmailedPlayer = null;
        public DateTime LastBlackmailed;
        public bool Blackmailed => BlackmailedPlayer != null;
        public PlayerControl ClosestBlackmail;
        
        public float BlackmailTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastBlackmailed;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.BlackmailCd, Utils.GetUnderdogChange(Player), CustomGameOptions.MafiosoAbilityCooldownDecrease) * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        //Camouflager Stuff
        public AbilityButton CamouflageButton;
        public bool CamoEnabled = false;
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
            var timeSpan = utcNow - LastCamouflaged;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.CamouflagerCd, Utils.GetUnderdogChange(Player), CustomGameOptions.MafiosoAbilityCooldownDecrease) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        //Grenadier Stuff
        public AbilityButton FlashButton;
        public bool FlashEnabled = false;
        public DateTime LastFlashed;
        public float FlashTimeRemaining;
        public static List<PlayerControl> ClosestPlayers;
        static readonly Color NormalVision = new Color32(212, 212, 212, 0);
        static readonly Color DimVision = new Color32(212, 212, 212, 51);
        static readonly Color BlindVision = new Color32(212, 212, 212, 255);
        public List<PlayerControl> FlashedPlayers;
        public bool Flashed => FlashTimeRemaining > 0f;

        public float FlashTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastFlashed;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.GrenadeCd, Utils.GetUnderdogChange(Player), CustomGameOptions.MafiosoAbilityCooldownDecrease) * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Flash()
        {
            if (FlashEnabled != true)
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
                    ((Renderer)HudManager.Instance.FullScreen).enabled = true;
                    ((Renderer)HudManager.Instance.FullScreen).gameObject.active = true;

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
                        ((Renderer)HudManager.Instance.FullScreen).enabled = true;
                        ((Renderer)HudManager.Instance.FullScreen).gameObject.active = true;

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

            if (FlashTimeRemaining > 0.5f)
            {
                if (PlayerControl.LocalPlayer.Is(Faction.Intruder) && MapBehaviour.Instance.infectedOverlay.SabSystem.Timer < 0.5f)
                    MapBehaviour.Instance.infectedOverlay.SabSystem.Timer = 0.5f;
            }
        }

        private static bool ShouldPlayerBeDimmed(PlayerControl player) => (player.Is(Faction.Intruder) || player.Data.IsDead) && !MeetingHud.Instance;

        private static bool ShouldPlayerBeBlinded(PlayerControl player) => !(player.Is(Faction.Intruder) || player.Data.IsDead || MeetingHud.Instance);

        public void UnFlash()
        {
            FlashEnabled = false;
            LastFlashed = DateTime.UtcNow;
            ((Renderer)HudManager.Instance.FullScreen).enabled = true;
            HudManager.Instance.FullScreen.color = NormalVision;
            FlashedPlayers.Clear();
        }

        //Janitor Stuff
        public AbilityButton CleanButton;
        public DateTime LastCleaned;
        
        public float CleanTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastCleaned;
            var num = Utils.GetModifiedCooldown((Utils.LastImp() && CustomGameOptions.SoloBoost ? (CustomGameOptions.JanitorCleanCd - CustomGameOptions.UnderdogKillBonus) :
                CustomGameOptions.JanitorCleanCd), Utils.GetUnderdogChange(Player), CustomGameOptions.MafiosoAbilityCooldownDecrease) * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        //Undertaker Stuff
        public AbilityButton DragButton;
        public AbilityButton DropButton;
        public DateTime LastDragged;
        public DeadBody CurrentlyDragging;

        public float DragTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastDragged;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.DragCd, Utils.GetUnderdogChange(Player)) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        //Disguiser Stuff
        public AbilityButton DisguiseButton;
        public DateTime LastDisguised;
        public PlayerControl MeasuredPlayer;
        public float DisguiserTimeRemaining;
        public bool Disguised => DisguiserTimeRemaining > 0f;
        public float DisguiserTimeRemaining2;
        public bool DelayActive => DisguiserTimeRemaining2 > 0f;
        public bool DisguiserEnabled = false;
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
            var timeSpan = utcNow - LastDisguised;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.DisguiseCooldown, Utils.GetUnderdogChange(Player), CustomGameOptions.MafiosoAbilityCooldownDecrease) * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public float MeasureTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastMeasured;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.MeasureCooldown, Utils.GetUnderdogChange(Player), CustomGameOptions.MafiosoAbilityCooldownDecrease) * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
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
            var timeSpan = utcNow - LastMorphed;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.MorphlingCd, Utils.GetUnderdogChange(Player), CustomGameOptions.MafiosoAbilityCooldownDecrease) * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public float SampleTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastSampled;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.SampleCooldown, Utils.GetUnderdogChange(Player), CustomGameOptions.MafiosoAbilityCooldownDecrease) * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
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
            var timeSpan = utcNow - LastInvis;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.InvisCd, Utils.GetUnderdogChange(Player), CustomGameOptions.MafiosoAbilityCooldownDecrease) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
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

                Player.myRend().color = color;
                Player.NameText().color = new Color32(0, 0, 0, 0);
                Player.cosmetics.colorBlindText.color = new Color32(0, 0, 0, 0);
            }
        }

        public void Uninvis()
        {
            InvisEnabled = false;
            LastInvis = DateTime.UtcNow;
            Utils.DefaultOutfit(Player);
            Player.myRend().color = new Color32(255, 255, 255, 255);
        }

        //Consigliere Stuff
        public List<byte> Investigated = new List<byte>();
        public AbilityButton InvestigateButton;
        public DateTime LastInvestigated;

        public float ConsigliereTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastInvestigated;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.ConsigCd, Utils.GetUnderdogChange(Player), CustomGameOptions.MafiosoAbilityCooldownDecrease) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        //Miner Stuff
        public readonly System.Collections.Generic.List<Vent> Vents = new System.Collections.Generic.List<Vent>();
        public AbilityButton MineButton;
        public DateTime LastMined;
        public bool CanPlace;

        public float MineTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastMined;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.MineCd, Utils.GetUnderdogChange(Player), CustomGameOptions.MafiosoAbilityCooldownDecrease) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        //Teleporter Stuff
        public AbilityButton TeleportButton;
        public DateTime LastTeleport;
        public Vector3 TeleportPoint = new Vector3(0, 0, 0);
        public DateTime LastMarked;
        public AbilityButton MarkButton;
        public bool CanMark;

        public float MarkTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastMarked;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.MarkCooldown, Utils.GetUnderdogChange(Player), CustomGameOptions.MafiosoAbilityCooldownDecrease) * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public float TeleportTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastTeleport;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.TeleportCd, Utils.GetUnderdogChange(Player), CustomGameOptions.MafiosoAbilityCooldownDecrease) * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public static void Teleport(PlayerControl teleporter)
        {
            teleporter.MyPhysics.ResetMoveState();
            var teleporterRole = Role.GetRole<Teleporter>(teleporter);
            var position = teleporterRole.TeleportPoint;
            teleporter.NetTransform.SnapTo(new Vector2(position.x, position.y));

            if (SubmergedCompatibility.isSubmerged())
            {
                if (PlayerControl.LocalPlayer.PlayerId == teleporter.PlayerId)
                {
                    SubmergedCompatibility.ChangeFloor(teleporter.GetTruePosition().y > -7);
                    SubmergedCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
                }
            }

            if (PlayerControl.LocalPlayer.PlayerId == teleporter.PlayerId)
            {
                Coroutines.Start(Utils.FlashCoroutine(new Color(0.6f, 0.1f, 0.2f, 1f)));

                if (Minigame.Instance)
                    Minigame.Instance.Close();
            }

            teleporter.moveable = true;
            teleporter.Collider.enabled = true;
            teleporter.NetTransform.enabled = true;
        }

        //Time Master Stuff
        public AbilityButton FreezeButton;
        public bool FreezeEnabled = false;
        public float FreezeTimeRemaining;
        public DateTime LastFrozen;
        public bool Frozen => FreezeTimeRemaining > 0f;
        
        public float FreezeTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastFrozen;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.FreezeCooldown, Utils.GetUnderdogChange(Player), CustomGameOptions.MafiosoAbilityCooldownDecrease) * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
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
            var timeSpan = utcNow - LastAmbushed;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.AmbushCooldown) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
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