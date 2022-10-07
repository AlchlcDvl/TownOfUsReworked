using System;

namespace TownOfUs.CustomOption
{
    public class Generate
    {
        public static CustomHeaderOption CrewInvestigativeRoles;
        public static CustomNumberOption DetectiveOn;
        public static CustomNumberOption InvestigatorOn;
        public static CustomNumberOption MysticOn;
        public static CustomNumberOption SheriffOn;
        public static CustomNumberOption MediumOn;
        public static CustomNumberOption SnitchOn;
        public static CustomNumberOption AgentOn;
        public static CustomNumberOption TrackerOn;
        public static CustomNumberOption OperativeOn;

        public static CustomHeaderOption CrewSovereignRoles;
        public static CustomNumberOption MayorOn;
        public static CustomNumberOption SwapperOn;

        public static CustomHeaderOption CrewProtectiveRoles;
        public static CustomNumberOption MedicOn;

        public static CustomHeaderOption CrewKillingRoles;
        public static CustomNumberOption VeteranOn;
        public static CustomNumberOption VigilanteOn;

        public static CustomHeaderOption CrewSupportRoles;
        public static CustomNumberOption AltruistOn;
        public static CustomNumberOption EngineerOn;
        public static CustomNumberOption ShifterOn;
        public static CustomNumberOption TimeLordOn;
        public static CustomNumberOption TransporterOn;

        public static CustomHeaderOption PostmortalCrewRoles;
        public static CustomNumberOption HaunterOn;

        public static CustomHeaderOption NeutralBenignRoles;
        public static CustomNumberOption AmnesiacOn;
        public static CustomNumberOption GuardianAngelOn;
        public static CustomNumberOption SurvivorOn;

        public static CustomHeaderOption NeutralChaosRoles;
        public static CustomNumberOption DraculaOn;

        public static CustomHeaderOption NeutralEvilRoles;
        public static CustomNumberOption ExecutionerOn;
        public static CustomNumberOption JesterOn;
        public static CustomNumberOption CannibalOn;
        public static CustomNumberOption TaskmasterOn;

        public static CustomHeaderOption NeutralKillingRoles;
        public static CustomNumberOption ArsonistOn;
        public static CustomNumberOption PlaguebearerOn;
        public static CustomNumberOption GlitchOn;
        public static CustomNumberOption WerewolfOn;
        public static CustomNumberOption JuggernautOn;

        public static CustomHeaderOption PostmortalNeutralRoles;
        public static CustomNumberOption PhantomOn;

        public static CustomHeaderOption ImpostorConcealingRoles;
        public static CustomNumberOption CamouflagerOn;
        public static CustomNumberOption GrenadierOn;
        public static CustomNumberOption UndertakerOn;
        public static CustomNumberOption JanitorOn;

        public static CustomHeaderOption ImpostorDeceptionRoles;
        public static CustomNumberOption MorphlingOn;
        public static CustomNumberOption DisguiserOn;
        public static CustomNumberOption PoisonerOn;
        public static CustomNumberOption WraithOn;

        public static CustomHeaderOption ImpostorKillingRoles;
        public static CustomNumberOption UnderdogOn;

        public static CustomHeaderOption ImpostorSupportRoles;
        public static CustomNumberOption BlackmailerOn;
        public static CustomNumberOption ConsigliereOn;
        public static CustomNumberOption MinerOn;
        public static CustomNumberOption TMOn;
        public static CustomNumberOption TraitorOn;

        public static CustomHeaderOption PositiveModifiers;
        public static CustomNumberOption BaitOn;
        public static CustomNumberOption DiseasedOn;
        public static CustomNumberOption TorchOn;
        public static CustomNumberOption LighterOn;
        public static CustomNumberOption ButtonBarryOn;
        public static CustomNumberOption TiebreakerOn;

        public static CustomHeaderOption NeutralModifiers;
        public static CustomNumberOption GiantOn;
        public static CustomNumberOption DwarfOn;
        public static CustomNumberOption LoversOn;

        public static CustomHeaderOption NegativeModifiers;
        public static CustomNumberOption CowardOn;
        public static CustomNumberOption DrunkOn;
        public static CustomNumberOption VolatileOn;

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

        public static CustomHeaderOption ModdedGameOptions;
        public static CustomNumberOption InitialCooldowns;

        public static CustomHeaderOption GameModifiers;
        public static CustomToggleOption ColourblindComms;
        public static CustomToggleOption MeetingColourblind;
        public static CustomStringOption WhoCanVent;
        public static CustomToggleOption CrewMustDie;

        public static CustomHeaderOption QualityChanges;
        public static CustomToggleOption CustomImpColors;
        public static CustomToggleOption CustomCrewColors;
        public static CustomToggleOption CustomNeutColors;
        public static CustomToggleOption CustomModifierColors;
        public static CustomToggleOption CustomObjectifierColors;
        public static CustomToggleOption ImpostorSeeRoles;
        public static CustomToggleOption DeadSeeRoles;
        public static CustomToggleOption DisableLevels;
        public static CustomToggleOption WhiteNameplates;
        public static CustomToggleOption ParallelMedScans;
        public static CustomStringOption SkipButtonDisable;
        public static CustomToggleOption SFXOn;

        public static CustomHeaderOption BetterPolusSettings;
        public static CustomToggleOption VentImprovements;
        public static CustomToggleOption VitalsLab;
        public static CustomToggleOption ColdTempDeathValley;
        public static CustomToggleOption WifiChartCourseSwap;

        public static CustomHeaderOption GameModeSettings;
        public static CustomStringOption GameMode;

        public static CustomHeaderOption ClassicSettings;
        public static CustomNumberOption MinNeutralNonKillingRoles;
        public static CustomNumberOption MaxNeutralNonKillingRoles;
        public static CustomNumberOption MinNeutralKillingRoles;
        public static CustomNumberOption MaxNeutralKillingRoles;

        public static CustomHeaderOption AllAnySettings;
        public static CustomToggleOption RandomNumberImps;

        public static CustomHeaderOption KillingOnlySettings;
        public static CustomNumberOption NeutralRoles;
        public static CustomToggleOption AddArsonist;
        public static CustomToggleOption AddPlaguebearer;

        public static CustomHeaderOption TaskTrackingSettings;
        public static CustomToggleOption SeeTasksDuringRound;
        public static CustomToggleOption SeeTasksDuringMeeting;
        public static CustomToggleOption SeeTasksWhenDead;

        public static CustomHeaderOption CrewInvestigativeSettings;
        public static CustomHeaderOption CrewProtectiveSettings;
        public static CustomHeaderOption CrewSupportSettings;
        public static CustomHeaderOption CrewSovereignSettings;
        public static CustomHeaderOption CrewKillingSettings;
        public static CustomHeaderOption PostmortalCrewSettings;
        public static CustomHeaderOption ImpConcealingSettings;
        public static CustomHeaderOption ImpDeceptionSettings;
        public static CustomHeaderOption ImpSupportSettings;
        public static CustomHeaderOption ImpKillingSettings;
        public static CustomHeaderOption NeutralEvilSettings;
        public static CustomHeaderOption NeutralBenignSettings;
        public static CustomHeaderOption NeutralKillingSettings;
        public static CustomHeaderOption NeutralChaosSettings;
        public static CustomHeaderOption NeutralPowerSettings;
        public static CustomHeaderOption PostmortalNeutralSettings;
        public static CustomHeaderOption PositiveModifierSettings;
        public static CustomHeaderOption NeutralModifierSettings;
        public static CustomHeaderOption NegativeModifierSettings;

        public static CustomHeaderOption Mayor;
        public static CustomNumberOption MayorCount;
        public static CustomNumberOption MayorVoteBank;
        public static CustomToggleOption MayorAnonymous;

        public static CustomHeaderOption Vigilante;
        public static CustomNumberOption VigilanteCount;
        public static CustomToggleOption VigiKillOther;
        public static CustomToggleOption VigiKillsJester;
        public static CustomToggleOption VigiKillsExecutioner;
        public static CustomToggleOption VigiKillsCannibal;
        public static CustomNumberOption VigiKillCd;

        public static CustomHeaderOption Engineer;
        public static CustomNumberOption EngineerCount;
        public static CustomStringOption EngineerPer;

        public static CustomHeaderOption Investigator;
        public static CustomNumberOption InvestigatorCount;
        public static CustomNumberOption FootprintSize;
        public static CustomNumberOption FootprintInterval;
        public static CustomNumberOption FootprintDuration;
        public static CustomToggleOption AnonymousFootPrint;
        public static CustomToggleOption VentFootprintVisible;

        public static CustomHeaderOption TimeLord;
        public static CustomNumberOption TimeLordCount;
        public static CustomToggleOption RewindRevive;
        public static CustomToggleOption TLImmunity;
        public static CustomNumberOption RewindDuration;
        public static CustomNumberOption RewindCooldown;
        public static CustomNumberOption RewindMaxUses;

        public static CustomHeaderOption Medic;
        public static CustomNumberOption MedicCount;
        public static CustomStringOption ShowShielded;
        public static CustomStringOption WhoGetsNotification;
        public static CustomToggleOption ShieldBreaks;
        public static CustomToggleOption MedicReportSwitch;
        public static CustomNumberOption MedicReportNameDuration;
        public static CustomNumberOption MedicReportColorDuration;

        public static CustomHeaderOption Sheriff;
        public static CustomNumberOption SheriffCount;
        public static CustomNumberOption InterrogateCooldown;
        public static CustomToggleOption NeutEvilRed;
        public static CustomToggleOption NeutKillingRed;
        public static CustomNumberOption SheriffAccuracy;
        public static CustomStringOption InterrogatePer;
        public static CustomToggleOption TraitorColourSwap;

        public static CustomHeaderOption Swapper;
        public static CustomNumberOption SwapperCount;
        public static CustomToggleOption SwapperButton;

        /*public static CustomHeaderOption Shaman;
        public static CustomNumberOption ShamanCount;*/

        public static CustomHeaderOption Agent;
        public static CustomNumberOption AgentCount;

        public static CustomHeaderOption Transporter;
        public static CustomNumberOption TransporterCount;
        public static CustomNumberOption TransportCooldown;
        public static CustomNumberOption TransportMaxUses;

        public static CustomHeaderOption Jester;
        public static CustomNumberOption JesterCount;
        public static CustomToggleOption JesterButton;
        public static CustomToggleOption JesterVent;
        public static CustomToggleOption JestSwitchVent;

        public static CustomHeaderOption Glitch;
        public static CustomNumberOption GlitchCount;
        public static CustomNumberOption MimicCooldownOption;
        public static CustomNumberOption MimicDurationOption;
        public static CustomNumberOption HackCooldownOption;
        public static CustomNumberOption HackDurationOption;
        public static CustomNumberOption GlitchKillCooldownOption;
        public static CustomStringOption GlitchHackDistanceOption;
        public static CustomToggleOption GlitchVent;

        public static CustomHeaderOption Juggernaut;
        public static CustomNumberOption JuggernautCount;
        public static CustomToggleOption JuggVent;
        public static CustomNumberOption JuggKillCooldownOption;
        public static CustomNumberOption JuggKillBonus;

