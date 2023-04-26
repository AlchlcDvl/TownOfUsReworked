using TownOfUsReworked.Classes;
using TownOfUsReworked.Data;
using HarmonyLib;

namespace TownOfUsReworked.CustomOptions
{
    [HarmonyPatch]
    public static class CustomGameOptions
    {
        //Global Options
        public static bool ConfirmEjects => Generate.ConfirmEjects.Get();
        public static float PlayerSpeed => Generate.PlayerSpeed.Get();
        public static float GhostSpeed => Generate.GhostSpeed.Get();
        public static int InteractionDistance => (int)Generate.InteractionDistance.Get();
        public static int EmergencyButtonCount => (int)Generate.EmergencyButtonCount.Get();
        public static int EmergencyButtonCooldown => (int)Generate.EmergencyButtonCooldown.Get();
        public static float InitialCooldowns => Generate.InitialCooldowns.Get();
        public static float ReportDistance => Generate.ReportDistance.Get();
        public static float ChatCooldown => Generate.ChatCooldown.Get();
        public static int DiscussionTime => (int)Generate.DiscussionTime.Get();
        public static int VotingTime => (int)Generate.VotingTime.Get();
        public static TaskBarMode TaskBarMode => (TaskBarMode)Generate.TaskBarMode.Get();
        public static bool EjectionRevealsRole => Generate.EjectionRevealsRole.Get();
        public static int LobbySize => (int)Generate.LobbySize.Get();

        //Game Modifiers
        public static WhoCanVentOptions WhoCanVent => (WhoCanVentOptions)Generate.WhoCanVent.Get();
        public static bool VisualTasks => Generate.VisualTasks.Get();
        public static bool AnonymousVoting => Generate.AnonymousVoting.Get();
        public static bool FactionSeeRoles => Generate.FactionSeeRoles.Get();
        public static bool ParallelMedScans => Generate.ParallelMedScans.Get();
        public static DisableSkipButtonMeetings SkipButtonDisable => (DisableSkipButtonMeetings)Generate.SkipButtonDisable.Get();
        public static bool NoNames => Generate.NoNames.Get();
        public static bool Whispers => Generate.Whispers.Get();
        public static bool WhispersAnnouncement => Generate.WhispersAnnouncement.Get();
        public static bool AppearanceAnimation => Generate.AppearanceAnimation.Get();
        public static bool LighterDarker => Generate.LighterDarker.Get();
        public static bool RandomSpawns => Generate.RandomSpawns.Get();
        public static bool EnableAbilities => Generate.EnableAbilities.Get();
        public static bool EnableModifiers => Generate.EnableModifiers.Get();
        public static bool EnableObjectifiers => Generate.EnableObjectifiers.Get();
        public static bool VentTargetting => Generate.VentTargetting.Get();

        //Better Sabotage Settings
        public static float ReactorShake => Generate.ReactorShake.Get();
        public static bool OxySlow => Generate.OxySlow.Get();
        public static bool ColourblindComms => Generate.ColourblindComms.Get();
        public static bool MeetingColourblind => Generate.MeetingColourblind.Get();
        public static bool NightVision => Generate.NightVision.Get();
        public static bool EvilsIgnoreNV => Generate.EvilsIgnoreNV.Get();

        //Announcement Settings
        public static bool LocationReports => Generate.LocationReports.Get();
        public static RoleFactionReports RoleFactionReports => (RoleFactionReports)Generate.RoleFactionReports.Get();
        public static RoleFactionReports KillerReports => (RoleFactionReports)Generate.KillerReports.Get();
        public static bool GameAnnouncements => Generate.GameAnnouncements.Get();

        //QOL Changes
        public static bool DeadSeeEverything => Generate.DeadSeeEverything.Get();
        public static bool DisableLevels => Generate.DisableLevels.Get();
        public static bool SeeTasks => Generate.SeeTasks.Get();
        public static bool CustomEject => Generate.CustomEject.Get();
        public static bool WhiteNameplates => Generate.WhiteNameplates.Get();

        //Game Modes
        public static GameMode GameMode => (GameMode)Generate.GameMode.Get();

        //Killing Only Settings
        public static bool AddArsonist => Generate.AddArsonist.Get();
        public static bool AddCryomaniac => Generate.AddCryomaniac.Get();
        public static bool AddPlaguebearer => Generate.AddPlaguebearer.Get();
        public static int NeutralRoles => (int)Generate.NeutralRoles.Get();

        //All Any Settings
        public static bool EnableUniques => Generate.EnableUniques.Get();

        //Map Settings
        public static Map Map => (Map)Generate.Map.Get();
        public static bool RandomMapEnabled => Generate.RandomMapEnabled.Get();
        public static float RandomMapSkeld => Generate.RandomMapSkeld.Get();
        public static float RandomMapMira => Generate.RandomMapMira.Get();
        public static float RandomMapPolus => Generate.RandomMapPolus.Get();
        public static float RandomMapAirship => Generate.RandomMapAirship.Get();
        public static float RandomMapSubmerged => SubmergedCompatibility.Loaded ? Generate.RandomMapSubmerged.Get() : 0f;
        public static float SmallMapDecreasedCooldown => Generate.SmallMapDecreasedCooldown.Get();
        public static float LargeMapIncreasedCooldown => Generate.LargeMapIncreasedCooldown.Get();
        public static int SmallMapIncreasedShortTasks => (int)Generate.SmallMapIncreasedShortTasks.Get();
        public static int SmallMapIncreasedLongTasks => (int)Generate.SmallMapIncreasedLongTasks.Get();
        public static int LargeMapDecreasedShortTasks => (int)Generate.LargeMapDecreasedShortTasks.Get();
        public static int LargeMapDecreasedLongTasks => (int)Generate.LargeMapDecreasedLongTasks.Get();
        public static bool AutoAdjustSettings => Generate.AutoAdjustSettings.Get();
        public static bool SmallMapHalfVision => Generate.SmallMapHalfVision.Get();

        //Polus Settings
        public static bool VitalsLab => Generate.VitalsLab.Get();
        public static bool ColdTempDeathValley => Generate.ColdTempDeathValley.Get();
        public static bool WifiChartCourseSwap => Generate.WifiChartCourseSwap.Get();
        public static bool PolusVentImprovements => Generate.PolusVentImprovements.Get();
        public static float SeismicTimer => Generate.SeismicTimer.Get();

        //Skeld Settings
        public static bool SkeldVentImprovements => Generate.SkeldVentImprovements.Get();
        //public static float ReactorTimer => Generate.ReactorTimer.Get();
        //public static float OxygenTimer => Generate.OxygenTimer.Get();

        //Airship Settings
        public static bool MeetingSpawnChoice => Generate.MeetingSpawnChoice.Get();
        public static bool MoveDivert => Generate.MoveDivert.Get();
        public static bool MoveFuel => Generate.MoveFuel.Get();
        public static bool MoveVitals => Generate.MoveVitals.Get();
        public static bool AddTeleporters => Generate.AddTeleporters.Get();
        public static bool CallPlatform => Generate.CallPlatform.Get();
        public static float MinDoorSwipeTime => Generate.MinDoorSwipeTime.Get();
        //public static float CrashTimer => Generate.CrashTimer.Get();
        public static AirshipSpawnType SpawnType => (AirshipSpawnType)Generate.SpawnType.Get();
        public static MoveAdmin MoveAdmin => (MoveAdmin)Generate.MoveAdmin.Get();
        public static MoveElectrical MoveElectrical => (MoveElectrical)Generate.MoveElectrical.Get();

