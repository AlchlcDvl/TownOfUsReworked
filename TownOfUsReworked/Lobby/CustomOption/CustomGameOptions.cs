using TownOfUsReworked.Patches;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.EngineerMod;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.SheriffMod;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod;
using TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.ExecutionerMod;
using TownOfUsReworked.PlayerLayers.Abilities.RevealerMod;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MediumMod;
using TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.GuardianAngelMod;
using TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.SerialKillerMod;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.ShifterMod;
using TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.ConsigliereMod;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.VigilanteMod;
using TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.UndertakerMod;
using TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.NeutralsMod;
using TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.DisguiserMod;

namespace TownOfUsReworked.Lobby.CustomOption
{
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
        public static float ChatCooldown => Generate.ChatCooldown.Get();
        public static int DiscussionTime => (int)Generate.DiscussionTime.Get();
        public static int VotingTime => (int)Generate.VotingTime.Get();
        public static TaskBarMode TaskBarMode => (TaskBarMode)Generate.TaskBarMode.Get();
        public static bool EjectionRevealsRole => Generate.EjectionRevealsRole.Get();

        //Game Modifiers
        public static bool ColourblindComms => Generate.ColourblindComms.Get();
        public static bool MeetingColourblind => Generate.MeetingColourblind.Get();
        public static WhoCanVentOptions WhoCanVent => (WhoCanVentOptions)Generate.WhoCanVent.Get();
        public static bool VisualTasks => Generate.VisualTasks.Get();
        public static bool AnonymousVoting => Generate.AnonymousVoting.Get();
        public static bool FactionSeeRoles => Generate.FactionSeeRoles.Get();
        public static bool ParallelMedScans => Generate.ParallelMedScans.Get();
        public static DisableSkipButtonMeetings SkipButtonDisable => (DisableSkipButtonMeetings)Generate.SkipButtonDisable.Get();
        public static bool LocationReports => Generate.LocationReports.Get();
        public static RoleFactionReports RoleFactionReports => (RoleFactionReports)Generate.RoleFactionReports.Get();
        public static bool KillerReports => Generate.KillerReports.Get();
        public static bool NoNames => Generate.NoNames.Get();
        public static bool Whispers => Generate.Whispers.Get();
        public static bool AppearanceAnimation => Generate.AppearanceAnimation.Get();
        public static bool LighterDarker => Generate.LighterDarker.Get();
        public static bool PlayerNumbers => Generate.PlayerNumbers.Get();
        public static bool RandomSpawns => Generate.RandomSpawns.Get();

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
        //public static Map Map => (Map)Generate.Map.Get();
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
        public static float ReactorTimer => Generate.ReactorTimer.Get();
        public static float OxygenTimer => Generate.OxygenTimer.Get();

        //Airship Settings
        public static bool NewSpawns => Generate.NewSpawns.Get();
        public static bool MeetingSpawnChoice => Generate.MeetingSpawnChoice.Get();
        public static bool MoveDivert => Generate.MoveDivert.Get();
        public static bool MoveFuel => Generate.MoveFuel.Get();
        public static bool MoveVitals => Generate.MoveVitals.Get();
        public static bool AddTeleporters => Generate.AddTeleporters.Get();
        public static bool CallPlatform => Generate.CallPlatform.Get();
        public static float MinDoorSwipeTime => Generate.MinDoorSwipeTime.Get();
        public static float CrashTimer => Generate.CrashTimer.Get();
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
        public static int InvestigatorOn => (int)Generate.InvestigatorOn.Get();
        public static int ConcealerOn => (int)Generate.ConcealerOn.Get();
        public static int TimeLordOn => (int)Generate.TimeLordOn.Get();
        public static int MedicOn => (int)Generate.MedicOn.Get();
        public static int GlitchOn => (int)Generate.GlitchOn.Get();
        public static int MorphlingOn => (int)Generate.MorphlingOn.Get();
        public static int ExecutionerOn => (int)Generate.ExecutionerOn.Get();
        public static int AgentOn => (int)Generate.AgentOn.Get();
        public static int CrewmateOn => (int)Generate.CrewmateOn.Get();
        public static int ImpostorOn => (int)Generate.ImpostorOn.Get();
        public static int WraithOn => (int)Generate.WraithOn.Get();
        public static int ArsonistOn => (int)Generate.ArsonistOn.Get();
        public static int AltruistOn => (int)Generate.AltruistOn.Get();
        public static int UndertakerOn => (int)Generate.UndertakerOn.Get();
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
        public static int GorgonOn => (int)Generate.GorgonOn.Get();
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
        public static int MorticianOn => (int)Generate.FramerOn.Get();
        public static int MurdererOn => (int)Generate.MurdererOn.Get();
        public static int WarperOn => (int)Generate.WarperOn.Get();
        public static int AnarchistOn => (int)Generate.AnarchistOn.Get();
        public static int DraculaOn => (int)Generate.DraculaOn.Get();
        public static int ConsigliereOn => (int)Generate.ConsigliereOn.Get();
        public static int MinerOn => (int)Generate.MinerOn.Get();
        public static int TimeMasterOn => (int)Generate.TimeMasterOn.Get();

