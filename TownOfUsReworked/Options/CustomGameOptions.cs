namespace TownOfUsReworked.Options;

public static class CustomGameOptions
{
    // Intruder Options
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

    // Syndicate Options
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

    // Neutral Options
    public static float NeutralVision => Generate.NeutralVision;
    public static bool LightsAffectNeutrals => Generate.LightsAffectNeutrals;
    public static NoSolo NoSolo => (NoSolo)Generate.NoSolo.GetInt();
    public static bool NeutralsVent => Generate.NeutralsVent;
    public static bool AvoidNeutralKingmakers => Generate.AvoidNeutralKingmakers;
    public static int NeutralMax => Generate.NeutralMax;
    public static int NeutralMin => Generate.NeutralMin;
    public static bool NeutralFlashlight => Generate.NeutralFlashlight;

    // Ghoul Settings
    public static float GhoulMarkCd => Generate.GhoulMarkCd;

    // Janitor Settings
    public static float CleanCd => Generate.CleanCd;
    public static bool SoloBoost => Generate.SoloBoost;
    public static bool UniqueJanitor => Generate.UniqueJanitor;
    public static bool JaniCooldownsLinked => Generate.JaniCooldownsLinked;
    public static JanitorOptions JanitorVentOptions => (JanitorOptions)Generate.JanitorVentOptions.GetInt();
    public static float DragModifier => Generate.DragModifier;
    public static float DragCd => Generate.DragCd;

    // Blackmailer Settings
    public static float BlackmailCd => Generate.BlackmailCd;
    public static bool UniqueBlackmailer => Generate.UniqueBlackmailer;
    public static bool WhispersNotPrivate => Generate.WhispersNotPrivate;
    public static bool BlackmailMates => Generate.BlackmailMates;
    public static bool BMRevealed => Generate.BMRevealed;

    // Grenadier Settings
    public static bool GrenadierIndicators => Generate.GrenadierIndicators;
    public static float FlashCd => Generate.FlashCd;
    public static float FlashDur => Generate.FlashDur;
    public static float FlashRadius => Generate.FlashRadius;
    public static bool GrenadierVent => Generate.GrenadierVent;
    public static bool UniqueGrenadier => Generate.UniqueGrenadier;
    public static bool SaboFlash => Generate.SaboFlash;

    // Camouflager Settings
    public static bool CamoHideSize => Generate.CamoHideSize;
    public static bool CamoHideSpeed => Generate.CamoHideSpeed;
    public static float CamouflagerCd => Generate.CamouflageCd;
    public static float CamouflageDur => Generate.CamouflageDur;
    public static bool UniqueCamouflager => Generate.UniqueCamouflager;

    // Morphling Settings
    public static bool MorphlingVent => Generate.MorphlingVent;
    public static float MorphCd => Generate.MorphCd;
    public static float SampleCd => Generate.SampleCd;
    public static float MorphDur => Generate.MorphDur;
    public static bool UniqueMorphling => Generate.UniqueMorphling;
    public static bool MorphCooldownsLinked => Generate.MorphCooldownsLinked;

    // Wraith Settings
    public static bool WraithVent => Generate.WraithVent;
    public static float InvisCd => Generate.InvisCd;
    public static float InvisDur => Generate.InvisDur;
    public static bool UniqueWraith => Generate.UniqueWraith;

    // Ambusher Settings
    public static float AmbushCd => Generate.AmbushCd;
    public static float AmbushDur => Generate.AmbushDur;
    public static bool UniqueAmbusher => Generate.UniqueAmbusher;
    public static bool AmbushMates => Generate.AmbushMates;

    // Enforcer Settings
    public static float EnforceCd => Generate.EnforceCd;
    public static float EnforceDur => Generate.EnforceDur;
    public static float EnforceRadius => Generate.EnforceRadius;
    public static float EnforceDelay => Generate.EnforceDelay;
    public static bool UniqueEnforcer => Generate.UniqueEnforcer;

    // Teleporter Settings
    public static bool TeleVent => Generate.TeleVent;
    public static float TeleportCd => Generate.TeleportCd;
    public static float TeleMarkCd => Generate.TeleMarkCd;
    public static bool UniqueTeleporter => Generate.UniqueTeleporter;
    public static bool TeleCooldownsLinked => Generate.TeleCooldownsLinked;

