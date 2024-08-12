namespace TownOfUsReworked.Options2;

// DO NOT OVERRIDE VALUES OF ANY OF THESE PROPERTIES ANY WHERE IN THE CODE OR ELSE THE OPTIONS WILL START TO FUCK OFF

[HeaderOption(MultiMenu2.Main)]
public static class GameSettings
{
    [NumberOption(MultiMenu2.Main, 0.25f, 10, 0.25f, Format.Multiplier)]
    public static float PlayerSpeed { get; set; } = 1.25f;

    [NumberOption(MultiMenu2.Main, 0.25f, 10, 0.25f, Format.Multiplier)]
    public static float GhostSpeed { get; set; } = 3;

    [NumberOption(MultiMenu2.Main, 0.5f, 5, 0.5f, Format.Distance)]
    public static float InteractionDistance { get; set; } = 2;

    [NumberOption(MultiMenu2.Main, 0, 100, 1)]
    public static int EmergencyButtonCount { get; set; } = 1;

    [NumberOption(MultiMenu2.Main, 0, 300, 2.5f, Format.Time)]
    public static float EmergencyButtonCooldown { get; set; } = 25;

    [NumberOption(MultiMenu2.Main, 0, 300, 5, Format.Time)]
    public static float DiscussionTime { get; set; } = 30;

    [NumberOption(MultiMenu2.Main, 5, 600, 15, Format.Time)]
    public static float VotingTime { get; set; } = 60;

    [StringOption(MultiMenu2.Main)]
    private static TBMode TaskBar { get; set; } = TBMode.MeetingOnly;
    public static TBMode TaskBarMode => GameModeSettings.GameMode switch
    {
        GameMode.TaskRace or GameMode.HideAndSeek => TBMode.Normal,
        _ => TaskBar
    }; // I want this to actually change according to the game modes

    [ToggleOption(MultiMenu2.Main)]
    public static bool ConfirmEjects { get; set; } = false;

    [ToggleOption(MultiMenu2.Main)]
    public static bool EjectionRevealsRoles { get; set; } = false;

    [ToggleOption(MultiMenu2.Main)]
    public static bool EnableInitialCds { get; set; } = true;

    [NumberOption(MultiMenu2.Main, 0, 30, 2.5f, Format.Time)]
    public static float InitialCooldowns { get; set; } = 10;

    [ToggleOption(MultiMenu2.Main)]
    public static bool EnableMeetingCds { get; set; } = true;

    [NumberOption(MultiMenu2.Main, 0, 30, 2.5f, Format.Time)]
    public static float MeetingCooldowns { get; set; } = 15;

    [ToggleOption(MultiMenu2.Main)]
    public static bool EnableFailCds { get; set; } = true;

    [NumberOption(MultiMenu2.Main, 0, 30, 2.5f, Format.Time)]
    public static float FailCooldowns { get; set; } = 5;

    [NumberOption(MultiMenu2.Main, 1, 20, 0.25f, Format.Distance)]
    public static float ReportDistance { get; set; } = 3.5f;

    [NumberOption(MultiMenu2.Main, 0, 3, 0.1f, Format.Time)]
    public static float ChatCooldown { get; set; } = 3;

    [NumberOption(MultiMenu2.Main, 50, 2000, 50)]
    public static int ChatCharacterLimit { get; set; } = 200;

    [NumberOption(MultiMenu2.Main, 2, 127, 1)]
    public static int LobbySize { get; set; } = 15;
}

[HeaderOption(MultiMenu2.Main)]
public static class GameModeSettings
{
    [StringOption(MultiMenu2.Main, [ "None" ])]
    public static GameMode GameMode { get; set; } = GameMode.Classic;

    [ToggleOption(MultiMenu2.Main)]
    public static bool IgnoreFactionCaps { get; set; } = false;

    [ToggleOption(MultiMenu2.Main)]
    public static bool IgnoreAlignmentCaps { get; set; } = false;

    [ToggleOption(MultiMenu2.Main)]
    public static bool IgnoreLayerCaps { get; set; } = false;

    [NumberOption(MultiMenu2.Main, 0, 13, 1)]
    public static int NeutralsCount { get; set; } = 1;

    [ToggleOption(MultiMenu2.Main)]
    public static bool AddArsonist { get; set; } = false;

    [ToggleOption(MultiMenu2.Main)]
    public static bool AddCryomaniac { get; set; } = false;

    [ToggleOption(MultiMenu2.Main)]
    public static bool AddPlaguebearer { get; set; } = false;

    [StringOption(MultiMenu2.Main)]
    public static HnSMode HnSMode { get; set; } = HnSMode.Classic;

    [NumberOption(MultiMenu2.Main, 0, 13, 1)]
    public static int HnSShortTasks { get; set; } = 4;

    [NumberOption(MultiMenu2.Main, 0, 13, 1)]
    public static int HnSCommonTasks { get; set; } = 4;

    [NumberOption(MultiMenu2.Main, 0, 13, 1)]
    public static int HnSLongTasks { get; set; } = 4;

    [NumberOption(MultiMenu2.Main, 1, 13, 1)]
    public static int HunterCount { get; set; } = 1;

    [NumberOption(MultiMenu2.Main, 5f, 60f, 5f, Format.Time)]
    public static float HuntCd { get; set; } = 10;

    [NumberOption(MultiMenu2.Main, 5f, 60f, 5f, Format.Time)]
    public static float StartTime { get; set; } = 10;

