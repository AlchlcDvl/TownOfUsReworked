using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Classes;
using Il2CppSystem.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using TownOfUsReworked.PlayerLayers.Modifiers;
using TownOfUsReworked.PlayerLayers.Abilities;
using TownOfUsReworked.PlayerLayers.Abilities.Abilities;
using TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.ConsigliereMod;
using Reactor.Utilities;
using TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.TimeMasterMod;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Godfather : Role
    {
        public Godfather(PlayerControl player) : base(player)
        {
            Name = "Godfather";
            Faction = Faction.Intruder;
            RoleType = RoleEnum.Godfather;
            StartText = "Promote Your Fellow <color=#FF0000FF>Intruders</color> To Do Better";
            AbilitiesText = "- You can promote a fellow <color=#FF0000FF>Intruder</color> into becoming your successor.";
            AttributesText = "- Promoting an <color=#FF0000FF>Intruder</color> turns them into a <color=#6400FFFF>Mafioso</color>.\n- If you die, " +
                "the <color=#6400FFFF>Mafioso</color> become the new <color=#404C08FF>Godfather</color>\nand inherits better abilities of their former" +
                " role.";
            Color = CustomGameOptions.CustomIntColors ? Colors.Godfather : Colors.Intruder;
            FactionName = "Intruder";
            FactionColor = Colors.Intruder;
            RoleAlignment = RoleAlignment.IntruderSupport;
            AlignmentName = "Intruder (Support)";
            Results = InspResults.GFMayorRebelPest;
            FactionDescription = IntruderFactionDescription;
            Objectives = IntrudersWinCon;
            AlignmentDescription = ISDescription;
            RoleDescription = "You are the Godfather! You are the leader of the Intruders. You can promote a fellow Intruder into becoming your Mafioso." +
                " When you die, the Mafioso will become the new Godfather and will inherit stronger variations of their former role!";
        }

        //Godfather Stuff
        public PlayerControl ClosestPlayer;
        public bool HasDeclared = false;
        public bool WasMafioso = false;
        public Role FormerRole = null;
        private KillButton _declareButton;
        private KillButton _killButton;
        public DateTime LastKilled { get; set; }
        public DateTime LastDeclared;
        public DeadBody CurrentTarget { get; set; }

        public override void Loses()
        {
            LostByRPC = true;
        }

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

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastKilled;
            var num = (WasMafioso ? (CustomGameOptions.IntKillCooldown - CustomGameOptions.MafiosoAbilityCooldownDecrease) : CustomGameOptions.IntKillCooldown) * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public KillButton KillButton
        {
            get => _killButton;
            set
            {
                _killButton = value;
                AddToAbilityButtons(value, this);
            }
        }

        public override void Wins()
        {
            if (IsRecruit)
                CabalWin = true;
            else
                IntruderWin = true;
        }

        internal override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected)
                return true;

            if (IsRecruit)
            {
                if (Utils.CabalWin())
                {
                    Wins();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.CabalWin);
                    writer.Write(Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return false;
                }
            }
            else if (Utils.IntrudersWin())
            {
                Wins();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable, -1);
                writer.Write((byte)WinLoseRPC.IntruderWin);
                writer.Write(Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            
            return false;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__32 __instance)
        {
            if (Player != PlayerControl.LocalPlayer)
                return;
                
            var team = new List<PlayerControl>();

            team.Add(PlayerControl.LocalPlayer);

            if (!IsRecruit)
            {
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player.Is(Faction) && player != PlayerControl.LocalPlayer)
                        team.Add(player);
                }
            }
            else
            {
                var jackal = Player.GetJackal();

                team.Add(jackal.Player);
                team.Add(jackal.GoodRecruit);
            }

            __instance.teamToShow = team;
        }

        public KillButton DeclareButton
        {
            get => _declareButton;
            set
            {
                _declareButton = value;
                AddToAbilityButtons(value, this);
            }
        }

        //Blackmailer Stuff
        private KillButton _blackmailButton;
        public PlayerControl Blackmailed = null;
        public DateTime LastBlackmailed;
        public bool blackmailed => Blackmailed != null;

        public KillButton BlackmailButton
        {
            get => _blackmailButton;
            set
            {
                _blackmailButton = value;
                AddToAbilityButtons(value, this);
            }
        }
        
        public float BlackmailTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastBlackmailed;
            var num = (CustomGameOptions.BlackmailCd - CustomGameOptions.MafiosoAbilityCooldownDecrease) * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        //Camouflager Stuff
        private KillButton _camouflageButton;
        public bool CamoEnabled = false;
        public DateTime LastCamouflaged { get; set; }
        public float CamoTimeRemaining;
        public bool Camouflaged => CamoTimeRemaining > 0f;

        public KillButton CamouflageButton
        {
            get => _camouflageButton;
            set
            {
                _camouflageButton = value;
                AddToAbilityButtons(value, this);
            }
        }

        public void Camouflage()
        {
            CamoEnabled = true;
            CamoTimeRemaining -= Time.deltaTime;
            Utils.Camouflage();
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
            var num = (CustomGameOptions.CamouflagerCd - CustomGameOptions.MafiosoAbilityCooldownDecrease) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        //Grenadier Stuff
        private KillButton _flashButton;
        public bool FlashEnabled = false;
        public DateTime LastFlashed;
        public float FlashTimeRemaining;
        public static List<PlayerControl> closestPlayers = null;
        static readonly Color normalVision = new Color32(212, 212, 212, 0);
        static readonly Color dimVision = new Color32(212, 212, 212, 51);
        static readonly Color blindVision = new Color32(212, 212, 212, 255);
        public List<PlayerControl> flashedPlayers = new List<PlayerControl>();
        public bool Flashed => FlashTimeRemaining > 0f;

        public KillButton FlashButton
        {
            get => _flashButton;
            set
            {
                _flashButton = value;
                AddToAbilityButtons(value, this);
            }
        }

        public float FlashTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastFlashed;
            var num = (CustomGameOptions.GrenadeCd - CustomGameOptions.MafiosoAbilityCooldownDecrease) * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Flash()
        {
            if (FlashEnabled == false)
            {
                closestPlayers = Utils.GetClosestPlayers(Player.GetTruePosition(), CustomGameOptions.FlashRadius);
                flashedPlayers = closestPlayers;
            }

            FlashEnabled = true;
            FlashTimeRemaining -= Time.deltaTime;

            //To stop the scenario where the flash and sabotage are called at the same time.
            var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
            var specials = system.specials.ToArray();
            var dummyActive = system.dummy.IsActive;
            var sabActive = specials.Any(s => s.IsActive);

            foreach (var player in closestPlayers)
            {
                if (PlayerControl.LocalPlayer.PlayerId == player.PlayerId)
                {
                    if (FlashTimeRemaining > CustomGameOptions.GrenadeDuration - 0.5f && (!sabActive || dummyActive))
                    {
                        float fade = (FlashTimeRemaining - CustomGameOptions.GrenadeDuration) * -2.0f;

                        if (ShouldPlayerBeBlinded(player))
                        {
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).gameObject.active = true;
                            DestroyableSingleton<HudManager>.Instance.FullScreen.color = Color32.Lerp(normalVision, blindVision, fade);
                        }
                        else if (ShouldPlayerBeDimmed(player))
                        {
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).gameObject.active = true;
                            DestroyableSingleton<HudManager>.Instance.FullScreen.color = Color32.Lerp(normalVision, dimVision, fade);

                            if (PlayerControl.LocalPlayer.Is(Faction.Intruder) && MapBehaviour.Instance.infectedOverlay.SabSystem.Timer < 0.5f)
                                MapBehaviour.Instance.infectedOverlay.SabSystem.Timer = 0.5f;
                        }
                        else
                        {
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).gameObject.active = true;
                            DestroyableSingleton<HudManager>.Instance.FullScreen.color = normalVision;
                        }
                    }
                    else if (FlashTimeRemaining <= (CustomGameOptions.GrenadeDuration - 0.5f) && FlashTimeRemaining >= 0.5f && (!sabActive || dummyActive))
                    {
                        if (ShouldPlayerBeBlinded(player))
                        {
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).gameObject.active = true;
                            DestroyableSingleton<HudManager>.Instance.FullScreen.color = blindVision;
                        }
                        else if (ShouldPlayerBeDimmed(player))
                        {
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).gameObject.active = true;
                            DestroyableSingleton<HudManager>.Instance.FullScreen.color = dimVision;

                            if (PlayerControl.LocalPlayer.Is(Faction.Intruder) && MapBehaviour.Instance.infectedOverlay.SabSystem.Timer < 0.5f)
                                MapBehaviour.Instance.infectedOverlay.SabSystem.Timer = 0.5f;
                        }
                        else
                        {
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).gameObject.active = true;
                            DestroyableSingleton<HudManager>.Instance.FullScreen.color = normalVision;
                        }
                    }
                    else if (FlashTimeRemaining < 0.5f && (!sabActive || dummyActive))
                    {
                        float fade2 = (FlashTimeRemaining * -2.0f) + 1.0f;

                        if (ShouldPlayerBeBlinded(player))
                        {
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).gameObject.active = true;
                            DestroyableSingleton<HudManager>.Instance.FullScreen.color = Color32.Lerp(blindVision, normalVision, fade2);
                        }
                        else if (ShouldPlayerBeDimmed(player))
                        {
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).gameObject.active = true;
                            DestroyableSingleton<HudManager>.Instance.FullScreen.color = Color32.Lerp(dimVision, normalVision, fade2);

                            if (PlayerControl.LocalPlayer.Is(Faction.Intruder) && MapBehaviour.Instance.infectedOverlay.SabSystem.Timer < 0.5f)
                                MapBehaviour.Instance.infectedOverlay.SabSystem.Timer = 0.5f;
                        }
                        else
                        {
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).gameObject.active = true;
                            DestroyableSingleton<HudManager>.Instance.FullScreen.color = normalVision;
                        }
                    }
                    else
                    {
                        ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
                        ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).gameObject.active = true;
                        DestroyableSingleton<HudManager>.Instance.FullScreen.color = normalVision;
                        FlashTimeRemaining = 0.0f;
                    }
                }
            }

            if (FlashTimeRemaining > 0.5f)
            {
                if (PlayerControl.LocalPlayer.Is(Faction.Intruder) && MapBehaviour.Instance.infectedOverlay.SabSystem.Timer < 0.5f)
                    MapBehaviour.Instance.infectedOverlay.SabSystem.Timer = 0.5f;
            }
        }

        private static bool ShouldPlayerBeDimmed(PlayerControl player)
        {
            return (player.Is(Faction.Intruder) || player.Data.IsDead) && !MeetingHud.Instance;
        }

        private static bool ShouldPlayerBeBlinded(PlayerControl player)
        {
            return !player.Is(Faction.Intruder) && !player.Data.IsDead && !MeetingHud.Instance;
        }

        public void UnFlash()
        {
            FlashEnabled = false;
            LastFlashed = DateTime.UtcNow;
            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
            DestroyableSingleton<HudManager>.Instance.FullScreen.color = normalVision;
            flashedPlayers.Clear();
        }

        //Janitor Stuff
        private KillButton _cleanButton;
        public DateTime LastCleaned;

        public KillButton CleanButton
        {
            get => _cleanButton;
            set
            {
                _cleanButton = value;
                AddToAbilityButtons(value, this);
            }
        }
        
        public float CleanTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastCleaned;
            var num = ((Utils.LastImp() ? (CustomGameOptions.JanitorCleanCd - CustomGameOptions.UnderdogKillBonus) : CustomGameOptions.JanitorCleanCd) -
                CustomGameOptions.MafiosoAbilityCooldownDecrease) * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        //Undertaker Stuff
        private KillButton _dragDropButton;
        public DateTime LastDragged { get; set; }
        public DeadBody CurrentlyDragging { get; set; }

        public KillButton DragDropButton
        {
            get => _dragDropButton;
            set
            {
                _dragDropButton = value;
                AddToAbilityButtons(value, this);
            }
        }

        public float DragTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastDragged;
            var num = (CustomGameOptions.DragCd - CustomGameOptions.MafiosoAbilityCooldownDecrease) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        //Disguiser Stuff
        private KillButton _disguiseButton;
        public DateTime LastDisguised;
        public PlayerControl MeasuredPlayer;
        public float TimeBeforeDisguised { get; set; }
        public float DisguiseTimeRemaining { get; set; }
        public float DisguiserTimeRemaining;
        public bool Disguised => DisguiserTimeRemaining > 0f;
        public bool DisguiserEnabled = false;
        public Sprite MeasureSprite => TownOfUsReworked.MeasureSprite;

        public KillButton DisguiseButton
        {
            get => _disguiseButton;
            set
            {
                _disguiseButton = value;
                AddToAbilityButtons(value, this);
            }
        }

        public void Disguise()
        {
            if (ClosestPlayer == null || MeasuredPlayer == null)
                return;

            DisguiserTimeRemaining -= Time.deltaTime;
            Utils.Morph(MeasuredPlayer, ClosestPlayer);

            if (Player.Data.IsDead)
                DisguiserTimeRemaining = 0f;
        }

        public void UnDisguise()
        {
            Utils.DefaultOutfit(MeasuredPlayer);
            MeasuredPlayer = null;
            LastDisguised = DateTime.UtcNow;
            DisguiseButton.graphic.sprite = MeasureSprite;
        }

        public float DisguiseTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastDisguised;
            var num = (CustomGameOptions.DisguiseCooldown - CustomGameOptions.MafiosoAbilityCooldownDecrease) * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        //Morphling Stuff
        private KillButton _morphButton;
        public DateTime LastMorphed;
        public PlayerControl MorphedPlayer;
        public PlayerControl SampledPlayer;
        public float MorphTimeRemaining;
        public bool Morphed => MorphTimeRemaining > 0f;

        public KillButton MorphButton
        {
            get => _morphButton;
            set
            {
                _morphButton = value;
                AddToAbilityButtons(value, this);
            }
        }

        public void Morph()
        {
            MorphTimeRemaining -= Time.deltaTime;
            Utils.Morph(Player, MorphedPlayer);

            if (Player.Data.IsDead)
                MorphTimeRemaining = 0f;
        }

        public void Unmorph()
        {
            MorphedPlayer = null;
            Utils.DefaultOutfit(Player);
            LastMorphed = DateTime.UtcNow;
        }

        public float MorphTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastMorphed;
            var num = (CustomGameOptions.MorphlingCd - CustomGameOptions.MafiosoAbilityCooldownDecrease) * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        //Wraith Stuff
        private KillButton _invisButton;
        public bool InvisEnabled;
        public DateTime LastInvis;
        public float InvisTimeRemaining;
        public bool IsInvis => InvisTimeRemaining > 0f;

        public KillButton InvisButton
        {
            get => _invisButton;
            set
            {
                _invisButton = value;
                AddToAbilityButtons(value, this);
            }
        }

        public float InvisTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastInvis;
            var num = (CustomGameOptions.InvisCd - CustomGameOptions.MafiosoAbilityCooldownDecrease) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Invis()
        {
            InvisEnabled = true;
            InvisTimeRemaining -= Time.deltaTime;
            
            if (Player.Data.IsDead)
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
                Player.nameText().color = new Color32(0, 0, 0, 0);
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

        //COnsigliere Stuff
        public List<byte> Investigated = new List<byte>();
        private KillButton _investigateButton;
        public DateTime LastInvestigated { get; set; }
        public string role = CustomGameOptions.ConsigInfo == ConsigInfo.Role ? "role" : "faction";
        public Ability ability => PlayerControl.LocalPlayer != null && PlayerControl.LocalPlayer.Is(AbilityEnum.Assassin) ? Ability.GetAbility<Assassin>(PlayerControl.LocalPlayer)
            : null;
        public string CanAssassinate => ability != null && CustomGameOptions.ConsigInfo == ConsigInfo.Role ? "You cannot assassinate players you have revealed" : "None";

        public float ConsigliereTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastInvestigated;
            var num = (CustomGameOptions.ConsigCd - CustomGameOptions.MafiosoAbilityCooldownDecrease) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public KillButton InvestigateButton
        {
            get => _investigateButton;
            set
            {
                _investigateButton = value;
                AddToAbilityButtons(value, this);
            }
        }

        //Miner Stuff
        public readonly System.Collections.Generic.List<Vent> Vents = new System.Collections.Generic.List<Vent>();
        private KillButton _mineButton;
        public DateTime LastMined;
        public bool CanPlace { get; set; }
        public Vector2 VentSize { get; set; }

        public KillButton MineButton
        {
            get => _mineButton;
            set
            {
                _mineButton = value;
                AddToAbilityButtons(value, this);
            }
        }

        public float MineTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastMined;
            var num = (CustomGameOptions.MineCd - CustomGameOptions.MafiosoAbilityCooldownDecrease) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        //Teleporter Stuff
        private KillButton _teleportButton;
        public DateTime LastTeleport;
        public Vector3 TeleportPoint;

        public KillButton TeleportButton
        {
            get => _teleportButton;
            set
            {
                _teleportButton = value;
                AddToAbilityButtons(value, this);
            }
        }

        public float TeleportTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastTeleport;
            var num = (CustomGameOptions.TeleportCd - CustomGameOptions.MafiosoAbilityCooldownDecrease) * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

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
        private KillButton _freezeButton;
        public bool FreezeEnabled = false;
        public float FreezeTimeRemaining;
        public DateTime LastFrozen { get; set; }
        public bool Frozen => FreezeTimeRemaining > 0f;
        
        public KillButton FreezeButton
        {
            get => _freezeButton;
            set
            {
                _freezeButton = value;
                AddToAbilityButtons(value, this);
            }
        }
        
        public float FreezeTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastFrozen;
            var num = (CustomGameOptions.FreezeCooldown - CustomGameOptions.MafiosoAbilityCooldownDecrease) * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public void TimeFreeze()
        {
            FreezeEnabled = true;
            FreezeTimeRemaining -= Time.deltaTime;
            Freeze.FreezeFunctions.FreezeAll();
        }

        public void Unfreeze()
        {
            FreezeEnabled = false;
            LastFrozen = DateTime.UtcNow;
            Freeze.FreezeFunctions.UnfreezeAll();
        }
    }
}