    // Consigliere Settings
    public static ConsigInfo ConsigInfo => (ConsigInfo)Generate.ConsigInfo.GetInt();
    public static float InvestigateCd => Generate.InvestigateCd;
    public static bool UniqueConsigliere => Generate.UniqueConsigliere;

    // Consort Settings
    public static float ConsortCd => Generate.ConsortCd;
    public static bool UniqueConsort => Generate.UniqueConsort;
    public static float ConsortDur => Generate.ConsortDur;

    // Disguiser Settings
    public static float DisguiseDur => Generate.DisguiseDur;
    public static float DisguiseCd => Generate.DisguiseCd;
    public static float DisguiseDelay => Generate.DisguiseDelay;
    public static DisguiserTargets DisguiseTarget => (DisguiserTargets)Generate.DisguiseTarget.GetInt();
    public static bool UniqueDisguiser => Generate.UniqueDisguiser;
    public static float MeasureCd => Generate.MeasureCd;
    public static bool DisgCooldownsLinked => Generate.DisgCooldownsLinked;

    // Godfather Settings
    public static bool UniqueGodfather => Generate.UniqueGodfather;
    public static float GFPromotionCdDecrease => Generate.GFPromotionCdDecrease;

    // Miner Settings
    public static float MineCd => Generate.MineCd;
    public static bool UniqueMiner => Generate.UniqueMiner;
    public static bool MinerSpawnOnMira => Generate.MinerSpawnOnMira;

    // Framer Settings
    public static float FrameCd => Generate.FrameCd;
    public static float ChaosDriveFrameRadius => Generate.ChaosDriveFrameRadius;
    public static bool UniqueFramer => Generate.UniqueFramer;

    // Spellslinger Settings
    public static float SpellCd => Generate.SpellCd;
    public static float SpellCdIncrease => Generate.SpellCdIncrease;
    public static bool UniqueSpellslinger => Generate.UniqueSpellslinger;

    // Collider Settings
    public static float CollideCd => Generate.CollideCd;
    public static float ChargeCd => Generate.ChargeCd;
    public static float ChargeDur => Generate.ChargeDur;
    public static float CollideRange => Generate.CollideRange;
    public static float CollideRangeIncrease => Generate.CollideRangeIncrease;
    public static bool UniqueCollider => Generate.UniqueCollider;
    public static bool CollideResetsCooldown => Generate.CollideResetsCooldown;
    public static bool ChargeCooldownsLinked => Generate.ChargeCooldownsLinked;

    // Shapeshifter Settings
    public static float ShapeshiftCd => Generate.ShapeshiftCd;
    public static float ShapeshiftDur => Generate.ShapeshiftDur;
    public static bool UniqueShapeshifter => Generate.UniqueShapeshifter;
    public static bool ShapeshiftMates => Generate.ShapeshiftMates;

    // Drunkard Settings
    public static float ConfuseCd => Generate.ConfuseCd;
    public static float ConfuseDur => Generate.ConfuseDur;
    public static bool UniqueDrunkard => Generate.UniqueDrunkard;
    public static bool ConfuseImmunity => Generate.ConfuseImmunity;

    // Timekeeper Settings
    public static float TimeCd => Generate.TimeCd;
    public static float TimeDur => Generate.TimeDur;
    public static bool UniqueTimekeeper => Generate.UniqueTimekeeper;
    public static bool TimeFreezeImmunity => Generate.TimeFreezeImmunity;
    public static bool TimeRewindImmunity => Generate.TimeRewindImmunity;

    // Crusader Settings
    public static float CrusadeCd => Generate.CrusadeCd;
    public static float CrusadeDur => Generate.CrusadeDur;
    public static bool UniqueCrusader => Generate.UniqueCrusader;
    public static float ChaosDriveCrusadeRadius => Generate.ChaosDriveCrusadeRadius;
    public static bool CrusadeMates => Generate.CrusadeMates;

    // Banshee Settings
    public static float ScreamCd => Generate.ScreamCd;
    public static float ScreamDur => Generate.ScreamDur;