    [ToggleOption(MultiMenu2.Main)]
    public static bool HunterVent { get; set; } = true;

    [NumberOption(MultiMenu2.Main, 0.1f, 1f, 0.05f, Format.Multiplier)]
    public static float HunterVision { get; set;} = 0.25f;

    [NumberOption(MultiMenu2.Main, 1f, 2f, 0.05f, Format.Multiplier)]
    public static float HuntedVision { get; set;} = 1.5f;

    [NumberOption(MultiMenu2.Main, 1f, 1.5f, 0.05f, Format.Multiplier)]
    public static float HunterSpeedModifier { get; set; } = 1.25f;

    [ToggleOption(MultiMenu2.Main)]
    public static bool HunterFlashlight { get; set; } = false;

    [ToggleOption(MultiMenu2.Main)]
    public static bool HuntedFlashlight { get; set; } = false;

    [ToggleOption(MultiMenu2.Main)]
    public static bool HuntedChat { get; set; } = true;

    [NumberOption(MultiMenu2.Main, 0, 13, 1)]
    public static int TRShortTasks { get; set; } = 4;

    [NumberOption(MultiMenu2.Main, 0, 13, 1)]
    public static int TRCommonTasks { get; set; } = 4;
}

[HeaderOption(MultiMenu2.Main)]
public static class GameModifiers
{
    [StringOption(MultiMenu2.Main)]
    public static WhoCanVentOptions WhoCanVent { get; set; } = WhoCanVentOptions.Default;

    [StringOption(MultiMenu2.Main)]
    public static AnonVotes AnonymousVoting { get; set; } = AnonVotes.Enabled;

    [StringOption(MultiMenu2.Main)]
    public static DisableSkipButtonMeetings NoSkipping { get; set; } = DisableSkipButtonMeetings.No;

    [ToggleOption(MultiMenu2.Main)]
    public static bool FirstKillShield { get; set; } = false;

    [StringOption(MultiMenu2.Main)]
    public static WhoCanSeeFirstKillShield WhoSeesFirstKillShield { get; set; } = WhoCanSeeFirstKillShield.Everyone;

    [ToggleOption(MultiMenu2.Main)]
    public static bool FactionSeeRoles { get; set; } = true;

    [ToggleOption(MultiMenu2.Main)]
    public static bool VisualTasks { get; set; } = false;

    [StringOption(MultiMenu2.Main)]
    public static PlayerNames PlayerNames { get; set; } = PlayerNames.Obstructed;

    [ToggleOption(MultiMenu2.Main)]
    public static bool Whispers { get; set; } = true;

    [ToggleOption(MultiMenu2.Main)]
    public static bool WhispersAnnouncement { get; set; } = true;

    [ToggleOption(MultiMenu2.Main)]
    public static bool AppearanceAnimation { get; set; } = true;

    [ToggleOption(MultiMenu2.Main)]
    public static bool EnableAbilities { get; set; } = true;

    [ToggleOption(MultiMenu2.Main)]
    public static bool EnableModifiers { get; set; } = true;

    [ToggleOption(MultiMenu2.Main)]
    public static bool EnableObjectifiers { get; set; } = true;

    [ToggleOption(MultiMenu2.Main)]
    public static bool VentTargeting { get; set; } = true;

    [ToggleOption(MultiMenu2.Main)]
    public static bool CooldownInVent { get; set; } = false;

    [ToggleOption(MultiMenu2.Main)]
    public static bool DeadSeeEverything { get; set; } = true;

    [ToggleOption(MultiMenu2.Main)]
    public static bool ParallelMedScans { get; set; } = false;

    [ToggleOption(MultiMenu2.Main)]
    public static bool HideVentAnims { get; set; } = true;

    [ToggleOption(MultiMenu2.Main)]
    public static bool JaniCanMutuallyExclusive { get; set; } = false;

    [ToggleOption(MultiMenu2.Main)]
    public static bool IndicateReportedBodies { get; set; } = false;

    [StringOption(MultiMenu2.Main)]
    public static RandomSpawning RandomSpawns { get; set; } = RandomSpawning.Disabled;
}

[HeaderOption(MultiMenu2.Main)]
public static class GameAnnouncementSettings
{
    [ToggleOption(MultiMenu2.Main)]
    public static bool GameAnnouncements { get; set; } = false;

    [ToggleOption(MultiMenu2.Main)]
    public static bool LocationReports { get; set; } = false;

    [StringOption(MultiMenu2.Main)]
    public static RoleFactionReports RoleFactionReports { get; set; } = RoleFactionReports.Neither;

    [StringOption(MultiMenu2.Main)]
    public static RoleFactionReports KillerReports { get; set; } = RoleFactionReports.Neither;
}

[HeaderOption(MultiMenu2.Main)]
public static class MapSettings
{
    public static MapEnum Map { get; set; }

    [NumberOption(MultiMenu2.Main, 0, 100, 10, Format.Percent)]
    public static int RandomMapSkeld { get; set; } = 10;

    [NumberOption(MultiMenu2.Main, 0, 100, 10, Format.Percent)]
    public static int RandomMapMira { get; set; } = 10;

    [NumberOption(MultiMenu2.Main, 0, 100, 10, Format.Percent)]
    public static int RandomMapPolus { get; set; } = 10;

    [NumberOption(MultiMenu2.Main, 0, 100, 10, Format.Percent)]
    public static int RandomMapdlekS { get; set; } = 10;

    [NumberOption(MultiMenu2.Main, 0, 100, 10, Format.Percent)]
    public static int RandomMapAirship { get; set; } = 10;