        //Role Spawn
        public static int MayorOn => (int)Generate.MayorOn.Get();
        public static int JesterOn => (int)Generate.JesterOn.Get();
        public static int SheriffOn => (int)Generate.SheriffOn.Get();
        public static int ShapeshifterOn => (int)Generate.ShapeshifterOn.Get();
        public static int JanitorOn => (int)Generate.JanitorOn.Get();
        public static int EngineerOn => (int)Generate.EngineerOn.Get();
        public static int SwapperOn => (int)Generate.SwapperOn.Get();
        public static int ShifterOn => (int)Generate.ShifterOn.Get();
        public static int AmnesiacOn => (int)Generate.AmnesiacOn.Get();
        public static int ConcealerOn => (int)Generate.ConcealerOn.Get();
        public static int PoliticianOn => (int)Generate.PoliticianOn.Get();
        public static int MedicOn => (int)Generate.MedicOn.Get();
        public static int GlitchOn => (int)Generate.GlitchOn.Get();
        public static int MorphlingOn => (int)Generate.MorphlingOn.Get();
        public static int ExecutionerOn => (int)Generate.ExecutionerOn.Get();
        public static int CrewmateOn => (int)Generate.CrewmateOn.Get();
        public static int ImpostorOn => (int)Generate.ImpostorOn.Get();
        public static int WraithOn => (int)Generate.WraithOn.Get();
        public static int ArsonistOn => (int)Generate.ArsonistOn.Get();
        public static int AltruistOn => (int)Generate.AltruistOn.Get();
        public static int JackalOn => (int)Generate.JackalOn.Get();
        public static int VigilanteOn => (int)Generate.VigilanteOn.Get();
        public static int GrenadierOn => (int)Generate.GrenadierOn.Get();
        public static int VeteranOn => (int)Generate.VeteranOn.Get();
        public static int TrackerOn => (int)Generate.TrackerOn.Get();
        public static int OperativeOn => (int)Generate.OperativeOn.Get();
        public static int PoisonerOn => (int)Generate.PoisonerOn.Get();
        public static int InspectorOn => (int)Generate.InspectorOn.Get();
        public static int EscortOn => (int)Generate.EscortOn.Get();
        public static int GodfatherOn => (int)Generate.GodfatherOn.Get();
        public static int RebelOn => (int)Generate.RebelOn.Get();
        public static int ConsortOn => (int)Generate.ConsortOn.Get();
        public static int TrollOn => (int)Generate.TrollOn.Get();
        public static int TransporterOn => (int)Generate.TransporterOn.Get();
        public static int MediumOn => (int)Generate.MediumOn.Get();
        public static int SurvivorOn => (int)Generate.SurvivorOn.Get();
        public static int GuardianAngelOn => (int)Generate.GuardianAngelOn.Get();
        public static int CoronerOn => (int)Generate.CoronerOn.Get();
        public static int BlackmailerOn => (int)Generate.BlackmailerOn.Get();
        public static int PlaguebearerOn => (int)Generate.PlaguebearerOn.Get();
        public static int JuggernautOn => (int)Generate.JuggernautOn.Get();
        public static int WerewolfOn => (int)Generate.WerewolfOn.Get();
        public static int TeleporterOn => (int)Generate.TeleporterOn.Get();
        public static int SerialKillerOn => (int)Generate.SerialKillerOn.Get();
        public static int DetectiveOn => (int)Generate.DetectiveOn.Get();
        public static int CamouflagerOn => (int)Generate.CamouflagerOn.Get();
        public static int ThiefOn => (int)Generate.ThiefOn.Get();
        public static int CryomaniacOn => (int)Generate.CryomaniacOn.Get();
        public static int DisguiserOn => (int)Generate.DisguiserOn.Get();
        public static int CannibalOn => (int)Generate.CannibalOn.Get();
        public static int VampireHunterOn => (int)Generate.VampireHunterOn.Get();
        public static int BomberOn => (int)Generate.BomberOn.Get();
        public static int FramerOn => (int)Generate.FramerOn.Get();
        public static int MurdererOn => (int)Generate.MurdererOn.Get();
        public static int WarperOn => (int)Generate.WarperOn.Get();
        public static int AnarchistOn => (int)Generate.AnarchistOn.Get();
        public static int DraculaOn => (int)Generate.DraculaOn.Get();
        public static int ConsigliereOn => (int)Generate.ConsigliereOn.Get();
        public static int MinerOn => (int)Generate.MinerOn.Get();
        public static int PhantomOn => (int)Generate.PhantomOn.Get();
        public static int RevealerOn => (int)Generate.RevealerOn.Get();
        public static int RetributionistOn => (int)Generate.RetributionistOn.Get();
        public static int NecromancerOn => (int)Generate.NecromancerOn.Get();
        public static int WhispererOn => (int)Generate.WhispererOn.Get();
        public static int SeerOn => (int)Generate.SeerOn.Get();
        public static int MysticOn => (int)Generate.MysticOn.Get();
        public static int ChameleonOn => (int)Generate.ChameleonOn.Get();
        public static int GuesserOn => (int)Generate.GuesserOn.Get();
        public static int BountyHunterOn => (int)Generate.BountyHunterOn.Get();
        public static int ActorOn => (int)Generate.ActorOn.Get();
        public static int AmbusherOn => (int)Generate.AmbusherOn.Get();
        public static int CrusaderOn => (int)Generate.CrusaderOn.Get();
        public static int BansheeOn => (int)Generate.BansheeOn.Get();
        public static int GhoulOn => (int)Generate.GhoulOn.Get();
        public static int EnforcerOn => (int)Generate.EnforcerOn.Get();

        //Ability Spawn
        public static int CrewAssassinOn => (int)Generate.CrewAssassinOn.Get();
        public static int IntruderAssassinOn => (int)Generate.IntruderAssassinOn.Get();
        public static int SyndicateAssassinOn => (int)Generate.SyndicateAssassinOn.Get();
        public static int NeutralAssassinOn => (int)Generate.NeutralAssassinOn.Get();
        public static int UnderdogOn => (int)Generate.UnderdogOn.Get();
        public static int SnitchOn => (int)Generate.SnitchOn.Get();
        public static int MultitaskerOn => (int)Generate.MultitaskerOn.Get();
        public static int TorchOn => (int)Generate.TorchOn.Get();
        public static int ButtonBarryOn => (int)Generate.ButtonBarryOn.Get();
        public static int TunnelerOn => (int)Generate.TunnelerOn.Get();
        public static int NinjaOn => (int)Generate.NinjaOn.Get();
        public static int RadarOn => (int)Generate.RadarOn.Get();
        public static int TiebreakerOn => (int)Generate.TiebreakerOn.Get();
        public static int InsiderOn => (int)Generate.InsiderOn.Get();
        public static int RuthlessOn => (int)Generate.RuthlessOn.Get();

        //Objectifier Spawn
        public static int RivalsOn => (int)Generate.RivalsOn.Get();
        public static int FanaticOn => (int)Generate.FanaticOn.Get();
        public static int TraitorOn => (int)Generate.TraitorOn.Get();
        public static int TaskmasterOn => (int)Generate.TaskmasterOn.Get();
        public static int CorruptedOn => (int)Generate.CorruptedOn.Get();
        public static int OverlordOn => (int)Generate.OverlordOn.Get();
        public static int LoversOn => (int)Generate.LoversOn.Get();
        public static int AlliedOn => (int)Generate.AlliedOn.Get();
        public static int MafiaOn => (int)Generate.MafiaOn.Get();

        //Modifier Spawn
        public static int ProfessionalOn => (int)Generate.ProfessionalOn.Get();
        public static int FlincherOn => (int)Generate.FlincherOn.Get();
        public static int DiseasedOn => (int)Generate.DiseasedOn.Get();
        public static int GiantOn => (int)Generate.GiantOn.Get();
        public static int DwarfOn => (int)Generate.DwarfOn.Get();
        public static int BaitOn => (int)Generate.BaitOn.Get();
        public static int CowardOn => (int)Generate.CowardOn.Get();
        public static int DrunkOn => (int)Generate.DrunkOn.Get();
        public static int VolatileOn => (int)Generate.VolatileOn.Get();
        public static int VIPOn => (int)Generate.VIPOn.Get();
        public static int ShyOn => (int)Generate.ShyOn.Get();
        public static int IndomitableOn => (int)Generate.IndomitableOn.Get();

        //Crew Options
        public static float CrewVision => Generate.CrewVision.Get();
        public static int ShortTasks => (int)Generate.ShortTasks.Get();
        public static int LongTasks => (int)Generate.LongTasks.Get();
        public static int CommonTasks => (int)Generate.CommonTasks.Get();
        public static bool GhostTasksCountToWin => Generate.GhostTasksCountToWin.Get();
        public static bool CustomCrewColors => Generate.CustomCrewColors.Get();
        public static bool CrewVent => Generate.CrewVent.Get();
        public static int CrewMax => (int)Generate.CrewMax.Get();
        public static int CrewMin => (int)Generate.CrewMin.Get();

        //Intruder Options
        public static float IntruderVision => Generate.IntruderVision.Get();
        public static float IntKillCooldown => Generate.IntruderKillCooldown.Get();
        public static int IntruderCount => (int)Generate.IntruderCount.Get();
        public static bool CustomIntColors => Generate.CustomIntColors.Get();
        public static bool IntrudersCanSabotage => Generate.IntrudersCanSabotage.Get();
        public static bool IntrudersVent => Generate.IntrudersVent.Get();
        public static float IntruderSabotageCooldown => Generate.IntruderSabotageCooldown.Get();
        public static int IntruderMax => (int)Generate.IntruderMax.Get();
        public static int IntruderMin => (int)Generate.IntruderMin.Get();

