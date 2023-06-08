namespace TownOfUsReworked.CustomOptions
{
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
        public static CustomNumberOption DiscussionTime;
        public static CustomNumberOption VotingTime;
        public static CustomToggleOption ConfirmEjects;
        public static CustomToggleOption EjectionRevealsRole;
        public static CustomStringOption TaskBarMode;
        public static CustomNumberOption ReportDistance;
        public static CustomNumberOption ChatCooldown;
        public static CustomNumberOption LobbySize;

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
        private static readonly string[] Maps = { "Skeld", "Mira HQ", "Polus", /*"dlekS",*/ "Airship" };

        //Game Modifier Options
        public static CustomHeaderOption GameModifiers;
        public static CustomToggleOption AnonymousVoting;
        public static CustomStringOption WhoCanVent;
        public static CustomStringOption SkipButtonDisable;
        public static CustomToggleOption FactionSeeRoles;
        public static CustomToggleOption ParallelMedScans;
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
        public static CustomToggleOption DisableLevels;
        public static CustomToggleOption WhiteNameplates;
        public static CustomToggleOption CustomEject;
        public static CustomToggleOption LighterDarker;
        public static CustomToggleOption ObstructNames;

        //Better Sabotages
        public static CustomHeaderOption BetterSabotages;
        public static CustomToggleOption OxySlow;
        public static CustomNumberOption ReactorShake;
        public static CustomToggleOption ColourblindComms;
        public static CustomToggleOption MeetingColourblind;
        public static CustomToggleOption NightVision;
        public static CustomToggleOption EvilsIgnoreNV;

        //Better Skeld Options
        public static CustomToggleOption SkeldVentImprovements;
        public static CustomHeaderOption BetterSkeld;

        //Better Airship Options
        public static CustomHeaderOption BetterAirshipSettings;
        public static CustomStringOption SpawnType;
        public static CustomStringOption MoveAdmin;
        public static CustomStringOption MoveElectrical;
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
        public static CustomHeaderOption AllAnySettings;
        public static CustomToggleOption EnableUniques;

        //CI Role Spawn
        public static CustomHeaderOption CrewInvestigativeRoles;
        public static CustomNumberOption DetectiveOn;
        public static CustomNumberOption CoronerOn;
        public static CustomNumberOption SheriffOn;
        public static CustomNumberOption MediumOn;
        public static CustomNumberOption TrackerOn;
        public static CustomNumberOption InspectorOn;
        public static CustomNumberOption OperativeOn;
        public static CustomNumberOption SeerOn;

        //CSv Role Spawn
        public static CustomHeaderOption CrewSovereignRoles;
        public static CustomNumberOption MayorOn;
        public static CustomNumberOption DictatorOn;
        public static CustomNumberOption MonarchOn;

        //CP Role Spawn
        public static CustomHeaderOption CrewProtectiveRoles;
        public static CustomNumberOption AltruistOn;
        public static CustomNumberOption MedicOn;

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

        //NH Role Spawn
        public static CustomHeaderOption NeutralHarbingerRoles;
        public static CustomNumberOption PlaguebearerOn;

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
        public static CustomNumberOption JanitorOn;

        //ID Role Spawn
        public static CustomHeaderOption IntruderDeceptionRoles;
        public static CustomNumberOption MorphlingOn;
        public static CustomNumberOption DisguiserOn;
        public static CustomNumberOption WraithOn;

        //IK Role Spawn
        public static CustomHeaderOption IntruderKillingRoles;
        public static CustomNumberOption AmbusherOn;
        public static CustomNumberOption EnforcerOn;

        //IS Role Spawn
        public static CustomHeaderOption IntruderSupportRoles;
        public static CustomNumberOption ConsigliereOn;
        public static CustomNumberOption GodfatherOn;
        public static CustomNumberOption ConsortOn;
        public static CustomNumberOption MinerOn;
        public static CustomNumberOption TeleporterOn;

        //IU Role Spawn
        public static CustomHeaderOption IntruderUtilityRoles;
        public static CustomNumberOption ImpostorOn;
        public static CustomNumberOption GhoulOn;

        //SSu Role Spawn
        public static CustomHeaderOption SyndicateSupportRoles;
        public static CustomNumberOption WarperOn;
        public static CustomNumberOption RebelOn;
        public static CustomNumberOption StalkerOn;

        //SD Role Spawn
        public static CustomHeaderOption SyndicateDisruptionRoles;
        public static CustomNumberOption FramerOn;
        public static CustomNumberOption ShapeshifterOn;
        public static CustomNumberOption ConcealerOn;
        public static CustomNumberOption DrunkardOn;
        public static CustomNumberOption SilencerOn;

        //SyK Role Spawn
        public static CustomHeaderOption SyndicatePowerRoles;
        public static CustomNumberOption SpellslingerOn;
        public static CustomNumberOption TimeKeeperOn;

        //SyK Role Spawn
        public static CustomHeaderOption SyndicateKillingRoles;
        public static CustomNumberOption BomberOn;
        public static CustomNumberOption CrusaderOn;
        public static CustomNumberOption ColliderOn;
        public static CustomNumberOption PoisonerOn;

        //SU Role Spawn
        public static CustomHeaderOption SyndicateUtilityRoles;
        public static CustomNumberOption AnarchistOn;
        public static CustomNumberOption BansheeOn;

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
        public static CustomNumberOption AstralOn;
        public static CustomNumberOption VolatileOn;
        public static CustomNumberOption ProfessionalOn;
        public static CustomNumberOption YellerOn;

        //Ability Spawn
        public static CustomHeaderOption Abilities;
        public static CustomNumberOption CrewAssassinOn;
        public static CustomNumberOption IntruderAssassinOn;
        public static CustomNumberOption SyndicateAssassinOn;
        public static CustomNumberOption NeutralAssassinOn;
        public static CustomNumberOption TorchOn;
        public static CustomNumberOption ButtonBarryOn;
        public static CustomNumberOption TiebreakerOn;
        public static CustomNumberOption TunnelerOn;
        public static CustomNumberOption UnderdogOn;
        public static CustomNumberOption SnitchOn;
        public static CustomNumberOption RadarOn;
        public static CustomNumberOption InsiderOn;
        public static CustomNumberOption MultitaskerOn;
        public static CustomNumberOption RuthlessOn;
        public static CustomNumberOption NinjaOn;
        public static CustomNumberOption SwapperOn;
        public static CustomNumberOption PoliticianOn;

        //Objectifier Spawn
        public static CustomHeaderOption Objectifiers;
        public static CustomNumberOption LoversOn;
        public static CustomNumberOption AlliedOn;
        public static CustomNumberOption MafiaOn;
        public static CustomNumberOption TraitorOn;
        public static CustomNumberOption RivalsOn;
        public static CustomNumberOption FanaticOn;
        public static CustomNumberOption TaskmasterOn;
        public static CustomNumberOption OverlordOn;
        public static CustomNumberOption CorruptedOn;
        public static CustomNumberOption DefectorOn;

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
        public static CustomToggleOption CrewFlashlight;

        //CSv Options
        public static CustomHeaderOption CrewSovereignSettings;
        public static CustomNumberOption CSvMax;
        public static CustomNumberOption CSvMin;

        //Mayor Options
        public static CustomHeaderOption Mayor;
        public static CustomNumberOption MayorCount;
        public static CustomToggleOption UniqueMayor;
        public static CustomNumberOption MayorVoteCount;
        public static CustomToggleOption RoundOneNoReveal;
        public static CustomToggleOption MayorButton;

        //Dictator Options
        public static CustomHeaderOption Dictator;
        public static CustomNumberOption DictatorCount;
        public static CustomToggleOption UniqueDictator;
        public static CustomToggleOption RoundOneNoDictReveal;
        public static CustomToggleOption DictatorButton;
        public static CustomToggleOption DictateAfterVoting;

        //Monarch Options
        public static CustomHeaderOption Monarch;
        public static CustomNumberOption MonarchCount;
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
        public static CustomNumberOption CAMin;

        //Mystic Options
        public static CustomHeaderOption MysticSettings;
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
        public static CustomNumberOption VigiBulletCount;

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
        public static CustomNumberOption FixCooldown;

        //Transporter Options
        public static CustomHeaderOption Transporter;
        public static CustomNumberOption TransporterCount;
        public static CustomToggleOption UniqueTransporter;
        public static CustomToggleOption TransSelf;
        public static CustomNumberOption TransportCooldown;
        public static CustomNumberOption TransportDuration;
        public static CustomNumberOption TransportMaxUses;

        //Retributionist Options
        public static CustomHeaderOption Retributionist;
        public static CustomToggleOption UniqueRetributionist;
        public static CustomNumberOption RetributionistCount;
        public static CustomToggleOption ReviveAfterVoting;
        public static CustomNumberOption MaxUses;

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
        public static CustomNumberOption SwoopCount;
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
        public static CustomStringOption WhoSeesDead;

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
        public static CustomNumberOption FootprintInterval;
        public static CustomNumberOption FootprintDuration;
        public static CustomToggleOption AnonymousFootPrint;

        //Coroner Options
        public static CustomHeaderOption Coroner;
        public static CustomNumberOption CoronerCount;
        public static CustomToggleOption UniqueCoroner;
        public static CustomNumberOption CoronerArrowDuration;
        public static CustomToggleOption CoronerReportName;
        public static CustomToggleOption CoronerReportRole;
        public static CustomNumberOption CoronerKillerNameTime;
        public static CustomNumberOption CompareCooldown;
        public static CustomNumberOption AutopsyCooldown;
        public static CustomNumberOption CompareLimit;

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
        public static CustomStringOption ShowMediumToDead;
        public static CustomStringOption DeadRevealed;

        //Sheriff Options
        public static CustomHeaderOption Sheriff;
        public static CustomNumberOption SheriffCount;
        public static CustomToggleOption UniqueSheriff;
        public static CustomNumberOption InterrogateCooldown;
        public static CustomToggleOption NeutEvilRed;
        public static CustomToggleOption NeutKillingRed;

        //CP Options
        public static CustomHeaderOption CrewProtectiveSettings;
        public static CustomNumberOption CPMax;
        public static CustomNumberOption CPMin;

        //Altruist Options
        public static CustomHeaderOption Altruist;
        public static CustomNumberOption AltruistCount;
        public static CustomToggleOption UniqueAltruist;
        public static CustomNumberOption AltReviveDuration;
        public static CustomToggleOption AltruistTargetBody;
        public static CustomNumberOption ReviveCooldown;
        public static CustomNumberOption ReviveCount;

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
        public static CustomToggleOption GhostsCanSabotage;
        public static CustomToggleOption IntruderFlashlight;

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
        public static CustomNumberOption DragCooldown;
        public static CustomStringOption JanitorVentOptions;
        public static CustomNumberOption DragModifier;

        //Blackmailer Options
        public static CustomHeaderOption Blackmailer;
        public static CustomNumberOption BlackmailerCount;
        public static CustomToggleOption UniqueBlackmailer;
        public static CustomNumberOption BlackmailCooldown;
        public static CustomToggleOption WhispersNotPrivate;
        public static CustomToggleOption BlackmailMates;
        public static CustomToggleOption BMRevealed;

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
        public static CustomToggleOption MorphCooldownsLinked;
        public static CustomNumberOption SampleCooldown;

        //Disguiser Options
        public static CustomHeaderOption Disguiser;
        public static CustomNumberOption DisguiserCount;
        public static CustomToggleOption UniqueDisguiser;
        public static CustomNumberOption DisguiseCooldown;
        public static CustomNumberOption TimeToDisguise;
        public static CustomNumberOption DisguiseDuration;
        public static CustomStringOption DisguiseTarget;
        public static CustomToggleOption DisgCooldownsLinked;
        public static CustomNumberOption MeasureCooldown;

        //Wraith Options
        public static CustomHeaderOption Wraith;
        public static CustomNumberOption WraithCount;
        public static CustomToggleOption UniqueWraith;
        public static CustomNumberOption InvisCooldown;
        public static CustomNumberOption InvisDuration;
        public static CustomToggleOption WraithVent;

        //IS Options
        public static CustomHeaderOption IntruderSupportSettings;
        public static CustomNumberOption ISMax;
        public static CustomNumberOption ISMin;

        //Teleporter Options
        public static CustomHeaderOption Teleporter;
        public static CustomNumberOption TeleporterCount;
        public static CustomToggleOption UniqueTeleporter;
        public static CustomNumberOption TeleportCd;
        public static CustomNumberOption MarkCooldown;
        public static CustomToggleOption TeleVent;
        public static CustomToggleOption TeleCooldownsLinked;

        //Consigliere Options
        public static CustomHeaderOption Consigliere;
        public static CustomNumberOption ConsigCount;
        public static CustomToggleOption UniqueConsigliere;
        public static CustomNumberOption InvestigateCooldown;
        public static CustomStringOption ConsigInfo;

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

        //Miner Options
        public static CustomHeaderOption Miner;
        public static CustomNumberOption MinerCount;
        public static CustomToggleOption UniqueMiner;
        public static CustomNumberOption MineCooldown;

        //IK Options
        public static CustomHeaderOption IntruderKillingSettings;
        public static CustomNumberOption IKMax;
        public static CustomNumberOption IKMin;

        //Ambusher Options
        public static CustomHeaderOption Ambusher;
        public static CustomNumberOption AmbusherCount;
        public static CustomToggleOption UniqueAmbusher;
        public static CustomNumberOption AmbushCooldown;
        public static CustomNumberOption AmbushDuration;
        public static CustomToggleOption AmbushMates;

        //Enforcer Options
        public static CustomHeaderOption Enforcer;
        public static CustomNumberOption EnforcerCount;
        public static CustomToggleOption UniqueEnforcer;
        public static CustomNumberOption EnforceCooldown;
        public static CustomNumberOption EnforceDuration;
        public static CustomNumberOption EnforceDelay;
        public static CustomNumberOption EnforceRadius;

        //IU Options
        public static CustomHeaderOption IntruderUtilitySettings;

        //Impostor Options
        public static CustomHeaderOption Impostor;
        public static CustomNumberOption ImpCount;

        //Ghoul Options
        public static CustomHeaderOption Ghoul;
        public static CustomNumberOption GhoulMarkCd;

        //Syndicate Options
        public static CustomHeaderOption SyndicateSettings;
        public static CustomToggleOption CustomSynColors;
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
        public static CustomNumberOption SDMin;

        //Shapeshifter Options
        public static CustomHeaderOption Shapeshifter;
        public static CustomNumberOption ShapeshifterCount;
        public static CustomToggleOption UniqueShapeshifter;
        public static CustomNumberOption ShapeshiftCooldown;
        public static CustomNumberOption ShapeshiftDuration;
        public static CustomToggleOption ShapeshiftMates;

        //Drunkard Options
        public static CustomHeaderOption Drunkard;
        public static CustomNumberOption DrunkardCount;
        public static CustomToggleOption UniqueDrunkard;
        public static CustomNumberOption ConfuseCooldown;
        public static CustomNumberOption ConfuseDuration;
        public static CustomToggleOption ConfuseImmunity;

        //Concealer Options
        public static CustomHeaderOption Concealer;
        public static CustomNumberOption ConcealerCount;
        public static CustomToggleOption UniqueConcealer;
        public static CustomNumberOption ConcealCooldown;
        public static CustomNumberOption ConcealDuration;
        public static CustomToggleOption ConcealMates;

        //Silencer Options
        public static CustomHeaderOption Silencer;
        public static CustomNumberOption SilencerCount;
        public static CustomToggleOption UniqueSilencer;
        public static CustomNumberOption SilenceCooldown;
        public static CustomToggleOption WhispersNotPrivateSilencer;
        public static CustomToggleOption SilenceMates;
        public static CustomToggleOption SilenceRevealed;

        //Framer Options
        public static CustomHeaderOption Framer;
        public static CustomNumberOption FrameCooldown;
        public static CustomNumberOption ChaosDriveFrameRadius;
        public static CustomNumberOption FramerCount;
        public static CustomToggleOption UniqueFramer;

        //SyK Options
        public static CustomHeaderOption SyndicateKillingSettings;
        public static CustomNumberOption SyKMax;
        public static CustomNumberOption SyKMin;

        //Crusader Options
        public static CustomHeaderOption Crusader;
        public static CustomNumberOption CrusaderCount;
        public static CustomToggleOption UniqueCrusader;
        public static CustomNumberOption CrusadeCooldown;
        public static CustomNumberOption CrusadeDuration;
        public static CustomNumberOption ChaosDriveCrusadeRadius;
        public static CustomToggleOption CrusadeMates;

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
        public static CustomNumberOption ChaosDriveBombRange;
        public static CustomToggleOption BombKillsSyndicate;

        //Poisoner Options
        public static CustomHeaderOption Poisoner;
        public static CustomNumberOption PoisonerCount;
        public static CustomToggleOption UniquePoisoner;
        public static CustomNumberOption PoisonCooldown;
        public static CustomNumberOption PoisonDuration;

        //Collider Options
        public static CustomHeaderOption Collider;
        public static CustomNumberOption ColliderCount;
        public static CustomToggleOption UniqueCollider;
        public static CustomNumberOption CollideCooldown;
        public static CustomNumberOption CollideRange;
        public static CustomNumberOption CollideRangeIncrease;

        //SSu Options
        public static CustomHeaderOption SyndicateSupportSettings;
        public static CustomNumberOption SSuMax;
        public static CustomNumberOption SSuMin;

        //Rebel Options
        public static CustomHeaderOption Rebel;
        public static CustomNumberOption RebelCount;
        public static CustomToggleOption UniqueRebel;
        public static CustomNumberOption SidekickAbilityCooldownDecrease;

        //Stalker Options
        public static CustomHeaderOption Stalker;
        public static CustomNumberOption StalkerCount;
        public static CustomToggleOption UniqueStalker;
        public static CustomNumberOption StalkCooldown;

        //Warper Options
        public static CustomHeaderOption Warper;
        public static CustomNumberOption WarperCount;
        public static CustomNumberOption WarpCooldown;
        public static CustomNumberOption WarpDuration;
        public static CustomToggleOption UniqueWarper;
        public static CustomToggleOption WarpSelf;

        //SU Options
        public static CustomHeaderOption SyndicateUtilitySettings;

        //Anarchist Options
        public static CustomHeaderOption Anarchist;
        public static CustomNumberOption AnarchistCount;
        public static CustomNumberOption AnarchKillCooldown;

        //Banshee Options
        public static CustomHeaderOption Banshee;
        public static CustomNumberOption ScreamCooldown;
        public static CustomNumberOption ScreamDuration;

        //SP Options
        public static CustomHeaderOption SyndicatePowerSettings;
        public static CustomNumberOption SPMax;
        public static CustomNumberOption SPMin;

        //Spellslinger Options
        public static CustomHeaderOption Spellslinger;
        public static CustomNumberOption SpellCooldown;
        public static CustomNumberOption SpellCooldownIncrease;
        public static CustomNumberOption SpellslingerCount;
        public static CustomToggleOption UniqueSpellslinger;

        //Time Keeper Options
        public static CustomHeaderOption TimeKeeper;
        public static CustomNumberOption TimeKeeperCount;
        public static CustomToggleOption UniqueTimeKeeper;
        public static CustomNumberOption TimeControlCooldown;
        public static CustomNumberOption TimeControlDuration;
        public static CustomToggleOption TimeFreezeImmunity;
        public static CustomToggleOption TimeRewindImmunity;

        //Neutral Options
        public static CustomHeaderOption NeutralSettings;
        public static CustomToggleOption CustomNeutColors;
        public static CustomNumberOption NeutralVision;
        public static CustomToggleOption LightsAffectNeutrals;
        public static CustomStringOption NoSolo;
        public static CustomNumberOption NeutralMax;
        public static CustomNumberOption NeutralMin;
        public static CustomToggleOption NeutralsVent;
        public static CustomToggleOption NeutralEvilsEndGame;
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
        public static CustomNumberOption NBMin;
        public static CustomToggleOption VigiKillsNB;

        //Amnesiac Options
        public static CustomHeaderOption Amnesiac;
        public static CustomNumberOption AmnesiacCount;
        public static CustomToggleOption RememberArrows;
        public static CustomNumberOption RememberArrowDelay;
        public static CustomToggleOption AmneVent;
        public static CustomToggleOption AmneSwitchVent;
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
        public static CustomNumberOption HauntCooldown;
        public static CustomNumberOption HauntCount;
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
        public static CustomNumberOption DoomCooldown;
        public static CustomNumberOption DoomCount;

        //Bounty Hunter Options
        public static CustomHeaderOption BountyHunter;
        public static CustomNumberOption BHCount;
        public static CustomToggleOption BHVent;
        public static CustomNumberOption BountyHunterCooldown;
        public static CustomNumberOption BountyHunterGuesses;
        public static CustomToggleOption UniqueBountyHunter;
        public static CustomToggleOption VigiKillsBH;

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

        //NH Options
        public static CustomHeaderOption NeutralHarbingerSettings;
        public static CustomNumberOption NHMax;
        public static CustomNumberOption NHMin;

        //Plaguebearer Options
        public static CustomHeaderOption Plaguebearer;
        public static CustomNumberOption PlaguebearerCount;
        public static CustomNumberOption InfectCooldown;
        public static CustomToggleOption PBVent;
        public static CustomToggleOption UniquePlaguebearer;

        //NK Options
        public static CustomHeaderOption NeutralKillingSettings;
        public static CustomNumberOption NKMax;
        public static CustomNumberOption NKMin;
        public static CustomToggleOption NKHasImpVision;
        public static CustomToggleOption NKsKnow;

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
        public static CustomToggleOption CryoFreezeAll;

        //Arsonist Options
        public static CustomHeaderOption Arsonist;
        public static CustomNumberOption ArsonistCount;
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
        public static CustomNumberOption AliveVampCount;
        public static CustomToggleOption DracVent;
        public static CustomToggleOption UniqueDracula;
        public static CustomToggleOption UndeadVent;

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
        public static CustomToggleOption ResurrectVent;
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
        public static CustomToggleOption PersuadedVent;

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
        public static CustomNumberOption PhantomTasksRemaining;
        public static CustomToggleOption PhantomPlayersAlerted;

        //Betrayer Options
        public static CustomHeaderOption Betrayer;
        public static CustomNumberOption BetrayerKillCooldown;
        public static CustomToggleOption BetrayerVent;

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
        public static CustomToggleOption SnitchSeestargetsInMeeting;
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
        public static CustomToggleOption AssassinPenalised;
        public static CustomToggleOption AssassinGuessModifiers;
        public static CustomToggleOption AssassinGuessObjectifiers;
        public static CustomToggleOption AssassinGuessAbilities;
        public static CustomToggleOption AssassinGuessInvestigative;
        public static CustomToggleOption AssassinateAfterVoting;
        public static CustomToggleOption UniqueAssassin;
        public static CustomToggleOption AssassinNotification;

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

        //Swapper Options
        public static CustomHeaderOption Swapper;
        public static CustomNumberOption SwapperCount;
        public static CustomToggleOption UniqueSwapper;
        public static CustomToggleOption SwapperButton;
        public static CustomToggleOption SwapAfterVoting;
        public static CustomToggleOption SwapSelf;

        //Politician Options
        public static CustomHeaderOption Politician;
        public static CustomNumberOption PoliticianCount;
        public static CustomToggleOption UniquePolitician;
        public static CustomNumberOption PoliticianVoteBank;
        public static CustomToggleOption PoliticianAnonymous;
        public static CustomToggleOption PoliticianButton;

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

        //Insider Options
        public static CustomHeaderOption Insider;
        public static CustomToggleOption InsiderKnows;
        public static CustomNumberOption InsiderCount;
        public static CustomToggleOption UniqueInsider;

        //Ruthless Options
        public static CustomHeaderOption Ruthless;
        public static CustomNumberOption RuthlessCount;
        public static CustomToggleOption UniqueRuthless;
        public static CustomToggleOption RuthlessKnows;

        //Ninja Options
        public static CustomHeaderOption Ninja;
        public static CustomNumberOption NinjaCount;
        public static CustomToggleOption UniqueNinja;

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
        public static CustomToggleOption TraitorColourSwap;
        public static CustomToggleOption SnitchSeesTraitor;
        public static CustomToggleOption RevealerRevealsTraitor;

        //Fanatic Options
        public static CustomHeaderOption Fanatic;
        public static CustomNumberOption FanaticCount;
        public static CustomToggleOption FanaticKnows;
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
        public static CustomToggleOption AllCorruptedWin;
        public static CustomToggleOption CorruptedVent;

        //Corrupted Options
        public static CustomHeaderOption Overlord;
        public static CustomNumberOption OverlordCount;
        public static CustomNumberOption OverlordMeetingWinCount;
        public static CustomToggleOption UniqueOverlord;
        public static CustomToggleOption OverlordKnows;

        //Lovers Options
        public static CustomHeaderOption Lovers;
        public static CustomNumberOption LoversCount;
        public static CustomToggleOption BothLoversDie;
        public static CustomToggleOption LoversChat;
        public static CustomToggleOption LoversFaction;
        public static CustomToggleOption LoversRoles;
        public static CustomToggleOption UniqueLovers;

        //Mafia Options
        public static CustomHeaderOption Mafia;
        public static CustomNumberOption MafiaCount;
        public static CustomToggleOption MafiaRoles;
        public static CustomToggleOption UniqueMafia;
        public static CustomToggleOption MafVent;

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

        //Defector Options
        public static CustomHeaderOption Defector;
        public static CustomToggleOption UniqueDefector;
        public static CustomNumberOption DefectorCount;
        public static CustomToggleOption DefectorKnows;
        public static CustomStringOption DefectorFaction;

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

        //Professional Options
        public static CustomHeaderOption Professional;
        public static CustomToggleOption ProfessionalKnows;
        public static CustomNumberOption ProfessionalCount;
        public static CustomToggleOption UniqueProfessional;

        //Shy Options
        public static CustomHeaderOption Shy;
        public static CustomNumberOption ShyCount;
        public static CustomToggleOption UniqueShy;

        //Astral Options
        public static CustomHeaderOption Astral;
        public static CustomNumberOption AstralCount;
        public static CustomToggleOption UniqueAstral;

        //Yeller Options
        public static CustomHeaderOption Yeller;
        public static CustomNumberOption YellerCount;
        public static CustomToggleOption UniqueYeller;

        //Indomitable Options
        public static CustomHeaderOption Indomitable;
        public static CustomNumberOption IndomitableCount;
        public static CustomToggleOption UniqueIndomitable;
        public static CustomToggleOption IndomitableKnows;

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

        /*//For Testing
        public static CustomNestedOption ExampleNested;
        public static CustomToggleOption ExampleToggle;
        public static CustomNumberOption ExampleNumber;
        public static CustomStringOption ExampleString;
        public static CustomHeaderOption ExampleHeader;*/

        private static Func<object, string> PercentFormat { get; } = value => $"{value:0}%";
        private static Func<object, string> CooldownFormat { get; } = value => $"{value:0.0#}s";
        private static Func<object, string> DistanceFormat { get; } = value => $"{value:0.0#}m";
        private static Func<object, string> MultiplierFormat { get; } = value => $"{value:0.0#}x";

        public static void GenerateAll()
        {
            SettingsPatches.ExportButton = new();
            SettingsPatches.ImportButton = new();
            SettingsPatches.PresetButton = new();

            if (ModCompatibility.SubLoaded)
                Maps.AddItem("Submerged");

            if (ModCompatibility.LILoaded)
                Maps.AddItem("LevelImpostor");

            var num = 0;

            /*ExampleNested = new(MultiMenu.main, "Exampled Nested Option");
            ExampleHeader = new(num++, MultiMenu.external, "Example Header Option");
            ExampleToggle = new(num++, MultiMenu.external, "Example Toggle Option", true);
            ExampleNumber = new(num++, MultiMenu.external, "Example Number Option", 1, 1, 5, 1, MultiplierFormat);
            ExampleString = new(num++, MultiMenu.external, "Example String Option", new[] { "Something", "Something Else", "Something Else Else" });
            ExampleNested.AddOptions(ExampleHeader, ExampleToggle, ExampleNumber, ExampleString);*/

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
            ReportDistance = new(num++, MultiMenu.main, "Player Report Distance", 3.5f, 1, 20, 0.25f, DistanceFormat);
            ChatCooldown = new(num++, MultiMenu.main, "Chat Cooldown", 3, 0, 3, 0.1f, CooldownFormat);
            LobbySize = new(num++, MultiMenu.main, "Lobby Size", 15, 2, 127, 1);

            GameModeSettings = new(MultiMenu.main, "Game Mode Settings");
            GameMode = new(num++, MultiMenu.main, "Game Mode", new[] { "Classic", "All Any", "Killing Only", "Custom", "Vanilla" });

            KillingOnlySettings = new(MultiMenu.main, "<color=#1D7CF2FF>Killing</color> Only Settings");
            NeutralRoles = new(num++, MultiMenu.main, "<color=#B3B3B3FF>Neutrals</color> Count", 1, 0, 13, 1);
            AddArsonist = new(num++, MultiMenu.main, "Add <color=#EE7600FF>Arsonist</color>", false);
            AddCryomaniac = new(num++, MultiMenu.main, "Add <color=#642DEAFF>Cryomaniac</color>", false);
            AddPlaguebearer = new(num++, MultiMenu.main, "Add <color=#CFFE61FF>Plaguebearer</color>", false);

            AllAnySettings = new(MultiMenu.main, "All Any Settings");
            EnableUniques = new(num++, MultiMenu.main, "Enable Uniques", false);

            GameModifiers = new(MultiMenu.main, "Game Modifiers");
            WhoCanVent = new(num++, MultiMenu.main, "Serial Venters", new[] { "Default", "Everyone", "Never" });
            AnonymousVoting = new(num++, MultiMenu.main, "Anonymous Voting", true);
            SkipButtonDisable = new(num++, MultiMenu.main, "No Skipping", new[] { "Never", "Emergency", "Always" });
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
            DisableLevels = new(num++, MultiMenu.main, "Disable Level Icons", false);
            WhiteNameplates = new(num++, MultiMenu.main, "Disable Player Nameplates", false);
            CustomEject = new(num++, MultiMenu.main, "Custom Ejection Messages", true);
            LighterDarker = new(num++, MultiMenu.main, "Enable Lighter Darker Colors", true);
            ObstructNames = new(num++, MultiMenu.main, "Hide Obstructed Player Names", true);

            MapSettings = new(MultiMenu.main, "Map Settings");
            Map = new(num++, MultiMenu.main, "Map", Maps);
            RandomMapEnabled = new(num++, MultiMenu.main, "Choose Random Map", false);
            RandomMapSkeld = new(num++, MultiMenu.main, "Skeld Chance", 0, 0, 100, 10, PercentFormat);
            RandomMapMira = new(num++, MultiMenu.main, "Mira Chance", 0, 0, 100, 10, PercentFormat);
            RandomMapPolus = new(num++, MultiMenu.main, "Polus Chance", 0, 0, 100, 10, PercentFormat);
            RandomMapAirship = new(num++, MultiMenu.main, "Airship Chance", 0, 0, 100, 10, PercentFormat);

            if (ModCompatibility.SubLoaded)
                RandomMapSubmerged = new(num++, MultiMenu.main, "Submerged Chance", 0, 0, 100, 10, PercentFormat);

            if (ModCompatibility.LILoaded)
                RandomMapSubmerged = new(num++, MultiMenu.main, "Level Impostor Chance", 0, 0, 100, 10, PercentFormat);

            //RandomMapdlekS = new(num++, MultiMenu.main, "dlekS Chance", 0, 0, 100, 10, PercentFormat); for when it comes back lol
            AutoAdjustSettings = new(num++, MultiMenu.main, "Auto Adjust Settings", false);
            SmallMapHalfVision = new(num++, MultiMenu.main, "Half Vision On Skeld/Mira HQ", false);
            SmallMapDecreasedCooldown = new(num++, MultiMenu.main, "Mira HQ Decreased Cooldowns", 0f, 0f, 15f, 2.5f, CooldownFormat);
            LargeMapIncreasedCooldown = new(num++, MultiMenu.main, "Airship/Submerged Increased Cooldowns", 0f, 0f, 15f, 2.5f, CooldownFormat);
            SmallMapIncreasedShortTasks = new(num++, MultiMenu.main, "Skeld/Mira HQ Increased Short Tasks", 0, 0, 5, 1);
            SmallMapIncreasedLongTasks = new(num++, MultiMenu.main, "Skeld/Mira HQ Increased Long Tasks", 0, 0, 3, 1);
            LargeMapDecreasedShortTasks = new(num++, MultiMenu.main, "Airship/Submerged Decreased Short Tasks", 0, 0, 5, 1);
            LargeMapDecreasedLongTasks = new(num++, MultiMenu.main, "Airship/Submerged Decreased Long Tasks", 0, 0, 3, 1);

            BetterSabotages = new(MultiMenu.main, "Better Sabotages Settings");
            ColourblindComms = new(num++, MultiMenu.main, "Camouflaged Comms", true);
            MeetingColourblind = new(num++, MultiMenu.main, "Camouflaged Meetings", false);
            //NightVision = new(num++, MultiMenu.main, "Night Vision Cameras", false);
            //EvilsIgnoreNV = new(num++, MultiMenu.main, "High Vision Evils Ignore Vision", false);
            OxySlow = new(num++, MultiMenu.main, "Oxygen Sabotage Slows Down Players", true);
            ReactorShake = new(num++, MultiMenu.main, "Reactor Sabotage Shakes The Screen By", 30, 0, 100, 1, PercentFormat);

            BetterSkeld = new(MultiMenu.main, "Better Skeld Settings");
            SkeldVentImprovements = new(num++, MultiMenu.main, "Changed Vent Layout", false);

            BetterPolusSettings = new(MultiMenu.main, "Polus Settings");
            PolusVentImprovements = new(num++, MultiMenu.main, "Changed Vent Layout", false);
            VitalsLab = new(num++, MultiMenu.main, "Vitals Moved To Lab", false);
            ColdTempDeathValley = new(num++, MultiMenu.main, "Cold Temp Moved To Death Valley", false);
            WifiChartCourseSwap = new(num++, MultiMenu.main, "Reboot Wifi And Chart Course Swapped", false);
            SeismicTimer = new(num++, MultiMenu.main, "Seimic Stabliser Malfunction Countdown", 60f, 30f, 90f, 5f, CooldownFormat);

            BetterAirshipSettings = new(MultiMenu.main, "Airship Settings");
            SpawnType = new(num++, MultiMenu.main, "Spawn Type", new[] { "Normal", "Fixed", "Random Synchronised", "Meeting" });
            CallPlatform = new(num++, MultiMenu.main, "Add Call Platform Button", false);
            AddTeleporters = new(num++, MultiMenu.main, "Add Meeting To Security Room Teleporter", false);
            MoveVitals = new(num++, MultiMenu.main, "Move Vitals", false);
            MoveFuel = new(num++, MultiMenu.main, "Move Fuel", false);
            MoveDivert = new(num++, MultiMenu.main, "Move Divert", false);
            MoveAdmin = new(num++, MultiMenu.main, "Move Admin", new[] { "Don't Move", "Right Of Cockpit", "Main Hall" });
            MoveElectrical = new(num++, MultiMenu.main, "Move Electrical Outlet", new[] { "Don't Move", "Vault", "Electrical" });

            CrewAuditorRoles = new(MultiMenu.crew, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Auditor</color> <color=#FFD700FF>Roles</color>");
            MysticOn = new(num++, MultiMenu.crew, "<color=#708EEFFF>Mystic</color>", 0, 0, 100, 10, PercentFormat);
            VampireHunterOn = new(num++, MultiMenu.crew, "<color=#C0C0C0FF>Vampire Hunter</color>", 0, 0, 100, 10, PercentFormat);

            CrewInvestigativeRoles = new(MultiMenu.crew, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Investigative</color> <color=#FFD700FF>Roles</color>");
            CoronerOn = new(num++, MultiMenu.crew, "<color=#4D99E6FF>Coroner</color>", 0, 0, 100, 10, PercentFormat);
            DetectiveOn = new(num++, MultiMenu.crew, "<color=#4D4DFFFF>Detective</color>", 0, 0, 100, 10, PercentFormat);
            InspectorOn = new(num++, MultiMenu.crew, "<color=#7E3C64FF>Inspector</color>", 0, 0, 100, 10, PercentFormat);
            MediumOn = new(num++, MultiMenu.crew, "<color=#A680FFFF>Medium</color>", 0, 0, 100, 10, PercentFormat);
            OperativeOn = new(num++, MultiMenu.crew, "<color=#A7D1B3FF>Operative</color>", 0, 0, 100, 10, PercentFormat);
            SeerOn = new(num++, MultiMenu.crew, "<color=#71368AFF>Seer</color>", 0, 0, 100, 10, PercentFormat);
            SheriffOn = new(num++, MultiMenu.crew, "<color=#FFCC80FF>Sheriff</color>", 0, 0, 100, 10, PercentFormat);
            TrackerOn = new(num++, MultiMenu.crew, "<color=#009900FF>Tracker</color>", 0, 0, 100, 10, PercentFormat);

            CrewKillingRoles = new(MultiMenu.crew, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Killing</color> <color=#FFD700FF>Roles</color>");
            VeteranOn = new(num++, MultiMenu.crew, "<color=#998040FF>Veteran</color>", 0, 0, 100, 10, PercentFormat);
            VigilanteOn = new(num++, MultiMenu.crew, "<color=#FFFF00FF>Vigilante</color>", 0, 0, 100, 10, PercentFormat);

            CrewProtectiveRoles = new(MultiMenu.crew, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Protective</color> <color=#FFD700FF>Roles</color>");
            AltruistOn = new(num++, MultiMenu.crew, "<color=#660000FF>Altruist</color>", 0, 0, 100, 10, PercentFormat);
            MedicOn = new(num++, MultiMenu.crew, "<color=#006600FF>Medic</color>", 0, 0, 100, 10, PercentFormat);

            CrewSovereignRoles = new(MultiMenu.crew, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Sovereign</color> <color=#FFD700FF>Roles</color>");
            DictatorOn = new(num++, MultiMenu.crew, "<color=#00CB97FF>Dictator</color>", 0, 0, 100, 10, PercentFormat);
            MayorOn = new(num++, MultiMenu.crew, "<color=#704FA8FF>Mayor</color>", 0, 0, 100, 10, PercentFormat);
            MonarchOn = new(num++, MultiMenu.crew, "<color=#FF004EFF>Monarch</color>", 0, 0, 100, 10, PercentFormat);

            CrewSupportRoles = new(MultiMenu.crew, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Support</color> <color=#FFD700FF>Roles</color>");
            ChameleonOn = new(num++, MultiMenu.crew, "<color=#5411F8FF>Chameleon</color>", 0, 0, 100, 10, PercentFormat);
            EngineerOn = new(num++, MultiMenu.crew, "<color=#FFA60AFF>Engineer</color>", 0, 0, 100, 10, PercentFormat);
            EscortOn = new(num++, MultiMenu.crew, "<color=#803333FF>Escort</color>", 0, 0, 100, 10, PercentFormat);
            RetributionistOn = new(num++, MultiMenu.crew, "<color=#8D0F8CFF>Retributionist</color>", 0, 0, 100, 10, PercentFormat);
            ShifterOn = new(num++, MultiMenu.crew, "<color=#DF851FFF>Shifter</color>", 0, 0, 100, 10, PercentFormat);
            TransporterOn = new(num++, MultiMenu.crew, "<color=#00EEFFFF>Transporter</color>", 0, 0, 100, 10, PercentFormat);

            CrewUtilityRoles = new(MultiMenu.crew, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Utility</color> <color=#FFD700FF>Roles</color>");
            CrewmateOn = new(num++, MultiMenu.crew, "<color=#8CFFFFFF>Crewmate</color>", 0, 0, 100, 10, PercentFormat);
            RevealerOn = new(num++, MultiMenu.crew, "<color=#D3D3D3FF>Revealer</color>", 0, 0, 100, 10, PercentFormat);

            NeutralBenignRoles = new(MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Benign</color> <color=#FFD700FF>Roles</color>");
            AmnesiacOn = new(num++, MultiMenu.neutral, "<color=#22FFFFFF>Amnesiac</color>", 0, 0, 100, 10, PercentFormat);
            GuardianAngelOn = new(num++, MultiMenu.neutral, "<color=#FFFFFFFF>Guardian Angel</color>", 0, 0, 100, 10, PercentFormat);
            SurvivorOn = new(num++, MultiMenu.neutral, "<color=#DDDD00FF>Survivor</color>", 0, 0, 100, 10, PercentFormat);
            ThiefOn = new(num++, MultiMenu.neutral, "<color=#80FF00FF>Thief</color>", 0, 0, 100, 10, PercentFormat);

            NeutralEvilRoles = new(MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Evil</color> <color=#FFD700FF>Roles</color>");
            ActorOn = new(num++, MultiMenu.neutral, "<color=#00ACC2FF>Actor</color>", 0, 0, 100, 10, PercentFormat);
            BountyHunterOn = new(num++, MultiMenu.neutral, "<color=#B51E39FF>Bounty Hunter</color>", 0, 0, 100, 10, PercentFormat);
            CannibalOn = new(num++, MultiMenu.neutral, "<color=#8C4005FF>Cannibal</color>", 0, 0, 100, 10, PercentFormat);
            ExecutionerOn = new(num++, MultiMenu.neutral, "<color=#CCCCCCFF>Executioner</color>", 0, 0, 100, 10, PercentFormat);
            GuesserOn = new(num++, MultiMenu.neutral, "<color=#EEE5BEFF>Guesser</color>", 0, 0, 100, 10, PercentFormat);
            JesterOn = new(num++, MultiMenu.neutral, "<color=#F7B3DAFF>Jester</color>", 0, 0, 100, 10, PercentFormat);
            TrollOn = new(num++, MultiMenu.neutral, "<color=#678D36FF>Troll</color>", 0, 0, 100, 10, PercentFormat);

            NeutralHarbingerRoles = new(MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Harbinger</color> <color=#FFD700FF>Roles</color>");
            PlaguebearerOn = new(num++, MultiMenu.neutral, "<color=#CFFE61FF>Plaguebearer</color>", 0, 0, 100, 10, PercentFormat);

            NeutralKillingRoles = new(MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killing</color> <color=#FFD700FF>Roles</color>");
            ArsonistOn = new(num++, MultiMenu.neutral, "<color=#EE7600FF>Arsonist</color>", 0, 0, 100, 10, PercentFormat);
            CryomaniacOn = new(num++, MultiMenu.neutral, "<color=#642DEAFF>Cryomaniac</color>", 0, 0, 100, 10, PercentFormat);
            GlitchOn = new(num++, MultiMenu.neutral, "<color=#00FF00FF>Glitch</color>", 0, 0, 100, 10, PercentFormat);
            JuggernautOn = new(num++, MultiMenu.neutral, "<color=#A12B56FF>Juggernaut</color>", 0, 0, 100, 10, PercentFormat);
            MurdererOn = new(num++, MultiMenu.neutral, "<color=#6F7BEAFF>Murderer</color>", 0, 0, 100, 10, PercentFormat);
            SerialKillerOn = new(num++, MultiMenu.neutral, "<color=#336EFFFF>Serial Killer</color>", 0, 0, 100, 10, PercentFormat);
            WerewolfOn = new(num++, MultiMenu.neutral, "<color=#9F703AFF>Werewolf</color>", 0, 0, 100, 10, PercentFormat);

            NeutralNeophyteRoles = new(MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Neophyte</color> <color=#FFD700FF>Roles</color>");
            DraculaOn = new(num++, MultiMenu.neutral, "<color=#AC8A00FF>Dracula</color>", 0, 0, 100, 10, PercentFormat);
            JackalOn = new(num++, MultiMenu.neutral, "<color=#45076AFF>Jackal</color>", 0, 0, 100, 10, PercentFormat);
            NecromancerOn = new(num++, MultiMenu.neutral, "<color=#BF5FFFFF>Necromancer</color>", 0, 0, 100, 10, PercentFormat);
            WhispererOn = new(num++, MultiMenu.neutral, "<color=#2D6AA5FF>Whisperer</color>", 0, 0, 100, 10, PercentFormat);

            NeutralNeophyteRoles = new(MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Proselyte</color> <color=#FFD700FF>Roles</color>");
            PhantomOn = new(num++, MultiMenu.neutral, "<color=#662962FF>Phantom</color>", 0, 0, 100, 10, PercentFormat);

            IntruderConcealingRoles = new(MultiMenu.intruder, "<color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Concealing</color> <color=#FFD700FF>Roles</color>");
            BlackmailerOn = new(num++, MultiMenu.intruder, "<color=#02A752FF>Blackmailer</color>", 0, 0, 100, 10, PercentFormat);
            CamouflagerOn = new(num++, MultiMenu.intruder, "<color=#378AC0FF>Camouflager</color>", 0, 0, 100, 10, PercentFormat);
            GrenadierOn = new(num++, MultiMenu.intruder, "<color=#85AA5BFF>Grenadier</color>", 0, 0, 100, 10, PercentFormat);
            JanitorOn = new(num++, MultiMenu.intruder, "<color=#2647A2FF>Janitor</color>", 0, 0, 100, 10, PercentFormat);

            IntruderDeceptionRoles = new(MultiMenu.intruder, "<color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Deception</color> <color=#FFD700FF>Roles</color>");
            DisguiserOn = new(num++, MultiMenu.intruder, "<color=#40B4FFFF>Disguiser</color>", 0, 0, 100, 10, PercentFormat);
            MorphlingOn = new(num++, MultiMenu.intruder, "<color=#BB45B0FF>Morphling</color>", 0, 0, 100, 10, PercentFormat);
            WraithOn = new(num++, MultiMenu.intruder, "<color=#5C4F75FF>Wraith</color>", 0, 0, 100, 10, PercentFormat);

            IntruderKillingRoles = new(MultiMenu.intruder, "<color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Killing</color> <color=#FFD700FF>Roles</color>");
            AmbusherOn = new(num++, MultiMenu.intruder, "<color=#2BD29CFF>Ambusher</color>", 0, 0, 100, 10, PercentFormat);
            EnforcerOn = new(num++, MultiMenu.intruder, "<color=#005643FF>Enforcer</color>", 0, 0, 100, 10, PercentFormat);

            IntruderSupportRoles = new(MultiMenu.intruder, "<color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Support</color> <color=#FFD700FF>Roles</color>");
            ConsigliereOn = new(num++, MultiMenu.intruder, "<color=#FFFF99FF>Consigliere</color>", 0, 0, 100, 10, PercentFormat);
            ConsortOn = new(num++, MultiMenu.intruder, "<color=#801780FF>Consort</color>", 0, 0, 100, 10, PercentFormat);
            GodfatherOn = new(num++, MultiMenu.intruder, "<color=#404C08FF>Godfather</color>", 0, 0, 100, 10, PercentFormat);
            MinerOn = new(num++, MultiMenu.intruder, "<color=#AA7632FF>Miner</color>", 0, 0, 100, 10, PercentFormat);
            TeleporterOn = new(num++, MultiMenu.intruder, "<color=#939593FF>Teleporter</color>", 0, 0, 100, 10, PercentFormat);

            IntruderUtilityRoles = new(MultiMenu.intruder, "<color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Utility</color> <color=#FFD700FF>Roles</color>");
            GhoulOn = new(num++, MultiMenu.intruder, "<color=#F1C40FFF>Ghoul</color>", 0, 0, 100, 10, PercentFormat);
            ImpostorOn = new(num++, MultiMenu.intruder, "<color=#FF0000FF>Impostor</color>", 0, 0, 100, 10, PercentFormat);

            SyndicateDisruptionRoles = new(MultiMenu.syndicate, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Disruption</color> <color=#FFD700FF>Roles</color>");
            ConcealerOn = new(num++, MultiMenu.syndicate, "<color=#C02525FF>Concealer</color>", 0, 0, 100, 10, PercentFormat);
            DrunkardOn = new(num++, MultiMenu.syndicate, "<color=#FF7900FF>Drunkard</color>", 0, 0, 100, 10, PercentFormat);
            FramerOn = new(num++, MultiMenu.syndicate, "<color=#00FFFFFF>Framer</color>", 0, 0, 100, 10, PercentFormat);
            ShapeshifterOn = new(num++, MultiMenu.syndicate, "<color=#2DFF00FF>Shapeshifter</color>", 0, 0, 100, 10, PercentFormat);
            SilencerOn = new(num++, MultiMenu.syndicate, "<color=#AAB43EFF>Silencer</color>", 0, 0, 100, 10, PercentFormat);

            SyndicateKillingRoles = new(MultiMenu.syndicate, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Killing</color> <color=#FFD700FF>Roles</color>");
            BomberOn = new(num++, MultiMenu.syndicate, "<color=#C9CC3FFF>Bomber</color>", 0, 0, 100, 10, PercentFormat);
            ColliderOn = new(num++, MultiMenu.syndicate, "<color=#B345FFFF>Collider</color>", 0, 0, 100, 10, PercentFormat);
            CrusaderOn = new(num++, MultiMenu.syndicate, "<color=#DF7AE8FF>Crusader</color>", 0, 0, 100, 10, PercentFormat);
            PoisonerOn = new(num++, MultiMenu.syndicate, "<color=#B5004CFF>Poisoner</color>", 0, 0, 100, 10, PercentFormat);

            SyndicateSupportRoles = new(MultiMenu.syndicate, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Support</color> <color=#FFD700FF>Roles</color>");
            RebelOn = new(num++, MultiMenu.syndicate, "<color=#FFFCCEFF>Rebel</color>", 0, 0, 100, 10, PercentFormat);
            StalkerOn = new(num++, MultiMenu.syndicate, "<color=#7E4D00FF>Stalker</color>", 0, 0, 100, 10, PercentFormat);
            WarperOn = new(num++, MultiMenu.syndicate, "<color=#8C7140FF>Warper</color>", 0, 0, 100, 10, PercentFormat);

            SyndicatePowerRoles = new(MultiMenu.syndicate, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Power</color> <color=#FFD700FF>Roles</color>");
            SpellslingerOn = new(num++, MultiMenu.syndicate, "<color=#0028F5FF>Spellslinger</color>", 0, 0, 100, 10, PercentFormat);
            TimeKeeperOn = new(num++, MultiMenu.syndicate, "<color=#3769FEFF>Time Keeper</color>", 0, 0, 100, 10, PercentFormat);

            SyndicateUtilityRoles = new(MultiMenu.syndicate, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Utility</color> <color=#FFD700FF>Roles</color>");
            AnarchistOn = new(num++, MultiMenu.syndicate, "<color=#008000FF>Anarchist</color>", 0, 0, 100, 10, PercentFormat);
            BansheeOn = new(num++, MultiMenu.syndicate, "<color=#E67E22FF>Banshee</color>", 0, 0, 100, 10, PercentFormat);

            Modifiers = new(MultiMenu.modifier, "<color=#7F7F7FFF>Modifiers</color>");
            AstralOn = new(num++, MultiMenu.modifier, "<color=#612BEFFF>Astral</color>", 0, 0, 100, 10, PercentFormat);
            BaitOn = new(num++, MultiMenu.modifier, "<color=#00B3B3FF>Bait</color>", 0, 0, 100, 10, PercentFormat);
            CowardOn = new(num++, MultiMenu.modifier, "<color=#456BA8FF>Coward</color>", 0, 0, 100, 10, PercentFormat);
            DiseasedOn = new(num++, MultiMenu.modifier, "<color=#374D1EFF>Diseased</color>", 0, 0, 100, 10, PercentFormat);
            DrunkOn = new(num++, MultiMenu.modifier, "<color=#758000FF>Drunk</color>", 0, 0, 100, 10, PercentFormat);
            DwarfOn = new(num++, MultiMenu.modifier, "<color=#FF8080FF>Dwarf</color>", 0, 0, 100, 10, PercentFormat);
            FlincherOn = new(num++, MultiMenu.modifier, "<color=#80B3FFFF>Flincher</color>", 0, 0, 100, 10, PercentFormat);
            GiantOn = new(num++, MultiMenu.modifier, "<color=#FFB34DFF>Giant</color>", 0, 0, 100, 10, PercentFormat);
            IndomitableOn = new(num++, MultiMenu.modifier, "<color=#2DE5BEFF>Indomitable</color>", 0, 0, 100, 10, PercentFormat);
            ProfessionalOn = new(num++, MultiMenu.modifier, "<color=#860B7AFF>Professional</color>", 0, 0, 100, 10, PercentFormat);
            ShyOn = new(num++, MultiMenu.modifier, "<color=#1002C5FF>Shy</color>", 0, 0, 100, 10, PercentFormat);
            VIPOn = new(num++, MultiMenu.modifier, "<color=#DCEE85FF>VIP</color>", 0, 0, 100, 10, PercentFormat);
            VolatileOn = new(num++, MultiMenu.modifier, "<color=#FFA60AFF>Volatile</color>", 0, 0, 100, 10, PercentFormat);
            YellerOn = new(num++, MultiMenu.modifier, "<color=#F6AAB7FF>Yeller</color>", 0, 0, 100, 10, PercentFormat);

            Objectifiers = new(MultiMenu.objectifier, "<color=#DD585BFF>Objectifiers</color>");
            AlliedOn = new(num++, MultiMenu.objectifier, "<color=#4545A9FF>Allied</color>", 0, 0, 100, 10, PercentFormat);
            CorruptedOn = new(num++, MultiMenu.objectifier, "<color=#4545FFFF>Corrupted</color>", 0, 0, 100, 10, PercentFormat);
            DefectorOn = new(num++, MultiMenu.objectifier, "<color=#E1C849FF>Defector</color>", 0, 0, 100, 10, PercentFormat);
            FanaticOn = new(num++, MultiMenu.objectifier, "<color=#678D36FF>Fanatic</color>", 0, 0, 100, 10, PercentFormat);
            LoversOn = new(num++, MultiMenu.objectifier, "<color=#FF66CCFF>Lovers</color>", 0, 0, 100, 10, PercentFormat);
            MafiaOn = new(num++, MultiMenu.objectifier, "<color=#00EEFFFF>Mafia</color>", 0, 0, 100, 10, PercentFormat);
            OverlordOn = new(num++, MultiMenu.objectifier, "<color=#008080FF>Overlord</color>", 0, 0, 100, 10, PercentFormat);
            RivalsOn = new(num++, MultiMenu.objectifier, "<color=#3D2D2CFF>Rivals</color>", 0, 0, 100, 10, PercentFormat);
            TaskmasterOn = new(num++, MultiMenu.objectifier, "<color=#ABABFFFF>Taskmaster</color>", 0, 0, 100, 10, PercentFormat);
            TraitorOn = new(num++, MultiMenu.objectifier, "<color=#370D43FF>Traitor</color>", 0, 0, 100, 10, PercentFormat);

            Abilities = new(MultiMenu.ability, "<color=#FF9900FF>Abilities</color>");
            ButtonBarryOn = new(num++, MultiMenu.ability, "<color=#E600FFFF>Button Barry</color>", 0, 0, 100, 10, PercentFormat);
            CrewAssassinOn = new(num++, MultiMenu.ability, "<color=#8CFFFFFF>Crew</color> <color=#073763FF>Assassin</color>", 0, 0, 100, 10, PercentFormat);
            InsiderOn = new(num++, MultiMenu.ability, "<color=#26FCFBFF>Insider</color>", 0, 0, 100, 10, PercentFormat);
            IntruderAssassinOn = new(num++, MultiMenu.ability, "<color=#FF0000FF>Intruder</color> <color=#073763FF>Assassin</color>", 0, 0, 100, 10, PercentFormat);
            MultitaskerOn = new(num++, MultiMenu.ability, "<color=#FF804DFF>Multitasker</color>", 0, 0, 100, 10, PercentFormat);
            NeutralAssassinOn = new(num++, MultiMenu.ability, "<color=#B3B3B3FF>Neutral</color> <color=#073763FF>Assassin</color>", 0, 0, 100, 10, PercentFormat);
            NinjaOn = new(num++, MultiMenu.ability, "<color=#A84300FF>Ninja</color>", 0, 0, 100, 10, PercentFormat);
            PoliticianOn = new(num++, MultiMenu.ability, "<color=#CCA3CCFF>Politician</color>", 0, 0, 100, 10, PercentFormat);
            RadarOn = new(num++, MultiMenu.ability, "<color=#FF0080FF>Radar</color>", 0, 0, 100, 10, PercentFormat);
            RuthlessOn = new(num++, MultiMenu.ability, "<color=#2160DDFF>Ruthless</color>", 0, 0, 100, 10, PercentFormat);
            SnitchOn = new(num++, MultiMenu.ability, "<color=#D4AF37FF>Snitch</color>", 0, 0, 100, 10, PercentFormat);
            SwapperOn = new(num++, MultiMenu.ability, "<color=#66E666FF>Swapper</color>", 0, 0, 100, 10, PercentFormat);
            SyndicateAssassinOn = new(num++, MultiMenu.ability, "<color=#008000FF>Syndicate</color> <color=#073763FF>Assassin</color>", 0, 0, 100, 10, PercentFormat);
            TiebreakerOn = new(num++, MultiMenu.ability, "<color=#99E699FF>Tiebreaker</color>", 0, 0, 100, 10, PercentFormat);
            TorchOn = new(num++, MultiMenu.ability, "<color=#FFFF99FF>Torch</color>", 0, 0, 100, 10, PercentFormat);
            TunnelerOn = new(num++, MultiMenu.ability, "<color=#E91E63FF>Tunneler</color>", 0, 0, 100, 10, PercentFormat);
            UnderdogOn = new(num++, MultiMenu.ability, "<color=#841A7FFF>Underdog</color>", 0, 0, 100, 10, PercentFormat);

            CrewSettings = new(MultiMenu.crew, "<color=#8CFFFFFF>Crew</color> Settings");
            CustomCrewColors = new(num++, MultiMenu.crew, "Enable Custom <color=#8CFFFFFF>Crew</color> Colors", true);
            CommonTasks = new(num++, MultiMenu.crew, "Common Tasks", 2, 0, 100, 1);
            LongTasks = new(num++, MultiMenu.crew, "Long Tasks", 1, 0, 100, 1);
            ShortTasks = new(num++, MultiMenu.crew, "Short Tasks", 4, 0, 100, 1);
            GhostTasksCountToWin = new(num++, MultiMenu.crew, "<color=#8CFFFFFF>Crew</color> Ghost Tasks Count To Win", true);
            CrewVision = new(num++, MultiMenu.crew, "<color=#8CFFFFFF>Crew</color> Vision", 1f, 0.25f, 5f, 0.25f, MultiplierFormat);
            //CrewFlashlight = new(num++, MultiMenu.crew, "<color=#8CFFFFFF>Crew</color> Uses A Flashlight", false);
            CrewMax = new(num++, MultiMenu.crew, "Max <color=#8CFFFFFF>Crew</color> <color=#FFD700FF>Roles</color>", 5, 0, 14, 1);
            CrewMin = new(num++, MultiMenu.crew, "Min <color=#8CFFFFFF>Crew</color> <color=#FFD700FF>Roles</color>", 5, 0, 14, 1);
            CrewVent = new(num++, MultiMenu.crew, "<color=#8CFFFFFF>Crew</color> Can Vent", false);

            CrewAuditorSettings = new(MultiMenu.crew, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Auditor</color> Settings");
            CAMax = new(num++, MultiMenu.crew, "Max <color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Auditor</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1);
            CAMin = new(num++, MultiMenu.crew, "Min <color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Auditor</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1);

            MysticSettings = new(MultiMenu.crew, "<color=#708EEFFF>Mystic</color>");
            MysticCount = new(num++, MultiMenu.crew, "<color=#708EEFFF>Mystic</color> Count", 1, 1, 14, 1);
            UniqueMystic = new(num++, MultiMenu.crew, "<color=#708EEFFF>Mystic</color> Is Unique In All Any", false);
            RevealCooldown = new(num++, MultiMenu.crew, "Reveal Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);

            VampireHunter = new(MultiMenu.crew, "<color=#C0C0C0FF>Vampire Hunter</color>");
            VampireHunterCount = new(num++, MultiMenu.crew, "<color=#C0C0C0FF>Vampire Hunter</color> Count", 1, 1, 14, 1);
            UniqueVampireHunter = new(num++, MultiMenu.crew, "<color=#C0C0C0FF>Vampire Hunter</color> Is Unique In All Any", false);
            StakeCooldown = new(num++, MultiMenu.crew, "Stake Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);

            CrewInvestigativeSettings = new(MultiMenu.crew, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Investigative</color> Settings");
            CIMax = new(num++, MultiMenu.crew, "Max <color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Investigative</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1);
            CIMin = new(num++, MultiMenu.crew, "Min <color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Investigative</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1);

            Coroner = new(MultiMenu.crew, "<color=#4D99E6FF>Coroner</color>");
            CoronerCount = new(num++, MultiMenu.crew, "<color=#4D99E6FF>Coroner</color> Count", 1, 1, 14, 1);
            UniqueCoroner = new(num++, MultiMenu.crew, "<color=#4D99E6FF>Coroner</color> Is Unique In All Any", false);
            CoronerArrowDuration = new(num++, MultiMenu.crew, "Dead Body Arrow Duration", 0.1f, 0f, 2f, 0.05f, CooldownFormat);
            CoronerReportRole = new(num++, MultiMenu.crew, "<color=#4D99E6FF>Coroner</color> Gets Killer's Role", false);
            CoronerReportName = new(num++, MultiMenu.crew, "<color=#4D99E6FF>Coroner</color> Gets Killer's Name", false);
            CoronerKillerNameTime = new(num++, MultiMenu.crew, "<color=#4D99E6FF>Coroner</color> Gets Killer's Name Under", 1f, 0.5f, 15f, 0.5f, CooldownFormat);
            CompareCooldown = new(num++, MultiMenu.crew, "Compare Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            AutopsyCooldown = new(num++, MultiMenu.crew, "Autopsy Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            CompareLimit = new(num++, MultiMenu.crew, "Compare Limit", 5, 1, 15, 1);

            Detective = new(MultiMenu.crew, "<color=#4D4DFFFF>Detective</color>");
            DetectiveCount = new(num++, MultiMenu.crew, "<color=#4D4DFFFF>Detective</color> Count", 1, 1, 14, 1);
            UniqueDetective = new(num++, MultiMenu.crew, "<color=#4D4DFFFF>Detective</color> Is Unique In All Any", false);
            ExamineCooldown = new(num++, MultiMenu.crew, "Examine Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            RecentKill = new(num++, MultiMenu.crew, "Bloody Player Duration", 10f, 5f, 60f, 2.5f, CooldownFormat);
            FootprintInterval = new(num++, MultiMenu.crew, "Footprint Interval", 0.15f, 0.05f, 2f, 0.05f, CooldownFormat);
            FootprintDuration = new(num++, MultiMenu.crew, "Footprint Duration", 10f, 1f, 10f, 0.5f, CooldownFormat);
            AnonymousFootPrint = new(num++, MultiMenu.crew, "Anonymous Footprint", false);

            Inspector = new(MultiMenu.crew, "<color=#7E3C64FF>Inspector</color>");
            InspectorCount = new(num++, MultiMenu.crew, "<color=#7E3C64FF>Inspector</color> Count", 1, 1, 14, 1);
            UniqueInspector = new(num++, MultiMenu.crew, "<color=#7E3C64FF>Inspector</color> Is Unique In All Any", false);
            InspectCooldown = new(num++, MultiMenu.crew, "Inspect Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);

            Medium = new(MultiMenu.crew, "<color=#A680FFFF>Medium</color>");
            MediumCount = new(num++, MultiMenu.crew, "<color=#A680FFFF>Medium</color> Count", 1, 1, 14, 1);
            UniqueMedium = new(num++, MultiMenu.crew, "<color=#A680FFFF>Medium</color> Is Unique In All Any", false);
            MediateCooldown = new(num++, MultiMenu.crew, "Mediate Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            ShowMediatePlayer = new(num++, MultiMenu.crew, "Reveal Appearance Of Mediate Target", true);
            ShowMediumToDead = new(num++, MultiMenu.crew, "Reveal The <color=#A680FFFF>Medium</color> To The Mediate Target", new[] { "No", "Target", "All Dead" });
            DeadRevealed = new(num++, MultiMenu.crew, "Who Is Revealed With Mediate", new[] { "Oldest Dead", "Newest Dead", "All Dead", "Random" });

            Seer = new(MultiMenu.crew, "<color=#71368AFF>Seer</color>");
            SeerCount = new(num++, MultiMenu.crew, "<color=#71368AFF>Seer</color> Count", 1, 1, 14, 1);
            UniqueSeer = new(num++, MultiMenu.crew, "<color=#71368AFF>Seer</color> Is Unique In All Any", false);
            SeerCooldown = new(num++, MultiMenu.crew, "Seer Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);

            Sheriff = new(MultiMenu.crew, "<color=#FFCC80FF>Sheriff</color>");
            SheriffCount = new(num++, MultiMenu.crew, "<color=#FFCC80FF>Sheriff</color> Count", 1, 1, 14, 1);
            UniqueSheriff = new(num++, MultiMenu.crew, "<color=#FFCC80FF>Sheriff</color> Is Unique In All Any", false);
            InterrogateCooldown = new(num++, MultiMenu.crew, "Interrogate Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            NeutEvilRed = new(num++, MultiMenu.crew, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Evils</color> Show Evil", false);
            NeutKillingRed = new(num++, MultiMenu.crew, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killings</color> Show Evil", false);

            Tracker = new(MultiMenu.crew, "<color=#009900FF>Tracker</color>");
            TrackerCount = new(num++, MultiMenu.crew, "<color=#009900FF>Tracker</color> Count", 1, 1, 14, 1);
            UniqueTracker = new(num++, MultiMenu.crew, "<color=#009900FF>Tracker</color> Is Unique In All Any", false);
            UpdateInterval = new(num++, MultiMenu.crew, "Arrow Update Interval", 5f, 0f, 15f, 0.5f, CooldownFormat);
            TrackCooldown = new(num++, MultiMenu.crew, "Track Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            ResetOnNewRound = new(num++, MultiMenu.crew, "<color=#009900FF>Tracker</color> Arrows Reset After Each Round", false);
            MaxTracks = new(num++, MultiMenu.crew, "Max Tracks", 5, 1, 15, 1);

            Operative = new(MultiMenu.crew, "<color=#A7D1B3FF>Operative</color>");
            OperativeCount = new(num++, MultiMenu.crew, "<color=#A7D1B3FF>Operative</color> Count", 1, 1, 14, 1);
            UniqueOperative = new(num++, MultiMenu.crew, "<color=#A7D1B3FF>Operative</color> Is Unique In All Any", false);
            BugCooldown = new(num++, MultiMenu.crew, "Bug Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            MinAmountOfTimeInBug = new(num++, MultiMenu.crew, "Min Amount Of Time In Bug To Trigger", 0f, 0f, 15f, 0.5f, CooldownFormat);
            BugsRemoveOnNewRound = new(num++, MultiMenu.crew, "Bugs Are Removed Each Round", true);
            MaxBugs = new(num++, MultiMenu.crew, "Bug Count", 5, 1, 15, 1);
            BugRange = new(num++, MultiMenu.crew, "Bug Radius", 1.5f, 0.5f, 5f, 0.25f, DistanceFormat);
            MinAmountOfPlayersInBug = new(num++, MultiMenu.crew, "Number Of <color=#FFD700FF>Roles</color> Required To Trigger Bug", 1, 1, 5, 1);
            WhoSeesDead = new(num++, MultiMenu.crew, "Who Sees Dead Bodies On Admin", new[] { "Nobody", "Operative", "Everyone But Operative", "Everyone" });

            CrewKillingSettings = new(MultiMenu.crew, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Killing</color> Settings");
            CKMax = new(num++, MultiMenu.crew, "Max <color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Killing</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1);
            CKMin = new(num++, MultiMenu.crew, "Min <color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Killing</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1);

            Veteran = new(MultiMenu.crew, "<color=#998040FF>Veteran</color>");
            VeteranCount = new(num++, MultiMenu.crew, "<color=#998040FF>Veteran</color> Count", 1, 1, 14, 1);
            UniqueVeteran = new(num++, MultiMenu.crew, "<color=#998040FF>Veteran</color> Is Unique In All Any", false);
            AlertCooldown = new(num++, MultiMenu.crew, "Alert Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            AlertDuration = new(num++, MultiMenu.crew, "Alert Duration", 10f, 5f, 30f, 1f, CooldownFormat);
            MaxAlerts = new(num++, MultiMenu.crew, "Max Alerts", 5, 1, 15, 1);

            Vigilante = new(MultiMenu.crew, "<color=#FFFF00FF>Vigilante</color>");
            VigilanteCount = new(num++, MultiMenu.crew, "<color=#FFFF00FF>Vigilante</color> Count", 1, 1, 14, 1);
            UniqueVigilante = new(num++, MultiMenu.crew, "<color=#FFFF00FF>Vigilante</color> Is Unique In All Any", false);
            MisfireKillsInno = new(num++, MultiMenu.crew, "Misfire Kills The Target", true);
            VigiKillAgain = new(num++, MultiMenu.crew, "<color=#FFFF00FF>Vigilante</color> Can Kill Again If Target Was Innocent", true);
            RoundOneNoShot = new(num++, MultiMenu.crew, "<color=#FFFF00FF>Vigilante</color> Cannot Shoot On The First Round", true);
            VigiNotifOptions = new(num++, MultiMenu.crew, "How Is The <color=#FFFF00FF>Vigilante</color> Notified Of Their Target's Innocence", new[] { "Never", "Flash", "Message" });
            VigiOptions = new(num++, MultiMenu.crew, "How Does <color=#FFFF00FF>Vigilante</color> Die", new[] { "Immediately", "Before Meeting", "After Meeting" });
            VigiKillCd = new(num++, MultiMenu.crew, "Shoot Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            VigiBulletCount = new(num++, MultiMenu.crew, "Max Bullets", 5, 1, 15, 1);

            CrewProtectiveSettings = new(MultiMenu.crew, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Protective</color> Settings");
            CPMax = new(num++, MultiMenu.crew, "Max <color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Protective</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1);
            CPMin = new(num++, MultiMenu.crew, "Min <color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Protective</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1);

            Altruist = new(MultiMenu.crew, "<color=#660000FF>Altruist</color>");
            AltruistCount = new(num++, MultiMenu.crew, "<color=#660000FF>Altruist</color> Count", 1, 1, 14, 1);
            UniqueAltruist = new(num++, MultiMenu.crew, "<color=#660000FF>Altruist</color> Is Unique In All Any", false);
            ReviveCount = new(num++, MultiMenu.crew, "Revive Count", 5, 1, 14, 1);
            ReviveCooldown = new(num++, MultiMenu.crew, "Revive Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            AltReviveDuration = new(num++, MultiMenu.crew, "<color=#660000FF>Altruist</color> Revive Duration", 10f, 1f, 15f, 1f, CooldownFormat);
            AltruistTargetBody = new(num++, MultiMenu.crew, "Target's Body Disappears On Beginning Of Revive", false);

            Medic = new(MultiMenu.crew, "<color=#006600FF>Medic</color>");
            MedicCount = new(num++, MultiMenu.crew, "<color=#006600FF>Medic</color> Count", 1, 1, 14, 1);
            UniqueMedic = new(num++, MultiMenu.crew, "<color=#006600FF>Medic</color> Is Unique In All Any", false);
            ShowShielded = new(num++, MultiMenu.crew, "Show Shielded Player", new[] { "Self", "Medic", "Self And Medic", "Everyone" });
            WhoGetsNotification = new(num++, MultiMenu.crew, "Who Gets Murder Attempt Indicator", new[] { "Medic", "Self", "Self And Medic", "Everyone", "Nobody" });
            ShieldBreaks = new(num++, MultiMenu.crew, "Shield Breaks On Murder Attempt", true);

            CrewSovereignSettings = new(MultiMenu.crew, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Sovereign</color> Settings");
            CSvMax = new(num++, MultiMenu.crew, "Max <color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Sovereign</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1);
            CSvMin = new(num++, MultiMenu.crew, "Min <color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Sovereign</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1);

            Dictator = new(MultiMenu.crew, "<color=#00CB97FF>Dictator</color>");
            DictatorCount = new(num++, MultiMenu.crew, "<color=#00CB97FF>Dictator</color> Count", 1, 1, 14, 1);
            UniqueDictator = new(num++, MultiMenu.crew, "<color=#00CB97FF>Dictator</color> Is Unique In All Any", false);
            RoundOneNoDictReveal = new(num++, MultiMenu.crew, "<color=#00CB97FF>Dictator</color> Cannot Reveal Round One", false);
            RoundOneNoDictReveal = new(num++, MultiMenu.crew, "<color=#00CB97FF>Dictator</color> Can Dictate After Voting", false);
            DictatorButton = new(num++, MultiMenu.crew, "<color=#00CB97FF>Dictator</color> Can Button", true);

            Mayor = new(MultiMenu.crew, "<color=#704FA8FF>Mayor</color>");
            MayorCount = new(num++, MultiMenu.crew, "<color=#704FA8FF>Mayor</color> Count", 1, 1, 14, 1);
            UniqueMayor = new(num++, MultiMenu.crew, "<color=#704FA8FF>Mayor</color> Is Unique In All Any", false);
            MayorVoteCount = new(num++, MultiMenu.crew, "Revealed <color=#704FA8FF>Mayor</color> Votes Count As", 2, 1, 10, 1);
            RoundOneNoReveal = new(num++, MultiMenu.crew, "<color=#704FA8FF>Mayor</color> Cannot Reveal Round One", false);
            MayorButton = new(num++, MultiMenu.crew, "<color=#704FA8FF>Mayor</color> Can Button", true);

            Monarch = new(MultiMenu.crew, "<color=#FF004EFF>Monarch</color>");
            MonarchCount = new(num++, MultiMenu.crew, "<color=#FF004EFF>Monarch</color> Count", 1, 1, 14, 1);
            UniqueMonarch = new(num++, MultiMenu.crew, "<color=#FF004EFF>Monarch</color> Is Unique In All Any", false);
            KnightingCooldown = new(num++, MultiMenu.crew, "Knighting Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            RoundOneNoKnighting = new(num++, MultiMenu.crew, "<color=#FF004EFF>Monarch</color> Cannot Knight Round One", false);
            KnightCount = new(num++, MultiMenu.crew, "Knight Count", 2, 1, 14, 1);
            KnightVoteCount = new(num++, MultiMenu.crew, "Knighted Votes Count As", 1, 1, 10, 1);
            MonarchButton = new(num++, MultiMenu.crew, "<color=#FF004EFF>Monarch</color> Can Button", true);
            KnightButton = new(num++, MultiMenu.crew, "Knights Can Button", true);

            CrewSupportSettings = new(MultiMenu.crew, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Support</color> Settings");
            CSMax = new(num++, MultiMenu.crew, "Max <color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Support</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1);
            CSMin = new(num++, MultiMenu.crew, "Min <color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Support</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1);

            Chameleon = new(MultiMenu.crew, "<color=#5411F8FF>Chameleon</color>");
            ChameleonCount = new(num++, MultiMenu.crew, "<color=#5411F8FF>Chameleon</color> Count", 1, 1, 14, 1);
            UniqueChameleon = new(num++, MultiMenu.crew, "<color=#5411F8FF>Chameleon</color> Is Unique In All Any", false);
            SwoopCooldown = new(num++, MultiMenu.crew, "Swoop Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            SwoopDuration = new(num++, MultiMenu.crew, "Swoop Duration", 10f, 5f, 30f, 1f, CooldownFormat);
            SwoopCount = new(num++, MultiMenu.crew, "Max Swoops", 5, 1, 15, 1);

            Engineer = new(MultiMenu.crew, "<color=#FFA60AFF>Engineer</color>");
            EngineerCount = new(num++, MultiMenu.crew, "<color=#FFA60AFF>Engineer</color> Count", 1, 1, 14, 1);
            UniqueEngineer = new(num++, MultiMenu.crew, "<color=#FFA60AFF>Engineer</color> Is Unique In All Any", false);
            FixCooldown = new(num++, MultiMenu.crew, "Fix Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            MaxFixes = new(num++, MultiMenu.crew, "Max Fixes", 5, 1, 15, 1);

            Escort = new(MultiMenu.crew, "<color=#803333FF>Escort</color>");
            EscortCount = new(num++, MultiMenu.crew, "<color=#803333FF>Escort</color> Count", 1, 1, 14, 1);
            UniqueEscort = new(num++, MultiMenu.crew, "<color=#803333FF>Escort</color> Is Unique In All Any", false);
            EscRoleblockCooldown = new(num++, MultiMenu.crew, "Roleblock Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            EscRoleblockDuration = new(num++, MultiMenu.crew, "Roleblock Duration", 10f, 5f, 30f, 1f, CooldownFormat);

            Retributionist = new(MultiMenu.crew, "<color=#8D0F8CFF>Retributionist</color>");
            RetributionistCount = new(num++, MultiMenu.crew, "<color=#8D0F8CFF>Retributionist</color> Count", 1, 1, 14, 1);
            UniqueRetributionist = new(num++, MultiMenu.crew, "<color=#8D0F8CFF>Retributionist</color> Is Unique In All Any", false);
            ReviveAfterVoting = new(num++, MultiMenu.crew, "<color=#8D0F8CFF>Retributionist</color> Can Mimic After Voting", true);
            MaxUses = new(num++, MultiMenu.crew, "Max Uses", 5, 1, 15, 1);

            Shifter = new(MultiMenu.crew, "<color=#DF851FFF>Shifter</color>");
            ShifterCount = new(num++, MultiMenu.crew, "<color=#DF851FFF>Shifter</color> Count", 1, 1, 14, 1);
            UniqueShifter = new(num++, MultiMenu.crew, "<color=#DF851FFF>Shifter</color> Is Unique In All Any", false);
            ShifterCd = new(num++, MultiMenu.crew, "Shift Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            ShiftedBecomes = new(num++, MultiMenu.crew, "Shifted Becomes", new[] { "Shifter", "Crewmate" });

            Transporter = new(MultiMenu.crew, "<color=#00EEFFFF>Transporter</color>");
            TransporterCount = new(num++, MultiMenu.crew, "<color=#00EEFFFF>Transporter</color> Count", 1, 1, 14, 1);
            UniqueTransporter = new(num++, MultiMenu.crew, "<color=#00EEFFFF>Transporter</color> Is Unique In All Any", false);
            TransportCooldown = new(num++, MultiMenu.crew, "Transport Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            TransportDuration = new(num++, MultiMenu.crew, "Transport Duration", 5f, 1f, 20f, 1f, CooldownFormat);
            TransportMaxUses = new(num++, MultiMenu.crew, "Max Transports", 5, 1, 15, 1);
            TransSelf = new(num++, MultiMenu.crew, "<color=#00EEFFFF>Transporter</color> Can Transport Themselves", true);

            CrewUtilitySettings = new(MultiMenu.crew, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Utility</color> Settings");

            Crewmate = new(MultiMenu.crew, "<color=#8CFFFFFF>Crewmate</color>");
            CrewCount = new(num++, MultiMenu.crew, "<color=#8CFFFFFF>Crewmate</color> Count", 1, 1, 14, 1);

            Revealer = new(MultiMenu.crew, "<color=#D3D3D3FF>Revealer</color>");
            RevealerTasksRemainingClicked = new(num++, MultiMenu.crew, "Tasks Remaining When <color=#D3D3D3FF>Revealer</color> Can Be Clicked", 5, 1, 10, 1);
            RevealerTasksRemainingAlert = new(num++, MultiMenu.crew, "Tasks Remaining When Revealed", 1, 1, 5, 1);
            RevealerRevealsNeutrals = new(num++, MultiMenu.crew, "<color=#D3D3D3FF>Revealer</color> Reveals <color=#B3B3B3FF>Neutrals</color>", false);
            RevealerRevealsCrew = new(num++, MultiMenu.crew, "<color=#D3D3D3FF>Revealer</color> Reveals <color=#8CFFFFFF>Crew</color>", false);
            RevealerRevealsRoles = new(num++, MultiMenu.crew, "<color=#D3D3D3FF>Revealer</color> Reveals Exact <color=#FFD700FF>Roles</color>", false);
            RevealerCanBeClickedBy = new(num++, MultiMenu.crew, "Who Can Click <color=#D3D3D3FF>Revealer</color>", new[] { "All", "Non Crew", "Evils Only" });

            NeutralSettings = new(MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> Settings");
            CustomNeutColors = new(num++, MultiMenu.neutral, "Enable Custom <color=#B3B3B3FF>Neutral</color> Colors", true);
            NeutralVision = new(num++, MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> Vision", 1.5f, 0.25f, 5f, 0.25f, MultiplierFormat);
            LightsAffectNeutrals = new(num++, MultiMenu.neutral, "Lights Sabotage Affects <color=#B3B3B3FF>Neutral</color> Vision", true);
            //NeutralFlashlight = new(num++, MultiMenu.neutral, "<color=#B3B3B3FF>Neutrals</color> Use A Flashlight", false);
            NeutralMax = new(num++, MultiMenu.neutral, "Max <color=#B3B3B3FF>Neutral</color> <color=#FFD700FF>Roles</color>", 5, 1, 14, 1);
            NeutralMin = new(num++, MultiMenu.neutral, "Min <color=#B3B3B3FF>Neutral</color> <color=#FFD700FF>Roles</color>", 5, 1, 14, 1);
            NoSolo = new(num++, MultiMenu.neutral, "<color=#B3B3B3FF>Neutrals</color> Together, Strong", new[] { "Never", "Same NKs", "Same Roles", "All NKs", "All Neutrals" });
            VigiKillsNB = new(num++, MultiMenu.neutral, "<color=#FFFF00FF>Vigilante</color> Kills <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Benigns</color>", true);
            NKHasImpVision = new(num++, MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killers</color> Have <color=#FF0000FF>Intruder</color> Vision", true);
            NKsKnow = new(num++, MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killers</color> Know Each Other", false);
            NeutralEvilsEndGame = new(num++, MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Evils</color> End The Game When Winning", false);
            NeutralsVent = new(num++, MultiMenu.neutral, "<color=#B3B3B3FF>Neutrals</color> Can Vent", true);

            NeutralApocalypseSettings = new(MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Apocalypse</color> Settings");

            Pestilence = new(MultiMenu.neutral, "<color=#424242FF>Pestilence</color>");
            PestSpawn = new(num++, MultiMenu.neutral, "<color=#424242FF>Pestilence</color> Can Spawn Directly", false);
            PlayersAlerted = new(num++, MultiMenu.neutral, "<color=#424242FF>Pestilence</color> Transformation Alerts Everyone", true);
            PestKillCooldown = new(num++, MultiMenu.neutral, "Obliterate Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            PestVent = new(num++, MultiMenu.neutral, "<color=#424242FF>Pestilence</color> Can Vent", true);

            NeutralBenignSettings = new(MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Benign</color> Settings");
            NBMax = new(num++, MultiMenu.neutral, "Max <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Benign</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1);
            NBMin = new(num++, MultiMenu.neutral, "Min <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Benign</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1);

            Amnesiac = new(MultiMenu.neutral, "<color=#22FFFFFF>Amnesiac</color>");
            AmnesiacCount = new(num++, MultiMenu.neutral, "<color=#22FFFFFF>Amnesiac</color> Count", 1, 1, 14, 1);
            UniqueAmnesiac = new(num++, MultiMenu.neutral, "<color=#22FFFFFF>Amnesiac</color> Is Unique In All Any", false);
            RememberArrows = new(num++, MultiMenu.neutral, "<color=#22FFFFFF>Amnesiac</color> Gets Arrows To Dead Bodies", false);
            RememberArrowDelay = new(num++, MultiMenu.neutral, "Arrow Appearance Delay", 5f, 0f, 15f, 1f, CooldownFormat);
            AmneVent = new(num++, MultiMenu.neutral, "<color=#22FFFFFF>Amnesiac</color> Can Hide In Vents", false);
            AmneSwitchVent = new(num++, MultiMenu.neutral, "<color=#22FFFFFF>Amnesiac</color> Can Switch Vents", false);

            GuardianAngel = new(MultiMenu.neutral, "<color=#FFFFFFFF>Guardian Angel</color>");
            GuardianAngelCount = new(num++, MultiMenu.neutral, "<color=#FFFFFFFF>Guardian Angel</color> Count", 1, 1, 14, 1);
            UniqueGuardianAngel = new(num++, MultiMenu.neutral, "<color=#FFFFFFFF>Guardian Angel</color> Is Unique In All Any", false);
            ProtectCd = new(num++, MultiMenu.neutral, "Protect Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            ProtectDuration = new(num++, MultiMenu.neutral, "Protect Duration", 10f, 5f, 30f, 1f, CooldownFormat);
            ProtectKCReset = new(num++, MultiMenu.neutral, "Cooldown Reset When Protected", 2.5f, 0f, 15f, 0.5f, CooldownFormat);
            MaxProtects = new(num++, MultiMenu.neutral, "Max Protects", 5, 1, 15, 1);
            ShowProtect = new(num++, MultiMenu.neutral, "Show Protected Player", new[] { "Self", "Guardian Angel", "Self And GA", "Everyone" });
            GATargetKnows = new(num++, MultiMenu.neutral, "Target Knows <color=#FFFFFFFF>Guardian Angel</color> Exists", false);
            ProtectBeyondTheGrave = new(num++, MultiMenu.neutral, "<color=#FFFFFFFF>Guardian Angel</color> Can Protect After Death", false);
            GAKnowsTargetRole = new(num++, MultiMenu.neutral, "<color=#FFFFFFFF>Guardian Angel</color> Knows Target's <color=#FFD700FF>Role</color>", false);
            GAVent = new(num++, MultiMenu.neutral, "<color=#FFFFFFFF>Guardian Angel</color> Can Hide In Vents", false);
            GASwitchVent = new(num++, MultiMenu.neutral, "<color=#FFFFFFFF>Guardian Angel</color> Can Switch Vents", false);

            Survivor = new(MultiMenu.neutral, "<color=#DDDD00FF>Survivor</color>");
            SurvivorCount = new(num++, MultiMenu.neutral, "<color=#DDDD00FF>Survivor</color> Count", 1, 1, 14, 1);
            UniqueSurvivor = new(num++, MultiMenu.neutral, "<color=#DDDD00FF>Survivor</color> Is Unique In All Any", false);
            VestCd = new(num++, MultiMenu.neutral, "Vest Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            VestDuration = new(num++, MultiMenu.neutral, "Vest Duration", 10f, 5f, 30f, 1f, CooldownFormat);
            VestKCReset = new(num++, MultiMenu.neutral, "Cooldown Reset When Vested", 2.5f, 0f, 15f, 0.5f, CooldownFormat);
            MaxVests = new(num++, MultiMenu.neutral, "Max Vests", 5, 1, 15, 1);
            SurvVent = new(num++, MultiMenu.neutral, "<color=#DDDD00FF>Survivor</color> Can Hide In Vents", false);
            SurvSwitchVent = new(num++, MultiMenu.neutral, "<color=#DDDD00FF>Survivor</color> Can Switch Vents", false);

            Thief = new(MultiMenu.neutral, "<color=#80FF00FF>Thief</color>");
            ThiefCount = new(num++, MultiMenu.neutral, "<color=#80FF00FF>Thief</color> Count", 1, 1, 14, 1);
            UniqueThief = new(num++, MultiMenu.neutral, "<color=#80FF00FF>Thief</color> Is Unique In All Any", false);
            ThiefKillCooldown = new(num++, MultiMenu.neutral, "Steal Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            ThiefSteals = new(num++, MultiMenu.neutral, "<color=#80FF00FF>Thief</color> Assigns <color=#80FF00FF>Thief</color> <color=#FFD700FF>Role</color> To Target", false);
            ThiefVent = new(num++, MultiMenu.neutral, "<color=#80FF00FF>Thief</color> Can Vent", false);

            NeutralEvilSettings = new(MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Evil</color> Settings");
            NEMax = new(num++, MultiMenu.neutral, "Max <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Evil</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1);
            NEMin = new(num++, MultiMenu.neutral, "Min <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Evil</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1);

            Actor = new(MultiMenu.neutral, "<color=#00ACC2FF>Actor</color>");
            ActorCount = new(num++, MultiMenu.neutral, "<color=#00ACC2FF>Actor</color> Count", 1, 1, 14, 1);
            UniqueActor= new(num++, MultiMenu.neutral, "<color=#00ACC2FF>Actor</color> Is Unique In All Any", false);
            ActorButton = new(num++, MultiMenu.neutral, "<color=#00ACC2FF>Actor</color> Can Button", true);
            ActorVent = new(num++, MultiMenu.neutral, "<color=#00ACC2FF>Actor</color> Can Hide In Vents", false);
            ActSwitchVent = new(num++, MultiMenu.neutral, "<color=#00ACC2FF>Actor</color> Can Switch Vents", false);
            VigiKillsActor = new(num++, MultiMenu.neutral, "<color=#FFFF00FF>Vigilante</color> Kills <color=#00ACC2FF>Actor</color>", false);

            BountyHunter = new(MultiMenu.neutral, "<color=#B51E39FF>Bounty Hunter</color>");
            BHCount = new(num++, MultiMenu.neutral, "<color=#B51E39FF>Bounty Hunter</color> Count", 1, 1, 14, 1);
            UniqueBountyHunter = new(num++, MultiMenu.neutral, "<color=#B51E39FF>Bounty Hunter</color> Is Unique In All Any", false);
            BountyHunterCooldown= new(num++, MultiMenu.neutral, "Guess Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            BountyHunterGuesses = new(num++, MultiMenu.neutral, "Max Guesses", 5, 1, 15, 1);
            BHVent = new(num++, MultiMenu.neutral, "<color=#B51E39FF>Bounty Hunter</color> Can Vent", false);
            VigiKillsBH = new(num++, MultiMenu.neutral, "<color=#FFFF00FF>Vigilante</color> Kills <color=#B51E39FF>Bounty Hunter</color>", false);

            Cannibal = new(MultiMenu.neutral, "<color=#8C4005FF>Cannibal</color>");
            CannibalCount = new(num++, MultiMenu.neutral, "<color=#8C4005FF>Cannibal</color> Count", 1, 1, 14, 1);
            UniqueCannibal = new(num++, MultiMenu.neutral, "<color=#8C4005FF>Cannibal</color> Is Unique In All Any", false);
            CannibalCd = new(num++, MultiMenu.neutral, "Eat Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            CannibalBodyCount = new(num++, MultiMenu.neutral, "Bodies Needed To Win", 1, 1, 5, 1);
            EatArrows = new(num++, MultiMenu.neutral, "<color=#8C4005FF>Cannibal</color> Gets Arrows To Dead Bodies", false);
            EatArrowDelay = new(num++, MultiMenu.neutral, "Arrow Appearance Delay", 5f, 0f, 15f, 1f, CooldownFormat);
            CannibalVent = new(num++, MultiMenu.neutral, "<color=#8C4005FF>Cannibal</color> Can Vent", false);
            VigiKillsCannibal = new(num++, MultiMenu.neutral, "<color=#FFFF00FF>Vigilante</color> Kills <color=#8C4005FF>Cannibal</color>", false);

            Executioner = new(MultiMenu.neutral, "<color=#CCCCCCFF>Executioner</color>");
            ExecutionerCount = new(num++, MultiMenu.neutral, "<color=#CCCCCCFF>Executioner</color> Count", 1, 1, 14, 1);
            UniqueExecutioner = new(num++, MultiMenu.neutral, "<color=#CCCCCCFF>Executioner</color> Is Unique In All Any", false);
            DoomCooldown = new(num++, MultiMenu.neutral, "Doom Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            DoomCount = new(num++, MultiMenu.neutral, "Doom Count", 5, 1, 14, 1);
            ExecutionerButton = new(num++, MultiMenu.neutral, "<color=#CCCCCCFF>Executioner</color> Can Button", true);
            ExeVent = new(num++, MultiMenu.neutral, "<color=#CCCCCCFF>Executioner</color> Can Hide In Vents", false);
            ExeSwitchVent = new(num++, MultiMenu.neutral, "<color=#CCCCCCFF>Executioner</color> Can Switch Vents", false);
            ExeTargetKnows = new(num++, MultiMenu.neutral, "Target Knows <color=#CCCCCCFF>Executioner</color> Exists", false);
            ExeKnowsTargetRole = new(num++, MultiMenu.neutral, "<color=#CCCCCCFF>Executioner</color> Knows Target's <color=#FFD700FF>Role</color>", false);
            ExeEjectScreen = new(num++, MultiMenu.neutral, "Target Ejection Reveals Existence Of <color=#CCCCCCFF>Executioner</color>", false);
            ExeCanHaveIntruderTargets = new(num++, MultiMenu.neutral, "<color=#CCCCCCFF>Executioner</color> Can Have <color=#FF0000FF>Intruder</color> Targets", false);
            ExeCanHaveSyndicateTargets = new(num++, MultiMenu.neutral, "<color=#CCCCCCFF>Executioner</color> Can Have <color=#008000FF>Syndicate</color> Targets", false);
            ExeCanHaveNeutralTargets = new(num++, MultiMenu.neutral, "<color=#CCCCCCFF>Executioner</color> Can Have <color=#B3B3B3FF>Neutral</color> Targets", false);
            ExeCanWinBeyondDeath = new(num++, MultiMenu.neutral, "<color=#CCCCCCFF>Executioner</color> Can Win After Death", false);
            VigiKillsExecutioner = new(num++, MultiMenu.neutral, "<color=#FFFF00FF>Vigilante</color> Kills <color=#CCCCCCFF>Executioner</color>", false);

            Guesser = new(MultiMenu.neutral, "<color=#EEE5BEFF>Guesser</color>");
            GuesserCount = new(num++, MultiMenu.neutral, "<color=#EEE5BEFF>Guesser</color> Count", 1, 1, 14, 1);
            UniqueGuesser = new(num++, MultiMenu.neutral, "<color=#EEE5BEFF>Guesser</color> Is Unique In All Any", false);
            GuesserButton = new(num++, MultiMenu.neutral, "<color=#EEE5BEFF>Guesser</color> Can Button", true);
            GuessVent = new(num++, MultiMenu.neutral, "<color=#EEE5BEFF>Guesser</color> Can Hide In Vents", false);
            GuessSwitchVent = new(num++, MultiMenu.neutral, "<color=#EEE5BEFF>Guesser</color> Can Switch Vents", false);
            GuessTargetKnows = new(num++, MultiMenu.neutral, "Target Knows <color=#EEE5BEFF>Guesser</color> Exists", false);
            MultipleGuesses = new(num++, MultiMenu.neutral, "<color=#EEE5BEFF>Guesser</color> Can Guess Multiple Times", true);
            GuessCount = new(num++, MultiMenu.neutral, "Max Guesses", 5, 1, 15, 1);
            GuesserAfterVoting = new(num++, MultiMenu.neutral, "<color=#EEE5BEFF>Guesser</color> Can Guess After Voting", false);
            VigiKillsGuesser = new(num++, MultiMenu.neutral, "<color=#FFFF00FF>Vigilante</color> Kills <color=#EEE5BEFF>Guesser</color>", false);

            Jester = new(MultiMenu.neutral, "<color=#F7B3DAFF>Jester</color>");
            JesterCount = new(num++, MultiMenu.neutral, "<color=#F7B3DAFF>Jester</color> Count", 1, 1, 14, 1);
            UniqueJester= new(num++, MultiMenu.neutral, "<color=#F7B3DAFF>Jester</color> Is Unique In All Any", false);
            JesterButton = new(num++, MultiMenu.neutral, "<color=#F7B3DAFF>Jester</color> Can Button", true);
            HauntCooldown = new(num++, MultiMenu.neutral, "Haunt Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            HauntCount = new(num++, MultiMenu.neutral, "Haunt Count", 5, 1, 14, 1);
            JesterVent = new(num++, MultiMenu.neutral, "<color=#F7B3DAFF>Jester</color> Can Hide In Vents", false);
            JestSwitchVent = new(num++, MultiMenu.neutral, "<color=#F7B3DAFF>Jester</color> Can Switch Vents", false);
            JestEjectScreen = new(num++, MultiMenu.neutral, "<color=#F7B3DAFF>Jester</color> Ejection Reveals Existence Of <color=#F7B3DAFF>Jester</color>", false);
            VigiKillsJester = new(num++, MultiMenu.neutral, "<color=#FFFF00FF>Vigilante</color> Kills <color=#F7B3DAFF>Jester</color>", false);

            Troll = new(MultiMenu.neutral, "<color=#678D36FF>Troll</color>");
            TrollCount = new(num++, MultiMenu.neutral, "<color=#678D36FF>Troll</color> Count", 1, 1, 14, 1);
            UniqueTroll = new(num++, MultiMenu.neutral, "<color=#678D36FF>Troll</color> Is Unique In All Any", false);
            InteractCooldown = new(num++, MultiMenu.neutral, "Interact Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            TrollVent = new(num++, MultiMenu.neutral, "<color=#678D36FF>Troll</color> Can Hide In Vent", false);
            TrollSwitchVent = new(num++, MultiMenu.neutral, "<color=#678D36FF>Troll</color> Can Switch Vents", false);

            NeutralHarbingerSettings = new(MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Harbinger</color> Settings");
            NHMax = new(num++, MultiMenu.neutral, "Max <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Harbinger</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1);
            NHMin = new(num++, MultiMenu.neutral, "Min <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Harbinger</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1);

            Plaguebearer = new(MultiMenu.neutral, "<color=#CFFE61FF>Plaguebearer</color>");
            PlaguebearerCount = new(num++, MultiMenu.neutral, "<color=#CFFE61FF>Plaguebearer</color> Count", 1, 1, 14, 1);
            UniquePlaguebearer = new(num++, MultiMenu.neutral, "<color=#CFFE61FF>Plaguebearer</color> Is Unique In All Any", false);
            InfectCooldown = new(num++, MultiMenu.neutral, "Infect Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            PBVent = new(num++, MultiMenu.neutral, "<color=#CFFE61FF>Plaguebearer</color> Can Vent", false);

            NeutralKillingSettings = new(MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killing</color> Settings");
            NKMax = new(num++, MultiMenu.neutral, "Max <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killing</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1);
            NKMin = new(num++, MultiMenu.neutral, "Min <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killing</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1);

            Arsonist = new(MultiMenu.neutral, "<color=#EE7600FF>Arsonist</color>");
            ArsonistCount = new(num++, MultiMenu.neutral, "<color=#EE7600FF>Arsonist</color> Count", 1, 1, 14, 1);
            UniqueArsonist = new(num++, MultiMenu.neutral, "<color=#EE7600FF>Arsonist</color> Is Unique In All Any", false);
            DouseCooldown = new(num++, MultiMenu.neutral, "Douse Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            IgniteCooldown = new(num++, MultiMenu.neutral, "Ignite Cooldown", 25f, 5f, 60f, 2.5f, CooldownFormat);
            ArsoLastKillerBoost = new(num++, MultiMenu.neutral, "Ignite Cooldown Removed When <color=#EE7600FF>Arsonist</color> Is Last Killer", false);
            ArsoIgniteAll = new(num++, MultiMenu.neutral, "Ignition Ignites All Doused Players", false);
            ArsoCooldownsLinked = new(num++, MultiMenu.neutral, "Douse And Ignite Cooldowns Are Linked", false);
            IgnitionCremates = new(num++, MultiMenu.neutral, "Ignition Cremates Bodies", false);
            ArsoVent = new(num++, MultiMenu.neutral, "<color=#EE7600FF>Arsonist</color> Can Vent", false);

            Cryomaniac = new(MultiMenu.neutral, "<color=#642DEAFF>Cryomaniac</color>");
            CryomaniacCount = new(num++, MultiMenu.neutral, "<color=#642DEAFF>Cryomaniac</color> Count", 1, 1, 14, 1);
            UniqueCryomaniac = new(num++, MultiMenu.neutral, "<color=#642DEAFF>Cryomaniac</color> Is Unique In All Any", false);
            CryoDouseCooldown = new(num++, MultiMenu.neutral, "Douse Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            CryoFreezeAll = new(num++, MultiMenu.neutral, "<color=#642DEAFF>Cryomaniac</color> Freeze Freezes All Doused Players", false);
            CryoVent = new(num++, MultiMenu.neutral, "<color=#642DEAFF>Cryomaniac</color> Can Vent", false);

            Glitch = new(MultiMenu.neutral, "<color=#00FF00FF>Glitch</color>");
            GlitchCount = new(num++, MultiMenu.neutral, "<color=#00FF00FF>Glitch</color> Count", 1, 1, 14, 1);
            UniqueGlitch = new(num++, MultiMenu.neutral, "<color=#00FF00FF>Glitch</color> Is Unique In All Any", false);
            MimicCooldown = new(num++, MultiMenu.neutral, "Mimic Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            HackCooldown = new(num++, MultiMenu.neutral, "Hack Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            MimicDuration = new(num++, MultiMenu.neutral, "Mimic Duration", 10f, 5f, 30f, 1f, CooldownFormat);
            HackDuration = new(num++, MultiMenu.neutral, "Hack Duration", 10f, 5f, 30f, 1f, CooldownFormat);
            GlitchKillCooldown = new(num++, MultiMenu.neutral, "Neutralise Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            GlitchVent = new(num++, MultiMenu.neutral, "<color=#00FF00FF>Glitch</color> Can Vent", false);

            Juggernaut = new(MultiMenu.neutral, "<color=#A12B56FF>Juggernaut</color>");
            JuggernautCount = new(num++, MultiMenu.neutral, "<color=#A12B56FF>Juggernaut</color> Count", 1, 1, 14, 1);
            UniqueJuggernaut = new(num++, MultiMenu.neutral, "<color=#A12B56FF>Juggernaut</color> Is Unique In All Any", false);
            JuggKillCooldown = new(num++, MultiMenu.neutral, "Assault Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            JuggKillBonus = new(num++, MultiMenu.neutral, "Assault Cooldown Bonus", 5f, 2.5f, 30f, 2.5f, CooldownFormat);
            JuggVent = new(num++, MultiMenu.neutral, "<color=#A12B56FF>Juggernaut</color> Can Vent", false);

            Murderer = new(MultiMenu.neutral, "<color=#6F7BEAFF>Murderer</color>");
            MurdCount = new(num++, MultiMenu.neutral, "<color=#6F7BEAFF>Murderer</color> Count", 1, 1, 14, 1);
            UniqueMurderer = new(num++, MultiMenu.neutral, "<color=#6F7BEAFF>Murderer</color> Is Unique In All Any", false);
            MurdKillCooldownOption = new(num++, MultiMenu.neutral, "Murder Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            MurdVent = new(num++, MultiMenu.neutral, "<color=#6F7BEAFF>Murderer</color> Can Vent", false);

            SerialKiller = new(MultiMenu.neutral, "<color=#336EFFFF>Serial Killer</color>");
            SKCount = new(num++, MultiMenu.neutral, "<color=#336EFFFF>Serial Killer</color> Count", 1, 1, 14, 1);
            UniqueSerialKiller = new(num++, MultiMenu.neutral, "<color=#336EFFFF>Serial Killer</color> Is Unique In All Any", false);
            BloodlustCooldown = new(num++, MultiMenu.neutral, "Bloodlust Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            BloodlustDuration = new(num++, MultiMenu.neutral, "Bloodlust Duration", 10f, 5f, 30f, 1f, CooldownFormat);
            LustKillCooldown = new(num++, MultiMenu.neutral, "Stab Cooldown", 5f, 0.5f, 15f, 0.5f, CooldownFormat);
            SKVentOptions = new(num++, MultiMenu.neutral, "<color=#336EFFFF>Serial Killer</color> Can Vent", new[] { "Always", "Bloodlust", "No Lust", "Never" });

            Werewolf = new(MultiMenu.neutral, "<color=#9F703AFF>Werewolf</color>");
            WerewolfCount = new(num++, MultiMenu.neutral, "<color=#9F703AFF>Werewolf</color> Count", 1, 1, 14, 1);
            UniqueWerewolf = new(num++, MultiMenu.neutral, "<color=#9F703AFF>Werewolf</color> Is Unique In All Any", false);
            MaulCooldown = new(num++, MultiMenu.neutral, "Maul Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            MaulRadius = new(num++, MultiMenu.neutral, "Maul Radius", 1.5f, 0.5f, 5f, 0.25f, DistanceFormat);
            WerewolfVent = new(num++, MultiMenu.neutral, "<color=#9F703AFF>Werewolf</color> Can Vent", false);

            NeutralNeophyteSettings = new(MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Neophyte</color> Settings");
            NNMax = new(num++, MultiMenu.neutral, "Max <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Neophyte</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1);
            NNMin = new(num++, MultiMenu.neutral, "Min <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Neophyte</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1);

            Dracula = new(MultiMenu.neutral, "<color=#AC8A00FF>Dracula</color>");
            DraculaCount = new(num++, MultiMenu.neutral, "<color=#AC8A00FF>Dracula</color> Count", 1, 1, 14, 1);
            UniqueDracula = new(num++, MultiMenu.neutral, "<color=#AC8A00FF>Dracula</color> Is Unique In All Any", false);
            BiteCooldown = new(num++, MultiMenu.neutral, "Bite Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            DracVent = new(num++, MultiMenu.neutral, "<color=#AC8A00FF>Dracula</color> Can Vent", false);
            AliveVampCount = new(num++, MultiMenu.neutral, "Alive <color=#7B8968FF>Undead</color> Count", 3, 1, 14, 1);
            UndeadVent = new(num++, MultiMenu.neutral, "Undead Can Vent", false);

            Jackal = new(MultiMenu.neutral, "<color=#45076AFF>Jackal</color>");
            JackalCount = new(num++, MultiMenu.neutral, "<color=#45076AFF>Jackal</color> Count", 1, 1, 14, 1);
            UniqueJackal = new(num++, MultiMenu.neutral, "<color=#45076AFF>Jackal</color> Is Unique In All Any", false);
            RecruitCooldown = new(num++, MultiMenu.neutral, "Recruit Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            JackalVent = new(num++, MultiMenu.neutral, "<color=#45076AFF>Jackal</color> Can Vent", false);
            RecruitVent = new(num++, MultiMenu.neutral, "Recruits Can Vent", false);

            Necromancer = new(MultiMenu.neutral, "<color=#BF5FFFFF>Necromancer</color>");
            NecromancerCount = new(num++, MultiMenu.neutral, "<color=#BF5FFFFF>Necromancer</color> Count", 1, 1, 14, 1);
            UniqueNecromancer = new(num++, MultiMenu.neutral, "<color=#BF5FFFFF>Necromancer</color> Is Unique In All Any", false);
            ResurrectCooldown = new(num++, MultiMenu.neutral, "Resurrect Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            ResurrectCooldownIncrease = new(num++, MultiMenu.neutral, "Resurrect Cooldown Increases By", 5f, 2.5f, 30f, 2.5f, CooldownFormat);
            ResurrectCount = new(num++, MultiMenu.neutral, "Resurrect Count", 5, 1, 14, 1);
            NecroKillCooldown = new(num++, MultiMenu.neutral, "Kill Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            NecroKillCooldownIncrease = new(num++, MultiMenu.neutral, "Kill Cooldown Increases By", 5f, 2.5f, 30f, 2.5f, CooldownFormat);
            NecroKillCount = new(num++, MultiMenu.neutral, "Kill Count", 5, 1, 14, 1);
            KillResurrectCooldownsLinked = new(num++, MultiMenu.neutral, "Kill And Resurrect Cooldowns Are Linked", false);
            NecromancerTargetBody = new(num++, MultiMenu.neutral, "Target's Body Disappears On Beginning Of Resurrect", false);
            NecroResurrectDuration = new(num++, MultiMenu.neutral, "Resurrect Duration", 10f, 1f, 15f, 1f, CooldownFormat);
            NecroVent = new(num++, MultiMenu.neutral, "<color=#BF5FFFFF>Necromancer</color> Can Vent", false);
            ResurrectVent = new(num++, MultiMenu.neutral, "Resurrected Can Vent", false);

            Whisperer = new(MultiMenu.neutral, "<color=#2D6AA5FF>Whisperer</color>");
            WhispererCount = new(num++, MultiMenu.neutral, "<color=#2D6AA5FF>Whisperer</color> Count", 1, 1, 14, 1);
            UniqueWhisperer = new(num++, MultiMenu.neutral, "<color=#2D6AA5FF>Whisperer</color> Is Unique In All Any", false);
            WhisperCooldown = new(num++, MultiMenu.neutral, "Whisper Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            WhisperCooldownIncrease = new(num++, MultiMenu.neutral, "Whisper Cooldown Increases By", 5f, 2.5f, 30f, 2.5f, CooldownFormat);
            WhisperRadius = new(num++, MultiMenu.neutral, "Whisper Radius", 1.5f, 0.5f, 5f, 0.25f, DistanceFormat);
            InitialWhisperRate = new(num++, MultiMenu.neutral, "Whisper Rate", 5, 5, 50, 5, PercentFormat);
            WhisperRateDecreases = new(num++, MultiMenu.neutral, "Whisper Rate Decreases Each Whisper", false);
            WhisperRateDecrease = new(num++, MultiMenu.neutral, "Whisper Rate Decrease", 5, 5, 50, 5, CooldownFormat);
            WhispVent = new(num++, MultiMenu.neutral, "<color=#2D6AA5FF>Whisperer</color> Can Vent", false);
            PersuadedVent = new(num++, MultiMenu.neutral, "Persuaded Can Vent", false);

            NeutralProselyteSettings = new(MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Proselyte</color> Settings");

            Betrayer = new(MultiMenu.neutral, "<color=#11806AFF>Betrayer</color>");
            BetrayerKillCooldown = new(num++, MultiMenu.neutral, "Kill Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            BetrayerVent = new(num++, MultiMenu.neutral, "<color=#11806AFF>Betrayer</color> Can Vent", true);

            Phantom = new(MultiMenu.neutral, "<color=#662962FF>Phantom</color>");
            PhantomTasksRemaining = new(num++, MultiMenu.neutral, "Tasks Remaining When <color=#662962FF>Phantom</color> Can Be Clicked", 5, 1, 10, 1);
            PhantomPlayersAlerted = new(num++, MultiMenu.neutral, "Players Are Alerted When <color=#662962FF>Phantom</color> Is Clickable", false);

            IntruderSettings = new(MultiMenu.intruder, "<color=#FF0000FF>Intruder</color> Settings");
            CustomIntColors = new(num++, MultiMenu.intruder, "Enable Custom <color=#FF0000FF>Intruder</color> Colors", true);
            IntruderCount = new(num++, MultiMenu.intruder, "<color=#FF0000FF>Intruder</color> Count", 1, 0, 4, 1);
            IntruderVision = new(num++, MultiMenu.intruder, "<color=#FF0000FF>Intruder</color> Vision", 2f, 0.25f, 5f, 0.25f, MultiplierFormat);
            //IntruderFlashlight = new(num++, MultiMenu.intruder, "<color=#FF0000FF>Intruders</color> Use A Flashlight", false);
            IntruderKillCooldown = new(num++, MultiMenu.intruder, "<color=#FF0000FF>Intruder</color> Kill Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            IntrudersVent = new(num++, MultiMenu.intruder, "<color=#FF0000FF>Intruder</color> Can Vent", true);
            IntrudersCanSabotage = new(num++, MultiMenu.intruder, "<color=#FF0000FF>Intruder</color> Can Sabotage", true);
            GhostsCanSabotage = new(num++, MultiMenu.intruder, "Dead <color=#FF0000FF>Intruders</color> Can Sabotage", false);
            IntruderMax = new(num++, MultiMenu.intruder, "Max <color=#FF0000FF>Intruder</color> <color=#FFD700FF>Roles</color>", 5, 1, 14, 1);
            IntruderMin = new(num++, MultiMenu.intruder, "Min <color=#FF0000FF>Intruder</color> <color=#FFD700FF>Roles</color>", 5, 1, 14, 1);

            IntruderConcealingSettings = new(MultiMenu.intruder, "<color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Concealing</color> Settings");
            ICMax = new(num++, MultiMenu.intruder, "Max <color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Concealing</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1);
            ICMin = new(num++, MultiMenu.intruder, "Min <color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Concealing</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1);

            Blackmailer = new(MultiMenu.intruder, "<color=#02A752FF>Blackmailer</color>");
            BlackmailerCount = new(num++, MultiMenu.intruder, "<color=#02A752FF>Blackmailer</color> Count", 1, 1, 14, 1);
            UniqueBlackmailer = new(num++, MultiMenu.intruder, "<color=#02A752FF>Blackmailer</color> Is Unique In All Any", false);
            BlackmailCooldown = new(num++, MultiMenu.intruder, "Blackmail Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            WhispersNotPrivate = new(num++, MultiMenu.intruder, "<color=#02A752FF>Blackmailer</color> Can Read Whispers", true);
            BlackmailMates = new(num++, MultiMenu.intruder, "<color=#02A752FF>Blackmailer</color> Can Blackmail Teammates", false);
            BMRevealed = new(num++, MultiMenu.intruder, "Blackmail Is Revealed To Everyone", true);

            Camouflager = new(MultiMenu.intruder, "<color=#378AC0FF>Camouflager</color>");
            CamouflagerCount = new(num++, MultiMenu.intruder, "<color=#378AC0FF>Camouflager</color> Count", 1, 1, 14, 1);
            UniqueCamouflager = new(num++, MultiMenu.intruder, "<color=#378AC0FF>Camouflager</color> Is Unique In All Any", false);
            CamouflagerCooldown = new(num++, MultiMenu.intruder, "Camouflage Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            CamouflagerDuration = new(num++, MultiMenu.intruder, "Camouflage Duration", 10f, 5f, 30f, 1f, CooldownFormat);
            CamoHideSize = new(num++, MultiMenu.intruder, "Camouflage Hides Player Size", false);
            CamoHideSpeed = new(num++, MultiMenu.intruder, "Camouflage Hides Player Speed", false);

            Grenadier = new(MultiMenu.intruder, "<color=#85AA5BFF>Grenadier</color>");
            GrenadierCount = new(num++, MultiMenu.intruder, "<color=#85AA5BFF>Grenadier</color> Count", 1, 1, 14, 1);
            UniqueGrenadier = new(num++, MultiMenu.intruder, "<color=#85AA5BFF>Grenadier</color> Is Unique In All Any", false);
            GrenadeCooldown = new(num++, MultiMenu.intruder, "Flash Grenade Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            GrenadeDuration = new(num++, MultiMenu.intruder, "Flash Grenade Duration", 10f, 5f, 30f, 1f, CooldownFormat);
            FlashRadius = new(num++, MultiMenu.intruder, "Flash Radius", 1.5f, 0.5f, 5f, 0.25f, DistanceFormat);
            GrenadierIndicators = new(num++, MultiMenu.intruder, "Indicate Flashed Players", false);
            GrenadierVent = new(num++, MultiMenu.intruder, "<color=#85AA5BFF>Grenadier</color> Can Vent", false);

            Janitor = new(MultiMenu.intruder, "<color=#2647A2FF>Janitor</color>");
            JanitorCount = new(num++, MultiMenu.intruder, "<color=#2647A2FF>Janitor</color> Count", 1, 1, 14, 1);
            UniqueJanitor = new(num++, MultiMenu.intruder, "<color=#2647A2FF>Janitor</color> Is Unique In All Any", false);
            JanitorCleanCd = new(num++, MultiMenu.intruder, "Clean Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            JaniCooldownsLinked = new(num++, MultiMenu.intruder, "Kill And Clean Cooldowns Are Linked", false);
            SoloBoost = new(num++, MultiMenu.intruder, "<color=#2647A2FF>Janitor</color> Has Lower Cooldown When Solo", false);
            DragCooldown = new(num++, MultiMenu.intruder, "Drag Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            DragModifier = new(num++, MultiMenu.intruder, "Drag Speed", 0.5f, 0.5f, 2f, 0.5f, MultiplierFormat);
            JanitorVentOptions = new(num++, MultiMenu.intruder, "<color=#2647A2FF>Janitor</color> Can Vent", new[] { "Never", "Body", "Bodyless", "Always" });

            IntruderDeceptionSettings = new(MultiMenu.intruder, "<color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Deception</color> Settings");
            IDMax = new(num++, MultiMenu.intruder, "Max <color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Deception</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1);
            IDMin = new(num++, MultiMenu.intruder, "Min <color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Deception</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1);

            Disguiser = new(MultiMenu.intruder, "<color=#40B4FFFF>Disguiser</color>");
            DisguiserCount = new(num++, MultiMenu.intruder, "<color=#40B4FFFF>Disguiser</color> Count", 1, 1, 14, 1);
            UniqueDisguiser = new(num++, MultiMenu.intruder, "<color=#40B4FFFF>Disguiser</color> Is Unique In All Any", false);
            DisguiseCooldown = new(num++, MultiMenu.intruder, "Disguise Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            TimeToDisguise = new(num++, MultiMenu.intruder, "Delay Before Disguising", 5f, 2.5f, 15f, 2.5f, CooldownFormat);
            DisguiseDuration = new(num++, MultiMenu.intruder, "Disguise Duration", 10f, 2.5f, 20f, 2.5f, CooldownFormat);
            MeasureCooldown = new(num++, MultiMenu.intruder, "Measure Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            DisgCooldownsLinked = new(num++, MultiMenu.intruder, "Measure And Disguise Cooldowns Are Linked", false);
            DisguiseTarget = new(num++, MultiMenu.intruder, "<color=#40B4FFFF>Disguiser</color> Can Disguise", new[] { "Everyone", "Only Intruders", "Non Intruders" });

            Morphling = new(MultiMenu.intruder, "<color=#BB45B0FF>Morphling</color>");
            MorphlingCount = new(num++, MultiMenu.intruder, "<color=#BB45B0FF>Morphling</color> Count", 1, 1, 14, 1);
            UniqueMorphling = new(num++, MultiMenu.intruder, "<color=#BB45B0FF>Morphling</color> Is Unique In All Any", false);
            MorphlingCooldown = new(num++, MultiMenu.intruder, "Morph Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            MorphlingDuration = new(num++, MultiMenu.intruder, "Morph Duration", 10f, 5f, 30f, 1f, CooldownFormat);
            SampleCooldown = new(num++, MultiMenu.intruder, "Sample Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            MorphCooldownsLinked = new(num++, MultiMenu.intruder, "Sample And Morph Cooldowns Are Linked", false);
            MorphlingVent = new(num++, MultiMenu.intruder, "<color=#BB45B0FF>Morphling</color> Can Vent", false);

            Wraith = new(MultiMenu.intruder, "<color=#5C4F75FF>Wraith</color>");
            WraithCount = new(num++, MultiMenu.intruder, "<color=#5C4F75FF>Wraith</color> Count", 1, 1, 14, 1);
            UniqueWraith = new(num++, MultiMenu.intruder, "<color=#5C4F75FF>Wraith</color> Is Unique In All Any", false);
            InvisCooldown = new(num++, MultiMenu.intruder, "Invis Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            InvisDuration = new(num++, MultiMenu.intruder, "Invis Duration", 10f, 5f, 30f, 1f, CooldownFormat);
            WraithVent = new(num++, MultiMenu.intruder, "<color=#5C4F75FF>Wraith</color> Can Vent", false);

            IntruderKillingSettings = new(MultiMenu.intruder, "<color=#FF0000FF>Impostor</color> <color=#1D7CF2FF>Killing</color> Settings");
            IKMax = new(num++, MultiMenu.intruder, "Max <color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Killing</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1);
            IKMin = new(num++, MultiMenu.intruder, "Min <color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Killing</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1);

            Ambusher = new(MultiMenu.intruder, "<color=#2BD29CFF>Ambusher</color>");
            AmbusherCount = new(num++, MultiMenu.intruder, "<color=#2BD29CFF>Ambusher</color> Count", 1, 1, 14, 1);
            UniqueAmbusher = new(num++, MultiMenu.intruder, "<color=#2BD29CFF>Ambusher</color> Is Unique In All Any", false);
            AmbushCooldown = new(num++, MultiMenu.intruder, "Ambush Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            AmbushDuration = new(num++, MultiMenu.intruder, "Ambush Duration", 10f, 5f, 30f, 1f, CooldownFormat);
            AmbushMates = new(num++, MultiMenu.intruder, "<color=#2BD29CFF>Ambusher</color> Can Ambush Teammates", false);

            Enforcer = new(MultiMenu.intruder, "<color=#005643FF>Enforcer</color>");
            EnforcerCount = new(num++, MultiMenu.intruder, "<color=#005643FF>Enforcer</color> Count", 1, 1, 14, 1);
            UniqueEnforcer = new(num++, MultiMenu.intruder, "<color=#005643FF>Enforcer</color> Is Unique In All Any", false);
            EnforceCooldown = new(num++, MultiMenu.intruder, "Enforce Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            EnforceDuration = new(num++, MultiMenu.intruder, "Enforce Duration", 10f, 5f, 30f, 1f, CooldownFormat);
            EnforceDelay = new(num++, MultiMenu.intruder, "Enforce Delay", 5f, 1f, 15f, 1f, CooldownFormat);
            EnforceRadius = new(num++, MultiMenu.intruder, "Enforce Explosion Radius", 1.5f, 0.5f, 5f, 0.25f, DistanceFormat);

            IntruderSupportSettings = new(MultiMenu.intruder, "<color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Support</color> Settings");
            ISMax = new(num++, MultiMenu.intruder, "Max <color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Support</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1);
            ISMin = new(num++, MultiMenu.intruder, "Min <color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Support</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1);

            Consigliere = new(MultiMenu.intruder, "<color=#FFFF99FF>Consigliere</color>");
            ConsigCount = new(num++, MultiMenu.intruder, "<color=#FFFF99FF>Consigliere</color> Count", 1, 1, 14, 1);
            UniqueConsigliere = new(num++, MultiMenu.intruder, "<color=#FFFF99FF>Consigliere</color> Is Unique In All Any", false);
            InvestigateCooldown = new(num++, MultiMenu.intruder, "Investigate Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            ConsigInfo = new(num++, MultiMenu.intruder, "Info That <color=#FFFF99FF>Consigliere</color> Sees", new[] { "Role", "Faction" });

            Consort = new(MultiMenu.intruder, "<color=#801780FF>Consort</color>");
            ConsortCount = new(num++, MultiMenu.intruder, "<color=#801780FF>Consort</color> Count", 1, 1, 14, 1);
            UniqueConsort = new(num++, MultiMenu.intruder, "<color=#801780FF>Consort</color> Is Unique In All Any", false);
            ConsRoleblockCooldown = new(num++, MultiMenu.intruder, "Roleblock Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            ConsRoleblockDuration = new(num++, MultiMenu.intruder, "Roleblock Duration", 10f, 5f, 30f, 1f, CooldownFormat);

            Godfather = new(MultiMenu.intruder, "<color=#404C08FF>Godfather</color>");
            GodfatherCount = new(num++, MultiMenu.intruder, "<color=#404C08FF>Godfather</color> Count", 1, 1, 14, 1);
            UniqueGodfather = new(num++, MultiMenu.intruder, "<color=#404C08FF>Godfather</color> Is Unique In All Any", false);
            MafiosoAbilityCooldownDecrease = new(num++, MultiMenu.intruder, "Ability Cooldown Bonus", 0.75f, 0.25f, 0.9f, 0.05f, MultiplierFormat);

            Miner = new(MultiMenu.intruder, "<color=#AA7632FF>Miner</color>");
            MinerCount = new(num++, MultiMenu.intruder, "<color=#AA7632FF>Miner</color> Count", 1, 1, 14, 1);
            UniqueMiner = new(num++, MultiMenu.intruder, "<color=#AA7632FF>Miner</color> Is Unique In All Any", false);
            MineCooldown = new(num++, MultiMenu.intruder, "Mine Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);

            Teleporter = new(MultiMenu.intruder, "<color=#939593FF>Teleporter</color>");
            TeleporterCount = new(num++, MultiMenu.intruder, "<color=#939593FF>Teleporter</color> Count", 1, 1, 14, 1);
            UniqueTeleporter = new(num++, MultiMenu.intruder, "<color=#939593FF>Teleporter</color> Is Unique In All Any", false);
            TeleportCd = new(num++, MultiMenu.intruder, "Teleport Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            MarkCooldown = new(num++, MultiMenu.intruder, "Mark Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            TeleCooldownsLinked = new(num++, MultiMenu.intruder, "Mark And Teleport Cooldowns Are Linked", false);
            TeleVent = new(num++, MultiMenu.intruder, "<color=#939593FF>Teleporter</color> Can Vent", false);

            IntruderUtilitySettings = new(MultiMenu.intruder, "<color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Utility</color> Settings");

            Ghoul = new(MultiMenu.intruder, "<color=#F1C40FFF>Ghoul</color>");
            GhoulMarkCd = new(num++, MultiMenu.intruder, "Mark Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);

            Impostor = new(MultiMenu.intruder, "<color=#FF0000FF>Impostor</color>");
            ImpCount = new(num++, MultiMenu.intruder, "<color=#FF0000FF>Impostor</color> Count", 1, 1, 14, 1);

            SyndicateSettings = new(MultiMenu.syndicate, "<color=#008000FF>Syndicate</color> Settings");
            CustomSynColors = new(num++, MultiMenu.syndicate, "Enable Custom <color=#008000FF>Syndicate</color> Colors", true);
            SyndicateCount = new(num++, MultiMenu.syndicate, "<color=#008000FF>Syndicate</color> Count", 1, 0, 4, 1);
            SyndicateVision = new(num++, MultiMenu.syndicate, "<color=#008000FF>Syndicate</color> Vision", 2f, 0.25f, 5f, 0.25f, MultiplierFormat);
            //SyndicateFlashlight = new(num++, MultiMenu.syndicate, "<color=#008000FF>Syndicate</color> Uses A Flashlight", false);
            ChaosDriveMeetingCount = new(num++, MultiMenu.syndicate, "Chaos Drive Timer", 3, 1, 10, 1);
            ChaosDriveKillCooldown = new(num++, MultiMenu.syndicate, "Chaos Drive Holder Kill Cooldown", 15f, 10f, 45f, 2.5f, CooldownFormat);
            SyndicateVent = new(num++, MultiMenu.syndicate, "<color=#008000FF>Syndicate</color> Can Vent", new[] { "Always", "Chaos Drive", "Never" });
            AltImps = new(num++, MultiMenu.syndicate, "<color=#008000FF>Syndicate</color> Replaces <color=#FF0000FF>Intruders</color>", false);
            GlobalDrive = new(num++, MultiMenu.syndicate, "Chaos Drive Is Global", false);
            SyndicateMax = new(num++, MultiMenu.syndicate, "Max <color=#008000FF>Syndicate</color> <color=#FFD700FF>Roles</color>", 5, 1, 14, 1);
            SyndicateMin = new(num++, MultiMenu.syndicate, "Min <color=#008000FF>Syndicate</color> <color=#FFD700FF>Roles</color>", 5, 1, 14, 1);

            SyndicateDisruptionSettings = new(MultiMenu.syndicate, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Disruption</color> Settings");
            SDMax = new(num++, MultiMenu.syndicate, "Max <color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Disruption</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1);
            SDMin = new(num++, MultiMenu.syndicate, "Min <color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Disruption</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1);

            Concealer = new(MultiMenu.syndicate, "<color=#C02525FF>Concealer</color>");
            ConcealerCount = new(num++, MultiMenu.syndicate, "<color=#C02525FF>Concealer</color> Count", 1, 1, 14, 1);
            UniqueConcealer = new(num++, MultiMenu.syndicate, "<color=#C02525FF>Concealer</color> Is Unique In All Any", false);
            ConcealCooldown = new(num++, MultiMenu.syndicate, "Conceal Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            ConcealDuration = new(num++, MultiMenu.syndicate, "Conceal Duration", 10f, 5f, 30f, 1f, CooldownFormat);
            ConcealMates = new(num++, MultiMenu.syndicate, "<color=#C02525FF>Concealer</color> Can Conceal Teammates", false);

            Drunkard = new(MultiMenu.syndicate, "<color=#FF7900FF>Drunkard</color>");
            DrunkardCount = new(num++, MultiMenu.syndicate, "<color=#FF7900FF>Drunkard</color> Count", 1, 1, 14, 1);
            UniqueDrunkard = new(num++, MultiMenu.syndicate, "<color=#FF7900FF>Drunkard</color> Is Unique In All Any", false);
            ConfuseCooldown = new(num++, MultiMenu.syndicate, "Confuse Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            ConfuseDuration = new(num++, MultiMenu.syndicate, "Confuse Duration", 10f, 5f, 30f, 1f, CooldownFormat);
            ConfuseImmunity = new(num++, MultiMenu.syndicate, "<color=#008000FF>Syndicate</color> Are Immune To Confuse", true);

            Framer = new(MultiMenu.syndicate, "<color=#00FFFFFF>Framer</color>");
            FramerCount = new(num++, MultiMenu.syndicate, "<color=#00FFFFFF>Framer</color> Count", 1, 1, 14, 1);
            UniqueFramer = new(num++, MultiMenu.syndicate, "<color=#00FFFFFF>Framer</color> Is Unique In All Any", false);
            FrameCooldown = new(num++, MultiMenu.syndicate, "Frame Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            ChaosDriveFrameRadius = new(num++, MultiMenu.syndicate, "Chaos Drive Frame Radius", 1.5f, 0.5f, 5f, 0.25f, DistanceFormat);

            Shapeshifter = new(MultiMenu.syndicate, "<color=#2DFF00FF>Shapeshifter</color>");
            ShapeshifterCount = new(num++, MultiMenu.syndicate, "<color=#2DFF00FF>Shapeshifter</color> Count", 1, 1, 14, 1);
            UniqueShapeshifter = new(num++, MultiMenu.syndicate, "<color=#2DFF00FF>Shapeshifter</color> Is Unique In All Any", false);
            ShapeshiftCooldown = new(num++, MultiMenu.syndicate, "Shapeshift Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            ShapeshiftDuration = new(num++, MultiMenu.syndicate, "Shapeshift Duration", 10f, 5f, 30f, 1f, CooldownFormat);
            ShapeshiftMates = new(num++, MultiMenu.syndicate, "<color=#2DFF00FF>Shapeshifter</color> Can Shapeshift Teammates", false);

            Silencer = new(MultiMenu.syndicate, "<color=#AAB43EFF>Silencer</color>");
            SilencerCount = new(num++, MultiMenu.syndicate, "<color=#AAB43EFF>Silencer</color> Count", 1, 1, 14, 1);
            UniqueSilencer = new(num++, MultiMenu.syndicate, "<color=#AAB43EFF>Silencer</color> Is Unique In All Any", false);
            SilenceCooldown = new(num++, MultiMenu.syndicate, "Silence Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            WhispersNotPrivateSilencer = new(num++, MultiMenu.syndicate, "<color=#AAB43EFF>Silencer</color> Can Read Whispers", true);
            SilenceMates = new(num++, MultiMenu.syndicate, "<color=#AAB43EFF>Silencer</color> Can Silence Teammates", false);
            SilenceRevealed = new(num++, MultiMenu.syndicate, "Silence Is Revealed To Everyone", true);

            SyndicateKillingSettings = new(MultiMenu.syndicate, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Killing</color> Settings");
            SyKMax = new(num++, MultiMenu.syndicate, "Max <color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Killing</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1);
            SyKMin = new(num++, MultiMenu.syndicate, "Min <color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Killing</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1);

            Bomber = new(MultiMenu.syndicate, "<color=#C9CC3FFF>Bomber</color>");
            BomberCount = new(num++, MultiMenu.syndicate, "<color=#C9CC3FFF>Bomber</color> Count", 1, 1, 14, 1);
            UniqueBomber = new(num++, MultiMenu.syndicate, "<color=#C9CC3FFF>Bomber</color> Is Unique In All Any", false);
            BombCooldown = new(num++, MultiMenu.syndicate, "Bomb Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            DetonateCooldown = new(num++, MultiMenu.syndicate, "Detonation Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            BombCooldownsLinked = new(num++, MultiMenu.syndicate, "Place And Detonate Cooldowns Are Linked", false);
            BombsRemoveOnNewRound = new(num++, MultiMenu.syndicate, "Bombs Are Cleared Every Meeting", false);
            BombsDetonateOnMeetingStart = new(num++, MultiMenu.syndicate, "Bombs Detonate Everytime A Meeting Is Called", false);
            BombRange = new(num++, MultiMenu.syndicate, "Bomb Radius", 1.5f, 0.5f, 5f, 0.25f, DistanceFormat);
            ChaosDriveBombRange = new(num++, MultiMenu.syndicate, "Chaos Drive Bomb Radius Increase", 0.5f, 0.5f, 5f, 0.25f, DistanceFormat);
            BombKillsSyndicate = new(num++, MultiMenu.syndicate, "Bomb Detonation Kills Members Of The <color=#008000FF>Syndicate</color>", true);

            Collider = new(MultiMenu.syndicate, "<color=#B345FFFF>Collider</color>");
            ColliderCount = new(num++, MultiMenu.syndicate, "<color=#B345FFFF>Collider</color> Count", 1, 1, 14, 1);
            UniqueCollider = new(num++, MultiMenu.syndicate, "<color=#B345FFFF>Collider</color> Is Unique In All Any", false);
            CollideCooldown = new(num++, MultiMenu.syndicate, "Collide Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            CollideRange = new(num++, MultiMenu.syndicate, "Collide Radius", 1.5f, 0.5f, 5f, 0.25f, DistanceFormat);
            CollideRangeIncrease = new(num++, MultiMenu.syndicate, "Chaos Drive Collide Radius Increase", 0.5f, 0.5f, 5f, 0.25f, DistanceFormat);

            Crusader = new(MultiMenu.syndicate, "<color=#DF7AE8FF>Crusader</color>");
            CrusaderCount = new(num++, MultiMenu.syndicate, "<color=#DF7AE8FF>Crusader</color> Count", 1, 1, 14, 1);
            UniqueCrusader = new(num++, MultiMenu.syndicate, "<color=#DF7AE8FF>Crusader</color> Is Unique In All Any", false);
            CrusadeCooldown = new(num++, MultiMenu.syndicate, "Crusade Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            CrusadeDuration = new(num++, MultiMenu.syndicate, "Crusade Duration", 10f, 5f, 30f, 1f, CooldownFormat);
            ChaosDriveCrusadeRadius = new(num++, MultiMenu.syndicate, "Chaos Drive Crusade Radius", 1.5f, 0.5f, 5f, 0.25f, DistanceFormat);
            CrusadeMates = new(num++, MultiMenu.syndicate, "<color=#DF7AE8FF>Crusader</color> Can Crusade Teammates", false);

            Poisoner = new(MultiMenu.syndicate, "<color=#B5004CFF>Poisoner</color>");
            PoisonerCount = new(num++, MultiMenu.syndicate, "<color=#B5004CFF>Poisoner</color> Count", 1, 1, 14, 1);
            UniquePoisoner = new(num++, MultiMenu.syndicate, "<color=#B5004CFF>Poisoner</color> Is Unique In All Any", false);
            PoisonCooldown = new(num++, MultiMenu.syndicate, "Poison Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            PoisonDuration = new(num++, MultiMenu.syndicate, "Poison Kill Delay", 5f, 1f, 15f, 1f, CooldownFormat);

            SyndicatePowerSettings = new(MultiMenu.syndicate, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Power</color> Settings");
            SPMax = new(num++, MultiMenu.syndicate, "Max <color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Power</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1);
            SPMin = new(num++, MultiMenu.syndicate, "Min <color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Power</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1);

            Spellslinger = new(MultiMenu.syndicate, "<color=#0028F5FF>Spellslinger</color>");
            SpellslingerCount = new(num++, MultiMenu.syndicate, "<color=#0028F5FF>Spellslinger</color> Count", 1, 1, 14, 1);
            UniqueSpellslinger = new(num++, MultiMenu.syndicate, "<color=#0028F5FF>Spellslinger</color> Is Unique In All Any", false);
            SpellCooldown = new(num++, MultiMenu.syndicate, "Spell Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            SpellCooldownIncrease = new(num++, MultiMenu.syndicate, "Spell Cooldown Increase", 5f, 2.5f, 30f, 2.5f, CooldownFormat);

            TimeKeeper = new(MultiMenu.syndicate, "<color=#3769FEFF>Time Keeper</color>");
            TimeKeeperCount = new(num++, MultiMenu.syndicate, "<color=#3769FEFF>Time Keeper</color> Count", 1, 1, 14, 1);
            UniqueTimeKeeper = new(num++, MultiMenu.syndicate, "<color=#3769FEFF>Time Keeper</color> Is Unique In All Any", false);
            TimeControlCooldown = new(num++, MultiMenu.syndicate, "Time Control Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            TimeControlDuration = new(num++, MultiMenu.syndicate, "Time Control Duration", 10f, 5f, 30f, 1f, CooldownFormat);
            TimeFreezeImmunity = new(num++, MultiMenu.syndicate, "<color=#008000FF>Syndicate</color> Are Immune To Freeze", true);
            TimeRewindImmunity = new(num++, MultiMenu.syndicate, "<color=#008000FF>Syndicate</color> Are Immune To Rewind", true);

            SyndicateSupportSettings = new(MultiMenu.syndicate, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Support</color> Settings");
            SSuMax = new(num++, MultiMenu.syndicate, "Max <color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Support</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1);
            SSuMin = new(num++, MultiMenu.syndicate, "Min <color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Support</color> <color=#FFD700FF>Roles</color>", 1, 1, 14, 1);

            Rebel = new(MultiMenu.syndicate, "<color=#FFFCCEFF>Rebel</color>");
            RebelCount = new(num++, MultiMenu.syndicate, "<color=#FFFCCEFF>Rebel</color> Count", 1, 1, 14, 1);
            UniqueRebel = new(num++, MultiMenu.syndicate, "<color=#FFFCCEFF>Rebel</color> Is Unique In All Any", false);
            SidekickAbilityCooldownDecrease = new(num++, MultiMenu.syndicate, "Ability Cooldown Bonus", 0.75f, 0.25f, 0.9f, 0.05f, MultiplierFormat);

            Stalker = new(MultiMenu.syndicate, "<color=#7E4D00FF>Stalker</color>");
            StalkerCount = new(num++, MultiMenu.syndicate, "<color=#7E4D00FF>Stalker</color> Count", 1, 1, 14, 1);
            UniqueStalker = new(num++, MultiMenu.syndicate, "<color=#7E4D00FF>Stalker</color> Is Unique In All Any", false);
            StalkCooldown = new(num++, MultiMenu.syndicate, "Stalk Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);

            Warper = new(MultiMenu.syndicate, "<color=#8C7140FF>Warper</color>");
            WarperCount = new(num++, MultiMenu.syndicate, "<color=#8C7140FF>Warper</color> Count", 1, 1, 14, 1);
            UniqueWarper = new(num++, MultiMenu.syndicate, "<color=#8C7140FF>Warper</color> Is Unique In All Any", false);
            WarpCooldown = new(num++, MultiMenu.syndicate, "Warp Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            WarpDuration = new(num++, MultiMenu.syndicate, "Warp Duration", 5f, 1f, 20f, 1f, CooldownFormat);
            WarpSelf = new(num++, MultiMenu.syndicate, "<color=#8C7140FF>Warper</color> Can Warp Themselves", true);

            SyndicateUtilitySettings = new(MultiMenu.syndicate, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Utility</color> Settings");

            Anarchist = new(MultiMenu.syndicate, "<color=#008000FF>Anarchist</color>");
            AnarchistCount = new(num++, MultiMenu.syndicate, "<color=#008000FF>Anarchist</color> Count", 1, 1, 14, 1);
            AnarchKillCooldown = new(num++, MultiMenu.syndicate, "Kill Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);

            Banshee = new(MultiMenu.syndicate, "<color=#E67E22FF>Banshee</color>");
            ScreamCooldown = new(num++, MultiMenu.syndicate, "Scream Cooldown", 25f, 10f, 60f, 1f, CooldownFormat);
            ScreamDuration = new(num++, MultiMenu.syndicate, "Scream Duration", 10f, 5f, 30f, 1f, CooldownFormat);

            ModifierSettings = new(MultiMenu.modifier, "<color=#7F7F7FFF>Modifier</color> Settings");
            CustomModifierColors = new(num++, MultiMenu.modifier, "Enable Custom <color=#7F7F7FFF>Modifier</color> Colors", true);
            MaxModifiers = new(num++, MultiMenu.modifier, "Max <color=#7F7F7FFF>Modifiers</color>", 5, 1, 14, 1);
            MinModifiers = new(num++, MultiMenu.modifier, "Min <color=#7F7F7FFF>Modifiers</color>", 5, 1, 14, 1);

            Astral = new(MultiMenu.modifier, "<color=#612BEFFF>Astral</color>");
            AstralCount = new(num++, MultiMenu.modifier, "<color=#612BEFFF>Astral</color> Count", 1, 1, 14, 1);
            UniqueAstral = new(num++, MultiMenu.modifier, "<color=#612BEFFF>Astral</color> Is Unique In All Any", false);

            Bait = new(MultiMenu.modifier, "<color=#00B3B3FF>Bait</color>");
            BaitCount = new(num++, MultiMenu.modifier, "<color=#00B3B3FF>Bait</color> Count", 1, 1, 14, 1);
            UniqueBait = new(num++, MultiMenu.modifier, "<color=#00B3B3FF>Bait</color> Is Unique In All Any", false);
            BaitKnows = new(num++, MultiMenu.modifier, "<color=#00B3B3FF>Bait</color> Knows Who They Are", true);
            BaitMinDelay = new(num++, MultiMenu.modifier, "Minimum Delay for <color=#00B3B3FF>Bait</color> Self Report", 0f, 0f, 15f, 0.5f, CooldownFormat);
            BaitMaxDelay = new(num++, MultiMenu.modifier, "Maximum Delay for <color=#00B3B3FF>Bait</color> Self Report", 1f, 0f, 15f, 0.5f, CooldownFormat);

            Coward = new(MultiMenu.modifier, "<color=#456BA8FF>Coward</color>");
            CowardCount = new(num++, MultiMenu.modifier, "<color=#456BA8FF>Coward</color> Count", 1, 1, 14, 1);
            UniqueCoward = new(num++, MultiMenu.modifier, "<color=#456BA8FF>Coward</color> Is Unique In All Any", false);

            Diseased = new(MultiMenu.modifier, "<color=#374D1EFF>Diseased</color>");
            DiseasedCount = new(num++, MultiMenu.modifier, "<color=#374D1EFF>Diseased</color> Count", 1, 1, 14, 1);
            UniqueDiseased = new(num++, MultiMenu.modifier, "<color=#374D1EFF>Diseased</color> Is Unique In All Any", false);
            DiseasedKnows = new(num++, MultiMenu.modifier, "<color=#374D1EFF>Diseased</color> Knows Who They Are", true);
            DiseasedKillMultiplier = new(num++, MultiMenu.modifier, "<color=#374D1EFF>Diseased</color> Kill Multiplier", 3f, 1.5f, 5f, 0.5f, MultiplierFormat);

            Drunk = new(MultiMenu.modifier, "<color=#758000FF>Drunk</color>");
            DrunkCount = new(num++, MultiMenu.modifier, "<color=#758000FF>Drunk</color> Count", 1, 1, 14, 1);
            UniqueDrunk = new(num++, MultiMenu.modifier, "<color=#758000FF>Drunk</color> Is Unique In All Any", false);
            DrunkControlsSwap = new(num++, MultiMenu.modifier, "Controls Reverse Over Time", false);
            DrunkInterval = new(num++, MultiMenu.modifier, "Reversed Controls Interval", 10f, 1f, 20f, 1f, CooldownFormat);

            Dwarf = new(MultiMenu.modifier, "<color=#FF8080FF>Dwarf</color>");
            DwarfCount = new(num++, MultiMenu.modifier, "<color=#FF8080FF>Dwarf</color> Count", 1, 1, 14, 1);
            UniqueDwarf = new(num++, MultiMenu.modifier, "<color=#FF8080FF>Dwarf</color> Is Unique In All Any", false);
            DwarfSpeed = new(num++, MultiMenu.modifier, "<color=#FF8080FF>Dwarf</color> Speed", 1.5f, 1f, 2f, 0.05f, MultiplierFormat);
            DwarfScale = new(num++, MultiMenu.modifier, "<color=#FF8080FF>Dwarf</color> Scale", 0.5f, 0.3f, 1f, 0.025f, MultiplierFormat);

            Flincher = new(MultiMenu.modifier, "<color=#80B3FFFF>Flincher</color>");
            FlincherCount = new(num++, MultiMenu.modifier, "<color=#80B3FFFF>Flincher</color> Count", 1, 1, 14, 1);
            UniqueFlincher = new(num++, MultiMenu.modifier, "<color=#80B3FFFF>Flincher</color> Is Unique In All Any", false);
            FlinchInterval = new(num++, MultiMenu.modifier, "Flinch Interval", 10f, 1f, 20f, 1f, CooldownFormat);

            Giant = new(MultiMenu.modifier, "<color=#FFB34DFF>Giant</color>");
            GiantCount = new(num++, MultiMenu.modifier, "<color=#FFB34DFF>Giant</color> Count", 1, 1, 14, 1);
            UniqueGiant = new(num++, MultiMenu.modifier, "<color=#FFB34DFF>Giant</color> Is Unique In All Any", false);
            GiantSpeed = new(num++, MultiMenu.modifier, "<color=#FFB34DFF>Giant</color> Speed", 0.75f, 0.5f, 1f, 0.05f, MultiplierFormat);
            GiantScale = new(num++, MultiMenu.modifier, "<color=#FFB34DFF>Giant</color> Scale", 1.5f, 1f, 3.0f, 0.025f, MultiplierFormat);

            Indomitable = new(MultiMenu.modifier, "<color=#2DE5BEFF>Indomitable</color>");
            IndomitableCount = new(num++, MultiMenu.modifier, "<color=#2DE5BEFF>Indomitable</color> Count", 1, 1, 14, 1);
            UniqueIndomitable = new(num++, MultiMenu.modifier, "<color=#2DE5BEFF>Indomitable</color> Is Unique In All Any", false);
            IndomitableKnows = new(num++, MultiMenu.modifier, "<color=#2DE5BEFF>Indomitable</color> Knows Who They Are", true);

            Professional = new(MultiMenu.modifier, "<color=#860B7AFF>Professional</color>");
            ProfessionalCount = new(num++, MultiMenu.modifier, "<color=#860B7AFF>Professional</color> Count", 1, 1, 14, 1);
            UniqueProfessional = new(num++, MultiMenu.modifier, "<color=#860B7AFF>Professional</color> Is Unique In All Any", false);
            ProfessionalKnows = new(num++, MultiMenu.modifier, "<color=#860B7AFF>Professional</color> Knows Who They Are", true);

            Shy = new(MultiMenu.modifier, "<color=#1002C5FF>Shy</color>");
            ShyCount = new(num++, MultiMenu.modifier, "<color=#1002C5FF>Shy</color> Count", 1, 1, 14, 1);
            UniqueShy = new(num++, MultiMenu.modifier, "<color=#1002C5FF>Shy</color> Is Unique In All Any", false);

            VIP = new(MultiMenu.modifier, "<color=#DCEE85FF>VIP</color>");
            VIPCount = new(num++, MultiMenu.modifier, "<color=#DCEE85FF>VIP</color> Count", 1, 1, 14, 1);
            UniqueVIP = new(num++, MultiMenu.modifier, "<color=#DCEE85FF>VIP</color> Is Unique In All Any", false);
            VIPKnows = new(num++, MultiMenu.modifier, "<color=#DCEE85FF>VIP</color> Knows Who They Are", true);

            Volatile = new(MultiMenu.modifier, "<color=#FFA60AFF>Volatile</color>");
            VolatileCount = new(num++, MultiMenu.modifier, "<color=#FFA60AFF>Volatile</color> Count", 1, 1, 14, 1);
            UniqueVolatile = new(num++, MultiMenu.modifier, "<color=#FFA60AFF>Volatile</color> Is Unique In All Any", false);
            VolatileInterval = new(num++, MultiMenu.modifier, "<color=#FFA60AFF>Volatile</color> Interval", 15f, 10f, 30f, 1f, CooldownFormat);
            VolatileKnows = new(num++, MultiMenu.modifier, "<color=#FFA60AFF>Volatile</color> Knows Who They Are", true);

            Yeller = new(MultiMenu.modifier, "<color=#F6AAB7FF>Yeller</color>");
            YellerCount = new(num++, MultiMenu.modifier, "<color=#F6AAB7FF>Yeller</color> Count", 1, 1, 14, 1);
            UniqueYeller = new(num++, MultiMenu.modifier, "<color=#F6AAB7FF>Yeller</color> Is Unique In All Any", false);

            AbilitySettings = new(MultiMenu.ability, "<color=#FF9900FF>Ability</color> Settings");
            CustomAbilityColors = new(num++, MultiMenu.ability, "Enable Custom <color=#FF9900FF>Ability</color> Colors", true);
            MaxAbilities = new(num++, MultiMenu.ability, "Max <color=#FF9900FF>Abilities</color>", 5, 1, 14, 1);
            MinAbilities = new(num++, MultiMenu.ability, "Min <color=#FF9900FF>Abilities</color>", 5, 1, 14, 1);

            Assassin = new(MultiMenu.ability, "<color=#073763FF>Assassin</color>");
            NumberOfImpostorAssassins = new(num++, MultiMenu.ability, "<color=#FF0000FF>Intruder</color> <color=#073763FF>Assassins</color> Count", 1, 1, 14, 1);
            NumberOfCrewAssassins = new(num++, MultiMenu.ability, "<color=#8CFFFFFF>Crew</color> <color=#073763FF>Assassins</color> Count", 1, 1, 14, 1);
            NumberOfNeutralAssassins = new(num++, MultiMenu.ability, "<color=#B3B3B3FF>Neutral</color> <color=#073763FF>Assassins</color> Count", 1, 1, 14, 1);
            NumberOfSyndicateAssassins = new(num++, MultiMenu.ability, "<color=#008000FF>Syndicate</color> <color=#073763FF>Assassins</color> Count", 1, 1, 14, 1);
            UniqueAssassin = new(num++, MultiMenu.ability, "<color=#073763FF>Assassin</color> Is Unique In All Any", false);
            AssassinKills = new(num++, MultiMenu.ability, "<color=#073763FF>Assassin</color> Guess Limit", 1, 1, 15, 1);
            AssassinMultiKill = new(num++, MultiMenu.ability, "<color=#073763FF>Assassin</color> Can Kill More Than Once Per Meeting", false);
            AssassinGuessNeutralBenign = new(num++, MultiMenu.ability, "<color=#073763FF>Assassin</color> Can Guess <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Benigns</color>", false);
            AssassinGuessNeutralEvil = new(num++, MultiMenu.ability, "<color=#073763FF>Assassin</color> Can Guess <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Evils</color>", false);
            AssassinGuessPest = new(num++, MultiMenu.ability, "<color=#073763FF>Assassin</color> Can Guess <color=#424242FF>Pestilence</color>", false);
            AssassinGuessModifiers = new(num++, MultiMenu.ability, "<color=#073763FF>Assassin</color> Can Guess Select <color=#7F7F7FFF>Modifiers</color>", false);
            AssassinGuessObjectifiers = new(num++, MultiMenu.ability, "<color=#073763FF>Assassin</color> Can Guess Select <color=#DD585BFF>Objectifiers</color>", false);
            AssassinGuessAbilities = new(num++, MultiMenu.ability, "<color=#073763FF>Assassin</color> Can Guess <color=#FF9900FF>Abilities</color>", false);
            AssassinGuessInvestigative = new(num++, MultiMenu.ability, "<color=#073763FF>Assassin</color> Can Guess <color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Investigatives</color>", false);
            AssassinateAfterVoting = new(num++, MultiMenu.ability, "<color=#073763FF>Assassin</color> Can Guess After Voting", false);

            ButtonBarry = new(MultiMenu.ability, "<color=#E600FFFF>Button Barry</color>");
            ButtonBarryCount = new(num++, MultiMenu.ability, "<color=#E600FFFF>Button Barry</color> Count", 1, 1, 14, 1);
            UniqueButtonBarry = new(num++, MultiMenu.ability, "<color=#E600FFFF>Button Barry</color> Is Unique In All Any", false);
            ButtonCooldown = new(num++, MultiMenu.ability, "Button Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);

            Insider = new(MultiMenu.ability, "<color=#26FCFBFF>Insider</color>");
            InsiderCount = new(num++, MultiMenu.ability, "<color=#26FCFBFF>Insider</color> Count", 1, 1, 14, 1);
            UniqueInsider = new(num++, MultiMenu.ability, "<color=#26FCFBFF>Insider</color> Is Unique In All Any", false);
            InsiderKnows = new(num++, MultiMenu.ability, "<color=#26FCFBFF>Insider</color> Knows Who They Are", true);

            Multitasker = new(MultiMenu.ability, "<color=#FF804DFF>Multitasker</color>");
            MultitaskerCount = new(num++, MultiMenu.ability, "<color=#FF804DFF>Multitasker</color> Count", 1, 1, 14, 1);
            UniqueMultitasker = new(num++, MultiMenu.ability, "<color=#FF804DFF>Multitasker</color> Is Unique In All Any", false);
            Transparancy = new(num++, MultiMenu.ability, "Task Transparancy", 50f, 10f, 80f, 5f, PercentFormat);

            Ninja = new(MultiMenu.ability, "<color=#A84300FF>Ninja</color>");
            NinjaCount = new(num++, MultiMenu.ability, "<color=#A84300FF>Ninja</color> Count", 1, 1, 14, 1);
            UniqueNinja = new(num++, MultiMenu.ability, "<color=#A84300FF>Ninja</color> Is Unique In All Any", false);

            Politician = new(MultiMenu.ability, "<color=#CCA3CCFF>Politician</color>");
            PoliticianCount = new(num++, MultiMenu.ability, "<color=#CCA3CCFF>Politician</color> Count", 1, 1, 14, 1);
            UniquePolitician = new(num++, MultiMenu.ability, "<color=#CCA3CCFF>Politician</color> Is Unique In All Any", false);
            PoliticianVoteBank = new(num++, MultiMenu.ability, "Initial <color=#CCA3CCFF>Politician</color> Initial Vote Bank", 0, 0, 10, 1);
            PoliticianAnonymous = new(num++, MultiMenu.ability, "Anonymous <color=#CCA3CCFF>Politician</color> Votes", false);
            PoliticianButton = new(num++, MultiMenu.ability, "<color=#CCA3CCFF>Politician</color> Can Button", true);

            Radar = new(MultiMenu.ability, "<color=#FF0080FF>Radar</color>");
            RadarCount = new(num++, MultiMenu.ability, "<color=#FF0080FF>Radar</color> Count", 1, 1, 14, 1);
            UniqueRadar = new(num++, MultiMenu.ability, "<color=#FF0080FF>Radar</color> Is Unique In All Any", false);

            Ruthless = new(MultiMenu.ability, "<color=#2160DDFF>Ruthless</color>");
            RuthlessCount = new(num++, MultiMenu.ability, "<color=#2160DDFF>Ruthless</color> Count", 1, 1, 14, 1);
            UniqueRuthless = new(num++, MultiMenu.ability, "<color=#2160DDFF>Ruthless</color> Is Unique In All Any", false);
            RuthlessKnows = new(num++, MultiMenu.ability, "<color=#2160DDFF>Ruthless</color> Knows Who They Are", true);

            Snitch = new(MultiMenu.ability, "<color=#D4AF37FF>Snitch</color>");
            SnitchCount = new(num++, MultiMenu.ability, "<color=#D4AF37FF>Snitch</color> Count", 1, 1, 14, 1);
            UniqueSnitch = new(num++, MultiMenu.ability, "<color=#D4AF37FF>Snitch</color> Is Unique In All Any", false);
            SnitchKnows = new(num++, MultiMenu.ability, "<color=#D4AF37FF>Snitch</color> Knows Who They Are", true);
            SnitchSeesNeutrals = new(num++, MultiMenu.ability, "<color=#D4AF37FF>Snitch</color> Sees <color=#B3B3B3FF>Neutrals</color>", false);
            SnitchSeesCrew = new(num++, MultiMenu.ability, "<color=#D4AF37FF>Snitch</color> Sees <color=#8CFFFFFF>Crew</color>", false);
            SnitchSeesRoles = new(num++, MultiMenu.ability, "<color=#D4AF37FF>Snitch</color> Sees Exact <color=#FFD700FF>Roles</color>", false);
            SnitchTasksRemaining = new(num++, MultiMenu.ability, "Tasks Remaining When Revealed", 1, 1, 5, 1);
            SnitchSeestargetsInMeeting = new(num++, MultiMenu.ability, "<color=#D4AF37FF>Snitch</color> Sees Evils In Meetings", true);

            Swapper = new(MultiMenu.ability, "<color=#66E666FF>Swapper</color>");
            SwapperCount = new(num++, MultiMenu.ability, "<color=#66E666FF>Swapper</color> Count", 1, 1, 14, 1);
            UniqueSwapper = new(num++, MultiMenu.ability, "<color=#66E666FF>Swapper</color> Is Unique In All Any", false);
            SwapperButton = new(num++, MultiMenu.ability, "<color=#66E666FF>Swapper</color> Can Button", true);
            SwapAfterVoting = new(num++, MultiMenu.ability, "<color=#66E666FF>Swapper</color> Can Swap After Voting", false);
            SwapSelf = new(num++, MultiMenu.ability, "<color=#66E666FF>Swapper</color> Can Swap Themself", true);

            Tiebreaker = new(MultiMenu.ability, "<color=#99E699FF>Tiebreaker</color>");
            TiebreakerCount = new(num++, MultiMenu.ability, "<color=#99E699FF>Tiebreaker</color> Count", 1, 1, 14, 1);
            UniqueTiebreaker = new(num++, MultiMenu.ability, "<color=#99E699FF>Tiebreaker</color> Is Unique In All Any", false);
            TiebreakerKnows = new(num++, MultiMenu.ability, "<color=#99E699FF>Tiebreaker</color> Knows Who They Are", true);

            Torch = new(MultiMenu.ability, "<color=#FFFF99FF>Torch</color>");
            TorchCount = new(num++, MultiMenu.ability, "<color=#FFFF99FF>Torch</color> Count", 1, 1, 14, 1);
            UniqueTorch = new(num++, MultiMenu.ability, "<color=#FFFF99FF>Torch</color> Is Unique In All Any", false);

            Tunneler = new(MultiMenu.ability, "<color=#E91E63FF>Tunneler</color>");
            TunnelerCount = new(num++, MultiMenu.ability, "<color=#E91E63FF>Tunneler</color> Count", 1, 1, 14, 1);
            UniqueTunneler = new(num++, MultiMenu.ability, "<color=#E91E63FF>Tunneler</color> Is Unique In All Any", false);
            TunnelerKnows = new(num++, MultiMenu.ability, "<color=#E91E63FF>Tunneler</color> Knows Who They Are", true);

            Underdog = new(MultiMenu.ability, "<color=#841A7FFF>Underdog</color>");
            UnderdogCount = new(num++, MultiMenu.ability, "<color=#841A7FFF>Underdog</color> Count", 1, 1, 14, 1);
            UniqueUnderdog = new(num++, MultiMenu.ability, "<color=#841A7FFF>Underdog</color> Is Unique In All Any", false);
            UnderdogKnows = new(num++, MultiMenu.ability, "<color=#841A7FFF>Underdog</color> Knows Who They Are", true);
            UnderdogKillBonus = new(num++, MultiMenu.ability, "Kill Cooldown Bonus", 5f, 2.5f, 30f, 2.5f, CooldownFormat);
            UnderdogIncreasedKC = new(num++, MultiMenu.ability, "Increased Kill Cooldown When 2+ Teammates", true);

            ObjectifierSettings = new(MultiMenu.objectifier, "<color=#DD585BFF>Objectifier</color> Settings");
            CustomObjectifierColors = new(num++, MultiMenu.objectifier, "Enable Custom <color=#DD585BFF>Objectifier</color> Colors", true);
            MaxObjectifiers = new(num++, MultiMenu.objectifier, "Max <color=#DD585BFF>Objectifiers</color>", 5, 1, 14, 1);
            MinObjectifiers = new(num++, MultiMenu.objectifier, "Min <color=#DD585BFF>Objectifiers</color>", 5, 1, 14, 1);

            Allied = new(MultiMenu.objectifier, "<color=#4545A9FF>Allied</color>");
            AlliedCount = new(num++, MultiMenu.objectifier, "<color=#4545A9FF>Allied</color> Count", 1, 1, 14, 1);
            UniqueAllied = new(num++, MultiMenu.objectifier, "<color=#4545A9FF>Allied</color> Is Unique In All Any", false);
            AlliedFaction = new(num++, MultiMenu.objectifier, "<color=#4545A9FF>Allied</color> Faction", new[] { "Random", "Intruder", "Syndicate", "Crew" });

            Corrupted = new(MultiMenu.objectifier, "<color=#4545FFFF>Corrupted</color>");
            CorruptedCount = new(num++, MultiMenu.objectifier, "<color=#4545FFFF>Corrupted</color> Count", 1, 1, 14, 1);
            UniqueCorrupted = new(num++, MultiMenu.objectifier, "<color=#4545FFFF>Corrupted</color> Is Unique In All Any", false);
            CorruptedKillCooldown = new(num++, MultiMenu.objectifier, "Corrupt Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            AllCorruptedWin = new(num++, MultiMenu.objectifier, "All <color=#4545FFFF>Corrupted</color> Win Together", false);
            CorruptedVent = new(num++, MultiMenu.objectifier, "<color=#4545FFFF>Corrupted</color> Can Vent", false);

            Defector = new(MultiMenu.objectifier, "<color=#E1C849FF>Defector</color>");
            DefectorCount = new(num++, MultiMenu.objectifier, "<color=#E1C849FF>Defector</color> Count", 1, 1, 14, 1);
            UniqueDefector = new(num++, MultiMenu.objectifier, "<color=#E1C849FF>Defector</color> Is Unique In All Any", false);
            DefectorKnows = new(num++, MultiMenu.objectifier, "<color=#E1C849FF>Defector</color> Knows Who They Are", true);
            DefectorFaction = new(num++, MultiMenu.objectifier, "<color=#E1C849FF>Defector</color> Faction", new[] { "Random", "Opposing Evil", "Crew" });

            Fanatic = new(MultiMenu.objectifier, "<color=#678D36FF>Fanatic</color>");
            FanaticCount = new(num++, MultiMenu.objectifier, "<color=#678D36FF>Fanatic</color> Count", 1, 1, 14, 1);
            UniqueFanatic = new(num++, MultiMenu.objectifier, "<color=#678D36FF>Fanatic</color> Is Unique In All Any", false);
            FanaticKnows = new(num++, MultiMenu.objectifier, "<color=#678D36FF>Fanatic</color> Knows Who They Are", true);
            FanaticColourSwap = new(num++, MultiMenu.objectifier, "Turned <color=#678D36FF>Fanatic</color> Swaps Colours For Investigative <color=#FFD700FF>Roles</color>", false);
            SnitchSeesFanatic = new(num++, MultiMenu.objectifier, "<color=#D4AF37FF>Snitch</color> Sees Turned <color=#678D36FF>Fanatic</color>", true);
            RevealerRevealsFanatic = new(num++, MultiMenu.objectifier, "<color=#D3D3D3FF>Revealer</color> Reveals Turned <color=#678D36FF>Fanatic</color>", false);

            Lovers = new(MultiMenu.objectifier, "<color=#FF66CCFF>Lovers</color>");
            LoversCount = new(num++, MultiMenu.objectifier, "<color=#FF66CCFF>Lovers</color> Count", 1, 1, 14, 1);
            UniqueLovers = new(num++, MultiMenu.objectifier, "<color=#FF66CCFF>Lovers</color> Is Unique In All Any", false);
            BothLoversDie = new(num++, MultiMenu.objectifier, "Both <color=#FF66CCFF>Lovers</color> Die", true);
            LoversChat = new(num++, MultiMenu.objectifier, "Enable <color=#FF66CCFF>Lovers</color> Chat", true);
            LoversFaction = new(num++, MultiMenu.objectifier, "<color=#FF66CCFF>Lovers</color> Can Be From The Same Faction", true);
            LoversRoles = new(num++, MultiMenu.objectifier, "<color=#FF66CCFF>Lovers</color> Know Each Other's <color=#FFD700FF>Roles</color>", true);

            Mafia = new(MultiMenu.objectifier, "<color=#00EEFFFF>Mafia</color>");
            MafiaCount = new(num++, MultiMenu.objectifier, "<color=#00EEFFFF>Mafia</color> Count", 2, 2, 14, 1);
            UniqueMafia = new(num++, MultiMenu.objectifier, "<color=#00EEFFFF>Mafia</color> Is Unique In All Any", false);
            MafiaRoles = new(num++, MultiMenu.objectifier, "<color=#00EEFFFF>Mafia</color> Know Each Other's <color=#FFD700FF>Roles</color>", true);
            MafVent = new(num++, MultiMenu.objectifier, "<color=#00EEFFFF>Mafia</color> Can Vent", false);

            Overlord = new(MultiMenu.objectifier, "<color=#008080FF>Overlord</color>");
            OverlordCount = new(num++, MultiMenu.objectifier, "<color=#008080FF>Overlord</color> Count", 1, 1, 14, 1);
            UniqueOverlord = new(num++, MultiMenu.objectifier, "<color=#008080FF>Overlord</color> Is Unique In All Any", false);
            OverlordKnows = new(num++, MultiMenu.objectifier, "<color=#008080FF>Overlord</color> Knows Who They Are", true);
            OverlordMeetingWinCount = new(num++, MultiMenu.objectifier, "<color=#008080FF>Overlord</color> Meeting Timer", 2, 1, 20, 1);

            Rivals = new(MultiMenu.objectifier, "<color=#3D2D2CFF>Rivals</color>");
            RivalsCount = new(num++, MultiMenu.objectifier, "<color=#3D2D2CFF>Rivals</color> Count", 1, 1, 14, 1);
            UniqueRivals = new(num++, MultiMenu.objectifier, "<color=#3D2D2CFF>Rivals</color> Is Unique In All Any", false);
            RivalsChat = new(num++, MultiMenu.objectifier, "Enable <color=#3D2D2CFF>Rivals</color> Chat", true);
            RivalsFaction = new(num++, MultiMenu.objectifier, "<color=#3D2D2CFF>Rivals</color> Can Be From The Same Faction", true);
            RivalsRoles = new(num++, MultiMenu.objectifier, "<color=#3D2D2CFF>Rivals</color> Know Each Other's <color=#FFD700FF>Roles</color>", true);

            Taskmaster = new(MultiMenu.objectifier, "<color=#ABABFFFF>Taskmaster</color>");
            TaskmasterCount = new(num++, MultiMenu.objectifier, "<color=#ABABFFFF>Taskmaster</color> Count", 1, 1, 14, 1);
            UniqueTaskmaster = new(num++, MultiMenu.objectifier, "<color=#ABABFFFF>Taskmaster</color> Is Unique In All Any", false);
            TMTasksRemaining = new(num++, MultiMenu.objectifier, "Tasks Remaining When Revealed", 1, 1, 5, 1);

            Traitor = new(MultiMenu.objectifier, "<color=#370D43FF>Traitor</color>");
            TraitorCount = new(num++, MultiMenu.objectifier, "<color=#370D43FF>Traitor</color> Count", 1, 1, 14, 1);
            UniqueTraitor = new(num++, MultiMenu.objectifier, "<color=#370D43FF>Traitor</color> Is Unique In All Any", false);
            TraitorKnows = new(num++, MultiMenu.objectifier, "<color=#370D43FF>Traitor</color> Knows Who They Are", true);
            SnitchSeesTraitor = new(num++, MultiMenu.objectifier, "<color=#D4AF37FF>Snitch</color> Sees Turned <color=#370D43FF>Traitor</color>", true);
            RevealerRevealsTraitor = new(num++, MultiMenu.objectifier, "<color=#D3D3D3FF>Revealer</color> Reveals Turned <color=#370D43FF>Traitor</color>", false);
            TraitorColourSwap = new(num++, MultiMenu.objectifier, "Turned <color=#370D43FF>Traitor</color> Swaps Colours For Investigative <color=#FFD700FF>Roles</color>", false);
        }
    }
}