        //Ability Spawn
        public static int AssassinOn => (int)Generate.AssassinOn.Get();
        public static int UnderdogOn => (int)Generate.UnderdogOn.Get();
        public static int RevealerOn => (int)Generate.RevealerOn.Get();
        public static int SnitchOn => (int)Generate.SnitchOn.Get();
        public static int MultitaskerOn => (int)Generate.MultitaskerOn.Get();
        public static int TorchOn => (int)Generate.TorchOn.Get();
        public static int LighterOn => (int)Generate.LighterOn.Get();
        public static int ButtonBarryOn => (int)Generate.ButtonBarryOn.Get();
        public static int TunnelerOn => (int)Generate.TunnelerOn.Get();
        public static int RadarOn => (int)Generate.RadarOn.Get();
        public static int TiebreakerOn => (int)Generate.TiebreakerOn.Get();

        //Objectifier Spawn
        public static int RivalsOn => (int)Generate.RivalsOn.Get();
        public static int FanaticOn => (int)Generate.FanaticOn.Get();
        public static int TraitorOn => (int)Generate.TraitorOn.Get();
        public static int TaskmasterOn => (int)Generate.TaskmasterOn.Get();
        public static int CorruptedOn => (int)Generate.CorruptedOn.Get();
        public static int OverlordOn => (int)Generate.OverlordOn.Get();
        public static int LoversOn => (int)Generate.LoversOn.Get();
        public static int PhantomOn => (int)Generate.PhantomOn.Get();

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
        public static int InsiderOn => (int)Generate.InsiderOn.Get();

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
        public static bool AltImps => Generate.AltImps.Get();
        public static bool SyndicateVent => Generate.SyndicateVent.Get();
        public static int SyndicateCount => (int)Generate.SyndicateCount.Get();
        public static bool CustomSynColors => Generate.CustomSynColors.Get();
        public static float ChaosDriveKillCooldown => Generate.ChaosDriveKillCooldown.Get();
        public static int ChaosDriveMeetingCount => (int)Generate.ChaosDriveMeetingCount.Get();
        public static int SyndicateMax => (int)Generate.SyndicateMax.Get();
        public static int SyndicateMin => (int)Generate.SyndicateMin.Get();

        //Neutral Options
        public static float NeutralVision => Generate.NeutralVision.Get();
        public static bool LightsAffectNeutrals => Generate.LightsAffectNeutrals.Get();
        public static NoSolo NoSolo => (NoSolo)Generate.NoSolo.Get();
        public static bool CustomNeutColors => Generate.CustomNeutColors.Get();
        public static int NeutralMax => (int)Generate.NeutralMax.Get();
        public static int NeutralMin => (int)Generate.NeutralMin.Get();

        //VH Settings
        public static int VampireHunterCount => (int)Generate.VampireHunterCount.Get();
        public static bool UniqueVampireHunter => Generate.UniqueVampireHunter.Get();
        public static float StakeCooldown => Generate.StakeCooldown.Get();

        //Agent Settings
        public static int AgentCount => (int)Generate.AgentCount.Get();
        public static bool UniqueAgent => Generate.UniqueAgent.Get();