        //Syndicate Options
        public static float SyndicateVision => Generate.SyndicateVision.Get();
        public static bool AltImps => Generate.AltImps.Get() || IntruderCount == 0;
        public static SyndicateVentOptions SyndicateVent => (SyndicateVentOptions)Generate.SyndicateVent.Get();
        public static int SyndicateCount => (int)Generate.SyndicateCount.Get();
        public static bool CustomSynColors => Generate.CustomSynColors.Get();
        public static bool GlobalDrive => Generate.GlobalDrive.Get();
        public static float ChaosDriveKillCooldown => Generate.ChaosDriveKillCooldown.Get();
        public static int ChaosDriveMeetingCount => (int)Generate.ChaosDriveMeetingCount.Get();
        public static int SyndicateMax => (int)Generate.SyndicateMax.Get();
        public static int SyndicateMin => (int)Generate.SyndicateMin.Get();

        //Neutral Options
        public static float NeutralVision => Generate.NeutralVision.Get();
        public static bool LightsAffectNeutrals => Generate.LightsAffectNeutrals.Get();
        public static NoSolo NoSolo => (NoSolo)Generate.NoSolo.Get();
        public static bool CustomNeutColors => Generate.CustomNeutColors.Get();
        public static bool NeutralsVent => Generate.NeutralsVent.Get();
        public static int NeutralMax => (int)Generate.NeutralMax.Get();
        public static int NeutralMin => (int)Generate.NeutralMin.Get();

        //Vampire Hunter Settings
        public static int VampireHunterCount => (int)Generate.VampireHunterCount.Get();
        public static bool UniqueVampireHunter => Generate.UniqueVampireHunter.Get();
        public static float StakeCooldown => Generate.StakeCooldown.Get();

        //Mystic Settings
        public static int MysticCount => (int)Generate.MysticCount.Get();
        public static bool UniqueMystic => Generate.UniqueMystic.Get();
        public static float RevealCooldown => Generate.RevealCooldown.Get();

        //Seer Settings
        public static int SeerCount => (int)Generate.SeerCount.Get();
        public static bool UniqueSeer => Generate.UniqueSeer.Get();
        public static float SeerCooldown => Generate.SeerCooldown.Get();

        //Detective Settings
        public static int DetectiveCount => (int)Generate.DetectiveCount.Get();
        public static float ExamineCd => Generate.ExamineCooldown.Get();
        public static bool UniqueDetective => Generate.UniqueDetective.Get();
        public static float RecentKill => Generate.RecentKill.Get();
        public static float FootprintInterval => Generate.FootprintInterval.Get();
        public static float FootprintDuration => Generate.FootprintDuration.Get();
        public static bool AnonymousFootPrint => Generate.AnonymousFootPrint.Get();
        public static bool VentFootprintVisible => Generate.VentFootprintVisible.Get();

        //Inspector Settings
        public static int InspectorCount => (int)Generate.InspectorCount.Get();
        public static float InspectCooldown => Generate.InspectCooldown.Get();
        public static bool UniqueInspector => Generate.UniqueInspector.Get();

        //Medium Settings
        public static int MediumCount => (int)Generate.MediumCount.Get();
        public static DeadRevealed DeadRevealed => (DeadRevealed)Generate.DeadRevealed.Get();
        public static float MediateCooldown => Generate.MediateCooldown.Get();
        public static bool ShowMediatePlayer => Generate.ShowMediatePlayer.Get();
        public static bool ShowMediumToDead => Generate.ShowMediumToDead.Get();
        public static bool UniqueMedium => Generate.UniqueMedium.Get();

        //Coroner Settings
        public static bool CoronerReportName => Generate.CoronerReportName.Get();
        public static bool CoronerReportRole => Generate.CoronerReportRole.Get();
        public static float CoronerArrowDuration => Generate.CoronerArrowDuration.Get();
        public static bool UniqueCoroner => Generate.UniqueCoroner.Get();
        public static int CoronerCount => (int)Generate.CoronerCount.Get();
        public static int CompareLimit => (int)Generate.CompareLimit.Get();
        public static float CoronerKillerNameTime => Generate.CoronerKillerNameTime.Get();
        public static float CompareCooldown => Generate.CompareCooldown.Get();
        public static float AutopsyCooldown => Generate.AutopsyCooldown.Get();

        //Revealer Settings
        public static bool RevealerKnows => Generate.RevealerKnows.Get();
        public static int RevealerTasksRemainingClicked => (int)Generate.RevealerTasksRemainingClicked.Get();
        public static int RevealerTasksRemainingAlert => (int)Generate.RevealerTasksRemainingAlert.Get();
        public static int RevealerCount => (int)Generate.RevealerCount.Get();
        public static RevealerCanBeClickedBy RevealerCanBeClickedBy => (RevealerCanBeClickedBy)Generate.RevealerCanBeClickedBy.Get();
        public static bool RevealerRevealsNeutrals => Generate.RevealerRevealsNeutrals.Get();
        public static bool RevealerRevealsCrew => Generate.RevealerRevealsCrew.Get();
        public static bool RevealerRevealsTraitor => Generate.RevealerRevealsTraitor.Get();
        public static bool RevealerRevealsFanatic => Generate.RevealerRevealsFanatic.Get();
        public static bool RevealerRevealsRoles => Generate.RevealerRevealsRoles.Get();

        //Sheriff Settings
        public static bool NeutEvilRed => Generate.NeutEvilRed.Get();
        public static bool NeutKillingRed => Generate.NeutKillingRed.Get();
        public static bool UniqueSheriff => Generate.UniqueSheriff.Get();
        public static float InterrogateCd => Generate.InterrogateCooldown.Get();
        public static int SheriffCount => (int)Generate.SheriffCount.Get();

        //Tracker Settings
        public static bool ResetOnNewRound => Generate.ResetOnNewRound.Get();
        public static float UpdateInterval => Generate.UpdateInterval.Get();
        public static int TrackerCount => (int)Generate.TrackerCount.Get();
        public static bool UniqueTracker => Generate.UniqueTracker.Get();
        public static float TrackCd => Generate.TrackCooldown.Get();
        public static int MaxTracks => (int)Generate.MaxTracks.Get();

        //Operative Settings
        public static float BugCooldown => Generate.BugCooldown.Get();
        public static int MaxBugs => (int)Generate.MaxBugs.Get();
        public static float MinAmountOfTimeInBug => Generate.MinAmountOfTimeInBug.Get();
        public static int OperativeCount => (int)Generate.OperativeCount.Get();
        public static float BugRange => Generate.BugRange.Get();
        public static int MinAmountOfPlayersInBug => (int)Generate.MinAmountOfPlayersInBug.Get();
        public static bool BugsRemoveOnNewRound => Generate.BugsRemoveOnNewRound.Get();
        public static bool UniqueOperative => Generate.UniqueOperative.Get();
        public static AdminDeadPlayers WhoSeesDead => (AdminDeadPlayers)Generate.WhoSeesDead.Get();

        //Veteran Settings
        public static float AlertCd => Generate.AlertCooldown.Get();
        public static float AlertDuration => Generate.AlertDuration.Get();
        public static int MaxAlerts => (int)Generate.MaxAlerts.Get();
        public static int VeteranCount => (int)Generate.VeteranCount.Get();
        public static bool UniqueVeteran => Generate.UniqueVeteran.Get();

        //Vigilante Settings
        public static VigiOptions VigiOptions => (VigiOptions)Generate.VigiOptions.Get();
        public static int VigilanteCount => (int)Generate.VigilanteCount.Get();
        public static int VigiBulletCount => (int)Generate.VigiBulletCount.Get();
        public static float VigiKillCd => Generate.VigiKillCd.Get();
        public static bool UniqueVigilante => Generate.UniqueVigilante.Get();
        public static bool MisfireKillsInno => Generate.MisfireKillsInno.Get();
        public static bool VigiKillAgain => Generate.VigiKillAgain.Get();
        public static bool RoundOneNoShot => Generate.RoundOneNoShot.Get();
        public static VigiNotif VigiNotifOptions => (VigiNotif)Generate.VigiNotifOptions.Get();

        //Altruist Settings
        public static bool AltruistTargetBody => Generate.AltruistTargetBody.Get();
        public static bool UniqueAltruist => Generate.UniqueAltruist.Get();
        public static float AltReviveDuration => Generate.AltReviveDuration.Get();
        public static int AltruistCount => (int)Generate.AltruistCount.Get();
        public static int ReviveCount => (int)Generate.ReviveCount.Get();
        public static float ReviveCooldown => Generate.ReviveCooldown.Get();

