namespace TownOfUsReworked.Data;

//Thanks to Town Of Host parts of this code
public static class ConstantVariables
{
    public static bool IsCountDown => GameStartManager.Instance?.startState == GameStartManager.StartingStates.Countdown;
    public static bool IsInGame => (AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started || GameManager.Instance?.GameHasStarted == true ||
        AmongUsClient.Instance.IsGameStarted) && !LobbyBehaviour.Instance;
    public static bool IsLobby => AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Joined || LobbyBehaviour.Instance;
    public static bool IsEnded => AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Ended || AmongUsClient.Instance.IsGameOver;
    public static bool IsHnS => GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek;
    public static bool IsNormal => GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.Normal;
    public static bool IsOnlineGame => AmongUsClient.Instance.NetworkMode == NetworkModes.OnlineGame;
    public static bool IsLocalGame => AmongUsClient.Instance.NetworkMode == NetworkModes.LocalGame;
    public static bool IsFreePlay => AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay;
    public static bool IsRoaming => IsInGame && !Meeting && !ActiveTask;
    public static bool IsMeeting => IsInGame && Meeting;
    public static bool IsAA => CustomGameOptions.GameMode == GameMode.AllAny;
    public static bool IsCustom => CustomGameOptions.GameMode == GameMode.Custom;
    public static bool IsClassic => CustomGameOptions.GameMode == GameMode.Classic;
    public static bool IsKilling => CustomGameOptions.GameMode == GameMode.KillingOnly;
    public static bool IsVanilla => CustomGameOptions.GameMode == GameMode.Vanilla;
    public static bool IsRoleList => CustomGameOptions.GameMode == GameMode.RoleList;
    public static bool IsTaskRace => CustomGameOptions.GameMode == GameMode.TaskRace;
    public static bool IsCustomHnS => CustomGameOptions.GameMode == GameMode.HideAndSeek;
    public static bool NoLobby => !(IsInGame || IsLobby || IsEnded || IsRoaming || IsMeeting);
    public static bool LastImp => CustomPlayer.AllPlayers.Count(x => x.Is(Faction.Intruder) && !x.HasDied()) == 1;
    public static bool LastSyn => CustomPlayer.AllPlayers.Count(x => x.Is(Faction.Syndicate) && !x.HasDied()) == 1;
    public static bool NoPlayers => CustomPlayer.AllPlayers.Count < 1 || CustomPlayer.Local == null || CustomPlayer.LocalCustom.Data == null || NoLobby;
    public static bool LocalBlocked => PlayerLayer.LocalLayers.Any(x => x.IsBlocked);
    public static bool LocalNotBlocked => !LocalBlocked;
    public static bool DeadSeeEverything
    {
        get
        {
            var flag = CustomPlayer.LocalCustom.IsDead && CustomGameOptions.DeadSeeEverything && Role.LocalRole.TrulyDead;

            if (!flag)
                return false;

            var otherFlag = false;

            if (CustomPlayer.Local.Is(LayerEnum.GuardianAngel))
            {
                var ga = (GuardianAngel)Role.LocalRole;
                otherFlag = !ga.Failed && ga.TargetPlayer != null && ga.TargetAlive && CustomGameOptions.ProtectBeyondTheGrave && ga.GraveProtectButton.Usable;
            }
            else if (CustomPlayer.Local.Is(LayerEnum.Jester))
            {
                var jest = (Jester)Role.LocalRole;
                otherFlag = jest.CanHaunt;
            }

            return flag && !otherFlag;
        }
    }

    public static bool CrewWins => !CustomPlayer.AllPlayers.Any(x => !x.HasDied() && !x.CrewSided() && (x.Is(Faction.Intruder) || x.Is(Faction.Syndicate) || x.Is(Alignment.NeutralKill)
        || x.Is(Alignment.NeutralNeo) || x.Is(Alignment.NeutralHarb) || x.Is(Alignment.NeutralPros) || x.NotOnTheSameSide() || (x.Is(Faction.Neutral) && CustomGameOptions.NoSolo
        == NoSolo.AllNeutrals) || x.Is(Alignment.NeutralApoc)));