    [NumberOption(MultiMenu2.Main, 0, 100, 10, Format.Percent)]
    public static int RandomMapFungle { get; set; } = 10;

    [NumberOption(MultiMenu2.Main, 0, 100, 10, Format.Percent, All = true)]
    public static int RandomMapSubmerged { get; set; } = 10;

    [NumberOption(MultiMenu2.Main, 0, 100, 10, Format.Percent, All = true)]
    public static int RandomMapLevelImpostor { get; set; } = 10;

    [ToggleOption(MultiMenu2.Main)]
    public static bool AutoAdjustSettings { get; set; } = false;

    [ToggleOption(MultiMenu2.Main)]
    public static bool SmallMapHalfVision { get; set; } = false;

    [NumberOption(MultiMenu2.Main, 0f, 15f, 2.5f, Format.Time)]
    public static float SmallMapDecreasedCooldown { get; set; } = 0f;

    [NumberOption(MultiMenu2.Main, 0f, 15f, 2.5f, Format.Time)]
    public static float LargeMapIncreasedCooldown { get; set; } = 0f;

    [NumberOption(MultiMenu2.Main, 0, 5, 1)]
    public static int SmallMapIncreasedShortTasks { get; set; } = 0;

    [NumberOption(MultiMenu2.Main, 0, 3, 1)]
    public static int SmallMapIncreasedLongTasks { get; set; } = 0;

    [NumberOption(MultiMenu2.Main, 0, 5, 1)]
    public static int LargeMapDecreasedShortTasks { get; set; } = 0;

    [NumberOption(MultiMenu2.Main, 0, 3, 1)]
    public static int LargeMapDecreasedLongTasks { get; set; } = 0;
}

[HeaderOption(MultiMenu2.Main)]
public static class BetterSabotages
{
    [ToggleOption(MultiMenu2.Main)]
    public static bool CamouflagedComms { get; set; } = true;

    [ToggleOption(MultiMenu2.Main)]
    public static bool CamouflagedMeetings { get; set; } = false;

    [ToggleOption(MultiMenu2.Main)]
    public static bool NightVision { get; set; } = false;

    [ToggleOption(MultiMenu2.Main)]
    public static bool EvilsIgnoreNV { get; set; } = false;

    [ToggleOption(MultiMenu2.Main)]
    public static bool OxySlow { get; set; } = true;

    [NumberOption(MultiMenu2.Main, 0, 100, 5, Format.Percent)]
    public static int ReactorShake { get; set; } = 30;
}

// Better Skeld Settings
[HeaderOption(MultiMenu2.Main)]
public static class BetterSkeldOptions
{
    [ToggleOption(MultiMenu2.Main)]
    public static bool EnableBetterSkeld { get; set; } = true;

    [ToggleOption(MultiMenu2.Main)]
    public static bool SkeldVentImprovements { get; set; } = false;

    [NumberOption(MultiMenu2.Main, 30f, 90f, 5f, Format.Time)]
    public static float SkeldReactorTimer { get; set; } = 60f;

    [NumberOption(MultiMenu2.Main, 30f, 90f, 5f, Format.Time)]
    public static float SkeldO2Timer { get; set; } = 60f;
}

[HeaderOption(MultiMenu2.Main)]
public static class BetterMiraHQOptions
{
    [ToggleOption(MultiMenu2.Main)]
    public static bool EnableBetterMiraHQ { get; set; } = true;

    [ToggleOption(MultiMenu2.Main)]
    public static bool MiraHQVentImprovements { get; set; } = false;

    [NumberOption(MultiMenu2.Main, 30f, 90f, 5f, Format.Time)]
    public static float MiraReactorTimer { get; set; } = 60f;

    [NumberOption(MultiMenu2.Main, 30f, 90f, 5f, Format.Time)]
    public static float MiraO2Timer { get; set; } = 60f;
}

[HeaderOption(MultiMenu2.Main)]
public static class BetterPolusOptions
{
    [ToggleOption(MultiMenu2.Main)]
    public static bool EnableBetterPolus { get; set; } = true;

    [ToggleOption(MultiMenu2.Main)]
    public static bool PolusVentImprovements { get; set; } = false;

    [ToggleOption(MultiMenu2.Main)]
    public static bool VitalsLab { get; set; } = false;

    [ToggleOption(MultiMenu2.Main)]
    public static bool ColdTempDeathValley { get; set; } = false;

    [ToggleOption(MultiMenu2.Main)]
    public static bool WifiChartCourseSwap { get; set; } = false;

    [NumberOption(MultiMenu2.Main, 30f, 90f, 5f, Format.Time)]
    public static float SeismicTimer { get; set; } = 60f;
}

[HeaderOption(MultiMenu2.Main)]
public static class BetterAirshipOptions
{
    [ToggleOption(MultiMenu2.Main)]
    public static bool EnableBetterAirship { get; set; } = true;

    [StringOption(MultiMenu2.Main)]
    public static AirshipSpawnType SpawnType { get; set; } = AirshipSpawnType.Normal;

    [ToggleOption(MultiMenu2.Main)]
    public static bool MoveVitals { get; set; } = false;

    [ToggleOption(MultiMenu2.Main)]
    public static bool MoveFuel { get; set; } = false;

    [ToggleOption(MultiMenu2.Main)]
    public static bool MoveDivert { get; set; } = false;

    [StringOption(MultiMenu2.Main)]
    public static MoveAdmin MoveAdmin { get; set; } = MoveAdmin.DontMove;

