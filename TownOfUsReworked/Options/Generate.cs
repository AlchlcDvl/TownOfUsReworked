namespace TownOfUsReworked.Options;

public static class Generate
{
    // Game Options
    public static CustomHeaderOption GameSettings;
    public static CustomNumberOption PlayerSpeed;
    public static CustomNumberOption GhostSpeed;
    public static CustomNumberOption InteractionDistance;
    public static CustomNumberOption EmergencyButtonCount;
    public static CustomNumberOption EmergencyButtonCooldown;
    public static CustomToggleOption EnableInitialCds;
    public static CustomNumberOption InitialCooldowns;
    public static CustomToggleOption EnableMeetingCds;
    public static CustomNumberOption MeetingCooldowns;
    public static CustomNumberOption DiscussionTime;
    public static CustomNumberOption VotingTime;
    public static CustomToggleOption ConfirmEjects;
    public static CustomToggleOption EjectionRevealsRole;
    public static CustomStringOption TaskBarMode;
    public static CustomNumberOption ReportDistance;
    public static CustomNumberOption ChatCooldown;
    public static CustomNumberOption ChatCharacterLimit;
    public static CustomNumberOption LobbySize;
    public static CustomToggleOption EnableFailCds;
    public static CustomNumberOption FailCooldowns;

    // Map Settings
    public static CustomHeaderOption MapSettings;
    public static CustomNumberOption RandomMapSkeld;
    public static CustomNumberOption RandomMapMira;
    public static CustomNumberOption RandomMapPolus;
    public static CustomNumberOption RandomMapdlekS;
    public static CustomNumberOption RandomMapAirship;
    public static CustomNumberOption RandomMapFungle;
    public static CustomNumberOption RandomMapSubmerged;
    public static CustomNumberOption RandomMapLevelImpostor;
    public static CustomToggleOption AutoAdjustSettings;
    public static CustomToggleOption SmallMapHalfVision;
    public static CustomNumberOption SmallMapDecreasedCooldown;
    public static CustomNumberOption LargeMapIncreasedCooldown;
    public static CustomNumberOption SmallMapIncreasedShortTasks;
    public static CustomNumberOption SmallMapIncreasedLongTasks;
    public static CustomNumberOption LargeMapDecreasedShortTasks;
    public static CustomNumberOption LargeMapDecreasedLongTasks;
    public static CustomStringOption Map;
    public static CustomStringOption RandomSpawns;

    // Game Modifier Options
    public static CustomHeaderOption GameModifiers;
    public static CustomStringOption AnonymousVoting;
    public static CustomStringOption WhoCanVent;
    public static CustomStringOption SkipButtonDisable;
    public static CustomToggleOption FirstKillShield;
    public static CustomStringOption WhoSeesFirstKillShield;
    public static CustomToggleOption FactionSeeRoles;
    public static CustomToggleOption VisualTasks;
    public static CustomStringOption PlayerNames;
    public static CustomToggleOption Whispers;
    public static CustomToggleOption WhispersAnnouncement;
    public static CustomToggleOption AppearanceAnimation;
    public static CustomToggleOption EnableModifiers;
    public static CustomToggleOption EnableAbilities;
    public static CustomToggleOption EnableObjectifiers;
    public static CustomToggleOption VentTargeting;
    public static CustomToggleOption CooldownInVent;
    public static CustomToggleOption DeadSeeEverything;
    public static CustomToggleOption ParallelMedScans;
    public static CustomToggleOption HideVentAnims;

    // Better Sabotages
    public static CustomHeaderOption BetterSabotages;
    public static CustomToggleOption OxySlow;
    public static CustomNumberOption ReactorShake;
    public static CustomToggleOption CamouflagedComms;
    public static CustomToggleOption CamouflagedMeetings;
    public static CustomToggleOption NightVision;
    public static CustomToggleOption EvilsIgnoreNV;

    // Better Skeld Options
    public static CustomHeaderOption BetterSkeld;
    public static CustomToggleOption EnableBetterSkeld;
    public static CustomToggleOption SkeldVentImprovements;
    public static CustomNumberOption SkeldO2Timer;
    public static CustomNumberOption SkeldReactorTimer;

    // Better Mira HQ Options
    public static CustomHeaderOption BetterMiraHQ;
    public static CustomToggleOption EnableBetterMiraHQ;
    public static CustomToggleOption MiraHQVentImprovements;
    public static CustomNumberOption MiraO2Timer;
    public static CustomNumberOption MiraReactorTimer;

    // Better Airship Options
    public static CustomHeaderOption BetterAirship;
    public static CustomToggleOption EnableBetterAirship;
    public static CustomStringOption SpawnType;
    public static CustomStringOption MoveAdmin;
    public static CustomStringOption MoveElectrical;
    public static CustomNumberOption MinDoorSwipeTime;
    public static CustomToggleOption MoveDivert;
    public static CustomToggleOption MoveFuel;
    public static CustomToggleOption MoveVitals;
    public static CustomNumberOption CrashTimer;

    // Better Fungle Options
    public static CustomHeaderOption BetterFungle;
    public static CustomToggleOption EnableBetterFungle;
    public static CustomNumberOption FungleReactorTimer;
    public static CustomNumberOption FungleMixupTimer;

    // Better Polus Options
    public static CustomHeaderOption BetterPolus;
    public static CustomToggleOption EnableBetterPolus;
    public static CustomToggleOption PolusVentImprovements;
    public static CustomToggleOption VitalsLab;
    public static CustomToggleOption ColdTempDeathValley;
    public static CustomToggleOption WifiChartCourseSwap;
    public static CustomNumberOption SeismicTimer;

    // Game Modes
    public static CustomHeaderOption GameModeSettings;
    public static CustomStringOption CurrentMode;

    // Killing Only Options
    public static CustomHeaderOption KOSettings;
    public static CustomNumberOption NeutralRoles;
    public static CustomToggleOption AddArsonist;
    public static CustomToggleOption AddCryomaniac;
    public static CustomToggleOption AddPlaguebearer;

    // Announcement Options
    public static CustomHeaderOption GameAnnouncementsSettings;
    public static CustomToggleOption GameAnnouncements;
    public static CustomStringOption RoleFactionReports;
    public static CustomStringOption KillerReports;
    public static CustomToggleOption LocationReports;

    // All Any Options
    public static CustomHeaderOption AARLSettings;
    public static CustomToggleOption EnableUniques;

    // Hide And Seek Options
    public static CustomHeaderOption HnSSettings;
    public static CustomNumberOption HunterCount;
    public static CustomNumberOption StartTime;
    public static CustomNumberOption HuntCd;
    public static CustomStringOption HnSType;
    public static CustomToggleOption HunterVent;
    public static CustomNumberOption HnSShortTasks;
    public static CustomNumberOption HnSCommonTasks;
    public static CustomNumberOption HnSLongTasks;
    public static CustomNumberOption HunterVision;
    public static CustomNumberOption HuntedVision;
    public static CustomNumberOption HunterSpeedModifier;
    public static CustomToggleOption HunterFlashlight;
    public static CustomToggleOption HuntedFlashlight;
    public static CustomToggleOption HuntedChat;

    // Task Race Options
    public static CustomHeaderOption TRSettings;
    public static CustomNumberOption TRShortTasks;
    public static CustomNumberOption TRCommonTasks;

    // Role Gen Settings
    public static CustomHeaderOption ClassCustSettings;
    public static CustomToggleOption IgnoreFactionCaps;
    public static CustomToggleOption IgnoreAlignmentCaps;
    public static CustomToggleOption IgnoreLayerCaps;

    // CI Role Spawn
    public static CustomHeaderOption CIRoles;
    public static CustomLayersOption DetectiveOn;
    public static CustomLayersOption CoronerOn;
    public static CustomLayersOption SheriffOn;
    public static CustomLayersOption MediumOn;
    public static CustomLayersOption TrackerOn;
    public static CustomLayersOption OperativeOn;
    public static CustomLayersOption SeerOn;

    // CSv Role Spawn
    public static CustomHeaderOption CSvRoles;
    public static CustomLayersOption MayorOn;
    public static CustomLayersOption DictatorOn;
    public static CustomLayersOption MonarchOn;

    // CrP Role Spawn
    public static CustomHeaderOption CrPRoles;
    public static CustomLayersOption AltruistOn;
    public static CustomLayersOption MedicOn;
    public static CustomLayersOption TrapperOn;

    // CA Role Spawn
    public static CustomHeaderOption CARoles;
    public static CustomLayersOption VampireHunterOn;
    public static CustomLayersOption MysticOn;

    // CK Role Spawn
    public static CustomHeaderOption CKRoles;
    public static CustomLayersOption VeteranOn;
    public static CustomLayersOption VigilanteOn;
    public static CustomLayersOption BastionOn;

    // CS Role Spawn
    public static CustomHeaderOption CSRoles;
    public static CustomLayersOption EngineerOn;
    public static CustomLayersOption ShifterOn;
    public static CustomLayersOption EscortOn;
    public static CustomLayersOption TransporterOn;
    public static CustomLayersOption RevealerOn;
    public static CustomLayersOption RetributionistOn;
    public static CustomLayersOption ChameleonOn;

    // CU Role Spawn
    public static CustomHeaderOption CURoles;
    public static CustomLayersOption CrewmateOn;

    // NB Role Spawn
    public static CustomHeaderOption NBRoles;
    public static CustomLayersOption AmnesiacOn;
    public static CustomLayersOption GuardianAngelOn;
    public static CustomLayersOption SurvivorOn;
    public static CustomLayersOption ThiefOn;

    // NH Role Spawn
    public static CustomHeaderOption NHRoles;
    public static CustomLayersOption PlaguebearerOn;

    // NP Role Spawn
    public static CustomHeaderOption NPRoles;
    public static CustomLayersOption PhantomOn;

    // NN Role Spawn
    public static CustomHeaderOption NNRoles;
    public static CustomLayersOption DraculaOn;
    public static CustomLayersOption JackalOn;
    public static CustomLayersOption NecromancerOn;
    public static CustomLayersOption WhispererOn;

    // NE Role Spawn
    public static CustomHeaderOption NERoles;
    public static CustomLayersOption ExecutionerOn;
    public static CustomLayersOption ActorOn;
    public static CustomLayersOption JesterOn;
    public static CustomLayersOption CannibalOn;
    public static CustomLayersOption BountyHunterOn;
    public static CustomLayersOption TrollOn;
    public static CustomLayersOption GuesserOn;

    // NK Role Spawn
    public static CustomHeaderOption NKRoles;
    public static CustomLayersOption ArsonistOn;
    public static CustomLayersOption CryomaniacOn;
    public static CustomLayersOption GlitchOn;
    public static CustomLayersOption MurdererOn;
    public static CustomLayersOption WerewolfOn;
    public static CustomLayersOption SerialKillerOn;
    public static CustomLayersOption JuggernautOn;

    // IC Role Spawn
    public static CustomHeaderOption ICRoles;
    public static CustomLayersOption BlackmailerOn;
    public static CustomLayersOption CamouflagerOn;
    public static CustomLayersOption GrenadierOn;
    public static CustomLayersOption JanitorOn;

    // ID Role Spawn
    public static CustomHeaderOption IDRoles;
    public static CustomLayersOption MorphlingOn;
    public static CustomLayersOption DisguiserOn;
    public static CustomLayersOption WraithOn;

    // IH Role Spawn
    public static CustomHeaderOption IHRoles;
    public static CustomLayersOption GodfatherOn;

    // IK Role Spawn
    public static CustomHeaderOption IKRoles;
    public static CustomLayersOption AmbusherOn;
    public static CustomLayersOption EnforcerOn;

    // IS Role Spawn
    public static CustomHeaderOption ISRoles;
    public static CustomLayersOption ConsigliereOn;
    public static CustomLayersOption ConsortOn;
    public static CustomLayersOption MinerOn;
    public static CustomLayersOption TeleporterOn;

    // IU Role Spawn
    public static CustomHeaderOption IURoles;
    public static CustomLayersOption ImpostorOn;
    public static CustomLayersOption GhoulOn;

    // SSu Role Spawn
    public static CustomHeaderOption SSuRoles;
    public static CustomLayersOption WarperOn;
    public static CustomLayersOption StalkerOn;

    // SD Role Spawn
    public static CustomHeaderOption SDRoles;
    public static CustomLayersOption FramerOn;
    public static CustomLayersOption ShapeshifterOn;
    public static CustomLayersOption ConcealerOn;
    public static CustomLayersOption DrunkardOn;
    public static CustomLayersOption SilencerOn;
    public static CustomLayersOption TimekeeperOn;

    // SyK Role Spawn
    public static CustomHeaderOption SPRoles;
    public static CustomLayersOption RebelOn;
    public static CustomLayersOption SpellslingerOn;

    // SyK Role Spawn
    public static CustomHeaderOption SyKRoles;
    public static CustomLayersOption BomberOn;
    public static CustomLayersOption CrusaderOn;
    public static CustomLayersOption ColliderOn;
    public static CustomLayersOption PoisonerOn;

    // SU Role Spawn
    public static CustomHeaderOption SURoles;
    public static CustomLayersOption AnarchistOn;
    public static CustomLayersOption BansheeOn;

    // Modifier Spawn
    public static CustomHeaderOption Modifiers;
    public static CustomLayersOption BaitOn;
    public static CustomLayersOption DiseasedOn;
    public static CustomLayersOption GiantOn;
    public static CustomLayersOption DwarfOn;
    public static CustomLayersOption CowardOn;
    public static CustomLayersOption VIPOn;
    public static CustomLayersOption ShyOn;
    public static CustomLayersOption DrunkOn;
    public static CustomLayersOption IndomitableOn;
    public static CustomLayersOption AstralOn;
    public static CustomLayersOption VolatileOn;
    public static CustomLayersOption ProfessionalOn;
    public static CustomLayersOption YellerOn;
    public static CustomLayersOption ColorblindOn;

    // Ability Spawn
    public static CustomHeaderOption Abilities;
    public static CustomLayersOption CrewAssassinOn;
    public static CustomLayersOption IntruderAssassinOn;
    public static CustomLayersOption SyndicateAssassinOn;
    public static CustomLayersOption NeutralAssassinOn;
    public static CustomLayersOption TorchOn;
    public static CustomLayersOption ButtonBarryOn;
    public static CustomLayersOption TiebreakerOn;
    public static CustomLayersOption TunnelerOn;
    public static CustomLayersOption UnderdogOn;
    public static CustomLayersOption SnitchOn;
    public static CustomLayersOption RadarOn;
    public static CustomLayersOption InsiderOn;
    public static CustomLayersOption MultitaskerOn;
    public static CustomLayersOption RuthlessOn;
    public static CustomLayersOption NinjaOn;
    public static CustomLayersOption SwapperOn;
    public static CustomLayersOption PoliticianOn;

    // Objectifier Spawn
    public static CustomHeaderOption Objectifiers;
    public static CustomLayersOption LoversOn;
    public static CustomLayersOption LinkedOn;
    public static CustomLayersOption AlliedOn;
    public static CustomLayersOption MafiaOn;
    public static CustomLayersOption TraitorOn;
    public static CustomLayersOption RivalsOn;
    public static CustomLayersOption FanaticOn;
    public static CustomLayersOption TaskmasterOn;
    public static CustomLayersOption OverlordOn;
    public static CustomLayersOption CorruptedOn;
    public static CustomLayersOption DefectorOn;

    // Crew Options
    public static CustomHeaderOption CrewSettings;
    public static CustomToggleOption CrewVent;
    public static CustomNumberOption ShortTasks;
    public static CustomNumberOption LongTasks;
    public static CustomNumberOption CommonTasks;
    public static CustomNumberOption CrewVision;
    public static CustomToggleOption GhostTasksCountToWin;
    public static CustomNumberOption CrewMax;
    public static CustomNumberOption CrewMin;
    public static CustomToggleOption CrewFlashlight;

    // CSv Options
    public static CustomHeaderOption CSvSettings;
    public static CustomNumberOption CSvMax;

    // Mayor Options
    public static CustomHeaderOption Mayor;
    public static CustomToggleOption UniqueMayor;
    public static CustomNumberOption MayorVoteCount;
    public static CustomToggleOption RoundOneNoMayorReveal;
    public static CustomToggleOption MayorButton;

    // Dictator Options
    public static CustomHeaderOption Dictator;
    public static CustomToggleOption UniqueDictator;
    public static CustomToggleOption RoundOneNoDictReveal;
    public static CustomToggleOption DictatorButton;
    public static CustomToggleOption DictateAfterVoting;

    // Monarch Options
    public static CustomHeaderOption Monarch;
    public static CustomToggleOption UniqueMonarch;
    public static CustomNumberOption KnightVoteCount;
    public static CustomNumberOption KnightCount;
    public static CustomNumberOption KnightingCd;
    public static CustomToggleOption RoundOneNoKnighting;
    public static CustomToggleOption KnightButton;
    public static CustomToggleOption MonarchButton;

    // Trapper Options
    public static CustomHeaderOption Trapper;
    public static CustomToggleOption UniqueTrapper;
    public static CustomNumberOption BuildCd;
    public static CustomNumberOption BuildDur;
    public static CustomNumberOption TrapCd;
    public static CustomNumberOption MaxTraps;

    // CA Options
    public static CustomHeaderOption CASettings;
    public static CustomNumberOption CAMax;

    // Mystic Options
    public static CustomHeaderOption Mystic;
    public static CustomToggleOption UniqueMystic;
    public static CustomNumberOption MysticRevealCd;

    // Vampire Hunter Options
    public static CustomHeaderOption VampireHunter;
    public static CustomToggleOption UniqueVampireHunter;
    public static CustomNumberOption StakeCd;

    // CK Options
    public static CustomHeaderOption CKSettings;
    public static CustomNumberOption CKMax;

    // Vigilante Options
    public static CustomHeaderOption Vigilante;
    public static CustomToggleOption UniqueVigilante;
    public static CustomStringOption VigiOptions;
    public static CustomStringOption VigiNotifOptions;
    public static CustomToggleOption MisfireKillsInno;
    public static CustomToggleOption VigiKillAgain;
    public static CustomToggleOption RoundOneNoShot;
    public static CustomNumberOption ShootCd;
    public static CustomNumberOption MaxBullets;

    // Veteran Options
    public static CustomHeaderOption Veteran;
    public static CustomToggleOption UniqueVeteran;
    public static CustomNumberOption AlertCd;
    public static CustomNumberOption AlertDur;
    public static CustomNumberOption MaxAlerts;

    // Bastion Options
    public static CustomHeaderOption Bastion;
    public static CustomToggleOption UniqueBastion;
    public static CustomToggleOption BombRemovedOnKill;
    public static CustomNumberOption BastionCd;
    public static CustomNumberOption MaxBombs;

    // CS Options
    public static CustomHeaderOption CSSettings;
    public static CustomNumberOption CSMax;

    // Engineer Options
    public static CustomHeaderOption Engineer;
    public static CustomNumberOption MaxFixes;
    public static CustomToggleOption UniqueEngineer;
    public static CustomNumberOption FixCd;

    // Transporter Options
    public static CustomHeaderOption Transporter;
    public static CustomToggleOption UniqueTransporter;
    public static CustomToggleOption TransSelf;
    public static CustomNumberOption TransportCd;
    public static CustomNumberOption TransportDur;
    public static CustomNumberOption MaxTransports;

    // Retributionist Options
    public static CustomHeaderOption Retributionist;
    public static CustomToggleOption UniqueRetributionist;
    public static CustomToggleOption ReviveAfterVoting;

    // Escort Options
    public static CustomHeaderOption Escort;
    public static CustomToggleOption UniqueEscort;
    public static CustomNumberOption EscortCd;
    public static CustomNumberOption EscortDur;

    // Chameleon Options
    public static CustomHeaderOption Chameleon;
    public static CustomToggleOption UniqueChameleon;
    public static CustomNumberOption MaxSwoops;
    public static CustomNumberOption SwoopCd;
    public static CustomNumberOption SwoopDur;

    // Shifter Options
    public static CustomHeaderOption Shifter;
    public static CustomToggleOption UniqueShifter;
    public static CustomNumberOption ShiftCd;
    public static CustomStringOption ShiftedBecomes;

    // CI Options
    public static CustomHeaderOption CISettings;
    public static CustomNumberOption CIMax;

    // Tracker Options
    public static CustomHeaderOption Tracker;
    public static CustomToggleOption UniqueTracker;
    public static CustomNumberOption UpdateInterval;
    public static CustomNumberOption TrackCd;
    public static CustomToggleOption ResetOnNewRound;
    public static CustomNumberOption MaxTracks;

    // Operative Options
    public static CustomHeaderOption Operative;
    public static CustomToggleOption UniqueOperative;
    public static CustomNumberOption BugCd;
    public static CustomToggleOption BugsRemoveOnNewRound;
    public static CustomNumberOption MaxBugs;
    public static CustomNumberOption MinAmountOfTimeInBug;
    public static CustomNumberOption BugRange;
    public static CustomNumberOption MinAmountOfPlayersInBug;
    public static CustomStringOption WhoSeesDead;
    public static CustomToggleOption PreciseOperativeInfo;

    // Seer Options
    public static CustomHeaderOption Seer;
    public static CustomToggleOption UniqueSeer;
    public static CustomNumberOption SeerCd;

    // Detective Options
    public static CustomHeaderOption Detective;
    public static CustomToggleOption UniqueDetective;
    public static CustomNumberOption ExamineCd;
    public static CustomNumberOption RecentKill;
    public static CustomNumberOption FootprintInterval;
    public static CustomNumberOption FootprintDur;
    public static CustomStringOption AnonymousFootPrint;

    // Coroner Options
    public static CustomHeaderOption Coroner;
    public static CustomToggleOption UniqueCoroner;
    public static CustomNumberOption CoronerArrowDur;
    public static CustomToggleOption CoronerReportName;
    public static CustomToggleOption CoronerReportRole;
    public static CustomNumberOption CoronerKillerNameTime;
    public static CustomNumberOption CompareCd;
    public static CustomNumberOption AutopsyCd;

    // Medium Options
    public static CustomHeaderOption Medium;
    public static CustomToggleOption UniqueMedium;
    public static CustomNumberOption MediateCd;
    public static CustomToggleOption ShowMediatePlayer;
    public static CustomStringOption ShowMediumToDead;
    public static CustomStringOption DeadRevealed;

    // Sheriff Options
    public static CustomHeaderOption Sheriff;
    public static CustomToggleOption UniqueSheriff;
    public static CustomNumberOption InterrogateCd;
    public static CustomToggleOption NeutEvilRed;
    public static CustomToggleOption NeutKillingRed;

    // CrP Options
    public static CustomHeaderOption CrPSettings;
    public static CustomNumberOption CrPMax;

    // Altruist Options
    public static CustomHeaderOption Altruist;
    public static CustomToggleOption UniqueAltruist;
    public static CustomNumberOption ReviveDur;
    public static CustomToggleOption AltruistTargetBody;
    public static CustomNumberOption ReviveCd;
    public static CustomNumberOption MaxRevives;

    // Medic Options
    public static CustomHeaderOption Medic;
    public static CustomToggleOption UniqueMedic;
    public static CustomStringOption ShowShielded;
    public static CustomStringOption WhoGetsNotification;
    public static CustomToggleOption ShieldBreaks;

    // CU Options
    public static CustomHeaderOption CUSettings;

    // Revealer Options
    public static CustomHeaderOption Revealer;
    public static CustomNumberOption RevealerTasksRemainingClicked;
    public static CustomNumberOption RevealerTasksRemainingAlert;
    public static CustomToggleOption RevealerRevealsNeutrals;
    public static CustomToggleOption RevealerRevealsCrew;
    public static CustomToggleOption RevealerRevealsRoles;
    public static CustomStringOption RevealerCanBeClickedBy;

    // Intruder Options
    public static CustomHeaderOption IntruderSettings;
    public static CustomNumberOption IntruderVision;
    public static CustomToggleOption IntrudersVent;
    public static CustomToggleOption IntrudersCanSabotage;
    public static CustomNumberOption IntKillCd;
    public static CustomNumberOption IntruderSabotageCooldown;
    public static CustomNumberOption IntruderCount;
    public static CustomNumberOption IntruderMax;
    public static CustomNumberOption IntruderMin;
    public static CustomToggleOption GhostsCanSabotage;
    public static CustomToggleOption IntruderFlashlight;

    // IC Options
    public static CustomHeaderOption ICSettings;
    public static CustomNumberOption ICMax;

    // Janitor Options
    public static CustomHeaderOption Janitor;
    public static CustomToggleOption UniqueJanitor;
    public static CustomNumberOption CleanCd;
    public static CustomToggleOption JaniCooldownsLinked;
    public static CustomToggleOption SoloBoost;
    public static CustomNumberOption DragCd;
    public static CustomStringOption JanitorVentOptions;
    public static CustomNumberOption DragModifier;

    // Blackmailer Options
    public static CustomHeaderOption Blackmailer;
    public static CustomToggleOption UniqueBlackmailer;
    public static CustomNumberOption BlackmailCd;
    public static CustomToggleOption WhispersNotPrivate;
    public static CustomToggleOption BlackmailMates;
    public static CustomToggleOption BMRevealed;

    // Grenadier Options
    public static CustomHeaderOption Grenadier;
    public static CustomToggleOption UniqueGrenadier;
    public static CustomNumberOption FlashCd;
    public static CustomNumberOption FlashDur;
    public static CustomToggleOption GrenadierIndicators;
    public static CustomToggleOption GrenadierVent;
    public static CustomToggleOption SaboFlash;
    public static CustomNumberOption FlashRadius;

    // Camouflager Options
    public static CustomHeaderOption Camouflager;
    public static CustomToggleOption UniqueCamouflager;
    public static CustomNumberOption CamouflageCd;
    public static CustomNumberOption CamouflageDur;
    public static CustomToggleOption CamoHideSpeed;
    public static CustomToggleOption CamoHideSize;

    // ID Options
    public static CustomHeaderOption IDSettings;
    public static CustomNumberOption IDMax;

    // Morphling Options
    public static CustomHeaderOption Morphling;
    public static CustomToggleOption UniqueMorphling;
    public static CustomNumberOption MorphCd;
    public static CustomNumberOption MorphDur;
    public static CustomToggleOption MorphlingVent;
    public static CustomToggleOption MorphCooldownsLinked;
    public static CustomNumberOption SampleCd;

    // Disguiser Options
    public static CustomHeaderOption Disguiser;
    public static CustomToggleOption UniqueDisguiser;
    public static CustomNumberOption DisguiseCd;
    public static CustomNumberOption DisguiseDelay;
    public static CustomNumberOption DisguiseDur;
    public static CustomStringOption DisguiseTarget;
    public static CustomToggleOption DisgCooldownsLinked;
    public static CustomNumberOption MeasureCd;

    // Wraith Options
    public static CustomHeaderOption Wraith;
    public static CustomToggleOption UniqueWraith;
    public static CustomNumberOption InvisCd;
    public static CustomNumberOption InvisDur;
    public static CustomToggleOption WraithVent;

    // IH Options
    public static CustomHeaderOption IHSettings;
    public static CustomNumberOption IHMax;

    // Godfather Options
    public static CustomHeaderOption Godfather;
    public static CustomToggleOption UniqueGodfather;
    public static CustomNumberOption GFPromotionCdDecrease;

    // IS Options
    public static CustomHeaderOption ISSettings;
    public static CustomNumberOption ISMax;

    // Teleporter Options
    public static CustomHeaderOption Teleporter;
    public static CustomToggleOption UniqueTeleporter;
    public static CustomNumberOption TeleportCd;
    public static CustomNumberOption TeleMarkCd;
    public static CustomToggleOption TeleVent;
    public static CustomToggleOption TeleCooldownsLinked;

    // Consigliere Options
    public static CustomHeaderOption Consigliere;
    public static CustomToggleOption UniqueConsigliere;
    public static CustomNumberOption InvestigateCd;
    public static CustomStringOption ConsigInfo;

    // Consort Options
    public static CustomHeaderOption Consort;
    public static CustomToggleOption UniqueConsort;
    public static CustomNumberOption ConsortCd;
    public static CustomNumberOption ConsortDur;

    // Miner Options
    public static CustomHeaderOption Miner;
    public static CustomToggleOption UniqueMiner;
    public static CustomNumberOption MineCd;

    // IK Options
    public static CustomHeaderOption IKSettings;
    public static CustomNumberOption IKMax;

    // Ambusher Options
    public static CustomHeaderOption Ambusher;
    public static CustomToggleOption UniqueAmbusher;
    public static CustomNumberOption AmbushCd;
    public static CustomNumberOption AmbushDur;
    public static CustomToggleOption AmbushMates;