        //Detective Settings
        public static int DetectiveCount => (int)Generate.DetectiveCount.Get();
        public static float ExamineCd => Generate.ExamineCooldown.Get();
        public static bool UniqueDetective => Generate.UniqueDetective.Get();
        public static float RecentKill => Generate.RecentKill.Get();
        public static float DetectiveRoleDuration => Generate.DetectiveRoleDuration.Get();
        public static float DetectiveFactionDuration => Generate.DetectiveFactionDuration.Get();

        //Inspector Settings
        public static int InspectorCount => (int)Generate.InspectorCount.Get();
        public static float InspectCooldown => Generate.InspectCooldown.Get();
        public static bool UniqueInspector => Generate.UniqueInspector.Get();

        //Investigator Settings
        public static int InvestigatorCount => (int)Generate.InvestigatorCount.Get();
        public static float FootprintSize => Generate.FootprintSize.Get();
        public static float FootprintInterval => Generate.FootprintInterval.Get();
        public static float FootprintDuration => Generate.FootprintDuration.Get();
        public static bool AnonymousFootPrint => Generate.AnonymousFootPrint.Get();
        public static bool VentFootprintVisible => Generate.VentFootprintVisible.Get();
        public static bool UniqueInvestigator => Generate.UniqueInvestigator.Get();

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
        public static float CoronerKillerNameTime => Generate.CoronerKillerNameTime.Get();

        //Sheriff Settings
        public static InterrogatePer InterrogatePer => (InterrogatePer)Generate.InterrogatePer.Get();
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
        public static float MinAmountOfPlayersInBug => Generate.MinAmountOfPlayersInBug.Get();
        public static bool BugsRemoveOnNewRound => Generate.BugsRemoveOnNewRound.Get();
        public static bool UniqueOperative => Generate.UniqueOperative.Get();

        //Veteran Settings
        public static float AlertCd => Generate.AlertCooldown.Get();
        public static float AlertDuration => Generate.AlertDuration.Get();
        public static int MaxAlerts => (int)Generate.MaxAlerts.Get();
        public static int VeteranCount => (int)Generate.VeteranCount.Get();
        public static bool UniqueVeteran => Generate.UniqueVeteran.Get();

        //Vigilante Settings
        public static VigiOptions VigiOptions => (VigiOptions)Generate.VigiOptions.Get();
        public static int VigilanteCount => (int)Generate.VigilanteCount.Get();
        public static float VigiKillCd => Generate.VigiKillCd.Get();
        public static bool VigiKnowsInno => Generate.VigiKnowsInno.Get();
        public static bool UniqueVigilante => Generate.UniqueVigilante.Get();
        public static bool MisfireKillsInno => Generate.MisfireKillsInno.Get();
        public static VigiNotif VigiNotifOptions => (VigiNotif)Generate.VigiNotifOptions.Get();

        //Altruist Settings
        public static bool AltruistTargetBody => Generate.AltruistTargetBody.Get();
        public static bool UniqueAltruist => Generate.UniqueAltruist.Get();
        public static float ReviveDuration => Generate.ReviveDuration.Get();
        public static int AltruistCount => (int)Generate.AltruistCount.Get();

        //Medic Settings
        public static ShieldOptions ShowShielded => (ShieldOptions)Generate.ShowShielded.Get();
        public static int MedicCount => (int)Generate.MedicCount.Get();
        public static bool UniqueMedic => Generate.UniqueMedic.Get();
        public static NotificationOptions NotificationShield => (NotificationOptions)Generate.WhoGetsNotification.Get();
        public static bool ShieldBreaks => Generate.ShieldBreaks.Get();

        //TL Settings
        public static float RewindDuration => Generate.RewindDuration.Get();
        public static float RewindCooldown => Generate.RewindCooldown.Get();
        public static int RewindMaxUses => (int)Generate.RewindMaxUses.Get();
        public static bool TLImmunity => Generate.TLImmunity.Get();
        public static bool RewindRevive => Generate.RewindRevive.Get();
        public static int TimeLordCount => (int)Generate.TimeLordCount.Get();
        public static bool UniqueTimeLord => Generate.UniqueTimeLord.Get();

