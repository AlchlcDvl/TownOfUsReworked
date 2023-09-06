namespace TownOfUsReworked.CustomOptions;

[HarmonyPatch]
public static class Generate
{
    //Game Options
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

    //Map Settings
    public static CustomHeaderOption MapSettings;
    public static CustomNumberOption RandomMapSkeld;
    public static CustomNumberOption RandomMapMira;
    public static CustomNumberOption RandomMapPolus;
    //public static CustomNumberOption RandomMapdlekS;
    public static CustomNumberOption RandomMapAirship;
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

    //Game Modifier Options
    public static CustomHeaderOption GameModifiers;
    public static CustomToggleOption AnonymousVoting;
    public static CustomStringOption WhoCanVent;
    public static CustomStringOption SkipButtonDisable;
    public static CustomToggleOption FirstKillShield;
    public static CustomStringOption WhoSeesFirstKillShield;
    public static CustomToggleOption FactionSeeRoles;
    public static CustomToggleOption VisualTasks;
    public static CustomToggleOption NoNames;
    public static CustomToggleOption Whispers;
    public static CustomToggleOption WhispersAnnouncement;
    public static CustomToggleOption AppearanceAnimation;
    public static CustomToggleOption RandomSpawns;
    public static CustomToggleOption EnableModifiers;
    public static CustomToggleOption EnableAbilities;
    public static CustomToggleOption EnableObjectifiers;
    public static CustomToggleOption VentTargeting;

    //QoL Options
    public static CustomHeaderOption QualityChanges;
    public static CustomToggleOption DeadSeeEverything;
    public static CustomToggleOption ObstructNames;
    public static CustomToggleOption ParallelMedScans;

    //Better Sabotages
    public static CustomHeaderOption BetterSabotages;
    public static CustomToggleOption OxySlow;
    public static CustomNumberOption ReactorShake;
    public static CustomToggleOption CamouflagedComms;
    public static CustomToggleOption CamouflagedMeetings;
    //public static CustomToggleOption NightVision;
    //public static CustomToggleOption EvilsIgnoreNV;

    //Better Skeld Options
    public static CustomHeaderOption BetterSkeld;
    public static CustomToggleOption SkeldVentImprovements;
    public static CustomNumberOption SkeldO2Timer;
    public static CustomNumberOption SkeldReactorTimer;

    //Better Mira HQ Options
    public static CustomHeaderOption BetterMiraHQ;
    public static CustomToggleOption MiraHQVentImprovements;
    public static CustomNumberOption MiraO2Timer;
    public static CustomNumberOption MiraReactorTimer;

    //Better Airship Options
    public static CustomHeaderOption BetterAirship;
    public static CustomStringOption SpawnType;
    public static CustomStringOption MoveAdmin;
    public static CustomStringOption MoveElectrical;
    public static CustomNumberOption MinDoorSwipeTime;
    public static CustomToggleOption MoveDivert;
    public static CustomToggleOption MoveFuel;
    public static CustomToggleOption MoveVitals;
    public static CustomNumberOption CrashTimer;

    //Better Polus Options
    public static CustomHeaderOption BetterPolus;
    public static CustomToggleOption PolusVentImprovements;
    public static CustomToggleOption VitalsLab;
    public static CustomToggleOption ColdTempDeathValley;
    public static CustomToggleOption WifiChartCourseSwap;
    public static CustomNumberOption SeismicTimer;

    //Game Modes
    public static CustomHeaderOption GameModeSettings;
    public static CustomStringOption CurrentMode;

    //Killing Only Options
    public static CustomHeaderOption KOSettings;
    public static CustomNumberOption NeutralRoles;
    public static CustomToggleOption AddArsonist;
    public static CustomToggleOption AddCryomaniac;
    public static CustomToggleOption AddPlaguebearer;

    //Announcement Options
    public static CustomHeaderOption GameAnnouncementsSettings;
    public static CustomToggleOption GameAnnouncements;
    public static CustomStringOption RoleFactionReports;
    public static CustomStringOption KillerReports;
    public static CustomToggleOption LocationReports;

    //All Any Options
    public static CustomHeaderOption AARLSettings;
    public static CustomToggleOption EnableUniques;

    //CI Role Spawn
    public static CustomHeaderOption CIRoles;
    public static CustomLayersOption DetectiveOn;
    public static CustomLayersOption CoronerOn;
    public static CustomLayersOption SheriffOn;
    public static CustomLayersOption MediumOn;
    public static CustomLayersOption TrackerOn;
    public static CustomLayersOption InspectorOn;
    public static CustomLayersOption OperativeOn;
    public static CustomLayersOption SeerOn;

    //CSv Role Spawn
    public static CustomHeaderOption CSvRoles;
    public static CustomLayersOption MayorOn;
    public static CustomLayersOption DictatorOn;
    public static CustomLayersOption MonarchOn;

    //CP Role Spawn
    public static CustomHeaderOption CPRoles;
    public static CustomLayersOption AltruistOn;
    public static CustomLayersOption MedicOn;

    //CA Role Spawn
    public static CustomHeaderOption CARoles;
    public static CustomLayersOption VampireHunterOn;
    public static CustomLayersOption MysticOn;

    //CK Role Spawn
    public static CustomHeaderOption CKRoles;
    public static CustomLayersOption VeteranOn;
    public static CustomLayersOption VigilanteOn;

    //CS Role Spawn
    public static CustomHeaderOption CSRoles;
    public static CustomLayersOption EngineerOn;
    public static CustomLayersOption ShifterOn;
    public static CustomLayersOption EscortOn;
    public static CustomLayersOption TransporterOn;
    public static CustomLayersOption RevealerOn;
    public static CustomLayersOption RetributionistOn;
    public static CustomLayersOption ChameleonOn;

    //CU Role Spawn
    public static CustomHeaderOption CURoles;
    public static CustomLayersOption CrewmateOn;

    //NB Role Spawn
    public static CustomHeaderOption NBRoles;
    public static CustomLayersOption AmnesiacOn;
    public static CustomLayersOption GuardianAngelOn;
    public static CustomLayersOption SurvivorOn;
    public static CustomLayersOption ThiefOn;

    //NH Role Spawn
    public static CustomHeaderOption NHRoles;
    public static CustomLayersOption PlaguebearerOn;

    //NP Role Spawn
    public static CustomHeaderOption NPRoles;
    public static CustomLayersOption PhantomOn;

    //NN Role Spawn
    public static CustomHeaderOption NNRoles;
    public static CustomLayersOption DraculaOn;
    public static CustomLayersOption JackalOn;
    public static CustomLayersOption NecromancerOn;
    public static CustomLayersOption WhispererOn;

    //NE Role Spawn
    public static CustomHeaderOption NERoles;
    public static CustomLayersOption ExecutionerOn;
    public static CustomLayersOption ActorOn;
    public static CustomLayersOption JesterOn;
    public static CustomLayersOption CannibalOn;
    public static CustomLayersOption BountyHunterOn;
    public static CustomLayersOption TrollOn;
    public static CustomLayersOption GuesserOn;

    //NK Role Spawn
    public static CustomHeaderOption NKRoles;
    public static CustomLayersOption ArsonistOn;
    public static CustomLayersOption CryomaniacOn;
    public static CustomLayersOption GlitchOn;
    public static CustomLayersOption MurdererOn;
    public static CustomLayersOption WerewolfOn;
    public static CustomLayersOption SerialKillerOn;
    public static CustomLayersOption JuggernautOn;

    //IC Role Spawn
    public static CustomHeaderOption ICRoles;
    public static CustomLayersOption BlackmailerOn;
    public static CustomLayersOption CamouflagerOn;
    public static CustomLayersOption GrenadierOn;
    public static CustomLayersOption JanitorOn;

    //ID Role Spawn
    public static CustomHeaderOption IDRoles;
    public static CustomLayersOption MorphlingOn;
    public static CustomLayersOption DisguiserOn;
    public static CustomLayersOption WraithOn;

    //IK Role Spawn
    public static CustomHeaderOption IKRoles;
    public static CustomLayersOption AmbusherOn;
    public static CustomLayersOption EnforcerOn;

    //IS Role Spawn
    public static CustomHeaderOption ISRoles;
    public static CustomLayersOption ConsigliereOn;
    public static CustomLayersOption GodfatherOn;
    public static CustomLayersOption ConsortOn;
    public static CustomLayersOption MinerOn;
    public static CustomLayersOption TeleporterOn;

    //IU Role Spawn
    public static CustomHeaderOption IURoles;
    public static CustomLayersOption ImpostorOn;
    public static CustomLayersOption GhoulOn;

    //SSu Role Spawn
    public static CustomHeaderOption SSuRoles;
    public static CustomLayersOption WarperOn;
    public static CustomLayersOption RebelOn;
    public static CustomLayersOption StalkerOn;

    //SD Role Spawn
    public static CustomHeaderOption SDRoles;
    public static CustomLayersOption FramerOn;
    public static CustomLayersOption ShapeshifterOn;
    public static CustomLayersOption ConcealerOn;
    public static CustomLayersOption DrunkardOn;
    public static CustomLayersOption SilencerOn;

    //SyK Role Spawn
    public static CustomHeaderOption SPRoles;
    public static CustomLayersOption SpellslingerOn;
    public static CustomLayersOption TimeKeeperOn;

    //SyK Role Spawn
    public static CustomHeaderOption SyKRoles;
    public static CustomLayersOption BomberOn;
    public static CustomLayersOption CrusaderOn;
    public static CustomLayersOption ColliderOn;
    public static CustomLayersOption PoisonerOn;

    //SU Role Spawn
    public static CustomHeaderOption SURoles;
    public static CustomLayersOption AnarchistOn;
    public static CustomLayersOption BansheeOn;

    //Modifier Spawn
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

    //Ability Spawn
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

    //Objectifier Spawn
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

    //Crew Options
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

    //CSv Options
    public static CustomHeaderOption CSvSettings;
    public static CustomNumberOption CSvMax;

    //Mayor Options
    public static CustomHeaderOption Mayor;
    public static CustomToggleOption UniqueMayor;
    public static CustomNumberOption MayorVoteCount;
    public static CustomToggleOption RoundOneNoMayorReveal;
    public static CustomToggleOption MayorButton;

    //Dictator Options
    public static CustomHeaderOption Dictator;
    public static CustomToggleOption UniqueDictator;
    public static CustomToggleOption RoundOneNoDictReveal;
    public static CustomToggleOption DictatorButton;
    public static CustomToggleOption DictateAfterVoting;

    //Monarch Options
    public static CustomHeaderOption Monarch;
    public static CustomToggleOption UniqueMonarch;
    public static CustomNumberOption KnightVoteCount;
    public static CustomNumberOption KnightCount;
    public static CustomNumberOption KnightingCd;
    public static CustomToggleOption RoundOneNoKnighting;
    public static CustomToggleOption KnightButton;
    public static CustomToggleOption MonarchButton;

    //CA Options
    public static CustomHeaderOption CASettings;
    public static CustomNumberOption CAMax;

    //Mystic Options
    public static CustomHeaderOption Mystic;
    public static CustomToggleOption UniqueMystic;
    public static CustomNumberOption MysticRevealCd;

    //Vampire Hunter Options
    public static CustomHeaderOption VampireHunter;
    public static CustomToggleOption UniqueVampireHunter;
    public static CustomNumberOption StakeCd;

    //CK Options
    public static CustomHeaderOption CKSettings;
    public static CustomNumberOption CKMax;

    //Vigilante Options
    public static CustomHeaderOption Vigilante;
    public static CustomToggleOption UniqueVigilante;
    public static CustomStringOption VigiOptions;
    public static CustomStringOption VigiNotifOptions;
    public static CustomToggleOption MisfireKillsInno;
    public static CustomToggleOption VigiKillAgain;
    public static CustomToggleOption RoundOneNoShot;
    public static CustomNumberOption ShootCd;
    public static CustomNumberOption MaxBullets;

    //Veteran Options
    public static CustomHeaderOption Veteran;
    public static CustomToggleOption UniqueVeteran;
    public static CustomNumberOption AlertCd;
    public static CustomNumberOption AlertDur;
    public static CustomNumberOption MaxAlerts;

    //CS Options
    public static CustomHeaderOption CSSettings;
    public static CustomNumberOption CSMax;

    //Engineer Options
    public static CustomHeaderOption Engineer;
    public static CustomNumberOption MaxFixes;
    public static CustomToggleOption UniqueEngineer;
    public static CustomNumberOption FixCd;

    //Transporter Options
    public static CustomHeaderOption Transporter;
    public static CustomToggleOption UniqueTransporter;
    public static CustomToggleOption TransSelf;
    public static CustomNumberOption TransportCd;
    public static CustomNumberOption TransportDur;
    public static CustomNumberOption MaxTransports;

    //Retributionist Options
    public static CustomHeaderOption Retributionist;
    public static CustomToggleOption UniqueRetributionist;
    public static CustomToggleOption ReviveAfterVoting;
    public static CustomNumberOption MaxUses;

    //Escort Options
    public static CustomHeaderOption Escort;
    public static CustomToggleOption UniqueEscort;
    public static CustomNumberOption EscortCd;
    public static CustomNumberOption EscortDur;

    //Chameleon Options
    public static CustomHeaderOption Chameleon;
    public static CustomToggleOption UniqueChameleon;
    public static CustomNumberOption MaxSwoops;
    public static CustomNumberOption SwoopCd;
    public static CustomNumberOption SwoopDur;

    //Shifter Options
    public static CustomHeaderOption Shifter;
    public static CustomToggleOption UniqueShifter;
    public static CustomNumberOption ShiftCd;
    public static CustomStringOption ShiftedBecomes;

    //CI Options
    public static CustomHeaderOption CISettings;
    public static CustomNumberOption CIMax;

    //Tracker Options
    public static CustomHeaderOption Tracker;
    public static CustomToggleOption UniqueTracker;
    public static CustomNumberOption UpdateInterval;
    public static CustomNumberOption TrackCd;
    public static CustomToggleOption ResetOnNewRound;
    public static CustomNumberOption MaxTracks;

    //Operative Options
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

    //Seer Options
    public static CustomHeaderOption Seer;
    public static CustomToggleOption UniqueSeer;
    public static CustomNumberOption SeerCd;

    //Detective Options
    public static CustomHeaderOption Detective;
    public static CustomToggleOption UniqueDetective;
    public static CustomNumberOption ExamineCd;
    public static CustomNumberOption RecentKill;
    public static CustomNumberOption FootprintInterval;
    public static CustomNumberOption FootprintDur;
    public static CustomToggleOption AnonymousFootPrint;

    //Coroner Options
    public static CustomHeaderOption Coroner;
    public static CustomToggleOption UniqueCoroner;
    public static CustomNumberOption CoronerArrowDur;
    public static CustomToggleOption CoronerReportName;
    public static CustomToggleOption CoronerReportRole;
    public static CustomNumberOption CoronerKillerNameTime;
    public static CustomNumberOption CompareCd;
    public static CustomNumberOption AutopsyCd;

    //Inspector Options
    public static CustomHeaderOption Inspector;
    public static CustomToggleOption UniqueInspector;
    public static CustomNumberOption InspectCd;

    //Medium Options
    public static CustomHeaderOption Medium;
    public static CustomToggleOption UniqueMedium;
    public static CustomNumberOption MediateCd;
    public static CustomToggleOption ShowMediatePlayer;
    public static CustomStringOption ShowMediumToDead;
    public static CustomStringOption DeadRevealed;

    //Sheriff Options
    public static CustomHeaderOption Sheriff;
    public static CustomToggleOption UniqueSheriff;
    public static CustomNumberOption InterrogateCd;
    public static CustomToggleOption NeutEvilRed;
    public static CustomToggleOption NeutKillingRed;

    //CP Options
    public static CustomHeaderOption CPSettings;
    public static CustomNumberOption CPMax;

    //Altruist Options
    public static CustomHeaderOption Altruist;
    public static CustomToggleOption UniqueAltruist;
    public static CustomNumberOption ReviveDur;
    public static CustomToggleOption AltruistTargetBody;
    public static CustomNumberOption ReviveCd;
    public static CustomNumberOption MaxRevives;

    //Medic Options
    public static CustomHeaderOption Medic;
    public static CustomToggleOption UniqueMedic;
    public static CustomStringOption ShowShielded;
    public static CustomStringOption WhoGetsNotification;
    public static CustomToggleOption ShieldBreaks;

    //CU Options
    public static CustomHeaderOption CUSettings;

    //Revealer Options
    public static CustomHeaderOption Revealer;
    public static CustomNumberOption RevealerTasksRemainingClicked;
    public static CustomNumberOption RevealerTasksRemainingAlert;
    public static CustomToggleOption RevealerRevealsNeutrals;
    public static CustomToggleOption RevealerRevealsCrew;
    public static CustomToggleOption RevealerRevealsRoles;
    public static CustomStringOption RevealerCanBeClickedBy;

    //Intruder Options
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

    //IC Options
    public static CustomHeaderOption ICSettings;
    public static CustomNumberOption ICMax;

    //Janitor Options
    public static CustomHeaderOption Janitor;
    public static CustomToggleOption UniqueJanitor;
    public static CustomNumberOption CleanCd;
    public static CustomToggleOption JaniCooldownsLinked;
    public static CustomToggleOption SoloBoost;
    public static CustomNumberOption DragCd;
    public static CustomStringOption JanitorVentOptions;
    public static CustomNumberOption DragModifier;

    //Blackmailer Options
    public static CustomHeaderOption Blackmailer;
    public static CustomToggleOption UniqueBlackmailer;
    public static CustomNumberOption BlackmailCd;
    public static CustomToggleOption WhispersNotPrivate;
    public static CustomToggleOption BlackmailMates;
    public static CustomToggleOption BMRevealed;

    //Grenadier Options
    public static CustomHeaderOption Grenadier;
    public static CustomToggleOption UniqueGrenadier;
    public static CustomNumberOption FlashCd;
    public static CustomNumberOption FlashDur;
    public static CustomToggleOption GrenadierIndicators;
    public static CustomToggleOption GrenadierVent;
    public static CustomNumberOption FlashRadius;

    //Camouflager Options
    public static CustomHeaderOption Camouflager;
    public static CustomToggleOption UniqueCamouflager;
    public static CustomNumberOption CamouflageCd;
    public static CustomNumberOption CamouflageDur;
    public static CustomToggleOption CamoHideSpeed;
    public static CustomToggleOption CamoHideSize;

    //ID Options
    public static CustomHeaderOption IDSettings;
    public static CustomNumberOption IDMax;

    //Morphling Options
    public static CustomHeaderOption Morphling;
    public static CustomToggleOption UniqueMorphling;
    public static CustomNumberOption MorphCd;
    public static CustomNumberOption MorphDur;
    public static CustomToggleOption MorphlingVent;
    public static CustomToggleOption MorphCooldownsLinked;
    public static CustomNumberOption SampleCd;

    //Disguiser Options
    public static CustomHeaderOption Disguiser;
    public static CustomToggleOption UniqueDisguiser;
    public static CustomNumberOption DisguiseCd;
    public static CustomNumberOption DisguiseDelay;
    public static CustomNumberOption DisguiseDur;
    public static CustomStringOption DisguiseTarget;
    public static CustomToggleOption DisgCooldownsLinked;
    public static CustomNumberOption MeasureCd;

    //Wraith Options
    public static CustomHeaderOption Wraith;
    public static CustomToggleOption UniqueWraith;
    public static CustomNumberOption InvisCd;
    public static CustomNumberOption InvisDur;
    public static CustomToggleOption WraithVent;

    //IS Options
    public static CustomHeaderOption ISSettings;
    public static CustomNumberOption ISMax;

    //Teleporter Options
    public static CustomHeaderOption Teleporter;
    public static CustomToggleOption UniqueTeleporter;
    public static CustomNumberOption TeleportCd;
    public static CustomNumberOption TeleMarkCd;
    public static CustomToggleOption TeleVent;
    public static CustomToggleOption TeleCooldownsLinked;

    //Consigliere Options
    public static CustomHeaderOption Consigliere;
    public static CustomToggleOption UniqueConsigliere;
    public static CustomNumberOption InvestigateCd;
    public static CustomStringOption ConsigInfo;

    //Consort Options
    public static CustomHeaderOption Consort;
    public static CustomToggleOption UniqueConsort;
    public static CustomNumberOption ConsortCd;
    public static CustomNumberOption ConsortDur;

    //Godfather Options
    public static CustomHeaderOption Godfather;
    public static CustomToggleOption UniqueGodfather;
    public static CustomNumberOption GFPromotionCdDecrease;

    //Miner Options
    public static CustomHeaderOption Miner;
    public static CustomToggleOption UniqueMiner;
    public static CustomNumberOption MineCd;

    //IK Options
    public static CustomHeaderOption IKSettings;
    public static CustomNumberOption IKMax;

    //Ambusher Options
    public static CustomHeaderOption Ambusher;
    public static CustomToggleOption UniqueAmbusher;
    public static CustomNumberOption AmbushCd;
    public static CustomNumberOption AmbushDur;
    public static CustomToggleOption AmbushMates;

    //Enforcer Options
    public static CustomHeaderOption Enforcer;
    public static CustomToggleOption UniqueEnforcer;
    public static CustomNumberOption EnforceCd;
    public static CustomNumberOption EnforceDur;
    public static CustomNumberOption EnforceDelay;
    public static CustomNumberOption EnforceRadius;

    //IU Options
    public static CustomHeaderOption IUSettings;

    //Ghoul Options
    public static CustomHeaderOption Ghoul;
    public static CustomNumberOption GhoulMarkCd;

    //Syndicate Options
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

    //SD Options
    public static CustomHeaderOption SDSettings;
    public static CustomNumberOption SDMax;

    //Shapeshifter Options
    public static CustomHeaderOption Shapeshifter;
    public static CustomToggleOption UniqueShapeshifter;
    public static CustomNumberOption ShapeshiftCd;
    public static CustomNumberOption ShapeshiftDur;
    public static CustomToggleOption ShapeshiftMates;

    //Drunkard Options
    public static CustomHeaderOption Drunkard;
    public static CustomToggleOption UniqueDrunkard;
    public static CustomNumberOption ConfuseCd;
    public static CustomNumberOption ConfuseDur;
    public static CustomToggleOption ConfuseImmunity;

    //Concealer Options
    public static CustomHeaderOption Concealer;
    public static CustomToggleOption UniqueConcealer;
    public static CustomNumberOption ConcealCd;
    public static CustomNumberOption ConcealDur;
    public static CustomToggleOption ConcealMates;

    //Silencer Options
    public static CustomHeaderOption Silencer;
    public static CustomToggleOption UniqueSilencer;
    public static CustomNumberOption SilenceCd;
    public static CustomToggleOption WhispersNotPrivateSilencer;
    public static CustomToggleOption SilenceMates;
    public static CustomToggleOption SilenceRevealed;

    //Framer Options
    public static CustomHeaderOption Framer;
    public static CustomNumberOption FrameCd;
    public static CustomNumberOption ChaosDriveFrameRadius;
    public static CustomToggleOption UniqueFramer;

    //SyK Options
    public static CustomHeaderOption SyKSettings;
    public static CustomNumberOption SyKMax;

    //Crusader Options
    public static CustomHeaderOption Crusader;
    public static CustomToggleOption UniqueCrusader;
    public static CustomNumberOption CrusadeCd;
    public static CustomNumberOption CrusadeDur;
    public static CustomNumberOption ChaosDriveCrusadeRadius;
    public static CustomToggleOption CrusadeMates;

    //Bomber Options
    public static CustomHeaderOption Bomber;
    public static CustomToggleOption UniqueBomber;
    public static CustomNumberOption BombCd;
    public static CustomNumberOption DetonateCd;
    public static CustomToggleOption BombCooldownsLinked;
    public static CustomToggleOption BombsRemoveOnNewRound;
    public static CustomToggleOption BombsDetonateOnMeetingStart;
    public static CustomNumberOption BombRange;
    public static CustomNumberOption ChaosDriveBombRange;
    public static CustomToggleOption BombKillsSyndicate;

    //Poisoner Options
    public static CustomHeaderOption Poisoner;
    public static CustomToggleOption UniquePoisoner;
    public static CustomNumberOption PoisonCd;
    public static CustomNumberOption PoisonDur;

    //Collider Options
    public static CustomHeaderOption Collider;
    public static CustomToggleOption UniqueCollider;
    public static CustomNumberOption CollideCd;
    public static CustomNumberOption ChargeCd;
    public static CustomNumberOption ChargeDur;
    public static CustomNumberOption CollideRange;
    public static CustomNumberOption CollideRangeIncrease;
    public static CustomToggleOption ChargeCooldownsLinked;
    public static CustomToggleOption CollideResetsCooldown;

    //SSu Options
    public static CustomHeaderOption SSuSettings;
    public static CustomNumberOption SSuMax;

    //Rebel Options
    public static CustomHeaderOption Rebel;
    public static CustomToggleOption UniqueRebel;
    public static CustomNumberOption RebPromotionCdDecrease;

    //Stalker Options
    public static CustomHeaderOption Stalker;
    public static CustomToggleOption UniqueStalker;
    public static CustomNumberOption StalkCd;

    //Warper Options
    public static CustomHeaderOption Warper;
    public static CustomNumberOption WarpCd;
    public static CustomNumberOption WarpDur;
    public static CustomToggleOption UniqueWarper;
    public static CustomToggleOption WarpSelf;

    //SU Options
    public static CustomHeaderOption SyndicateUtilitySettings;

    //Anarchist Options
    public static CustomHeaderOption Anarchist;
    public static CustomNumberOption AnarchKillCd;

    //Banshee Options
    public static CustomHeaderOption Banshee;
    public static CustomNumberOption ScreamCd;
    public static CustomNumberOption ScreamDur;

    //SP Options
    public static CustomHeaderOption SPSettings;
    public static CustomNumberOption SPMax;

    //Spellslinger Options
    public static CustomHeaderOption Spellslinger;
    public static CustomNumberOption SpellCd;
    public static CustomNumberOption SpellCdIncrease;
    public static CustomToggleOption UniqueSpellslinger;

    //Time Keeper Options
    public static CustomHeaderOption TimeKeeper;
    public static CustomToggleOption UniqueTimeKeeper;
    public static CustomNumberOption TimeCd;
    public static CustomNumberOption TimeDur;
    public static CustomToggleOption TimeFreezeImmunity;
    public static CustomToggleOption TimeRewindImmunity;