    // Enforcer Options
    public static CustomHeaderOption Enforcer;
    public static CustomToggleOption UniqueEnforcer;
    public static CustomNumberOption EnforceCd;
    public static CustomNumberOption EnforceDur;
    public static CustomNumberOption EnforceDelay;
    public static CustomNumberOption EnforceRadius;

    // IU Options
    public static CustomHeaderOption IUSettings;

    // Ghoul Options
    public static CustomHeaderOption Ghoul;
    public static CustomNumberOption GhoulMarkCd;

    // Syndicate Options
    public static CustomHeaderOption SyndicateSettings;
    public static CustomNumberOption SyndicateVision;
    public static CustomStringOption SyndicateVent;
    public static CustomNumberOption ChaosDriveMeetingCount;
    public static CustomNumberOption CDKillCd;
    public static CustomNumberOption SyndicateCount;
    public static CustomToggleOption AltImps;
    public static CustomToggleOption GlobalDrive;
    public static CustomNumberOption SyndicateMax;
    public static CustomNumberOption SyndicateMin;
    public static CustomToggleOption SyndicateFlashlight;

    // SD Options
    public static CustomHeaderOption SDSettings;
    public static CustomNumberOption SDMax;

    // Shapeshifter Options
    public static CustomHeaderOption Shapeshifter;
    public static CustomToggleOption UniqueShapeshifter;
    public static CustomNumberOption ShapeshiftCd;
    public static CustomNumberOption ShapeshiftDur;
    public static CustomToggleOption ShapeshiftMates;

    // Drunkard Options
    public static CustomHeaderOption Drunkard;
    public static CustomToggleOption UniqueDrunkard;
    public static CustomNumberOption ConfuseCd;
    public static CustomNumberOption ConfuseDur;
    public static CustomToggleOption ConfuseImmunity;

    // Concealer Options
    public static CustomHeaderOption Concealer;
    public static CustomToggleOption UniqueConcealer;
    public static CustomNumberOption ConcealCd;
    public static CustomNumberOption ConcealDur;
    public static CustomToggleOption ConcealMates;

    // Silencer Options
    public static CustomHeaderOption Silencer;
    public static CustomToggleOption UniqueSilencer;
    public static CustomNumberOption SilenceCd;
    public static CustomToggleOption WhispersNotPrivateSilencer;
    public static CustomToggleOption SilenceMates;
    public static CustomToggleOption SilenceRevealed;

    // Framer Options
    public static CustomHeaderOption Framer;
    public static CustomNumberOption FrameCd;
    public static CustomNumberOption ChaosDriveFrameRadius;
    public static CustomToggleOption UniqueFramer;

    // SyK Options
    public static CustomHeaderOption SyKSettings;
    public static CustomNumberOption SyKMax;

    // Crusader Options
    public static CustomHeaderOption Crusader;
    public static CustomToggleOption UniqueCrusader;
    public static CustomNumberOption CrusadeCd;
    public static CustomNumberOption CrusadeDur;
    public static CustomNumberOption ChaosDriveCrusadeRadius;
    public static CustomToggleOption CrusadeMates;

    // Bomber Options
    public static CustomHeaderOption Bomber;
    public static CustomToggleOption UniqueBomber;
    public static CustomNumberOption BombCd;
    public static CustomNumberOption DetonateCd;
    public static CustomToggleOption BombCooldownsLinked;
    public static CustomToggleOption BombsRemoveOnNewRound;
    public static CustomToggleOption BombsDetonateOnMeetingStart;
    public static CustomNumberOption BombRange;
    public static CustomNumberOption ChaosDriveBombRange;

    // Poisoner Options
    public static CustomHeaderOption Poisoner;
    public static CustomToggleOption UniquePoisoner;
    public static CustomNumberOption PoisonCd;
    public static CustomNumberOption PoisonDur;

    // Collider Options
    public static CustomHeaderOption Collider;
    public static CustomToggleOption UniqueCollider;
    public static CustomNumberOption CollideCd;
    public static CustomNumberOption ChargeCd;
    public static CustomNumberOption ChargeDur;
    public static CustomNumberOption CollideRange;
    public static CustomNumberOption CollideRangeIncrease;
    public static CustomToggleOption ChargeCooldownsLinked;
    public static CustomToggleOption CollideResetsCooldown;

    // SSu Options
    public static CustomHeaderOption SSuSettings;
    public static CustomNumberOption SSuMax;

    // Rebel Options
    public static CustomHeaderOption Rebel;
    public static CustomToggleOption UniqueRebel;
    public static CustomNumberOption RebPromotionCdDecrease;

    // Stalker Options
    public static CustomHeaderOption Stalker;
    public static CustomToggleOption UniqueStalker;
    public static CustomNumberOption StalkCd;

    // Warper Options
    public static CustomHeaderOption Warper;
    public static CustomNumberOption WarpCd;
    public static CustomNumberOption WarpDur;
    public static CustomToggleOption UniqueWarper;
    public static CustomToggleOption WarpSelf;

    // SU Options
    public static CustomHeaderOption SUSettings;

    // Banshee Options
    public static CustomHeaderOption Banshee;
    public static CustomNumberOption ScreamCd;
    public static CustomNumberOption ScreamDur;

    // SP Options
    public static CustomHeaderOption SPSettings;
    public static CustomNumberOption SPMax;

    // Spellslinger Options
    public static CustomHeaderOption Spellslinger;
    public static CustomNumberOption SpellCd;
    public static CustomNumberOption SpellCdIncrease;
    public static CustomToggleOption UniqueSpellslinger;

    // Timekeeper Options
    public static CustomHeaderOption Timekeeper;
    public static CustomToggleOption UniqueTimekeeper;
    public static CustomNumberOption TimeCd;
    public static CustomNumberOption TimeDur;
    public static CustomToggleOption TimeFreezeImmunity;
    public static CustomToggleOption TimeRewindImmunity;

    // Neutral Options
    public static CustomHeaderOption NeutralSettings;
    public static CustomNumberOption NeutralVision;
    public static CustomToggleOption LightsAffectNeutrals;
    public static CustomStringOption NoSolo;
    public static CustomNumberOption NeutralMax;
    public static CustomNumberOption NeutralMin;
    public static CustomToggleOption NeutralsVent;
    public static CustomToggleOption AvoidNeutralKingmakers;
    public static CustomToggleOption NeutralFlashlight;

    // NA Options
    public static CustomHeaderOption NASettings;

    // Pestilence Options
    public static CustomHeaderOption Pestilence;
    public static CustomToggleOption PestSpawn;
    public static CustomToggleOption PlayersAlerted;
    public static CustomNumberOption ObliterateCd;
    public static CustomToggleOption PestVent;

    // NB Options
    public static CustomHeaderOption NBSettings;
    public static CustomNumberOption NBMax;
    public static CustomToggleOption VigiKillsNB;

    // Amnesiac Options
    public static CustomHeaderOption Amnesiac;
    public static CustomToggleOption RememberArrows;
    public static CustomNumberOption RememberArrowDelay;
    public static CustomToggleOption AmneVent;
    public static CustomToggleOption AmneSwitchVent;
    public static CustomToggleOption UniqueAmnesiac;
    public static CustomToggleOption AmneToThief;

    // Survivor Options
    public static CustomHeaderOption Survivor;
    public static CustomNumberOption VestCd;
    public static CustomNumberOption VestDur;
    public static CustomToggleOption SurvVent;
    public static CustomToggleOption SurvSwitchVent;
    public static CustomNumberOption MaxVests;
    public static CustomToggleOption UniqueSurvivor;

    // Guardian Angel Options
    public static CustomHeaderOption GuardianAngel;
    public static CustomNumberOption ProtectCd;
    public static CustomNumberOption ProtectDur;
    public static CustomNumberOption MaxProtects;
    public static CustomStringOption ShowProtect;
    public static CustomStringOption GaOnTargetDeath;
    public static CustomToggleOption GATargetKnows;
    public static CustomToggleOption ProtectBeyondTheGrave;
    public static CustomToggleOption GAKnowsTargetRole;
    public static CustomToggleOption GAVent;
    public static CustomToggleOption GASwitchVent;
    public static CustomToggleOption UniqueGuardianAngel;
    public static CustomToggleOption GuardianAngelCanPickTargets;
    public static CustomToggleOption GAToSurv;

    // Thief Options
    public static CustomHeaderOption Thief;
    public static CustomToggleOption ThiefVent;
    public static CustomNumberOption StealCd;
    public static CustomToggleOption UniqueThief;
    public static CustomToggleOption ThiefSteals;
    public static CustomToggleOption ThiefCanGuess;
    public static CustomToggleOption ThiefCanGuessAfterVoting;

    // NE Options
    public static CustomHeaderOption NESettings;
    public static CustomNumberOption NEMax;
    public static CustomToggleOption NeutralEvilsEndGame;

    // Jester Options
    public static CustomHeaderOption Jester;
    public static CustomToggleOption JesterButton;
    public static CustomToggleOption JesterVent;
    public static CustomToggleOption JestSwitchVent;
    public static CustomToggleOption JestEjectScreen;
    public static CustomToggleOption VigiKillsJester;
    public static CustomToggleOption UniqueJester;

    // Actor Options
    public static CustomHeaderOption Actor;
    public static CustomToggleOption ActorButton;
    public static CustomToggleOption ActorVent;
    public static CustomToggleOption ActSwitchVent;
    public static CustomToggleOption VigiKillsActor;
    public static CustomToggleOption UniqueActor;
    public static CustomToggleOption ActorCanPickRole;
    public static CustomNumberOption ActorRoleCount;

    // Troll Options
    public static CustomHeaderOption Troll;
    public static CustomNumberOption InteractCd;
    public static CustomToggleOption TrollVent;
    public static CustomToggleOption TrollSwitchVent;
    public static CustomToggleOption UniqueTroll;

    // Cannibal Options
    public static CustomHeaderOption Cannibal;
    public static CustomNumberOption EatCd;
    public static CustomNumberOption BodiesNeeded;
    public static CustomToggleOption CannibalVent;
    public static CustomToggleOption EatArrows;
    public static CustomNumberOption EatArrowDelay;
    public static CustomToggleOption VigiKillsCannibal;
    public static CustomToggleOption UniqueCannibal;

    // Executioner Options
    public static CustomHeaderOption Executioner;
    public static CustomStringOption OnTargetDead;
    public static CustomToggleOption ExecutionerButton;
    public static CustomToggleOption ExeVent;
    public static CustomToggleOption ExeSwitchVent;
    public static CustomToggleOption ExeTargetKnows;
    public static CustomToggleOption ExeKnowsTargetRole;
    public static CustomToggleOption ExeEjectScreen;
    public static CustomToggleOption ExeCanWinBeyondDeath;
    public static CustomToggleOption VigiKillsExecutioner;
    public static CustomToggleOption UniqueExecutioner;
    public static CustomToggleOption ExecutionerCanPickTargets;
    public static CustomToggleOption ExeToJest;

    // Bounty Hunter Options
    public static CustomHeaderOption BountyHunter;
    public static CustomToggleOption BHVent;
    public static CustomNumberOption GuessCd;
    public static CustomNumberOption BountyHunterGuesses;
    public static CustomToggleOption UniqueBountyHunter;
    public static CustomToggleOption VigiKillsBH;
    public static CustomToggleOption BountyHunterCanPickTargets;
    public static CustomToggleOption BHToTroll;

    // Guesser Options
    public static CustomHeaderOption Guesser;
    public static CustomStringOption OnTargetGone;
    public static CustomToggleOption GuesserButton;
    public static CustomToggleOption GuessVent;
    public static CustomToggleOption GuessSwitchVent;
    public static CustomToggleOption GuessTargetKnows;
    public static CustomToggleOption VigiKillsGuesser;
    public static CustomToggleOption UniqueGuesser;
    public static CustomNumberOption MaxGuesses;
    public static CustomToggleOption GuesserAfterVoting;
    public static CustomToggleOption MultipleGuesses;
    public static CustomToggleOption GuesserCanPickTargets;
    public static CustomToggleOption GuessToAct;

    // NH Options
    public static CustomHeaderOption NHSettings;
    public static CustomNumberOption NHMax;

    // Plaguebearer Options
    public static CustomHeaderOption Plaguebearer;
    public static CustomNumberOption InfectCd;
    public static CustomToggleOption PBVent;
    public static CustomToggleOption UniquePlaguebearer;

    // NK Options
    public static CustomHeaderOption NKSettings;
    public static CustomNumberOption NKMax;
    public static CustomToggleOption NKHasImpVision;
    public static CustomToggleOption NKsKnow;

    // Glitch Options
    public static CustomHeaderOption Glitch;
    public static CustomNumberOption HackCd;
    public static CustomNumberOption MimicCd;
    public static CustomNumberOption MimicDur;
    public static CustomNumberOption HackDur;
    public static CustomNumberOption NeutraliseCd;
    public static CustomToggleOption GlitchVent;
    public static CustomToggleOption UniqueGlitch;

    // Juggernaut Options
    public static CustomHeaderOption Juggernaut;
    public static CustomToggleOption JuggVent;
    public static CustomNumberOption AssaultCd;
    public static CustomNumberOption AssaultBonus;
    public static CustomToggleOption UniqueJuggernaut;

    // Cryomaniac Options
    public static CustomHeaderOption Cryomaniac;
    public static CustomNumberOption CryoDouseCd;
    public static CustomNumberOption CryoKillCd;
    public static CustomToggleOption CryoVent;
    public static CustomToggleOption UniqueCryomaniac;
    public static CustomToggleOption CryoFreezeAll;
    public static CustomToggleOption CryoLastKillerBoost;

    // Arsonist Options
    public static CustomHeaderOption Arsonist;
    public static CustomNumberOption ArsoDouseCd;
    public static CustomNumberOption IgniteCd;
    public static CustomToggleOption ArsoVent;
    public static CustomToggleOption ArsoIgniteAll;
    public static CustomToggleOption ArsoLastKillerBoost;
    public static CustomToggleOption ArsoCooldownsLinked;
    public static CustomToggleOption UniqueArsonist;
    public static CustomToggleOption IgnitionCremates;

    // Murderer Options
    public static CustomHeaderOption Murderer;
    public static CustomToggleOption MurdVent;
    public static CustomNumberOption MurderCd;
    public static CustomToggleOption UniqueMurderer;

    // Serial Killer Options
    public static CustomHeaderOption SerialKiller;
    public static CustomNumberOption BloodlustCd;
    public static CustomNumberOption BloodlustDur;
    public static CustomNumberOption StabCd;
    public static CustomStringOption SKVentOptions;
    public static CustomToggleOption UniqueSerialKiller;

    // Werewolf Options
    public static CustomHeaderOption Werewolf;
    public static CustomNumberOption MaulCd;
    public static CustomNumberOption MaulRadius;
    public static CustomStringOption WerewolfVent;
    public static CustomToggleOption CanStillAttack;
    public static CustomToggleOption UniqueWerewolf;

    // NN Options
    public static CustomHeaderOption NNSettings;
    public static CustomNumberOption NNMax;
    public static CustomToggleOption NNHasImpVision;

    // Dracula Options
    public static CustomHeaderOption Dracula;
    public static CustomNumberOption BiteCd;
    public static CustomNumberOption AliveVampCount;
    public static CustomToggleOption DracVent;
    public static CustomToggleOption UniqueDracula;
    public static CustomToggleOption UndeadVent;

    // Necromancer Options
    public static CustomHeaderOption Necromancer;
    public static CustomNumberOption ResurrectCd;
    public static CustomNumberOption NecroKillCd;
    public static CustomNumberOption NecroKillCdIncrease;
    public static CustomToggleOption NecroKillCdIncreases;
    public static CustomNumberOption MaxNecroKills;
    public static CustomNumberOption ResurrectCdIncrease;
    public static CustomToggleOption ResurrectCdIncreases;
    public static CustomNumberOption MaxResurrections;
    public static CustomToggleOption NecroVent;
    public static CustomToggleOption ResurrectVent;
    public static CustomToggleOption NecroCooldownsLinked;
    public static CustomToggleOption NecromancerTargetBody;
    public static CustomNumberOption ResurrectDur;
    public static CustomToggleOption UniqueNecromancer;

    // Whisperer Options
    public static CustomHeaderOption Whisperer;
    public static CustomNumberOption WhisperCd;
    public static CustomNumberOption WhisperRadius;
    public static CustomNumberOption WhisperCdIncrease;
    public static CustomToggleOption WhisperCdIncreases;
    public static CustomNumberOption WhisperRateDecrease;
    public static CustomNumberOption WhisperRate;
    public static CustomToggleOption WhisperRateDecreases;
    public static CustomToggleOption WhispVent;
    public static CustomToggleOption UniqueWhisperer;
    public static CustomToggleOption PersuadedVent;

    // Jackal Options
    public static CustomHeaderOption Jackal;
    public static CustomNumberOption RecruitCd;
    public static CustomToggleOption JackalVent;
    public static CustomToggleOption RecruitVent;
    public static CustomToggleOption UniqueJackal;

    // NP Options
    public static CustomHeaderOption NPSettings;

    // Phantom Options
    public static CustomHeaderOption Phantom;
    public static CustomNumberOption PhantomTasksRemaining;
    public static CustomToggleOption PhantomPlayersAlerted;

    // Betrayer Options
    public static CustomHeaderOption Betrayer;
    public static CustomNumberOption BetrayCd;
    public static CustomToggleOption BetrayerVent;

    // Ability Options
    public static CustomHeaderOption AbilitySettings;
    public static CustomNumberOption MaxAbilities;
    public static CustomNumberOption MinAbilities;

    // Snitch Options
    public static CustomHeaderOption Snitch;
    public static CustomToggleOption SnitchSeesNeutrals;
    public static CustomToggleOption SnitchSeesCrew;
    public static CustomToggleOption SnitchSeesRoles;
    public static CustomNumberOption SnitchTasksRemaining;
    public static CustomToggleOption SnitchSeestargetsInMeeting;
    public static CustomToggleOption SnitchKnows;
    public static CustomToggleOption UniqueSnitch;

    // Assassin Options
    public static CustomHeaderOption Assassin;
    public static CustomNumberOption AssassinKills;
    public static CustomToggleOption AssassinMultiKill;
    public static CustomToggleOption AssassinGuessNeutralBenign;
    public static CustomToggleOption AssassinGuessNeutralEvil;
    public static CustomToggleOption AssassinGuessPest;
    public static CustomToggleOption AssassinPenalised;
    public static CustomToggleOption AssassinGuessModifiers;
    public static CustomToggleOption AssassinGuessObjectifiers;
    public static CustomToggleOption AssassinGuessAbilities;
    public static CustomToggleOption AssassinGuessInvestigative;
    public static CustomToggleOption AssassinateAfterVoting;
    public static CustomToggleOption UniqueCrewAssassin;
    public static CustomToggleOption UniqueNeutralAssassin;
    public static CustomToggleOption UniqueIntruderAssassin;
    public static CustomToggleOption UniqueSyndicateAssassin;
    public static CustomToggleOption AssassinNotification;

    // Underdog Options
    public static CustomHeaderOption Underdog;
    public static CustomToggleOption UniqueUnderdog;
    public static CustomToggleOption UnderdogKnows;
    public static CustomNumberOption UnderdogKillBonus;
    public static CustomToggleOption UnderdogIncreasedKC;

    // Multitasker Options
    public static CustomHeaderOption Multitasker;
    public static CustomToggleOption UniqueMultitasker;
    public static CustomNumberOption Transparancy;

    // Button Barry Options
    public static CustomHeaderOption ButtonBarry;
    public static CustomToggleOption UniqueButtonBarry;
    public static CustomNumberOption ButtonCooldown;

    // Swapper Options
    public static CustomHeaderOption Swapper;
    public static CustomToggleOption UniqueSwapper;
    public static CustomToggleOption SwapperButton;
    public static CustomToggleOption SwapAfterVoting;
    public static CustomToggleOption SwapSelf;

    // Politician Options
    public static CustomHeaderOption Politician;
    public static CustomToggleOption UniquePolitician;
    public static CustomNumberOption PoliticianVoteBank;
    public static CustomToggleOption PoliticianButton;

    // Tiebreaker Options
    public static CustomHeaderOption Tiebreaker;
    public static CustomToggleOption UniqueTiebreaker;
    public static CustomToggleOption TiebreakerKnows;

    // Torch Options
    public static CustomHeaderOption Torch;
    public static CustomToggleOption UniqueTorch;

    // Tunneler Options
    public static CustomHeaderOption Tunneler;
    public static CustomToggleOption TunnelerKnows;
    public static CustomToggleOption UniqueTunneler;

    // Radar Options
    public static CustomHeaderOption Radar;
    public static CustomToggleOption UniqueRadar;

    // Insider Options
    public static CustomHeaderOption Insider;
    public static CustomToggleOption InsiderKnows;
    public static CustomToggleOption UniqueInsider;

    // Ruthless Options
    public static CustomHeaderOption Ruthless;
    public static CustomToggleOption UniqueRuthless;
    public static CustomToggleOption RuthlessKnows;

    // Ninja Options
    public static CustomHeaderOption Ninja;
    public static CustomToggleOption UniqueNinja;

    // Objectifier Options
    public static CustomHeaderOption ObjectifierSettings;
    public static CustomNumberOption MaxObjectifiers;
    public static CustomNumberOption MinObjectifiers;

    // Traitor Options
    public static CustomHeaderOption Traitor;
    public static CustomToggleOption UniqueTraitor;
    public static CustomToggleOption TraitorKnows;
    public static CustomToggleOption TraitorColourSwap;
    public static CustomToggleOption SnitchSeesTraitor;
    public static CustomToggleOption RevealerRevealsTraitor;

    // Fanatic Options
    public static CustomHeaderOption Fanatic;
    public static CustomToggleOption FanaticKnows;
    public static CustomToggleOption UniqueFanatic;
    public static CustomToggleOption FanaticColourSwap;
    public static CustomToggleOption SnitchSeesFanatic;
    public static CustomToggleOption RevealerRevealsFanatic;

    // Allied Options
    public static CustomHeaderOption Allied;
    public static CustomStringOption AlliedFaction;
    public static CustomToggleOption UniqueAllied;

    // Corrupted Options
    public static CustomHeaderOption Corrupted;
    public static CustomNumberOption CorruptCd;
    public static CustomToggleOption UniqueCorrupted;
    public static CustomToggleOption AllCorruptedWin;
    public static CustomToggleOption CorruptedVent;

    // Corrupted Options
    public static CustomHeaderOption Overlord;
    public static CustomNumberOption OverlordMeetingWinCount;
    public static CustomToggleOption UniqueOverlord;
    public static CustomToggleOption OverlordKnows;

    // Linked Options
    public static CustomHeaderOption Linked;
    public static CustomToggleOption UniqueLinked;
    public static CustomToggleOption LinkedChat;
    public static CustomToggleOption LinkedRoles;

    // Lovers Options
    public static CustomHeaderOption Lovers;
    public static CustomToggleOption BothLoversDie;
    public static CustomToggleOption LoversChat;
    public static CustomToggleOption LoversRoles;
    public static CustomToggleOption UniqueLovers;

    // Mafia Options
    public static CustomHeaderOption Mafia;
    public static CustomToggleOption MafiaRoles;
    public static CustomToggleOption UniqueMafia;
    public static CustomToggleOption MafVent;

    // Rivals Options
    public static CustomHeaderOption Rivals;
    public static CustomToggleOption RivalsChat;
    public static CustomToggleOption RivalsRoles;
    public static CustomToggleOption UniqueRivals;

    // Taskmaster Options
    public static CustomHeaderOption Taskmaster;
    public static CustomNumberOption TMTasksRemaining;
    public static CustomToggleOption UniqueTaskmaster;

    // Defector Options
    public static CustomHeaderOption Defector;
    public static CustomToggleOption UniqueDefector;
    public static CustomToggleOption DefectorKnows;
    public static CustomStringOption DefectorFaction;

    // Modifier Options
    public static CustomHeaderOption ModifierSettings;
    public static CustomNumberOption MaxModifiers;
    public static CustomNumberOption MinModifiers;

    // Giant Options
    public static CustomHeaderOption Giant;
    public static CustomToggleOption UniqueGiant;
    public static CustomNumberOption GiantSpeed;
    public static CustomNumberOption GiantScale;

    // Dwarf Options
    public static CustomHeaderOption Dwarf;
    public static CustomNumberOption DwarfSpeed;
    public static CustomNumberOption DwarfScale;
    public static CustomToggleOption UniqueDwarf;

    // Diseased Options
    public static CustomHeaderOption Diseased;
    public static CustomNumberOption DiseasedKillMultiplier;
    public static CustomToggleOption DiseasedKnows;
    public static CustomToggleOption UniqueDiseased;

    // Bait Options
    public static CustomHeaderOption Bait;
    public static CustomNumberOption BaitMinDelay;
    public static CustomNumberOption BaitMaxDelay;
    public static CustomToggleOption BaitKnows;
    public static CustomToggleOption UniqueBait;

    // Drunk Options
    public static CustomHeaderOption Drunk;
    public static CustomToggleOption DrunkKnows;
    public static CustomToggleOption DrunkControlsSwap;
    public static CustomNumberOption DrunkInterval;
    public static CustomToggleOption UniqueDrunk;

    // Coward Options
    public static CustomHeaderOption Coward;
    public static CustomToggleOption UniqueCoward;

    // Professional Options
    public static CustomHeaderOption Professional;
    public static CustomToggleOption ProfessionalKnows;
    public static CustomToggleOption UniqueProfessional;

    // Shy Options
    public static CustomHeaderOption Shy;
    public static CustomToggleOption UniqueShy;

    // Colorblind Options
    public static CustomHeaderOption Colorblind;
    public static CustomToggleOption UniqueColorblind;

    // Astral Options
    public static CustomHeaderOption Astral;
    public static CustomToggleOption UniqueAstral;

    // Yeller Options
    public static CustomHeaderOption Yeller;
    public static CustomToggleOption UniqueYeller;

    // Indomitable Options
    public static CustomHeaderOption Indomitable;
    public static CustomToggleOption UniqueIndomitable;
    public static CustomToggleOption IndomitableKnows;

    // VIP Options
    public static CustomHeaderOption VIP;
    public static CustomToggleOption UniqueVIP;
    public static CustomToggleOption VIPKnows;

    // Volatile Options
    public static CustomHeaderOption Volatile;
    public static CustomNumberOption VolatileInterval;
    public static CustomToggleOption VolatileKnows;
    public static CustomToggleOption UniqueVolatile;

    // Role List Entry Options
    public static CustomHeaderOption RoleList;
    public static RoleListEntryOption Entry1;
    public static RoleListEntryOption Entry2;
    public static RoleListEntryOption Entry3;
    public static RoleListEntryOption Entry4;
    public static RoleListEntryOption Entry5;
    public static RoleListEntryOption Entry6;
    public static RoleListEntryOption Entry7;
    public static RoleListEntryOption Entry8;
    public static RoleListEntryOption Entry9;
    public static RoleListEntryOption Entry10;
    public static RoleListEntryOption Entry11;
    public static RoleListEntryOption Entry12;
    public static RoleListEntryOption Entry13;
    public static RoleListEntryOption Entry14;
    public static RoleListEntryOption Entry15;

    // Role List Ban Options
    public static CustomHeaderOption BanList;
    public static RoleListEntryOption Ban1;
    public static RoleListEntryOption Ban2;
    public static RoleListEntryOption Ban3;
    public static RoleListEntryOption Ban4;
    public static RoleListEntryOption Ban5;

    // Free Ban Options
    public static CustomHeaderOption FreeBans;
    public static CustomToggleOption BanCrewmate;
    public static CustomToggleOption BanImpostor;
    public static CustomToggleOption BanAnarchist;
    public static CustomToggleOption BanMurderer;

    // Postmortal Options
    public static CustomHeaderOption EnablePostmortals;
    public static CustomToggleOption EnablePhantom;
    public static CustomToggleOption EnableRevealer;
    public static CustomToggleOption EnableGhoul;
    public static CustomToggleOption EnableBanshee;

    // Client Options
    public static CustomHeaderOption ClientOptions;
    public static CustomToggleOption LighterDarker;
    public static CustomToggleOption WhiteNameplates;
    public static CustomToggleOption NoLevels;
    public static CustomToggleOption CustomCrewColors;
    public static CustomToggleOption CustomIntColors;
    public static CustomToggleOption CustomNeutColors;
    public static CustomToggleOption CustomSynColors;
    public static CustomToggleOption CustomModColors;
    public static CustomToggleOption CustomObjColors;
    public static CustomToggleOption CustomAbColors;
    public static CustomToggleOption CustomEjects;
    public static CustomToggleOption HideOtherGhosts;
    public static CustomToggleOption OptimisationMode;

    /*For Testing
    public static CustomToggleOption ExampleToggle;
    public static CustomNumberOption ExampleNumber;
    public static CustomStringOption ExampleString;
    public static CustomHeaderOption ExampleHeader;
    public static CustomLayersOption ExampleLayers;*/