        public static CustomHeaderOption Morphling;
        public static CustomNumberOption MorphlingCount;
        public static CustomNumberOption MorphlingCooldown;
        public static CustomNumberOption MorphlingDuration;
        public static CustomToggleOption MorphlingVent;

        public static CustomHeaderOption Executioner;
        public static CustomNumberOption ExecutionerCount;
        public static CustomStringOption OnTargetDead;
        public static CustomToggleOption ExecutionerButton;
        public static CustomToggleOption ExeVent;
        public static CustomToggleOption ExeSwitchVent;
        public static CustomToggleOption ExeTargetKnows;
        public static CustomToggleOption ExeKnowsTargetRole;

        public static CustomHeaderOption Phantom;
        public static CustomNumberOption PhantomTasksRemaining;

        public static CustomHeaderOption Snitch;
        public static CustomNumberOption SnitchCount;
        public static CustomToggleOption SnitchSeesNeutrals;
        public static CustomToggleOption SnitchSeesCrew;
        public static CustomToggleOption SnitchSeesRoles;
        public static CustomNumberOption SnitchTasksRemaining;
        public static CustomToggleOption SnitchSeesImpInMeeting;
        public static CustomToggleOption SnitchSeesTraitor;

        public static CustomHeaderOption Altruist;
        public static CustomNumberOption AltruistCount;
        public static CustomNumberOption ReviveDuration;
        public static CustomToggleOption AltruistTargetBody;

        public static CustomHeaderOption Miner;
        public static CustomNumberOption MinerCount;
        public static CustomNumberOption MineCooldown;

        public static CustomHeaderOption Shifter;
        public static CustomNumberOption ShifterCount;
        public static CustomNumberOption ShifterCd;
        public static CustomStringOption ShiftedBecomes;

        public static CustomHeaderOption Wraith;
        public static CustomNumberOption WraithCount;
        public static CustomNumberOption InvisCooldown;
        public static CustomNumberOption InvisDuration;
        public static CustomToggleOption WraithVent;

        public static CustomHeaderOption Arsonist;
        public static CustomNumberOption ArsonistCount;
        public static CustomNumberOption DouseCooldown;
        public static CustomNumberOption IgniteCooldown;
        public static CustomNumberOption MaxDoused;
        public static CustomToggleOption ArsoVent;

        public static CustomHeaderOption Undertaker;
        public static CustomNumberOption UndertakerCount;
        public static CustomNumberOption DragCooldown;
        public static CustomToggleOption UndertakerVent;
        public static CustomToggleOption UndertakerVentWithBody;
        public static CustomNumberOption DragModifier;

        public static CustomHeaderOption CustomHeader;
        public static CustomNumberOption NumberOfImpostorAssassins;
        public static CustomNumberOption NumberOfNeutralAssassins;
        public static CustomNumberOption NumberOfCrewAssassins;
        public static CustomToggleOption AmneTurnImpAssassin;
        public static CustomToggleOption AmneTurnCrewAssassin;
        public static CustomToggleOption AmneTurnNeutAssassin;
        public static CustomToggleOption TraitorCanAssassin;
        public static CustomNumberOption AssassinKills;
        public static CustomToggleOption AssassinMultiKill;
        public static CustomToggleOption AssassinSnitchViaCrewmate;
        public static CustomToggleOption AssassinGuessNeutralBenign;
        public static CustomToggleOption AssassinGuessNeutralEvil;
        public static CustomToggleOption AssassinGuessPest;
        public static CustomToggleOption AssassinGuessModifiers;
        public static CustomToggleOption AssassinGuessLovers;
        public static CustomToggleOption AssassinateAfterVoting;

        public static CustomHeaderOption Underdog;
        public static CustomNumberOption UnderdogCount;
        public static CustomNumberOption UnderdogKillBonus;
        public static CustomToggleOption UnderdogIncreasedKC;

        public static CustomHeaderOption Camouflager;
        public static CustomNumberOption CamouflagerCount;
        public static CustomNumberOption CamouflagerCooldown;
        public static CustomNumberOption CamouflagerDuration;

        public static CustomHeaderOption Haunter;
        public static CustomNumberOption HaunterTasksRemainingClicked;
        public static CustomNumberOption HaunterTasksRemainingAlert;
        public static CustomToggleOption HaunterRevealsNeutrals;
        public static CustomToggleOption HaunterRevealsCrew;
        public static CustomToggleOption HaunterRevealsRoles;
        public static CustomToggleOption HaunterRevealsTraitor;
        public static CustomStringOption HaunterCanBeClickedBy;

        public static CustomHeaderOption Grenadier;
        public static CustomNumberOption GrenadierCount;
        public static CustomNumberOption GrenadeCooldown;
        public static CustomNumberOption GrenadeDuration;
        public static CustomToggleOption GrenadierIndicators;
        public static CustomToggleOption GrenadierVent;
        public static CustomNumberOption FlashRadius;
        
        public static CustomHeaderOption Disguiser;
        public static CustomNumberOption DisguiserCount;
        public static CustomNumberOption DisguiseCooldown;
        public static CustomNumberOption TimeToDisguise;
        public static CustomNumberOption DisguiseDuration;
        public static CustomStringOption DisguiseTarget;

        public static CustomHeaderOption Veteran;
        public static CustomNumberOption VeteranCount;
        public static CustomNumberOption AlertCooldown;
        public static CustomNumberOption AlertDuration;
        public static CustomNumberOption MaxAlerts;

        public static CustomHeaderOption Tracker;
        public static CustomNumberOption TrackerCount;
        public static CustomNumberOption UpdateInterval;
        public static CustomNumberOption TrackCooldown;
        public static CustomToggleOption ResetOnNewRound;
        public static CustomNumberOption MaxTracks;

        public static CustomHeaderOption Operative;
        public static CustomNumberOption OperativeCount;
        public static CustomNumberOption BugCooldown;
        public static CustomToggleOption BugsRemoveOnNewRound;
        public static CustomNumberOption MaxBugs;
        public static CustomNumberOption MinAmountOfTimeInBug;
        public static CustomNumberOption BugRange;
        public static CustomNumberOption MinAmountOfPlayersInBug;

        public static CustomHeaderOption Poisoner;
        public static CustomNumberOption PoisonerCount;
        public static CustomNumberOption PoisonCooldown;
        public static CustomNumberOption PoisonDuration;
        public static CustomToggleOption PoisonerVent;

        public static CustomHeaderOption Traitor;
        public static CustomNumberOption LatestSpawn;
        public static CustomToggleOption NeutralKillingStopsTraitor;

        public static CustomHeaderOption Amnesiac;
        public static CustomNumberOption AmnesiacCount;
        public static CustomToggleOption RememberArrows;
        public static CustomNumberOption RememberArrowDelay;

        public static CustomHeaderOption Medium;
        public static CustomNumberOption MediumCount;
        public static CustomNumberOption MediateCooldown;
        public static CustomToggleOption ShowMediatePlayer;
        public static CustomToggleOption ShowMediumToDead;
        public static CustomStringOption DeadRevealed;

        public static CustomHeaderOption Survivor;
        public static CustomNumberOption SurvivorCount;
        public static CustomNumberOption VestCd;
        public static CustomNumberOption VestDuration;
        public static CustomNumberOption VestKCReset;
        public static CustomNumberOption MaxVests;

        public static CustomHeaderOption GuardianAngel;
        public static CustomNumberOption GuardianAngelCount;
        public static CustomNumberOption ProtectCd;
        public static CustomNumberOption ProtectDuration;
        public static CustomNumberOption ProtectKCReset;
        public static CustomNumberOption MaxProtects;
        public static CustomStringOption ShowProtect;
        public static CustomStringOption GaOnTargetDeath;
        public static CustomToggleOption GATargetKnows;
        public static CustomToggleOption GAKnowsTargetRole;

        public static CustomHeaderOption Mystic;
        public static CustomNumberOption MysticCount;
        public static CustomNumberOption MysticArrowDuration;
        public static CustomToggleOption MysticReportName;
        public static CustomToggleOption MysticReportRole;

        public static CustomHeaderOption Blackmailer;
        public static CustomNumberOption BlackmailerCount;
        public static CustomNumberOption BlackmailCooldown;

        public static CustomHeaderOption Plaguebearer;
        public static CustomNumberOption PlaguebearerCount;
        public static CustomNumberOption InfectCooldown;
        public static CustomToggleOption PBVent;

        public static CustomHeaderOption Pestilence;
        public static CustomNumberOption PestKillCooldown;
        public static CustomToggleOption PestVent;

        public static CustomHeaderOption Werewolf;
        public static CustomNumberOption WerewolfCount;
        public static CustomNumberOption RampageCooldown;
        public static CustomNumberOption RampageDuration;
        public static CustomNumberOption RampageKillCooldown;
        public static CustomToggleOption WerewolfVent;

        public static CustomHeaderOption Detective;
        public static CustomNumberOption DetectiveCount;
        public static CustomNumberOption InitialExamineCooldown;
        public static CustomNumberOption ExamineCooldown;
        public static CustomNumberOption RecentKill;
        public static CustomToggleOption DetectiveReportOn;
        public static CustomNumberOption DetectiveRoleDuration;
        public static CustomNumberOption DetectiveFactionDuration;

        public static CustomHeaderOption Janitor;
        public static CustomNumberOption JanitorCount;
        public static CustomNumberOption JanitorCleanCd;

        public static CustomHeaderOption Cannibal;
        public static CustomNumberOption CannibalCount;
        public static CustomNumberOption CannibalCd;
        public static CustomNumberOption CannibalBodyCount;
        public static CustomToggleOption CannibalVent;

        public static CustomHeaderOption TimeMaster;
        public static CustomNumberOption TMCount;
        public static CustomNumberOption FreezeDuration;
        public static CustomNumberOption FreezerCooldown;

        public static CustomHeaderOption Consigliere;
        public static CustomNumberOption ConsigCount;
        public static CustomNumberOption RevealCooldown;
        public static CustomStringOption ConsigInfo;

        public static CustomHeaderOption Dracula;
        public static CustomNumberOption DraculaCount;
        public static CustomNumberOption BiteCooldown;
        public static CustomToggleOption DraculaConvertNeuts;
        public static CustomToggleOption DracVent;

        public static CustomHeaderOption Vampire;
        public static CustomNumberOption AliveVampCount;
        public static CustomToggleOption VampVent;

        public static CustomHeaderOption Taskmaster;
        public static CustomNumberOption TaskmasterCount;
        public static CustomNumberOption TMTasksRemaining;
        public static CustomToggleOption TaskmasterVent;

        public static CustomHeaderOption Giant;
        public static CustomNumberOption GiantSpeed;
        public static CustomNumberOption GiantScale;

        public static CustomHeaderOption Dwarf;
        public static CustomNumberOption DwarfSpeed;
        public static CustomNumberOption DwarfScale;

        public static CustomHeaderOption Diseased;
        public static CustomNumberOption DiseasedKillMultiplier;

