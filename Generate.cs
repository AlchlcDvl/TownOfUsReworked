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
    public static CustomNumberOption InitialCooldowns;
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
    private static readonly List<string> Maps = new() { "Skeld", "Mira HQ", "Polus", /*"dlekS",*/ "Airship" };

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
    public static CustomToggleOption VentTargetting;

    //QoL Options
    public static CustomHeaderOption QualityChanges;
    public static CustomToggleOption DeadSeeEverything;
    public static CustomToggleOption ObstructNames;
    public static CustomToggleOption ParallelMedScans;

    //Better Sabotages
    public static CustomHeaderOption BetterSabotages;
    public static CustomToggleOption OxySlow;
    public static CustomNumberOption ReactorShake;
    public static CustomToggleOption ColourblindComms;
    public static CustomToggleOption MeetingColourblind;
    //public static CustomToggleOption NightVision;
    //public static CustomToggleOption EvilsIgnoreNV;

    //Better Skeld Options
    public static CustomHeaderOption BetterSkeld;
    public static CustomToggleOption SkeldVentImprovements;

    //Better Mira HQ Options
    public static CustomHeaderOption BetterMiraHQ;
    public static CustomToggleOption MiraHQVentImprovements;

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
    public static CustomStringOption GameMode;

    //Killing Only Options
    public static CustomHeaderOption KillingOnlySettings;
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
    public static CustomHeaderOption AllAnyRoleListSettings;
    public static CustomToggleOption EnableUniques;

    //CI Role Spawn
    public static CustomHeaderOption CrewInvestigativeRoles;
    public static CustomLayersOption DetectiveOn;
    public static CustomLayersOption CoronerOn;
    public static CustomLayersOption SheriffOn;
    public static CustomLayersOption MediumOn;
    public static CustomLayersOption TrackerOn;
    public static CustomLayersOption InspectorOn;
    public static CustomLayersOption OperativeOn;
    public static CustomLayersOption SeerOn;

    //CSv Role Spawn
    public static CustomHeaderOption CrewSovereignRoles;
    public static CustomLayersOption MayorOn;
    public static CustomLayersOption DictatorOn;
    public static CustomLayersOption MonarchOn;

    //CP Role Spawn
    public static CustomHeaderOption CrewProtectiveRoles;
    public static CustomLayersOption AltruistOn;
    public static CustomLayersOption MedicOn;

    //CA Role Spawn
    public static CustomHeaderOption CrewAuditorRoles;
    public static CustomLayersOption VampireHunterOn;
    public static CustomLayersOption MysticOn;

    //CK Role Spawn
    public static CustomHeaderOption CrewKillingRoles;
    public static CustomLayersOption VeteranOn;
    public static CustomLayersOption VigilanteOn;

    //CS Role Spawn
    public static CustomHeaderOption CrewSupportRoles;
    public static CustomLayersOption EngineerOn;
    public static CustomLayersOption ShifterOn;
    public static CustomLayersOption EscortOn;
    public static CustomLayersOption TransporterOn;
    public static CustomLayersOption RevealerOn;
    public static CustomLayersOption RetributionistOn;
    public static CustomLayersOption ChameleonOn;

    //CU Role Spawn
    public static CustomHeaderOption CrewUtilityRoles;
    public static CustomLayersOption CrewmateOn;

    //NB Role Spawn
    public static CustomHeaderOption NeutralBenignRoles;
    public static CustomLayersOption AmnesiacOn;
    public static CustomLayersOption GuardianAngelOn;
    public static CustomLayersOption SurvivorOn;
    public static CustomLayersOption ThiefOn;

    //NH Role Spawn
    public static CustomHeaderOption NeutralHarbingerRoles;
    public static CustomLayersOption PlaguebearerOn;

    //NP Role Spawn
    public static CustomHeaderOption NeutralProselyteRoles;
    public static CustomLayersOption PhantomOn;

    //NN Role Spawn
    public static CustomHeaderOption NeutralNeophyteRoles;
    public static CustomLayersOption DraculaOn;
    public static CustomLayersOption JackalOn;
    public static CustomLayersOption NecromancerOn;
    public static CustomLayersOption WhispererOn;

    //NE Role Spawn
    public static CustomHeaderOption NeutralEvilRoles;
    public static CustomLayersOption ExecutionerOn;
    public static CustomLayersOption ActorOn;
    public static CustomLayersOption JesterOn;
    public static CustomLayersOption CannibalOn;
    public static CustomLayersOption BountyHunterOn;
    public static CustomLayersOption TrollOn;
    public static CustomLayersOption GuesserOn;

    //NK Role Spawn
    public static CustomHeaderOption NeutralKillingRoles;
    public static CustomLayersOption ArsonistOn;
    public static CustomLayersOption CryomaniacOn;
    public static CustomLayersOption GlitchOn;
    public static CustomLayersOption MurdererOn;
    public static CustomLayersOption WerewolfOn;
    public static CustomLayersOption SerialKillerOn;
    public static CustomLayersOption JuggernautOn;

    //IC Role Spawn
    public static CustomHeaderOption IntruderConcealingRoles;
    public static CustomLayersOption BlackmailerOn;
    public static CustomLayersOption CamouflagerOn;
    public static CustomLayersOption GrenadierOn;
    public static CustomLayersOption JanitorOn;

    //ID Role Spawn
    public static CustomHeaderOption IntruderDeceptionRoles;
    public static CustomLayersOption MorphlingOn;
    public static CustomLayersOption DisguiserOn;
    public static CustomLayersOption WraithOn;

    //IK Role Spawn
    public static CustomHeaderOption IntruderKillingRoles;
    public static CustomLayersOption AmbusherOn;
    public static CustomLayersOption EnforcerOn;

    //IS Role Spawn
    public static CustomHeaderOption IntruderSupportRoles;
    public static CustomLayersOption ConsigliereOn;
    public static CustomLayersOption GodfatherOn;
    public static CustomLayersOption ConsortOn;
    public static CustomLayersOption MinerOn;
    public static CustomLayersOption TeleporterOn;

    //IU Role Spawn
    public static CustomHeaderOption IntruderUtilityRoles;
    public static CustomLayersOption ImpostorOn;
    public static CustomLayersOption GhoulOn;

    //SSu Role Spawn
    public static CustomHeaderOption SyndicateSupportRoles;
    public static CustomLayersOption WarperOn;
    public static CustomLayersOption RebelOn;
    public static CustomLayersOption StalkerOn;

    //SD Role Spawn
    public static CustomHeaderOption SyndicateDisruptionRoles;
    public static CustomLayersOption FramerOn;
    public static CustomLayersOption ShapeshifterOn;
    public static CustomLayersOption ConcealerOn;
    public static CustomLayersOption DrunkardOn;
    public static CustomLayersOption SilencerOn;

    //SyK Role Spawn
    public static CustomHeaderOption SyndicatePowerRoles;
    public static CustomLayersOption SpellslingerOn;
    public static CustomLayersOption TimeKeeperOn;

    //SyK Role Spawn
    public static CustomHeaderOption SyndicateKillingRoles;
    public static CustomLayersOption BomberOn;
    public static CustomLayersOption CrusaderOn;
    public static CustomLayersOption ColliderOn;
    public static CustomLayersOption PoisonerOn;

    //SU Role Spawn
    public static CustomHeaderOption SyndicateUtilityRoles;
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
    public static CustomHeaderOption CrewSovereignSettings;
    public static CustomNumberOption CSvMax;

    //Mayor Options
    public static CustomHeaderOption Mayor;
    public static CustomToggleOption UniqueMayor;
    public static CustomNumberOption MayorVoteCount;
    public static CustomToggleOption RoundOneNoReveal;
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
    public static CustomNumberOption KnightingCooldown;
    public static CustomToggleOption RoundOneNoKnighting;
    public static CustomToggleOption KnightButton;
    public static CustomToggleOption MonarchButton;

    //CA Options
    public static CustomHeaderOption CrewAuditorSettings;
    public static CustomNumberOption CAMax;

    //Mystic Options
    public static CustomHeaderOption MysticSettings;
    public static CustomToggleOption UniqueMystic;
    public static CustomNumberOption RevealCooldown;

    //Vampire Hunter Options
    public static CustomHeaderOption VampireHunter;
    public static CustomToggleOption UniqueVampireHunter;
    public static CustomNumberOption StakeCooldown;

    //CK Options
    public static CustomHeaderOption CrewKillingSettings;
    public static CustomNumberOption CKMax;

    //Vigilante Options
    public static CustomHeaderOption Vigilante;
    public static CustomToggleOption UniqueVigilante;
    public static CustomStringOption VigiOptions;
    public static CustomStringOption VigiNotifOptions;
    public static CustomToggleOption MisfireKillsInno;
    public static CustomToggleOption VigiKillAgain;
    public static CustomToggleOption RoundOneNoShot;
    public static CustomNumberOption VigiKillCd;
    public static CustomNumberOption VigiBulletCount;

    //Veteran Options
    public static CustomHeaderOption Veteran;
    public static CustomToggleOption UniqueVeteran;
    public static CustomNumberOption AlertCooldown;
    public static CustomNumberOption AlertDuration;
    public static CustomNumberOption MaxAlerts;

    //CS Options
    public static CustomHeaderOption CrewSupportSettings;
    public static CustomNumberOption CSMax;

    //Engineer Options
    public static CustomHeaderOption Engineer;
    public static CustomNumberOption MaxFixes;
    public static CustomToggleOption UniqueEngineer;
    public static CustomNumberOption FixCooldown;

    //Transporter Options
    public static CustomHeaderOption Transporter;
    public static CustomToggleOption UniqueTransporter;
    public static CustomToggleOption TransSelf;
    public static CustomNumberOption TransportCooldown;
    public static CustomNumberOption TransportDuration;
    public static CustomNumberOption TransportMaxUses;

    //Retributionist Options
    public static CustomHeaderOption Retributionist;
    public static CustomToggleOption UniqueRetributionist;
    public static CustomToggleOption ReviveAfterVoting;
    public static CustomNumberOption MaxUses;

    //Escort Options
    public static CustomHeaderOption Escort;
    public static CustomToggleOption UniqueEscort;
    public static CustomNumberOption EscRoleblockCooldown;
    public static CustomNumberOption EscRoleblockDuration;

    //Chameleon Options
    public static CustomHeaderOption Chameleon;
    public static CustomToggleOption UniqueChameleon;
    public static CustomNumberOption SwoopCount;
    public static CustomNumberOption SwoopCooldown;
    public static CustomNumberOption SwoopDuration;

    //Shifter Options
    public static CustomHeaderOption Shifter;
    public static CustomToggleOption UniqueShifter;
    public static CustomNumberOption ShifterCd;
    public static CustomStringOption ShiftedBecomes;

    //CI Options
    public static CustomHeaderOption CrewInvestigativeSettings;
    public static CustomNumberOption CIMax;

    //Tracker Options
    public static CustomHeaderOption Tracker;
    public static CustomToggleOption UniqueTracker;
    public static CustomNumberOption UpdateInterval;
    public static CustomNumberOption TrackCooldown;
    public static CustomToggleOption ResetOnNewRound;
    public static CustomNumberOption MaxTracks;

    //Operative Options
    public static CustomHeaderOption Operative;
    public static CustomToggleOption UniqueOperative;
    public static CustomNumberOption BugCooldown;
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
    public static CustomNumberOption SeerCooldown;

    //Detective Options
    public static CustomHeaderOption Detective;
    public static CustomToggleOption UniqueDetective;
    public static CustomNumberOption ExamineCooldown;
    public static CustomNumberOption RecentKill;
    public static CustomNumberOption FootprintInterval;
    public static CustomNumberOption FootprintDuration;
    public static CustomToggleOption AnonymousFootPrint;

    //Coroner Options
    public static CustomHeaderOption Coroner;
    public static CustomToggleOption UniqueCoroner;
    public static CustomNumberOption CoronerArrowDuration;
    public static CustomToggleOption CoronerReportName;
    public static CustomToggleOption CoronerReportRole;
    public static CustomNumberOption CoronerKillerNameTime;
    public static CustomNumberOption CompareCooldown;
    public static CustomNumberOption AutopsyCooldown;

    //Inspector Options
    public static CustomHeaderOption Inspector;
    public static CustomToggleOption UniqueInspector;
    public static CustomNumberOption InspectCooldown;

    //Medium Options
    public static CustomHeaderOption Medium;
    public static CustomToggleOption UniqueMedium;
    public static CustomNumberOption MediateCooldown;
    public static CustomNumberOption MediateDuration;
    public static CustomToggleOption ShowMediatePlayer;
    public static CustomStringOption ShowMediumToDead;
    public static CustomStringOption DeadRevealed;

    //Sheriff Options
    public static CustomHeaderOption Sheriff;
    public static CustomToggleOption UniqueSheriff;
    public static CustomNumberOption InterrogateCooldown;
    public static CustomToggleOption NeutEvilRed;
    public static CustomToggleOption NeutKillingRed;

    //CP Options
    public static CustomHeaderOption CrewProtectiveSettings;
    public static CustomNumberOption CPMax;

    //Altruist Options
    public static CustomHeaderOption Altruist;
    public static CustomToggleOption UniqueAltruist;
    public static CustomNumberOption AltReviveDuration;
    public static CustomToggleOption AltruistTargetBody;
    public static CustomNumberOption ReviveCooldown;
    public static CustomNumberOption ReviveCount;

    //Medic Options
    public static CustomHeaderOption Medic;
    public static CustomToggleOption UniqueMedic;
    public static CustomStringOption ShowShielded;
    public static CustomStringOption WhoGetsNotification;
    public static CustomToggleOption ShieldBreaks;

    //CU Options
    public static CustomHeaderOption CrewUtilitySettings;

    //Revealer Options
    public static CustomHeaderOption Revealer;
    public static CustomToggleOption RevealerKnows;
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
    public static CustomNumberOption IntruderKillCooldown;
    public static CustomNumberOption IntruderSabotageCooldown;
    public static CustomNumberOption IntruderCount;
    public static CustomNumberOption IntruderMax;
    public static CustomNumberOption IntruderMin;
    public static CustomToggleOption GhostsCanSabotage;
    public static CustomToggleOption IntruderFlashlight;

    //IC Options
    public static CustomHeaderOption IntruderConcealingSettings;
    public static CustomNumberOption ICMax;

    //Janitor Options
    public static CustomHeaderOption Janitor;
    public static CustomToggleOption UniqueJanitor;
    public static CustomNumberOption JanitorCleanCd;
    public static CustomToggleOption JaniCooldownsLinked;
    public static CustomToggleOption SoloBoost;
    public static CustomNumberOption DragCooldown;
    public static CustomStringOption JanitorVentOptions;
    public static CustomNumberOption DragModifier;

    //Blackmailer Options
    public static CustomHeaderOption Blackmailer;
    public static CustomToggleOption UniqueBlackmailer;
    public static CustomNumberOption BlackmailCooldown;
    public static CustomToggleOption WhispersNotPrivate;
    public static CustomToggleOption BlackmailMates;
    public static CustomToggleOption BMRevealed;

    //Grenadier Options
    public static CustomHeaderOption Grenadier;
    public static CustomToggleOption UniqueGrenadier;
    public static CustomNumberOption GrenadeCooldown;
    public static CustomNumberOption GrenadeDuration;
    public static CustomToggleOption GrenadierIndicators;
    public static CustomToggleOption GrenadierVent;
    public static CustomNumberOption FlashRadius;

    //Camouflager Options
    public static CustomHeaderOption Camouflager;
    public static CustomToggleOption UniqueCamouflager;
    public static CustomNumberOption CamouflagerCooldown;
    public static CustomNumberOption CamouflagerDuration;
    public static CustomToggleOption CamoHideSpeed;
    public static CustomToggleOption CamoHideSize;

    //ID Options
    public static CustomHeaderOption IntruderDeceptionSettings;
    public static CustomNumberOption IDMax;

    //Morphling Options
    public static CustomHeaderOption Morphling;
    public static CustomToggleOption UniqueMorphling;
    public static CustomNumberOption MorphlingCooldown;
    public static CustomNumberOption MorphlingDuration;
    public static CustomToggleOption MorphlingVent;
    public static CustomToggleOption MorphCooldownsLinked;
    public static CustomNumberOption SampleCooldown;

    //Disguiser Options
    public static CustomHeaderOption Disguiser;
    public static CustomToggleOption UniqueDisguiser;
    public static CustomNumberOption DisguiseCooldown;
    public static CustomNumberOption TimeToDisguise;
    public static CustomNumberOption DisguiseDuration;
    public static CustomStringOption DisguiseTarget;
    public static CustomToggleOption DisgCooldownsLinked;
    public static CustomNumberOption MeasureCooldown;

    //Wraith Options
    public static CustomHeaderOption Wraith;
    public static CustomToggleOption UniqueWraith;
    public static CustomNumberOption InvisCooldown;
    public static CustomNumberOption InvisDuration;
    public static CustomToggleOption WraithVent;

    //IS Options
    public static CustomHeaderOption IntruderSupportSettings;
    public static CustomNumberOption ISMax;

    //Teleporter Options
    public static CustomHeaderOption Teleporter;
    public static CustomToggleOption UniqueTeleporter;
    public static CustomNumberOption TeleportCd;
    public static CustomNumberOption MarkCooldown;
    public static CustomToggleOption TeleVent;
    public static CustomToggleOption TeleCooldownsLinked;

    //Consigliere Options
    public static CustomHeaderOption Consigliere;
    public static CustomToggleOption UniqueConsigliere;
    public static CustomNumberOption InvestigateCooldown;
    public static CustomStringOption ConsigInfo;

    //Consort Options
    public static CustomHeaderOption Consort;
    public static CustomToggleOption UniqueConsort;
    public static CustomNumberOption ConsRoleblockCooldown;
    public static CustomNumberOption ConsRoleblockDuration;

    //Godfather Options
    public static CustomHeaderOption Godfather;
    public static CustomToggleOption UniqueGodfather;
    public static CustomNumberOption MafiosoAbilityCooldownDecrease;

    //Miner Options
    public static CustomHeaderOption Miner;
    public static CustomToggleOption UniqueMiner;
    public static CustomNumberOption MineCooldown;

    //IK Options
    public static CustomHeaderOption IntruderKillingSettings;
    public static CustomNumberOption IKMax;

    //Ambusher Options
    public static CustomHeaderOption Ambusher;
    public static CustomToggleOption UniqueAmbusher;
    public static CustomNumberOption AmbushCooldown;
    public static CustomNumberOption AmbushDuration;
    public static CustomToggleOption AmbushMates;

    //Enforcer Options
    public static CustomHeaderOption Enforcer;
    public static CustomToggleOption UniqueEnforcer;
    public static CustomNumberOption EnforceCooldown;
    public static CustomNumberOption EnforceDuration;
    public static CustomNumberOption EnforceDelay;
    public static CustomNumberOption EnforceRadius;

    //IU Options
    public static CustomHeaderOption IntruderUtilitySettings;

    //Ghoul Options
    public static CustomHeaderOption Ghoul;
    public static CustomNumberOption GhoulMarkCd;

    //Syndicate Options
    public static CustomHeaderOption SyndicateSettings;
    public static CustomNumberOption SyndicateVision;
    public static CustomStringOption SyndicateVent;
    public static CustomNumberOption ChaosDriveMeetingCount;
    public static CustomNumberOption ChaosDriveKillCooldown;
    public static CustomNumberOption SyndicateCount;
    public static CustomToggleOption AltImps;
    public static CustomToggleOption GlobalDrive;
    public static CustomNumberOption SyndicateMax;
    public static CustomNumberOption SyndicateMin;
    public static CustomToggleOption SyndicateFlashlight;

    //SD Options
    public static CustomHeaderOption SyndicateDisruptionSettings;
    public static CustomNumberOption SDMax;

    //Shapeshifter Options
    public static CustomHeaderOption Shapeshifter;
    public static CustomToggleOption UniqueShapeshifter;
    public static CustomNumberOption ShapeshiftCooldown;
    public static CustomNumberOption ShapeshiftDuration;
    public static CustomToggleOption ShapeshiftMates;

    //Drunkard Options
    public static CustomHeaderOption Drunkard;
    public static CustomToggleOption UniqueDrunkard;
    public static CustomNumberOption ConfuseCooldown;
    public static CustomNumberOption ConfuseDuration;
    public static CustomToggleOption ConfuseImmunity;

    //Concealer Options
    public static CustomHeaderOption Concealer;
    public static CustomToggleOption UniqueConcealer;
    public static CustomNumberOption ConcealCooldown;
    public static CustomNumberOption ConcealDuration;
    public static CustomToggleOption ConcealMates;

    //Silencer Options
    public static CustomHeaderOption Silencer;
    public static CustomToggleOption UniqueSilencer;
    public static CustomNumberOption SilenceCooldown;
    public static CustomToggleOption WhispersNotPrivateSilencer;
    public static CustomToggleOption SilenceMates;
    public static CustomToggleOption SilenceRevealed;

    //Framer Options
    public static CustomHeaderOption Framer;
    public static CustomNumberOption FrameCooldown;
    public static CustomNumberOption ChaosDriveFrameRadius;
    public static CustomToggleOption UniqueFramer;

    //SyK Options
    public static CustomHeaderOption SyndicateKillingSettings;
    public static CustomNumberOption SyKMax;

    //Crusader Options
    public static CustomHeaderOption Crusader;
    public static CustomToggleOption UniqueCrusader;
    public static CustomNumberOption CrusadeCooldown;
    public static CustomNumberOption CrusadeDuration;
    public static CustomNumberOption ChaosDriveCrusadeRadius;
    public static CustomToggleOption CrusadeMates;

    //Bomber Options
    public static CustomHeaderOption Bomber;
    public static CustomToggleOption UniqueBomber;
    public static CustomNumberOption BombCooldown;
    public static CustomNumberOption DetonateCooldown;
    public static CustomToggleOption BombCooldownsLinked;
    public static CustomToggleOption BombsRemoveOnNewRound;
    public static CustomToggleOption BombsDetonateOnMeetingStart;
    public static CustomNumberOption BombRange;
    public static CustomNumberOption ChaosDriveBombRange;
    public static CustomToggleOption BombKillsSyndicate;

    //Poisoner Options
    public static CustomHeaderOption Poisoner;
    public static CustomToggleOption UniquePoisoner;
    public static CustomNumberOption PoisonCooldown;
    public static CustomNumberOption PoisonDuration;

    //Collider Options
    public static CustomHeaderOption Collider;
    public static CustomToggleOption UniqueCollider;
    public static CustomNumberOption CollideCooldown;
    public static CustomNumberOption ChargeCooldown;
    public static CustomNumberOption ChargeDuration;
    public static CustomNumberOption CollideRange;
    public static CustomNumberOption CollideRangeIncrease;
    public static CustomToggleOption ChargeCooldownsLinked;
    public static CustomToggleOption CollideResetsCooldown;

    //SSu Options
    public static CustomHeaderOption SyndicateSupportSettings;
    public static CustomNumberOption SSuMax;

    //Rebel Options
    public static CustomHeaderOption Rebel;
    public static CustomToggleOption UniqueRebel;
    public static CustomNumberOption SidekickAbilityCooldownDecrease;

    //Stalker Options
    public static CustomHeaderOption Stalker;
    public static CustomToggleOption UniqueStalker;
    public static CustomNumberOption StalkCooldown;

    //Warper Options
    public static CustomHeaderOption Warper;
    public static CustomNumberOption WarpCooldown;
    public static CustomNumberOption WarpDuration;
    public static CustomToggleOption UniqueWarper;
    public static CustomToggleOption WarpSelf;

    //SU Options
    public static CustomHeaderOption SyndicateUtilitySettings;

    //Anarchist Options
    public static CustomHeaderOption Anarchist;
    public static CustomNumberOption AnarchKillCooldown;

    //Banshee Options
    public static CustomHeaderOption Banshee;
    public static CustomNumberOption ScreamCooldown;
    public static CustomNumberOption ScreamDuration;

    //SP Options
    public static CustomHeaderOption SyndicatePowerSettings;
    public static CustomNumberOption SPMax;

    //Spellslinger Options
    public static CustomHeaderOption Spellslinger;
    public static CustomNumberOption SpellCooldown;
    public static CustomNumberOption SpellCooldownIncrease;
    public static CustomToggleOption UniqueSpellslinger;

    //Time Keeper Options
    public static CustomHeaderOption TimeKeeper;
    public static CustomToggleOption UniqueTimeKeeper;
    public static CustomNumberOption TimeControlCooldown;
    public static CustomNumberOption TimeControlDuration;
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
    public static CustomToggleOption AvoidNeutralBenignKingmakers;
    public static CustomToggleOption AvoidNeutralEvilKingmakers;
    public static CustomToggleOption NeutralFlashlight;

    //NA Options
    public static CustomHeaderOption NeutralApocalypseSettings;

    //Pestilence Options
    public static CustomHeaderOption Pestilence;
    public static CustomToggleOption PestSpawn;
    public static CustomToggleOption PlayersAlerted;
    public static CustomNumberOption PestKillCooldown;
    public static CustomToggleOption PestVent;

    //NB Options
    public static CustomHeaderOption NeutralBenignSettings;
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
    public static CustomNumberOption VestDuration;
    public static CustomNumberOption VestKCReset;
    public static CustomToggleOption SurvVent;
    public static CustomToggleOption SurvSwitchVent;
    public static CustomNumberOption MaxVests;
    public static CustomToggleOption UniqueSurvivor;

    //Guardian Angel Options
    public static CustomHeaderOption GuardianAngel;
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
    public static CustomToggleOption GuardianAngelCanPickTargets;

    //Thief Options
    public static CustomHeaderOption Thief;
    public static CustomToggleOption ThiefVent;
    public static CustomNumberOption ThiefKillCooldown;
    public static CustomToggleOption UniqueThief;
    public static CustomToggleOption ThiefSteals;
    public static CustomToggleOption ThiefCanGuess;
    public static CustomToggleOption ThiefCanGuessAfterVoting;

    //NE Options
    public static CustomHeaderOption NeutralEvilSettings;
    public static CustomNumberOption NEMax;
    public static CustomToggleOption NeutralEvilsEndGame;

    //Jester Options
    public static CustomHeaderOption Jester;
    public static CustomToggleOption JesterButton;
    public static CustomToggleOption JesterVent;
    public static CustomToggleOption JestSwitchVent;
    public static CustomToggleOption JestEjectScreen;
    public static CustomToggleOption VigiKillsJester;
    public static CustomNumberOption HauntCooldown;
    public static CustomNumberOption HauntCount;
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
    public static CustomNumberOption InteractCooldown;
    public static CustomToggleOption TrollVent;
    public static CustomToggleOption TrollSwitchVent;
    public static CustomToggleOption UniqueTroll;

    //Cannibal Options
    public static CustomHeaderOption Cannibal;
    public static CustomNumberOption CannibalCd;
    public static CustomNumberOption CannibalBodyCount;
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
    public static CustomNumberOption DoomCooldown;
    public static CustomNumberOption DoomCount;
    public static CustomToggleOption ExecutionerCanPickTargets;

    //Bounty Hunter Options
    public static CustomHeaderOption BountyHunter;
    public static CustomToggleOption BHVent;
    public static CustomNumberOption BountyHunterCooldown;
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
    public static CustomNumberOption GuessCount;
    public static CustomToggleOption GuesserAfterVoting;
    public static CustomToggleOption MultipleGuesses;
    public static CustomToggleOption GuesserCanPickTargets;

    //NH Options
    public static CustomHeaderOption NeutralHarbingerSettings;
    public static CustomNumberOption NHMax;

    //Plaguebearer Options
    public static CustomHeaderOption Plaguebearer;
    public static CustomNumberOption InfectCooldown;
    public static CustomToggleOption PBVent;
    public static CustomToggleOption UniquePlaguebearer;

    //NK Options
    public static CustomHeaderOption NeutralKillingSettings;
    public static CustomNumberOption NKMax;
    public static CustomToggleOption NKHasImpVision;
    public static CustomToggleOption NKsKnow;

    //Glitch Options
    public static CustomHeaderOption Glitch;
    public static CustomNumberOption HackCooldown;
    public static CustomNumberOption MimicCooldown;
    public static CustomNumberOption MimicDuration;
    public static CustomNumberOption HackDuration;
    public static CustomNumberOption GlitchKillCooldown;
    public static CustomToggleOption GlitchVent;
    public static CustomToggleOption UniqueGlitch;

    //Juggernaut Options
    public static CustomHeaderOption Juggernaut;
    public static CustomToggleOption JuggVent;
    public static CustomNumberOption JuggKillCooldown;
    public static CustomNumberOption JuggKillBonus;
    public static CustomToggleOption UniqueJuggernaut;

    //Cryomaniac Options
    public static CustomHeaderOption Cryomaniac;
    public static CustomNumberOption CryoDouseCooldown;
    public static CustomToggleOption CryoVent;
    public static CustomToggleOption UniqueCryomaniac;
    public static CustomToggleOption CryoFreezeAll;
    public static CustomToggleOption CryoLastKillerBoost;

    //Arsonist Options
    public static CustomHeaderOption Arsonist;
    public static CustomNumberOption DouseCooldown;
    public static CustomNumberOption IgniteCooldown;
    public static CustomToggleOption ArsoVent;
    public static CustomToggleOption ArsoIgniteAll;
    public static CustomToggleOption ArsoLastKillerBoost;
    public static CustomToggleOption ArsoCooldownsLinked;
    public static CustomToggleOption UniqueArsonist;
    public static CustomToggleOption IgnitionCremates;

    //Murderer Options
    public static CustomHeaderOption Murderer;
    public static CustomToggleOption MurdVent;
    public static CustomNumberOption MurdKillCooldownOption;
    public static CustomToggleOption UniqueMurderer;

    //Serial Killer Options
    public static CustomHeaderOption SerialKiller;
    public static CustomNumberOption BloodlustCooldown;
    public static CustomNumberOption BloodlustDuration;
    public static CustomNumberOption LustKillCooldown;
    public static CustomStringOption SKVentOptions;
    public static CustomToggleOption UniqueSerialKiller;

    //Werewolf Options
    public static CustomHeaderOption Werewolf;
    public static CustomNumberOption MaulCooldown;
    public static CustomNumberOption MaulRadius;
    public static CustomToggleOption WerewolfVent;
    public static CustomToggleOption UniqueWerewolf;

    //NN Options
    public static CustomHeaderOption NeutralNeophyteSettings;
    public static CustomNumberOption NNMax;
    public static CustomToggleOption NNHasImpVision;

    //Dracula Options
    public static CustomHeaderOption Dracula;
    public static CustomNumberOption BiteCooldown;
    public static CustomNumberOption AliveVampCount;
    public static CustomToggleOption DracVent;
    public static CustomToggleOption UniqueDracula;
    public static CustomToggleOption UndeadVent;

    //Necromancer Options
    public static CustomHeaderOption Necromancer;
    public static CustomNumberOption ResurrectCooldown;
    public static CustomNumberOption NecroKillCooldown;
    public static CustomNumberOption NecroKillCooldownIncrease;
    public static CustomToggleOption NecroKillCooldownIncreases;
    public static CustomNumberOption NecroKillCount;
    public static CustomNumberOption ResurrectCooldownIncrease;
    public static CustomToggleOption ResurrectCooldownIncreases;
    public static CustomNumberOption ResurrectCount;
    public static CustomToggleOption NecroVent;
    public static CustomToggleOption ResurrectVent;
    public static CustomToggleOption KillResurrectCooldownsLinked;
    public static CustomToggleOption NecromancerTargetBody;
    public static CustomNumberOption NecroResurrectDuration;
    public static CustomToggleOption UniqueNecromancer;

    //Whisperer Options
    public static CustomHeaderOption Whisperer;
    public static CustomNumberOption WhisperCooldown;
    public static CustomNumberOption WhisperRadius;
    public static CustomNumberOption WhisperCooldownIncrease;
    public static CustomToggleOption WhisperCooldownIncreases;
    public static CustomNumberOption WhisperRateDecrease;
    public static CustomNumberOption InitialWhisperRate;
    public static CustomToggleOption WhisperRateDecreases;
    public static CustomToggleOption WhispVent;
    public static CustomToggleOption UniqueWhisperer;
    public static CustomToggleOption PersuadedVent;

    //Jackal Options
    public static CustomHeaderOption Jackal;
    public static CustomNumberOption RecruitCooldown;
    public static CustomToggleOption JackalVent;
    public static CustomToggleOption RecruitVent;
    public static CustomToggleOption UniqueJackal;

    //NP Options
    public static CustomHeaderOption NeutralProselyteSettings;

    //Phantom Options
    public static CustomHeaderOption Phantom;
    public static CustomNumberOption PhantomTasksRemaining;
    public static CustomToggleOption PhantomPlayersAlerted;

    //Betrayer Options
    public static CustomHeaderOption Betrayer;
    public static CustomNumberOption BetrayerKillCooldown;
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
    public static CustomNumberOption CorruptedKillCooldown;
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

    /*For Testing
    public static CustomNestedOption ExampleNested;
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
        if (SubLoaded)
            Maps.Add("Submerged");

        if (LILoaded)
            Maps.Add("LevelImpostor");
        
        Maps.Add("Random");

        SettingsPatches.ExportButton = new();
        SettingsPatches.ImportButton = new();
        SettingsPatches.PresetButton = new();

        var num = 0;

        /*ExampleNested = new(MultiMenu.main, "Exampled Nested Option");
        ExampleLayers = new(num++, MultiMenu.main, "Example Layers Option", "Layer");
        ExampleHeader = new(num++, MultiMenu.external, "Example Header Option");
        ExampleToggle = new(num++, MultiMenu.external, "Example Toggle Option", true/false);
        ExampleNumber = new(num++, MultiMenu.external, "Example Number Option", 1, 1, 5, 1, MultiplierFormat);
        ExampleString = new(num++, MultiMenu.external, "Example String Option", new[] { "Something", "Something Else", "Something Else Else" });
        ExampleLayers.AddOptions(ExampleHeader, ExampleToggle, ExampleNumber, ExampleString);
        ExampleNested.AddOptions(ExampleHeader, ExampleToggle, ExampleNumber, ExampleString, ExampleLayers);*/

        GameSettings = new(MultiMenu.main, "Game Settings");
        PlayerSpeed = new(num++, MultiMenu.main, "Player Speed", 1.25f, 0.25f, 10, 0.25f, MultiplierFormat);
        GhostSpeed = new(num++, MultiMenu.main, "Ghost Speed", 3, 0.25f, 10, 0.25f, MultiplierFormat);
        InteractionDistance = new(num++, MultiMenu.main, "Interaction Distance", 2, 0.5f, 5, 0.5f, DistanceFormat);
        EmergencyButtonCount = new(num++, MultiMenu.main, "Emergency Button Count", 1, 0, 100, 1);
        EmergencyButtonCooldown = new(num++, MultiMenu.main, "Emergency Button Cooldown", 25, 0, 300, 5, CooldownFormat);
        DiscussionTime = new(num++, MultiMenu.main, "Discussion Time", 30, 0, 300, 5, CooldownFormat);
        VotingTime = new(num++, MultiMenu.main, "Voting Time", 60, 5, 600, 15, CooldownFormat);
        TaskBarMode = new(num++, MultiMenu.main, "Taskbar Updates", new[] { "Meeting Only", "Normal", "Invisible" });
        ConfirmEjects = new(num++, MultiMenu.main, "Confirm Ejects", false);
        EjectionRevealsRole = new(num++, MultiMenu.main, "Ejection Reveals <color=#FFD700FF>Roles</color>", false);
        InitialCooldowns = new(num++, MultiMenu.main, "Game Start Cooldowns", 10, 0, 30, 2.5f, CooldownFormat);
        MeetingCooldowns = new(num++, MultiMenu.main, "Meeting End Cooldowns", 15, 0, 30, 2.5f, CooldownFormat);
        ReportDistance = new(num++, MultiMenu.main, "Player Report Distance", 3.5f, 1, 20, 0.25f, DistanceFormat);
        ChatCooldown = new(num++, MultiMenu.main, "Chat Cooldown", 3, 0, 3, 0.1f, CooldownFormat);
        ChatCharacterLimit = new(num++, MultiMenu.main, "Chat Character Limit", 200, 50, 2000, 50);
        LobbySize = new(num++, MultiMenu.main, "Lobby Size", 15, 3, 127, 1);

        GameModeSettings = new(MultiMenu.main, "Game Mode Settings");
        GameMode = new(num++, MultiMenu.main, "Game Mode", new[] { "Classic", "All Any", "Killing Only", "Role List", "Custom", "Vanilla" });

        KillingOnlySettings = new(MultiMenu.main, "<color=#1D7CF2FF>Killing</color> Only Settings");
        NeutralRoles = new(num++, MultiMenu.main, "<color=#B3B3B3FF>Neutrals</color> Count", 1, 0, 13, 1);
        AddArsonist = new(num++, MultiMenu.main, "Add <color=#EE7600FF>Arsonist</color>", false);
        AddCryomaniac = new(num++, MultiMenu.main, "Add <color=#642DEAFF>Cryomaniac</color>", false);
        AddPlaguebearer = new(num++, MultiMenu.main, "Add <color=#CFFE61FF>Plaguebearer</color>", false);

        AllAnyRoleListSettings = new(MultiMenu.main, "All Any/Role List Settings");
        EnableUniques = new(num++, MultiMenu.main, "Enable Uniques", false);

        GameModifiers = new(MultiMenu.main, "Game Modifiers");
        WhoCanVent = new(num++, MultiMenu.main, "Serial Venters", new[] { "Default", "Everyone", "Never" });
        AnonymousVoting = new(num++, MultiMenu.main, "Anonymous Voting", true);
        SkipButtonDisable = new(num++, MultiMenu.main, "No Skipping", new[] { "Never", "Emergency", "Always" });
        FirstKillShield = new(num++, MultiMenu.main, "Enable First Kill Shield", false);
        WhoSeesFirstKillShield = new(num++, MultiMenu.main, "Who Sees First Kill Shield", new[] { "Everyone", "Shielded", "No One" });
        FactionSeeRoles = new(num++, MultiMenu.main, "Factioned Evils See The <color=#FFD700FF>Roles</color> Of Their Team", true);
        VisualTasks = new(num++, MultiMenu.main, "Visual Tasks", false);
        NoNames = new(num++, MultiMenu.main, "No Player Names", false);
        Whispers = new(num++, MultiMenu.main, "PSSST *Whispers*", true);
        WhispersAnnouncement = new(num++, MultiMenu.main, "Everyone Is Alerted To Whispers", true);
        AppearanceAnimation = new(num++, MultiMenu.main, "Kill Animations Show Modified Player", true);
        RandomSpawns = new(num++, MultiMenu.main, "Random Player Spawns", false);
        EnableAbilities = new(num++, MultiMenu.main, "Enable <color=#FF9900FF>Abilities</color>", true);
        EnableModifiers = new(num++, MultiMenu.main, "Enable <color=#7F7F7FFF>Modifiers</color>", true);
        EnableObjectifiers = new(num++, MultiMenu.main, "Enable <color=#DD585BFF>Objectifiers</color>", true);
        VentTargetting = new(num++, MultiMenu.main, "Players In Vents Can Be Targetted", true);

        GameAnnouncementsSettings = new(MultiMenu.main, "Game Announcement Settings");
        GameAnnouncements = new(num++, MultiMenu.main, "Enable Game Announcements", false);
        LocationReports = new(num++, MultiMenu.main, "Reported Body's Location Is Announced", false);
        RoleFactionReports = new(num++, MultiMenu.main, "Every Body's Role/Faction Is Announced", new[] { "Never", "Roles", "Factions" });
        KillerReports = new(num++, MultiMenu.main, "Every Body's Killer's Role/Faction Is Announced", new[] { "Never", "Roles", "Factions" });

        QualityChanges = new(MultiMenu.main, "Quality Additions");
        DeadSeeEverything = new(num++, MultiMenu.main, "Dead Can See Everything", true);
        ParallelMedScans = new(num++, MultiMenu.main, "Parallel Medbay Scans", false);
        ObstructNames = new(num++, MultiMenu.main, "Hide Obstructed Player Names", true);

        MapSettings = new(MultiMenu.main, "Map Settings");
        Map = new(num++, MultiMenu.main, "Map", Maps.ToArray());
        RandomMapSkeld = new(num++, MultiMenu.main, "Skeld Chance", 0, 0, 100, 10, PercentFormat);
        RandomMapMira = new(num++, MultiMenu.main, "Mira Chance", 0, 0, 100, 10, PercentFormat);
        RandomMapPolus = new(num++, MultiMenu.main, "Polus Chance", 0, 0, 100, 10, PercentFormat);
        //RandomMapdlekS = new(num++, MultiMenu.main, "dlekS Chance", 0, 0, 100, 10, PercentFormat); for when it comes back lol
        RandomMapAirship = new(num++, MultiMenu.main, "Airship Chance", 0, 0, 100, 10, PercentFormat);

        if (SubLoaded)
            RandomMapSubmerged = new(num++, MultiMenu.main, "Submerged Chance", 0, 0, 100, 10, PercentFormat);

        if (LILoaded)
            RandomMapLevelImpostor = new(num++, MultiMenu.main, "Level Impostor Chance", 0, 0, 100, 10, PercentFormat);

        AutoAdjustSettings = new(num++, MultiMenu.main, "Auto Adjust Settings", false);
        SmallMapHalfVision = new(num++, MultiMenu.main, "Half Vision On Skeld/Mira HQ", false);
        SmallMapDecreasedCooldown = new(num++, MultiMenu.main, "Mira HQ Decreased Cooldowns", 0f, 0f, 15f, 2.5f, CooldownFormat);
        LargeMapIncreasedCooldown = new(num++, MultiMenu.main, "Airship/Submerged Increased Cooldowns", 0f, 0f, 15f, 2.5f, CooldownFormat);
        SmallMapIncreasedShortTasks = new(num++, MultiMenu.main, "Skeld/Mira HQ Increased Short Tasks", 0, 0, 5, 1);
        SmallMapIncreasedLongTasks = new(num++, MultiMenu.main, "Skeld/Mira HQ Increased Long Tasks", 0, 0, 3, 1);
        LargeMapDecreasedShortTasks = new(num++, MultiMenu.main, "Airship/Submerged Decreased Short Tasks", 0, 0, 5, 1);
        LargeMapDecreasedLongTasks = new(num++, MultiMenu.main, "Airship/Submerged Decreased Long Tasks", 0, 0, 3, 1);

        BetterSabotages = new(MultiMenu.main, "Sabotage Settings");
        ColourblindComms = new(num++, MultiMenu.main, "Camouflaged Comms", true);
        MeetingColourblind = new(num++, MultiMenu.main, "Camouflaged Meetings", false);
        //NightVision = new(num++, MultiMenu.main, "Night Vision Cameras", false);
        //EvilsIgnoreNV = new(num++, MultiMenu.main, "High Vision Evils Ignore Vision", false);
        OxySlow = new(num++, MultiMenu.main, "Oxygen Sabotage Slows Down Players", true);
        ReactorShake = new(num++, MultiMenu.main, "Reactor Sabotage Shakes The Screen By", 30, 0, 100, 1, PercentFormat);

        BetterSkeld = new(MultiMenu.main, "Skeld Settings");
        SkeldVentImprovements = new(num++, MultiMenu.main, "Changed Skeld Vent Layout", false);

        BetterMiraHQ = new(MultiMenu.main, "Mira HQ Settings");
        MiraHQVentImprovements = new(num++, MultiMenu.main, "Changed Mira HQ Vent Layout", false);

        BetterPolus = new(MultiMenu.main, "Polus Settings");
        PolusVentImprovements = new(num++, MultiMenu.main, "Changed Polus Vent Layout", false);
        VitalsLab = new(num++, MultiMenu.main, "Vitals Moved To Lab", false);
        ColdTempDeathValley = new(num++, MultiMenu.main, "Cold Temp Moved To Death Valley", false);
        WifiChartCourseSwap = new(num++, MultiMenu.main, "Reboot Wifi And Chart Course Swapped", false);
        SeismicTimer = new(num++, MultiMenu.main, "Seimic Stabliser Malfunction Countdown", 60f, 30f, 90f, 5f, CooldownFormat);

        BetterAirship = new(MultiMenu.main, "Airship Settings");
        SpawnType = new(num++, MultiMenu.main, "Spawn Type", new[] { "Normal", "Fixed", "Random Synchronised", "Meeting" });
        MoveVitals = new(num++, MultiMenu.main, "Move Vitals", false);
        MoveFuel = new(num++, MultiMenu.main, "Move Fuel", false);
        MoveDivert = new(num++, MultiMenu.main, "Move Divert Power", false);
        MoveAdmin = new(num++, MultiMenu.main, "Move Admin Table", new[] { "Don't Move", "Right Of Cockpit", "Main Hall" });
        MoveElectrical = new(num++, MultiMenu.main, "Move Electrical Outlet", new[] { "Don't Move", "Vault", "Electrical" });
        MinDoorSwipeTime = new(num++, MultiMenu.main, "Min Time For Door Swipe", 0.4f, 0f, 10f, 0.1f);
        CrashTimer = new(num++, MultiMenu.main, "Heli Crash Countdown", 90f, 30f, 100f, 5f);

        CrewAuditorRoles = new(MultiMenu.crew, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Auditor</color> <color=#FFD700FF>Roles</color>");
        MysticOn = new(num++, MultiMenu.crew, "<color=#708EEFFF>Mystic</color>");
        VampireHunterOn = new(num++, MultiMenu.crew, "<color=#C0C0C0FF>Vampire Hunter</color>");

        CrewInvestigativeRoles = new(MultiMenu.crew, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Investigative</color> <color=#FFD700FF>Roles</color>");
        CoronerOn = new(num++, MultiMenu.crew, "<color=#4D99E6FF>Coroner</color>");
        DetectiveOn = new(num++, MultiMenu.crew, "<color=#4D4DFFFF>Detective</color>");
        InspectorOn = new(num++, MultiMenu.crew, "<color=#7E3C64FF>Inspector</color>");
        MediumOn = new(num++, MultiMenu.crew, "<color=#A680FFFF>Medium</color>");
        OperativeOn = new(num++, MultiMenu.crew, "<color=#A7D1B3FF>Operative</color>");
        SeerOn = new(num++, MultiMenu.crew, "<color=#71368AFF>Seer</color>");
        SheriffOn = new(num++, MultiMenu.crew, "<color=#FFCC80FF>Sheriff</color>");
        TrackerOn = new(num++, MultiMenu.crew, "<color=#009900FF>Tracker</color>");

        CrewKillingRoles = new(MultiMenu.crew, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Killing</color> <color=#FFD700FF>Roles</color>");
        VeteranOn = new(num++, MultiMenu.crew, "<color=#998040FF>Veteran</color>");
        VigilanteOn = new(num++, MultiMenu.crew, "<color=#FFFF00FF>Vigilante</color>");

        CrewProtectiveRoles = new(MultiMenu.crew, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Protective</color> <color=#FFD700FF>Roles</color>");
        AltruistOn = new(num++, MultiMenu.crew, "<color=#660000FF>Altruist</color>");
        MedicOn = new(num++, MultiMenu.crew, "<color=#006600FF>Medic</color>");

        CrewSovereignRoles = new(MultiMenu.crew, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Sovereign</color> <color=#FFD700FF>Roles</color>");
        DictatorOn = new(num++, MultiMenu.crew, "<color=#00CB97FF>Dictator</color>");
        MayorOn = new(num++, MultiMenu.crew, "<color=#704FA8FF>Mayor</color>");
        MonarchOn = new(num++, MultiMenu.crew, "<color=#FF004EFF>Monarch</color>");

        CrewSupportRoles = new(MultiMenu.crew, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Support</color> <color=#FFD700FF>Roles</color>");
        ChameleonOn = new(num++, MultiMenu.crew, "<color=#5411F8FF>Chameleon</color>");
        EngineerOn = new(num++, MultiMenu.crew, "<color=#FFA60AFF>Engineer</color>");
        EscortOn = new(num++, MultiMenu.crew, "<color=#803333FF>Escort</color>");
        RetributionistOn = new(num++, MultiMenu.crew, "<color=#8D0F8CFF>Retributionist</color>");
        ShifterOn = new(num++, MultiMenu.crew, "<color=#DF851FFF>Shifter</color>");
        TransporterOn = new(num++, MultiMenu.crew, "<color=#00EEFFFF>Transporter</color>");

        CrewUtilityRoles = new(MultiMenu.crew, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Utility</color> <color=#FFD700FF>Roles</color>");
        CrewmateOn = new(num++, MultiMenu.crew, "<color=#8CFFFFFF>Crewmate</color>");
        RevealerOn = new(num++, MultiMenu.crew, "<color=#D3D3D3FF>Revealer</color>");

        NeutralBenignRoles = new(MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Benign</color> <color=#FFD700FF>Roles</color>");
        AmnesiacOn = new(num++, MultiMenu.neutral, "<color=#22FFFFFF>Amnesiac</color>");
        GuardianAngelOn = new(num++, MultiMenu.neutral, "<color=#FFFFFFFF>Guardian Angel</color>");
        SurvivorOn = new(num++, MultiMenu.neutral, "<color=#DDDD00FF>Survivor</color>");
        ThiefOn = new(num++, MultiMenu.neutral, "<color=#80FF00FF>Thief</color>");

        NeutralEvilRoles = new(MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Evil</color> <color=#FFD700FF>Roles</color>");
        ActorOn = new(num++, MultiMenu.neutral, "<color=#00ACC2FF>Actor</color>");
        BountyHunterOn = new(num++, MultiMenu.neutral, "<color=#B51E39FF>Bounty Hunter</color>");
        CannibalOn = new(num++, MultiMenu.neutral, "<color=#8C4005FF>Cannibal</color>");
        ExecutionerOn = new(num++, MultiMenu.neutral, "<color=#CCCCCCFF>Executioner</color>");
        GuesserOn = new(num++, MultiMenu.neutral, "<color=#EEE5BEFF>Guesser</color>");
        JesterOn = new(num++, MultiMenu.neutral, "<color=#F7B3DAFF>Jester</color>");
        TrollOn = new(num++, MultiMenu.neutral, "<color=#678D36FF>Troll</color>");

        NeutralHarbingerRoles = new(MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Harbinger</color> <color=#FFD700FF>Roles</color>");
        PlaguebearerOn = new(num++, MultiMenu.neutral, "<color=#CFFE61FF>Plaguebearer</color>");

        NeutralKillingRoles = new(MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killing</color> <color=#FFD700FF>Roles</color>");
        ArsonistOn = new(num++, MultiMenu.neutral, "<color=#EE7600FF>Arsonist</color>");
        CryomaniacOn = new(num++, MultiMenu.neutral, "<color=#642DEAFF>Cryomaniac</color>");
        GlitchOn = new(num++, MultiMenu.neutral, "<color=#00FF00FF>Glitch</color>");
        JuggernautOn = new(num++, MultiMenu.neutral, "<color=#A12B56FF>Juggernaut</color>");
        MurdererOn = new(num++, MultiMenu.neutral, "<color=#6F7BEAFF>Murderer</color>");
        SerialKillerOn = new(num++, MultiMenu.neutral, "<color=#336EFFFF>Serial Killer</color>");
        WerewolfOn = new(num++, MultiMenu.neutral, "<color=#9F703AFF>Werewolf</color>");

        NeutralNeophyteRoles = new(MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Neophyte</color> <color=#FFD700FF>Roles</color>");
        DraculaOn = new(num++, MultiMenu.neutral, "<color=#AC8A00FF>Dracula</color>");
        JackalOn = new(num++, MultiMenu.neutral, "<color=#45076AFF>Jackal</color>");
        NecromancerOn = new(num++, MultiMenu.neutral, "<color=#BF5FFFFF>Necromancer</color>");
        WhispererOn = new(num++, MultiMenu.neutral, "<color=#2D6AA5FF>Whisperer</color>");

        NeutralNeophyteRoles = new(MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Proselyte</color> <color=#FFD700FF>Roles</color>");
        PhantomOn = new(num++, MultiMenu.neutral, "<color=#662962FF>Phantom</color>");

        IntruderConcealingRoles = new(MultiMenu.intruder, "<color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Concealing</color> <color=#FFD700FF>Roles</color>");
        BlackmailerOn = new(num++, MultiMenu.intruder, "<color=#02A752FF>Blackmailer</color>");
        CamouflagerOn = new(num++, MultiMenu.intruder, "<color=#378AC0FF>Camouflager</color>");
        GrenadierOn = new(num++, MultiMenu.intruder, "<color=#85AA5BFF>Grenadier</color>");
        JanitorOn = new(num++, MultiMenu.intruder, "<color=#2647A2FF>Janitor</color>");

        IntruderDeceptionRoles = new(MultiMenu.intruder, "<color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Deception</color> <color=#FFD700FF>Roles</color>");
        DisguiserOn = new(num++, MultiMenu.intruder, "<color=#40B4FFFF>Disguiser</color>");
        MorphlingOn = new(num++, MultiMenu.intruder, "<color=#BB45B0FF>Morphling</color>");
        WraithOn = new(num++, MultiMenu.intruder, "<color=#5C4F75FF>Wraith</color>");

        IntruderKillingRoles = new(MultiMenu.intruder, "<color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Killing</color> <color=#FFD700FF>Roles</color>");
        AmbusherOn = new(num++, MultiMenu.intruder, "<color=#2BD29CFF>Ambusher</color>");
        EnforcerOn = new(num++, MultiMenu.intruder, "<color=#005643FF>Enforcer</color>");

        IntruderSupportRoles = new(MultiMenu.intruder, "<color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Support</color> <color=#FFD700FF>Roles</color>");
        ConsigliereOn = new(num++, MultiMenu.intruder, "<color=#FFFF99FF>Consigliere</color>");
        ConsortOn = new(num++, MultiMenu.intruder, "<color=#801780FF>Consort</color>");
        GodfatherOn = new(num++, MultiMenu.intruder, "<color=#404C08FF>Godfather</color>");
        MinerOn = new(num++, MultiMenu.intruder, "<color=#AA7632FF>Miner</color>");
        TeleporterOn = new(num++, MultiMenu.intruder, "<color=#939593FF>Teleporter</color>");

        IntruderUtilityRoles = new(MultiMenu.intruder, "<color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Utility</color> <color=#FFD700FF>Roles</color>");
        GhoulOn = new(num++, MultiMenu.intruder, "<color=#F1C40FFF>Ghoul</color>");
        ImpostorOn = new(num++, MultiMenu.intruder, "<color=#FF0000FF>Impostor</color>");

        SyndicateDisruptionRoles = new(MultiMenu.syndicate, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Disruption</color> <color=#FFD700FF>Roles</color>");
        ConcealerOn = new(num++, MultiMenu.syndicate, "<color=#C02525FF>Concealer</color>");
        DrunkardOn = new(num++, MultiMenu.syndicate, "<color=#FF7900FF>Drunkard</color>");
        FramerOn = new(num++, MultiMenu.syndicate, "<color=#00FFFFFF>Framer</color>");
        ShapeshifterOn = new(num++, MultiMenu.syndicate, "<color=#2DFF00FF>Shapeshifter</color>");
        SilencerOn = new(num++, MultiMenu.syndicate, "<color=#AAB43EFF>Silencer</color>");

        SyndicateKillingRoles = new(MultiMenu.syndicate, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Killing</color> <color=#FFD700FF>Roles</color>");
        BomberOn = new(num++, MultiMenu.syndicate, "<color=#C9CC3FFF>Bomber</color>");
        ColliderOn = new(num++, MultiMenu.syndicate, "<color=#B345FFFF>Collider</color>");
        CrusaderOn = new(num++, MultiMenu.syndicate, "<color=#DF7AE8FF>Crusader</color>");
        PoisonerOn = new(num++, MultiMenu.syndicate, "<color=#B5004CFF>Poisoner</color>");

        SyndicateSupportRoles = new(MultiMenu.syndicate, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Support</color> <color=#FFD700FF>Roles</color>");
        RebelOn = new(num++, MultiMenu.syndicate, "<color=#FFFCCEFF>Rebel</color>");
        StalkerOn = new(num++, MultiMenu.syndicate, "<color=#7E4D00FF>Stalker</color>");
        WarperOn = new(num++, MultiMenu.syndicate, "<color=#8C7140FF>Warper</color>");

        SyndicatePowerRoles = new(MultiMenu.syndicate, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Power</color> <color=#FFD700FF>Roles</color>");
        SpellslingerOn = new(num++, MultiMenu.syndicate, "<color=#0028F5FF>Spellslinger</color>");
        TimeKeeperOn = new(num++, MultiMenu.syndicate, "<color=#3769FEFF>Time Keeper</color>");

        SyndicateUtilityRoles = new(MultiMenu.syndicate, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Utility</color> <color=#FFD700FF>Roles</color>");
        AnarchistOn = new(num++, MultiMenu.syndicate, "<color=#008000FF>Anarchist</color>");
        BansheeOn = new(num++, MultiMenu.syndicate, "<color=#E67E22FF>Banshee</color>");

        Modifiers = new(MultiMenu.modifier, "<color=#7F7F7FFF>Modifiers</color>");
        AstralOn = new(num++, MultiMenu.modifier, "<color=#612BEFFF>Astral</color>");
        BaitOn = new(num++, MultiMenu.modifier, "<color=#00B3B3FF>Bait</color>");
        CowardOn = new(num++, MultiMenu.modifier, "<color=#456BA8FF>Coward</color>");
        DiseasedOn = new(num++, MultiMenu.modifier, "<color=#374D1EFF>Diseased</color>");
        DrunkOn = new(num++, MultiMenu.modifier, "<color=#758000FF>Drunk</color>");
        DwarfOn = new(num++, MultiMenu.modifier, "<color=#FF8080FF>Dwarf</color>");
        GiantOn = new(num++, MultiMenu.modifier, "<color=#FFB34DFF>Giant</color>");
        IndomitableOn = new(num++, MultiMenu.modifier, "<color=#2DE5BEFF>Indomitable</color>");
        ProfessionalOn = new(num++, MultiMenu.modifier, "<color=#860B7AFF>Professional</color>");
        ShyOn = new(num++, MultiMenu.modifier, "<color=#1002C5FF>Shy</color>");
        VIPOn = new(num++, MultiMenu.modifier, "<color=#DCEE85FF>VIP</color>");
        VolatileOn = new(num++, MultiMenu.modifier, "<color=#FFA60AFF>Volatile</color>");
        YellerOn = new(num++, MultiMenu.modifier, "<color=#F6AAB7FF>Yeller</color>");

        Objectifiers = new(MultiMenu.objectifier, "<color=#DD585BFF>Objectifiers</color>");
        AlliedOn = new(num++, MultiMenu.objectifier, "<color=#4545A9FF>Allied</color>");
        CorruptedOn = new(num++, MultiMenu.objectifier, "<color=#4545FFFF>Corrupted</color>");
        DefectorOn = new(num++, MultiMenu.objectifier, "<color=#E1C849FF>Defector</color>");
        FanaticOn = new(num++, MultiMenu.objectifier, "<color=#678D36FF>Fanatic</color>");
        LinkedOn = new(num++, MultiMenu.objectifier, "<color=#FF351FFF>Linked</color>", 1, 7);
        LoversOn = new(num++, MultiMenu.objectifier, "<color=#FF66CCFF>Lovers</color>", 1, 7);
        MafiaOn = new(num++, MultiMenu.objectifier, "<color=#00EEFFFF>Mafia</color>", 2);
        OverlordOn = new(num++, MultiMenu.objectifier, "<color=#008080FF>Overlord</color>");
        RivalsOn = new(num++, MultiMenu.objectifier, "<color=#3D2D2CFF>Rivals</color>", 1, 7);
        TaskmasterOn = new(num++, MultiMenu.objectifier, "<color=#ABABFFFF>Taskmaster</color>");
        TraitorOn = new(num++, MultiMenu.objectifier, "<color=#370D43FF>Traitor</color>");

        Abilities = new(MultiMenu.ability, "<color=#FF9900FF>Abilities</color>");
        ButtonBarryOn = new(num++, MultiMenu.ability, "<color=#E600FFFF>Button Barry</color>");
        CrewAssassinOn = new(num++, MultiMenu.ability, "<color=#8CFFFFFF>Crew</color> <color=#073763FF>Assassin</color>");
        InsiderOn = new(num++, MultiMenu.ability, "<color=#26FCFBFF>Insider</color>");
        IntruderAssassinOn = new(num++, MultiMenu.ability, "<color=#FF0000FF>Intruder</color> <color=#073763FF>Assassin</color>");
        MultitaskerOn = new(num++, MultiMenu.ability, "<color=#FF804DFF>Multitasker</color>");
        NeutralAssassinOn = new(num++, MultiMenu.ability, "<color=#B3B3B3FF>Neutral</color> <color=#073763FF>Assassin</color>");
        NinjaOn = new(num++, MultiMenu.ability, "<color=#A84300FF>Ninja</color>");
        PoliticianOn = new(num++, MultiMenu.ability, "<color=#CCA3CCFF>Politician</color>");
        RadarOn = new(num++, MultiMenu.ability, "<color=#FF0080FF>Radar</color>");
        RuthlessOn = new(num++, MultiMenu.ability, "<color=#2160DDFF>Ruthless</color>");
        SnitchOn = new(num++, MultiMenu.ability, "<color=#D4AF37FF>Snitch</color>");
        SwapperOn = new(num++, MultiMenu.ability, "<color=#66E666FF>Swapper</color>");
        SyndicateAssassinOn = new(num++, MultiMenu.ability, "<color=#008000FF>Syndicate</color> <color=#073763FF>Assassin</color>");
        TiebreakerOn = new(num++, MultiMenu.ability, "<color=#99E699FF>Tiebreaker</color>");
        TorchOn = new(num++, MultiMenu.ability, "<color=#FFFF99FF>Torch</color>");
        TunnelerOn = new(num++, MultiMenu.ability, "<color=#E91E63FF>Tunneler</color>");
        UnderdogOn = new(num++, MultiMenu.ability, "<color=#841A7FFF>Underdog</color>");

        CrewSettings = new(MultiMenu.crew, "<color=#8CFFFFFF>Crew</color> Settings");
        CommonTasks = new(num++, MultiMenu.crew, "Common Tasks", 2, 0, 100, 1);
        LongTasks = new(num++, MultiMenu.crew, "Long Tasks", 1, 0, 100, 1);
        ShortTasks = new(num++, MultiMenu.crew, "Short Tasks", 4, 0, 100, 1);
        GhostTasksCountToWin = new(num++, MultiMenu.crew, "<color=#8CFFFFFF>Crew</color> Ghost Tasks Count To Win", true);
        CrewVision = new(num++, MultiMenu.crew, "<color=#8CFFFFFF>Crew</color> Vision", 1f, 0.25f, 5f, 0.25f, MultiplierFormat);
        CrewFlashlight = new(num++, MultiMenu.crew, "<color=#8CFFFFFF>Crew</color> Uses A Flashlight", false);
        CrewMax = new(num++, MultiMenu.crew, "Max <color=#8CFFFFFF>Crew</color> <color=#FFD700FF>Roles</color>", 5, 0, 14, 1);
        CrewMin = new(num++, MultiMenu.crew, "Min <color=#8CFFFFFF>Crew</color> <color=#FFD700FF>Roles</color>", 5, 0, 14, 1);
        CrewVent = new(num++, MultiMenu.crew, "<color=#8CFFFFFF>Crew</color> Can Vent", false);

        CrewAuditorSettings = new(MultiMenu.crew, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Auditor</color> Settings", new[] { VampireHunterOn, MysticOn });
        CAMax = new(num++, MultiMenu.crew, "Max <color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Auditor</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1, new[] { VampireHunterOn,
            MysticOn });

        MysticSettings = new(MultiMenu.crew, "<color=#708EEFFF>Mystic</color>", MysticOn);
        UniqueMystic = new(num++, MultiMenu.crew, "<color=#708EEFFF>Mystic</color> Is Unique", false, MysticOn);
        RevealCooldown = new(num++, MultiMenu.crew, "Reveal Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, MysticOn);

        VampireHunter = new(MultiMenu.crew, "<color=#C0C0C0FF>Vampire Hunter</color>", new[] { VampireHunterOn, DraculaOn }, true);
        UniqueVampireHunter = new(num++, MultiMenu.crew, "<color=#C0C0C0FF>Vampire Hunter</color> Is Unique", false, new[] { VampireHunterOn, DraculaOn }, true);
        StakeCooldown = new(num++, MultiMenu.crew, "Stake Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, new[] { VampireHunterOn, DraculaOn }, true);

        CrewInvestigativeSettings = new(MultiMenu.crew, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Investigative</color> Settings", new[] { CoronerOn, DetectiveOn, InspectorOn,
            MediumOn, SeerOn, SheriffOn, TrackerOn, OperativeOn });
        CIMax = new(num++, MultiMenu.crew, "Max <color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Investigative</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1, new[] { CoronerOn,
            DetectiveOn, InspectorOn, MediumOn, SeerOn, SheriffOn, TrackerOn, OperativeOn });

        Coroner = new(MultiMenu.crew, "<color=#4D99E6FF>Coroner</color>", CoronerOn);
        UniqueCoroner = new(num++, MultiMenu.crew, "<color=#4D99E6FF>Coroner</color> Is Unique", false, CoronerOn);
        CoronerArrowDuration = new(num++, MultiMenu.crew, "Dead Body Arrow Duration", 0.1f, 0f, 2f, 0.05f, CooldownFormat, CoronerOn);
        CoronerReportRole = new(num++, MultiMenu.crew, "<color=#4D99E6FF>Coroner</color> Gets Killer's Role", false, CoronerOn);
        CoronerReportName = new(num++, MultiMenu.crew, "<color=#4D99E6FF>Coroner</color> Gets Killer's Name", false, CoronerOn);
        CoronerKillerNameTime = new(num++, MultiMenu.crew, "<color=#4D99E6FF>Coroner</color> Gets Killer's Name Under", 1f, 0.5f, 15f, 0.5f, CooldownFormat, CoronerOn);
        CompareCooldown = new(num++, MultiMenu.crew, "Compare Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, CoronerOn);
        AutopsyCooldown = new(num++, MultiMenu.crew, "Autopsy Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, CoronerOn);

        Detective = new(MultiMenu.crew, "<color=#4D4DFFFF>Detective</color>", DetectiveOn);
        UniqueDetective = new(num++, MultiMenu.crew, "<color=#4D4DFFFF>Detective</color> Is Unique", false, DetectiveOn);
        ExamineCooldown = new(num++, MultiMenu.crew, "Examine Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, DetectiveOn);
        RecentKill = new(num++, MultiMenu.crew, "Bloody Player Duration", 10f, 5f, 60f, 2.5f, CooldownFormat, DetectiveOn);
        FootprintInterval = new(num++, MultiMenu.crew, "Footprint Interval", 0.15f, 0.05f, 2f, 0.05f, CooldownFormat, DetectiveOn);
        FootprintDuration = new(num++, MultiMenu.crew, "Footprint Duration", 10f, 1f, 10f, 0.5f, CooldownFormat, DetectiveOn);
        AnonymousFootPrint = new(num++, MultiMenu.crew, "Anonymous Footprint", false, DetectiveOn);

        Inspector = new(MultiMenu.crew, "<color=#7E3C64FF>Inspector</color>", InspectorOn);
        UniqueInspector = new(num++, MultiMenu.crew, "<color=#7E3C64FF>Inspector</color> Is Unique", false, InspectorOn);
        InspectCooldown = new(num++, MultiMenu.crew, "Inspect Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, InspectorOn);

        Medium = new(MultiMenu.crew, "<color=#A680FFFF>Medium</color>", MediumOn);
        UniqueMedium = new(num++, MultiMenu.crew, "<color=#A680FFFF>Medium</color> Is Unique", false, MediumOn);
        MediateCooldown = new(num++, MultiMenu.crew, "Mediate Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, MediumOn);
        ShowMediatePlayer = new(num++, MultiMenu.crew, "Reveal Appearance Of Mediate Target", true, MediumOn);
        ShowMediumToDead = new(num++, MultiMenu.crew, "Reveal The <color=#A680FFFF>Medium</color> To The Mediate Target", new[] { "No", "Target", "All Dead" }, MediumOn);
        DeadRevealed = new(num++, MultiMenu.crew, "Who Is Revealed With Mediate", new[] { "Oldest Dead", "Newest Dead", "All Dead", "Random" }, MediumOn);

        Seer = new(MultiMenu.crew, "<color=#71368AFF>Seer</color>", SeerOn);
        UniqueSeer = new(num++, MultiMenu.crew, "<color=#71368AFF>Seer</color> Is Unique", false, SeerOn);
        SeerCooldown = new(num++, MultiMenu.crew, "Seer Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, SeerOn);

        Operative = new(MultiMenu.crew, "<color=#A7D1B3FF>Operative</color>", OperativeOn);
        UniqueOperative = new(num++, MultiMenu.crew, "<color=#A7D1B3FF>Operative</color> Is Unique", false, OperativeOn);
        BugCooldown = new(num++, MultiMenu.crew, "Bug Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, OperativeOn);
        MinAmountOfTimeInBug = new(num++, MultiMenu.crew, "Min Amount Of Time In Bug To Trigger", 0f, 0f, 15f, 0.5f, CooldownFormat, OperativeOn);
        BugsRemoveOnNewRound = new(num++, MultiMenu.crew, "Bugs Are Removed Each Round", true, OperativeOn);
        MaxBugs = new(num++, MultiMenu.crew, "Bug Count", 5, 1, 15, 1, OperativeOn);
        BugRange = new(num++, MultiMenu.crew, "Bug Radius", 1.5f, 0.5f, 5f, 0.25f, DistanceFormat, OperativeOn);
        MinAmountOfPlayersInBug = new(num++, MultiMenu.crew, "Number Of <color=#FFD700FF>Roles</color> Required To Trigger Bug", 1, 1, 5, 1, OperativeOn);
        WhoSeesDead = new(num++, MultiMenu.crew, "Who Sees Dead Bodies On Admin", new[] { "Nobody", "Operative", "Everyone But Operative", "Everyone" }, OperativeOn);
        PreciseOperativeInfo = new(num++, MultiMenu.crew, "<color=#A7D1B3FF>Operative</color> Gets Precise Information", false, OperativeOn);

        Sheriff = new(MultiMenu.crew, "<color=#FFCC80FF>Sheriff</color>", SheriffOn);
        UniqueSheriff = new(num++, MultiMenu.crew, "<color=#FFCC80FF>Sheriff</color> Is Unique", false, SheriffOn);
        InterrogateCooldown = new(num++, MultiMenu.crew, "Interrogate Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, SheriffOn);
        NeutEvilRed = new(num++, MultiMenu.crew, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Evils</color> Show Evil", false, SheriffOn);
        NeutKillingRed = new(num++, MultiMenu.crew, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killings</color> Show Evil", false, SheriffOn);

        Tracker = new(MultiMenu.crew, "<color=#009900FF>Tracker</color>", TrackerOn);
        UniqueTracker = new(num++, MultiMenu.crew, "<color=#009900FF>Tracker</color> Is Unique", false, TrackerOn);
        UpdateInterval = new(num++, MultiMenu.crew, "Arrow Update Interval", 5f, 0f, 15f, 0.5f, CooldownFormat, TrackerOn);
        TrackCooldown = new(num++, MultiMenu.crew, "Track Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, TrackerOn);
        ResetOnNewRound = new(num++, MultiMenu.crew, "<color=#009900FF>Tracker</color> Arrows Reset After Each Round", false, TrackerOn);
        MaxTracks = new(num++, MultiMenu.crew, "Max Tracks", 5, 1, 15, 1, TrackerOn);

        CrewKillingSettings = new(MultiMenu.crew, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Killing</color> Settings", new[] { VigilanteOn, VeteranOn });
        CKMax = new(num++, MultiMenu.crew, "Max <color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Killing</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1, new[] { VigilanteOn, VeteranOn
            });

        Veteran = new(MultiMenu.crew, "<color=#998040FF>Veteran</color>", VeteranOn);
        UniqueVeteran = new(num++, MultiMenu.crew, "<color=#998040FF>Veteran</color> Is Unique", false, VeteranOn);
        AlertCooldown = new(num++, MultiMenu.crew, "Alert Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, VeteranOn);
        AlertDuration = new(num++, MultiMenu.crew, "Alert Duration", 10f, 5f, 30f, 1f, CooldownFormat, VeteranOn);
        MaxAlerts = new(num++, MultiMenu.crew, "Max Alerts", 5, 1, 15, 1, VeteranOn);

        Vigilante = new(MultiMenu.crew, "<color=#FFFF00FF>Vigilante</color>", VigilanteOn);
        UniqueVigilante = new(num++, MultiMenu.crew, "<color=#FFFF00FF>Vigilante</color> Is Unique", false, VigilanteOn);
        MisfireKillsInno = new(num++, MultiMenu.crew, "Misfire Kills The Target", true, VigilanteOn);
        VigiKillAgain = new(num++, MultiMenu.crew, "<color=#FFFF00FF>Vigilante</color> Can Kill Again If Target Was Innocent", true, VigilanteOn);
        RoundOneNoShot = new(num++, MultiMenu.crew, "<color=#FFFF00FF>Vigilante</color> Cannot Shoot On The First Round", true, VigilanteOn);
        VigiNotifOptions = new(num++, MultiMenu.crew, "How Is The <color=#FFFF00FF>Vigilante</color> Notified Of Their Target's Innocence", new[] { "Never", "Flash", "Message" },
            VigilanteOn);
        VigiOptions = new(num++, MultiMenu.crew, "How Does <color=#FFFF00FF>Vigilante</color> Die", new[] { "Immediately", "Before Meeting", "After Meeting" }, VigilanteOn);
        VigiKillCd = new(num++, MultiMenu.crew, "Shoot Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, VigilanteOn);
        VigiBulletCount = new(num++, MultiMenu.crew, "Max Bullets", 5, 1, 15, 1, VigilanteOn);

        CrewProtectiveSettings = new(MultiMenu.crew, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Protective</color> Settings", new[] { AltruistOn, MedicOn });
        CPMax = new(num++, MultiMenu.crew, "Max <color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Protective</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1, new[] { AltruistOn, MedicOn
            });

        Altruist = new(MultiMenu.crew, "<color=#660000FF>Altruist</color>", AltruistOn);
        UniqueAltruist = new(num++, MultiMenu.crew, "<color=#660000FF>Altruist</color> Is Unique", false, AltruistOn);
        ReviveCount = new(num++, MultiMenu.crew, "Revive Count", 5, 1, 14, 1, AltruistOn);
        ReviveCooldown = new(num++, MultiMenu.crew, "Revive Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, AltruistOn);
        AltReviveDuration = new(num++, MultiMenu.crew, "<color=#660000FF>Altruist</color> Revive Duration", 10f, 1f, 15f, 1f, CooldownFormat, AltruistOn);
        AltruistTargetBody = new(num++, MultiMenu.crew, "Target's Body Disappears On Beginning Of Revive", false, AltruistOn);

        Medic = new(MultiMenu.crew, "<color=#006600FF>Medic</color>", MedicOn);
        UniqueMedic = new(num++, MultiMenu.crew, "<color=#006600FF>Medic</color> Is Unique", false, MedicOn);
        ShowShielded = new(num++, MultiMenu.crew, "Show Shielded Player", new[] { "Self", "Medic", "Self And Medic", "Everyone" }, MedicOn);
        WhoGetsNotification = new(num++, MultiMenu.crew, "Who Gets Murder Attempt Indicator", new[] { "Medic", "Self", "Self And Medic", "Everyone", "Nobody" }, MedicOn);
        ShieldBreaks = new(num++, MultiMenu.crew, "Shield Breaks On Murder Attempt", true, MedicOn);

        CrewSovereignSettings = new(MultiMenu.crew, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Sovereign</color> Settings", new[] { MayorOn, DictatorOn, MonarchOn });
        CSvMax = new(num++, MultiMenu.crew, "Max <color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Sovereign</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1, new[] { MayorOn, DictatorOn,
            MonarchOn });

        Dictator = new(MultiMenu.crew, "<color=#00CB97FF>Dictator</color>", DictatorOn);
        UniqueDictator = new(num++, MultiMenu.crew, "<color=#00CB97FF>Dictator</color> Is Unique", false, DictatorOn);
        RoundOneNoDictReveal = new(num++, MultiMenu.crew, "<color=#00CB97FF>Dictator</color> Cannot Reveal Round One", false, DictatorOn);
        DictateAfterVoting = new(num++, MultiMenu.crew, "<color=#00CB97FF>Dictator</color> Can Dictate After Voting", false, DictatorOn);
        DictatorButton = new(num++, MultiMenu.crew, "<color=#00CB97FF>Dictator</color> Can Button", true, DictatorOn);

        Mayor = new(MultiMenu.crew, "<color=#704FA8FF>Mayor</color>", MayorOn);
        UniqueMayor = new(num++, MultiMenu.crew, "<color=#704FA8FF>Mayor</color> Is Unique", false, MayorOn);
        MayorVoteCount = new(num++, MultiMenu.crew, "Revealed <color=#704FA8FF>Mayor</color> Votes Count As", 2, 1, 10, 1, MayorOn);
        RoundOneNoReveal = new(num++, MultiMenu.crew, "<color=#704FA8FF>Mayor</color> Cannot Reveal Round One", false, MayorOn);
        MayorButton = new(num++, MultiMenu.crew, "<color=#704FA8FF>Mayor</color> Can Button", true, MayorOn);

        Monarch = new(MultiMenu.crew, "<color=#FF004EFF>Monarch</color>", MonarchOn);
        UniqueMonarch = new(num++, MultiMenu.crew, "<color=#FF004EFF>Monarch</color> Is Unique", false, MonarchOn);
        KnightingCooldown = new(num++, MultiMenu.crew, "Knighting Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, MonarchOn);
        RoundOneNoKnighting = new(num++, MultiMenu.crew, "<color=#FF004EFF>Monarch</color> Cannot Knight Round One", false, MonarchOn);
        KnightCount = new(num++, MultiMenu.crew, "Knight Count", 2, 1, 14, 1, MonarchOn);
        KnightVoteCount = new(num++, MultiMenu.crew, "Knighted Votes Count As", 1, 1, 10, 1, MonarchOn);
        MonarchButton = new(num++, MultiMenu.crew, "<color=#FF004EFF>Monarch</color> Can Button", true, MonarchOn);
        KnightButton = new(num++, MultiMenu.crew, "Knights Can Button", true, MonarchOn);

        CrewSupportSettings = new(MultiMenu.crew, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Support</color> Settings", new[] { ChameleonOn, EngineerOn, EscortOn, RetributionistOn,
            ShifterOn, TransporterOn });
        CSMax = new(num++, MultiMenu.crew, "Max <color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Support</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1, new[] { ChameleonOn, ShifterOn,
            EngineerOn, EscortOn, RetributionistOn, TransporterOn });

        Chameleon = new(MultiMenu.crew, "<color=#5411F8FF>Chameleon</color>", ChameleonOn);
        UniqueChameleon = new(num++, MultiMenu.crew, "<color=#5411F8FF>Chameleon</color> Is Unique", false, ChameleonOn);
        SwoopCooldown = new(num++, MultiMenu.crew, "Swoop Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, ChameleonOn);
        SwoopDuration = new(num++, MultiMenu.crew, "Swoop Duration", 10f, 5f, 30f, 1f, CooldownFormat, ChameleonOn);
        SwoopCount = new(num++, MultiMenu.crew, "Max Swoops", 5, 1, 15, 1, ChameleonOn);

        Engineer = new(MultiMenu.crew, "<color=#FFA60AFF>Engineer</color>", EngineerOn);
        UniqueEngineer = new(num++, MultiMenu.crew, "<color=#FFA60AFF>Engineer</color> Is Unique", false, EngineerOn);
        FixCooldown = new(num++, MultiMenu.crew, "Fix Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, EngineerOn);
        MaxFixes = new(num++, MultiMenu.crew, "Max Fixes", 5, 1, 15, 1, EngineerOn);

        Escort = new(MultiMenu.crew, "<color=#803333FF>Escort</color>", EscortOn);
        UniqueEscort = new(num++, MultiMenu.crew, "<color=#803333FF>Escort</color> Is Unique", false, EscortOn);
        EscRoleblockCooldown = new(num++, MultiMenu.crew, "Roleblock Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, EscortOn);
        EscRoleblockDuration = new(num++, MultiMenu.crew, "Roleblock Duration", 10f, 5f, 30f, 1f, CooldownFormat, EscortOn);

        Retributionist = new(MultiMenu.crew, "<color=#8D0F8CFF>Retributionist</color>", RetributionistOn);
        UniqueRetributionist = new(num++, MultiMenu.crew, "<color=#8D0F8CFF>Retributionist</color> Is Unique", false, RetributionistOn);
        ReviveAfterVoting = new(num++, MultiMenu.crew, "<color=#8D0F8CFF>Retributionist</color> Can Mimic After Voting", true, RetributionistOn);
        MaxUses = new(num++, MultiMenu.crew, "Max Uses", 5, 1, 15, 1, RetributionistOn);

        Shifter = new(MultiMenu.crew, "<color=#DF851FFF>Shifter</color>", ShifterOn);
        UniqueShifter = new(num++, MultiMenu.crew, "<color=#DF851FFF>Shifter</color> Is Unique", false, ShifterOn);
        ShifterCd = new(num++, MultiMenu.crew, "Shift Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, ShifterOn);
        ShiftedBecomes = new(num++, MultiMenu.crew, "Shifted Becomes", new[] { "Shifter", "Crewmate" }, ShifterOn);

        Transporter = new(MultiMenu.crew, "<color=#00EEFFFF>Transporter</color>", TransporterOn);
        UniqueTransporter = new(num++, MultiMenu.crew, "<color=#00EEFFFF>Transporter</color> Is Unique", false, TransporterOn);
        TransportCooldown = new(num++, MultiMenu.crew, "Transport Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, TransporterOn);
        TransportDuration = new(num++, MultiMenu.crew, "Transport Duration", 5f, 1f, 20f, 1f, CooldownFormat, TransporterOn);
        TransportMaxUses = new(num++, MultiMenu.crew, "Max Transports", 5, 1, 15, 1, TransporterOn);
        TransSelf = new(num++, MultiMenu.crew, "<color=#00EEFFFF>Transporter</color> Can Transport Themselves", true, TransporterOn);

        CrewUtilitySettings = new(MultiMenu.crew, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Utility</color> Settings", RevealerOn);

        Revealer = new(MultiMenu.crew, "<color=#D3D3D3FF>Revealer</color>", RevealerOn);
        RevealerTasksRemainingClicked = new(num++, MultiMenu.crew, "Tasks Remaining When <color=#D3D3D3FF>Revealer</color> Can Be Clicked", 5, 1, 10, 1, RevealerOn);
        RevealerTasksRemainingAlert = new(num++, MultiMenu.crew, "Tasks Remaining When Revealed", 1, 1, 5, 1, RevealerOn);
        RevealerRevealsNeutrals = new(num++, MultiMenu.crew, "<color=#D3D3D3FF>Revealer</color> Reveals <color=#B3B3B3FF>Neutrals</color>", false, RevealerOn);
        RevealerRevealsCrew = new(num++, MultiMenu.crew, "<color=#D3D3D3FF>Revealer</color> Reveals <color=#8CFFFFFF>Crew</color>", false, RevealerOn);
        RevealerRevealsRoles = new(num++, MultiMenu.crew, "<color=#D3D3D3FF>Revealer</color> Reveals Exact <color=#FFD700FF>Roles</color>", false, RevealerOn);
        RevealerCanBeClickedBy = new(num++, MultiMenu.crew, "Who Can Click <color=#D3D3D3FF>Revealer</color>", new[] { "All", "Non Crew", "Evils Only" }, RevealerOn);

        NeutralSettings = new(MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> Settings", new[] { AmnesiacOn, GuardianAngelOn, SurvivorOn, ThiefOn, PlaguebearerOn, ActorOn, JackalOn,
            BountyHunterOn, CannibalOn, ExecutionerOn, GuesserOn, JesterOn, TrollOn, ArsonistOn, CryomaniacOn, GlitchOn, JuggernautOn, MurdererOn, SerialKillerOn, WerewolfOn, DraculaOn,
            WhispererOn, NecromancerOn });
        NeutralVision = new(num++, MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> Vision", 1.5f, 0.25f, 5f, 0.25f, MultiplierFormat, new[] { AmnesiacOn, GuardianAngelOn, SurvivorOn,
            ThiefOn, PlaguebearerOn, ActorOn, BountyHunterOn, CannibalOn, ExecutionerOn, GuesserOn, JesterOn, TrollOn, ArsonistOn, CryomaniacOn, GlitchOn, JuggernautOn, MurdererOn,
            SerialKillerOn, WerewolfOn, DraculaOn, WhispererOn, JackalOn, NecromancerOn });
        LightsAffectNeutrals = new(num++, MultiMenu.neutral, "Lights Sabotage Affects <color=#B3B3B3FF>Neutral</color> Vision", true, new[] { AmnesiacOn, GuardianAngelOn, SurvivorOn,
            ThiefOn, PlaguebearerOn, ActorOn, BountyHunterOn, CannibalOn, ExecutionerOn, GuesserOn, JesterOn, TrollOn, ArsonistOn, CryomaniacOn, GlitchOn, JuggernautOn, MurdererOn,
            SerialKillerOn, WerewolfOn, DraculaOn, WhispererOn, JackalOn, NecromancerOn });
        NeutralFlashlight = new(num++, MultiMenu.neutral, "<color=#B3B3B3FF>Neutrals</color> Use A Flashlight", false, new[] { AmnesiacOn, GuardianAngelOn, SurvivorOn, ThiefOn, JackalOn,
            PlaguebearerOn, ActorOn, BountyHunterOn, CannibalOn, ExecutionerOn, GuesserOn, JesterOn, TrollOn, ArsonistOn, CryomaniacOn, GlitchOn, JuggernautOn, MurdererOn, DraculaOn,
            SerialKillerOn, WerewolfOn, WhispererOn, NecromancerOn });
        NeutralMax = new(num++, MultiMenu.neutral, "Max <color=#B3B3B3FF>Neutral</color> <color=#FFD700FF>Roles</color>", 5, 1, 14, 1, new[] { AmnesiacOn, GuardianAngelOn, SurvivorOn,
            ThiefOn, PlaguebearerOn, ActorOn, BountyHunterOn, CannibalOn, ExecutionerOn, GuesserOn, JesterOn, TrollOn, ArsonistOn, CryomaniacOn, GlitchOn, JuggernautOn, MurdererOn,
            SerialKillerOn, WerewolfOn, DraculaOn, WhispererOn, JackalOn, NecromancerOn });
        NeutralMin = new(num++, MultiMenu.neutral, "Min <color=#B3B3B3FF>Neutral</color> <color=#FFD700FF>Roles</color>", 5, 1, 14, 1, new[] { AmnesiacOn, GuardianAngelOn, SurvivorOn,
            ThiefOn, PlaguebearerOn, ActorOn, BountyHunterOn, CannibalOn, ExecutionerOn, GuesserOn, JesterOn, TrollOn, ArsonistOn, CryomaniacOn, GlitchOn, JuggernautOn, MurdererOn,
            SerialKillerOn, WerewolfOn, DraculaOn, WhispererOn, JackalOn, NecromancerOn });
        NoSolo = new(num++, MultiMenu.neutral, "<color=#B3B3B3FF>Neutrals</color> Together, Strong", new[] { "Never", "Same NKs", "Same Roles", "All NKs", "All Neutrals" }, new[] {
            AmnesiacOn, GuardianAngelOn, SurvivorOn, ThiefOn, PlaguebearerOn, ActorOn, BountyHunterOn, CannibalOn, ExecutionerOn, GuesserOn, JesterOn, TrollOn, ArsonistOn,
            CryomaniacOn, GlitchOn, JuggernautOn, MurdererOn, SerialKillerOn, WerewolfOn, DraculaOn, WhispererOn, JackalOn, NecromancerOn });
        NeutralsVent = new(num++, MultiMenu.neutral, "<color=#B3B3B3FF>Neutrals</color> Can Vent", true, new[] { AmnesiacOn, GuardianAngelOn, SurvivorOn, ThiefOn, PlaguebearerOn,
            ActorOn, BountyHunterOn, CannibalOn, ExecutionerOn, GuesserOn, JesterOn, TrollOn, ArsonistOn, CryomaniacOn, GlitchOn, JuggernautOn, MurdererOn, SerialKillerOn, WerewolfOn,
            DraculaOn, WhispererOn, JackalOn, NecromancerOn });

        NeutralApocalypseSettings = new(MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Apocalypse</color> Settings", PlaguebearerOn);

        Pestilence = new(MultiMenu.neutral, "<color=#424242FF>Pestilence</color>", PlaguebearerOn);
        PestSpawn = new(num++, MultiMenu.neutral, "<color=#424242FF>Pestilence</color> Can Spawn Directly", false, PlaguebearerOn);
        PlayersAlerted = new(num++, MultiMenu.neutral, "<color=#424242FF>Pestilence</color> Transformation Alerts Everyone", true, PlaguebearerOn);
        PestKillCooldown = new(num++, MultiMenu.neutral, "Obliterate Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, PlaguebearerOn);
        PestVent = new(num++, MultiMenu.neutral, "<color=#424242FF>Pestilence</color> Can Vent", true, PlaguebearerOn);

        NeutralBenignSettings = new(MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Benign</color> Settings", new[] { AmnesiacOn, GuardianAngelOn, SurvivorOn,
            ThiefOn, VigilanteOn });
        NBMax = new(num++, MultiMenu.neutral, "Max <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Benign</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1, new[] { AmnesiacOn,
            GuardianAngelOn, SurvivorOn, ThiefOn });
        VigiKillsNB = new(num++, MultiMenu.neutral, "<color=#FFFF00FF>Vigilante</color> Kills <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Benigns</color>", true, new[] { ThiefOn,
            AmnesiacOn, GuardianAngelOn, SurvivorOn, VigilanteOn });
        AvoidNeutralBenignKingmakers = new(num++, MultiMenu.neutral, "Avoid <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Benign</color> Kingmakers", false, new[] { GuardianAngelOn, SurvivorOn });

        Amnesiac = new(MultiMenu.neutral, "<color=#22FFFFFF>Amnesiac</color>", AmnesiacOn);
        UniqueAmnesiac = new(num++, MultiMenu.neutral, "<color=#22FFFFFF>Amnesiac</color> Is Unique", false, AmnesiacOn);
        RememberArrows = new(num++, MultiMenu.neutral, "<color=#22FFFFFF>Amnesiac</color> Gets Arrows To Dead Bodies", false, AmnesiacOn);
        RememberArrowDelay = new(num++, MultiMenu.neutral, "Arrow Appearance Delay", 5f, 0f, 15f, 1f, CooldownFormat, AmnesiacOn);
        AmneVent = new(num++, MultiMenu.neutral, "<color=#22FFFFFF>Amnesiac</color> Can Hide In Vents", false, AmnesiacOn);
        AmneSwitchVent = new(num++, MultiMenu.neutral, "<color=#22FFFFFF>Amnesiac</color> Can Switch Vents", false, AmnesiacOn);

        GuardianAngel = new(MultiMenu.neutral, "<color=#FFFFFFFF>Guardian Angel</color>", GuardianAngelOn);
        UniqueGuardianAngel = new(num++, MultiMenu.neutral, "<color=#FFFFFFFF>Guardian Angel</color> Is Unique", false, GuardianAngelOn);
        GuardianAngelCanPickTargets = new(num++, MultiMenu.neutral, "<color=#FFFFFFFF>Guardian Angel</color> Can Pick Their Own Target", false, GuardianAngelOn);
        ProtectCd = new(num++, MultiMenu.neutral, "Protect Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, GuardianAngelOn);
        ProtectDuration = new(num++, MultiMenu.neutral, "Protect Duration", 10f, 5f, 30f, 1f, CooldownFormat, GuardianAngelOn);
        ProtectKCReset = new(num++, MultiMenu.neutral, "Cooldown Reset When Protected", 2.5f, 0f, 15f, 0.5f, CooldownFormat, GuardianAngelOn);
        MaxProtects = new(num++, MultiMenu.neutral, "Max Protects", 5, 1, 15, 1, GuardianAngelOn);
        ShowProtect = new(num++, MultiMenu.neutral, "Show Protected Player", new[] { "Self", "Guardian Angel", "Self And GA", "Everyone" }, GuardianAngelOn);
        GATargetKnows = new(num++, MultiMenu.neutral, "Target Knows <color=#FFFFFFFF>Guardian Angel</color> Exists", false, GuardianAngelOn);
        ProtectBeyondTheGrave = new(num++, MultiMenu.neutral, "<color=#FFFFFFFF>Guardian Angel</color> Can Protect After Death", false, GuardianAngelOn);
        GAKnowsTargetRole = new(num++, MultiMenu.neutral, "<color=#FFFFFFFF>Guardian Angel</color> Knows Target's <color=#FFD700FF>Role</color>", false, GuardianAngelOn);
        GAVent = new(num++, MultiMenu.neutral, "<color=#FFFFFFFF>Guardian Angel</color> Can Hide In Vents", false, GuardianAngelOn);
        GASwitchVent = new(num++, MultiMenu.neutral, "<color=#FFFFFFFF>Guardian Angel</color> Can Switch Vents", false, GuardianAngelOn);

        Survivor = new(MultiMenu.neutral, "<color=#DDDD00FF>Survivor</color>", SurvivorOn);
        UniqueSurvivor = new(num++, MultiMenu.neutral, "<color=#DDDD00FF>Survivor</color> Is Unique", false, SurvivorOn);
        VestCd = new(num++, MultiMenu.neutral, "Vest Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, SurvivorOn);
        VestDuration = new(num++, MultiMenu.neutral, "Vest Duration", 10f, 5f, 30f, 1f, CooldownFormat, SurvivorOn);
        VestKCReset = new(num++, MultiMenu.neutral, "Cooldown Reset When Vested", 2.5f, 0f, 15f, 0.5f, CooldownFormat, SurvivorOn);
        MaxVests = new(num++, MultiMenu.neutral, "Max Vests", 5, 1, 15, 1, SurvivorOn);
        SurvVent = new(num++, MultiMenu.neutral, "<color=#DDDD00FF>Survivor</color> Can Hide In Vents", false, SurvivorOn);
        SurvSwitchVent = new(num++, MultiMenu.neutral, "<color=#DDDD00FF>Survivor</color> Can Switch Vents", false, SurvivorOn);

        Thief = new(MultiMenu.neutral, "<color=#80FF00FF>Thief</color>", ThiefOn);
        UniqueThief = new(num++, MultiMenu.neutral, "<color=#80FF00FF>Thief</color> Is Unique", false, ThiefOn);
        ThiefKillCooldown = new(num++, MultiMenu.neutral, "Steal Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, ThiefOn);
        ThiefSteals = new(num++, MultiMenu.neutral, "<color=#80FF00FF>Thief</color> Assigns <color=#80FF00FF>Thief</color> <color=#FFD700FF>Role</color> To Target", false, ThiefOn);
        ThiefCanGuess = new(num++, MultiMenu.neutral, "<color=#80FF00FF>Thief</color> Can Guess To Steal Roles", false, ThiefOn);
        ThiefCanGuessAfterVoting = new(num++, MultiMenu.neutral, "<color=#80FF00FF>Thief</color> Can Guess After Voting", false, ThiefOn);
        ThiefVent = new(num++, MultiMenu.neutral, "<color=#80FF00FF>Thief</color> Can Vent", false, ThiefOn);

        NeutralEvilSettings = new(MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Evil</color> Settings", new[] { ActorOn, BountyHunterOn, CannibalOn,
            ExecutionerOn, GuesserOn, JesterOn, TrollOn });
        NEMax = new(num++, MultiMenu.neutral, "Max <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Evil</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1, new[] { ActorOn, JesterOn,
            BountyHunterOn, CannibalOn, ExecutionerOn, GuesserOn, TrollOn });
        NeutralEvilsEndGame = new(num++, MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Evils</color> End The Game When Winning", false, new[] { ActorOn, JesterOn,
            BountyHunterOn, CannibalOn, ExecutionerOn, GuesserOn, TrollOn });
        AvoidNeutralEvilKingmakers = new(num++, MultiMenu.neutral, "Avoid <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Evil</color> Kingmakers", false, new[] { ActorOn, BountyHunterOn, CannibalOn, ExecutionerOn,
         GuesserOn, JesterOn, TrollOn });

        Actor = new(MultiMenu.neutral, "<color=#00ACC2FF>Actor</color>", ActorOn);
        UniqueActor= new(num++, MultiMenu.neutral, "<color=#00ACC2FF>Actor</color> Is Unique", false, ActorOn);
        ActorCanPickRole = new(num++, MultiMenu.neutral, "<color=#00ACC2FF>Actor</color> Can Choose A Target Role", false, ActorOn);
        ActorButton = new(num++, MultiMenu.neutral, "<color=#00ACC2FF>Actor</color> Can Button", true, ActorOn);
        ActorVent = new(num++, MultiMenu.neutral, "<color=#00ACC2FF>Actor</color> Can Hide In Vents", false, ActorOn);
        ActSwitchVent = new(num++, MultiMenu.neutral, "<color=#00ACC2FF>Actor</color> Can Switch Vents", false, ActorOn);
        VigiKillsActor = new(num++, MultiMenu.neutral, "<color=#FFFF00FF>Vigilante</color> Kills <color=#00ACC2FF>Actor</color>", false, new[] { ActorOn, VigilanteOn }, true);

        BountyHunter = new(MultiMenu.neutral, "<color=#B51E39FF>Bounty Hunter</color>", BountyHunterOn);
        UniqueBountyHunter = new(num++, MultiMenu.neutral, "<color=#B51E39FF>Bounty Hunter</color> Is Unique", false, BountyHunterOn);
        BountyHunterCanPickTargets = new(num++, MultiMenu.neutral, "<color=#B51E39FF>Bounty Hunter</color> Can Indirectly Pick Their Own Target", false, BountyHunterOn);
        BountyHunterCooldown = new(num++, MultiMenu.neutral, "Guess Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, BountyHunterOn);
        BountyHunterGuesses = new(num++, MultiMenu.neutral, "Max Guesses", 5, 1, 15, 1, BountyHunterOn);
        BHVent = new(num++, MultiMenu.neutral, "<color=#B51E39FF>Bounty Hunter</color> Can Vent", false, BountyHunterOn);
        VigiKillsBH = new(num++, MultiMenu.neutral, "<color=#FFFF00FF>Vigilante</color> Kills <color=#B51E39FF>Bounty Hunter</color>", false, new[] { ActorOn, BountyHunterOn }, true);

        Cannibal = new(MultiMenu.neutral, "<color=#8C4005FF>Cannibal</color>", CannibalOn);
        UniqueCannibal = new(num++, MultiMenu.neutral, "<color=#8C4005FF>Cannibal</color> Is Unique", false, CannibalOn);
        CannibalCd = new(num++, MultiMenu.neutral, "Eat Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, CannibalOn);
        CannibalBodyCount = new(num++, MultiMenu.neutral, "Bodies Needed To Win", 1, 1, 5, 1, CannibalOn);
        EatArrows = new(num++, MultiMenu.neutral, "<color=#8C4005FF>Cannibal</color> Gets Arrows To Dead Bodies", false, CannibalOn);
        EatArrowDelay = new(num++, MultiMenu.neutral, "Arrow Appearance Delay", 5f, 0f, 15f, 1f, CooldownFormat, CannibalOn);
        CannibalVent = new(num++, MultiMenu.neutral, "<color=#8C4005FF>Cannibal</color> Can Vent", false, CannibalOn);
        VigiKillsCannibal = new(num++, MultiMenu.neutral, "<color=#FFFF00FF>Vigilante</color> Kills <color=#8C4005FF>Cannibal</color>", false, new[] { CannibalOn, VigilanteOn }, true);

        Executioner = new(MultiMenu.neutral, "<color=#CCCCCCFF>Executioner</color>", ExecutionerOn);
        UniqueExecutioner = new(num++, MultiMenu.neutral, "<color=#CCCCCCFF>Executioner</color> Is Unique", false, ExecutionerOn);
        ExecutionerCanPickTargets = new(num++, MultiMenu.neutral, "<color=#CCCCCCFF>Executioner</color> Can Pick Their Own Target", false, ExecutionerOn);
        DoomCooldown = new(num++, MultiMenu.neutral, "Doom Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, ExecutionerOn);
        DoomCount = new(num++, MultiMenu.neutral, "Doom Count", 5, 1, 14, 1, ExecutionerOn);
        ExecutionerButton = new(num++, MultiMenu.neutral, "<color=#CCCCCCFF>Executioner</color> Can Button", true, ExecutionerOn);
        ExeVent = new(num++, MultiMenu.neutral, "<color=#CCCCCCFF>Executioner</color> Can Hide In Vents", false, ExecutionerOn);
        ExeSwitchVent = new(num++, MultiMenu.neutral, "<color=#CCCCCCFF>Executioner</color> Can Switch Vents", false, ExecutionerOn);
        ExeTargetKnows = new(num++, MultiMenu.neutral, "Target Knows <color=#CCCCCCFF>Executioner</color> Exists", false, ExecutionerOn);
        ExeKnowsTargetRole = new(num++, MultiMenu.neutral, "<color=#CCCCCCFF>Executioner</color> Knows Target's <color=#FFD700FF>Role</color>", false, ExecutionerOn);
        ExeEjectScreen = new(num++, MultiMenu.neutral, "Target Ejection Reveals Existence Of <color=#CCCCCCFF>Executioner</color>", false, ExecutionerOn);
        ExeCanWinBeyondDeath = new(num++, MultiMenu.neutral, "<color=#CCCCCCFF>Executioner</color> Can Win After Death", false, ExecutionerOn);
        VigiKillsExecutioner = new(num++, MultiMenu.neutral, "<color=#FFFF00FF>Vigilante</color> Kills <color=#CCCCCCFF>Executioner</color>", false, new[] { ExecutionerOn, VigilanteOn },
            true);

        Guesser = new(MultiMenu.neutral, "<color=#EEE5BEFF>Guesser</color>", GuesserOn);
        UniqueGuesser = new(num++, MultiMenu.neutral, "<color=#EEE5BEFF>Guesser</color> Is Unique", false, GuesserOn);
        GuesserCanPickTargets = new(num++, MultiMenu.neutral, "<color=#EEE5BEFF>Guesser</color> Can Pick Their Own Target", false, GuesserOn);
        GuesserButton = new(num++, MultiMenu.neutral, "<color=#EEE5BEFF>Guesser</color> Can Button", true, GuesserOn);
        GuessVent = new(num++, MultiMenu.neutral, "<color=#EEE5BEFF>Guesser</color> Can Hide In Vents", false, GuesserOn);
        GuessSwitchVent = new(num++, MultiMenu.neutral, "<color=#EEE5BEFF>Guesser</color> Can Switch Vents", false, GuesserOn);
        GuessTargetKnows = new(num++, MultiMenu.neutral, "Target Knows <color=#EEE5BEFF>Guesser</color> Exists", false, GuesserOn);
        MultipleGuesses = new(num++, MultiMenu.neutral, "<color=#EEE5BEFF>Guesser</color> Can Guess Multiple Times", true, GuesserOn);
        GuessCount = new(num++, MultiMenu.neutral, "Max Guesses", 5, 1, 15, 1, GuesserOn);
        GuesserAfterVoting = new(num++, MultiMenu.neutral, "<color=#EEE5BEFF>Guesser</color> Can Guess After Voting", false, GuesserOn);
        VigiKillsGuesser = new(num++, MultiMenu.neutral, "<color=#FFFF00FF>Vigilante</color> Kills <color=#EEE5BEFF>Guesser</color>", false, new[] { GuesserOn, VigilanteOn }, true);

        Jester = new(MultiMenu.neutral, "<color=#F7B3DAFF>Jester</color>", JesterOn);
        UniqueJester = new(num++, MultiMenu.neutral, "<color=#F7B3DAFF>Jester</color> Is Unique", false, JesterOn);
        JesterButton = new(num++, MultiMenu.neutral, "<color=#F7B3DAFF>Jester</color> Can Button", true, JesterOn);
        HauntCooldown = new(num++, MultiMenu.neutral, "Haunt Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, JesterOn);
        HauntCount = new(num++, MultiMenu.neutral, "Haunt Count", 5, 1, 14, 1, JesterOn);
        JesterVent = new(num++, MultiMenu.neutral, "<color=#F7B3DAFF>Jester</color> Can Hide In Vents", false, JesterOn);
        JestSwitchVent = new(num++, MultiMenu.neutral, "<color=#F7B3DAFF>Jester</color> Can Switch Vents", false, JesterOn);
        JestEjectScreen = new(num++, MultiMenu.neutral, "<color=#F7B3DAFF>Jester</color> Ejection Reveals Existence Of <color=#F7B3DAFF>Jester</color>", false, JesterOn);
        VigiKillsJester = new(num++, MultiMenu.neutral, "<color=#FFFF00FF>Vigilante</color> Kills <color=#F7B3DAFF>Jester</color>", false, new[] { JesterOn, VigilanteOn }, true);

        Troll = new(MultiMenu.neutral, "<color=#678D36FF>Troll</color>", TrollOn);
        UniqueTroll = new(num++, MultiMenu.neutral, "<color=#678D36FF>Troll</color> Is Unique", false, TrollOn);
        InteractCooldown = new(num++, MultiMenu.neutral, "Interact Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, TrollOn);
        TrollVent = new(num++, MultiMenu.neutral, "<color=#678D36FF>Troll</color> Can Hide In Vent", false, TrollOn);
        TrollSwitchVent = new(num++, MultiMenu.neutral, "<color=#678D36FF>Troll</color> Can Switch Vents", false, TrollOn);

        NeutralHarbingerSettings = new(MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Harbinger</color> Settings", PlaguebearerOn);
        NHMax = new(num++, MultiMenu.neutral, "Max <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Harbinger</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1, PlaguebearerOn);

        Plaguebearer = new(MultiMenu.neutral, "<color=#CFFE61FF>Plaguebearer</color>", PlaguebearerOn);
        UniquePlaguebearer = new(num++, MultiMenu.neutral, "<color=#CFFE61FF>Plaguebearer</color> Is Unique", false, PlaguebearerOn);
        InfectCooldown = new(num++, MultiMenu.neutral, "Infect Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, PlaguebearerOn);
        PBVent = new(num++, MultiMenu.neutral, "<color=#CFFE61FF>Plaguebearer</color> Can Vent", false, PlaguebearerOn);

        NeutralKillingSettings = new(MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killing</color> Settings", new[] { ArsonistOn, CryomaniacOn, GlitchOn,
            JuggernautOn, MurdererOn, SerialKillerOn, WerewolfOn });
        NKMax = new(num++, MultiMenu.neutral, "Max <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killing</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1, new[] { ArsonistOn,
            CryomaniacOn, GlitchOn, JuggernautOn, MurdererOn, SerialKillerOn, WerewolfOn });
        NKHasImpVision = new(num++, MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killers</color> Have <color=#FF0000FF>Intruder</color> Vision", true, new[] {
            ArsonistOn, CryomaniacOn, GlitchOn, JuggernautOn, MurdererOn, SerialKillerOn, WerewolfOn });
        NKsKnow = new(num++, MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killers</color> Know Each Other", false, new[] { ArsonistOn, CryomaniacOn, GlitchOn,
            JuggernautOn, MurdererOn, SerialKillerOn, WerewolfOn });

        Arsonist = new(MultiMenu.neutral, "<color=#EE7600FF>Arsonist</color>", ArsonistOn);
        UniqueArsonist = new(num++, MultiMenu.neutral, "<color=#EE7600FF>Arsonist</color> Is Unique", false, ArsonistOn);
        DouseCooldown = new(num++, MultiMenu.neutral, "Douse Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, ArsonistOn);
        IgniteCooldown = new(num++, MultiMenu.neutral, "Ignite Cooldown", 25f, 5f, 60f, 2.5f, CooldownFormat, ArsonistOn);
        ArsoLastKillerBoost = new(num++, MultiMenu.neutral, "Ignite Cooldown Removed When <color=#EE7600FF>Arsonist</color> Is Last Killer", false, ArsonistOn);
        ArsoIgniteAll = new(num++, MultiMenu.neutral, "Ignition Ignites All Doused Players", false, ArsonistOn);
        ArsoCooldownsLinked = new(num++, MultiMenu.neutral, "Douse And Ignite Cooldowns Are Linked", false, ArsonistOn);
        IgnitionCremates = new(num++, MultiMenu.neutral, "Ignition Cremates Bodies", false, ArsonistOn);
        ArsoVent = new(num++, MultiMenu.neutral, "<color=#EE7600FF>Arsonist</color> Can Vent", false, ArsonistOn);

        Cryomaniac = new(MultiMenu.neutral, "<color=#642DEAFF>Cryomaniac</color>", CryomaniacOn);
        UniqueCryomaniac = new(num++, MultiMenu.neutral, "<color=#642DEAFF>Cryomaniac</color> Is Unique", false, CryomaniacOn);
        CryoDouseCooldown = new(num++, MultiMenu.neutral, "Douse Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, CryomaniacOn);
        CryoFreezeAll = new(num++, MultiMenu.neutral, "<color=#642DEAFF>Cryomaniac</color> Freeze Freezes All Doused Players", false, CryomaniacOn);
        CryoLastKillerBoost = new(num++, MultiMenu.neutral, "<color=#642DEAFF>Cryomaniac</color> Can Kill Normally When Last Killer", false, CryomaniacOn);
        CryoVent = new(num++, MultiMenu.neutral, "<color=#642DEAFF>Cryomaniac</color> Can Vent", false, CryomaniacOn);

        Glitch = new(MultiMenu.neutral, "<color=#00FF00FF>Glitch</color>", GlitchOn);
        UniqueGlitch = new(num++, MultiMenu.neutral, "<color=#00FF00FF>Glitch</color> Is Unique", false, GlitchOn);
        MimicCooldown = new(num++, MultiMenu.neutral, "Mimic Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, GlitchOn);
        HackCooldown = new(num++, MultiMenu.neutral, "Hack Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, GlitchOn);
        MimicDuration = new(num++, MultiMenu.neutral, "Mimic Duration", 10f, 5f, 30f, 1f, CooldownFormat, GlitchOn);
        HackDuration = new(num++, MultiMenu.neutral, "Hack Duration", 10f, 5f, 30f, 1f, CooldownFormat, GlitchOn);
        GlitchKillCooldown = new(num++, MultiMenu.neutral, "Neutralise Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, GlitchOn);
        GlitchVent = new(num++, MultiMenu.neutral, "<color=#00FF00FF>Glitch</color> Can Vent", false, GlitchOn);

        Juggernaut = new(MultiMenu.neutral, "<color=#A12B56FF>Juggernaut</color>", JuggernautOn);
        UniqueJuggernaut = new(num++, MultiMenu.neutral, "<color=#A12B56FF>Juggernaut</color> Is Unique", false, JuggernautOn);
        JuggKillCooldown = new(num++, MultiMenu.neutral, "Assault Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, JuggernautOn);
        JuggKillBonus = new(num++, MultiMenu.neutral, "Assault Cooldown Bonus", 5f, 2.5f, 30f, 2.5f, CooldownFormat, JuggernautOn);
        JuggVent = new(num++, MultiMenu.neutral, "<color=#A12B56FF>Juggernaut</color> Can Vent", false, JuggernautOn);

        Murderer = new(MultiMenu.neutral, "<color=#6F7BEAFF>Murderer</color>", MurdererOn);
        UniqueMurderer = new(num++, MultiMenu.neutral, "<color=#6F7BEAFF>Murderer</color> Is Unique", false, MurdererOn);
        MurdKillCooldownOption = new(num++, MultiMenu.neutral, "Murder Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, MurdererOn);
        MurdVent = new(num++, MultiMenu.neutral, "<color=#6F7BEAFF>Murderer</color> Can Vent", false, MurdererOn);

        SerialKiller = new(MultiMenu.neutral, "<color=#336EFFFF>Serial Killer</color>", SerialKillerOn);
        UniqueSerialKiller = new(num++, MultiMenu.neutral, "<color=#336EFFFF>Serial Killer</color> Is Unique", false, SerialKillerOn);
        BloodlustCooldown = new(num++, MultiMenu.neutral, "Bloodlust Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, SerialKillerOn);
        BloodlustDuration = new(num++, MultiMenu.neutral, "Bloodlust Duration", 10f, 5f, 30f, 1f, CooldownFormat, SerialKillerOn);
        LustKillCooldown = new(num++, MultiMenu.neutral, "Stab Cooldown", 5f, 0.5f, 15f, 0.5f, CooldownFormat, SerialKillerOn);
        SKVentOptions = new(num++, MultiMenu.neutral, "<color=#336EFFFF>Serial Killer</color> Can Vent", new[] { "Always", "Bloodlust", "No Lust", "Never" }, SerialKillerOn);

        Werewolf = new(MultiMenu.neutral, "<color=#9F703AFF>Werewolf</color>", WerewolfOn);
        UniqueWerewolf = new(num++, MultiMenu.neutral, "<color=#9F703AFF>Werewolf</color> Is Unique", false, WerewolfOn);
        MaulCooldown = new(num++, MultiMenu.neutral, "Maul Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, WerewolfOn);
        MaulRadius = new(num++, MultiMenu.neutral, "Maul Radius", 1.5f, 0.5f, 5f, 0.25f, DistanceFormat, WerewolfOn);
        WerewolfVent = new(num++, MultiMenu.neutral, "<color=#9F703AFF>Werewolf</color> Can Vent", false, WerewolfOn);

        NeutralNeophyteSettings = new(MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Neophyte</color> Settings", new[] { DraculaOn, WhispererOn, JackalOn,
            NecromancerOn });
        NNMax = new(num++, MultiMenu.neutral, "Max <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Neophyte</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1, new[] { DraculaOn,
            WhispererOn, JackalOn, NecromancerOn });
        NNHasImpVision = new(num++, MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Neophytes</color> Have <color=#FF0000FF>Intruder</color> Vision", true, new[] {
            DraculaOn, WhispererOn, JackalOn, NecromancerOn });

        Dracula = new(MultiMenu.neutral, "<color=#AC8A00FF>Dracula</color>", DraculaOn);
        UniqueDracula = new(num++, MultiMenu.neutral, "<color=#AC8A00FF>Dracula</color> Is Unique", false, DraculaOn);
        BiteCooldown = new(num++, MultiMenu.neutral, "Bite Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, DraculaOn);
        DracVent = new(num++, MultiMenu.neutral, "<color=#AC8A00FF>Dracula</color> Can Vent", false, DraculaOn);
        AliveVampCount = new(num++, MultiMenu.neutral, "Alive <color=#7B8968FF>Undead</color> Count", 3, 1, 14, 1, DraculaOn);
        UndeadVent = new(num++, MultiMenu.neutral, "Undead Can Vent", false, DraculaOn);

        Jackal = new(MultiMenu.neutral, "<color=#45076AFF>Jackal</color>", JackalOn);
        UniqueJackal = new(num++, MultiMenu.neutral, "<color=#45076AFF>Jackal</color> Is Unique", false, JackalOn);
        RecruitCooldown = new(num++, MultiMenu.neutral, "Recruit Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, JackalOn);
        JackalVent = new(num++, MultiMenu.neutral, "<color=#45076AFF>Jackal</color> Can Vent", false, JackalOn);
        RecruitVent = new(num++, MultiMenu.neutral, "Recruits Can Vent", false, JackalOn);

        Necromancer = new(MultiMenu.neutral, "<color=#BF5FFFFF>Necromancer</color>", NecromancerOn);
        UniqueNecromancer = new(num++, MultiMenu.neutral, "<color=#BF5FFFFF>Necromancer</color> Is Unique", false, NecromancerOn);
        ResurrectCooldown = new(num++, MultiMenu.neutral, "Resurrect Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, NecromancerOn);
        ResurrectCooldownIncrease = new(num++, MultiMenu.neutral, "Resurrect Cooldown Increases By", 5f, 2.5f, 30f, 2.5f, CooldownFormat, NecromancerOn);
        ResurrectCooldownIncreases = new(num++, MultiMenu.neutral, "Resurrect Cooldown Increases", true, NecromancerOn);
        ResurrectCount = new(num++, MultiMenu.neutral, "Resurrect Count", 5, 1, 14, 1, NecromancerOn);
        NecroKillCooldown = new(num++, MultiMenu.neutral, "Kill Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, NecromancerOn);
        NecroKillCooldownIncrease = new(num++, MultiMenu.neutral, "Kill Cooldown Increases By", 5f, 2.5f, 30f, 2.5f, CooldownFormat, NecromancerOn);
        NecroKillCooldownIncreases = new(num++, MultiMenu.neutral, "Kill Cooldown Increases", true, NecromancerOn);
        NecroKillCount = new(num++, MultiMenu.neutral, "Kill Count", 5, 1, 14, 1, NecromancerOn);
        KillResurrectCooldownsLinked = new(num++, MultiMenu.neutral, "Kill And Resurrect Cooldowns Are Linked", false, NecromancerOn);
        NecromancerTargetBody = new(num++, MultiMenu.neutral, "Target's Body Disappears On Beginning Of Resurrect", false, NecromancerOn);
        NecroResurrectDuration = new(num++, MultiMenu.neutral, "Resurrect Duration", 10f, 1f, 15f, 1f, CooldownFormat, NecromancerOn);
        NecroVent = new(num++, MultiMenu.neutral, "<color=#BF5FFFFF>Necromancer</color> Can Vent", false, NecromancerOn);
        ResurrectVent = new(num++, MultiMenu.neutral, "Resurrected Can Vent", false, NecromancerOn);

        Whisperer = new(MultiMenu.neutral, "<color=#2D6AA5FF>Whisperer</color>", WhispererOn);
        UniqueWhisperer = new(num++, MultiMenu.neutral, "<color=#2D6AA5FF>Whisperer</color> Is Unique", false, WhispererOn);
        WhisperCooldown = new(num++, MultiMenu.neutral, "Whisper Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, WhispererOn);
        WhisperCooldownIncreases = new(num++, MultiMenu.neutral, "Whisper Cooldown Increases Each Whisper", false, WhispererOn);
        WhisperCooldownIncrease = new(num++, MultiMenu.neutral, "Whisper Cooldown Increases By", 5f, 2.5f, 30f, 2.5f, CooldownFormat, WhispererOn);
        WhisperRadius = new(num++, MultiMenu.neutral, "Whisper Radius", 1.5f, 0.5f, 5f, 0.25f, DistanceFormat, WhispererOn);
        InitialWhisperRate = new(num++, MultiMenu.neutral, "Whisper Rate", 5, 5, 50, 5, PercentFormat, WhispererOn);
        WhisperRateDecreases = new(num++, MultiMenu.neutral, "Whisper Rate Decreases Each Whisper", false, WhispererOn);
        WhisperRateDecrease = new(num++, MultiMenu.neutral, "Whisper Rate Decrease", 5, 5, 50, 5, PercentFormat, WhispererOn);
        WhispVent = new(num++, MultiMenu.neutral, "<color=#2D6AA5FF>Whisperer</color> Can Vent", false, WhispererOn);
        PersuadedVent = new(num++, MultiMenu.neutral, "Persuaded Can Vent", false, WhispererOn);

        NeutralProselyteSettings = new(MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Proselyte</color> Settings", new[] { PhantomOn, TraitorOn, FanaticOn });

        Betrayer = new(MultiMenu.neutral, "<color=#11806AFF>Betrayer</color>", new[] { TraitorOn, FanaticOn });
        BetrayerKillCooldown = new(num++, MultiMenu.neutral, "Kill Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, new[] { TraitorOn, FanaticOn });
        BetrayerVent = new(num++, MultiMenu.neutral, "<color=#11806AFF>Betrayer</color> Can Vent", true, new[] { TraitorOn, FanaticOn });

        Phantom = new(MultiMenu.neutral, "<color=#662962FF>Phantom</color>", PhantomOn);
        PhantomTasksRemaining = new(num++, MultiMenu.neutral, "Tasks Remaining When <color=#662962FF>Phantom</color> Can Be Clicked", 5, 1, 10, 1, PhantomOn);
        PhantomPlayersAlerted = new(num++, MultiMenu.neutral, "Players Are Alerted When <color=#662962FF>Phantom</color> Is Clickable", false, PhantomOn);

        IntruderSettings = new(MultiMenu.intruder, "<color=#FF0000FF>Intruder</color> Settings");
        IntruderCount = new(num++, MultiMenu.intruder, "<color=#FF0000FF>Intruder</color> Count", 1, 0, 4, 1);
        IntruderVision = new(num++, MultiMenu.intruder, "<color=#FF0000FF>Intruder</color> Vision", 2f, 0.25f, 5f, 0.25f, MultiplierFormat);
        IntruderFlashlight = new(num++, MultiMenu.intruder, "<color=#FF0000FF>Intruders</color> Use A Flashlight", false);
        IntruderKillCooldown = new(num++, MultiMenu.intruder, "<color=#FF0000FF>Intruder</color> Kill Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
        IntrudersVent = new(num++, MultiMenu.intruder, "<color=#FF0000FF>Intruder</color> Can Vent", true);
        IntrudersCanSabotage = new(num++, MultiMenu.intruder, "<color=#FF0000FF>Intruder</color> Can Sabotage", true);
        GhostsCanSabotage = new(num++, MultiMenu.intruder, "Dead <color=#FF0000FF>Intruders</color> Can Sabotage", false);
        IntruderMax = new(num++, MultiMenu.intruder, "Max <color=#FF0000FF>Intruder</color> <color=#FFD700FF>Roles</color>", 5, 1, 14, 1);
        IntruderMin = new(num++, MultiMenu.intruder, "Min <color=#FF0000FF>Intruder</color> <color=#FFD700FF>Roles</color>", 5, 1, 14, 1);

        IntruderConcealingSettings = new(MultiMenu.intruder, "<color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Concealing</color> Settings", new[] { BlackmailerOn, CamouflagerOn,
            GrenadierOn, JanitorOn });
        ICMax = new(num++, MultiMenu.intruder, "Max <color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Concealing</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1, new[] { JanitorOn,
            BlackmailerOn, CamouflagerOn, GrenadierOn });

        Blackmailer = new(MultiMenu.intruder, "<color=#02A752FF>Blackmailer</color>", BlackmailerOn);
        UniqueBlackmailer = new(num++, MultiMenu.intruder, "<color=#02A752FF>Blackmailer</color> Is Unique", false, BlackmailerOn);
        BlackmailCooldown = new(num++, MultiMenu.intruder, "Blackmail Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, BlackmailerOn);
        WhispersNotPrivate = new(num++, MultiMenu.intruder, "<color=#02A752FF>Blackmailer</color> Can Read Whispers", true, BlackmailerOn);
        BlackmailMates = new(num++, MultiMenu.intruder, "<color=#02A752FF>Blackmailer</color> Can Blackmail Teammates", false, BlackmailerOn);
        BMRevealed = new(num++, MultiMenu.intruder, "Blackmail Is Revealed To Everyone", true, BlackmailerOn);

        Camouflager = new(MultiMenu.intruder, "<color=#378AC0FF>Camouflager</color>", CamouflagerOn);
        UniqueCamouflager = new(num++, MultiMenu.intruder, "<color=#378AC0FF>Camouflager</color> Is Unique", false, CamouflagerOn);
        CamouflagerCooldown = new(num++, MultiMenu.intruder, "Camouflage Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, CamouflagerOn);
        CamouflagerDuration = new(num++, MultiMenu.intruder, "Camouflage Duration", 10f, 5f, 30f, 1f, CooldownFormat, CamouflagerOn);
        CamoHideSize = new(num++, MultiMenu.intruder, "Camouflage Hides Player Size", false, CamouflagerOn);
        CamoHideSpeed = new(num++, MultiMenu.intruder, "Camouflage Hides Player Speed", false, CamouflagerOn);

        Grenadier = new(MultiMenu.intruder, "<color=#85AA5BFF>Grenadier</color>", GrenadierOn);
        UniqueGrenadier = new(num++, MultiMenu.intruder, "<color=#85AA5BFF>Grenadier</color> Is Unique", false, GrenadierOn);
        GrenadeCooldown = new(num++, MultiMenu.intruder, "Flash Grenade Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, GrenadierOn);
        GrenadeDuration = new(num++, MultiMenu.intruder, "Flash Grenade Duration", 10f, 5f, 30f, 1f, CooldownFormat, GrenadierOn);
        FlashRadius = new(num++, MultiMenu.intruder, "Flash Radius", 1.5f, 0.5f, 5f, 0.25f, DistanceFormat, GrenadierOn);
        GrenadierIndicators = new(num++, MultiMenu.intruder, "Indicate Flashed Players", false, GrenadierOn);
        GrenadierVent = new(num++, MultiMenu.intruder, "<color=#85AA5BFF>Grenadier</color> Can Vent", false, GrenadierOn);

        Janitor = new(MultiMenu.intruder, "<color=#2647A2FF>Janitor</color>", JanitorOn);
        UniqueJanitor = new(num++, MultiMenu.intruder, "<color=#2647A2FF>Janitor</color> Is Unique", false, JanitorOn);
        JanitorCleanCd = new(num++, MultiMenu.intruder, "Clean Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, JanitorOn);
        JaniCooldownsLinked = new(num++, MultiMenu.intruder, "Kill And Clean Cooldowns Are Linked", false, JanitorOn);
        SoloBoost = new(num++, MultiMenu.intruder, "<color=#2647A2FF>Janitor</color> Has Lower Cooldown When Solo", false, JanitorOn);
        DragCooldown = new(num++, MultiMenu.intruder, "Drag Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, JanitorOn);
        DragModifier = new(num++, MultiMenu.intruder, "Drag Speed", 0.5f, 0.25f, 3f, 0.25f, MultiplierFormat, JanitorOn);
        JanitorVentOptions = new(num++, MultiMenu.intruder, "<color=#2647A2FF>Janitor</color> Can Vent", new[] { "Never", "Body", "Bodyless", "Always" }, JanitorOn);

        IntruderDeceptionSettings = new(MultiMenu.intruder, "<color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Deception</color> Settings", new[] { DisguiserOn, MorphlingOn, WraithOn });
        IDMax = new(num++, MultiMenu.intruder, "Max <color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Deception</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1, new[] { MorphlingOn,
            DisguiserOn, WraithOn });

        Disguiser = new(MultiMenu.intruder, "<color=#40B4FFFF>Disguiser</color>", DisguiserOn);
        UniqueDisguiser = new(num++, MultiMenu.intruder, "<color=#40B4FFFF>Disguiser</color> Is Unique", false, DisguiserOn);
        DisguiseCooldown = new(num++, MultiMenu.intruder, "Disguise Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, DisguiserOn);
        TimeToDisguise = new(num++, MultiMenu.intruder, "Delay Before Disguising", 5f, 2.5f, 15f, 2.5f, CooldownFormat, DisguiserOn);
        DisguiseDuration = new(num++, MultiMenu.intruder, "Disguise Duration", 10f, 2.5f, 20f, 2.5f, CooldownFormat, DisguiserOn);
        MeasureCooldown = new(num++, MultiMenu.intruder, "Measure Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, DisguiserOn);
        DisgCooldownsLinked = new(num++, MultiMenu.intruder, "Measure And Disguise Cooldowns Are Linked", false, DisguiserOn);
        DisguiseTarget = new(num++, MultiMenu.intruder, "<color=#40B4FFFF>Disguiser</color> Can Disguise", new[] { "Everyone", "Only Intruders", "Non Intruders" }, DisguiserOn);

        Morphling = new(MultiMenu.intruder, "<color=#BB45B0FF>Morphling</color>", MorphlingOn);
        UniqueMorphling = new(num++, MultiMenu.intruder, "<color=#BB45B0FF>Morphling</color> Is Unique", false, MorphlingOn);
        MorphlingCooldown = new(num++, MultiMenu.intruder, "Morph Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, MorphlingOn);
        MorphlingDuration = new(num++, MultiMenu.intruder, "Morph Duration", 10f, 5f, 30f, 1f, CooldownFormat, MorphlingOn);
        SampleCooldown = new(num++, MultiMenu.intruder, "Sample Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, MorphlingOn);
        MorphCooldownsLinked = new(num++, MultiMenu.intruder, "Sample And Morph Cooldowns Are Linked", false, MorphlingOn);
        MorphlingVent = new(num++, MultiMenu.intruder, "<color=#BB45B0FF>Morphling</color> Can Vent", false, MorphlingOn);

        Wraith = new(MultiMenu.intruder, "<color=#5C4F75FF>Wraith</color>", WraithOn);
        UniqueWraith = new(num++, MultiMenu.intruder, "<color=#5C4F75FF>Wraith</color> Is Unique", false, WraithOn);
        InvisCooldown = new(num++, MultiMenu.intruder, "Invis Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, WraithOn);
        InvisDuration = new(num++, MultiMenu.intruder, "Invis Duration", 10f, 5f, 30f, 1f, CooldownFormat, WraithOn);
        WraithVent = new(num++, MultiMenu.intruder, "<color=#5C4F75FF>Wraith</color> Can Vent", false, WraithOn);

        IntruderKillingSettings = new(MultiMenu.intruder, "<color=#FF0000FF>Impostor</color> <color=#1D7CF2FF>Killing</color> Settings", new[] { AmbusherOn, EnforcerOn });
        IKMax = new(num++, MultiMenu.intruder, "Max <color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Killing</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1, new[] {
            AmbusherOn, EnforcerOn });

        Ambusher = new(MultiMenu.intruder, "<color=#2BD29CFF>Ambusher</color>", AmbusherOn);
        UniqueAmbusher = new(num++, MultiMenu.intruder, "<color=#2BD29CFF>Ambusher</color> Is Unique", false, AmbusherOn);
        AmbushCooldown = new(num++, MultiMenu.intruder, "Ambush Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, AmbusherOn);
        AmbushDuration = new(num++, MultiMenu.intruder, "Ambush Duration", 10f, 5f, 30f, 1f, CooldownFormat, AmbusherOn);
        AmbushMates = new(num++, MultiMenu.intruder, "<color=#2BD29CFF>Ambusher</color> Can Ambush Teammates", false, AmbusherOn);

        Enforcer = new(MultiMenu.intruder, "<color=#005643FF>Enforcer</color>", EnforcerOn);
        UniqueEnforcer = new(num++, MultiMenu.intruder, "<color=#005643FF>Enforcer</color> Is Unique", false, EnforcerOn);
        EnforceCooldown = new(num++, MultiMenu.intruder, "Enforce Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, EnforcerOn);
        EnforceDuration = new(num++, MultiMenu.intruder, "Enforce Duration", 10f, 5f, 30f, 1f, CooldownFormat, EnforcerOn);
        EnforceDelay = new(num++, MultiMenu.intruder, "Enforce Delay", 5f, 1f, 15f, 1f, CooldownFormat, EnforcerOn);
        EnforceRadius = new(num++, MultiMenu.intruder, "Enforce Explosion Radius", 1.5f, 0.5f, 5f, 0.25f, DistanceFormat, EnforcerOn);

        IntruderSupportSettings = new(MultiMenu.intruder, "<color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Support</color> Settings", new[] { ConsigliereOn, ConsortOn, GodfatherOn,
            MinerOn, TeleporterOn });
        ISMax = new(num++, MultiMenu.intruder, "Max <color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Support</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1, new[] { MinerOn,
            ConsigliereOn, ConsortOn, GodfatherOn, TeleporterOn });

        Consigliere = new(MultiMenu.intruder, "<color=#FFFF99FF>Consigliere</color>", ConsigliereOn);
        UniqueConsigliere = new(num++, MultiMenu.intruder, "<color=#FFFF99FF>Consigliere</color> Is Unique", false, ConsigliereOn);
        InvestigateCooldown = new(num++, MultiMenu.intruder, "Investigate Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, ConsigliereOn);
        ConsigInfo = new(num++, MultiMenu.intruder, "Info That <color=#FFFF99FF>Consigliere</color> Sees", new[] { "Role", "Faction" }, ConsigliereOn);

        Consort = new(MultiMenu.intruder, "<color=#801780FF>Consort</color>", ConsortOn);
        UniqueConsort = new(num++, MultiMenu.intruder, "<color=#801780FF>Consort</color> Is Unique", false, ConsortOn);
        ConsRoleblockCooldown = new(num++, MultiMenu.intruder, "Roleblock Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, ConsortOn);
        ConsRoleblockDuration = new(num++, MultiMenu.intruder, "Roleblock Duration", 10f, 5f, 30f, 1f, CooldownFormat, ConsortOn);

        Godfather = new(MultiMenu.intruder, "<color=#404C08FF>Godfather</color>", GodfatherOn);
        UniqueGodfather = new(num++, MultiMenu.intruder, "<color=#404C08FF>Godfather</color> Is Unique", false, GodfatherOn);
        MafiosoAbilityCooldownDecrease = new(num++, MultiMenu.intruder, "Ability Cooldown Bonus", 0.75f, 0.25f, 0.9f, 0.05f, MultiplierFormat, GodfatherOn);

        Miner = new(MultiMenu.intruder, "<color=#AA7632FF>Miner</color>", MinerOn);
        UniqueMiner = new(num++, MultiMenu.intruder, "<color=#AA7632FF>Miner</color> Is Unique", false, MinerOn);
        MineCooldown = new(num++, MultiMenu.intruder, "Mine Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, MinerOn);

        Teleporter = new(MultiMenu.intruder, "<color=#939593FF>Teleporter</color>", TeleporterOn);
        UniqueTeleporter = new(num++, MultiMenu.intruder, "<color=#939593FF>Teleporter</color> Is Unique", false, TeleporterOn);
        TeleportCd = new(num++, MultiMenu.intruder, "Teleport Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, TeleporterOn);
        MarkCooldown = new(num++, MultiMenu.intruder, "Mark Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, TeleporterOn);
        TeleCooldownsLinked = new(num++, MultiMenu.intruder, "Mark And Teleport Cooldowns Are Linked", false, TeleporterOn);
        TeleVent = new(num++, MultiMenu.intruder, "<color=#939593FF>Teleporter</color> Can Vent", false, TeleporterOn);

        IntruderUtilitySettings = new(MultiMenu.intruder, "<color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Utility</color> Settings", GhoulOn);

        Ghoul = new(MultiMenu.intruder, "<color=#F1C40FFF>Ghoul</color>", GhoulOn);
        GhoulMarkCd = new(num++, MultiMenu.intruder, "Mark Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, GhoulOn);

        SyndicateSettings = new(MultiMenu.syndicate, "<color=#008000FF>Syndicate</color> Settings");
        SyndicateCount = new(num++, MultiMenu.syndicate, "<color=#008000FF>Syndicate</color> Count", 1, 0, 4, 1);
        SyndicateVision = new(num++, MultiMenu.syndicate, "<color=#008000FF>Syndicate</color> Vision", 2f, 0.25f, 5f, 0.25f, MultiplierFormat);
        SyndicateFlashlight = new(num++, MultiMenu.syndicate, "<color=#008000FF>Syndicate</color> Uses A Flashlight", false);
        ChaosDriveMeetingCount = new(num++, MultiMenu.syndicate, "Chaos Drive Timer", 3, 1, 10, 1);
        ChaosDriveKillCooldown = new(num++, MultiMenu.syndicate, "Chaos Drive Holder Kill Cooldown", 15f, 10f, 45f, 2.5f, CooldownFormat);
        SyndicateVent = new(num++, MultiMenu.syndicate, "<color=#008000FF>Syndicate</color> Can Vent", new[] { "Always", "Chaos Drive", "Never" });
        AltImps = new(num++, MultiMenu.syndicate, "<color=#008000FF>Syndicate</color> Replaces <color=#FF0000FF>Intruders</color>", false);
        GlobalDrive = new(num++, MultiMenu.syndicate, "Chaos Drive Is Global", false);
        SyndicateMax = new(num++, MultiMenu.syndicate, "Max <color=#008000FF>Syndicate</color> <color=#FFD700FF>Roles</color>", 5, 1, 14, 1);
        SyndicateMin = new(num++, MultiMenu.syndicate, "Min <color=#008000FF>Syndicate</color> <color=#FFD700FF>Roles</color>", 5, 1, 14, 1);

        SyndicateDisruptionSettings = new(MultiMenu.syndicate, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Disruption</color> Settings", new[] { ConcealerOn, DrunkardOn, FramerOn,
            ShapeshifterOn, SilencerOn });
        SDMax = new(num++, MultiMenu.syndicate, "Max <color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Disruption</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1, new[] { FramerOn,
            ConcealerOn, DrunkardOn, ShapeshifterOn, SilencerOn });

        Concealer = new(MultiMenu.syndicate, "<color=#C02525FF>Concealer</color>", ConcealerOn);
        UniqueConcealer = new(num++, MultiMenu.syndicate, "<color=#C02525FF>Concealer</color> Is Unique", false, ConcealerOn);
        ConcealCooldown = new(num++, MultiMenu.syndicate, "Conceal Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, ConcealerOn);
        ConcealDuration = new(num++, MultiMenu.syndicate, "Conceal Duration", 10f, 5f, 30f, 1f, CooldownFormat, ConcealerOn);
        ConcealMates = new(num++, MultiMenu.syndicate, "<color=#C02525FF>Concealer</color> Can Conceal Teammates", false, ConcealerOn);

        Drunkard = new(MultiMenu.syndicate, "<color=#FF7900FF>Drunkard</color>", DrunkardOn);
        UniqueDrunkard = new(num++, MultiMenu.syndicate, "<color=#FF7900FF>Drunkard</color> Is Unique", false, DrunkardOn);
        ConfuseCooldown = new(num++, MultiMenu.syndicate, "Confuse Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, DrunkardOn);
        ConfuseDuration = new(num++, MultiMenu.syndicate, "Confuse Duration", 10f, 5f, 30f, 1f, CooldownFormat, DrunkardOn);
        ConfuseImmunity = new(num++, MultiMenu.syndicate, "<color=#008000FF>Syndicate</color> Are Immune To Confuse", true, DrunkardOn);

        Framer = new(MultiMenu.syndicate, "<color=#00FFFFFF>Framer</color>", FramerOn);
        UniqueFramer = new(num++, MultiMenu.syndicate, "<color=#00FFFFFF>Framer</color> Is Unique", false, FramerOn);
        FrameCooldown = new(num++, MultiMenu.syndicate, "Frame Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, FramerOn);
        ChaosDriveFrameRadius = new(num++, MultiMenu.syndicate, "Chaos Drive Frame Radius", 1.5f, 0.5f, 5f, 0.25f, DistanceFormat, FramerOn);

        Shapeshifter = new(MultiMenu.syndicate, "<color=#2DFF00FF>Shapeshifter</color>", ShapeshifterOn);
        UniqueShapeshifter = new(num++, MultiMenu.syndicate, "<color=#2DFF00FF>Shapeshifter</color> Is Unique", false, ShapeshifterOn);
        ShapeshiftCooldown = new(num++, MultiMenu.syndicate, "Shapeshift Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, ShapeshifterOn);
        ShapeshiftDuration = new(num++, MultiMenu.syndicate, "Shapeshift Duration", 10f, 5f, 30f, 1f, CooldownFormat, ShapeshifterOn);
        ShapeshiftMates = new(num++, MultiMenu.syndicate, "<color=#2DFF00FF>Shapeshifter</color> Can Shapeshift Teammates", false, ShapeshifterOn);

        Silencer = new(MultiMenu.syndicate, "<color=#AAB43EFF>Silencer</color>", SilencerOn);
        UniqueSilencer = new(num++, MultiMenu.syndicate, "<color=#AAB43EFF>Silencer</color> Is Unique", false, SilencerOn);
        SilenceCooldown = new(num++, MultiMenu.syndicate, "Silence Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, SilencerOn);
        WhispersNotPrivateSilencer = new(num++, MultiMenu.syndicate, "<color=#AAB43EFF>Silencer</color> Can Read Whispers", true, SilencerOn);
        SilenceMates = new(num++, MultiMenu.syndicate, "<color=#AAB43EFF>Silencer</color> Can Silence Teammates", false, SilencerOn);
        SilenceRevealed = new(num++, MultiMenu.syndicate, "Silence Is Revealed To Everyone", true, SilencerOn);

        SyndicateKillingSettings = new(MultiMenu.syndicate, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Killing</color> Settings", new[] { BomberOn, ColliderOn, CrusaderOn,
            PoisonerOn });
        SyKMax = new(num++, MultiMenu.syndicate, "Max <color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Killing</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1, new[] { BomberOn,
            ColliderOn, CrusaderOn, PoisonerOn });

        Bomber = new(MultiMenu.syndicate, "<color=#C9CC3FFF>Bomber</color>", BomberOn);
        UniqueBomber = new(num++, MultiMenu.syndicate, "<color=#C9CC3FFF>Bomber</color> Is Unique", false, BomberOn);
        BombCooldown = new(num++, MultiMenu.syndicate, "Bomb Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, BomberOn);
        DetonateCooldown = new(num++, MultiMenu.syndicate, "Detonation Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, BomberOn);
        BombCooldownsLinked = new(num++, MultiMenu.syndicate, "Place And Detonate Cooldowns Are Linked", false, BomberOn);
        BombsRemoveOnNewRound = new(num++, MultiMenu.syndicate, "Bombs Are Cleared Every Meeting", false, BomberOn);
        BombsDetonateOnMeetingStart = new(num++, MultiMenu.syndicate, "Bombs Detonate Everytime A Meeting Is Called", false, BomberOn);
        BombRange = new(num++, MultiMenu.syndicate, "Bomb Radius", 1.5f, 0.5f, 5f, 0.25f, DistanceFormat, BomberOn);
        ChaosDriveBombRange = new(num++, MultiMenu.syndicate, "Chaos Drive Bomb Radius Increase", 0.5f, 0.5f, 5f, 0.25f, DistanceFormat, BomberOn);
        BombKillsSyndicate = new(num++, MultiMenu.syndicate, "Bomb Detonation Kills Members Of The <color=#008000FF>Syndicate</color>", true, BomberOn);

        Collider = new(MultiMenu.syndicate, "<color=#B345FFFF>Collider</color>", ColliderOn);
        UniqueCollider = new(num++, MultiMenu.syndicate, "<color=#B345FFFF>Collider</color> Is Unique", false, ColliderOn);
        CollideCooldown = new(num++, MultiMenu.syndicate, "Set Charges Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, ColliderOn);
        ChargeCooldown = new(num++, MultiMenu.syndicate, "Charge Cooldown With Chose Drive", 25f, 10f, 60f, 2.5f, CooldownFormat, ColliderOn);
        ChargeDuration = new(num++, MultiMenu.syndicate, "Charge Duration", 10f, 5f, 30f, 1f, CooldownFormat, ColliderOn);
        CollideRange = new(num++, MultiMenu.syndicate, "Collide Radius", 1.5f, 0.5f, 5f, 0.25f, DistanceFormat, ColliderOn);
        CollideRangeIncrease = new(num++, MultiMenu.syndicate, "Chaos Drive Collide Radius Increase", 0.5f, 0.5f, 5f, 0.25f, DistanceFormat, ColliderOn);
        ChargeCooldownsLinked = new(num++, MultiMenu.syndicate, "Charge Cooldowns Are Linked", false, ColliderOn);
        CollideResetsCooldown = new(num++, MultiMenu.syndicate, "Collision Resets Charge Cooldowns", false, ColliderOn);

        Crusader = new(MultiMenu.syndicate, "<color=#DF7AE8FF>Crusader</color>", CrusaderOn);
        UniqueCrusader = new(num++, MultiMenu.syndicate, "<color=#DF7AE8FF>Crusader</color> Is Unique", false, CrusaderOn);
        CrusadeCooldown = new(num++, MultiMenu.syndicate, "Crusade Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, CrusaderOn);
        CrusadeDuration = new(num++, MultiMenu.syndicate, "Crusade Duration", 10f, 5f, 30f, 1f, CooldownFormat, CrusaderOn);
        ChaosDriveCrusadeRadius = new(num++, MultiMenu.syndicate, "Chaos Drive Crusade Radius", 1.5f, 0.5f, 5f, 0.25f, DistanceFormat, CrusaderOn);
        CrusadeMates = new(num++, MultiMenu.syndicate, "<color=#DF7AE8FF>Crusader</color> Can Crusade Teammates", false, CrusaderOn);

        Poisoner = new(MultiMenu.syndicate, "<color=#B5004CFF>Poisoner</color>", PoisonerOn);
        UniquePoisoner = new(num++, MultiMenu.syndicate, "<color=#B5004CFF>Poisoner</color> Is Unique", false, PoisonerOn);
        PoisonCooldown = new(num++, MultiMenu.syndicate, "Poison Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, PoisonerOn);
        PoisonDuration = new(num++, MultiMenu.syndicate, "Poison Kill Delay", 5f, 1f, 15f, 1f, CooldownFormat, PoisonerOn);

        SyndicatePowerSettings = new(MultiMenu.syndicate, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Power</color> Settings", new[] { SpellslingerOn, TimeKeeperOn });
        SPMax = new(num++, MultiMenu.syndicate, "Max <color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Power</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1, new[] { SpellslingerOn,
            TimeKeeperOn });

        Spellslinger = new(MultiMenu.syndicate, "<color=#0028F5FF>Spellslinger</color>", SpellslingerOn);
        UniqueSpellslinger = new(num++, MultiMenu.syndicate, "<color=#0028F5FF>Spellslinger</color> Is Unique", false, SpellslingerOn);
        SpellCooldown = new(num++, MultiMenu.syndicate, "Spell Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, SpellslingerOn);
        SpellCooldownIncrease = new(num++, MultiMenu.syndicate, "Spell Cooldown Increase", 5f, 2.5f, 30f, 2.5f, CooldownFormat, SpellslingerOn);

        TimeKeeper = new(MultiMenu.syndicate, "<color=#3769FEFF>Time Keeper</color>", TimeKeeperOn);
        UniqueTimeKeeper = new(num++, MultiMenu.syndicate, "<color=#3769FEFF>Time Keeper</color> Is Unique", false, TimeKeeperOn);
        TimeControlCooldown = new(num++, MultiMenu.syndicate, "Time Control Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, TimeKeeperOn);
        TimeControlDuration = new(num++, MultiMenu.syndicate, "Time Control Duration", 10f, 5f, 30f, 1f, CooldownFormat, TimeKeeperOn);
        TimeFreezeImmunity = new(num++, MultiMenu.syndicate, "<color=#008000FF>Syndicate</color> Are Immune To Freeze", true, TimeKeeperOn);
        TimeRewindImmunity = new(num++, MultiMenu.syndicate, "<color=#008000FF>Syndicate</color> Are Immune To Rewind", true, TimeKeeperOn);

        SyndicateSupportSettings = new(MultiMenu.syndicate, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Support</color> Settings", new[] { RebelOn, StalkerOn, WarperOn });
        SSuMax = new(num++, MultiMenu.syndicate, "Max <color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Support</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1, new[] { RebelOn,
            StalkerOn, WarperOn });

        Rebel = new(MultiMenu.syndicate, "<color=#FFFCCEFF>Rebel</color>", RebelOn);
        UniqueRebel = new(num++, MultiMenu.syndicate, "<color=#FFFCCEFF>Rebel</color> Is Unique", false, RebelOn);
        SidekickAbilityCooldownDecrease = new(num++, MultiMenu.syndicate, "Ability Cooldown Bonus", 0.75f, 0.25f, 0.9f, 0.05f, MultiplierFormat, RebelOn);

        Stalker = new(MultiMenu.syndicate, "<color=#7E4D00FF>Stalker</color>", StalkerOn);
        UniqueStalker = new(num++, MultiMenu.syndicate, "<color=#7E4D00FF>Stalker</color> Is Unique", false, StalkerOn);
        StalkCooldown = new(num++, MultiMenu.syndicate, "Stalk Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, StalkerOn);

        Warper = new(MultiMenu.syndicate, "<color=#8C7140FF>Warper</color>", WarperOn);
        UniqueWarper = new(num++, MultiMenu.syndicate, "<color=#8C7140FF>Warper</color> Is Unique", false, WarperOn);
        WarpCooldown = new(num++, MultiMenu.syndicate, "Warp Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, WarperOn);
        WarpDuration = new(num++, MultiMenu.syndicate, "Warp Duration", 5f, 1f, 20f, 1f, CooldownFormat, WarperOn);
        WarpSelf = new(num++, MultiMenu.syndicate, "<color=#8C7140FF>Warper</color> Can Warp Themselves", true, WarperOn);

        SyndicateUtilitySettings = new(MultiMenu.syndicate, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Utility</color> Settings");

        Anarchist = new(MultiMenu.syndicate, "<color=#008000FF>Anarchist</color>");
        AnarchKillCooldown = new(num++, MultiMenu.syndicate, "Kill Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);

        Banshee = new(MultiMenu.syndicate, "<color=#E67E22FF>Banshee</color>", BansheeOn);
        ScreamCooldown = new(num++, MultiMenu.syndicate, "Scream Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, BansheeOn);
        ScreamDuration = new(num++, MultiMenu.syndicate, "Scream Duration", 10f, 5f, 30f, 1f, CooldownFormat, BansheeOn);

        ModifierSettings = new(MultiMenu.modifier, "<color=#7F7F7FFF>Modifier</color> Settings", new[] { AstralOn, BaitOn, CowardOn, DiseasedOn, DrunkOn, DwarfOn, GiantOn, ShyOn, VIPOn,
            IndomitableOn, ProfessionalOn, VolatileOn, YellerOn });
        MaxModifiers = new(num++, MultiMenu.modifier, "Max <color=#7F7F7FFF>Modifiers</color>", 5, 1, 14, 1, new[] { AstralOn, BaitOn, CowardOn, DiseasedOn, DrunkOn, DwarfOn, GiantOn,
            IndomitableOn, ShyOn, VIPOn, ProfessionalOn, VolatileOn, YellerOn });
        MinModifiers = new(num++, MultiMenu.modifier, "Min <color=#7F7F7FFF>Modifiers</color>", 5, 1, 14, 1, new[] { AstralOn, BaitOn, CowardOn, DiseasedOn, DrunkOn, DwarfOn, GiantOn,
            IndomitableOn, ShyOn, VIPOn, ProfessionalOn, VolatileOn, YellerOn });

        Astral = new(MultiMenu.modifier, "<color=#612BEFFF>Astral</color>", AstralOn);
        UniqueAstral = new(num++, MultiMenu.modifier, "<color=#612BEFFF>Astral</color> Is Unique", false, AstralOn);

        Bait = new(MultiMenu.modifier, "<color=#00B3B3FF>Bait</color>", BaitOn);
        UniqueBait = new(num++, MultiMenu.modifier, "<color=#00B3B3FF>Bait</color> Is Unique", false, BaitOn);
        BaitKnows = new(num++, MultiMenu.modifier, "<color=#00B3B3FF>Bait</color> Knows Who They Are", true, BaitOn);
        BaitMinDelay = new(num++, MultiMenu.modifier, "Minimum Delay for <color=#00B3B3FF>Bait</color> Self Report", 0f, 0f, 15f, 0.5f, CooldownFormat, BaitOn);
        BaitMaxDelay = new(num++, MultiMenu.modifier, "Maximum Delay for <color=#00B3B3FF>Bait</color> Self Report", 1f, 0f, 15f, 0.5f, CooldownFormat, BaitOn);

        Coward = new(MultiMenu.modifier, "<color=#456BA8FF>Coward</color>", CowardOn);
        UniqueCoward = new(num++, MultiMenu.modifier, "<color=#456BA8FF>Coward</color> Is Unique", false, CowardOn);

        Diseased = new(MultiMenu.modifier, "<color=#374D1EFF>Diseased</color>", DiseasedOn);
        UniqueDiseased = new(num++, MultiMenu.modifier, "<color=#374D1EFF>Diseased</color> Is Unique", false, DiseasedOn);
        DiseasedKnows = new(num++, MultiMenu.modifier, "<color=#374D1EFF>Diseased</color> Knows Who They Are", true, DiseasedOn);
        DiseasedKillMultiplier = new(num++, MultiMenu.modifier, "<color=#374D1EFF>Diseased</color> Kill Multiplier", 3f, 1.5f, 5f, 0.5f, MultiplierFormat, DiseasedOn);

        Drunk = new(MultiMenu.modifier, "<color=#758000FF>Drunk</color>", DrunkOn);
        UniqueDrunk = new(num++, MultiMenu.modifier, "<color=#758000FF>Drunk</color> Is Unique", false, DrunkOn);
        DrunkControlsSwap = new(num++, MultiMenu.modifier, "Controls Reverse Over Time", false, DrunkOn);
        DrunkKnows = new(num++, MultiMenu.modifier, "<color=#758000FF>Drunk</color> Knows Who They Are", true, DrunkOn);
        DrunkInterval = new(num++, MultiMenu.modifier, "Reversed Controls Interval", 10f, 1f, 20f, 1f, CooldownFormat, DrunkOn);

        Dwarf = new(MultiMenu.modifier, "<color=#FF8080FF>Dwarf</color>", DwarfOn);
        UniqueDwarf = new(num++, MultiMenu.modifier, "<color=#FF8080FF>Dwarf</color> Is Unique", false, DwarfOn);
        DwarfSpeed = new(num++, MultiMenu.modifier, "<color=#FF8080FF>Dwarf</color> Speed", 1.5f, 1f, 2f, 0.05f, MultiplierFormat, DwarfOn);
        DwarfScale = new(num++, MultiMenu.modifier, "<color=#FF8080FF>Dwarf</color> Scale", 0.5f, 0.3f, 1f, 0.025f, MultiplierFormat, DwarfOn);

        Giant = new(MultiMenu.modifier, "<color=#FFB34DFF>Giant</color>", GiantOn);
        UniqueGiant = new(num++, MultiMenu.modifier, "<color=#FFB34DFF>Giant</color> Is Unique", false, GiantOn);
        GiantSpeed = new(num++, MultiMenu.modifier, "<color=#FFB34DFF>Giant</color> Speed", 0.75f, 0.5f, 1f, 0.05f, MultiplierFormat, GiantOn);
        GiantScale = new(num++, MultiMenu.modifier, "<color=#FFB34DFF>Giant</color> Scale", 1.5f, 1f, 3.0f, 0.025f, MultiplierFormat, GiantOn);

        Indomitable = new(MultiMenu.modifier, "<color=#2DE5BEFF>Indomitable</color>", IndomitableOn);
        UniqueIndomitable = new(num++, MultiMenu.modifier, "<color=#2DE5BEFF>Indomitable</color> Is Unique", false, IndomitableOn);
        IndomitableKnows = new(num++, MultiMenu.modifier, "<color=#2DE5BEFF>Indomitable</color> Knows Who They Are", true, IndomitableOn);

        Professional = new(MultiMenu.modifier, "<color=#860B7AFF>Professional</color>", ProfessionalOn);
        UniqueProfessional = new(num++, MultiMenu.modifier, "<color=#860B7AFF>Professional</color> Is Unique", false, ProfessionalOn);
        ProfessionalKnows = new(num++, MultiMenu.modifier, "<color=#860B7AFF>Professional</color> Knows Who They Are", true, ProfessionalOn);

        Shy = new(MultiMenu.modifier, "<color=#1002C5FF>Shy</color>", ShyOn);
        UniqueShy = new(num++, MultiMenu.modifier, "<color=#1002C5FF>Shy</color> Is Unique", false, ShyOn);

        VIP = new(MultiMenu.modifier, "<color=#DCEE85FF>VIP</color>", VIPOn);
        UniqueVIP = new(num++, MultiMenu.modifier, "<color=#DCEE85FF>VIP</color> Is Unique", false, VIPOn);
        VIPKnows = new(num++, MultiMenu.modifier, "<color=#DCEE85FF>VIP</color> Knows Who They Are", true, VIPOn);

        Volatile = new(MultiMenu.modifier, "<color=#FFA60AFF>Volatile</color>", VolatileOn);
        UniqueVolatile = new(num++, MultiMenu.modifier, "<color=#FFA60AFF>Volatile</color> Is Unique", false, VolatileOn);
        VolatileInterval = new(num++, MultiMenu.modifier, "<color=#FFA60AFF>Volatile</color> Interval", 15f, 10f, 30f, 1f, CooldownFormat, VolatileOn);
        VolatileKnows = new(num++, MultiMenu.modifier, "<color=#FFA60AFF>Volatile</color> Knows Who They Are", true, VolatileOn);

        Yeller = new(MultiMenu.modifier, "<color=#F6AAB7FF>Yeller</color>", YellerOn);
        UniqueYeller = new(num++, MultiMenu.modifier, "<color=#F6AAB7FF>Yeller</color> Is Unique", false, YellerOn);

        AbilitySettings = new(MultiMenu.ability, "<color=#FF9900FF>Ability</color> Settings", new[] { CrewAssassinOn, NeutralAssassinOn, IntruderAssassinOn, SyndicateAssassinOn, NinjaOn,
            ButtonBarryOn, InsiderOn, MultitaskerOn, PoliticianOn, RadarOn, RuthlessOn, SnitchOn, SwapperOn, TiebreakerOn, TunnelerOn, UnderdogOn });
        MaxAbilities = new(num++, MultiMenu.ability, "Max <color=#FF9900FF>Abilities</color>", 5, 1, 14, 1, new[] { CrewAssassinOn, NeutralAssassinOn, IntruderAssassinOn, NinjaOn, RadarOn,
            SyndicateAssassinOn, ButtonBarryOn, InsiderOn, MultitaskerOn, PoliticianOn, RuthlessOn, SnitchOn, SwapperOn, TiebreakerOn, TunnelerOn, UnderdogOn });
        MinAbilities = new(num++, MultiMenu.ability, "Min <color=#FF9900FF>Abilities</color>", 5, 1, 14, 1, new[] { CrewAssassinOn, NeutralAssassinOn, IntruderAssassinOn, RadarOn, NinjaOn,
            SyndicateAssassinOn, ButtonBarryOn, InsiderOn, MultitaskerOn, PoliticianOn, RuthlessOn, SnitchOn, SwapperOn, TiebreakerOn, TunnelerOn, UnderdogOn });

        Assassin = new(MultiMenu.ability, "<color=#073763FF>Assassin</color>", new[] { CrewAssassinOn, NeutralAssassinOn, IntruderAssassinOn, SyndicateAssassinOn });
        UniqueCrewAssassin = new(num++, MultiMenu.ability, "<color=#8CFFFFFF>Crew</color> <color=#073763FF>Assassin</color> Is Unique", false, CrewAssassinOn);
        UniqueNeutralAssassin = new(num++, MultiMenu.ability, "<color=#B3B3B3FF>Neutral</color> <color=#073763FF>Assassin</color> Is Unique", false, NeutralAssassinOn);
        UniqueIntruderAssassin = new(num++, MultiMenu.ability, "<color=#FF0000FF>Intruder</color> <color=#073763FF>Assassin</color> Is Unique", false, IntruderAssassinOn);
        UniqueSyndicateAssassin = new(num++, MultiMenu.ability, "<color=#008000FF>Syndicate</color> <color=#073763FF>Assassin</color> Is Unique", false, SyndicateAssassinOn);
        AssassinKills = new(num++, MultiMenu.ability, "<color=#073763FF>Assassin</color> Guess Limit", 1, 1, 15, 1, new[] { CrewAssassinOn, NeutralAssassinOn, IntruderAssassinOn,
            SyndicateAssassinOn });
        AssassinMultiKill = new(num++, MultiMenu.ability, "<color=#073763FF>Assassin</color> Can Kill More Than Once Per Meeting", false, new[] { CrewAssassinOn, NeutralAssassinOn,
            IntruderAssassinOn, SyndicateAssassinOn });
        AssassinGuessNeutralBenign = new(num++, MultiMenu.ability, "<color=#073763FF>Assassin</color> Can Guess <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Benigns</color>",
            false, new[] { CrewAssassinOn, NeutralAssassinOn, IntruderAssassinOn, SyndicateAssassinOn });
        AssassinGuessNeutralEvil = new(num++, MultiMenu.ability, "<color=#073763FF>Assassin</color> Can Guess <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Evils</color>", false,
            new[] { CrewAssassinOn, NeutralAssassinOn, IntruderAssassinOn, SyndicateAssassinOn });
        AssassinGuessInvestigative = new(num++, MultiMenu.ability, "<color=#073763FF>Assassin</color> Can Guess <color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Investigatives</color>",
            false, new[] { CrewAssassinOn, NeutralAssassinOn, IntruderAssassinOn, SyndicateAssassinOn });
        AssassinGuessPest = new(num++, MultiMenu.ability, "<color=#073763FF>Assassin</color> Can Guess <color=#424242FF>Pestilence</color>", false, new[] { CrewAssassinOn,
            NeutralAssassinOn, IntruderAssassinOn, SyndicateAssassinOn });
        AssassinGuessModifiers = new(num++, MultiMenu.ability, "<color=#073763FF>Assassin</color> Can Guess Select <color=#7F7F7FFF>Modifiers</color>", false, new[] { CrewAssassinOn,
            NeutralAssassinOn, IntruderAssassinOn, SyndicateAssassinOn });
        AssassinGuessObjectifiers = new(num++, MultiMenu.ability, "<color=#073763FF>Assassin</color> Can Guess Select <color=#DD585BFF>Objectifiers</color>", false, new[] { CrewAssassinOn,
            NeutralAssassinOn, IntruderAssassinOn, SyndicateAssassinOn });
        AssassinGuessAbilities = new(num++, MultiMenu.ability, "<color=#073763FF>Assassin</color> Can Guess <color=#FF9900FF>Abilities</color>", false, new[] { CrewAssassinOn,
            NeutralAssassinOn, IntruderAssassinOn, SyndicateAssassinOn });
        AssassinateAfterVoting = new(num++, MultiMenu.ability, "<color=#073763FF>Assassin</color> Can Guess After Voting", false, new[] { CrewAssassinOn, NeutralAssassinOn,
            IntruderAssassinOn, SyndicateAssassinOn });

        ButtonBarry = new(MultiMenu.ability, "<color=#E600FFFF>Button Barry</color>", ButtonBarryOn);
        UniqueButtonBarry = new(num++, MultiMenu.ability, "<color=#E600FFFF>Button Barry</color> Is Unique", false, ButtonBarryOn);
        ButtonCooldown = new(num++, MultiMenu.ability, "Button Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, ButtonBarryOn);

        Insider = new(MultiMenu.ability, "<color=#26FCFBFF>Insider</color>", InsiderOn);
        UniqueInsider = new(num++, MultiMenu.ability, "<color=#26FCFBFF>Insider</color> Is Unique", false, InsiderOn);
        InsiderKnows = new(num++, MultiMenu.ability, "<color=#26FCFBFF>Insider</color> Knows Who They Are", true, InsiderOn);

        Multitasker = new(MultiMenu.ability, "<color=#FF804DFF>Multitasker</color>", MultitaskerOn);
        UniqueMultitasker = new(num++, MultiMenu.ability, "<color=#FF804DFF>Multitasker</color> Is Unique", false, MultitaskerOn);
        Transparancy = new(num++, MultiMenu.ability, "Task Transparancy", 50f, 10f, 80f, 5f, PercentFormat, MultitaskerOn);

        Ninja = new(MultiMenu.ability, "<color=#A84300FF>Ninja</color>", NinjaOn);
        UniqueNinja = new(num++, MultiMenu.ability, "<color=#A84300FF>Ninja</color> Is Unique", false, NinjaOn);

        Politician = new(MultiMenu.ability, "<color=#CCA3CCFF>Politician</color>", PoliticianOn);
        UniquePolitician = new(num++, MultiMenu.ability, "<color=#CCA3CCFF>Politician</color> Is Unique", false, PoliticianOn);
        PoliticianVoteBank = new(num++, MultiMenu.ability, "Initial <color=#CCA3CCFF>Politician</color> Initial Vote Bank", 0, 0, 10, 1, PoliticianOn);
        PoliticianAnonymous = new(num++, MultiMenu.ability, "Anonymous <color=#CCA3CCFF>Politician</color> Votes", false, PoliticianOn);
        PoliticianButton = new(num++, MultiMenu.ability, "<color=#CCA3CCFF>Politician</color> Can Button", true, PoliticianOn);

        Radar = new(MultiMenu.ability, "<color=#FF0080FF>Radar</color>", RadarOn);
        UniqueRadar = new(num++, MultiMenu.ability, "<color=#FF0080FF>Radar</color> Is Unique", false, RadarOn);

        Ruthless = new(MultiMenu.ability, "<color=#2160DDFF>Ruthless</color>", RuthlessOn);
        UniqueRuthless = new(num++, MultiMenu.ability, "<color=#2160DDFF>Ruthless</color> Is Unique", false, RuthlessOn);
        RuthlessKnows = new(num++, MultiMenu.ability, "<color=#2160DDFF>Ruthless</color> Knows Who They Are", true, RuthlessOn);

        Snitch = new(MultiMenu.ability, "<color=#D4AF37FF>Snitch</color>", SnitchOn);
        UniqueSnitch = new(num++, MultiMenu.ability, "<color=#D4AF37FF>Snitch</color> Is Unique", false, SnitchOn);
        SnitchKnows = new(num++, MultiMenu.ability, "<color=#D4AF37FF>Snitch</color> Knows Who They Are", true, SnitchOn);
        SnitchSeesNeutrals = new(num++, MultiMenu.ability, "<color=#D4AF37FF>Snitch</color> Sees <color=#B3B3B3FF>Neutrals</color>", false, SnitchOn);
        SnitchSeesCrew = new(num++, MultiMenu.ability, "<color=#D4AF37FF>Snitch</color> Sees <color=#8CFFFFFF>Crew</color>", false, SnitchOn);
        SnitchSeesRoles = new(num++, MultiMenu.ability, "<color=#D4AF37FF>Snitch</color> Sees Exact <color=#FFD700FF>Roles</color>", false, SnitchOn);
        SnitchTasksRemaining = new(num++, MultiMenu.ability, "Tasks Remaining When Revealed", 1, 1, 5, 1, SnitchOn);
        SnitchSeestargetsInMeeting = new(num++, MultiMenu.ability, "<color=#D4AF37FF>Snitch</color> Sees Evils In Meetings", true, SnitchOn);

        Swapper = new(MultiMenu.ability, "<color=#66E666FF>Swapper</color>", SwapperOn);
        UniqueSwapper = new(num++, MultiMenu.ability, "<color=#66E666FF>Swapper</color> Is Unique", false, SwapperOn);
        SwapperButton = new(num++, MultiMenu.ability, "<color=#66E666FF>Swapper</color> Can Button", true, SwapperOn);
        SwapAfterVoting = new(num++, MultiMenu.ability, "<color=#66E666FF>Swapper</color> Can Swap After Voting", false, SwapperOn);
        SwapSelf = new(num++, MultiMenu.ability, "<color=#66E666FF>Swapper</color> Can Swap Themself", true, SwapperOn);

        Tiebreaker = new(MultiMenu.ability, "<color=#99E699FF>Tiebreaker</color>", TiebreakerOn);
        UniqueTiebreaker = new(num++, MultiMenu.ability, "<color=#99E699FF>Tiebreaker</color> Is Unique", false, TiebreakerOn);
        TiebreakerKnows = new(num++, MultiMenu.ability, "<color=#99E699FF>Tiebreaker</color> Knows Who They Are", true, TiebreakerOn);

        Torch = new(MultiMenu.ability, "<color=#FFFF99FF>Torch</color>", TorchOn);
        UniqueTorch = new(num++, MultiMenu.ability, "<color=#FFFF99FF>Torch</color> Is Unique", false, TorchOn);

        Tunneler = new(MultiMenu.ability, "<color=#E91E63FF>Tunneler</color>", TunnelerOn);
        UniqueTunneler = new(num++, MultiMenu.ability, "<color=#E91E63FF>Tunneler</color> Is Unique", false, TunnelerOn);
        TunnelerKnows = new(num++, MultiMenu.ability, "<color=#E91E63FF>Tunneler</color> Knows Who They Are", true, TunnelerOn);

        Underdog = new(MultiMenu.ability, "<color=#841A7FFF>Underdog</color>", UnderdogOn);
        UniqueUnderdog = new(num++, MultiMenu.ability, "<color=#841A7FFF>Underdog</color> Is Unique", false, UnderdogOn);
        UnderdogKnows = new(num++, MultiMenu.ability, "<color=#841A7FFF>Underdog</color> Knows Who They Are", true, UnderdogOn);
        UnderdogKillBonus = new(num++, MultiMenu.ability, "Kill Cooldown Bonus", 5f, 2.5f, 30f, 2.5f, CooldownFormat, UnderdogOn);
        UnderdogIncreasedKC = new(num++, MultiMenu.ability, "Increased Kill Cooldown When 2+ Teammates", true, UnderdogOn);

        ObjectifierSettings = new(MultiMenu.objectifier, "<color=#DD585BFF>Objectifier</color> Settings", new[] { AlliedOn, CorruptedOn, DefectorOn, FanaticOn, LinkedOn, LoversOn, MafiaOn,
            RivalsOn, OverlordOn, TaskmasterOn, TraitorOn });
        MaxObjectifiers = new(num++, MultiMenu.objectifier, "Max <color=#DD585BFF>Objectifiers</color>", 5, 1, 14, 1, new[] { AlliedOn, CorruptedOn, DefectorOn, FanaticOn, LinkedOn,
            LoversOn, RivalsOn, MafiaOn, OverlordOn, TaskmasterOn, TraitorOn });
        MinObjectifiers = new(num++, MultiMenu.objectifier, "Min <color=#DD585BFF>Objectifiers</color>", 5, 1, 14, 1, new[] { AlliedOn, CorruptedOn, DefectorOn, FanaticOn, LinkedOn,
            LoversOn, RivalsOn, MafiaOn, OverlordOn, TaskmasterOn, TraitorOn });

        Allied = new(MultiMenu.objectifier, "<color=#4545A9FF>Allied</color>", AlliedOn);
        UniqueAllied = new(num++, MultiMenu.objectifier, "<color=#4545A9FF>Allied</color> Is Unique", false, AlliedOn);
        AlliedFaction = new(num++, MultiMenu.objectifier, "<color=#4545A9FF>Allied</color> Faction", new[] { "Random", "Intruder", "Syndicate", "Crew" }, AlliedOn);

        Corrupted = new(MultiMenu.objectifier, "<color=#4545FFFF>Corrupted</color>", CorruptedOn);
        UniqueCorrupted = new(num++, MultiMenu.objectifier, "<color=#4545FFFF>Corrupted</color> Is Unique", false, CorruptedOn);
        CorruptedKillCooldown = new(num++, MultiMenu.objectifier, "Corrupt Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, CorruptedOn);
        AllCorruptedWin = new(num++, MultiMenu.objectifier, "All <color=#4545FFFF>Corrupted</color> Win Together", false, CorruptedOn);
        CorruptedVent = new(num++, MultiMenu.objectifier, "<color=#4545FFFF>Corrupted</color> Can Vent", false, CorruptedOn);

        Defector = new(MultiMenu.objectifier, "<color=#E1C849FF>Defector</color>", DefectorOn);
        UniqueDefector = new(num++, MultiMenu.objectifier, "<color=#E1C849FF>Defector</color> Is Unique", false, DefectorOn);
        DefectorKnows = new(num++, MultiMenu.objectifier, "<color=#E1C849FF>Defector</color> Knows Who They Are", true, DefectorOn);
        DefectorFaction = new(num++, MultiMenu.objectifier, "<color=#E1C849FF>Defector</color> Faction", new[] { "Random", "Opposing Evil", "Crew" }, DefectorOn);

        Fanatic = new(MultiMenu.objectifier, "<color=#678D36FF>Fanatic</color>", FanaticOn);
        UniqueFanatic = new(num++, MultiMenu.objectifier, "<color=#678D36FF>Fanatic</color> Is Unique", false, FanaticOn);
        FanaticKnows = new(num++, MultiMenu.objectifier, "<color=#678D36FF>Fanatic</color> Knows Who They Are", true, FanaticOn);
        FanaticColourSwap = new(num++, MultiMenu.objectifier, "Turned <color=#678D36FF>Fanatic</color> Swaps Colours For Investigative <color=#FFD700FF>Roles</color>", false,
            FanaticOn);
        SnitchSeesFanatic = new(num++, MultiMenu.objectifier, "<color=#D4AF37FF>Snitch</color> Sees Turned <color=#678D36FF>Fanatic</color>", true, new[] { FanaticOn, SnitchOn }, true);
        RevealerRevealsFanatic = new(num++, MultiMenu.objectifier, "<color=#D3D3D3FF>Revealer</color> Reveals Turned <color=#678D36FF>Fanatic</color>", false, new[] { FanaticOn, RevealerOn
            }, true);

        Linked = new(MultiMenu.objectifier, "<color=#FF351FFF>Linked</color>", LinkedOn);
        UniqueLinked = new(num++, MultiMenu.objectifier, "<color=#FF351FFF>Linked</color> Is Unique", false, LinkedOn);
        LinkedChat = new(num++, MultiMenu.objectifier, "Enable <color=#FF351FFF>Linked</color> Chat", true, LinkedOn);
        LinkedRoles = new(num++, MultiMenu.objectifier, "<color=#FF351FFF>Linked</color> Know Each Other's <color=#FFD700FF>Roles</color>", true, LinkedOn);

        Lovers = new(MultiMenu.objectifier, "<color=#FF66CCFF>Lovers</color>", LoversOn);
        UniqueLovers = new(num++, MultiMenu.objectifier, "<color=#FF66CCFF>Lovers</color> Is Unique", false, LoversOn);
        BothLoversDie = new(num++, MultiMenu.objectifier, "Both <color=#FF66CCFF>Lovers</color> Die", true, LoversOn);
        LoversChat = new(num++, MultiMenu.objectifier, "Enable <color=#FF66CCFF>Lovers</color> Chat", true, LoversOn);
        LoversRoles = new(num++, MultiMenu.objectifier, "<color=#FF66CCFF>Lovers</color> Know Each Other's <color=#FFD700FF>Roles</color>", true, LoversOn);

        Mafia = new(MultiMenu.objectifier, "<color=#00EEFFFF>Mafia</color>", MafiaOn);
        UniqueMafia = new(num++, MultiMenu.objectifier, "<color=#00EEFFFF>Mafia</color> Is Unique", false, MafiaOn);
        MafiaRoles = new(num++, MultiMenu.objectifier, "<color=#00EEFFFF>Mafia</color> Know Each Other's <color=#FFD700FF>Roles</color>", true, MafiaOn);
        MafVent = new(num++, MultiMenu.objectifier, "<color=#00EEFFFF>Mafia</color> Can Vent", false, MafiaOn);

        Overlord = new(MultiMenu.objectifier, "<color=#008080FF>Overlord</color>", OverlordOn);
        UniqueOverlord = new(num++, MultiMenu.objectifier, "<color=#008080FF>Overlord</color> Is Unique", false, OverlordOn);
        OverlordKnows = new(num++, MultiMenu.objectifier, "<color=#008080FF>Overlord</color> Knows Who They Are", true, OverlordOn);
        OverlordMeetingWinCount = new(num++, MultiMenu.objectifier, "<color=#008080FF>Overlord</color> Meeting Timer", 2, 1, 20, 1, OverlordOn);

        Rivals = new(MultiMenu.objectifier, "<color=#3D2D2CFF>Rivals</color>", RivalsOn);
        UniqueRivals = new(num++, MultiMenu.objectifier, "<color=#3D2D2CFF>Rivals</color> Is Unique", false, RivalsOn);
        RivalsChat = new(num++, MultiMenu.objectifier, "Enable <color=#3D2D2CFF>Rivals</color> Chat", true, RivalsOn);
        RivalsRoles = new(num++, MultiMenu.objectifier, "<color=#3D2D2CFF>Rivals</color> Know Each Other's <color=#FFD700FF>Roles</color>", true, RivalsOn);

        Taskmaster = new(MultiMenu.objectifier, "<color=#ABABFFFF>Taskmaster</color>", TaskmasterOn);
        UniqueTaskmaster = new(num++, MultiMenu.objectifier, "<color=#ABABFFFF>Taskmaster</color> Is Unique", false, TaskmasterOn);
        TMTasksRemaining = new(num++, MultiMenu.objectifier, "Tasks Remaining When Revealed", 1, 1, 5, 1, TaskmasterOn);

        Traitor = new(MultiMenu.objectifier, "<color=#370D43FF>Traitor</color>", TraitorOn);
        UniqueTraitor = new(num++, MultiMenu.objectifier, "<color=#370D43FF>Traitor</color> Is Unique", false, TraitorOn);
        TraitorKnows = new(num++, MultiMenu.objectifier, "<color=#370D43FF>Traitor</color> Knows Who They Are", true, TraitorOn);
        SnitchSeesTraitor = new(num++, MultiMenu.objectifier, "<color=#D4AF37FF>Snitch</color> Sees Turned <color=#370D43FF>Traitor</color>", true, new[] { TraitorOn, SnitchOn }, true);
        RevealerRevealsTraitor = new(num++, MultiMenu.objectifier, "<color=#D3D3D3FF>Revealer</color> Reveals Turned <color=#370D43FF>Traitor</color>", false, new[] { TraitorOn, RevealerOn
            }, true);
        TraitorColourSwap = new(num++, MultiMenu.objectifier, "Turned <color=#370D43FF>Traitor</color> Swaps Colours For Investigative <color=#FFD700FF>Roles</color>", false, TraitorOn);

        RoleList = new(MultiMenu.rolelist, "Role List Entries");
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

        BanList = new(MultiMenu.rolelist, "Role List Bans");
        Ban1 = new(num++, "Ban");
        Ban2 = new(num++, "Ban");
        Ban3 = new(num++, "Ban");
        Ban4 = new(num++, "Ban");
        Ban5 = new(num++, "Ban");

        CustomOption.SaveSettings("DefaultSettings");
    }
}