    //Neutral Options
    public static CustomHeaderOption NeutralSettings;
    public static CustomNumberOption NeutralVision;
    public static CustomToggleOption LightsAffectNeutrals;
    public static CustomStringOption NoSolo;
    public static CustomNumberOption NeutralMax;
    public static CustomNumberOption NeutralMin;
    public static CustomToggleOption NeutralsVent;
    public static CustomToggleOption AvoidNeutralKingmakers;
    public static CustomToggleOption NeutralFlashlight;

    //NA Options
    public static CustomHeaderOption NASettings;

    //Pestilence Options
    public static CustomHeaderOption Pestilence;
    public static CustomToggleOption PestSpawn;
    public static CustomToggleOption PlayersAlerted;
    public static CustomNumberOption ObliterateCd;
    public static CustomToggleOption PestVent;

    //NB Options
    public static CustomHeaderOption NBSettings;
    public static CustomNumberOption NBMax;
    public static CustomToggleOption VigiKillsNB;

    //Amnesiac Options
    public static CustomHeaderOption Amnesiac;
    public static CustomToggleOption RememberArrows;
    public static CustomNumberOption RememberArrowDelay;
    public static CustomToggleOption AmneVent;
    public static CustomToggleOption AmneSwitchVent;
    public static CustomToggleOption UniqueAmnesiac;

    //Survivor Options
    public static CustomHeaderOption Survivor;
    public static CustomNumberOption VestCd;
    public static CustomNumberOption VestDur;
    public static CustomNumberOption VestKCReset;
    public static CustomToggleOption SurvVent;
    public static CustomToggleOption SurvSwitchVent;
    public static CustomNumberOption MaxVests;
    public static CustomToggleOption UniqueSurvivor;

    //Guardian Angel Options
    public static CustomHeaderOption GuardianAngel;
    public static CustomNumberOption ProtectCd;
    public static CustomNumberOption ProtectDur;
    public static CustomNumberOption ProtectKCReset;
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

    //Thief Options
    public static CustomHeaderOption Thief;
    public static CustomToggleOption ThiefVent;
    public static CustomNumberOption StealCd;
    public static CustomToggleOption UniqueThief;
    public static CustomToggleOption ThiefSteals;
    public static CustomToggleOption ThiefCanGuess;
    public static CustomToggleOption ThiefCanGuessAfterVoting;

    //NE Options
    public static CustomHeaderOption NESettings;
    public static CustomNumberOption NEMax;
    public static CustomToggleOption NeutralEvilsEndGame;

    //Jester Options
    public static CustomHeaderOption Jester;
    public static CustomToggleOption JesterButton;
    public static CustomToggleOption JesterVent;
    public static CustomToggleOption JestSwitchVent;
    public static CustomToggleOption JestEjectScreen;
    public static CustomToggleOption VigiKillsJester;
    public static CustomNumberOption HauntCd;
    public static CustomNumberOption MaxHaunts;
    public static CustomToggleOption UniqueJester;

    //Actor Options
    public static CustomHeaderOption Actor;
    public static CustomToggleOption ActorButton;
    public static CustomToggleOption ActorVent;
    public static CustomToggleOption ActSwitchVent;
    public static CustomToggleOption VigiKillsActor;
    public static CustomToggleOption UniqueActor;
    public static CustomToggleOption ActorCanPickRole;

    //Troll Options
    public static CustomHeaderOption Troll;
    public static CustomNumberOption InteractCd;
    public static CustomToggleOption TrollVent;
    public static CustomToggleOption TrollSwitchVent;
    public static CustomToggleOption UniqueTroll;

    //Cannibal Options
    public static CustomHeaderOption Cannibal;
    public static CustomNumberOption EatCd;
    public static CustomNumberOption BodiesNeeded;
    public static CustomToggleOption CannibalVent;
    public static CustomToggleOption EatArrows;
    public static CustomNumberOption EatArrowDelay;
    public static CustomToggleOption VigiKillsCannibal;
    public static CustomToggleOption UniqueCannibal;

    //Executioner Options
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
    public static CustomNumberOption DoomCd;
    public static CustomNumberOption MaxDooms;
    public static CustomToggleOption ExecutionerCanPickTargets;

    //Bounty Hunter Options
    public static CustomHeaderOption BountyHunter;
    public static CustomToggleOption BHVent;
    public static CustomNumberOption GuessCd;
    public static CustomNumberOption BountyHunterGuesses;
    public static CustomToggleOption UniqueBountyHunter;
    public static CustomToggleOption VigiKillsBH;
    public static CustomToggleOption BountyHunterCanPickTargets;

    //Guesser Options
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

    //NH Options
    public static CustomHeaderOption NHSettings;
    public static CustomNumberOption NHMax;

    //Plaguebearer Options
    public static CustomHeaderOption Plaguebearer;
    public static CustomNumberOption InfectCd;
    public static CustomToggleOption PBVent;
    public static CustomToggleOption UniquePlaguebearer;

    //NK Options
    public static CustomHeaderOption NKSettings;
    public static CustomNumberOption NKMax;
    public static CustomToggleOption NKHasImpVision;
    public static CustomToggleOption NKsKnow;

    //Glitch Options
    public static CustomHeaderOption Glitch;
    public static CustomNumberOption HackCd;
    public static CustomNumberOption MimicCd;
    public static CustomNumberOption MimicDur;
    public static CustomNumberOption HackDur;
    public static CustomNumberOption NeutraliseCd;
    public static CustomToggleOption GlitchVent;
    public static CustomToggleOption UniqueGlitch;

    //Juggernaut Options
    public static CustomHeaderOption Juggernaut;
    public static CustomToggleOption JuggVent;
    public static CustomNumberOption AssaultCd;
    public static CustomNumberOption AssaultBonus;
    public static CustomToggleOption UniqueJuggernaut;

    //Cryomaniac Options
    public static CustomHeaderOption Cryomaniac;
    public static CustomNumberOption CryoDouseCd;
    public static CustomToggleOption CryoVent;
    public static CustomToggleOption UniqueCryomaniac;
    public static CustomToggleOption CryoFreezeAll;
    public static CustomToggleOption CryoLastKillerBoost;

    //Arsonist Options
    public static CustomHeaderOption Arsonist;
    public static CustomNumberOption ArsoDouseCd;
    public static CustomNumberOption IgniteCd;
    public static CustomToggleOption ArsoVent;
    public static CustomToggleOption ArsoIgniteAll;
    public static CustomToggleOption ArsoLastKillerBoost;
    public static CustomToggleOption ArsoCooldownsLinked;
    public static CustomToggleOption UniqueArsonist;
    public static CustomToggleOption IgnitionCremates;

    //Murderer Options
    public static CustomHeaderOption Murderer;
    public static CustomToggleOption MurdVent;
    public static CustomNumberOption MurderCd;
    public static CustomToggleOption UniqueMurderer;

    //Serial Killer Options
    public static CustomHeaderOption SerialKiller;
    public static CustomNumberOption BloodlustCd;
    public static CustomNumberOption BloodlustDur;
    public static CustomNumberOption StabCd;
    public static CustomStringOption SKVentOptions;
    public static CustomToggleOption UniqueSerialKiller;

    //Werewolf Options
    public static CustomHeaderOption Werewolf;
    public static CustomNumberOption MaulCd;
    public static CustomNumberOption MaulRadius;
    public static CustomToggleOption WerewolfVent;
    public static CustomToggleOption UniqueWerewolf;

    //NN Options
    public static CustomHeaderOption NNSettings;
    public static CustomNumberOption NNMax;
    public static CustomToggleOption NNHasImpVision;

    //Dracula Options
    public static CustomHeaderOption Dracula;
    public static CustomNumberOption BiteCd;
    public static CustomNumberOption AliveVampCount;
    public static CustomToggleOption DracVent;
    public static CustomToggleOption UniqueDracula;
    public static CustomToggleOption UndeadVent;

    //Necromancer Options
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

    //Whisperer Options
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

    //Jackal Options
    public static CustomHeaderOption Jackal;
    public static CustomNumberOption RecruitCd;
    public static CustomToggleOption JackalVent;
    public static CustomToggleOption RecruitVent;
    public static CustomToggleOption UniqueJackal;

    //NP Options
    public static CustomHeaderOption NPSettings;

    //Phantom Options
    public static CustomHeaderOption Phantom;
    public static CustomNumberOption PhantomTasksRemaining;
    public static CustomToggleOption PhantomPlayersAlerted;

    //Betrayer Options
    public static CustomHeaderOption Betrayer;
    public static CustomNumberOption BetrayCd;
    public static CustomToggleOption BetrayerVent;

    //Ability Options
    public static CustomHeaderOption AbilitySettings;
    public static CustomNumberOption MaxAbilities;
    public static CustomNumberOption MinAbilities;

    //Snitch Options
    public static CustomHeaderOption Snitch;
    public static CustomToggleOption SnitchSeesNeutrals;
    public static CustomToggleOption SnitchSeesCrew;
    public static CustomToggleOption SnitchSeesRoles;
    public static CustomNumberOption SnitchTasksRemaining;
    public static CustomToggleOption SnitchSeestargetsInMeeting;
    public static CustomToggleOption SnitchKnows;
    public static CustomToggleOption UniqueSnitch;

    //Assassin Options
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

    //Underdog Options
    public static CustomHeaderOption Underdog;
    public static CustomToggleOption UniqueUnderdog;
    public static CustomToggleOption UnderdogKnows;
    public static CustomNumberOption UnderdogKillBonus;
    public static CustomToggleOption UnderdogIncreasedKC;

    //Multitasker Options
    public static CustomHeaderOption Multitasker;
    public static CustomToggleOption UniqueMultitasker;
    public static CustomNumberOption Transparancy;

    //Button Barry Options
    public static CustomHeaderOption ButtonBarry;
    public static CustomToggleOption UniqueButtonBarry;
    public static CustomNumberOption ButtonCooldown;

    //Swapper Options
    public static CustomHeaderOption Swapper;
    public static CustomToggleOption UniqueSwapper;
    public static CustomToggleOption SwapperButton;
    public static CustomToggleOption SwapAfterVoting;
    public static CustomToggleOption SwapSelf;

    //Politician Options
    public static CustomHeaderOption Politician;
    public static CustomToggleOption UniquePolitician;
    public static CustomNumberOption PoliticianVoteBank;
    public static CustomToggleOption PoliticianAnonymous;
    public static CustomToggleOption PoliticianButton;

    //Tiebreaker Options
    public static CustomHeaderOption Tiebreaker;
    public static CustomToggleOption UniqueTiebreaker;
    public static CustomToggleOption TiebreakerKnows;

    //Torch Options
    public static CustomHeaderOption Torch;
    public static CustomToggleOption UniqueTorch;

    //Tunneler Options
    public static CustomHeaderOption Tunneler;
    public static CustomToggleOption TunnelerKnows;
    public static CustomToggleOption UniqueTunneler;

    //Radar Options
    public static CustomHeaderOption Radar;
    public static CustomToggleOption UniqueRadar;

    //Insider Options
    public static CustomHeaderOption Insider;
    public static CustomToggleOption InsiderKnows;
    public static CustomToggleOption UniqueInsider;

    //Ruthless Options
    public static CustomHeaderOption Ruthless;
    public static CustomToggleOption UniqueRuthless;
    public static CustomToggleOption RuthlessKnows;

    //Ninja Options
    public static CustomHeaderOption Ninja;
    public static CustomToggleOption UniqueNinja;

    //Objectifier Options
    public static CustomHeaderOption ObjectifierSettings;
    public static CustomNumberOption MaxObjectifiers;
    public static CustomNumberOption MinObjectifiers;

    //Traitor Options
    public static CustomHeaderOption Traitor;
    public static CustomToggleOption UniqueTraitor;
    public static CustomToggleOption TraitorKnows;
    public static CustomToggleOption TraitorColourSwap;
    public static CustomToggleOption SnitchSeesTraitor;
    public static CustomToggleOption RevealerRevealsTraitor;

    //Fanatic Options
    public static CustomHeaderOption Fanatic;
    public static CustomToggleOption FanaticKnows;
    public static CustomToggleOption UniqueFanatic;
    public static CustomToggleOption FanaticColourSwap;
    public static CustomToggleOption SnitchSeesFanatic;
    public static CustomToggleOption RevealerRevealsFanatic;

    //Allied Options
    public static CustomHeaderOption Allied;
    public static CustomStringOption AlliedFaction;
    public static CustomToggleOption UniqueAllied;

    //Corrupted Options
    public static CustomHeaderOption Corrupted;
    public static CustomNumberOption CorruptCd;
    public static CustomToggleOption UniqueCorrupted;
    public static CustomToggleOption AllCorruptedWin;
    public static CustomToggleOption CorruptedVent;

    //Corrupted Options
    public static CustomHeaderOption Overlord;
    public static CustomNumberOption OverlordMeetingWinCount;
    public static CustomToggleOption UniqueOverlord;
    public static CustomToggleOption OverlordKnows;

    //Linked Options
    public static CustomHeaderOption Linked;
    public static CustomToggleOption UniqueLinked;
    public static CustomToggleOption LinkedChat;
    public static CustomToggleOption LinkedRoles;

    //Lovers Options
    public static CustomHeaderOption Lovers;
    public static CustomToggleOption BothLoversDie;
    public static CustomToggleOption LoversChat;
    public static CustomToggleOption LoversRoles;
    public static CustomToggleOption UniqueLovers;

    //Mafia Options
    public static CustomHeaderOption Mafia;
    public static CustomToggleOption MafiaRoles;
    public static CustomToggleOption UniqueMafia;
    public static CustomToggleOption MafVent;

    //Rivals Options
    public static CustomHeaderOption Rivals;
    public static CustomToggleOption RivalsChat;
    public static CustomToggleOption RivalsRoles;
    public static CustomToggleOption UniqueRivals;

    //Taskmaster Options
    public static CustomHeaderOption Taskmaster;
    public static CustomNumberOption TMTasksRemaining;
    public static CustomToggleOption UniqueTaskmaster;

    //Defector Options
    public static CustomHeaderOption Defector;
    public static CustomToggleOption UniqueDefector;
    public static CustomToggleOption DefectorKnows;
    public static CustomStringOption DefectorFaction;

    //Modifier Options
    public static CustomHeaderOption ModifierSettings;
    public static CustomNumberOption MaxModifiers;
    public static CustomNumberOption MinModifiers;

    //Giant Options
    public static CustomHeaderOption Giant;
    public static CustomToggleOption UniqueGiant;
    public static CustomNumberOption GiantSpeed;
    public static CustomNumberOption GiantScale;

    //Dwarf Options
    public static CustomHeaderOption Dwarf;
    public static CustomNumberOption DwarfSpeed;
    public static CustomNumberOption DwarfScale;
    public static CustomToggleOption UniqueDwarf;

    //Diseased Options
    public static CustomHeaderOption Diseased;
    public static CustomNumberOption DiseasedKillMultiplier;
    public static CustomToggleOption DiseasedKnows;
    public static CustomToggleOption UniqueDiseased;

    //Bait Options
    public static CustomHeaderOption Bait;
    public static CustomNumberOption BaitMinDelay;
    public static CustomNumberOption BaitMaxDelay;
    public static CustomToggleOption BaitKnows;
    public static CustomToggleOption UniqueBait;

    //Drunk Options
    public static CustomHeaderOption Drunk;
    public static CustomToggleOption DrunkKnows;
    public static CustomToggleOption DrunkControlsSwap;
    public static CustomNumberOption DrunkInterval;
    public static CustomToggleOption UniqueDrunk;

    //Coward Options
    public static CustomHeaderOption Coward;
    public static CustomToggleOption UniqueCoward;

    //Professional Options
    public static CustomHeaderOption Professional;
    public static CustomToggleOption ProfessionalKnows;
    public static CustomToggleOption UniqueProfessional;

    //Shy Options
    public static CustomHeaderOption Shy;
    public static CustomToggleOption UniqueShy;

    //Astral Options
    public static CustomHeaderOption Astral;
    public static CustomToggleOption UniqueAstral;

    //Yeller Options
    public static CustomHeaderOption Yeller;
    public static CustomToggleOption UniqueYeller;

    //Indomitable Options
    public static CustomHeaderOption Indomitable;
    public static CustomToggleOption UniqueIndomitable;
    public static CustomToggleOption IndomitableKnows;

    //VIP Options
    public static CustomHeaderOption VIP;
    public static CustomToggleOption UniqueVIP;
    public static CustomToggleOption VIPKnows;

    //Volatile Options
    public static CustomHeaderOption Volatile;
    public static CustomNumberOption VolatileInterval;
    public static CustomToggleOption VolatileKnows;
    public static CustomToggleOption UniqueVolatile;

    //Role List Entry Options
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

    //Role List Ban Options
    public static CustomHeaderOption BanList;
    public static RoleListEntryOption Ban1;
    public static RoleListEntryOption Ban2;
    public static RoleListEntryOption Ban3;
    public static RoleListEntryOption Ban4;
    public static RoleListEntryOption Ban5;

    //Free Ban Options
    public static CustomHeaderOption FreeBans;
    public static CustomToggleOption BanCrewmate;
    public static CustomToggleOption BanImpostor;
    public static CustomToggleOption BanAnarchist;

    //Postmortal Options
    public static CustomHeaderOption EnablePostmortals;
    public static CustomToggleOption EnablePhantom;
    public static CustomToggleOption EnableRevealer;
    public static CustomToggleOption EnableGhoul;
    public static CustomToggleOption EnableBanshee;

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

    public static void GenerateAll()
    {
        var maps = new List<string>() { "Skeld", "Mira HQ", "Polus", /*"dlekS",*/ "Airship" };

        if (SubLoaded)
            maps.Add("Submerged");

        if (LILoaded)
            maps.Add("LevelImpostor");
        
        maps.Add("Random");

        SettingsPatches.ExportButton = new();
        SettingsPatches.ImportButton = new();
        SettingsPatches.PresetButton = new();

        var num = 0;

        /*ExampleLayers = new(num++, MultiMenu.Main, "Example Layers Option");
        ExampleHeader = new(num++, MultiMenu.External, "Example Header Option");
        ExampleToggle = new(num++, MultiMenu.External, "Example Toggle Option", true/false);
        ExampleNumber = new(num++, MultiMenu.External, "Example Number Option", 1, 1, 5, 1);
        ExampleString = new(num++, MultiMenu.External, "Example String Option", new[] { "Something", "Something Else", "Something Else Else" });*/

        GameSettings = new(MultiMenu.Main, "Game Settings");
        PlayerSpeed = new(num++, MultiMenu.Main, "Player Speed", 1.25f, 0.25f, 10, 0.25f, MultiplierFormat);
        GhostSpeed = new(num++, MultiMenu.Main, "Ghost Speed", 3, 0.25f, 10, 0.25f, MultiplierFormat);
        InteractionDistance = new(num++, MultiMenu.Main, "Interaction Distance", 2, 0.5f, 5, 0.5f, DistanceFormat);
        EmergencyButtonCount = new(num++, MultiMenu.Main, "Emergency Button Count", 1, 0, 100, 1);
        EmergencyButtonCooldown = new(num++, MultiMenu.Main, "Emergency Button Cooldown", 25, 0, 300, 5, CooldownFormat);
        DiscussionTime = new(num++, MultiMenu.Main, "Discussion Time", 30, 0, 300, 5, CooldownFormat);
        VotingTime = new(num++, MultiMenu.Main, "Voting Time", 60, 5, 600, 15, CooldownFormat);
        TaskBarMode = new(num++, MultiMenu.Main, "Taskbar Updates", new[] { "Meeting", "Rounds", "Invisible" });
        ConfirmEjects = new(num++, MultiMenu.Main, "Confirm Ejects", false);
        EjectionRevealsRole = new(num++, MultiMenu.Main, "Ejection Reveals <color=#FFD700FF>Roles</color>", false, ConfirmEjects);
        EnableInitialCds = new(num++, MultiMenu.Main, "Enable Game Start Cooldowns", true);
        InitialCooldowns = new(num++, MultiMenu.Main, "Game Start Cooldown", 10, 0, 30, 2.5f, CooldownFormat, EnableInitialCds);
        EnableMeetingCds = new(num++, MultiMenu.Main, "Enable Meeting End Cooldowns", true);
        MeetingCooldowns = new(num++, MultiMenu.Main, "Meeting End Cooldown", 15, 0, 30, 2.5f, CooldownFormat, EnableMeetingCds);
        ReportDistance = new(num++, MultiMenu.Main, "Player Report Distance", 3.5f, 1, 20, 0.25f, DistanceFormat);
        ChatCooldown = new(num++, MultiMenu.Main, "Chat Cooldown", 3, 0, 3, 0.1f, CooldownFormat);
        ChatCharacterLimit = new(num++, MultiMenu.Main, "Chat Character Limit", 200, 50, 2000, 50);
        LobbySize = new(num++, MultiMenu.Main, "Lobby Size", 15, 2, 127, 1);

        GameModeSettings = new(MultiMenu.Main, "Game Mode Settings");
        CurrentMode = new(num++, MultiMenu.Main, "Game Mode", new[] { "Classic", "All Any", "<color=#1D7CF2FF>Killing</color> Only", "Role List", "Custom", "Vanilla" });

        KOSettings = new(MultiMenu.Main, "<color=#1D7CF2FF>Killing</color> Only Settings", GameMode.KillingOnly);
        NeutralRoles = new(num++, MultiMenu.Main, "<color=#B3B3B3FF>Neutrals</color> Count", 1, 0, 13, 1, KOSettings);
        AddArsonist = new(num++, MultiMenu.Main, "Add <color=#EE7600FF>Arsonist</color>", false, KOSettings);
        AddCryomaniac = new(num++, MultiMenu.Main, "Add <color=#642DEAFF>Cryomaniac</color>", false, KOSettings);
        AddPlaguebearer = new(num++, MultiMenu.Main, "Add <color=#CFFE61FF>Plaguebearer</color>", false, KOSettings);

        AARLSettings = new(MultiMenu.Main, "All Any/Role List Settings", new object[] { GameMode.AllAny, GameMode.RoleList });
        EnableUniques = new(num++, MultiMenu.Main, "Enable Uniques", false, AARLSettings);

        GameModifiers = new(MultiMenu.Main, "Game Modifiers");
        WhoCanVent = new(num++, MultiMenu.Main, "Serial Venters", new[] { "Default", "Everyone", "Never" });
        AnonymousVoting = new(num++, MultiMenu.Main, "Anonymous Voting", true);
        SkipButtonDisable = new(num++, MultiMenu.Main, "No Skipping", new[] { "Never", "Emergency", "Always" });
        FirstKillShield = new(num++, MultiMenu.Main, "First Kill Shield", false);
        WhoSeesFirstKillShield = new(num++, MultiMenu.Main, "Who Sees First Kill Shield", new[] { "Everyone", "Shielded", "No One" }, FirstKillShield);
        FactionSeeRoles = new(num++, MultiMenu.Main, "Factioned Evils See The <color=#FFD700FF>Roles</color> Of Their Team", true);
        VisualTasks = new(num++, MultiMenu.Main, "Visual Tasks", false);
        NoNames = new(num++, MultiMenu.Main, "No Player Names", false);
        Whispers = new(num++, MultiMenu.Main, "Whispering", true);
        WhispersAnnouncement = new(num++, MultiMenu.Main, "Everyone Is Alerted To Whispers", true, Whispers);
        AppearanceAnimation = new(num++, MultiMenu.Main, "Kill Animations Show Modified Player", true);
        RandomSpawns = new(num++, MultiMenu.Main, "Random Player Spawns", false);
        EnableAbilities = new(num++, MultiMenu.Main, "Enable <color=#FF9900FF>Abilities</color>", true);
        EnableModifiers = new(num++, MultiMenu.Main, "Enable <color=#7F7F7FFF>Modifiers</color>", true);
        EnableObjectifiers = new(num++, MultiMenu.Main, "Enable <color=#DD585BFF>Objectifiers</color>", true);
        VentTargeting = new(num++, MultiMenu.Main, "Players In Vents Can Be Targeted", true);

        GameAnnouncementsSettings = new(MultiMenu.Main, "Game Announcement Settings");
        GameAnnouncements = new(num++, MultiMenu.Main, "Enable Game Announcements", false);
        LocationReports = new(num++, MultiMenu.Main, "Reported Body's Location Is Announced", false, GameAnnouncements);
        RoleFactionReports = new(num++, MultiMenu.Main, "Every Body's <color=#FFD700FF>Role</color>/<color=#00E66DFF>Faction</color> Is Announced", new[] { "Never", "Role", "Faction" },
            GameAnnouncements);
        KillerReports = new(num++, MultiMenu.Main, "Every Body's Killer's <color=#FFD700FF>Role</color>/<color=#00E66DFF>Faction</color> Is Announced", new[] { "Never", "Role", "Faction" },
            GameAnnouncements);

        QualityChanges = new(MultiMenu.Main, "Quality Additions");
        DeadSeeEverything = new(num++, MultiMenu.Main, "Dead Can See Everything", true);
        ParallelMedScans = new(num++, MultiMenu.Main, "Parallel Medbay Scans", false);
        ObstructNames = new(num++, MultiMenu.Main, "Hide Obstructed Player Names", true);

        MapSettings = new(MultiMenu.Main, "Map Settings");
        Map = new(num++, MultiMenu.Main, "Map", maps.ToArray());
        RandomMapSkeld = new(num++, MultiMenu.Main, "Skeld Chance", 0, 0, 100, 10, PercentFormat, MapEnum.Random);
        RandomMapMira = new(num++, MultiMenu.Main, "Mira Chance", 0, 0, 100, 10, PercentFormat, MapEnum.Random);
        RandomMapPolus = new(num++, MultiMenu.Main, "Polus Chance", 0, 0, 100, 10, PercentFormat, MapEnum.Random);
        //RandomMapdlekS = new(num++, MultiMenu.Main, "dlekS Chance", 0, 0, 100, 10, PercentFormat, MapEnum.Random); for when it comes back lol
        RandomMapAirship = new(num++, MultiMenu.Main, "Airship Chance", 0, 0, 100, 10, PercentFormat, MapEnum.Random);

        if (SubLoaded)
            RandomMapSubmerged = new(num++, MultiMenu.Main, "Submerged Chance", 0, 0, 100, 10, PercentFormat, MapEnum.Random);

        if (LILoaded)
            RandomMapLevelImpostor = new(num++, MultiMenu.Main, "Level Impostor Chance", 0, 0, 100, 10, PercentFormat, MapEnum.Random);

        AutoAdjustSettings = new(num++, MultiMenu.Main, "Auto Adjust Settings", false);
        SmallMapHalfVision = new(num++, MultiMenu.Main, "Half Vision On Small Maps", false, new object[] { MapEnum.Skeld, /*MapEnum.dlekS,*/ MapEnum.Random, MapEnum.MiraHQ });
        SmallMapDecreasedCooldown = new(num++, MultiMenu.Main, "Small Maps Decreased Cooldowns", 0f, 0f, 15f, 2.5f, CooldownFormat, new object[] { MapEnum.Skeld, MapEnum.MiraHQ,
            /*MapEnum.dlekS,*/ MapEnum.Random });
        LargeMapIncreasedCooldown = new(num++, MultiMenu.Main, "Large Maps Increased Cooldowns", 0f, 0f, 15f, 2.5f, CooldownFormat, new object[] { MapEnum.Airship, MapEnum.Submerged,
            MapEnum.Random });
        SmallMapIncreasedShortTasks = new(num++, MultiMenu.Main, "Small Maps Increased Short Tasks", 0, 0, 5, 1, new object[] { MapEnum.Skeld, /*MapEnum.dlekS,*/ MapEnum.Random,
            MapEnum.MiraHQ });
        SmallMapIncreasedLongTasks = new(num++, MultiMenu.Main, "Small Maps Increased Long Tasks", 0, 0, 3, 1, new object[] { MapEnum.Airship, MapEnum.Submerged, MapEnum.Random });
        LargeMapDecreasedShortTasks = new(num++, MultiMenu.Main, "Large Maps Decreased Short Tasks", 0, 0, 5, 1, new object[] { MapEnum.Skeld, /*MapEnum.dlekS,*/ MapEnum.Random,
            MapEnum.MiraHQ });
        LargeMapDecreasedLongTasks = new(num++, MultiMenu.Main, "Large Maps Decreased Long Tasks", 0, 0, 3, 1, new object[] { MapEnum.Airship, MapEnum.Submerged, MapEnum.Random });

        BetterSabotages = new(MultiMenu.Main, "Better Sabotages");
        CamouflagedComms = new(num++, MultiMenu.Main, "Camouflaged Comms", true);
        CamouflagedMeetings = new(num++, MultiMenu.Main, "Camouflaged Meetings", false);
        //NightVision = new(num++, MultiMenu.Main, "Night Vision Cameras", false);
        //EvilsIgnoreNV = new(num++, MultiMenu.Main, "Evils Ignore Night Vision", false);
        OxySlow = new(num++, MultiMenu.Main, "Oxygen Sabotage Slows Down Players", true, new object[] { MapEnum.Skeld, /*MapEnum.dlekS,*/ MapEnum.Random, MapEnum.MiraHQ });
        ReactorShake = new(num++, MultiMenu.Main, "Reactor Sabotage Shakes The Screen By", 30, 0, 100, 5, PercentFormat);

        BetterSkeld = new(MultiMenu.Main, "Skeld Settings", new object[] { MapEnum.Skeld, /*MapEnum.dlekS,*/ MapEnum.Random });
        SkeldVentImprovements = new(num++, MultiMenu.Main, "Changed Skeld Vent Layout", false, BetterSkeld);
        SkeldReactorTimer = new(num++, MultiMenu.Main, "Skeld Reactor Meltdown Countdown", 60f, 30f, 90f, 5f, CooldownFormat, BetterSkeld);
        SkeldO2Timer = new(num++, MultiMenu.Main, "Skeld Oxygen Depletion Countdown", 60f, 30f, 90f, 5f, CooldownFormat, BetterSkeld);

        BetterMiraHQ = new(MultiMenu.Main, "Mira HQ Settings", new object[] { MapEnum.MiraHQ, MapEnum.Random });
        MiraHQVentImprovements = new(num++, MultiMenu.Main, "Changed Mira HQ Vent Layout", false, BetterMiraHQ);
        MiraReactorTimer = new(num++, MultiMenu.Main, "Mira HQ Reactor Meltdown Countdown", 60f, 30f, 90f, 5f, CooldownFormat, BetterMiraHQ);
        MiraO2Timer = new(num++, MultiMenu.Main, "Mira HQ Oxygen Depletion Countdown", 60f, 30f, 90f, 5f, CooldownFormat, BetterMiraHQ);

        BetterPolus = new(MultiMenu.Main, "Polus Settings", new object[] { MapEnum.Polus, MapEnum.Random });
        PolusVentImprovements = new(num++, MultiMenu.Main, "Changed Polus Vent Layout", false, BetterPolus);
        VitalsLab = new(num++, MultiMenu.Main, "Vitals Moved To Lab", false, BetterPolus);
        ColdTempDeathValley = new(num++, MultiMenu.Main, "Cold Temp Moved To Death Valley", false, BetterPolus);
        WifiChartCourseSwap = new(num++, MultiMenu.Main, "Reboot Wifi And Chart Course Swapped", false, BetterPolus);
        SeismicTimer = new(num++, MultiMenu.Main, "Seimic Stabliser Malfunction Countdown", 60f, 30f, 90f, 5f, CooldownFormat, BetterPolus);

        BetterAirship = new(MultiMenu.Main, "Airship Settings", new object[] { MapEnum.Airship, MapEnum.Random });
        SpawnType = new(num++, MultiMenu.Main, "Spawn Type", new[] { "Normal", "Fixed", "Synchronised", "Meeting" }, BetterAirship);
        MoveVitals = new(num++, MultiMenu.Main, "Move Vitals", false, BetterAirship);
        MoveFuel = new(num++, MultiMenu.Main, "Move Fuel", false, BetterAirship);
        MoveDivert = new(num++, MultiMenu.Main, "Move Divert Power", false, BetterAirship);
        MoveAdmin = new(num++, MultiMenu.Main, "Move Admin Table", new[] { "Don't Move", "Cockpit", "Main Hall" }, BetterAirship);
        MoveElectrical = new(num++, MultiMenu.Main, "Move Electrical Outlet", new[] { "Don't Move", "Vault", "Electrical" }, BetterAirship);
        MinDoorSwipeTime = new(num++, MultiMenu.Main, "Min Time For Door Swipe", 0.4f, 0f, 10f, 0.1f, BetterAirship);
        CrashTimer = new(num++, MultiMenu.Main, "Heli Crash Countdown", 90f, 30f, 100f, 5f, BetterAirship);

        CrewSettings = new(MultiMenu.Crew, "<color=#8CFFFFFF>Crew</color> Settings");
        CommonTasks = new(num++, MultiMenu.Crew, "Common Tasks", 2, 0, 100, 1);
        LongTasks = new(num++, MultiMenu.Crew, "Long Tasks", 1, 0, 100, 1);
        ShortTasks = new(num++, MultiMenu.Crew, "Short Tasks", 4, 0, 100, 1);
        GhostTasksCountToWin = new(num++, MultiMenu.Crew, "<color=#8CFFFFFF>Crew</color> Ghost Tasks Count To Win", true);
        CrewVision = new(num++, MultiMenu.Crew, "<color=#8CFFFFFF>Crew</color> Vision", 1f, 0.25f, 5f, 0.25f, MultiplierFormat);
        CrewFlashlight = new(num++, MultiMenu.Crew, "<color=#8CFFFFFF>Crew</color> Uses A Flashlight", false);
        CrewMax = new(num++, MultiMenu.Crew, "Max <color=#8CFFFFFF>Crew</color> <color=#FFD700FF>Roles</color>", 5, 0, 14, 1, new object[] { GameMode.Classic, GameMode.AllAny,
            GameMode.Custom });
        CrewMin = new(num++, MultiMenu.Crew, "Min <color=#8CFFFFFF>Crew</color> <color=#FFD700FF>Roles</color>", 5, 0, 14, 1, new object[] { GameMode.Classic, GameMode.AllAny,
            GameMode.Custom });
        CrewVent = new(num++, MultiMenu.Crew, "<color=#8CFFFFFF>Crew</color> Can Vent", false);

        CARoles = new(MultiMenu.Crew, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Auditor</color> <color=#FFD700FF>Roles</color>", new object[] { GameMode.Classic, GameMode.AllAny,
            GameMode.Custom });
        MysticOn = new(num++, MultiMenu.Crew, "<color=#708EEFFF>Mystic</color>", parent: CARoles);
        VampireHunterOn = new(num++, MultiMenu.Crew, "<color=#C0C0C0FF>Vampire Hunter</color>", parent: CARoles);