        //Mayor Settings
        public static bool MayorAnonymous => Generate.MayorAnonymous.Get();
        public static int MayorVoteBank => (int)Generate.MayorVoteBank.Get();
        public static bool UniqueMayor => Generate.UniqueMayor.Get();
        public static int MayorCount => (int)Generate.MayorCount.Get();

        //Swapper Settings
        public static bool SwapperButton => Generate.SwapperButton.Get();
        public static int SwapperCount => (int)Generate.SwapperCount.Get();
        public static bool SwapAfterVoting => Generate.SwapAfterVoting.Get();
        public static bool UniqueSwapper => Generate.UniqueSwapper.Get();

        //Engineer Settings
        public static int EngineerCount => (int)Generate.EngineerCount.Get();
        public static bool UniqueEngineer => Generate.UniqueEngineer.Get();
        public static EngineerFixPer EngineerFixPer => (EngineerFixPer)Generate.EngineerPer.Get();

        //Escort Settings
        public static int EscortCount => (int)Generate.EscortCount.Get();
        public static float EscRoleblockCooldown => Generate.EscRoleblockCooldown.Get();
        public static float EscRoleblockDuration => Generate.EscRoleblockDuration.Get();
        public static bool UniqueEscort => Generate.UniqueEscort.Get();

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
        public static BecomeOptions GaOnTargetDeath => (BecomeOptions)Generate.GaOnTargetDeath.Get();
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

        //Jester Settings
        public static bool VigiKillsJester => Generate.VigiKillsJester.Get();
        public static bool JestEjectScreen => Generate.JestEjectScreen.Get();
        public static bool JestVentSwitch => Generate.JestSwitchVent.Get();
        public static bool JesterButton => Generate.JesterButton.Get();
        public static bool JesterVent => Generate.JesterVent.Get();
        public static int JesterCount => (int)Generate.JesterCount.Get();
        public static bool UniqueJester => Generate.UniqueJester.Get();

        //Troll Settings
        public static bool TrollVent => Generate.TrollVent.Get();
        public static float InteractCooldown => Generate.InteractCooldown.Get();
        public static int TrollCount => (int)Generate.TrollCount.Get();
        public static bool TrollVentSwitch => Generate.TrollSwitchVent.Get();
        public static bool UniqueTroll => Generate.UniqueTroll.Get();

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
        public static OnTargetDead OnTargetDead => (OnTargetDead)Generate.OnTargetDead.Get();
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

        //Glitch Settings
        public static bool GlitchVent => Generate.GlitchVent.Get();
        public static float GlitchCooldown => Generate.GlitchCooldown.Get();
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

        //Plaguebearer Settings
        public static bool PBVent => Generate.PBVent.Get();
        public static float InfectCd => Generate.InfectCooldown.Get();
        public static int PlaguebearerCount => (int)Generate.PlaguebearerCount.Get();
        public static bool UniquePlaguebearer => Generate.UniquePlaguebearer.Get();

        //Arsonist Settings
        public static bool ArsoIgniteNeedsCooldown => Generate.ArsoIgniteNeedsCooldown.Get();
        public static bool ArsoVent => Generate.ArsoVent.Get();
        public static float DouseCd => Generate.DouseCooldown.Get();
        public static float IgniteCd => Generate.IgniteCooldown.Get();
        public static int ArsonistCount => (int)Generate.ArsonistCount.Get();
        public static bool UniqueArsonist => Generate.UniqueArsonist.Get();

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
        public static bool DraculaConvertNeuts => Generate.DraculaConvertNeuts.Get();
        public static float BiteCd => Generate.BiteCooldown.Get();
        public static int DraculaCount => (int)Generate.DraculaCount.Get();
        public static int AliveVampCount => (int)Generate.AliveVampCount.Get();
        public static bool UniqueDracula => Generate.UniqueDracula.Get();

        //Jackal Settings
        public static bool JackalVent => Generate.JackalVent.Get();
        public static bool RecruitVent => Generate.RecruitVent.Get();
        public static int JackalCount => (int)Generate.JackalCount.Get();
        public static bool UniqueJackal => Generate.UniqueJackal.Get();