    [StringOption(MultiMenu2.Main)]
    public static MoveElectrical MoveElectrical { get; set; } = MoveElectrical.DontMove;

    [NumberOption(MultiMenu2.Main, 0f, 10f, 0.1f)]
    public static float MinDoorSwipeTime { get; set; } = 0.4f;

    [NumberOption(MultiMenu2.Main, 30f, 100f, 5f, Format.Time)]
    public static float CrashTimer { get; set; } = 90f;

}

[HeaderOption(MultiMenu2.Main)]
public static class BetterFungleOptions
{
    [ToggleOption(MultiMenu2.Main)]
    public static bool EnableBetterFungle { get; set; } = true;

    [NumberOption(MultiMenu2.Main, 30f, 90f, 5f, Format.Time)]
    public static float FungleReactorTimer { get; set; } = 60f;

    [NumberOption(MultiMenu2.Main, 4f, 20f, 1f, Format.Time)]
    public static float FungleMixupTimer { get; set; } = 8f;

}

[HeaderOption(MultiMenu2.Main)]
public static class CrewSettings
{
    [NumberOption(MultiMenu2.Main, 0, 100, 1)]
    public static int CommonTasks { get; set; } = 2;

    [NumberOption(MultiMenu2.Main, 0, 100, 1)]
    public static int LongTasks { get; set; } = 1;

    [NumberOption(MultiMenu2.Main, 0, 100, 1)]
    public static int ShortTasks { get; set; } = 4;

    [ToggleOption(MultiMenu2.Main)]
    public static bool GhostTasksCountToWin { get; set; } = true;

    [NumberOption(MultiMenu2.Main, 0.25f, 5f, 0.25f, Format.Multiplier)]
    public static float CrewVision { get; set; } = 1f;

    [ToggleOption(MultiMenu2.Main)]
    public static bool CrewFlashlight { get; set; } = false;

    [NumberOption(MultiMenu2.Main, 0, 14, 1)]
    public static int CrewMax { get; set; } = 5;

    [NumberOption(MultiMenu2.Main, 0, 14, 1)]
    public static int CrewMin { get; set; } = 5;

    [ToggleOption(MultiMenu2.Main)]
    public static bool CrewVent { get; set; } = false;
}

[HeaderOption(MultiMenu2.Layer, HeaderType.Layer)]
public static class CrewAuditorRoles
{
    [LayersOption(MultiMenu2.Layer, "#708EEFFF", LayerEnum.Mystic)]
    public static RoleOptionData Mystic { get; set; }

    [LayersOption(MultiMenu2.Layer, "#C0C0C0FF", LayerEnum.VampireHunter)]
    public static RoleOptionData VampireHunter { get; set; }
}

[HeaderOption(MultiMenu2.Layer, HeaderType.Layer)]
public static class CrewInvestigativeRoles
{
    [LayersOption(MultiMenu2.Layer, "#4D99E6FF", LayerEnum.Coroner)]
    public static RoleOptionData Coroner { get; set; }

    [LayersOption(MultiMenu2.Layer, "#4D4DFFFF", LayerEnum.Detective)]
    public static RoleOptionData Detective { get; set; }

    [LayersOption(MultiMenu2.Layer, "#A680FFFF", LayerEnum.Medium)]
    public static RoleOptionData Medium { get; set; }

    [LayersOption(MultiMenu2.Layer, "#A7D1B3FF", LayerEnum.Operative)]
    public static RoleOptionData Operative { get; set; }

    [LayersOption(MultiMenu2.Layer, "#71368AFF", LayerEnum.Seer)]
    public static RoleOptionData Seer { get; set; }

    [LayersOption(MultiMenu2.Layer, "#FFCC80FF", LayerEnum.Sheriff)]
    public static RoleOptionData Sheriff { get; set; }

    [LayersOption(MultiMenu2.Layer, "#009900FF", LayerEnum.Tracker)]
    public static RoleOptionData Tracker { get; set; }
}

[HeaderOption(MultiMenu2.Layer, HeaderType.Layer)]
public static class CrewKillingRoles
{
    [LayersOption(MultiMenu2.Layer, "#7E3C64FF", LayerEnum.Bastion)]
    private static RoleOptionData BastionPriv { get; set; }
    public static RoleOptionData Bastion
    {
        get
        {
            var result = BastionPriv.Clone();

            if (IsKilling)
                result.Chance = 100;

            return result;
        }
    }

    [LayersOption(MultiMenu2.Layer, "#998040FF", LayerEnum.Veteran)]
    private static RoleOptionData VeteranPriv { get; set; }
    public static RoleOptionData Veteran
    {
        get
        {
            var result = VeteranPriv.Clone();

            if (IsKilling)
                result.Chance = 100;

            return result;
        }
    }

    [LayersOption(MultiMenu2.Layer, "#FFFF00FF", LayerEnum.Vigilante)]
    private static RoleOptionData VigilantePriv { get; set; }
    public static RoleOptionData Vigilante
    {
        get
        {
            var result = VigilantePriv.Clone();

            if (IsKilling)
                result.Chance = 100;

            return result;
        }
    }
}

[HeaderOption(MultiMenu2.Layer, HeaderType.Layer)]
public static class CrewProtectiveRoles
{
    [LayersOption(MultiMenu2.Layer, "#660000FF", LayerEnum.Altruist)]
    public static RoleOptionData Altruist { get; set; }