        //Medic Settings
        public static ShieldOptions ShowShielded => (ShieldOptions)Generate.ShowShielded.Get();
        public static int MedicCount => (int)Generate.MedicCount.Get();
        public static bool UniqueMedic => Generate.UniqueMedic.Get();
        public static NotificationOptions NotificationShield => (NotificationOptions)Generate.WhoGetsNotification.Get();
        public static bool ShieldBreaks => Generate.ShieldBreaks.Get();

        //Mayor Settings
        public static bool MayorAnonymous => Generate.MayorAnonymous.Get();
        public static int MayorVoteBank => (int)Generate.MayorVoteBank.Get();
        public static bool UniqueMayor => Generate.UniqueMayor.Get();
        public static int MayorCount => (int)Generate.MayorCount.Get();
        public static bool MayorButton => Generate.MayorButton.Get();

        //Swapper Settings
        public static bool SwapperButton => Generate.SwapperButton.Get();
        public static int SwapperCount => (int)Generate.SwapperCount.Get();
        public static bool SwapAfterVoting => Generate.SwapAfterVoting.Get();
        public static bool SwapSelf => Generate.SwapSelf.Get();
        public static bool UniqueSwapper => Generate.UniqueSwapper.Get();

        //Engineer Settings
        public static int EngineerCount => (int)Generate.EngineerCount.Get();
        public static int MaxFixes => (int)Generate.MaxFixes.Get();
        public static bool UniqueEngineer => Generate.UniqueEngineer.Get();
        public static float FixCooldown => Generate.FixCooldown.Get();

        //Escort Settings
        public static int EscortCount => (int)Generate.EscortCount.Get();
        public static float EscRoleblockCooldown => Generate.EscRoleblockCooldown.Get();
        public static float EscRoleblockDuration => Generate.EscRoleblockDuration.Get();
        public static bool UniqueEscort => Generate.UniqueEscort.Get();

        //Chameleon Settings
        public static int ChameleonCount => (int)Generate.ChameleonCount.Get();
        public static float SwoopCooldown => Generate.SwoopCooldown.Get();
        public static float SwoopDuration => Generate.SwoopDuration.Get();
        public static bool UniqueChameleon => Generate.UniqueChameleon.Get();
        public static int SwoopCount => (int)Generate.SwoopCount.Get();

        //Retributionist Settings
        public static int RetributionistCount => (int)Generate.RetributionistCount.Get();
        public static bool UniqueRetributionist => Generate.UniqueRetributionist.Get();

        //Shifter Settings
        public static BecomeEnum ShiftedBecomes => (BecomeEnum)Generate.ShiftedBecomes.Get();
        public static int ShifterCount => (int)Generate.ShifterCount.Get();
        public static bool UniqueShifter => Generate.UniqueShifter.Get();
        public static float ShifterCd => Generate.ShifterCd.Get();

        //Transporter Settings
        public static float TransportCooldown => Generate.TransportCooldown.Get();
        public static int TransportMaxUses => (int)Generate.TransportMaxUses.Get();
        public static int TransporterCount => (int)Generate.TransporterCount.Get();
        public static bool UniqueTransporter => Generate.UniqueTransporter.Get();
        public static bool TransSelf => Generate.TransSelf.Get();

        //Crewmate Settings
        public static int CrewCount => (int)Generate.CrewCount.Get();

        //Amnesiac Settings
        public static bool RememberArrows => Generate.RememberArrows.Get();
        public static int AmnesiacCount => (int)Generate.AmnesiacCount.Get();
        public static bool AmneVent => Generate.AmneVent.Get();
        public static bool AmneVentSwitch => Generate.AmneSwitchVent.Get();
        public static bool AmneTurnAssassin => Generate.AmneTurnAssassin.Get();
        public static float RememberArrowDelay => Generate.RememberArrowDelay.Get();
        public static bool UniqueAmnesiac => Generate.UniqueAmnesiac.Get();

        //Survivor Settings
        public static float VestCd => Generate.VestCd.Get();
        public static float VestDuration => Generate.VestDuration.Get();
        public static float VestKCReset => Generate.VestKCReset.Get();
        public static int MaxVests => (int)Generate.MaxVests.Get();
        public static int SurvivorCount => (int)Generate.SurvivorCount.Get();
        public static bool SurvVent => Generate.SurvVent.Get();
        public static bool SurvVentSwitch => Generate.SurvSwitchVent.Get();
        public static bool UniqueSurvivor => Generate.UniqueSurvivor.Get();

        //GA Settings
        public static float ProtectCd => Generate.ProtectCd.Get();
        public static float ProtectDuration => Generate.ProtectDuration.Get();
        public static float ProtectKCReset => Generate.ProtectKCReset.Get();
        public static int MaxProtects => (int)Generate.MaxProtects.Get();
        public static ProtectOptions ShowProtect => (ProtectOptions)Generate.ShowProtect.Get();
        public static int GuardianAngelCount => (int)Generate.GuardianAngelCount.Get();
        public static bool GAVent => Generate.GAVent.Get();
        public static bool GAVentSwitch => Generate.GASwitchVent.Get();
        public static bool ProtectBeyondTheGrave => Generate.ProtectBeyondTheGrave.Get();
        public static bool GATargetKnows => Generate.GATargetKnows.Get();
        public static bool GAKnowsTargetRole => Generate.GAKnowsTargetRole.Get();
        public static bool UniqueGuardianAngel => Generate.UniqueGuardianAngel.Get();

        //Thief Settings
        public static int ThiefCount => (int)Generate.ThiefCount.Get();
        public static bool ThiefVent => Generate.ThiefVent.Get();
        public static float ThiefKillCooldown => Generate.ThiefKillCooldown.Get();
        public static bool UniqueThief => Generate.UniqueThief.Get();
        public static bool ThiefSteals => Generate.ThiefSteals.Get();

        //Jester Settings
        public static bool VigiKillsJester => Generate.VigiKillsJester.Get();
        public static bool JestEjectScreen => Generate.JestEjectScreen.Get();
        public static bool JestVentSwitch => Generate.JestSwitchVent.Get();
        public static bool JesterButton => Generate.JesterButton.Get();
        public static bool JesterVent => Generate.JesterVent.Get();
        public static int JesterCount => (int)Generate.JesterCount.Get();
        public static int HauntCount => (int)Generate.HauntCount.Get();
        public static bool UniqueJester => Generate.UniqueJester.Get();
        public static float HauntCooldown => Generate.HauntCooldown.Get();

        //Actor Settings
        public static bool VigiKillsActor => Generate.VigiKillsActor.Get();
        public static bool ActVentSwitch => Generate.ActSwitchVent.Get();
        public static bool ActorButton => Generate.ActorButton.Get();
        public static bool ActorVent => Generate.ActorVent.Get();
        public static int ActorCount => (int)Generate.ActorCount.Get();
        public static bool UniqueActor => Generate.UniqueActor.Get();

        //Troll Settings
        public static bool TrollVent => Generate.TrollVent.Get();
        public static float InteractCooldown => Generate.InteractCooldown.Get();
        public static int TrollCount => (int)Generate.TrollCount.Get();
        public static bool TrollVentSwitch => Generate.TrollSwitchVent.Get();
        public static bool UniqueTroll => Generate.UniqueTroll.Get();

        //Bounty Hunter Settings
        public static bool BHVent => Generate.BHVent.Get();
        public static float BountyHunterCooldown => Generate.BountyHunterCooldown.Get();
        public static int BHCount => (int)Generate.BHCount.Get();
        public static int BountyHunterGuesses => (int)Generate.BountyHunterGuesses.Get();
        public static bool UniqueBountyHunter => Generate.UniqueBountyHunter.Get();
        public static bool VigiKillsBH => Generate.VigiKillsBH.Get();

        //Cannibal Settings
        public static float EatArrowDelay => Generate.EatArrowDelay.Get();
        public static bool EatArrows => Generate.EatArrows.Get();
        public static bool CannibalVent => Generate.CannibalVent.Get();
        public static float CannibalCd => Generate.CannibalCd.Get();
        public static int CannibalCount => (int)Generate.CannibalCount.Get();
        public static int CannibalBodyCount => (int)Generate.CannibalBodyCount.Get();
        public static bool VigiKillsCannibal => Generate.VigiKillsCannibal.Get();
        public static bool UniqueCannibal => Generate.UniqueCannibal.Get();

