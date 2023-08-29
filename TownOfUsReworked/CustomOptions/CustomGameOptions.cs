namespace TownOfUsReworked.CustomOptions;

[HarmonyPatch]
public static class CustomGameOptions
{
    //Global Options
    public static bool ConfirmEjects => Generate.ConfirmEjects;
    public static float PlayerSpeed => Generate.PlayerSpeed;
    public static float GhostSpeed => Generate.GhostSpeed;
    public static int InteractionDistance => Generate.InteractionDistance;
    public static int EmergencyButtonCount => Generate.EmergencyButtonCount;
    public static int EmergencyButtonCooldown => Generate.EmergencyButtonCooldown;
    public static float InitialCooldowns => Generate.InitialCooldowns;
    public static float MeetingCooldowns => Generate.MeetingCooldowns;
    public static float ReportDistance => Generate.ReportDistance;
    public static float ChatCooldown => Generate.ChatCooldown;
    public static int ChatCharacterLimit => Generate.ChatCharacterLimit;
    public static int DiscussionTime => Generate.DiscussionTime;
    public static int VotingTime => Generate.VotingTime;
    public static TaskBar TaskBarMode => (TaskBar)Generate.TaskBarMode.GetInt();
    public static bool EjectionRevealsRole => Generate.EjectionRevealsRole;
    public static int LobbySize => Generate.LobbySize;

    //Game Modifiers
    public static WhoCanVentOptions WhoCanVent => (WhoCanVentOptions)Generate.WhoCanVent.GetInt();
    public static bool VisualTasks => Generate.VisualTasks;
    public static bool AnonymousVoting => Generate.AnonymousVoting;
    public static bool FactionSeeRoles => Generate.FactionSeeRoles;
    public static bool ParallelMedScans => Generate.ParallelMedScans;
    public static DisableSkipButtonMeetings SkipButtonDisable => (DisableSkipButtonMeetings)Generate.SkipButtonDisable.GetInt();
    public static bool NoNames => Generate.NoNames;
    public static bool Whispers => Generate.Whispers;
    public static bool WhispersAnnouncement => Generate.WhispersAnnouncement;
    public static bool AppearanceAnimation => Generate.AppearanceAnimation;
    public static bool RandomSpawns => Generate.RandomSpawns;
    public static bool EnableAbilities => Generate.EnableAbilities;
    public static bool EnableModifiers => Generate.EnableModifiers;
    public static bool EnableObjectifiers => Generate.EnableObjectifiers;
    public static bool VentTargeting => Generate.VentTargeting;
    public static bool FirstKillShield => Generate.FirstKillShield;
    public static WhoCanSeeFirstKillShield WhoSeesFirstKillShield => (WhoCanSeeFirstKillShield)Generate.WhoSeesFirstKillShield.GetInt();

    //Better Sabotage Settings
    public static float ReactorShake => Generate.ReactorShake;
    public static bool OxySlow => Generate.OxySlow;
    public static bool CamouflagedComms => Generate.CamouflagedComms;
    public static bool CamouflagedMeetings => Generate.CamouflagedMeetings;
    //public static bool NightVision => Generate.NightVision;
    //public static bool EvilsIgnoreNV => Generate.EvilsIgnoreNV;

    //Announcement Settings
    public static bool LocationReports => Generate.LocationReports;
    public static RoleFactionReports RoleFactionReports => (RoleFactionReports)Generate.RoleFactionReports.GetInt();
    public static RoleFactionReports KillerReports => (RoleFactionReports)Generate.KillerReports.GetInt();
    public static bool GameAnnouncements => Generate.GameAnnouncements;

    //QOL Changes
    public static bool DeadSeeEverything => Generate.DeadSeeEverything;
    public static bool ObstructNames => Generate.ObstructNames;

    //Game Modes
    public static GameMode GameMode => (GameMode)Generate.CurrentMode.GetInt();

    //Killing Only Settings
    public static bool AddArsonist => Generate.AddArsonist;
    public static bool AddCryomaniac => Generate.AddCryomaniac;
    public static bool AddPlaguebearer => Generate.AddPlaguebearer;
    public static int NeutralRoles => Generate.NeutralRoles;

    //All Any Settings
    public static bool EnableUniques => Generate.EnableUniques;