    [LayersOption(MultiMenu2.Layer, "#006600FF", LayerEnum.Medic)]
    public static RoleOptionData Medic { get; set; }

    [LayersOption(MultiMenu2.Layer, "#BE1C8CFF", LayerEnum.Trapper)]
    public static RoleOptionData Trapper { get; set; }
}

[HeaderOption(MultiMenu2.Layer, HeaderType.Layer)]
public static class CrewSovereignRoles
{
    [LayersOption(MultiMenu2.Layer, "#00CB97FF", LayerEnum.Dictator)]
    public static RoleOptionData Dictator { get; set; }

    [LayersOption(MultiMenu2.Layer, "#704FA8FF", LayerEnum.Mayor)]
    public static RoleOptionData Mayor { get; set; }

    [LayersOption(MultiMenu2.Layer, "#FF004EFF", LayerEnum.Monarch)]
    public static RoleOptionData Monarch { get; set; }
}

[HeaderOption(MultiMenu2.Layer, HeaderType.Layer)]
public static class CrewSupportRoles
{
    [LayersOption(MultiMenu2.Layer, "#5411F8FF", LayerEnum.Chameleon)]
    public static RoleOptionData Chameleon { get; set; }

    [LayersOption(MultiMenu2.Layer, "#FFA60AFF", LayerEnum.Engineer)]
    public static RoleOptionData Engineer { get; set; }

    [LayersOption(MultiMenu2.Layer, "#803333FF", LayerEnum.Escort)]
    public static RoleOptionData Escort { get; set; }

    [LayersOption(MultiMenu2.Layer, "#8D0F8CFF", LayerEnum.Retributionist)]
    public static RoleOptionData Retributionist { get; set; }

    [LayersOption(MultiMenu2.Layer, "#DF851FFF", LayerEnum.Shifter)]
    public static RoleOptionData Shifter { get; set; }

    [LayersOption(MultiMenu2.Layer, "#00EEFFFF", LayerEnum.Transporter)]
    public static RoleOptionData Transporter { get; set; }
}

[HeaderOption(MultiMenu2.Layer, HeaderType.Layer)]
public static class CrewUtilityRoles
{
    [LayersOption(MultiMenu2.Layer, "#8CFFFFFF", LayerEnum.Crewmate)]
    private static RoleOptionData CrewmatePriv { get; set; }
    public static RoleOptionData Crewmate
    {
        get
        {
            var result = CrewmatePriv.Clone();

            if (!IsCustom)
                result.Chance = 100;

            return result;
        }
    }

    [LayersOption(MultiMenu2.Layer, "#D3D3D3FF", LayerEnum.Revealer)]
    public static RoleOptionData Revealer { get; set; }
}

[HeaderOption(MultiMenu2.Layer, HeaderType.Layer)]
public static class NeutralBenignRoles
{
    [LayersOption(MultiMenu2.Layer, "#22FFFFFF", LayerEnum.Amnesiac)]
    public static RoleOptionData Amnesiac { get; set; }

    [LayersOption(MultiMenu2.Layer, "#FFFFFFFF", LayerEnum.GuardianAngel)]
    public static RoleOptionData GuardianAngel { get; set; }

    [LayersOption(MultiMenu2.Layer, "#DDDD00FF", LayerEnum.Survivor)]
    public static RoleOptionData Survivor { get; set; }

    [LayersOption(MultiMenu2.Layer, "#80FF00FF", LayerEnum.Thief)]
    public static RoleOptionData Thief { get; set; }
}

[HeaderOption(MultiMenu2.Layer, HeaderType.Layer)]
public static class NeutralEvilRoles
{
    [LayersOption(MultiMenu2.Layer, "#00ACC2FF", LayerEnum.Actor)]
    public static RoleOptionData Actor { get; set; }

    [LayersOption(MultiMenu2.Layer, "#B51E39FF", LayerEnum.BountyHunter)]
    public static RoleOptionData BountyHunter { get; set; }

    [LayersOption(MultiMenu2.Layer, "#8C4005FF", LayerEnum.Cannibal)]
    public static RoleOptionData Cannibal { get; set; }

    [LayersOption(MultiMenu2.Layer, "#CCCCCCFF", LayerEnum.Executioner)]
    public static RoleOptionData Executioner { get; set; }

    [LayersOption(MultiMenu2.Layer, "#EEE5BEFF", LayerEnum.Guesser)]
    public static RoleOptionData Guesser { get; set; }

    [LayersOption(MultiMenu2.Layer, "#F7B3DAFF", LayerEnum.Jester)]
    public static RoleOptionData Jester { get; set; }

    [LayersOption(MultiMenu2.Layer, "#678D36FF", LayerEnum.Troll)]
    public static RoleOptionData Troll { get; set; }
}

[HeaderOption(MultiMenu2.Layer, HeaderType.Layer)]
public static class NeutralHarbingerRoles
{
    [LayersOption(MultiMenu2.Layer, "#CFFE61FF", LayerEnum.Plaguebearer)]
    public static RoleOptionData Plaguebearer { get; set; }
}

[HeaderOption(MultiMenu2.Layer, HeaderType.Layer)]
public static class NeutralKillingRoles
{
    [LayersOption(MultiMenu2.Layer, "#EE7600FF", LayerEnum.Arsonist)]
    public static RoleOptionData Arsonist { get; set; }

    [LayersOption(MultiMenu2.Layer, "#642DEAFF", LayerEnum.Cryomaniac)]
    public static RoleOptionData Cryomaniac { get; set; }