        //Executioner Settings
        public static int ExecutionerCount => (int)Generate.ExecutionerCount.Get();
        public static bool ExeCanHaveNeutralTargets => Generate.ExeCanHaveNeutralTargets.Get();
        public static bool ExeCanHaveIntruderTargets => Generate.ExeCanHaveIntruderTargets.Get();
        public static bool ExeCanHaveSyndicateTargets => Generate.ExeCanHaveSyndicateTargets.Get();
        public static bool ExeCanWinBeyondDeath => Generate.ExeCanWinBeyondDeath.Get();
        public static bool VigiKillsExecutioner => Generate.VigiKillsExecutioner.Get();
        public static bool ExeVent => Generate.ExeVent.Get();
        public static bool ExecutionerButton => Generate.ExecutionerButton.Get();
        public static bool ExeTargetKnows => Generate.ExeTargetKnows.Get();
        public static bool ExeKnowsTargetRole => Generate.ExeKnowsTargetRole.Get();
        public static bool ExeEjectScreen => Generate.ExeEjectScreen.Get();
        public static bool ExeVentSwitch => Generate.ExeSwitchVent.Get();
        public static bool UniqueExecutioner => Generate.UniqueExecutioner.Get();
        public static int DoomCount => (int)Generate.DoomCount.Get();
        public static float DoomCooldown => Generate.DoomCooldown.Get();

        //Guesser Settings
        public static int GuesserCount => (int)Generate.GuesserCount.Get();
        public static bool VigiKillsGuesser => Generate.VigiKillsGuesser.Get();
        public static bool GuessVent => Generate.GuessVent.Get();
        public static bool GuesserButton => Generate.GuesserButton.Get();
        public static bool GuesserTargetKnows => Generate.GuessTargetKnows.Get();
        public static bool GuessVentSwitch => Generate.GuessSwitchVent.Get();
        public static bool UniqueGuesser => Generate.UniqueGuesser.Get();
        public static bool GuesserAfterVoting => Generate.GuesserAfterVoting.Get();
        public static bool MultipleGuesses => Generate.MultipleGuesses.Get();
        public static int GuessCount => (int)Generate.GuessCount.Get();

        //Glitch Settings
        public static bool GlitchVent => Generate.GlitchVent.Get();
        public static float MimicCooldown => Generate.MimicCooldown.Get();
        public static float HackCooldown => Generate.HackCooldown.Get();
        public static int GlitchCount => (int)Generate.GlitchCount.Get();
        public static float MimicDuration => Generate.MimicDuration.Get();
        public static float HackDuration => Generate.HackDuration.Get();
        public static float GlitchKillCooldown => Generate.GlitchKillCooldown.Get();
        public static bool UniqueGlitch => Generate.UniqueGlitch.Get();

        //Juggernaut Settings
        public static float JuggKillBonus => Generate.JuggKillBonus.Get();
        public static bool JuggVent => Generate.JuggVent.Get();
        public static int JuggernautCount => (int)Generate.JuggernautCount.Get();
        public static float JuggKillCooldown => Generate.JuggKillCooldown.Get();
        public static bool UniqueJuggernaut => Generate.UniqueJuggernaut.Get();

        //Cryomaniac Settings
        public static bool CryoVent => Generate.CryoVent.Get();
        public static int CryomaniacCount => (int)Generate.CryomaniacCount.Get();
        public static float CryoDouseCooldown => Generate.CryoDouseCooldown.Get();
        public static bool UniqueCryomaniac => Generate.UniqueCryomaniac.Get();
        public static bool CryoFreezeAll => Generate.CryoFreezeAll.Get();

        //Plaguebearer Settings
        public static bool PBVent => Generate.PBVent.Get();
        public static float InfectCd => Generate.InfectCooldown.Get();
        public static int PlaguebearerCount => (int)Generate.PlaguebearerCount.Get();
        public static bool UniquePlaguebearer => Generate.UniquePlaguebearer.Get();

        //Arsonist Settings
        public static bool ArsoLastKillerBoost => Generate.ArsoLastKillerBoost.Get();
        public static bool ArsoVent => Generate.ArsoVent.Get();
        public static float DouseCd => Generate.DouseCooldown.Get();
        public static float IgniteCd => Generate.IgniteCooldown.Get();
        public static int ArsonistCount => (int)Generate.ArsonistCount.Get();
        public static bool UniqueArsonist => Generate.UniqueArsonist.Get();
        public static bool ArsoCooldownsLinked => Generate.ArsoCooldownsLinked.Get();
        public static bool ArsoIgniteAll => Generate.ArsoIgniteAll.Get();

        //Murderer Settings
        public static float MurdKCD => Generate.MurdKillCooldownOption.Get();
        public static bool MurdVent => Generate.MurdVent.Get();
        public static int MurdCount => (int)Generate.MurdCount.Get();
        public static bool UniqueMurderer => Generate.UniqueMurderer.Get();

        //SK Settings
        public static float BloodlustCd => Generate.BloodlustCooldown.Get();
        public static float BloodlustDuration => Generate.BloodlustDuration.Get();
        public static int SKCount => (int)Generate.SKCount.Get();
        public static bool UniqueSerialKiller => Generate.UniqueSerialKiller.Get();
        public static float LustKillCd => Generate.LustKillCooldown.Get();
        public static SKVentOptions SKVentOptions => (SKVentOptions)Generate.SKVentOptions.Get();

        //WW Settings
        public static bool WerewolfVent => Generate.WerewolfVent.Get();
        public static float MaulRadius => Generate.MaulRadius.Get();
        public static float MaulCooldown => Generate.MaulCooldown.Get();
        public static int WerewolfCount => (int)Generate.WerewolfCount.Get();
        public static bool UniqueWerewolf => Generate.UniqueWerewolf.Get();

        //Dracula Settings
        public static bool DracVent => Generate.DracVent.Get();
        public static bool UndeadVent => Generate.UndeadVent.Get();
        public static float BiteCd => Generate.BiteCooldown.Get();
        public static int DraculaCount => (int)Generate.DraculaCount.Get();
        public static int AliveVampCount => (int)Generate.AliveVampCount.Get();
        public static bool UniqueDracula => Generate.UniqueDracula.Get();

        //Necromancer Settings
        public static bool NecroVent => Generate.NecroVent.Get();
        public static bool KillResurrectCooldownsLinked => Generate.KillResurrectCooldownsLinked.Get();
        public static float ResurrectCooldown => Generate.ResurrectCooldown.Get();
        public static float NecroKillCooldown => Generate.NecroKillCooldown.Get();
        public static float NecroKillCooldownIncrease => Generate.NecroKillCooldownIncrease.Get();
        public static int NecroKillCount => (int)Generate.NecroKillCount.Get();
        public static float ResurrectCooldownIncrease => Generate.ResurrectCooldownIncrease.Get();
        public static int ResurrectCount => (int)Generate.ResurrectCount.Get();
        public static int NecromancerCount => (int)Generate.NecromancerCount.Get();
        public static bool UniqueNecromancer => Generate.UniqueNecromancer.Get();
        public static bool NecromancerTargetBody => Generate.NecromancerTargetBody.Get();
        public static float NecroResurrectDuration => Generate.NecroResurrectDuration.Get();
        public static bool ResurrectVent => Generate.ResurrectVent.Get();

        //Whisperer Settings
        public static bool WhispVent => Generate.WhispVent.Get();
        public static float WhisperCooldown => Generate.WhisperCooldown.Get();
        public static float WhisperRadius => Generate.WhisperRadius.Get();
        public static float WhisperCooldownIncrease => Generate.WhisperCooldownIncrease.Get();
        public static float InitialWhisperRate => Generate.InitialWhisperRate.Get();
        public static float WhisperRateDecrease => Generate.WhisperRateDecrease.Get();
        public static bool WhisperRateDecreases => Generate.WhisperRateDecreases.Get();
        public static int WhispererCount => (int)Generate.WhispererCount.Get();
        public static bool UniqueWhisperer => Generate.UniqueWhisperer.Get();
        public static bool PersuadedVent => Generate.PersuadedVent.Get();

        //Jackal Settings
        public static bool JackalVent => Generate.JackalVent.Get();
        public static bool RecruitVent => Generate.RecruitVent.Get();
        public static int JackalCount => (int)Generate.JackalCount.Get();
        public static bool UniqueJackal => Generate.UniqueJackal.Get();
        public static float RecruitCooldown => Generate.RecruitCooldown.Get();

        //Phantom Settings
        public static int PhantomCount => (int)Generate.PhantomCount.Get();
        public static int PhantomTasksRemaining => (int)Generate.PhantomTasksRemaining.Get();
        public static bool PhantomPlayersAlerted => Generate.PhantomPlayersAlerted.Get();