        //Vampire Settings
        public static bool VampVent => Generate.VampVent.Get();

        //Dampyr Settings
        public static bool DampVent => Generate.DampVent.Get();
        public static float DampBiteCd => Generate.DampBiteCooldown.Get();

        //Pestilence Settings
        public static float PestKillCd => Generate.PestKillCooldown.Get();
        public static bool PlayersAlerted => Generate.PlayersAlerted.Get();
        public static bool PestSpawn => Generate.PestSpawn.Get();
        public static bool PestVent => Generate.PestVent.Get();

        //Janitor Settings
        public static float JanitorCleanCd => Generate.JanitorCleanCd.Get();
        public static int JanitorCount => (int)Generate.JanitorCount.Get();
        public static bool SoloBoost => Generate.SoloBoost.Get();
        public static bool UniqueJanitor => Generate.UniqueJanitor.Get();

        //Blackmailer Settings
        public static float BlackmailCd => Generate.BlackmailCooldown.Get();
        public static int BlackmailerCount => (int)Generate.BlackmailerCount.Get();
        public static bool UniqueBlackmailer => Generate.UniqueBlackmailer.Get();

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

        //Undertaker Settings
        public static UndertakerOptions UndertakerVentOptions => (UndertakerOptions)Generate.UndertakerVentOptions.Get();
        public static int DragModifier => (int)Generate.DragModifier.Get();
        public static float DragCd => Generate.DragCooldown.Get();
        public static int UndertakerCount => (int)Generate.UndertakerCount.Get();
        public static bool UniqueUndertaker => Generate.UniqueUndertaker.Get();

        //Morphling Settings
        public static bool MorphlingVent => Generate.MorphlingVent.Get();
        public static int MorphlingCount => (int)Generate.MorphlingCount.Get();
        public static float MorphlingCd => Generate.MorphlingCooldown.Get();
        public static float MorphlingDuration => Generate.MorphlingDuration.Get();
        public static bool UniqueMorphling => Generate.UniqueMorphling.Get();

        //Wraith Settings
        public static bool WraithVent => Generate.WraithVent.Get();
        public static float InvisCd => Generate.InvisCooldown.Get();
        public static float InvisDuration => Generate.InvisDuration.Get();
        public static int WraithCount => (int)Generate.WraithCount.Get();
        public static bool UniqueWraith => Generate.UniqueWraith.Get();

        //Poisoner Settings
        public static bool PoisonerVent => Generate.PoisonerVent.Get();
        public static float PoisonCd => Generate.PoisonCooldown.Get();
        public static float PoisonDuration => Generate.PoisonDuration.Get();
        public static int PoisonerCount => (int)Generate.PoisonerCount.Get();
        public static bool UniquePoisoner => Generate.UniquePoisoner.Get();

        //Teleporter Settings
        public static bool TeleVent => Generate.TeleVent.Get();
        public static float TeleportCd => Generate.TeleportCd.Get();
        public static int TeleporterCount => (int)Generate.TeleporterCount.Get();
        public static bool UniqueTeleporter => Generate.UniqueTeleporter.Get();

        //Consigliere Settings
        public static ConsigInfo ConsigInfo => (ConsigInfo)Generate.ConsigInfo.Get();
        public static float ConsigCd => Generate.RevealCooldown.Get();
        public static int ConsigliereCount => (int)Generate.ConsigCount.Get();
        public static bool UniqueConsigliere => Generate.UniqueConsigliere.Get();

        //TM Settings
        public static float FreezeDuration => Generate.FreezeDuration.Get();
        public static float FreezeCooldown => Generate.FreezeCooldown.Get();
        public static int TimeMasterCount => (int)Generate.TimeMasterCount.Get();
        public static bool UniqueTimeMaster => Generate.UniqueTimeMaster.Get();

        //Consort Settings
        public static int ConsortCount => (int)Generate.ConsortCount.Get();
        public static float ConsRoleblockCooldown => Generate.ConsRoleblockCooldown.Get();
        public static bool UniqueConsort => Generate.UniqueConsort.Get();
        public static float ConsRoleblockDuration => Generate.ConsRoleblockDuration.Get();