    [LayersOption(MultiMenu2.Layer, "#00FF00FF", LayerEnum.Glitch)]
    public static RoleOptionData Glitch { get; set; }

    [LayersOption(MultiMenu2.Layer, "#A12B56FF", LayerEnum.Juggernaut)]
    public static RoleOptionData Juggernaut { get; set; }

    [LayersOption(MultiMenu2.Layer, "#6F7BEAFF", LayerEnum.Murderer)]
    private static RoleOptionData MurdererPriv { get; set; }
    public static RoleOptionData Murderer
    {
        get
        {
            var result = MurdererPriv.Clone();

            if (IsKilling)
                result.Chance = 5;

            return result;
        }
    }

    [LayersOption(MultiMenu2.Layer, "#336EFFFF", LayerEnum.SerialKiller)]
    public static RoleOptionData SerialKiller { get; set; }

    [LayersOption(MultiMenu2.Layer, "#9F703AFF", LayerEnum.Werewolf)]
    public static RoleOptionData Werewolf { get; set; }
}

[HeaderOption(MultiMenu2.Layer, HeaderType.Layer)]
public static class NeutralNeophyteRoles
{
    [LayersOption(MultiMenu2.Layer, "#AC8A00FF", LayerEnum.Dracula)]
    public static RoleOptionData Dracula { get; set; }

    [LayersOption(MultiMenu2.Layer, "#45076AFF", LayerEnum.Jackal)]
    public static RoleOptionData Jackal { get; set; }

    [LayersOption(MultiMenu2.Layer, "#BF5FFFFF", LayerEnum.Necromancer)]
    public static RoleOptionData Necromancer { get; set; }

    [LayersOption(MultiMenu2.Layer, "#2D6AA5FF", LayerEnum.Whisperer)]
    public static RoleOptionData Whisperer { get; set; }
}

[HeaderOption(MultiMenu2.Layer, HeaderType.Layer)]
public static class NeutralProselyteRoles
{
    [LayersOption(MultiMenu2.Layer, "#662962FF", LayerEnum.Phantom)]
    public static RoleOptionData Phantom { get; set; }
}

[HeaderOption(MultiMenu2.Layer, HeaderType.Layer)]
public static class IntruderConcealingRoles
{
    [LayersOption(MultiMenu2.Layer, "#02A752FF", LayerEnum.Blackmailer)]
    public static RoleOptionData Blackmailer { get; set; }

    [LayersOption(MultiMenu2.Layer, "#378AC0FF", LayerEnum.Camouflager)]
    public static RoleOptionData Camouflager { get; set; }

    [LayersOption(MultiMenu2.Layer, "#85AA5BFF", LayerEnum.Grenadier)]
    public static RoleOptionData Grenadier { get; set; }

    [LayersOption(MultiMenu2.Layer, "#2647A2FF", LayerEnum.Janitor)]
    public static RoleOptionData Janitor { get; set; }
}

[HeaderOption(MultiMenu2.Layer, HeaderType.Layer)]
public static class IntruderDeceptionRoles
{
    [LayersOption(MultiMenu2.Layer, "#40B4FFFF", LayerEnum.Disguiser)]
    public static RoleOptionData Disguiser { get; set; }

    [LayersOption(MultiMenu2.Layer, "#BB45B0FF", LayerEnum.Morphling)]
    public static RoleOptionData Morphling { get; set; }

    [LayersOption(MultiMenu2.Layer, "#5C4F75FF", LayerEnum.Wraith)]
    public static RoleOptionData Wraith { get; set; }
}

[HeaderOption(MultiMenu2.Layer, HeaderType.Layer)]
public static class IntruderHeadRoles
{
    [LayersOption(MultiMenu2.Layer, "#404C08FF", LayerEnum.Godfather)]
    public static RoleOptionData Godfather { get; set; }
}

[HeaderOption(MultiMenu2.Layer, HeaderType.Layer)]
public static class IntruderKillingRoles
{
    [LayersOption(MultiMenu2.Layer, "#2BD29CFF", LayerEnum.Ambusher)]
    public static RoleOptionData Ambusher { get; set; }

    [LayersOption(MultiMenu2.Layer, "#005643FF", LayerEnum.Enforcer)]
    public static RoleOptionData Enforcer { get; set; }
}

[HeaderOption(MultiMenu2.Layer, HeaderType.Layer)]
public static class IntruderSupportRoles
{
    [LayersOption(MultiMenu2.Layer, "#FFFF99FF", LayerEnum.Consigliere)]
    public static RoleOptionData Consigliere { get; set; }

    [LayersOption(MultiMenu2.Layer, "#801780FF", LayerEnum.Consort)]
    public static RoleOptionData Consort { get; set; }

    [LayersOption(MultiMenu2.Layer, "#AA7632FF", LayerEnum.Miner)]
    public static RoleOptionData Miner { get; set; }

    [LayersOption(MultiMenu2.Layer, "#939593FF", LayerEnum.Teleporter)]
    public static RoleOptionData Teleporter { get; set; }
}

[HeaderOption(MultiMenu2.Layer, HeaderType.Layer)]
public static class IntruderUtilityRoles
{
    [LayersOption(MultiMenu2.Layer, "#F1C40FFF", LayerEnum.Ghoul)]
    public static RoleOptionData Ghoul { get; set; }