        //Pestilence Settings
        public static float PestKillCd => Generate.PestKillCooldown.Get();
        public static bool PlayersAlerted => Generate.PlayersAlerted.Get();
        public static bool PestSpawn => Generate.PestSpawn.Get();
        public static bool PestVent => Generate.PestVent.Get();

        //Ghoul Settings
        public static float GhoulMarkCd => Generate.GhoulMarkCd.Get();

        //Janitor Settings
        public static float JanitorCleanCd => Generate.JanitorCleanCd.Get();
        public static int JanitorCount => (int)Generate.JanitorCount.Get();
        public static bool SoloBoost => Generate.SoloBoost.Get();
        public static bool UniqueJanitor => Generate.UniqueJanitor.Get();
        public static bool JaniCooldownsLinked => Generate.JaniCooldownsLinked.Get();
        public static JanitorOptions JanitorVentOptions => (JanitorOptions)Generate.JanitorVentOptions.Get();
        public static int DragModifier => (int)Generate.DragModifier.Get();
        public static float DragCd => Generate.DragCooldown.Get();

        //Blackmailer Settings
        public static float BlackmailCd => Generate.BlackmailCooldown.Get();
        public static int BlackmailerCount => (int)Generate.BlackmailerCount.Get();
        public static bool UniqueBlackmailer => Generate.UniqueBlackmailer.Get();
        public static bool WhispersNotPrivate => Generate.WhispersNotPrivate.Get();

        //Grenadier Settings
        public static bool GrenadierIndicators => Generate.GrenadierIndicators.Get();
        public static float GrenadeCd => Generate.GrenadeCooldown.Get();
        public static int GrenadierCount => (int)Generate.GrenadierCount.Get();
        public static float GrenadeDuration => Generate.GrenadeDuration.Get();
        public static float FlashRadius => Generate.FlashRadius.Get();
        public static bool GrenadierVent => Generate.GrenadierVent.Get();
        public static bool UniqueGrenadier => Generate.UniqueGrenadier.Get();

        //Camouflager Settings
        public static bool CamoHideSize => Generate.CamoHideSize.Get();
        public static bool CamoHideSpeed => Generate.CamoHideSpeed.Get();
        public static float CamouflagerCd => Generate.CamouflagerCooldown.Get();
        public static float CamouflagerDuration => Generate.CamouflagerDuration.Get();
        public static int CamouflagerCount => (int)Generate.CamouflagerCount.Get();
        public static bool UniqueCamouflager => Generate.UniqueCamouflager.Get();

        //Morphling Settings
        public static bool MorphlingVent => Generate.MorphlingVent.Get();
        public static int MorphlingCount => (int)Generate.MorphlingCount.Get();
        public static float MorphlingCd => Generate.MorphlingCooldown.Get();
        public static float SampleCooldown => Generate.SampleCooldown.Get();
        public static float MorphlingDuration => Generate.MorphlingDuration.Get();
        public static bool UniqueMorphling => Generate.UniqueMorphling.Get();
        public static bool MorphCooldownsLinked => Generate.MorphCooldownsLinked.Get();

        //Wraith Settings
        public static bool WraithVent => Generate.WraithVent.Get();
        public static float InvisCd => Generate.InvisCooldown.Get();
        public static float InvisDuration => Generate.InvisDuration.Get();
        public static int WraithCount => (int)Generate.WraithCount.Get();
        public static bool UniqueWraith => Generate.UniqueWraith.Get();

        //Ambusher Settings
        public static float AmbushCooldown => Generate.AmbushCooldown.Get();
        public static float AmbushDuration => Generate.AmbushDuration.Get();
        public static int AmbusherCount => (int)Generate.AmbusherCount.Get();
        public static bool UniqueAmbusher => Generate.UniqueAmbusher.Get();

        //Enforcer Settings
        public static float EnforceCooldown => Generate.EnforceCooldown.Get();
        public static float EnforceDuration => Generate.EnforceDuration.Get();
        public static int EnforcerCount => (int)Generate.EnforcerCount.Get();
        public static float EnforceRadius => Generate.EnforceRadius.Get();
        public static float EnforceDelay => Generate.EnforceDelay.Get();
        public static bool UniqueEnforcer => Generate.UniqueEnforcer.Get();

        //Teleporter Settings
        public static bool TeleVent => Generate.TeleVent.Get();
        public static float TeleportCd => Generate.TeleportCd.Get();
        public static float MarkCooldown => Generate.MarkCooldown.Get();
        public static int TeleporterCount => (int)Generate.TeleporterCount.Get();
        public static bool UniqueTeleporter => Generate.UniqueTeleporter.Get();
        public static bool TeleCooldownsLinked => Generate.TeleCooldownsLinked.Get();

        //Consigliere Settings
        public static ConsigInfo ConsigInfo => (ConsigInfo)Generate.ConsigInfo.Get();
        public static float ConsigCd => Generate.InvestigateCooldown.Get();
        public static int ConsigliereCount => (int)Generate.ConsigCount.Get();
        public static bool UniqueConsigliere => Generate.UniqueConsigliere.Get();

        //Consort Settings
        public static int ConsortCount => (int)Generate.ConsortCount.Get();
        public static float ConsRoleblockCooldown => Generate.ConsRoleblockCooldown.Get();
        public static bool UniqueConsort => Generate.UniqueConsort.Get();
        public static float ConsRoleblockDuration => Generate.ConsRoleblockDuration.Get();

        //Disguiser Settings
        public static int DisguiserCount => (int)Generate.DisguiserCount.Get();
        public static float DisguiseDuration => Generate.DisguiseDuration.Get();
        public static float DisguiseCooldown => Generate.DisguiseCooldown.Get();
        public static float TimeToDisguise => Generate.TimeToDisguise.Get();
        public static DisguiserTargets DisguiseTarget => (DisguiserTargets)Generate.DisguiseTarget.Get();
        public static bool UniqueDisguiser => Generate.UniqueDisguiser.Get();
        public static float MeasureCooldown => Generate.MeasureCooldown.Get();
        public static bool DisgCooldownsLinked => Generate.DisgCooldownsLinked.Get();

        //Godfather Settings
        public static int GodfatherCount => (int)Generate.GodfatherCount.Get();
        public static bool UniqueGodfather => Generate.UniqueGodfather.Get();
        public static float MafiosoAbilityCooldownDecrease => Generate.MafiosoAbilityCooldownDecrease.Get();

        //Miner Settings
        public static float MineCd => Generate.MineCooldown.Get();
        public static int MinerCount => (int)Generate.MinerCount.Get();
        public static bool UniqueMiner => Generate.UniqueMiner.Get();

        //Impostor Settings
        public static int ImpCount => (int)Generate.ImpCount.Get();

        //Anarchist Settings
        public static int AnarchistCount => (int)Generate.AnarchistCount.Get();
        public static float ChaosDriveCooldownDecrease => Generate.ChaosDriveCooldownDecrease.Get();

        //Framer Settings
        public static int FramerCount => (int)Generate.FramerCount.Get();
        public static float FrameCooldown => Generate.FrameCooldown.Get();
        public static float ChaosDriveFrameRadius => Generate.ChaosDriveFrameRadius.Get();
        public static bool UniqueFramer => Generate.UniqueFramer.Get();

        //Shapeshifter Settings
        public static int ShapeshifterCount => (int)Generate.ShapeshifterCount.Get();
        public static float ShapeshiftCooldown => Generate.ShapeshiftCooldown.Get();
        public static float ShapeshiftDuration => Generate.ShapeshiftDuration.Get();
        public static bool UniqueShapeshifter => Generate.UniqueShapeshifter.Get();

        //Crusader Settings
        public static float CrusadeCooldown => Generate.CrusadeCooldown.Get();
        public static float CrusadeDuration => Generate.CrusadeDuration.Get();
        public static int CrusaderCount => (int)Generate.CrusaderCount.Get();
        public static bool UniqueCrusader => Generate.UniqueCrusader.Get();
        public static float ChaosDriveCrusadeRadius => Generate.ChaosDriveCrusadeRadius.Get();

        //Banshee Settings
        public static float ScreamCooldown => Generate.ScreamCooldown.Get();
        public static float ScreamDuration => Generate.ScreamDuration.Get();

