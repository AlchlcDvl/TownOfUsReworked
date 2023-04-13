using TownOfUsReworked.Data;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using Il2CppSystem.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Functions;
using TownOfUsReworked.Custom;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class PromotedGodfather : IntruderRole
    {
        public PromotedGodfather(PlayerControl player) : base(player)
        {
            Name = "PromotedGodfather";
            RoleType = RoleEnum.PromotedGodfather;
            StartText = "Promote Your Fellow <color=#FF0000FF>Intruders</color> To Do Better";
            AbilitiesText = "- You can promote a fellow <color=#FF0000FF>Intruder</color> into becoming your successor\n- Promoting an <color=#FF0000FF>Intruder</color> turns them " +
                "into a <color=#6400FFFF>Mafioso</color>\n- If you die, the <color=#6400FFFF>Mafioso</color> will become the new <color=#404C08FF>PromotedGodfather</color>\nand inherits better " +
                $"abilities of their former role\n{AbilitiesText}";
            Color = CustomGameOptions.CustomIntColors ? Colors.Godfather : Colors.Intruder;
            RoleAlignment = RoleAlignment.IntruderSupport;
            AlignmentName = IS;
            BlockMenu = new CustomMenu(Player, new CustomMenu.Select(ConsClick));
        }

        //PromotedGodfather Stuff
        public Role FormerRole;
        public DeadBody CurrentTarget;
        public PlayerControl ClosestTarget;

        //Blackmailer Stuff
        public AbilityButton BlackmailButton;
        public PlayerControl BlackmailedPlayer;
        public DateTime LastBlackmailed;
        public bool Blackmailed => BlackmailedPlayer != null;

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
                if (PlayerControl.LocalPlayer == player)
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

                    if (MapBehaviour.Instance)
                        MapBehaviour.Instance.Close();

                    if (Minigame.Instance)
                        Minigame.Instance.Close();
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
        public DateTime LastDragged;
        public AbilityButton DragButton;
        public AbilityButton DropButton;
        public DeadBody CurrentlyDragging;

        public float DragTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastDragged;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.DragCd, CustomButtons.GetUnderdogChange(Player)) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        public static void DragBody(PlayerControl __instance)
        {
            if (!__instance.Is(RoleEnum.PromotedGodfather))
                return;

            var role = GetRole<PromotedGodfather>(__instance);

            if (role.FormerRole?.RoleType != RoleEnum.Janitor)
                return;

            var body = role.CurrentlyDragging;

            if (body == null)
                return;

            if (__instance.Data.IsDead)
            {
                role.CurrentlyDragging = null;

                foreach (var component in body?.bodyRenderers)
                    component.material.SetFloat("_Outline", 0f);

                return;
            }

            var truePosition = __instance.GetTruePosition();
            var velocity = __instance.gameObject.GetComponent<Rigidbody2D>().velocity.normalized;
            Vector3 newPos = ((Vector2)__instance.transform.position) - (velocity / 3f) + body.myCollider.offset;
            newPos.z = 0.02f;

            //WHY ARE THERE DIFFERENT LOCAL Z INDEXS FOR DIFFERENT DECALS ON DIFFERENT LEVELS?!?!?!
            //AD: idk ¯\_(ツ)_/¯
            if (SubmergedCompatibility.IsSubmerged())
            {
                if (newPos.y > -7f)
                    newPos.z = 0.0208f;
                else
                    newPos.z = -0.0273f;
            }

            if (!PhysicsHelpers.AnythingBetween(truePosition, newPos, Constants.ShipAndObjectsMask, false))
                body.transform.position = newPos;

            if (!__instance.AmOwner)
                return;

            foreach (var component in body?.bodyRenderers)
            {
                component.material.SetColor("_OutlineColor", UnityEngine.Color.green);
                component.material.SetFloat("_Outline", 1f);
            }

            __instance.moveable = true;
        }

        public float CleanTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastCleaned;
            var num = CustomButtons.GetModifiedCooldown(ConstantVariables.LastImp && CustomGameOptions.SoloBoost ? (CustomGameOptions.JanitorCleanCd - CustomGameOptions.UnderdogKillBonus) :
                CustomGameOptions.JanitorCleanCd, CustomButtons.GetUnderdogChange(Player), CustomGameOptions.MafiosoAbilityCooldownDecrease) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
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
            Freeze.UnfreezeAll();
        }

        //Ambusher Stuff
        public bool AmbushEnabled;
        public DateTime LastAmbushed;
        public float AmbushTimeRemaining;
        public bool OnAmbush => AmbushTimeRemaining > 0f;
        public PlayerControl AmbushedPlayer;
        public AbilityButton AmbushButton;

        public float AmbushTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastAmbushed;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.AmbushCooldown, CustomButtons.GetUnderdogChange(Player), CustomGameOptions.MafiosoAbilityCooldownDecrease) * 1000f;
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

        //Consort Stuff
        public DateTime LastBlock;
        public float BlockTimeRemaining;
        public AbilityButton BlockButton;
        public PlayerControl BlockTarget;
        public bool BlockEnabled;
        public bool Blocking => BlockTimeRemaining > 0f;
        public CustomMenu BlockMenu;

        public void UnBlock()
        {
            BlockEnabled = false;

            foreach (var layer in GetLayers(BlockTarget))
                layer.IsBlocked = false;

            BlockTarget = null;
            LastBlock = DateTime.UtcNow;
            Utils.DefaultOutfit(Player);
        }

        public void Block()
        {
            BlockEnabled = true;
            BlockTimeRemaining -= Time.deltaTime;

            if (Player.Data.IsDead || BlockTarget.Data.IsDead || BlockTarget.Data.Disconnected || MeetingHud.Instance || !BlockTarget.IsBlocked())
                BlockTimeRemaining = 0f;
        }

        public float RoleblockTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastBlock;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.ConsRoleblockCooldown, CustomGameOptions.MafiosoAbilityCooldownDecrease, CustomButtons.GetUnderdogChange(Player))
                * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        public void ConsClick(PlayerControl player)
        {
            var interact = Utils.Interact(Player, player);

            if (interact[3])
                BlockTarget = player;
            else if (interact[1])
                LastBlock.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        //Enforcer Stuff
        public AbilityButton BombButton;
        public PlayerControl BombedPlayer;
        public bool BombEnabled;
        public DateTime LastBombed;
        public float BombTimeRemaining;
        public float BombTimeRemaining2;
        public bool Bombing => BombTimeRemaining > 0f;
        public bool BombDelayActive => BombTimeRemaining2 > 0f;
        public bool BombSuccessful;

        public float BombTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastBombed;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.EnforceCooldown, CustomGameOptions.MafiosoAbilityCooldownDecrease) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        public void Boom()
        {
            if (!BombEnabled && PlayerControl.LocalPlayer == BombedPlayer)
            {
                Utils.Flash(Color, "There's a bomb on you!", 2);
                GetRole(BombedPlayer).Bombed = true;
            }

            BombEnabled = true;
            BombTimeRemaining -= Time.deltaTime;

            if (Player.Data.IsDead || MeetingHud.Instance || BombSuccessful)
                BombTimeRemaining = 0f;
        }

        public void BombDelay()
        {
            BombTimeRemaining2 -= Time.deltaTime;

            if (Player.Data.IsDead || MeetingHud.Instance)
                BombTimeRemaining2 = 0f;
        }

        public void Unboom()
        {
            BombEnabled = false;
            LastBombed = DateTime.UtcNow;
            GetRole(BombedPlayer).Bombed = false;

            if (!BombSuccessful)
                Explode();

            BombedPlayer = null;
        }

        private void Explode()
        {
            foreach (var player in Utils.GetClosestPlayers(BombedPlayer.GetTruePosition(), CustomGameOptions.EnforceRadius))
            {
                Utils.Spread(BombedPlayer, player);

                if (player.IsVesting() || player.IsProtected() || player.IsOnAlert() || player.IsShielded() || player.IsRetShielded())
                    continue;

                if (!player.Is(RoleEnum.Pestilence))
                    Utils.RpcMurderPlayer(BombedPlayer, player, DeathReasonEnum.Bombed, false);
            }
        }
    }
}