namespace TownOfUsReworked.Data
{
    //Thanks to Town Of Host for this code
    [HarmonyPatch]
    public static class ConstantVariables
    {
        public static bool IsCountDown => GameStartManager.Instance && GameStartManager.Instance.startState == GameStartManager.StartingStates.Countdown;
        public static bool IsInGame => (AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started || GameManager.Instance?.GameHasStarted ==  true) && !LobbyBehaviour.Instance;
        public static bool IsLobby => AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Joined || LobbyBehaviour.Instance;
        public static bool IsEnded => AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Ended;
        public static bool IsHnS => GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek;
        public static bool IsNormal => GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.Normal;
        public static bool IsOnlineGame => AmongUsClient.Instance.NetworkMode == NetworkModes.OnlineGame;
        public static bool IsLocalGame => AmongUsClient.Instance.NetworkMode == NetworkModes.LocalGame;
        public static bool IsFreePlay => AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay;
        public static bool IsRoaming => IsInGame && !MeetingHud.Instance && !Minigame.Instance;
        public static bool IsMeeting => IsInGame && MeetingHud.Instance;
        public static bool IsAA => CustomGameOptions.GameMode == GameMode.AllAny;
        public static bool IsCustom => CustomGameOptions.GameMode == GameMode.Custom;
        public static bool IsClassic => CustomGameOptions.GameMode == GameMode.Classic;
        public static bool IsKilling => CustomGameOptions.GameMode == GameMode.KillingOnly;
        public static bool IsVanilla => CustomGameOptions.GameMode == GameMode.Vanilla;
        public static bool NoLobby => !(IsInGame || IsLobby || IsEnded || IsRoaming || IsMeeting);
        public static bool LastImp => PlayerControl.AllPlayerControls.Count(x => x.Is(Faction.Intruder) && !(x.Data.IsDead || x.Data.Disconnected)) == 1;
        public static bool LastSyn => PlayerControl.AllPlayerControls.Count(x => x.Is(Faction.Syndicate) && !(x.Data.IsDead || x.Data.Disconnected)) == 1;
        public static bool Inactive => PlayerControl.AllPlayerControls.Count <= 1 || PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null || NoLobby;
        public static bool NoPlayers => PlayerControl.AllPlayerControls.Count < 1 || PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null;
        public static bool DeadSeeEverything => PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything && Role.LocalRole.TrulyDead;