        //Bomber Settings
        public static float BombCooldown => Generate.BombCooldown.Get();
        public static float DetonateCooldown => Generate.DetonateCooldown.Get();
        public static int BomberCount => (int)Generate.BomberCount.Get();
        public static float BombRange => Generate.BombRange.Get();
        public static bool UniqueBomber => Generate.UniqueBomber.Get();
        public static bool BombCooldownsLinked => Generate.BombCooldownsLinked.Get();
        public static bool BombsDetonateOnMeetingStart => Generate.BombsDetonateOnMeetingStart.Get();
        public static bool BombsRemoveOnNewRound => Generate.BombsRemoveOnNewRound.Get();
        public static float ChaosDriveBombRange => Generate.ChaosDriveBombRange.Get();

        //Politician Settings
        public static bool PoliticianAnonymous => Generate.PoliticianAnonymous.Get();
        public static int PoliticianVoteBank => (int)Generate.PoliticianVoteBank.Get();
        public static int ChaosDriveVoteAdd => (int)Generate.ChaosDriveVoteAdd.Get();
        public static bool UniquePolitician => Generate.UniquePolitician.Get();
        public static int PoliticianCount => (int)Generate.PoliticianCount.Get();
        public static bool PoliticianButton => Generate.PoliticianButton.Get();

        //Concealer Settings
        public static int ConcealerCount => (int)Generate.ConcealerCount.Get();
        public static float ConcealCooldown => Generate.ConcealCooldown.Get();
        public static float ConcealDuration => Generate.ConcealDuration.Get();
        public static bool UniqueConcealer => Generate.UniqueConcealer.Get();

        //Poisoner Settings
        public static float PoisonCd => Generate.PoisonCooldown.Get();
        public static float PoisonDuration => Generate.PoisonDuration.Get();
        public static int PoisonerCount => (int)Generate.PoisonerCount.Get();
        public static bool UniquePoisoner => Generate.UniquePoisoner.Get();

        //Rebel Settings
        public static int RebelCount => (int)Generate.RebelCount.Get();
        public static bool UniqueRebel => Generate.UniqueRebel.Get();
        public static float SidekickAbilityCooldownDecrease => Generate.SidekickAbilityCooldownDecrease.Get();

        //Warper Settings
        public static float WarpCooldown => Generate.WarpCooldown.Get();
        public static bool UniqueWarper => Generate.UniqueWarper.Get();
        public static bool WarpSelf => Generate.WarpSelf.Get();
        public static int WarperCount => (int)Generate.WarperCount.Get();

        //Betrayer Settings
        public static float BetrayerKillCooldown => Generate.BetrayerKillCooldown.Get();
        public static bool BetrayerVent => Generate.BetrayerVent.Get();

        //Modifier Settings
        public static bool CustomModifierColors => Generate.CustomModifierColors.Get();
        public static int MaxModifiers => (int)Generate.MaxModifiers.Get();
        public static int MinModifiers => (int)Generate.MinModifiers.Get();

        //Objectifier Settings
        public static bool CustomObjectifierColors => Generate.CustomObjectifierColors.Get();
        public static int MaxObjectifiers => (int)Generate.MaxObjectifiers.Get();
        public static int MinObjectifiers => (int)Generate.MinObjectifiers.Get();

        //Ability Settings
        public static bool CustomAbilityColors => Generate.CustomAbilityColors.Get();
        public static int MaxAbilities => (int)Generate.MaxAbilities.Get();
        public static int MinAbilities => (int)Generate.MinAbilities.Get();

        //Snitch Settings
        public static bool SnitchSeesNeutrals => Generate.SnitchSeesNeutrals.Get();
        public static int SnitchCount => (int)Generate.SnitchCount.Get();
        public static bool SnitchSeesCrew => Generate.SnitchSeesCrew.Get();
        public static bool SnitchSeesRoles => Generate.SnitchSeesRoles.Get();
        public static bool SnitchSeestargetsInMeeting => Generate.SnitchSeestargetsInMeeting.Get();
        public static bool SnitchSeesTraitor => Generate.SnitchSeesTraitor.Get();
        public static bool SnitchSeesFanatic => Generate.SnitchSeesFanatic.Get();
        public static bool SnitchKnows => Generate.SnitchKnows.Get();
        public static int SnitchTasksRemaining => (int)Generate.SnitchTasksRemaining.Get();
        public static bool UniqueSnitch => Generate.UniqueSnitch.Get();

        //Assassin Settings
        public static int AssassinKills => (int)Generate.AssassinKills.Get();
        public static int NumberOfImpostorAssassins => (int)Generate.NumberOfImpostorAssassins.Get();
        public static int NumberOfCrewAssassins => (int)Generate.NumberOfCrewAssassins.Get();
        public static int NumberOfNeutralAssassins => (int)Generate.NumberOfNeutralAssassins.Get();
        public static int NumberOfSyndicateAssassins => (int)Generate.NumberOfSyndicateAssassins.Get();
        public static bool AssassinGuessNeutralBenign => Generate.AssassinGuessNeutralBenign.Get();
        public static bool AssassinGuessNeutralEvil => Generate.AssassinGuessNeutralEvil.Get();
        public static bool AssassinGuessPest => Generate.AssassinGuessPest.Get();
        public static bool AssassinGuessModifiers => Generate.AssassinGuessModifiers.Get();
        public static bool AssassinGuessObjectifiers => Generate.AssassinGuessObjectifiers.Get();
        public static bool AssassinGuessAbilities => Generate.AssassinGuessAbilities.Get();
        public static bool AssassinMultiKill => Generate.AssassinMultiKill.Get();
        public static bool AssassinateAfterVoting => Generate.AssassinateAfterVoting.Get();
        public static bool UniqueAssassin => Generate.UniqueAssassin.Get();

        //Underdog Settings
        public static bool UnderdogIncreasedKC => Generate.UnderdogIncreasedKC.Get();
        public static float UnderdogKillBonus => Generate.UnderdogKillBonus.Get();
        public static bool UniqueUnderdog => Generate.UniqueUnderdog.Get();
        public static bool UnderdogKnows => Generate.UnderdogKnows.Get();
        public static int UnderdogCount => (int)Generate.UnderdogCount.Get();

        //Multitasker Settings
        public static int MultitaskerCount => (int)Generate.MultitaskerCount.Get();
        public static float Transparancy => Generate.Transparancy.Get();
        public static bool UniqueMultitasker => Generate.UniqueMultitasker.Get();

        //BB Settings
        public static int ButtonBarryCount => (int)Generate.ButtonBarryCount.Get();
        public static float ButtonCooldown => Generate.ButtonCooldown.Get();
        public static bool UniqueButtonBarry => Generate.UniqueButtonBarry.Get();

        //Tiebreaker Settings
        public static bool TiebreakerKnows => Generate.TiebreakerKnows.Get();
        public static int TiebreakerCount => (int)Generate.TiebreakerCount.Get();
        public static bool UniqueTiebreaker => Generate.UniqueTiebreaker.Get();

        //Torch Settings
        public static int TorchCount => (int)Generate.TorchCount.Get();
        public static bool UniqueTorch => Generate.UniqueTorch.Get();

        //Tunneler Settings
        public static bool TunnelerKnows => Generate.TunnelerKnows.Get();
        public static int TunnelerCount => (int)Generate.TunnelerCount.Get();
        public static bool UniqueTunneler => Generate.UniqueTunneler.Get();

        //Radar Settings
        public static bool UniqueRadar => Generate.UniqueRadar.Get();
        public static int RadarCount => (int)Generate.RadarCount.Get();

        //Ninja Settings
        public static int NinjaCount => (int)Generate.NinjaCount.Get();
        public static bool UniqueNinja => Generate.UniqueNinja.Get();

        //Ruthless Settings
        public static int RuthlessCount => (int)Generate.RuthlessCount.Get();
        public static bool UniqueRuthless => Generate.UniqueRuthless.Get();
        public static bool RuthlessKnows => Generate.RuthlessKnows.Get();

        //Insider Settings
        public static bool InsiderKnows => Generate.InsiderKnows.Get();
        public static int InsiderCount => (int)Generate.InsiderCount.Get();
        public static bool UniqueInsider => Generate.UniqueInsider.Get();

        //Traitor Settings
        public static int TraitorCount => (int)Generate.TraitorCount.Get();
        public static bool TraitorColourSwap => Generate.TraitorColourSwap.Get();
        public static bool TraitorKnows => Generate.TraitorKnows.Get();
        public static bool UniqueTraitor => Generate.UniqueTraitor.Get();

        //Fanatic Settings
        public static bool FanaticKnows => Generate.FanaticKnows.Get();
        public static int FanaticCount => (int)Generate.FanaticCount.Get();
        public static bool UniqueFanatic => Generate.UniqueFanatic.Get();
        public static bool FanaticColourSwap => Generate.FanaticColourSwap.Get();