    // Bomber Settings
    public static float BombCd => Generate.BombCd;
    public static float DetonateCd => Generate.DetonateCd;
    public static float BombRange => Generate.BombRange;
    public static bool UniqueBomber => Generate.UniqueBomber;
    public static bool BombCooldownsLinked => Generate.BombCooldownsLinked;
    public static bool BombsDetonateOnMeetingStart => Generate.BombsDetonateOnMeetingStart;
    public static bool BombsRemoveOnNewRound => Generate.BombsRemoveOnNewRound;
    public static float ChaosDriveBombRange => Generate.ChaosDriveBombRange;

    // Concealer Settings
    public static float ConcealCd => Generate.ConcealCd;
    public static float ConcealDur => Generate.ConcealDur;
    public static bool UniqueConcealer => Generate.UniqueConcealer;
    public static bool ConcealMates => Generate.ConcealMates;

    // Silencer Settings
    public static float SilenceCd => Generate.SilenceCd;
    public static bool UniqueSilencer => Generate.UniqueSilencer;
    public static bool WhispersNotPrivateSilencer => Generate.WhispersNotPrivateSilencer;
    public static bool SilenceMates => Generate.SilenceMates;
    public static bool SilenceRevealed => Generate.SilenceRevealed;

    // Stalker Settings
    public static bool UniqueStalker => Generate.UniqueStalker;
    public static float StalkCd => Generate.StalkCd;

    // Poisoner Settings
    public static float PoisonCd => Generate.PoisonCd;
    public static float PoisonDur => Generate.PoisonDur;
    public static bool UniquePoisoner => Generate.UniquePoisoner;

    // Rebel Settings
    public static bool UniqueRebel => Generate.UniqueRebel;
    public static float RebPromotionCdDecrease => Generate.RebPromotionCdDecrease;

    // Warper Settings
    public static float WarpCd => Generate.WarpCd;
    public static bool UniqueWarper => Generate.UniqueWarper;
    public static bool WarpSelf => Generate.WarpSelf;
    public static float WarpDur => Generate.WarpDur;

    // Modifier Settings
    public static int MaxModifiers => Generate.MaxModifiers;
    public static int MinModifiers => Generate.MinModifiers;

    // Objectifier Settings
    public static int MaxObjectifiers => Generate.MaxObjectifiers;
    public static int MinObjectifiers => Generate.MinObjectifiers;

    // Ability Settings
    public static int MaxAbilities => Generate.MaxAbilities;
    public static int MinAbilities => Generate.MinAbilities;

    // Snitch Settings
    public static bool SnitchSeesNeutrals => Generate.SnitchSeesNeutrals;
    public static bool SnitchSeesCrew => Generate.SnitchSeesCrew;
    public static bool SnitchSeesRoles => Generate.SnitchSeesRoles;
    public static bool SnitchSeestargetsInMeeting => Generate.SnitchSeestargetsInMeeting;
    public static bool SnitchSeesTraitor => Generate.SnitchSeesTraitor;
    public static bool SnitchSeesFanatic => Generate.SnitchSeesFanatic;
    public static bool SnitchKnows => Generate.SnitchKnows;
    public static int SnitchTasksRemaining => Generate.SnitchTasksRemaining;
    public static bool UniqueSnitch => Generate.UniqueSnitch;

    // Assassin Settings
    public static int AssassinKills => Generate.AssassinKills;
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

    // Underdog Settings
    public static bool UnderdogIncreasedKC => Generate.UnderdogIncreasedKC;
    public static float UnderdogKillBonus => Generate.UnderdogKillBonus;
    public static bool UniqueUnderdog => Generate.UniqueUnderdog;
    public static bool UnderdogKnows => Generate.UnderdogKnows;

    // Multitasker Settings
    public static float Transparancy => Generate.Transparancy;
    public static bool UniqueMultitasker => Generate.UniqueMultitasker;

    // BB Settings
    public static float ButtonCooldown => Generate.ButtonCooldown;
    public static bool UniqueButtonBarry => Generate.UniqueButtonBarry;