    public static bool IntrudersWin => !CustomPlayer.AllPlayers.Any(x => !x.HasDied() && !x.IntruderSided() && (x.Is(Faction.Crew) || x.Is(Alignment.NeutralKill) ||
        x.Is(Alignment.NeutralHarb) || x.Is(Faction.Syndicate) || x.Is(Alignment.NeutralNeo) || x.Is(Alignment.NeutralPros) || x.NotOnTheSameSide() || (x.Is(Faction.Neutral) &&
        CustomGameOptions.NoSolo == NoSolo.AllNeutrals) || x.Is(Alignment.NeutralApoc)));

    public static bool SyndicateWins => !CustomPlayer.AllPlayers.Any(x => !x.HasDied() && (x.Is(Alignment.NeutralKill) || x.Is(Faction.Intruder) || x.Is(Alignment.NeutralNeo) ||
        x.Is(Alignment.NeutralHarb) || x.Is(Faction.Crew) || x.Is(Alignment.NeutralPros) || x.NotOnTheSameSide() || (x.Is(Faction.Neutral) && CustomGameOptions.NoSolo ==
        NoSolo.AllNeutrals) || x.Is(Alignment.NeutralApoc)) && !x.SyndicateSided());

    public static bool AllNeutralsWin => (!CustomPlayer.AllPlayers.Any(x => !x.HasDied() && (x.NotOnTheSameSide() || x.Is(Faction.Crew) || x.Is(Faction.Syndicate) || x.Is(Faction.Intruder))))
        && CustomGameOptions.NoSolo == NoSolo.AllNeutrals;

    public static bool AllNKsWin => (!CustomPlayer.AllPlayers.Any(x => !x.HasDied() && (x.Is(Faction.Intruder) || x.Is(Alignment.NeutralNeo) || x.Is(Faction.Syndicate) ||
        x.Is(Alignment.NeutralHarb) || x.Is(Alignment.NeutralPros) || x.Is(Faction.Crew) || x.Is(LayerEnum.Allied) || x.Is(Alignment.NeutralApoc) || x.NotOnTheSameSide()))) &&
        CustomGameOptions.NoSolo == NoSolo.AllNKs;

    public static bool NoOneWins => !CustomPlayer.AllPlayers.Any(x => !x.HasDied());

    public static bool CabalWin => !CustomPlayer.AllPlayers.Any(x => !x.HasDied() && !x.IsRecruit() && (x.Is(Faction.Intruder) || x.Is(Faction.Crew) || x.Is(Alignment.NeutralKill) ||
        (x.Is(Alignment.NeutralNeo) && !x.Is(LayerEnum.Jackal)) || x.Is(Faction.Syndicate) || x.Is(Alignment.NeutralPros) || x.NotOnTheSameSide() || x.Is(Alignment.NeutralHarb) ||
        x.Is(Alignment.NeutralApoc) || (x.Is(Faction.Neutral) && CustomGameOptions.NoSolo == NoSolo.AllNeutrals)));

    public static bool UndeadWin => !CustomPlayer.AllPlayers.Any(x => !x.HasDied() && !x.IsBitten() && (x.Is(Faction.Intruder) || x.Is(Faction.Crew) || x.Is(Alignment.NeutralKill) ||
        (x.Is(Alignment.NeutralNeo) && !x.Is(LayerEnum.Dracula)) || x.Is(Faction.Syndicate) || x.Is(Alignment.NeutralPros) || x.NotOnTheSameSide() || x.Is(Alignment.NeutralHarb) ||
        x.Is(Alignment.NeutralApoc) || (x.Is(Faction.Neutral) && CustomGameOptions.NoSolo == NoSolo.AllNeutrals)));

    public static bool SectWin => !CustomPlayer.AllPlayers.Any(x => !x.HasDied() && !x.IsPersuaded() && (x.Is(Faction.Intruder) || x.Is(Faction.Crew) || x.Is(Alignment.NeutralKill) ||
        (x.Is(Alignment.NeutralNeo) && !x.Is(LayerEnum.Whisperer)) || x.Is(Faction.Syndicate) || x.Is(Alignment.NeutralPros) || x.NotOnTheSameSide() || x.Is(Alignment.NeutralHarb) ||
        x.Is(Alignment.NeutralApoc) || (x.Is(Faction.Neutral) && CustomGameOptions.NoSolo == NoSolo.AllNeutrals)));