        //Godfather Settings
        public static int GodfatherCount => (int)Generate.GodfatherCount.Get();
        public static bool UniqueGodfather => Generate.UniqueGodfather.Get();

        //Miner Settings
        public static float MineCd => Generate.MineCooldown.Get();
        public static int MinerCount => (int)Generate.MinerCount.Get();
        public static bool UniqueMiner => Generate.UniqueMiner.Get();

        //Impostor Settings
        public static int ImpCount => (int)Generate.ImpCount.Get();

        //Anarchist Settings
        public static int AnarchistCount => (int)Generate.AnarchistCount.Get();

        //Mafioso Settings
        public static bool PromotedMafiosoCanPromote => Generate.PromotedMafiosoCanPromote.Get();
        public static float MafiosoAbilityCooldownDecrease => Generate.MafiosoAbilityCooldownDecrease.Get();

        //Sidekick Settings
        public static bool PromotedSidekickCanPromote => Generate.PromotedSidekickCanPromote.Get();
        public static float SidekickAbilityCooldownDecrease => Generate.SidekickAbilityCooldownDecrease.Get();

        //Framer Settings
        public static int FramerCount => (int)Generate.FramerCount.Get();
        public static float FrameCooldown => Generate.FrameCooldown.Get();
        public static bool UniqueFramer => Generate.UniqueFramer.Get();

        //ShapeShifter Settings
        public static int ShapeshifterCount => (int)Generate.ShapeshifterCount.Get();
        public static float ShapeshiftCooldown => Generate.ShapeshiftCooldown.Get();
        public static float ShapeshiftDuration => Generate.ShapeshiftDuration.Get();
        public static bool UniqueShapeshifter => Generate.UniqueShapeshifter.Get();

        //Gorgon Settings
        public static float GazeCooldown => Generate.GazeCooldown.Get();
        public static float GazeTime => Generate.GazeTime.Get();
        public static int GorgonCount => (int)Generate.GorgonCount.Get();
        public static bool UniqueGorgon => Generate.UniqueGorgon.Get();

        //Bomber Settings
        public static float BombCooldown => Generate.BombCooldown.Get();
        public static int BomberCount => (int)Generate.BomberCount.Get();
        public static float BombRange => Generate.BombRange.Get();
        public static bool UniqueBomber => Generate.UniqueBomber.Get();

        //Concealer Settings
        public static int ConcealerCount => (int)Generate.ConcealerCount.Get();
        public static float ConcealCooldown => Generate.ConcealCooldown.Get();
        public static float ConcealDuration => Generate.ConcealDuration.Get();
        public static bool UniqueConcealer => Generate.UniqueConcealer.Get();

        //Disguiser Settings
        public static int DisguiserCount => (int)Generate.DisguiserCount.Get();
        public static float DisguiseDuration => Generate.DisguiseDuration.Get();
        public static float DisguiseCooldown => Generate.DisguiseCooldown.Get();
        public static float TimeToDisguise => Generate.TimeToDisguise.Get();
        public static DisguiserTargets DisguiseTarget => (DisguiserTargets)Generate.DisguiseTarget.Get();
        public static bool UniqueDisguiser => Generate.UniqueDisguiser.Get();

        //Rebel Settings
        public static int RebelCount => (int)Generate.RebelCount.Get();
        public static bool UniqueRebel => Generate.UniqueRebel.Get();

        //Warper Settings
        public static float WarpCooldown => Generate.WarpCooldown.Get();
        public static bool UniqueWarper => Generate.UniqueWarper.Get();
        public static int WarperCount => (int)Generate.WarperCount.Get();

        //Modifier Settings
        public static bool CustomModifierColors => Generate.CustomModifierColors.Get();

        //Objectifier Settings
        public static bool CustomObjectifierColors => Generate.CustomObjectifierColors.Get();

        //Ability Settings
        public static bool CustomAbilityColors => Generate.CustomAbilityColors.Get();

