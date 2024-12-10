namespace TownOfUsReworked.Classes;

// Thanks to Town Of Host parts of this code
public static class GameStates
{
    public static bool IsCountDown() => GameStartManager.Instance?.startState == GameStartManager.StartingStates.Countdown;

    public static bool IsInGame() => !Lobby() && (AmongUsClient.Instance?.GameState == InnerNetClient.GameStates.Started || GameManager.Instance?.GameHasStarted == true ||
        AmongUsClient.Instance?.IsGameStarted == true);

    public static bool IsLobby() => AmongUsClient.Instance?.GameState == InnerNetClient.GameStates.Joined || Lobby();

    public static bool IsEnded() => AmongUsClient.Instance?.GameState == InnerNetClient.GameStates.Ended || AmongUsClient.Instance?.IsGameOver == true;

    public static bool IsHnS() => GameOptionsManager.Instance?.CurrentGameOptions?.GameMode == GameModes.HideNSeek;

    public static bool IsNormal() => GameOptionsManager.Instance?.CurrentGameOptions?.GameMode == GameModes.Normal;

    public static bool IsOnlineGame() => AmongUsClient.Instance?.NetworkMode == NetworkModes.OnlineGame;

    public static bool IsLocalGame() => AmongUsClient.Instance?.NetworkMode == NetworkModes.LocalGame;

    public static bool IsFreePlay() => AmongUsClient.Instance?.NetworkMode == NetworkModes.FreePlay;

    public static bool IsMeeting() => IsInGame() && Meeting();

    public static bool IsAA() => GameModeSettings.GameMode == GameMode.AllAny;

    public static bool IsCustom() => GameModeSettings.GameMode == GameMode.Custom;

    public static bool IsClassic() => GameModeSettings.GameMode == GameMode.Classic;

    public static bool IsKilling() => GameModeSettings.GameMode == GameMode.KillingOnly;

    public static bool IsVanilla() => GameModeSettings.GameMode == GameMode.Vanilla;

    public static bool IsRoleList() => GameModeSettings.GameMode == GameMode.RoleList;

    public static bool IsTaskRace() => GameModeSettings.GameMode == GameMode.TaskRace;

    public static bool IsCustomHnS() => GameModeSettings.GameMode == GameMode.HideAndSeek;

    public static bool NoLobby() => !(IsInGame() || IsLobby() || IsEnded() || IsMeeting());

    public static bool LastImp() => AllPlayers().Count(x => x.Is(Faction.Intruder) && !x.HasDied()) == 1;

    public static bool LastSyn() => AllPlayers().Count(x => x.Is(Faction.Syndicate) && !x.HasDied()) == 1;

    public static bool NoPlayers() => AllPlayers().Count < 1 || !CustomPlayer.Local || !CustomPlayer.LocalCustom.Data || NoLobby();

    public static bool LocalBlocked() => PlayerLayer.LocalLayers().Any(x => x.IsBlocked);

    public static bool LocalNotBlocked() => !LocalBlocked();

    public static bool BlockIsExposed() => CustomPlayer.Local.GetButtons().Any(x => x.BlockExposed) || Blocked.BlockExposed;

    public static bool DeadSeeEverything()
    {
        if (!CustomPlayer.LocalCustom.Dead || !GameModifiers.DeadSeeEverything || !Role.LocalRole.TrulyDead)
            return false;

        var otherFlag = false;

        if (CustomPlayer.Local.TryGetLayer<GuardianAngel>(out var ga))
            otherFlag = !ga.Failed && ga.TargetAlive && GuardianAngel.ProtectBeyondTheGrave && ga.GraveProtectButton.Usable();
        else if (CustomPlayer.Local.TryGetLayer<Jester>(out var jest))
            otherFlag = jest.CanHaunt;

        return !otherFlag;
    }

    public static bool CrewWins() => !AllPlayers().Any(x => !x.HasDied() && !x.CrewSided() && x.NotCrew());

    private static bool NotCrew(this PlayerControl player)
    {
        var role = player.GetRole();
        var faction = role.Faction;
        var alignment = role.Alignment;
        return faction is Faction.Intruder or Faction.Syndicate || alignment is Alignment.NeutralKill or Alignment.NeutralNeo or Alignment.NeutralHarb or Alignment.NeutralPros or
            Alignment.NeutralApoc || player.NotOnTheSameSide() || (faction == Faction.Neutral && NeutralSettings.NoSolo == NoSolo.AllNeutrals);
    }

    public static bool IntrudersWin() => !AllPlayers().Any(x => !x.HasDied() && !x.IntruderSided() && x.NotIntruder());

    private static bool NotIntruder(this PlayerControl player)
    {
        var role = player.GetRole();
        var faction = role.Faction;
        var alignment = role.Alignment;
        return faction is Faction.Crew or Faction.Syndicate || alignment is Alignment.NeutralKill or Alignment.NeutralNeo or Alignment.NeutralHarb or Alignment.NeutralPros or
            Alignment.NeutralApoc || player.NotOnTheSameSide() || (faction == Faction.Neutral && NeutralSettings.NoSolo == NoSolo.AllNeutrals);
    }

    public static bool SyndicateWins() => !AllPlayers().Any(x => !x.HasDied() && !x.SyndicateSided() && x.NotSyndicate());

    private static bool NotSyndicate(this PlayerControl player)
    {
        var role = player.GetRole();
        var faction = role.Faction;
        var alignment = role.Alignment;
        return faction is Faction.Crew or Faction.Intruder || alignment is Alignment.NeutralKill or Alignment.NeutralNeo or Alignment.NeutralHarb or Alignment.NeutralPros or
            Alignment.NeutralApoc || player.NotOnTheSameSide() || (faction == Faction.Neutral && NeutralSettings.NoSolo == NoSolo.AllNeutrals);
    }