    public static bool ReanimatedWin => !CustomPlayer.AllPlayers.Any(x => !x.HasDied() && (x.Is(Faction.Intruder) || x.Is(Faction.Crew) || x.Is(Faction.Syndicate) || x.NotOnTheSameSide() ||
        x.Is(Alignment.NeutralKill) || (x.Is(Alignment.NeutralNeo) && !x.Is(LayerEnum.Necromancer)) || x.Is(Alignment.NeutralPros) || x.Is(Alignment.NeutralHarb) || (x.Is(Faction.Neutral) &&
        CustomGameOptions.NoSolo == NoSolo.AllNeutrals) || x.Is(Alignment.NeutralApoc)) && !x.IsResurrected());

    public static bool GameHasEnded => Role.RoleWins || Objectifier.ObjectifierWins || PlayerLayer.NobodyWins;

    public static bool SameNKWins(LayerEnum nk) => !CustomPlayer.AllPlayers.Any(x => !x.HasDied() && (x.Is(Faction.Intruder) || x.Is(Faction.Crew) || (!x.Is(nk) &&
        x.Is(Alignment.NeutralKill)) || x.Is(Alignment.NeutralNeo) || x.Is(Alignment.NeutralHarb) || x.Is(Alignment.NeutralApoc) || x.Is(Faction.Syndicate) || x.Is(Alignment.NeutralPros) ||
        x.NotOnTheSameSide())) && CustomGameOptions.NoSolo == NoSolo.SameNKs;

    public static bool SoloNKWins(PlayerControl player) => !CustomPlayer.AllPlayers.Any(x => !x.HasDied() && (x.Is(Faction.Intruder) || x.Is(Faction.Crew) || (x.Is(Alignment.NeutralKill) &&
        x != player) || x.Is(Alignment.NeutralNeo) || x.Is(Alignment.NeutralHarb) || x.Is(Alignment.NeutralApoc) || x.Is(Faction.Syndicate) || x.Is(Alignment.NeutralPros) ||
        x.NotOnTheSameSide())) && CustomGameOptions.NoSolo == NoSolo.Never;

    public static bool CorruptedWin(PlayerControl player) => !CustomPlayer.AllPlayers.Any(x => !x.HasDied() && !x.Is(LayerEnum.Corrupted) && ((x != player &&
        !CustomGameOptions.AllCorruptedWin) || CustomGameOptions.AllCorruptedWin));

    public static bool LoversWin(PlayerControl player) => CustomPlayer.AllPlayers.Count(x => !x.HasDied()) <= 3 && PlayerLayer.GetLayers<Lovers>().Any(x => x.LoversAlive && x == player);

    public static bool RivalsWin(PlayerControl player) => CustomPlayer.AllPlayers.Count(x => !x.HasDied()) <= 2 && PlayerLayer.GetLayers<Rivals>().Any(x => x.IsWinningRival && x == player);

    public static bool MafiaWin => !CustomPlayer.AllPlayers.Any(x => !x.HasDied() && !x.Is(LayerEnum.Mafia));

    public static bool ApocWins => !CustomPlayer.AllPlayers.Any(x => !x.HasDied() && (x.Is(Faction.Intruder) || x.Is(Faction.Syndicate) || x.Is(Faction.Crew) || x.Is(Alignment.NeutralKill)
        || x.Is(Alignment.NeutralNeo) || x.Is(Alignment.NeutralPros) || x.NotOnTheSameSide() || (x.Is(Faction.Neutral) && CustomGameOptions.NoSolo == NoSolo.AllNeutrals)));

    public static bool HunterWins => !CustomPlayer.AllPlayers.Any(x => !x.HasDied() && x.Is(LayerEnum.Hunted));

    public static bool HuntedWins => !CustomPlayer.AllPlayers.Any(x => !x.HasDied() && x.Is(LayerEnum.Hunter));
}