    // Swapper Settings
    public static bool SwapperButton => Generate.SwapperButton;
    public static bool SwapAfterVoting => Generate.SwapAfterVoting;
    public static bool SwapSelf => Generate.SwapSelf;
    public static bool UniqueSwapper => Generate.UniqueSwapper;

    // Politician Settings
    public static int PoliticianVoteBank => Generate.PoliticianVoteBank;
    public static bool UniquePolitician => Generate.UniquePolitician;
    public static bool PoliticianButton => Generate.PoliticianButton;

    // Tiebreaker Settings
    public static bool TiebreakerKnows => Generate.TiebreakerKnows;
    public static bool UniqueTiebreaker => Generate.UniqueTiebreaker;

    // Torch Settings
    public static bool UniqueTorch => Generate.UniqueTorch;

    // Tunneler Settings
    public static bool TunnelerKnows => Generate.TunnelerKnows;
    public static bool UniqueTunneler => Generate.UniqueTunneler;

    // Radar Settings
    public static bool UniqueRadar => Generate.UniqueRadar;

    // Ninja Settings
    public static bool UniqueNinja => Generate.UniqueNinja;

    // Ruthless Settings
    public static bool UniqueRuthless => Generate.UniqueRuthless;
    public static bool RuthlessKnows => Generate.RuthlessKnows;

    // Insider Settings
    public static bool InsiderKnows => Generate.InsiderKnows;
    public static bool UniqueInsider => Generate.UniqueInsider;

    // Traitor Settings
    public static bool TraitorColourSwap => Generate.TraitorColourSwap;
    public static bool TraitorKnows => Generate.TraitorKnows;
    public static bool UniqueTraitor => Generate.UniqueTraitor;
    public static bool RevealerRevealsTraitor => Generate.RevealerRevealsTraitor;

    // Fanatic Settings
    public static bool FanaticKnows => Generate.FanaticKnows;
    public static bool UniqueFanatic => Generate.UniqueFanatic;
    public static bool FanaticColourSwap => Generate.FanaticColourSwap;
    public static bool RevealerRevealsFanatic => Generate.RevealerRevealsFanatic;

    // Taskmaster Settings
    public static int TMTasksRemaining => Generate.TMTasksRemaining;
    public static bool UniqueTaskmaster => Generate.UniqueTaskmaster;

    // Lovers Settings
    public static bool BothLoversDie => Generate.BothLoversDie;
    public static bool UniqueLovers => Generate.UniqueLovers;
    public static bool LoversChat => Generate.LoversChat;
    public static bool LoversRoles => Generate.LoversRoles;

    // Linked Settings
    public static bool UniqueLinked => Generate.UniqueLinked;
    public static bool LinkedChat => Generate.LinkedChat;
    public static bool LinkedRoles => Generate.LinkedRoles;

    // Defector Settings
    public static bool DefectorKnows => Generate.DefectorKnows;
    public static bool UniqueDefector => Generate.UniqueDefector;
    public static DefectorFaction DefectorFaction => (DefectorFaction)Generate.DefectorFaction.GetInt();

    // Rivals Settings
    public static bool RivalsChat => Generate.RivalsChat;
    public static bool RivalsRoles => Generate.RivalsRoles;
    public static bool UniqueRivals => Generate.UniqueRivals;

    // Mafia Settings
    public static bool MafiaRoles => Generate.MafiaRoles;
    public static bool UniqueMafia => Generate.UniqueMafia;
    public static bool MafVent => Generate.MafVent;

    // Giant Settings
    public static float GiantSpeed => Generate.GiantSpeed;
    public static float GiantScale => Generate.GiantScale;
    public static bool UniqueGiant => Generate.UniqueGiant;

    // Indomitable Settings
    public static bool UniqueIndomitable => Generate.UniqueIndomitable;
    public static bool IndomitableKnows => Generate.IndomitableKnows;

    // Overlord Settings
    public static bool UniqueOverlord => Generate.UniqueOverlord;
    public static int OverlordMeetingWinCount => Generate.OverlordMeetingWinCount;
    public static bool OverlordKnows => Generate.OverlordKnows;

    // Allied Settings
    public static bool UniqueAllied => Generate.UniqueAllied;
    public static AlliedFaction AlliedFaction => (AlliedFaction)Generate.AlliedFaction.GetInt();