        //Snitch Settings
        public static bool SnitchSeesNeutrals => Generate.SnitchSeesNeutrals.Get();
        public static int SnitchCount => (int)Generate.SnitchCount.Get();
        public static bool SnitchSeesCrew => Generate.SnitchSeesCrew.Get();
        public static bool SnitchSeesRoles => Generate.SnitchSeesRoles.Get();
        public static bool SnitchSeesImpInMeeting => Generate.SnitchSeesImpInMeeting.Get();
        public static bool SnitchSeesTraitor => Generate.SnitchSeesTraitor.Get();
        public static bool SnitchKnows => Generate.SnitchKnows.Get();
        public static int SnitchTasksRemaining => (int)Generate.SnitchTasksRemaining.Get();

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

        //Underdog Settings
        public static bool UnderdogIncreasedKC => Generate.UnderdogIncreasedKC.Get();
        public static float UnderdogKillBonus => Generate.UnderdogKillBonus.Get();
        public static bool UnderdogKnows => Generate.UnderdogKnows.Get();
        public static int UnderdogCount => (int)Generate.UnderdogCount.Get();

        //Revealer Settings
        public static bool RevealerKnows => Generate.RevealerKnows.Get();
        public static int RevealerTasksRemainingClicked => (int)Generate.RevealerTasksRemainingClicked.Get();
        public static int RevealerTasksRemainingAlert => (int)Generate.RevealerTasksRemainingAlert.Get();
        public static int RevealerCount => (int)Generate.RevealerCount.Get();
        public static RevealerCanBeClickedBy RevealerCanBeClickedBy => (RevealerCanBeClickedBy)Generate.RevealerCanBeClickedBy.Get();
        public static bool RevealerRevealsNeutrals => Generate.RevealerRevealsNeutrals.Get();
        public static bool RevealerRevealsCrew => Generate.RevealerRevealsCrew.Get();
        public static bool RevealerRevealsTraitor => Generate.RevealerRevealsTraitor.Get();
        public static bool RevealerRevealsRoles => Generate.RevealerRevealsRoles.Get();

        //Multitasker Settings
        public static int MultitaskerCount => (int)Generate.MultitaskerCount.Get();
        public static float Transparancy => Generate.Transparancy.Get();

        //BB Settings
        public static int ButtonBarryCount => (int)Generate.ButtonBarryCount.Get();

        //Tiebreaker Settings
        public static bool TiebreakerKnows => Generate.TiebreakerKnows.Get();
        public static int TiebreakerCount => (int)Generate.TiebreakerCount.Get();

        //Torch Settings
        public static int TorchCount => (int)Generate.TorchCount.Get();

        //Tunneler Settings
        public static bool TunnelerKnows => Generate.TunnelerKnows.Get();
        public static int TunnelerCount => (int)Generate.TunnelerCount.Get();

        //Lovers Settings
        public static bool BothLoversDie => Generate.BothLoversDie.Get();
        public static bool LoversChat => Generate.LoversChat.Get();
        public static int LoversCount => (int)Generate.LoversCount.Get();
        public static bool LoversFaction => Generate.LoversFaction.Get();
        public static bool LoversRoles => Generate.LoversRoles.Get();

        //Rivals Settings
        public static bool RivalsChat => Generate.RivalsChat.Get();
        public static int RivalsCount => (int)Generate.RivalsCount.Get();
        public static bool RivalsFaction => Generate.RivalsFaction.Get();
        public static bool RivalsRoles => Generate.RivalsRoles.Get();

        //Radar Settings
        public static int RadarCount => (int)Generate.RadarCount.Get();

        //Lighter Settings
        public static int LighterCount => (int)Generate.LighterCount.Get();

        //Insider Settings
        public static bool InsiderKnows => Generate.InsiderKnows.Get();
        public static int InsiderCount => (int)Generate.InsiderCount.Get();

        //Traitor Settings
        public static int TraitorCount => (int)Generate.TraitorCount.Get();
        public static bool TraitorColourSwap => Generate.TraitorColourSwap.Get();
        public static bool TraitorKnows => Generate.TraitorKnows.Get();
        public static bool TraitorCanAssassin => Generate.TraitorCanAssassin.Get();