    public static bool AllNeutralsWin() => !AllPlayers().Any(x => !x.HasDied() && (x.NotOnTheSameSide() || x.GetFaction() is Faction.Crew or Faction.Syndicate or Faction.Intruder)) &&
        NeutralSettings.NoSolo == NoSolo.AllNeutrals;

    public static bool AllNKsWin() => NeutralSettings.NoSolo == NoSolo.AllNKs && !AllPlayers().Any(x => !x.HasDied() && x.NotNK());

    private static bool NotNK(this PlayerControl player)
    {
        var role = player.GetRole();
        var faction = role.Faction;
        var alignment = role.Alignment;
        return faction is Faction.Crew or Faction.Intruder or Faction.Syndicate || alignment is Alignment.NeutralNeo or Alignment.NeutralHarb or Alignment.NeutralPros or
            Alignment.NeutralApoc || player.NotOnTheSameSide();
    }

    public static bool NoOneWins() => !AllPlayers().Any(x => !x.HasDied());

    public static bool CabalWin() => !AllPlayers().Any(x => !x.HasDied() && x.NotSubFaction(SubFaction.Cabal, LayerEnum.Jackal));

    public static bool UndeadWin() => !AllPlayers().Any(x => !x.HasDied() && x.NotSubFaction(SubFaction.Undead, LayerEnum.Dracula));

    public static bool SectWin() => !AllPlayers().Any(x => !x.HasDied() && x.NotSubFaction(SubFaction.Sect, LayerEnum.Whisperer));

    public static bool ReanimatedWin() => !AllPlayers().Any(x => !x.HasDied() && x.NotSubFaction(SubFaction.Reanimated, LayerEnum.Necromancer));

    private static bool NotSubFaction(this PlayerControl player, SubFaction sub, LayerEnum neo)
    {
        var role = player.GetRole();
        var faction = role.Faction;
        var alignment = role.Alignment;
        return (faction is Faction.Crew or Faction.Intruder or Faction.Syndicate || (alignment == Alignment.NeutralNeo && role.Type != neo) || alignment is Alignment.NeutralHarb or
            Alignment.NeutralPros or Alignment.NeutralApoc || player.NotOnTheSameSide() || (faction == Faction.Neutral && NeutralSettings.NoSolo == NoSolo.AllNeutrals)) && role.SubFaction !=
            sub;
    }

    public static bool SameNKWins(LayerEnum nk) => !AllPlayers().Any(x => !x.HasDied() && (x.Is(Faction.Intruder) || x.Is(Faction.Crew) || x.Is(Faction.Syndicate) || (!x.Is(nk) &&
        x.Is(Alignment.NeutralKill)) || x.Is(Alignment.NeutralNeo) || x.Is(Alignment.NeutralHarb) || x.Is(Alignment.NeutralApoc) || x.Is(Alignment.NeutralPros) || x.NotOnTheSameSide())) &&
        NeutralSettings.NoSolo == NoSolo.SameNKs;

    public static bool SoloNKWins(PlayerControl player) => !AllPlayers().Any(x => !x.HasDied() && (x.Is(Faction.Intruder) || x.Is(Faction.Crew) || (x.Is(Alignment.NeutralKill) &&
        x != player) || x.Is(Alignment.NeutralNeo) || x.Is(Alignment.NeutralHarb) || x.Is(Alignment.NeutralApoc) || x.Is(Faction.Syndicate) || x.Is(Alignment.NeutralPros) ||
        x.NotOnTheSameSide())) && NeutralSettings.NoSolo == NoSolo.Never;

    public static bool CorruptedWin(PlayerControl player) => !AllPlayers().Any(x => !x.HasDied() && !x.Is(LayerEnum.Corrupted) && ((x != player && !Corrupted.AllCorruptedWin) ||
        Corrupted.AllCorruptedWin));

    public static bool LoversWin(PlayerControl player) => AllPlayers().Count(x => !x.HasDied()) <= 3 && PlayerLayer.GetLayers<Lovers>().Any(x => x.LoversAlive && x == player);

    public static bool RivalsWin(PlayerControl player) => AllPlayers().Count(x => !x.HasDied()) <= 2 && PlayerLayer.GetLayers<Rivals>().Any(x => x.IsWinningRival && x == player);

    public static bool MafiaWin() => !AllPlayers().Any(x => !x.HasDied() && !x.Is(LayerEnum.Mafia));

    public static bool ApocWins() => !AllPlayers().Any(x => !x.HasDied() && (x.Is(Faction.Intruder) || x.Is(Faction.Syndicate) || x.Is(Faction.Crew) || x.Is(Alignment.NeutralKill) ||
        x.Is(Alignment.NeutralNeo) || x.Is(Alignment.NeutralPros) || x.NotOnTheSameSide() || (x.Is(Faction.Neutral) && NeutralSettings.NoSolo == NoSolo.AllNeutrals)));

    public static bool HunterWins() => !AllPlayers().Any(x => !x.HasDied() && x.Is(LayerEnum.Hunted));

    public static bool HuntedWin() => !AllPlayers().Any(x => !x.HasDied() && x.Is(LayerEnum.Hunter));

    public static bool DefectorWins() => !AllPlayers().Any(x => !x.HasDied() && !x.Is(LayerEnum.Defector) && !x.Is(Faction.Neutral));

    public static bool BetrayerWins() => !AllPlayers().Any(x => !x.HasDied() && !x.Is(LayerEnum.Betrayer) && !(x.Is(Alignment.NeutralBen) || x.Is(Alignment.NeutralEvil)));

    public static bool OverlordWins() => MeetingPatches.MeetingCount >= Overlord.OverlordMeetingWinCount;
}