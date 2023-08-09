namespace TownOfUsReworked.CustomOptions;

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
    public static float MeetingCooldowns => Generate.MeetingCooldowns.Get();
    public static float ReportDistance => Generate.ReportDistance.Get();
    public static float ChatCooldown => Generate.ChatCooldown.Get();
    public static int ChatCharacterLimit => (int)Generate.ChatCharacterLimit.Get();
    public static int DiscussionTime => (int)Generate.DiscussionTime.Get();
    public static int VotingTime => (int)Generate.VotingTime.Get();
    public static TaskBar TaskBarMode => (TaskBar)Generate.TaskBarMode.Get();
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
    public static bool RandomSpawns => Generate.RandomSpawns.Get();
    public static bool EnableAbilities => Generate.EnableAbilities.Get();
    public static bool EnableModifiers => Generate.EnableModifiers.Get();
    public static bool EnableObjectifiers => Generate.EnableObjectifiers.Get();
    public static bool VentTargetting => Generate.VentTargetting.Get();
    public static bool FirstKillShield => Generate.FirstKillShield.Get();
    public static WhoCanSeeFirstKillShield WhoSeesFirstKillShield => (WhoCanSeeFirstKillShield)Generate.WhoSeesFirstKillShield.Get();

    //Better Sabotage Settings
    public static float ReactorShake => Generate.ReactorShake.Get();
    public static bool OxySlow => Generate.OxySlow.Get();
    public static bool ColourblindComms => Generate.ColourblindComms.Get();
    public static bool MeetingColourblind => Generate.MeetingColourblind.Get();
    //public static bool NightVision => Generate.NightVision.Get();
    //public static bool EvilsIgnoreNV => Generate.EvilsIgnoreNV.Get();

    //Announcement Settings
    public static bool LocationReports => Generate.LocationReports.Get();
    public static RoleFactionReports RoleFactionReports => (RoleFactionReports)Generate.RoleFactionReports.Get();
    public static RoleFactionReports KillerReports => (RoleFactionReports)Generate.KillerReports.Get();
    public static bool GameAnnouncements => Generate.GameAnnouncements.Get();

    //QOL Changes
    public static bool DeadSeeEverything => Generate.DeadSeeEverything.Get();
    public static bool ObstructNames => Generate.ObstructNames.Get();

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
    public static MapEnum Map
    {
        get
        {
            var map = Generate.Map.Get();

            if (map is 0 or 1 or 2 or 3)
                return (MapEnum)map;
            else if (map == 4)
            {
                if (SubLoaded)
                    return MapEnum.Submerged;
                else if (LILoaded)
                    return MapEnum.LevelImpostor;
                else
                    return MapEnum.Random;
            }
            else if (map == 5 && LILoaded)
                return MapEnum.LevelImpostor;
            else
                return MapEnum.Random;
        }
    }
    public static float RandomMapSkeld => Generate.RandomMapSkeld.Get();
    public static float RandomMapMira => Generate.RandomMapMira.Get();
    public static float RandomMapPolus => Generate.RandomMapPolus.Get();
    public static float RandomMapAirship => Generate.RandomMapAirship.Get();
    public static float RandomMapSubmerged => SubLoaded ? Generate.RandomMapSubmerged.Get() : 0f;
    public static float RandomMapLevelImpostor => LILoaded ? Generate.RandomMapLevelImpostor.Get() : 0f;
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

    //Mira Settings
    public static bool MiraHQVentImprovements => Generate.MiraHQVentImprovements.Get();

    //Skeld Settings
    public static bool SkeldVentImprovements => Generate.SkeldVentImprovements.Get();
    //public static float ReactorTimer => Generate.ReactorTimer.Get();
    //public static float OxygenTimer => Generate.OxygenTimer.Get();

    //Airship Settings
    public static bool MoveDivert => Generate.MoveDivert.Get();
    public static bool MoveFuel => Generate.MoveFuel.Get();
    public static bool MoveVitals => Generate.MoveVitals.Get();
    public static float MinDoorSwipeTime => Generate.MinDoorSwipeTime.Get();
    public static float CrashTimer => Generate.CrashTimer.Get();
    public static AirshipSpawnType SpawnType => (AirshipSpawnType)Generate.SpawnType.Get();
    public static MoveAdmin MoveAdmin => (MoveAdmin)Generate.MoveAdmin.Get();
    public static MoveElectrical MoveElectrical => (MoveElectrical)Generate.MoveElectrical.Get();

    //Role Spawn
    public static int MayorOn => Generate.MayorOn.GetChance();
    public static int JesterOn => Generate.JesterOn.GetChance();
    public static int SheriffOn => Generate.SheriffOn.GetChance();
    public static int ShapeshifterOn => Generate.ShapeshifterOn.GetChance();
    public static int JanitorOn => Generate.JanitorOn.GetChance();
    public static int EngineerOn => Generate.EngineerOn.GetChance();
    public static int SwapperOn => Generate.SwapperOn.GetChance();
    public static int ShifterOn => Generate.ShifterOn.GetChance();
    public static int AmnesiacOn => Generate.AmnesiacOn.GetChance();
    public static int ConcealerOn => Generate.ConcealerOn.GetChance();
    public static int PoliticianOn => Generate.PoliticianOn.GetChance();
    public static int MedicOn => Generate.MedicOn.GetChance();
    public static int GlitchOn => Generate.GlitchOn.GetChance();
    public static int MorphlingOn => Generate.MorphlingOn.GetChance();
    public static int ExecutionerOn => Generate.ExecutionerOn.GetChance();
    public static int CrewmateOn => Generate.CrewmateOn.GetChance();
    public static int ImpostorOn => Generate.ImpostorOn.GetChance();
    public static int WraithOn => Generate.WraithOn.GetChance();
    public static int ArsonistOn => Generate.ArsonistOn.GetChance();
    public static int AltruistOn => Generate.AltruistOn.GetChance();
    public static int JackalOn => Generate.JackalOn.GetChance();
    public static int VigilanteOn => Generate.VigilanteOn.GetChance();
    public static int GrenadierOn => Generate.GrenadierOn.GetChance();
    public static int VeteranOn => Generate.VeteranOn.GetChance();
    public static int TrackerOn => Generate.TrackerOn.GetChance();
    public static int OperativeOn => Generate.OperativeOn.GetChance();
    public static int PoisonerOn => Generate.PoisonerOn.GetChance();
    public static int InspectorOn => Generate.InspectorOn.GetChance();
    public static int EscortOn => Generate.EscortOn.GetChance();
    public static int GodfatherOn => Generate.GodfatherOn.GetChance();
    public static int RebelOn => Generate.RebelOn.GetChance();
    public static int ConsortOn => Generate.ConsortOn.GetChance();
    public static int TrollOn => Generate.TrollOn.GetChance();
    public static int TransporterOn => Generate.TransporterOn.GetChance();
    public static int MediumOn => Generate.MediumOn.GetChance();
    public static int SurvivorOn => Generate.SurvivorOn.GetChance();
    public static int GuardianAngelOn => Generate.GuardianAngelOn.GetChance();
    public static int CoronerOn => Generate.CoronerOn.GetChance();
    public static int BlackmailerOn => Generate.BlackmailerOn.GetChance();
    public static int PlaguebearerOn => Generate.PlaguebearerOn.GetChance();
    public static int JuggernautOn => Generate.JuggernautOn.GetChance();
    public static int WerewolfOn => Generate.WerewolfOn.GetChance();
    public static int TeleporterOn => Generate.TeleporterOn.GetChance();
    public static int SerialKillerOn => Generate.SerialKillerOn.GetChance();
    public static int DetectiveOn => Generate.DetectiveOn.GetChance();
    public static int CamouflagerOn => Generate.CamouflagerOn.GetChance();
    public static int ThiefOn => Generate.ThiefOn.GetChance();
    public static int CryomaniacOn => Generate.CryomaniacOn.GetChance();
    public static int DisguiserOn => Generate.DisguiserOn.GetChance();
    public static int CannibalOn => Generate.CannibalOn.GetChance();
    public static int VampireHunterOn => Generate.VampireHunterOn.GetChance();
    public static int BomberOn => Generate.BomberOn.GetChance();
    public static int FramerOn => Generate.FramerOn.GetChance();
    public static int MurdererOn => Generate.MurdererOn.GetChance();
    public static int WarperOn => Generate.WarperOn.GetChance();
    public static int AnarchistOn => Generate.AnarchistOn.GetChance();
    public static int DraculaOn => Generate.DraculaOn.GetChance();
    public static int ConsigliereOn => Generate.ConsigliereOn.GetChance();
    public static int MinerOn => Generate.MinerOn.GetChance();
    public static int PhantomOn => Generate.PhantomOn.GetChance();
    public static int RevealerOn => Generate.RevealerOn.GetChance();
    public static int RetributionistOn => Generate.RetributionistOn.GetChance();
    public static int NecromancerOn => Generate.NecromancerOn.GetChance();
    public static int WhispererOn => Generate.WhispererOn.GetChance();
    public static int SeerOn => Generate.SeerOn.GetChance();
    public static int MysticOn => Generate.MysticOn.GetChance();
    public static int ChameleonOn => Generate.ChameleonOn.GetChance();
    public static int GuesserOn => Generate.GuesserOn.GetChance();
    public static int BountyHunterOn => Generate.BountyHunterOn.GetChance();
    public static int ActorOn => Generate.ActorOn.GetChance();
    public static int AmbusherOn => Generate.AmbusherOn.GetChance();
    public static int CrusaderOn => Generate.CrusaderOn.GetChance();
    public static int BansheeOn => Generate.BansheeOn.GetChance();
    public static int GhoulOn => Generate.GhoulOn.GetChance();
    public static int EnforcerOn => Generate.EnforcerOn.GetChance();
    public static int DictatorOn => Generate.DictatorOn.GetChance();
    public static int MonarchOn => Generate.MonarchOn.GetChance();
    public static int SpellslingerOn => Generate.SpellslingerOn.GetChance();
    public static int StalkerOn => Generate.StalkerOn.GetChance();
    public static int ColliderOn => Generate.ColliderOn.GetChance();
    public static int DrunkardOn => Generate.DrunkardOn.GetChance();
    public static int TimeKeeperOn => Generate.TimeKeeperOn.GetChance();
    public static int SilencerOn => Generate.SilencerOn.GetChance();

    //Ability Spawn
    public static int CrewAssassinOn => Generate.CrewAssassinOn.GetChance();
    public static int IntruderAssassinOn => Generate.IntruderAssassinOn.GetChance();
    public static int SyndicateAssassinOn => Generate.SyndicateAssassinOn.GetChance();
    public static int NeutralAssassinOn => Generate.NeutralAssassinOn.GetChance();
    public static int UnderdogOn => Generate.UnderdogOn.GetChance();
    public static int SnitchOn => Generate.SnitchOn.GetChance();
    public static int MultitaskerOn => Generate.MultitaskerOn.GetChance();
    public static int TorchOn => Generate.TorchOn.GetChance();
    public static int ButtonBarryOn => Generate.ButtonBarryOn.GetChance();
    public static int TunnelerOn => Generate.TunnelerOn.GetChance();
    public static int NinjaOn => Generate.NinjaOn.GetChance();
    public static int RadarOn => Generate.RadarOn.GetChance();
    public static int TiebreakerOn => Generate.TiebreakerOn.GetChance();
    public static int InsiderOn => Generate.InsiderOn.GetChance();
    public static int RuthlessOn => Generate.RuthlessOn.GetChance();

    //Objectifier Spawn
    public static int RivalsOn => Generate.RivalsOn.GetChance();
    public static int FanaticOn => Generate.FanaticOn.GetChance();
    public static int TraitorOn => Generate.TraitorOn.GetChance();
    public static int TaskmasterOn => Generate.TaskmasterOn.GetChance();
    public static int CorruptedOn => Generate.CorruptedOn.GetChance();
    public static int OverlordOn => Generate.OverlordOn.GetChance();
    public static int LoversOn => Generate.LoversOn.GetChance();
    public static int AlliedOn => Generate.AlliedOn.GetChance();
    public static int MafiaOn => Generate.MafiaOn.GetChance();
    public static int DefectorOn => Generate.DefectorOn.GetChance();
    public static int LinkedOn => Generate.LinkedOn.GetChance();

    //Modifier Spawn
    public static int ProfessionalOn => Generate.ProfessionalOn.GetChance();
    public static int DiseasedOn => Generate.DiseasedOn.GetChance();
    public static int GiantOn => Generate.GiantOn.GetChance();
    public static int DwarfOn => Generate.DwarfOn.GetChance();
    public static int BaitOn => Generate.BaitOn.GetChance();
    public static int CowardOn => Generate.CowardOn.GetChance();
    public static int DrunkOn => Generate.DrunkOn.GetChance();
    public static int VolatileOn => Generate.VolatileOn.GetChance();
    public static int VIPOn => Generate.VIPOn.GetChance();
    public static int ShyOn => Generate.ShyOn.GetChance();
    public static int IndomitableOn => Generate.IndomitableOn.GetChance();
    public static int AstralOn => Generate.AstralOn.GetChance();
    public static int YellerOn => Generate.YellerOn.GetChance();

    //Crew Options
    public static float CrewVision => Generate.CrewVision.Get();
    public static int ShortTasks => (int)Generate.ShortTasks.Get();
    public static int LongTasks => (int)Generate.LongTasks.Get();
    public static int CommonTasks => (int)Generate.CommonTasks.Get();
    public static bool GhostTasksCountToWin => Generate.GhostTasksCountToWin.Get();
    public static bool CrewFlashlight => Generate.CrewFlashlight.Get();
    public static bool CrewVent => Generate.CrewVent.Get();
    public static int CrewMax => (int)Generate.CrewMax.Get();
    public static int CrewMin => (int)Generate.CrewMin.Get();

    //Intruder Options
    public static float IntruderVision => Generate.IntruderVision.Get();
    public static float IntKillCooldown => Generate.IntruderKillCooldown.Get();
    public static int IntruderCount => (int)Generate.IntruderCount.Get();
    public static bool IntrudersCanSabotage => Generate.IntrudersCanSabotage.Get();
    public static bool IntrudersVent => Generate.IntrudersVent.Get();
    public static float IntruderSabotageCooldown => Generate.IntruderSabotageCooldown.Get();
    public static int IntruderMax => (int)Generate.IntruderMax.Get();
    public static int IntruderMin => (int)Generate.IntruderMin.Get();
    public static bool IntruderFlashlight => Generate.IntruderFlashlight.Get();
    public static bool GhostsCanSabotage => Generate.GhostsCanSabotage.Get();

    //Syndicate Options
    public static float SyndicateVision => Generate.SyndicateVision.Get();
    public static bool AltImps => Generate.AltImps.Get() || IntruderCount == 0;
    public static SyndicateVentOptions SyndicateVent => (SyndicateVentOptions)Generate.SyndicateVent.Get();
    public static int SyndicateCount => (int)Generate.SyndicateCount.Get();
    public static bool GlobalDrive => Generate.GlobalDrive.Get();
    public static float ChaosDriveKillCooldown => Generate.ChaosDriveKillCooldown.Get();
    public static int ChaosDriveMeetingCount => (int)Generate.ChaosDriveMeetingCount.Get();
    public static int SyndicateMax => (int)Generate.SyndicateMax.Get();
    public static int SyndicateMin => (int)Generate.SyndicateMin.Get();
    public static bool SyndicateFlashlight => Generate.SyndicateFlashlight.Get();

    //Neutral Options
    public static float NeutralVision => Generate.NeutralVision.Get();
    public static bool LightsAffectNeutrals => Generate.LightsAffectNeutrals.Get();
    public static NoSolo NoSolo => (NoSolo)Generate.NoSolo.Get();
    public static bool NeutralsVent => Generate.NeutralsVent.Get();
    public static bool AvoidNeutralBenignKingmakers => Generate.AvoidNeutralBenignKingmakers.Get();
    public static bool AvoidNeutralEvilKingmakers => Generate.AvoidNeutralEvilKingmakers.Get();
    public static int NeutralMax => (int)Generate.NeutralMax.Get();
    public static int NeutralMin => (int)Generate.NeutralMin.Get();
    public static bool NeutralFlashlight => Generate.NeutralFlashlight.Get();

    //Vampire Hunter Settings
    public static int VampireHunterCount => Generate.VampireHunterOn.GetCount();
    public static bool UniqueVampireHunter => Generate.UniqueVampireHunter.Get();
    public static float StakeCooldown => Generate.StakeCooldown.Get();

    //Mystic Settings
    public static int MysticCount => Generate.MysticOn.GetCount();
    public static bool UniqueMystic => Generate.UniqueMystic.Get();
    public static float RevealCooldown => Generate.RevealCooldown.Get();

    //Seer Settings
    public static int SeerCount => Generate.SeerOn.GetCount();
    public static bool UniqueSeer => Generate.UniqueSeer.Get();
    public static float SeerCooldown => Generate.SeerCooldown.Get();

    //Detective Settings
    public static int DetectiveCount => Generate.DetectiveOn.GetCount();
    public static float ExamineCd => Generate.ExamineCooldown.Get();
    public static bool UniqueDetective => Generate.UniqueDetective.Get();
    public static float RecentKill => Generate.RecentKill.Get();
    public static float FootprintInterval => Generate.FootprintInterval.Get();
    public static float FootprintDuration => Generate.FootprintDuration.Get();
    public static bool AnonymousFootPrint => Generate.AnonymousFootPrint.Get();

    //Inspector Settings
    public static int InspectorCount => Generate.InspectorOn.GetCount();
    public static float InspectCooldown => Generate.InspectCooldown.Get();
    public static bool UniqueInspector => Generate.UniqueInspector.Get();

    //Medium Settings
    public static int MediumCount => Generate.MediumOn.GetCount();
    public static DeadRevealed DeadRevealed => (DeadRevealed)Generate.DeadRevealed.Get();
    public static float MediateCooldown => Generate.MediateCooldown.Get();
    public static bool ShowMediatePlayer => Generate.ShowMediatePlayer.Get();
    public static ShowMediumToDead ShowMediumToDead => (ShowMediumToDead)Generate.ShowMediumToDead.Get();
    public static bool UniqueMedium => Generate.UniqueMedium.Get();

    //Coroner Settings
    public static bool CoronerReportName => Generate.CoronerReportName.Get();
    public static bool CoronerReportRole => Generate.CoronerReportRole.Get();
    public static float CoronerArrowDuration => Generate.CoronerArrowDuration.Get();
    public static bool UniqueCoroner => Generate.UniqueCoroner.Get();
    public static int CoronerCount => Generate.CoronerOn.GetCount();
    public static float CoronerKillerNameTime => Generate.CoronerKillerNameTime.Get();
    public static float CompareCooldown => Generate.CompareCooldown.Get();
    public static float AutopsyCooldown => Generate.AutopsyCooldown.Get();

    //Revealer Settings
    public static bool RevealerKnows => Generate.RevealerKnows.Get();
    public static int RevealerTasksRemainingClicked => (int)Generate.RevealerTasksRemainingClicked.Get();
    public static int RevealerTasksRemainingAlert => (int)Generate.RevealerTasksRemainingAlert.Get();
    public static int RevealerCount => Generate.RevealerOn.GetCount();
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
    public static int SheriffCount => Generate.SheriffOn.GetCount();

    //Tracker Settings
    public static bool ResetOnNewRound => Generate.ResetOnNewRound.Get();
    public static float UpdateInterval => Generate.UpdateInterval.Get();
    public static int TrackerCount => Generate.TrackerOn.GetCount();
    public static bool UniqueTracker => Generate.UniqueTracker.Get();
    public static float TrackCd => Generate.TrackCooldown.Get();
    public static int MaxTracks => (int)Generate.MaxTracks.Get();

    //Operative Settings
    public static float BugCooldown => Generate.BugCooldown.Get();
    public static int MaxBugs => (int)Generate.MaxBugs.Get();
    public static float MinAmountOfTimeInBug => Generate.MinAmountOfTimeInBug.Get();
    public static int OperativeCount => Generate.OperativeOn.GetCount();
    public static float BugRange => Generate.BugRange.Get();
    public static int MinAmountOfPlayersInBug => (int)Generate.MinAmountOfPlayersInBug.Get();
    public static bool BugsRemoveOnNewRound => Generate.BugsRemoveOnNewRound.Get();
    public static bool UniqueOperative => Generate.UniqueOperative.Get();
    public static AdminDeadPlayers WhoSeesDead => (AdminDeadPlayers)Generate.WhoSeesDead.Get();
    public static bool PreciseOperativeInfo => Generate.PreciseOperativeInfo.Get();

    //Veteran Settings
    public static float AlertCd => Generate.AlertCooldown.Get();
    public static float AlertDuration => Generate.AlertDuration.Get();
    public static int MaxAlerts => (int)Generate.MaxAlerts.Get();
    public static int VeteranCount => Generate.VeteranOn.GetCount();
    public static bool UniqueVeteran => Generate.UniqueVeteran.Get();

    //Vigilante Settings
    public static VigiOptions VigiOptions => (VigiOptions)Generate.VigiOptions.Get();
    public static int VigilanteCount => Generate.VigilanteOn.GetCount();
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
    public static int AltruistCount => Generate.AltruistOn.GetCount();
    public static int ReviveCount => (int)Generate.ReviveCount.Get();
    public static float ReviveCooldown => Generate.ReviveCooldown.Get();

    //Medic Settings
    public static ShieldOptions ShowShielded => (ShieldOptions)Generate.ShowShielded.Get();
    public static int MedicCount => Generate.MedicOn.GetCount();
    public static bool UniqueMedic => Generate.UniqueMedic.Get();
    public static NotificationOptions NotificationShield => (NotificationOptions)Generate.WhoGetsNotification.Get();
    public static bool ShieldBreaks => Generate.ShieldBreaks.Get();

    //Dictator Settings
    public static bool UniqueDictator => Generate.UniqueDictator.Get();
    public static int DictatorCount => Generate.DictatorOn.GetCount();
    public static bool DictatorButton => Generate.DictatorButton.Get();
    public static bool RoundOneNoDictReveal => Generate.RoundOneNoDictReveal.Get();
    public static bool DictateAfterVoting => Generate.DictateAfterVoting.Get();

    //Mayor Settings
    public static bool UniqueMayor => Generate.UniqueMayor.Get();
    public static int MayorCount => Generate.MayorOn.GetCount();
    public static int MayorVoteCount => (int)Generate.MayorVoteCount.Get();
    public static bool MayorButton => Generate.MayorButton.Get();
    public static bool RoundOneNoReveal => Generate.RoundOneNoReveal.Get();

    //Monarch Settings
    public static bool UniqueMonarch => Generate.UniqueMonarch.Get();
    public static int MonarchCount => Generate.MonarchOn.GetCount();
    public static int KnightVoteCount => (int)Generate.KnightVoteCount.Get();
    public static int KnightCount => (int)Generate.KnightCount.Get();
    public static bool KnightButton => Generate.KnightButton.Get();
    public static bool MonarchButton => Generate.MonarchButton.Get();
    public static bool RoundOneNoKnighting => Generate.RoundOneNoKnighting.Get();
    public static float KnightingCooldown => Generate.KnightingCooldown.Get();

    //Engineer Settings
    public static int EngineerCount => Generate.EngineerOn.GetCount();
    public static int MaxFixes => (int)Generate.MaxFixes.Get();
    public static bool UniqueEngineer => Generate.UniqueEngineer.Get();
    public static float FixCooldown => Generate.FixCooldown.Get();

    //Escort Settings
    public static int EscortCount => Generate.EscortOn.GetCount();
    public static float EscRoleblockCooldown => Generate.EscRoleblockCooldown.Get();
    public static float EscRoleblockDuration => Generate.EscRoleblockDuration.Get();
    public static bool UniqueEscort => Generate.UniqueEscort.Get();

    //Chameleon Settings
    public static int ChameleonCount => Generate.ChameleonOn.GetCount();
    public static float SwoopCooldown => Generate.SwoopCooldown.Get();
    public static float SwoopDuration => Generate.SwoopDuration.Get();
    public static bool UniqueChameleon => Generate.UniqueChameleon.Get();
    public static int SwoopCount => (int)Generate.SwoopCount.Get();

    //Retributionist Settings
    public static int RetributionistCount => Generate.RetributionistOn.GetCount();
    public static bool UniqueRetributionist => Generate.UniqueRetributionist.Get();
    public static bool ReviveAfterVoting => Generate.ReviveAfterVoting.Get();
    public static int MaxUses => (int)Generate.MaxUses.Get();

    //Shifter Settings
    public static BecomeEnum ShiftedBecomes => (BecomeEnum)Generate.ShiftedBecomes.Get();
    public static int ShifterCount => Generate.ShifterOn.GetCount();
    public static bool UniqueShifter => Generate.UniqueShifter.Get();
    public static float ShifterCd => Generate.ShifterCd.Get();

    //Transporter Settings
    public static float TransportCooldown => Generate.TransportCooldown.Get();
    public static int TransportMaxUses => (int)Generate.TransportMaxUses.Get();
    public static int TransporterCount => Generate.TransporterOn.GetCount();
    public static bool UniqueTransporter => Generate.UniqueTransporter.Get();
    public static bool TransSelf => Generate.TransSelf.Get();
    public static float TransportDuration => Generate.TransportDuration.Get();

    //Crewmate Settings
    public static int CrewCount => Generate.CrewmateOn.GetCount();

    //Amnesiac Settings
    public static bool RememberArrows => Generate.RememberArrows.Get();
    public static int AmnesiacCount => Generate.AmnesiacOn.GetCount();
    public static bool AmneVent => Generate.AmneVent.Get();
    public static bool AmneVentSwitch => Generate.AmneSwitchVent.Get();
    public static float RememberArrowDelay => Generate.RememberArrowDelay.Get();
    public static bool UniqueAmnesiac => Generate.UniqueAmnesiac.Get();

    //Survivor Settings
    public static float VestCd => Generate.VestCd.Get();
    public static float VestDuration => Generate.VestDuration.Get();
    public static float VestKCReset => Generate.VestKCReset.Get();
    public static int MaxVests => (int)Generate.MaxVests.Get();
    public static int SurvivorCount => Generate.SurvivorOn.GetCount();
    public static bool SurvVent => Generate.SurvVent.Get();
    public static bool SurvVentSwitch => Generate.SurvSwitchVent.Get();
    public static bool UniqueSurvivor => Generate.UniqueSurvivor.Get();

    //GA Settings
    public static float ProtectCd => Generate.ProtectCd.Get();
    public static float ProtectDuration => Generate.ProtectDuration.Get();
    public static float ProtectKCReset => Generate.ProtectKCReset.Get();
    public static int MaxProtects => (int)Generate.MaxProtects.Get();
    public static ProtectOptions ShowProtect => (ProtectOptions)Generate.ShowProtect.Get();
    public static int GuardianAngelCount => Generate.GuardianAngelOn.GetCount();
    public static bool GAVent => Generate.GAVent.Get();
    public static bool GAVentSwitch => Generate.GASwitchVent.Get();
    public static bool ProtectBeyondTheGrave => Generate.ProtectBeyondTheGrave.Get();
    public static bool GATargetKnows => Generate.GATargetKnows.Get();
    public static bool GAKnowsTargetRole => Generate.GAKnowsTargetRole.Get();
    public static bool UniqueGuardianAngel => Generate.UniqueGuardianAngel.Get();
    public static bool GuardianAngelCanPickTargets => Generate.GuardianAngelCanPickTargets.Get();

    //Thief Settings
    public static int ThiefCount => Generate.ThiefOn.GetCount();
    public static bool ThiefVent => Generate.ThiefVent.Get();
    public static float ThiefKillCooldown => Generate.ThiefKillCooldown.Get();
    public static bool UniqueThief => Generate.UniqueThief.Get();
    public static bool ThiefSteals => Generate.ThiefSteals.Get();
    public static bool ThiefCanGuess => Generate.ThiefCanGuess.Get();
    public static bool ThiefCanGuessAfterVoting => Generate.ThiefCanGuessAfterVoting.Get();

    //Jester Settings
    public static bool VigiKillsJester => Generate.VigiKillsJester.Get();
    public static bool JestEjectScreen => Generate.JestEjectScreen.Get();
    public static bool JestVentSwitch => Generate.JestSwitchVent.Get();
    public static bool JesterButton => Generate.JesterButton.Get();
    public static bool JesterVent => Generate.JesterVent.Get();
    public static int JesterCount => Generate.JesterOn.GetCount();
    public static int HauntCount => (int)Generate.HauntCount.Get();
    public static bool UniqueJester => Generate.UniqueJester.Get();
    public static float HauntCooldown => Generate.HauntCooldown.Get();

    //Actor Settings
    public static bool VigiKillsActor => Generate.VigiKillsActor.Get();
    public static bool ActVentSwitch => Generate.ActSwitchVent.Get();
    public static bool ActorButton => Generate.ActorButton.Get();
    public static bool ActorVent => Generate.ActorVent.Get();
    public static int ActorCount => Generate.ActorOn.GetCount();
    public static bool UniqueActor => Generate.UniqueActor.Get();
    public static bool ActorCanPickRole => Generate.ActorCanPickRole.Get();

    //Troll Settings
    public static bool TrollVent => Generate.TrollVent.Get();
    public static float InteractCooldown => Generate.InteractCooldown.Get();
    public static int TrollCount => Generate.TrollOn.GetCount();
    public static bool TrollVentSwitch => Generate.TrollSwitchVent.Get();
    public static bool UniqueTroll => Generate.UniqueTroll.Get();

    //Bounty Hunter Settings
    public static bool BHVent => Generate.BHVent.Get();
    public static float BountyHunterCooldown => Generate.BountyHunterCooldown.Get();
    public static int BHCount => Generate.BountyHunterOn.GetCount();
    public static int BountyHunterGuesses => (int)Generate.BountyHunterGuesses.Get();
    public static bool UniqueBountyHunter => Generate.UniqueBountyHunter.Get();
    public static bool VigiKillsBH => Generate.VigiKillsBH.Get();
    public static bool BountyHunterCanPickTargets => Generate.BountyHunterCanPickTargets.Get();

    //Cannibal Settings
    public static float EatArrowDelay => Generate.EatArrowDelay.Get();
    public static bool EatArrows => Generate.EatArrows.Get();
    public static bool CannibalVent => Generate.CannibalVent.Get();
    public static float CannibalCd => Generate.CannibalCd.Get();
    public static int CannibalCount => Generate.CannibalOn.GetCount();
    public static int CannibalBodyCount => (int)Generate.CannibalBodyCount.Get();
    public static bool VigiKillsCannibal => Generate.VigiKillsCannibal.Get();
    public static bool UniqueCannibal => Generate.UniqueCannibal.Get();

    //Executioner Settings
    public static int ExecutionerCount => Generate.ExecutionerOn.GetCount();
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
    public static bool ExecutionerCanPickTargets => Generate.ExecutionerCanPickTargets.Get();

    //Guesser Settings
    public static int GuesserCount => Generate.GuesserOn.GetCount();
    public static bool VigiKillsGuesser => Generate.VigiKillsGuesser.Get();
    public static bool GuessVent => Generate.GuessVent.Get();
    public static bool GuesserButton => Generate.GuesserButton.Get();
    public static bool GuesserTargetKnows => Generate.GuessTargetKnows.Get();
    public static bool GuessVentSwitch => Generate.GuessSwitchVent.Get();
    public static bool UniqueGuesser => Generate.UniqueGuesser.Get();
    public static bool GuesserAfterVoting => Generate.GuesserAfterVoting.Get();
    public static bool MultipleGuesses => Generate.MultipleGuesses.Get();
    public static int GuessCount => (int)Generate.GuessCount.Get();
    public static bool GuesserCanPickTargets => Generate.GuesserCanPickTargets.Get();

    //Glitch Settings
    public static bool GlitchVent => Generate.GlitchVent.Get();
    public static float MimicCooldown => Generate.MimicCooldown.Get();
    public static float HackCooldown => Generate.HackCooldown.Get();
    public static int GlitchCount => Generate.GlitchOn.GetCount();
    public static float MimicDuration => Generate.MimicDuration.Get();
    public static float HackDuration => Generate.HackDuration.Get();
    public static float GlitchKillCooldown => Generate.GlitchKillCooldown.Get();
    public static bool UniqueGlitch => Generate.UniqueGlitch.Get();

    //Juggernaut Settings
    public static float JuggKillBonus => Generate.JuggKillBonus.Get();
    public static bool JuggVent => Generate.JuggVent.Get();
    public static int JuggernautCount => Generate.JuggernautOn.GetCount();
    public static float JuggKillCooldown => Generate.JuggKillCooldown.Get();
    public static bool UniqueJuggernaut => Generate.UniqueJuggernaut.Get();

    //Cryomaniac Settings
    public static bool CryoVent => Generate.CryoVent.Get();
    public static int CryomaniacCount => Generate.CryomaniacOn.GetCount();
    public static float CryoDouseCooldown => Generate.CryoDouseCooldown.Get();
    public static bool UniqueCryomaniac => Generate.UniqueCryomaniac.Get();
    public static bool CryoFreezeAll => Generate.CryoFreezeAll.Get();
    public static bool CryoLastKillerBoost => Generate.CryoLastKillerBoost.Get();

    //Plaguebearer Settings
    public static bool PBVent => Generate.PBVent.Get();
    public static float InfectCd => Generate.InfectCooldown.Get();
    public static int PlaguebearerCount => Generate.PlaguebearerOn.GetCount();
    public static bool UniquePlaguebearer => Generate.UniquePlaguebearer.Get();

    //Arsonist Settings
    public static bool ArsoLastKillerBoost => Generate.ArsoLastKillerBoost.Get();
    public static bool ArsoVent => Generate.ArsoVent.Get();
    public static float DouseCd => Generate.DouseCooldown.Get();
    public static float IgniteCd => Generate.IgniteCooldown.Get();
    public static int ArsonistCount => Generate.ArsonistOn.GetCount();
    public static bool UniqueArsonist => Generate.UniqueArsonist.Get();
    public static bool ArsoCooldownsLinked => Generate.ArsoCooldownsLinked.Get();
    public static bool ArsoIgniteAll => Generate.ArsoIgniteAll.Get();
    public static bool IgnitionCremates => Generate.IgnitionCremates.Get();

    //Murderer Settings
    public static float MurdKCD => Generate.MurdKillCooldownOption.Get();
    public static bool MurdVent => Generate.MurdVent.Get();
    public static int MurdCount => Generate.MurdererOn.GetCount();
    public static bool UniqueMurderer => Generate.UniqueMurderer.Get();

    //SK Settings
    public static float BloodlustCd => Generate.BloodlustCooldown.Get();
    public static float BloodlustDuration => Generate.BloodlustDuration.Get();
    public static int SKCount => Generate.SerialKillerOn.GetCount();
    public static bool UniqueSerialKiller => Generate.UniqueSerialKiller.Get();
    public static float LustKillCd => Generate.LustKillCooldown.Get();
    public static SKVentOptions SKVentOptions => (SKVentOptions)Generate.SKVentOptions.Get();

    //WW Settings
    public static bool WerewolfVent => Generate.WerewolfVent.Get();
    public static float MaulRadius => Generate.MaulRadius.Get();
    public static float MaulCooldown => Generate.MaulCooldown.Get();
    public static int WerewolfCount => Generate.WerewolfOn.GetCount();
    public static bool UniqueWerewolf => Generate.UniqueWerewolf.Get();

    //Dracula Settings
    public static bool DracVent => Generate.DracVent.Get();
    public static bool UndeadVent => Generate.UndeadVent.Get();
    public static float BiteCd => Generate.BiteCooldown.Get();
    public static int DraculaCount => Generate.DraculaOn.GetCount();
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
    public static int NecromancerCount => Generate.NecromancerOn.GetCount();
    public static bool UniqueNecromancer => Generate.UniqueNecromancer.Get();
    public static bool NecromancerTargetBody => Generate.NecromancerTargetBody.Get();
    public static float NecroResurrectDuration => Generate.NecroResurrectDuration.Get();
    public static bool ResurrectVent => Generate.ResurrectVent.Get();
    public static bool NecroKillCooldownIncreases => Generate.NecroKillCooldownIncreases.Get();
    public static bool ResurrectCooldownIncreases => Generate.ResurrectCooldownIncreases.Get();

    //Whisperer Settings
    public static bool WhispVent => Generate.WhispVent.Get();
    public static float WhisperCooldown => Generate.WhisperCooldown.Get();
    public static float WhisperRadius => Generate.WhisperRadius.Get();
    public static float WhisperCooldownIncrease => Generate.WhisperCooldownIncrease.Get();
    public static bool WhisperCooldownIncreases => Generate.WhisperCooldownIncreases.Get();
    public static int InitialWhisperRate => (int)Generate.InitialWhisperRate.Get();
    public static int WhisperRateDecrease => (int)Generate.WhisperRateDecrease.Get();
    public static bool WhisperRateDecreases => Generate.WhisperRateDecreases.Get();
    public static int WhispererCount => Generate.WhispererOn.GetCount();
    public static bool UniqueWhisperer => Generate.UniqueWhisperer.Get();
    public static bool PersuadedVent => Generate.PersuadedVent.Get();

    //Jackal Settings
    public static bool JackalVent => Generate.JackalVent.Get();
    public static bool RecruitVent => Generate.RecruitVent.Get();
    public static int JackalCount => Generate.JackalOn.GetCount();
    public static bool UniqueJackal => Generate.UniqueJackal.Get();
    public static float RecruitCooldown => Generate.RecruitCooldown.Get();

    //Phantom Settings
    public static int PhantomTasksRemaining => (int)Generate.PhantomTasksRemaining.Get();
    public static bool PhantomPlayersAlerted => Generate.PhantomPlayersAlerted.Get();
    public static int PhantomCount => Generate.PhantomOn.GetCount();

    //Pestilence Settings
    public static float PestKillCd => Generate.PestKillCooldown.Get();
    public static bool PlayersAlerted => Generate.PlayersAlerted.Get();
    public static bool PestSpawn => Generate.PestSpawn.Get();
    public static bool PestVent => Generate.PestVent.Get();

    //Ghoul Settings
    public static float GhoulMarkCd => Generate.GhoulMarkCd.Get();
    public static int GhoulCount => Generate.GhoulOn.GetCount();

    //Janitor Settings
    public static float JanitorCleanCd => Generate.JanitorCleanCd.Get();
    public static int JanitorCount => Generate.JanitorOn.GetCount();
    public static bool SoloBoost => Generate.SoloBoost.Get();
    public static bool UniqueJanitor => Generate.UniqueJanitor.Get();
    public static bool JaniCooldownsLinked => Generate.JaniCooldownsLinked.Get();
    public static JanitorOptions JanitorVentOptions => (JanitorOptions)Generate.JanitorVentOptions.Get();
    public static int DragModifier => (int)Generate.DragModifier.Get();
    public static float DragCd => Generate.DragCooldown.Get();

    //Blackmailer Settings
    public static float BlackmailCd => Generate.BlackmailCooldown.Get();
    public static int BlackmailerCount => Generate.BlackmailerOn.GetCount();
    public static bool UniqueBlackmailer => Generate.UniqueBlackmailer.Get();
    public static bool WhispersNotPrivate => Generate.WhispersNotPrivate.Get();
    public static bool BlackmailMates => Generate.BlackmailMates.Get();
    public static bool BMRevealed => Generate.BMRevealed.Get();

    //Grenadier Settings
    public static bool GrenadierIndicators => Generate.GrenadierIndicators.Get();
    public static float GrenadeCd => Generate.GrenadeCooldown.Get();
    public static int GrenadierCount => Generate.GrenadierOn.GetCount();
    public static float GrenadeDuration => Generate.GrenadeDuration.Get();
    public static float FlashRadius => Generate.FlashRadius.Get();
    public static bool GrenadierVent => Generate.GrenadierVent.Get();
    public static bool UniqueGrenadier => Generate.UniqueGrenadier.Get();

    //Camouflager Settings
    public static bool CamoHideSize => Generate.CamoHideSize.Get();
    public static bool CamoHideSpeed => Generate.CamoHideSpeed.Get();
    public static float CamouflagerCd => Generate.CamouflagerCooldown.Get();
    public static float CamouflagerDuration => Generate.CamouflagerDuration.Get();
    public static int CamouflagerCount => Generate.CamouflagerOn.GetCount();
    public static bool UniqueCamouflager => Generate.UniqueCamouflager.Get();

    //Morphling Settings
    public static bool MorphlingVent => Generate.MorphlingVent.Get();
    public static int MorphlingCount => Generate.MorphlingOn.GetCount();
    public static float MorphlingCd => Generate.MorphlingCooldown.Get();
    public static float SampleCooldown => Generate.SampleCooldown.Get();
    public static float MorphlingDuration => Generate.MorphlingDuration.Get();
    public static bool UniqueMorphling => Generate.UniqueMorphling.Get();
    public static bool MorphCooldownsLinked => Generate.MorphCooldownsLinked.Get();

    //Wraith Settings
    public static bool WraithVent => Generate.WraithVent.Get();
    public static float InvisCd => Generate.InvisCooldown.Get();
    public static float InvisDuration => Generate.InvisDuration.Get();
    public static int WraithCount => Generate.WraithOn.GetCount();
    public static bool UniqueWraith => Generate.UniqueWraith.Get();

    //Ambusher Settings
    public static float AmbushCooldown => Generate.AmbushCooldown.Get();
    public static float AmbushDuration => Generate.AmbushDuration.Get();
    public static int AmbusherCount => Generate.AmbusherOn.GetCount();
    public static bool UniqueAmbusher => Generate.UniqueAmbusher.Get();
    public static bool AmbushMates => Generate.AmbushMates.Get();

    //Enforcer Settings
    public static float EnforceCooldown => Generate.EnforceCooldown.Get();
    public static float EnforceDuration => Generate.EnforceDuration.Get();
    public static int EnforcerCount => Generate.EnforcerOn.GetCount();
    public static float EnforceRadius => Generate.EnforceRadius.Get();
    public static float EnforceDelay => Generate.EnforceDelay.Get();
    public static bool UniqueEnforcer => Generate.UniqueEnforcer.Get();

    //Teleporter Settings
    public static bool TeleVent => Generate.TeleVent.Get();
    public static float TeleportCd => Generate.TeleportCd.Get();
    public static float MarkCooldown => Generate.MarkCooldown.Get();
    public static int TeleporterCount => Generate.TeleporterOn.GetCount();
    public static bool UniqueTeleporter => Generate.UniqueTeleporter.Get();
    public static bool TeleCooldownsLinked => Generate.TeleCooldownsLinked.Get();

    //Consigliere Settings
    public static ConsigInfo ConsigInfo => (ConsigInfo)Generate.ConsigInfo.Get();
    public static float ConsigCd => Generate.InvestigateCooldown.Get();
    public static int ConsigliereCount => Generate.ConsigliereOn.GetCount();
    public static bool UniqueConsigliere => Generate.UniqueConsigliere.Get();

    //Consort Settings
    public static int ConsortCount => Generate.ConsortOn.GetCount();
    public static float ConsRoleblockCooldown => Generate.ConsRoleblockCooldown.Get();
    public static bool UniqueConsort => Generate.UniqueConsort.Get();
    public static float ConsRoleblockDuration => Generate.ConsRoleblockDuration.Get();

    //Disguiser Settings
    public static int DisguiserCount => Generate.DisguiserOn.GetCount();
    public static float DisguiseDuration => Generate.DisguiseDuration.Get();
    public static float DisguiseCooldown => Generate.DisguiseCooldown.Get();
    public static float TimeToDisguise => Generate.TimeToDisguise.Get();
    public static DisguiserTargets DisguiseTarget => (DisguiserTargets)Generate.DisguiseTarget.Get();
    public static bool UniqueDisguiser => Generate.UniqueDisguiser.Get();
    public static float MeasureCooldown => Generate.MeasureCooldown.Get();
    public static bool DisgCooldownsLinked => Generate.DisgCooldownsLinked.Get();

    //Godfather Settings
    public static int GodfatherCount => Generate.GodfatherOn.GetCount();
    public static bool UniqueGodfather => Generate.UniqueGodfather.Get();
    public static float MafiosoAbilityCooldownDecrease => Generate.MafiosoAbilityCooldownDecrease.Get();

    //Miner Settings
    public static float MineCd => Generate.MineCooldown.Get();
    public static int MinerCount => Generate.MinerOn.GetCount();
    public static bool UniqueMiner => Generate.UniqueMiner.Get();

    //Impostor Settings
    public static int ImpCount => Generate.ImpostorOn.GetCount();

    //Anarchist Settings
    public static int AnarchistCount => Generate.AnarchistOn.GetCount();
    public static float AnarchKillCooldown => Generate.AnarchKillCooldown.Get();

    //Framer Settings
    public static int FramerCount => Generate.FramerOn.GetCount();
    public static float FrameCooldown => Generate.FrameCooldown.Get();
    public static float ChaosDriveFrameRadius => Generate.ChaosDriveFrameRadius.Get();
    public static bool UniqueFramer => Generate.UniqueFramer.Get();

    //Spellslinger Settings
    public static int SpellslingerCount => Generate.SpellslingerOn.GetCount();
    public static float SpellCooldown => Generate.SpellCooldown.Get();
    public static float SpellCooldownIncrease => Generate.SpellCooldownIncrease.Get();
    public static bool UniqueSpellslinger => Generate.UniqueSpellslinger.Get();

    //Collider Settings
    public static int ColliderCount => Generate.ColliderOn.GetCount();
    public static float CollideCooldown => Generate.CollideCooldown.Get();
    public static float ChargeCooldown => Generate.ChargeCooldown.Get();
    public static float ChargeDuration => Generate.ChargeDuration.Get();
    public static float CollideRange => Generate.CollideRange.Get();
    public static float CollideRangeIncrease => Generate.CollideRangeIncrease.Get();
    public static bool UniqueCollider => Generate.UniqueCollider.Get();
    public static bool CollideResetsCooldown => Generate.CollideResetsCooldown.Get();
    public static bool ChargeCooldownsLinked => Generate.ChargeCooldownsLinked.Get();

    //Shapeshifter Settings
    public static int ShapeshifterCount => Generate.ShapeshifterOn.GetCount();
    public static float ShapeshiftCooldown => Generate.ShapeshiftCooldown.Get();
    public static float ShapeshiftDuration => Generate.ShapeshiftDuration.Get();
    public static bool UniqueShapeshifter => Generate.UniqueShapeshifter.Get();
    public static bool ShapeshiftMates => Generate.ShapeshiftMates.Get();

    //Drunkard Settings
    public static int DrunkardCount => Generate.DrunkardOn.GetCount();
    public static float ConfuseCooldown => Generate.ConfuseCooldown.Get();
    public static float ConfuseDuration => Generate.ConfuseDuration.Get();
    public static bool UniqueDrunkard => Generate.UniqueDrunkard.Get();
    public static bool ConfuseImmunity => Generate.ConfuseImmunity.Get();

    //Time Keeper Settings
    public static int TimeKeeperCount => Generate.TimeKeeperOn.GetCount();
    public static float TimeControlCooldown => Generate.TimeControlCooldown.Get();
    public static float TimeControlDuration => Generate.TimeControlDuration.Get();
    public static bool UniqueTimeKeeper => Generate.UniqueTimeKeeper.Get();
    public static bool TimeFreezeImmunity => Generate.TimeFreezeImmunity.Get();
    public static bool TimeRewindImmunity => Generate.TimeRewindImmunity.Get();

    //Crusader Settings
    public static float CrusadeCooldown => Generate.CrusadeCooldown.Get();
    public static float CrusadeDuration => Generate.CrusadeDuration.Get();
    public static int CrusaderCount => Generate.CrusaderOn.GetCount();
    public static bool UniqueCrusader => Generate.UniqueCrusader.Get();
    public static float ChaosDriveCrusadeRadius => Generate.ChaosDriveCrusadeRadius.Get();
    public static bool CrusadeMates => Generate.CrusadeMates.Get();

    //Banshee Settings
    public static float ScreamCooldown => Generate.ScreamCooldown.Get();
    public static float ScreamDuration => Generate.ScreamDuration.Get();
    public static int BansheeCount => Generate.BansheeOn.GetCount();

    //Bomber Settings
    public static float BombCooldown => Generate.BombCooldown.Get();
    public static float DetonateCooldown => Generate.DetonateCooldown.Get();
    public static int BomberCount => Generate.BomberOn.GetCount();
    public static float BombRange => Generate.BombRange.Get();
    public static bool UniqueBomber => Generate.UniqueBomber.Get();
    public static bool BombCooldownsLinked => Generate.BombCooldownsLinked.Get();
    public static bool BombsDetonateOnMeetingStart => Generate.BombsDetonateOnMeetingStart.Get();
    public static bool BombsRemoveOnNewRound => Generate.BombsRemoveOnNewRound.Get();
    public static float ChaosDriveBombRange => Generate.ChaosDriveBombRange.Get();
    public static bool BombKillsSyndicate => Generate.BombKillsSyndicate.Get();

    //Concealer Settings
    public static int ConcealerCount => Generate.ConcealerOn.GetCount();
    public static float ConcealCooldown => Generate.ConcealCooldown.Get();
    public static float ConcealDuration => Generate.ConcealDuration.Get();
    public static bool UniqueConcealer => Generate.UniqueConcealer.Get();
    public static bool ConcealMates => Generate.ConcealMates.Get();

    //Silencer Settings
    public static float SilenceCooldown => Generate.SilenceCooldown.Get();
    public static int SilencerCount => Generate.SilencerOn.GetCount();
    public static bool UniqueSilencer => Generate.UniqueSilencer.Get();
    public static bool WhispersNotPrivateSilencer => Generate.WhispersNotPrivateSilencer.Get();
    public static bool SilenceMates => Generate.SilenceMates.Get();
    public static bool SilenceRevealed => Generate.SilenceRevealed.Get();

    //Stalker Settings
    public static int StalkerCount => Generate.StalkerOn.GetCount();
    public static bool UniqueStalker => Generate.UniqueStalker.Get();
    public static float StalkCd => Generate.StalkCooldown.Get();

    //Poisoner Settings
    public static float PoisonCd => Generate.PoisonCooldown.Get();
    public static float PoisonDuration => Generate.PoisonDuration.Get();
    public static int PoisonerCount => Generate.PoisonerOn.GetCount();
    public static bool UniquePoisoner => Generate.UniquePoisoner.Get();

    //Rebel Settings
    public static int RebelCount => Generate.RebelOn.GetCount();
    public static bool UniqueRebel => Generate.UniqueRebel.Get();
    public static float SidekickAbilityCooldownDecrease => Generate.SidekickAbilityCooldownDecrease.Get();

    //Warper Settings
    public static float WarpCooldown => Generate.WarpCooldown.Get();
    public static bool UniqueWarper => Generate.UniqueWarper.Get();
    public static bool WarpSelf => Generate.WarpSelf.Get();
    public static int WarperCount => Generate.WarperOn.GetCount();
    public static float WarpDuration => Generate.WarpDuration.Get();

    //Betrayer Settings
    public static float BetrayerKillCooldown => Generate.BetrayerKillCooldown.Get();
    public static bool BetrayerVent => Generate.BetrayerVent.Get();

    //Modifier Settings
    public static int MaxModifiers => (int)Generate.MaxModifiers.Get();
    public static int MinModifiers => (int)Generate.MinModifiers.Get();

    //Objectifier Settings
    public static int MaxObjectifiers => (int)Generate.MaxObjectifiers.Get();
    public static int MinObjectifiers => (int)Generate.MinObjectifiers.Get();

    //Ability Settings
    public static int MaxAbilities => (int)Generate.MaxAbilities.Get();
    public static int MinAbilities => (int)Generate.MinAbilities.Get();

    //Snitch Settings
    public static bool SnitchSeesNeutrals => Generate.SnitchSeesNeutrals.Get();
    public static int SnitchCount => Generate.SnitchOn.GetCount();
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
    public static int NumberOfIntruderAssassins => Generate.IntruderAssassinOn.GetCount();
    public static int NumberOfCrewAssassins => Generate.CrewAssassinOn.GetCount();
    public static int NumberOfNeutralAssassins => Generate.NeutralAssassinOn.GetCount();
    public static int NumberOfSyndicateAssassins => Generate.SyndicateAssassinOn.GetCount();
    public static bool AssassinGuessNeutralBenign => Generate.AssassinGuessNeutralBenign.Get();
    public static bool AssassinGuessNeutralEvil => Generate.AssassinGuessNeutralEvil.Get();
    public static bool AssassinGuessPest => Generate.AssassinGuessPest.Get();
    public static bool AssassinGuessModifiers => Generate.AssassinGuessModifiers.Get();
    public static bool AssassinGuessObjectifiers => Generate.AssassinGuessObjectifiers.Get();
    public static bool AssassinGuessAbilities => Generate.AssassinGuessAbilities.Get();
    public static bool AssassinMultiKill => Generate.AssassinMultiKill.Get();
    public static bool AssassinateAfterVoting => Generate.AssassinateAfterVoting.Get();
    public static bool AssassinGuessInvestigative => Generate.AssassinGuessInvestigative.Get();
    public static bool UniqueCrewAssassin => Generate.UniqueCrewAssassin.Get();
    public static bool UniqueNeutralAssassin => Generate.UniqueNeutralAssassin.Get();
    public static bool UniqueIntruderAssassin => Generate.UniqueIntruderAssassin.Get();
    public static bool UniqueSyndicateAssassin => Generate.UniqueSyndicateAssassin.Get();

    //Underdog Settings
    public static bool UnderdogIncreasedKC => Generate.UnderdogIncreasedKC.Get();
    public static float UnderdogKillBonus => Generate.UnderdogKillBonus.Get();
    public static bool UniqueUnderdog => Generate.UniqueUnderdog.Get();
    public static bool UnderdogKnows => Generate.UnderdogKnows.Get();
    public static int UnderdogCount => Generate.UnderdogOn.GetCount();

    //Multitasker Settings
    public static int MultitaskerCount => Generate.MultitaskerOn.GetCount();
    public static float Transparancy => Generate.Transparancy.Get();
    public static bool UniqueMultitasker => Generate.UniqueMultitasker.Get();

    //BB Settings
    public static int ButtonBarryCount => Generate.ButtonBarryOn.GetCount();
    public static float ButtonCooldown => Generate.ButtonCooldown.Get();
    public static bool UniqueButtonBarry => Generate.UniqueButtonBarry.Get();

    //Swapper Settings
    public static bool SwapperButton => Generate.SwapperButton.Get();
    public static int SwapperCount => Generate.SwapperOn.GetCount();
    public static bool SwapAfterVoting => Generate.SwapAfterVoting.Get();
    public static bool SwapSelf => Generate.SwapSelf.Get();
    public static bool UniqueSwapper => Generate.UniqueSwapper.Get();

    //Politician Settings
    public static bool PoliticianAnonymous => Generate.PoliticianAnonymous.Get();
    public static int PoliticianVoteBank => (int)Generate.PoliticianVoteBank.Get();
    public static bool UniquePolitician => Generate.UniquePolitician.Get();
    public static int PoliticianCount => Generate.PoliticianOn.GetCount();
    public static bool PoliticianButton => Generate.PoliticianButton.Get();

    //Tiebreaker Settings
    public static bool TiebreakerKnows => Generate.TiebreakerKnows.Get();
    public static int TiebreakerCount => Generate.TiebreakerOn.GetCount();
    public static bool UniqueTiebreaker => Generate.UniqueTiebreaker.Get();

    //Torch Settings
    public static int TorchCount => Generate.TorchOn.GetCount();
    public static bool UniqueTorch => Generate.UniqueTorch.Get();

    //Tunneler Settings
    public static bool TunnelerKnows => Generate.TunnelerKnows.Get();
    public static int TunnelerCount => Generate.TunnelerOn.GetCount();
    public static bool UniqueTunneler => Generate.UniqueTunneler.Get();

    //Radar Settings
    public static bool UniqueRadar => Generate.UniqueRadar.Get();
    public static int RadarCount => Generate.RadarOn.GetCount();

    //Ninja Settings
    public static int NinjaCount => Generate.NinjaOn.GetCount();
    public static bool UniqueNinja => Generate.UniqueNinja.Get();

    //Ruthless Settings
    public static int RuthlessCount => Generate.RuthlessOn.GetCount();
    public static bool UniqueRuthless => Generate.UniqueRuthless.Get();
    public static bool RuthlessKnows => Generate.RuthlessKnows.Get();

    //Insider Settings
    public static bool InsiderKnows => Generate.InsiderKnows.Get();
    public static int InsiderCount => Generate.InsiderOn.GetCount();
    public static bool UniqueInsider => Generate.UniqueInsider.Get();

    //Traitor Settings
    public static int TraitorCount => Generate.TraitorOn.GetCount();
    public static bool TraitorColourSwap => Generate.TraitorColourSwap.Get();
    public static bool TraitorKnows => Generate.TraitorKnows.Get();
    public static bool UniqueTraitor => Generate.UniqueTraitor.Get();

    //Fanatic Settings
    public static bool FanaticKnows => Generate.FanaticKnows.Get();
    public static int FanaticCount => Generate.FanaticOn.GetCount();
    public static bool UniqueFanatic => Generate.UniqueFanatic.Get();
    public static bool FanaticColourSwap => Generate.FanaticColourSwap.Get();

    //Taskmaster Settings
    public static int TMTasksRemaining => (int)Generate.TMTasksRemaining.Get();
    public static int TaskmasterCount => Generate.TaskmasterOn.GetCount();
    public static bool UniqueTaskmaster => Generate.UniqueTaskmaster.Get();

    //Lovers Settings
    public static bool BothLoversDie => Generate.BothLoversDie.Get();
    public static bool UniqueLovers => Generate.UniqueLovers.Get();
    public static bool LoversChat => Generate.LoversChat.Get();
    public static int LoversCount => Generate.LoversOn.GetCount();
    public static bool LoversRoles => Generate.LoversRoles.Get();

    //Linked Settings
    public static bool UniqueLinked => Generate.UniqueLinked.Get();
    public static bool LinkedChat => Generate.LinkedChat.Get();
    public static int LinkedCount => Generate.LinkedOn.GetCount();
    public static bool LinkedRoles => Generate.LinkedRoles.Get();

    //Defector Settings
    public static bool DefectorKnows => Generate.DefectorKnows.Get();
    public static bool UniqueDefector => Generate.UniqueDefector.Get();
    public static int DefectorCount => Generate.DefectorOn.GetCount();
    public static DefectorFaction DefectorFaction => (DefectorFaction)Generate.DefectorFaction.Get();

    //Rivals Settings
    public static bool RivalsChat => Generate.RivalsChat.Get();
    public static int RivalsCount => Generate.RivalsOn.GetCount();
    public static bool RivalsRoles => Generate.RivalsRoles.Get();
    public static bool UniqueRivals => Generate.UniqueRivals.Get();

    //Mafia Settings
    public static int MafiaCount => Generate.MafiaOn.GetCount();
    public static bool MafiaRoles => Generate.MafiaRoles.Get();
    public static bool UniqueMafia => Generate.UniqueMafia.Get();
    public static bool MafVent => Generate.MafVent.Get();

    //Giant Settings
    public static int GiantCount => Generate.GiantOn.GetCount();
    public static float GiantSpeed => Generate.GiantSpeed.Get();
    public static float GiantScale => Generate.GiantScale.Get();
    public static bool UniqueGiant => Generate.UniqueGiant.Get();

    //Indomitable Settings
    public static bool UniqueIndomitable => Generate.UniqueIndomitable.Get();
    public static int IndomitableCount => Generate.IndomitableOn.GetCount();
    public static bool IndomitableKnows => Generate.IndomitableKnows.Get();

    //Overlord Settings
    public static int OverlordCount => Generate.OverlordOn.GetCount();
    public static bool UniqueOverlord => Generate.UniqueOverlord.Get();
    public static int OverlordMeetingWinCount => (int)Generate.OverlordMeetingWinCount.Get();
    public static bool OverlordKnows => Generate.OverlordKnows.Get();

    //Allied Settings
    public static int AlliedCount => Generate.AlliedOn.GetCount();
    public static bool UniqueAllied => Generate.UniqueAllied.Get();
    public static AlliedFaction AlliedFaction => (AlliedFaction)Generate.AlliedFaction.Get();

    //Corrupted Settings
    public static int CorruptedCount => Generate.CorruptedOn.GetCount();
    public static bool UniqueCorrupted => Generate.UniqueCorrupted.Get();
    public static float CorruptedKillCooldown => Generate.CorruptedKillCooldown.Get();
    public static bool AllCorruptedWin => Generate.AllCorruptedWin.Get();
    public static bool CorruptedVent => Generate.CorruptedVent.Get();

    //Dwarf Settings
    public static float DwarfSpeed => Generate.DwarfSpeed.Get();
    public static bool UniqueDwarf => Generate.UniqueDwarf.Get();
    public static float DwarfScale => Generate.DwarfScale.Get();
    public static int DwarfCount => Generate.DwarfOn.GetCount();

    //Drunk Settings
    public static bool DrunkControlsSwap => Generate.DrunkControlsSwap.Get();
    public static int DrunkCount => Generate.DrunkOn.GetCount();
    public static bool UniqueDrunk => Generate.UniqueDrunk.Get();
    public static bool DrunkKnows => Generate.DrunkKnows.Get();
    public static float DrunkInterval => Generate.DrunkInterval.Get();

    //Bait Settings
    public static bool BaitKnows => Generate.BaitKnows.Get();
    public static float BaitMinDelay => Generate.BaitMinDelay.Get();
    public static float BaitMaxDelay => Generate.BaitMaxDelay.Get();
    public static int BaitCount => Generate.BaitOn.GetCount();
    public static bool UniqueBait => Generate.UniqueBait.Get();

    //Diseased Settings
    public static bool DiseasedKnows => Generate.DiseasedKnows.Get();
    public static float DiseasedMultiplier => Generate.DiseasedKillMultiplier.Get();
    public static int DiseasedCount => Generate.DiseasedOn.GetCount();
    public static bool UniqueDiseased => Generate.UniqueDiseased.Get();

    //Shy Settings
    public static int ShyCount => Generate.ShyOn.GetCount();
    public static bool UniqueShy => Generate.UniqueShy.Get();

    //Astral Settings
    public static int AstralCount => Generate.AstralOn.GetCount();
    public static bool UniqueAstral => Generate.UniqueAstral.Get();

    //Yeller Settings
    public static int YellerCount => Generate.YellerOn.GetCount();
    public static bool UniqueYeller => Generate.UniqueYeller.Get();

    //VIP Settings
    public static bool VIPKnows => Generate.VIPKnows.Get();
    public static bool UniqueVIP => Generate.UniqueVIP.Get();
    public static int VIPCount => Generate.VIPOn.GetCount();

    //Volatile Settings
    public static int VolatileCount => Generate.VolatileOn.GetCount();
    public static float VolatileInterval => Generate.VolatileInterval.Get();
    public static bool UniqueVolatile => Generate.UniqueVolatile.Get();
    public static bool VolatileKnows => Generate.VolatileKnows.Get();

    //Professional Settings
    public static bool ProfessionalKnows => Generate.ProfessionalKnows.Get();
    public static bool UniqueProfessional => Generate.UniqueProfessional.Get();
    public static int ProfessionalCount => Generate.ProfessionalOn.GetCount();

    //Coward Settings
    public static int CowardCount => Generate.CowardOn.GetCount();
    public static bool UniqueCoward => Generate.UniqueCoward.Get();

    //NB Settings
    public static int NBMax => (int)Generate.NBMax.Get();
    public static bool VigiKillsNB => Generate.VigiKillsNB.Get();

    //NK Settings
    public static int NKMax => (int)Generate.NKMax.Get();
    public static bool NKHasImpVision => Generate.NKHasImpVision.Get();
    public static bool NKsKnow => Generate.NKsKnow.Get();

    //CSv Settings
    public static int CSvMax => (int)Generate.CSvMax.Get();

    //CA Settings
    public static int CAMax => (int)Generate.CAMax.Get();

    //CK Settings
    public static int CKMax => (int)Generate.CKMax.Get();

    //CS Settings
    public static int CSMax => (int)Generate.CSMax.Get();

    //CI Settings
    public static int CIMax => (int)Generate.CIMax.Get();

    //CP Settings
    public static int CPMax => (int)Generate.CPMax.Get();

    //IC Settings
    public static int ICMax => (int)Generate.ICMax.Get();

    //ID Settings
    public static int IDMax => (int)Generate.IDMax.Get();

    //IS Settings
    public static int ISMax => (int)Generate.ISMax.Get();

    //IK Settings
    public static int IKMax => (int)Generate.IKMax.Get();

    //SD Settings
    public static int SDMax => (int)Generate.SDMax.Get();

    //SyK Settings
    public static int SyKMax => (int)Generate.SyKMax.Get();

    //SSu Settings
    public static int SSuMax => (int)Generate.SSuMax.Get();

    //SP Settings
    public static int SPMax => (int)Generate.SPMax.Get();

    //NE Settings
    public static int NEMax => (int)Generate.NEMax.Get();
    public static bool NeutralEvilsEndGame => Generate.NeutralEvilsEndGame.Get();

    //NN Settings
    public static int NNMax => (int)Generate.NNMax.Get();
    public static bool NNHasImpVision => Generate.NNHasImpVision.Get();

    //NH Settings
    public static int NHMax => (int)Generate.NHMax.Get();
}