        //Fanatic Settings
        public static bool FanaticKnows => Generate.FanaticKnows.Get();
        public static int FanaticCount => (int)Generate.FanaticCount.Get();
        public static bool FanaticCanAssassin => Generate.FanaticCanAssassin.Get();

        //Phantom Settings
        public static int PhantomCount => (int)Generate.PhantomCount.Get();
        public static bool PhantomKnows => Generate.PhantomKnows.Get();
        public static int PhantomTasksRemaining => (int)Generate.PhantomTasksRemaining.Get();

        //Taskmaster Settings
        public static int TMTasksRemaining => (int)Generate.TMTasksRemaining.Get();
        public static int TaskmasterCount => (int)Generate.TaskmasterCount.Get();

        //Giant Settings
        public static int GiantCount => (int)Generate.GiantCount.Get();
        public static float GiantSpeed => Generate.GiantSpeed.Get();
        public static float GiantScale => Generate.GiantScale.Get();

        //Dwarf Settings
        public static float DwarfSpeed => Generate.DwarfSpeed.Get();
        public static float DwarfScale => Generate.DwarfScale.Get();
        public static int DwarfCount => (int)Generate.DwarfCount.Get();

        //Drunk Settings
        public static bool DrunkControlsSwap => Generate.DrunkControlsSwap.Get();
        public static int DrunkCount => (int)Generate.DrunkCount.Get();
        public static float DrunkInterval => Generate.DrunkInterval.Get();

        //Bait Settings
        public static bool BaitKnows => Generate.BaitKnows.Get();
        public static float BaitMinDelay => Generate.BaitMinDelay.Get();
        public static float BaitMaxDelay => Generate.BaitMaxDelay.Get();
        public static int BaitCount => (int)Generate.BaitCount.Get();

        //Diseased Settings
        public static bool DiseasedKnows => Generate.DiseasedKnows.Get();
        public static float DiseasedMultiplier => Generate.DiseasedKillMultiplier.Get();
        public static int DiseasedCount => (int)Generate.DiseasedCount.Get();

        //Shy Settings
        public static int ShyCount => (int)Generate.ShyCount.Get();

        //VIP Settings
        public static bool VIPKnows => Generate.VIPKnows.Get();
        public static int VIPCount => (int)Generate.VIPCount.Get();

        //Volatile Settings
        public static int VolatileCount => (int)Generate.VolatileCount.Get();
        public static float VolatileInterval => Generate.VolatileInterval.Get();
        public static bool VolatileKnows => Generate.VolatileKnows.Get();

        //Professional Settings
        public static bool ProfessionalKnows => Generate.ProfessionalKnows.Get();
        public static int ProfessionalCount => (int)Generate.ProfessionalCount.Get();

        //Flincher Settings
        public static float FlinchInterval => Generate.FlinchInterval.Get();
        public static int FlincherCount => (int)Generate.FlincherCount.Get();

        //Coward Settings
        public static int CowardCount => (int)Generate.CowardCount.Get();

        //NB Settings
        public static int NBMax => (int)Generate.NBMax.Get();
        public static int NBMin => (int)Generate.NBMin.Get();
        public static bool VigiKillsNB => Generate.VigiKillsNB.Get();

        //NK Settings
        public static int NKMax => (int)Generate.NKMax.Get();
        public static int NKMin => (int)Generate.NKMin.Get();
        public static bool NKHasImpVision => Generate.NKHasImpVision.Get();
        public static NKsKnow NKsKnow => (NKsKnow)Generate.NKsKnow.Get();

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
        public static int IKMax => 0;
        public static int IKMin => 0;

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
        public static int SPMax => 0;
        public static int SPMin => 0;

        //NE Settings
        public static int NEMax => (int)Generate.NEMax.Get();
        public static int NEMin => (int)Generate.NEMin.Get();

        //NN Settings
        public static int NNMax => (int)Generate.NNMax.Get();
        public static int NNMin => (int)Generate.NNMin.Get();

        //Corrupted Settings
        public static int CorruptedCount => (int)Generate.CorruptedCount.Get();
        public static float CorruptedKillCooldown => Generate.CorruptedKillCooldown.Get();
    }
}