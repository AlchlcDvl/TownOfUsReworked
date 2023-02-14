using System;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.Lobby.CustomOption
{
    public class Generate
    {
        //Global Options
        public static CustomHeaderOption GlobalSettings;
        public static CustomNumberOption PlayerSpeed;
        public static CustomNumberOption GhostSpeed;
        public static CustomNumberOption InteractionDistance;
        public static CustomNumberOption EmergencyButtonCount;
        public static CustomNumberOption EmergencyButtonCooldown;
        public static CustomNumberOption InitialCooldowns;
        public static CustomNumberOption DiscussionTime;
        public static CustomNumberOption VotingTime;
        public static CustomToggleOption ConfirmEjects;
        public static CustomToggleOption EjectionRevealsRole;
        public static CustomStringOption TaskBarMode;
        //public static CustomNumberOption ChatCooldown;

        //All Any Options
        public static CustomHeaderOption AllAnySettings;
        public static CustomToggleOption EnableUniques;

        //CI Role Spawn
        public static CustomHeaderOption CrewInvestigativeRoles;
        public static CustomNumberOption DetectiveOn;
        //public static CustomNumberOption InvestigatorOn;
        public static CustomNumberOption CoronerOn;
        public static CustomNumberOption SheriffOn;
        public static CustomNumberOption MediumOn;
        public static CustomNumberOption AgentOn;
        public static CustomNumberOption TrackerOn;
        public static CustomNumberOption InspectorOn;
        public static CustomNumberOption OperativeOn;
        public static CustomNumberOption SeerOn;

        //CSv Role Spawn
        public static CustomHeaderOption CrewSovereignRoles;
        public static CustomNumberOption MayorOn;
        public static CustomNumberOption SwapperOn;

        //CP Role Spawn
        public static CustomHeaderOption CrewProtectiveRoles;
        public static CustomNumberOption AltruistOn;
        public static CustomNumberOption MedicOn;
        public static CustomNumberOption TimeLordOn;

        //CA Role Spawn
        public static CustomHeaderOption CrewAuditorRoles;
        public static CustomNumberOption VampireHunterOn;
        public static CustomNumberOption MysticOn;

        //CK Role Spawn
        public static CustomHeaderOption CrewKillingRoles;
        public static CustomNumberOption VeteranOn;
        public static CustomNumberOption VigilanteOn;

        //CS Role Spawn
        public static CustomHeaderOption CrewSupportRoles;
        public static CustomNumberOption EngineerOn;
        public static CustomNumberOption ShifterOn;
        public static CustomNumberOption EscortOn;
        public static CustomNumberOption TransporterOn;
        public static CustomNumberOption RevealerOn;
        public static CustomNumberOption RetributionistOn;
        public static CustomNumberOption ChameleonOn;

        //CU Role Spawn
        public static CustomHeaderOption CrewUtilityRoles;
        public static CustomNumberOption CrewmateOn;

        //NB Role Spawn
        public static CustomHeaderOption NeutralBenignRoles;
        public static CustomNumberOption AmnesiacOn;
        public static CustomNumberOption GuardianAngelOn;
        public static CustomNumberOption SurvivorOn;
        public static CustomNumberOption ThiefOn;

        //NP Role Spawn
        public static CustomHeaderOption NeutralProselyteRoles;
        public static CustomNumberOption PhantomOn;

        //NN Role Spawn
        public static CustomHeaderOption NeutralNeophyteRoles;
        public static CustomNumberOption DraculaOn;
        public static CustomNumberOption JackalOn;
        public static CustomNumberOption NecromancerOn;
        public static CustomNumberOption WhispererOn;

        //NE Role Spawn
        public static CustomHeaderOption NeutralEvilRoles;
        public static CustomNumberOption ExecutionerOn;
        public static CustomNumberOption ActorOn;
        public static CustomNumberOption JesterOn;
        public static CustomNumberOption CannibalOn;
        public static CustomNumberOption BountyHunterOn;
        public static CustomNumberOption TrollOn;
        public static CustomNumberOption GuesserOn;

        //NK Role Spawn
        public static CustomHeaderOption NeutralKillingRoles;
        public static CustomNumberOption ArsonistOn;
        public static CustomNumberOption CryomaniacOn;
        public static CustomNumberOption PlaguebearerOn;
        public static CustomNumberOption GlitchOn;
        public static CustomNumberOption MurdererOn;
        public static CustomNumberOption WerewolfOn;
        public static CustomNumberOption SerialKillerOn;
        public static CustomNumberOption JuggernautOn;

        //IC Role Spawn
        public static CustomHeaderOption IntruderConcealingRoles;
        public static CustomNumberOption BlackmailerOn;
        public static CustomNumberOption CamouflagerOn;
        public static CustomNumberOption GrenadierOn;
        public static CustomNumberOption UndertakerOn;
        public static CustomNumberOption JanitorOn;

        //ID Role Spawn
        public static CustomHeaderOption IntruderDeceptionRoles;
        public static CustomNumberOption MorphlingOn;
        public static CustomNumberOption DisguiserOn;
        public static CustomNumberOption WraithOn;

        //IK Role Spawn
        public static CustomHeaderOption IntruderKillingRoles;

        //IS Role Spawn
        public static CustomHeaderOption IntruderSupportRoles;
        public static CustomNumberOption ConsigliereOn;
        public static CustomNumberOption GodfatherOn;
        public static CustomNumberOption ConsortOn;
        public static CustomNumberOption MinerOn;
        public static CustomNumberOption TimeMasterOn;
        public static CustomNumberOption TeleporterOn;

        //IU Role Spawn
        public static CustomHeaderOption IntruderUtilityRoles;
        public static CustomNumberOption ImpostorOn;

        //SSu Role Spawn
        public static CustomHeaderOption SyndicateSupportRoles;
        public static CustomNumberOption WarperOn;
        public static CustomNumberOption RebelOn;
        public static CustomNumberOption ConcealerOn;
        public static CustomNumberOption BeamerOn;

        //SD Role Spawn
        public static CustomHeaderOption SyndicateDisruptionRoles;
        public static CustomNumberOption FramerOn;
        public static CustomNumberOption ShapeshifterOn;
        public static CustomNumberOption PoisonerOn;
        public static CustomNumberOption DrunkardOn;

        //SyK Role Spawn
        public static CustomHeaderOption SyndicateKillingRoles;
        public static CustomNumberOption GorgonOn;
        public static CustomNumberOption BomberOn;

        //SU Role Spawn
        public static CustomHeaderOption SyndicateUtilityRoles;
        public static CustomNumberOption AnarchistOn;

        //Modifier Spawn
        public static CustomHeaderOption Modifiers;
        public static CustomNumberOption BaitOn;
        public static CustomNumberOption DiseasedOn;
        public static CustomNumberOption GiantOn;
        public static CustomNumberOption DwarfOn;
        public static CustomNumberOption CowardOn;
        public static CustomNumberOption VIPOn;
        public static CustomNumberOption ShyOn;
        public static CustomNumberOption DrunkOn;
        public static CustomNumberOption FlincherOn;
        public static CustomNumberOption IndomitableOn;
        public static CustomNumberOption VolatileOn;
        public static CustomNumberOption ProfessionalOn;

        //Ability Spawn
        public static CustomHeaderOption Abilities;
        public static CustomNumberOption CrewAssassinOn;
        public static CustomNumberOption IntruderAssassinOn;
        public static CustomNumberOption SyndicateAssassinOn;
        public static CustomNumberOption NeutralAssassinOn;
        public static CustomNumberOption TorchOn;
        public static CustomNumberOption LighterOn;
        public static CustomNumberOption ButtonBarryOn;
        public static CustomNumberOption TiebreakerOn;
        public static CustomNumberOption TunnelerOn;
        public static CustomNumberOption UnderdogOn;
        public static CustomNumberOption SnitchOn;
        public static CustomNumberOption RadarOn;
        public static CustomNumberOption InsiderOn;
        public static CustomNumberOption MultitaskerOn;
        public static CustomNumberOption RuthlessOn;

        //Objectifier Spawn
        public static CustomHeaderOption Objectifiers;
        public static CustomNumberOption LoversOn;
        public static CustomNumberOption AlliedOn;
        public static CustomNumberOption TraitorOn;
        public static CustomNumberOption RivalsOn;
        public static CustomNumberOption FanaticOn;
        public static CustomNumberOption TaskmasterOn;
        public static CustomNumberOption OverlordOn;
        public static CustomNumberOption CorruptedOn;

        //Map Settings
        public static CustomHeaderOption MapSettings;
        public static CustomToggleOption RandomMapEnabled;
        public static CustomNumberOption RandomMapSkeld;
        public static CustomNumberOption RandomMapMira;
        public static CustomNumberOption RandomMapPolus;
        public static CustomNumberOption RandomMapAirship;
        public static CustomNumberOption RandomMapSubmerged;
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
        public static CustomToggleOption ColourblindComms;
        public static CustomToggleOption MeetingColourblind;
        public static CustomToggleOption AnonymousVoting;
        public static CustomStringOption WhoCanVent;
        public static CustomStringOption SkipButtonDisable;
        public static CustomToggleOption FactionSeeRoles;
        public static CustomToggleOption ParallelMedScans;
        public static CustomToggleOption VisualTasks;
        public static CustomStringOption RoleFactionReports;
        public static CustomToggleOption KillerReports;
        public static CustomToggleOption LocationReports;
        public static CustomToggleOption NoNames;
        public static CustomToggleOption Whispers;
        public static CustomToggleOption AppearanceAnimation;
        public static CustomToggleOption LighterDarker;
        public static CustomToggleOption RandomSpawns;
        public static CustomToggleOption EnableModifiers;
        public static CustomToggleOption EnableAbilities;
        public static CustomToggleOption EnableObjectifiers;
        public static CustomToggleOption VentTargetting;

        //QoL Options
        public static CustomHeaderOption QualityChanges;
        public static CustomToggleOption DeadSeeEverything;
        public static CustomToggleOption DisableLevels;
        public static CustomToggleOption WhiteNameplates;
        public static CustomToggleOption SeeTasks;
        public static CustomToggleOption CustomEject;

        //Better Skeld Options
        public static CustomHeaderOption BetterSkeldSettings;
        public static CustomToggleOption SkeldVentImprovements;

        //Better Airship Options
        public static CustomHeaderOption BetterAirshipSettings;
        public static CustomStringOption SpawnType;
        public static CustomStringOption MoveAdmin;
        public static CustomStringOption MoveElectrical;
        public static CustomToggleOption NewSpawns;
        public static CustomToggleOption MeetingSpawnChoice;
        public static CustomNumberOption MinDoorSwipeTime;
        public static CustomToggleOption CallPlatform;
        public static CustomToggleOption MoveDivert;
        public static CustomToggleOption MoveFuel;
        public static CustomToggleOption AddTeleporters;
        public static CustomToggleOption MoveVitals;

        //Better Polus Options
        public static CustomHeaderOption BetterPolusSettings;
        public static CustomToggleOption PolusVentImprovements;
        public static CustomToggleOption VitalsLab;
        public static CustomToggleOption ColdTempDeathValley;
        public static CustomToggleOption WifiChartCourseSwap;

        //Game Modes
        public static CustomHeaderOption GameModeSettings;
        public static CustomStringOption GameMode;

        //Killing Only Options
        public static CustomHeaderOption KillingOnlySettings;
        public static CustomNumberOption NeutralRoles;
        public static CustomToggleOption AddArsonist;
        public static CustomToggleOption AddCryomaniac;
        public static CustomToggleOption AddPlaguebearer;

        //Crew Options
        public static CustomHeaderOption CrewSettings;
        public static CustomToggleOption CustomCrewColors;
        public static CustomToggleOption CrewVent;
        public static CustomNumberOption ShortTasks;
        public static CustomNumberOption LongTasks;
        public static CustomNumberOption CommonTasks;
        public static CustomNumberOption CrewVision;
        public static CustomToggleOption GhostTasksCountToWin;
        public static CustomNumberOption CrewMax;
        public static CustomNumberOption CrewMin;

        //CSv Options
        public static CustomHeaderOption CrewSovereignSettings;
        public static CustomNumberOption CSvMax;
        public static CustomNumberOption CSvMin;

        //Mayor Options
        public static CustomHeaderOption Mayor;
        public static CustomNumberOption MayorCount;
        public static CustomToggleOption UniqueMayor;
        public static CustomNumberOption MayorVoteBank;
        public static CustomToggleOption MayorAnonymous;
        public static CustomToggleOption MayorButton;

        //Swapper Options
        public static CustomHeaderOption Swapper;
        public static CustomNumberOption SwapperCount;
        public static CustomToggleOption UniqueSwapper;
        public static CustomToggleOption SwapperButton;
        public static CustomToggleOption SwapAfterVoting;
        public static CustomToggleOption SwapSelf;

        //CA Options
        public static CustomHeaderOption CrewAuditorSettings;
        public static CustomNumberOption CAMax;
        public static CustomNumberOption CAMin;

        //Mystic Options
        public static CustomHeaderOption Mystic;
        public static CustomToggleOption UniqueMystic;
        public static CustomNumberOption MysticCount;
        public static CustomNumberOption RevealCooldown;

        //Vampire Hunter Options
        public static CustomHeaderOption VampireHunter;
        public static CustomToggleOption UniqueVampireHunter;
        public static CustomNumberOption VampireHunterCount;
        public static CustomNumberOption StakeCooldown;

        //CK Options
        public static CustomHeaderOption CrewKillingSettings;
        public static CustomNumberOption CKMax;
        public static CustomNumberOption CKMin;

        //Vigilante Options
        public static CustomHeaderOption Vigilante;
        public static CustomNumberOption VigilanteCount;
        public static CustomToggleOption UniqueVigilante;
        public static CustomStringOption VigiOptions;
        public static CustomStringOption VigiNotifOptions;
        public static CustomToggleOption MisfireKillsInno;
        public static CustomToggleOption VigiKillAgain;
        public static CustomToggleOption RoundOneNoShot;
        public static CustomNumberOption VigiKillCd;

        //Veteran Options
        public static CustomHeaderOption Veteran;
        public static CustomNumberOption VeteranCount;
        public static CustomToggleOption UniqueVeteran;
        public static CustomNumberOption AlertCooldown;
        public static CustomNumberOption AlertDuration;
        public static CustomNumberOption MaxAlerts;

        //CS Options
        public static CustomHeaderOption CrewSupportSettings;
        public static CustomNumberOption CSMax;
        public static CustomNumberOption CSMin;

        //Engineer Options
        public static CustomHeaderOption Engineer;
        public static CustomNumberOption EngineerCount;
        public static CustomNumberOption MaxFixes;
        public static CustomToggleOption UniqueEngineer;

        //Transporter Options
        public static CustomHeaderOption Transporter;
        public static CustomNumberOption TransporterCount;
        public static CustomToggleOption UniqueTransporter;
        public static CustomNumberOption TransportCooldown;
        public static CustomNumberOption TransportMaxUses;

        //Retributionist Options
        public static CustomHeaderOption Retributionist;
        public static CustomToggleOption UniqueRetributionist;
        public static CustomNumberOption RetributionistCount;

        //Escort Options
        public static CustomHeaderOption Escort;
        public static CustomNumberOption EscortCount;
        public static CustomToggleOption UniqueEscort;
        public static CustomNumberOption EscRoleblockCooldown;
        public static CustomNumberOption EscRoleblockDuration;

        //Chameleon Options
        public static CustomHeaderOption Chameleon;
        public static CustomNumberOption ChameleonCount;
        public static CustomToggleOption UniqueChameleon;
        public static CustomNumberOption SwoopCooldown;
        public static CustomNumberOption SwoopDuration;

        //Shifter Options
        public static CustomHeaderOption Shifter;
        public static CustomNumberOption ShifterCount;
        public static CustomToggleOption UniqueShifter;
        public static CustomNumberOption ShifterCd;
        public static CustomStringOption ShiftedBecomes;

        //CI Options
        public static CustomHeaderOption CrewInvestigativeSettings;
        public static CustomNumberOption CIMax;
        public static CustomNumberOption CIMin;

        //Tracker Options
        public static CustomHeaderOption Tracker;
        public static CustomNumberOption TrackerCount;
        public static CustomToggleOption UniqueTracker;
        public static CustomNumberOption UpdateInterval;
        public static CustomNumberOption TrackCooldown;
        public static CustomToggleOption ResetOnNewRound;
        public static CustomNumberOption MaxTracks;

        //Operative Options
        public static CustomHeaderOption Operative;
        public static CustomNumberOption OperativeCount;
        public static CustomToggleOption UniqueOperative;
        public static CustomNumberOption BugCooldown;
        public static CustomToggleOption BugsRemoveOnNewRound;
        public static CustomNumberOption MaxBugs;
        public static CustomNumberOption MinAmountOfTimeInBug;
        public static CustomNumberOption BugRange;
        public static CustomNumberOption MinAmountOfPlayersInBug;

        //Seer Options
        public static CustomHeaderOption Seer;
        public static CustomToggleOption UniqueSeer;
        public static CustomNumberOption SeerCount;
        public static CustomNumberOption SeerCooldown;

        //Detective Options
        public static CustomHeaderOption Detective;
        public static CustomNumberOption DetectiveCount;
        public static CustomToggleOption UniqueDetective;
        public static CustomNumberOption ExamineCooldown;
        public static CustomNumberOption RecentKill;
        public static CustomNumberOption FootprintSize;
        public static CustomNumberOption FootprintInterval;
        public static CustomNumberOption FootprintDuration;
        public static CustomToggleOption AnonymousFootPrint;
        public static CustomToggleOption VentFootprintVisible;

        //Coroner Options
        public static CustomHeaderOption Coroner;
        public static CustomNumberOption CoronerCount;
        public static CustomToggleOption UniqueCoroner;
        public static CustomNumberOption CoronerArrowDuration;
        public static CustomToggleOption CoronerReportName;
        public static CustomToggleOption CoronerReportRole;
        public static CustomNumberOption CoronerKillerNameTime;

        //Inspector Options
        public static CustomHeaderOption Inspector;
        public static CustomNumberOption InspectorCount;
        public static CustomToggleOption UniqueInspector;
        public static CustomNumberOption InspectCooldown;

        //Medium Options
        public static CustomHeaderOption Medium;
        public static CustomNumberOption MediumCount;
        public static CustomToggleOption UniqueMedium;
        public static CustomNumberOption MediateCooldown;
        public static CustomNumberOption MediateDuration;
        public static CustomToggleOption ShowMediatePlayer;
        public static CustomToggleOption ShowMediumToDead;
        public static CustomStringOption DeadRevealed;

        //Sheriff Options
        public static CustomHeaderOption Sheriff;
        public static CustomNumberOption SheriffCount;
        public static CustomToggleOption UniqueSheriff;
        public static CustomNumberOption InterrogateCooldown;
        public static CustomToggleOption NeutEvilRed;
        public static CustomToggleOption NeutKillingRed;

        //Agent Options
        public static CustomHeaderOption Agent;
        public static CustomToggleOption UniqueAgent;
        public static CustomNumberOption AgentCount;
        
        //CP Options
        public static CustomHeaderOption CrewProtectiveSettings;
        public static CustomNumberOption CPMax;
        public static CustomNumberOption CPMin;

        //Time Lord Options
        public static CustomHeaderOption TimeLord;
        public static CustomNumberOption TimeLordCount;
        public static CustomToggleOption UniqueTimeLord;
        public static CustomToggleOption RewindRevive;
        public static CustomToggleOption TLImmunity;
        public static CustomNumberOption RewindDuration;
        public static CustomNumberOption RewindCooldown;
        public static CustomNumberOption RewindMaxUses;

        //Altruist Options
        public static CustomHeaderOption Altruist;
        public static CustomNumberOption AltruistCount;
        public static CustomToggleOption UniqueAltruist;
        public static CustomNumberOption AltReviveDuration;
        public static CustomToggleOption AltruistTargetBody;

        //Medic Options
        public static CustomHeaderOption Medic;
        public static CustomNumberOption MedicCount;
        public static CustomToggleOption UniqueMedic;
        public static CustomStringOption ShowShielded;
        public static CustomStringOption WhoGetsNotification;
        public static CustomToggleOption ShieldBreaks;

        //CU Options
        public static CustomHeaderOption CrewUtilitySettings;

        //Crewmate Options
        public static CustomHeaderOption Crewmate;
        public static CustomNumberOption CrewCount;

        //Revealer Options
        public static CustomHeaderOption Revealer;
        public static CustomNumberOption RevealerCount;
        public static CustomToggleOption RevealerKnows;
        public static CustomNumberOption RevealerTasksRemainingClicked;
        public static CustomNumberOption RevealerTasksRemainingAlert;
        public static CustomToggleOption RevealerRevealsNeutrals;
        public static CustomToggleOption RevealerRevealsCrew;
        public static CustomToggleOption RevealerRevealsRoles;
        public static CustomStringOption RevealerCanBeClickedBy;

        //Intruder Options
        public static CustomHeaderOption IntruderSettings;
        public static CustomToggleOption CustomIntColors;
        public static CustomNumberOption IntruderVision;
        public static CustomToggleOption IntrudersVent;
        public static CustomToggleOption IntrudersCanSabotage;
        public static CustomNumberOption IntruderKillCooldown;
        public static CustomNumberOption IntruderSabotageCooldown;
        public static CustomNumberOption IntruderCount;
        public static CustomNumberOption IntruderMax;
        public static CustomNumberOption IntruderMin;

        //IC Options
        public static CustomHeaderOption IntruderConcealingSettings;
        public static CustomNumberOption ICMax;
        public static CustomNumberOption ICMin;

        //Janitor Options
        public static CustomHeaderOption Janitor;
        public static CustomNumberOption JanitorCount;
        public static CustomToggleOption UniqueJanitor;
        public static CustomNumberOption JanitorCleanCd;
        public static CustomToggleOption JaniCooldownsLinked;
        public static CustomToggleOption SoloBoost;

        //Blackmailer Options
        public static CustomHeaderOption Blackmailer;
        public static CustomNumberOption BlackmailerCount;
        public static CustomToggleOption UniqueBlackmailer;
        public static CustomNumberOption BlackmailCooldown;

        //Grenadier Options
        public static CustomHeaderOption Grenadier;
        public static CustomNumberOption GrenadierCount;
        public static CustomToggleOption UniqueGrenadier;
        public static CustomNumberOption GrenadeCooldown;
        public static CustomNumberOption GrenadeDuration;
        public static CustomToggleOption GrenadierIndicators;
        public static CustomToggleOption GrenadierVent;
        public static CustomNumberOption FlashRadius;

        //Camouflager Options
        public static CustomHeaderOption Camouflager;
        public static CustomNumberOption CamouflagerCount;
        public static CustomToggleOption UniqueCamouflager;
        public static CustomNumberOption CamouflagerCooldown;
        public static CustomNumberOption CamouflagerDuration;
        public static CustomToggleOption CamoHideSpeed;
        public static CustomToggleOption CamoHideSize;

        //Undertaker Options
        public static CustomHeaderOption Undertaker;
        public static CustomNumberOption UndertakerCount;
        public static CustomToggleOption UniqueUndertaker;
        public static CustomNumberOption DragCooldown;
        public static CustomStringOption UndertakerVentOptions;
        public static CustomNumberOption DragModifier;

        //ID Options
        public static CustomHeaderOption IntruderDeceptionSettings;
        public static CustomNumberOption IDMax;
        public static CustomNumberOption IDMin;

        //Morphling Options
        public static CustomHeaderOption Morphling;
        public static CustomNumberOption MorphlingCount;
        public static CustomToggleOption UniqueMorphling;
        public static CustomNumberOption MorphlingCooldown;
        public static CustomNumberOption MorphlingDuration;
        public static CustomToggleOption MorphlingVent;
        
        //Disguiser Options
        public static CustomHeaderOption Disguiser;
        public static CustomNumberOption DisguiserCount;
        public static CustomToggleOption UniqueDisguiser;
        public static CustomNumberOption DisguiseCooldown;
        public static CustomNumberOption TimeToDisguise;
        public static CustomNumberOption DisguiseDuration;
        public static CustomStringOption DisguiseTarget;

        //Wraith Options
        public static CustomHeaderOption Wraith;
        public static CustomNumberOption WraithCount;
        public static CustomToggleOption UniqueWraith;
        public static CustomNumberOption InvisCooldown;
        public static CustomNumberOption InvisDuration;
        public static CustomToggleOption WraithVent;

        //Poisoner Options
        public static CustomHeaderOption Poisoner;
        public static CustomNumberOption PoisonerCount;
        public static CustomToggleOption UniquePoisoner;
        public static CustomNumberOption PoisonCooldown;
        public static CustomNumberOption PoisonDuration;

        //IS Options
        public static CustomHeaderOption IntruderSupportSettings;
        public static CustomNumberOption ISMax;
        public static CustomNumberOption ISMin;

        //Teleporter Options
        public static CustomHeaderOption Teleporter;
        public static CustomNumberOption TeleporterCount;
        public static CustomToggleOption UniqueTeleporter;
        public static CustomNumberOption TeleportCd;
        public static CustomToggleOption TeleVent;

        //Consigliere Options
        public static CustomHeaderOption Consigliere;
        public static CustomNumberOption ConsigCount;
        public static CustomToggleOption UniqueConsigliere;
        public static CustomNumberOption InvestigateCooldown;
        public static CustomStringOption ConsigInfo;

        //Time Master Options
        public static CustomHeaderOption TimeMaster;
        public static CustomNumberOption TimeMasterCount;
        public static CustomToggleOption UniqueTimeMaster;
        public static CustomToggleOption TMImmunity;
        public static CustomToggleOption IntruderImmunity;
        public static CustomNumberOption FreezeDuration;
        public static CustomNumberOption FreezeCooldown;

        //Consort Options
        public static CustomHeaderOption Consort;
        public static CustomNumberOption ConsortCount;
        public static CustomToggleOption UniqueConsort;
        public static CustomNumberOption ConsRoleblockCooldown;
        public static CustomNumberOption ConsRoleblockDuration;

        //Godfather Options
        public static CustomHeaderOption Godfather;
        public static CustomNumberOption GodfatherCount;
        public static CustomToggleOption UniqueGodfather;
        public static CustomNumberOption MafiosoAbilityCooldownDecrease;
        public static CustomToggleOption PromotedMafiosoCanPromote;

        //Miner Options
        public static CustomHeaderOption Miner;
        public static CustomNumberOption MinerCount;
        public static CustomToggleOption UniqueMiner;
        public static CustomNumberOption MineCooldown;

        //IK Options
        public static CustomHeaderOption IntruderKillingSettings;
        public static CustomNumberOption IKMax;
        public static CustomNumberOption IKMin;

        //IU Options
        public static CustomHeaderOption IntruderUtilitySettings;

        //Impostor Options
        public static CustomHeaderOption Impostor;
        public static CustomNumberOption ImpCount;

        //Syndicate Options
        public static CustomHeaderOption SyndicateSettings;
        public static CustomToggleOption CustomSynColors;
        public static CustomNumberOption SyndicateVision;
        public static CustomStringOption SyndicateVent;
        public static CustomNumberOption ChaosDriveMeetingCount;
        public static CustomNumberOption ChaosDriveKillCooldown;
        public static CustomNumberOption SyndicateCount;
        public static CustomToggleOption AltImps;
        public static CustomNumberOption SyndicateMax;
        public static CustomNumberOption SyndicateMin;

        //SD Options
        public static CustomHeaderOption SyndicateDisruptionSettings;
        public static CustomNumberOption SDMax;
        public static CustomNumberOption SDMin;

        //Shapeshifter Options
        public static CustomHeaderOption Shapeshifter;
        public static CustomNumberOption ShapeshifterCount;
        public static CustomToggleOption UniqueShapeshifter;
        public static CustomNumberOption ShapeshiftCooldown;
        public static CustomNumberOption ShapeshiftDuration;

        //Drunkard Options
        public static CustomHeaderOption Drunkard;
        public static CustomNumberOption DrunkardCount;
        public static CustomToggleOption UniqueDrunkard;
        public static CustomNumberOption ConfuseCooldown;
        public static CustomNumberOption ConfuseDuration;
        public static CustomToggleOption SyndicateImmunity;

        //Puppeteer Options
        public static CustomHeaderOption Framer;
        public static CustomNumberOption FrameCooldown;
        public static CustomNumberOption FramerCount;
        public static CustomToggleOption UniqueFramer;
        
        //SyK Options
        public static CustomHeaderOption SyndicateKillingSettings;
        public static CustomNumberOption SyKMax;
        public static CustomNumberOption SyKMin;

        //Gorgon Options
        public static CustomHeaderOption Gorgon;
        public static CustomNumberOption GorgonCount;
        public static CustomToggleOption UniqueGorgon;
        public static CustomNumberOption GazeTime;
        public static CustomNumberOption GazeDelay;
        public static CustomNumberOption GazeCooldown;

        //Bomber Options
        public static CustomHeaderOption Bomber;
        public static CustomNumberOption BomberCount;
        public static CustomToggleOption UniqueBomber;
        public static CustomNumberOption BombCooldown;
        public static CustomNumberOption DetonateCooldown;
        public static CustomToggleOption BombCooldownsLinked;
        public static CustomToggleOption BombsRemoveOnNewRound;
        public static CustomToggleOption BombsDetonateOnMeetingStart;
        public static CustomNumberOption BombRange;

        //SSu Options
        public static CustomHeaderOption SyndicateSupportSettings;
        public static CustomNumberOption SSuMax;
        public static CustomNumberOption SSuMin;

        //Beamer Options
        public static CustomHeaderOption Beamer;
        public static CustomNumberOption BeamerCount;
        public static CustomToggleOption UniqueBeamer;
        public static CustomNumberOption BeamCooldown;

        //Concealer Options
        public static CustomHeaderOption Concealer;
        public static CustomNumberOption ConcealerCount;
        public static CustomToggleOption UniqueConcealer;
        public static CustomNumberOption ConcealCooldown;
        public static CustomNumberOption ConcealDuration;

        //Rebel Options
        public static CustomHeaderOption Rebel;
        public static CustomNumberOption RebelCount;
        public static CustomToggleOption UniqueRebel;
        public static CustomNumberOption SidekickAbilityCooldownDecrease;
        public static CustomToggleOption PromotedSidekickCanPromote;

        //Warper Options
        public static CustomHeaderOption Warper;
        public static CustomNumberOption WarperCount;
        public static CustomNumberOption WarpCooldown;
        public static CustomToggleOption UniqueWarper;

        //SU Options
        public static CustomHeaderOption SyndicateUtilitySettings;

        //Anarchist Options
        public static CustomHeaderOption Anarchist;
        public static CustomNumberOption AnarchistCount;

        //SP Options
        public static CustomHeaderOption SyndicatePowerSettings;
        public static CustomNumberOption SPMax;
        public static CustomNumberOption SPMin;

        //Neutral Options
        public static CustomHeaderOption NeutralSettings;
        public static CustomToggleOption CustomNeutColors;
        public static CustomNumberOption NeutralVision;
        public static CustomToggleOption LightsAffectNeutrals;
        public static CustomStringOption NoSolo;
        public static CustomNumberOption NeutralMax;
        public static CustomNumberOption NeutralMin;

        //NB Options
        public static CustomHeaderOption NeutralBenignSettings;
        public static CustomNumberOption NBMax;
        public static CustomNumberOption NBMin;
        public static CustomToggleOption VigiKillsNB;

        //Amnesiac Options
        public static CustomHeaderOption Amnesiac;
        public static CustomNumberOption AmnesiacCount;
        public static CustomToggleOption RememberArrows;
        public static CustomNumberOption RememberArrowDelay;
        public static CustomToggleOption AmneVent;
        public static CustomToggleOption AmneSwitchVent;
        public static CustomToggleOption AmneTurnAssassin;
        public static CustomToggleOption UniqueAmnesiac;

        //Survivor Options
        public static CustomHeaderOption Survivor;
        public static CustomNumberOption SurvivorCount;
        public static CustomNumberOption VestCd;
        public static CustomNumberOption VestDuration;
        public static CustomNumberOption VestKCReset;
        public static CustomToggleOption SurvVent;
        public static CustomToggleOption SurvSwitchVent;
        public static CustomNumberOption MaxVests;
        public static CustomToggleOption UniqueSurvivor;

        //Guardian Angel Options
        public static CustomHeaderOption GuardianAngel;
        public static CustomNumberOption GuardianAngelCount;
        public static CustomNumberOption ProtectCd;
        public static CustomNumberOption ProtectDuration;
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

        //Thief Options
        public static CustomHeaderOption Thief;
        public static CustomNumberOption ThiefCount;
        public static CustomToggleOption ThiefVent;
        public static CustomNumberOption ThiefKillCooldown;
        public static CustomToggleOption UniqueThief;
        public static CustomToggleOption ThiefSteals;

        //NE Options
        public static CustomHeaderOption NeutralEvilSettings;
        public static CustomNumberOption NEMax;
        public static CustomNumberOption NEMin;

        //Jester Options
        public static CustomHeaderOption Jester;
        public static CustomNumberOption JesterCount;
        public static CustomToggleOption JesterButton;
        public static CustomToggleOption JesterVent;
        public static CustomToggleOption JestSwitchVent;
        public static CustomToggleOption JestEjectScreen;
        public static CustomToggleOption VigiKillsJester;
        public static CustomToggleOption UniqueJester;

        //Actor Options
        public static CustomHeaderOption Actor;
        public static CustomNumberOption ActorCount;
        public static CustomToggleOption ActorButton;
        public static CustomToggleOption ActorVent;
        public static CustomToggleOption ActSwitchVent;
        public static CustomToggleOption VigiKillsActor;
        public static CustomToggleOption UniqueActor;

        //Troll Options
        public static CustomHeaderOption Troll;
        public static CustomNumberOption TrollCount;
        public static CustomNumberOption InteractCooldown;
        public static CustomToggleOption TrollVent;
        public static CustomToggleOption TrollSwitchVent;
        public static CustomToggleOption UniqueTroll;

        //Cannibal Options
        public static CustomHeaderOption Cannibal;
        public static CustomNumberOption CannibalCount;
        public static CustomNumberOption CannibalCd;
        public static CustomNumberOption CannibalBodyCount;
        public static CustomToggleOption CannibalVent;
        public static CustomToggleOption EatArrows;
        public static CustomNumberOption EatArrowDelay;
        public static CustomToggleOption VigiKillsCannibal;
        public static CustomToggleOption UniqueCannibal;

        //Executioner Options
        public static CustomHeaderOption Executioner;
        public static CustomNumberOption ExecutionerCount;
        public static CustomStringOption OnTargetDead;
        public static CustomToggleOption ExecutionerButton;
        public static CustomToggleOption ExeVent;
        public static CustomToggleOption ExeSwitchVent;
        public static CustomToggleOption ExeTargetKnows;
        public static CustomToggleOption ExeKnowsTargetRole;
        public static CustomToggleOption ExeCanHaveIntruderTargets;
        public static CustomToggleOption ExeCanHaveSyndicateTargets;
        public static CustomToggleOption ExeCanHaveNeutralTargets;
        public static CustomToggleOption ExeEjectScreen;
        public static CustomToggleOption ExeCanWinBeyondDeath;
        public static CustomToggleOption VigiKillsExecutioner;
        public static CustomToggleOption UniqueExecutioner;

        //Bounty Hunter Options
        public static CustomHeaderOption BountyHunter;
        public static CustomNumberOption BHCount;
        public static CustomToggleOption BHVent;
        public static CustomNumberOption BountyHunterCooldown;
        public static CustomNumberOption BountyHunterGuesses;
        public static CustomToggleOption UniqueBountyHunter;

        //Guesser Options
        public static CustomHeaderOption Guesser;
        public static CustomNumberOption GuesserCount;
        public static CustomStringOption OnTargetGone;
        public static CustomToggleOption GuesserButton;
        public static CustomToggleOption GuessVent;
        public static CustomToggleOption GuessSwitchVent;
        public static CustomToggleOption GuessTargetKnows;
        public static CustomToggleOption VigiKillsGuesser;
        public static CustomToggleOption UniqueGuesser;
        public static CustomNumberOption GuessCount;
        public static CustomToggleOption GuesserAfterVoting;
        public static CustomToggleOption MultipleGuesses;

        //NK Options
        public static CustomHeaderOption NeutralKillingSettings;
        public static CustomNumberOption NKMax;
        public static CustomNumberOption NKMin;
        public static CustomToggleOption NKHasImpVision;
        public static CustomStringOption NKsKnow;

        //Glitch Options
        public static CustomHeaderOption Glitch;
        public static CustomNumberOption GlitchCount;
        public static CustomNumberOption HackCooldown;
        public static CustomNumberOption MimicCooldown;
        public static CustomNumberOption MimicDuration;
        public static CustomNumberOption HackDuration;
        public static CustomNumberOption GlitchKillCooldown;
        public static CustomToggleOption GlitchVent;
        public static CustomToggleOption UniqueGlitch;

        //Juggernaut Options
        public static CustomHeaderOption Juggernaut;
        public static CustomNumberOption JuggernautCount;
        public static CustomToggleOption JuggVent;
        public static CustomNumberOption JuggKillCooldown;
        public static CustomNumberOption JuggKillBonus;
        public static CustomToggleOption UniqueJuggernaut;

        //Cryomaniac Options
        public static CustomHeaderOption Cryomaniac;
        public static CustomNumberOption CryomaniacCount;
        public static CustomNumberOption CryoDouseCooldown;
        public static CustomToggleOption CryoVent;
        public static CustomToggleOption UniqueCryomaniac;

        //Plaguebearer Options
        public static CustomHeaderOption Plaguebearer;
        public static CustomNumberOption PlaguebearerCount;
        public static CustomNumberOption InfectCooldown;
        public static CustomToggleOption PBVent;
        public static CustomToggleOption UniquePlaguebearer;

        //Arsonist Options
        public static CustomHeaderOption Arsonist;
        public static CustomNumberOption ArsonistCount;
        public static CustomNumberOption DouseCooldown;
        public static CustomNumberOption IgniteCooldown;
        public static CustomToggleOption ArsoVent;
        public static CustomToggleOption ArsoLastKillerBoost;
        public static CustomToggleOption ArsoCooldownsLinked;
        public static CustomToggleOption UniqueArsonist;

        //Murderer Options
        public static CustomHeaderOption Murderer;
        public static CustomNumberOption MurdCount;
        public static CustomToggleOption MurdVent;
        public static CustomNumberOption MurdKillCooldownOption;
        public static CustomToggleOption UniqueMurderer;

        //Serial Killer Options
        public static CustomHeaderOption SerialKiller;
        public static CustomNumberOption SKCount;
        public static CustomNumberOption BloodlustCooldown;
        public static CustomNumberOption BloodlustDuration;
        public static CustomNumberOption LustKillCooldown;
        public static CustomStringOption SKVentOptions;
        public static CustomToggleOption UniqueSerialKiller;

        //Werewolf Options
        public static CustomHeaderOption Werewolf;
        public static CustomNumberOption WerewolfCount;
        public static CustomNumberOption MaulCooldown;
        public static CustomNumberOption MaulRadius;
        public static CustomToggleOption WerewolfVent;
        public static CustomToggleOption UniqueWerewolf;

        //NN Options
        public static CustomHeaderOption NeutralNeophyteSettings;
        public static CustomNumberOption NNMax;
        public static CustomNumberOption NNMin;

        //Dracula Options
        public static CustomHeaderOption Dracula;
        public static CustomNumberOption DraculaCount;
        public static CustomNumberOption BiteCooldown;
        public static CustomToggleOption DraculaConvertNeuts;
        public static CustomNumberOption AliveVampCount;
        public static CustomToggleOption DracVent;
        public static CustomToggleOption UniqueDracula;

        //Necromancer Options
        public static CustomHeaderOption Necromancer;
        public static CustomNumberOption NecromancerCount;
        public static CustomNumberOption ResurrectCooldown;
        public static CustomNumberOption NecroKillCooldown;
        public static CustomNumberOption NecroKillCooldownIncrease;
        public static CustomNumberOption NecroKillCount;
        public static CustomNumberOption ResurrectCooldownIncrease;
        public static CustomNumberOption ResurrectCount;
        public static CustomToggleOption NecroVent;
        public static CustomToggleOption KillResurrectCooldownsLinked;
        public static CustomToggleOption NecromancerTargetBody;
        public static CustomNumberOption NecroResurrectDuration;
        public static CustomToggleOption UniqueNecromancer;

        //Whisperer Options
        public static CustomHeaderOption Whisperer;
        public static CustomNumberOption WhispererCount;
        public static CustomNumberOption WhisperCooldown;
        public static CustomNumberOption WhisperRadius;
        public static CustomNumberOption WhisperCooldownIncrease;
        public static CustomNumberOption WhisperRateDecrease;
        public static CustomNumberOption InitialWhisperRate;
        public static CustomToggleOption WhisperRateDecreases;
        public static CustomToggleOption WhispVent;
        public static CustomToggleOption UniqueWhisperer;

        //Jackal Options
        public static CustomHeaderOption Jackal;
        public static CustomNumberOption JackalCount;
        public static CustomNumberOption RecruitCooldown;
        public static CustomToggleOption JackalVent;
        public static CustomToggleOption RecruitVent;
        public static CustomToggleOption UniqueJackal;

        //NP Options
        public static CustomHeaderOption NeutralProselyteSettings;

        //Phantom Options
        public static CustomHeaderOption Phantom;
        public static CustomNumberOption PhantomCount;
        public static CustomNumberOption PhantomTasksRemaining;
        public static CustomToggleOption PhantomPlayersAlerted;

        //Vampire Options
        public static CustomHeaderOption Vampire;
        public static CustomToggleOption VampVent;

        //Dampyr Options
        public static CustomHeaderOption Dampyr;
        public static CustomNumberOption DampBiteCooldown;
        public static CustomToggleOption DampVent;

        //Pestilence Options
        public static CustomHeaderOption Pestilence;
        public static CustomToggleOption PestSpawn;
        public static CustomToggleOption PlayersAlerted;
        public static CustomNumberOption PestKillCooldown;
        public static CustomToggleOption PestVent;

        //Ability Options
        public static CustomHeaderOption AbilitySettings;
        public static CustomToggleOption CustomAbilityColors;
        public static CustomNumberOption MaxAbilities;
        public static CustomNumberOption MinAbilities;

        //Snitch Options
        public static CustomHeaderOption Snitch;
        public static CustomNumberOption SnitchCount;
        public static CustomToggleOption SnitchSeesNeutrals;
        public static CustomToggleOption SnitchSeesCrew;
        public static CustomToggleOption SnitchSeesRoles;
        public static CustomNumberOption SnitchTasksRemaining;
        public static CustomToggleOption SnitchSeesImpInMeeting;
        public static CustomToggleOption SnitchKnows;
        public static CustomToggleOption UniqueSnitch;

        //Assassin Options
        public static CustomHeaderOption Assassin;
        public static CustomNumberOption NumberOfImpostorAssassins;
        public static CustomNumberOption NumberOfNeutralAssassins;
        public static CustomNumberOption NumberOfCrewAssassins;
        public static CustomNumberOption NumberOfSyndicateAssassins;
        public static CustomNumberOption AssassinKills;
        public static CustomToggleOption AssassinMultiKill;
        public static CustomToggleOption AssassinGuessNeutralBenign;
        public static CustomToggleOption AssassinGuessNeutralEvil;
        public static CustomToggleOption AssassinGuessPest;
        public static CustomToggleOption AssassinGuessModifiers;
        public static CustomToggleOption AssassinGuessObjectifiers;
        public static CustomToggleOption AssassinGuessAbilities;
        public static CustomToggleOption AssassinateAfterVoting;
        public static CustomToggleOption UniqueAssassin;

        //Underdog Options
        public static CustomHeaderOption Underdog;
        public static CustomNumberOption UnderdogCount;
        public static CustomToggleOption UniqueUnderdog;
        public static CustomToggleOption UnderdogKnows;
        public static CustomNumberOption UnderdogKillBonus;
        public static CustomToggleOption UnderdogIncreasedKC;

        //Multitasker Options
        public static CustomHeaderOption Multitasker;
        public static CustomToggleOption UniqueMultitasker;
        public static CustomNumberOption MultitaskerCount;
        public static CustomNumberOption Transparancy;

        //Button Barry Options
        public static CustomHeaderOption ButtonBarry;
        public static CustomToggleOption UniqueButtonBarry;
        public static CustomNumberOption ButtonBarryCount;
        public static CustomNumberOption ButtonCooldown;

        //Tiebreaker Options
        public static CustomHeaderOption Tiebreaker;
        public static CustomToggleOption UniqueTiebreaker;
        public static CustomToggleOption TiebreakerKnows;
        public static CustomNumberOption TiebreakerCount;

        //Torch Options
        public static CustomHeaderOption Torch;
        public static CustomNumberOption TorchCount;
        public static CustomToggleOption UniqueTorch;

        //Tunneler Options
        public static CustomHeaderOption Tunneler;
        public static CustomToggleOption TunnelerKnows;
        public static CustomNumberOption TunnelerCount;
        public static CustomToggleOption UniqueTunneler;

        //Radar Options
        public static CustomHeaderOption Radar;
        public static CustomNumberOption RadarCount;
        public static CustomToggleOption UniqueRadar;

        //Lighter Options
        public static CustomHeaderOption Lighter;
        public static CustomNumberOption LighterCount;
        public static CustomToggleOption UniqueLighter;

        //Insider Options
        public static CustomHeaderOption Insider;
        public static CustomToggleOption InsiderKnows;
        public static CustomNumberOption InsiderCount;
        public static CustomToggleOption UniqueInsider;

        //Objectifier Options
        public static CustomHeaderOption ObjectifierSettings;
        public static CustomToggleOption CustomObjectifierColors;
        public static CustomNumberOption MaxObjectifiers;
        public static CustomNumberOption MinObjectifiers;

        //Traitor Options
        public static CustomHeaderOption Traitor;
        public static CustomNumberOption TraitorCount;
        public static CustomToggleOption UniqueTraitor;
        public static CustomToggleOption TraitorKnows;
        public static CustomToggleOption TraitorCanAssassin;
        public static CustomToggleOption TraitorColourSwap;
        public static CustomToggleOption SnitchSeesTraitor;
        public static CustomToggleOption RevealerRevealsTraitor;

        //Fanatic Options
        public static CustomHeaderOption Fanatic;
        public static CustomNumberOption FanaticCount;
        public static CustomToggleOption FanaticKnows;
        public static CustomToggleOption FanaticCanAssassin;
        public static CustomToggleOption UniqueFanatic;
        public static CustomToggleOption FanaticColourSwap;
        public static CustomToggleOption SnitchSeesFanatic;
        public static CustomToggleOption RevealerRevealsFanatic;

        //Allied Options
        public static CustomHeaderOption Allied;
        public static CustomNumberOption AlliedCount;
        public static CustomStringOption AlliedFaction;
        public static CustomToggleOption UniqueAllied;

        //Corrupted Options
        public static CustomHeaderOption Corrupted;
        public static CustomNumberOption CorruptedCount;
        public static CustomNumberOption CorruptedKillCooldown;
        public static CustomToggleOption UniqueCorrupted;

        //Corrupted Options
        public static CustomHeaderOption Overlord;
        public static CustomNumberOption OverlordCount;
        public static CustomNumberOption OverlordMeetingWinCount;
        public static CustomToggleOption UniqueOverlord;

        //Lovers Options
        public static CustomHeaderOption Lovers;
        public static CustomNumberOption LoversCount;
        public static CustomToggleOption BothLoversDie;
        public static CustomToggleOption LoversChat;
        public static CustomToggleOption LoversFaction;
        public static CustomToggleOption LoversRoles;
        public static CustomToggleOption UniqueLovers;

        //Rivals Options
        public static CustomHeaderOption Rivals;
        public static CustomNumberOption RivalsCount;
        public static CustomToggleOption RivalsChat;
        public static CustomToggleOption RivalsFaction;
        public static CustomToggleOption RivalsRoles;
        public static CustomToggleOption UniqueRivals;

        //Taskmaster Options
        public static CustomHeaderOption Taskmaster;
        public static CustomNumberOption TaskmasterCount;
        public static CustomNumberOption TMTasksRemaining;
        public static CustomToggleOption UniqueTaskmaster;

        //Modifier Options
        public static CustomHeaderOption ModifierSettings;
        public static CustomToggleOption CustomModifierColors;
        public static CustomNumberOption MaxModifiers;
        public static CustomNumberOption MinModifiers;

        //Giant Options
        public static CustomHeaderOption Giant;
        public static CustomToggleOption UniqueGiant;
        public static CustomNumberOption GiantCount;
        public static CustomNumberOption GiantSpeed;
        public static CustomNumberOption GiantScale;

        //Dwarf Options
        public static CustomHeaderOption Dwarf;
        public static CustomNumberOption DwarfCount;
        public static CustomNumberOption DwarfSpeed;
        public static CustomNumberOption DwarfScale;
        public static CustomToggleOption UniqueDwarf;

        //Diseased Options
        public static CustomHeaderOption Diseased;
        public static CustomNumberOption DiseasedCount;
        public static CustomNumberOption DiseasedKillMultiplier;
        public static CustomToggleOption DiseasedKnows;
        public static CustomToggleOption UniqueDiseased;

        //Bait Options
        public static CustomHeaderOption Bait;
        public static CustomNumberOption BaitCount;
        public static CustomNumberOption BaitMinDelay;
        public static CustomNumberOption BaitMaxDelay;
        public static CustomToggleOption BaitKnows;
        public static CustomToggleOption UniqueBait;

        //Drunk Options
        public static CustomHeaderOption Drunk;
        public static CustomNumberOption DrunkCount;
        public static CustomToggleOption DrunkControlsSwap;
        public static CustomNumberOption DrunkInterval;
        public static CustomToggleOption UniqueDrunk;

        //Coward Options
        public static CustomHeaderOption Coward;
        public static CustomNumberOption CowardCount;
        public static CustomToggleOption UniqueCoward;

        //Ruthless Options
        public static CustomHeaderOption Ruthless;
        public static CustomNumberOption RuthlessCount;
        public static CustomToggleOption UniqueRuthless;

        //Professional Options
        public static CustomHeaderOption Professional;
        public static CustomToggleOption ProfessionalKnows;
        public static CustomNumberOption ProfessionalCount;
        public static CustomToggleOption UniqueProfessional;

        //Shy Options
        public static CustomHeaderOption Shy;
        public static CustomNumberOption ShyCount;
        public static CustomToggleOption UniqueShy;

        //Indomitable Options
        public static CustomHeaderOption Indomitable;
        public static CustomNumberOption IndomitableCount;
        public static CustomToggleOption UniqueIndomitable;

        //VIP Options
        public static CustomHeaderOption VIP;
        public static CustomToggleOption UniqueVIP;
        public static CustomNumberOption VIPCount;
        public static CustomToggleOption VIPKnows;

        //Flincher Options
        public static CustomHeaderOption Flincher;
        public static CustomNumberOption FlincherCount;
        public static CustomNumberOption FlinchInterval;
        public static CustomToggleOption UniqueFlincher;

        //Volatile Options
        public static CustomHeaderOption Volatile;
        public static CustomNumberOption VolatileCount;
        public static CustomNumberOption VolatileInterval;
        public static CustomToggleOption VolatileKnows;
        public static CustomToggleOption UniqueVolatile;

        public static Func<object, string> PercentFormat { get; } = value => $"{value:0}%";
        public static Func<object, string> CooldownFormat { get; } = value => $"{value:0.0#}s";
        public static Func<object, string> MultiplierFormat { get; } = value => $"{value:0.0#}x";

        public static void GenerateAll()
        {
            var num = 0;

            Patches.ExportButton = new Export(-1);
            Patches.ImportButton = new Import(-1);

            GlobalSettings = new CustomHeaderOption(num++, MultiMenu.main, "Global Settings");
            PlayerSpeed = new CustomNumberOption(true, num++, MultiMenu.main, "Player Speed", 1f, 0.25f, 10f, 0.25f, MultiplierFormat);
            GhostSpeed = new CustomNumberOption(true, num++, MultiMenu.main, "Ghost Speed", 1.5f, 0.25f, 10f, 0.25f, MultiplierFormat);
            InteractionDistance = new CustomNumberOption(true, num++, MultiMenu.main, "Interaction Distance", 1f, 0.25f, 10f, 0.25f, MultiplierFormat);
            EmergencyButtonCount = new CustomNumberOption(true, num++, MultiMenu.main, "Emergency Count", 1, 0, 100, 1);
            EmergencyButtonCooldown = new CustomNumberOption(true, num++, MultiMenu.main, "Emergency Button Cooldown", 20f, 0f, 300f, 5f, CooldownFormat);
            DiscussionTime = new CustomNumberOption(true, num++, MultiMenu.main, "Discussion Time", 30f, 0f, 300f, 5f, CooldownFormat);
            VotingTime = new CustomNumberOption(true, num++, MultiMenu.main, "Voting Time", 60f, 5f, 600f, 15f, CooldownFormat);
            //LobbySize = new CustomNumberOption(true, num++, MultiMenu.main, "Lobby Size", 15, 4, 127, 1);
            TaskBarMode = new CustomStringOption(true, num++, MultiMenu.main, "Taskbar Updates", new[] {"Normal", "Meeting Only", "Invisible"});
            ConfirmEjects = new CustomToggleOption(true, num++, MultiMenu.main, "Confirm Ejects", false);
            EjectionRevealsRole = new CustomToggleOption(true, num++, MultiMenu.main, "Ejection Reveals Roles", false);
            InitialCooldowns = new CustomNumberOption(true, num++, MultiMenu.main, "Game Start Cooldowns", 10f, 10f, 30f, 2.5f, CooldownFormat);
            //ChatCooldown = new CustomNumberOption(true, num++, MultiMenu.main, "Chat Cooldown", 0f, 3f, 10f, 0.5f, CooldownFormat);

            GameModeSettings = new CustomHeaderOption(num++, MultiMenu.main, "Game Mode Settings");
            GameMode = new CustomStringOption(true, num++, MultiMenu.main, "Game Mode", new[] {"Classic", "All Any", "Killing Only", "Custom"});

            KillingOnlySettings = new CustomHeaderOption(num++, MultiMenu.main, "<color=#1D7CF2FF>Killing</color> Only Mode Settings");
            NeutralRoles = new CustomNumberOption(true, num++, MultiMenu.main, "<color=#B3B3B3FF>Neutral</color> Count", 1, 0f, 13, 1);
            AddArsonist = new CustomToggleOption(true, num++, MultiMenu.main, "Add <color=#EE7600FF>Arsonist</color>", false);
            AddCryomaniac = new CustomToggleOption(true, num++, MultiMenu.main, "Add <color=#642DEAFF>Cryomaniac</color>", false);
            AddPlaguebearer = new CustomToggleOption(true, num++, MultiMenu.main, "Add <color=#CFFE61FF>Plaguebearer</color>", false);

            AllAnySettings = new CustomHeaderOption(num++, MultiMenu.main, "All Any Mode Settings");
            EnableUniques = new CustomToggleOption(true, num++, MultiMenu.main, "Enable Uniques", false);

            GameModifiers = new CustomHeaderOption(num++, MultiMenu.main, "Game Modifiers");
            ColourblindComms = new CustomToggleOption(true, num++, MultiMenu.main, "Camouflaged Comms", true);
            MeetingColourblind = new CustomToggleOption(true, num++, MultiMenu.main, "Camouflaged Meetings", false);
            WhoCanVent = new CustomStringOption(true, num++, MultiMenu.main, "Serial Venters", new[] {"Default", "Everyone", "No One" });
            ParallelMedScans = new CustomToggleOption(true, num++, MultiMenu.main, "Parallel Medbay Scans", false);
            AnonymousVoting = new CustomToggleOption(true, num++, MultiMenu.main, "Anonymous Voting", true);
            SkipButtonDisable = new CustomStringOption(true, num++, MultiMenu.main, "No Skipping", new[] {"Never", "Emergency", "Always"});
            FactionSeeRoles = new CustomToggleOption(true, num++, MultiMenu.main, "Factioned Evils See The Roles Of Their Team", true);
            VisualTasks = new CustomToggleOption(true, num++, MultiMenu.main, "Tasks Can Be Visualised", false);
            LocationReports = new CustomToggleOption(true, num++, MultiMenu.main, "Body Reports Now Display The Body's Location", false);
            RoleFactionReports = new CustomStringOption(true, num++, MultiMenu.main, "Body Reports Now Display The Body's Role/Faction", new [] {"Roles", "Factions", "Never"});
            KillerReports = new CustomToggleOption(true, num++, MultiMenu.main, "Body Reports Now Display The Body's Killer's Role", false);
            NoNames = new CustomToggleOption(true, num++, MultiMenu.main, "No Player Names", false);
            Whispers = new CustomToggleOption(true, num++, MultiMenu.main, "PSSST *Whispers*", true);
            AppearanceAnimation = new CustomToggleOption(true, num++, MultiMenu.main, "Kill Animations Show Morphed Player", true);
            LighterDarker = new CustomToggleOption(true, num++, MultiMenu.main, "Enable Lighter Darker Colors", true);
            RandomSpawns = new CustomToggleOption(true, num++, MultiMenu.main, "Enable Random Player Spawns", false);
            EnableAbilities = new CustomToggleOption(true, num++, MultiMenu.main, "Enable <color=#FF9900FF>Abilities</color>", true);
            EnableModifiers = new CustomToggleOption(true, num++, MultiMenu.main, "Enable <color=#7F7F7FFF>Modifiers</color>", true);
            EnableObjectifiers = new CustomToggleOption(true, num++, MultiMenu.main, "Enable <color=#DD585BFF>Objectifiers</color>", true);
            VentTargetting = new CustomToggleOption(true, num++, MultiMenu.main, "Players In Vents Can Be Targetted", true);

            QualityChanges = new CustomHeaderOption(num++, MultiMenu.main, "Quality Additions");
            DeadSeeEverything = new CustomToggleOption(true, num++, MultiMenu.main, "Dead Can See Everyone's Roles && Votes", true);
            DisableLevels = new CustomToggleOption(true, num++, MultiMenu.main, "Disable Level Icons", false);
            WhiteNameplates = new CustomToggleOption(true, num++, MultiMenu.main, "Disable Player Nameplates", false);
            SeeTasks = new CustomToggleOption(true, num++, MultiMenu.main, "See Tasks During The Game", true);
            CustomEject = new CustomToggleOption(true, num++, MultiMenu.main, "Custom Ejection Messages", true);

            MapSettings = new CustomHeaderOption(num++, MultiMenu.main, "Map Settings");
            //Map = new CustomStringOption(true, num++, MultiMenu.main, "Map", new[] {"Skeld", "Mira HQ", "Polus", "Airship", "Submerged"});
            RandomMapEnabled = new CustomToggleOption(true, num++, MultiMenu.main, "Choose Random Map", false);
            RandomMapSkeld = new CustomNumberOption(true, num++, MultiMenu.main, "Skeld Chance", 0f, 0f, 100f, 10f, PercentFormat);
            RandomMapMira = new CustomNumberOption(true, num++, MultiMenu.main, "Mira Chance", 0f, 0f, 100f, 10f, PercentFormat);
            RandomMapPolus = new CustomNumberOption(true, num++, MultiMenu.main, "Polus Chance", 0f, 0f, 100f, 10f, PercentFormat);
            RandomMapAirship = new CustomNumberOption(true, num++, MultiMenu.main, "Airship Chance", 0f, 0f, 100f, 10f, PercentFormat);
            RandomMapSubmerged = new CustomNumberOption(true, num++, MultiMenu.main, "Submerged Chance", 0f, 0f, 100f, 10f, PercentFormat);
            AutoAdjustSettings = new CustomToggleOption(true, num++, MultiMenu.main, "Auto Adjust Settings", false);
            SmallMapHalfVision = new CustomToggleOption(true, num++, MultiMenu.main, "Half Vision On Skeld/Mira HQ", false);
            SmallMapDecreasedCooldown = new CustomNumberOption(true, num++, MultiMenu.main, "Mira HQ Decreased Cooldowns", 0f, 0f, 15f, 2.5f, CooldownFormat);
            LargeMapIncreasedCooldown = new CustomNumberOption(true, num++, MultiMenu.main, "Airship/Submerged Increased Cooldowns", 0f, 0f, 15f, 2.5f, CooldownFormat);
            SmallMapIncreasedShortTasks = new CustomNumberOption(true, num++, MultiMenu.main, "Skeld/Mira HQ Increased Short Tasks", 0, 0, 5, 1);
            SmallMapIncreasedLongTasks = new CustomNumberOption(true, num++, MultiMenu.main, "Skeld/Mira HQ Increased Long Tasks", 0, 0, 3, 1);
            LargeMapDecreasedShortTasks = new CustomNumberOption(true, num++, MultiMenu.main, "Airship/Submerged Decreased Short Tasks", 0, 0, 5, 1);
            LargeMapDecreasedLongTasks = new CustomNumberOption(true, num++, MultiMenu.main, "Airship/Submerged Decreased Long Tasks", 0, 0, 3, 1);

            BetterSkeldSettings = new CustomHeaderOption(num++, MultiMenu.main, "Skeld Settings");
            SkeldVentImprovements = new CustomToggleOption(true, num++, MultiMenu.main, "Changed Vent Layout", false);
            //ReactorTimer = new CustomNumberOption(true, num++, MultiMenu.main, "Reactor Meltdown Countdown", 60f, 30f, 90f, 5f, CooldownFormat);
            //OxygenTimer = new CustomNumberOption(true, num++, MultiMenu.main, "Oxygen Depletion Countdown", 60f, 30f, 90f, 5f, CooldownFormat);

            BetterPolusSettings = new CustomHeaderOption(num++, MultiMenu.main, "Polus Settings");
            PolusVentImprovements = new CustomToggleOption(true, num++, MultiMenu.main, "Changed Vent Layout", false);
            VitalsLab = new CustomToggleOption(true, num++, MultiMenu.main, "Vitals Moved To Lab", false);
            ColdTempDeathValley = new CustomToggleOption(true, num++, MultiMenu.main, "Cold Temp Moved To Death Valley", false);
            WifiChartCourseSwap = new CustomToggleOption(true, num++, MultiMenu.main, "Reboot Wifi And Chart Course Swapped", false);
            //SeismicTimer = new CustomNumberOption(true, num++, MultiMenu.main, "Seimic Stabliser Malfunction Countdown", 60f, 30f, 90f, 5f, CooldownFormat);

            BetterAirshipSettings = new CustomHeaderOption(num++, MultiMenu.main, "Airship Settings");
            NewSpawns = new CustomToggleOption(true, num++, MultiMenu.main, "Add New Spawns", false);
            SpawnType = new CustomStringOption(true, num++, MultiMenu.main, "Spawn Type", new [] {"Normal", "Fixed", "Random Synchronised"});
            MeetingSpawnChoice = new CustomToggleOption(true, num++, MultiMenu.main, "Spawn Near Meeting Table After A Meeting", false);
            CallPlatform = new CustomToggleOption(true, num++, MultiMenu.main, "Add Call Platform Button", false);
            AddTeleporters = new CustomToggleOption(true, num++, MultiMenu.main, "Add Meeting To Security Room Teleporter", false);
            MoveVitals = new CustomToggleOption(true, num++, MultiMenu.main, "Move Vitals", false);
            MoveFuel = new CustomToggleOption(true, num++, MultiMenu.main, "Move Fuel", false);
            MoveDivert = new CustomToggleOption(true, num++, MultiMenu.main, "Move Divert", false);
            MoveAdmin = new CustomStringOption(true, num++, MultiMenu.main, "Move Admin", new [] {"Don't Move", "Right Of Cockpit", "Main Hall"});
            MoveElectrical = new CustomStringOption(true, num++, MultiMenu.main, "Move Electrical Outlet", new [] {"Don't Move", "Vault", "Electrical"});
            //CrashTimer = new CustomNumberOption(true, num++, MultiMenu.main, "Crash Course Countdown", 60f, 30f, 90f, 5f, CooldownFormat);

            CrewAuditorRoles = new CustomHeaderOption(num++, MultiMenu.crew, "<color=#8BFDFDFF>Crew</color> <color=#1D7CF2FF>Auditor</color> <color=#FFD700FF>Roles</color>");
            MysticOn = new CustomNumberOption(true, num++, MultiMenu.crew, "<color=#708EEFFF>Mystic</color>", 0, 0, 100, 10, PercentFormat);
            VampireHunterOn = new CustomNumberOption(true, num++, MultiMenu.crew, "<color=#C0C0C0FF>Vampire Hunter</color>", 0, 0, 100, 10, PercentFormat);

            CrewInvestigativeRoles = new CustomHeaderOption(num++, MultiMenu.crew, "<color=#8BFDFDFF>Crew</color> <color=#1D7CF2FF>Investigative</color> <color=#FFD700FF>Roles</color>");
            AgentOn = new CustomNumberOption(true, num++, MultiMenu.crew, "<color=#CCA3CCFF>Agent</color>", 0, 0, 100, 10, PercentFormat);
            CoronerOn = new CustomNumberOption(true, num++, MultiMenu.crew, "<color=#4D99E6FF>Coroner</color>", 0, 0, 100, 10, PercentFormat);
            DetectiveOn = new CustomNumberOption(true, num++, MultiMenu.crew, "<color=#4D4DFFFF>Detective</color>", 0, 0, 100, 10, PercentFormat);
            InspectorOn = new CustomNumberOption(true, num++, MultiMenu.crew, "<color=#7E3C64FF>Inspector</color>", 0, 0, 100, 10, PercentFormat);
            //InvestigatorOn = new CustomNumberOption(true, num++, MultiMenu.crew, "<color=#00B3B3FF>Investigator</color>", 0, 0, 100, 10, PercentFormat);
            MediumOn = new CustomNumberOption(true, num++, MultiMenu.crew, "<color=#A680FFFF>Medium</color>", 0, 0, 100, 10, PercentFormat);
            OperativeOn = new CustomNumberOption(true, num++, MultiMenu.crew, "<color=#A7D1B3FF>Operative</color>", 0, 0, 100, 10, PercentFormat);
            SeerOn = new CustomNumberOption(true, num++, MultiMenu.crew, "<color=#71368AFF>Seer</color>", 0, 0, 100, 10, PercentFormat);
            SheriffOn = new CustomNumberOption(true, num++, MultiMenu.crew, "<color=#FFCC80FF>Sheriff</color>", 0, 0, 100, 10, PercentFormat);
            TrackerOn = new CustomNumberOption(true, num++, MultiMenu.crew, "<color=#009900FF>Tracker</color>", 0, 0, 100, 10, PercentFormat);

            CrewKillingRoles = new CustomHeaderOption(num++, MultiMenu.crew, "<color=#8BFDFDFF>Crew</color> <color=#1D7CF2FF>Killing</color> <color=#FFD700FF>Roles</color>");
            VeteranOn = new CustomNumberOption(true, num++, MultiMenu.crew, "<color=#998040FF>Veteran</color>", 0, 0, 100, 10, PercentFormat);
            VigilanteOn = new CustomNumberOption(true, num++, MultiMenu.crew, "<color=#FFFF00FF>Vigilante</color>", 0, 0, 100, 10, PercentFormat);

            CrewProtectiveRoles = new CustomHeaderOption(num++, MultiMenu.crew, "<color=#8BFDFDFF>Crew</color> <color=#1D7CF2FF>Protective</color> <color=#FFD700FF>Roles</color>");
            AltruistOn = new CustomNumberOption(true, num++, MultiMenu.crew, "<color=#660000FF>Altruist</color>", 0, 0, 100, 10, PercentFormat);
            MedicOn = new CustomNumberOption(true, num++, MultiMenu.crew, "<color=#006600FF>Medic</color>", 0, 0, 100, 10, PercentFormat);
            TimeLordOn = new CustomNumberOption(true, num++, MultiMenu.crew, "<color=#0000FFFF>Time Lord</color>", 0, 0, 100, 10, PercentFormat);

            CrewSovereignRoles = new CustomHeaderOption(num++, MultiMenu.crew, "<color=#8BFDFDFF>Crew</color> <color=#1D7CF2FF>Sovereign</color> <color=#FFD700FF>Roles</color>");
            MayorOn = new CustomNumberOption(true, num++, MultiMenu.crew, "<color=#704FA8FF>Mayor</color>", 0, 0, 100, 10, PercentFormat);
            SwapperOn = new CustomNumberOption(true, num++, MultiMenu.crew, "<color=#66E666FF>Swapper</color>", 0, 0, 100, 10, PercentFormat);

            CrewSupportRoles = new CustomHeaderOption(num++, MultiMenu.crew, "<color=#8BFDFDFF>Crew</color> <color=#1D7CF2FF>Support</color> <color=#FFD700FF>Roles</color>");
            ChameleonOn = new CustomNumberOption(true, num++, MultiMenu.crew, "<color=#5411F8FF>Chameleon</color>", 0, 0, 100, 10, PercentFormat);
            EngineerOn = new CustomNumberOption(true, num++, MultiMenu.crew, "<color=#FFA60AFF>Engineer</color>", 0, 0, 100, 10, PercentFormat);
            EscortOn = new CustomNumberOption(true, num++, MultiMenu.crew, "<color=#803333FF>Escort</color>", 0, 0, 100, 10, PercentFormat);
            RetributionistOn = new CustomNumberOption(true, num++, MultiMenu.crew, "<color=#8D0F8CFF>Retributionist</color>", 0, 0, 100, 10, PercentFormat);
            ShifterOn = new CustomNumberOption(true, num++, MultiMenu.crew, "<color=#DF851FFF>Shifter</color>", 0, 0, 100, 10, PercentFormat);
            TransporterOn = new CustomNumberOption(true, num++, MultiMenu.crew, "<color=#00EEFFFF>Transporter</color>", 0, 0, 100, 10, PercentFormat);

            CrewUtilityRoles = new CustomHeaderOption(num++, MultiMenu.crew, "<color=#8BFDFDFF>Crew</color> <color=#1D7CF2FF>Utility</color> <color=#FFD700FF>Roles</color>");
            CrewmateOn = new CustomNumberOption(true, num++, MultiMenu.crew, "<color=#8BFDFDFF>Crewmate</color>", 0, 0, 100, 10, PercentFormat);
            RevealerOn = new CustomNumberOption(true, num++, MultiMenu.crew, "<color=#D3D3D3FF>Revealer</color>", 0, 0, 100, 10, PercentFormat);

            NeutralBenignRoles = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Benign</color> <color=#FFD700FF>Roles</color>");
            AmnesiacOn = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#22FFFFFF>Amnesiac</color>", 0, 0, 100, 10, PercentFormat);
            GuardianAngelOn = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#FFFFFFFF>Guardian Angel</color>", 0, 0, 100, 10, PercentFormat);
            SurvivorOn = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#DDDD00FF>Survivor</color>", 0, 0, 100, 10, PercentFormat);
            ThiefOn = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#80FF00FF>Thief</color>", 0, 0, 100, 10, PercentFormat);

            NeutralEvilRoles = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Evil</color> <color=#FFD700FF>Roles</color>");
            ActorOn = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#00ACC2FF>Actor</color>", 0, 0, 100, 10, PercentFormat);
            BountyHunterOn = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#B51E39FF>Bounty Hunter</color>", 0, 0, 100, 10, PercentFormat);
            CannibalOn = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#8C4005FF>Cannibal</color>", 0, 0, 100, 10, PercentFormat);
            ExecutionerOn = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#CCCCCCFF>Executioner</color>", 0, 0, 100, 10, PercentFormat);
            GuesserOn = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#EEE5BEFF>Guesser</color>", 0, 0, 100, 10, PercentFormat);
            JesterOn = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#F7B3DAFF>Jester</color>", 0, 0, 100, 10, PercentFormat);
            TrollOn = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#678D36FF>Troll</color>", 0, 0, 100, 10, PercentFormat);

            NeutralKillingRoles = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killing</color> <color=#FFD700FF>Roles</color>");
            ArsonistOn = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#EE7600FF>Arsonist</color>", 0, 0, 100, 10, PercentFormat);
            CryomaniacOn = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#642DEAFF>Cryomaniac</color>", 0, 0, 100, 10, PercentFormat);
            GlitchOn = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#00FF00FF>Glitch</color>", 0, 0, 100, 10, PercentFormat);
            JuggernautOn = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#A12B56FF>Juggernaut</color>", 0, 0, 100, 10, PercentFormat);
            MurdererOn = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#6F7BEAFF>Murderer</color>", 0, 0, 100, 10, PercentFormat);
            PlaguebearerOn = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#CFFE61FF>Plaguebearer</color>", 0, 0, 100, 10, PercentFormat);
            SerialKillerOn = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#336EFFFF>Serial Killer</color>", 0, 0, 100, 10, PercentFormat);
            WerewolfOn = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#9F703AFF>Werewolf</color>", 0, 0, 100, 10, PercentFormat);

            NeutralNeophyteRoles = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Neophyte</color> <color=#FFD700FF>Roles</color>");
            DraculaOn = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#AC8A00FF>Dracula</color>", 0, 0, 100, 10, PercentFormat);
            JackalOn = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#45076AFF>Jackal</color>", 0, 0, 100, 10, PercentFormat);
            NecromancerOn = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#FF1919FF>Necromancer</color>", 0, 0, 100, 10, PercentFormat);
            WhispererOn = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#2D6AA5FF>Whisperer</color>", 0, 0, 100, 10, PercentFormat);

            NeutralNeophyteRoles = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Proselyte</color> <color=#FFD700FF>Roles</color>");
            PhantomOn = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#662962FF>Phantom</color>", 0, 0, 100, 10, PercentFormat);

            IntruderConcealingRoles = new CustomHeaderOption(num++, MultiMenu.intruder, "<color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Concealing</color> <color=#FFD700FF>Roles</color>");
            BlackmailerOn = new CustomNumberOption(true, num++, MultiMenu.intruder, "<color=#02A752FF>Blackmailer</color>", 0, 0, 100, 10, PercentFormat);
            CamouflagerOn = new CustomNumberOption(true, num++, MultiMenu.intruder, "<color=#378AC0FF>Camouflager</color>", 0, 0, 100, 10, PercentFormat);
            GrenadierOn = new CustomNumberOption(true, num++, MultiMenu.intruder, "<color=#85AA5BFF>Grenadier</color>", 0, 0, 100, 10, PercentFormat);
            JanitorOn = new CustomNumberOption(true, num++, MultiMenu.intruder, "<color=#2647A2FF>Janitor</color>", 0, 0, 100, 10, PercentFormat);
            UndertakerOn = new CustomNumberOption(true, num++, MultiMenu.intruder, "<color=#005643FF>Undertaker</color>", 0, 0, 100, 10, PercentFormat);

            IntruderDeceptionRoles = new CustomHeaderOption(num++, MultiMenu.intruder, "<color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Deception</color> <color=#FFD700FF>Roles</color>");
            DisguiserOn = new CustomNumberOption(true, num++, MultiMenu.intruder, "<color=#40B4FFFF>Disguiser</color>", 0, 0, 100, 10, PercentFormat);
            MorphlingOn = new CustomNumberOption(true, num++, MultiMenu.intruder, "<color=#BB45B0FF>Morphling</color>", 0, 0, 100, 10, PercentFormat);
            WraithOn = new CustomNumberOption(true, num++, MultiMenu.intruder, "<color=#5C4F75FF>Wraith</color>", 0, 0, 100, 10, PercentFormat);

            //IntruderKillingRoles = new CustomHeaderOption(num++, MultiMenu.intruder, "<color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Killing</color> <color=#FFD700FF>Roles</color>");

            IntruderSupportRoles = new CustomHeaderOption(num++, MultiMenu.intruder, "<color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Support</color> <color=#FFD700FF>Roles</color>");
            ConsigliereOn = new CustomNumberOption(true, num++, MultiMenu.intruder, "<color=#FFFF99FF>Consigliere</color>", 0, 0, 100, 10, PercentFormat);
            ConsortOn = new CustomNumberOption(true, num++, MultiMenu.intruder, "<color=#801780FF>Consort</color>", 0, 0, 100, 10, PercentFormat);
            GodfatherOn = new CustomNumberOption(true, num++, MultiMenu.intruder, "<color=#404C08FF>Godfather</color>", 0, 0, 100, 10, PercentFormat);
            MinerOn = new CustomNumberOption(true, num++, MultiMenu.intruder, "<color=#AA7632FF>Miner</color>", 0, 0, 100, 10, PercentFormat);
            TeleporterOn = new CustomNumberOption(true, num++, MultiMenu.intruder, "<color=#6AA84FFF>Teleporter</color>", 0, 0, 100, 10, PercentFormat);
            TimeMasterOn = new CustomNumberOption(true, num++, MultiMenu.intruder, "<color=#0000A7FF>Time Master</color>", 0, 0, 100, 10, PercentFormat);

            IntruderUtilityRoles = new CustomHeaderOption(num++, MultiMenu.intruder, "<color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Utility</color> <color=#FFD700FF>Roles</color>");
            ImpostorOn = new CustomNumberOption(true, num++, MultiMenu.intruder, "<color=#FF0000FF>Impostor</color>", 0, 0, 100, 10, PercentFormat);

            SyndicateDisruptionRoles = new CustomHeaderOption(num++, MultiMenu.syndicate, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Disruption</color> <color=#FFD700FF>Roles</color>");
            DrunkardOn = new CustomNumberOption(true, num++, MultiMenu.syndicate, "<color=#1E300BFF>Drunkard</color>", 0, 0, 100, 10, PercentFormat);
            FramerOn = new CustomNumberOption(true, num++, MultiMenu.syndicate, "<color=#00FFFFFF>Framer</color>", 0, 0, 100, 10, PercentFormat);
            PoisonerOn = new CustomNumberOption(true, num++, MultiMenu.syndicate, "<color=#B5004CFF>Poisoner</color>", 0, 0, 100, 10, PercentFormat);
            ShapeshifterOn = new CustomNumberOption(true, num++, MultiMenu.syndicate, "<color=#311C45FF>Shapeshifter</color>", 0, 0, 100, 10, PercentFormat);

            SyndicateKillingRoles = new CustomHeaderOption(num++, MultiMenu.syndicate, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Killing</color> <color=#FFD700FF>Roles</color>");
            BomberOn = new CustomNumberOption(true, num++, MultiMenu.syndicate, "<color=#C9CC3FFF>Bomber</color>", 0, 0, 100, 10, PercentFormat);
            GorgonOn = new CustomNumberOption(true, num++, MultiMenu.syndicate, "<color=#7E4D00FF>Gorgon</color>", 0, 0, 100, 10, PercentFormat);

            SyndicateSupportRoles = new CustomHeaderOption(num++, MultiMenu.syndicate, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Support</color> <color=#FFD700FF>Roles</color>");
            BeamerOn = new CustomNumberOption(true, num++, MultiMenu.syndicate, "<color=#0028F5FF>Beamer</color>", 0, 0, 100, 10, PercentFormat);
            ConcealerOn = new CustomNumberOption(true, num++, MultiMenu.syndicate, "<color=#C02525FF>Concealer</color>", 0, 0, 100, 10, PercentFormat);
            RebelOn = new CustomNumberOption(true, num++, MultiMenu.syndicate, "<color=#FFFCCEFF>Rebel</color>", 0, 0, 100, 10, PercentFormat);
            WarperOn = new CustomNumberOption(true, num++, MultiMenu.syndicate, "<color=#8C7140FF>Warper</color>", 0, 0, 100, 10, PercentFormat);

            SyndicateUtilityRoles = new CustomHeaderOption(num++, MultiMenu.syndicate, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Utility</color> <color=#FFD700FF>Roles</color>");
            AnarchistOn = new CustomNumberOption(true, num++, MultiMenu.syndicate, "<color=#008000FF>Anarchist</color>", 0, 0, 100, 10, PercentFormat);

            Objectifiers = new CustomHeaderOption(num++, MultiMenu.objectifier, "<color=#DD585BFF>Objectifiers</color>");
            AlliedOn = new CustomNumberOption(true, num++, MultiMenu.objectifier, "<color=#4545A9FF>Allied</color>", 0, 0, 100, 10, PercentFormat);
            CorruptedOn = new CustomNumberOption(true, num++, MultiMenu.objectifier, "<color=#4545FFFF>Corrupted</color>", 0, 0, 100, 10, PercentFormat);
            FanaticOn = new CustomNumberOption(true, num++, MultiMenu.objectifier, "<color=#678D36FF>Fanatic</color>", 0, 0, 100, 10, PercentFormat);
            LoversOn = new CustomNumberOption(true, num++, MultiMenu.objectifier, "<color=#FF66CCFF>Lovers</color>", 0, 0, 100, 10, PercentFormat);
            OverlordOn = new CustomNumberOption(true, num++, MultiMenu.objectifier, "<color=#008080FF>Overlord</color>", 0, 0, 100, 10, PercentFormat);
            RivalsOn = new CustomNumberOption(true, num++, MultiMenu.objectifier, "<color=#3D2D2CFF>Rivals</color>", 0, 0, 100, 10, PercentFormat);
            TaskmasterOn = new CustomNumberOption(true, num++, MultiMenu.objectifier, "<color=#ABABFFFF>Taskmaster</color>", 0, 0, 100, 10, PercentFormat);
            TraitorOn = new CustomNumberOption(true, num++, MultiMenu.objectifier, "<color=#370D43FF>Traitor</color>", 0, 0, 100, 10, PercentFormat);

            Abilities = new CustomHeaderOption(num++, MultiMenu.ability, "<color=#FF9900FF>Abilities</color>");
            ButtonBarryOn = new CustomNumberOption(true, num++, MultiMenu.ability, "<color=#E600FFFF>Button Barry</color>", 0, 0, 100, 10, PercentFormat);
            CrewAssassinOn = new CustomNumberOption(true, num++, MultiMenu.ability, "<color=#8BFDFDFF>Crew</color> <color=#073763FF>Assassin</color>", 0, 0, 100, 10, PercentFormat);
            InsiderOn = new CustomNumberOption(true, num++, MultiMenu.ability, "<color=#26FCFBFF>Insider</color>", 0, 0, 100, 10, PercentFormat);
            IntruderAssassinOn = new CustomNumberOption(true, num++, MultiMenu.ability, "<color=#FF0000FF>Intruder</color> <color=#073763FF>Assassin</color>", 0, 0, 100, 10, PercentFormat);
            LighterOn = new CustomNumberOption(true, num++, MultiMenu.ability, "<color=#1AFF74FF>Lighter</color>", 0, 0, 100, 10, PercentFormat);
            MultitaskerOn = new CustomNumberOption(true, num++, MultiMenu.ability, "<color=#FF804DFF>Multitasker</color>", 0, 0, 100, 10, PercentFormat);
            NeutralAssassinOn = new CustomNumberOption(true, num++, MultiMenu.ability, "<color=#B3B3B3FF>Neutral</color> <color=#073763FF>Assassin</color>", 0, 0, 100, 10, PercentFormat);
            RadarOn = new CustomNumberOption(true, num++, MultiMenu.ability, "<color=#FF0080FF>Radar</color>", 0, 0, 100, 10, PercentFormat);
            RuthlessOn = new CustomNumberOption(true, num++, MultiMenu.ability, "<color=#2160DDFF>Ruthless</color>", 0, 0, 100, 10, PercentFormat);
            SnitchOn = new CustomNumberOption(true, num++, MultiMenu.ability, "<color=#D4AF37FF>Snitch</color>", 0, 0, 100, 10, PercentFormat);
            SyndicateAssassinOn = new CustomNumberOption(true, num++, MultiMenu.ability, "<color=#008000FF>Syndicate</color> <color=#073763FF>Assassin</color>", 0, 0, 100, 10, PercentFormat);
            TiebreakerOn = new CustomNumberOption(true, num++, MultiMenu.ability, "<color=#99E699FF>Tiebreaker</color>", 0, 0, 100, 10, PercentFormat);
            TorchOn = new CustomNumberOption(true, num++, MultiMenu.ability, "<color=#FFFF99FF>Torch</color>", 0, 0, 100, 10, PercentFormat);
            TunnelerOn = new CustomNumberOption(true, num++, MultiMenu.ability, "<color=#E91E63FF>Tunneler</color>", 0, 0, 100, 10, PercentFormat);
            UnderdogOn = new CustomNumberOption(true, num++, MultiMenu.ability, "<color=#841A7FFF>Underdog</color>", 0, 0, 100, 10, PercentFormat);

            Modifiers = new CustomHeaderOption(num++, MultiMenu.modifier, "<color=#7F7F7FFF>Modifiers</color>");
            BaitOn = new CustomNumberOption(true, num++, MultiMenu.modifier, "<color=#00B3B3FF>Bait</color>", 0, 0, 100, 10, PercentFormat);
            CowardOn = new CustomNumberOption(true, num++, MultiMenu.modifier, "<color=#456BA8FF>Coward</color>", 0, 0, 100, 10, PercentFormat);
            DiseasedOn = new CustomNumberOption(true, num++, MultiMenu.modifier, "<color=#374D1EFF>Diseased</color>", 0, 0, 100, 10, PercentFormat);
            DrunkOn = new CustomNumberOption(true, num++, MultiMenu.modifier, "<color=#758000FF>Drunk</color>", 0, 0, 100, 10, PercentFormat);
            DwarfOn = new CustomNumberOption(true, num++, MultiMenu.modifier, "<color=#FF8080FF>Dwarf</color>", 0, 0, 100, 10, PercentFormat);
            FlincherOn = new CustomNumberOption(true, num++, MultiMenu.modifier, "<color=#80B3FFFF>Flincher</color>", 0, 0, 100, 10, PercentFormat);
            GiantOn = new CustomNumberOption(true, num++, MultiMenu.modifier, "<color=#FFB34DFF>Giant</color>", 0, 0, 100, 10, PercentFormat);
            IndomitableOn = new CustomNumberOption(true, num++, MultiMenu.modifier, "<color=#2DE5BEFF>Indomitable</color>", 0, 0, 100, 10, PercentFormat);
            ProfessionalOn = new CustomNumberOption(true, num++, MultiMenu.modifier, "<color=#860B7AFF>Professional</color>", 0, 0, 100, 10, PercentFormat);
            ShyOn = new CustomNumberOption(true, num++, MultiMenu.modifier, "<color=#1002C5FF>Shy</color>", 0, 0, 100, 10, PercentFormat);
            VIPOn = new CustomNumberOption(true, num++, MultiMenu.modifier, "<color=#DCEE85FF>VIP</color>", 0, 0, 100, 10, PercentFormat);
            VolatileOn = new CustomNumberOption(true, num++, MultiMenu.modifier, "<color=#FFA60AFF>Volatile</color>", 0, 0, 100, 10, PercentFormat);

            CrewSettings = new CustomHeaderOption(num++, MultiMenu.crew, "<color=#8BFDFDFF>Crew</color> Settings");
            CustomCrewColors = new CustomToggleOption(true, num++, MultiMenu.crew, "Enable Custom <color=#8BFDFDFF>Crew</color> Colors", true);
            ShortTasks = new CustomNumberOption(true, num++, MultiMenu.crew, "Short Task Count", 1, 0, 100, 1);
            LongTasks = new CustomNumberOption(true, num++, MultiMenu.crew, "Long Task Count", 1, 0, 100, 1);
            CommonTasks = new CustomNumberOption(true, num++, MultiMenu.crew, "Common Task Count", 1, 0, 100, 1);
            GhostTasksCountToWin = new CustomToggleOption(true, num++, MultiMenu.crew, "<color=#8BFDFDFF>Crew</color> Ghost Tasks Count To Win", true);
            CrewVision = new CustomNumberOption(true, num++, MultiMenu.crew, "<color=#8BFDFDFF>Crew</color> Vision", 1f, 0.25f, 5f, 0.25f, MultiplierFormat);
            CrewMax = new CustomNumberOption(true, num++, MultiMenu.crew, "Max <color=#8BFDFDFF>Crew</color> <color=#FFD700FF>Roles</color>", 5, 0, 14, 1);
            CrewMin = new CustomNumberOption(true, num++, MultiMenu.crew, "Min <color=#8BFDFDFF>Crew</color> <color=#FFD700FF>Roles</color>", 5, 0, 14, 1);
            CrewVent = new CustomToggleOption(true, num++, MultiMenu.crew, "<color=#8BFDFDFF>Crew</color> Can Vent", false);

            CrewAuditorSettings = new CustomHeaderOption(num++, MultiMenu.crew, "<color=#8BFDFDFF>Crew</color> <color=#1D7CF2FF>Auditor</color> Settings");
            CAMax = new CustomNumberOption(true, num++, MultiMenu.crew, "Max <color=#8BFDFDFF>Crew</color> <color=#1D7CF2FF>Auditor</color> Roles", 1, 1, 14, 1);
            CAMin = new CustomNumberOption(true, num++, MultiMenu.crew, "Min <color=#8BFDFDFF>Crew</color> <color=#1D7CF2FF>Auditor</color> Roles", 1, 1, 14, 1);

            Mystic = new CustomHeaderOption(num++, MultiMenu.crew, "<color=#708EEFFF>Mystic</color>");
            MysticCount = new CustomNumberOption(true, num++, MultiMenu.crew, "<color=#708EEFFF>Mystic</color> Count", 1, 1, 14, 1);
            UniqueMystic = new CustomToggleOption(true, num++, MultiMenu.crew, "<color=#708EEFFF>Mystic</color> Is Unique In All Any", false);
            RevealCooldown = new CustomNumberOption(true, num++, MultiMenu.crew, "Reveal Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);

            VampireHunter = new CustomHeaderOption(num++, MultiMenu.crew, "<color=#C0C0C0FF>Vampire Hunter</color>");
            VampireHunterCount = new CustomNumberOption(true, num++, MultiMenu.crew, "<color=#C0C0C0FF>Vampire Hunter</color> Count", 1, 1, 14, 1);
            UniqueVampireHunter = new CustomToggleOption(true, num++, MultiMenu.crew, "<color=#C0C0C0FF>Vampire Hunter</color> Is Unique In All Any", false);
            StakeCooldown = new CustomNumberOption(true, num++, MultiMenu.crew, "Stake Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);

            CrewInvestigativeSettings = new CustomHeaderOption(num++, MultiMenu.crew, "<color=#8BFDFDFF>Crew</color> <color=#1D7CF2FF>Investigative</color> Settings");
            CIMax = new CustomNumberOption(true, num++, MultiMenu.crew, "Max <color=#8BFDFDFF>Crew</color> <color=#1D7CF2FF>Investigative</color> Roles", 1, 1, 14, 1);
            CIMin = new CustomNumberOption(true, num++, MultiMenu.crew, "Min <color=#8BFDFDFF>Crew</color> <color=#1D7CF2FF>Investigative</color> Roles", 1, 1, 14, 1);

            Agent = new CustomHeaderOption(num++, MultiMenu.crew, "<color=#CCA3CCFF>Agent</color>");
            AgentCount = new CustomNumberOption(true, num++, MultiMenu.crew, "<color=#CCA3CCFF>Agent</color> Count", 1, 1, 14, 1);
            UniqueAgent = new CustomToggleOption(true, num++, MultiMenu.crew, "<color=#CCA3CCFF>Agent</color> Is Unique In All Any", false);

            Coroner = new CustomHeaderOption(num++, MultiMenu.crew, "<color=#4D99E6FF>Coroner</color>");
            CoronerCount = new CustomNumberOption(true, num++, MultiMenu.crew, "<color=#4D99E6FF>Coroner</color> Count", 1, 1, 14, 1);
            UniqueCoroner = new CustomToggleOption(true, num++, MultiMenu.crew, "<color=#4D99E6FF>Coroner</color> Is Unique In All Any", false);
            CoronerArrowDuration = new CustomNumberOption(true, num++, MultiMenu.crew, "Dead Body Arrow Duration", 0.1f, 0f, 2f, 0.05f, CooldownFormat);
            CoronerReportRole = new CustomToggleOption(true, num++, MultiMenu.crew, "<color=#4D99E6FF>Coroner</color> Gets Killer's Role", false);
            CoronerReportName = new CustomToggleOption(true, num++, MultiMenu.crew, "<color=#4D99E6FF>Coroner</color> Gets Killer's Name", false);
            CoronerKillerNameTime = new CustomNumberOption(true, num++, MultiMenu.crew, "<color=#4D99E6FF>Coroner</color> Get Killer's Name Under", 1f, 0.5f, 15f, 0.5f, CooldownFormat);

            Detective = new CustomHeaderOption(num++, MultiMenu.crew, "<color=#4D4DFFFF>Detective</color>");
            DetectiveCount = new CustomNumberOption(true, num++, MultiMenu.crew, "<color=#4D4DFFFF>Detective</color> Count", 1, 1, 14, 1);
            UniqueDetective = new CustomToggleOption(true, num++, MultiMenu.crew, "<color=#4D4DFFFF>Detective</color> Is Unique In All Any", false);
            ExamineCooldown = new CustomNumberOption(true, num++, MultiMenu.crew, "Examine Cooldown", 10f, 1f, 20f, 1f, CooldownFormat);
            RecentKill = new CustomNumberOption(true, num++, MultiMenu.crew, "Bloody Player Duration", 25f, 10f, 60f, 2.5f, CooldownFormat);
            FootprintSize = new CustomNumberOption(true, num++, MultiMenu.crew, "Footprint Size", 4f, 1f, 10f, 1f);
            FootprintInterval = new CustomNumberOption(true, num++, MultiMenu.crew, "Footprint Interval", 0.15f, 0.05f, 2f, 0.05f, CooldownFormat);
            FootprintDuration = new CustomNumberOption(true, num++, MultiMenu.crew, "Footprint Duration", 10f, 0.5f, 10f, 0.5f, CooldownFormat);
            AnonymousFootPrint = new CustomToggleOption(true, num++, MultiMenu.crew, "Anonymous Footprint", false);
            VentFootprintVisible = new CustomToggleOption(true, num++, MultiMenu.crew, "Footprints Are Visible Near Vents", false);

            Inspector = new CustomHeaderOption(num++, MultiMenu.crew, "<color=#7E3C64FF>Inspector</color>");
            InspectorCount = new CustomNumberOption(true, num++, MultiMenu.crew, "<color=#7E3C64FF>Inspector</color> Count", 1, 1, 14, 1);
            UniqueInspector = new CustomToggleOption(true, num++, MultiMenu.crew, "<color=#7E3C64FF>Inspector</color> Is Unique In All Any", false);
            InspectCooldown = new CustomNumberOption(true, num++, MultiMenu.crew, "Inspect Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);

            Medium = new CustomHeaderOption(num++, MultiMenu.crew, "<color=#A680FFFF>Medium</color>");
            MediumCount = new CustomNumberOption(true, num++, MultiMenu.crew, "<color=#A680FFFF>Medium</color> Count", 1, 1, 14, 1);
            UniqueMedium = new CustomToggleOption(true, num++, MultiMenu.crew, "<color=#A680FFFF>Medium</color> Is Unique In All Any", false);
            MediateCooldown = new CustomNumberOption(true, num++, MultiMenu.crew, "Mediate Cooldown", 10f, 1f, 20f, 1f, CooldownFormat);
            MediateDuration = new CustomNumberOption(true, num++, MultiMenu.crew, "Mediate Duration", 10f, 1f, 20f, 1f, CooldownFormat);
            ShowMediatePlayer = new CustomToggleOption(true, num++, MultiMenu.crew, "Reveal Appearance Of Mediate Target", true);
            ShowMediumToDead = new CustomToggleOption(true, num++, MultiMenu.crew, "Reveal The <color=#A680FFFF>Medium</color> To The Mediate Target", true);
            DeadRevealed = new CustomStringOption(true, num++, MultiMenu.crew, "Who Is Revealed With Mediate", new[] {"Oldest Dead", "Newest Dead", "All Dead"});

            Seer = new CustomHeaderOption(num++, MultiMenu.crew, "<color=#71368AFF>Seer</color>");
            SeerCount = new CustomNumberOption(true, num++, MultiMenu.crew, "<color=#71368AFF>Seer</color> Count", 1, 1, 14, 1);
            UniqueSeer = new CustomToggleOption(true, num++, MultiMenu.crew, "<color=#71368AFF>Seer</color> Is Unique In All Any", false);
            SeerCooldown = new CustomNumberOption(true, num++, MultiMenu.crew, "Seer Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);

            Sheriff = new CustomHeaderOption(num++, MultiMenu.crew, "<color=#FFCC80FF>Sheriff</color>");
            SheriffCount = new CustomNumberOption(true, num++, MultiMenu.crew, "<color=#FFCC80FF>Sheriff</color> Count", 1, 1, 14, 1);
            UniqueSheriff = new CustomToggleOption(true, num++, MultiMenu.crew, "<color=#FFCC80FF>Sheriff</color> Is Unique In All Any", false);
            InterrogateCooldown = new CustomNumberOption(true, num++, MultiMenu.crew, "Interrogate Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            NeutEvilRed = new CustomToggleOption(true, num++, MultiMenu.crew, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Evils</color> Show Evil", false);
            NeutKillingRed = new CustomToggleOption(true, num++, MultiMenu.crew, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killings</color> Show Evil", false);

            Tracker = new CustomHeaderOption(num++, MultiMenu.crew, "<color=#009900FF>Tracker</color>");
            TrackerCount = new CustomNumberOption(true, num++, MultiMenu.crew, "<color=#009900FF>Tracker</color> Count", 1, 1, 14, 1);
            UniqueTracker = new CustomToggleOption(true, num++, MultiMenu.crew, "<color=#009900FF>Tracker</color> Is Unique In All Any", false);
            UpdateInterval = new CustomNumberOption(true, num++, MultiMenu.crew, "Arrow Update Interval", 5f, 0f, 15f, 0.5f, CooldownFormat);
            TrackCooldown = new CustomNumberOption(true, num++, MultiMenu.crew, "Track Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            ResetOnNewRound = new CustomToggleOption(true, num++, MultiMenu.crew, "<color=#009900FF>Tracker</color> Arrows Reset After Each Round", false);
            MaxTracks = new CustomNumberOption(true, num++, MultiMenu.crew, "Track Count Per Round", 5, 1, 15, 1);

            Operative = new CustomHeaderOption(num++, MultiMenu.crew, "<color=#A7D1B3FF>Operative</color>");
            OperativeCount = new CustomNumberOption(true, num++, MultiMenu.crew, "<color=#A7D1B3FF>Operative</color> Count", 1, 1, 14, 1);
            UniqueOperative = new CustomToggleOption(true, num++, MultiMenu.crew, "<color=#A7D1B3FF>Operative</color> Is Unique In All Any", false);
            MinAmountOfTimeInBug = new CustomNumberOption(true, num++, MultiMenu.crew, "Min Amount Of Time In Bug To Register", 0f, 0f, 15f, 0.5f, CooldownFormat);
            BugCooldown = new CustomNumberOption(true, num++, MultiMenu.crew, "Bug Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            BugsRemoveOnNewRound = new CustomToggleOption(true, num++, MultiMenu.crew, "Bugs Are Removed Each Round", true);
            MaxBugs = new CustomNumberOption(true, num++, MultiMenu.crew, "Bug Count", 5, 1, 15, 1);
            BugRange = new CustomNumberOption(true, num++, MultiMenu.crew, "Bug Range", 0.25f, 0.05f, 1f, 0.05f, MultiplierFormat);
            MinAmountOfPlayersInBug = new CustomNumberOption(true, num++, MultiMenu.crew, "Number Of Roles Required To Trigger Bug", 3, 1, 5, 1);

            CrewKillingSettings = new CustomHeaderOption(num++, MultiMenu.crew, "<color=#8BFDFDFF>Crew</color> <color=#1D7CF2FF>Killing</color> Settings");
            CKMax = new CustomNumberOption(true, num++, MultiMenu.crew, "Max <color=#8BFDFDFF>Crew</color> <color=#1D7CF2FF>Killing</color> Roles", 1, 1, 14, 1);
            CKMin = new CustomNumberOption(true, num++, MultiMenu.crew, "Min <color=#8BFDFDFF>Crew</color> <color=#1D7CF2FF>Killing</color> Roles", 1, 1, 14, 1);

            Veteran = new CustomHeaderOption(num++, MultiMenu.crew, "<color=#998040FF>Veteran</color>");
            VeteranCount = new CustomNumberOption(true, num++, MultiMenu.crew, "<color=#998040FF>Veteran</color> Count", 1, 1, 14, 1);
            UniqueVeteran = new CustomToggleOption(true, num++, MultiMenu.crew, "<color=#998040FF>Veteran</color> Is Unique In All Any", false);
            AlertCooldown = new CustomNumberOption(true, num++, MultiMenu.crew, "Alert Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            AlertDuration = new CustomNumberOption(true, num++, MultiMenu.crew, "Alert Duration", 10f, 5f, 30f, 1f, CooldownFormat);
            MaxAlerts = new CustomNumberOption(true, num++, MultiMenu.crew, "Number Of Alerts", 5, 1, 15, 1);

            Vigilante = new CustomHeaderOption(num++, MultiMenu.crew, "<color=#FFFF00FF>Vigilante</color>");
            VigilanteCount = new CustomNumberOption(true, num++, MultiMenu.crew, "<color=#FFFF00FF>Vigilante</color> Count", 1, 1, 14, 1);
            UniqueVigilante = new CustomToggleOption(true, num++, MultiMenu.crew, "<color=#FFFF00FF>Vigilante</color> Is Unique In All Any", false);
            MisfireKillsInno = new CustomToggleOption(true, num++, MultiMenu.crew, "<color=#FFFF00FF>Vigilante</color> Misfire Kills The Innocent Target", true);
            VigiKillAgain = new CustomToggleOption(true, num++, MultiMenu.crew, "<color=#FFFF00FF>Vigilante</color> Can Kill Again If Target Was Innocent", true);
            RoundOneNoShot = new CustomToggleOption(true, num++, MultiMenu.crew, "<color=#FFFF00FF>Vigilante</color> Cannot Shoot On The First Round", true);
            VigiNotifOptions = new CustomStringOption(true, num++, MultiMenu.crew, "How Is The <color=#FFFF00FF>Vigilante</color> Notified Of Their Target's Innocence", new[] {"Never", "Flash", "Message"});
            VigiOptions = new CustomStringOption(true, num++, MultiMenu.crew, "How Does <color=#FFFF00FF>Vigilante</color> Die", new[] {"Immediately", "Pre-Meeting", "Post-Meeting"});
            VigiKillCd = new CustomNumberOption(true, num++, MultiMenu.crew, "Shoot Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);

            CrewProtectiveSettings = new CustomHeaderOption(num++, MultiMenu.crew, "<color=#8BFDFDFF>Crew</color> <color=#1D7CF2FF>Protective</color> Settings");
            CPMax = new CustomNumberOption(true, num++, MultiMenu.crew, "Max <color=#8BFDFDFF>Crew</color> <color=#1D7CF2FF>Protective</color> Roles", 1, 1, 14, 1);
            CPMin = new CustomNumberOption(true, num++, MultiMenu.crew, "Min <color=#8BFDFDFF>Crew</color> <color=#1D7CF2FF>Protective</color> Roles", 1, 1, 14, 1);

            Altruist = new CustomHeaderOption(num++, MultiMenu.crew, "<color=#660000FF>Altruist</color>");
            AltruistCount = new CustomNumberOption(true, num++, MultiMenu.crew, "<color=#660000FF>Altruist</color> Count", 1, 1, 14, 1);
            UniqueAltruist = new CustomToggleOption(true, num++, MultiMenu.crew, "<color=#660000FF>Altruist</color> Is Unique In All Any", false);
            AltReviveDuration = new CustomNumberOption(true, num++, MultiMenu.crew, "<color=#660000FF>Altruist</color> Revive Duration", 10f, 1f, 15f, 1f, CooldownFormat);
            AltruistTargetBody = new CustomToggleOption(true, num++, MultiMenu.crew, "Target's Body Disappears On Beginning Of Revive", false);

            Medic = new CustomHeaderOption(num++, MultiMenu.crew, "<color=#006600FF>Medic</color>");
            MedicCount = new CustomNumberOption(true, num++, MultiMenu.crew, "<color=#006600FF>Medic</color> Count", 1, 1, 14, 1);
            UniqueMedic = new CustomToggleOption(true, num++, MultiMenu.crew, "<color=#006600FF>Medic</color> Is Unique In All Any", false);
            ShowShielded = new CustomStringOption(true, num++, MultiMenu.crew, "Show Shielded Player", new[] {"Self", "Medic", "Self+Medic", "Everyone"});
            WhoGetsNotification = new CustomStringOption(true, num++, MultiMenu.crew, "Who Gets Murder Attempt Indicator", new[] {"Medic", "Self", "Self+Medic", "Everyone", "Nobody"});
            ShieldBreaks = new CustomToggleOption(true, num++, MultiMenu.crew, "Shield Breaks On Murder Attempt", false);

            TimeLord = new CustomHeaderOption(num++, MultiMenu.crew, "<color=#0000FFFF>Time Lord</color>");
            TimeLordCount = new CustomNumberOption(true, num++, MultiMenu.crew, "<color=#0000FFFF>Time Lord</color> Count", 1, 1, 14, 1);
            UniqueTimeLord = new CustomToggleOption(true, num++, MultiMenu.crew, "<color=#0000FFFF>Time Lord</color> Is Unique In All Any", false);
            RewindRevive = new CustomToggleOption(true, num++, MultiMenu.crew, "Revive During Rewind", false);
            TLImmunity = new CustomToggleOption(true, num++, MultiMenu.crew, "<color=#0000FFFF>Time Lord</color> To Rewind And Freeze", false);
            RewindDuration = new CustomNumberOption(true, num++, MultiMenu.crew, "Rewind Duration", 2f, 0.5f, 10f, 0.5f, CooldownFormat);
            RewindCooldown = new CustomNumberOption(true, num++, MultiMenu.crew, "Rewind Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            RewindMaxUses = new CustomNumberOption(true, num++, MultiMenu.crew, "Maximum Number Of Rewinds", 5, 1, 15, 1);

            CrewSovereignSettings = new CustomHeaderOption(num++, MultiMenu.crew, "<color=#8BFDFDFF>Crew</color> <color=#1D7CF2FF>Sovereign</color> Settings");
            CSvMax = new CustomNumberOption(true, num++, MultiMenu.crew, "Max <color=#8BFDFDFF>Crew</color> <color=#1D7CF2FF>Sovereign</color> Roles", 1, 1, 14, 1);
            CSvMin = new CustomNumberOption(true, num++, MultiMenu.crew, "Min <color=#8BFDFDFF>Crew</color> <color=#1D7CF2FF>Sovereign</color> Roles", 1, 1, 14, 1);

            Mayor = new CustomHeaderOption(num++, MultiMenu.crew, "<color=#704FA8FF>Mayor</color>");
            MayorCount = new CustomNumberOption(true, num++, MultiMenu.crew, "<color=#704FA8FF>Mayor</color> Count", 1, 1, 14, 1);
            UniqueMayor = new CustomToggleOption(true, num++, MultiMenu.crew, "<color=#704FA8FF>Mayor</color> Is Unique In All Any", false);
            MayorVoteBank = new CustomNumberOption(true, num++, MultiMenu.crew, "Initial <color=#704FA8FF>Mayor</color> Vote Bank", 2, 0, 10, 1);
            MayorAnonymous = new CustomToggleOption(true, num++, MultiMenu.crew, "Anonymous <color=#704FA8FF>Mayor</color> Votes", false);
            MayorButton = new CustomToggleOption(true, num++, MultiMenu.crew, "<color=#704FA8FF>Mayor</color> Can Button", true);

            Swapper = new CustomHeaderOption(num++, MultiMenu.crew, "<color=#66E666FF>Swapper</color>");
            SwapperCount = new CustomNumberOption(true, num++, MultiMenu.crew, "<color=#66E666FF>Swapper</color> Count", 1, 1, 14, 1);
            UniqueSwapper = new CustomToggleOption(true, num++, MultiMenu.crew, "<color=#66E666FF>Swapper</color> Is Unique In All Any", false);
            SwapperButton = new CustomToggleOption(true, num++, MultiMenu.crew, "<color=#66E666FF>Swapper</color> Can Button", true);
            SwapAfterVoting = new CustomToggleOption(true, num++, MultiMenu.crew, "<color=#66E666FF>Swapper</color> Can Swap After Voting", false);
            SwapSelf = new CustomToggleOption(true, num++, MultiMenu.crew, "<color=#66E666FF>Swapper</color> Can Swap Themself", false);

            CrewSupportSettings = new CustomHeaderOption(num++, MultiMenu.crew, "<color=#8BFDFDFF>Crew</color> <color=#1D7CF2FF>Support</color> Settings");
            CSMax = new CustomNumberOption(true, num++, MultiMenu.crew, "Max <color=#8BFDFDFF>Crew</color> <color=#1D7CF2FF>Support</color> Roles", 1, 1, 14, 1);
            CSMin = new CustomNumberOption(true, num++, MultiMenu.crew, "Min <color=#8BFDFDFF>Crew</color> <color=#1D7CF2FF>Support</color> Roles", 1, 1, 14, 1);

            Chameleon = new CustomHeaderOption(num++, MultiMenu.crew, "<color=#5411F8FF>Chameleon</color>");
            ChameleonCount = new CustomNumberOption(true, num++, MultiMenu.crew, "<color=#5411F8FF>Chameleon</color> Count", 1, 1, 14, 1);
            UniqueChameleon = new CustomToggleOption(true, num++, MultiMenu.crew, "<color=#5411F8FF>Chameleon</color> Is Unique In All Any", false);
            SwoopCooldown = new CustomNumberOption(true, num++, MultiMenu.crew, "Swoop Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            SwoopDuration = new CustomNumberOption(true, num++, MultiMenu.crew, "Swoop Duration", 25f, 10f, 60f, 2.5f, CooldownFormat);

            Engineer = new CustomHeaderOption(num++, MultiMenu.crew, "<color=#FFA60AFF>Engineer</color>");
            EngineerCount = new CustomNumberOption(true, num++, MultiMenu.crew, "<color=#FFA60AFF>Engineer</color> Count", 1, 1, 14, 1);
            UniqueEngineer = new CustomToggleOption(true, num++, MultiMenu.crew, "<color=#FFA60AFF>Engineer</color> Is Unique In All Any", false);
            MaxFixes = new CustomNumberOption(true, num++, MultiMenu.crew, "<color=#FFA60AFF>Engineer</color> Fix Count", 1, 1, 14, 1);

            Escort = new CustomHeaderOption(num++, MultiMenu.crew, "<color=#803333FF>Escort</color>");
            EscortCount = new CustomNumberOption(true, num++, MultiMenu.crew, "<color=#803333FF>Escort</color> Count", 1, 1, 14, 1);
            UniqueEscort = new CustomToggleOption(true, num++, MultiMenu.crew, "<color=#803333FF>Escort</color> Is Unique In All Any", false);
            EscRoleblockCooldown = new CustomNumberOption(true, num++, MultiMenu.crew, "Roleblock Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            EscRoleblockDuration = new CustomNumberOption(true, num++, MultiMenu.crew, "Roleblock Duration", 25f, 10f, 60f, 2.5f, CooldownFormat);

            Retributionist = new CustomHeaderOption(num++, MultiMenu.crew, "<color=#8D0F8CFF>Retributionist</color>");
            RetributionistCount = new CustomNumberOption(true, num++, MultiMenu.crew, "<color=#8D0F8CFF>Retributionist</color> Count", 1, 1, 14, 1);
            UniqueRetributionist = new CustomToggleOption(true, num++, MultiMenu.crew, "<color=#8D0F8CFF>Retributionist</color> Is Unique In All Any", false);

            Shifter = new CustomHeaderOption(num++, MultiMenu.crew, "<color=#DF851FFF>Shifter</color>");
            ShifterCount = new CustomNumberOption(true, num++, MultiMenu.crew, "<color=#DF851FFF>Shifter</color> Count", 1, 1, 14, 1);
            UniqueShifter = new CustomToggleOption(true, num++, MultiMenu.crew, "<color=#DF851FFF>Shifter</color> Is Unique In All Any", false);
            ShifterCd = new CustomNumberOption(true, num++, MultiMenu.crew, "Shift Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            ShiftedBecomes = new CustomStringOption(true, num++, MultiMenu.crew, "Shifted Becomes", new[] {"Shifter", "Crewmate"});

            Transporter = new CustomHeaderOption(num++, MultiMenu.crew, "<color=#00EEFFFF>Transporter</color>");
            TransporterCount = new CustomNumberOption(true, num++, MultiMenu.crew, "<color=#00EEFFFF>Transporter</color> Count", 1, 1, 14, 1);
            UniqueTransporter = new CustomToggleOption(true, num++, MultiMenu.crew, "<color=#00EEFFFF>Transporter</color> Is Unique In All Any", false);
            TransportCooldown = new CustomNumberOption(true, num++, MultiMenu.crew, "Transport Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            TransportMaxUses = new CustomNumberOption(true, num++, MultiMenu.crew, " Number Of Transports", 5, 1, 15, 1);

            CrewUtilitySettings = new CustomHeaderOption(num++, MultiMenu.crew, "<color=#8BFDFDFF>Crew</color> <color=#1D7CF2FF>Utility</color> Settings");

            Crewmate = new CustomHeaderOption(num++, MultiMenu.crew, "<color=#8BFDFDFF>Crewmate</color>");
            CrewCount = new CustomNumberOption(true, num++, MultiMenu.crew, "<color=#8BFDFDFF>Crewmate</color> Count", 1, 1, 14, 1);

            Revealer = new CustomHeaderOption(num++, MultiMenu.crew, "<color=#D3D3D3FF>Revealer</color>");
            RevealerTasksRemainingClicked = new CustomNumberOption(true, num++, MultiMenu.crew, "Tasks Remaining When <color=#D3D3D3FF>Revealer</color> Can Be Clicked", 5, 1, 10, 1);
            RevealerTasksRemainingAlert = new CustomNumberOption(true, num++, MultiMenu.crew, "Tasks Remaining When Revealed", 1, 1, 5, 1);
            RevealerRevealsNeutrals = new CustomToggleOption(true, num++, MultiMenu.crew, "<color=#D3D3D3FF>Revealer</color> Reveals <color=#B3B3B3FF>Neutrals</color>", false);
            RevealerRevealsCrew = new CustomToggleOption(true, num++, MultiMenu.crew, "<color=#D3D3D3FF>Revealer</color> Reveals <color=#8BFDFDFF>Crew</color>", false);
            RevealerRevealsRoles = new CustomToggleOption(true, num++, MultiMenu.crew, "<color=#D3D3D3FF>Revealer</color> Reveals Exact <color=#FFD700FF>Roles</color>", false);
            RevealerCanBeClickedBy = new CustomStringOption(true, num++, MultiMenu.crew, "Who Can Click <color=#D3D3D3FF>Revealer</color>", new[] {"All", "Non-Crew", "Imps Only"});

            NeutralSettings = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> Settings");
            CustomNeutColors = new CustomToggleOption(true, num++, MultiMenu.neutral, "Enable Custom <color=#B3B3B3FF>Neutral</color> Colors", true);
            NeutralVision = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> Vision", 1f, 0.25f, 5f, 0.25f, MultiplierFormat);
            LightsAffectNeutrals = new CustomToggleOption(true, num++, MultiMenu.neutral, "Lights Sabotage Affects <color=#B3B3B3FF>Neutral</color> Vision", true);
            NeutralMax = new CustomNumberOption(true, num++, MultiMenu.neutral, "Max <color=#B3B3B3FF>Neutral</color> <color=#FFD700FF>Roles</color>", 5, 1, 14, 1);
            NeutralMin = new CustomNumberOption(true, num++, MultiMenu.neutral, "Min <color=#B3B3B3FF>Neutral</color> <color=#FFD700FF>Roles</color>", 5, 1, 14, 1);
            NoSolo = new CustomStringOption(true, num++, MultiMenu.neutral, "<color=#B3B3B3FF>Neutrals</color> Together, Strong", new[] {"Never", "Same NKs", "Same Roles", "All NKs", "All Neutrals"});

            NeutralBenignSettings = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Benign</color> Settings");
            NBMax = new CustomNumberOption(true, num++, MultiMenu.neutral, "Max <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Benign</color> Roles", 1, 1, 14, 1);
            NBMin = new CustomNumberOption(true, num++, MultiMenu.neutral, "Min <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Benign</color> Roles", 1, 1, 14, 1);
            VigiKillsNB = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#FFFF00FF>Vigilante</color> Kills <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Benigns</color>", false);

            Amnesiac = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#22FFFFFF>Amnesiac</color>");
            AmnesiacCount = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#22FFFFFF>Amnesiac</color> Count", 1, 1, 14, 1);
            UniqueAmnesiac = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#22FFFFFF>Amnesiac</color> Is Unique In All Any", false);
            RememberArrows = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#22FFFFFF>Amnesiac</color> Gets Arrows To Dead Bodies", false);
            RememberArrowDelay = new CustomNumberOption(true, num++, MultiMenu.neutral, "Time After Death Arrow Appears", 5f, 0f, 15f, 1f, CooldownFormat);
            AmneVent = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#22FFFFFF>Amnesiac</color> Can Hide In Vents", false);
            AmneSwitchVent = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#22FFFFFF>Amnesiac</color> Can Switch Vents", false);
            AmneTurnAssassin = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#22FFFFFF>Amnesiac</color> Can Become <color=#073763FF>Assassin</color>", false);

            GuardianAngel = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#FFFFFFFF>Guardian Angel</color>");
            GuardianAngelCount = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#FFFFFFFF>Guardian Angel</color> Count", 1, 1, 14, 1);
            UniqueGuardianAngel = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#FFFFFFFF>Guardian Angel</color> Is Unique In All Any", false);
            ProtectCd = new CustomNumberOption(true, num++, MultiMenu.neutral, "Protect Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            ProtectDuration = new CustomNumberOption(true, num++, MultiMenu.neutral, "Protect Duration", 10f, 5f, 15f, 1f, CooldownFormat);
            ProtectKCReset = new CustomNumberOption(true, num++, MultiMenu.neutral, "Kill Cooldown Reset When Protected", 2.5f, 0f, 15f, 0.5f, CooldownFormat);
            MaxProtects = new CustomNumberOption(true, num++, MultiMenu.neutral, "Maximum Number Of Protects", 5, 1, 15, 1);
            ShowProtect = new CustomStringOption(true, num++, MultiMenu.neutral, "Show Protected Player", new[] {"Self", "Guardian Angel", "Self+GA", "Everyone"});
            GATargetKnows = new CustomToggleOption(true, num++, MultiMenu.neutral, "Target Knows <color=#FFFFFFFF>Guardian Angel</color> Exists", false);
            ProtectBeyondTheGrave = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#FFFFFFFF>Guardian Angel</color> Can Protect After Death", false);
            GAKnowsTargetRole = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#FFFFFFFF>Guardian Angel</color> Knows Target's <color=#FFD700FF>Role</color>", false);
            GAVent = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#FFFFFFFF>Guardian Angel</color> Can Hide In Vents", false);
            GASwitchVent = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#FFFFFFFF>Guardian Angel</color> Can Switch Vents", false);

            Survivor = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#DDDD00FF>Survivor</color>");
            SurvivorCount = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#DDDD00FF>Survivor</color> Count", 1, 1, 14, 1);
            UniqueSurvivor = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#DDDD00FF>Survivor</color> Is Unique In All Any", false);
            VestCd = new CustomNumberOption(true, num++, MultiMenu.neutral, "Vest Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            VestDuration = new CustomNumberOption(true, num++, MultiMenu.neutral, "Vest Duration", 10f, 5f, 15f, 1f, CooldownFormat);
            VestKCReset = new CustomNumberOption(true, num++, MultiMenu.neutral, "Kill Cooldown Reset On Attack", 2.5f, 0f, 15f, 0.5f, CooldownFormat);
            MaxVests = new CustomNumberOption(true, num++, MultiMenu.neutral, "Number Of Vests", 5, 1, 15, 1);
            SurvVent = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#DDDD00FF>Survivor</color> Can Hide In Vents", false);
            SurvSwitchVent = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#DDDD00FF>Survivor</color> Can Switch Vents", false);

            Thief = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#80FF00FF>Thief</color>");
            ThiefCount = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#80FF00FF>Thief</color> Count", 1, 1, 14, 1);
            UniqueThief = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#80FF00FF>Thief</color> Is Unique In All Any", false);
            ThiefKillCooldown = new CustomNumberOption(true, num++, MultiMenu.neutral, "Kill Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            ThiefSteals = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#80FF00FF>Thief</color> Assigns <color=#80FF00FF>Thief</color> <color=#FFD700FF>Role</color> To Target", false);
            ThiefVent = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#80FF00FF>Thief</color> Can Vent", false);

            NeutralEvilSettings = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Evil</color> Settings");
            NEMax = new CustomNumberOption(true, num++, MultiMenu.neutral, "Max <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Evil</color> Roles", 1, 1, 14, 1);
            NEMin = new CustomNumberOption(true, num++, MultiMenu.neutral, "Min <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Evil</color> Roles", 1, 1, 14, 1);

            Actor = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#00ACC2FF>Actor</color>");
            ActorCount = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#00ACC2FF>Actor</color> Count", 1, 1, 14, 1);
            UniqueActor= new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#00ACC2FF>Actor</color> Is Unique In All Any", false);
            ActorButton = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#00ACC2FF>Actor</color> Can Button", true);
            ActorVent = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#00ACC2FF>Actor</color> Can Hide In Vents", false);
            ActSwitchVent = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#00ACC2FF>Actor</color> Can Switch Vents", false);
            VigiKillsActor = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#FFFF00FF>Vigilante</color> Kills <color=#00ACC2FF>Actor</color>", false);

            BountyHunter = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#B51E39FF>Bounty Hunter</color>");
            BHCount = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#B51E39FF>Bounty Hunter</color> Count", 1, 1, 14, 1);
            UniqueBountyHunter = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#B51E39FF>Bounty Hunter</color> Is Unique In All Any", false);
            BountyHunterCooldown= new CustomNumberOption(true, num++, MultiMenu.neutral, "Guess Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            BountyHunterGuesses = new CustomNumberOption(true, num++, MultiMenu.neutral, "Guess Count", 5, 1, 15, 1);
            BHVent = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#B51E39FF>Bounty Hunter</color> Can Vent", false);

            Cannibal = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#8C4005FF>Cannibal</color>"); 
            CannibalCount = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#8C4005FF>Cannibal</color> Count", 1, 1, 14, 1);
            UniqueCannibal = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#8C4005FF>Cannibal</color> Is Unique In All Any", false);
            CannibalCd = new CustomNumberOption(true, num++, MultiMenu.neutral, "Eat Cooldown", 10f, 10f, 60f, 2.5f, CooldownFormat);
            CannibalBodyCount = new CustomNumberOption(true, num++, MultiMenu.neutral, "Number Of Bodies To Eat", 1, 1, 5, 1);
            CannibalVent = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#8C4005FF>Cannibal</color> Can Vent", false);
            EatArrows = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#8C4005FF>Cannibal</color> Gets Arrows To Dead Bodies", false);
            EatArrowDelay = new CustomNumberOption(true, num++, MultiMenu.neutral, "Time After Death Arrow Appears", 5f, 0f, 15f, 1f, CooldownFormat);
            VigiKillsCannibal = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#FFFF00FF>Vigilante</color> Kills <color=#8C4005FF>Cannibal</color>", false);

            Executioner = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#CCCCCCFF>Executioner</color>");
            ExecutionerCount = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#CCCCCCFF>Executioner</color> Count", 1, 1, 14, 1);
            UniqueExecutioner = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#CCCCCCFF>Executioner</color> Is Unique In All Any", false);
            ExecutionerButton = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#CCCCCCFF>Executioner</color> Can Button", true);
            ExeVent = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#CCCCCCFF>Executioner</color> Can Hide In Vents", false);
            ExeSwitchVent = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#CCCCCCFF>Executioner</color> Can Switch Vents", false);
            ExeTargetKnows = new CustomToggleOption(true, num++, MultiMenu.neutral, "Target Knows <color=#CCCCCCFF>Executioner</color> Exists", false);
            ExeKnowsTargetRole = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#CCCCCCFF>Executioner</color> Knows Target's <color=#FFD700FF>Role</color>", false);
            ExeEjectScreen = new CustomToggleOption(true, num++, MultiMenu.neutral, "Target Ejection Reveals Existence Of <color=#CCCCCCFF>Executioner</color>", false);
            ExeCanHaveIntruderTargets = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#CCCCCCFF>Executioner</color> Can Have <color=#FF0000FF>Intruder</color> Targets", false);
            ExeCanHaveSyndicateTargets = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#CCCCCCFF>Executioner</color> Can Have <color=#008000FF>Syndicate</color> Targets", false);
            ExeCanHaveNeutralTargets = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#CCCCCCFF>Executioner</color> Can Have <color=#B3B3B3FF>Neutral</color> Targets", false);
            ExeCanWinBeyondDeath = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#CCCCCCFF>Executioner</color> Can Win After Death", false);
            VigiKillsExecutioner = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#FFFF00FF>Vigilante</color> Kills <color=#CCCCCCFF>Executioner</color>", false);

            Guesser = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#EEE5BEFF>Guesser</color>");
            GuesserCount = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#EEE5BEFF>Guesser</color> Count", 1, 1, 14, 1);
            UniqueGuesser = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#EEE5BEFF>Guesser</color> Is Unique In All Any", false);
            GuesserButton = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#EEE5BEFF>Guesser</color> Can Button", true);
            GuessVent = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#EEE5BEFF>Guesser</color> Can Hide In Vents", false);
            GuessSwitchVent = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#EEE5BEFF>Guesser</color> Can Switch Vents", false);
            GuessTargetKnows = new CustomToggleOption(true, num++, MultiMenu.neutral, "Target Knows <color=#EEE5BEFF>Guesser</color> Exists", false);
            MultipleGuesses = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#EEE5BEFF>Guesser</color> Can Guess Multiple Times", true);
            GuessCount = new CustomNumberOption(true, num++, MultiMenu.neutral, "Number Of Guesses", 1, 1, 14, 1);
            GuesserAfterVoting = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#EEE5BEFF>Guesser</color> Can Guess After Voting", false);
            VigiKillsGuesser = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#FFFF00FF>Vigilante</color> Kills <color=#EEE5BEFF>Guesser</color>", false);

            Jester = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#F7B3DAFF>Jester</color>");
            JesterCount = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#F7B3DAFF>Jester</color> Count", 1, 1, 14, 1);
            UniqueJester= new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#F7B3DAFF>Jester</color> Is Unique In All Any", false);
            JesterButton = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#F7B3DAFF>Jester</color> Can Button", true);
            JesterVent = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#F7B3DAFF>Jester</color> Can Hide In Vents", false);
            JestSwitchVent = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#F7B3DAFF>Jester</color> Can Switch Vents", false);
            JestEjectScreen = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#F7B3DAFF>Jester</color> Target Ejection Reveals Existence Of <color=#F7B3DAFF>Jester</color>", false);
            VigiKillsJester = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#FFFF00FF>Vigilante</color> Kills <color=#F7B3DAFF>Jester</color>", false);

            Troll = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#678D36FF>Troll</color>");
            TrollCount = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#678D36FF>Troll</color> Count", 1, 1, 14, 1);
            UniqueTroll = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#678D36FF>Troll</color> Is Unique In All Any", false);
            InteractCooldown = new CustomNumberOption(true, num++, MultiMenu.neutral, "Interact Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            TrollVent = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#678D36FF>Troll</color> Can Vent", false);
            TrollSwitchVent = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#678D36FF>Troll</color> Can Switch Vents", false);

            NeutralKillingSettings = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killing</color> Settings");
            NKMax = new CustomNumberOption(true, num++, MultiMenu.neutral, "Max <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killing</color> Roles", 1, 1, 14, 1);
            NKMin = new CustomNumberOption(true, num++, MultiMenu.neutral, "Min <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killing</color> Roles", 1, 1, 14, 1);
            NKHasImpVision = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killers</color> Have <color=#FF0000FF>Intruder</color> Vision", true);
            NKsKnow = new CustomStringOption(true, num++, MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killers</color> Know And Can Win With Each Other", new[] {"Never", "Same Roles", "All NKs"});

            Arsonist = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#EE7600FF>Arsonist</color>");
            ArsonistCount = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#EE7600FF>Arsonist</color> Count", 1, 1, 14, 1);
            UniqueArsonist = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#EE7600FF>Arsonist</color> Is Unique In All Any", false);
            DouseCooldown = new CustomNumberOption(true, num++, MultiMenu.neutral, "Douse Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            IgniteCooldown = new CustomNumberOption(true, num++, MultiMenu.neutral, "Ignite Cooldown", 25f, 5f, 60f, 2.5f, CooldownFormat);
            ArsoLastKillerBoost = new CustomToggleOption(true, num++, MultiMenu.neutral, "Ignite Cooldown Removed When <color=#EE7600FF>Arsonist</color> Is Last Killer", false);
            ArsoCooldownsLinked = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#EE7600FF>Arsonist</color> Douse And Ignite Cooldowns Are Linked", false);
            ArsoVent = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#EE7600FF>Arsonist</color> Can Vent", false);

            Cryomaniac = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#642DEAFF>Cryomaniac</color>");
            CryomaniacCount = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#642DEAFF>Cryomaniac</color> Count", 1, 1, 14, 1);
            UniqueCryomaniac = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#642DEAFF>Cryomaniac</color> Is Unique In All Any", false);
            CryoDouseCooldown = new CustomNumberOption(true, num++, MultiMenu.neutral, "Douse Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            CryoVent = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#642DEAFF>Cryomaniac</color> Can Vent", false);

            Glitch = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#00FF00FF>Glitch</color>");
            GlitchCount = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#00FF00FF>Glitch</color> Count", 1, 1, 14, 1);
            UniqueGlitch = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#00FF00FF>Glitch</color> Is Unique In All Any", false);
            MimicCooldown = new CustomNumberOption(true, num++, MultiMenu.neutral, "Mimic Cooldown", 25f, 10f, 120f, 2.5f, CooldownFormat);
            HackCooldown = new CustomNumberOption(true, num++, MultiMenu.neutral, "Hack Cooldown", 25f, 10f, 120f, 2.5f, CooldownFormat);
            MimicDuration = new CustomNumberOption(true, num++, MultiMenu.neutral, "Mimic Duration", 10f, 1f, 30f, 1f, CooldownFormat);
            HackDuration = new CustomNumberOption(true, num++, MultiMenu.neutral, "Hack Duration", 10f, 1f, 30f, 1f, CooldownFormat);
            GlitchKillCooldown = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#00FF00FF>Glitch</color> Kill Cooldown", 25f, 10f, 120f, 2.5f, CooldownFormat);
            GlitchVent = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#00FF00FF>Glitch</color> Can Vent", false);

            Juggernaut = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#A12B56FF>Juggernaut</color>");
            JuggernautCount = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#A12B56FF>Juggernaut</color> Count", 1, 1, 14, 1);
            UniqueJuggernaut = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#A12B56FF>Juggernaut</color> Is Unique In All Any", false);
            JuggKillCooldown = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#A12B56FF>Juggernaut</color> Kill Cooldown", 25f, 10f, 120f, 2.5f, CooldownFormat);
            JuggKillBonus = new CustomNumberOption(true, num++, MultiMenu.neutral, "Kill Cooldown Bonus", 5f, 2.5f, 30f, 2.5f, CooldownFormat);
            JuggVent = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#A12B56FF>Juggernaut</color> Can Vent", false);

            Murderer = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#6F7BEAFF>Murderer</color>");
            MurdCount = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#6F7BEAFF>Murderer</color> Count", 1, 1, 14, 1);
            UniqueMurderer = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#6F7BEAFF>Murderer</color> Is Unique In All Any", false);
            MurdKillCooldownOption = new CustomNumberOption(true, num++, MultiMenu.neutral, "Kill Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            MurdVent = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#6F7BEAFF>Murderer</color> Can Vent", false);

            Plaguebearer = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#CFFE61FF>Plaguebearer</color>");
            PlaguebearerCount = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#CFFE61FF>Plaguebearer</color> Count", 1, 1, 14, 1);
            UniquePlaguebearer = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#CFFE61FF>Plaguebearer</color> Is Unique In All Any", false);
            InfectCooldown = new CustomNumberOption(true, num++, MultiMenu.neutral, "Infect Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            PBVent = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#CFFE61FF>Plaguebearer</color> Can Vent", false);

            SerialKiller = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#336EFFFF>Serial Killer</color>");
            SKCount = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#336EFFFF>Serial Killer</color> Count", 1, 1, 14, 1);
            UniqueSerialKiller = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#336EFFFF>Serial Killer</color> Is Unique In All Any", false);
            BloodlustCooldown = new CustomNumberOption(true, num++, MultiMenu.neutral, "Bloodlust Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            BloodlustDuration = new CustomNumberOption(true, num++, MultiMenu.neutral, "Bloodlust Duration", 25f, 10f, 60f, 2.5f, CooldownFormat);
            LustKillCooldown = new CustomNumberOption(true, num++, MultiMenu.neutral, "Stab Cooldown", 10f, 0.5f, 15f, 0.5f, CooldownFormat);
            SKVentOptions = new CustomStringOption(true, num++, MultiMenu.neutral, "<color=#336EFFFF>Serial Killer</color> Can Vent", new[] {"Always", "Bloodlust", "No Lust", "Never"});

            Werewolf = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#9F703AFF>Werewolf</color>");
            WerewolfCount = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#9F703AFF>Werewolf</color> Count", 1, 1, 14, 1);
            UniqueWerewolf = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#9F703AFF>Werewolf</color> Is Unique In All Any", false);
            MaulCooldown = new CustomNumberOption(true, num++, MultiMenu.neutral, "Maul Cooldown", 30f, 10f, 60f, 2.5f, CooldownFormat);
            MaulRadius = new CustomNumberOption(true, num++, MultiMenu.neutral, "Maul Radius", 1f, 0.25f, 5f, 0.25f, MultiplierFormat);
            WerewolfVent = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#9F703AFF>Werewolf</color> Can Vent", false);

            NeutralNeophyteSettings = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Neophyte</color> Settings");
            NNMax = new CustomNumberOption(true, num++, MultiMenu.neutral, "Max <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Neophyte</color> Roles", 1, 1, 14, 1);
            NNMin = new CustomNumberOption(true, num++, MultiMenu.neutral, "Min <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Neophyte</color> Roles", 1, 1, 14, 1);

            Dracula = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#AC8A00FF>Dracula</color>");
            DraculaCount = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#AC8A00FF>Dracula</color> Count", 1, 1, 14, 1);
            UniqueDracula = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#AC8A00FF>Dracula</color> Is Unique In All Any", false);
            BiteCooldown = new CustomNumberOption(true, num++, MultiMenu.neutral, "Bite Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            DraculaConvertNeuts = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#AC8A00FF>Dracula</color> Can Convert <color=#B3B3B3FF>Neutrals</color>", false);
            DracVent = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#AC8A00FF>Dracula</color> Can Vent", false);
            AliveVampCount = new CustomNumberOption(true, num++, MultiMenu.neutral, "Alive <color=#7B8968FF>Undead</color> Count", 3, 1, 14, 1);

            Jackal = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#45076AFF>Jackal</color>");
            JackalCount = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#45076AFF>Jackal</color> Count", 1, 1, 14, 1);
            UniqueJackal = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#45076AFF>Jackal</color> Is Unique In All Any", false);
            RecruitCooldown = new CustomNumberOption(true, num++, MultiMenu.neutral, "Recruit Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            JackalVent = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#45076AFF>Jackal</color> Can Vent", false);
            RecruitVent = new CustomToggleOption(true, num++, MultiMenu.neutral, "Recruits Can Vent", false);

            Necromancer = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#FF1919FF>Necromancer</color>");
            NecromancerCount = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#FF1919FF>Necromancer</color> Count", 1, 1, 14, 1);
            UniqueNecromancer = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#FF1919FF>Necromancer</color> Is Unique In All Any", false);
            ResurrectCooldown = new CustomNumberOption(true, num++, MultiMenu.neutral, "Resurrect Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            ResurrectCooldownIncrease = new CustomNumberOption(true, num++, MultiMenu.neutral, "Resurrect Cooldown Increases By", 5f, 2.5f, 30f, 2.5f, CooldownFormat);
            ResurrectCount = new CustomNumberOption(true, num++, MultiMenu.neutral, "Resurrect Count", 5, 1, 14, 1);
            NecroKillCooldown = new CustomNumberOption(true, num++, MultiMenu.neutral, "Kill Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            NecroKillCooldownIncrease = new CustomNumberOption(true, num++, MultiMenu.neutral, "Kill Cooldown Increases By", 5f, 2.5f, 30f, 2.5f, CooldownFormat);
            NecroKillCount = new CustomNumberOption(true, num++, MultiMenu.neutral, "Kill Count", 5, 1, 14, 1);
            KillResurrectCooldownsLinked = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#FF1919FF>Necromancer</color> Kill And Resurrect Cooldowns Are Linked", false);
            NecromancerTargetBody = new CustomToggleOption(true, num++, MultiMenu.neutral, "Target's Body Disappears On Beginning Of Resurrect", false);
            NecroResurrectDuration = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#FF1919FF>Necromancer</color> Resurrect Duration", 10f, 1f, 15f, 1f, CooldownFormat);
            NecroVent = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#FF1919FF>Necromancer</color> Can Vent", false);

            Whisperer = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#2D6AA5FF>Whisperer</color>");
            WhispererCount = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#2D6AA5FF>Whisperer</color> Count", 1, 1, 14, 1);
            UniqueWhisperer = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#2D6AA5FF>Whisperer</color> Is Unique In All Any", false);
            WhisperCooldown = new CustomNumberOption(true, num++, MultiMenu.neutral, "Whisper Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            WhisperCooldownIncrease = new CustomNumberOption(true, num++, MultiMenu.neutral, "Whisper Cooldown Increases By", 5f, 2.5f, 30f, 2.5f, CooldownFormat);
            WhisperRadius = new CustomNumberOption(true, num++, MultiMenu.neutral, "Whisper Radius", 1f, 0.25f, 5f, 0.25f, MultiplierFormat);
            WhispVent = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#2D6AA5FF>Whisperer</color> Can Vent", false);
            InitialWhisperRate = new CustomNumberOption(true, num++, MultiMenu.neutral, "Whisper Rate", 5, 5, 50, 5, PercentFormat);
            WhisperRateDecreases = new CustomToggleOption(true, num++, MultiMenu.neutral, "Whisper Rate Decrease Each Whisper", false);
            WhisperRateDecrease = new CustomNumberOption(true, num++, MultiMenu.neutral, "Whisper Rate Decrease", 5, 5, 50, 5, PercentFormat);

            NeutralProselyteSettings = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Proselyte</color> Settings");

            Dampyr = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#DF7AE8FF>Dampyr</color>");
            DampBiteCooldown = new CustomNumberOption(true, num++, MultiMenu.neutral, "Bite Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            DampVent = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#DF7AE8FF>Dampyr</color> Can Vent", false);

            Pestilence = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#424242FF>Pestilence</color>");
            PestSpawn = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#424242FF>Pestilence</color> Can Spawn Directly", false);
            PlayersAlerted = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#424242FF>Pestilence</color> Transform Alerts Everyone", false);
            PestKillCooldown = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#424242FF>Pestilence</color> Kill Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            PestVent = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#424242FF>Pestilence</color> Can Vent", false);

            Phantom = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#662962FF>Phantom</color>");
            PhantomCount = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#662962FF>Phantom</color> Count", 1, 1, 14, 1);
            PhantomTasksRemaining = new CustomNumberOption(true, num++, MultiMenu.neutral, "Tasks Remaining When <color=#662962FF>Phantom</color> Can Be Clicked", 5, 1, 10, 1);
            PhantomPlayersAlerted = new CustomToggleOption(true, num++, MultiMenu.neutral, "Players Are Alerted When <color=#662962FF>Phantom</color> Is Clickable", false);

            Vampire = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#2BD29CFF>Vampire</color>");
            VampVent = new CustomToggleOption(true, num++, MultiMenu.neutral, "<color=#2BD29CFF>Vampire</color> Can Vent", false);

            IntruderSettings = new CustomHeaderOption(num++, MultiMenu.intruder, "<color=#FF0000FF>Intruder</color> Settings");
            CustomIntColors = new CustomToggleOption(true, num++, MultiMenu.intruder, "Enable Custom <color=#FF0000FF>Intruder</color> Colors", true);
            IntruderCount = new CustomNumberOption(true, num++, MultiMenu.intruder, "<color=#FF0000FF>Intruder</color> Count", 1, 0, 4, 1);
            IntruderVision = new CustomNumberOption(true, num++, MultiMenu.intruder, "<color=#FF0000FF>Intruder</color> Vision", 2f, 0.25f, 5f, 0.25f, MultiplierFormat);
            IntruderKillCooldown = new CustomNumberOption(true, num++, MultiMenu.intruder, "<color=#FF0000FF>Intruder</color> Kill Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            IntruderSabotageCooldown = new CustomNumberOption(true, num++, MultiMenu.intruder, "<color=#FF0000FF>Intruder</color> Sabotage Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            IntrudersVent = new CustomToggleOption(true, num++, MultiMenu.intruder, "<color=#FF0000FF>Intruder</color> Can Vent", true);
            IntrudersCanSabotage = new CustomToggleOption(true, num++, MultiMenu.intruder, "<color=#FF0000FF>Intruder</color> Can Sabotage", true);
            IntruderMax = new CustomNumberOption(true, num++, MultiMenu.intruder, "Max <color=#FF0000FF>Intruder</color> <color=#FFD700FF>Roles</color>", 5, 1, 14, 1);
            IntruderMin = new CustomNumberOption(true, num++, MultiMenu.intruder, "Min <color=#FF0000FF>Intruder</color> <color=#FFD700FF>Roles</color>", 5, 1, 14, 1);

            IntruderConcealingSettings = new CustomHeaderOption(num++, MultiMenu.intruder, "<color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Concealing</color> Settings");
            ICMax = new CustomNumberOption(true, num++, MultiMenu.intruder, "Max <color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Concealing</color> Roles", 1, 1, 14, 1);
            ICMin = new CustomNumberOption(true, num++, MultiMenu.intruder, "Min <color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Concealing</color> Roles", 1, 1, 14, 1);

            Blackmailer = new CustomHeaderOption(num++, MultiMenu.intruder, "<color=#02A752FF>Blackmailer</color>");
            BlackmailerCount = new CustomNumberOption(true, num++, MultiMenu.intruder, "<color=#02A752FF>Blackmailer</color> Count", 1, 1, 14, 1);
            UniqueBlackmailer = new CustomToggleOption(true, num++, MultiMenu.intruder, "<color=#02A752FF>Blackmailer</color> Is Unique In All Any", false);
            BlackmailCooldown = new CustomNumberOption(true, num++, MultiMenu.intruder, "Blackmail Cooldown", 10f, 1f, 15f, 1f, CooldownFormat);

            Camouflager = new CustomHeaderOption(num++, MultiMenu.intruder, "<color=#378AC0FF>Camouflager</color>");
            CamouflagerCount = new CustomNumberOption(true, num++, MultiMenu.intruder, "<color=#378AC0FF>Camouflager</color> Count", 1, 1, 14, 1);
            UniqueCamouflager = new CustomToggleOption(true, num++, MultiMenu.intruder, "<color=#378AC0FF>Camouflager</color> Is Unique In All Any", false);
            CamouflagerCooldown = new CustomNumberOption(true, num++, MultiMenu.intruder, "Camouflage Cooldown", 25f, 10f, 40f, 2.5f, CooldownFormat);
            CamouflagerDuration = new CustomNumberOption(true, num++, MultiMenu.intruder, "Camouflage Duration", 10f, 5f, 15f, 1f, CooldownFormat);
            CamoHideSize = new CustomToggleOption(true, num++, MultiMenu.intruder, "Camouflage Hides Player Size", false);
            CamoHideSpeed = new CustomToggleOption(true, num++, MultiMenu.intruder, "Camouflage Hides Player Speed", false);

            Grenadier = new CustomHeaderOption(num++, MultiMenu.intruder, "<color=#85AA5BFF>Grenadier</color>");
            GrenadierCount = new CustomNumberOption(true, num++, MultiMenu.intruder, "<color=#85AA5BFF>Grenadier</color> Count", 1, 1, 14, 1);
            UniqueGrenadier = new CustomToggleOption(true, num++, MultiMenu.intruder, "<color=#85AA5BFF>Grenadier</color> Is Unique In All Any", false);
            GrenadeCooldown = new CustomNumberOption(true, num++, MultiMenu.intruder, "Flash Grenade Cooldown", 25f, 10f, 40f, 2.5f, CooldownFormat);
            GrenadeDuration = new CustomNumberOption(true, num++, MultiMenu.intruder, "Flash Grenade Duration", 10f, 5f, 15f, 1f, CooldownFormat);
            FlashRadius = new CustomNumberOption(true, num++, MultiMenu.intruder, "Flash Radius", 1f, 0.25f, 5f, 0.25f, MultiplierFormat);
            GrenadierIndicators = new CustomToggleOption(true, num++, MultiMenu.intruder, "Indicate Flashed Players", false);
            GrenadierVent = new CustomToggleOption(true, num++, MultiMenu.intruder, "<color=#85AA5BFF>Grenadier</color> Can Vent", false);

            Janitor = new CustomHeaderOption(num++, MultiMenu.intruder, "<color=#2647A2FF>Janitor</color>");
            JanitorCount = new CustomNumberOption(true, num++, MultiMenu.intruder, "<color=#2647A2FF>Janitor</color> Count", 1, 1, 14, 1);
            UniqueJanitor = new CustomToggleOption(true, num++, MultiMenu.intruder, "<color=#2647A2FF>Janitor</color> Is Unique In All Any", false);
            JanitorCleanCd = new CustomNumberOption (true, num++, MultiMenu.intruder, "Clean Cooldown", 25f, 0f, 40f, 2.5f, CooldownFormat);
            JaniCooldownsLinked = new CustomToggleOption(true, num++, MultiMenu.intruder, "<color=#2647A2FF>Janitor</color> Kill And Clean Cooldowns Are Linked", false);
            SoloBoost = new CustomToggleOption(true, num++, MultiMenu.intruder, "<color=#2647A2FF>Janitor</color> Has Lower Cooldown When Solo", false);

            Undertaker = new CustomHeaderOption(num++, MultiMenu.intruder, "<color=#005643FF>Undertaker</color>");
            UndertakerCount = new CustomNumberOption(true, num++, MultiMenu.intruder, "<color=#005643FF>Undertaker</color> Count", 1, 1, 14, 1);
            UniqueUndertaker = new CustomToggleOption(true, num++, MultiMenu.intruder, "<color=#005643FF>Undertaker</color> Is Unique In All Any", false);
            DragCooldown = new CustomNumberOption(true, num++, MultiMenu.intruder, "Drag Cooldown", 25f, 10f, 40f, 2.5f, CooldownFormat);
            DragModifier = new CustomNumberOption(true, num++, MultiMenu.intruder, "Drag Speed", 0.1f, 0.5f, 2f, 0.5f, MultiplierFormat);
            UndertakerVentOptions = new CustomStringOption(true, num++, MultiMenu.intruder, "<color=#005643FF>Undertaker</color> Can Vent", new[] {"Never", "Body", "Bodyless", "Always"});

            IntruderDeceptionSettings = new CustomHeaderOption(num++, MultiMenu.intruder, "<color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Deception</color> Settings");
            IDMax = new CustomNumberOption(true, num++, MultiMenu.intruder, "Max <color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Deception</color> Roles", 1, 1, 14, 1);
            IDMin = new CustomNumberOption(true, num++, MultiMenu.intruder, "Min <color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Deception</color> Roles", 1, 1, 14, 1);

            Disguiser = new CustomHeaderOption(num++, MultiMenu.intruder, "<color=#40B4FFFF>Disguiser</color>");
            DisguiserCount = new CustomNumberOption(true, num++, MultiMenu.intruder, "<color=#40B4FFFF>Disguiser</color> Count", 1, 1, 14, 1);
            UniqueDisguiser = new CustomToggleOption(true, num++, MultiMenu.intruder, "<color=#40B4FFFF>Disguiser</color> Is Unique In All Any", false);
            DisguiseCooldown = new CustomNumberOption(true, num++, MultiMenu.intruder, "Disguise Cooldown", 30f, 10f, 60f, 2.5f, CooldownFormat);
            TimeToDisguise = new CustomNumberOption(true, num++, MultiMenu.intruder, "Delay Before Disguising", 5f, 2.5f, 15f, 2.5f, CooldownFormat);
            DisguiseDuration = new CustomNumberOption(true, num++, MultiMenu.intruder, "Disguise Duration", 10f, 2.5f, 20f, 2.5f, CooldownFormat);
            DisguiseTarget = new CustomStringOption(true, num++, MultiMenu.intruder, "<color=#40B4FFFF>Disguiser</color> Can Disguise", new[] {"Only Intruders", "Non Intruders", "Everyone"});

            Morphling = new CustomHeaderOption(num++, MultiMenu.intruder, "<color=#BB45B0FF>Morphling</color>");
            MorphlingCount = new CustomNumberOption(true, num++, MultiMenu.intruder, "<color=#BB45B0FF>Morphling</color> Count", 1, 1, 14, 1);
            UniqueMorphling = new CustomToggleOption(true, num++, MultiMenu.intruder, "<color=#BB45B0FF>Morphling</color> Is Unique In All Any", false);
            MorphlingCooldown = new CustomNumberOption(true, num++, MultiMenu.intruder, "Morph Cooldown", 25f, 10f, 40f, 2.5f, CooldownFormat);
            MorphlingDuration = new CustomNumberOption(true, num++, MultiMenu.intruder, "Morph Duration", 10f, 5f, 15f, 1f, CooldownFormat);
            MorphlingVent = new CustomToggleOption(true, num++, MultiMenu.intruder, "<color=#BB45B0FF>Morphling</color> Can Vent", false);

            Wraith = new CustomHeaderOption(num++, MultiMenu.intruder, "<color=#5C4F75FF>Wraith</color>");
            WraithCount = new CustomNumberOption(true, num++, MultiMenu.intruder, "<color=#5C4F75FF>Wraith</color> Count", 1, 1, 14, 1);
            UniqueWraith = new CustomToggleOption(true, num++, MultiMenu.intruder, "<color=#5C4F75FF>Wraith</color> Is Unique In All Any", false);
            InvisCooldown = new CustomNumberOption(true, num++, MultiMenu.intruder, "Invis Cooldown", 25f, 10f, 40f, 2.5f, CooldownFormat);
            InvisDuration = new CustomNumberOption(true, num++, MultiMenu.intruder, "Invis Duration", 10f, 5f, 15f, 1f, CooldownFormat);
            WraithVent = new CustomToggleOption(true, num++, MultiMenu.intruder, "<color=#5C4F75FF>Wraith</color> Can Vent", false);

            //IntruderKillingSettings = new CustomHeaderOption(num++, MultiMenu.intruder, "<color=#FF0000FF>Impostor</color> <color=#1D7CF2FF>Killing</color> Settings");
            //IKMax = new CustomNumberOption(true, num++, MultiMenu.intruder, "Max <color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Killing</color> Roles", 1, 1, 14, 1);
            //IKMin = new CustomNumberOption(true, num++, MultiMenu.intruder, "Min <color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Killing</color> Roles", 1, 1, 14, 1);

            IntruderSupportSettings = new CustomHeaderOption(num++, MultiMenu.intruder, "<color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Support</color> Settings");
            ISMax = new CustomNumberOption(true, num++, MultiMenu.intruder, "Max <color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Support</color> Roles", 1, 1, 14, 1);
            ISMin = new CustomNumberOption(true, num++, MultiMenu.intruder, "Min <color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Support</color> Roles", 1, 1, 14, 1);

            Consigliere = new CustomHeaderOption(num++, MultiMenu.intruder, "<color=#FFFF99FF>Consigliere</color>");
            ConsigCount = new CustomNumberOption(true, num++, MultiMenu.intruder, "<color=#FFFF99FF>Consigliere</color> Count", 1, 1, 14, 1);
            UniqueConsigliere = new CustomToggleOption(true, num++, MultiMenu.intruder, "<color=#FFFF99FF>Consigliere</color> Is Unique In All Any", false);
            InvestigateCooldown = new CustomNumberOption(true, num++, MultiMenu.intruder, "Reveal Cooldown", 25f, 10f, 100f, 2.5f, CooldownFormat);
            ConsigInfo = new CustomStringOption(true, num++, MultiMenu.intruder, "Info That <color=#FFFF99FF>Consigliere</color> Sees", new[] {"Role", "Faction"});

            Consort = new CustomHeaderOption(num++, MultiMenu.intruder, "<color=#801780FF>Consort</color>");
            ConsortCount = new CustomNumberOption(true, num++, MultiMenu.intruder, "<color=#801780FF>Consort</color> Count", 1, 1, 14, 1);
            UniqueConsort = new CustomToggleOption(true, num++, MultiMenu.intruder, "<color=#801780FF>Consort</color> Is Unique In All Any", false);
            ConsRoleblockCooldown = new CustomNumberOption(true, num++, MultiMenu.intruder, "Roleblock Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            ConsRoleblockDuration = new CustomNumberOption(true, num++, MultiMenu.intruder, "Roleblock Duration", 25f, 10f, 60f, 2.5f, CooldownFormat);

            Godfather = new CustomHeaderOption(num++, MultiMenu.intruder, "<color=#404C08FF>Godfather</color>");
            GodfatherCount = new CustomNumberOption(true, num++, MultiMenu.intruder, "<color=#404C08FF>Godfather</color> Count", 1, 1, 14, 1);
            UniqueGodfather = new CustomToggleOption(true, num++, MultiMenu.intruder, "<color=#404C08FF>Godfather</color> Is Unique In All Any", false);
            MafiosoAbilityCooldownDecrease = new CustomNumberOption(true, num++, MultiMenu.intruder, "Ability Cooldown Bonus", 0.75f, 0.25f, 0.9f, 0.05f, MultiplierFormat);
            PromotedMafiosoCanPromote = new CustomToggleOption(true, num++, MultiMenu.intruder, "Promoted <color=#404C08FF>Godfather</color> Can Declare Others", false);

            Miner = new CustomHeaderOption(num++, MultiMenu.intruder, "<color=#AA7632FF>Miner</color>");
            MinerCount = new CustomNumberOption(true, num++, MultiMenu.intruder, "<color=#AA7632FF>Miner</color> Count", 1, 1, 14, 1);
            UniqueMiner = new CustomToggleOption(true, num++, MultiMenu.intruder, "<color=#AA7632FF>Miner</color> Is Unique In All Any", false);
            MineCooldown = new CustomNumberOption(true, num++, MultiMenu.intruder, "Mine Cooldown", 25f, 10f, 40f, 2.5f, CooldownFormat);

            Teleporter = new CustomHeaderOption(num++, MultiMenu.intruder, "<color=#6AA84FFF>Teleporter</color>");
            TeleporterCount = new CustomNumberOption(true, num++, MultiMenu.intruder, "<color=#6AA84FFF>Teleporter</color> Count", 1, 1, 14, 1);
            UniqueTeleporter = new CustomToggleOption(true, num++, MultiMenu.intruder, "<color=#6AA84FFF>Teleporter</color> Is Unique In All Any", false);
            TeleportCd = new CustomNumberOption(true, num++, MultiMenu.intruder, "Teleport Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            TeleVent = new CustomToggleOption(true, num++, MultiMenu.intruder, "<color=#6AA84FFF>Teleporter</color> Can Vent", false);

            TimeMaster = new CustomHeaderOption(num++, MultiMenu.intruder, "<color=#0000A7FF>Time Master</color>");
            TimeMasterCount = new CustomNumberOption(true, num++, MultiMenu.intruder, "<color=#0000A7FF>Time Master</color> Count", 1, 1, 14, 1);
            UniqueTimeMaster = new CustomToggleOption(true, num++, MultiMenu.intruder, "<color=#0000A7FF>Time Master</color> Is Unique In All Any", false);
            TMImmunity = new CustomToggleOption(true, num++, MultiMenu.intruder, "<color=#0000A7FF>Time Master</color> Is Immune To Freeze And Rewind", false);
            IntruderImmunity = new CustomToggleOption(true, num++, MultiMenu.intruder, "Other <color=#FF0000FF>Intruders</color> Are Immune To Freeze", false);
            FreezeCooldown = new CustomNumberOption(true, num++, MultiMenu.intruder, "Freeze Cooldown", 25, 10, 40, 2.5f, CooldownFormat);
            FreezeDuration = new CustomNumberOption(true, num++, MultiMenu.intruder, "Freeze Duration", 20.0f, 5f, 60f, 5f, CooldownFormat);

            IntruderUtilitySettings = new CustomHeaderOption(num++, MultiMenu.intruder, "<color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Utility</color> Settings");

            Impostor = new CustomHeaderOption(num++, MultiMenu.intruder, "<color=#FF0000FF>Impostor</color>");
            ImpCount = new CustomNumberOption(true, num++, MultiMenu.intruder, "<color=#FF0000FF>Impostor</color> Count", 1, 1, 14, 1);

            SyndicateSettings = new CustomHeaderOption(num++, MultiMenu.syndicate, "<color=#008000FF>Syndicate</color> Settings");
            CustomSynColors = new CustomToggleOption(true, num++, MultiMenu.syndicate, "Enable Custom <color=#008000FF>Syndicate</color> Colors", true);
            SyndicateCount = new CustomNumberOption(true, num++, MultiMenu.syndicate, "<color=#008000FF>Syndicate</color> Count", 1, 0, 4, 1);
            SyndicateVision = new CustomNumberOption(true, num++, MultiMenu.syndicate, "<color=#008000FF>Syndicate</color> Vision", 2f, 0.25f, 5f, 0.25f, MultiplierFormat);
            ChaosDriveMeetingCount = new CustomNumberOption(true, num++, MultiMenu.syndicate, "Number Of Meetings Required For Chaos Drive Spawn", 3, 1, 4, 1);
            ChaosDriveKillCooldown = new CustomNumberOption(true, num++, MultiMenu.syndicate, "Chaos Drive Holder Kill Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            SyndicateVent = new CustomStringOption(true, num++, MultiMenu.syndicate, "<color=#008000FF>Syndicate</color> Can Vent", new[] {"Always", "Chaos Drive", "Never"});
            AltImps = new CustomToggleOption(true, num++, MultiMenu.syndicate, "<color=#008000FF>Syndicate</color> Replaces <color=#FF0000FF>Intruders</color>", false);
            SyndicateMax = new CustomNumberOption(true, num++, MultiMenu.syndicate, "Max <color=#008000FF>Syndicate</color> <color=#FFD700FF>Roles</color>", 5, 1, 14, 1);
            SyndicateMin = new CustomNumberOption(true, num++, MultiMenu.syndicate, "Min <color=#008000FF>Syndicate</color> <color=#FFD700FF>Roles</color>", 5, 1, 14, 1);

            SyndicateDisruptionSettings = new CustomHeaderOption(num++, MultiMenu.syndicate, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Disruption</color> Settings");
            SDMax = new CustomNumberOption(true, num++, MultiMenu.syndicate, "Max <color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Disruption</color> Roles", 1, 1, 14, 1);
            SDMin = new CustomNumberOption(true, num++, MultiMenu.syndicate, "Min <color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Disruption</color> Roles", 1, 1, 14, 1);

            Concealer = new CustomHeaderOption(num++, MultiMenu.syndicate, "<color=#C02525FF>Concealer</color>");
            ConcealerCount = new CustomNumberOption(true, num++, MultiMenu.syndicate, "<color=#C02525FF>Concealer</color> Count", 1, 1, 14, 1);
            UniqueConcealer = new CustomToggleOption(true, num++, MultiMenu.syndicate, "<color=#C02525FF>Concealer</color> Is Unique In All Any", false);
            ConcealCooldown = new CustomNumberOption(true, num++, MultiMenu.syndicate, "Conceal Cooldown", 25f, 10f, 40f, 2.5f, CooldownFormat);
            ConcealDuration = new CustomNumberOption(true, num++, MultiMenu.syndicate, "Conceal Duration", 10f, 5f, 15f, 1f, CooldownFormat);

            Drunkard = new CustomHeaderOption(num++, MultiMenu.syndicate, "<color=#1E300BFF>Drunkard</color>");
            DrunkardCount = new CustomNumberOption(true, num++, MultiMenu.syndicate, "<color=#1E300BFF>Drunkard</color> Count", 1, 1, 14, 1);
            UniqueDrunkard = new CustomToggleOption(true, num++, MultiMenu.syndicate, "<color=#1E300BFF>Drunkard</color> Is Unique In All Any", false);
            SyndicateImmunity = new CustomToggleOption(true, num++, MultiMenu.syndicate, "Other <color=#008000FF>Syndicate</color> Are Immune To Confuse", false);
            ConfuseCooldown = new CustomNumberOption(true, num++, MultiMenu.syndicate, "Confuse Cooldown", 25, 10, 40, 2.5f, CooldownFormat);
            ConfuseDuration = new CustomNumberOption(true, num++, MultiMenu.syndicate, "Confuse Duration", 20.0f, 5f, 60f, 5f, CooldownFormat);

            Framer = new CustomHeaderOption(num++, MultiMenu.syndicate, "<color=#00FFFFFF>Framer</color>");
            FramerCount = new CustomNumberOption(true, num++, MultiMenu.syndicate, "<color=#00FFFFFF>Framer</color> Count", 1, 1, 14, 1);
            UniqueFramer = new CustomToggleOption(true, num++, MultiMenu.syndicate, "<color=#00FFFFFF>Framer</color> Is Unique In All Any", false);
            FrameCooldown = new CustomNumberOption(true, num++, MultiMenu.syndicate, "Frame Cooldown", 25f, 10f, 40f, 2.5f, CooldownFormat);

            Poisoner = new CustomHeaderOption(num++, MultiMenu.syndicate, "<color=#B5004CFF>Poisoner</color>");
            PoisonerCount = new CustomNumberOption(true, num++, MultiMenu.syndicate, "<color=#B5004CFF>Poisoner</color> Count", 1, 1, 14, 1);
            UniquePoisoner = new CustomToggleOption(true, num++, MultiMenu.syndicate, "<color=#B5004CFF>Poisoner</color> Is Unique In All Any", false);
            PoisonCooldown = new CustomNumberOption(true, num++, MultiMenu.syndicate, "Poison Cooldown", 25f, 10f, 40f, 2.5f, CooldownFormat);
            PoisonDuration = new CustomNumberOption(true, num++, MultiMenu.syndicate, "Poison Kill Delay", 5f, 1f, 15f, 1f, CooldownFormat);

            Shapeshifter = new CustomHeaderOption(num++, MultiMenu.syndicate, "<color=#311C45FF>Shapeshifter</color>");
            ShapeshifterCount = new CustomNumberOption(true, num++, MultiMenu.syndicate, "<color=#311C45FF>Shapeshifter</color> Count", 1, 1, 14, 1);
            UniqueShapeshifter = new CustomToggleOption(true, num++, MultiMenu.syndicate, "<color=#311C45FF>Shapeshifter</color> Is Unique In All Any", false);
            ShapeshiftCooldown = new CustomNumberOption(true, num++, MultiMenu.syndicate, "Shapeshift Cooldown", 25f, 10f, 40f, 2.5f, CooldownFormat);
            ShapeshiftDuration = new CustomNumberOption(true, num++, MultiMenu.syndicate, "Shapeshift Duration", 10f, 5f, 15f, 1f, CooldownFormat);

            SyndicateKillingSettings = new CustomHeaderOption(num++, MultiMenu.syndicate, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Killing</color> Settings");
            SyKMax = new CustomNumberOption(true, num++, MultiMenu.syndicate, "Max <color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Killing</color> Roles", 1, 1, 14, 1);
            SyKMin = new CustomNumberOption(true, num++, MultiMenu.syndicate, "Min <color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Killing</color> Roles", 1, 1, 14, 1);

            Bomber = new CustomHeaderOption(num++, MultiMenu.syndicate, "<color=#C9CC3FFF>Bomber</color>");
            OperativeCount = new CustomNumberOption(true, num++, MultiMenu.syndicate, "<color=#C9CC3FFF>Bomber</color> Count", 1, 1, 14, 1);
            UniqueBomber = new CustomToggleOption(true, num++, MultiMenu.syndicate, "<color=#C9CC3FFF>Bomber</color> Is Unique In All Any", false);
            BombCooldown = new CustomNumberOption(true, num++, MultiMenu.syndicate, "Bomb Cooldown", 25f, 10f, 40f, 2.5f, CooldownFormat);
            DetonateCooldown = new CustomNumberOption(true, num++, MultiMenu.syndicate, "Bomb Detonation Cooldown", 25f, 10f, 40f, 2.5f, CooldownFormat);
            BombCooldownsLinked = new CustomToggleOption(true, num++, MultiMenu.syndicate, "<color=#C9CC3FFF>Bomber</color> Place, Detonate And Kill Cooldowns Are Linked", false);
            BombsRemoveOnNewRound = new CustomToggleOption(true, num++, MultiMenu.syndicate, "Bombs Are Cleared Every Meeting", false);
            BombsDetonateOnMeetingStart = new CustomToggleOption(true, num++, MultiMenu.syndicate, "Bombs Detonate Everytime A Meeting Is Called", false);
            BombRange = new CustomNumberOption(true, num++, MultiMenu.syndicate, "Bomb Radius", 0.25f, 0.05f, 1f, 0.05f, MultiplierFormat);

            Gorgon = new CustomHeaderOption(num++, MultiMenu.syndicate, "<color=#7E4D00FF>Gorgon</color>");
            GorgonCount = new CustomNumberOption(true, num++, MultiMenu.syndicate, "<color=#7E4D00FF>Gorgon</color> Count", 1, 1, 14, 1);
            UniqueGorgon = new CustomToggleOption(true, num++, MultiMenu.syndicate, "<color=#7E4D00FF>Gorgon</color> Is Unique In All Any", false);
            GazeCooldown = new CustomNumberOption(true, num++, MultiMenu.syndicate, "Gaze Cooldown", 25f, 10f, 40f, 2.5f, CooldownFormat);
            GazeDelay = new CustomNumberOption(true, num++, MultiMenu.syndicate, "Gaze Delay", 10f, 5f, 15f, 1f, CooldownFormat);
            GazeTime = new CustomNumberOption(true, num++, MultiMenu.syndicate, "Gaze Time", 10f, 5f, 15f, 1f, CooldownFormat);

            SyndicateSupportSettings = new CustomHeaderOption(num++, MultiMenu.syndicate, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Support</color> Settings");
            SSuMax = new CustomNumberOption(true, num++, MultiMenu.syndicate, "Max <color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Support</color> Roles", 1, 1, 14, 1);
            SSuMin = new CustomNumberOption(true, num++, MultiMenu.syndicate, "Min <color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Support</color> Roles", 1, 1, 14, 1);

            Beamer = new CustomHeaderOption(num++, MultiMenu.syndicate, "<color=#0028F5FF>Beamer</color>");
            BeamerCount = new CustomNumberOption(true, num++, MultiMenu.syndicate, "<color=#0028F5FF>Beamer</color> Count", 1, 1, 14, 1);
            UniqueBeamer = new CustomToggleOption(true, num++, MultiMenu.syndicate, "<color=#0028F5FF>Beamer</color> Is Unique In All Any", false);
            BeamCooldown = new CustomNumberOption(true, num++, MultiMenu.syndicate, "Beam Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);

            Rebel = new CustomHeaderOption(num++, MultiMenu.syndicate, "<color=#FFFCCEFF>Rebel</color>");
            RebelCount = new CustomNumberOption(true, num++, MultiMenu.syndicate, "<color=#FFFCCEFF>Rebel</color> Count", 1, 1, 14, 1);
            UniqueRebel = new CustomToggleOption(true, num++, MultiMenu.syndicate, "<color=#FFFCCEFF>Rebel</color> Is Unique In All Any", false);
            SidekickAbilityCooldownDecrease = new CustomNumberOption(true, num++, MultiMenu.syndicate, "Ability Cooldown Bonus", 0.75f, 0.25f, 0.9f, 0.05f, MultiplierFormat);
            PromotedSidekickCanPromote = new CustomToggleOption(true, num++, MultiMenu.syndicate, "Promoted <color=#FFFCCEFF>Rebel</color> Can Sidekick Others", false);

            Warper = new CustomHeaderOption(num++, MultiMenu.syndicate, "<color=#8C7140FF>Warper</color>");
            WarperCount = new CustomNumberOption(true, num++, MultiMenu.syndicate, "<color=#8C7140FF>Warper</color> Count", 1, 1, 14, 1);
            UniqueWarper = new CustomToggleOption(true, num++, MultiMenu.syndicate, "<color=#8C7140FF>Warper</color> Is Unique In All Any", false);
            WarpCooldown = new CustomNumberOption(true, num++, MultiMenu.syndicate, "Warp Cooldown", 25, 10, 40, 2.5f, CooldownFormat);

            SyndicateUtilitySettings = new CustomHeaderOption(num++, MultiMenu.syndicate, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Utility</color> Settings");

            Anarchist = new CustomHeaderOption(num++, MultiMenu.syndicate, "<color=#008000FF>Anarchist</color>");
            AnarchistCount = new CustomNumberOption(true, num++, MultiMenu.syndicate, "<color=#008000FF>Anarchist</color> Count", 1, 1, 14, 1);

            ModifierSettings = new CustomHeaderOption(num++, MultiMenu.modifier, "<color=#7F7F7FFF>Modifier</color> Settings");
            CustomModifierColors = new CustomToggleOption(true, num++, MultiMenu.modifier, "Enable Custom <color=#7F7F7FFF>Modifier</color> Colors", true);
            MaxModifiers = new CustomNumberOption(true, num++, MultiMenu.modifier, "Max <color=#7F7F7FFF>Modifier</color>", 1, 1, 14, 1);
            MinModifiers = new CustomNumberOption(true, num++, MultiMenu.modifier, "Min <color=#7F7F7FFF>Modifier</color>", 1, 1, 14, 1);

            Bait = new CustomHeaderOption(num++, MultiMenu.modifier, "<color=#00B3B3FF>Bait</color>");
            BaitCount = new CustomNumberOption(true, num++, MultiMenu.modifier, "<color=#00B3B3FF>Bait</color> Count", 1, 1, 14, 1);
            UniqueBait = new CustomToggleOption(true, num++, MultiMenu.modifier, "<color=#00B3B3FF>Bait</color> Is Unique In All Any", false);
            BaitKnows = new CustomToggleOption(true, num++, MultiMenu.modifier, "<color=#00B3B3FF>Bait</color> Knows Who They Are On Game Start", true);
            BaitMinDelay = new CustomNumberOption(true, num++, MultiMenu.modifier, "Minimum Delay for <color=#00B3B3FF>Bait</color> Self Report", 0f, 0f, 15f, 0.5f, CooldownFormat);
            BaitMaxDelay = new CustomNumberOption(true, num++, MultiMenu.modifier, "Maximum Delay for <color=#00B3B3FF>Bait</color> Self Report", 1f, 0f, 15f, 0.5f, CooldownFormat);

            Coward = new CustomHeaderOption(num++, MultiMenu.modifier, "<color=#456BA8FF>Coward</color>");
            CowardCount = new CustomNumberOption(true, num++, MultiMenu.modifier, "<color=#456BA8FF>Coward</color> Count", 1, 1, 14, 1);
            UniqueCoward = new CustomToggleOption(true, num++, MultiMenu.modifier, "Modifier Is Unique In All Any", false);

            Diseased = new CustomHeaderOption(num++, MultiMenu.modifier, "<color=#374D1EFF>Diseased</color>");
            DiseasedCount = new CustomNumberOption(true, num++, MultiMenu.modifier, "<color=#374D1EFF>Diseased</color> Count", 1, 1, 14, 1);
            UniqueDiseased = new CustomToggleOption(true, num++, MultiMenu.modifier, "<color=#374D1EFF>Diseased</color> Is Unique In All Any", false);
            DiseasedKnows = new CustomToggleOption(true, num++, MultiMenu.modifier, "<color=#374D1EFF>Diseased</color> Knows Who They Are On Game Start", true);
            DiseasedKillMultiplier = new CustomNumberOption(true, num++, MultiMenu.modifier, "<color=#374D1EFF>Diseased</color> Kill Multiplier", 3f, 1.5f, 5f, 0.5f, MultiplierFormat);

            Dwarf = new CustomHeaderOption(num++, MultiMenu.modifier, "<color=#FF8080FF>Dwarf</color>");
            DwarfCount = new CustomNumberOption(true, num++, MultiMenu.modifier, "<color=#FF8080FF>Dwarf</color> Count", 1, 1, 14, 1);
            UniqueDwarf = new CustomToggleOption(true, num++, MultiMenu.modifier, "<color=#FF8080FF>Dwarf</color> Is Unique In All Any", false);
            DwarfSpeed = new CustomNumberOption(true, num++, MultiMenu.modifier, "<color=#FF8080FF>Dwarf</color> Speed", 1.5f, 1.0f, 2f, 0.05f, MultiplierFormat);
            DwarfScale = new CustomNumberOption(true, num++, MultiMenu.modifier, "<color=#FF8080FF>Dwarf</color> Scale", 0.5f, 0.3f, 0.6f, 0.025f, MultiplierFormat);

            Giant = new CustomHeaderOption(num++, MultiMenu.modifier, "<color=#FFB34DFF>Giant</color>");
            GiantCount = new CustomNumberOption(true, num++, MultiMenu.modifier, "<color=#FFB34DFF>Giant</color> Count", 1, 1, 14, 1);
            UniqueGiant = new CustomToggleOption(true, num++, MultiMenu.modifier, "<color=#FFB34DFF>Giant</color> Is Unique In All Any", false);
            GiantSpeed = new CustomNumberOption(true, num++, MultiMenu.modifier, "<color=#FFB34DFF>Giant</color> Speed", 0.75f, 0.5f, 1f, 0.05f, MultiplierFormat);
            GiantScale = new CustomNumberOption(true, num++, MultiMenu.modifier, "<color=#FFB34DFF>Giant</color> Scale", 1.5f, 1.5f, 3.0f, 0.025f, MultiplierFormat);

            Drunk = new CustomHeaderOption(num++, MultiMenu.modifier, "<color=#758000FF>Drunk</color>");
            DrunkCount = new CustomNumberOption(true, num++, MultiMenu.modifier, "<color=#758000FF>Drunk</color> Count", 1, 1, 14, 1);
            UniqueDrunk = new CustomToggleOption(true, num++, MultiMenu.modifier, "<color=#758000FF>Drunk</color> Is Unique In All Any", false);
            DrunkControlsSwap = new CustomToggleOption(true, num++, MultiMenu.modifier, "<color=#758000FF>Drunk</color> Controls Reverse Over Time", false);
            DrunkInterval = new CustomNumberOption(true, num++, MultiMenu.modifier, "Reversed Controls Interval", 1f, 1f, 20f, 1f, CooldownFormat);

            Flincher = new CustomHeaderOption(num++, MultiMenu.modifier, "<color=#80B3FFFF>Flincher</color>");
            FlincherCount = new CustomNumberOption(true, num++, MultiMenu.modifier, "<color=#80B3FFFF>Flincher</color> Count", 1, 1, 14, 1);
            UniqueFlincher = new CustomToggleOption(true, num++, MultiMenu.modifier, "<color=#80B3FFFF>Flincher</color> Is Unique In All Any", false);
            FlinchInterval = new CustomNumberOption(true, num++, MultiMenu.modifier, "Flinch Interval", 1f, 1f, 20f, 1f, CooldownFormat);

            Professional = new CustomHeaderOption(num++, MultiMenu.modifier, "<color=#860B7AFF>Professional</color>");
            ProfessionalCount = new CustomNumberOption(true, num++, MultiMenu.modifier, "<color=#860B7AFF>Professional</color> Count", 1, 1, 14, 1);
            UniqueProfessional = new CustomToggleOption(true, num++, MultiMenu.modifier, "<color=#860B7AFF>Professional</color> Is Unique In All Any", false);
            ProfessionalKnows = new CustomToggleOption(true, num++, MultiMenu.modifier, "<color=#860B7AFF>Professional</color> Knows Who They Are On Game Start", true);

            Shy = new CustomHeaderOption(num++, MultiMenu.modifier, "<color=#1002C5FF>Shy</color>");
            ShyCount = new CustomNumberOption(true, num++, MultiMenu.modifier, "<color=#1002C5FF>Shy</color> Count", 1, 1, 14, 1);
            UniqueShy = new CustomToggleOption(true, num++, MultiMenu.modifier, "<color=#1002C5FF>Shy</color> Is Unique In All Any", false);

            VIP = new CustomHeaderOption(num++, MultiMenu.modifier, "<color=#DCEE85FF>VIP</color>");
            VIPCount = new CustomNumberOption(true, num++, MultiMenu.modifier, "<color=#DCEE85FF>VIP</color> Count", 1, 1, 14, 1);
            UniqueVIP = new CustomToggleOption(true, num++, MultiMenu.modifier, "<color=#DCEE85FF>VIP</color> Is Unique In All Any", false);
            VIPKnows = new CustomToggleOption(true, num++, MultiMenu.modifier, "<color=#DCEE85FF>VIP</color> Knows Who They Are On Game Start", true);

            Volatile = new CustomHeaderOption(num++, MultiMenu.modifier, "<color=#FFA60AFF>Volatile</color>");
            VolatileCount = new CustomNumberOption(true, num++, MultiMenu.modifier, "<color=#FFA60AFF>Volatile</color> Count", 1, 1, 14, 1);
            UniqueVolatile = new CustomToggleOption(true, num++, MultiMenu.modifier, "<color=#FFA60AFF>Volatile</color> Is Unique In All Any", false);
            VolatileInterval = new CustomNumberOption(true, num++, MultiMenu.modifier, "<color=#FFA60AFF>Volatile</color> Interval", 15f, 10f, 30f, 1f, CooldownFormat);
            VolatileKnows = new CustomToggleOption(true, num++, MultiMenu.modifier, "<color=#FFA60AFF>Volatile</color> Knows Who They Are On Game Start", true);

            AbilitySettings = new CustomHeaderOption(num++, MultiMenu.ability, "<color=#FF9900FF>Ability</color> Settings");
            CustomAbilityColors = new CustomToggleOption(true, num++, MultiMenu.ability, "Enable Custom <color=#FF9900FF>Ability</color> Colors", true);
            MaxAbilities = new CustomNumberOption(true, num++, MultiMenu.ability, "Max <color=#FF9900FF>Ability</color>", 1, 1, 14, 1);
            MinAbilities = new CustomNumberOption(true, num++, MultiMenu.ability, "Min <color=#FF9900FF>Ability</color>", 1, 1, 14, 1);

            Assassin = new CustomHeaderOption(num++, MultiMenu.ability, "<color=#073763FF>Assassin</color>");
            NumberOfImpostorAssassins = new CustomNumberOption(true, num++, MultiMenu.ability, "<color=#FF0000FF>Intruder</color> <color=#073763FF>Assassins</color> Count", 1, 1, 14, 1);
            NumberOfCrewAssassins = new CustomNumberOption(true, num++, MultiMenu.ability, "<color=#8BFDFDFF>Crew</color> <color=#073763FF>Assassins</color> Count", 1, 1, 14, 1);
            NumberOfNeutralAssassins = new CustomNumberOption(true, num++, MultiMenu.ability, "<color=#B3B3B3FF>Neutral</color> <color=#073763FF>Assassins</color> Count", 1, 1, 14, 1);
            NumberOfSyndicateAssassins = new CustomNumberOption(true, num++, MultiMenu.ability, "<color=#008000FF>Syndicate</color> <color=#073763FF>Assassins</color> Count", 1, 1, 14, 1);
            UniqueAssassin = new CustomToggleOption(true, num++, MultiMenu.ability, "<color=#073763FF>Assassin</color> Is Unique In All Any", false);
            AssassinKills = new CustomNumberOption(true, num++, MultiMenu.ability, "<color=#073763FF>Assassin</color> Guess Limit", 1, 1, 15, 1);
            AssassinMultiKill = new CustomToggleOption(true, num++, MultiMenu.ability, "<color=#073763FF>Assassin</color> Can Kill More Than Once Per Meeting", false);
            AssassinGuessNeutralBenign = new CustomToggleOption(true, num++, MultiMenu.ability, "<color=#073763FF>Assassin</color> Can Guess <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Benigns</color>", false);
            AssassinGuessNeutralEvil = new CustomToggleOption(true, num++, MultiMenu.ability, "<color=#073763FF>Assassin</color> Can Guess <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Evils</color>", false);
            AssassinGuessPest = new CustomToggleOption(true, num++, MultiMenu.ability, "<color=#073763FF>Assassin</color> Can Guess <color=#424242FF>Pestilence</color>", false);
            AssassinGuessModifiers = new CustomToggleOption(true, num++, MultiMenu.ability, "<color=#073763FF>Assassin</color> Can Guess Select <color=#7F7F7FFF>Modifiers</color>", false);
            AssassinGuessObjectifiers = new CustomToggleOption(true, num++, MultiMenu.ability, "<color=#073763FF>Assassin</color> Can Guess Select <color=#DD585BFF>Objectifiers</color>", false);
            AssassinGuessAbilities = new CustomToggleOption(true, num++, MultiMenu.ability, "<color=#073763FF>Assassin</color> Can Guess <color=#FF9900FF>Abilities</color>", false);
            AssassinateAfterVoting = new CustomToggleOption(true, num++, MultiMenu.ability, "<color=#073763FF>Assassin</color> Can Guess After Voting", false);

            ButtonBarry = new CustomHeaderOption(num++, MultiMenu.ability, "<color=#E600FFFF>Button Barry</color>");
            ButtonBarryCount = new CustomNumberOption(true, num++, MultiMenu.ability, "<color=#E600FFFF>Button Barry</color> Count", 1, 1, 14, 1);
            UniqueButtonBarry = new CustomToggleOption(true, num++, MultiMenu.ability, "<color=#E600FFFF>Button Barry</color> Is Unique In All Any", false);

            Insider = new CustomHeaderOption(num++, MultiMenu.ability, "<color=#26FCFBFF>Insider</color>");
            InsiderCount = new CustomNumberOption(true, num++, MultiMenu.ability, "<color=#26FCFBFF>Insider</color> Count", 1, 1, 14, 1);
            UniqueInsider = new CustomToggleOption(true, num++, MultiMenu.ability, "<color=#26FCFBFF>Insider</color> Is Unique In All Any", false);
            InsiderKnows = new CustomToggleOption(true, num++, MultiMenu.ability, "<color=#26FCFBFF>Insider</color> Knows Who They Are On Game Start", true);

            Lighter = new CustomHeaderOption(num++, MultiMenu.ability, "<color=#1AFF74FF>Lighter</color>");
            LighterCount = new CustomNumberOption(true, num++, MultiMenu.ability, "<color=#1AFF74FF>Lighter</color> Count", 1, 1, 14, 1);
            UniqueLighter = new CustomToggleOption(true, num++, MultiMenu.ability, "<color=#1AFF74FF>Lighter</color> Is Unique In All Any", false);

            Multitasker = new CustomHeaderOption(num++, MultiMenu.ability, "<color=#FF804DFF>Multitasker</color>");
            MultitaskerCount = new CustomNumberOption(true, num++, MultiMenu.ability, "<color=#FF804DFF>Multitasker</color> Count", 1, 1, 14, 1);
            UniqueMultitasker = new CustomToggleOption(true, num++, MultiMenu.ability, "<color=#FF804DFF>Multitasker</color> Is Unique In All Any", false);
            Transparancy = new CustomNumberOption(true, num++, MultiMenu.ability, "Task Transparancy", 50f, 10f, 80f, 5f, PercentFormat);

            Radar = new CustomHeaderOption(num++, MultiMenu.ability, "<color=#FF0080FF>Radar</color>");
            RadarCount = new CustomNumberOption(true, num++, MultiMenu.ability, "<color=#FF0080FF>Radar</color> Count", 1, 1, 14, 1);
            UniqueRadar = new CustomToggleOption(true, num++, MultiMenu.ability, "<color=#FF0080FF>Radar</color> Is Unique In All Any", false);

            Snitch = new CustomHeaderOption(num++, MultiMenu.ability, "<color=#D4AF37FF>Snitch</color>");
            SnitchCount = new CustomNumberOption(true, num++, MultiMenu.ability, "<color=#D4AF37FF>Snitch</color> Count", 1, 1, 14, 1);
            UniqueSnitch = new CustomToggleOption(true, num++, MultiMenu.ability, "<color=#D4AF37FF>Snitch</color> Is Unique In All Any", false);
            SnitchKnows = new CustomToggleOption(true, num++, MultiMenu.ability, "<color=#D4AF37FF>Snitch</color> Knows Who They Are On Game Start", true);
            SnitchSeesNeutrals = new CustomToggleOption(true, num++, MultiMenu.ability, "<color=#D4AF37FF>Snitch</color> Sees <color=#B3B3B3FF>Neutrals</color>", false);
            SnitchSeesCrew = new CustomToggleOption(true, num++, MultiMenu.ability, "<color=#D4AF37FF>Snitch</color> Sees <color=#8BFDFDFF>Crew</color>", false);
            SnitchSeesRoles = new CustomToggleOption(true, num++, MultiMenu.ability, "<color=#D4AF37FF>Snitch</color> Sees Exact Roles", false);
            SnitchTasksRemaining = new CustomNumberOption(true, num++, MultiMenu.ability, "Tasks Remaining When Revealed", 1, 1, 5, 1);
            SnitchSeesImpInMeeting = new CustomToggleOption(true, num++, MultiMenu.ability, "<color=#D4AF37FF>Snitch</color> Sees <color=#FF0000FF>Intruders</color> In Meetings", true);

            Tiebreaker = new CustomHeaderOption(num++, MultiMenu.ability, "<color=#99E699FF>Tiebreaker</color>");
            TiebreakerCount = new CustomNumberOption(true, num++, MultiMenu.ability, "<color=#99E699FF>Tiebreaker</color> Count", 1, 1, 14, 1);
            UniqueTiebreaker = new CustomToggleOption(true, num++, MultiMenu.ability, "<color=#99E699FF>Tiebreaker</color> Is Unique In All Any", false);
            TiebreakerKnows = new CustomToggleOption(true, num++, MultiMenu.ability, "<color=#99E699FF>Tiebreaker</color> Knows Who They Are On Game Start", true);

            Torch = new CustomHeaderOption(num++, MultiMenu.ability, "<color=#FFFF99FF>Torch</color>");
            TorchCount = new CustomNumberOption(true, num++, MultiMenu.ability, "<color=#FFFF99FF>Torch</color> Count", 1, 1, 14, 1);
            UniqueTorch = new CustomToggleOption(true, num++, MultiMenu.ability, "<color=#FFFF99FF>Torch</color> Is Unique In All Any", false);

            Tunneler = new CustomHeaderOption(num++, MultiMenu.ability, "<color=#E91E63FF>Tunneler</color>");
            TunnelerCount = new CustomNumberOption(true, num++, MultiMenu.ability, "<color=#E91E63FF>Tunneler</color> Count", 1, 1, 14, 1);
            UniqueTunneler = new CustomToggleOption(true, num++, MultiMenu.ability, "<color=#E91E63FF>Tunneler</color> Is Unique In All Any", false);
            TunnelerKnows = new CustomToggleOption(true, num++, MultiMenu.ability, "<color=#E91E63FF>Tunneler</color> Knows Who They Are On Game Start", true);

            Underdog = new CustomHeaderOption(num++, MultiMenu.ability, "<color=#841A7FFF>Underdog</color>");
            UnderdogCount = new CustomNumberOption(true, num++, MultiMenu.ability, "<color=#841A7FFF>Underdog</color> Count", 1, 1, 14, 1);
            UniqueUnderdog = new CustomToggleOption(true, num++, MultiMenu.ability, "<color=#841A7FFF>Underdog</color> Is Unique In All Any", false);
            UnderdogKnows = new CustomToggleOption(true, num++, MultiMenu.ability, "<color=#841A7FFF>Underdog</color> Knows Who They Are On Game Start", true);
            UnderdogKillBonus = new CustomNumberOption(true, num++, MultiMenu.ability, "Kill Cooldown Bonus", 5f, 2.5f, 30f, 2.5f, CooldownFormat);
            UnderdogIncreasedKC = new CustomToggleOption(true, num++, MultiMenu.ability, "Increased Kill Cooldown When 2+ Teammates", true);

            ObjectifierSettings = new CustomHeaderOption(num++, MultiMenu.objectifier, "<color=#DD585BFF>Objectifier</color> Settings");
            CustomObjectifierColors = new CustomToggleOption(true, num++, MultiMenu.objectifier, "Enable Custom <color=#DD585BFF>Objectifier</color> Colors", true);
            MaxObjectifiers = new CustomNumberOption(true, num++, MultiMenu.objectifier, "Max <color=#DD585BFF>Objectifier</color>", 1, 1, 14, 1);
            MinObjectifiers = new CustomNumberOption(true, num++, MultiMenu.objectifier, "Min <color=#DD585BFF>Objectifier</color>", 1, 1, 14, 1);

            Allied = new CustomHeaderOption(num++, MultiMenu.objectifier, "<color=#4545A9FF>Allied</color>");
            AlliedCount = new CustomNumberOption(true, num++, MultiMenu.objectifier, "<color=#4545A9FF>Allied</color> Count", 1, 1, 14, 1);
            UniqueAllied = new CustomToggleOption(true, num++, MultiMenu.objectifier, "<color=#4545A9FF>Allied</color> Is Unique In All Any", false);
            AlliedFaction = new CustomStringOption(true, num++, MultiMenu.objectifier, "<color=#4545A9FF>Allied</color> Faction", new[] {"Intruder", "Syndicate", "Crew", "Random"});

            Corrupted = new CustomHeaderOption(num++, MultiMenu.objectifier, "<color=#4545FFFF>Corrupted</color>");
            CorruptedCount = new CustomNumberOption(true, num++, MultiMenu.objectifier, "<color=#4545FFFF>Corrupted</color> Count", 1, 1, 14, 1);
            UniqueCorrupted = new CustomToggleOption(true, num++, MultiMenu.objectifier, "<color=#4545FFFF>Corrupted</color> Is Unique In All Any", false);
            CorruptedKillCooldown = new CustomNumberOption(true, num++, MultiMenu.objectifier, "<color=#4545FFFF>Corrupted</color> Kill Cooldown", 1f, 1f, 20f, 1f, CooldownFormat);

            Fanatic = new CustomHeaderOption(num++, MultiMenu.objectifier, "<color=#678D36FF>Fanatic</color>");
            FanaticCount = new CustomNumberOption(true, num++, MultiMenu.objectifier, "<color=#678D36FF>Fanatic</color> Count", 1, 1, 14, 1);
            UniqueFanatic = new CustomToggleOption(true, num++, MultiMenu.objectifier, "<color=#678D36FF>Fanatic</color> Is Unique In All Any", false);
            FanaticKnows = new CustomToggleOption(true, num++, MultiMenu.objectifier, "<color=#678D36FF>Fanatic</color> Knows Who They Are On Game Start", true);
            FanaticCanAssassin = new CustomToggleOption(true, num++, MultiMenu.objectifier, "Turned <color=#678D36FF>Fanatic</color> Gets <color=#073763FF>Assassin</color>", false);
            FanaticColourSwap = new CustomToggleOption(true, num++, MultiMenu.objectifier, "Turned <color=#678D36FF>Fanatic</color> Swaps Colours For Investigative Roles", false);
            SnitchSeesFanatic = new CustomToggleOption(true, num++, MultiMenu.objectifier, "<color=#D4AF37FF>Snitch</color> Sees Turned <color=#678D36FF>Fanatic</color>", true);
            RevealerRevealsFanatic = new CustomToggleOption(true, num++, MultiMenu.objectifier, "<color=#D3D3D3FF>Revealer</color> Reveals Turned <color=#678D36FF>Fanatic</color>", false);

            Lovers = new CustomHeaderOption(num++, MultiMenu.objectifier, "<color=#FF66CCFF>Lovers</color>");
            LoversCount = new CustomNumberOption(true, num++, MultiMenu.objectifier, "<color=#FF66CCFF>Lovers</color> Count", 1, 1, 14, 1);
            UniqueLovers = new CustomToggleOption(true, num++, MultiMenu.objectifier, "<color=#FF66CCFF>Lovers</color> Is Unique In All Any", false);
            BothLoversDie = new CustomToggleOption(true, num++, MultiMenu.objectifier, "Both <color=#FF66CCFF>Lovers</color> Die", true);
            LoversChat = new CustomToggleOption(true, num++, MultiMenu.objectifier, "Enable <color=#FF66CCFF>Lovers</color> Chat", true);
            LoversFaction = new CustomToggleOption(true, num++, MultiMenu.objectifier, "<color=#FF66CCFF>Lovers</color> Can Be From The Same Faction", true);
            LoversRoles = new CustomToggleOption(true, num++, MultiMenu.objectifier, "<color=#FF66CCFF>Lovers</color> Know Each Other's Roles", true);

            Overlord = new CustomHeaderOption(num++, MultiMenu.objectifier, "<color=#008080FF>Overlord</color>");
            OverlordCount = new CustomNumberOption(true, num++, MultiMenu.objectifier, "<color=#008080FF>Overlord</color> Count", 1, 1, 14, 1);
            UniqueOverlord = new CustomToggleOption(true, num++, MultiMenu.objectifier, "<color=#008080FF>Overlord</color> Is Unique In All Any", false);
            OverlordMeetingWinCount = new CustomNumberOption(true, num++, MultiMenu.objectifier, "<color=#008080FF>Overlord</color> Meeting Timer", 2f, 1f, 20f, 1f);

            Rivals = new CustomHeaderOption(num++, MultiMenu.objectifier, "<color=#3D2D2CFF>Rivals</color>");
            RivalsCount = new CustomNumberOption(true, num++, MultiMenu.objectifier, "<color=#3D2D2CFF>Rivals</color> Count", 1, 1, 14, 1);
            UniqueRivals = new CustomToggleOption(true, num++, MultiMenu.objectifier, "<color=#3D2D2CFF>Rivals</color> Is Unique In All Any", false);
            RivalsChat = new CustomToggleOption(true, num++, MultiMenu.objectifier, "Enable <color=#3D2D2CFF>Rivals</color> Chat", true);
            RivalsFaction = new CustomToggleOption(true, num++, MultiMenu.objectifier, "<color=#3D2D2CFF>Rivals</color> Can Be From The Same Faction", true);
            RivalsRoles = new CustomToggleOption(true, num++, MultiMenu.objectifier, "<color=#3D2D2CFF>Rivals</color> Know Each Other's Roles", true);

            Taskmaster = new CustomHeaderOption(num++, MultiMenu.objectifier, "<color=#ABABFFFF>Taskmaster</color>");
            TaskmasterCount = new CustomNumberOption(true, num++, MultiMenu.objectifier, "<color=#ABABFFFF>Taskmaster</color> Count", 1, 1, 14, 1);
            UniqueTaskmaster = new CustomToggleOption(true, num++, MultiMenu.objectifier, "<color=#ABABFFFF>Taskmaster</color> Is Unique In All Any", false);
            TMTasksRemaining = new CustomNumberOption(true, num++, MultiMenu.objectifier, "Tasks Remaining When Revealed", 1, 1, 5, 1);

            Traitor = new CustomHeaderOption(num++, MultiMenu.objectifier, "<color=#370D43FF>Traitor</color>");
            TraitorCount = new CustomNumberOption(true, num++, MultiMenu.objectifier, "<color=#370D43FF>Traitor</color> Count", 1, 1, 14, 1);
            UniqueTraitor = new CustomToggleOption(true, num++, MultiMenu.objectifier, "<color=#370D43FF>Traitor</color> Is Unique In All Any", false);
            TraitorKnows = new CustomToggleOption(true, num++, MultiMenu.objectifier, "<color=#370D43FF>Traitor</color> Knows Who They Are On Game Start", false);
            TraitorCanAssassin = new CustomToggleOption(true, num++, MultiMenu.objectifier, "Turned <color=#FF0000FF>Traitor</color> Gets <color=#073763FF>Assassin</color>", false);
            SnitchSeesTraitor = new CustomToggleOption(true, num++, MultiMenu.objectifier, "<color=#D4AF37FF>Snitch</color> Sees Turned <color=#370D43FF>Traitor</color>", true);
            RevealerRevealsTraitor = new CustomToggleOption(true, num++, MultiMenu.objectifier, "<color=#D3D3D3FF>Revealer</color> Reveals Turned <color=#370D43FF>Traitor</color>", false);
            TraitorColourSwap = new CustomToggleOption(true, num++, MultiMenu.objectifier, "Turned <color=#370D43FF>Traitor</color> Swaps Colours For Investigative Roles", false);
        }
    }
}