        CIRoles = new(MultiMenu.Crew, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Investigative</color> <color=#FFD700FF>Roles</color>", new object[] { GameMode.Classic,
            GameMode.AllAny, GameMode.Custom });
        CoronerOn = new(num++, MultiMenu.Crew, "<color=#4D99E6FF>Coroner</color>", parent: CIRoles);
        DetectiveOn = new(num++, MultiMenu.Crew, "<color=#4D4DFFFF>Detective</color>", parent: CIRoles);
        InspectorOn = new(num++, MultiMenu.Crew, "<color=#7E3C64FF>Inspector</color>", parent: CIRoles);
        MediumOn = new(num++, MultiMenu.Crew, "<color=#A680FFFF>Medium</color>", parent: CIRoles);
        OperativeOn = new(num++, MultiMenu.Crew, "<color=#A7D1B3FF>Operative</color>", parent: CIRoles);
        SeerOn = new(num++, MultiMenu.Crew, "<color=#71368AFF>Seer</color>", parent: CIRoles);
        SheriffOn = new(num++, MultiMenu.Crew, "<color=#FFCC80FF>Sheriff</color>", parent: CIRoles);
        TrackerOn = new(num++, MultiMenu.Crew, "<color=#009900FF>Tracker</color>", parent: CIRoles);

        CKRoles = new(MultiMenu.Crew, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Killing</color> <color=#FFD700FF>Roles</color>", new object[] { GameMode.Classic, GameMode.AllAny,
            GameMode.Custom });
        VeteranOn = new(num++, MultiMenu.Crew, "<color=#998040FF>Veteran</color>", parent: CKRoles);
        VigilanteOn = new(num++, MultiMenu.Crew, "<color=#FFFF00FF>Vigilante</color>", parent: CKRoles);

        CPRoles = new(MultiMenu.Crew, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Protective</color> <color=#FFD700FF>Roles</color>", new object[] { GameMode.Classic, GameMode.AllAny,
            GameMode.Custom });
        AltruistOn = new(num++, MultiMenu.Crew, "<color=#660000FF>Altruist</color>", parent: CPRoles);
        MedicOn = new(num++, MultiMenu.Crew, "<color=#006600FF>Medic</color>", parent: CPRoles);

        CSvRoles = new(MultiMenu.Crew, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Sovereign</color> <color=#FFD700FF>Roles</color>", new object[] { GameMode.Classic, GameMode.AllAny,
            GameMode.Custom });
        DictatorOn = new(num++, MultiMenu.Crew, "<color=#00CB97FF>Dictator</color>", parent: CSvRoles);
        MayorOn = new(num++, MultiMenu.Crew, "<color=#704FA8FF>Mayor</color>", parent: CSvRoles);
        MonarchOn = new(num++, MultiMenu.Crew, "<color=#FF004EFF>Monarch</color>", parent: CSvRoles);

        CSRoles = new(MultiMenu.Crew, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Support</color> <color=#FFD700FF>Roles</color>", new object[] { GameMode.Classic, GameMode.AllAny,
            GameMode.Custom });
        ChameleonOn = new(num++, MultiMenu.Crew, "<color=#5411F8FF>Chameleon</color>", parent: CSRoles);
        EngineerOn = new(num++, MultiMenu.Crew, "<color=#FFA60AFF>Engineer</color>", parent: CSRoles);
        EscortOn = new(num++, MultiMenu.Crew, "<color=#803333FF>Escort</color>", parent: CSRoles);
        RetributionistOn = new(num++, MultiMenu.Crew, "<color=#8D0F8CFF>Retributionist</color>", parent: CSRoles);
        ShifterOn = new(num++, MultiMenu.Crew, "<color=#DF851FFF>Shifter</color>", parent: CSRoles);
        TransporterOn = new(num++, MultiMenu.Crew, "<color=#00EEFFFF>Transporter</color>", parent: CSRoles);

        CURoles = new(MultiMenu.Crew, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Utility</color> <color=#FFD700FF>Roles</color>", new object[] { GameMode.Classic, GameMode.AllAny,
            GameMode.Custom });
        CrewmateOn = new(num++, MultiMenu.Crew, "<color=#8CFFFFFF>Crewmate</color>", parent: CURoles);
        RevealerOn = new(num++, MultiMenu.Crew, "<color=#D3D3D3FF>Revealer</color>", parent: CURoles);

        CASettings = new(MultiMenu.Crew, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Auditor</color> Settings", new object[] { VampireHunterOn, MysticOn, LayerEnum.VampireHunter,
            LayerEnum.CrewAudit, LayerEnum.Mystic, LayerEnum.RandomCrew });
        CAMax = new(num++, MultiMenu.Crew, "Max <color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Auditors</color>", 1, 1, 14, 1, CASettings);

        Mystic = new(MultiMenu.Crew, "<color=#708EEFFF>Mystic</color>", new object[] { MysticOn, LayerEnum.CrewAudit, LayerEnum.Mystic, LayerEnum.RandomCrew });
        UniqueMystic = new(num++, MultiMenu.Crew, "<color=#708EEFFF>Mystic</color> Is Unique", false, new object[] { Mystic, EnableUniques }, true);
        MysticRevealCd = new(num++, MultiMenu.Crew, "Reveal Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Mystic);

        VampireHunter = new(MultiMenu.Crew, "<color=#C0C0C0FF>Vampire Hunter</color>", new object[] { VampireHunterOn, LayerEnum.VampireHunter, LayerEnum.CrewAudit, LayerEnum.RandomCrew });
        UniqueVampireHunter = new(num++, MultiMenu.Crew, "<color=#C0C0C0FF>Vampire Hunter</color> Is Unique", false, new object[] { VampireHunter, EnableUniques }, true);
        StakeCd = new(num++, MultiMenu.Crew, "Stake Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, VampireHunter);

        CISettings = new(MultiMenu.Crew, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Investigative</color> Settings", new object[] { CoronerOn, DetectiveOn, InspectorOn, SeerOn,
            MediumOn, SheriffOn, TrackerOn, OperativeOn, LayerEnum.CrewInvest, LayerEnum.Coroner, LayerEnum.Detective, LayerEnum.Inspector, LayerEnum.Seer, LayerEnum.Medium,
            LayerEnum.Sheriff, LayerEnum.Tracker, LayerEnum.Operative, LayerEnum.RandomCrew, LayerEnum.Mystic, LayerEnum.CrewAudit, MysticOn });
        CIMax = new(num++, MultiMenu.Crew, "Max <color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Investigatives</color>", 1, 1, 14, 1, CISettings);

        Coroner = new(MultiMenu.Crew, "<color=#4D99E6FF>Coroner</color>", new object[] { CoronerOn, LayerEnum.Coroner, LayerEnum.CrewInvest, LayerEnum.RandomCrew });
        UniqueCoroner = new(num++, MultiMenu.Crew, "<color=#4D99E6FF>Coroner</color> Is Unique", false, new object[] { Coroner, EnableUniques }, true);
        CoronerArrowDur = new(num++, MultiMenu.Crew, "Dead Body Arrow Duration", 0.1f, 0f, 2f, 0.05f, CooldownFormat, Coroner);
        CoronerReportRole = new(num++, MultiMenu.Crew, "<color=#4D99E6FF>Coroner</color> Gets Roles", false, Coroner);
        CoronerReportName = new(num++, MultiMenu.Crew, "<color=#4D99E6FF>Coroner</color> Gets Killer's Name", false, Coroner);
        CoronerKillerNameTime = new(num++, MultiMenu.Crew, "<color=#4D99E6FF>Coroner</color> Gets Killer's Name Under", 1f, 0.5f, 15f, 0.5f, CooldownFormat, Coroner);
        CompareCd = new(num++, MultiMenu.Crew, "Compare Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Coroner);
        AutopsyCd = new(num++, MultiMenu.Crew, "Autopsy Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Coroner);

        Detective = new(MultiMenu.Crew, "<color=#4D4DFFFF>Detective</color>", new object[] { DetectiveOn, LayerEnum.Detective, LayerEnum.CrewInvest, LayerEnum.RandomCrew });
        UniqueDetective = new(num++, MultiMenu.Crew, "<color=#4D4DFFFF>Detective</color> Is Unique", false, new object[] { Detective, EnableUniques }, true);
        ExamineCd = new(num++, MultiMenu.Crew, "Examine Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Detective);
        RecentKill = new(num++, MultiMenu.Crew, "Bloody Hands Duration", 10f, 5f, 60f, 2.5f, CooldownFormat, Detective);
        FootprintInterval = new(num++, MultiMenu.Crew, "Footprint Interval", 0.15f, 0.05f, 2f, 0.05f, CooldownFormat, Detective);
        FootprintDur = new(num++, MultiMenu.Crew, "Footprint Duration", 10f, 1f, 10f, 0.5f, CooldownFormat, Detective);
        AnonymousFootPrint = new(num++, MultiMenu.Crew, "Anonymous Footprint", false, Detective);

        Inspector = new(MultiMenu.Crew, "<color=#7E3C64FF>Inspector</color>", new object[] { InspectorOn, LayerEnum.Inspector, LayerEnum.CrewInvest, LayerEnum.RandomCrew });
        UniqueInspector = new(num++, MultiMenu.Crew, "<color=#7E3C64FF>Inspector</color> Is Unique", false, Inspector);
        InspectCd = new(num++, MultiMenu.Crew, "Inspect Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Inspector);

        Medium = new(MultiMenu.Crew, "<color=#A680FFFF>Medium</color>", new object[] { MediumOn, LayerEnum.Medium, LayerEnum.CrewInvest, LayerEnum.RandomCrew });
        UniqueMedium = new(num++, MultiMenu.Crew, "<color=#A680FFFF>Medium</color> Is Unique", false, new object[] { Medium, EnableUniques }, true);
        MediateCd = new(num++, MultiMenu.Crew, "Mediate Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Medium);
        ShowMediatePlayer = new(num++, MultiMenu.Crew, "Reveal Appearance Of Mediate Target", true, Medium);
        ShowMediumToDead = new(num++, MultiMenu.Crew, "Reveal The <color=#A680FFFF>Medium</color> To The Mediate Target", new[] { "No", "Target", "All Dead" }, Medium);
        DeadRevealed = new(num++, MultiMenu.Crew, "Who Is Revealed With Mediate", new[] { "Oldest Dead", "Newest Dead", "All Dead", "Random" }, Medium);

        Operative = new(MultiMenu.Crew, "<color=#A7D1B3FF>Operative</color>", new object[] { OperativeOn, LayerEnum.Operative, LayerEnum.CrewInvest, LayerEnum.RandomCrew });
        UniqueOperative = new(num++, MultiMenu.Crew, "<color=#A7D1B3FF>Operative</color> Is Unique", false, new object[] { Operative, EnableUniques }, true);
        BugCd = new(num++, MultiMenu.Crew, "Bug Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Operative);
        MinAmountOfTimeInBug = new(num++, MultiMenu.Crew, "Min Amount Of Time In Bug To Trigger", 0f, 0f, 15f, 0.5f, CooldownFormat, Operative);
        BugsRemoveOnNewRound = new(num++, MultiMenu.Crew, "Bugs Are Removed Each Round", true, Operative);
        MaxBugs = new(num++, MultiMenu.Crew, "Max Bugs", 5, 1, 15, 1, Operative);
        BugRange = new(num++, MultiMenu.Crew, "Bug Radius", 1.5f, 0.5f, 5f, 0.25f, DistanceFormat, Operative);
        MinAmountOfPlayersInBug = new(num++, MultiMenu.Crew, "Number Of <color=#FFD700FF>Roles</color> Required To Trigger Bug", 1, 1, 5, 1, Operative);
        WhoSeesDead = new(num++, MultiMenu.Crew, "Who Sees Dead Bodies On Admin", new[] { "Nobody", "Operative", "Everyone But Operative", "Everyone" }, Operative);
        PreciseOperativeInfo = new(num++, MultiMenu.Crew, "<color=#A7D1B3FF>Operative</color> Gets Precise Information", false, Operative);

        Seer = new(MultiMenu.Crew, "<color=#71368AFF>Seer</color>", new object[] { SeerOn, LayerEnum.Seer, LayerEnum.CrewInvest, LayerEnum.RandomCrew, LayerEnum.Mystic, LayerEnum.CrewAudit,
            MysticOn });
        UniqueSeer = new(num++, MultiMenu.Crew, "<color=#71368AFF>Seer</color> Is Unique", false, new object[] { Seer, EnableUniques }, true);
        SeerCd = new(num++, MultiMenu.Crew, "Seer Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Seer);

        Sheriff = new(MultiMenu.Crew, "<color=#FFCC80FF>Sheriff</color>", new object[] { SheriffOn, LayerEnum.Sheriff, LayerEnum.CrewInvest, LayerEnum.RandomCrew, LayerEnum.Seer, SeerOn });
        UniqueSheriff = new(num++, MultiMenu.Crew, "<color=#FFCC80FF>Sheriff</color> Is Unique", false, new object[] { Sheriff, EnableUniques }, true);
        InterrogateCd = new(num++, MultiMenu.Crew, "Interrogate Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Sheriff);
        NeutEvilRed = new(num++, MultiMenu.Crew, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Evils</color> Show Evil", false, Sheriff);
        NeutKillingRed = new(num++, MultiMenu.Crew, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killers</color> Show Evil", false, Sheriff);

        Tracker = new(MultiMenu.Crew, "<color=#009900FF>Tracker</color>", new object[] { TrackerOn, LayerEnum.Tracker, LayerEnum.CrewInvest, LayerEnum.RandomCrew });
        UniqueTracker = new(num++, MultiMenu.Crew, "<color=#009900FF>Tracker</color> Is Unique", false, new object[] { Tracker, EnableUniques }, true);
        UpdateInterval = new(num++, MultiMenu.Crew, "Arrow Update Interval", 5f, 0f, 15f, 0.5f, CooldownFormat, Tracker);
        TrackCd = new(num++, MultiMenu.Crew, "Track Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Tracker);
        ResetOnNewRound = new(num++, MultiMenu.Crew, "<color=#009900FF>Tracker</color> Arrows Reset After Each Round", false, Tracker);
        MaxTracks = new(num++, MultiMenu.Crew, "Max Tracks", 5, 1, 15, 1, Tracker);

        CKSettings = new(MultiMenu.Crew, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Killing</color> Settings", new object[] { VigilanteOn, VeteranOn, LayerEnum.Vigilante,
            LayerEnum.Veteran, LayerEnum.CrewKill, LayerEnum.RandomCrew, VampireHunterOn, LayerEnum.CrewAudit, LayerEnum.VampireHunter });
        CKMax = new(num++, MultiMenu.Crew, "Max <color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Killers</color>", 1, 1, 14, 1, CKSettings);

        Veteran = new(MultiMenu.Crew, "<color=#998040FF>Veteran</color>", new object[] { VeteranOn, LayerEnum.Veteran, LayerEnum.CrewKill, LayerEnum.RandomCrew });
        UniqueVeteran = new(num++, MultiMenu.Crew, "<color=#998040FF>Veteran</color> Is Unique", false, new object[] { Veteran, EnableUniques }, true);
        AlertCd = new(num++, MultiMenu.Crew, "Alert Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Veteran);
        AlertDur = new(num++, MultiMenu.Crew, "Alert Duration", 10f, 5f, 30f, 1f, CooldownFormat, Veteran);
        MaxAlerts = new(num++, MultiMenu.Crew, "Max Alerts", 5, 1, 15, 1, Veteran);

        Vigilante = new(MultiMenu.Crew, "<color=#FFFF00FF>Vigilante</color>", new object[] { VigilanteOn, LayerEnum.Vigilante, LayerEnum.CrewKill, LayerEnum.RandomCrew, LayerEnum.CrewAudit,
            LayerEnum.VampireHunter, VampireHunterOn });
        UniqueVigilante = new(num++, MultiMenu.Crew, "<color=#FFFF00FF>Vigilante</color> Is Unique", false, new object[] { Vigilante, EnableUniques }, true);
        MisfireKillsInno = new(num++, MultiMenu.Crew, "Misfire Kills The Target", true, Vigilante);
        VigiKillAgain = new(num++, MultiMenu.Crew, "<color=#FFFF00FF>Vigilante</color> Can Kill Again If Target Was Innocent", true, Vigilante);
        RoundOneNoShot = new(num++, MultiMenu.Crew, "<color=#FFFF00FF>Vigilante</color> Cannot Shoot On The First Round", true, Vigilante);
        VigiOptions = new(num++, MultiMenu.Crew, "How Does <color=#FFFF00FF>Vigilante</color> Die", new[] { "Immediately", "Before Meeting", "After Meeting" }, Vigilante);
        VigiNotifOptions = new(num++, MultiMenu.Crew, "How Is The <color=#FFFF00FF>Vigilante</color> Notified Of Their Target's Innocence", new[] { "Never", "Flash", "Message" },
            Vigilante);
        ShootCd = new(num++, MultiMenu.Crew, "Shoot Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Vigilante);
        MaxBullets = new(num++, MultiMenu.Crew, "Max Bullets", 5, 1, 15, 1, Vigilante);

        CPSettings = new(MultiMenu.Crew, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Protective</color> Settings", new object[] { AltruistOn, MedicOn, LayerEnum.Altruist,
            LayerEnum.Medic, LayerEnum.CrewProt, LayerEnum.RandomCrew });
        CPMax = new(num++, MultiMenu.Crew, "Max <color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Protectives</color>", 1, 1, 14, 1, CPSettings);

        Altruist = new(MultiMenu.Crew, "<color=#660000FF>Altruist</color>", new object[] { AltruistOn, LayerEnum.Altruist, LayerEnum.CrewProt, LayerEnum.RandomCrew });
        UniqueAltruist = new(num++, MultiMenu.Crew, "<color=#660000FF>Altruist</color> Is Unique", false, new object[] { Altruist, EnableUniques }, true);
        MaxRevives = new(num++, MultiMenu.Crew, "Max Revives", 5, 1, 14, 1, Altruist);
        ReviveCd = new(num++, MultiMenu.Crew, "Revive Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Altruist);
        ReviveDur = new(num++, MultiMenu.Crew, "Revive Duration", 10f, 1f, 15f, 1f, CooldownFormat, Altruist);
        AltruistTargetBody = new(num++, MultiMenu.Crew, "Target's Body Disappears On Beginning Of Revive", false, Altruist);

        Medic = new(MultiMenu.Crew, "<color=#006600FF>Medic</color>", new object[] { MedicOn, LayerEnum.Medic, LayerEnum.CrewProt, LayerEnum.RandomCrew });
        UniqueMedic = new(num++, MultiMenu.Crew, "<color=#006600FF>Medic</color> Is Unique", false, new object[] { Medic, EnableUniques }, true);
        ShowShielded = new(num++, MultiMenu.Crew, "Show Shielded Player", new[] { "Self", "<color=#006600FF>Medic</color>", "Self And <color=#006600FF>Medic</color>", "Everyone", "Nobody"
            }, Medic);
        WhoGetsNotification = new(num++, MultiMenu.Crew, "Who Gets Murder Attempt Indicator", new[] { "<color=#006600FF>Medic</color>", "Self", "Self And <color=#006600FF>Medic</color>",
            "Everyone", "Nobody" }, Medic);
        ShieldBreaks = new(num++, MultiMenu.Crew, "Shield Breaks On Murder Attempt", true, Medic);

        CSvSettings = new(MultiMenu.Crew, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Sovereign</color> Settings", new object[] { MayorOn, DictatorOn, MonarchOn, LayerEnum.Monarch,
            LayerEnum.Mayor, LayerEnum.Monarch, LayerEnum.CrewSov, LayerEnum.RandomCrew });
        CSvMax = new(num++, MultiMenu.Crew, "Max <color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Sovereigns</color>", 1, 1, 14, 1, CSvSettings);

        Dictator = new(MultiMenu.Crew, "<color=#00CB97FF>Dictator</color>", new object[] { DictatorOn, LayerEnum.Dictator, LayerEnum.CrewSov, LayerEnum.RandomCrew });
        UniqueDictator = new(num++, MultiMenu.Crew, "<color=#00CB97FF>Dictator</color> Is Unique", false, new object[] { Dictator, EnableUniques }, true);
        RoundOneNoDictReveal = new(num++, MultiMenu.Crew, "<color=#00CB97FF>Dictator</color> Cannot Reveal Round One", false, Dictator);
        DictateAfterVoting = new(num++, MultiMenu.Crew, "<color=#00CB97FF>Dictator</color> Can Dictate After Voting", false, Dictator);
        DictatorButton = new(num++, MultiMenu.Crew, "<color=#00CB97FF>Dictator</color> Can Button", true, Dictator);

        Mayor = new(MultiMenu.Crew, "<color=#704FA8FF>Mayor</color>", new object[] { MayorOn, LayerEnum.Mayor, LayerEnum.CrewSov, LayerEnum.RandomCrew });
        UniqueMayor = new(num++, MultiMenu.Crew, "<color=#704FA8FF>Mayor</color> Is Unique", false, new object[] { Mayor, EnableUniques }, true);
        MayorVoteCount = new(num++, MultiMenu.Crew, "Revealed <color=#704FA8FF>Mayor</color> Votes Count As", 2, 1, 10, 1, Mayor);
        RoundOneNoMayorReveal = new(num++, MultiMenu.Crew, "<color=#704FA8FF>Mayor</color> Cannot Reveal Round One", false, Mayor);
        MayorButton = new(num++, MultiMenu.Crew, "<color=#704FA8FF>Mayor</color> Can Button", true, Mayor);

        Monarch = new(MultiMenu.Crew, "<color=#FF004EFF>Monarch</color>", new object[] { MonarchOn, LayerEnum.Monarch, LayerEnum.CrewSov, LayerEnum.RandomCrew });
        UniqueMonarch = new(num++, MultiMenu.Crew, "<color=#FF004EFF>Monarch</color> Is Unique", false, new object[] { Monarch, EnableUniques }, true);
        KnightingCd = new(num++, MultiMenu.Crew, "Knighting Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Monarch);
        RoundOneNoKnighting = new(num++, MultiMenu.Crew, "<color=#FF004EFF>Monarch</color> Cannot Knight Round One", false, Monarch);
        KnightCount = new(num++, MultiMenu.Crew, "Knight Count", 2, 1, 14, 1, Monarch);
        KnightVoteCount = new(num++, MultiMenu.Crew, "Knighted Votes Count As", 1, 1, 10, 1, Monarch);
        MonarchButton = new(num++, MultiMenu.Crew, "<color=#FF004EFF>Monarch</color> Can Button", true, Monarch);
        KnightButton = new(num++, MultiMenu.Crew, "Knights Can Button", true, Monarch);

        CSSettings = new(MultiMenu.Crew, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Support</color> Settings", new object[] { ChameleonOn, EngineerOn, EscortOn, RetributionistOn,
            ShifterOn, TransporterOn, LayerEnum.RandomCrew, LayerEnum.Chameleon, LayerEnum.Engineer, LayerEnum.Escort, LayerEnum.Retributionist, LayerEnum.Shifter, LayerEnum.Transporter,
            LayerEnum.CrewSupport });
        CSMax = new(num++, MultiMenu.Crew, "Max <color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Supporters</color>", 1, 1, 14, 1, CSSettings);

        Chameleon = new(MultiMenu.Crew, "<color=#5411F8FF>Chameleon</color>", new object[] { ChameleonOn, LayerEnum.Chameleon, LayerEnum.CrewSupport, LayerEnum.RandomCrew });
        UniqueChameleon = new(num++, MultiMenu.Crew, "<color=#5411F8FF>Chameleon</color> Is Unique", false, new object[] { Chameleon, EnableUniques }, true);
        SwoopCd = new(num++, MultiMenu.Crew, "Swoop Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Chameleon);
        SwoopDur = new(num++, MultiMenu.Crew, "Swoop Duration", 10f, 5f, 30f, 1f, CooldownFormat, Chameleon);
        MaxSwoops = new(num++, MultiMenu.Crew, "Max Swoops", 5, 1, 15, 1, Chameleon);

        Engineer = new(MultiMenu.Crew, "<color=#FFA60AFF>Engineer</color>", new object[] { EngineerOn, LayerEnum.Engineer, LayerEnum.CrewSupport, LayerEnum.RandomCrew });
        UniqueEngineer = new(num++, MultiMenu.Crew, "<color=#FFA60AFF>Engineer</color> Is Unique", false, new object[] { Engineer, EnableUniques }, true);
        FixCd = new(num++, MultiMenu.Crew, "Fix Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Engineer);
        MaxFixes = new(num++, MultiMenu.Crew, "Max Fixes", 5, 1, 15, 1, Engineer);

        Escort = new(MultiMenu.Crew, "<color=#803333FF>Escort</color>", new object[] { EscortOn, LayerEnum.Engineer, LayerEnum.CrewSupport, LayerEnum.RandomCrew });
        UniqueEscort = new(num++, MultiMenu.Crew, "<color=#803333FF>Escort</color> Is Unique", false, new object[] { Escort, EnableUniques }, true);
        EscortCd = new(num++, MultiMenu.Crew, "Roleblock Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Escort);
        EscortDur = new(num++, MultiMenu.Crew, "Roleblock Duration", 10f, 5f, 30f, 1f, CooldownFormat, Escort);

        Retributionist = new(MultiMenu.Crew, "<color=#8D0F8CFF>Retributionist</color>", new object[] { RetributionistOn, LayerEnum.Retributionist, LayerEnum.CrewSupport,
            LayerEnum.RandomCrew });
        UniqueRetributionist = new(num++, MultiMenu.Crew, "<color=#8D0F8CFF>Retributionist</color> Is Unique", false, new object[] { Retributionist, EnableUniques }, true);
        ReviveAfterVoting = new(num++, MultiMenu.Crew, "<color=#8D0F8CFF>Retributionist</color> Can Mimic After Voting", true, Retributionist);
        MaxUses = new(num++, MultiMenu.Crew, "Total Limit On Limited Abilities", 5, 1, 15, 1, Retributionist);

        Shifter = new(MultiMenu.Crew, "<color=#DF851FFF>Shifter</color>", new object[] { ShifterOn, LayerEnum.Shifter, LayerEnum.CrewSupport, LayerEnum.RandomCrew });
        UniqueShifter = new(num++, MultiMenu.Crew, "<color=#DF851FFF>Shifter</color> Is Unique", false, new object[] { Shifter, EnableUniques }, true);
        ShiftCd = new(num++, MultiMenu.Crew, "Shift Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Shifter);
        ShiftedBecomes = new(num++, MultiMenu.Crew, "Shifted Becomes", new[] { "Shifter", "Crewmate" }, Shifter);

        Transporter = new(MultiMenu.Crew, "<color=#00EEFFFF>Transporter</color>", new object[] { TransporterOn, LayerEnum.Transporter, LayerEnum.CrewSupport, LayerEnum.RandomCrew });
        UniqueTransporter = new(num++, MultiMenu.Crew, "<color=#00EEFFFF>Transporter</color> Is Unique", false, new object[] { Transporter, EnableUniques }, true);
        TransportCd = new(num++, MultiMenu.Crew, "Transport Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Transporter);
        TransportDur = new(num++, MultiMenu.Crew, "Transport Duration", 5f, 1f, 20f, 1f, CooldownFormat, Transporter);
        MaxTransports = new(num++, MultiMenu.Crew, "Max Transports", 5, 1, 15, 1, Transporter);
        TransSelf = new(num++, MultiMenu.Crew, "<color=#00EEFFFF>Transporter</color> Can Transport Themselves", true, Transporter);

        CUSettings = new(MultiMenu.Crew, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Utility</color> Settings", RevealerOn);

        Revealer = new(MultiMenu.Crew, "<color=#D3D3D3FF>Revealer</color>", RevealerOn);
        RevealerTasksRemainingClicked = new(num++, MultiMenu.Crew, "Tasks Remaining When <color=#D3D3D3FF>Revealer</color> Can Be Clicked", 5, 1, 10, 1, Revealer);
        RevealerTasksRemainingAlert = new(num++, MultiMenu.Crew, "Tasks Remaining When Revealed", 1, 1, 5, 1, Revealer);
        RevealerRevealsNeutrals = new(num++, MultiMenu.Crew, "<color=#D3D3D3FF>Revealer</color> Reveals <color=#B3B3B3FF>Neutrals</color>", false, Revealer);
        RevealerRevealsCrew = new(num++, MultiMenu.Crew, "<color=#D3D3D3FF>Revealer</color> Reveals <color=#8CFFFFFF>Crew</color>", false, Revealer);
        RevealerRevealsRoles = new(num++, MultiMenu.Crew, "<color=#D3D3D3FF>Revealer</color> Reveals Exact <color=#FFD700FF>Roles</color>", false, Revealer);
        RevealerCanBeClickedBy = new(num++, MultiMenu.Crew, "Who Can Click <color=#D3D3D3FF>Revealer</color>", new[] { "All", "Non Crew", "Evils Only" }, Revealer);

        NBRoles = new(MultiMenu.Neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Benign</color> <color=#FFD700FF>Roles</color>", new object[] { GameMode.Classic, GameMode.AllAny,
            GameMode.Custom });
        AmnesiacOn = new(num++, MultiMenu.Neutral, "<color=#22FFFFFF>Amnesiac</color>", parent: NBRoles);
        GuardianAngelOn = new(num++, MultiMenu.Neutral, "<color=#FFFFFFFF>Guardian Angel</color>", parent: NBRoles);
        SurvivorOn = new(num++, MultiMenu.Neutral, "<color=#DDDD00FF>Survivor</color>", parent: NBRoles);
        ThiefOn = new(num++, MultiMenu.Neutral, "<color=#80FF00FF>Thief</color>", parent: NBRoles);

        NERoles = new(MultiMenu.Neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Evil</color> <color=#FFD700FF>Roles</color>", new object[] { GameMode.Classic, GameMode.AllAny,
            GameMode.Custom });
        ActorOn = new(num++, MultiMenu.Neutral, "<color=#00ACC2FF>Actor</color>", parent: NERoles);
        BountyHunterOn = new(num++, MultiMenu.Neutral, "<color=#B51E39FF>Bounty Hunter</color>", parent: NERoles);
        CannibalOn = new(num++, MultiMenu.Neutral, "<color=#8C4005FF>Cannibal</color>", parent: NERoles);
        ExecutionerOn = new(num++, MultiMenu.Neutral, "<color=#CCCCCCFF>Executioner</color>", parent: NERoles);
        GuesserOn = new(num++, MultiMenu.Neutral, "<color=#EEE5BEFF>Guesser</color>", parent: NERoles);
        JesterOn = new(num++, MultiMenu.Neutral, "<color=#F7B3DAFF>Jester</color>", parent: NERoles);
        TrollOn = new(num++, MultiMenu.Neutral, "<color=#678D36FF>Troll</color>", parent: NERoles);

        NHRoles = new(MultiMenu.Neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Harbinger</color> <color=#FFD700FF>Roles</color>", new object[] { GameMode.Classic,
            GameMode.AllAny, GameMode.Custom, GameMode.KillingOnly });
        PlaguebearerOn = new(num++, MultiMenu.Neutral, "<color=#CFFE61FF>Plaguebearer</color>", parent: new object[] { NHRoles, AddPlaguebearer });

        NKRoles = new(MultiMenu.Neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killing</color> <color=#FFD700FF>Roles</color>", new object[] { GameMode.Classic,
            GameMode.KillingOnly, GameMode.AllAny, GameMode.Custom });
        ArsonistOn = new(num++, MultiMenu.Neutral, "<color=#EE7600FF>Arsonist</color>", parent: new object[] { NKRoles, AddArsonist });
        CryomaniacOn = new(num++, MultiMenu.Neutral, "<color=#642DEAFF>Cryomaniac</color>", parent: new object[] { NKRoles, AddCryomaniac });
        GlitchOn = new(num++, MultiMenu.Neutral, "<color=#00FF00FF>Glitch</color>", parent: NKRoles);
        JuggernautOn = new(num++, MultiMenu.Neutral, "<color=#A12B56FF>Juggernaut</color>", parent: NKRoles);
        MurdererOn = new(num++, MultiMenu.Neutral, "<color=#6F7BEAFF>Murderer</color>", parent: NKRoles);
        SerialKillerOn = new(num++, MultiMenu.Neutral, "<color=#336EFFFF>Serial Killer</color>", parent: NKRoles);
        WerewolfOn = new(num++, MultiMenu.Neutral, "<color=#9F703AFF>Werewolf</color>", parent: NKRoles);

        NNRoles = new(MultiMenu.Neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Neophyte</color> <color=#FFD700FF>Roles</color>", new object[] { GameMode.Classic,
            GameMode.AllAny, GameMode.Custom });
        DraculaOn = new(num++, MultiMenu.Neutral, "<color=#AC8A00FF>Dracula</color>", parent: NNRoles);
        JackalOn = new(num++, MultiMenu.Neutral, "<color=#45076AFF>Jackal</color>", parent: NNRoles);
        NecromancerOn = new(num++, MultiMenu.Neutral, "<color=#BF5FFFFF>Necromancer</color>", parent: NNRoles);
        WhispererOn = new(num++, MultiMenu.Neutral, "<color=#2D6AA5FF>Whisperer</color>", parent: NNRoles);

        NPRoles = new(MultiMenu.Neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Proselyte</color> <color=#FFD700FF>Roles</color>", new object[] { GameMode.Classic,
            GameMode.AllAny, GameMode.Custom });
        PhantomOn = new(num++, MultiMenu.Neutral, "<color=#662962FF>Phantom</color>", parent: NPRoles);

        NeutralSettings = new(MultiMenu.Neutral, "<color=#B3B3B3FF>Neutral</color> Settings", new object[] { AmnesiacOn, GuardianAngelOn, SurvivorOn, ThiefOn, PlaguebearerOn, ActorOn,
            JackalOn, BountyHunterOn, CannibalOn, ExecutionerOn, GuesserOn, JesterOn, TrollOn, ArsonistOn, CryomaniacOn, GlitchOn, JuggernautOn, MurdererOn, SerialKillerOn, WerewolfOn,
            DraculaOn, WhispererOn, NecromancerOn, LayerEnum.RandomNeutral, LayerEnum.Amnesiac, LayerEnum.GuardianAngel, LayerEnum.Survivor, LayerEnum.Thief, LayerEnum.Plaguebearer,
            LayerEnum.Jackal, LayerEnum.BountyHunter, LayerEnum.Cannibal, LayerEnum.Executioner, LayerEnum.Guesser, LayerEnum.Jester, LayerEnum.Troll, LayerEnum.Arsonist,
            LayerEnum.Cryomaniac, LayerEnum.Glitch, LayerEnum.Juggernaut, LayerEnum.Murderer, LayerEnum.SerialKiller, LayerEnum.Werewolf, LayerEnum.Dracula, LayerEnum.Whisperer,
            LayerEnum.Necromancer, LayerEnum.Necromancer, LayerEnum.NeutralApoc, LayerEnum.NeutralBen, LayerEnum.NeutralEvil, LayerEnum.NeutralHarb, LayerEnum.NeutralKill,
            LayerEnum.NeutralNeo });
        NeutralVision = new(num++, MultiMenu.Neutral, "<color=#B3B3B3FF>Neutral</color> Vision", 1.5f, 0.25f, 5f, 0.25f, MultiplierFormat, NeutralSettings);
        LightsAffectNeutrals = new(num++, MultiMenu.Neutral, "Lights Sabotage Affects <color=#B3B3B3FF>Neutral</color> Vision", true, NeutralSettings);
        NeutralFlashlight = new(num++, MultiMenu.Neutral, "<color=#B3B3B3FF>Neutrals</color> Use A Flashlight", false, NeutralSettings);
        NeutralMax = new(num++, MultiMenu.Neutral, "Max <color=#B3B3B3FF>Neutrals</color>", 5, 1, 14, 1, NeutralSettings);
        NeutralMin = new(num++, MultiMenu.Neutral, "Min <color=#B3B3B3FF>Neutrals</color>", 5, 1, 14, 1, NeutralSettings);
        NoSolo = new(num++, MultiMenu.Neutral, "<color=#B3B3B3FF>Neutrals</color> Together, Strong", new[] { "Never", "Same NKs", "All NKs", "All Neutrals" }, NeutralSettings);
        AvoidNeutralKingmakers = new(num++, MultiMenu.Neutral, "Avoid <color=#B3B3B3FF>Neutral</color> Kingmakers", false, NeutralSettings);
        NeutralsVent = new(num++, MultiMenu.Neutral, "<color=#B3B3B3FF>Neutrals</color> Can Vent", true, NeutralSettings);

        NASettings = new(MultiMenu.Neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Apocalypse</color> Settings", new object[] { PlaguebearerOn, LayerEnum.NeutralApoc,
            LayerEnum.NeutralHarb, LayerEnum.RandomNeutral });

        Pestilence = new(MultiMenu.Neutral, "<color=#424242FF>Pestilence</color>", new object[] { PlaguebearerOn, LayerEnum.Pestilence, LayerEnum.Plaguebearer, LayerEnum.NeutralApoc,
            LayerEnum.NeutralHarb, LayerEnum.RandomNeutral });
        PestSpawn = new(num++, MultiMenu.Neutral, "<color=#424242FF>Pestilence</color> Can Spawn Directly", false, Pestilence);
        PlayersAlerted = new(num++, MultiMenu.Neutral, "<color=#424242FF>Pestilence</color> Transformation Alerts Everyone", true, Pestilence);
        ObliterateCd = new(num++, MultiMenu.Neutral, "Obliterate Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Pestilence);
        PestVent = new(num++, MultiMenu.Neutral, "<color=#424242FF>Pestilence</color> Can Vent", true, Pestilence);

        NBSettings = new(MultiMenu.Neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Benign</color> Settings", new object[] { AmnesiacOn, GuardianAngelOn, SurvivorOn, ThiefOn,
            LayerEnum.RandomNeutral, LayerEnum.Amnesiac, LayerEnum.GuardianAngel, LayerEnum.Survivor, LayerEnum.Thief, LayerEnum.NeutralBen });
        NBMax = new(num++, MultiMenu.Neutral, "Max <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Benigns</color>", 1, 1, 14, 1, NBSettings);
        VigiKillsNB = new(num++, MultiMenu.Neutral, "<color=#FFFF00FF>Vigilante</color> Kills <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Benigns</color>", true, new object[] {
            Vigilante, NBSettings }, true);

        Amnesiac = new(MultiMenu.Neutral, "<color=#22FFFFFF>Amnesiac</color>", new object[] { AmnesiacOn, LayerEnum.Amnesiac, LayerEnum.NeutralBen, LayerEnum.RandomNeutral });
        UniqueAmnesiac = new(num++, MultiMenu.Neutral, "<color=#22FFFFFF>Amnesiac</color> Is Unique", false, new object[] { Amnesiac, EnableUniques }, true);
        RememberArrows = new(num++, MultiMenu.Neutral, "<color=#22FFFFFF>Amnesiac</color> Gets Arrows To Dead Bodies", false, Amnesiac);
        RememberArrowDelay = new(num++, MultiMenu.Neutral, "Arrow Appearance Delay", 5f, 0f, 15f, 1f, CooldownFormat, Amnesiac);
        AmneVent = new(num++, MultiMenu.Neutral, "<color=#22FFFFFF>Amnesiac</color> Can Hide In Vents", false, Amnesiac);
        AmneSwitchVent = new(num++, MultiMenu.Neutral, "<color=#22FFFFFF>Amnesiac</color> Can Switch Vents", false, Amnesiac);

        GuardianAngel = new(MultiMenu.Neutral, "<color=#FFFFFFFF>Guardian Angel</color>", new object[] { GuardianAngelOn, LayerEnum.GuardianAngel, LayerEnum.NeutralBen,
            LayerEnum.RandomNeutral });
        UniqueGuardianAngel = new(num++, MultiMenu.Neutral, "<color=#FFFFFFFF>Guardian Angel</color> Is Unique", false, new object[] { GuardianAngel, EnableUniques }, true);
        GuardianAngelCanPickTargets = new(num++, MultiMenu.Neutral, "<color=#FFFFFFFF>Guardian Angel</color> Can Pick Their Own Target", false, GuardianAngel);
        ProtectCd = new(num++, MultiMenu.Neutral, "Protect Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, GuardianAngel);
        ProtectDur = new(num++, MultiMenu.Neutral, "Protect Duration", 10f, 5f, 30f, 1f, CooldownFormat, GuardianAngel);
        ProtectKCReset = new(num++, MultiMenu.Neutral, "Cooldown Reset When Protected", 2.5f, 0f, 15f, 0.5f, CooldownFormat, GuardianAngel);
        MaxProtects = new(num++, MultiMenu.Neutral, "Max Protects", 5, 1, 15, 1, GuardianAngel);
        ShowProtect = new(num++, MultiMenu.Neutral, "Show Protected Player", new[] { "Self", "Guardian Angel", "Self And GA", "Everyone", "Nobody" }, GuardianAngel);
        GATargetKnows = new(num++, MultiMenu.Neutral, "Target Knows <color=#FFFFFFFF>Guardian Angel</color> Exists", false, GuardianAngel);
        ProtectBeyondTheGrave = new(num++, MultiMenu.Neutral, "<color=#FFFFFFFF>Guardian Angel</color> Can Protect After Death", false, GuardianAngel);
        GAKnowsTargetRole = new(num++, MultiMenu.Neutral, "<color=#FFFFFFFF>Guardian Angel</color> Knows Target's <color=#FFD700FF>Role</color>", false, GuardianAngel);
        GAVent = new(num++, MultiMenu.Neutral, "<color=#FFFFFFFF>Guardian Angel</color> Can Hide In Vents", false, GuardianAngel);
        GASwitchVent = new(num++, MultiMenu.Neutral, "<color=#FFFFFFFF>Guardian Angel</color> Can Switch Vents", false, GuardianAngel);

        Survivor = new(MultiMenu.Neutral, "<color=#DDDD00FF>Survivor</color>", new object[] { SurvivorOn, LayerEnum.Survivor, LayerEnum.NeutralBen, LayerEnum.RandomNeutral,
            LayerEnum.GuardianAngel, GuardianAngelOn });
        UniqueSurvivor = new(num++, MultiMenu.Neutral, "<color=#DDDD00FF>Survivor</color> Is Unique", false, new object[] { Survivor, EnableUniques }, true);
        VestCd = new(num++, MultiMenu.Neutral, "Vest Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Survivor);
        VestDur = new(num++, MultiMenu.Neutral, "Vest Duration", 10f, 5f, 30f, 1f, CooldownFormat, Survivor);
        VestKCReset = new(num++, MultiMenu.Neutral, "Cooldown Reset When Vested", 2.5f, 0f, 15f, 0.5f, CooldownFormat, Survivor);
        MaxVests = new(num++, MultiMenu.Neutral, "Max Vests", 5, 1, 15, 1, Survivor);
        SurvVent = new(num++, MultiMenu.Neutral, "<color=#DDDD00FF>Survivor</color> Can Hide In Vents", false, Survivor);
        SurvSwitchVent = new(num++, MultiMenu.Neutral, "<color=#DDDD00FF>Survivor</color> Can Switch Vents", false, Survivor);

        Thief = new(MultiMenu.Neutral, "<color=#80FF00FF>Thief</color>", new object[] { ThiefOn, LayerEnum.Thief, LayerEnum.NeutralBen, LayerEnum.RandomNeutral, LayerEnum.Amnesiac,
            AmnesiacOn });
        UniqueThief = new(num++, MultiMenu.Neutral, "<color=#80FF00FF>Thief</color> Is Unique", false, new object[] { Thief, EnableUniques }, true);
        StealCd = new(num++, MultiMenu.Neutral, "Steal Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Thief);
        ThiefSteals = new(num++, MultiMenu.Neutral, "<color=#80FF00FF>Thief</color> Assigns <color=#80FF00FF>Thief</color> <color=#FFD700FF>Role</color> To Target", false, Thief);
        ThiefCanGuess = new(num++, MultiMenu.Neutral, "<color=#80FF00FF>Thief</color> Can Guess To Steal Roles", false, Thief);
        ThiefCanGuessAfterVoting = new(num++, MultiMenu.Neutral, "<color=#80FF00FF>Thief</color> Can Guess After Voting", false, Thief);
        ThiefVent = new(num++, MultiMenu.Neutral, "<color=#80FF00FF>Thief</color> Can Vent", false, Thief);

        NESettings = new(MultiMenu.Neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Evil</color> Settings", new object[] { ActorOn, BountyHunterOn, CannibalOn, ExecutionerOn,
            TrollOn, GuesserOn, JesterOn, LayerEnum.RandomNeutral, LayerEnum.Actor, LayerEnum.NeutralEvil, LayerEnum.BountyHunter, LayerEnum.Cannibal, LayerEnum.Executioner,
            LayerEnum.Troll, LayerEnum.Guesser, LayerEnum.Jester });
        NEMax = new(num++, MultiMenu.Neutral, "Max <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Evils</color>", 1, 1, 14, 1, NESettings);
        NeutralEvilsEndGame = new(num++, MultiMenu.Neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Evils</color> End The Game When Winning", false, NESettings);

        Actor = new(MultiMenu.Neutral, "<color=#00ACC2FF>Actor</color>", new object[] { ActorOn, LayerEnum.Actor, LayerEnum.NeutralEvil, LayerEnum.RandomNeutral, LayerEnum.Guesser,
            GuesserOn });
        UniqueActor = new(num++, MultiMenu.Neutral, "<color=#00ACC2FF>Actor</color> Is Unique", false, new object[] { Actor, EnableUniques }, true);
        ActorCanPickRole = new(num++, MultiMenu.Neutral, "<color=#00ACC2FF>Actor</color> Can Choose A Target Role", false, Actor);
        ActorButton = new(num++, MultiMenu.Neutral, "<color=#00ACC2FF>Actor</color> Can Button", true, Actor);
        ActorVent = new(num++, MultiMenu.Neutral, "<color=#00ACC2FF>Actor</color> Can Hide In Vents", false, Actor);
        ActSwitchVent = new(num++, MultiMenu.Neutral, "<color=#00ACC2FF>Actor</color> Can Switch Vents", false, Actor);
        VigiKillsActor = new(num++, MultiMenu.Neutral, "<color=#FFFF00FF>Vigilante</color> Kills <color=#00ACC2FF>Actor</color>", false, new[] { ActorOn, VigilanteOn }, true);

        BountyHunter = new(MultiMenu.Neutral, "<color=#B51E39FF>Bounty Hunter</color>", new object[] { BountyHunterOn, LayerEnum.BountyHunter, LayerEnum.NeutralEvil, LayerEnum.RandomNeutral
            });
        UniqueBountyHunter = new(num++, MultiMenu.Neutral, "<color=#B51E39FF>Bounty Hunter</color> Is Unique", false, new object[] { BountyHunter, EnableUniques }, true);
        BountyHunterCanPickTargets = new(num++, MultiMenu.Neutral, "<color=#B51E39FF>Bounty Hunter</color> Can Indirectly Pick Their Own Target", false, BountyHunter);
        GuessCd = new(num++, MultiMenu.Neutral, "Guess Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, BountyHunter);
        BountyHunterGuesses = new(num++, MultiMenu.Neutral, "Max Guesses", 5, 1, 15, 1, BountyHunter);
        BHVent = new(num++, MultiMenu.Neutral, "<color=#B51E39FF>Bounty Hunter</color> Can Vent", false, BountyHunter);
        VigiKillsBH = new(num++, MultiMenu.Neutral, "<color=#FFFF00FF>Vigilante</color> Kills <color=#B51E39FF>Bounty Hunter</color>", false, new object[] { Vigilante, BountyHunterOn },
            true);

        Cannibal = new(MultiMenu.Neutral, "<color=#8C4005FF>Cannibal</color>", new object[] { CannibalOn, LayerEnum.Cannibal, LayerEnum.NeutralEvil, LayerEnum.RandomNeutral });
        UniqueCannibal = new(num++, MultiMenu.Neutral, "<color=#8C4005FF>Cannibal</color> Is Unique", false, new object[] { Cannibal, EnableUniques }, true);
        EatCd = new(num++, MultiMenu.Neutral, "Eat Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Cannibal);
        BodiesNeeded = new(num++, MultiMenu.Neutral, "Bodies Needed To Win", 1, 1, 5, 1, Cannibal);
        EatArrows = new(num++, MultiMenu.Neutral, "<color=#8C4005FF>Cannibal</color> Gets Arrows To Dead Bodies", false, Cannibal);
        EatArrowDelay = new(num++, MultiMenu.Neutral, "Arrow Appearance Delay", 5f, 0f, 15f, 1f, CooldownFormat, Cannibal);
        CannibalVent = new(num++, MultiMenu.Neutral, "<color=#8C4005FF>Cannibal</color> Can Vent", false, Cannibal);
        VigiKillsCannibal = new(num++, MultiMenu.Neutral, "<color=#FFFF00FF>Vigilante</color> Kills <color=#8C4005FF>Cannibal</color>", false, new object[] { CannibalOn, Vigilante }, true);

        Executioner = new(MultiMenu.Neutral, "<color=#CCCCCCFF>Executioner</color>", new object[] { ExecutionerOn, LayerEnum.Executioner, LayerEnum.NeutralEvil, LayerEnum.RandomNeutral });
        UniqueExecutioner = new(num++, MultiMenu.Neutral, "<color=#CCCCCCFF>Executioner</color> Is Unique", false, new object[] { Executioner, EnableUniques }, true);
        ExecutionerCanPickTargets = new(num++, MultiMenu.Neutral, "<color=#CCCCCCFF>Executioner</color> Can Pick Their Own Target", false, Executioner);
        DoomCd = new(num++, MultiMenu.Neutral, "Doom Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Executioner);
        MaxDooms = new(num++, MultiMenu.Neutral, "Doom Count", 5, 1, 14, 1, Executioner);
        ExecutionerButton = new(num++, MultiMenu.Neutral, "<color=#CCCCCCFF>Executioner</color> Can Button", true, Executioner);
        ExeVent = new(num++, MultiMenu.Neutral, "<color=#CCCCCCFF>Executioner</color> Can Hide In Vents", false, Executioner);
        ExeSwitchVent = new(num++, MultiMenu.Neutral, "<color=#CCCCCCFF>Executioner</color> Can Switch Vents", false, Executioner);
        ExeTargetKnows = new(num++, MultiMenu.Neutral, "Target Knows <color=#CCCCCCFF>Executioner</color> Exists", false, Executioner);
        ExeKnowsTargetRole = new(num++, MultiMenu.Neutral, "<color=#CCCCCCFF>Executioner</color> Knows Target's <color=#FFD700FF>Role</color>", false, Executioner);
        ExeEjectScreen = new(num++, MultiMenu.Neutral, "Target Ejection Reveals Existence Of <color=#CCCCCCFF>Executioner</color>", false, Executioner);
        ExeCanWinBeyondDeath = new(num++, MultiMenu.Neutral, "<color=#CCCCCCFF>Executioner</color> Can Win After Death", false, Executioner);
        VigiKillsExecutioner = new(num++, MultiMenu.Neutral, "<color=#FFFF00FF>Vigilante</color> Kills <color=#CCCCCCFF>Executioner</color>", false, new object[] { ExecutionerOn, Vigilante
            }, true);

        Guesser = new(MultiMenu.Neutral, "<color=#EEE5BEFF>Guesser</color>", new object[] { GuesserOn, LayerEnum.Guesser, LayerEnum.NeutralEvil, LayerEnum.RandomNeutral });
        UniqueGuesser = new(num++, MultiMenu.Neutral, "<color=#EEE5BEFF>Guesser</color> Is Unique", false, new object[] { Guesser, EnableUniques }, true);
        GuesserCanPickTargets = new(num++, MultiMenu.Neutral, "<color=#EEE5BEFF>Guesser</color> Can Pick Their Own Target", false, Guesser);
        GuesserButton = new(num++, MultiMenu.Neutral, "<color=#EEE5BEFF>Guesser</color> Can Button", true, Guesser);
        GuessVent = new(num++, MultiMenu.Neutral, "<color=#EEE5BEFF>Guesser</color> Can Hide In Vents", false, Guesser);
        GuessSwitchVent = new(num++, MultiMenu.Neutral, "<color=#EEE5BEFF>Guesser</color> Can Switch Vents", false, Guesser);
        GuessTargetKnows = new(num++, MultiMenu.Neutral, "Target Knows <color=#EEE5BEFF>Guesser</color> Exists", false, Guesser);
        MultipleGuesses = new(num++, MultiMenu.Neutral, "<color=#EEE5BEFF>Guesser</color> Can Guess Multiple Times", true, Guesser);
        MaxGuesses = new(num++, MultiMenu.Neutral, "Max Guesses", 5, 1, 15, 1, Guesser);
        GuesserAfterVoting = new(num++, MultiMenu.Neutral, "<color=#EEE5BEFF>Guesser</color> Can Guess After Voting", false, Guesser);
        VigiKillsGuesser = new(num++, MultiMenu.Neutral, "<color=#FFFF00FF>Vigilante</color> Kills <color=#EEE5BEFF>Guesser</color>", false, new object[] { GuesserOn, Vigilante }, true);

        Jester = new(MultiMenu.Neutral, "<color=#F7B3DAFF>Jester</color>", new object[] { JesterOn, LayerEnum.Jester, LayerEnum.NeutralEvil, LayerEnum.RandomNeutral, LayerEnum.Executioner,
            ExecutionerOn});
        UniqueJester = new(num++, MultiMenu.Neutral, "<color=#F7B3DAFF>Jester</color> Is Unique", false, new object[] { Jester, EnableUniques }, true);
        JesterButton = new(num++, MultiMenu.Neutral, "<color=#F7B3DAFF>Jester</color> Can Button", true, Jester);
        HauntCd = new(num++, MultiMenu.Neutral, "Haunt Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Jester);
        MaxHaunts = new(num++, MultiMenu.Neutral, "Haunt Count", 5, 1, 14, 1, Jester);
        JesterVent = new(num++, MultiMenu.Neutral, "<color=#F7B3DAFF>Jester</color> Can Hide In Vents", false, Jester);
        JestSwitchVent = new(num++, MultiMenu.Neutral, "<color=#F7B3DAFF>Jester</color> Can Switch Vents", false, Jester);
        JestEjectScreen = new(num++, MultiMenu.Neutral, "<color=#F7B3DAFF>Jester</color> Ejection Reveals Existence Of <color=#F7B3DAFF>Jester</color>", false, Jester);
        VigiKillsJester = new(num++, MultiMenu.Neutral, "<color=#FFFF00FF>Vigilante</color> Kills <color=#F7B3DAFF>Jester</color>", false, new object[] { JesterOn, Vigilante }, true);

        Troll = new(MultiMenu.Neutral, "<color=#678D36FF>Troll</color>", new object[] { TrollOn, LayerEnum.Troll, LayerEnum.NeutralEvil, LayerEnum.RandomNeutral, LayerEnum.BountyHunter,
            BountyHunterOn });
        UniqueTroll = new(num++, MultiMenu.Neutral, "<color=#678D36FF>Troll</color> Is Unique", false, new object[] { Troll, EnableUniques }, true);
        InteractCd = new(num++, MultiMenu.Neutral, "Interact Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Troll);
        TrollVent = new(num++, MultiMenu.Neutral, "<color=#678D36FF>Troll</color> Can Hide In Vent", false, Troll);
        TrollSwitchVent = new(num++, MultiMenu.Neutral, "<color=#678D36FF>Troll</color> Can Switch Vents", false, Troll);

        NHSettings = new(MultiMenu.Neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Harbinger</color> Settings", new object[] { PlaguebearerOn, LayerEnum.Plaguebearer,
            LayerEnum.NeutralHarb, LayerEnum.RandomNeutral });
        NHMax = new(num++, MultiMenu.Neutral, "Max <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Harbingers</color>", 1, 1, 14, 1, NHSettings);

        Plaguebearer = new(MultiMenu.Neutral, "<color=#CFFE61FF>Plaguebearer</color>", new object[] { PlaguebearerOn, LayerEnum.Plaguebearer, LayerEnum.NeutralHarb, LayerEnum.RandomNeutral
            });
        UniquePlaguebearer = new(num++, MultiMenu.Neutral, "<color=#CFFE61FF>Plaguebearer</color> Is Unique", false, new object[] { Plaguebearer, EnableUniques }, true);
        InfectCd = new(num++, MultiMenu.Neutral, "Infect Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Plaguebearer);
        PBVent = new(num++, MultiMenu.Neutral, "<color=#CFFE61FF>Plaguebearer</color> Can Vent", false, Plaguebearer);

        NKSettings = new(MultiMenu.Neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killing</color> Settings", new object[] { ArsonistOn, CryomaniacOn, GlitchOn, JuggernautOn,
            MurdererOn, SerialKillerOn, WerewolfOn, LayerEnum.RandomNeutral, LayerEnum.Arsonist, LayerEnum.NeutralKill, LayerEnum.Cryomaniac, LayerEnum.Glitch, LayerEnum.Juggernaut,
            LayerEnum.Murderer, LayerEnum.SerialKiller, LayerEnum.Werewolf });
        NKMax = new(num++, MultiMenu.Neutral, "Max <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killers</color>", 1, 1, 14, 1, NKSettings);
        NKHasImpVision = new(num++, MultiMenu.Neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killers</color> Have <color=#FF0000FF>Intruder</color> Vision", true, NKSettings);
        NKsKnow = new(num++, MultiMenu.Neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killers</color> Know Each Other", false, NKSettings);

        Arsonist = new(MultiMenu.Neutral, "<color=#EE7600FF>Arsonist</color>", new object[] { ArsonistOn, LayerEnum.Arsonist, LayerEnum.NeutralKill, LayerEnum.RandomNeutral });
        UniqueArsonist = new(num++, MultiMenu.Neutral, "<color=#EE7600FF>Arsonist</color> Is Unique", false, new object[] { Arsonist, EnableUniques }, true);
        ArsoDouseCd = new(num++, MultiMenu.Neutral, "Douse Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Arsonist);
        IgniteCd = new(num++, MultiMenu.Neutral, "Ignite Cooldown", 25f, 5f, 60f, 2.5f, CooldownFormat, Arsonist);
        ArsoLastKillerBoost = new(num++, MultiMenu.Neutral, "Ignite Cooldown Removed When <color=#EE7600FF>Arsonist</color> Is Last Killer", false, Arsonist);
        ArsoIgniteAll = new(num++, MultiMenu.Neutral, "Ignition Ignites All Doused Players", false, Arsonist);
        ArsoCooldownsLinked = new(num++, MultiMenu.Neutral, "Douse And Ignite Cooldowns Are Linked", false, Arsonist);
        IgnitionCremates = new(num++, MultiMenu.Neutral, "Ignition Cremates Bodies", false, Arsonist);
        ArsoVent = new(num++, MultiMenu.Neutral, "<color=#EE7600FF>Arsonist</color> Can Vent", false, Arsonist);

        Cryomaniac = new(MultiMenu.Neutral, "<color=#642DEAFF>Cryomaniac</color>", new object[] { CryomaniacOn, LayerEnum.Cryomaniac, LayerEnum.NeutralKill, LayerEnum.RandomNeutral });
        UniqueCryomaniac = new(num++, MultiMenu.Neutral, "<color=#642DEAFF>Cryomaniac</color> Is Unique", false, new object[] { Cryomaniac, EnableUniques }, true);
        CryoDouseCd = new(num++, MultiMenu.Neutral, "Douse Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Cryomaniac);
        CryoFreezeAll = new(num++, MultiMenu.Neutral, "<color=#642DEAFF>Cryomaniac</color> Freeze Freezes All Doused Players", false, Cryomaniac);
        CryoLastKillerBoost = new(num++, MultiMenu.Neutral, "<color=#642DEAFF>Cryomaniac</color> Can Kill Normally When Last Killer", false, Cryomaniac);
        CryoVent = new(num++, MultiMenu.Neutral, "<color=#642DEAFF>Cryomaniac</color> Can Vent", false, Cryomaniac);

        Glitch = new(MultiMenu.Neutral, "<color=#00FF00FF>Glitch</color>", new object[] { GlitchOn, LayerEnum.Glitch, LayerEnum.NeutralKill, LayerEnum.RandomNeutral });
        UniqueGlitch = new(num++, MultiMenu.Neutral, "<color=#00FF00FF>Glitch</color> Is Unique", false, new object[] { Glitch, EnableUniques }, true);
        MimicCd = new(num++, MultiMenu.Neutral, "Mimic Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Glitch);
        HackCd = new(num++, MultiMenu.Neutral, "Hack Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Glitch);
        MimicDur = new(num++, MultiMenu.Neutral, "Mimic Duration", 10f, 5f, 30f, 1f, CooldownFormat, Glitch);
        HackDur = new(num++, MultiMenu.Neutral, "Hack Duration", 10f, 5f, 30f, 1f, CooldownFormat, Glitch);
        NeutraliseCd = new(num++, MultiMenu.Neutral, "Neutralise Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Glitch);
        GlitchVent = new(num++, MultiMenu.Neutral, "<color=#00FF00FF>Glitch</color> Can Vent", false, Glitch);

        Juggernaut = new(MultiMenu.Neutral, "<color=#A12B56FF>Juggernaut</color>", new object[] { JuggernautOn, LayerEnum.Juggernaut, LayerEnum.NeutralKill, LayerEnum.RandomNeutral });
        UniqueJuggernaut = new(num++, MultiMenu.Neutral, "<color=#A12B56FF>Juggernaut</color> Is Unique", false, new object[] { Juggernaut, EnableUniques }, true);
        AssaultCd = new(num++, MultiMenu.Neutral, "Assault Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Juggernaut);
        AssaultBonus = new(num++, MultiMenu.Neutral, "Assault Cooldown Bonus", 5f, 2.5f, 30f, 2.5f, CooldownFormat, Juggernaut);
        JuggVent = new(num++, MultiMenu.Neutral, "<color=#A12B56FF>Juggernaut</color> Can Vent", false, Juggernaut);

        Murderer = new(MultiMenu.Neutral, "<color=#6F7BEAFF>Murderer</color>", new object[] { MurdererOn, LayerEnum.Murderer, LayerEnum.NeutralKill, LayerEnum.RandomNeutral });
        UniqueMurderer = new(num++, MultiMenu.Neutral, "<color=#6F7BEAFF>Murderer</color> Is Unique", false, new object[] { Murderer, EnableUniques }, true);
        MurderCd = new(num++, MultiMenu.Neutral, "Murder Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Murderer);
        MurdVent = new(num++, MultiMenu.Neutral, "<color=#6F7BEAFF>Murderer</color> Can Vent", false, Murderer);

        SerialKiller = new(MultiMenu.Neutral, "<color=#336EFFFF>Serial Killer</color>", new object[] { SerialKillerOn, LayerEnum.SerialKiller, LayerEnum.NeutralKill,
            LayerEnum.RandomNeutral });
        UniqueSerialKiller = new(num++, MultiMenu.Neutral, "<color=#336EFFFF>Serial Killer</color> Is Unique", false, new object[] { SerialKiller, EnableUniques }, true);
        BloodlustCd = new(num++, MultiMenu.Neutral, "Bloodlust Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, SerialKiller);
        BloodlustDur = new(num++, MultiMenu.Neutral, "Bloodlust Duration", 10f, 5f, 30f, 1f, CooldownFormat, SerialKiller);
        StabCd = new(num++, MultiMenu.Neutral, "Stab Cooldown", 5f, 0.5f, 15f, 0.5f, CooldownFormat, SerialKiller);
        SKVentOptions = new(num++, MultiMenu.Neutral, "<color=#336EFFFF>Serial Killer</color> Can Vent", new[] { "Always", "Bloodlust", "No Lust", "Never" }, SerialKiller);

        Werewolf = new(MultiMenu.Neutral, "<color=#9F703AFF>Werewolf</color>", new object[] { WerewolfOn, LayerEnum.Werewolf, LayerEnum.NeutralKill, LayerEnum.RandomNeutral });
        UniqueWerewolf = new(num++, MultiMenu.Neutral, "<color=#9F703AFF>Werewolf</color> Is Unique", false, new object[] { Werewolf, EnableUniques }, true);
        MaulCd = new(num++, MultiMenu.Neutral, "Maul Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Werewolf);
        MaulRadius = new(num++, MultiMenu.Neutral, "Maul Radius", 1.5f, 0.5f, 5f, 0.25f, DistanceFormat, Werewolf);
        WerewolfVent = new(num++, MultiMenu.Neutral, "<color=#9F703AFF>Werewolf</color> Can Vent", false, Werewolf);

        NNSettings = new(MultiMenu.Neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Neophyte</color> Settings", new object[] { DraculaOn, WhispererOn, JackalOn, NecromancerOn,
            LayerEnum.Dracula, LayerEnum.Whisperer, LayerEnum.Jackal, LayerEnum.Necromancer, LayerEnum.RandomNeutral, LayerEnum.NeutralNeo });
        NNMax = new(num++, MultiMenu.Neutral, "Max <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Neophytes</color>", 1, 1, 14, 1, NNSettings);
        NNHasImpVision = new(num++, MultiMenu.Neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Neophytes</color> Have <color=#FF0000FF>Intruder</color> Vision", true,
            NNSettings);

        Dracula = new(MultiMenu.Neutral, "<color=#AC8A00FF>Dracula</color>", new object[] { DraculaOn, LayerEnum.Dracula, LayerEnum.NeutralNeo, LayerEnum.RandomNeutral });
        UniqueDracula = new(num++, MultiMenu.Neutral, "<color=#AC8A00FF>Dracula</color> Is Unique", false, new object[] { Dracula, EnableUniques }, true);
        BiteCd = new(num++, MultiMenu.Neutral, "Bite Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Dracula);
        DracVent = new(num++, MultiMenu.Neutral, "<color=#AC8A00FF>Dracula</color> Can Vent", false, Dracula);
        AliveVampCount = new(num++, MultiMenu.Neutral, "Alive <color=#7B8968FF>Undead</color> Count", 3, 1, 14, 1, Dracula);
        UndeadVent = new(num++, MultiMenu.Neutral, "Undead Can Vent", false, Dracula);

        Jackal = new(MultiMenu.Neutral, "<color=#45076AFF>Jackal</color>", new object[] { JackalOn, LayerEnum.Jackal, LayerEnum.NeutralNeo, LayerEnum.RandomNeutral });
        UniqueJackal = new(num++, MultiMenu.Neutral, "<color=#45076AFF>Jackal</color> Is Unique", false, new object[] { Jackal, EnableUniques }, true);
        RecruitCd = new(num++, MultiMenu.Neutral, "Recruit Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Jackal);
        JackalVent = new(num++, MultiMenu.Neutral, "<color=#45076AFF>Jackal</color> Can Vent", false, Jackal);
        RecruitVent = new(num++, MultiMenu.Neutral, "Recruits Can Vent", false, Jackal);

        Necromancer = new(MultiMenu.Neutral, "<color=#BF5FFFFF>Necromancer</color>", new object[] { NecromancerOn, LayerEnum.Necromancer, LayerEnum.NeutralNeo, LayerEnum.RandomNeutral });
        UniqueNecromancer = new(num++, MultiMenu.Neutral, "<color=#BF5FFFFF>Necromancer</color> Is Unique", false, new object[] { Necromancer, EnableUniques }, true);
        ResurrectCd = new(num++, MultiMenu.Neutral, "Resurrect Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Necromancer);
        ResurrectCdIncreases = new(num++, MultiMenu.Neutral, "Resurrect Cooldown Increases", true, Necromancer);
        ResurrectCdIncrease = new(num++, MultiMenu.Neutral, "Resurrect Cooldown Increases By", 5f, 2.5f, 30f, 2.5f, CooldownFormat, new object[] { Necromancer, ResurrectCdIncreases },
            true);
        MaxResurrections = new(num++, MultiMenu.Neutral, "Max Resurrections", 5, 1, 14, 1, Necromancer);
        NecroKillCd = new(num++, MultiMenu.Neutral, "Kill Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Necromancer);
        NecroKillCdIncreases = new(num++, MultiMenu.Neutral, "Kill Cooldown Increases", true, Necromancer);
        NecroKillCdIncrease = new(num++, MultiMenu.Neutral, "Kill Cooldown Increases By", 5f, 2.5f, 30f, 2.5f, CooldownFormat, new object[] { Necromancer, NecroKillCdIncreases }, true);
        MaxNecroKills = new(num++, MultiMenu.Neutral, "Kill Count", 5, 1, 14, 1, Necromancer);
        NecroCooldownsLinked = new(num++, MultiMenu.Neutral, "Kill And Resurrect Cooldowns Are Linked", false, Necromancer);
        NecromancerTargetBody = new(num++, MultiMenu.Neutral, "Target's Body Disappears On Beginning Of Resurrect", false, Necromancer);
        ResurrectDur = new(num++, MultiMenu.Neutral, "Resurrect Duration", 10f, 1f, 15f, 1f, CooldownFormat, Necromancer);
        NecroVent = new(num++, MultiMenu.Neutral, "<color=#BF5FFFFF>Necromancer</color> Can Vent", false, Necromancer);
        ResurrectVent = new(num++, MultiMenu.Neutral, "Resurrected Can Vent", false, Necromancer);

        Whisperer = new(MultiMenu.Neutral, "<color=#2D6AA5FF>Whisperer</color>", new object[] { WhispererOn, LayerEnum.Whisperer, LayerEnum.NeutralNeo, LayerEnum.RandomNeutral });
        UniqueWhisperer = new(num++, MultiMenu.Neutral, "<color=#2D6AA5FF>Whisperer</color> Is Unique", false, new object[] { Whisperer, EnableUniques }, true);
        WhisperCd = new(num++, MultiMenu.Neutral, "Whisper Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Whisperer);
        WhisperCdIncreases = new(num++, MultiMenu.Neutral, "Whisper Cooldown Increases", false, Whisperer);
        WhisperCdIncrease = new(num++, MultiMenu.Neutral, "Whisper Cooldown Increases By", 5f, 2.5f, 30f, 2.5f, CooldownFormat, new object[] { Whisperer, WhisperCdIncreases }, true);
        WhisperRadius = new(num++, MultiMenu.Neutral, "Whisper Radius", 1.5f, 0.5f, 5f, 0.25f, DistanceFormat, Whisperer);
        WhisperRate = new(num++, MultiMenu.Neutral, "Whisper Rate", 5, 5, 50, 5, PercentFormat, Whisperer);
        WhisperRateDecreases = new(num++, MultiMenu.Neutral, "Whisper Rate Decreases", false, Whisperer);
        WhisperRateDecrease = new(num++, MultiMenu.Neutral, "Whisper Rate Decrease", 5, 5, 50, 5, PercentFormat, new object[] { Whisperer, WhisperRateDecreases }, true);
        WhispVent = new(num++, MultiMenu.Neutral, "<color=#2D6AA5FF>Whisperer</color> Can Vent", false, Whisperer);
        PersuadedVent = new(num++, MultiMenu.Neutral, "Persuaded Can Vent", false, Whisperer);

        NPSettings = new(MultiMenu.Neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Proselyte</color> Settings", new[] { PhantomOn, TraitorOn, FanaticOn });

        Betrayer = new(MultiMenu.Neutral, "<color=#11806AFF>Betrayer</color>");
        BetrayCd = new(num++, MultiMenu.Neutral, "Kill Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Betrayer);
        BetrayerVent = new(num++, MultiMenu.Neutral, "<color=#11806AFF>Betrayer</color> Can Vent", true, Betrayer);

        Phantom = new(MultiMenu.Neutral, "<color=#662962FF>Phantom</color>", PhantomOn);
        PhantomTasksRemaining = new(num++, MultiMenu.Neutral, "Tasks Remaining When <color=#662962FF>Phantom</color> Can Be Clicked", 5, 1, 10, 1, Phantom);
        PhantomPlayersAlerted = new(num++, MultiMenu.Neutral, "Players Are Alerted When <color=#662962FF>Phantom</color> Is Clickable", false, Phantom);

        IntruderSettings = new(MultiMenu.Intruder, "<color=#FF0000FF>Intruder</color> Settings");
        IntruderCount = new(num++, MultiMenu.Intruder, "<color=#FF0000FF>Intruder</color> Count", 1, 0, 4, 1);
        IntruderVision = new(num++, MultiMenu.Intruder, "<color=#FF0000FF>Intruder</color> Vision", 2f, 0.25f, 5f, 0.25f, MultiplierFormat);
        IntruderFlashlight = new(num++, MultiMenu.Intruder, "<color=#FF0000FF>Intruders</color> Use A Flashlight", false);
        IntKillCd = new(num++, MultiMenu.Intruder, "<color=#FF0000FF>Intruder</color> Kill Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
        IntrudersVent = new(num++, MultiMenu.Intruder, "<color=#FF0000FF>Intruder</color> Can Vent", true);
        IntrudersCanSabotage = new(num++, MultiMenu.Intruder, "<color=#FF0000FF>Intruder</color> Can Sabotage", true);
        GhostsCanSabotage = new(num++, MultiMenu.Intruder, "Dead <color=#FF0000FF>Intruders</color> Can Sabotage", false);
        IntruderMax = new(num++, MultiMenu.Intruder, "Max <color=#FF0000FF>Intruder</color> <color=#FFD700FF>Roles</color>", 5, 1, 14, 1, new object[] { GameMode.Classic, GameMode.AllAny,
            GameMode.Custom });
        IntruderMin = new(num++, MultiMenu.Intruder, "Min <color=#FF0000FF>Intruder</color> <color=#FFD700FF>Roles</color>", 5, 1, 14, 1, new object[] { GameMode.Classic, GameMode.AllAny,
            GameMode.Custom });

        ICRoles = new(MultiMenu.Intruder, "<color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Concealing</color> <color=#FFD700FF>Roles</color>", new object[] { GameMode.Classic,
            GameMode.AllAny, GameMode.KillingOnly, GameMode.Custom });
        BlackmailerOn = new(num++, MultiMenu.Intruder, "<color=#02A752FF>Blackmailer</color>", parent: ICRoles);
        CamouflagerOn = new(num++, MultiMenu.Intruder, "<color=#378AC0FF>Camouflager</color>", parent: ICRoles);
        GrenadierOn = new(num++, MultiMenu.Intruder, "<color=#85AA5BFF>Grenadier</color>", parent: ICRoles);
        JanitorOn = new(num++, MultiMenu.Intruder, "<color=#2647A2FF>Janitor</color>", parent: ICRoles);

        IDRoles = new(MultiMenu.Intruder, "<color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Deception</color> <color=#FFD700FF>Roles</color>", new object[] { GameMode.Classic,
            GameMode.AllAny, GameMode.KillingOnly, GameMode.Custom });
        DisguiserOn = new(num++, MultiMenu.Intruder, "<color=#40B4FFFF>Disguiser</color>", parent: IDRoles);
        MorphlingOn = new(num++, MultiMenu.Intruder, "<color=#BB45B0FF>Morphling</color>", parent: IDRoles);
        WraithOn = new(num++, MultiMenu.Intruder, "<color=#5C4F75FF>Wraith</color>", parent: IDRoles);

        IKRoles = new(MultiMenu.Intruder, "<color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Killing</color> <color=#FFD700FF>Roles</color>", new object[] { GameMode.Classic,
            GameMode.AllAny, GameMode.KillingOnly, GameMode.Custom });
        AmbusherOn = new(num++, MultiMenu.Intruder, "<color=#2BD29CFF>Ambusher</color>", parent: IKRoles);
        EnforcerOn = new(num++, MultiMenu.Intruder, "<color=#005643FF>Enforcer</color>", parent: IKRoles);

        ISRoles = new(MultiMenu.Intruder, "<color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Support</color> <color=#FFD700FF>Roles</color>", new object[] { GameMode.Classic,
            GameMode.AllAny, GameMode.KillingOnly, GameMode.Custom });
        ConsigliereOn = new(num++, MultiMenu.Intruder, "<color=#FFFF99FF>Consigliere</color>", parent: ISRoles);
        ConsortOn = new(num++, MultiMenu.Intruder, "<color=#801780FF>Consort</color>", parent: ISRoles);
        GodfatherOn = new(num++, MultiMenu.Intruder, "<color=#404C08FF>Godfather</color>", parent: ISRoles);
        MinerOn = new(num++, MultiMenu.Intruder, "<color=#AA7632FF>Miner</color>", parent: ISRoles);
        TeleporterOn = new(num++, MultiMenu.Intruder, "<color=#939593FF>Teleporter</color>", parent: ISRoles);

        IURoles = new(MultiMenu.Intruder, "<color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Utility</color> <color=#FFD700FF>Roles</color>", new object[] { GameMode.Classic,
            GameMode.AllAny, GameMode.KillingOnly, GameMode.Custom });
        GhoulOn = new(num++, MultiMenu.Intruder, "<color=#F1C40FFF>Ghoul</color>", parent: IURoles);
        ImpostorOn = new(num++, MultiMenu.Intruder, "<color=#FF0000FF>Impostor</color>", parent: IURoles);

        ICSettings = new(MultiMenu.Intruder, "<color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Concealing</color> Settings", new object[] { BlackmailerOn, CamouflagerOn, GrenadierOn,
            JanitorOn, LayerEnum.Blackmailer, LayerEnum.Camouflager, LayerEnum.Grenadier, LayerEnum.Janitor, LayerEnum.IntruderConceal, LayerEnum.RandomIntruder });
        ICMax = new(num++, MultiMenu.Intruder, "Max <color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Concealers</color>", 1, 1, 14, 1, ICSettings);

        Blackmailer = new(MultiMenu.Intruder, "<color=#02A752FF>Blackmailer</color>", new object[] { BlackmailerOn, LayerEnum.Blackmailer, LayerEnum.IntruderConceal,
            LayerEnum.RandomIntruder });
        UniqueBlackmailer = new(num++, MultiMenu.Intruder, "<color=#02A752FF>Blackmailer</color> Is Unique", false, new object[] { Blackmailer, EnableUniques }, true);
        BlackmailCd = new(num++, MultiMenu.Intruder, "Blackmail Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Blackmailer);
        WhispersNotPrivate = new(num++, MultiMenu.Intruder, "<color=#02A752FF>Blackmailer</color> Can Read Whispers", true, Blackmailer);
        BlackmailMates = new(num++, MultiMenu.Intruder, "<color=#02A752FF>Blackmailer</color> Can Blackmail Teammates", false, Blackmailer);
        BMRevealed = new(num++, MultiMenu.Intruder, "Blackmail Is Revealed To Everyone", true, Blackmailer);

        Camouflager = new(MultiMenu.Intruder, "<color=#378AC0FF>Camouflager</color>", new object[] { CamouflagerOn, LayerEnum.Camouflager, LayerEnum.IntruderConceal,
            LayerEnum.RandomIntruder });
        UniqueCamouflager = new(num++, MultiMenu.Intruder, "<color=#378AC0FF>Camouflager</color> Is Unique", false, new object[] { Camouflager, EnableUniques }, true);
        CamouflageCd = new(num++, MultiMenu.Intruder, "Camouflage Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Camouflager);
        CamouflageDur = new(num++, MultiMenu.Intruder, "Camouflage Duration", 10f, 5f, 30f, 1f, CooldownFormat, Camouflager);
        CamoHideSize = new(num++, MultiMenu.Intruder, "Camouflage Hides Player Size", false, Camouflager);
        CamoHideSpeed = new(num++, MultiMenu.Intruder, "Camouflage Hides Player Speed", false, Camouflager);

        CamouflagedMeetings.Parents = new object[] { CamouflagedComms, Camouflager };

        Grenadier = new(MultiMenu.Intruder, "<color=#85AA5BFF>Grenadier</color>", new object[] { GrenadierOn, LayerEnum.Grenadier, LayerEnum.IntruderConceal, LayerEnum.RandomIntruder });
        UniqueGrenadier = new(num++, MultiMenu.Intruder, "<color=#85AA5BFF>Grenadier</color> Is Unique", false, new object[] { Grenadier, EnableUniques }, true);
        FlashCd = new(num++, MultiMenu.Intruder, "Flash Grenade Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Grenadier);
        FlashDur = new(num++, MultiMenu.Intruder, "Flash Grenade Duration", 10f, 5f, 30f, 1f, CooldownFormat, Grenadier);
        FlashRadius = new(num++, MultiMenu.Intruder, "Flash Radius", 1.5f, 0.5f, 5f, 0.25f, DistanceFormat, Grenadier);
        GrenadierIndicators = new(num++, MultiMenu.Intruder, "Indicate Flashed Players", false, Grenadier);
        GrenadierVent = new(num++, MultiMenu.Intruder, "<color=#85AA5BFF>Grenadier</color> Can Vent", false, Grenadier);

        Janitor = new(MultiMenu.Intruder, "<color=#2647A2FF>Janitor</color>", new object[] { JanitorOn, LayerEnum.Janitor, LayerEnum.IntruderConceal, LayerEnum.RandomIntruder });
        UniqueJanitor = new(num++, MultiMenu.Intruder, "<color=#2647A2FF>Janitor</color> Is Unique", false, new object[] { Janitor, EnableUniques }, true);
        CleanCd = new(num++, MultiMenu.Intruder, "Clean Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Janitor);
        JaniCooldownsLinked = new(num++, MultiMenu.Intruder, "Kill And Clean Cooldowns Are Linked", false, Janitor);
        SoloBoost = new(num++, MultiMenu.Intruder, "<color=#2647A2FF>Janitor</color> Has Lower Cooldown When Solo", false, Janitor);
        DragCd = new(num++, MultiMenu.Intruder, "Drag Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Janitor);
        DragModifier = new(num++, MultiMenu.Intruder, "Drag Speed", 0.5f, 0.25f, 3f, 0.25f, MultiplierFormat, Janitor);
        JanitorVentOptions = new(num++, MultiMenu.Intruder, "<color=#2647A2FF>Janitor</color> Can Vent", new[] { "Never", "Body", "Bodyless", "Always" }, Janitor);

        IDSettings = new(MultiMenu.Intruder, "<color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Deception</color> Settings", new object[] { DisguiserOn, MorphlingOn, WraithOn,
            LayerEnum.Disguiser, LayerEnum.Morphling, LayerEnum.Wraith, LayerEnum.IntruderDecep, LayerEnum.RandomIntruder });
        IDMax = new(num++, MultiMenu.Intruder, "Max <color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Deceivers</color>", 1, 1, 14, 1, IDSettings);

        Disguiser = new(MultiMenu.Intruder, "<color=#40B4FFFF>Disguiser</color>", new object[] { DisguiserOn, LayerEnum.Disguiser, LayerEnum.IntruderDecep, LayerEnum.RandomIntruder });
        UniqueDisguiser = new(num++, MultiMenu.Intruder, "<color=#40B4FFFF>Disguiser</color> Is Unique", false, new object[] { Disguiser, EnableUniques }, true);
        DisguiseCd = new(num++, MultiMenu.Intruder, "Disguise Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Disguiser);
        DisguiseDelay = new(num++, MultiMenu.Intruder, "Delay Before Disguising", 5f, 2.5f, 15f, 2.5f, CooldownFormat, Disguiser);
        DisguiseDur = new(num++, MultiMenu.Intruder, "Disguise Duration", 10f, 2.5f, 20f, 2.5f, CooldownFormat, Disguiser);
        MeasureCd = new(num++, MultiMenu.Intruder, "Measure Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Disguiser);
        DisgCooldownsLinked = new(num++, MultiMenu.Intruder, "Measure And Disguise Cooldowns Are Linked", false, Disguiser);
        DisguiseTarget = new(num++, MultiMenu.Intruder, "<color=#40B4FFFF>Disguiser</color> Can Disguise", new[] { "Everyone", "Only Intruders", "Non Intruders" }, Disguiser);

        Morphling = new(MultiMenu.Intruder, "<color=#BB45B0FF>Morphling</color>", new object[] { MorphlingOn, LayerEnum.Morphling, LayerEnum.IntruderDecep, LayerEnum.RandomIntruder });
        UniqueMorphling = new(num++, MultiMenu.Intruder, "<color=#BB45B0FF>Morphling</color> Is Unique", false, new object[] { Morphling, EnableUniques }, true);
        MorphCd = new(num++, MultiMenu.Intruder, "Morph Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Morphling);
        MorphDur = new(num++, MultiMenu.Intruder, "Morph Duration", 10f, 5f, 30f, 1f, CooldownFormat, Morphling);
        SampleCd = new(num++, MultiMenu.Intruder, "Sample Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Morphling);
        MorphCooldownsLinked = new(num++, MultiMenu.Intruder, "Sample And Morph Cooldowns Are Linked", false, Morphling);
        MorphlingVent = new(num++, MultiMenu.Intruder, "<color=#BB45B0FF>Morphling</color> Can Vent", false, Morphling);

        Wraith = new(MultiMenu.Intruder, "<color=#5C4F75FF>Wraith</color>", new object[] { WraithOn, LayerEnum.Wraith, LayerEnum.IntruderDecep, LayerEnum.RandomIntruder });
        UniqueWraith = new(num++, MultiMenu.Intruder, "<color=#5C4F75FF>Wraith</color> Is Unique", false, new object[] { Wraith, EnableUniques }, true);
        InvisCd = new(num++, MultiMenu.Intruder, "Invis Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Wraith);
        InvisDur = new(num++, MultiMenu.Intruder, "Invis Duration", 10f, 5f, 30f, 1f, CooldownFormat, Wraith);
        WraithVent = new(num++, MultiMenu.Intruder, "<color=#5C4F75FF>Wraith</color> Can Vent", false, Wraith);

        IKSettings = new(MultiMenu.Intruder, "<color=#FF0000FF>Impostor</color> <color=#1D7CF2FF>Killing</color> Settings", new object[] { AmbusherOn, EnforcerOn, LayerEnum.Ambusher,
            LayerEnum.Enforcer, LayerEnum.RandomIntruder, LayerEnum.IntruderKill });
        IKMax = new(num++, MultiMenu.Intruder, "Max <color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Killers</color>", 1, 1, 14, 1, IKSettings);

        Ambusher = new(MultiMenu.Intruder, "<color=#2BD29CFF>Ambusher</color>", new object[] { AmbusherOn, LayerEnum.Ambusher, LayerEnum.IntruderKill, LayerEnum.RandomIntruder });
        UniqueAmbusher = new(num++, MultiMenu.Intruder, "<color=#2BD29CFF>Ambusher</color> Is Unique", false, new object[] { Ambusher, EnableUniques }, true);
        AmbushCd = new(num++, MultiMenu.Intruder, "Ambush Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Ambusher);
        AmbushDur = new(num++, MultiMenu.Intruder, "Ambush Duration", 10f, 5f, 30f, 1f, CooldownFormat, Ambusher);
        AmbushMates = new(num++, MultiMenu.Intruder, "<color=#2BD29CFF>Ambusher</color> Can Ambush Teammates", false, Ambusher);

        Enforcer = new(MultiMenu.Intruder, "<color=#005643FF>Enforcer</color>", new object[] { EnforcerOn, LayerEnum.Enforcer, LayerEnum.IntruderKill, LayerEnum.RandomIntruder });
        UniqueEnforcer = new(num++, MultiMenu.Intruder, "<color=#005643FF>Enforcer</color> Is Unique", false, new object[] { Enforcer, EnableUniques }, true);
        EnforceCd = new(num++, MultiMenu.Intruder, "Enforce Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Enforcer);
        EnforceDur = new(num++, MultiMenu.Intruder, "Enforce Duration", 10f, 5f, 30f, 1f, CooldownFormat, Enforcer);
        EnforceDelay = new(num++, MultiMenu.Intruder, "Enforce Delay", 5f, 1f, 15f, 1f, CooldownFormat, Enforcer);
        EnforceRadius = new(num++, MultiMenu.Intruder, "Enforce Explosion Radius", 1.5f, 0.5f, 5f, 0.25f, DistanceFormat, Enforcer);

        ISSettings = new(MultiMenu.Intruder, "<color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Support</color> Settings", new object[] { ConsigliereOn, ConsortOn, GodfatherOn, MinerOn,
            TeleporterOn, LayerEnum.Consigliere, LayerEnum.Consort, LayerEnum.Godfather, LayerEnum.Miner, LayerEnum.Teleporter });
        ISMax = new(num++, MultiMenu.Intruder, "Max <color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Supporters</color>", 1, 1, 14, 1, ISSettings);

        Consigliere = new(MultiMenu.Intruder, "<color=#FFFF99FF>Consigliere</color>", new object[] { ConsigliereOn, LayerEnum.Consigliere, LayerEnum.IntruderSupport,
            LayerEnum.RandomIntruder });
        UniqueConsigliere = new(num++, MultiMenu.Intruder, "<color=#FFFF99FF>Consigliere</color> Is Unique", false, new object[] { Consigliere, EnableUniques }, true);
        InvestigateCd = new(num++, MultiMenu.Intruder, "Investigate Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Consigliere);
        ConsigInfo = new(num++, MultiMenu.Intruder, "Info That <color=#FFFF99FF>Consigliere</color> Sees", new[] { "Role", "Faction" }, Consigliere);

        Consort = new(MultiMenu.Intruder, "<color=#801780FF>Consort</color>", new object[] { ConsortOn, LayerEnum.Consort, LayerEnum.IntruderSupport, LayerEnum.RandomIntruder });
        UniqueConsort = new(num++, MultiMenu.Intruder, "<color=#801780FF>Consort</color> Is Unique", false, new object[] { Consort, EnableUniques }, true);
        ConsortCd = new(num++, MultiMenu.Intruder, "Roleblock Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Consort);
        ConsortDur = new(num++, MultiMenu.Intruder, "Roleblock Duration", 10f, 5f, 30f, 1f, CooldownFormat, Consort);

        Godfather = new(MultiMenu.Intruder, "<color=#404C08FF>Godfather</color>", new object[] { GodfatherOn, LayerEnum.Godfather, LayerEnum.IntruderSupport, LayerEnum.RandomIntruder });
        UniqueGodfather = new(num++, MultiMenu.Intruder, "<color=#404C08FF>Godfather</color> Is Unique", false, new object[] { Godfather, EnableUniques }, true);
        GFPromotionCdDecrease = new(num++, MultiMenu.Intruder, "Ability Cooldown Bonus", 0.75f, 0.25f, 0.9f, 0.05f, MultiplierFormat, Godfather);

        Miner = new(MultiMenu.Intruder, "<color=#AA7632FF>Miner</color>", new object[] { MinerOn, LayerEnum.Miner, LayerEnum.IntruderSupport });
        UniqueMiner = new(num++, MultiMenu.Intruder, "<color=#AA7632FF>Miner</color> Is Unique", false, new object[] { Miner, EnableUniques }, true);
        MineCd = new(num++, MultiMenu.Intruder, "Mine Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Miner);

        Teleporter = new(MultiMenu.Intruder, "<color=#939593FF>Teleporter</color>", new object[] { TeleporterOn, LayerEnum.Teleporter, LayerEnum.IntruderSupport, LayerEnum.RandomIntruder
            });
        UniqueTeleporter = new(num++, MultiMenu.Intruder, "<color=#939593FF>Teleporter</color> Is Unique", false, new object[] { Teleporter, EnableUniques }, true);
        TeleportCd = new(num++, MultiMenu.Intruder, "Teleport Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Teleporter);
        TeleMarkCd = new(num++, MultiMenu.Intruder, "Mark Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Teleporter);
        TeleCooldownsLinked = new(num++, MultiMenu.Intruder, "Mark And Teleport Cooldowns Are Linked", false, Teleporter);
        TeleVent = new(num++, MultiMenu.Intruder, "<color=#939593FF>Teleporter</color> Can Vent", false, Teleporter);

        IUSettings = new(MultiMenu.Intruder, "<color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Utility</color> Settings", GhoulOn);

        Ghoul = new(MultiMenu.Intruder, "<color=#F1C40FFF>Ghoul</color>", GhoulOn);
        GhoulMarkCd = new(num++, MultiMenu.Intruder, "Mark Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Ghoul);

        SyndicateSettings = new(MultiMenu.Syndicate, "<color=#008000FF>Syndicate</color> Settings");
        SyndicateCount = new(num++, MultiMenu.Syndicate, "<color=#008000FF>Syndicate</color> Count", 1, 0, 4, 1);
        SyndicateVision = new(num++, MultiMenu.Syndicate, "<color=#008000FF>Syndicate</color> Vision", 2f, 0.25f, 5f, 0.25f, MultiplierFormat);
        SyndicateFlashlight = new(num++, MultiMenu.Syndicate, "<color=#008000FF>Syndicate</color> Uses A Flashlight", false);
        ChaosDriveMeetingCount = new(num++, MultiMenu.Syndicate, "Chaos Drive Timer", 3, 1, 10, 1);
        CDKillCd = new(num++, MultiMenu.Syndicate, "Chaos Drive Holder Kill Cooldown", 15f, 10f, 45f, 2.5f, CooldownFormat);
        SyndicateVent = new(num++, MultiMenu.Syndicate, "<color=#008000FF>Syndicate</color> Can Vent", new[] { "Always", "Chaos Drive", "Never" });
        AltImps = new(num++, MultiMenu.Syndicate, "<color=#008000FF>Syndicate</color> Replaces <color=#FF0000FF>Intruders</color>", false);
        GlobalDrive = new(num++, MultiMenu.Syndicate, "Chaos Drive Is Global", false);
        SyndicateMax = new(num++, MultiMenu.Syndicate, "Max <color=#008000FF>Syndicate</color> <color=#FFD700FF>Roles</color>", 5, 1, 14, 1, new object[] { GameMode.Classic,
            GameMode.AllAny, GameMode.Custom });
        SyndicateMin = new(num++, MultiMenu.Syndicate, "Min <color=#008000FF>Syndicate</color> <color=#FFD700FF>Roles</color>", 5, 1, 14, 1, new object[] { GameMode.Classic,
            GameMode.AllAny, GameMode.Custom });

        SDRoles = new(MultiMenu.Syndicate, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Disruption</color> <color=#FFD700FF>Roles</color>", new object[] { GameMode.Classic,
            GameMode.AllAny, GameMode.KillingOnly, GameMode.Custom });
        ConcealerOn = new(num++, MultiMenu.Syndicate, "<color=#C02525FF>Concealer</color>", parent: SDRoles);
        DrunkardOn = new(num++, MultiMenu.Syndicate, "<color=#FF7900FF>Drunkard</color>", parent: SDRoles);
        FramerOn = new(num++, MultiMenu.Syndicate, "<color=#00FFFFFF>Framer</color>", parent: SDRoles);
        ShapeshifterOn = new(num++, MultiMenu.Syndicate, "<color=#2DFF00FF>Shapeshifter</color>", parent: SDRoles);
        SilencerOn = new(num++, MultiMenu.Syndicate, "<color=#AAB43EFF>Silencer</color>", parent: SDRoles);

        SyKRoles = new(MultiMenu.Syndicate, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Killing</color> <color=#FFD700FF>Roles</color>", new object[] { GameMode.Classic,
            GameMode.AllAny, GameMode.KillingOnly, GameMode.Custom });
        BomberOn = new(num++, MultiMenu.Syndicate, "<color=#C9CC3FFF>Bomber</color>", parent: SyKRoles);
        ColliderOn = new(num++, MultiMenu.Syndicate, "<color=#B345FFFF>Collider</color>", parent: SyKRoles);
        CrusaderOn = new(num++, MultiMenu.Syndicate, "<color=#DF7AE8FF>Crusader</color>", parent: SyKRoles);
        PoisonerOn = new(num++, MultiMenu.Syndicate, "<color=#B5004CFF>Poisoner</color>", parent: SyKRoles);

        SPRoles = new(MultiMenu.Syndicate, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Power</color> <color=#FFD700FF>Roles</color>", new object[] { GameMode.Classic,
            GameMode.AllAny, GameMode.KillingOnly, GameMode.Custom });
        SpellslingerOn = new(num++, MultiMenu.Syndicate, "<color=#0028F5FF>Spellslinger</color>", parent: SPRoles);
        TimeKeeperOn = new(num++, MultiMenu.Syndicate, "<color=#3769FEFF>Time Keeper</color>", parent: SPRoles);

        SSuRoles = new(MultiMenu.Syndicate, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Support</color> <color=#FFD700FF>Roles</color>", new object[] { GameMode.Classic,
            GameMode.AllAny, GameMode.KillingOnly, GameMode.Custom });
        RebelOn = new(num++, MultiMenu.Syndicate, "<color=#FFFCCEFF>Rebel</color>", parent: SSuRoles);
        StalkerOn = new(num++, MultiMenu.Syndicate, "<color=#7E4D00FF>Stalker</color>", parent: SSuRoles);
        WarperOn = new(num++, MultiMenu.Syndicate, "<color=#8C7140FF>Warper</color>", parent: SSuRoles);

        SURoles = new(MultiMenu.Syndicate, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Utility</color> <color=#FFD700FF>Roles</color>", new object[] { GameMode.Classic,
            GameMode.AllAny, GameMode.KillingOnly, GameMode.Custom });
        AnarchistOn = new(num++, MultiMenu.Syndicate, "<color=#008000FF>Anarchist</color>", parent: SURoles);
        BansheeOn = new(num++, MultiMenu.Syndicate, "<color=#E67E22FF>Banshee</color>", parent: SURoles);

        SDSettings = new(MultiMenu.Syndicate, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Disruption</color> Settings", new object[] { ConcealerOn, DrunkardOn, FramerOn,
            ShapeshifterOn, SilencerOn, LayerEnum.RandomSyndicate, LayerEnum.Concealer, LayerEnum.Drunkard, LayerEnum.Framer, LayerEnum.Shapeshifter, LayerEnum.SyndicateDisrup });
        SDMax = new(num++, MultiMenu.Syndicate, "Max <color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Disruptors</color>", 1, 1, 14, 1, SDSettings);

        Concealer = new(MultiMenu.Syndicate, "<color=#C02525FF>Concealer</color>", new object[] { ConcealerOn, LayerEnum.Concealer, LayerEnum.SyndicateDisrup, LayerEnum.RandomSyndicate });
        UniqueConcealer = new(num++, MultiMenu.Syndicate, "<color=#C02525FF>Concealer</color> Is Unique", false, new object[] { Concealer, EnableUniques }, true);
        ConcealCd = new(num++, MultiMenu.Syndicate, "Conceal Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Concealer);
        ConcealDur = new(num++, MultiMenu.Syndicate, "Conceal Duration", 10f, 5f, 30f, 1f, CooldownFormat, Concealer);
        ConcealMates = new(num++, MultiMenu.Syndicate, "<color=#C02525FF>Concealer</color> Can Conceal Teammates", false, Concealer);

        Drunkard = new(MultiMenu.Syndicate, "<color=#FF7900FF>Drunkard</color>", new object[] { DrunkardOn, LayerEnum.Drunkard, LayerEnum.SyndicateDisrup, LayerEnum.RandomSyndicate });
        UniqueDrunkard = new(num++, MultiMenu.Syndicate, "<color=#FF7900FF>Drunkard</color> Is Unique", false, new object[] { Drunkard, EnableUniques }, true);
        ConfuseCd = new(num++, MultiMenu.Syndicate, "Confuse Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Drunkard);
        ConfuseDur = new(num++, MultiMenu.Syndicate, "Confuse Duration", 10f, 5f, 30f, 1f, CooldownFormat, Drunkard);
        ConfuseImmunity = new(num++, MultiMenu.Syndicate, "<color=#008000FF>Syndicate</color> Are Immune To Confuse", true, Drunkard);

        Framer = new(MultiMenu.Syndicate, "<color=#00FFFFFF>Framer</color>", new object[] { FramerOn, LayerEnum.Framer, LayerEnum.SyndicateDisrup, LayerEnum.RandomSyndicate });
        UniqueFramer = new(num++, MultiMenu.Syndicate, "<color=#00FFFFFF>Framer</color> Is Unique", false, new object[] { Framer, EnableUniques }, true);
        FrameCd = new(num++, MultiMenu.Syndicate, "Frame Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Framer);
        ChaosDriveFrameRadius = new(num++, MultiMenu.Syndicate, "Chaos Drive Frame Radius", 1.5f, 0.5f, 5f, 0.25f, DistanceFormat, Framer);

        Shapeshifter = new(MultiMenu.Syndicate, "<color=#2DFF00FF>Shapeshifter</color>", new object[] { ShapeshifterOn, LayerEnum.Shapeshifter, LayerEnum.SyndicateDisrup,
            LayerEnum.RandomSyndicate });
        UniqueShapeshifter = new(num++, MultiMenu.Syndicate, "<color=#2DFF00FF>Shapeshifter</color> Is Unique", false, new object[] { Shapeshifter, EnableUniques }, true);
        ShapeshiftCd = new(num++, MultiMenu.Syndicate, "Shapeshift Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Shapeshifter);
        ShapeshiftDur = new(num++, MultiMenu.Syndicate, "Shapeshift Duration", 10f, 5f, 30f, 1f, CooldownFormat, Shapeshifter);
        ShapeshiftMates = new(num++, MultiMenu.Syndicate, "<color=#2DFF00FF>Shapeshifter</color> Can Shapeshift Teammates", false, Shapeshifter);

        Silencer = new(MultiMenu.Syndicate, "<color=#AAB43EFF>Silencer</color>", new object[] { SilencerOn, LayerEnum.Silencer, LayerEnum.SyndicateDisrup, LayerEnum.RandomSyndicate });
        UniqueSilencer = new(num++, MultiMenu.Syndicate, "<color=#AAB43EFF>Silencer</color> Is Unique", false, new object[] { Silencer, EnableUniques }, true);
        SilenceCd = new(num++, MultiMenu.Syndicate, "Silence Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Silencer);
        WhispersNotPrivateSilencer = new(num++, MultiMenu.Syndicate, "<color=#AAB43EFF>Silencer</color> Can Read Whispers", true, Silencer);
        SilenceMates = new(num++, MultiMenu.Syndicate, "<color=#AAB43EFF>Silencer</color> Can Silence Teammates", false, Silencer);
        SilenceRevealed = new(num++, MultiMenu.Syndicate, "Silence Is Revealed To Everyone", true, Silencer);

        SyKSettings = new(MultiMenu.Syndicate, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Killing</color> Settings", new object[] { BomberOn, ColliderOn, CrusaderOn, PoisonerOn,
            LayerEnum.RandomSyndicate, LayerEnum.Bomber, LayerEnum.Collider, LayerEnum.Crusader, LayerEnum.Poisoner, LayerEnum.SyndicateKill });
        SyKMax = new(num++, MultiMenu.Syndicate, "Max <color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Killers</color>", 1, 1, 14, 1, SyKSettings);

        Bomber = new(MultiMenu.Syndicate, "<color=#C9CC3FFF>Bomber</color>", new object[] { BomberOn, LayerEnum.Bomber, LayerEnum.SyndicateKill, LayerEnum.RandomSyndicate });
        UniqueBomber = new(num++, MultiMenu.Syndicate, "<color=#C9CC3FFF>Bomber</color> Is Unique", false, new object[] { Bomber, EnableUniques }, true);
        BombCd = new(num++, MultiMenu.Syndicate, "Bomb Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Bomber);
        DetonateCd = new(num++, MultiMenu.Syndicate, "Detonation Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Bomber);
        BombCooldownsLinked = new(num++, MultiMenu.Syndicate, "Place And Detonate Cooldowns Are Linked", false, Bomber);
        BombsRemoveOnNewRound = new(num++, MultiMenu.Syndicate, "Bombs Are Cleared Every Meeting", false, Bomber);
        BombsDetonateOnMeetingStart = new(num++, MultiMenu.Syndicate, "Bombs Detonate Everytime A Meeting Is Called", false, Bomber);
        BombRange = new(num++, MultiMenu.Syndicate, "Bomb Radius", 1.5f, 0.5f, 5f, 0.25f, DistanceFormat, Bomber);
        ChaosDriveBombRange = new(num++, MultiMenu.Syndicate, "Chaos Drive Bomb Radius Increase", 0.5f, 0.5f, 5f, 0.25f, DistanceFormat, Bomber);
        BombKillsSyndicate = new(num++, MultiMenu.Syndicate, "Bomb Detonation Kills Members Of The <color=#008000FF>Syndicate</color>", true, Bomber);

        Collider = new(MultiMenu.Syndicate, "<color=#B345FFFF>Collider</color>", new object[] { ColliderOn, LayerEnum.Collider, LayerEnum.SyndicateKill, LayerEnum.RandomSyndicate });
        UniqueCollider = new(num++, MultiMenu.Syndicate, "<color=#B345FFFF>Collider</color> Is Unique", false, new object[] { Collider, EnableUniques }, true);
        CollideCd = new(num++, MultiMenu.Syndicate, "Set Charges Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Collider);
        ChargeCd = new(num++, MultiMenu.Syndicate, "Charge Cooldown With Chose Drive", 25f, 10f, 60f, 2.5f, CooldownFormat, Collider);
        ChargeDur = new(num++, MultiMenu.Syndicate, "Charge Duration", 10f, 5f, 30f, 1f, CooldownFormat, Collider);
        CollideRange = new(num++, MultiMenu.Syndicate, "Collide Radius", 1.5f, 0.5f, 5f, 0.25f, DistanceFormat, Collider);
        CollideRangeIncrease = new(num++, MultiMenu.Syndicate, "Chaos Drive Collide Radius Increase", 0.5f, 0.5f, 5f, 0.25f, DistanceFormat, Collider);
        ChargeCooldownsLinked = new(num++, MultiMenu.Syndicate, "Charge Cooldowns Are Linked", false, Collider);
        CollideResetsCooldown = new(num++, MultiMenu.Syndicate, "Collision Resets Charge Cooldowns", false, Collider);

        Crusader = new(MultiMenu.Syndicate, "<color=#DF7AE8FF>Crusader</color>", new object[] { CrusaderOn, LayerEnum.Crusader, LayerEnum.SyndicateKill, LayerEnum.RandomSyndicate });
        UniqueCrusader = new(num++, MultiMenu.Syndicate, "<color=#DF7AE8FF>Crusader</color> Is Unique", false, new object[] { Crusader, EnableUniques }, true);
        CrusadeCd = new(num++, MultiMenu.Syndicate, "Crusade Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Crusader);
        CrusadeDur = new(num++, MultiMenu.Syndicate, "Crusade Duration", 10f, 5f, 30f, 1f, CooldownFormat, Crusader);
        ChaosDriveCrusadeRadius = new(num++, MultiMenu.Syndicate, "Chaos Drive Crusade Radius", 1.5f, 0.5f, 5f, 0.25f, DistanceFormat, Crusader);
        CrusadeMates = new(num++, MultiMenu.Syndicate, "<color=#DF7AE8FF>Crusader</color> Can Crusade Teammates", false, Crusader);

        Poisoner = new(MultiMenu.Syndicate, "<color=#B5004CFF>Poisoner</color>", new object[] { PoisonerOn, LayerEnum.Poisoner, LayerEnum.SyndicateKill, LayerEnum.RandomSyndicate });
        UniquePoisoner = new(num++, MultiMenu.Syndicate, "<color=#B5004CFF>Poisoner</color> Is Unique", false, new object[] { Poisoner, EnableUniques }, true);
        PoisonCd = new(num++, MultiMenu.Syndicate, "Poison Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Poisoner);
        PoisonDur = new(num++, MultiMenu.Syndicate, "Poison Kill Delay", 5f, 1f, 15f, 1f, CooldownFormat, Poisoner);

        SPSettings = new(MultiMenu.Syndicate, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Power</color> Settings", new object[] { SpellslingerOn, TimeKeeperOn,
            LayerEnum.Spellslinger, LayerEnum.TimeKeeper, LayerEnum.RandomSyndicate, LayerEnum.SyndicatePower });
        SPMax = new(num++, MultiMenu.Syndicate, "Max <color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Powers</color>", 1, 1, 14, 1, SPSettings);

        Spellslinger = new(MultiMenu.Syndicate, "<color=#0028F5FF>Spellslinger</color>", new object[] { SpellslingerOn, LayerEnum.Spellslinger, LayerEnum.SyndicatePower,
            LayerEnum.RandomSyndicate });
        UniqueSpellslinger = new(num++, MultiMenu.Syndicate, "<color=#0028F5FF>Spellslinger</color> Is Unique", false, new object[] { Spellslinger, EnableUniques }, true);
        SpellCd = new(num++, MultiMenu.Syndicate, "Spell Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Spellslinger);
        SpellCdIncrease = new(num++, MultiMenu.Syndicate, "Spell Cooldown Increase", 5f, 2.5f, 30f, 2.5f, CooldownFormat, Spellslinger);

        TimeKeeper = new(MultiMenu.Syndicate, "<color=#3769FEFF>Time Keeper</color>", new object[] { TimeKeeperOn, LayerEnum.TimeKeeper, LayerEnum.SyndicatePower, LayerEnum.RandomSyndicate
            });
        UniqueTimeKeeper = new(num++, MultiMenu.Syndicate, "<color=#3769FEFF>Time Keeper</color> Is Unique", false, new object[] { TimeKeeper, EnableUniques }, true);
        TimeCd = new(num++, MultiMenu.Syndicate, "Time Control Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, TimeKeeper);
        TimeDur = new(num++, MultiMenu.Syndicate, "Time Control Duration", 10f, 5f, 30f, 1f, CooldownFormat, TimeKeeper);
        TimeFreezeImmunity = new(num++, MultiMenu.Syndicate, "<color=#008000FF>Syndicate</color> Are Immune To Freeze", true, TimeKeeper);
        TimeRewindImmunity = new(num++, MultiMenu.Syndicate, "<color=#008000FF>Syndicate</color> Are Immune To Rewind", true, TimeKeeper);

        SSuSettings = new(MultiMenu.Syndicate, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Support</color> Settings", new object[] { RebelOn, StalkerOn, WarperOn, LayerEnum.Rebel,
            LayerEnum.Stalker, LayerEnum.Warper, LayerEnum.RandomSyndicate, LayerEnum.SyndicateSupport });
        SSuMax = new(num++, MultiMenu.Syndicate, "Max <color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Supporters</color>", 1, 1, 14, 1, SSuSettings);

        Rebel = new(MultiMenu.Syndicate, "<color=#FFFCCEFF>Rebel</color>", new object[] { RebelOn, LayerEnum.Rebel, LayerEnum.SyndicateSupport, LayerEnum.RandomSyndicate });
        UniqueRebel = new(num++, MultiMenu.Syndicate, "<color=#FFFCCEFF>Rebel</color> Is Unique", false, new object[] { Rebel, EnableUniques }, true);
        RebPromotionCdDecrease = new(num++, MultiMenu.Syndicate, "Ability Cooldown Bonus", 0.75f, 0.25f, 0.9f, 0.05f, MultiplierFormat, Rebel);

        Stalker = new(MultiMenu.Syndicate, "<color=#7E4D00FF>Stalker</color>", new object[] { StalkerOn, LayerEnum.Stalker, LayerEnum.SyndicateSupport, LayerEnum.RandomSyndicate });
        UniqueStalker = new(num++, MultiMenu.Syndicate, "<color=#7E4D00FF>Stalker</color> Is Unique", false, new object[] { Stalker, EnableUniques }, true);
        StalkCd = new(num++, MultiMenu.Syndicate, "Stalk Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Stalker);

        Warper = new(MultiMenu.Syndicate, "<color=#8C7140FF>Warper</color>", new object[] { WarperOn, LayerEnum.Warper, LayerEnum.SyndicateSupport, LayerEnum.RandomSyndicate });
        UniqueWarper = new(num++, MultiMenu.Syndicate, "<color=#8C7140FF>Warper</color> Is Unique", false, new object[] { Warper, EnableUniques }, true);
        WarpCd = new(num++, MultiMenu.Syndicate, "Warp Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Warper);
        WarpDur = new(num++, MultiMenu.Syndicate, "Warp Duration", 5f, 1f, 20f, 1f, CooldownFormat, Warper);
        WarpSelf = new(num++, MultiMenu.Syndicate, "<color=#8C7140FF>Warper</color> Can Warp Themselves", true, Warper);

        SyndicateUtilitySettings = new(MultiMenu.Syndicate, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Utility</color> Settings");

        Anarchist = new(MultiMenu.Syndicate, "<color=#008000FF>Anarchist</color>");
        AnarchKillCd = new(num++, MultiMenu.Syndicate, "Kill Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);

        Banshee = new(MultiMenu.Syndicate, "<color=#E67E22FF>Banshee</color>", BansheeOn);
        ScreamCd = new(num++, MultiMenu.Syndicate, "Scream Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Banshee);
        ScreamDur = new(num++, MultiMenu.Syndicate, "Scream Duration", 10f, 5f, 30f, 1f, CooldownFormat, Banshee);

        Modifiers = new(MultiMenu.Modifier, "<color=#7F7F7FFF>Modifiers</color>", new object[] { GameMode.Classic, GameMode.KillingOnly, GameMode.AllAny, GameMode.Custom });
        AstralOn = new(num++, MultiMenu.Modifier, "<color=#612BEFFF>Astral</color>", parent: Modifiers);
        BaitOn = new(num++, MultiMenu.Modifier, "<color=#00B3B3FF>Bait</color>", parent: Modifiers);
        CowardOn = new(num++, MultiMenu.Modifier, "<color=#456BA8FF>Coward</color>", parent: Modifiers);
        DiseasedOn = new(num++, MultiMenu.Modifier, "<color=#374D1EFF>Diseased</color>", parent: Modifiers);
        DrunkOn = new(num++, MultiMenu.Modifier, "<color=#758000FF>Drunk</color>", parent: Modifiers);
        DwarfOn = new(num++, MultiMenu.Modifier, "<color=#FF8080FF>Dwarf</color>", parent: Modifiers);
        GiantOn = new(num++, MultiMenu.Modifier, "<color=#FFB34DFF>Giant</color>", parent: Modifiers);
        IndomitableOn = new(num++, MultiMenu.Modifier, "<color=#2DE5BEFF>Indomitable</color>", parent: Modifiers);
        ProfessionalOn = new(num++, MultiMenu.Modifier, "<color=#860B7AFF>Professional</color>", parent: Modifiers);
        ShyOn = new(num++, MultiMenu.Modifier, "<color=#1002C5FF>Shy</color>", parent: Modifiers);
        VIPOn = new(num++, MultiMenu.Modifier, "<color=#DCEE85FF>VIP</color>", parent: Modifiers);
        VolatileOn = new(num++, MultiMenu.Modifier, "<color=#FFA60AFF>Volatile</color>", parent: Modifiers);
        YellerOn = new(num++, MultiMenu.Modifier, "<color=#F6AAB7FF>Yeller</color>", parent: Modifiers);

        ModifierSettings = new(MultiMenu.Modifier, "<color=#7F7F7FFF>Modifier</color> Settings", new[] { AstralOn, BaitOn, CowardOn, DiseasedOn, DrunkOn, DwarfOn, GiantOn, ShyOn, VIPOn,
            IndomitableOn, ProfessionalOn, VolatileOn, YellerOn });
        MaxModifiers = new(num++, MultiMenu.Modifier, "Max <color=#7F7F7FFF>Modifiers</color>", 5, 1, 14, 1, ModifierSettings);
        MinModifiers = new(num++, MultiMenu.Modifier, "Min <color=#7F7F7FFF>Modifiers</color>", 5, 1, 14, 1, ModifierSettings);

        Astral = new(MultiMenu.Modifier, "<color=#612BEFFF>Astral</color>", new object[] { AstralOn, EnableUniques }, true);
        UniqueAstral = new(num++, MultiMenu.Modifier, "<color=#612BEFFF>Astral</color> Is Unique", false, Astral);

        Bait = new(MultiMenu.Modifier, "<color=#00B3B3FF>Bait</color>", BaitOn);
        UniqueBait = new(num++, MultiMenu.Modifier, "<color=#00B3B3FF>Bait</color> Is Unique", false, new object[] { Bait, EnableUniques }, true);
        BaitKnows = new(num++, MultiMenu.Modifier, "<color=#00B3B3FF>Bait</color> Knows Who They Are", true, Bait);
        BaitMinDelay = new(num++, MultiMenu.Modifier, "Minimum Delay for <color=#00B3B3FF>Bait</color> Self Report", 0f, 0f, 15f, 0.5f, CooldownFormat, Bait);
        BaitMaxDelay = new(num++, MultiMenu.Modifier, "Maximum Delay for <color=#00B3B3FF>Bait</color> Self Report", 1f, 0f, 15f, 0.5f, CooldownFormat, Bait);

        Coward = new(MultiMenu.Modifier, "<color=#456BA8FF>Coward</color>", new object[] { CowardOn, EnableUniques }, true);
        UniqueCoward = new(num++, MultiMenu.Modifier, "<color=#456BA8FF>Coward</color> Is Unique", false, Coward);

        Diseased = new(MultiMenu.Modifier, "<color=#374D1EFF>Diseased</color>", DiseasedOn);
        UniqueDiseased = new(num++, MultiMenu.Modifier, "<color=#374D1EFF>Diseased</color> Is Unique", false, new object[] { Diseased, EnableUniques }, true);
        DiseasedKnows = new(num++, MultiMenu.Modifier, "<color=#374D1EFF>Diseased</color> Knows Who They Are", true, Diseased);
        DiseasedKillMultiplier = new(num++, MultiMenu.Modifier, "<color=#374D1EFF>Diseased</color> Kill Multiplier", 3f, 1.5f, 5f, 0.5f, MultiplierFormat, Diseased);

        Drunk = new(MultiMenu.Modifier, "<color=#758000FF>Drunk</color>", DrunkOn);
        UniqueDrunk = new(num++, MultiMenu.Modifier, "<color=#758000FF>Drunk</color> Is Unique", false, new object[] { Drunk, EnableUniques }, true);
        DrunkControlsSwap = new(num++, MultiMenu.Modifier, "Controls Reverse Over Time", false, Drunk);
        DrunkKnows = new(num++, MultiMenu.Modifier, "<color=#758000FF>Drunk</color> Knows Who They Are", true, Drunk);
        DrunkInterval = new(num++, MultiMenu.Modifier, "Reversed Controls Interval", 10f, 1f, 20f, 1f, CooldownFormat, Drunk);

        Dwarf = new(MultiMenu.Modifier, "<color=#FF8080FF>Dwarf</color>", DwarfOn);
        UniqueDwarf = new(num++, MultiMenu.Modifier, "<color=#FF8080FF>Dwarf</color> Is Unique", false, new object[] { Dwarf, EnableUniques }, true);
        DwarfSpeed = new(num++, MultiMenu.Modifier, "<color=#FF8080FF>Dwarf</color> Speed", 1.5f, 1f, 2f, 0.05f, MultiplierFormat, Dwarf);
        DwarfScale = new(num++, MultiMenu.Modifier, "<color=#FF8080FF>Dwarf</color> Scale", 0.5f, 0.3f, 1f, 0.025f, MultiplierFormat, Dwarf);

        Giant = new(MultiMenu.Modifier, "<color=#FFB34DFF>Giant</color>", GiantOn);
        UniqueGiant = new(num++, MultiMenu.Modifier, "<color=#FFB34DFF>Giant</color> Is Unique", false, new object[] { Giant, EnableUniques }, true);
        GiantSpeed = new(num++, MultiMenu.Modifier, "<color=#FFB34DFF>Giant</color> Speed", 0.75f, 0.5f, 1f, 0.05f, MultiplierFormat, Giant);
        GiantScale = new(num++, MultiMenu.Modifier, "<color=#FFB34DFF>Giant</color> Scale", 1.5f, 1f, 3.0f, 0.025f, MultiplierFormat, Giant);

        Indomitable = new(MultiMenu.Modifier, "<color=#2DE5BEFF>Indomitable</color>", IndomitableOn);
        UniqueIndomitable = new(num++, MultiMenu.Modifier, "<color=#2DE5BEFF>Indomitable</color> Is Unique", false, new object[] { Indomitable, EnableUniques }, true);
        IndomitableKnows = new(num++, MultiMenu.Modifier, "<color=#2DE5BEFF>Indomitable</color> Knows Who They Are", true, Indomitable);

        Professional = new(MultiMenu.Modifier, "<color=#860B7AFF>Professional</color>", ProfessionalOn);
        UniqueProfessional = new(num++, MultiMenu.Modifier, "<color=#860B7AFF>Professional</color> Is Unique", false, new object[] { Professional, EnableUniques }, true);
        ProfessionalKnows = new(num++, MultiMenu.Modifier, "<color=#860B7AFF>Professional</color> Knows Who They Are", true, Professional);

        Shy = new(MultiMenu.Modifier, "<color=#1002C5FF>Shy</color>", new object[] { ShyOn, EnableUniques }, true);
        UniqueShy = new(num++, MultiMenu.Modifier, "<color=#1002C5FF>Shy</color> Is Unique", false, Shy);

        VIP = new(MultiMenu.Modifier, "<color=#DCEE85FF>VIP</color>", VIPOn);
        UniqueVIP = new(num++, MultiMenu.Modifier, "<color=#DCEE85FF>VIP</color> Is Unique", false, new object[] { VIP, EnableUniques }, true);
        VIPKnows = new(num++, MultiMenu.Modifier, "<color=#DCEE85FF>VIP</color> Knows Who They Are", true, VIP);

        Volatile = new(MultiMenu.Modifier, "<color=#FFA60AFF>Volatile</color>", VolatileOn);
        UniqueVolatile = new(num++, MultiMenu.Modifier, "<color=#FFA60AFF>Volatile</color> Is Unique", false, new object[] { Volatile, EnableUniques }, true);
        VolatileInterval = new(num++, MultiMenu.Modifier, "<color=#FFA60AFF>Volatile</color> Interval", 15f, 10f, 30f, 1f, CooldownFormat, Volatile);
        VolatileKnows = new(num++, MultiMenu.Modifier, "<color=#FFA60AFF>Volatile</color> Knows Who They Are", true, Volatile);

        Yeller = new(MultiMenu.Modifier, "<color=#F6AAB7FF>Yeller</color>", new object[] { YellerOn, EnableUniques }, true);
        UniqueYeller = new(num++, MultiMenu.Modifier, "<color=#F6AAB7FF>Yeller</color> Is Unique", false, Yeller);

        Abilities = new(MultiMenu.Ability, "<color=#FF9900FF>Abilities</color>", new object[] { GameMode.Classic, GameMode.KillingOnly, GameMode.AllAny, GameMode.Custom });
        ButtonBarryOn = new(num++, MultiMenu.Ability, "<color=#E600FFFF>Button Barry</color>", parent: Abilities);
        CrewAssassinOn = new(num++, MultiMenu.Ability, "<color=#8CFFFFFF>Crew</color> <color=#073763FF>Assassin</color>", parent: Abilities);
        InsiderOn = new(num++, MultiMenu.Ability, "<color=#26FCFBFF>Insider</color>", parent: Abilities);
        IntruderAssassinOn = new(num++, MultiMenu.Ability, "<color=#FF0000FF>Intruder</color> <color=#073763FF>Assassin</color>", parent: Abilities);
        MultitaskerOn = new(num++, MultiMenu.Ability, "<color=#FF804DFF>Multitasker</color>", parent: Abilities);
        NeutralAssassinOn = new(num++, MultiMenu.Ability, "<color=#B3B3B3FF>Neutral</color> <color=#073763FF>Assassin</color>", parent: Abilities);
        NinjaOn = new(num++, MultiMenu.Ability, "<color=#A84300FF>Ninja</color>", parent: Abilities);
        PoliticianOn = new(num++, MultiMenu.Ability, "<color=#CCA3CCFF>Politician</color>", parent: Abilities);
        RadarOn = new(num++, MultiMenu.Ability, "<color=#FF0080FF>Radar</color>", parent: Abilities);
        RuthlessOn = new(num++, MultiMenu.Ability, "<color=#2160DDFF>Ruthless</color>", parent: Abilities);
        SnitchOn = new(num++, MultiMenu.Ability, "<color=#D4AF37FF>Snitch</color>", parent: Abilities);
        SwapperOn = new(num++, MultiMenu.Ability, "<color=#66E666FF>Swapper</color>", parent: Abilities);
        SyndicateAssassinOn = new(num++, MultiMenu.Ability, "<color=#008000FF>Syndicate</color> <color=#073763FF>Assassin</color>", parent: Abilities);
        TiebreakerOn = new(num++, MultiMenu.Ability, "<color=#99E699FF>Tiebreaker</color>", parent: Abilities);
        TorchOn = new(num++, MultiMenu.Ability, "<color=#FFFF99FF>Torch</color>", parent: Abilities);
        TunnelerOn = new(num++, MultiMenu.Ability, "<color=#E91E63FF>Tunneler</color>", parent: Abilities);
        UnderdogOn = new(num++, MultiMenu.Ability, "<color=#841A7FFF>Underdog</color>", parent: Abilities);

        AbilitySettings = new(MultiMenu.Ability, "<color=#FF9900FF>Ability</color> Settings", new[] { CrewAssassinOn, NeutralAssassinOn, IntruderAssassinOn, SyndicateAssassinOn, NinjaOn,
            ButtonBarryOn, InsiderOn, MultitaskerOn, PoliticianOn, RadarOn, RuthlessOn, SnitchOn, SwapperOn, TiebreakerOn, TunnelerOn, UnderdogOn });
        MaxAbilities = new(num++, MultiMenu.Ability, "Max <color=#FF9900FF>Abilities</color>", 5, 1, 14, 1, AbilitySettings);
        MinAbilities = new(num++, MultiMenu.Ability, "Min <color=#FF9900FF>Abilities</color>", 5, 1, 14, 1, AbilitySettings);

        Assassin = new(MultiMenu.Ability, "<color=#073763FF>Assassin</color>", new[] { CrewAssassinOn, NeutralAssassinOn, IntruderAssassinOn, SyndicateAssassinOn });
        UniqueCrewAssassin = new(num++, MultiMenu.Ability, "<color=#8CFFFFFF>Crew</color> <color=#073763FF>Assassin</color> Is Unique", false, new object[] { CrewAssassinOn,
            EnableUniques }, true);
        UniqueNeutralAssassin = new(num++, MultiMenu.Ability, "<color=#B3B3B3FF>Neutral</color> <color=#073763FF>Assassin</color> Is Unique", false, new object[] { NeutralAssassinOn,
            EnableUniques }, true);
        UniqueIntruderAssassin = new(num++, MultiMenu.Ability, "<color=#FF0000FF>Intruder</color> <color=#073763FF>Assassin</color> Is Unique", false, new object[] { IntruderAssassinOn,
            EnableUniques }, true);
        UniqueSyndicateAssassin = new(num++, MultiMenu.Ability, "<color=#008000FF>Syndicate</color> <color=#073763FF>Assassin</color> Is Unique", false, new object[] { SyndicateAssassinOn,
            EnableUniques }, true);
        AssassinKills = new(num++, MultiMenu.Ability, "<color=#073763FF>Assassin</color> Guess Limit", 1, 1, 15, 1, Assassin);
        AssassinMultiKill = new(num++, MultiMenu.Ability, "<color=#073763FF>Assassin</color> Can Kill More Than Once Per Meeting", false, Assassin);
        AssassinGuessNeutralBenign = new(num++, MultiMenu.Ability, "<color=#073763FF>Assassin</color> Can Guess <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Benigns</color>", false,
            Assassin);
        AssassinGuessNeutralEvil = new(num++, MultiMenu.Ability, "<color=#073763FF>Assassin</color> Can Guess <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Evils</color>", false,
            Assassin);
        AssassinGuessInvestigative = new(num++, MultiMenu.Ability, "<color=#073763FF>Assassin</color> Can Guess <color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Investigatives</color>",
            false, new[] { Assassin, CISettings }, true);
        AssassinGuessPest = new(num++, MultiMenu.Ability, "<color=#073763FF>Assassin</color> Can Guess <color=#424242FF>Pestilence</color>", false, new[] { Assassin, Pestilence }, true);
        AssassinGuessModifiers = new(num++, MultiMenu.Ability, "<color=#073763FF>Assassin</color> Can Guess Select <color=#7F7F7FFF>Modifiers</color>", false, new object[] { Assassin,
            EnableModifiers }, true);
        AssassinGuessObjectifiers = new(num++, MultiMenu.Ability, "<color=#073763FF>Assassin</color> Can Guess Select <color=#DD585BFF>Objectifiers</color>", false, new object[] { Assassin,
            EnableObjectifiers }, true);
        AssassinGuessAbilities = new(num++, MultiMenu.Ability, "<color=#073763FF>Assassin</color> Can Guess <color=#FF9900FF>Abilities</color>", false, new object[] { Assassin,
            EnableAbilities }, true);
        AssassinateAfterVoting = new(num++, MultiMenu.Ability, "<color=#073763FF>Assassin</color> Can Guess After Voting", false, Assassin);

        ButtonBarry = new(MultiMenu.Ability, "<color=#E600FFFF>Button Barry</color>", ButtonBarryOn);
        UniqueButtonBarry = new(num++, MultiMenu.Ability, "<color=#E600FFFF>Button Barry</color> Is Unique", false, new object[] { ButtonBarry, EnableUniques }, true);
        ButtonCooldown = new(num++, MultiMenu.Ability, "Button Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, ButtonBarry);

        Insider = new(MultiMenu.Ability, "<color=#26FCFBFF>Insider</color>", InsiderOn);
        UniqueInsider = new(num++, MultiMenu.Ability, "<color=#26FCFBFF>Insider</color> Is Unique", false, new object[] { Insider, EnableUniques }, true);
        InsiderKnows = new(num++, MultiMenu.Ability, "<color=#26FCFBFF>Insider</color> Knows Who They Are", true, Insider);

        Multitasker = new(MultiMenu.Ability, "<color=#FF804DFF>Multitasker</color>", MultitaskerOn);
        UniqueMultitasker = new(num++, MultiMenu.Ability, "<color=#FF804DFF>Multitasker</color> Is Unique", false, new object[] { Multitasker, EnableUniques }, true);
        Transparancy = new(num++, MultiMenu.Ability, "Task Transparancy", 50f, 10f, 80f, 5f, PercentFormat, Multitasker);

        Ninja = new(MultiMenu.Ability, "<color=#A84300FF>Ninja</color>", new object[] { NinjaOn, EnableUniques }, true);
        UniqueNinja = new(num++, MultiMenu.Ability, "<color=#A84300FF>Ninja</color> Is Unique", false, Ninja);

        Politician = new(MultiMenu.Ability, "<color=#CCA3CCFF>Politician</color>", PoliticianOn);
        UniquePolitician = new(num++, MultiMenu.Ability, "<color=#CCA3CCFF>Politician</color> Is Unique", false, new object[] { Politician, EnableUniques }, true);
        PoliticianVoteBank = new(num++, MultiMenu.Ability, "Initial <color=#CCA3CCFF>Politician</color> Initial Vote Bank", 0, 0, 10, 1, Politician);
        PoliticianAnonymous = new(num++, MultiMenu.Ability, "Anonymous <color=#CCA3CCFF>Politician</color> Votes", false, Politician);
        PoliticianButton = new(num++, MultiMenu.Ability, "<color=#CCA3CCFF>Politician</color> Can Button", true, Politician);

        Radar = new(MultiMenu.Ability, "<color=#FF0080FF>Radar</color>", new object[] { RadarOn, EnableUniques }, true);
        UniqueRadar = new(num++, MultiMenu.Ability, "<color=#FF0080FF>Radar</color> Is Unique", false, Radar);

        Ruthless = new(MultiMenu.Ability, "<color=#2160DDFF>Ruthless</color>", RuthlessOn);
        UniqueRuthless = new(num++, MultiMenu.Ability, "<color=#2160DDFF>Ruthless</color> Is Unique", false, new object[] { Ruthless, EnableUniques }, true);
        RuthlessKnows = new(num++, MultiMenu.Ability, "<color=#2160DDFF>Ruthless</color> Knows Who They Are", true, Ruthless);

        Snitch = new(MultiMenu.Ability, "<color=#D4AF37FF>Snitch</color>", SnitchOn);
        UniqueSnitch = new(num++, MultiMenu.Ability, "<color=#D4AF37FF>Snitch</color> Is Unique", false, new object[] { Snitch, EnableUniques }, true);
        SnitchKnows = new(num++, MultiMenu.Ability, "<color=#D4AF37FF>Snitch</color> Knows Who They Are", true, Snitch);
        SnitchSeesNeutrals = new(num++, MultiMenu.Ability, "<color=#D4AF37FF>Snitch</color> Sees <color=#B3B3B3FF>Neutrals</color>", false, Snitch);
        SnitchSeesCrew = new(num++, MultiMenu.Ability, "<color=#D4AF37FF>Snitch</color> Sees <color=#8CFFFFFF>Crew</color>", false, Snitch);
        SnitchSeesRoles = new(num++, MultiMenu.Ability, "<color=#D4AF37FF>Snitch</color> Sees Exact <color=#FFD700FF>Roles</color>", false, Snitch);
        SnitchTasksRemaining = new(num++, MultiMenu.Ability, "Tasks Remaining When Revealed", 1, 1, 5, 1, Snitch);
        SnitchSeestargetsInMeeting = new(num++, MultiMenu.Ability, "<color=#D4AF37FF>Snitch</color> Sees Evils In Meetings", true, Snitch);

        Swapper = new(MultiMenu.Ability, "<color=#66E666FF>Swapper</color>", SwapperOn);
        UniqueSwapper = new(num++, MultiMenu.Ability, "<color=#66E666FF>Swapper</color> Is Unique", false, new object[] { Swapper, EnableUniques }, true);
        SwapperButton = new(num++, MultiMenu.Ability, "<color=#66E666FF>Swapper</color> Can Button", true, Swapper);
        SwapAfterVoting = new(num++, MultiMenu.Ability, "<color=#66E666FF>Swapper</color> Can Swap After Voting", false, Swapper);
        SwapSelf = new(num++, MultiMenu.Ability, "<color=#66E666FF>Swapper</color> Can Swap Themself", true, Swapper);

        Tiebreaker = new(MultiMenu.Ability, "<color=#99E699FF>Tiebreaker</color>", TiebreakerOn);
        UniqueTiebreaker = new(num++, MultiMenu.Ability, "<color=#99E699FF>Tiebreaker</color> Is Unique", false, new object[] { Tiebreaker, EnableUniques }, true);
        TiebreakerKnows = new(num++, MultiMenu.Ability, "<color=#99E699FF>Tiebreaker</color> Knows Who They Are", true, Tiebreaker);

        Torch = new(MultiMenu.Ability, "<color=#FFFF99FF>Torch</color>", new object[] { TorchOn, EnableUniques }, true);
        UniqueTorch = new(num++, MultiMenu.Ability, "<color=#FFFF99FF>Torch</color> Is Unique", false, Torch);

        Tunneler = new(MultiMenu.Ability, "<color=#E91E63FF>Tunneler</color>", TunnelerOn);
        UniqueTunneler = new(num++, MultiMenu.Ability, "<color=#E91E63FF>Tunneler</color> Is Unique", false, new object[] { Tunneler, EnableUniques }, true);
        TunnelerKnows = new(num++, MultiMenu.Ability, "<color=#E91E63FF>Tunneler</color> Knows Who They Are", true, Tunneler);

        Underdog = new(MultiMenu.Ability, "<color=#841A7FFF>Underdog</color>", UnderdogOn);
        UniqueUnderdog = new(num++, MultiMenu.Ability, "<color=#841A7FFF>Underdog</color> Is Unique", false, new object[] { Underdog, EnableUniques }, true);
        UnderdogKnows = new(num++, MultiMenu.Ability, "<color=#841A7FFF>Underdog</color> Knows Who They Are", true, Underdog);
        UnderdogKillBonus = new(num++, MultiMenu.Ability, "Kill Cooldown Bonus", 5f, 2.5f, 30f, 2.5f, CooldownFormat, Underdog);
        UnderdogIncreasedKC = new(num++, MultiMenu.Ability, "Increased Kill Cooldown When 2+ Teammates", true, Underdog);

        Objectifiers = new(MultiMenu.Objectifier, "<color=#DD585BFF>Objectifiers</color>", new object[] { GameMode.Classic, GameMode.KillingOnly, GameMode.AllAny, GameMode.Custom });
        AlliedOn = new(num++, MultiMenu.Objectifier, "<color=#4545A9FF>Allied</color>", parent: Objectifiers);
        CorruptedOn = new(num++, MultiMenu.Objectifier, "<color=#4545FFFF>Corrupted</color>", parent: Objectifiers);
        DefectorOn = new(num++, MultiMenu.Objectifier, "<color=#E1C849FF>Defector</color>", parent: Objectifiers);
        FanaticOn = new(num++, MultiMenu.Objectifier, "<color=#678D36FF>Fanatic</color>", parent: Objectifiers);
        LinkedOn = new(num++, MultiMenu.Objectifier, "<color=#FF351FFF>Linked</color> Pairs", 1, 7, Objectifiers);
        LoversOn = new(num++, MultiMenu.Objectifier, "<color=#FF66CCFF>Lovers</color> Pairs", 1, 7, Objectifiers);
        MafiaOn = new(num++, MultiMenu.Objectifier, "<color=#00EEFFFF>Mafia</color>", 2, parent: Objectifiers);
        OverlordOn = new(num++, MultiMenu.Objectifier, "<color=#008080FF>Overlord</color>", parent: Objectifiers);
        RivalsOn = new(num++, MultiMenu.Objectifier, "<color=#3D2D2CFF>Rivals</color> Pairs", 1, 7, Objectifiers);
        TaskmasterOn = new(num++, MultiMenu.Objectifier, "<color=#ABABFFFF>Taskmaster</color>", parent: Objectifiers);
        TraitorOn = new(num++, MultiMenu.Objectifier, "<color=#370D43FF>Traitor</color>", parent: Objectifiers);

        Betrayer.Parents = new object[] { TraitorOn, FanaticOn };
        NPSettings.Parents = new object[] { TraitorOn, FanaticOn, PhantomOn };

        ObjectifierSettings = new(MultiMenu.Objectifier, "<color=#DD585BFF>Objectifier</color> Settings", new[] { AlliedOn, CorruptedOn, DefectorOn, FanaticOn, LinkedOn, LoversOn, MafiaOn,
            RivalsOn, OverlordOn, TaskmasterOn, TraitorOn });
        MaxObjectifiers = new(num++, MultiMenu.Objectifier, "Max <color=#DD585BFF>Objectifiers</color>", 5, 1, 14, 1, ObjectifierSettings);
        MinObjectifiers = new(num++, MultiMenu.Objectifier, "Min <color=#DD585BFF>Objectifiers</color>", 5, 1, 14, 1, ObjectifierSettings);

        Allied = new(MultiMenu.Objectifier, "<color=#4545A9FF>Allied</color>", AlliedOn);
        UniqueAllied = new(num++, MultiMenu.Objectifier, "<color=#4545A9FF>Allied</color> Is Unique", false, new object[] { Allied, EnableUniques }, true);
        AlliedFaction = new(num++, MultiMenu.Objectifier, "<color=#4545A9FF>Allied</color> Faction", new[] { "Random", "Intruder", "Syndicate", "Crew" }, Allied);

        Corrupted = new(MultiMenu.Objectifier, "<color=#4545FFFF>Corrupted</color>", CorruptedOn);
        UniqueCorrupted = new(num++, MultiMenu.Objectifier, "<color=#4545FFFF>Corrupted</color> Is Unique", false, new object[] { Corrupted, EnableUniques }, true);
        CorruptCd = new(num++, MultiMenu.Objectifier, "Corrupt Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, Corrupted);
        AllCorruptedWin = new(num++, MultiMenu.Objectifier, "All <color=#4545FFFF>Corrupted</color> Win Together", false, Corrupted);
        CorruptedVent = new(num++, MultiMenu.Objectifier, "<color=#4545FFFF>Corrupted</color> Can Vent", false, Corrupted);

        Defector = new(MultiMenu.Objectifier, "<color=#E1C849FF>Defector</color>", DefectorOn);
        UniqueDefector = new(num++, MultiMenu.Objectifier, "<color=#E1C849FF>Defector</color> Is Unique", false, new object[] { Defector, EnableUniques }, true);
        DefectorKnows = new(num++, MultiMenu.Objectifier, "<color=#E1C849FF>Defector</color> Knows Who They Are", true, Defector);
        DefectorFaction = new(num++, MultiMenu.Objectifier, "<color=#E1C849FF>Defector</color> Faction", new[] { "Random", "Opposing Evil", "Crew" }, Defector);

        Fanatic = new(MultiMenu.Objectifier, "<color=#678D36FF>Fanatic</color>", FanaticOn);
        UniqueFanatic = new(num++, MultiMenu.Objectifier, "<color=#678D36FF>Fanatic</color> Is Unique", false, new object[] { Fanatic, EnableUniques }, true);
        FanaticKnows = new(num++, MultiMenu.Objectifier, "<color=#678D36FF>Fanatic</color> Knows Who They Are", true, Fanatic);
        FanaticColourSwap = new(num++, MultiMenu.Objectifier, "Turned <color=#678D36FF>Fanatic</color> Swaps Colours For Investigative <color=#FFD700FF>Roles</color>", false, Fanatic);
        SnitchSeesFanatic = new(num++, MultiMenu.Objectifier, "<color=#D4AF37FF>Snitch</color> Sees Turned <color=#678D36FF>Fanatic</color>", true, new[] { FanaticOn, SnitchOn }, true);
        RevealerRevealsFanatic = new(num++, MultiMenu.Objectifier, "<color=#D3D3D3FF>Revealer</color> Reveals Turned <color=#678D36FF>Fanatic</color>", false, new[] { FanaticOn, RevealerOn
            }, true);

        Linked = new(MultiMenu.Objectifier, "<color=#FF351FFF>Linked</color>", LinkedOn);
        UniqueLinked = new(num++, MultiMenu.Objectifier, "<color=#FF351FFF>Linked</color> Is Unique", false, new object[] { Linked, EnableUniques }, true);
        LinkedChat = new(num++, MultiMenu.Objectifier, "Enable <color=#FF351FFF>Linked</color> Chat", true, Linked);
        LinkedRoles = new(num++, MultiMenu.Objectifier, "<color=#FF351FFF>Linked</color> Know Each Other's <color=#FFD700FF>Roles</color>", true, Linked);

        Lovers = new(MultiMenu.Objectifier, "<color=#FF66CCFF>Lovers</color>", LoversOn);
        UniqueLovers = new(num++, MultiMenu.Objectifier, "<color=#FF66CCFF>Lovers</color> Is Unique", false, new object[] { Lovers, EnableUniques }, true);
        BothLoversDie = new(num++, MultiMenu.Objectifier, "Both <color=#FF66CCFF>Lovers</color> Die", true, Lovers);
        LoversChat = new(num++, MultiMenu.Objectifier, "Enable <color=#FF66CCFF>Lovers</color> Chat", true, Lovers);
        LoversRoles = new(num++, MultiMenu.Objectifier, "<color=#FF66CCFF>Lovers</color> Know Each Other's <color=#FFD700FF>Roles</color>", true, Lovers);

        Mafia = new(MultiMenu.Objectifier, "<color=#00EEFFFF>Mafia</color>", MafiaOn);
        UniqueMafia = new(num++, MultiMenu.Objectifier, "<color=#00EEFFFF>Mafia</color> Is Unique", false, new object[] { Mafia, EnableUniques }, true);
        MafiaRoles = new(num++, MultiMenu.Objectifier, "<color=#00EEFFFF>Mafia</color> Know Each Other's <color=#FFD700FF>Roles</color>", true, Mafia);
        MafVent = new(num++, MultiMenu.Objectifier, "<color=#00EEFFFF>Mafia</color> Can Vent", false, Mafia);

        Overlord = new(MultiMenu.Objectifier, "<color=#008080FF>Overlord</color>", OverlordOn);
        UniqueOverlord = new(num++, MultiMenu.Objectifier, "<color=#008080FF>Overlord</color> Is Unique", false, new object[] { Overlord, EnableUniques }, true);
        OverlordKnows = new(num++, MultiMenu.Objectifier, "<color=#008080FF>Overlord</color> Knows Who They Are", true, Overlord);
        OverlordMeetingWinCount = new(num++, MultiMenu.Objectifier, "<color=#008080FF>Overlord</color> Meeting Timer", 2, 1, 20, 1, Overlord);

        Rivals = new(MultiMenu.Objectifier, "<color=#3D2D2CFF>Rivals</color>", RivalsOn);
        UniqueRivals = new(num++, MultiMenu.Objectifier, "<color=#3D2D2CFF>Rivals</color> Is Unique", false, new object[] { Rivals, EnableUniques }, true);
        RivalsChat = new(num++, MultiMenu.Objectifier, "Enable <color=#3D2D2CFF>Rivals</color> Chat", true, Rivals);
        RivalsRoles = new(num++, MultiMenu.Objectifier, "<color=#3D2D2CFF>Rivals</color> Know Each Other's <color=#FFD700FF>Roles</color>", true, Rivals);

        Taskmaster = new(MultiMenu.Objectifier, "<color=#ABABFFFF>Taskmaster</color>", TaskmasterOn);
        UniqueTaskmaster = new(num++, MultiMenu.Objectifier, "<color=#ABABFFFF>Taskmaster</color> Is Unique", false, new object[] { Taskmaster, EnableUniques }, true);
        TMTasksRemaining = new(num++, MultiMenu.Objectifier, "Tasks Remaining When Revealed", 1, 1, 5, 1, Taskmaster);

        Traitor = new(MultiMenu.Objectifier, "<color=#370D43FF>Traitor</color>", TraitorOn);
        UniqueTraitor = new(num++, MultiMenu.Objectifier, "<color=#370D43FF>Traitor</color> Is Unique", false, new object[] { Traitor, EnableUniques }, true);
        TraitorKnows = new(num++, MultiMenu.Objectifier, "<color=#370D43FF>Traitor</color> Knows Who They Are", true, Traitor);
        SnitchSeesTraitor = new(num++, MultiMenu.Objectifier, "<color=#D4AF37FF>Snitch</color> Sees Turned <color=#370D43FF>Traitor</color>", true, new object[] { Traitor, SnitchOn },
            true);
        RevealerRevealsTraitor = new(num++, MultiMenu.Objectifier, "<color=#D3D3D3FF>Revealer</color> Reveals Turned <color=#370D43FF>Traitor</color>", false, new object[] { Traitor,
            RevealerOn }, true);
        TraitorColourSwap = new(num++, MultiMenu.Objectifier, "Turned <color=#370D43FF>Traitor</color> Swaps Colours For Investigative <color=#FFD700FF>Roles</color>", false, Traitor);

        RoleList = new(MultiMenu.RoleList, "Role List Entries");
        Entry1 = new(num++, "Entry");
        Entry2 = new(num++, "Entry");
        Entry3 = new(num++, "Entry");
        Entry4 = new(num++, "Entry");
        Entry5 = new(num++, "Entry");
        Entry6 = new(num++, "Entry");
        Entry7 = new(num++, "Entry");
        Entry8 = new(num++, "Entry");
        Entry9 = new(num++, "Entry");
        Entry10 = new(num++, "Entry");
        Entry11 = new(num++, "Entry");
        Entry12 = new(num++, "Entry");
        Entry13 = new(num++, "Entry");
        Entry14 = new(num++, "Entry");
        Entry15 = new(num++, "Entry");

        BanList = new(MultiMenu.RoleList, "Role List Bans");
        Ban1 = new(num++, "Ban");
        Ban2 = new(num++, "Ban");
        Ban3 = new(num++, "Ban");
        Ban4 = new(num++, "Ban");
        Ban5 = new(num++, "Ban");

        FreeBans = new(MultiMenu.RoleList, "Free Bans");
        BanCrewmate = new(num++, MultiMenu.RoleList, "Ban <color=#8CFFFFFF>Crewmate</color>", false);
        BanImpostor = new(num++, MultiMenu.RoleList, "Ban <color=#FF0000FF>Impostor</color>", false);
        BanAnarchist = new(num++, MultiMenu.RoleList, "Ban <color=#008000FF>Anarchist</color>", false);

        EnablePostmortals = new(MultiMenu.RoleList, "Postmortals");
        EnableBanshee = new(num++, MultiMenu.RoleList, "Enable <color=#E67E22FF>Banshee</color>", false);
        EnableGhoul = new(num++, MultiMenu.RoleList, "Enable <color=#F1C40FFF>Ghoul</color>", false);
        EnablePhantom = new(num++, MultiMenu.RoleList, "Enable <color=#662962FF>Phantom</color>", false);
        EnableRevealer = new(num++, MultiMenu.RoleList, "Enable <color=#D3D3D3FF>Revealer</color>", false);

        CustomOption.SaveSettings("DefaultSettings");

        LogInfo($"There exist {num} options lmao (number jumpscare)");
    }
}