    // Corrupted Settings
    public static bool UniqueCorrupted => Generate.UniqueCorrupted;
    public static float CorruptCd => Generate.CorruptCd;
    public static bool AllCorruptedWin => Generate.AllCorruptedWin;
    public static bool CorruptedVent => Generate.CorruptedVent;

    // Dwarf Settings
    public static float DwarfSpeed => Generate.DwarfSpeed;
    public static bool UniqueDwarf => Generate.UniqueDwarf;
    public static float DwarfScale => Generate.DwarfScale;

    // Drunk Settings
    public static bool DrunkControlsSwap => Generate.DrunkControlsSwap;
    public static bool UniqueDrunk => Generate.UniqueDrunk;
    public static bool DrunkKnows => Generate.DrunkKnows;
    public static float DrunkInterval => Generate.DrunkInterval;

    // Bait Settings
    public static bool BaitKnows => Generate.BaitKnows;
    public static float BaitMinDelay => Generate.BaitMinDelay;
    public static float BaitMaxDelay => Generate.BaitMaxDelay;
    public static bool UniqueBait => Generate.UniqueBait;

    // Diseased Settings
    public static bool DiseasedKnows => Generate.DiseasedKnows;
    public static float DiseasedMultiplier => Generate.DiseasedKillMultiplier;
    public static bool UniqueDiseased => Generate.UniqueDiseased;

    // Shy Settings
    public static bool UniqueShy => Generate.UniqueShy;

    // Colorblind Settings
    public static bool UniqueColorblind => Generate.UniqueColorblind;

    // Astral Settings
    public static bool UniqueAstral => Generate.UniqueAstral;

    // Yeller Settings
    public static bool UniqueYeller => Generate.UniqueYeller;

    // VIP Settings
    public static bool VIPKnows => Generate.VIPKnows;
    public static bool UniqueVIP => Generate.UniqueVIP;

    // Volatile Settings
    public static float VolatileInterval => Generate.VolatileInterval;
    public static bool UniqueVolatile => Generate.UniqueVolatile;
    public static bool VolatileKnows => Generate.VolatileKnows;

    // Professional Settings
    public static bool ProfessionalKnows => Generate.ProfessionalKnows;
    public static bool UniqueProfessional => Generate.UniqueProfessional;

    // Coward Settings
    public static bool UniqueCoward => Generate.UniqueCoward;

    // NB Settings
    public static int NBMax => Generate.NBMax;
    public static bool VigiKillsNB => Generate.VigiKillsNB;

    // NK Settings
    public static int NKMax => Generate.NKMax;
    public static bool NKHasImpVision => Generate.NKHasImpVision;
    public static bool NKsKnow => Generate.NKsKnow;

    // CSv Settings
    public static int CSvMax => Generate.CSvMax;

    // CA Settings
    public static int CAMax => Generate.CAMax;

    // CK Settings
    public static int CKMax => Generate.CKMax;

    // CS Settings
    public static int CSMax => Generate.CSMax;

    // CI Settings
    public static int CIMax => Generate.CIMax;

    // CrP Settings
    public static int CrPMax => Generate.CrPMax;

    // IC Settings
    public static int ICMax => Generate.ICMax;

    // ID Settings
    public static int IDMax => Generate.IDMax;

    // IS Settings
    public static int ISMax => Generate.ISMax;

    // IH Settings
    public static int IHMax => Generate.IHMax;

    // IK Settings
    public static int IKMax => Generate.IKMax;

    // SD Settings
    public static int SDMax => Generate.SDMax;

    // SyK Settings
    public static int SyKMax => Generate.SyKMax;

    // SSu Settings
    public static int SSuMax => Generate.SSuMax;

    // SP Settings
    public static int SPMax => Generate.SPMax;

    // NE Settings
    public static int NEMax => Generate.NEMax;
    public static bool NeutralEvilsEndGame => Generate.NeutralEvilsEndGame;

    // NN Settings
    public static int NNMax => Generate.NNMax;
    public static bool NNHasImpVision => Generate.NNHasImpVision;

    // NH Settings
    public static int NHMax => Generate.NHMax;
}