        public static bool CrewWins => !PlayerControl.AllPlayerControls.Any(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Intruder) || x.Is(Faction.Syndicate) ||
            x.Is(RoleAlignment.NeutralKill) || x.Is(RoleAlignment.NeutralNeo) || x.Is(RoleAlignment.NeutralPros) || x.NotOnTheSameSide()) && !x.CrewSided());

        public static bool IntrudersWin => !PlayerControl.AllPlayerControls.Any(x => !x.Data.IsDead && !x.Data.Disconnected && !x.IntruderSided() && (x.Is(Faction.Crew) ||
            x.Is(RoleAlignment.NeutralKill) || x.Is(Faction.Syndicate) || x.Is(RoleAlignment.NeutralNeo) || x.Is(RoleAlignment.NeutralPros) || x.NotOnTheSameSide()));

        public static bool SyndicateWins => !PlayerControl.AllPlayerControls.Any(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(RoleAlignment.NeutralKill) ||
            x.Is(Faction.Intruder) || x.Is(RoleAlignment.NeutralNeo) || x.Is(Faction.Crew) || x.Is(RoleAlignment.NeutralPros) || x.NotOnTheSameSide()) && !x.SyndicateSided());

        public static bool AllNeutralsWin => (!PlayerControl.AllPlayerControls.Any(x => !x.Data.IsDead && !x.Data.Disconnected && (x.NotOnTheSameSide() || x.Is(Faction.Crew) ||
            x.Is(Faction.Syndicate) || x.Is(Faction.Intruder)))) && CustomGameOptions.NoSolo == NoSolo.AllNeutrals;

        public static bool PestOrPBWins => !PlayerControl.AllPlayerControls.Any(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Intruder) || x.Is(Faction.Crew) ||
            (x.Is(RoleAlignment.NeutralKill) && !x.Is(RoleEnum.Plaguebearer)) || x.Is(RoleAlignment.NeutralNeo) || x.Is(Faction.Syndicate) || x.Is(RoleAlignment.NeutralPros) ||
            x.Is(ObjectifierEnum.Allied) || x.NotOnTheSameSide()));

        public static bool AllNKsWin => (!PlayerControl.AllPlayerControls.Any(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Intruder) || x.Is(RoleAlignment.NeutralNeo) ||
            x.Is(Faction.Syndicate) || x.Is(RoleAlignment.NeutralPros) || x.Is(Faction.Crew) || x.Is(ObjectifierEnum.Allied) || x.NotOnTheSameSide()))) && CustomGameOptions.NoSolo ==
            NoSolo.AllNKs;

        public static bool NoOneWins => !PlayerControl.AllPlayerControls.Any(x => !x.Data.IsDead && !x.Data.Disconnected);

        public static bool CabalWin => !PlayerControl.AllPlayerControls.Any(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Intruder) || x.Is(Faction.Crew) ||
            x.Is(RoleAlignment.NeutralKill) || (x.Is(RoleAlignment.NeutralNeo) && !x.Is(RoleEnum.Jackal)) || x.Is(Faction.Syndicate) || x.Is(RoleAlignment.NeutralPros) ||
            x.NotOnTheSameSide()) && !x.IsRecruit());

        public static bool UndeadWin => !PlayerControl.AllPlayerControls.Any(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Intruder) || x.Is(Faction.Crew) ||
            x.Is(RoleAlignment.NeutralKill) || (x.Is(RoleAlignment.NeutralNeo) && !x.Is(RoleEnum.Dracula)) || x.Is(Faction.Syndicate) || x.Is(RoleAlignment.NeutralPros) ||
            x.NotOnTheSameSide()) && !x.IsBitten());

        public static bool SectWin => !PlayerControl.AllPlayerControls.Any(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Intruder) || x.Is(Faction.Crew) ||
            x.Is(RoleAlignment.NeutralKill) || (x.Is(RoleAlignment.NeutralNeo) && !x.Is(RoleEnum.Whisperer)) || x.Is(Faction.Syndicate) || x.Is(RoleAlignment.NeutralPros) ||
            x.NotOnTheSameSide()) && !x.IsPersuaded());

        public static bool ReanimatedWin => !PlayerControl.AllPlayerControls.Any(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Intruder) || x.Is(Faction.Crew) ||
            x.Is(RoleAlignment.NeutralKill) || (x.Is(RoleAlignment.NeutralNeo) && !x.Is(RoleEnum.Necromancer)) || x.Is(Faction.Syndicate) || x.Is(RoleAlignment.NeutralPros) ||
            x.NotOnTheSameSide()) && !x.IsResurrected());

        public static bool GameHasEnded => Role.RoleWins || Objectifier.ObjectifierWins || PlayerLayer.NobodyWins;

        public static bool SameNKWins(RoleEnum nk) => !PlayerControl.AllPlayerControls.Any(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Intruder) || x.Is(Faction.Crew) ||
            (x.Is(RoleAlignment.NeutralKill) && !x.Is(nk)) || x.Is(RoleAlignment.NeutralNeo) || x.Is(Faction.Syndicate) || x.Is(RoleAlignment.NeutralPros) || x.NotOnTheSameSide())) &&
            CustomGameOptions.NoSolo == NoSolo.SameNKs;

        public static bool SoloNKWins(PlayerControl player) => !PlayerControl.AllPlayerControls.Any(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Intruder) ||
            (x.Is(RoleAlignment.NeutralKill) && x != player) || x.Is(RoleAlignment.NeutralNeo) || x.Is(Faction.Syndicate) || x.Is(RoleAlignment.NeutralPros) || x.Is(Faction.Crew) ||
            x.NotOnTheSameSide())) && CustomGameOptions.NoSolo == NoSolo.Never;

        public static bool CorruptedWin(PlayerControl player) => !PlayerControl.AllPlayerControls.Any(x => !x.Data.IsDead && !x.Data.Disconnected && !x.Is(ObjectifierEnum.Corrupted) &&
            ((x != player && !CustomGameOptions.AllCorruptedWin) || CustomGameOptions.AllCorruptedWin));

        public static bool LoversWin(PlayerControl player) => PlayerControl.AllPlayerControls.Count(x => !x.Data.IsDead && !x.Data.Disconnected) <= 3 &&
            Objectifier.GetObjectifiers<Lovers>(ObjectifierEnum.Lovers).Any(x => x.LoversAlive && x == player);

        public static bool RivalsWin(PlayerControl player) => PlayerControl.AllPlayerControls.Count(x => !x.Data.IsDead && !x.Data.Disconnected) <= 2 &&
            Objectifier.GetObjectifiers<Rivals>(ObjectifierEnum.Rivals).Any(x => x.IsWinningRival && x == player);

        public static bool MafiaWin => !PlayerControl.AllPlayerControls.Any(x => !x.Data.IsDead && !x.Data.Disconnected && !x.Is(ObjectifierEnum.Mafia));
    }
}