    private static Func<object, object, string> PercentFormat => (value, _) => $"{value:0}%";
    private static Func<object, object, string> CooldownFormat => (value, _) => $"{value:0.0#}s";
    private static Func<object, object, string> DistanceFormat => (value, _) => $"{value:0.0#}m";
    private static Func<object, object, string> MultiplierFormat => (value, _) => $"{value:0.0#}x";

    private static bool Generated;

    public static void GenerateAll()
    {
        if (Generated)
            return;

        Generated = true;

        GenerateGameOptions();
        GenerateClientOptions();

        CustomOption.SaveSettings("Default");

        LogMessage($"There exist {CustomOption.AllOptions.Count + 2} total options lmao (number jumpscare)");
    }

    private static void GenerateClientOptions()
    {
        ClientOptions = new(MultiMenu.Client, "Client Settings", clientOnly: true);
        CustomCrewColors = new(MultiMenu.Client, "Custom Crew Colors", () => TownOfUsReworked.CustomCrewColors.Value = !TownOfUsReworked.CustomCrewColors.Value,
            TownOfUsReworked.CustomCrewColors.Value, clientOnly: true);
        CustomNeutColors = new(MultiMenu.Client, "Custom Neutral Colors", () => TownOfUsReworked.CustomNeutColors.Value = !TownOfUsReworked.CustomNeutColors.Value,
            TownOfUsReworked.CustomNeutColors.Value, clientOnly: true);
        CustomIntColors = new(MultiMenu.Client, "Custom Intruder Colors", () => TownOfUsReworked.CustomIntColors.Value = !TownOfUsReworked.CustomIntColors.Value,
            TownOfUsReworked.CustomIntColors.Value, clientOnly: true);
        CustomSynColors = new(MultiMenu.Client, "Custom Syndicate Colors", () => TownOfUsReworked.CustomSynColors.Value = !TownOfUsReworked.CustomSynColors.Value,
            TownOfUsReworked.CustomSynColors.Value, clientOnly: true);
        CustomModColors = new(MultiMenu.Client, "Custom Modifier Colors", () => TownOfUsReworked.CustomModColors.Value = !TownOfUsReworked.CustomModColors.Value,
            TownOfUsReworked.CustomModColors.Value, clientOnly: true);
        CustomObjColors = new(MultiMenu.Client, "Custom Objectifier Colors", () => TownOfUsReworked.CustomObjColors.Value = !TownOfUsReworked.CustomObjColors.Value,
            TownOfUsReworked.CustomObjColors.Value, clientOnly: true);
        CustomAbColors = new(MultiMenu.Client, "Custom Ability Colors", () => TownOfUsReworked.CustomAbColors.Value = !TownOfUsReworked.CustomAbColors.Value,
            TownOfUsReworked.CustomAbColors.Value, clientOnly: true);
        CustomEjects = new(MultiMenu.Client, "Custom Ejects", () => TownOfUsReworked.CustomEjects.Value = !TownOfUsReworked.CustomEjects.Value, TownOfUsReworked.CustomEjects.Value,
            clientOnly: true);
        OptimisationMode = new(MultiMenu.Client, "Optimisation Mode", () => TownOfUsReworked.OptimisationMode.Value = !TownOfUsReworked.OptimisationMode.Value,
            TownOfUsReworked.OptimisationMode.Value, clientOnly: true);
        LighterDarker = new(MultiMenu.Client, "Lighter Darker Colors", CustomToggleOption.LighterDarker, TownOfUsReworked.LighterDarker.Value, clientOnly: true);
        WhiteNameplates = new(MultiMenu.Client, "White Nameplates", CustomToggleOption.WhiteNameplates, TownOfUsReworked.WhiteNameplates.Value, clientOnly: true);
        HideOtherGhosts = new(MultiMenu.Client, "Hide Other Ghosts", () => TownOfUsReworked.HideOtherGhosts.Value = !TownOfUsReworked.HideOtherGhosts.Value,
            TownOfUsReworked.HideOtherGhosts.Value, clientOnly: true);
        NoLevels = new(MultiMenu.Client, "No Levels", CustomToggleOption.NoLevels, TownOfUsReworked.NoLevels.Value, clientOnly: true);
    }

    private static void GenerateGameOptions()
    {
        // Option saving stuff
        SettingsPatches.PresetButton = new();
        SettingsPatches.SaveSettings = new(MultiMenu.Main, "Save Current Settings", CustomButtonOption.SaveSettings);

        /*Just a template for me to use
        ExampleLayers = new(MultiMenu.Main, "Example Layers Option");
        ExampleHeader = new(MultiMenu.Main, "Example Header Option");
        ExampleToggle = new(MultiMenu.Main, "Example Toggle Option", true/false);
        ExampleNumber = new(MultiMenu.Main, "Example Number Option", 1, 1, 5, 1);
        ExampleString = new(MultiMenu.Main, "Example String Option", ["Something", "Something Else", "Something Else Else"]);*/

        GameSettings = new(MultiMenu.Main, "Game Settings");
        PlayerSpeed = new(MultiMenu.Main, "Player Speed", 1.25f, 0.25f, 10, 0.25f, MultiplierFormat);
        GhostSpeed = new(MultiMenu.Main, "Ghost Speed", 3, 0.25f, 10, 0.25f, MultiplierFormat);
        InteractionDistance = new(MultiMenu.Main, "Interaction Distance", 2, 0.5f, 5, 0.5f, DistanceFormat);
        EmergencyButtonCount = new(MultiMenu.Main, "Emergency Button Count", 1, 0, 100, 1);
        EmergencyButtonCooldown = new(MultiMenu.Main, "Emergency Button Cooldown", 25, 0, 300, 5, CooldownFormat);
        DiscussionTime = new(MultiMenu.Main, "Discussion Time", 30, 0, 300, 5, CooldownFormat);
        VotingTime = new(MultiMenu.Main, "Voting Time", 60, 5, 600, 15, CooldownFormat);
        TaskBarMode = new(MultiMenu.Main, "Taskbar Updates", ["Meeting", "Rounds", "Invisible"]);
        ConfirmEjects = new(MultiMenu.Main, "Confirm Ejects", false);
        EjectionRevealsRole = new(MultiMenu.Main, "Ejection Reveals <color=#FFD700FF>Roles</color>", false, ConfirmEjects);
        EnableInitialCds = new(MultiMenu.Main, "Enable Game Start Cooldowns", true);
        InitialCooldowns = new(MultiMenu.Main, "Game Start Cooldown", 10, 0, 30, 2.5f, CooldownFormat, EnableInitialCds);
        EnableMeetingCds = new(MultiMenu.Main, "Enable Meeting End Cooldowns", true);
        MeetingCooldowns = new(MultiMenu.Main, "Meeting End Cooldown", 15, 0, 30, 2.5f, CooldownFormat, EnableMeetingCds);
        EnableFailCds = new(MultiMenu.Main, "Enable Ability Failure Cooldowns", true);
        FailCooldowns = new(MultiMenu.Main, "Ability Failure Cooldown", 5, 0, 30, 2.5f, CooldownFormat, EnableFailCds);
        ReportDistance = new(MultiMenu.Main, "Player Report Distance", 3.5f, 1, 20, 0.25f, DistanceFormat);
        ChatCooldown = new(MultiMenu.Main, "Chat Cooldown", 3, 0, 3, 0.1f, CooldownFormat);
        ChatCharacterLimit = new(MultiMenu.Main, "Chat Character Limit", 200, 50, 2000, 50);
        LobbySize = new(MultiMenu.Main, "Lobby Size", 15, 2, 127, 1);

        GameModeSettings = new(MultiMenu.Main, "Game Mode Settings");
        CurrentMode = new(MultiMenu.Main, "Game Mode", ["<color=#C02A2CFF>Classic</color>", "<color=#CBD542FF>All Any</color>", "<color=#06E00CFF>Killing Only</color>",
            "<color=#FA1C79FF>Role List</color>", "<color=#7500AFFF>Hide And Seek</color>", "<color=#1E49CFFF>Task Race</color>", "<color=#E6956AFF>Custom</color>", "Vanilla"]);

        AARLSettings = new(MultiMenu.Main, "<color=#CBD542FF>All Any</color>/<color=#FA1C79FF>Role List</color> Settings", [GameMode.AllAny, GameMode.RoleList]);
        EnableUniques = new(MultiMenu.Main, "Enable Uniques", false, AARLSettings);

        ClassCustSettings = new(MultiMenu.Main, "<color=#C02A2CFF>Classic</color>/<color=#E6956AFF>Custom</color> Settings", [GameMode.Classic, GameMode.Custom]);
        IgnoreAlignmentCaps = new(MultiMenu.Main, "Ignore <color=#1D7CF2FF>Alignment</color> Caps", false, ClassCustSettings);
        IgnoreFactionCaps = new(MultiMenu.Main, "Ignore <color=#00E66DFF>Faction</color> Caps", false, ClassCustSettings);
        IgnoreLayerCaps = new(MultiMenu.Main, "Ignore Non-<color=#FFD700FF>Role</color> Layer Caps", false, ClassCustSettings);

        KOSettings = new(MultiMenu.Main, "<color=#06E00CFF>Killing Only</color> Settings", GameMode.KillingOnly);
        NeutralRoles = new(MultiMenu.Main, "<color=#B3B3B3FF>Neutrals</color> Count", 1, 0, 13, 1, KOSettings);
        AddArsonist = new(MultiMenu.Main, "Add <color=#EE7600FF>Arsonist</color>", false, KOSettings);
        AddCryomaniac = new(MultiMenu.Main, "Add <color=#642DEAFF>Cryomaniac</color>", false, KOSettings);
        AddPlaguebearer = new(MultiMenu.Main, "Add <color=#CFFE61FF>Plaguebearer</color>", false, KOSettings);

        HnSSettings = new(MultiMenu.Main, "<color=#7500AFFF>Hide And Seek</color> Settings", GameMode.HideAndSeek);
        HnSType = new(MultiMenu.Main, "<color=#7500AFFF>Hide And Seek</color> Game Type", ["Classic", "Infection"], HnSSettings);
        HnSShortTasks = new(MultiMenu.Main, "<color=#7500AFFF>Hide And Seek</color> Short Tasks", 4, 0, 13, 1, HnSSettings);
        HnSCommonTasks = new(MultiMenu.Main, "<color=#7500AFFF>Hide And Seek</color> Common Tasks", 4, 0, 13, 1, HnSSettings);
        HnSLongTasks = new(MultiMenu.Main, "<color=#7500AFFF>Hide And Seek</color> Long Tasks", 4, 0, 13, 1, HnSSettings);
        HunterCount = new(MultiMenu.Main, "<color=#FF004EFF>Hunter</color> Count", 1, 1, 13, 1, HnSSettings);
        HuntCd = new(MultiMenu.Main, "Hunt Cooldown", 10f, 5f, 60f, 5f, CooldownFormat, HnSSettings);
        StartTime = new(MultiMenu.Main, "Start Time", 10f, 5f, 60f, 5f, CooldownFormat, HnSSettings);
        HunterVent = new(MultiMenu.Main, "<color=#FF004EFF>Hunter</color> Can Vent", true, HnSSettings);
        HunterVision = new(MultiMenu.Main, "<color=#FF004EFF>Hunter</color> Vision", 0.25f, 0.1f, 1f, 0.05f, MultiplierFormat, HnSSettings);
        HuntedVision = new(MultiMenu.Main, "<color=#1F51FFFF>Hunted</color> Vision", 1.5f, 1f, 2f, 0.05f, MultiplierFormat, HnSSettings);
        HunterSpeedModifier = new(MultiMenu.Main, "<color=#FF004EFF>Hunter</color> Speed Modifier", 1.25f, 1f, 1.5f, 0.05f, MultiplierFormat, HnSSettings);
        HunterFlashlight = new(MultiMenu.Main, "<color=#FF004EFF>Hunters</color> Use A Flashlight", false, HnSSettings);
        HuntedFlashlight = new(MultiMenu.Main, "<color=#1F51FFFF>Hunted</color> Use A Flashlight", false, HnSSettings);
        HuntedChat = new(MultiMenu.Main, "<color=#1F51FFFF>Hunted</color> Can Chat", true, HnSSettings);

        TRSettings = new(MultiMenu.Main, "<color=#1E49CFFF>Task Race</color> Settings", GameMode.TaskRace);
        TRShortTasks = new(MultiMenu.Main, "<color=#1E49CFFF>Task Race</color> Short Tasks", 4, 0, 13, 1, TRSettings);
        TRCommonTasks = new(MultiMenu.Main, "<color=#1E49CFFF>Task Race</color> Common Tasks", 4, 0, 13, 1, TRSettings);

        GameModifiers = new(MultiMenu.Main, "Game Modifiers");
        WhoCanVent = new(MultiMenu.Main, "Serial Venters", ["Default", "Everyone", "Never"]);
        AnonymousVoting = new(MultiMenu.Main, "Anonymous Voting", ["Grey", "Non-<color=#CCA3CCFF>Politicians</color> Only", "<color=#CCA3CCFF>Politician</color> Only", "Not Visible",
            "Disabled"]);
        SkipButtonDisable = new(MultiMenu.Main, "No Skipping", ["Never", "Emergency", "Always"]);
        FirstKillShield = new(MultiMenu.Main, "First Kill Shield", false);
        WhoSeesFirstKillShield = new(MultiMenu.Main, "Who Sees First Kill Shield", ["Everyone", "Shielded", "No One"], FirstKillShield);
        FactionSeeRoles = new(MultiMenu.Main, "Factioned Evils See The <color=#FFD700FF>Roles</color> Of Their Team", true);
        VisualTasks = new(MultiMenu.Main, "Visual Tasks", false);
        PlayerNames = new(MultiMenu.Main, "Player Names", ["Hide Obstructed", "Always Visible", "Not Visible"]);
        Whispers = new(MultiMenu.Main, "Whispering", true);
        WhispersAnnouncement = new(MultiMenu.Main, "Everyone Is Alerted To Whispers", true, Whispers);
        AppearanceAnimation = new(MultiMenu.Main, "Kill Animations Show Modified Player", true);
        EnableAbilities = new(MultiMenu.Main, "Enable <color=#FF9900FF>Abilities</color>", true);
        EnableModifiers = new(MultiMenu.Main, "Enable <color=#7F7F7FFF>Modifiers</color>", true);
        EnableObjectifiers = new(MultiMenu.Main, "Enable <color=#DD585BFF>Objectifiers</color>", true);
        VentTargeting = new(MultiMenu.Main, "Players In Vents Can Be Targeted", true);
        CooldownInVent = new(MultiMenu.Main, "Cooldown Decreases Even While In A Vent", false);
        DeadSeeEverything = new(MultiMenu.Main, "Dead Can See Everything", true);
        ParallelMedScans = new(MultiMenu.Main, "Parallel Medbay Scans", false);
        HideVentAnims = new(MultiMenu.Main, "Hide Vent Animations In The Dark", true);

        GameAnnouncementsSettings = new(MultiMenu.Main, "Game Announcement Settings");
        GameAnnouncements = new(MultiMenu.Main, "Enable Game Announcements", false);
        LocationReports = new(MultiMenu.Main, "Reported Body's Location Is Announced", false, GameAnnouncements);
        RoleFactionReports = new(MultiMenu.Main, "Every Body's <color=#FFD700FF>Role</color>/<color=#00E66DFF>Faction</color> Is Announced", ["Never", "Role", "Faction"],
            GameAnnouncements);
        KillerReports = new(MultiMenu.Main, "Every Body's Killer's <color=#FFD700FF>Role</color>/<color=#00E66DFF>Faction</color> Is Announced", ["Never", "Role", "Faction"],
            GameAnnouncements);

        var maps = new List<string>() { "Skeld", "Mira HQ", "Polus", "dlekS", "Airship", "Fungle" };

        if (SubLoaded)
            maps.Add("Submerged");

        if (LILoaded)
            maps.Add("LevelImpostor");

        maps.Add("Random");

        MapSettings = new(MultiMenu.Main, "Map Settings");
        Map = new(MultiMenu.Main, "Map", [..maps]);
        RandomMapSkeld = new(MultiMenu.Main, "Skeld Chance", 10, 0, 100, 10, PercentFormat, MapEnum.Random);
        RandomMapMira = new(MultiMenu.Main, "Mira Chance", 10, 0, 100, 10, PercentFormat, MapEnum.Random);
        RandomMapPolus = new(MultiMenu.Main, "Polus Chance", 10, 0, 100, 10, PercentFormat, MapEnum.Random);
        RandomMapdlekS = new(MultiMenu.Main, "dlekS Chance", 10, 0, 100, 10, PercentFormat, MapEnum.Random);
        RandomMapAirship = new(MultiMenu.Main, "Airship Chance", 10, 0, 100, 10, PercentFormat, MapEnum.Random);
        RandomMapFungle = new(MultiMenu.Main, "Fungle Chance", 10, 0, 100, 10, PercentFormat, MapEnum.Random);
        RandomMapSubmerged = new(MultiMenu.Main, "Submerged Chance", 10, 0, 100, 10, PercentFormat, [MapEnum.Random, SubLoaded], true);
        RandomMapLevelImpostor = new(MultiMenu.Main, "LevelImpostor Chance", 10, 0, 100, 10, PercentFormat, [MapEnum.Random, LILoaded], true);
        AutoAdjustSettings = new(MultiMenu.Main, "Auto Adjust Settings", false);
        SmallMapHalfVision = new(MultiMenu.Main, "Half Vision On Small Maps", false, [MapEnum.Skeld, MapEnum.dlekS, MapEnum.Random, MapEnum.MiraHQ]);
        SmallMapDecreasedCooldown = new(MultiMenu.Main, "Small Maps Decreased Cooldowns", 0f, 0f, 15f, 2.5f, CooldownFormat, [MapEnum.Skeld, MapEnum.MiraHQ, MapEnum.dlekS, MapEnum.Random]);
        LargeMapIncreasedCooldown = new(MultiMenu.Main, "Large Maps Increased Cooldowns", 0f, 0f, 15f, 2.5f, CooldownFormat, [MapEnum.Airship, MapEnum.Submerged, MapEnum.Random,
            MapEnum.Fungle]);
        SmallMapIncreasedShortTasks = new(MultiMenu.Main, "Small Maps Increased Short Tasks", 0, 0, 5, 1, [MapEnum.Skeld, MapEnum.dlekS, MapEnum.Random, MapEnum.MiraHQ]);
        SmallMapIncreasedLongTasks = new(MultiMenu.Main, "Small Maps Increased Long Tasks", 0, 0, 3, 1, [MapEnum.Skeld, MapEnum.dlekS, MapEnum.Random, MapEnum.MiraHQ]);
        LargeMapDecreasedShortTasks = new(MultiMenu.Main, "Large Maps Decreased Short Tasks", 0, 0, 5, 1, [MapEnum.Airship, MapEnum.Submerged, MapEnum.Random, MapEnum.Fungle]);
        LargeMapDecreasedLongTasks = new(MultiMenu.Main, "Large Maps Decreased Long Tasks", 0, 0, 3, 1, [MapEnum.Airship, MapEnum.Submerged, MapEnum.Random, MapEnum.Fungle]);
        RandomSpawns = new(MultiMenu.Main, "Random Player Spawns", ["Disabled", "Game Start", "Post Meetings", "Both"]);

        BetterSabotages = new(MultiMenu.Main, "Better Sabotages");
        CamouflagedComms = new(MultiMenu.Main, "Camouflaged Comms", true);
        CamouflagedMeetings = new(MultiMenu.Main, "Camouflaged Meetings", false);
        NightVision = new(MultiMenu.Main, "Night Vision Cameras", false);
        EvilsIgnoreNV = new(MultiMenu.Main, "Evils Ignore Night Vision", false, NightVision);
        OxySlow = new(MultiMenu.Main, "Oxygen Sabotage Slows Down Players", true, [MapEnum.Skeld, MapEnum.dlekS, MapEnum.Random, MapEnum.MiraHQ]);
        ReactorShake = new(MultiMenu.Main, "Reactor Sabotage Shakes The Screen By", 30, 0, 100, 5, PercentFormat);

        BetterSkeld = new(MultiMenu.Main, "Skeld Settings", [MapEnum.Skeld, MapEnum.dlekS, MapEnum.Random]);
        EnableBetterSkeld = new(MultiMenu.Main, "Enable Better Skeld Changes", true, BetterSkeld);
        SkeldVentImprovements = new(MultiMenu.Main, "Changed Skeld Vent Layout", false, BetterSkeld);
        SkeldReactorTimer = new(MultiMenu.Main, "Skeld Reactor Meltdown Countdown", 60f, 30f, 90f, 5f, CooldownFormat, BetterSkeld);
        SkeldO2Timer = new(MultiMenu.Main, "Skeld Oxygen Depletion Countdown", 60f, 30f, 90f, 5f, CooldownFormat, BetterSkeld);

        BetterMiraHQ = new(MultiMenu.Main, "Mira HQ Settings", [MapEnum.MiraHQ, MapEnum.Random]);
        EnableBetterMiraHQ = new(MultiMenu.Main, "Enable Better Mira HQ Changes", true, BetterMiraHQ);
        MiraHQVentImprovements = new(MultiMenu.Main, "Changed Mira HQ Vent Layout", false, BetterMiraHQ);
        MiraReactorTimer = new(MultiMenu.Main, "Mira HQ Reactor Meltdown Countdown", 60f, 30f, 90f, 5f, CooldownFormat, BetterMiraHQ);
        MiraO2Timer = new(MultiMenu.Main, "Mira HQ Oxygen Depletion Countdown", 60f, 30f, 90f, 5f, CooldownFormat, BetterMiraHQ);

        BetterPolus = new(MultiMenu.Main, "Polus Settings", [MapEnum.Polus, MapEnum.Random]);
        EnableBetterPolus = new(MultiMenu.Main, "Enable Better Polus Changes", true, BetterPolus);
        PolusVentImprovements = new(MultiMenu.Main, "Changed Polus Vent Layout", false, BetterPolus);
        VitalsLab = new(MultiMenu.Main, "Vitals Moved To Lab", false, BetterPolus);
        ColdTempDeathValley = new(MultiMenu.Main, "Cold Temp Moved To Death Valley", false, BetterPolus);
        WifiChartCourseSwap = new(MultiMenu.Main, "Reboot Wifi And Chart Course Swapped", false, BetterPolus);
        SeismicTimer = new(MultiMenu.Main, "Seimic Stabliser Malfunction Countdown", 60f, 30f, 90f, 5f, CooldownFormat, BetterPolus);

        BetterAirship = new(MultiMenu.Main, "Airship Settings", [MapEnum.Airship, MapEnum.Random]);
        EnableBetterAirship = new(MultiMenu.Main, "Enable Better Airship Changes", true, BetterAirship);
        SpawnType = new(MultiMenu.Main, "Spawn Type", ["Normal", "Fixed", "Synchronised", "Meeting"], BetterAirship);
        MoveVitals = new(MultiMenu.Main, "Move Vitals", false, BetterAirship);
        MoveFuel = new(MultiMenu.Main, "Move Fuel", false, BetterAirship);
        MoveDivert = new(MultiMenu.Main, "Move Divert Power", false, BetterAirship);
        MoveAdmin = new(MultiMenu.Main, "Move Admin Table", ["Don't Move", "Cockpit", "Main Hall"], BetterAirship);
        MoveElectrical = new(MultiMenu.Main, "Move Electrical Outlet", ["Don't Move", "Vault", "Electrical"], BetterAirship);
        MinDoorSwipeTime = new(MultiMenu.Main, "Min Time For Door Swipe", 0.4f, 0f, 10f, 0.1f, BetterAirship);
        CrashTimer = new(MultiMenu.Main, "Heli Crash Countdown", 90f, 30f, 100f, 5f, BetterAirship);

        BetterFungle = new(MultiMenu.Main, "Fungle Settings", [MapEnum.Fungle, MapEnum.Random]);
        EnableBetterFungle = new(MultiMenu.Main, "Enable Better Fungle Changes", true, BetterFungle);
        FungleReactorTimer = new(MultiMenu.Main, "Fungle Reactor Meltdown Countdown", 60f, 30f, 90f, 5f, CooldownFormat, BetterFungle);
        FungleMixupTimer = new(MultiMenu.Main, "Fungle Mushroom Mixup Timer", 8f, 4f, 20f, 1f, CooldownFormat, BetterFungle);

        CrewSettings = new(MultiMenu.Crew, "<color=#8CFFFFFF>Crew</color> Settings");
        CommonTasks = new(MultiMenu.Crew, "Common Tasks", 2, 0, 100, 1);
        LongTasks = new(MultiMenu.Crew, "Long Tasks", 1, 0, 100, 1);
        ShortTasks = new(MultiMenu.Crew, "Short Tasks", 4, 0, 100, 1);
        GhostTasksCountToWin = new(MultiMenu.Crew, "<color=#8CFFFFFF>Crew</color> Ghost Tasks Count To Win", true);
        CrewVision = new(MultiMenu.Crew, "<color=#8CFFFFFF>Crew</color> Vision", 1f, 0.25f, 5f, 0.25f, MultiplierFormat);
        CrewFlashlight = new(MultiMenu.Crew, "<color=#8CFFFFFF>Crew</color> Uses A Flashlight", false);
        CrewMax = new(MultiMenu.Crew, "Max <color=#8CFFFFFF>Crew</color> <color=#FFD700FF>Roles</color>", 5, 0, 14, 1, [GameMode.Classic, GameMode.AllAny, GameMode.Custom]);
        CrewMin = new(MultiMenu.Crew, "Min <color=#8CFFFFFF>Crew</color> <color=#FFD700FF>Roles</color>", 5, 0, 14, 1, [GameMode.Classic, GameMode.AllAny, GameMode.Custom]);
        CrewVent = new(MultiMenu.Crew, "<color=#8CFFFFFF>Crew</color> Can Vent", false);

        CARoles = new(MultiMenu.Crew, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Auditor</color> <color=#FFD700FF>Roles</color>", [GameMode.Classic, GameMode.AllAny, GameMode.Custom]);
        MysticOn = new(MultiMenu.Crew, "<color=#708EEFFF>Mystic</color>", parent: CARoles);
        VampireHunterOn = new(MultiMenu.Crew, "<color=#C0C0C0FF>Vampire Hunter</color>", parent: CARoles);

        CIRoles = new(MultiMenu.Crew, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Investigative</color> <color=#FFD700FF>Roles</color>", [GameMode.Classic, GameMode.AllAny,
            GameMode.Custom]);
        CoronerOn = new(MultiMenu.Crew, "<color=#4D99E6FF>Coroner</color>", parent: CIRoles);
        DetectiveOn = new(MultiMenu.Crew, "<color=#4D4DFFFF>Detective</color>", parent: CIRoles);
        MediumOn = new(MultiMenu.Crew, "<color=#A680FFFF>Medium</color>", parent: CIRoles);
        OperativeOn = new(MultiMenu.Crew, "<color=#A7D1B3FF>Operative</color>", parent: CIRoles);
        SeerOn = new(MultiMenu.Crew, "<color=#71368AFF>Seer</color>", parent: CIRoles);
        SheriffOn = new(MultiMenu.Crew, "<color=#FFCC80FF>Sheriff</color>", parent: CIRoles);
        TrackerOn = new(MultiMenu.Crew, "<color=#009900FF>Tracker</color>", parent: CIRoles);

        CKRoles = new(MultiMenu.Crew, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Killing</color> <color=#FFD700FF>Roles</color>", [GameMode.Classic, GameMode.AllAny, GameMode.Custom]);
        BastionOn = new(MultiMenu.Crew, "<color=#7E3C64FF>Bastion</color>", parent: CKRoles);
        VeteranOn = new(MultiMenu.Crew, "<color=#998040FF>Veteran</color>", parent: CKRoles);
        VigilanteOn = new(MultiMenu.Crew, "<color=#FFFF00FF>Vigilante</color>", parent: CKRoles);

        CrPRoles = new(MultiMenu.Crew, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Protective</color> <color=#FFD700FF>Roles</color>", [GameMode.Classic, GameMode.AllAny,
            GameMode.Custom]);
        AltruistOn = new(MultiMenu.Crew, "<color=#660000FF>Altruist</color>", parent: CrPRoles);
        MedicOn = new(MultiMenu.Crew, "<color=#006600FF>Medic</color>", parent: CrPRoles);
        TrapperOn = new(MultiMenu.Crew, "<color=#BE1C8CFF>Trapper</color>", parent: CrPRoles);

        CSvRoles = new(MultiMenu.Crew, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Sovereign</color> <color=#FFD700FF>Roles</color>", [GameMode.Classic, GameMode.AllAny,
            GameMode.Custom]);
        DictatorOn = new(MultiMenu.Crew, "<color=#00CB97FF>Dictator</color>", parent: CSvRoles);
        MayorOn = new(MultiMenu.Crew, "<color=#704FA8FF>Mayor</color>", parent: CSvRoles);
        MonarchOn = new(MultiMenu.Crew, "<color=#FF004EFF>Monarch</color>", parent: CSvRoles);