        //Taskmaster Settings
        public static int TMTasksRemaining => (int)Generate.TMTasksRemaining.Get();
        public static int TaskmasterCount => (int)Generate.TaskmasterCount.Get();
        public static bool UniqueTaskmaster => Generate.UniqueTaskmaster.Get();

        //Lovers Settings
        public static bool BothLoversDie => Generate.BothLoversDie.Get();
        public static bool UniqueLovers => Generate.UniqueLovers.Get();
        public static bool LoversChat => Generate.LoversChat.Get();
        public static int LoversCount => (int)Generate.LoversCount.Get();
        public static bool LoversFaction => Generate.LoversFaction.Get();
        public static bool LoversRoles => Generate.LoversRoles.Get();

        //Rivals Settings
        public static bool RivalsChat => Generate.RivalsChat.Get();
        public static int RivalsCount => (int)Generate.RivalsCount.Get();
        public static bool RivalsFaction => Generate.RivalsFaction.Get();
        public static bool RivalsRoles => Generate.RivalsRoles.Get();
        public static bool UniqueRivals => Generate.UniqueRivals.Get();

        //Mafia Settings
        public static int MafiaCount => (int)Generate.MafiaCount.Get();
        public static bool MafiaRoles => Generate.MafiaRoles.Get();
        public static bool UniqueMafia => Generate.UniqueMafia.Get();
        public static bool MafVent => Generate.MafVent.Get();

        //Giant Settings
        public static int GiantCount => (int)Generate.GiantCount.Get();
        public static float GiantSpeed => Generate.GiantSpeed.Get();
        public static float GiantScale => Generate.GiantScale.Get();
        public static bool UniqueGiant => Generate.UniqueGiant.Get();

        //Indomitable Settings
        public static bool UniqueIndomitable => Generate.UniqueIndomitable.Get();
        public static int IndomitableCount => (int)Generate.IndomitableCount.Get();
        public static bool IndomitableKnows => Generate.IndomitableKnows.Get();

        //Overlord Settings
        public static int OverlordCount => (int)Generate.OverlordCount.Get();
        public static bool UniqueOverlord => Generate.UniqueOverlord.Get();
        public static int OverlordMeetingWinCount => (int)Generate.OverlordMeetingWinCount.Get();
        public static bool OverlordKnows => Generate.OverlordKnows.Get();

        //Allied Settings
        public static int AlliedCount => (int)Generate.AlliedCount.Get();
        public static bool UniqueAllied => Generate.UniqueAllied.Get();
        public static AlliedFaction AlliedFaction => (AlliedFaction)Generate.AlliedFaction.Get();

        //Corrupted Settings
        public static int CorruptedCount => (int)Generate.CorruptedCount.Get();
        public static bool UniqueCorrupted => Generate.UniqueCorrupted.Get();
        public static float CorruptedKillCooldown => Generate.CorruptedKillCooldown.Get();
        public static bool AllCorruptedWin => Generate.AllCorruptedWin.Get();
        public static bool CorruptedVent => Generate.CorruptedVent.Get();

        //Dwarf Settings
        public static float DwarfSpeed => Generate.DwarfSpeed.Get();
        public static bool UniqueDwarf => Generate.UniqueDwarf.Get();
        public static float DwarfScale => Generate.DwarfScale.Get();
        public static int DwarfCount => (int)Generate.DwarfCount.Get();

        //Drunk Settings
        public static bool DrunkControlsSwap => Generate.DrunkControlsSwap.Get();
        public static int DrunkCount => (int)Generate.DrunkCount.Get();
        public static bool UniqueDrunk => Generate.UniqueDrunk.Get();
        public static float DrunkInterval => Generate.DrunkInterval.Get();

        //Bait Settings
        public static bool BaitKnows => Generate.BaitKnows.Get();
        public static float BaitMinDelay => Generate.BaitMinDelay.Get();
        public static float BaitMaxDelay => Generate.BaitMaxDelay.Get();
        public static int BaitCount => (int)Generate.BaitCount.Get();
        public static bool UniqueBait => Generate.UniqueBait.Get();

        //Diseased Settings
        public static bool DiseasedKnows => Generate.DiseasedKnows.Get();
        public static float DiseasedMultiplier => Generate.DiseasedKillMultiplier.Get();
        public static int DiseasedCount => (int)Generate.DiseasedCount.Get();
        public static bool UniqueDiseased => Generate.UniqueDiseased.Get();

        //Shy Settings
        public static int ShyCount => (int)Generate.ShyCount.Get();
        public static bool UniqueShy => Generate.UniqueShy.Get();

        //VIP Settings
        public static bool VIPKnows => Generate.VIPKnows.Get();
        public static bool UniqueVIP => Generate.UniqueVIP.Get();
        public static int VIPCount => (int)Generate.VIPCount.Get();

        //Volatile Settings
        public static int VolatileCount => (int)Generate.VolatileCount.Get();
        public static float VolatileInterval => Generate.VolatileInterval.Get();
        public static bool UniqueVolatile => Generate.UniqueVolatile.Get();
        public static bool VolatileKnows => Generate.VolatileKnows.Get();

        //Professional Settings
        public static bool ProfessionalKnows => Generate.ProfessionalKnows.Get();
        public static bool UniqueProfessional => Generate.UniqueProfessional.Get();
        public static int ProfessionalCount => (int)Generate.ProfessionalCount.Get();

        //Flincher Settings
        public static float FlinchInterval => Generate.FlinchInterval.Get();
        public static int FlincherCount => (int)Generate.FlincherCount.Get();
        public static bool UniqueFlincher => Generate.UniqueFlincher.Get();

        //Coward Settings
        public static int CowardCount => (int)Generate.CowardCount.Get();
        public static bool UniqueCoward => Generate.UniqueCoward.Get();

        //NB Settings
        public static int NBMax => (int)Generate.NBMax.Get();
        public static int NBMin => (int)Generate.NBMin.Get();
        public static bool VigiKillsNB => Generate.VigiKillsNB.Get();

        //NK Settings
        public static int NKMax => (int)Generate.NKMax.Get();
        public static int NKMin => (int)Generate.NKMin.Get();
        public static bool NKHasImpVision => Generate.NKHasImpVision.Get();
        public static bool NKsKnow => Generate.NKsKnow.Get();

        //CSv Settings
        public static int CSvMax => (int)Generate.CSvMax.Get();
        public static int CSvMin => (int)Generate.CSvMin.Get();

        //CA Settings
        public static int CAMax => (int)Generate.CAMax.Get();
        public static int CAMin => (int)Generate.CAMin.Get();

        //CK Settings
        public static int CKMax => (int)Generate.CKMax.Get();
        public static int CKMin => (int)Generate.CKMin.Get();

        //CS Settings
        public static int CSMax => (int)Generate.CSMax.Get();
        public static int CSMin => (int)Generate.CSMin.Get();

        //CI Settings
        public static int CIMax => (int)Generate.CIMax.Get();
        public static int CIMin => (int)Generate.CIMin.Get();

        //CP Settings
        public static int CPMax => (int)Generate.CPMax.Get();
        public static int CPMin => (int)Generate.CPMin.Get();

        //IC Settings
        public static int ICMax => (int)Generate.ICMax.Get();
        public static int ICMin => (int)Generate.ICMin.Get();

        //ID Settings
        public static int IDMax => (int)Generate.IDMax.Get();
        public static int IDMin => (int)Generate.IDMin.Get();

        //IS Settings
        public static int ISMax => (int)Generate.ISMax.Get();
        public static int ISMin => (int)Generate.ISMin.Get();

        //IK Settings
        public static int IKMax => (int)Generate.IKMax.Get();
        public static int IKMin => (int)Generate.IKMin.Get();

        //SD Settings
        public static int SDMax => (int)Generate.SDMax.Get();
        public static int SDMin => (int)Generate.SDMin.Get();

        //SyK Settings
        public static int SyKMax => (int)Generate.SyKMax.Get();
        public static int SyKMin => (int)Generate.SyKMin.Get();

        //SSu Settings
        public static int SSuMax => (int)Generate.SSuMax.Get();
        public static int SSuMin => (int)Generate.SSuMin.Get();

        //SP Settings
        public static int SPMax => (int)Generate.SPMax.Get();
        public static int SPMin => (int)Generate.SPMin.Get();

        //NE Settings
        public static int NEMax => (int)Generate.NEMax.Get();
        public static int NEMin => (int)Generate.NEMin.Get();

        //NN Settings
        public static int NNMax => (int)Generate.NNMax.Get();
        public static int NNMin => (int)Generate.NNMin.Get();
    }
}