        public static CustomHeaderOption Bait;
        public static CustomNumberOption BaitMinDelay;
        public static CustomNumberOption BaitMaxDelay;

        public static CustomHeaderOption Lovers;
        public static CustomToggleOption BothLoversDie;

        public static Func<object, string> PercentFormat { get; } = value => $"{value:0}%";
        private static Func<object, string> CooldownFormat { get; } = value => $"{value:0.0#}s";
        private static Func<object, string> MultiplierFormat { get; } = value => $"{value:0.0#}x";
        private static Func<object, string> ValueFix { get; } = value => $"{value:0.00#}";


        public static void GenerateAll()
        {
            var num = 0;

            Patches.ExportButton = new Export(num++);
            Patches.ImportButton = new Import(num++);

            CrewInvestigativeRoles = new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#8BFDFDFF>Crew</color> <color=#1D7CF2FF>Investigative</color>");
            AgentOn = new CustomNumberOption(true, num++, MultiMenu.crewmate, "<color=#CCA3CCFF>Agent</color>", 0f, 0f, 100f, 10f, PercentFormat);
            DetectiveOn = new CustomNumberOption(true, num++, MultiMenu.crewmate, "<color=#4D4DFFFF>Detective</color>", 0f, 0f, 100f, 10f, PercentFormat);
            InvestigatorOn = new CustomNumberOption(true, num++, MultiMenu.crewmate, "<color=#00B3B3FF>Investigator</color>", 0f, 0f, 100f, 10f, PercentFormat);
            MediumOn = new CustomNumberOption(true, num++, MultiMenu.crewmate, "<color=#A680FFFF>Medium</color>", 0f, 0f, 100f, 10f, PercentFormat);
            MysticOn = new CustomNumberOption(true, num++, MultiMenu.crewmate, "<color=#4D99E6FF>Mystic</color>", 0f, 0f, 100f, 10f, PercentFormat);
            OperativeOn = new CustomNumberOption(true, num++, MultiMenu.crewmate, "<color=#A7D1B3FF>Operative</color>", 0f, 0f, 100f, 10f, PercentFormat);
            SheriffOn = new CustomNumberOption(true, num++, MultiMenu.crewmate, "<color=#FFCC80FF>Sheriff</color>", 0f, 0f, 100f, 10f, PercentFormat);
            SnitchOn = new CustomNumberOption(true, num++, MultiMenu.crewmate, "<color=#D4AF37FF>Snitch</color>", 0f, 0f, 100f, 10f, PercentFormat);
            TrackerOn = new CustomNumberOption(true, num++, MultiMenu.crewmate, "<color=#009900FF>Tracker</color>", 0f, 0f, 100f, 10f, PercentFormat);

            CrewKillingRoles = new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#8BFDFDFF>Crew</color> <color=#1D7CF2FF>Killing</color>");
            VeteranOn = new CustomNumberOption(true, num++, MultiMenu.crewmate, "<color=#998040FF>Veteran</color>", 0f, 0f, 100f, 10f, PercentFormat);
            VigilanteOn = new CustomNumberOption(true, num++, MultiMenu.crewmate, "<color=#FFFF00FF>Vigilante</color>", 0f, 0f, 100f, 10f, PercentFormat);

            CrewProtectiveRoles = new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#8BFDFDFF>Crew</color> <color=#1D7CF2FF>Protective</color>");
            MedicOn = new CustomNumberOption(true, num++, MultiMenu.crewmate, "<color=#006600FF>Medic</color>", 0f, 0f, 100f, 10f, PercentFormat);

            CrewSovereignRoles = new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#8BFDFDFF>Crew</color> <color=#1D7CF2FF>Sovereign</color> Roles");
            MayorOn = new CustomNumberOption(true, num++, MultiMenu.crewmate, "<color=#704FA8FF>Mayor</color>", 0f, 0f, 100f, 10f, PercentFormat);
            SwapperOn = new CustomNumberOption(true, num++, MultiMenu.crewmate, "<color=#66E666FF>Swapper</color>", 0f, 0f, 100f, 10f, PercentFormat);

            CrewSupportRoles = new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#8BFDFDFF>Crew</color> <color=#1D7CF2FF>Support</color> Roles");
            AltruistOn = new CustomNumberOption(true, num++, MultiMenu.crewmate, "<color=#660000FF>Altruist</color>", 0f, 0f, 100f, 10f, PercentFormat);
            EngineerOn = new CustomNumberOption(true, num++, MultiMenu.crewmate, "<color=#FFA60AFF>Engineer</color>", 0f, 0f, 100f, 10f, PercentFormat);
            ShifterOn = new CustomNumberOption(true, num++, MultiMenu.crewmate,  "<color=#DF851FFF>Shifter</color>", 0f, 0f, 100f, 10f, PercentFormat);
            TimeLordOn = new CustomNumberOption(true, num++, MultiMenu.crewmate, "<color=#0000FFFF>Time Lord</color>", 0f, 0f, 100f, 10f, PercentFormat);
            TransporterOn = new CustomNumberOption(true, num++, MultiMenu.crewmate, "<color=#00EEFFFF>Transporter</color>", 0f, 0f, 100f, 10f, PercentFormat);

            PostmortalCrewRoles = new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#1D7CF2FF>Postmortal</color> <color=#8BFDFDFF>Crew</color>");
            HaunterOn = new CustomNumberOption(true, num++, MultiMenu.crewmate, "<color=#D3D3D3FF>Haunter</color>", 0f, 0f, 100f, 10f, PercentFormat);

            NeutralBenignRoles = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Benign</color>");
            AmnesiacOn = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#22FFFFFF>Amnesiac</color>", 0f, 0f, 100f, 10f, PercentFormat);
            GuardianAngelOn = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#FFFFFFFF>Guardian Angel</color>", 0f, 0f, 100f, 10f, PercentFormat);
            SurvivorOn = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#DDDD00FF>Survivor</color>", 0f, 0f, 100f, 10f, PercentFormat);

            NeutralChaosRoles = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Chaos</color>");
            DraculaOn = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#AC8A00FF>Dracula</color>", 0f, 0f, 100f, 10f, PercentFormat);

            NeutralEvilRoles = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Evil</color>");
            CannibalOn = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#8C4005FF>Cannibal</color>", 0f, 0f, 100f, 10f, PercentFormat);
            ExecutionerOn = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#CCCCCCFF>Executioner</color>", 0f, 0f, 100f, 10f, PercentFormat);
            JesterOn = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#F7B3DAFF>Jester</color>", 0f, 0f, 100f, 10f, PercentFormat);
            TaskmasterOn = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#ABABFFFF>Taskmaster</color>", 0f, 0f, 100f, 10f, PercentFormat);

            NeutralKillingRoles = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killing</color>");
            ArsonistOn = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#EE7600FF>Arsonist</color>", 0f, 0f, 100f, 10f, PercentFormat);
            GlitchOn = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#00FF00FF>Glitch</color>", 0f, 0f, 100f, 10f, PercentFormat);
            JuggernautOn = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#A12B56FF>Juggernaut</color>", 0f, 0f, 100f, 10f, PercentFormat);
            PlaguebearerOn = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#CFFE61FF>Plaguebearer</color>", 0f, 0f, 100f, 10f, PercentFormat);
            WerewolfOn = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#9F703AFF>Werewolf</color>", 0f, 0f, 100f, 10f, PercentFormat);

            PostmortalNeutralRoles = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#1D7CF2FF>Postmortal</color> <color=#B3B3B3FF>Neutral</color>");
            PhantomOn = new CustomNumberOption(true, num++, MultiMenu.neutral, "<color=#662962FF>Phantom</color>", 0f, 0f, 100f, 10f, PercentFormat);

            ImpostorConcealingRoles = new CustomHeaderOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Concealing</color>");
            CamouflagerOn = new CustomNumberOption(true, num++, MultiMenu.imposter, "<color=#378AC0FF>Camouflager</color>", 0f, 0f, 100f, 10f, PercentFormat);
            GrenadierOn = new CustomNumberOption(true, num++, MultiMenu.imposter, "<color=#85AA5BFF>Grenadier</color>", 0f, 0f, 100f, 10f, PercentFormat);
            JanitorOn = new CustomNumberOption(true, num++, MultiMenu.imposter, "<color=#2647A2FF>Janitor</color>", 0f, 0f, 100f, 10f, PercentFormat);
            UndertakerOn = new CustomNumberOption(true, num++, MultiMenu.imposter, "<color=#005643FF>Undertaker</color>", 0f, 0f, 100f, 10f, PercentFormat);

            ImpostorDeceptionRoles = new CustomHeaderOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Deception</color>");
            DisguiserOn = new CustomNumberOption(true, num++, MultiMenu.imposter, "<color=#006600FF>Disguiser</color>", 0f, 0f, 100f, 10f, PercentFormat);
            MorphlingOn = new CustomNumberOption(true, num++, MultiMenu.imposter, "<color=#BB45B0FF>Morphling</color>", 0f, 0f, 100f, 10f, PercentFormat);
            PoisonerOn = new CustomNumberOption(true, num++, MultiMenu.imposter, "<color=#B5004CFF>Poisoner</color>", 0f, 0f, 100f, 10f, PercentFormat);
            WraithOn = new CustomNumberOption(true, num++, MultiMenu.imposter, "<color=#FFB875FF>Wraith</color>", 0f, 0f, 100f, 10f, PercentFormat);

            ImpostorKillingRoles = new CustomHeaderOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Killing</color>");
            UnderdogOn = new CustomNumberOption(true, num++, MultiMenu.imposter, "<color=#841A7FFF>Underdog</color>", 0f, 0f, 100f, 10f, PercentFormat);

            ImpostorSupportRoles = new CustomHeaderOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Support</color>");
            BlackmailerOn = new CustomNumberOption(true, num++, MultiMenu.imposter, "<color=#02A752FF>Blackmailer</color>", 0f, 0f, 100f, 10f, PercentFormat);
            ConsigliereOn = new CustomNumberOption(true, num++, MultiMenu.imposter, "<color=#FFFF99FF>Consigliere</color>", 0f, 0f, 100f, 10f, PercentFormat);
            MinerOn = new CustomNumberOption(true, num++, MultiMenu.imposter, "<color=#AA7632FF>Miner</color>", 0f, 0f, 100f, 10f, PercentFormat);
            TMOn = new CustomNumberOption(true, num++, MultiMenu.imposter, "<color=#0000A7FF>Time Master</color>", 0f, 0f, 100f, 10f, PercentFormat);
            TraitorOn = new CustomNumberOption(true, num++, MultiMenu.imposter, "<color=#370D43FF>Traitor</color>", 0f, 0f, 100f, 10f, PercentFormat);

            PositiveModifiers = new CustomHeaderOption(num++, MultiMenu.modifiers, "<color=#7FFF00FF>Positive</color> Modifiers");
            BaitOn = new CustomNumberOption(true, num++, MultiMenu.modifiers, "<color=#00B3B3FF>Bait</color>", 0f, 0f, 100f, 10f, PercentFormat);
            ButtonBarryOn = new CustomNumberOption(true, num++, MultiMenu.modifiers, "<color=#E600FFFF>Button Barry</color>", 0f, 0f, 100f, 10f, PercentFormat);
            DiseasedOn = new CustomNumberOption(true, num++, MultiMenu.modifiers, "<color=#808080FF>Diseased</color>", 0f, 0f, 100f, 10f, PercentFormat);
            TorchOn = new CustomNumberOption(true, num++, MultiMenu.modifiers, "<color=#FFFF99FF>Torch</color>", 0f, 0f, 100f, 10f, PercentFormat);
            LighterOn = new CustomNumberOption(true, num++, MultiMenu.modifiers, "<color=#1AFF74FF>Lighter</color>", 0f, 0f, 100f, 10f, PercentFormat);
            TiebreakerOn = new CustomNumberOption(true, num++, MultiMenu.modifiers, "<color=#99E699FF>Tiebreaker</color>", 0f, 0f, 100f, 10f, PercentFormat);

            NeutralModifiers = new CustomHeaderOption(num++, MultiMenu.modifiers, "<color=#B3B3B3FF>Neutral</color> Modifiers");
            DwarfOn = new CustomNumberOption(true, num++, MultiMenu.modifiers, "<color=#FF8080FF>Dwarf</color>", 0f, 0f, 100f, 10f, PercentFormat);
            GiantOn = new CustomNumberOption(true, num++, MultiMenu.modifiers, "<color=#FFB34DFF>Giant</color>", 0f, 0f, 100f, 10f, PercentFormat);
            LoversOn = new CustomNumberOption(true, num++, MultiMenu.modifiers, "<color=#FF66CCFF>Lovers</color>", 0f, 0f, 100f, 10f, PercentFormat);

            NegativeModifiers = new CustomHeaderOption(num++, MultiMenu.modifiers, "<color=#DD0000FF>Negative</color> Modifiers");
            CowardOn = new CustomNumberOption(true, num++, MultiMenu.modifiers, "<color=#808080FF>Coward</color>", 0f, 0f, 100f, 10f, PercentFormat);
            DrunkOn = new CustomNumberOption(true, num++, MultiMenu.modifiers, "<color=#758000FF>Drunk</color>", 0f, 0f, 100f, 10f, PercentFormat);
            VolatileOn = new CustomNumberOption(true, num++, MultiMenu.modifiers, "<color=#FFA60AFF>Volatile</color>", 0f, 0f, 100f, 10f, PercentFormat);

            GameModeSettings = new CustomHeaderOption(num++, MultiMenu.main, "Game Mode Settings");
            GameMode = new CustomStringOption(num++, MultiMenu.main, "Game Mode", new[] {"Classic", "All Any", "Killing Only", "Custom"});

            AllAnySettings = new CustomHeaderOption(num++, MultiMenu.main, "All Any Mode Settings");
            RandomNumberImps = new CustomToggleOption(num++, MultiMenu.main, "Random Number Of <color=#FF0000FF>Intruders</color>", true);

            KillingOnlySettings = new CustomHeaderOption(num++, MultiMenu.main, "<color=#1D7CF2FF>Killing</color> Only Mode Settings");
            NeutralRoles = new CustomNumberOption(num++, MultiMenu.main, "<color=#B3B3B3FF>Neutral</color> Count", 1f, 0f, 5f, 1f);
            AddArsonist = new CustomToggleOption(num++, MultiMenu.main, "Add <color=#EE7600FF>Arsonist</color>", true);
            AddPlaguebearer = new CustomToggleOption(num++, MultiMenu.main, "Add <color=#CFFE61FF>Plaguebearer</color>", true);

            ClassicSettings = new CustomHeaderOption(num++, MultiMenu.main, "Classic And Custom Mode Settings");
            MinNeutralNonKillingRoles = new CustomNumberOption(num++, MultiMenu.main, "Min <color=#B3B3B3FF>Neutral</color> Non-<color=#1D7CF2FF>Killings</color>", 1f, 0f, 13f, 1f);
            MaxNeutralNonKillingRoles = new CustomNumberOption(num++, MultiMenu.main, "Max <color=#B3B3B3FF>Neutral</color> Non-<color=#1D7CF2FF>Killings</color>", 1f, 0f, 13f, 1f);
            MinNeutralKillingRoles = new CustomNumberOption(num++, MultiMenu.main, "Min <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killings</color>", 1f, 0f, 13f, 1f);
            MaxNeutralKillingRoles = new CustomNumberOption(num++, MultiMenu.main, "Max <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killings</color>", 1f, 0f, 13f, 1f);

            CustomHeader = new CustomHeaderOption(num++, MultiMenu.modifiers, "<color=#073763FF>Assassin</color> Settings");
            NumberOfImpostorAssassins = new CustomNumberOption(num++, MultiMenu.modifiers, "Number Of <color=#FF0000FF>Intruder</color> <color=#073763FF>Assassins</color>", 1, 0, 4, 1);
            NumberOfCrewAssassins = new CustomNumberOption(num++, MultiMenu.modifiers, "Number Of <color=#8BFDFDFF>Crew</color> <color=#073763FF>Assassins</color>", 1, 0, 14, 1);
            NumberOfNeutralAssassins = new CustomNumberOption(num++, MultiMenu.modifiers, "Number Of <color=#B3B3B3FF>Neutral</color> <color=#073763FF>Assassins</color>", 1, 0, 5, 1);
            AmneTurnImpAssassin = new CustomToggleOption(num++, MultiMenu.modifiers, "<color=#22FFFFFF>Amnesiac</color> Turned <color=#FF0000FF>Intruder</color> Gets Ability", false);
            AmneTurnCrewAssassin = new CustomToggleOption(num++, MultiMenu.modifiers, "<color=#22FFFFFF>Amnesiac</color> Turned <color=#8BFDFDFF>Crew</color> Gets Ability", false);
            AmneTurnNeutAssassin = new CustomToggleOption(num++, MultiMenu.modifiers, "<color=#22FFFFFF>Amnesiac</color> Turned <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killing</color> Gets Ability", false);
            TraitorCanAssassin = new CustomToggleOption(num++, MultiMenu.modifiers, "<color=#FF0000FF>Traitor</color> Gets Ability", false);
            AssassinKills = new CustomNumberOption(num++, MultiMenu.modifiers, "Number Of <color=#073763FF>Assassin</color> Kills", 1, 1, 15, 1);
            AssassinMultiKill = new CustomToggleOption(num++, MultiMenu.modifiers, "<color=#073763FF>Assassin</color> Can Kill More Than Once Per Meeting", false);
            AssassinSnitchViaCrewmate = new CustomToggleOption(num++, MultiMenu.modifiers, "<color=#073763FF>Assassinate</color> <color=#D4AF37FF>Snitch</color> Via <color=#8BFDFDFF>Crewmate</color> Guess", false);
            AssassinGuessNeutralBenign = new CustomToggleOption(num++, MultiMenu.modifiers, "<color=#073763FF>Assassin</color> Can Guess <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Benigns</color>", false);
            AssassinGuessNeutralEvil = new CustomToggleOption(num++, MultiMenu.modifiers, "<color=#073763FF>Assassin</color> Can Guess <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Evils</color>", false);
            AssassinGuessPest = new CustomToggleOption(num++, MultiMenu.modifiers, "<color=#073763FF>Assassin</color> Can Guess <color=#424242FF>Pestilence</color>", false);
            AssassinGuessModifiers = new CustomToggleOption(num++, MultiMenu.modifiers, "<color=#073763FF>Assassin</color> Can Guess Select Modifiers", false);
            AssassinGuessLovers = new CustomToggleOption(num++, MultiMenu.modifiers, "<color=#073763FF>Assassin</color> Can Guess <color=#FF66CCFF>Lovers</color>", false);
            AssassinateAfterVoting = new CustomToggleOption(num++, MultiMenu.modifiers, "<color=#073763FF>Assassin</color> Can Guess After Voting", false);

            MapSettings = new CustomHeaderOption(num++, MultiMenu.main, "Map Settings");
            RandomMapEnabled = new CustomToggleOption(num++, MultiMenu.main, "Choose Random Map", false);
            RandomMapSkeld = new CustomNumberOption(num++, MultiMenu.main, "Skeld Chance", 0f, 0f, 100f, 10f, PercentFormat);
            RandomMapMira = new CustomNumberOption(num++, MultiMenu.main, "Mira Chance", 0f, 0f, 100f, 10f, PercentFormat);
            RandomMapPolus = new CustomNumberOption(num++, MultiMenu.main, "Polus Chance", 0f, 0f, 100f, 10f, PercentFormat);
            RandomMapAirship = new CustomNumberOption(num++, MultiMenu.main, "Airship Chance", 0f, 0f, 100f, 10f, PercentFormat);
            RandomMapSubmerged = new CustomNumberOption(num++, MultiMenu.main, "Submerged Chance", 0f, 0f, 100f, 10f, PercentFormat);
            AutoAdjustSettings = new CustomToggleOption(num++, MultiMenu.main, "Auto Adjust Settings", false);
            SmallMapHalfVision = new CustomToggleOption(num++, MultiMenu.main, "Half Vision On Skeld/Mira HQ", false);
            SmallMapDecreasedCooldown = new CustomNumberOption(num++, MultiMenu.main, "Mira HQ Decreased Cooldowns", 0f, 0f, 15f, 2.5f, CooldownFormat);
            LargeMapIncreasedCooldown = new CustomNumberOption(num++, MultiMenu.main, "Airship/Submerged Increased Cooldowns", 0f, 0f, 15f, 2.5f, CooldownFormat);
            SmallMapIncreasedShortTasks = new CustomNumberOption(num++, MultiMenu.main, "Skeld/Mira HQ Increased Short Tasks", 0, 0, 5, 1);
            SmallMapIncreasedLongTasks = new CustomNumberOption(num++, MultiMenu.main, "Skeld/Mira HQ Increased Long Tasks", 0, 0, 3, 1);
            LargeMapDecreasedShortTasks = new CustomNumberOption(num++, MultiMenu.main, "Airship/Submerged Decreased Short Tasks", 0, 0, 5, 1);
            LargeMapDecreasedLongTasks = new CustomNumberOption(num++, MultiMenu.main, "Airship/Submerged Decreased Long Tasks", 0, 0, 3, 1);

            BetterPolusSettings = new CustomHeaderOption(num++, MultiMenu.main, "Better Polus Settings");
            VentImprovements = new CustomToggleOption(num++, MultiMenu.main, "Better Polus Vent Layout", false);
            VitalsLab = new CustomToggleOption(num++, MultiMenu.main, "Vitals Moved To Lab", false);
            ColdTempDeathValley = new CustomToggleOption(num++, MultiMenu.main, "Cold Temp Moved To Death Valley", false);
            WifiChartCourseSwap = new CustomToggleOption(num++, MultiMenu.main, "Reboot Wifi And Chart Course Swapped", false);

            GameModifiers = new CustomHeaderOption(num++, MultiMenu.main, "Game Modifiers");
            ColourblindComms = new CustomToggleOption(num++, MultiMenu.main, "Camouflaged Comms", true);
            //MeetingColourblind = new CustomToggleOption(num++, MultiMenu.main, "Camouflaged Meetings", false);
            WhoCanVent = new CustomStringOption(num++, MultiMenu.main, "Serial Venters", new[] {"Default", "Everyone", "No One" });
            //CrewMustDie = new CustomToggleOption(num++, MultiMenu.main, "All Crew Must Die", false);

            ModdedGameOptions = new CustomHeaderOption(num++, MultiMenu.main, "Custom Game Options");
            InitialCooldowns = new CustomNumberOption(num++, MultiMenu.main, "Game Start Cooldowns", 10, 10, 30, 2.5f, CooldownFormat);

            QualityChanges = new CustomHeaderOption(num++, MultiMenu.main, "Quality Changes");
            ImpostorSeeRoles = new CustomToggleOption(num++, MultiMenu.main, "<color=#FF0000FF>Intruders</color> Can See The Roles Of Their Team", false);
            DeadSeeRoles = new CustomToggleOption(num++, MultiMenu.main, "Dead Can See Everyone's Roles & Votes", false);
            ParallelMedScans = new CustomToggleOption(num++, MultiMenu.main, "Parallel Medbay Scans", false);
            SkipButtonDisable = new CustomStringOption(num++, MultiMenu.main, "Disable Meeting Skip Button", new[] { "No", "Emergency", "Always" });
            SFXOn = new CustomToggleOption(num++, MultiMenu.main, "Sound Effects", true);
            DisableLevels = new CustomToggleOption(num++, MultiMenu.main, "Disable Level Icons", false);
            WhiteNameplates = new CustomToggleOption(num++, MultiMenu.main, "Disable Player Nameplates", false);
            CustomCrewColors = new CustomToggleOption(num++, MultiMenu.main, "Enable Custom <color=#8BFDFDFF>Crew</color> Colors", true);
            CustomImpColors = new CustomToggleOption(num++, MultiMenu.main, "Enable Custom <color=#FF0000FF>Intruder</color> Colors", true);
            CustomNeutColors = new CustomToggleOption(num++, MultiMenu.main, "Enable Custom <color=#B3B3B3FF>Neutral</color> Colors", true);
            CustomModifierColors = new CustomToggleOption(num++, MultiMenu.main, "Enable Custom <color=#7F7F7FFF>Modifier</color> Colors", true);
            CustomObjectifierColors = new CustomToggleOption(num++, MultiMenu.main, "Enable Custom <color=#DD585BFF>Objectifier</color> Colors", true);

            TaskTrackingSettings = new CustomHeaderOption(num++, MultiMenu.main, "Task Tracking Settings");
            SeeTasksDuringRound = new CustomToggleOption(num++, MultiMenu.main, "See Tasks During Round", false);
            SeeTasksDuringMeeting = new CustomToggleOption(num++, MultiMenu.main, "See Tasks During Meetings", false);
            SeeTasksWhenDead = new CustomToggleOption(num++, MultiMenu.main, "See Tasks When Dead", true);

            CrewInvestigativeSettings = new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#8BFDFDFF>Crew</color> <color=#1D7CF2FF>Investigative</color> Settings");

            Agent = new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#CCA3CCFF>Agent</color>");
            AgentCount = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#CCA3CCFF>Agent</color> Count", 1, 1, 14, 1);

            Detective = new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#4D4DFFFF>Detective</color>");
            DetectiveCount = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#4D4DFFFF>Detective</color> Count", 1, 1, 14, 1);
            InitialExamineCooldown = new CustomNumberOption(num++, MultiMenu.crewmate, "Initial Examine Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            ExamineCooldown = new CustomNumberOption(num++, MultiMenu.crewmate, "Examine Cooldown", 10f, 1f, 15f, 1f, CooldownFormat);
            RecentKill = new CustomNumberOption(num++, MultiMenu.crewmate, "Bloody Player Duration", 25f, 10f, 60f, 2.5f, CooldownFormat);

            Investigator = new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#00B3B3FF>Investigator</color>");
            InvestigatorCount = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#00B3B3FF>Investigator</color> Count", 1, 1, 14, 1);
            FootprintSize = new CustomNumberOption(num++, MultiMenu.crewmate, "Footprint Size", 4f, 1f, 10f, 1f);
            FootprintInterval = new CustomNumberOption(num++, MultiMenu.crewmate, "Footprint Interval", 0.1f, 0.05f, 1f, 0.05f, CooldownFormat);
            FootprintDuration = new CustomNumberOption(num++, MultiMenu.crewmate, "Footprint Duration", 10f, 1f, 10f, 0.5f, CooldownFormat);
            AnonymousFootPrint = new CustomToggleOption(num++, MultiMenu.crewmate, "Anonymous Footprint", false);
            VentFootprintVisible = new CustomToggleOption(num++, MultiMenu.crewmate, "Footprint Vent Visible", false);

            Medium = new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#A680FFFF>Medium</color>");
            MediumCount = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#A680FFFF>Medium</color> Count", 1, 1, 14, 1);
            MediateCooldown = new CustomNumberOption(num++, MultiMenu.crewmate, "Mediate Cooldown", 10f, 1f, 15f, 1f, CooldownFormat);
            ShowMediatePlayer = new CustomToggleOption(num++, MultiMenu.crewmate, "Reveal Appearance Of Mediate Target", true);
            ShowMediumToDead = new CustomToggleOption(num++, MultiMenu.crewmate, "Reveal The <color=#A680FFFF>Medium</color> To The Mediate Target", true);
            DeadRevealed = new CustomStringOption(num++, MultiMenu.crewmate, "Who Is Revealed With Mediate", new[] { "Oldest Dead", "Newest Dead", "All Dead" });

            Mystic = new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#4D99E6FF>Mystic</color>");
            MysticCount = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#4D99E6FF>Mystic</color> Count", 1, 1, 14, 1);
            MysticArrowDuration = new CustomNumberOption(num++, MultiMenu.crewmate, "Dead Body Arrow Duration", 0.1f, 0f, 2f, 0.05f, CooldownFormat);
            MysticReportName = new CustomToggleOption(num++, MultiMenu.crewmate, "<color=#4D99E6FF>Mystic</color> Gets Killer's Name", false);
            MysticReportRole = new CustomToggleOption(num++, MultiMenu.crewmate, "<color=#4D99E6FF>Mystic</color> Gets Killer's Role", false);

            /*Shaman = new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#ABABFFFF>Shaman</color>");
            ShamanCount = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#ABABFFFF>Shaman</color> Count", 1, 1, 14, 1);*/

            Sheriff = new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#FFCC80FF>Sheriff</color>");
            SheriffCount = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#FFCC80FF>Sheriff</color> Count", 1, 1, 14, 1);
            InterrogateCooldown = new CustomNumberOption(num++, MultiMenu.crewmate, "Interrogate Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            NeutEvilRed = new CustomToggleOption(num++, MultiMenu.crewmate, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Evils</color> Show Evil", false);
            NeutKillingRed = new CustomToggleOption(num++, MultiMenu.crewmate, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killings</color> Show Evil", false);
            SheriffAccuracy = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#FFCC80FF>Sheriff</color> Accuracy", 100f, 0f, 100f, 5f, PercentFormat);
            InterrogatePer = new CustomStringOption(num++, MultiMenu.crewmate, "<color=#FFCC80FF>Sheriff</color> Reveals Per", new[] {"Round", "Game"});
            TraitorColourSwap = new CustomToggleOption(num++, MultiMenu.crewmate, "<color=#370D43FF>Traitor</color> Does Not Swap Colours", false);

            Snitch = new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#D4AF37FF>Snitch</color>");
            SnitchCount = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#D4AF37FF>Snitch</color> Count", 1, 1, 14, 1);
            SnitchSeesNeutrals = new CustomToggleOption(num++, MultiMenu.crewmate, "<color=#D4AF37FF>Snitch</color> Sees <color=#B3B3B3FF>Neutrals</color>", false);
            SnitchSeesCrew = new CustomToggleOption(num++, MultiMenu.crewmate, "<color=#D4AF37FF>Snitch</color> Sees <color=#8BFDFDFF>Crew</color>", false);
            SnitchSeesRoles = new CustomToggleOption(num++, MultiMenu.crewmate, "<color=#D4AF37FF>Snitch</color> Sees Exact Roles", false);
            SnitchTasksRemaining = new CustomNumberOption(num++, MultiMenu.crewmate, "Tasks Remaining When Revealed", 1, 1, 5, 1);
            SnitchSeesImpInMeeting = new CustomToggleOption(num++, MultiMenu.crewmate, "<color=#D4AF37FF>Snitch</color> Sees <color=#FF0000FF>Intruders</color> In Meetings", true);
            SnitchSeesTraitor = new CustomToggleOption(num++, MultiMenu.crewmate, "<color=#D4AF37FF>Snitch</color> Sees <color=#370D43FF>Traitor</color>", true);

            Tracker = new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#009900FF>Tracker</color>");
            TrackerCount = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#009900FF>Tracker</color> Count", 1, 1, 14, 1);
            UpdateInterval = new CustomNumberOption(num++, MultiMenu.crewmate, "Arrow Update Interval", 5f, 0.5f, 15f, 0.5f, CooldownFormat);
            TrackCooldown = new CustomNumberOption(num++, MultiMenu.crewmate, "Track Cooldown", 25f, 10f, 40f, 2.5f, CooldownFormat);
            ResetOnNewRound = new CustomToggleOption(num++, MultiMenu.crewmate, "<color=#009900FF>Tracker</color> Arrows Reset After Each Round", false);
            MaxTracks = new CustomNumberOption(num++, MultiMenu.crewmate, "Track Count Per Round", 5, 1, 15, 1);

            Operative = new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#A7D1B3FF>Operative</color>");
            OperativeCount = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#A7D1B3FF>Operative</color> Count", 1, 1, 14, 1);
            MinAmountOfTimeInBug = new CustomNumberOption(num++, MultiMenu.crewmate, "Min Amount Of Time In Bug To Register", 1f, 0f, 15f, 0.5f, CooldownFormat);
            BugCooldown = new CustomNumberOption(num++, MultiMenu.crewmate, "Bug Cooldown", 25f, 10f, 40f, 2.5f, CooldownFormat);
            BugsRemoveOnNewRound = new CustomToggleOption(num++, MultiMenu.crewmate, "Bugs Are Removed Each Round", true);
            MaxBugs = new CustomNumberOption(num++, MultiMenu.crewmate, "Bug Count", 5, 1, 15, 1);
            BugRange = new CustomNumberOption(num++, MultiMenu.crewmate, "Bug Range", 1f, 0.5f, 5f, 0.5f, MultiplierFormat);
            MinAmountOfPlayersInBug = new CustomNumberOption(num++, MultiMenu.crewmate, "Number Of Roles Required To Trigger Bug", 3, 1, 5, 1);

            CrewKillingSettings = new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#8BFDFDFF>Crew</color> <color=#1D7CF2FF>Killing</color> Settings");

            Veteran = new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#998040FF>Veteran</color>");
            VeteranCount = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#998040FF>Veteran</color> Count", 1, 1, 14, 1);
            AlertCooldown = new CustomNumberOption(num++, MultiMenu.crewmate, "Alert Cooldown", 25, 10, 60, 2.5f, CooldownFormat);
            AlertDuration = new CustomNumberOption(num++, MultiMenu.crewmate, "Alert Duration", 10, 5, 15, 1f, CooldownFormat);
            MaxAlerts = new CustomNumberOption(num++, MultiMenu.crewmate, "Number Of Alerts", 5, 1, 15, 1);

            Vigilante = new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#FFFF00FF>Vigilante</color>");
            VigilanteCount = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#FFFF00FF>Vigilante</color> Count", 1, 1, 14, 1);
            VigiKillOther = new CustomToggleOption(num++, MultiMenu.crewmate, "<color=#FFFF00FF>Vigilante</color> Misfire Kills Target", false);
            VigiKillsCannibal = new CustomToggleOption(num++, MultiMenu.crewmate, "<color=#FFFF00FF>Vigilante</color> Kills <color=#8C4005FF>Cannibal</color>", false);
            VigiKillsExecutioner = new CustomToggleOption(num++, MultiMenu.crewmate, "<color=#FFFF00FF>Vigilante</color> Kills <color=#CCCCCCFF>Executioner</color>", false);
            VigiKillsJester = new CustomToggleOption(num++, MultiMenu.crewmate, "<color=#FFFF00FF>Vigilante</color> Kills <color=#F7B3DAFF>Jester</color>", false);
            VigiKillCd = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#FFFF00FF>Vigilante</color> Kill Cooldown", 25f, 10f, 40f, 2.5f, CooldownFormat);

            CrewProtectiveSettings = new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#8BFDFDFF>Crew</color> <color=#1D7CF2FF>Protective</color> Settings");

            Medic = new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#006600FF>Medic</color>");
            MedicCount = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#006600FF>Medic</color> Count", 1, 1, 14, 1);
            ShowShielded = new CustomStringOption(num++, MultiMenu.crewmate, "Show Shielded Player", new[] {"Self", "Medic", "Self+Medic", "Everyone" });
            WhoGetsNotification = new CustomStringOption(num++, MultiMenu.crewmate, "Who Gets Murder Attempt Indicator", new[] {"Medic", "Shielded", "Everyone", "Nobody" });
            ShieldBreaks = new CustomToggleOption(num++, MultiMenu.crewmate, "Shield Breaks On Murder Attempt", false);

            CrewSovereignSettings = new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#8BFDFDFF>Crew</color> <color=#1D7CF2FF>Sovereign</color> Settings");

            Mayor = new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#704FA8FF>Mayor</color>");
            MayorCount = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#704FA8FF>Mayor</color> Count", 1, 1, 14, 1);
            MayorVoteBank = new CustomNumberOption(num++, MultiMenu.crewmate, "Initial <color=#704FA8FF>Mayor</color> Vote Bank", 1, 1, 5, 1);
            MayorAnonymous = new CustomToggleOption(num++, MultiMenu.crewmate, "Anonymous <color=#704FA8FF>Mayor</color> Votes", false);

            Swapper = new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#66E666FF>Swapper</color>");
            SwapperCount = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#66E666FF>Swapper</color> Count", 1, 1, 14, 1);
            SwapperButton = new CustomToggleOption(num++, MultiMenu.crewmate, "<color=#66E666FF>Swapper</color> Can Button", true);

            CrewSupportSettings = new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#8BFDFDFF>Crew</color> <color=#1D7CF2FF>Support</color> Settings");

            Altruist = new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#660000FF>Altruist</color>");
            AltruistCount = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#660000FF>Altruist</color> Count", 1, 1, 14, 1);
            ReviveDuration = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#660000FF>Altruist</color> Revive Duration", 10, 1, 15, 1f, CooldownFormat);
            AltruistTargetBody = new CustomToggleOption(num++, MultiMenu.crewmate, "Target's Body Disappears On Beginning Of Revive", false);

            Engineer = new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#FFA60AFF>Engineer</color>");
            EngineerCount = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#FFA60AFF>Engineer</color> Count", 1, 1, 14, 1);
            EngineerPer = new CustomStringOption(num++, MultiMenu.crewmate, "<color=#FFA60AFF>Engineer</color> Fixes Once Per", new[] { "Round", "Game" });

            Shifter = new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#DF851FFF>Shifter</color>");
            ShifterCount = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#DF851FFF>Shifter</color> Count", 1, 1, 14, 1);
            ShifterCd = new CustomNumberOption(num++, MultiMenu.crewmate, "Shift Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            ShiftedBecomes = new CustomStringOption(num++, MultiMenu.crewmate, "Shifted Becomes", new[] { "Shifter", "Crewmate" });

            TimeLord = new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#0000FFFF>Time Lord</color>");
            TimeLordCount = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#0000FFFF>Time Lord</color> Count", 1, 1, 14, 1);
            RewindRevive = new CustomToggleOption(num++, MultiMenu.crewmate, "Revive During Rewind", false);
            //TLImmunity = new CustomToggleOption(num++, MultiMenu.crewmate, "<color=#0000FFFF>Time Lord</color> Doesn't Rewind", false);
            RewindDuration = new CustomNumberOption(num++, MultiMenu.crewmate, "Rewind Duration", 2f, 2f, 5f, 0.5f, CooldownFormat);
            RewindCooldown = new CustomNumberOption(num++, MultiMenu.crewmate, "Rewind Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            RewindMaxUses = new CustomNumberOption(num++, MultiMenu.crewmate, "Maximum Number Of Rewinds", 5, 1, 15, 1);

            Transporter = new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#00EEFFFF>Transporter</color>");
            TransporterCount = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#00EEFFFF>Transporter</color> Count", 1, 1, 14, 1);
            TransportCooldown = new CustomNumberOption(num++, MultiMenu.crewmate, "Transport Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            TransportMaxUses = new CustomNumberOption(num++, MultiMenu.crewmate, "Number Of Transports", 5, 1, 15, 1);

            PostmortalCrewSettings = new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#1D7CF2FF>Postmortal</color> <color=#8BFDFDFF>Crew</color> Settings");

            Haunter = new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#D3D3D3FF>Haunter</color>");
            HaunterTasksRemainingClicked = new CustomNumberOption(num++, MultiMenu.crewmate, "Tasks Remaining When <color=#D3D3D3FF>Haunter</color> Can Be Clicked", 5, 1, 10, 1);
            HaunterTasksRemainingAlert = new CustomNumberOption(num++, MultiMenu.crewmate, "Tasks Remaining When Revealed", 1, 1, 5, 1);
            HaunterRevealsNeutrals = new CustomToggleOption(num++, MultiMenu.crewmate, "<color=#D3D3D3FF>Haunter</color> Reveals <color=#B3B3B3FF>Neutrals</color>", false);
            HaunterRevealsCrew = new CustomToggleOption(num++, MultiMenu.crewmate, "<color=#D3D3D3FF>Haunter</color> Reveals <color=#8BFDFDFF>Crew</color>", false);
            HaunterRevealsTraitor = new CustomToggleOption(num++, MultiMenu.crewmate, "<color=#D3D3D3FF>Haunter</color> Reveals <color=#370D43FF>Traitor</color>", false);
            HaunterRevealsRoles = new CustomToggleOption(num++, MultiMenu.crewmate, "<color=#D3D3D3FF>Haunter</color> Reveals Exact Roles", false);
            HaunterCanBeClickedBy = new CustomStringOption(num++, MultiMenu.crewmate, "Who Can Click <color=#D3D3D3FF>Haunter</color>", new[] { "All", "Non-Crew", "Imps Only" });

            NeutralBenignSettings = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Benign</color> Settings");

            Amnesiac = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#22FFFFFF>Amnesiac</color>");
            AmnesiacCount = new CustomNumberOption(num++, MultiMenu.neutral, "<color=#22FFFFFF>Amnesiac</color> Count", 1, 1, 14, 1);
            RememberArrows = new CustomToggleOption(num++, MultiMenu.neutral, "<color=#22FFFFFF>Amnesiac</color> Gets Arrows To Dead Bodies", false);
            RememberArrowDelay = new CustomNumberOption(num++, MultiMenu.neutral, "Time After Death Arrow Appears", 5f, 0f, 15f, 1f, CooldownFormat);

            GuardianAngel = new CustomHeaderOption(num++, MultiMenu.neutral, "Guardian Angel");
            GuardianAngelCount = new CustomNumberOption(num++, MultiMenu.neutral, "Guardian Angel Count", 1, 1, 14, 1);
            ProtectCd = new CustomNumberOption(num++, MultiMenu.neutral, "Protect Cooldown", 25, 10, 60, 2.5f, CooldownFormat);
            ProtectDuration = new CustomNumberOption(num++, MultiMenu.neutral, "Protect Duration", 10, 5, 15, 1f, CooldownFormat);
            ProtectKCReset = new CustomNumberOption(num++, MultiMenu.neutral, "Kill Cooldown Reset When Protected", 2.5f, 0f, 15f, 0.5f, CooldownFormat);
            MaxProtects = new CustomNumberOption(num++, MultiMenu.neutral, "Maximum Number Of Protects", 5, 1, 15, 1);
            ShowProtect = new CustomStringOption(num++, MultiMenu.neutral, "Show Protected Player", new[] { "Self", "Guardian Angel", "Self+GA", "Everyone" });
            GaOnTargetDeath = new CustomStringOption(num++, MultiMenu.neutral, "GA Becomes On Target Dead", new[] { "Crewmate", "Amnesiac", "Survivor", "Jester" });
            GATargetKnows = new CustomToggleOption(num++, MultiMenu.neutral, "Target Knows GA Exists", false);
            GAKnowsTargetRole = new CustomToggleOption(num++, MultiMenu.neutral, "GA Knows Target's Role", false);

            Survivor = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#DDDD00FF>Survivor</color>");
            SurvivorCount = new CustomNumberOption(num++, MultiMenu.neutral, "<color=#DDDD00FF>Survivor</color> Count", 1, 1, 14, 1);
            VestCd = new CustomNumberOption(num++, MultiMenu.neutral, "Vest Cooldown", 25, 10, 60, 2.5f, CooldownFormat);
            VestDuration = new CustomNumberOption(num++, MultiMenu.neutral, "Vest Duration", 10, 5, 15, 1f, CooldownFormat);
            VestKCReset = new CustomNumberOption(num++, MultiMenu.neutral, "Kill Cooldown Reset On Attack", 2.5f, 0f, 15f, 0.5f, CooldownFormat);
            MaxVests = new CustomNumberOption(num++, MultiMenu.neutral, "Number Of Vests", 5, 1, 15, 1);

            NeutralChaosSettings = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Chaos</color> Settings");
            
            Dracula = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#AC8A00FF>Dracula</color>");
            DraculaCount = new CustomNumberOption(num++, MultiMenu.neutral, "<color=#AC8A00FF>Dracula</color> Count", 1, 1, 14, 1);
            BiteCooldown = new CustomNumberOption(num++, MultiMenu.neutral, "Bite Cooldown", 25, 10, 60, 2.5f, CooldownFormat);
            DraculaConvertNeuts = new CustomToggleOption(num++, MultiMenu.neutral, "<color=#AC8A00FF>Dracula</color> Can Convert <color=#B3B3B3FF>Neutrals</color>", false);
            DracVent = new CustomToggleOption(num++, MultiMenu.neutral, "<color=#AC8A00FF>Dracula</color> Can Vent", false);

            NeutralEvilSettings = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Evil</color> Settings");

            Cannibal = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#8C4005FF>Cannibal</color>"); 
            CannibalCount = new CustomNumberOption(num++, MultiMenu.neutral, "<color=#8C4005FF>Cannibal</color> Count", 1, 1, 14, 1);
            CannibalCd = new CustomNumberOption(num++, MultiMenu.neutral, "Eat Cooldown", 10f, 10f, 60f, 2.5f, CooldownFormat);
            CannibalBodyCount = new CustomNumberOption(num++, MultiMenu.neutral, "Number Of Bodies To Eat", 1, 1, 5, 1);
            CannibalVent = new CustomToggleOption(num++, MultiMenu.neutral, "<color=#8C4005FF>Cannibal</color> Can Vent", false);

            Executioner = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#CCCCCCFF>Executioner</color>");
            ExecutionerCount = new CustomNumberOption(num++, MultiMenu.neutral, "<color=#CCCCCCFF>Executioner</color> Count", 1, 1, 14, 1);
            OnTargetDead = new CustomStringOption(num++, MultiMenu.neutral, "<color=#CCCCCCFF>Executioner</color> Becomes On Target Dead", new[] {"Crewmate", "Amnesiac", "Survivor", "Jester" });
            ExecutionerButton = new CustomToggleOption(num++, MultiMenu.neutral, "<color=#CCCCCCFF>Executioner</color> Can Button", true);
            ExeVent = new CustomToggleOption(num++, MultiMenu.neutral, "<color=#CCCCCCFF>Executioner</color> Can Hide In Vents", false);
            ExeSwitchVent = new CustomToggleOption(num++, MultiMenu.neutral, "<color=#CCCCCCFF>Executioner</color> Can Switch Vents", false);
            ExeTargetKnows = new CustomToggleOption(num++, MultiMenu.neutral, "Target Knows <color=#CCCCCCFF>Executioner</color> Exists", false);
            ExeKnowsTargetRole = new CustomToggleOption(num++, MultiMenu.neutral, "<color=#CCCCCCFF>Executioner</color> Knows Target's Role", false);

            Jester = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#F7B3DAFF>Jester</color>");
            JesterCount = new CustomNumberOption(num++, MultiMenu.neutral, "<color=#F7B3DAFF>Jester</color> Count", 1, 1, 14, 1);
            JesterButton = new CustomToggleOption(num++, MultiMenu.neutral, "<color=#F7B3DAFF>Jester</color> Can Button", true);
            JesterVent = new CustomToggleOption(num++, MultiMenu.neutral, "<color=#F7B3DAFF>Jester</color> Can Hide In Vents", false);
            JestSwitchVent = new CustomToggleOption(num++, MultiMenu.neutral, "<color=#F7B3DAFF>Jester</color> Can Switch Vents", false);

            Taskmaster = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#ABABFFFF>Taskmaster</color>");
            TaskmasterCount = new CustomNumberOption(num++, MultiMenu.neutral, "<color=#ABABFFFF>Taskmaster</color> Count", 1, 1, 14, 1);
            TMTasksRemaining = new CustomNumberOption(num++, MultiMenu.neutral, "Tasks Remaining When Revealed", 1, 1, 5, 1);
            TaskmasterVent = new CustomToggleOption(num++, MultiMenu.neutral, "<color=#ABABFFFF>Taskmaster</color> Can Vent", false);

            NeutralKillingSettings = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killing</color> Settings");

            Arsonist = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#EE7600FF>Arsonist</color>");
            ArsonistCount = new CustomNumberOption(num++, MultiMenu.neutral, "<color=#EE7600FF>Arsonist</color> Count", 1, 1, 14, 1);
            DouseCooldown = new CustomNumberOption(num++, MultiMenu.neutral, "Douse Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            IgniteCooldown = new CustomNumberOption(num++, MultiMenu.neutral, "Ignite Cooldown", 25f, 0f, 60f, 2.5f, CooldownFormat);
            MaxDoused = new CustomNumberOption(num++, MultiMenu.neutral, "Number Of Alive Players That Can Be Doused", 5, 1, 15, 1);
            ArsoVent = new CustomToggleOption(num++, MultiMenu.neutral, "<color=#EE7600FF>Arsonist</color> Can Vent", false);

            Glitch = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#00FF00FF>Glitch</color>");
            GlitchCount = new CustomNumberOption(num++, MultiMenu.neutral, "<color=#00FF00FF>Glitch</color> Count", 1, 1, 14, 1);
            MimicCooldownOption = new CustomNumberOption(num++, MultiMenu.neutral, "Mimic Cooldown", 25f, 10f, 120f, 2.5f, CooldownFormat);
            MimicDurationOption = new CustomNumberOption(num++, MultiMenu.neutral, "Mimic Duration", 10f, 1f, 30f, 1f, CooldownFormat);
            HackCooldownOption = new CustomNumberOption(num++, MultiMenu.neutral, "Hack Cooldown", 25f, 10f, 120f, 2.5f, CooldownFormat);
            HackDurationOption = new CustomNumberOption(num++, MultiMenu.neutral, "Hack Duration", 10f, 1f, 30f, 1f, CooldownFormat);
            GlitchKillCooldownOption = new CustomNumberOption(num++, MultiMenu.neutral, "<color=#00FF00FF>Glitch</color> Kill Cooldown", 25f, 10f, 120f, 2.5f, CooldownFormat);
            GlitchHackDistanceOption = new CustomStringOption(num++, MultiMenu.neutral, "<color=#00FF00FF>Glitch</color> Hack Distance", new[] { "Short", "Normal", "Long" });
            GlitchVent = new CustomToggleOption(num++, MultiMenu.neutral, "<color=#00FF00FF>Glitch</color> Can Vent", false);

            Juggernaut = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#A12B56FF>Juggernaut</color>");
            JuggernautCount = new CustomNumberOption(num++, MultiMenu.neutral, "<color=#A12B56FF>Juggernaut</color> Count", 1, 1, 14, 1);
            JuggKillCooldownOption = new CustomNumberOption(num++, MultiMenu.neutral, "<color=#A12B56FF>Juggernaut</color> Kill Cooldown", 25f, 10f, 120f, 2.5f, CooldownFormat);
            JuggKillBonus = new CustomNumberOption(num++, MultiMenu.neutral, "Kill Cooldown Bonus", 5, 2.5f, 30, 2.5f, CooldownFormat);
            JuggVent = new CustomToggleOption(num++, MultiMenu.neutral, "<color=#A12B56FF>Juggernaut</color> Can Vent", false);

            Plaguebearer = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#CFFE61FF>Plaguebearer</color>");
            PlaguebearerCount = new CustomNumberOption(num++, MultiMenu.neutral, "<color=#CFFE61FF>Plaguebearer</color> Count", 1, 1, 14, 1);
            InfectCooldown = new CustomNumberOption(num++, MultiMenu.neutral, "Infect Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            PBVent = new CustomToggleOption(num++, MultiMenu.neutral, "<color=#CFFE61FF>Plaguebearer</color> Can Vent", false);

            Pestilence = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#424242FF>Pestilence</color>");
            PestKillCooldown = new CustomNumberOption(num++, MultiMenu.neutral, "<color=#424242FF>Pestilence</color> Kill Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            PestVent = new CustomToggleOption(num++, MultiMenu.neutral, "<color=#424242FF>Pestilence</color> Can Vent", false);

            Werewolf = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#9F703AFF>Werewolf</color>");
            WerewolfCount = new CustomNumberOption(num++, MultiMenu.neutral, "<color=#9F703AFF>Werewolf</color> Count", 1, 1, 14, 1);
            RampageCooldown = new CustomNumberOption(num++, MultiMenu.neutral, "Rampage Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            RampageDuration = new CustomNumberOption(num++, MultiMenu.neutral, "Rampage Duration", 25f, 10f, 60f, 2.5f, CooldownFormat);
            RampageKillCooldown = new CustomNumberOption(num++, MultiMenu.neutral, "Rampage Kill Cooldown", 10f, 0.5f, 15f, 0.5f, CooldownFormat);
            WerewolfVent = new CustomToggleOption(num++, MultiMenu.neutral, "<color=#9F703AFF>Werewolf</color> Can Vent When Rampaged", false);

            NeutralPowerSettings = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Power</color> Settings");
            
            Vampire = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#7B8968FF>Vampire</color>");
            AliveVampCount = new CustomNumberOption(num++, MultiMenu.neutral, "Alive <color=#7B8968FF>Vampire</color> Count", 1, 1, 14, 1);
            VampVent = new CustomToggleOption(num++, MultiMenu.neutral, "<color=#7B8968FF>Vampire</color> Can Vent", false);

            PostmortalNeutralSettings = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#1D7CF2FF>Postmortal</color> <color=#B3B3B3FF>Neutral</color> Settings");

            Phantom = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#662962FF>Phantom</color>");
            PhantomTasksRemaining = new CustomNumberOption(num++, MultiMenu.neutral, "Tasks Remaining When <color=#662962FF>Phantom</color> Can Be Clicked", 5, 1, 10, 1);

            ImpConcealingSettings = new CustomHeaderOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Concealing</color> Settings");

            Camouflager = new CustomHeaderOption(num++, MultiMenu.imposter, "<color=#378AC0FF>Camouflager</color>");
            CamouflagerCount = new CustomNumberOption(num++, MultiMenu.imposter, "<color=#378AC0FF>Camouflager</color> Count", 1, 1, 14, 1);
            CamouflagerCooldown = new CustomNumberOption(num++, MultiMenu.imposter, "Camouflage Cooldown", 25, 10, 40, 2.5f, CooldownFormat);
            CamouflagerDuration = new CustomNumberOption(num++, MultiMenu.imposter, "Camouflage Duration", 10, 5, 15, 1f, CooldownFormat);

            Grenadier = new CustomHeaderOption(num++, MultiMenu.imposter, "<color=#85AA5BFF>Grenadier</color>");
            GrenadierCount = new CustomNumberOption(num++, MultiMenu.imposter, "<color=#85AA5BFF>Grenadier</color> Count", 1, 1, 14, 1);
            GrenadeCooldown = new CustomNumberOption(num++, MultiMenu.imposter, "Flash Grenade Cooldown", 25, 10, 40, 2.5f, CooldownFormat);
            GrenadeDuration = new CustomNumberOption(num++, MultiMenu.imposter, "Flash Grenade Duration", 10, 5, 15, 1f, CooldownFormat);
            FlashRadius = new CustomNumberOption(num++, MultiMenu.imposter, "Flash Radius", 1f, 0.25f, 5f, 0.25f, MultiplierFormat);
            GrenadierIndicators = new CustomToggleOption(num++, MultiMenu.imposter, "Indicate Flashed <color=#8BFDFDFF>Crewmates</color>", false);
            GrenadierVent = new CustomToggleOption(num++, MultiMenu.imposter, "<color=#85AA5BFF>Grenadier</color> Can Vent", false);

            Janitor = new CustomHeaderOption(num++, MultiMenu.imposter, "<color=#2647A2FF>Janitor</color>");
            JanitorCount = new CustomNumberOption(num++, MultiMenu.imposter, "<color=#2647A2FF>Janitor</color> Count", 1, 1, 14, 1);
            JanitorCleanCd = new CustomNumberOption (num++, MultiMenu.imposter, "<color=#2647A2FF>Janitor</color> Clean Cooldown", 25, 0, 40, 2.5f, CooldownFormat);

            Undertaker = new CustomHeaderOption(num++, MultiMenu.imposter, "<color=#005643FF>Undertaker</color>");
            UndertakerCount = new CustomNumberOption(num++, MultiMenu.imposter, "<color=#005643FF>Undertaker</color> Count", 1, 1, 14, 1);
            DragCooldown = new CustomNumberOption(num++, MultiMenu.imposter, "Drag Cooldown", 25, 10, 40, 2.5f, CooldownFormat);
            UndertakerVent = new CustomToggleOption(num++, MultiMenu.imposter, "<color=#005643FF>Undertaker</color> Can Vent", false);
            UndertakerVentWithBody = new CustomToggleOption(num++, MultiMenu.imposter, "<color=#005643FF>Undertaker</color> Can Vent While Dragging", false);
            //UndertakerVentHide = new CustomToggleOption(num++, MultiMenu.imposter, "Undertaker Can Hide Bodies In Vents", false);
            DragModifier = new CustomNumberOption(num++, MultiMenu.imposter, "Drag Speed", 0.1f, 0.5f, 10f, 0.5f, MultiplierFormat);

            ImpDeceptionSettings = new CustomHeaderOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Deception</color> Settings");
            
            /*Disguiser = new CustomHeaderOption(num++, MultiMenu.imposter, "<color=#006600FF>Disguiser</color>");
            DisguiseCooldown = new CustomNumberOption(num++, MultiMenu.imposter, "Disguise Cooldown", 30, 10, 60, 2.5f, CooldownFormat);
            TimeToDisguise = new CustomNumberOption(num++, MultiMenu.imposter, "Delay Before Disguising", 5, 2.5f, 15, 2.5f, CooldownFormat);
            DisguiseDuration = new CustomNumberOption(num++, MultiMenu.imposter, "Disguise Duration", 10, 2.5f, 20f, 2.5f, CooldownFormat);*/

            Morphling = new CustomHeaderOption(num++, MultiMenu.imposter, "<color=#BB45B0FF>Morphling</color>");
            MorphlingCount = new CustomNumberOption(num++, MultiMenu.imposter, "<color=#BB45B0FF>Morphling</color> Count", 1, 1, 14, 1);
            MorphlingCooldown = new CustomNumberOption(num++, MultiMenu.imposter, "Morph Cooldown", 25, 10, 40, 2.5f, CooldownFormat);
            MorphlingDuration = new CustomNumberOption(num++, MultiMenu.imposter, "Morph Duration", 10, 5, 15, 1f, CooldownFormat);
            MorphlingVent = new CustomToggleOption(num++, MultiMenu.imposter, "<color=#BB45B0FF>Morphling</color> Can Vent", false);

            Poisoner = new CustomHeaderOption(num++, MultiMenu.imposter, "<color=#B5004CFF>Poisoner</color>");
            PoisonerCount = new CustomNumberOption(num++, MultiMenu.imposter, "<color=#B5004CFF>Poisoner</color> Count", 1, 1, 14, 1);
            PoisonCooldown = new CustomNumberOption(num++, MultiMenu.imposter, "Poison Cooldown", 25, 10, 40, 2.5f, CooldownFormat);
            PoisonDuration = new CustomNumberOption(num++, MultiMenu.imposter, "Poison Kill Delay", 5, 1, 15, 1f, CooldownFormat);
            PoisonerVent = new CustomToggleOption(num++, MultiMenu.imposter, "<color=#B5004CFF>Poisoner</color> Can Vent", false);

            Wraith = new CustomHeaderOption(num++, MultiMenu.imposter, "<color=#FFB875FF>Wraith</color>");
            WraithCount = new CustomNumberOption(num++, MultiMenu.imposter, "<color=#FFB875FF>Wraith</color> Count", 1, 1, 14, 1);
            InvisCooldown = new CustomNumberOption(num++, MultiMenu.imposter, "Invis Cooldown", 25, 10, 40, 2.5f, CooldownFormat);
            InvisDuration = new CustomNumberOption(num++, MultiMenu.imposter, "Invis Duration", 10, 5, 15, 1f, CooldownFormat);
            WraithVent = new CustomToggleOption(num++, MultiMenu.imposter, "<color=#FFB875FF>Wraith</color> Can Vent", false);

            ImpKillingSettings = new CustomHeaderOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Impostor</color> <color=#1D7CF2FF>Killing</color> Settings");

            Underdog = new CustomHeaderOption(num++, MultiMenu.imposter, "<color=#841A7FFF>Underdog</color>");
            UnderdogCount = new CustomNumberOption(num++, MultiMenu.imposter, "<color=#841A7FFF>Underdog</color> Count", 1, 1, 14, 1);
            UnderdogKillBonus = new CustomNumberOption(num++, MultiMenu.imposter, "Kill Cooldown Bonus", 5, 2.5f, 30, 2.5f, CooldownFormat);
            UnderdogIncreasedKC = new CustomToggleOption(num++, MultiMenu.imposter, "Increased Kill Cooldown When 2+ <color=#FF0000FF>Intruders</color>", true);

            ImpSupportSettings = new CustomHeaderOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Support</color> Settings");

            Blackmailer = new CustomHeaderOption(num++, MultiMenu.imposter, "<color=#02A752FF>Blackmailer</color>");
            BlackmailerCount = new CustomNumberOption(num++, MultiMenu.imposter, "<color=#02A752FF>Blackmailer</color> Count", 1, 1, 14, 1);
            BlackmailCooldown = new CustomNumberOption(num++, MultiMenu.imposter, "Initial Blackmail Cooldown", 10, 1, 15, 1f, CooldownFormat);

            Consigliere = new CustomHeaderOption(num++, MultiMenu.imposter,  "<color=#FFFF99FF>Consigliere</color>");
            RevealCooldown = new CustomNumberOption(num++, MultiMenu.imposter, "Reveal Cooldown", 25f, 10f, 100f, 2.5f, CooldownFormat);
            ConsigInfo = new CustomStringOption(num++, MultiMenu.imposter, "Info That <color=#FFFF99FF>Consigliere</color> Sees", new[] { "Role", "Team" });

            Miner = new CustomHeaderOption(num++, MultiMenu.imposter, "<color=#AA7632FF>Miner</color>");
            MinerCount = new CustomNumberOption(num++, MultiMenu.imposter, "<color=#AA7632FF>Miner</color> Count", 1, 1, 14, 1);
            MineCooldown = new CustomNumberOption(num++, MultiMenu.imposter, "Mine Cooldown", 25, 10, 40, 2.5f, CooldownFormat);

            TimeMaster = new CustomHeaderOption(num++, MultiMenu.imposter, "<color=#0000A7FF>Time Master</color>");
            TMCount = new CustomNumberOption(num++, MultiMenu.imposter, "<color=#0000A7FF>Time Master</color> Count", 1, 1, 14, 1);
            FreezerCooldown = new CustomNumberOption(num++, MultiMenu.imposter, "Freeze Cooldown", 25, 10, 40, 2.5f, CooldownFormat);
            FreezeDuration = new CustomNumberOption(num++, MultiMenu.imposter, "Freeze Duration", 20.0f, 5f, 60f, 5f, CooldownFormat);

            Traitor = new CustomHeaderOption(num++, MultiMenu.imposter, "<color=#370D43FF>Traitor</color>");
            LatestSpawn = new CustomNumberOption(num++, MultiMenu.imposter, "Minimum People Alive When <color=#370D43FF>Traitor</color> Can Spawn", 5, 3, 15, 1);
            NeutralKillingStopsTraitor = new CustomToggleOption(num++, MultiMenu.imposter, "<color=#370D43FF>Traitor</color> Won't Spawn If A <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killing</color> Role Is Alive", false);

            PositiveModifierSettings = new CustomHeaderOption(num++, MultiMenu.modifiers, "<color=#7FFF00FF>Positive</color> Modifier Settings");

            Bait = new CustomHeaderOption(num++, MultiMenu.modifiers, "<color=#00B3B3FF>Bait</color>");
            BaitMinDelay = new CustomNumberOption(num++, MultiMenu.modifiers, "Minimum Delay for <color=#00B3B3FF>Bait</color> Self Report", 0f, 0f, 15f, 0.5f, CooldownFormat);
            BaitMaxDelay = new CustomNumberOption(num++, MultiMenu.modifiers, "Maximum Delay for <color=#00B3B3FF>Bait</color> Self Report", 1f, 0f, 15f, 0.5f, CooldownFormat);

            Diseased = new CustomHeaderOption(num++, MultiMenu.modifiers, "<color=#808080FF>Diseased</color>");
            DiseasedKillMultiplier = new CustomNumberOption(num++, MultiMenu.modifiers, "<color=#808080FF>Diseased</color> Kill Multiplier", 3f, 1.5f, 5f, 0.5f, MultiplierFormat);

            NeutralModifierSettings = new CustomHeaderOption(num++, MultiMenu.modifiers, "<color=#B3B3B3FF>Neutral</color> Modifier Settings");

            Dwarf = new CustomHeaderOption(num++, MultiMenu.modifiers, "<color=#FF8080FF>Dwarf</color>");
            DwarfSpeed = new CustomNumberOption(num++, MultiMenu.modifiers, "<color=#FF8080FF>Dwarf</color> Speed", 1.3f, 1.0f, 2f, 0.05f, MultiplierFormat);
            DwarfScale = new CustomNumberOption(num++, MultiMenu.modifiers, "<color=#FF8080FF>Dwarf</color> Scale", 0.5f, 0.3f, 0.6f, 0.025f, MultiplierFormat);

            Giant = new CustomHeaderOption(num++, MultiMenu.modifiers, "<color=#FFB34DFF>Giant</color>");
            GiantSpeed = new CustomNumberOption(num++, MultiMenu.modifiers, "<color=#FFB34DFF>Giant</color> Speed", 0.75f, 0.5f, 1f, 0.05f, MultiplierFormat);
            GiantScale = new CustomNumberOption(num++, MultiMenu.modifiers, "<color=#FFB34DFF>Giant</color> Scale", 1.5f, 1.5f, 3.0f, 0.025f, MultiplierFormat);

            Lovers = new CustomHeaderOption(num++, MultiMenu.modifiers, "<color=#FF66CCFF>Lovers</color>");
            BothLoversDie = new CustomToggleOption(num++, MultiMenu.modifiers, "Both <color=#FF66CCFF>Lovers</color> Die", true);
        }
    }
}