        CSRoles = new(MultiMenu.Crew, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Support</color> <color=#FFD700FF>Roles</color>", [GameMode.Classic, GameMode.AllAny, GameMode.Custom]);
        ChameleonOn = new(MultiMenu.Crew, "<color=#5411F8FF>Chameleon</color>", parent: CSRoles);
        EngineerOn = new(MultiMenu.Crew, "<color=#FFA60AFF>Engineer</color>", parent: CSRoles);
        EscortOn = new(MultiMenu.Crew, "<color=#803333FF>Escort</color>", parent: CSRoles);
        RetributionistOn = new(MultiMenu.Crew, "<color=#8D0F8CFF>Retributionist</color>", parent: CSRoles);
        ShifterOn = new(MultiMenu.Crew, "<color=#DF851FFF>Shifter</color>", parent: CSRoles);
        TransporterOn = new(MultiMenu.Crew, "<color=#00EEFFFF>Transporter</color>", parent: CSRoles);

        CURoles = new(MultiMenu.Crew, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Utility</color> <color=#FFD700FF>Roles</color>", [GameMode.Classic, GameMode.AllAny, GameMode.Custom]);
        CrewmateOn = new(MultiMenu.Crew, "<color=#8CFFFFFF>Crewmate</color>", parent: [CURoles, GameMode.Custom], all: true);
        RevealerOn = new(MultiMenu.Crew, "<color=#D3D3D3FF>Revealer</color>", parent: CURoles);

        CASettings = new(MultiMenu.Crew, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Auditor</color> Settings", [VampireHunterOn, MysticOn, LayerEnum.VampireHunter, LayerEnum.CrewAudit,
            LayerEnum.Mystic, LayerEnum.RandomCrew]);
        CAMax = new(MultiMenu.Crew, "Max <color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Auditors</color>", 1, 1, 14, 1, CASettings);

        Mystic = new(MultiMenu.Crew, "<color=#708EEFFF>Mystic</color>", [MysticOn, LayerEnum.CrewAudit, LayerEnum.Mystic, LayerEnum.RandomCrew]);
        UniqueMystic = new(MultiMenu.Crew, "<color=#708EEFFF>Mystic</color> Is Unique", false, [Mystic, EnableUniques], true);
        MysticRevealCd = new(MultiMenu.Crew, "Reveal Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Mystic);

        VampireHunter = new(MultiMenu.Crew, "<color=#C0C0C0FF>Vampire Hunter</color>", [VampireHunterOn, LayerEnum.VampireHunter, LayerEnum.CrewAudit, LayerEnum.RandomCrew]);
        UniqueVampireHunter = new(MultiMenu.Crew, "<color=#C0C0C0FF>Vampire Hunter</color> Is Unique", false, [VampireHunter, EnableUniques], true);
        StakeCd = new(MultiMenu.Crew, "Stake Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, VampireHunter);

        CISettings = new(MultiMenu.Crew, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Investigative</color> Settings", [CoronerOn, DetectiveOn, SeerOn, MediumOn, SheriffOn, TrackerOn,
            OperativeOn, LayerEnum.CrewInvest, LayerEnum.Coroner, LayerEnum.Detective, LayerEnum.Seer, LayerEnum.Medium, LayerEnum.Sheriff, LayerEnum.Tracker, LayerEnum.Operative,
            LayerEnum.RandomCrew, LayerEnum.Mystic, LayerEnum.CrewAudit, MysticOn, LayerEnum.RegularCrew]);
        CIMax = new(MultiMenu.Crew, "Max <color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Investigatives</color>", 1, 1, 14, 1, CISettings);

        Coroner = new(MultiMenu.Crew, "<color=#4D99E6FF>Coroner</color>", [CoronerOn, LayerEnum.Coroner, LayerEnum.CrewInvest, LayerEnum.RandomCrew, LayerEnum.RegularCrew]);
        UniqueCoroner = new(MultiMenu.Crew, "<color=#4D99E6FF>Coroner</color> Is Unique", false, [Coroner, EnableUniques], true);
        CoronerArrowDur = new(MultiMenu.Crew, "Dead Body Arrow Duration", 0.1f, 0f, 2f, 0.05f, CooldownFormat, Coroner);
        CoronerReportRole = new(MultiMenu.Crew, "<color=#4D99E6FF>Coroner</color> Gets Roles", false, Coroner);
        CoronerReportName = new(MultiMenu.Crew, "<color=#4D99E6FF>Coroner</color> Gets Killer's Name", false, Coroner);
        CoronerKillerNameTime = new(MultiMenu.Crew, "<color=#4D99E6FF>Coroner</color> Gets Killer's Name Under", 1f, 0.5f, 15f, 0.5f, CooldownFormat, [Coroner, CoronerReportName], true);
        CompareCd = new(MultiMenu.Crew, "Compare Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Coroner);
        AutopsyCd = new(MultiMenu.Crew, "Autopsy Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Coroner);

        Detective = new(MultiMenu.Crew, "<color=#4D4DFFFF>Detective</color>", [DetectiveOn, LayerEnum.Detective, LayerEnum.CrewInvest, LayerEnum.RandomCrew, LayerEnum.RegularCrew]);
        UniqueDetective = new(MultiMenu.Crew, "<color=#4D4DFFFF>Detective</color> Is Unique", false, [Detective, EnableUniques], true);
        ExamineCd = new(MultiMenu.Crew, "Examine Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Detective);
        RecentKill = new(MultiMenu.Crew, "Bloody Hands Duration", 10f, 5f, 60f, 2.5f, CooldownFormat, Detective);
        FootprintInterval = new(MultiMenu.Crew, "Footprint Interval", 0.15f, 0.05f, 2f, 0.05f, CooldownFormat, Detective);
        FootprintDur = new(MultiMenu.Crew, "Footprint Duration", 10f, 1f, 10f, 0.5f, CooldownFormat, Detective);
        AnonymousFootPrint = new(MultiMenu.Crew, "Anonymous Footprint", ["Only When Camouflaged", "Always Visible", "Always Camouflaged"], Detective);

        Medium = new(MultiMenu.Crew, "<color=#A680FFFF>Medium</color>", [MediumOn, LayerEnum.Medium, LayerEnum.CrewInvest, LayerEnum.RandomCrew , LayerEnum.RegularCrew]);
        UniqueMedium = new(MultiMenu.Crew, "<color=#A680FFFF>Medium</color> Is Unique", false, [Medium, EnableUniques], true);
        MediateCd = new(MultiMenu.Crew, "Mediate Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Medium);
        ShowMediatePlayer = new(MultiMenu.Crew, "Reveal Appearance Of Mediate Target", true, Medium);
        ShowMediumToDead = new(MultiMenu.Crew, "Reveal The <color=#A680FFFF>Medium</color> To The Mediate Target", ["No", "Target", "All Dead"], Medium);
        DeadRevealed = new(MultiMenu.Crew, "Who Is Revealed With Mediate", ["Oldest Dead", "Newest Dead", "All Dead", "Random"], Medium);

        Operative = new(MultiMenu.Crew, "<color=#A7D1B3FF>Operative</color>", [OperativeOn, LayerEnum.Operative, LayerEnum.CrewInvest, LayerEnum.RandomCrew, LayerEnum.RegularCrew]);
        UniqueOperative = new(MultiMenu.Crew, "<color=#A7D1B3FF>Operative</color> Is Unique", false, [Operative, EnableUniques], true);
        BugCd = new(MultiMenu.Crew, "Bug Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Operative);
        MinAmountOfTimeInBug = new(MultiMenu.Crew, "Min Amount Of Time In Bug To Trigger", 0f, 0f, 15f, 0.5f, CooldownFormat, Operative);
        BugsRemoveOnNewRound = new(MultiMenu.Crew, "Bugs Are Removed Each Round", true, Operative);
        MaxBugs = new(MultiMenu.Crew, "Max Bugs", 5, 1, 15, 1, Operative);
        BugRange = new(MultiMenu.Crew, "Bug Radius", 1.5f, 0.5f, 5f, 0.25f, DistanceFormat, Operative);
        MinAmountOfPlayersInBug = new(MultiMenu.Crew, "Number Of <color=#FFD700FF>Roles</color> Required To Trigger Bug", 1, 1, 5, 1, Operative);
        WhoSeesDead = new(MultiMenu.Crew, "Who Sees Dead Bodies On Admin", ["Nobody", "Operative", "Everyone But Operative", "Everyone"], Operative);
        PreciseOperativeInfo = new(MultiMenu.Crew, "<color=#A7D1B3FF>Operative</color> Gets Precise Information", false, Operative);

        Seer = new(MultiMenu.Crew, "<color=#71368AFF>Seer</color>", [SeerOn, LayerEnum.Seer, LayerEnum.CrewInvest, LayerEnum.RandomCrew, Mystic, LayerEnum.RegularCrew]);
        UniqueSeer = new(MultiMenu.Crew, "<color=#71368AFF>Seer</color> Is Unique", false, [Seer, EnableUniques], true);
        SeerCd = new(MultiMenu.Crew, "Seer Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Seer);

        Sheriff = new(MultiMenu.Crew, "<color=#FFCC80FF>Sheriff</color>", [SheriffOn, LayerEnum.Sheriff, LayerEnum.CrewInvest, LayerEnum.RandomCrew, Seer, LayerEnum.RegularCrew]);
        UniqueSheriff = new(MultiMenu.Crew, "<color=#FFCC80FF>Sheriff</color> Is Unique", false, [Sheriff, EnableUniques], true);
        InterrogateCd = new(MultiMenu.Crew, "Interrogate Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Sheriff);
        NeutEvilRed = new(MultiMenu.Crew, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Evils</color> Show Evil", false, Sheriff);
        NeutKillingRed = new(MultiMenu.Crew, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killers</color> Show Evil", false, Sheriff);

        Tracker = new(MultiMenu.Crew, "<color=#009900FF>Tracker</color>", [TrackerOn, LayerEnum.Tracker, LayerEnum.CrewInvest, LayerEnum.RandomCrew, LayerEnum.RegularCrew]);
        UniqueTracker = new(MultiMenu.Crew, "<color=#009900FF>Tracker</color> Is Unique", false, [Tracker, EnableUniques], true);
        MaxTracks = new(MultiMenu.Crew, "Max Tracks", 5, 1, 15, 1, Tracker);
        TrackCd = new(MultiMenu.Crew, "Track Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Tracker);
        ResetOnNewRound = new(MultiMenu.Crew, "<color=#009900FF>Tracker</color> Arrows Reset After Each Round", false, Tracker);
        UpdateInterval = new(MultiMenu.Crew, "Arrow Update Interval", 5f, 0f, 15f, 0.5f, CooldownFormat, Tracker);

        CKSettings = new(MultiMenu.Crew, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Killing</color> Settings", [VigilanteOn, VeteranOn, LayerEnum.Vigilante, LayerEnum.Veteran,
            LayerEnum.Bastion, LayerEnum.CrewKill, LayerEnum.RandomCrew, VampireHunterOn, LayerEnum.CrewAudit, LayerEnum.VampireHunter, LayerEnum.RegularCrew]);
        CKMax = new(MultiMenu.Crew, "Max <color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Killers</color>", 1, 1, 14, 1, CKSettings);

        Bastion = new(MultiMenu.Crew, "<color=#7E3C64FF>Bastion</color>", [BastionOn, LayerEnum.Bastion, LayerEnum.CrewKill, LayerEnum.RandomCrew, LayerEnum.RegularCrew]);
        UniqueBastion = new(MultiMenu.Crew, "<color=#7E3C64FF>Bastion</color> Is Unique", false, [Bastion, EnableUniques], true);
        MaxBombs = new(MultiMenu.Crew, "Max Bombs", 5, 1, 15, 1, Bastion);
        BastionCd = new(MultiMenu.Crew, "<color=#7E3C64FF>Bastion</color> Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Bastion);
        BombRemovedOnKill = new(MultiMenu.Crew, "Bombs Are Removed Upon Kills", true, Bastion);

        Veteran = new(MultiMenu.Crew, "<color=#998040FF>Veteran</color>", [VeteranOn, LayerEnum.Veteran, LayerEnum.CrewKill, LayerEnum.RandomCrew, LayerEnum.RegularCrew]);
        UniqueVeteran = new(MultiMenu.Crew, "<color=#998040FF>Veteran</color> Is Unique", false, [Veteran, EnableUniques], true);
        MaxAlerts = new(MultiMenu.Crew, "Max Alerts", 5, 1, 15, 1, Veteran);
        AlertCd = new(MultiMenu.Crew, "Alert Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Veteran);
        AlertDur = new(MultiMenu.Crew, "Alert Duration", 10f, 5f, 30f, 1f, CooldownFormat, Veteran);

        Vigilante = new(MultiMenu.Crew, "<color=#FFFF00FF>Vigilante</color>", [VigilanteOn, LayerEnum.Vigilante, LayerEnum.CrewKill, LayerEnum.RandomCrew, VampireHunter,
            LayerEnum.RegularCrew]);
        UniqueVigilante = new(MultiMenu.Crew, "<color=#FFFF00FF>Vigilante</color> Is Unique", false, [Vigilante, EnableUniques], true);
        MisfireKillsInno = new(MultiMenu.Crew, "Misfire Kills The Target", true, Vigilante);
        VigiKillAgain = new(MultiMenu.Crew, "<color=#FFFF00FF>Vigilante</color> Can Kill Again If Target Was Innocent", true, Vigilante);
        RoundOneNoShot = new(MultiMenu.Crew, "<color=#FFFF00FF>Vigilante</color> Cannot Shoot On The First Round", true, Vigilante);
        VigiOptions = new(MultiMenu.Crew, "How Does <color=#FFFF00FF>Vigilante</color> Die", ["Immediately", "Before Meeting", "After Meeting"], Vigilante);
        VigiNotifOptions = new(MultiMenu.Crew, "How Is The <color=#FFFF00FF>Vigilante</color> Notified Of Their Target's Innocence", ["Never", "Flash", "Message"], Vigilante);
        MaxBullets = new(MultiMenu.Crew, "Max Bullets", 5, 1, 15, 1, Vigilante);
        ShootCd = new(MultiMenu.Crew, "Shoot Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Vigilante);

        CrPSettings = new(MultiMenu.Crew, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Protective</color> Settings", [AltruistOn, MedicOn, LayerEnum.Altruist, LayerEnum.Medic,
            LayerEnum.CrewProt, LayerEnum.RandomCrew, LayerEnum.RegularCrew, TrapperOn, LayerEnum.Trapper]);
        CrPMax = new(MultiMenu.Crew, "Max <color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Protectives</color>", 1, 1, 14, 1, CrPSettings);

        Altruist = new(MultiMenu.Crew, "<color=#660000FF>Altruist</color>", [AltruistOn, LayerEnum.Altruist, LayerEnum.CrewProt, LayerEnum.RandomCrew, LayerEnum.RegularCrew]);
        UniqueAltruist = new(MultiMenu.Crew, "<color=#660000FF>Altruist</color> Is Unique", false, [Altruist, EnableUniques], true);
        MaxRevives = new(MultiMenu.Crew, "Max Revives", 5, 1, 14, 1, Altruist);
        ReviveCd = new(MultiMenu.Crew, "Revive Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Altruist);
        ReviveDur = new(MultiMenu.Crew, "Revive Duration", 10f, 1f, 15f, 1f, CooldownFormat, Altruist);
        AltruistTargetBody = new(MultiMenu.Crew, "Target's Body Disappears On Beginning Of Revive", false, Altruist);

        Medic = new(MultiMenu.Crew, "<color=#006600FF>Medic</color>", [MedicOn, LayerEnum.Medic, LayerEnum.CrewProt, LayerEnum.RandomCrew, LayerEnum.RegularCrew]);
        UniqueMedic = new(MultiMenu.Crew, "<color=#006600FF>Medic</color> Is Unique", false, [Medic, EnableUniques], true);
        ShowShielded = new(MultiMenu.Crew, "Show Shielded Player", ["Self", "<color=#006600FF>Medic</color>", "Self And <color=#006600FF>Medic</color>", "Everyone", "Nobody"], Medic);
        WhoGetsNotification = new(MultiMenu.Crew, "Who Gets Murder Attempt Indicator", ["<color=#006600FF>Medic</color>", "Self", "Self And <color=#006600FF>Medic</color>", "Everyone",
            "Nobody"], Medic);
        ShieldBreaks = new(MultiMenu.Crew, "Shield Breaks On Murder Attempt", true, Medic);

        Trapper = new(MultiMenu.Crew, "<color=#BE1C8CFF>Trapper</color>", [TrapperOn, LayerEnum.Trapper, LayerEnum.CrewProt, LayerEnum.RandomCrew]);
        UniqueTrapper = new(MultiMenu.Crew, "<color=#BE1C8CFF>Trapper</color> Is Unique", false, [Trapper, EnableUniques], true);
        MaxTraps = new(MultiMenu.Crew, "Max Traps", 5, 1, 15, 1, Trapper);
        BuildCd = new(MultiMenu.Crew, "Build Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Trapper);
        BuildDur = new(MultiMenu.Crew, "Build Duration", 10f, 5f, 30f, 1f, CooldownFormat, Trapper);
        TrapCd = new(MultiMenu.Crew, "Trap Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Trapper);

        CSvSettings = new(MultiMenu.Crew, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Sovereign</color> Settings", [MayorOn, DictatorOn, MonarchOn, LayerEnum.Monarch, LayerEnum.Mayor,
            LayerEnum.Monarch, LayerEnum.CrewSov, LayerEnum.RandomCrew]);
        CSvMax = new(MultiMenu.Crew, "Max <color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Sovereigns</color>", 1, 1, 14, 1, CSvSettings);

        Dictator = new(MultiMenu.Crew, "<color=#00CB97FF>Dictator</color>", [DictatorOn, LayerEnum.Dictator, LayerEnum.CrewSov, LayerEnum.RandomCrew]);
        UniqueDictator = new(MultiMenu.Crew, "<color=#00CB97FF>Dictator</color> Is Unique", false, [Dictator, EnableUniques], true);
        RoundOneNoDictReveal = new(MultiMenu.Crew, "<color=#00CB97FF>Dictator</color> Cannot Reveal Round One", false, Dictator);
        DictateAfterVoting = new(MultiMenu.Crew, "<color=#00CB97FF>Dictator</color> Can Dictate After Voting", false, Dictator);
        DictatorButton = new(MultiMenu.Crew, "<color=#00CB97FF>Dictator</color> Can Button", true, Dictator);

        Mayor = new(MultiMenu.Crew, "<color=#704FA8FF>Mayor</color>", [MayorOn, LayerEnum.Mayor, LayerEnum.CrewSov, LayerEnum.RandomCrew]);
        UniqueMayor = new(MultiMenu.Crew, "<color=#704FA8FF>Mayor</color> Is Unique", false, [Mayor, EnableUniques], true);
        MayorVoteCount = new(MultiMenu.Crew, "Revealed <color=#704FA8FF>Mayor</color> Votes Count As", 2, 1, 10, 1, Mayor);
        RoundOneNoMayorReveal = new(MultiMenu.Crew, "<color=#704FA8FF>Mayor</color> Cannot Reveal Round One", false, Mayor);
        MayorButton = new(MultiMenu.Crew, "<color=#704FA8FF>Mayor</color> Can Button", true, Mayor);

        Monarch = new(MultiMenu.Crew, "<color=#FF004EFF>Monarch</color>", [MonarchOn, LayerEnum.Monarch, LayerEnum.CrewSov, LayerEnum.RandomCrew]);
        UniqueMonarch = new(MultiMenu.Crew, "<color=#FF004EFF>Monarch</color> Is Unique", false, [Monarch, EnableUniques], true);
        KnightingCd = new(MultiMenu.Crew, "Knighting Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Monarch);
        RoundOneNoKnighting = new(MultiMenu.Crew, "<color=#FF004EFF>Monarch</color> Cannot Knight Round One", false, Monarch);
        KnightCount = new(MultiMenu.Crew, "Knight Count", 2, 1, 14, 1, Monarch);
        KnightVoteCount = new(MultiMenu.Crew, "Knighted Votes Count As", 1, 1, 10, 1, Monarch);
        MonarchButton = new(MultiMenu.Crew, "<color=#FF004EFF>Monarch</color> Can Button", true, Monarch);
        KnightButton = new(MultiMenu.Crew, "Knights Can Button", true, Monarch);

        CSSettings = new(MultiMenu.Crew, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Support</color> Settings", [ChameleonOn, EngineerOn, EscortOn, RetributionistOn, ShifterOn,
            TransporterOn, LayerEnum.RandomCrew, LayerEnum.Chameleon, LayerEnum.Engineer, LayerEnum.Escort, LayerEnum.Retributionist, LayerEnum.Shifter, LayerEnum.Transporter,
            LayerEnum.CrewSupport, LayerEnum.RegularCrew]);
        CSMax = new(MultiMenu.Crew, "Max <color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Supporters</color>", 1, 1, 14, 1, CSSettings);

        Chameleon = new(MultiMenu.Crew, "<color=#5411F8FF>Chameleon</color>", [ChameleonOn, LayerEnum.Chameleon, LayerEnum.CrewSupport, LayerEnum.RandomCrew, LayerEnum.RegularCrew]);
        UniqueChameleon = new(MultiMenu.Crew, "<color=#5411F8FF>Chameleon</color> Is Unique", false, [Chameleon, EnableUniques], true);
        MaxSwoops = new(MultiMenu.Crew, "Max Swoops", 5, 1, 15, 1, Chameleon);
        SwoopCd = new(MultiMenu.Crew, "Swoop Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Chameleon);
        SwoopDur = new(MultiMenu.Crew, "Swoop Duration", 10f, 5f, 30f, 1f, CooldownFormat, Chameleon);

        Engineer = new(MultiMenu.Crew, "<color=#FFA60AFF>Engineer</color>", [EngineerOn, LayerEnum.Engineer, LayerEnum.CrewSupport, LayerEnum.RandomCrew, LayerEnum.RegularCrew]);
        UniqueEngineer = new(MultiMenu.Crew, "<color=#FFA60AFF>Engineer</color> Is Unique", false, [Engineer, EnableUniques], true);
        MaxFixes = new(MultiMenu.Crew, "Max Fixes", 5, 0, 15, 1, Engineer);
        FixCd = new(MultiMenu.Crew, "Fix Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Engineer);

        Escort = new(MultiMenu.Crew, "<color=#803333FF>Escort</color>", [EscortOn, LayerEnum.Engineer, LayerEnum.CrewSupport, LayerEnum.RandomCrew, LayerEnum.RegularCrew]);
        UniqueEscort = new(MultiMenu.Crew, "<color=#803333FF>Escort</color> Is Unique", false, [Escort, EnableUniques], true);
        EscortCd = new(MultiMenu.Crew, "<color=#803333FF>Escort</color> Roleblock Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Escort);
        EscortDur = new(MultiMenu.Crew, "<color=#803333FF>Escort</color> Roleblock Duration", 10f, 5f, 30f, 1f, CooldownFormat, Escort);

        Retributionist = new(MultiMenu.Crew, "<color=#8D0F8CFF>Retributionist</color>", [RetributionistOn, LayerEnum.Retributionist, LayerEnum.CrewSupport, LayerEnum.RandomCrew,
            LayerEnum.RegularCrew]);
        UniqueRetributionist = new(MultiMenu.Crew, "<color=#8D0F8CFF>Retributionist</color> Is Unique", false, [Retributionist, EnableUniques], true);
        ReviveAfterVoting = new(MultiMenu.Crew, "<color=#8D0F8CFF>Retributionist</color> Can Mimic After Voting", true, Retributionist);

        Shifter = new(MultiMenu.Crew, "<color=#DF851FFF>Shifter</color>", [ShifterOn, LayerEnum.Shifter, LayerEnum.CrewSupport, LayerEnum.RandomCrew, LayerEnum.RegularCrew]);
        UniqueShifter = new(MultiMenu.Crew, "<color=#DF851FFF>Shifter</color> Is Unique", false, [Shifter, EnableUniques], true);
        ShiftCd = new(MultiMenu.Crew, "Shift Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Shifter);
        ShiftedBecomes = new(MultiMenu.Crew, "Shifted Becomes", ["Shifter", "Crewmate"], Shifter);

        Transporter = new(MultiMenu.Crew, "<color=#00EEFFFF>Transporter</color>", [TransporterOn, LayerEnum.Transporter, LayerEnum.CrewSupport, LayerEnum.RandomCrew,
            LayerEnum.RegularCrew]);
        UniqueTransporter = new(MultiMenu.Crew, "<color=#00EEFFFF>Transporter</color> Is Unique", false, [Transporter, EnableUniques], true);
        MaxTransports = new(MultiMenu.Crew, "Max Transports", 5, 1, 15, 1, Transporter);
        TransportCd = new(MultiMenu.Crew, "Transport Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Transporter);
        TransportDur = new(MultiMenu.Crew, "Transport Duration", 5f, 1f, 20f, 1f, CooldownFormat, Transporter);
        TransSelf = new(MultiMenu.Crew, "<color=#00EEFFFF>Transporter</color> Can Transport Themselves", true, Transporter);

        CUSettings = new(MultiMenu.Crew, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Utility</color> Settings");

        Revealer = new(MultiMenu.Crew, "<color=#D3D3D3FF>Revealer</color>");
        RevealerTasksRemainingClicked = new(MultiMenu.Crew, "Tasks Remaining When <color=#D3D3D3FF>Revealer</color> Can Be Clicked", 5, 1, 10, 1, Revealer);
        RevealerTasksRemainingAlert = new(MultiMenu.Crew, "Tasks Remaining When Revealed", 1, 1, 5, 1, Revealer);
        RevealerRevealsNeutrals = new(MultiMenu.Crew, "<color=#D3D3D3FF>Revealer</color> Reveals <color=#B3B3B3FF>Neutrals</color>", false, Revealer);
        RevealerRevealsCrew = new(MultiMenu.Crew, "<color=#D3D3D3FF>Revealer</color> Reveals <color=#8CFFFFFF>Crew</color>", false, Revealer);
        RevealerRevealsRoles = new(MultiMenu.Crew, "<color=#D3D3D3FF>Revealer</color> Reveals Exact <color=#FFD700FF>Roles</color>", false, Revealer);
        RevealerCanBeClickedBy = new(MultiMenu.Crew, "Who Can Click <color=#D3D3D3FF>Revealer</color>", ["All", "Non Crew", "Evils Only"], Revealer);

        CUSettings.Parents = [Revealer];

        NBRoles = new(MultiMenu.Neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Benign</color> <color=#FFD700FF>Roles</color>", [GameMode.Classic, GameMode.AllAny,
            GameMode.Custom]);
        AmnesiacOn = new(MultiMenu.Neutral, "<color=#22FFFFFF>Amnesiac</color>", parent: NBRoles);
        GuardianAngelOn = new(MultiMenu.Neutral, "<color=#FFFFFFFF>Guardian Angel</color>", parent: NBRoles);
        SurvivorOn = new(MultiMenu.Neutral, "<color=#DDDD00FF>Survivor</color>", parent: NBRoles);
        ThiefOn = new(MultiMenu.Neutral, "<color=#80FF00FF>Thief</color>", parent: NBRoles);

        NERoles = new(MultiMenu.Neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Evil</color> <color=#FFD700FF>Roles</color>", [GameMode.Classic, GameMode.AllAny,
            GameMode.Custom]);
        ActorOn = new(MultiMenu.Neutral, "<color=#00ACC2FF>Actor</color>", parent: NERoles);
        BountyHunterOn = new(MultiMenu.Neutral, "<color=#B51E39FF>Bounty Hunter</color>", parent: NERoles);
        CannibalOn = new(MultiMenu.Neutral, "<color=#8C4005FF>Cannibal</color>", parent: NERoles);
        ExecutionerOn = new(MultiMenu.Neutral, "<color=#CCCCCCFF>Executioner</color>", parent: NERoles);
        GuesserOn = new(MultiMenu.Neutral, "<color=#EEE5BEFF>Guesser</color>", parent: NERoles);
        JesterOn = new(MultiMenu.Neutral, "<color=#F7B3DAFF>Jester</color>", parent: NERoles);
        TrollOn = new(MultiMenu.Neutral, "<color=#678D36FF>Troll</color>", parent: NERoles);

        NHRoles = new(MultiMenu.Neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Harbinger</color> <color=#FFD700FF>Roles</color>", [GameMode.Classic, GameMode.AllAny,
            GameMode.Custom, GameMode.KillingOnly, AddPlaguebearer]);
        PlaguebearerOn = new(MultiMenu.Neutral, "<color=#CFFE61FF>Plaguebearer</color>", parent: [NHRoles, AddPlaguebearer]);

        NKRoles = new(MultiMenu.Neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killing</color> <color=#FFD700FF>Roles</color>", [GameMode.Classic, GameMode.KillingOnly,
            GameMode.AllAny, GameMode.Custom, AddArsonist, AddCryomaniac]);
        ArsonistOn = new(MultiMenu.Neutral, "<color=#EE7600FF>Arsonist</color>", parent: [NKRoles, AddArsonist]);
        CryomaniacOn = new(MultiMenu.Neutral, "<color=#642DEAFF>Cryomaniac</color>", parent: [NKRoles, AddCryomaniac]);
        GlitchOn = new(MultiMenu.Neutral, "<color=#00FF00FF>Glitch</color>", parent: NKRoles);
        JuggernautOn = new(MultiMenu.Neutral, "<color=#A12B56FF>Juggernaut</color>", parent: NKRoles);
        MurdererOn = new(MultiMenu.Neutral, "<color=#6F7BEAFF>Murderer</color>", parent: NKRoles);
        SerialKillerOn = new(MultiMenu.Neutral, "<color=#336EFFFF>Serial Killer</color>", parent: NKRoles);
        WerewolfOn = new(MultiMenu.Neutral, "<color=#9F703AFF>Werewolf</color>", parent: NKRoles);

        NNRoles = new(MultiMenu.Neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Neophyte</color> <color=#FFD700FF>Roles</color>", [GameMode.Classic, GameMode.AllAny,
            GameMode.Custom]);
        DraculaOn = new(MultiMenu.Neutral, "<color=#AC8A00FF>Dracula</color>", parent: NNRoles);
        JackalOn = new(MultiMenu.Neutral, "<color=#45076AFF>Jackal</color>", parent: NNRoles);
        NecromancerOn = new(MultiMenu.Neutral, "<color=#BF5FFFFF>Necromancer</color>", parent: NNRoles);
        WhispererOn = new(MultiMenu.Neutral, "<color=#2D6AA5FF>Whisperer</color>", parent: NNRoles);

        NPRoles = new(MultiMenu.Neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Proselyte</color> <color=#FFD700FF>Roles</color>", [GameMode.Classic, GameMode.AllAny,
            GameMode.Custom]);
        PhantomOn = new(MultiMenu.Neutral, "<color=#662962FF>Phantom</color>", parent: NPRoles);

        NeutralSettings = new(MultiMenu.Neutral, "<color=#B3B3B3FF>Neutral</color> Settings");
        NeutralVision = new(MultiMenu.Neutral, "<color=#B3B3B3FF>Neutral</color> Vision", 1.5f, 0.25f, 5f, 0.25f, MultiplierFormat, NeutralSettings);
        LightsAffectNeutrals = new(MultiMenu.Neutral, "Lights Sabotage Affects <color=#B3B3B3FF>Neutral</color> Vision", true, NeutralSettings);
        NeutralFlashlight = new(MultiMenu.Neutral, "<color=#B3B3B3FF>Neutrals</color> Use A Flashlight", false, NeutralSettings);
        NeutralMax = new(MultiMenu.Neutral, "Max <color=#B3B3B3FF>Neutrals</color>", 5, 1, 14, 1, NeutralSettings);
        NeutralMin = new(MultiMenu.Neutral, "Min <color=#B3B3B3FF>Neutrals</color>", 5, 1, 14, 1, NeutralSettings);
        NoSolo = new(MultiMenu.Neutral, "<color=#B3B3B3FF>Neutrals</color> Together, Strong", ["Never", "Same NKs", "All NKs", "All Neutrals"], NeutralSettings);
        AvoidNeutralKingmakers = new(MultiMenu.Neutral, "Avoid <color=#B3B3B3FF>Neutral</color> Kingmakers", false, NeutralSettings);
        NeutralsVent = new(MultiMenu.Neutral, "<color=#B3B3B3FF>Neutrals</color> Can Vent", true, NeutralSettings);

        NASettings = new(MultiMenu.Neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Apocalypse</color> Settings", [PlaguebearerOn, LayerEnum.NeutralApoc, LayerEnum.NeutralHarb,
            LayerEnum.RandomNeutral, LayerEnum.HarmfulNeutral, LayerEnum.Plaguebearer, LayerEnum.Pestilence]);

        Pestilence = new(MultiMenu.Neutral, "<color=#424242FF>Pestilence</color>", [PlaguebearerOn, LayerEnum.Pestilence, LayerEnum.Plaguebearer, LayerEnum.NeutralApoc,
            LayerEnum.NeutralHarb, LayerEnum.RandomNeutral, LayerEnum.HarmfulNeutral]);
        PestSpawn = new(MultiMenu.Neutral, "<color=#424242FF>Pestilence</color> Can Spawn Directly", false, Pestilence);
        PlayersAlerted = new(MultiMenu.Neutral, "<color=#424242FF>Pestilence</color> Transformation Alerts Everyone", true, Pestilence);
        ObliterateCd = new(MultiMenu.Neutral, "Obliterate Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Pestilence);
        PestVent = new(MultiMenu.Neutral, "<color=#424242FF>Pestilence</color> Can Vent", true, Pestilence);

        NBSettings = new(MultiMenu.Neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Benign</color> Settings", [AmnesiacOn, GuardianAngelOn, SurvivorOn, ThiefOn,
            LayerEnum.RandomNeutral, LayerEnum.Amnesiac, LayerEnum.GuardianAngel, LayerEnum.Survivor, LayerEnum.Thief, LayerEnum.NeutralBen, LayerEnum.RegularNeutral]);
        NBMax = new(MultiMenu.Neutral, "Max <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Benigns</color>", 1, 1, 14, 1, NBSettings);
        VigiKillsNB = new(MultiMenu.Neutral, "<color=#FFFF00FF>Vigilante</color> Kills <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Benigns</color>", true, [Vigilante, NBSettings],
            true);

        Amnesiac = new(MultiMenu.Neutral, "<color=#22FFFFFF>Amnesiac</color>", [AmnesiacOn, LayerEnum.Amnesiac, LayerEnum.NeutralBen, LayerEnum.RandomNeutral, LayerEnum.RegularNeutral]);
        UniqueAmnesiac = new(MultiMenu.Neutral, "<color=#22FFFFFF>Amnesiac</color> Is Unique", false, [Amnesiac, EnableUniques], true);
        RememberArrows = new(MultiMenu.Neutral, "<color=#22FFFFFF>Amnesiac</color> Gets Arrows To Dead Bodies", false, Amnesiac);
        RememberArrowDelay = new(MultiMenu.Neutral, "Remember Arrow Appearance Delay", 5f, 0f, 15f, 1f, CooldownFormat, Amnesiac);
        AmneVent = new(MultiMenu.Neutral, "<color=#22FFFFFF>Amnesiac</color> Can Hide In Vents", false, Amnesiac);
        AmneSwitchVent = new(MultiMenu.Neutral, "<color=#22FFFFFF>Amnesiac</color> Can Switch Vents", false, AmneVent);
        AmneToThief = new(MultiMenu.Neutral, "<color=#22FFFFFF>Amnesiac</color> Can Turn Into <color=#80FF00FF>Thief</color>", true, Amnesiac);

        GuardianAngel = new(MultiMenu.Neutral, "<color=#FFFFFFFF>Guardian Angel</color>", [GuardianAngelOn, LayerEnum.GuardianAngel, LayerEnum.NeutralBen, LayerEnum.RandomNeutral,
            LayerEnum.RegularNeutral]);
        UniqueGuardianAngel = new(MultiMenu.Neutral, "<color=#FFFFFFFF>Guardian Angel</color> Is Unique", false, [GuardianAngel, EnableUniques], true);
        GuardianAngelCanPickTargets = new(MultiMenu.Neutral, "<color=#FFFFFFFF>Guardian Angel</color> Can Pick Their Own Target", false, GuardianAngel);
        ProtectCd = new(MultiMenu.Neutral, "Protect Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, GuardianAngel);
        ProtectDur = new(MultiMenu.Neutral, "Protect Duration", 10f, 5f, 30f, 1f, CooldownFormat, GuardianAngel);
        MaxProtects = new(MultiMenu.Neutral, "Max Protects", 5, 1, 15, 1, GuardianAngel);
        ShowProtect = new(MultiMenu.Neutral, "Show Protected Player", ["Self", "Guardian Angel", "Self And GA", "Everyone", "Nobody"], GuardianAngel);
        GATargetKnows = new(MultiMenu.Neutral, "Target Knows <color=#FFFFFFFF>Guardian Angel</color> Exists", false, GuardianAngel);
        ProtectBeyondTheGrave = new(MultiMenu.Neutral, "<color=#FFFFFFFF>Guardian Angel</color> Can Protect After Death", false, GuardianAngel);
        GAKnowsTargetRole = new(MultiMenu.Neutral, "<color=#FFFFFFFF>Guardian Angel</color> Knows Target's <color=#FFD700FF>Role</color>", false, GuardianAngel);
        GAVent = new(MultiMenu.Neutral, "<color=#FFFFFFFF>Guardian Angel</color> Can Hide In Vents", false, GuardianAngel);
        GASwitchVent = new(MultiMenu.Neutral, "<color=#FFFFFFFF>Guardian Angel</color> Can Switch Vents", false, GAVent);
        GAToSurv = new(MultiMenu.Neutral, "<color=#FFFFFFFF>Guardian Angel</color> Can Turn Into <color=#DDDD00FF>Survivor</color>", true, GuardianAngel);

        Survivor = new(MultiMenu.Neutral, "<color=#DDDD00FF>Survivor</color>", [SurvivorOn, LayerEnum.Survivor, LayerEnum.NeutralBen, LayerEnum.RandomNeutral, GuardianAngel,
            LayerEnum.RegularNeutral]);
        UniqueSurvivor = new(MultiMenu.Neutral, "<color=#DDDD00FF>Survivor</color> Is Unique", false, [Survivor, EnableUniques], true);
        VestCd = new(MultiMenu.Neutral, "Vest Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Survivor);
        VestDur = new(MultiMenu.Neutral, "Vest Duration", 10f, 5f, 30f, 1f, CooldownFormat, Survivor);
        MaxVests = new(MultiMenu.Neutral, "Max Vests", 5, 1, 15, 1, Survivor);
        SurvVent = new(MultiMenu.Neutral, "<color=#DDDD00FF>Survivor</color> Can Hide In Vents", false, Survivor);
        SurvSwitchVent = new(MultiMenu.Neutral, "<color=#DDDD00FF>Survivor</color> Can Switch Vents", false, SurvVent);

        Thief = new(MultiMenu.Neutral, "<color=#80FF00FF>Thief</color>", [ThiefOn, LayerEnum.Thief, LayerEnum.NeutralBen, LayerEnum.RandomNeutral, Amnesiac, LayerEnum.RegularNeutral]);
        UniqueThief = new(MultiMenu.Neutral, "<color=#80FF00FF>Thief</color> Is Unique", false, [Thief, EnableUniques], true);
        StealCd = new(MultiMenu.Neutral, "Steal Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Thief);
        ThiefSteals = new(MultiMenu.Neutral, "<color=#80FF00FF>Thief</color> Assigns <color=#80FF00FF>Thief</color> <color=#FFD700FF>Role</color> To Target", false, Thief);
        ThiefCanGuess = new(MultiMenu.Neutral, "<color=#80FF00FF>Thief</color> Can Guess To Steal Roles", false, Thief);
        ThiefCanGuessAfterVoting = new(MultiMenu.Neutral, "<color=#80FF00FF>Thief</color> Can Guess After Voting", false, Thief);
        ThiefVent = new(MultiMenu.Neutral, "<color=#80FF00FF>Thief</color> Can Vent", false, Thief);

        NESettings = new(MultiMenu.Neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Evil</color> Settings", [ActorOn, BountyHunterOn, CannibalOn, ExecutionerOn, TrollOn,
            GuesserOn, JesterOn, LayerEnum.RandomNeutral, LayerEnum.Actor, LayerEnum.NeutralEvil, LayerEnum.BountyHunter, LayerEnum.Cannibal, LayerEnum.Executioner, LayerEnum.Troll,
            LayerEnum.Guesser, LayerEnum.Jester, LayerEnum.RegularNeutral]);
        NEMax = new(MultiMenu.Neutral, "Max <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Evils</color>", 1, 1, 14, 1, NESettings);
        NeutralEvilsEndGame = new(MultiMenu.Neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Evils</color> End The Game When Winning", false, NESettings);

        Actor = new(MultiMenu.Neutral, "<color=#00ACC2FF>Actor</color>");
        UniqueActor = new(MultiMenu.Neutral, "<color=#00ACC2FF>Actor</color> Is Unique", false, [Actor, EnableUniques], true);
        ActorCanPickRole = new(MultiMenu.Neutral, "<color=#00ACC2FF>Actor</color> Can Choose A Target Role", false, Actor);
        ActorButton = new(MultiMenu.Neutral, "<color=#00ACC2FF>Actor</color> Can Button", true, Actor);
        ActorVent = new(MultiMenu.Neutral, "<color=#00ACC2FF>Actor</color> Can Hide In Vents", false, Actor);
        ActSwitchVent = new(MultiMenu.Neutral, "<color=#00ACC2FF>Actor</color> Can Switch Vents", false, ActorVent);
        ActorRoleCount = new(MultiMenu.Neutral, "<color=#00ACC2FF>Actor</color> Role List Guess Count", 3, 1, 5, 1, Actor);
        VigiKillsActor = new(MultiMenu.Neutral, "<color=#FFFF00FF>Vigilante</color> Kills <color=#00ACC2FF>Actor</color>", false, [Actor, Vigilante], true);

        BountyHunter = new(MultiMenu.Neutral, "<color=#B51E39FF>Bounty Hunter</color>", [BountyHunterOn, LayerEnum.BountyHunter, LayerEnum.NeutralEvil, LayerEnum.RandomNeutral,
            LayerEnum.RegularNeutral]);
        UniqueBountyHunter = new(MultiMenu.Neutral, "<color=#B51E39FF>Bounty Hunter</color> Is Unique", false, [BountyHunter, EnableUniques], true);
        BountyHunterCanPickTargets = new(MultiMenu.Neutral, "<color=#B51E39FF>Bounty Hunter</color> Can Indirectly Pick Their Own Target", false, BountyHunter);
        BountyHunterGuesses = new(MultiMenu.Neutral, "Max Target Guesses", 5, 1, 15, 1, BountyHunter);
        GuessCd = new(MultiMenu.Neutral, "Guess Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, BountyHunter);
        BHVent = new(MultiMenu.Neutral, "<color=#B51E39FF>Bounty Hunter</color> Can Vent", false, BountyHunter);
        VigiKillsBH = new(MultiMenu.Neutral, "<color=#FFFF00FF>Vigilante</color> Kills <color=#B51E39FF>Bounty Hunter</color>", false, [Vigilante, BountyHunter], true);
        BHToTroll = new(MultiMenu.Neutral, "<color=#B51E39FF>Bounty Hunter</color> Can Turn Into <color=#678D36FF>Troll</color>", true, BountyHunter);

        Cannibal = new(MultiMenu.Neutral, "<color=#8C4005FF>Cannibal</color>", [CannibalOn, LayerEnum.Cannibal, LayerEnum.NeutralEvil, LayerEnum.RandomNeutral, LayerEnum.RegularNeutral]);
        UniqueCannibal = new(MultiMenu.Neutral, "<color=#8C4005FF>Cannibal</color> Is Unique", false, [Cannibal, EnableUniques], true);
        EatCd = new(MultiMenu.Neutral, "Eat Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Cannibal);
        BodiesNeeded = new(MultiMenu.Neutral, "Bodies Needed To Win", 1, 1, 5, 1, Cannibal);
        EatArrows = new(MultiMenu.Neutral, "<color=#8C4005FF>Cannibal</color> Gets Arrows To Dead Bodies", false, Cannibal);
        EatArrowDelay = new(MultiMenu.Neutral, "Hunger Arrow Appearance Delay", 5f, 0f, 15f, 1f, CooldownFormat, Cannibal);
        CannibalVent = new(MultiMenu.Neutral, "<color=#8C4005FF>Cannibal</color> Can Vent", false, Cannibal);
        VigiKillsCannibal = new(MultiMenu.Neutral, "<color=#FFFF00FF>Vigilante</color> Kills <color=#8C4005FF>Cannibal</color>", false, [Cannibal, Vigilante], true);

        Executioner = new(MultiMenu.Neutral, "<color=#CCCCCCFF>Executioner</color>", [ExecutionerOn, LayerEnum.Executioner, LayerEnum.NeutralEvil, LayerEnum.RandomNeutral,
            LayerEnum.RegularNeutral]);
        UniqueExecutioner = new(MultiMenu.Neutral, "<color=#CCCCCCFF>Executioner</color> Is Unique", false, [Executioner, EnableUniques], true);
        ExecutionerCanPickTargets = new(MultiMenu.Neutral, "<color=#CCCCCCFF>Executioner</color> Can Pick Their Own Target", false, Executioner);
        ExecutionerButton = new(MultiMenu.Neutral, "<color=#CCCCCCFF>Executioner</color> Can Button", true, Executioner);
        ExeVent = new(MultiMenu.Neutral, "<color=#CCCCCCFF>Executioner</color> Can Hide In Vents", false, Executioner);
        ExeSwitchVent = new(MultiMenu.Neutral, "<color=#CCCCCCFF>Executioner</color> Can Switch Vents", false, ExeVent);
        ExeTargetKnows = new(MultiMenu.Neutral, "Target Knows <color=#CCCCCCFF>Executioner</color> Exists", false, Executioner);
        ExeKnowsTargetRole = new(MultiMenu.Neutral, "<color=#CCCCCCFF>Executioner</color> Knows Target's <color=#FFD700FF>Role</color>", false, Executioner);
        ExeEjectScreen = new(MultiMenu.Neutral, "Target Ejection Reveals Existence Of <color=#CCCCCCFF>Executioner</color>", false, Executioner);
        ExeCanWinBeyondDeath = new(MultiMenu.Neutral, "<color=#CCCCCCFF>Executioner</color> Can Win After Death", false, Executioner);
        VigiKillsExecutioner = new(MultiMenu.Neutral, "<color=#FFFF00FF>Vigilante</color> Kills <color=#CCCCCCFF>Executioner</color>", false, [Executioner, Vigilante], true);
        ExeToJest = new(MultiMenu.Neutral, "<color=#CCCCCCFF>Exeutioner</color> Can Turn Into <color=#F7B3DAFF>Jester</color>", true, Executioner);

        Guesser = new(MultiMenu.Neutral, "<color=#EEE5BEFF>Guesser</color>", [GuesserOn, LayerEnum.Guesser, LayerEnum.NeutralEvil, LayerEnum.RandomNeutral, LayerEnum.RegularNeutral]);
        UniqueGuesser = new(MultiMenu.Neutral, "<color=#EEE5BEFF>Guesser</color> Is Unique", false, [Guesser, EnableUniques], true);
        GuesserCanPickTargets = new(MultiMenu.Neutral, "<color=#EEE5BEFF>Guesser</color> Can Pick Their Own Target", false, Guesser);
        GuesserButton = new(MultiMenu.Neutral, "<color=#EEE5BEFF>Guesser</color> Can Button", true, Guesser);
        GuessVent = new(MultiMenu.Neutral, "<color=#EEE5BEFF>Guesser</color> Can Hide In Vents", false, Guesser);
        GuessSwitchVent = new(MultiMenu.Neutral, "<color=#EEE5BEFF>Guesser</color> Can Switch Vents", false, GuessVent);
        GuessTargetKnows = new(MultiMenu.Neutral, "Target Knows <color=#EEE5BEFF>Guesser</color> Exists", false, Guesser);
        MultipleGuesses = new(MultiMenu.Neutral, "<color=#EEE5BEFF>Guesser</color> Can Guess Multiple Times", true, Guesser);
        MaxGuesses = new(MultiMenu.Neutral, "Max Meeting Guesses", 5, 1, 15, 1, Guesser);
        GuesserAfterVoting = new(MultiMenu.Neutral, "<color=#EEE5BEFF>Guesser</color> Can Guess After Voting", false, Guesser);
        VigiKillsGuesser = new(MultiMenu.Neutral, "<color=#FFFF00FF>Vigilante</color> Kills <color=#EEE5BEFF>Guesser</color>", false, [Guesser, Vigilante], true);
        GuessToAct = new(MultiMenu.Neutral, "<color=#EEE5BEFF>Guesser</color> Can Turn Into <color=#00ACC2FF>Actor</color>", true, Guesser);

        Actor.Parents = [ActorOn, LayerEnum.Actor, LayerEnum.NeutralEvil, LayerEnum.RandomNeutral, Guesser, LayerEnum.RegularNeutral];

        Jester = new(MultiMenu.Neutral, "<color=#F7B3DAFF>Jester</color>", [JesterOn, LayerEnum.Jester, LayerEnum.NeutralEvil, LayerEnum.RandomNeutral, Executioner, LayerEnum.RegularNeutral
            ]);
        UniqueJester = new(MultiMenu.Neutral, "<color=#F7B3DAFF>Jester</color> Is Unique", false, [Jester, EnableUniques], true);
        JesterButton = new(MultiMenu.Neutral, "<color=#F7B3DAFF>Jester</color> Can Button", true, Jester);
        JesterVent = new(MultiMenu.Neutral, "<color=#F7B3DAFF>Jester</color> Can Hide In Vents", false, Jester);
        JestSwitchVent = new(MultiMenu.Neutral, "<color=#F7B3DAFF>Jester</color> Can Switch Vents", false, JesterVent);
        JestEjectScreen = new(MultiMenu.Neutral, "<color=#F7B3DAFF>Jester</color> Ejection Reveals Existence Of <color=#F7B3DAFF>Jester</color>", false, Jester);
        VigiKillsJester = new(MultiMenu.Neutral, "<color=#FFFF00FF>Vigilante</color> Kills <color=#F7B3DAFF>Jester</color>", false, [Jester, Vigilante], true);

        Troll = new(MultiMenu.Neutral, "<color=#678D36FF>Troll</color>", [TrollOn, LayerEnum.Troll, LayerEnum.NeutralEvil, LayerEnum.RandomNeutral, BountyHunter, LayerEnum.RegularNeutral]);
        UniqueTroll = new(MultiMenu.Neutral, "<color=#678D36FF>Troll</color> Is Unique", false, [Troll, EnableUniques], true);
        InteractCd = new(MultiMenu.Neutral, "Interact Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Troll);
        TrollVent = new(MultiMenu.Neutral, "<color=#678D36FF>Troll</color> Can Hide In Vent", false, Troll);
        TrollSwitchVent = new(MultiMenu.Neutral, "<color=#678D36FF>Troll</color> Can Switch Vents", false, TrollVent);

        NHSettings = new(MultiMenu.Neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Harbinger</color> Settings", [PlaguebearerOn, LayerEnum.Plaguebearer, LayerEnum.NeutralHarb,
            LayerEnum.RandomNeutral]);
        NHMax = new(MultiMenu.Neutral, "Max <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Harbingers</color>", 1, 1, 14, 1, NHSettings);

        Plaguebearer = new(MultiMenu.Neutral, "<color=#CFFE61FF>Plaguebearer</color>", [PlaguebearerOn, LayerEnum.Plaguebearer, LayerEnum.NeutralHarb, LayerEnum.RandomNeutral]);
        UniquePlaguebearer = new(MultiMenu.Neutral, "<color=#CFFE61FF>Plaguebearer</color> Is Unique", false, [Plaguebearer, EnableUniques], true);
        InfectCd = new(MultiMenu.Neutral, "Infect Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Plaguebearer);
        PBVent = new(MultiMenu.Neutral, "<color=#CFFE61FF>Plaguebearer</color> Can Vent", false, Plaguebearer);

        NKSettings = new(MultiMenu.Neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killing</color> Settings", [ArsonistOn, CryomaniacOn, GlitchOn, JuggernautOn, MurdererOn,
            SerialKillerOn, WerewolfOn, LayerEnum.RandomNeutral, LayerEnum.Arsonist, LayerEnum.NeutralKill, LayerEnum.Cryomaniac, LayerEnum.Glitch, LayerEnum.Juggernaut, LayerEnum.Murderer,
            LayerEnum.SerialKiller, LayerEnum.Werewolf, LayerEnum.HarmfulNeutral]);
        NKMax = new(MultiMenu.Neutral, "Max <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killers</color>", 1, 1, 14, 1, NKSettings);
        NKHasImpVision = new(MultiMenu.Neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killers</color> Have <color=#FF1919FF>Intruder</color> Vision", true, NKSettings);
        NKsKnow = new(MultiMenu.Neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killers</color> Know Each Other", false, NKSettings);

        Arsonist = new(MultiMenu.Neutral, "<color=#EE7600FF>Arsonist</color>", [ArsonistOn, LayerEnum.Arsonist, LayerEnum.NeutralKill, LayerEnum.RandomNeutral, LayerEnum.HarmfulNeutral]);
        UniqueArsonist = new(MultiMenu.Neutral, "<color=#EE7600FF>Arsonist</color> Is Unique", false, [Arsonist, EnableUniques], true);
        ArsoDouseCd = new(MultiMenu.Neutral, "<color=#EE7600FF>Arsonist</color> Douse Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Arsonist);
        IgniteCd = new(MultiMenu.Neutral, "Ignite Cooldown", 25f, 5f, 60f, 2.5f, CooldownFormat, Arsonist);
        ArsoLastKillerBoost = new(MultiMenu.Neutral, "Ignite Cooldown Removed When <color=#EE7600FF>Arsonist</color> Is Last Killer", false, Arsonist);
        ArsoIgniteAll = new(MultiMenu.Neutral, "Ignition Ignites All Doused Players", false, Arsonist);
        ArsoCooldownsLinked = new(MultiMenu.Neutral, "Douse And Ignite Cooldowns Are Linked", false, Arsonist);
        IgnitionCremates = new(MultiMenu.Neutral, "Ignition Cremates Bodies", false, Arsonist);
        ArsoVent = new(MultiMenu.Neutral, "<color=#EE7600FF>Arsonist</color> Can Vent", false, Arsonist);

        Cryomaniac = new(MultiMenu.Neutral, "<color=#642DEAFF>Cryomaniac</color>", [CryomaniacOn, LayerEnum.Cryomaniac, LayerEnum.NeutralKill, LayerEnum.RandomNeutral,
            LayerEnum.HarmfulNeutral]);
        UniqueCryomaniac = new(MultiMenu.Neutral, "<color=#642DEAFF>Cryomaniac</color> Is Unique", false, [Cryomaniac, EnableUniques], true);
        CryoDouseCd = new(MultiMenu.Neutral, "<color=#642DEAFF>Cryomaniac</color> Douse Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Cryomaniac);
        CryoFreezeAll = new(MultiMenu.Neutral, "<color=#642DEAFF>Cryomaniac</color> Freeze Freezes All Doused Players", false, Cryomaniac);
        CryoLastKillerBoost = new(MultiMenu.Neutral, "<color=#642DEAFF>Cryomaniac</color> Can Kill Normally When Last Killer", false, Cryomaniac);
        CryoKillCd = new(MultiMenu.Neutral, "<color=#642DEAFF>Cryomaniac</color> Kill Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, CryoLastKillerBoost);
        CryoVent = new(MultiMenu.Neutral, "<color=#642DEAFF>Cryomaniac</color> Can Vent", false, Cryomaniac);

        Glitch = new(MultiMenu.Neutral, "<color=#00FF00FF>Glitch</color>", [GlitchOn, LayerEnum.Glitch, LayerEnum.NeutralKill, LayerEnum.RandomNeutral, LayerEnum.HarmfulNeutral]);
        UniqueGlitch = new(MultiMenu.Neutral, "<color=#00FF00FF>Glitch</color> Is Unique", false, [Glitch, EnableUniques], true);
        MimicCd = new(MultiMenu.Neutral, "Mimic Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Glitch);
        MimicDur = new(MultiMenu.Neutral, "Mimic Duration", 10f, 5f, 30f, 1f, CooldownFormat, Glitch);
        HackCd = new(MultiMenu.Neutral, "Hack Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Glitch);
        HackDur = new(MultiMenu.Neutral, "Hack Duration", 10f, 5f, 30f, 1f, CooldownFormat, Glitch);
        NeutraliseCd = new(MultiMenu.Neutral, "Neutralise Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Glitch);
        GlitchVent = new(MultiMenu.Neutral, "<color=#00FF00FF>Glitch</color> Can Vent", false, Glitch);

        Juggernaut = new(MultiMenu.Neutral, "<color=#A12B56FF>Juggernaut</color>", [JuggernautOn, LayerEnum.Juggernaut, LayerEnum.NeutralKill, LayerEnum.RandomNeutral,
            LayerEnum.HarmfulNeutral]);
        UniqueJuggernaut = new(MultiMenu.Neutral, "<color=#A12B56FF>Juggernaut</color> Is Unique", false, [Juggernaut, EnableUniques], true);
        AssaultCd = new(MultiMenu.Neutral, "Assault Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Juggernaut);
        AssaultBonus = new(MultiMenu.Neutral, "Assault Cooldown Bonus", 5f, 2.5f, 30f, 2.5f, CooldownFormat, Juggernaut);
        JuggVent = new(MultiMenu.Neutral, "<color=#A12B56FF>Juggernaut</color> Can Vent", false, Juggernaut);

        Murderer = new(MultiMenu.Neutral, "<color=#6F7BEAFF>Murderer</color>", [MurdererOn, LayerEnum.Murderer, LayerEnum.NeutralKill, LayerEnum.RandomNeutral, LayerEnum.HarmfulNeutral]);
        UniqueMurderer = new(MultiMenu.Neutral, "<color=#6F7BEAFF>Murderer</color> Is Unique", false, [Murderer, EnableUniques], true);
        MurderCd = new(MultiMenu.Neutral, "Murder Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Murderer);
        MurdVent = new(MultiMenu.Neutral, "<color=#6F7BEAFF>Murderer</color> Can Vent", false, Murderer);

        SerialKiller = new(MultiMenu.Neutral, "<color=#336EFFFF>Serial Killer</color>", [SerialKillerOn, LayerEnum.SerialKiller, LayerEnum.NeutralKill, LayerEnum.RandomNeutral,
            LayerEnum.HarmfulNeutral]);
        UniqueSerialKiller = new(MultiMenu.Neutral, "<color=#336EFFFF>Serial Killer</color> Is Unique", false, [SerialKiller, EnableUniques], true);
        BloodlustCd = new(MultiMenu.Neutral, "Bloodlust Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, SerialKiller);
        BloodlustDur = new(MultiMenu.Neutral, "Bloodlust Duration", 10f, 5f, 30f, 1f, CooldownFormat, SerialKiller);
        StabCd = new(MultiMenu.Neutral, "Stab Cooldown", 5f, 0.5f, 15f, 0.5f, CooldownFormat, SerialKiller);
        SKVentOptions = new(MultiMenu.Neutral, "<color=#336EFFFF>Serial Killer</color> Can Vent", ["Always", "Bloodlust", "No Lust", "Never"], SerialKiller);

        Werewolf = new(MultiMenu.Neutral, "<color=#9F703AFF>Werewolf</color>", [WerewolfOn, LayerEnum.Werewolf, LayerEnum.NeutralKill, LayerEnum.RandomNeutral, LayerEnum.HarmfulNeutral]);
        UniqueWerewolf = new(MultiMenu.Neutral, "<color=#9F703AFF>Werewolf</color> Is Unique", false, [Werewolf, EnableUniques], true);
        MaulCd = new(MultiMenu.Neutral, "Maul Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Werewolf);
        MaulRadius = new(MultiMenu.Neutral, "Maul Radius", 1.5f, 0.5f, 5f, 0.25f, DistanceFormat, Werewolf);
        CanStillAttack = new(MultiMenu.Neutral, "<color=#AC8A00FF>Dracula</color> Can Attack Every Round", false, Werewolf);
        WerewolfVent = new(MultiMenu.Neutral, "<color=#9F703AFF>Werewolf</color> Can Vent", ["Always", "When Attacking", "When Not Attacking", "Never"], Werewolf);

        NNSettings = new(MultiMenu.Neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Neophyte</color> Settings", [DraculaOn, WhispererOn, JackalOn, NecromancerOn,
            LayerEnum.Dracula, LayerEnum.Whisperer, LayerEnum.Jackal, LayerEnum.Necromancer, LayerEnum.RandomNeutral, LayerEnum.NeutralNeo, LayerEnum.HarmfulNeutral]);
        NNMax = new(MultiMenu.Neutral, "Max <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Neophytes</color>", 1, 1, 14, 1, NNSettings);
        NNHasImpVision = new(MultiMenu.Neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Neophytes</color> Have <color=#FF1919FF>Intruder</color> Vision", true, NNSettings);

        Dracula = new(MultiMenu.Neutral, "<color=#AC8A00FF>Dracula</color>", [DraculaOn, LayerEnum.Dracula, LayerEnum.NeutralNeo, LayerEnum.RandomNeutral, LayerEnum.HarmfulNeutral]);
        UniqueDracula = new(MultiMenu.Neutral, "<color=#AC8A00FF>Dracula</color> Is Unique", false, [Dracula, EnableUniques], true);
        BiteCd = new(MultiMenu.Neutral, "Bite Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Dracula);
        DracVent = new(MultiMenu.Neutral, "<color=#AC8A00FF>Dracula</color> Can Vent", false, Dracula);
        AliveVampCount = new(MultiMenu.Neutral, "Alive <color=#7B8968FF>Undead</color> Count", 3, 1, 14, 1, Dracula);
        UndeadVent = new(MultiMenu.Neutral, "Undead Can Vent", false, Dracula);

        Jackal = new(MultiMenu.Neutral, "<color=#45076AFF>Jackal</color>", [JackalOn, LayerEnum.Jackal, LayerEnum.NeutralNeo, LayerEnum.RandomNeutral, LayerEnum.HarmfulNeutral]);
        UniqueJackal = new(MultiMenu.Neutral, "<color=#45076AFF>Jackal</color> Is Unique", false, [Jackal, EnableUniques], true);
        RecruitCd = new(MultiMenu.Neutral, "Recruit Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Jackal);
        JackalVent = new(MultiMenu.Neutral, "<color=#45076AFF>Jackal</color> Can Vent", false, Jackal);
        RecruitVent = new(MultiMenu.Neutral, "Recruits Can Vent", false, Jackal);

        Necromancer = new(MultiMenu.Neutral, "<color=#BF5FFFFF>Necromancer</color>", [NecromancerOn, LayerEnum.Necromancer, LayerEnum.NeutralNeo, LayerEnum.RandomNeutral,
            LayerEnum.HarmfulNeutral]);
        UniqueNecromancer = new(MultiMenu.Neutral, "<color=#BF5FFFFF>Necromancer</color> Is Unique", false, [Necromancer, EnableUniques], true);
        ResurrectCd = new(MultiMenu.Neutral, "Resurrect Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Necromancer);
        ResurrectCdIncreases = new(MultiMenu.Neutral, "Resurrect Cooldown Increases", true, Necromancer);
        ResurrectCdIncrease = new(MultiMenu.Neutral, "Resurrect Cooldown Increases By", 5f, 2.5f, 30f, 2.5f, CooldownFormat, ResurrectCdIncreases);
        MaxResurrections = new(MultiMenu.Neutral, "Max Resurrections", 5, 1, 14, 1, Necromancer);
        NecroKillCd = new(MultiMenu.Neutral, "Sacrifice Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Necromancer);
        NecroKillCdIncreases = new(MultiMenu.Neutral, "Sacrifice Cooldown Increases", true, Necromancer);
        NecroKillCdIncrease = new(MultiMenu.Neutral, "Sacrifice Cooldown Increases By", 5f, 2.5f, 30f, 2.5f, CooldownFormat, NecroKillCdIncreases);
        MaxNecroKills = new(MultiMenu.Neutral, "Max Sacrifices", 5, 1, 14, 1, Necromancer);
        NecroCooldownsLinked = new(MultiMenu.Neutral, "Sacrifice And Resurrect Cooldowns Are Linked", false, Necromancer);
        NecromancerTargetBody = new(MultiMenu.Neutral, "Target's Body Disappears On Beginning Of Resurrect", false, Necromancer);
        ResurrectDur = new(MultiMenu.Neutral, "Resurrect Duration", 10f, 1f, 15f, 1f, CooldownFormat, Necromancer);
        NecroVent = new(MultiMenu.Neutral, "<color=#BF5FFFFF>Necromancer</color> Can Vent", false, Necromancer);
        ResurrectVent = new(MultiMenu.Neutral, "Resurrected Can Vent", false, Necromancer);

        Whisperer = new(MultiMenu.Neutral, "<color=#2D6AA5FF>Whisperer</color>", [WhispererOn, LayerEnum.Whisperer, LayerEnum.NeutralNeo, LayerEnum.RandomNeutral,
            LayerEnum.HarmfulNeutral]);
        UniqueWhisperer = new(MultiMenu.Neutral, "<color=#2D6AA5FF>Whisperer</color> Is Unique", false, [Whisperer, EnableUniques], true);
        WhisperCd = new(MultiMenu.Neutral, "Whisper Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Whisperer);
        WhisperCdIncreases = new(MultiMenu.Neutral, "Whisper Cooldown Increases", false, Whisperer);
        WhisperCdIncrease = new(MultiMenu.Neutral, "Whisper Cooldown Increases By", 5f, 2.5f, 30f, 2.5f, CooldownFormat, WhisperCdIncreases);
        WhisperRadius = new(MultiMenu.Neutral, "Whisper Radius", 1.5f, 0.5f, 5f, 0.25f, DistanceFormat, Whisperer);
        WhisperRate = new(MultiMenu.Neutral, "Whisper Rate", 5, 5, 50, 5, PercentFormat, Whisperer);
        WhisperRateDecreases = new(MultiMenu.Neutral, "Whisper Rate Decreases", false, Whisperer);
        WhisperRateDecrease = new(MultiMenu.Neutral, "Whisper Rate Decreases By", 5, 5, 50, 5, PercentFormat, WhisperRateDecreases);
        WhispVent = new(MultiMenu.Neutral, "<color=#2D6AA5FF>Whisperer</color> Can Vent", false, Whisperer);
        PersuadedVent = new(MultiMenu.Neutral, "Persuaded Can Vent", false, Whisperer);

        NPSettings = new(MultiMenu.Neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Proselyte</color> Settings");

        Betrayer = new(MultiMenu.Neutral, "<color=#11806AFF>Betrayer</color>");
        BetrayCd = new(MultiMenu.Neutral, "Betray Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Betrayer);
        BetrayerVent = new(MultiMenu.Neutral, "<color=#11806AFF>Betrayer</color> Can Vent", true, Betrayer);

        Phantom = new(MultiMenu.Neutral, "<color=#662962FF>Phantom</color>");
        PhantomTasksRemaining = new(MultiMenu.Neutral, "Tasks Remaining When <color=#662962FF>Phantom</color> Can Be Clicked", 5, 1, 10, 1, Phantom);
        PhantomPlayersAlerted = new(MultiMenu.Neutral, "Players Are Alerted When <color=#662962FF>Phantom</color> Is Clickable", false, Phantom);

        NeutralSettings.Parents = [Actor, Pestilence, Amnesiac, GuardianAngel, Survivor, Thief, BountyHunter, Cannibal, Executioner, Guesser, Jester, Troll, Plaguebearer, Arsonist,  Glitch,
            Cryomaniac, Juggernaut, Murderer, SerialKiller, Werewolf, Dracula, Necromancer, Whisperer, Betrayer, Phantom];

        NPSettings.Parents = [Betrayer, Phantom];

        IntruderSettings = new(MultiMenu.Intruder, "<color=#FF1919FF>Intruder</color> Settings");
        IntruderCount = new(MultiMenu.Intruder, "<color=#FF1919FF>Intruder</color> Count", 1, 0, 4, 1);
        IntruderVision = new(MultiMenu.Intruder, "<color=#FF1919FF>Intruder</color> Vision", 2f, 0.25f, 5f, 0.25f, MultiplierFormat);
        IntruderFlashlight = new(MultiMenu.Intruder, "<color=#FF1919FF>Intruders</color> Use A Flashlight", false);
        IntKillCd = new(MultiMenu.Intruder, "<color=#FF1919FF>Intruder</color> Kill Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
        IntrudersVent = new(MultiMenu.Intruder, "<color=#FF1919FF>Intruder</color> Can Vent", true);
        IntrudersCanSabotage = new(MultiMenu.Intruder, "<color=#FF1919FF>Intruder</color> Can Sabotage", true);
        GhostsCanSabotage = new(MultiMenu.Intruder, "Dead <color=#FF1919FF>Intruders</color> Can Sabotage", false);
        IntruderMax = new(MultiMenu.Intruder, "Max <color=#FF1919FF>Intruder</color> <color=#FFD700FF>Roles</color>", 5, 1, 14, 1, [GameMode.Classic, GameMode.AllAny, GameMode.Custom]);
        IntruderMin = new(MultiMenu.Intruder, "Min <color=#FF1919FF>Intruder</color> <color=#FFD700FF>Roles</color>", 5, 1, 14, 1, [GameMode.Classic, GameMode.AllAny, GameMode.Custom]);

        ICRoles = new(MultiMenu.Intruder, "<color=#FF1919FF>Intruder</color> <color=#1D7CF2FF>Concealing</color> <color=#FFD700FF>Roles</color>", [GameMode.Classic, GameMode.AllAny,
            GameMode.KillingOnly, GameMode.Custom]);
        BlackmailerOn = new(MultiMenu.Intruder, "<color=#02A752FF>Blackmailer</color>", parent: ICRoles);
        CamouflagerOn = new(MultiMenu.Intruder, "<color=#378AC0FF>Camouflager</color>", parent: ICRoles);
        GrenadierOn = new(MultiMenu.Intruder, "<color=#85AA5BFF>Grenadier</color>", parent: ICRoles);
        JanitorOn = new(MultiMenu.Intruder, "<color=#2647A2FF>Janitor</color>", parent: ICRoles);

        IDRoles = new(MultiMenu.Intruder, "<color=#FF1919FF>Intruder</color> <color=#1D7CF2FF>Deception</color> <color=#FFD700FF>Roles</color>", [GameMode.Classic, GameMode.AllAny,
            GameMode.KillingOnly, GameMode.Custom]);
        DisguiserOn = new(MultiMenu.Intruder, "<color=#40B4FFFF>Disguiser</color>", parent: IDRoles);
        MorphlingOn = new(MultiMenu.Intruder, "<color=#BB45B0FF>Morphling</color>", parent: IDRoles);
        WraithOn = new(MultiMenu.Intruder, "<color=#5C4F75FF>Wraith</color>", parent: IDRoles);

        IHRoles = new(MultiMenu.Intruder, "<color=#FF1919FF>Intruder</color> <color=#1D7CF2FF>Head</color> <color=#FFD700FF>Roles</color>", [GameMode.Classic, GameMode.AllAny,
            GameMode.KillingOnly, GameMode.Custom]);
        GodfatherOn = new(MultiMenu.Intruder, "<color=#404C08FF>Godfather</color>", parent: IHRoles);

        IKRoles = new(MultiMenu.Intruder, "<color=#FF1919FF>Intruder</color> <color=#1D7CF2FF>Killing</color> <color=#FFD700FF>Roles</color>", [GameMode.Classic, GameMode.AllAny,
            GameMode.KillingOnly, GameMode.Custom]);
        AmbusherOn = new(MultiMenu.Intruder, "<color=#2BD29CFF>Ambusher</color>", parent: IKRoles);
        EnforcerOn = new(MultiMenu.Intruder, "<color=#005643FF>Enforcer</color>", parent: IKRoles);

        ISRoles = new(MultiMenu.Intruder, "<color=#FF1919FF>Intruder</color> <color=#1D7CF2FF>Support</color> <color=#FFD700FF>Roles</color>", [GameMode.Classic, GameMode.AllAny,
            GameMode.KillingOnly, GameMode.Custom]);
        ConsigliereOn = new(MultiMenu.Intruder, "<color=#FFFF99FF>Consigliere</color>", parent: ISRoles);
        ConsortOn = new(MultiMenu.Intruder, "<color=#801780FF>Consort</color>", parent: ISRoles);
        MinerOn = new(MultiMenu.Intruder, "<color=#AA7632FF>Miner</color>", parent: ISRoles);
        TeleporterOn = new(MultiMenu.Intruder, "<color=#939593FF>Teleporter</color>", parent: ISRoles);

        IURoles = new(MultiMenu.Intruder, "<color=#FF1919FF>Intruder</color> <color=#1D7CF2FF>Utility</color> <color=#FFD700FF>Roles</color>", [GameMode.Classic, GameMode.AllAny,
            GameMode.KillingOnly, GameMode.Custom]);
        GhoulOn = new(MultiMenu.Intruder, "<color=#F1C40FFF>Ghoul</color>", parent: IURoles);
        ImpostorOn = new(MultiMenu.Intruder, "<color=#FF1919FF>Impostor</color>", parent: [IURoles, GameMode.Custom], all: true);

        ICSettings = new(MultiMenu.Intruder, "<color=#FF1919FF>Intruder</color> <color=#1D7CF2FF>Concealing</color> Settings", [BlackmailerOn, CamouflagerOn, GrenadierOn, JanitorOn,
            LayerEnum.Blackmailer, LayerEnum.Camouflager, LayerEnum.Grenadier, LayerEnum.Janitor, LayerEnum.IntruderConceal, LayerEnum.RandomIntruder, LayerEnum.RegularIntruder]);
        ICMax = new(MultiMenu.Intruder, "Max <color=#FF1919FF>Intruder</color> <color=#1D7CF2FF>Concealers</color>", 1, 1, 14, 1, ICSettings);

        Blackmailer = new(MultiMenu.Intruder, "<color=#02A752FF>Blackmailer</color>", [BlackmailerOn, LayerEnum.Blackmailer, LayerEnum.IntruderConceal, LayerEnum.RandomIntruder,
            LayerEnum.RegularIntruder]);
        UniqueBlackmailer = new(MultiMenu.Intruder, "<color=#02A752FF>Blackmailer</color> Is Unique", false, [Blackmailer, EnableUniques], true);
        BlackmailCd = new(MultiMenu.Intruder, "Blackmail Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Blackmailer);
        WhispersNotPrivate = new(MultiMenu.Intruder, "<color=#02A752FF>Blackmailer</color> Can Read Whispers", true, Blackmailer);
        BlackmailMates = new(MultiMenu.Intruder, "<color=#02A752FF>Blackmailer</color> Can Blackmail Teammates", false, Blackmailer);
        BMRevealed = new(MultiMenu.Intruder, "Blackmail Is Revealed To Everyone", true, Blackmailer);

        Camouflager = new(MultiMenu.Intruder, "<color=#378AC0FF>Camouflager</color>", [CamouflagerOn, LayerEnum.Camouflager, LayerEnum.IntruderConceal, LayerEnum.RandomIntruder,
            LayerEnum.RegularIntruder]);
        UniqueCamouflager = new(MultiMenu.Intruder, "<color=#378AC0FF>Camouflager</color> Is Unique", false, [Camouflager, EnableUniques], true);
        CamouflageCd = new(MultiMenu.Intruder, "Camouflage Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Camouflager);
        CamouflageDur = new(MultiMenu.Intruder, "Camouflage Duration", 10f, 5f, 30f, 1f, CooldownFormat, Camouflager);
        CamoHideSize = new(MultiMenu.Intruder, "Camouflage Hides Player Size", false, Camouflager);
        CamoHideSpeed = new(MultiMenu.Intruder, "Camouflage Hides Player Speed", false, Camouflager);

        CamouflagedMeetings.Parents = [CamouflagedComms, Camouflager];

        Grenadier = new(MultiMenu.Intruder, "<color=#85AA5BFF>Grenadier</color>", [GrenadierOn, LayerEnum.Grenadier, LayerEnum.IntruderConceal, LayerEnum.RandomIntruder,
            LayerEnum.RegularIntruder]);
        UniqueGrenadier = new(MultiMenu.Intruder, "<color=#85AA5BFF>Grenadier</color> Is Unique", false, [Grenadier, EnableUniques], true);
        FlashCd = new(MultiMenu.Intruder, "Flash Grenade Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Grenadier);
        FlashDur = new(MultiMenu.Intruder, "Flash Grenade Duration", 10f, 5f, 30f, 1f, CooldownFormat, Grenadier);
        FlashRadius = new(MultiMenu.Intruder, "Flash Radius", 4.5f, 0.5f, 10f, 0.5f, DistanceFormat, Grenadier);
        GrenadierIndicators = new(MultiMenu.Intruder, "Indicate Flashed Players", false, Grenadier);
        SaboFlash = new(MultiMenu.Intruder, "<color=#85AA5BFF>Grenadier</color> Can Flash During Sabotages", false, Grenadier);
        GrenadierVent = new(MultiMenu.Intruder, "<color=#85AA5BFF>Grenadier</color> Can Vent", false, Grenadier);

        Janitor = new(MultiMenu.Intruder, "<color=#2647A2FF>Janitor</color>", [JanitorOn, LayerEnum.Janitor, LayerEnum.IntruderConceal, LayerEnum.RandomIntruder,
            LayerEnum.RegularIntruder]);
        UniqueJanitor = new(MultiMenu.Intruder, "<color=#2647A2FF>Janitor</color> Is Unique", false, [Janitor, EnableUniques], true);
        CleanCd = new(MultiMenu.Intruder, "Clean Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Janitor);
        JaniCooldownsLinked = new(MultiMenu.Intruder, "Kill And Clean Cooldowns Are Linked", true, Janitor);
        SoloBoost = new(MultiMenu.Intruder, "<color=#2647A2FF>Janitor</color> Has Lower Cooldown When Solo", false, Janitor);
        DragCd = new(MultiMenu.Intruder, "Drag Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Janitor);
        DragModifier = new(MultiMenu.Intruder, "Drag Speed", 0.5f, 0.25f, 3f, 0.25f, MultiplierFormat, Janitor);
        JanitorVentOptions = new(MultiMenu.Intruder, "<color=#2647A2FF>Janitor</color> Can Vent", ["Never", "Body", "Bodyless", "Always"], Janitor);

        IDSettings = new(MultiMenu.Intruder, "<color=#FF1919FF>Intruder</color> <color=#1D7CF2FF>Deception</color> Settings", [DisguiserOn, MorphlingOn, WraithOn, LayerEnum.Disguiser,
            LayerEnum.Morphling, LayerEnum.Wraith, LayerEnum.IntruderDecep, LayerEnum.RandomIntruder, LayerEnum.RegularIntruder]);
        IDMax = new(MultiMenu.Intruder, "Max <color=#FF1919FF>Intruder</color> <color=#1D7CF2FF>Deceivers</color>", 1, 1, 14, 1, IDSettings);

        Disguiser = new(MultiMenu.Intruder, "<color=#40B4FFFF>Disguiser</color>", [DisguiserOn, LayerEnum.Disguiser, LayerEnum.IntruderDecep, LayerEnum.RandomIntruder,
            LayerEnum.RegularIntruder]);
        UniqueDisguiser = new(MultiMenu.Intruder, "<color=#40B4FFFF>Disguiser</color> Is Unique", false, [Disguiser, EnableUniques], true);
        DisguiseCd = new(MultiMenu.Intruder, "Disguise Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Disguiser);
        DisguiseDelay = new(MultiMenu.Intruder, "Delay Before Disguising", 5f, 2.5f, 15f, 2.5f, CooldownFormat, Disguiser);
        DisguiseDur = new(MultiMenu.Intruder, "Disguise Duration", 10f, 5f, 30f, 2.5f, CooldownFormat, Disguiser);
        MeasureCd = new(MultiMenu.Intruder, "Measure Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Disguiser);
        DisgCooldownsLinked = new(MultiMenu.Intruder, "Measure And Disguise Cooldowns Are Linked", false, Disguiser);
        DisguiseTarget = new(MultiMenu.Intruder, "<color=#40B4FFFF>Disguiser</color> Can Disguise", ["Everyone", "Only Intruders", "Non Intruders"], Disguiser);

        Morphling = new(MultiMenu.Intruder, "<color=#BB45B0FF>Morphling</color>", [MorphlingOn, LayerEnum.Morphling, LayerEnum.IntruderDecep, LayerEnum.RandomIntruder,
            LayerEnum.RegularIntruder]);
        UniqueMorphling = new(MultiMenu.Intruder, "<color=#BB45B0FF>Morphling</color> Is Unique", false, [Morphling, EnableUniques], true);
        MorphCd = new(MultiMenu.Intruder, "Morph Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Morphling);
        MorphDur = new(MultiMenu.Intruder, "Morph Duration", 10f, 5f, 30f, 1f, CooldownFormat, Morphling);
        SampleCd = new(MultiMenu.Intruder, "Sample Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Morphling);
        MorphCooldownsLinked = new(MultiMenu.Intruder, "Sample And Morph Cooldowns Are Linked", false, Morphling);
        MorphlingVent = new(MultiMenu.Intruder, "<color=#BB45B0FF>Morphling</color> Can Vent", false, Morphling);

        Wraith = new(MultiMenu.Intruder, "<color=#5C4F75FF>Wraith</color>", [WraithOn, LayerEnum.Wraith, LayerEnum.IntruderDecep, LayerEnum.RandomIntruder, LayerEnum.RegularIntruder]);
        UniqueWraith = new(MultiMenu.Intruder, "<color=#5C4F75FF>Wraith</color> Is Unique", false, [Wraith, EnableUniques], true);
        InvisCd = new(MultiMenu.Intruder, "Invis Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Wraith);
        InvisDur = new(MultiMenu.Intruder, "Invis Duration", 10f, 5f, 30f, 1f, CooldownFormat, Wraith);
        WraithVent = new(MultiMenu.Intruder, "<color=#5C4F75FF>Wraith</color> Can Vent", false, Wraith);

        IHSettings = new(MultiMenu.Intruder, "<color=#FF1919FF>Intruder</color> <color=#1D7CF2FF>Support</color> Settings", [GodfatherOn, LayerEnum.Godfather, LayerEnum.RandomIntruder,
            LayerEnum.IntruderHead]);
        IHMax = new(MultiMenu.Intruder, "Max <color=#FF1919FF>Intruder</color> <color=#1D7CF2FF>Heads</color>", 1, 1, 14, 1, IHSettings);

        Godfather = new(MultiMenu.Intruder, "<color=#404C08FF>Godfather</color>", [GodfatherOn, LayerEnum.Godfather, LayerEnum.IntruderHead, LayerEnum.RandomIntruder]);
        UniqueGodfather = new(MultiMenu.Intruder, "<color=#404C08FF>Godfather</color> Is Unique", false, [Godfather, EnableUniques], true);
        GFPromotionCdDecrease = new(MultiMenu.Intruder, "Ability Cooldown Bonus", 0.75f, 0.25f, 0.9f, 0.05f, MultiplierFormat, Godfather);

        IKSettings = new(MultiMenu.Intruder, "<color=#FF1919FF>Impostor</color> <color=#1D7CF2FF>Killing</color> Settings", [AmbusherOn, EnforcerOn, LayerEnum.Ambusher, LayerEnum.Enforcer,
            LayerEnum.RandomIntruder, LayerEnum.IntruderKill, LayerEnum.RegularIntruder]);
        IKMax = new(MultiMenu.Intruder, "Max <color=#FF1919FF>Intruder</color> <color=#1D7CF2FF>Killers</color>", 1, 1, 14, 1, IKSettings);

        Ambusher = new(MultiMenu.Intruder, "<color=#2BD29CFF>Ambusher</color>", [AmbusherOn, LayerEnum.Ambusher, LayerEnum.IntruderKill, LayerEnum.RandomIntruder,
            LayerEnum.RegularIntruder]);
        UniqueAmbusher = new(MultiMenu.Intruder, "<color=#2BD29CFF>Ambusher</color> Is Unique", false, [Ambusher, EnableUniques], true);
        AmbushCd = new(MultiMenu.Intruder, "Ambush Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Ambusher);
        AmbushDur = new(MultiMenu.Intruder, "Ambush Duration", 10f, 5f, 30f, 1f, CooldownFormat, Ambusher);
        AmbushMates = new(MultiMenu.Intruder, "<color=#2BD29CFF>Ambusher</color> Can Ambush Teammates", false, Ambusher);

        Enforcer = new(MultiMenu.Intruder, "<color=#005643FF>Enforcer</color>", [EnforcerOn, LayerEnum.Enforcer, LayerEnum.IntruderKill, LayerEnum.RandomIntruder,
            LayerEnum.RegularIntruder]);
        UniqueEnforcer = new(MultiMenu.Intruder, "<color=#005643FF>Enforcer</color> Is Unique", false, [Enforcer, EnableUniques], true);
        EnforceCd = new(MultiMenu.Intruder, "Enforce Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Enforcer);
        EnforceDur = new(MultiMenu.Intruder, "Enforce Duration", 10f, 5f, 30f, 1f, CooldownFormat, Enforcer);
        EnforceDelay = new(MultiMenu.Intruder, "Enforce Delay", 5f, 1f, 15f, 1f, CooldownFormat, Enforcer);
        EnforceRadius = new(MultiMenu.Intruder, "Enforce Explosion Radius", 1.5f, 0.5f, 5f, 0.25f, DistanceFormat, Enforcer);

        ISSettings = new(MultiMenu.Intruder, "<color=#FF1919FF>Intruder</color> <color=#1D7CF2FF>Support</color> Settings", [ConsigliereOn, ConsortOn, GodfatherOn, MinerOn, TeleporterOn,
            LayerEnum.Consigliere, LayerEnum.Consort, LayerEnum.Godfather, LayerEnum.Miner, LayerEnum.Teleporter, LayerEnum.RegularIntruder]);
        ISMax = new(MultiMenu.Intruder, "Max <color=#FF1919FF>Intruder</color> <color=#1D7CF2FF>Supporters</color>", 1, 1, 14, 1, ISSettings);

        Consigliere = new(MultiMenu.Intruder, "<color=#FFFF99FF>Consigliere</color>", [ConsigliereOn, LayerEnum.Consigliere, LayerEnum.IntruderSupport, LayerEnum.RandomIntruder,
            LayerEnum.RegularIntruder]);
        UniqueConsigliere = new(MultiMenu.Intruder, "<color=#FFFF99FF>Consigliere</color> Is Unique", false, [Consigliere, EnableUniques], true);
        InvestigateCd = new(MultiMenu.Intruder, "Investigate Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Consigliere);
        ConsigInfo = new(MultiMenu.Intruder, "Info That <color=#FFFF99FF>Consigliere</color> Sees", ["Role", "Faction"], Consigliere);

        Consort = new(MultiMenu.Intruder, "<color=#801780FF>Consort</color>", [ConsortOn, LayerEnum.Consort, LayerEnum.IntruderSupport, LayerEnum.RandomIntruder,
            LayerEnum.RegularIntruder]);
        UniqueConsort = new(MultiMenu.Intruder, "<color=#801780FF>Consort</color> Is Unique", false, [Consort, EnableUniques], true);
        ConsortCd = new(MultiMenu.Intruder, "<color=#801780FF>Consort</color> Roleblock Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Consort);
        ConsortDur = new(MultiMenu.Intruder, "<color=#801780FF>Consort</color> Roleblock Duration", 10f, 5f, 30f, 1f, CooldownFormat, Consort);

        Miner = new(MultiMenu.Intruder, "<color=#AA7632FF>Miner</color>", [MinerOn, LayerEnum.Miner, LayerEnum.IntruderSupport, LayerEnum.RegularIntruder, LayerEnum.RandomIntruder]);
        UniqueMiner = new(MultiMenu.Intruder, "<color=#AA7632FF>Miner</color> Is Unique", false, [Miner, EnableUniques], true);
        MineCd = new(MultiMenu.Intruder, "Mine Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Miner);

        Teleporter = new(MultiMenu.Intruder, "<color=#939593FF>Teleporter</color>", [TeleporterOn, LayerEnum.Teleporter, LayerEnum.IntruderSupport, LayerEnum.RandomIntruder,
            LayerEnum.RegularIntruder]);
        UniqueTeleporter = new(MultiMenu.Intruder, "<color=#939593FF>Teleporter</color> Is Unique", false, [Teleporter, EnableUniques], true);
        TeleportCd = new(MultiMenu.Intruder, "Teleport Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Teleporter);
        TeleMarkCd = new(MultiMenu.Intruder, "<color=#939593FF>Teleporter</color> Mark Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Teleporter);
        TeleCooldownsLinked = new(MultiMenu.Intruder, "Mark And Teleport Cooldowns Are Linked", false, Teleporter);
        TeleVent = new(MultiMenu.Intruder, "<color=#939593FF>Teleporter</color> Can Vent", false, Teleporter);

        IUSettings = new(MultiMenu.Intruder, "<color=#FF1919FF>Intruder</color> <color=#1D7CF2FF>Utility</color> Settings");

        Ghoul = new(MultiMenu.Intruder, "<color=#F1C40FFF>Ghoul</color>");
        GhoulMarkCd = new(MultiMenu.Intruder, "<color=#F1C40FFF>Ghoul</color> Mark Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Ghoul);

        IUSettings.Parents = [Ghoul];

        SyndicateSettings = new(MultiMenu.Syndicate, "<color=#008000FF>Syndicate</color> Settings");
        SyndicateCount = new(MultiMenu.Syndicate, "<color=#008000FF>Syndicate</color> Count", 1, 0, 4, 1);
        SyndicateVision = new(MultiMenu.Syndicate, "<color=#008000FF>Syndicate</color> Vision", 2f, 0.25f, 5f, 0.25f, MultiplierFormat);
        SyndicateFlashlight = new(MultiMenu.Syndicate, "<color=#008000FF>Syndicate</color> Uses A Flashlight", false);
        ChaosDriveMeetingCount = new(MultiMenu.Syndicate, "Chaos Drive Timer", 3, 1, 10, 1);
        CDKillCd = new(MultiMenu.Syndicate, "Chaos Drive Holder Kill Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
        SyndicateVent = new(MultiMenu.Syndicate, "<color=#008000FF>Syndicate</color> Can Vent", ["Always", "Chaos Drive", "Never"]);
        AltImps = new(MultiMenu.Syndicate, "<color=#008000FF>Syndicate</color> Replaces <color=#FF1919FF>Intruders</color>", false);
        GlobalDrive = new(MultiMenu.Syndicate, "Chaos Drive Is Global", false);
        SyndicateMax = new(MultiMenu.Syndicate, "Max <color=#008000FF>Syndicate</color> <color=#FFD700FF>Roles</color>", 5, 1, 14, 1, [GameMode.Classic, GameMode.AllAny, GameMode.Custom]);
        SyndicateMin = new(MultiMenu.Syndicate, "Min <color=#008000FF>Syndicate</color> <color=#FFD700FF>Roles</color>", 5, 1, 14, 1, [GameMode.Classic, GameMode.AllAny, GameMode.Custom]);

        SDRoles = new(MultiMenu.Syndicate, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Disruption</color> <color=#FFD700FF>Roles</color>", [GameMode.Classic, GameMode.AllAny,
            GameMode.KillingOnly, GameMode.Custom]);
        ConcealerOn = new(MultiMenu.Syndicate, "<color=#C02525FF>Concealer</color>", parent: SDRoles);
        DrunkardOn = new(MultiMenu.Syndicate, "<color=#FF7900FF>Drunkard</color>", parent: SDRoles);
        FramerOn = new(MultiMenu.Syndicate, "<color=#00FFFFFF>Framer</color>", parent: SDRoles);
        ShapeshifterOn = new(MultiMenu.Syndicate, "<color=#2DFF00FF>Shapeshifter</color>", parent: SDRoles);
        SilencerOn = new(MultiMenu.Syndicate, "<color=#AAB43EFF>Silencer</color>", parent: SDRoles);
        TimekeeperOn = new(MultiMenu.Syndicate, "<color=#3769FEFF>Timekeeper</color>", parent: SDRoles);

        SyKRoles = new(MultiMenu.Syndicate, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Killing</color> <color=#FFD700FF>Roles</color>", [GameMode.Classic, GameMode.AllAny,
            GameMode.KillingOnly, GameMode.Custom]);
        BomberOn = new(MultiMenu.Syndicate, "<color=#C9CC3FFF>Bomber</color>", parent: SyKRoles);
        ColliderOn = new(MultiMenu.Syndicate, "<color=#B345FFFF>Collider</color>", parent: SyKRoles);
        CrusaderOn = new(MultiMenu.Syndicate, "<color=#DF7AE8FF>Crusader</color>", parent: SyKRoles);
        PoisonerOn = new(MultiMenu.Syndicate, "<color=#B5004CFF>Poisoner</color>", parent: SyKRoles);

        SPRoles = new(MultiMenu.Syndicate, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Power</color> <color=#FFD700FF>Roles</color>", [GameMode.Classic, GameMode.AllAny,
            GameMode.KillingOnly, GameMode.Custom]);
        RebelOn = new(MultiMenu.Syndicate, "<color=#FFFCCEFF>Rebel</color>", parent: SPRoles);
        SpellslingerOn = new(MultiMenu.Syndicate, "<color=#0028F5FF>Spellslinger</color>", parent: SPRoles);

        SSuRoles = new(MultiMenu.Syndicate, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Support</color> <color=#FFD700FF>Roles</color>", [GameMode.Classic, GameMode.AllAny,
            GameMode.KillingOnly, GameMode.Custom]);
        StalkerOn = new(MultiMenu.Syndicate, "<color=#7E4D00FF>Stalker</color>", parent: SSuRoles);
        WarperOn = new(MultiMenu.Syndicate, "<color=#8C7140FF>Warper</color>", parent: SSuRoles);

        SURoles = new(MultiMenu.Syndicate, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Utility</color> <color=#FFD700FF>Roles</color>", [GameMode.Classic, GameMode.AllAny,
            GameMode.KillingOnly, GameMode.Custom]);
        AnarchistOn = new(MultiMenu.Syndicate, "<color=#008000FF>Anarchist</color>", parent: [SURoles, GameMode.Custom], all: true);
        BansheeOn = new(MultiMenu.Syndicate, "<color=#E67E22FF>Banshee</color>", parent: SURoles);

        SDSettings = new(MultiMenu.Syndicate, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Disruption</color> Settings", [ConcealerOn, DrunkardOn, FramerOn, ShapeshifterOn,
            SilencerOn, LayerEnum.RandomSyndicate, LayerEnum.Concealer, LayerEnum.Drunkard, LayerEnum.Framer, LayerEnum.Shapeshifter, LayerEnum.SyndicateDisrup, TimekeeperOn,
            LayerEnum.Timekeeper]);
        SDMax = new(MultiMenu.Syndicate, "Max <color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Disruptors</color>", 1, 1, 14, 1, SDSettings);

        Concealer = new(MultiMenu.Syndicate, "<color=#C02525FF>Concealer</color>", [ConcealerOn, LayerEnum.Concealer, LayerEnum.SyndicateDisrup, LayerEnum.RandomSyndicate]);
        UniqueConcealer = new(MultiMenu.Syndicate, "<color=#C02525FF>Concealer</color> Is Unique", false, [Concealer, EnableUniques], true);
        ConcealCd = new(MultiMenu.Syndicate, "Conceal Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Concealer);
        ConcealDur = new(MultiMenu.Syndicate, "Conceal Duration", 10f, 5f, 30f, 1f, CooldownFormat, Concealer);
        ConcealMates = new(MultiMenu.Syndicate, "<color=#C02525FF>Concealer</color> Can Conceal Teammates", false, Concealer);

        Drunkard = new(MultiMenu.Syndicate, "<color=#FF7900FF>Drunkard</color>", [DrunkardOn, LayerEnum.Drunkard, LayerEnum.SyndicateDisrup, LayerEnum.RandomSyndicate]);
        UniqueDrunkard = new(MultiMenu.Syndicate, "<color=#FF7900FF>Drunkard</color> Is Unique", false, [Drunkard, EnableUniques], true);
        ConfuseCd = new(MultiMenu.Syndicate, "Confuse Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Drunkard);
        ConfuseDur = new(MultiMenu.Syndicate, "Confuse Duration", 10f, 5f, 30f, 1f, CooldownFormat, Drunkard);
        ConfuseImmunity = new(MultiMenu.Syndicate, "<color=#008000FF>Syndicate</color> Are Immune To Confuse", true, Drunkard);

        Framer = new(MultiMenu.Syndicate, "<color=#00FFFFFF>Framer</color>", [FramerOn, LayerEnum.Framer, LayerEnum.SyndicateDisrup, LayerEnum.RandomSyndicate]);
        UniqueFramer = new(MultiMenu.Syndicate, "<color=#00FFFFFF>Framer</color> Is Unique", false, [Framer, EnableUniques], true);
        FrameCd = new(MultiMenu.Syndicate, "Frame Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Framer);
        ChaosDriveFrameRadius = new(MultiMenu.Syndicate, "Chaos Drive Frame Radius", 1.5f, 0.5f, 5f, 0.25f, DistanceFormat, Framer);

        Shapeshifter = new(MultiMenu.Syndicate, "<color=#2DFF00FF>Shapeshifter</color>", [ShapeshifterOn, LayerEnum.Shapeshifter, LayerEnum.SyndicateDisrup, LayerEnum.RandomSyndicate]);
        UniqueShapeshifter = new(MultiMenu.Syndicate, "<color=#2DFF00FF>Shapeshifter</color> Is Unique", false, [Shapeshifter, EnableUniques], true);
        ShapeshiftCd = new(MultiMenu.Syndicate, "Shapeshift Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Shapeshifter);
        ShapeshiftDur = new(MultiMenu.Syndicate, "Shapeshift Duration", 10f, 5f, 30f, 1f, CooldownFormat, Shapeshifter);
        ShapeshiftMates = new(MultiMenu.Syndicate, "<color=#2DFF00FF>Shapeshifter</color> Can Shapeshift Teammates", false, Shapeshifter);

        Silencer = new(MultiMenu.Syndicate, "<color=#AAB43EFF>Silencer</color>", [SilencerOn, LayerEnum.Silencer, LayerEnum.SyndicateDisrup, LayerEnum.RandomSyndicate]);
        UniqueSilencer = new(MultiMenu.Syndicate, "<color=#AAB43EFF>Silencer</color> Is Unique", false, [Silencer, EnableUniques], true);
        SilenceCd = new(MultiMenu.Syndicate, "Silence Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Silencer);
        WhispersNotPrivateSilencer = new(MultiMenu.Syndicate, "<color=#AAB43EFF>Silencer</color> Can Read Whispers", true, Silencer);
        SilenceMates = new(MultiMenu.Syndicate, "<color=#AAB43EFF>Silencer</color> Can Silence Teammates", false, Silencer);
        SilenceRevealed = new(MultiMenu.Syndicate, "Silence Is Revealed To Everyone", true, Silencer);

        Timekeeper = new(MultiMenu.Syndicate, "<color=#3769FEFF>Timekeeper</color>", [TimekeeperOn, LayerEnum.Timekeeper, LayerEnum.SyndicateDisrup, LayerEnum.RandomSyndicate]);
        UniqueTimekeeper = new(MultiMenu.Syndicate, "<color=#3769FEFF>Timekeeper</color> Is Unique", false, [Timekeeper, EnableUniques], true);
        TimeCd = new(MultiMenu.Syndicate, "Time Control Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Timekeeper);
        TimeDur = new(MultiMenu.Syndicate, "Time Control Duration", 10f, 5f, 30f, 1f, CooldownFormat, Timekeeper);
        TimeFreezeImmunity = new(MultiMenu.Syndicate, "<color=#008000FF>Syndicate</color> Are Immune To Freeze", true, Timekeeper);
        TimeRewindImmunity = new(MultiMenu.Syndicate, "<color=#008000FF>Syndicate</color> Are Immune To Rewind", true, Timekeeper);

        SyKSettings = new(MultiMenu.Syndicate, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Killing</color> Settings", [BomberOn, ColliderOn, CrusaderOn, PoisonerOn,
            LayerEnum.RandomSyndicate, LayerEnum.Bomber, LayerEnum.Collider, LayerEnum.Crusader, LayerEnum.Poisoner, LayerEnum.SyndicateKill]);
        SyKMax = new(MultiMenu.Syndicate, "Max <color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Killers</color>", 1, 1, 14, 1, SyKSettings);

        Bomber = new(MultiMenu.Syndicate, "<color=#C9CC3FFF>Bomber</color>", [BomberOn, LayerEnum.Bomber, LayerEnum.SyndicateKill, LayerEnum.RandomSyndicate]);
        UniqueBomber = new(MultiMenu.Syndicate, "<color=#C9CC3FFF>Bomber</color> Is Unique", false, [Bomber, EnableUniques], true);
        BombCd = new(MultiMenu.Syndicate, "Bomb Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Bomber);
        DetonateCd = new(MultiMenu.Syndicate, "Detonation Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Bomber);
        BombCooldownsLinked = new(MultiMenu.Syndicate, "Place And Detonate Cooldowns Are Linked", false, Bomber);
        BombsRemoveOnNewRound = new(MultiMenu.Syndicate, "Bombs Are Cleared Every Meeting", false, Bomber);
        BombsDetonateOnMeetingStart = new(MultiMenu.Syndicate, "Bombs Detonate Everytime A Meeting Is Called", false, Bomber);
        BombRange = new(MultiMenu.Syndicate, "Bomb Radius", 1.5f, 0.5f, 5f, 0.25f, DistanceFormat, Bomber);
        ChaosDriveBombRange = new(MultiMenu.Syndicate, "Chaos Drive Bomb Radius Increase", 0.5f, 0.5f, 5f, 0.25f, DistanceFormat, Bomber);

        Collider = new(MultiMenu.Syndicate, "<color=#B345FFFF>Collider</color>", [ColliderOn, LayerEnum.Collider, LayerEnum.SyndicateKill, LayerEnum.RandomSyndicate]);
        UniqueCollider = new(MultiMenu.Syndicate, "<color=#B345FFFF>Collider</color> Is Unique", false, [Collider, EnableUniques], true);
        CollideCd = new(MultiMenu.Syndicate, "Set Charges Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Collider);
        ChargeCd = new(MultiMenu.Syndicate, "Charge Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Collider);
        ChargeDur = new(MultiMenu.Syndicate, "Charge Duration", 10f, 5f, 30f, 1f, CooldownFormat, Collider);
        CollideRange = new(MultiMenu.Syndicate, "Collide Radius", 1.5f, 0.5f, 5f, 0.25f, DistanceFormat, Collider);
        CollideRangeIncrease = new(MultiMenu.Syndicate, "Chaos Drive Collide Radius Increase", 0.5f, 0.5f, 5f, 0.25f, DistanceFormat, Collider);
        ChargeCooldownsLinked = new(MultiMenu.Syndicate, "Charge Cooldowns Are Linked", false, Collider);
        CollideResetsCooldown = new(MultiMenu.Syndicate, "Collision Resets Charge Cooldowns", false, Collider);

        Crusader = new(MultiMenu.Syndicate, "<color=#DF7AE8FF>Crusader</color>", [CrusaderOn, LayerEnum.Crusader, LayerEnum.SyndicateKill, LayerEnum.RandomSyndicate]);
        UniqueCrusader = new(MultiMenu.Syndicate, "<color=#DF7AE8FF>Crusader</color> Is Unique", false, [Crusader, EnableUniques], true);
        CrusadeCd = new(MultiMenu.Syndicate, "Crusade Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Crusader);
        CrusadeDur = new(MultiMenu.Syndicate, "Crusade Duration", 10f, 5f, 30f, 1f, CooldownFormat, Crusader);
        ChaosDriveCrusadeRadius = new(MultiMenu.Syndicate, "Chaos Drive Crusade Radius", 1.5f, 0.5f, 5f, 0.25f, DistanceFormat, Crusader);
        CrusadeMates = new(MultiMenu.Syndicate, "<color=#DF7AE8FF>Crusader</color> Can Crusade Teammates", false, Crusader);

        Poisoner = new(MultiMenu.Syndicate, "<color=#B5004CFF>Poisoner</color>", [PoisonerOn, LayerEnum.Poisoner, LayerEnum.SyndicateKill, LayerEnum.RandomSyndicate]);
        UniquePoisoner = new(MultiMenu.Syndicate, "<color=#B5004CFF>Poisoner</color> Is Unique", false, [Poisoner, EnableUniques], true);
        PoisonCd = new(MultiMenu.Syndicate, "Poison Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Poisoner);
        PoisonDur = new(MultiMenu.Syndicate, "Poison Kill Delay", 5f, 1f, 15f, 1f, CooldownFormat, Poisoner);

        SPSettings = new(MultiMenu.Syndicate, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Power</color> Settings", [RebelOn, SpellslingerOn, LayerEnum.Rebel,
            LayerEnum.Spellslinger, LayerEnum.RandomSyndicate, LayerEnum.SyndicatePower]);
        SPMax = new(MultiMenu.Syndicate, "Max <color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Powers</color>", 1, 1, 14, 1, SPSettings);

        Rebel = new(MultiMenu.Syndicate, "<color=#FFFCCEFF>Rebel</color>", [RebelOn, LayerEnum.Rebel, LayerEnum.SyndicatePower, LayerEnum.RandomSyndicate]);
        UniqueRebel = new(MultiMenu.Syndicate, "<color=#FFFCCEFF>Rebel</color> Is Unique", false, [Rebel, EnableUniques], true);
        RebPromotionCdDecrease = new(MultiMenu.Syndicate, "Ability Cooldown Bonus", 0.75f, 0.25f, 0.9f, 0.05f, MultiplierFormat, Rebel);

        Spellslinger = new(MultiMenu.Syndicate, "<color=#0028F5FF>Spellslinger</color>", [SpellslingerOn, LayerEnum.Spellslinger, LayerEnum.SyndicatePower, LayerEnum.RandomSyndicate]);
        UniqueSpellslinger = new(MultiMenu.Syndicate, "<color=#0028F5FF>Spellslinger</color> Is Unique", false, [Spellslinger, EnableUniques], true);
        SpellCd = new(MultiMenu.Syndicate, "Spellbind Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Spellslinger);
        SpellCdIncrease = new(MultiMenu.Syndicate, "Spellbind Cooldown Increases By", 5f, 2.5f, 30f, 2.5f, CooldownFormat, Spellslinger);

        SSuSettings = new(MultiMenu.Syndicate, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Support</color> Settings", [StalkerOn, WarperOn, LayerEnum.Stalker, LayerEnum.Warper,
            LayerEnum.RandomSyndicate, LayerEnum.SyndicateSupport]);
        SSuMax = new(MultiMenu.Syndicate, "Max <color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Supporters</color>", 1, 1, 14, 1, SSuSettings);

        Stalker = new(MultiMenu.Syndicate, "<color=#7E4D00FF>Stalker</color>", [StalkerOn, LayerEnum.Stalker, LayerEnum.SyndicateSupport, LayerEnum.RandomSyndicate]);
        UniqueStalker = new(MultiMenu.Syndicate, "<color=#7E4D00FF>Stalker</color> Is Unique", false, [Stalker, EnableUniques], true);
        StalkCd = new(MultiMenu.Syndicate, "Stalk Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Stalker);

        Warper = new(MultiMenu.Syndicate, "<color=#8C7140FF>Warper</color>", [WarperOn, LayerEnum.Warper, LayerEnum.SyndicateSupport, LayerEnum.RandomSyndicate]);
        UniqueWarper = new(MultiMenu.Syndicate, "<color=#8C7140FF>Warper</color> Is Unique", false, [Warper, EnableUniques], true);
        WarpCd = new(MultiMenu.Syndicate, "Warp Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Warper);
        WarpDur = new(MultiMenu.Syndicate, "Warp Duration", 5f, 1f, 20f, 1f, CooldownFormat, Warper);
        WarpSelf = new(MultiMenu.Syndicate, "<color=#8C7140FF>Warper</color> Can Warp Themselves", true, Warper);

        SUSettings = new(MultiMenu.Syndicate, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Utility</color> Settings");

        Banshee = new(MultiMenu.Syndicate, "<color=#E67E22FF>Banshee</color>");
        ScreamCd = new(MultiMenu.Syndicate, "Scream Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Banshee);
        ScreamDur = new(MultiMenu.Syndicate, "Scream Duration", 10f, 5f, 30f, 1f, CooldownFormat, Banshee);

        SUSettings.Parents = [Banshee];

        Modifiers = new(MultiMenu.Modifier, "<color=#7F7F7FFF>Modifiers</color>", [GameMode.Classic, GameMode.KillingOnly, GameMode.AllAny, GameMode.Custom]);
        AstralOn = new(MultiMenu.Modifier, "<color=#612BEFFF>Astral</color>", parent: Modifiers);
        BaitOn = new(MultiMenu.Modifier, "<color=#00B3B3FF>Bait</color>", parent: Modifiers);
        ColorblindOn = new(MultiMenu.Modifier, "<color=#B34D99FF>Colorblind</color>", parent: Modifiers);
        CowardOn = new(MultiMenu.Modifier, "<color=#456BA8FF>Coward</color>", parent: Modifiers);
        DiseasedOn = new(MultiMenu.Modifier, "<color=#374D1EFF>Diseased</color>", parent: Modifiers);
        DrunkOn = new(MultiMenu.Modifier, "<color=#758000FF>Drunk</color>", parent: Modifiers);
        DwarfOn = new(MultiMenu.Modifier, "<color=#FF8080FF>Dwarf</color>", parent: Modifiers);
        GiantOn = new(MultiMenu.Modifier, "<color=#FFB34DFF>Giant</color>", parent: Modifiers);
        IndomitableOn = new(MultiMenu.Modifier, "<color=#2DE5BEFF>Indomitable</color>", parent: Modifiers);
        ProfessionalOn = new(MultiMenu.Modifier, "<color=#860B7AFF>Professional</color>", parent: Modifiers);
        ShyOn = new(MultiMenu.Modifier, "<color=#1002C5FF>Shy</color>", parent: Modifiers);
        VIPOn = new(MultiMenu.Modifier, "<color=#DCEE85FF>VIP</color>", parent: Modifiers);
        VolatileOn = new(MultiMenu.Modifier, "<color=#FFA60AFF>Volatile</color>", parent: Modifiers);
        YellerOn = new(MultiMenu.Modifier, "<color=#F6AAB7FF>Yeller</color>", parent: Modifiers);

        ModifierSettings = new(MultiMenu.Modifier, "<color=#7F7F7FFF>Modifier</color> Settings", [AstralOn, BaitOn, CowardOn, DiseasedOn, DrunkOn, DwarfOn, GiantOn, ShyOn, VIPOn,
            IndomitableOn, ProfessionalOn, VolatileOn, YellerOn, ColorblindOn]);
        MaxModifiers = new(MultiMenu.Modifier, "Max <color=#7F7F7FFF>Modifiers</color>", 5, 1, 14, 1, ModifierSettings);
        MinModifiers = new(MultiMenu.Modifier, "Min <color=#7F7F7FFF>Modifiers</color>", 5, 1, 14, 1, ModifierSettings);

        Astral = new(MultiMenu.Modifier, "<color=#612BEFFF>Astral</color>", [AstralOn, EnableUniques], true);
        UniqueAstral = new(MultiMenu.Modifier, "<color=#612BEFFF>Astral</color> Is Unique", false, Astral);

        Bait = new(MultiMenu.Modifier, "<color=#00B3B3FF>Bait</color>", BaitOn);
        UniqueBait = new(MultiMenu.Modifier, "<color=#00B3B3FF>Bait</color> Is Unique", false, [Bait, EnableUniques], true);
        BaitKnows = new(MultiMenu.Modifier, "<color=#00B3B3FF>Bait</color> Knows Who They Are", true, Bait);
        BaitMinDelay = new(MultiMenu.Modifier, "Minimum Delay for <color=#00B3B3FF>Bait</color> Self Report", 0f, 0f, 15f, 0.5f, CooldownFormat, Bait);
        BaitMaxDelay = new(MultiMenu.Modifier, "Maximum Delay for <color=#00B3B3FF>Bait</color> Self Report", 1f, 0f, 15f, 0.5f, CooldownFormat, Bait);

        Colorblind = new(MultiMenu.Modifier, "<color=#B34D99FF>Colorblind</color>", [ColorblindOn, EnableUniques], true);
        UniqueColorblind = new(MultiMenu.Modifier, "<color=#B34D99FF>Colorblind</color> Is Unique", false, Colorblind);

        Coward = new(MultiMenu.Modifier, "<color=#456BA8FF>Coward</color>", [CowardOn, EnableUniques], true);
        UniqueCoward = new(MultiMenu.Modifier, "<color=#456BA8FF>Coward</color> Is Unique", false, Coward);

        Diseased = new(MultiMenu.Modifier, "<color=#374D1EFF>Diseased</color>", DiseasedOn);
        UniqueDiseased = new(MultiMenu.Modifier, "<color=#374D1EFF>Diseased</color> Is Unique", false, [Diseased, EnableUniques], true);
        DiseasedKnows = new(MultiMenu.Modifier, "<color=#374D1EFF>Diseased</color> Knows Who They Are", true, Diseased);
        DiseasedKillMultiplier = new(MultiMenu.Modifier, "<color=#374D1EFF>Diseased</color> Kill Multiplier", 3f, 1.5f, 5f, 0.5f, MultiplierFormat, Diseased);

        Drunk = new(MultiMenu.Modifier, "<color=#758000FF>Drunk</color>", DrunkOn);
        UniqueDrunk = new(MultiMenu.Modifier, "<color=#758000FF>Drunk</color> Is Unique", false, [Drunk, EnableUniques], true);
        DrunkControlsSwap = new(MultiMenu.Modifier, "Controls Reverse Over Time", false, Drunk);
        DrunkKnows = new(MultiMenu.Modifier, "<color=#758000FF>Drunk</color> Knows Who They Are", true, Drunk);
        DrunkInterval = new(MultiMenu.Modifier, "Reversed Controls Interval", 10f, 1f, 20f, 1f, CooldownFormat, DrunkControlsSwap);

        Dwarf = new(MultiMenu.Modifier, "<color=#FF8080FF>Dwarf</color>", DwarfOn);
        UniqueDwarf = new(MultiMenu.Modifier, "<color=#FF8080FF>Dwarf</color> Is Unique", false, [Dwarf, EnableUniques], true);
        DwarfSpeed = new(MultiMenu.Modifier, "<color=#FF8080FF>Dwarf</color> Speed", 1.5f, 1f, 2f, 0.05f, MultiplierFormat, Dwarf);
        DwarfScale = new(MultiMenu.Modifier, "<color=#FF8080FF>Dwarf</color> Scale", 0.5f, 0.3f, 1f, 0.025f, MultiplierFormat, Dwarf);

        Giant = new(MultiMenu.Modifier, "<color=#FFB34DFF>Giant</color>", GiantOn);
        UniqueGiant = new(MultiMenu.Modifier, "<color=#FFB34DFF>Giant</color> Is Unique", false, [Giant, EnableUniques], true);
        GiantSpeed = new(MultiMenu.Modifier, "<color=#FFB34DFF>Giant</color> Speed", 0.75f, 0.5f, 1f, 0.05f, MultiplierFormat, Giant);
        GiantScale = new(MultiMenu.Modifier, "<color=#FFB34DFF>Giant</color> Scale", 1.5f, 1f, 3, 0.025f, MultiplierFormat, Giant);

        Indomitable = new(MultiMenu.Modifier, "<color=#2DE5BEFF>Indomitable</color>", IndomitableOn);
        UniqueIndomitable = new(MultiMenu.Modifier, "<color=#2DE5BEFF>Indomitable</color> Is Unique", false, [Indomitable, EnableUniques], true);
        IndomitableKnows = new(MultiMenu.Modifier, "<color=#2DE5BEFF>Indomitable</color> Knows Who They Are", true, Indomitable);

        Professional = new(MultiMenu.Modifier, "<color=#860B7AFF>Professional</color>", ProfessionalOn);
        UniqueProfessional = new(MultiMenu.Modifier, "<color=#860B7AFF>Professional</color> Is Unique", false, [Professional, EnableUniques], true);
        ProfessionalKnows = new(MultiMenu.Modifier, "<color=#860B7AFF>Professional</color> Knows Who They Are", true, Professional);

        Shy = new(MultiMenu.Modifier, "<color=#1002C5FF>Shy</color>", [ShyOn, EnableUniques], true);
        UniqueShy = new(MultiMenu.Modifier, "<color=#1002C5FF>Shy</color> Is Unique", false, Shy);

        VIP = new(MultiMenu.Modifier, "<color=#DCEE85FF>VIP</color>", VIPOn);
        UniqueVIP = new(MultiMenu.Modifier, "<color=#DCEE85FF>VIP</color> Is Unique", false, [VIP, EnableUniques], true);
        VIPKnows = new(MultiMenu.Modifier, "<color=#DCEE85FF>VIP</color> Knows Who They Are", true, VIP);

        Volatile = new(MultiMenu.Modifier, "<color=#FFA60AFF>Volatile</color>", VolatileOn);
        UniqueVolatile = new(MultiMenu.Modifier, "<color=#FFA60AFF>Volatile</color> Is Unique", false, [Volatile, EnableUniques], true);
        VolatileInterval = new(MultiMenu.Modifier, "<color=#FFA60AFF>Volatile</color> Interval", 15f, 10f, 30f, 1f, CooldownFormat, Volatile);
        VolatileKnows = new(MultiMenu.Modifier, "<color=#FFA60AFF>Volatile</color> Knows Who They Are", true, Volatile);

        Yeller = new(MultiMenu.Modifier, "<color=#F6AAB7FF>Yeller</color>", [YellerOn, EnableUniques], true);
        UniqueYeller = new(MultiMenu.Modifier, "<color=#F6AAB7FF>Yeller</color> Is Unique", false, Yeller);

        Abilities = new(MultiMenu.Ability, "<color=#FF9900FF>Abilities</color>", [GameMode.Classic, GameMode.KillingOnly, GameMode.AllAny, GameMode.Custom]);
        ButtonBarryOn = new(MultiMenu.Ability, "<color=#E600FFFF>Button Barry</color>", parent: Abilities);
        CrewAssassinOn = new(MultiMenu.Ability, "<color=#8CFFFFFF>Bullseye</color>", parent: Abilities);
        IntruderAssassinOn = new(MultiMenu.Ability, "<color=#FF1919FF>Hitman</color>", parent: Abilities);
        InsiderOn = new(MultiMenu.Ability, "<color=#26FCFBFF>Insider</color>", parent: Abilities);
        MultitaskerOn = new(MultiMenu.Ability, "<color=#FF804DFF>Multitasker</color>", parent: Abilities);
        NinjaOn = new(MultiMenu.Ability, "<color=#A84300FF>Ninja</color>", parent: Abilities);
        PoliticianOn = new(MultiMenu.Ability, "<color=#CCA3CCFF>Politician</color>", parent: Abilities);
        RadarOn = new(MultiMenu.Ability, "<color=#FF0080FF>Radar</color>", parent: Abilities);
        RuthlessOn = new(MultiMenu.Ability, "<color=#2160DDFF>Ruthless</color>", parent: Abilities);
        SnitchOn = new(MultiMenu.Ability, "<color=#D4AF37FF>Snitch</color>", parent: Abilities);
        SwapperOn = new(MultiMenu.Ability, "<color=#66E666FF>Swapper</color>", parent: Abilities);
        NeutralAssassinOn = new(MultiMenu.Ability, "<color=#B3B3B3FF>Slayer</color>", parent: Abilities);
        SyndicateAssassinOn = new(MultiMenu.Ability, "<color=#008000FF>Sniper</color>", parent: Abilities);
        TiebreakerOn = new(MultiMenu.Ability, "<color=#99E699FF>Tiebreaker</color>", parent: Abilities);
        TorchOn = new(MultiMenu.Ability, "<color=#FFFF99FF>Torch</color>", parent: Abilities);
        TunnelerOn = new(MultiMenu.Ability, "<color=#E91E63FF>Tunneler</color>", parent: Abilities);
        UnderdogOn = new(MultiMenu.Ability, "<color=#841A7FFF>Underdog</color>", parent: Abilities);

        AbilitySettings = new(MultiMenu.Ability, "<color=#FF9900FF>Ability</color> Settings", [CrewAssassinOn, NeutralAssassinOn, IntruderAssassinOn, SyndicateAssassinOn, NinjaOn,
            ButtonBarryOn, InsiderOn, MultitaskerOn, PoliticianOn, RadarOn, RuthlessOn, SnitchOn, SwapperOn, TiebreakerOn, TunnelerOn, UnderdogOn]);
        MaxAbilities = new(MultiMenu.Ability, "Max <color=#FF9900FF>Abilities</color>", 5, 1, 14, 1, AbilitySettings);
        MinAbilities = new(MultiMenu.Ability, "Min <color=#FF9900FF>Abilities</color>", 5, 1, 14, 1, AbilitySettings);

        Assassin = new(MultiMenu.Ability, "<color=#073763FF>Assassin</color>", [CrewAssassinOn, NeutralAssassinOn, IntruderAssassinOn, SyndicateAssassinOn]);
        UniqueCrewAssassin = new(MultiMenu.Ability, "<color=#8CFFFFFF>Bullseye</color> Is Unique", false, [CrewAssassinOn, EnableUniques], true);
        UniqueNeutralAssassin = new(MultiMenu.Ability, "<color=#B3B3B3FF>Slayer</color> Is Unique", false, [NeutralAssassinOn, EnableUniques], true);
        UniqueIntruderAssassin = new(MultiMenu.Ability, "<color=#FF1919FF>Hitman</color> Is Unique", false, [IntruderAssassinOn, EnableUniques], true);
        UniqueSyndicateAssassin = new(MultiMenu.Ability, "<color=#008000FF>Sniper</color> Is Unique", false, [SyndicateAssassinOn, EnableUniques], true);
        AssassinKills = new(MultiMenu.Ability, "<color=#073763FF>Assassin</color> Guess Limit", 1, 1, 15, 1, Assassin);
        AssassinMultiKill = new(MultiMenu.Ability, "<color=#073763FF>Assassin</color> Can Kill More Than Once Per Meeting", false, Assassin);
        AssassinGuessNeutralBenign = new(MultiMenu.Ability, "<color=#073763FF>Assassin</color> Can Guess <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Benigns</color>", false,
            Assassin);
        AssassinGuessNeutralEvil = new(MultiMenu.Ability, "<color=#073763FF>Assassin</color> Can Guess <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Evils</color>", false, Assassin);
        AssassinGuessInvestigative = new(MultiMenu.Ability, "<color=#073763FF>Assassin</color> Can Guess <color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Investigatives</color>", false,
            [Assassin, CISettings], true);
        AssassinGuessPest = new(MultiMenu.Ability, "<color=#073763FF>Assassin</color> Can Guess <color=#424242FF>Pestilence</color>", false, [Assassin, Pestilence], true);
        AssassinGuessModifiers = new(MultiMenu.Ability, "<color=#073763FF>Assassin</color> Can Guess Select <color=#7F7F7FFF>Modifiers</color>", false, [Assassin, EnableModifiers], true);
        AssassinGuessObjectifiers = new(MultiMenu.Ability, "<color=#073763FF>Assassin</color> Can Guess Select <color=#DD585BFF>Objectifiers</color>", false, [Assassin, EnableObjectifiers],
            true);
        AssassinGuessAbilities = new(MultiMenu.Ability, "<color=#073763FF>Assassin</color> Can Guess <color=#FF9900FF>Abilities</color>", false, [Assassin, EnableAbilities], true);
        AssassinateAfterVoting = new(MultiMenu.Ability, "<color=#073763FF>Assassin</color> Can Guess After Voting", false, Assassin);

        ButtonBarry = new(MultiMenu.Ability, "<color=#E600FFFF>Button Barry</color>", ButtonBarryOn);
        UniqueButtonBarry = new(MultiMenu.Ability, "<color=#E600FFFF>Button Barry</color> Is Unique", false, [ButtonBarry, EnableUniques], true);
        ButtonCooldown = new(MultiMenu.Ability, "Button Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, ButtonBarry);

        Insider = new(MultiMenu.Ability, "<color=#26FCFBFF>Insider</color>", InsiderOn);
        UniqueInsider = new(MultiMenu.Ability, "<color=#26FCFBFF>Insider</color> Is Unique", false, [Insider, EnableUniques], true);
        InsiderKnows = new(MultiMenu.Ability, "<color=#26FCFBFF>Insider</color> Knows Who They Are", true, Insider);

        Multitasker = new(MultiMenu.Ability, "<color=#FF804DFF>Multitasker</color>", MultitaskerOn);
        UniqueMultitasker = new(MultiMenu.Ability, "<color=#FF804DFF>Multitasker</color> Is Unique", false, [Multitasker, EnableUniques], true);
        Transparancy = new(MultiMenu.Ability, "Task Transparancy", 50f, 10f, 80f, 5f, PercentFormat, Multitasker);

        Ninja = new(MultiMenu.Ability, "<color=#A84300FF>Ninja</color>", [NinjaOn, EnableUniques], true);
        UniqueNinja = new(MultiMenu.Ability, "<color=#A84300FF>Ninja</color> Is Unique", false, Ninja);

        Politician = new(MultiMenu.Ability, "<color=#CCA3CCFF>Politician</color>", PoliticianOn);
        UniquePolitician = new(MultiMenu.Ability, "<color=#CCA3CCFF>Politician</color> Is Unique", false, [Politician, EnableUniques], true);
        PoliticianVoteBank = new(MultiMenu.Ability, "Initial <color=#CCA3CCFF>Politician</color> Initial Vote Bank", 0, 0, 10, 1, Politician);
        PoliticianButton = new(MultiMenu.Ability, "<color=#CCA3CCFF>Politician</color> Can Button", true, Politician);

        Radar = new(MultiMenu.Ability, "<color=#FF0080FF>Radar</color>", [RadarOn, EnableUniques], true);
        UniqueRadar = new(MultiMenu.Ability, "<color=#FF0080FF>Radar</color> Is Unique", false, Radar);

        Ruthless = new(MultiMenu.Ability, "<color=#2160DDFF>Ruthless</color>", RuthlessOn);
        UniqueRuthless = new(MultiMenu.Ability, "<color=#2160DDFF>Ruthless</color> Is Unique", false, [Ruthless, EnableUniques], true);
        RuthlessKnows = new(MultiMenu.Ability, "<color=#2160DDFF>Ruthless</color> Knows Who They Are", true, Ruthless);

        Snitch = new(MultiMenu.Ability, "<color=#D4AF37FF>Snitch</color>", SnitchOn);
        UniqueSnitch = new(MultiMenu.Ability, "<color=#D4AF37FF>Snitch</color> Is Unique", false, [Snitch, EnableUniques], true);
        SnitchKnows = new(MultiMenu.Ability, "<color=#D4AF37FF>Snitch</color> Knows Who They Are", true, Snitch);
        SnitchSeesNeutrals = new(MultiMenu.Ability, "<color=#D4AF37FF>Snitch</color> Sees <color=#B3B3B3FF>Neutrals</color>", false, Snitch);
        SnitchSeesCrew = new(MultiMenu.Ability, "<color=#D4AF37FF>Snitch</color> Sees <color=#8CFFFFFF>Crew</color>", false, Snitch);
        SnitchSeesRoles = new(MultiMenu.Ability, "<color=#D4AF37FF>Snitch</color> Sees Exact <color=#FFD700FF>Roles</color>", false, Snitch);
        SnitchTasksRemaining = new(MultiMenu.Ability, "Tasks Remaining When Revealed", 1, 1, 5, 1, Snitch);
        SnitchSeestargetsInMeeting = new(MultiMenu.Ability, "<color=#D4AF37FF>Snitch</color> Sees Evils In Meetings", true, Snitch);

        Swapper = new(MultiMenu.Ability, "<color=#66E666FF>Swapper</color>", SwapperOn);
        UniqueSwapper = new(MultiMenu.Ability, "<color=#66E666FF>Swapper</color> Is Unique", false, [Swapper, EnableUniques], true);
        SwapperButton = new(MultiMenu.Ability, "<color=#66E666FF>Swapper</color> Can Button", true, Swapper);
        SwapAfterVoting = new(MultiMenu.Ability, "<color=#66E666FF>Swapper</color> Can Swap After Voting", false, Swapper);
        SwapSelf = new(MultiMenu.Ability, "<color=#66E666FF>Swapper</color> Can Swap Themself", true, Swapper);

        Tiebreaker = new(MultiMenu.Ability, "<color=#99E699FF>Tiebreaker</color>", TiebreakerOn);
        UniqueTiebreaker = new(MultiMenu.Ability, "<color=#99E699FF>Tiebreaker</color> Is Unique", false, [Tiebreaker, EnableUniques], true);
        TiebreakerKnows = new(MultiMenu.Ability, "<color=#99E699FF>Tiebreaker</color> Knows Who They Are", true, Tiebreaker);

        Torch = new(MultiMenu.Ability, "<color=#FFFF99FF>Torch</color>", [TorchOn, EnableUniques], true);
        UniqueTorch = new(MultiMenu.Ability, "<color=#FFFF99FF>Torch</color> Is Unique", false, Torch);

        Tunneler = new(MultiMenu.Ability, "<color=#E91E63FF>Tunneler</color>", TunnelerOn);
        UniqueTunneler = new(MultiMenu.Ability, "<color=#E91E63FF>Tunneler</color> Is Unique", false, [Tunneler, EnableUniques], true);
        TunnelerKnows = new(MultiMenu.Ability, "<color=#E91E63FF>Tunneler</color> Knows Who They Are", true, Tunneler);

        Underdog = new(MultiMenu.Ability, "<color=#841A7FFF>Underdog</color>", UnderdogOn);
        UniqueUnderdog = new(MultiMenu.Ability, "<color=#841A7FFF>Underdog</color> Is Unique", false, [Underdog, EnableUniques], true);
        UnderdogKnows = new(MultiMenu.Ability, "<color=#841A7FFF>Underdog</color> Knows Who They Are", true, Underdog);
        UnderdogKillBonus = new(MultiMenu.Ability, "Kill Cooldown Bonus", 5f, 2.5f, 30f, 2.5f, CooldownFormat, Underdog);
        UnderdogIncreasedKC = new(MultiMenu.Ability, "Increased Kill Cooldown When 2+ Teammates", true, Underdog);

        Objectifiers = new(MultiMenu.Objectifier, "<color=#DD585BFF>Objectifiers</color>", [GameMode.Classic, GameMode.KillingOnly, GameMode.AllAny, GameMode.Custom]);
        AlliedOn = new(MultiMenu.Objectifier, "<color=#4545A9FF>Allied</color>", parent: Objectifiers);
        CorruptedOn = new(MultiMenu.Objectifier, "<color=#4545FFFF>Corrupted</color>", parent: Objectifiers);
        DefectorOn = new(MultiMenu.Objectifier, "<color=#E1C849FF>Defector</color>", parent: Objectifiers);
        FanaticOn = new(MultiMenu.Objectifier, "<color=#678D36FF>Fanatic</color>", parent: Objectifiers);
        LinkedOn = new(MultiMenu.Objectifier, "<color=#FF351FFF>Linked</color> Pairs", 1, 7, Objectifiers);
        LoversOn = new(MultiMenu.Objectifier, "<color=#FF66CCFF>Lovers</color> Pairs", 1, 7, Objectifiers);
        MafiaOn = new(MultiMenu.Objectifier, "<color=#00EEFFFF>Mafia</color>", 2, parent: Objectifiers);
        OverlordOn = new(MultiMenu.Objectifier, "<color=#008080FF>Overlord</color>", parent: Objectifiers);
        RivalsOn = new(MultiMenu.Objectifier, "<color=#3D2D2CFF>Rivals</color> Pairs", 1, 7, Objectifiers);
        TaskmasterOn = new(MultiMenu.Objectifier, "<color=#ABABFFFF>Taskmaster</color>", parent: Objectifiers);
        TraitorOn = new(MultiMenu.Objectifier, "<color=#370D43FF>Traitor</color>", parent: Objectifiers);

        Betrayer.Parents = [TraitorOn, FanaticOn];

        ObjectifierSettings = new(MultiMenu.Objectifier, "<color=#DD585BFF>Objectifier</color> Settings", [AlliedOn, CorruptedOn, DefectorOn, FanaticOn, LinkedOn, LoversOn, MafiaOn,
            RivalsOn, OverlordOn, TaskmasterOn, TraitorOn]);
        MaxObjectifiers = new(MultiMenu.Objectifier, "Max <color=#DD585BFF>Objectifiers</color>", 5, 1, 14, 1, ObjectifierSettings);
        MinObjectifiers = new(MultiMenu.Objectifier, "Min <color=#DD585BFF>Objectifiers</color>", 5, 1, 14, 1, ObjectifierSettings);

        Allied = new(MultiMenu.Objectifier, "<color=#4545A9FF>Allied</color>", AlliedOn);
        UniqueAllied = new(MultiMenu.Objectifier, "<color=#4545A9FF>Allied</color> Is Unique", false, [Allied, EnableUniques], true);
        AlliedFaction = new(MultiMenu.Objectifier, "<color=#4545A9FF>Allied</color> Faction", ["Random", "Intruder", "Syndicate", "Crew"], Allied);

        Corrupted = new(MultiMenu.Objectifier, "<color=#4545FFFF>Corrupted</color>", CorruptedOn);
        UniqueCorrupted = new(MultiMenu.Objectifier, "<color=#4545FFFF>Corrupted</color> Is Unique", false, [Corrupted, EnableUniques], true);
        CorruptCd = new(MultiMenu.Objectifier, "Corrupt Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Corrupted);
        AllCorruptedWin = new(MultiMenu.Objectifier, "All <color=#4545FFFF>Corrupted</color> Win Together", false, Corrupted);
        CorruptedVent = new(MultiMenu.Objectifier, "<color=#4545FFFF>Corrupted</color> Can Vent", false, Corrupted);

        Defector = new(MultiMenu.Objectifier, "<color=#E1C849FF>Defector</color>", DefectorOn);
        UniqueDefector = new(MultiMenu.Objectifier, "<color=#E1C849FF>Defector</color> Is Unique", false, [Defector, EnableUniques], true);
        DefectorKnows = new(MultiMenu.Objectifier, "<color=#E1C849FF>Defector</color> Knows Who They Are", true, Defector);
        DefectorFaction = new(MultiMenu.Objectifier, "<color=#E1C849FF>Defector</color> Faction", ["Random", "Non Evil Faction", "Non Neutral", "Non Crew", "Opposing Evil", "Neutral",
            "Crew"], Defector);

        Fanatic = new(MultiMenu.Objectifier, "<color=#678D36FF>Fanatic</color>", FanaticOn);
        UniqueFanatic = new(MultiMenu.Objectifier, "<color=#678D36FF>Fanatic</color> Is Unique", false, [Fanatic, EnableUniques], true);
        FanaticKnows = new(MultiMenu.Objectifier, "<color=#678D36FF>Fanatic</color> Knows Who They Are", true, Fanatic);
        FanaticColourSwap = new(MultiMenu.Objectifier, "Turned <color=#678D36FF>Fanatic</color> Swaps Colours For Investigative <color=#FFD700FF>Roles</color>", false, Fanatic);
        SnitchSeesFanatic = new(MultiMenu.Objectifier, "<color=#D4AF37FF>Snitch</color> Sees Turned <color=#678D36FF>Fanatic</color>", true, [FanaticOn, SnitchOn], true);
        RevealerRevealsFanatic = new(MultiMenu.Objectifier, "<color=#D3D3D3FF>Revealer</color> Reveals Turned <color=#678D36FF>Fanatic</color>", false, [FanaticOn, RevealerOn], true);

        Linked = new(MultiMenu.Objectifier, "<color=#FF351FFF>Linked</color>", LinkedOn);
        UniqueLinked = new(MultiMenu.Objectifier, "<color=#FF351FFF>Linked</color> Is Unique", false, [Linked, EnableUniques], true);
        LinkedChat = new(MultiMenu.Objectifier, "Enable <color=#FF351FFF>Linked</color> Chat", true, Linked);
        LinkedRoles = new(MultiMenu.Objectifier, "<color=#FF351FFF>Linked</color> Know Each Other's <color=#FFD700FF>Roles</color>", true, Linked);

        Lovers = new(MultiMenu.Objectifier, "<color=#FF66CCFF>Lovers</color>", LoversOn);
        UniqueLovers = new(MultiMenu.Objectifier, "<color=#FF66CCFF>Lovers</color> Is Unique", false, [Lovers, EnableUniques], true);
        BothLoversDie = new(MultiMenu.Objectifier, "Both <color=#FF66CCFF>Lovers</color> Die", true, Lovers);
        LoversChat = new(MultiMenu.Objectifier, "Enable <color=#FF66CCFF>Lovers</color> Chat", true, Lovers);
        LoversRoles = new(MultiMenu.Objectifier, "<color=#FF66CCFF>Lovers</color> Know Each Other's <color=#FFD700FF>Roles</color>", true, Lovers);

        Mafia = new(MultiMenu.Objectifier, "<color=#00EEFFFF>Mafia</color>", MafiaOn);
        UniqueMafia = new(MultiMenu.Objectifier, "<color=#00EEFFFF>Mafia</color> Is Unique", false, [Mafia, EnableUniques], true);
        MafiaRoles = new(MultiMenu.Objectifier, "<color=#00EEFFFF>Mafia</color> Know Each Other's <color=#FFD700FF>Roles</color>", true, Mafia);
        MafVent = new(MultiMenu.Objectifier, "<color=#00EEFFFF>Mafia</color> Can Vent", false, Mafia);

        Overlord = new(MultiMenu.Objectifier, "<color=#008080FF>Overlord</color>", OverlordOn);
        UniqueOverlord = new(MultiMenu.Objectifier, "<color=#008080FF>Overlord</color> Is Unique", false, [Overlord, EnableUniques], true);
        OverlordKnows = new(MultiMenu.Objectifier, "<color=#008080FF>Overlord</color> Knows Who They Are", true, Overlord);
        OverlordMeetingWinCount = new(MultiMenu.Objectifier, "<color=#008080FF>Overlord</color> Meeting Timer", 2, 1, 20, 1, Overlord);

        Rivals = new(MultiMenu.Objectifier, "<color=#3D2D2CFF>Rivals</color>", RivalsOn);
        UniqueRivals = new(MultiMenu.Objectifier, "<color=#3D2D2CFF>Rivals</color> Is Unique", false, [Rivals, EnableUniques], true);
        RivalsChat = new(MultiMenu.Objectifier, "Enable <color=#3D2D2CFF>Rivals</color> Chat", true, Rivals);
        RivalsRoles = new(MultiMenu.Objectifier, "<color=#3D2D2CFF>Rivals</color> Know Each Other's <color=#FFD700FF>Roles</color>", true, Rivals);

        Taskmaster = new(MultiMenu.Objectifier, "<color=#ABABFFFF>Taskmaster</color>", TaskmasterOn);
        UniqueTaskmaster = new(MultiMenu.Objectifier, "<color=#ABABFFFF>Taskmaster</color> Is Unique", false, [Taskmaster, EnableUniques], true);
        TMTasksRemaining = new(MultiMenu.Objectifier, "Tasks Remaining When Revealed", 1, 1, 5, 1, Taskmaster);

        Traitor = new(MultiMenu.Objectifier, "<color=#370D43FF>Traitor</color>", TraitorOn);
        UniqueTraitor = new(MultiMenu.Objectifier, "<color=#370D43FF>Traitor</color> Is Unique", false, [Traitor, EnableUniques], true);
        TraitorKnows = new(MultiMenu.Objectifier, "<color=#370D43FF>Traitor</color> Knows Who They Are", true, Traitor);
        SnitchSeesTraitor = new(MultiMenu.Objectifier, "<color=#D4AF37FF>Snitch</color> Sees Turned <color=#370D43FF>Traitor</color>", true, [Traitor, SnitchOn], true);
        RevealerRevealsTraitor = new(MultiMenu.Objectifier, "<color=#D3D3D3FF>Revealer</color> Reveals Turned <color=#370D43FF>Traitor</color>", false, [Traitor, RevealerOn], true);
        TraitorColourSwap = new(MultiMenu.Objectifier, "Turned <color=#370D43FF>Traitor</color> Swaps Colours For Investigative <color=#FFD700FF>Roles</color>", false, Traitor);

        RoleList = new(MultiMenu.RoleList, "Role List Entries");
        Entry1 = new("Entry");
        Entry2 = new("Entry");
        Entry3 = new("Entry");
        Entry4 = new("Entry");
        Entry5 = new("Entry");
        Entry6 = new("Entry");
        Entry7 = new("Entry");
        Entry8 = new("Entry");
        Entry9 = new("Entry");
        Entry10 = new("Entry");
        Entry11 = new("Entry");
        Entry12 = new("Entry");
        Entry13 = new("Entry");
        Entry14 = new("Entry");
        Entry15 = new("Entry");

        BanList = new(MultiMenu.RoleList, "Role List Bans");
        Ban1 = new("Ban");
        Ban2 = new("Ban");
        Ban3 = new("Ban");
        Ban4 = new("Ban");
        Ban5 = new("Ban");

        FreeBans = new(MultiMenu.RoleList, "Free Bans");
        BanCrewmate = new(MultiMenu.RoleList, "Ban <color=#8CFFFFFF>Crewmate</color>", false);
        BanImpostor = new(MultiMenu.RoleList, "Ban <color=#FF1919FF>Impostor</color>", false);
        BanAnarchist = new(MultiMenu.RoleList, "Ban <color=#008000FF>Anarchist</color>", false);
        BanMurderer = new(MultiMenu.RoleList, "Ban <color=#6F7BEAFF>Murderer</color>", false);

        EnablePostmortals = new(MultiMenu.RoleList, "Postmortals");
        EnableBanshee = new(MultiMenu.RoleList, "Enable <color=#E67E22FF>Banshee</color>", false);
        EnableGhoul = new(MultiMenu.RoleList, "Enable <color=#F1C40FFF>Ghoul</color>", false);
        EnablePhantom = new(MultiMenu.RoleList, "Enable <color=#662962FF>Phantom</color>", false);
        EnableRevealer = new(MultiMenu.RoleList, "Enable <color=#D3D3D3FF>Revealer</color>", false);

        Revealer.Parents = [EnableRevealer, RevealerOn];
        Ghoul.Parents = [EnableGhoul, GhoulOn];
        Phantom.Parents = [EnablePhantom, PhantomOn];
        Banshee.Parents = [EnableBanshee, BansheeOn];
    }
}