    [LayersOption(MultiMenu2.Layer, "#FF1919FF", LayerEnum.Impostor)]
    private static RoleOptionData ImpostorPriv { get; set; }
    public static RoleOptionData Impostor
    {
        get
        {
            var result = ImpostorPriv.Clone();

            if (IsKilling)
                result.Chance = 5;
            else if (!IsCustom)
                result.Chance = 100;

            return result;
        }
    }
}

[HeaderOption(MultiMenu2.Layer, HeaderType.Layer)]
public static class SyndicateDisruptionRoles
{
    [LayersOption(MultiMenu2.Layer, "#C02525FF", LayerEnum.Concealer)]
    public static RoleOptionData Concealer { get; set; }

    [LayersOption(MultiMenu2.Layer, "#FF7900FF", LayerEnum.Drunkard)]
    public static RoleOptionData Drunkard { get; set; }

    [LayersOption(MultiMenu2.Layer, "#00FFFFFF", LayerEnum.Framer)]
    public static RoleOptionData Framer { get; set; }

    [LayersOption(MultiMenu2.Layer, "#2DFF00FF", LayerEnum.Shapeshifter)]
    public static RoleOptionData Shapeshifter { get; set; }

    [LayersOption(MultiMenu2.Layer, "#AAB43EFF", LayerEnum.Silencer)]
    public static RoleOptionData Silencer { get; set; }

    [LayersOption(MultiMenu2.Layer, "#3769FEFF", LayerEnum.Timekeeper)]
    public static RoleOptionData Timekeeper { get; set; }
}

[HeaderOption(MultiMenu2.Layer, HeaderType.Layer)]
public static class SyndicateKillingRoles
{
    [LayersOption(MultiMenu2.Layer, "#C9CC3FFF", LayerEnum.Bomber)]
    public static RoleOptionData Bomber { get; set; }

    [LayersOption(MultiMenu2.Layer, "#B345FFFF", LayerEnum.Collider)]
    public static RoleOptionData Collider { get; set; }

    [LayersOption(MultiMenu2.Layer, "#DF7AE8FF", LayerEnum.Crusader)]
    public static RoleOptionData Crusader { get; set; }

    [LayersOption(MultiMenu2.Layer, "#B5004CFF", LayerEnum.Poisoner)]
    public static RoleOptionData Poisoner { get; set; }
}

[HeaderOption(MultiMenu2.Layer, HeaderType.Layer)]
public static class SyndicatePowerRoles
{
    [LayersOption(MultiMenu2.Layer, "#FFFCCEFF", LayerEnum.Rebel)]
    public static RoleOptionData Rebel { get; set; }

    [LayersOption(MultiMenu2.Layer, "#0028F5FF", LayerEnum.Spellslinger)]
    public static RoleOptionData Spellslinger { get; set; }
}

[HeaderOption(MultiMenu2.Layer, HeaderType.Layer)]
public static class SyndicateSupportRoles
{
    [LayersOption(MultiMenu2.Layer, "#7E4D00FF", LayerEnum.Stalker)]
    public static RoleOptionData Stalker { get; set; }

    [LayersOption(MultiMenu2.Layer, "#8C7140FF", LayerEnum.Warper)]
    public static RoleOptionData Warper { get; set; }
}

[HeaderOption(MultiMenu2.Layer, HeaderType.Layer)]
public static class SyndicateUtilityRoles
{
    [LayersOption(MultiMenu2.Layer, "#008000FF", LayerEnum.Anarchist)]
    private static RoleOptionData AnarchistPriv { get; set; }
    public static RoleOptionData Anarchist
    {
        get
        {
            var result = AnarchistPriv.Clone();

            if (IsKilling)
                result.Chance = 5;
            else if (!IsCustom)
                result.Chance = 100;

            return result;
        }
    }

    [LayersOption(MultiMenu2.Layer, "#E67E22FF", LayerEnum.Banshee)]
    public static RoleOptionData Banshee { get; set; }
}

[HeaderOption(MultiMenu2.Layer, HeaderType.Layer)]
public static class Modifiers
{
    [LayersOption(MultiMenu2.Layer, "#612BEFFF", LayerEnum.Astral)]
    public static RoleOptionData Astral { get; set; }

    [LayersOption(MultiMenu2.Layer, "#00B3B3FF", LayerEnum.Bait)]
    public static RoleOptionData Bait { get; set; }

    [LayersOption(MultiMenu2.Layer, "#B34D99FF", LayerEnum.Colorblind)]
    public static RoleOptionData Colorblind { get; set; }

    [LayersOption(MultiMenu2.Layer, "#456BA8FF", LayerEnum.Coward)]
    public static RoleOptionData Coward { get; set; }

    [LayersOption(MultiMenu2.Layer, "#374D1EFF", LayerEnum.Diseased)]
    public static RoleOptionData Diseased { get; set; }

    [LayersOption(MultiMenu2.Layer, "#758000FF", LayerEnum.Drunk)]
    public static RoleOptionData Drunk { get; set; }

    [LayersOption(MultiMenu2.Layer, "#FF8080FF", LayerEnum.Dwarf)]
    public static RoleOptionData Dwarf { get; set; }

    [LayersOption(MultiMenu2.Layer, "#FFB34DFF", LayerEnum.Giant)]
    public static RoleOptionData Giant { get; set; }

    [LayersOption(MultiMenu2.Layer, "#2DE5BEFF", LayerEnum.Indomitable)]
    public static RoleOptionData Indomitable { get; set; }

    [LayersOption(MultiMenu2.Layer, "#860B7AFF", LayerEnum.Professional)]
    public static RoleOptionData Professional { get; set; }