    //Map Settings
    public static MapEnum Map
    {
        get
        {
            var map = Generate.Map.GetInt();

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
    public static int RandomMapSkeld => Generate.RandomMapSkeld;
    public static int RandomMapMira => Generate.RandomMapMira;
    public static int RandomMapPolus => Generate.RandomMapPolus;
    public static int RandomMapAirship => Generate.RandomMapAirship;
    public static int RandomMapSubmerged => SubLoaded ? Generate.RandomMapSubmerged : 0;
    public static int RandomMapLevelImpostor => LILoaded ? Generate.RandomMapLevelImpostor : 0;
    public static float SmallMapDecreasedCooldown => Generate.SmallMapDecreasedCooldown;
    public static float LargeMapIncreasedCooldown => Generate.LargeMapIncreasedCooldown;
    public static int SmallMapIncreasedShortTasks => Generate.SmallMapIncreasedShortTasks;
    public static int SmallMapIncreasedLongTasks => Generate.SmallMapIncreasedLongTasks;
    public static int LargeMapDecreasedShortTasks => Generate.LargeMapDecreasedShortTasks;
    public static int LargeMapDecreasedLongTasks => Generate.LargeMapDecreasedLongTasks;
    public static bool AutoAdjustSettings => Generate.AutoAdjustSettings;
    public static bool SmallMapHalfVision => Generate.SmallMapHalfVision;

    //Polus Settings
    public static bool VitalsLab => Generate.VitalsLab;
    public static bool ColdTempDeathValley => Generate.ColdTempDeathValley;
    public static bool WifiChartCourseSwap => Generate.WifiChartCourseSwap;
    public static bool PolusVentImprovements => Generate.PolusVentImprovements;
    public static float SeismicTimer => Generate.SeismicTimer;

    //Mira Settings
    public static bool MiraHQVentImprovements => Generate.MiraHQVentImprovements;
    public static float MiraO2Timer => Generate.MiraO2Timer;
    public static float MiraReactorTimer => Generate.MiraReactorTimer;

    //Skeld Settings
    public static bool SkeldVentImprovements => Generate.SkeldVentImprovements;
    public static float SkeldO2Timer => Generate.SkeldO2Timer;
    public static float SkeldReactorTimer => Generate.SkeldReactorTimer;

    //Airship Settings
    public static bool MoveDivert => Generate.MoveDivert;
    public static bool MoveFuel => Generate.MoveFuel;
    public static bool MoveVitals => Generate.MoveVitals;
    public static float MinDoorSwipeTime => Generate.MinDoorSwipeTime;
    public static float CrashTimer => Generate.CrashTimer;
    public static AirshipSpawnType SpawnType => (AirshipSpawnType)Generate.SpawnType.GetInt();
    public static MoveAdmin MoveAdmin => (MoveAdmin)Generate.MoveAdmin.GetInt();
    public static MoveElectrical MoveElectrical => (MoveElectrical)Generate.MoveElectrical.GetInt();

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
    public static float CrewVision => Generate.CrewVision;
    public static int ShortTasks => Generate.ShortTasks;
    public static int LongTasks => Generate.LongTasks;
    public static int CommonTasks => Generate.CommonTasks;
    public static bool GhostTasksCountToWin => Generate.GhostTasksCountToWin;
    public static bool CrewFlashlight => Generate.CrewFlashlight;
    public static bool CrewVent => Generate.CrewVent;
    public static int CrewMax => Generate.CrewMax;
    public static int CrewMin => Generate.CrewMin;

    //Intruder Options
    public static float IntruderVision => Generate.IntruderVision;
    public static float IntKillCd => Generate.IntKillCd;
    public static int IntruderCount => Generate.IntruderCount;
    public static bool IntrudersCanSabotage => Generate.IntrudersCanSabotage;
    public static bool IntrudersVent => Generate.IntrudersVent;
    public static float IntruderSabotageCooldown => Generate.IntruderSabotageCooldown;
    public static int IntruderMax => Generate.IntruderMax;
    public static int IntruderMin => Generate.IntruderMin;
    public static bool IntruderFlashlight => Generate.IntruderFlashlight;
    public static bool GhostsCanSabotage => Generate.GhostsCanSabotage;

    //Syndicate Options
    public static float SyndicateVision => Generate.SyndicateVision;
    public static bool AltImps => Generate.AltImps || IntruderCount == 0;
    public static SyndicateVentOptions SyndicateVent => (SyndicateVentOptions)Generate.SyndicateVent.GetInt();
    public static int SyndicateCount => Generate.SyndicateCount;
    public static bool GlobalDrive => Generate.GlobalDrive;
    public static float CDKillCd => Generate.CDKillCd;
    public static int ChaosDriveMeetingCount => Generate.ChaosDriveMeetingCount;
    public static int SyndicateMax => Generate.SyndicateMax;
    public static int SyndicateMin => Generate.SyndicateMin;
    public static bool SyndicateFlashlight => Generate.SyndicateFlashlight;

    //Neutral Options
    public static float NeutralVision => Generate.NeutralVision;
    public static bool LightsAffectNeutrals => Generate.LightsAffectNeutrals;
    public static NoSolo NoSolo => (NoSolo)Generate.NoSolo.GetInt();
    public static bool NeutralsVent => Generate.NeutralsVent;
    public static bool AvoidNeutralKingmakers => Generate.AvoidNeutralKingmakers;
    public static int NeutralMax => Generate.NeutralMax;
    public static int NeutralMin => Generate.NeutralMin;
    public static bool NeutralFlashlight => Generate.NeutralFlashlight;

    //Vampire Hunter Settings
    public static int VampireHunterCount => Generate.VampireHunterOn.GetCount();
    public static bool UniqueVampireHunter => Generate.UniqueVampireHunter;
    public static float StakeCd => Generate.StakeCd;

    //Mystic Settings
    public static int MysticCount => Generate.MysticOn.GetCount();
    public static bool UniqueMystic => Generate.UniqueMystic;
    public static float MysticRevealCd => Generate.MysticRevealCd;

    //Seer Settings
    public static int SeerCount => Generate.SeerOn.GetCount();
    public static bool UniqueSeer => Generate.UniqueSeer;
    public static float SeerCd => Generate.SeerCd;

    //Detective Settings
    public static int DetectiveCount => Generate.DetectiveOn.GetCount();
    public static float ExamineCd => Generate.ExamineCd;
    public static bool UniqueDetective => Generate.UniqueDetective;
    public static float RecentKill => Generate.RecentKill;
    public static float FootprintInterval => Generate.FootprintInterval;
    public static float FootprintDur => Generate.FootprintDur;
    public static bool AnonymousFootPrint => Generate.AnonymousFootPrint;

    //Inspector Settings
    public static int InspectorCount => Generate.InspectorOn.GetCount();
    public static float InspectCd => Generate.InspectCd;
    public static bool UniqueInspector => Generate.UniqueInspector;

    //Medium Settings
    public static int MediumCount => Generate.MediumOn.GetCount();
    public static DeadRevealed DeadRevealed => (DeadRevealed)Generate.DeadRevealed.GetInt();
    public static float MediateCd => Generate.MediateCd;
    public static bool ShowMediatePlayer => Generate.ShowMediatePlayer;
    public static ShowMediumToDead ShowMediumToDead => (ShowMediumToDead)Generate.ShowMediumToDead.GetInt();
    public static bool UniqueMedium => Generate.UniqueMedium;

    //Coroner Settings
    public static bool CoronerReportName => Generate.CoronerReportName;
    public static bool CoronerReportRole => Generate.CoronerReportRole;
    public static float CoronerArrowDur => Generate.CoronerArrowDur;
    public static bool UniqueCoroner => Generate.UniqueCoroner;
    public static int CoronerCount => Generate.CoronerOn.GetCount();
    public static float CoronerKillerNameTime => Generate.CoronerKillerNameTime;
    public static float CompareCd => Generate.CompareCd;
    public static float AutopsyCd => Generate.AutopsyCd;

    //Revealer Settings
    public static int RevealerTasksRemainingClicked => Generate.RevealerTasksRemainingClicked;
    public static int RevealerTasksRemainingAlert => Generate.RevealerTasksRemainingAlert;
    public static int RevealerCount => IsRoleList ? 1 : Generate.RevealerOn.GetCount();
    public static RevealerCanBeClickedBy RevealerCanBeClickedBy => (RevealerCanBeClickedBy)Generate.RevealerCanBeClickedBy.GetInt();
    public static bool RevealerRevealsNeutrals => Generate.RevealerRevealsNeutrals;
    public static bool RevealerRevealsCrew => Generate.RevealerRevealsCrew;
    public static bool RevealerRevealsTraitor => Generate.RevealerRevealsTraitor;
    public static bool RevealerRevealsFanatic => Generate.RevealerRevealsFanatic;
    public static bool RevealerRevealsRoles => Generate.RevealerRevealsRoles;

    //Sheriff Settings
    public static bool NeutEvilRed => Generate.NeutEvilRed;
    public static bool NeutKillingRed => Generate.NeutKillingRed;
    public static bool UniqueSheriff => Generate.UniqueSheriff;
    public static float InterrogateCd => Generate.InterrogateCd;
    public static int SheriffCount => Generate.SheriffOn.GetCount();

    //Tracker Settings
    public static bool ResetOnNewRound => Generate.ResetOnNewRound;
    public static float UpdateInterval => Generate.UpdateInterval;
    public static int TrackerCount => Generate.TrackerOn.GetCount();
    public static bool UniqueTracker => Generate.UniqueTracker;
    public static float TrackCd => Generate.TrackCd;
    public static int MaxTracks => Generate.MaxTracks;

    //Operative Settings
    public static float BugCd => Generate.BugCd;
    public static int MaxBugs => Generate.MaxBugs;
    public static float MinAmountOfTimeInBug => Generate.MinAmountOfTimeInBug;
    public static int OperativeCount => Generate.OperativeOn.GetCount();
    public static float BugRange => Generate.BugRange;
    public static int MinAmountOfPlayersInBug => Generate.MinAmountOfPlayersInBug;
    public static bool BugsRemoveOnNewRound => Generate.BugsRemoveOnNewRound;
    public static bool UniqueOperative => Generate.UniqueOperative;
    public static AdminDeadPlayers WhoSeesDead => (AdminDeadPlayers)Generate.WhoSeesDead.GetInt();
    public static bool PreciseOperativeInfo => Generate.PreciseOperativeInfo;

    //Veteran Settings
    public static float AlertCd => Generate.AlertCd;
    public static float AlertDur => Generate.AlertDur;
    public static int MaxAlerts => Generate.MaxAlerts;
    public static int VeteranCount => Generate.VeteranOn.GetCount();
    public static bool UniqueVeteran => Generate.UniqueVeteran;

    //Vigilante Settings
    public static VigiOptions VigiOptions => (VigiOptions)Generate.VigiOptions.GetInt();
    public static int VigilanteCount => Generate.VigilanteOn.GetCount();
    public static int MaxBullets => Generate.MaxBullets;
    public static float ShootCd => Generate.ShootCd;
    public static bool UniqueVigilante => Generate.UniqueVigilante;
    public static bool MisfireKillsInno => Generate.MisfireKillsInno;
    public static bool VigiKillAgain => Generate.VigiKillAgain;
    public static bool RoundOneNoShot => Generate.RoundOneNoShot;
    public static VigiNotif VigiNotifOptions => (VigiNotif)Generate.VigiNotifOptions.GetInt();

    //Altruist Settings
    public static bool AltruistTargetBody => Generate.AltruistTargetBody;
    public static bool UniqueAltruist => Generate.UniqueAltruist;
    public static float ReviveDur => Generate.ReviveDur;
    public static int AltruistCount => Generate.AltruistOn.GetCount();
    public static int MaxRevives => Generate.MaxRevives;
    public static float ReviveCd => Generate.ReviveCd;

    //Medic Settings
    public static ShieldOptions ShowShielded => (ShieldOptions)Generate.ShowShielded.GetInt();
    public static int MedicCount => Generate.MedicOn.GetCount();
    public static bool UniqueMedic => Generate.UniqueMedic;
    public static ShieldOptions NotificationShield => (ShieldOptions)Generate.WhoGetsNotification.GetInt();
    public static bool ShieldBreaks => Generate.ShieldBreaks;

    //Dictator Settings
    public static bool UniqueDictator => Generate.UniqueDictator;
    public static int DictatorCount => Generate.DictatorOn.GetCount();
    public static bool DictatorButton => Generate.DictatorButton;
    public static bool RoundOneNoDictReveal => Generate.RoundOneNoDictReveal;
    public static bool DictateAfterVoting => Generate.DictateAfterVoting;

    //Mayor Settings
    public static bool UniqueMayor => Generate.UniqueMayor;
    public static int MayorCount => Generate.MayorOn.GetCount();
    public static int MayorVoteCount => Generate.MayorVoteCount;
    public static bool MayorButton => Generate.MayorButton;
    public static bool RoundOneNoMayorReveal => Generate.RoundOneNoMayorReveal;

    //Monarch Settings
    public static bool UniqueMonarch => Generate.UniqueMonarch;
    public static int MonarchCount => Generate.MonarchOn.GetCount();
    public static int KnightVoteCount => Generate.KnightVoteCount;
    public static int KnightCount => Generate.KnightCount;
    public static bool KnightButton => Generate.KnightButton;
    public static bool MonarchButton => Generate.MonarchButton;
    public static bool RoundOneNoKnighting => Generate.RoundOneNoKnighting;
    public static float KnightingCd => Generate.KnightingCd;

    //Engineer Settings
    public static int EngineerCount => Generate.EngineerOn.GetCount();
    public static int MaxFixes => Generate.MaxFixes;
    public static bool UniqueEngineer => Generate.UniqueEngineer;
    public static float FixCd => Generate.FixCd;

    //Escort Settings
    public static int EscortCount => Generate.EscortOn.GetCount();
    public static float EscortCd => Generate.EscortCd;
    public static float EscortDur => Generate.EscortDur;
    public static bool UniqueEscort => Generate.UniqueEscort;

    //Chameleon Settings
    public static int ChameleonCount => Generate.ChameleonOn.GetCount();
    public static float SwoopCd => Generate.SwoopCd;
    public static float SwoopDur => Generate.SwoopDur;
    public static bool UniqueChameleon => Generate.UniqueChameleon;
    public static int MaxSwoops => Generate.MaxSwoops;

    //Retributionist Settings
    public static int RetributionistCount => Generate.RetributionistOn.GetCount();
    public static bool UniqueRetributionist => Generate.UniqueRetributionist;
    public static bool ReviveAfterVoting => Generate.ReviveAfterVoting;
    public static int MaxUses => Generate.MaxUses;

    //Shifter Settings
    public static BecomeEnum ShiftedBecomes => (BecomeEnum)Generate.ShiftedBecomes.GetInt();
    public static int ShifterCount => Generate.ShifterOn.GetCount();
    public static bool UniqueShifter => Generate.UniqueShifter;
    public static float ShiftCd => Generate.ShiftCd;

    //Transporter Settings
    public static float TransportCd => Generate.TransportCd;
    public static int MaxTransports => Generate.MaxTransports;
    public static int TransporterCount => Generate.TransporterOn.GetCount();
    public static bool UniqueTransporter => Generate.UniqueTransporter;
    public static bool TransSelf => Generate.TransSelf;
    public static float TransportDur => Generate.TransportDur;

    //Crewmate Settings
    public static int CrewCount => Generate.CrewmateOn.GetCount();

    //Amnesiac Settings
    public static bool RememberArrows => Generate.RememberArrows;
    public static int AmnesiacCount => Generate.AmnesiacOn.GetCount();
    public static bool AmneVent => Generate.AmneVent;
    public static bool AmneVentSwitch => Generate.AmneSwitchVent;
    public static float RememberArrowDelay => Generate.RememberArrowDelay;
    public static bool UniqueAmnesiac => Generate.UniqueAmnesiac;

    //Survivor Settings
    public static float VestCd => Generate.VestCd;
    public static float VestDur => Generate.VestDur;
    public static float VestKCReset => Generate.VestKCReset;
    public static int MaxVests => Generate.MaxVests;
    public static int SurvivorCount => Generate.SurvivorOn.GetCount();
    public static bool SurvVent => Generate.SurvVent;
    public static bool SurvVentSwitch => Generate.SurvSwitchVent;
    public static bool UniqueSurvivor => Generate.UniqueSurvivor;

    //GA Settings
    public static float ProtectCd => Generate.ProtectCd;
    public static float ProtectDur => Generate.ProtectDur;
    public static float ProtectKCReset => Generate.ProtectKCReset;
    public static int MaxProtects => Generate.MaxProtects;
    public static ProtectOptions ShowProtect => (ProtectOptions)Generate.ShowProtect.GetInt();
    public static int GuardianAngelCount => Generate.GuardianAngelOn.GetCount();
    public static bool GAVent => Generate.GAVent;
    public static bool GAVentSwitch => Generate.GASwitchVent;
    public static bool ProtectBeyondTheGrave => Generate.ProtectBeyondTheGrave;
    public static bool GATargetKnows => Generate.GATargetKnows;
    public static bool GAKnowsTargetRole => Generate.GAKnowsTargetRole;
    public static bool UniqueGuardianAngel => Generate.UniqueGuardianAngel;
    public static bool GuardianAngelCanPickTargets => Generate.GuardianAngelCanPickTargets;

    //Thief Settings
    public static int ThiefCount => Generate.ThiefOn.GetCount();
    public static bool ThiefVent => Generate.ThiefVent;
    public static float StealCd => Generate.StealCd;
    public static bool UniqueThief => Generate.UniqueThief;
    public static bool ThiefSteals => Generate.ThiefSteals;
    public static bool ThiefCanGuess => Generate.ThiefCanGuess;
    public static bool ThiefCanGuessAfterVoting => Generate.ThiefCanGuessAfterVoting;

    //Jester Settings
    public static bool VigiKillsJester => Generate.VigiKillsJester;
    public static bool JestEjectScreen => Generate.JestEjectScreen;
    public static bool JestVentSwitch => Generate.JestSwitchVent;
    public static bool JesterButton => Generate.JesterButton;
    public static bool JesterVent => Generate.JesterVent;
    public static int JesterCount => Generate.JesterOn.GetCount();
    public static int MaxHaunts => Generate.MaxHaunts;
    public static bool UniqueJester => Generate.UniqueJester;
    public static float HauntCd => Generate.HauntCd;

    //Actor Settings
    public static bool VigiKillsActor => Generate.VigiKillsActor;
    public static bool ActVentSwitch => Generate.ActSwitchVent;
    public static bool ActorButton => Generate.ActorButton;
    public static bool ActorVent => Generate.ActorVent;
    public static int ActorCount => Generate.ActorOn.GetCount();
    public static bool UniqueActor => Generate.UniqueActor;
    public static bool ActorCanPickRole => Generate.ActorCanPickRole;

    //Troll Settings
    public static bool TrollVent => Generate.TrollVent;
    public static float InteractCd => Generate.InteractCd;
    public static int TrollCount => Generate.TrollOn.GetCount();
    public static bool TrollVentSwitch => Generate.TrollSwitchVent;
    public static bool UniqueTroll => Generate.UniqueTroll;

    //Bounty Hunter Settings
    public static bool BHVent => Generate.BHVent;
    public static float GuessCd => Generate.GuessCd;
    public static int BHCount => Generate.BountyHunterOn.GetCount();
    public static int BountyHunterGuesses => Generate.BountyHunterGuesses;
    public static bool UniqueBountyHunter => Generate.UniqueBountyHunter;
    public static bool VigiKillsBH => Generate.VigiKillsBH;
    public static bool BountyHunterCanPickTargets => Generate.BountyHunterCanPickTargets;

    //Cannibal Settings
    public static float EatArrowDelay => Generate.EatArrowDelay;
    public static bool EatArrows => Generate.EatArrows;
    public static bool CannibalVent => Generate.CannibalVent;
    public static float EatCd => Generate.EatCd;
    public static int CannibalCount => Generate.CannibalOn.GetCount();
    public static int BodiesNeeded => Generate.BodiesNeeded;
    public static bool VigiKillsCannibal => Generate.VigiKillsCannibal;
    public static bool UniqueCannibal => Generate.UniqueCannibal;

    //Executioner Settings
    public static int ExecutionerCount => Generate.ExecutionerOn.GetCount();
    public static bool ExeCanWinBeyondDeath => Generate.ExeCanWinBeyondDeath;
    public static bool VigiKillsExecutioner => Generate.VigiKillsExecutioner;
    public static bool ExeVent => Generate.ExeVent;
    public static bool ExecutionerButton => Generate.ExecutionerButton;
    public static bool ExeTargetKnows => Generate.ExeTargetKnows;
    public static bool ExeKnowsTargetRole => Generate.ExeKnowsTargetRole;
    public static bool ExeEjectScreen => Generate.ExeEjectScreen;
    public static bool ExeVentSwitch => Generate.ExeSwitchVent;
    public static bool UniqueExecutioner => Generate.UniqueExecutioner;
    public static int MaxDooms => Generate.MaxDooms;
    public static float DoomCd => Generate.DoomCd;
    public static bool ExecutionerCanPickTargets => Generate.ExecutionerCanPickTargets;

    //Guesser Settings
    public static int GuesserCount => Generate.GuesserOn.GetCount();
    public static bool VigiKillsGuesser => Generate.VigiKillsGuesser;
    public static bool GuessVent => Generate.GuessVent;
    public static bool GuesserButton => Generate.GuesserButton;
    public static bool GuesserTargetKnows => Generate.GuessTargetKnows;
    public static bool GuessVentSwitch => Generate.GuessSwitchVent;
    public static bool UniqueGuesser => Generate.UniqueGuesser;
    public static bool GuesserAfterVoting => Generate.GuesserAfterVoting;
    public static bool MultipleGuesses => Generate.MultipleGuesses;
    public static int MaxGuesses => Generate.MaxGuesses;
    public static bool GuesserCanPickTargets => Generate.GuesserCanPickTargets;

    //Glitch Settings
    public static bool GlitchVent => Generate.GlitchVent;
    public static float MimicCd => Generate.MimicCd;
    public static float HackCd => Generate.HackCd;
    public static int GlitchCount => Generate.GlitchOn.GetCount();
    public static float MimicDur => Generate.MimicDur;
    public static float HackDur => Generate.HackDur;
    public static float NeutraliseCd => Generate.NeutraliseCd;
    public static bool UniqueGlitch => Generate.UniqueGlitch;

    //Juggernaut Settings
    public static float AssaultBonus => Generate.AssaultBonus;
    public static bool JuggVent => Generate.JuggVent;
    public static int JuggernautCount => Generate.JuggernautOn.GetCount();
    public static float AssaultCd => Generate.AssaultCd;
    public static bool UniqueJuggernaut => Generate.UniqueJuggernaut;

    //Cryomaniac Settings
    public static bool CryoVent => Generate.CryoVent;
    public static int CryomaniacCount => Generate.CryomaniacOn.GetCount();
    public static float CryoDouseCd => Generate.CryoDouseCd;
    public static bool UniqueCryomaniac => Generate.UniqueCryomaniac;
    public static bool CryoFreezeAll => Generate.CryoFreezeAll;
    public static bool CryoLastKillerBoost => Generate.CryoLastKillerBoost;

    //Plaguebearer Settings
    public static bool PBVent => Generate.PBVent;
    public static float InfectCd => Generate.InfectCd;
    public static int PlaguebearerCount => Generate.PlaguebearerOn.GetCount();
    public static bool UniquePlaguebearer => Generate.UniquePlaguebearer;

    //Arsonist Settings
    public static bool ArsoLastKillerBoost => Generate.ArsoLastKillerBoost;
    public static bool ArsoVent => Generate.ArsoVent;
    public static float ArsoDouseCd => Generate.ArsoDouseCd;
    public static float IgniteCd => Generate.IgniteCd;
    public static int ArsonistCount => Generate.ArsonistOn.GetCount();
    public static bool UniqueArsonist => Generate.UniqueArsonist;
    public static bool ArsoCooldownsLinked => Generate.ArsoCooldownsLinked;
    public static bool ArsoIgniteAll => Generate.ArsoIgniteAll;
    public static bool IgnitionCremates => Generate.IgnitionCremates;

    //Murderer Settings
    public static float MurderCd => Generate.MurderCd;
    public static bool MurdVent => Generate.MurdVent;
    public static int MurdCount => Generate.MurdererOn.GetCount();
    public static bool UniqueMurderer => Generate.UniqueMurderer;

    //SK Settings
    public static float BloodlustCd => Generate.BloodlustCd;
    public static float BloodlustDur => Generate.BloodlustDur;
    public static int SKCount => Generate.SerialKillerOn.GetCount();
    public static bool UniqueSerialKiller => Generate.UniqueSerialKiller;
    public static float StabCd => Generate.StabCd;
    public static SKVentOptions SKVentOptions => (SKVentOptions)Generate.SKVentOptions.GetInt();

    //WW Settings
    public static bool WerewolfVent => Generate.WerewolfVent;
    public static float MaulRadius => Generate.MaulRadius;
    public static float MaulCd => Generate.MaulCd;
    public static int WerewolfCount => Generate.WerewolfOn.GetCount();
    public static bool UniqueWerewolf => Generate.UniqueWerewolf;

    //Dracula Settings
    public static bool DracVent => Generate.DracVent;
    public static bool UndeadVent => Generate.UndeadVent;
    public static float BiteCd => Generate.BiteCd;
    public static int DraculaCount => Generate.DraculaOn.GetCount();
    public static int AliveVampCount => Generate.AliveVampCount;
    public static bool UniqueDracula => Generate.UniqueDracula;

    //Necromancer Settings
    public static bool NecroVent => Generate.NecroVent;
    public static bool NecroCooldownsLinked => Generate.NecroCooldownsLinked;
    public static float ResurrectCd => Generate.ResurrectCd;
    public static float NecroKillCd => Generate.NecroKillCd;
    public static float NecroKillCdIncrease => Generate.NecroKillCdIncrease;
    public static int MaxNecroKills => Generate.MaxNecroKills;
    public static float ResurrectCdIncrease => Generate.ResurrectCdIncrease;
    public static int MaxResurrections => Generate.MaxResurrections;
    public static int NecromancerCount => Generate.NecromancerOn.GetCount();
    public static bool UniqueNecromancer => Generate.UniqueNecromancer;
    public static bool NecromancerTargetBody => Generate.NecromancerTargetBody;
    public static float ResurrectDur => Generate.ResurrectDur;
    public static bool ResurrectVent => Generate.ResurrectVent;
    public static bool NecroKillCdIncreases => Generate.NecroKillCdIncreases;
    public static bool ResurrectCdIncreases => Generate.ResurrectCdIncreases;

    //Whisperer Settings
    public static bool WhispVent => Generate.WhispVent;
    public static float WhisperCd => Generate.WhisperCd;
    public static float WhisperRadius => Generate.WhisperRadius;
    public static float WhisperCdIncrease => Generate.WhisperCdIncrease;
    public static bool WhisperCdIncreases => Generate.WhisperCdIncreases;
    public static int WhisperRate => Generate.WhisperRate;
    public static int WhisperRateDecrease => Generate.WhisperRateDecrease;
    public static bool WhisperRateDecreases => Generate.WhisperRateDecreases;
    public static int WhispererCount => Generate.WhispererOn.GetCount();
    public static bool UniqueWhisperer => Generate.UniqueWhisperer;
    public static bool PersuadedVent => Generate.PersuadedVent;

    //Jackal Settings
    public static bool JackalVent => Generate.JackalVent;
    public static bool RecruitVent => Generate.RecruitVent;
    public static int JackalCount => Generate.JackalOn.GetCount();
    public static bool UniqueJackal => Generate.UniqueJackal;
    public static float RecruitCd => Generate.RecruitCd;

    //Phantom Settings
    public static int PhantomTasksRemaining => Generate.PhantomTasksRemaining;
    public static bool PhantomPlayersAlerted => Generate.PhantomPlayersAlerted;
    public static int PhantomCount => IsRoleList ? 1 : Generate.PhantomOn.GetCount();

    //Pestilence Settings
    public static float ObliterateCd => Generate.ObliterateCd;
    public static bool PlayersAlerted => Generate.PlayersAlerted;
    public static bool PestSpawn => Generate.PestSpawn;
    public static bool PestVent => Generate.PestVent;

    //Ghoul Settings
    public static float GhoulMarkCd => Generate.GhoulMarkCd;
    public static int GhoulCount => IsRoleList ? 1 : Generate.GhoulOn.GetCount();

    //Janitor Settings
    public static float CleanCd => Generate.CleanCd;
    public static int JanitorCount => Generate.JanitorOn.GetCount();
    public static bool SoloBoost => Generate.SoloBoost;
    public static bool UniqueJanitor => Generate.UniqueJanitor;
    public static bool JaniCooldownsLinked => Generate.JaniCooldownsLinked;
    public static JanitorOptions JanitorVentOptions => (JanitorOptions)Generate.JanitorVentOptions.GetInt();
    public static int DragModifier => Generate.DragModifier;
    public static float DragCd => Generate.DragCd;

    //Blackmailer Settings
    public static float BlackmailCd => Generate.BlackmailCd;
    public static int BlackmailerCount => Generate.BlackmailerOn.GetCount();
    public static bool UniqueBlackmailer => Generate.UniqueBlackmailer;
    public static bool WhispersNotPrivate => Generate.WhispersNotPrivate;
    public static bool BlackmailMates => Generate.BlackmailMates;
    public static bool BMRevealed => Generate.BMRevealed;

    //Grenadier Settings
    public static bool GrenadierIndicators => Generate.GrenadierIndicators;
    public static float FlashCd => Generate.FlashCd;
    public static int GrenadierCount => Generate.GrenadierOn.GetCount();
    public static float FlashDur => Generate.FlashDur;
    public static float FlashRadius => Generate.FlashRadius;
    public static bool GrenadierVent => Generate.GrenadierVent;
    public static bool UniqueGrenadier => Generate.UniqueGrenadier;

    //Camouflager Settings
    public static bool CamoHideSize => Generate.CamoHideSize;
    public static bool CamoHideSpeed => Generate.CamoHideSpeed;
    public static float CamouflagerCd => Generate.CamouflageCd;
    public static float CamouflageDur => Generate.CamouflageDur;
    public static int CamouflagerCount => Generate.CamouflagerOn.GetCount();
    public static bool UniqueCamouflager => Generate.UniqueCamouflager;

    //Morphling Settings
    public static bool MorphlingVent => Generate.MorphlingVent;
    public static int MorphlingCount => Generate.MorphlingOn.GetCount();
    public static float MorphCd => Generate.MorphCd;
    public static float SampleCd => Generate.SampleCd;
    public static float MorphDur => Generate.MorphDur;
    public static bool UniqueMorphling => Generate.UniqueMorphling;
    public static bool MorphCooldownsLinked => Generate.MorphCooldownsLinked;

    //Wraith Settings
    public static bool WraithVent => Generate.WraithVent;
    public static float InvisCd => Generate.InvisCd;
    public static float InvisDur => Generate.InvisDur;
    public static int WraithCount => Generate.WraithOn.GetCount();
    public static bool UniqueWraith => Generate.UniqueWraith;

    //Ambusher Settings
    public static float AmbushCd => Generate.AmbushCd;
    public static float AmbushDur => Generate.AmbushDur;
    public static int AmbusherCount => Generate.AmbusherOn.GetCount();
    public static bool UniqueAmbusher => Generate.UniqueAmbusher;
    public static bool AmbushMates => Generate.AmbushMates;

    //Enforcer Settings
    public static float EnforceCd => Generate.EnforceCd;
    public static float EnforceDur => Generate.EnforceDur;
    public static int EnforcerCount => Generate.EnforcerOn.GetCount();
    public static float EnforceRadius => Generate.EnforceRadius;
    public static float EnforceDelay => Generate.EnforceDelay;
    public static bool UniqueEnforcer => Generate.UniqueEnforcer;

    //Teleporter Settings
    public static bool TeleVent => Generate.TeleVent;
    public static float TeleportCd => Generate.TeleportCd;
    public static float TeleMarkCd => Generate.TeleMarkCd;
    public static int TeleporterCount => Generate.TeleporterOn.GetCount();
    public static bool UniqueTeleporter => Generate.UniqueTeleporter;
    public static bool TeleCooldownsLinked => Generate.TeleCooldownsLinked;

    //Consigliere Settings
    public static ConsigInfo ConsigInfo => (ConsigInfo)Generate.ConsigInfo.GetInt();
    public static float InvestigateCd => Generate.InvestigateCd;
    public static int ConsigliereCount => Generate.ConsigliereOn.GetCount();
    public static bool UniqueConsigliere => Generate.UniqueConsigliere;

    //Consort Settings
    public static int ConsortCount => Generate.ConsortOn.GetCount();
    public static float ConsortCd => Generate.ConsortCd;
    public static bool UniqueConsort => Generate.UniqueConsort;
    public static float ConsortDur => Generate.ConsortDur;

    //Disguiser Settings
    public static int DisguiserCount => Generate.DisguiserOn.GetCount();
    public static float DisguiseDur => Generate.DisguiseDur;
    public static float DisguiseCd => Generate.DisguiseCd;
    public static float DisguiseDelay => Generate.DisguiseDelay;
    public static DisguiserTargets DisguiseTarget => (DisguiserTargets)Generate.DisguiseTarget.GetInt();
    public static bool UniqueDisguiser => Generate.UniqueDisguiser;
    public static float MeasureCd => Generate.MeasureCd;
    public static bool DisgCooldownsLinked => Generate.DisgCooldownsLinked;

    //Godfather Settings
    public static int GodfatherCount => Generate.GodfatherOn.GetCount();
    public static bool UniqueGodfather => Generate.UniqueGodfather;
    public static float GFPromotionCdDecrease => Generate.GFPromotionCdDecrease;

    //Miner Settings
    public static float MineCd => Generate.MineCd;
    public static int MinerCount => Generate.MinerOn.GetCount();
    public static bool UniqueMiner => Generate.UniqueMiner;

    //Impostor Settings
    public static int ImpCount => Generate.ImpostorOn.GetCount();

    //Anarchist Settings
    public static int AnarchistCount => Generate.AnarchistOn.GetCount();
    public static float AnarchKillCd => Generate.AnarchKillCd;

    //Framer Settings
    public static int FramerCount => Generate.FramerOn.GetCount();
    public static float FrameCd => Generate.FrameCd;
    public static float ChaosDriveFrameRadius => Generate.ChaosDriveFrameRadius;
    public static bool UniqueFramer => Generate.UniqueFramer;

    //Spellslinger Settings
    public static int SpellslingerCount => Generate.SpellslingerOn.GetCount();
    public static float SpellCd => Generate.SpellCd;
    public static float SpellCdIncrease => Generate.SpellCdIncrease;
    public static bool UniqueSpellslinger => Generate.UniqueSpellslinger;

    //Collider Settings
    public static int ColliderCount => Generate.ColliderOn.GetCount();
    public static float CollideCd => Generate.CollideCd;
    public static float ChargeCd => Generate.ChargeCd;
    public static float ChargeDur => Generate.ChargeDur;
    public static float CollideRange => Generate.CollideRange;
    public static float CollideRangeIncrease => Generate.CollideRangeIncrease;
    public static bool UniqueCollider => Generate.UniqueCollider;
    public static bool CollideResetsCooldown => Generate.CollideResetsCooldown;
    public static bool ChargeCooldownsLinked => Generate.ChargeCooldownsLinked;

    //Shapeshifter Settings
    public static int ShapeshifterCount => Generate.ShapeshifterOn.GetCount();
    public static float ShapeshiftCd => Generate.ShapeshiftCd;
    public static float ShapeshiftDur => Generate.ShapeshiftDur;
    public static bool UniqueShapeshifter => Generate.UniqueShapeshifter;
    public static bool ShapeshiftMates => Generate.ShapeshiftMates;

    //Drunkard Settings
    public static int DrunkardCount => Generate.DrunkardOn.GetCount();
    public static float ConfuseCd => Generate.ConfuseCd;
    public static float ConfuseDur => Generate.ConfuseDur;
    public static bool UniqueDrunkard => Generate.UniqueDrunkard;
    public static bool ConfuseImmunity => Generate.ConfuseImmunity;

    //Time Keeper Settings
    public static int TimeKeeperCount => Generate.TimeKeeperOn.GetCount();
    public static float TimeCd => Generate.TimeCd;
    public static float TimeDur => Generate.TimeDur;
    public static bool UniqueTimeKeeper => Generate.UniqueTimeKeeper;
    public static bool TimeFreezeImmunity => Generate.TimeFreezeImmunity;
    public static bool TimeRewindImmunity => Generate.TimeRewindImmunity;

    //Crusader Settings
    public static float CrusadeCd => Generate.CrusadeCd;
    public static float CrusadeDur => Generate.CrusadeDur;
    public static int CrusaderCount => Generate.CrusaderOn.GetCount();
    public static bool UniqueCrusader => Generate.UniqueCrusader;
    public static float ChaosDriveCrusadeRadius => Generate.ChaosDriveCrusadeRadius;
    public static bool CrusadeMates => Generate.CrusadeMates;

    //Banshee Settings
    public static float ScreamCd => Generate.ScreamCd;
    public static float ScreamDur => Generate.ScreamDur;
    public static int BansheeCount => IsRoleList ? 1 : Generate.BansheeOn.GetCount();

    //Bomber Settings
    public static float BombCd => Generate.BombCd;
    public static float DetonateCd => Generate.DetonateCd;
    public static int BomberCount => Generate.BomberOn.GetCount();
    public static float BombRange => Generate.BombRange;
    public static bool UniqueBomber => Generate.UniqueBomber;
    public static bool BombCooldownsLinked => Generate.BombCooldownsLinked;
    public static bool BombsDetonateOnMeetingStart => Generate.BombsDetonateOnMeetingStart;
    public static bool BombsRemoveOnNewRound => Generate.BombsRemoveOnNewRound;
    public static float ChaosDriveBombRange => Generate.ChaosDriveBombRange;
    public static bool BombKillsSyndicate => Generate.BombKillsSyndicate;

    //Concealer Settings
    public static int ConcealerCount => Generate.ConcealerOn.GetCount();
    public static float ConcealCd => Generate.ConcealCd;
    public static float ConcealDur => Generate.ConcealDur;
    public static bool UniqueConcealer => Generate.UniqueConcealer;
    public static bool ConcealMates => Generate.ConcealMates;

    //Silencer Settings
    public static float SilenceCd => Generate.SilenceCd;
    public static int SilencerCount => Generate.SilencerOn.GetCount();
    public static bool UniqueSilencer => Generate.UniqueSilencer;
    public static bool WhispersNotPrivateSilencer => Generate.WhispersNotPrivateSilencer;
    public static bool SilenceMates => Generate.SilenceMates;
    public static bool SilenceRevealed => Generate.SilenceRevealed;

    //Stalker Settings
    public static int StalkerCount => Generate.StalkerOn.GetCount();
    public static bool UniqueStalker => Generate.UniqueStalker;
    public static float StalkCd => Generate.StalkCd;

    //Poisoner Settings
    public static float PoisonCd => Generate.PoisonCd;
    public static float PoisonDur => Generate.PoisonDur;
    public static int PoisonerCount => Generate.PoisonerOn.GetCount();
    public static bool UniquePoisoner => Generate.UniquePoisoner;

    //Rebel Settings
    public static int RebelCount => Generate.RebelOn.GetCount();
    public static bool UniqueRebel => Generate.UniqueRebel;
    public static float RebPromotionCdDecrease => Generate.RebPromotionCdDecrease;

    //Warper Settings
    public static float WarpCd => Generate.WarpCd;
    public static bool UniqueWarper => Generate.UniqueWarper;
    public static bool WarpSelf => Generate.WarpSelf;
    public static int WarperCount => Generate.WarperOn.GetCount();
    public static float WarpDur => Generate.WarpDur;

    //Betrayer Settings
    public static float BetrayCd => Generate.BetrayCd;
    public static bool BetrayerVent => Generate.BetrayerVent;

    //Modifier Settings
    public static int MaxModifiers => Generate.MaxModifiers;
    public static int MinModifiers => Generate.MinModifiers;

    //Objectifier Settings
    public static int MaxObjectifiers => Generate.MaxObjectifiers;
    public static int MinObjectifiers => Generate.MinObjectifiers;

    //Ability Settings
    public static int MaxAbilities => Generate.MaxAbilities;
    public static int MinAbilities => Generate.MinAbilities;

    //Snitch Settings
    public static bool SnitchSeesNeutrals => Generate.SnitchSeesNeutrals;
    public static int SnitchCount => Generate.SnitchOn.GetCount();
    public static bool SnitchSeesCrew => Generate.SnitchSeesCrew;
    public static bool SnitchSeesRoles => Generate.SnitchSeesRoles;
    public static bool SnitchSeestargetsInMeeting => Generate.SnitchSeestargetsInMeeting;
    public static bool SnitchSeesTraitor => Generate.SnitchSeesTraitor;
    public static bool SnitchSeesFanatic => Generate.SnitchSeesFanatic;
    public static bool SnitchKnows => Generate.SnitchKnows;
    public static int SnitchTasksRemaining => Generate.SnitchTasksRemaining;
    public static bool UniqueSnitch => Generate.UniqueSnitch;

    //Assassin Settings
    public static int AssassinKills => Generate.AssassinKills;
    public static int NumberOfIntruderAssassins => Generate.IntruderAssassinOn.GetCount();
    public static int NumberOfCrewAssassins => Generate.CrewAssassinOn.GetCount();
    public static int NumberOfNeutralAssassins => Generate.NeutralAssassinOn.GetCount();
    public static int NumberOfSyndicateAssassins => Generate.SyndicateAssassinOn.GetCount();
    public static bool AssassinGuessNeutralBenign => Generate.AssassinGuessNeutralBenign;
    public static bool AssassinGuessNeutralEvil => Generate.AssassinGuessNeutralEvil;
    public static bool AssassinGuessPest => Generate.AssassinGuessPest;
    public static bool AssassinGuessModifiers => Generate.AssassinGuessModifiers;
    public static bool AssassinGuessObjectifiers => Generate.AssassinGuessObjectifiers;
    public static bool AssassinGuessAbilities => Generate.AssassinGuessAbilities;
    public static bool AssassinMultiKill => Generate.AssassinMultiKill;
    public static bool AssassinateAfterVoting => Generate.AssassinateAfterVoting;
    public static bool AssassinGuessInvestigative => Generate.AssassinGuessInvestigative;
    public static bool UniqueCrewAssassin => Generate.UniqueCrewAssassin;
    public static bool UniqueNeutralAssassin => Generate.UniqueNeutralAssassin;
    public static bool UniqueIntruderAssassin => Generate.UniqueIntruderAssassin;
    public static bool UniqueSyndicateAssassin => Generate.UniqueSyndicateAssassin;

    //Underdog Settings
    public static bool UnderdogIncreasedKC => Generate.UnderdogIncreasedKC;
    public static float UnderdogKillBonus => Generate.UnderdogKillBonus;
    public static bool UniqueUnderdog => Generate.UniqueUnderdog;
    public static bool UnderdogKnows => Generate.UnderdogKnows;
    public static int UnderdogCount => Generate.UnderdogOn.GetCount();

    //Multitasker Settings
    public static int MultitaskerCount => Generate.MultitaskerOn.GetCount();
    public static float Transparancy => Generate.Transparancy;
    public static bool UniqueMultitasker => Generate.UniqueMultitasker;

    //BB Settings
    public static int ButtonBarryCount => Generate.ButtonBarryOn.GetCount();
    public static float ButtonCooldown => Generate.ButtonCooldown;
    public static bool UniqueButtonBarry => Generate.UniqueButtonBarry;

    //Swapper Settings
    public static bool SwapperButton => Generate.SwapperButton;
    public static int SwapperCount => Generate.SwapperOn.GetCount();
    public static bool SwapAfterVoting => Generate.SwapAfterVoting;
    public static bool SwapSelf => Generate.SwapSelf;
    public static bool UniqueSwapper => Generate.UniqueSwapper;

    //Politician Settings
    public static bool PoliticianAnonymous => Generate.PoliticianAnonymous;
    public static int PoliticianVoteBank => Generate.PoliticianVoteBank;
    public static bool UniquePolitician => Generate.UniquePolitician;
    public static int PoliticianCount => Generate.PoliticianOn.GetCount();
    public static bool PoliticianButton => Generate.PoliticianButton;

    //Tiebreaker Settings
    public static bool TiebreakerKnows => Generate.TiebreakerKnows;
    public static int TiebreakerCount => Generate.TiebreakerOn.GetCount();
    public static bool UniqueTiebreaker => Generate.UniqueTiebreaker;

    //Torch Settings
    public static int TorchCount => Generate.TorchOn.GetCount();
    public static bool UniqueTorch => Generate.UniqueTorch;

    //Tunneler Settings
    public static bool TunnelerKnows => Generate.TunnelerKnows;
    public static int TunnelerCount => Generate.TunnelerOn.GetCount();
    public static bool UniqueTunneler => Generate.UniqueTunneler;

    //Radar Settings
    public static bool UniqueRadar => Generate.UniqueRadar;
    public static int RadarCount => Generate.RadarOn.GetCount();

    //Ninja Settings
    public static int NinjaCount => Generate.NinjaOn.GetCount();
    public static bool UniqueNinja => Generate.UniqueNinja;

    //Ruthless Settings
    public static int RuthlessCount => Generate.RuthlessOn.GetCount();
    public static bool UniqueRuthless => Generate.UniqueRuthless;
    public static bool RuthlessKnows => Generate.RuthlessKnows;

    //Insider Settings
    public static bool InsiderKnows => Generate.InsiderKnows;
    public static int InsiderCount => Generate.InsiderOn.GetCount();
    public static bool UniqueInsider => Generate.UniqueInsider;

    //Traitor Settings
    public static int TraitorCount => Generate.TraitorOn.GetCount();
    public static bool TraitorColourSwap => Generate.TraitorColourSwap;
    public static bool TraitorKnows => Generate.TraitorKnows;
    public static bool UniqueTraitor => Generate.UniqueTraitor;

    //Fanatic Settings
    public static bool FanaticKnows => Generate.FanaticKnows;
    public static int FanaticCount => Generate.FanaticOn.GetCount();
    public static bool UniqueFanatic => Generate.UniqueFanatic;
    public static bool FanaticColourSwap => Generate.FanaticColourSwap;

    //Taskmaster Settings
    public static int TMTasksRemaining => Generate.TMTasksRemaining;
    public static int TaskmasterCount => Generate.TaskmasterOn.GetCount();
    public static bool UniqueTaskmaster => Generate.UniqueTaskmaster;

    //Lovers Settings
    public static bool BothLoversDie => Generate.BothLoversDie;
    public static bool UniqueLovers => Generate.UniqueLovers;
    public static bool LoversChat => Generate.LoversChat;
    public static int LoversCount => Generate.LoversOn.GetCount();
    public static bool LoversRoles => Generate.LoversRoles;

    //Linked Settings
    public static bool UniqueLinked => Generate.UniqueLinked;
    public static bool LinkedChat => Generate.LinkedChat;
    public static int LinkedCount => Generate.LinkedOn.GetCount();
    public static bool LinkedRoles => Generate.LinkedRoles;

    //Defector Settings
    public static bool DefectorKnows => Generate.DefectorKnows;
    public static bool UniqueDefector => Generate.UniqueDefector;
    public static int DefectorCount => Generate.DefectorOn.GetCount();
    public static DefectorFaction DefectorFaction => (DefectorFaction)Generate.DefectorFaction.GetInt();

    //Rivals Settings
    public static bool RivalsChat => Generate.RivalsChat;
    public static int RivalsCount => Generate.RivalsOn.GetCount();
    public static bool RivalsRoles => Generate.RivalsRoles;
    public static bool UniqueRivals => Generate.UniqueRivals;

    //Mafia Settings
    public static int MafiaCount => Generate.MafiaOn.GetCount();
    public static bool MafiaRoles => Generate.MafiaRoles;
    public static bool UniqueMafia => Generate.UniqueMafia;
    public static bool MafVent => Generate.MafVent;

    //Giant Settings
    public static int GiantCount => Generate.GiantOn.GetCount();
    public static float GiantSpeed => Generate.GiantSpeed;
    public static float GiantScale => Generate.GiantScale;
    public static bool UniqueGiant => Generate.UniqueGiant;

    //Indomitable Settings
    public static bool UniqueIndomitable => Generate.UniqueIndomitable;
    public static int IndomitableCount => Generate.IndomitableOn.GetCount();
    public static bool IndomitableKnows => Generate.IndomitableKnows;

    //Overlord Settings
    public static int OverlordCount => Generate.OverlordOn.GetCount();
    public static bool UniqueOverlord => Generate.UniqueOverlord;
    public static int OverlordMeetingWinCount => Generate.OverlordMeetingWinCount;
    public static bool OverlordKnows => Generate.OverlordKnows;

    //Allied Settings
    public static int AlliedCount => Generate.AlliedOn.GetCount();
    public static bool UniqueAllied => Generate.UniqueAllied;
    public static AlliedFaction AlliedFaction => (AlliedFaction)Generate.AlliedFaction.GetInt();

    //Corrupted Settings
    public static int CorruptedCount => Generate.CorruptedOn.GetCount();
    public static bool UniqueCorrupted => Generate.UniqueCorrupted;
    public static float CorruptCd => Generate.CorruptCd;
    public static bool AllCorruptedWin => Generate.AllCorruptedWin;
    public static bool CorruptedVent => Generate.CorruptedVent;

    //Dwarf Settings
    public static float DwarfSpeed => Generate.DwarfSpeed;
    public static bool UniqueDwarf => Generate.UniqueDwarf;
    public static float DwarfScale => Generate.DwarfScale;
    public static int DwarfCount => Generate.DwarfOn.GetCount();

    //Drunk Settings
    public static bool DrunkControlsSwap => Generate.DrunkControlsSwap;
    public static int DrunkCount => Generate.DrunkOn.GetCount();
    public static bool UniqueDrunk => Generate.UniqueDrunk;
    public static bool DrunkKnows => Generate.DrunkKnows;
    public static float DrunkInterval => Generate.DrunkInterval;

    //Bait Settings
    public static bool BaitKnows => Generate.BaitKnows;
    public static float BaitMinDelay => Generate.BaitMinDelay;
    public static float BaitMaxDelay => Generate.BaitMaxDelay;
    public static int BaitCount => Generate.BaitOn.GetCount();
    public static bool UniqueBait => Generate.UniqueBait;

    //Diseased Settings
    public static bool DiseasedKnows => Generate.DiseasedKnows;
    public static float DiseasedMultiplier => Generate.DiseasedKillMultiplier;
    public static int DiseasedCount => Generate.DiseasedOn.GetCount();
    public static bool UniqueDiseased => Generate.UniqueDiseased;

    //Shy Settings
    public static int ShyCount => Generate.ShyOn.GetCount();
    public static bool UniqueShy => Generate.UniqueShy;

    //Astral Settings
    public static int AstralCount => Generate.AstralOn.GetCount();
    public static bool UniqueAstral => Generate.UniqueAstral;

    //Yeller Settings
    public static int YellerCount => Generate.YellerOn.GetCount();
    public static bool UniqueYeller => Generate.UniqueYeller;

    //VIP Settings
    public static bool VIPKnows => Generate.VIPKnows;
    public static bool UniqueVIP => Generate.UniqueVIP;
    public static int VIPCount => Generate.VIPOn.GetCount();

    //Volatile Settings
    public static int VolatileCount => Generate.VolatileOn.GetCount();
    public static float VolatileInterval => Generate.VolatileInterval;
    public static bool UniqueVolatile => Generate.UniqueVolatile;
    public static bool VolatileKnows => Generate.VolatileKnows;

    //Professional Settings
    public static bool ProfessionalKnows => Generate.ProfessionalKnows;
    public static bool UniqueProfessional => Generate.UniqueProfessional;
    public static int ProfessionalCount => Generate.ProfessionalOn.GetCount();

    //Coward Settings
    public static int CowardCount => Generate.CowardOn.GetCount();
    public static bool UniqueCoward => Generate.UniqueCoward;

    //NB Settings
    public static int NBMax => Generate.NBMax;
    public static bool VigiKillsNB => Generate.VigiKillsNB;

    //NK Settings
    public static int NKMax => Generate.NKMax;
    public static bool NKHasImpVision => Generate.NKHasImpVision;
    public static bool NKsKnow => Generate.NKsKnow;

    //CSv Settings
    public static int CSvMax => Generate.CSvMax;

    //CA Settings
    public static int CAMax => Generate.CAMax;

    //CK Settings
    public static int CKMax => Generate.CKMax;

    //CS Settings
    public static int CSMax => Generate.CSMax;

    //CI Settings
    public static int CIMax => Generate.CIMax;

    //CP Settings
    public static int CPMax => Generate.CPMax;

    //IC Settings
    public static int ICMax => Generate.ICMax;

    //ID Settings
    public static int IDMax => Generate.IDMax;

    //IS Settings
    public static int ISMax => Generate.ISMax;

    //IK Settings
    public static int IKMax => Generate.IKMax;

    //SD Settings
    public static int SDMax => Generate.SDMax;

    //SyK Settings
    public static int SyKMax => Generate.SyKMax;

    //SSu Settings
    public static int SSuMax => Generate.SSuMax;

    //SP Settings
    public static int SPMax => Generate.SPMax;

    //NE Settings
    public static int NEMax => Generate.NEMax;
    public static bool NeutralEvilsEndGame => Generate.NeutralEvilsEndGame;

    //NN Settings
    public static int NNMax => Generate.NNMax;
    public static bool NNHasImpVision => Generate.NNHasImpVision;

    //NH Settings
    public static int NHMax => Generate.NHMax;

    //Free Bans
    public static bool BanCrewmate => Generate.BanCrewmate;
    public static bool BanImpostor => Generate.BanImpostor;
    public static bool BanAnarchist => Generate.BanAnarchist;

    //Enabling Postmortals
    public static bool EnableBanshee => Generate.EnableBanshee;
    public static bool EnableGhoul => Generate.EnableGhoul;
    public static bool EnablePhantom => Generate.EnablePhantom;
    public static bool EnableRevealer => Generate.EnableRevealer;
}