    [LayersOption(MultiMenu2.Layer, "#1002C5FF", LayerEnum.Shy)]
    public static RoleOptionData Shy { get; set; }

    [LayersOption(MultiMenu2.Layer, "#DCEE85FF", LayerEnum.VIP)]
    public static RoleOptionData VIP { get; set; }

    [LayersOption(MultiMenu2.Layer, "#FFA60AFF", LayerEnum.Volatile)]
    public static RoleOptionData Volatile { get; set; }

    [LayersOption(MultiMenu2.Layer, "#F6AAB7FF", LayerEnum.Yeller)]
    public static RoleOptionData Yeller { get; set; }
}

[HeaderOption(MultiMenu2.Layer, HeaderType.Layer)]
public static class Abilities
{
    [LayersOption(MultiMenu2.Layer, "#8CFFFFFF", LayerEnum.Bullseye)]
    public static RoleOptionData Bullseye { get; set; }

    [LayersOption(MultiMenu2.Layer, "#E600FFFF", LayerEnum.ButtonBarry)]
    public static RoleOptionData ButtonBarry { get; set; }

    [LayersOption(MultiMenu2.Layer, "#FF1919FF", LayerEnum.Hitman)]
    public static RoleOptionData Hitman { get; set; }

    [LayersOption(MultiMenu2.Layer, "#26FCFBFF", LayerEnum.Insider)]
    public static RoleOptionData Insider { get; set; }

    [LayersOption(MultiMenu2.Layer, "#FF804DFF", LayerEnum.Multitasker)]
    public static RoleOptionData Multitasker { get; set; }

    [LayersOption(MultiMenu2.Layer, "#A84300FF", LayerEnum.Ninja)]
    public static RoleOptionData Ninja { get; set; }

    [LayersOption(MultiMenu2.Layer, "#CCA3CCFF", LayerEnum.Politician)]
    public static RoleOptionData Politician { get; set; }

    [LayersOption(MultiMenu2.Layer, "#FF0080FF", LayerEnum.Radar)]
    public static RoleOptionData Radar { get; set; }

    [LayersOption(MultiMenu2.Layer, "#2160DDFF", LayerEnum.Ruthless)]
    public static RoleOptionData Ruthless { get; set; }

    [LayersOption(MultiMenu2.Layer, "#B3B3B3FF", LayerEnum.Slayer)]
    public static RoleOptionData Slayer { get; set; }

    [LayersOption(MultiMenu2.Layer, "#008000FF", LayerEnum.Sniper)]
    public static RoleOptionData Sniper { get; set; }

    [LayersOption(MultiMenu2.Layer, "#D4AF37FF", LayerEnum.Snitch)]
    public static RoleOptionData Snitch { get; set; }

    [LayersOption(MultiMenu2.Layer, "#66E666FF", LayerEnum.Swapper)]
    public static RoleOptionData Swapper { get; set; }

    [LayersOption(MultiMenu2.Layer, "#99E699FF", LayerEnum.Tiebreaker)]
    public static RoleOptionData Tiebreaker { get; set; }

    [LayersOption(MultiMenu2.Layer, "#FFFF99FF", LayerEnum.Torch)]
    public static RoleOptionData Torch { get; set; }

    [LayersOption(MultiMenu2.Layer, "#E91E63FF", LayerEnum.Tunneler)]
    public static RoleOptionData Tunneler { get; set; }

    [LayersOption(MultiMenu2.Layer, "#841A7FFF", LayerEnum.Underdog)]
    public static RoleOptionData Underdog { get; set; }
}

[HeaderOption(MultiMenu2.Layer, HeaderType.Layer)]
public static class Objectifiers
{
    [LayersOption(MultiMenu2.Layer, "#4545A9FF", LayerEnum.Allied)]
    public static RoleOptionData Allied { get; set; }

    [LayersOption(MultiMenu2.Layer, "#4545FFFF", LayerEnum.Corrupted)]
    public static RoleOptionData Corrupted { get; set; }

    [LayersOption(MultiMenu2.Layer, "#E1C849FF", LayerEnum.Defector)]
    public static RoleOptionData Defector { get; set; }

    [LayersOption(MultiMenu2.Layer, "#678D36FF", LayerEnum.Fanatic)]
    public static RoleOptionData Fanatic { get; set; }

    [LayersOption(MultiMenu2.Layer, "#FF351FFF", LayerEnum.Linked, Min = 1, Max = 7)]
    public static RoleOptionData Linked { get; set; }

    [LayersOption(MultiMenu2.Layer, "#FF66CCFF", LayerEnum.Lovers, Min = 1, Max = 7)]
    public static RoleOptionData Lovers { get; set; }

    [LayersOption(MultiMenu2.Layer, "#00EEFFFF", LayerEnum.Mafia, Min = 2)]
    public static RoleOptionData Mafia { get; set; }

    [LayersOption(MultiMenu2.Layer, "#008080FF", LayerEnum.Overlord)]
    public static RoleOptionData Overlord { get; set; }

    [LayersOption(MultiMenu2.Layer, "#3D2D2CFF", LayerEnum.Rivals, Min = 1, Max = 7)]
    public static RoleOptionData Rivals { get; set; }

    [LayersOption(MultiMenu2.Layer, "#ABABFFFF", LayerEnum.Taskmaster)]
    public static RoleOptionData Taskmaster { get; set; }

    [LayersOption(MultiMenu2.Layer, "#370D43FF", LayerEnum.Traitor)]
    public static RoleOptionData Traitor { get; set; }
}