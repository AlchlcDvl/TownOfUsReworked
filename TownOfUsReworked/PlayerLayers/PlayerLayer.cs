namespace TownOfUsReworked.PlayerLayers;

public abstract class PlayerLayer
{
    public virtual Color Color => Colors.Layer;
    public virtual string Name => "None";
    public string Short => Info.AllInfo.Find(x => x.Name == Name)?.Short;
    public virtual PlayerLayerEnum LayerType => PlayerLayerEnum.None;
    public virtual LayerEnum Type => LayerEnum.None;
    public virtual Func<string> Description => () => "- None";
    public virtual Func<string> Attributes => () => "- None";

    public PlayerControl Player { get; set; }
    public bool IsBlocked { get; set; }
    public bool Winner { get; set; }
    public bool Ignore { get; set; }

    public bool IsDead => Data.IsDead;
    public bool Disconnected => Data.Disconnected;
    public bool IsAlive => !(IsDead || Disconnected);

    public GameData.PlayerInfo Data => Player.Data;
    public string PlayerName => Data.PlayerName;
    public bool Local => Player == CustomPlayer.Local;
    public byte PlayerId => Player.PlayerId;
    public int TasksLeft => Data.Tasks.Count(x => !x.Complete);
    public int TasksCompleted => Data.Tasks.Count(x => x.Complete);
    public int TotalTasks => Data.Tasks.Count;
    public bool TasksDone => Player != null && Player.CanDoTasks() && (TasksLeft <= 0 || TasksCompleted >= TotalTasks);

    public string ColorString => $"<color=#{Color.ToHtmlStringRGBA()}>";

    public static bool NobodyWins { get; set; }

    public static readonly List<PlayerLayer> AllLayers = new();
    public static List<PlayerLayer> LocalLayers => GetLayers(CustomPlayer.Local);

    public virtual void OnLobby() => Patches.EndGame.Reset();

    public virtual void UpdateHud(HudManager __instance) {}

    public virtual void UpdateMeeting(MeetingHud __instance) => ButtonUtils.DisableAllButtons();

    public virtual void VoteComplete(MeetingHud __instance) {}

    public virtual void ConfirmVotePrefix(MeetingHud __instance) {}

    public virtual void ConfirmVotePostfix(MeetingHud __instance) {}

    public virtual void ClearVote(MeetingHud __instance) {}

    public virtual void SelectVote(MeetingHud __instance, int id) {}

    public virtual void UpdateMap(MapBehaviour __instance) {}

    public virtual void TryEndEffect() {}

    public virtual void ExitingLayer() {}

    public virtual void EnteringLayer() {}

    public virtual void OnMeetingStart(MeetingHud __instance)
    {
        Player.DisableButtons();
        Patches.EndGame.Reset();
        Ash.DestroyAll();
    }

    public virtual void OnMeetingEnd(MeetingHud __instance)
    {
        Player.EnableButtons();
        Patches.EndGame.Reset();
        ButtonUtils.ResetCustomTimers(CooldownType.Meeting);
    }

    public virtual void OnBodyReport(GameData.PlayerInfo info) => Patches.EndGame.Reset();

    public virtual void UponTaskComplete(PlayerControl player, uint taskId) {}

    public virtual void ReadRPC(MessageReader reader) {}

    protected PlayerLayer(PlayerControl player)
    {
        Player = player;
        AllLayers.Add(this);
    }

    public void GameEnd()
    {
        if (Player == null || Disconnected)
            return;
        else if (IsDead)
        {
            if (Type == LayerEnum.Phantom && TasksDone && ((Role)this).Faithful)
            {
                Role.PhantomWins = true;
                ((Phantom)this).CompletedTasks = true;
                CallRpc(CustomRPC.WinLose, WinLoseRPC.PhantomWin, this);
                EndGame();
            }
            else if (LayerType == PlayerLayerEnum.Role && ((Role)this).Alignment == Alignment.NeutralEvil && CustomGameOptions.NeutralEvilsEndGame)
            {
                if (Type == LayerEnum.Jester && ((Jester)this).VotedOut)
                {
                    Role.JesterWins = true;
                    Winner = true;
                    EndGame();
                }
                else if (Type == LayerEnum.Executioner && ((Executioner)this).TargetVotedOut)
                {
                    Role.ExecutionerWins = true;
                    Winner = true;
                    EndGame();
                }
                else if (Type == LayerEnum.Actor && ((Actor)this).Guessed)
                {
                    Role.ActorWins = true;
                    Winner = true;
                    EndGame();
                }
                else if (Type == LayerEnum.Troll && ((Troll)this).Killed)
                {
                    Role.TrollWins = true;
                    Winner = true;
                    EndGame();
                }
            }

            return;
        }
        else if (LayerType == PlayerLayerEnum.Objectifier)
        {
            if (Type == LayerEnum.Corrupted && CorruptedWin(Player))
            {
                Objectifier.CorruptedWins = true;

                if (CustomGameOptions.AllCorruptedWin)
                    Objectifier.GetObjectifiers(LayerEnum.Corrupted).ForEach(x => x.Winner = true);

                Winner = true;
                CallRpc(CustomRPC.WinLose, WinLoseRPC.CorruptedWin, this);
                EndGame();
            }
            else if (Type == LayerEnum.Lovers && LoversWin(Player))
            {
                Objectifier.LoveWins = true;
                Winner = true;
                Objectifier.GetObjectifier(((Lovers)this).OtherLover).Winner = true;
                CallRpc(CustomRPC.WinLose, WinLoseRPC.LoveWin, this);
                EndGame();
            }
            else if (Type == LayerEnum.Rivals && RivalsWin(Player))
            {
                Objectifier.RivalWins = true;
                Winner = true;
                CallRpc(CustomRPC.WinLose, WinLoseRPC.RivalWin, this);
                EndGame();
            }
            else if (Type == LayerEnum.Taskmaster && TasksDone)
            {
                Objectifier.TaskmasterWins = true;
                Winner = true;
                CallRpc(CustomRPC.WinLose, WinLoseRPC.TaskmasterWin, this);
                EndGame();
            }
            else if (Type == LayerEnum.Mafia && MafiaWin)
            {
                Objectifier.MafiaWins = true;
                CallRpc(CustomRPC.WinLose, WinLoseRPC.MafiaWins);
                EndGame();
            }
            else if (Type == LayerEnum.Overlord && MeetingPatches.MeetingCount >= CustomGameOptions.OverlordMeetingWinCount && IsAlive)
            {
                Objectifier.OverlordWins = true;
                Objectifier.GetObjectifiers(LayerEnum.Overlord).Where(ov => ov.IsAlive).ForEach(x => x.Winner = true);
                CallRpc(CustomRPC.WinLose, WinLoseRPC.OverlordWin);
                EndGame();
            }
        }
        else if (LayerType == PlayerLayerEnum.Role)
        {
            var role = (Role)this;

            if ((role.IsRecruit || Type == LayerEnum.Jackal) && CabalWin)
            {
                Role.CabalWin = true;
                CallRpc(CustomRPC.WinLose, WinLoseRPC.CabalWin, this);
                EndGame();
            }
            else if ((role.IsPersuaded || Type == LayerEnum.Whisperer) && SectWin)
            {
                Role.SectWin = true;
                CallRpc(CustomRPC.WinLose, WinLoseRPC.SectWin);
                EndGame();
            }
            else if ((role.IsBitten || Type == LayerEnum.Dracula) && UndeadWin)
            {
                Role.UndeadWin = true;
                CallRpc(CustomRPC.WinLose, WinLoseRPC.UndeadWin);
                EndGame();
            }
            else if ((role.IsResurrected || Type == LayerEnum.Necromancer) && ReanimatedWin)
            {
                Role.ReanimatedWin = true;
                CallRpc(CustomRPC.WinLose, WinLoseRPC.ReanimatedWin);
                EndGame();
            }
            else if (role.Faction == Faction.Syndicate && (role.Faithful || Type == LayerEnum.Betrayer || role.IsSynAlly || role.IsSynDefect || role.IsSynFanatic || role.IsSynTraitor) &&
                SyndicateWins)
            {
                Role.SyndicateWin = true;
                CallRpc(CustomRPC.WinLose, WinLoseRPC.SyndicateWin);
                EndGame();
            }
            else if (role.Faction == Faction.Intruder && (role.Faithful || Type == LayerEnum.Betrayer || role.IsIntDefect || role.IsIntAlly || role.IsIntFanatic || role.IsIntTraitor) &&
                IntrudersWin)
            {
                Role.IntruderWin = true;
                CallRpc(CustomRPC.WinLose, WinLoseRPC.IntruderWin);
                EndGame();
            }
            else if (role.Faction == Faction.Crew && (role.Faithful || role.IsCrewAlly || role.IsCrewDefect) && CrewWins)
            {
                Role.CrewWin = true;
                CallRpc(CustomRPC.WinLose, WinLoseRPC.CrewWin);
                EndGame();
            }
            else if (role.Faithful && ApocWins && role.Alignment is Alignment.NeutralApoc or Alignment.NeutralHarb)
            {
                Role.ApocalypseWins = true;
                CallRpc(CustomRPC.WinLose, WinLoseRPC.ApocalypseWins);
                EndGame();
            }
            else if (AllNeutralsWin && role.Faithful)
            {
                Role.AllNeutralsWin = true;
                CallRpc(CustomRPC.WinLose, WinLoseRPC.AllNeutralsWin);
                EndGame();
            }
            else if (AllNKsWin && role.Faithful && role.Alignment == Alignment.NeutralKill)
            {
                Role.NKWins = true;
                CallRpc(CustomRPC.WinLose, WinLoseRPC.AllNKsWin);
                EndGame();
            }
            else if (role.Faithful && role.Alignment == Alignment.NeutralKill && (SameNKWins(role.Type) || SoloNKWins(Player)))
            {
                switch (role.Type)
                {
                    case LayerEnum.Glitch:
                        Role.GlitchWins = true;
                        break;

                    case LayerEnum.Arsonist:
                        Role.ArsonistWins = true;
                        break;

                    case LayerEnum.Cryomaniac:
                        Role.CryomaniacWins = true;
                        break;

                    case LayerEnum.Juggernaut:
                        Role.JuggernautWins = true;
                        break;

                    case LayerEnum.Murderer:
                        Role.MurdererWins = true;
                        break;

                    case LayerEnum.Werewolf:
                        Role.WerewolfWins = true;
                        break;

                    case LayerEnum.SerialKiller:
                        Role.SerialKillerWins = true;
                        break;
                }

                if (CustomGameOptions.NoSolo == NoSolo.SameNKs)
                {
                    foreach (var role2 in Role.GetRoles(role.Type))
                    {
                        if (!role2.Disconnected && role2.Faithful)
                            role2.Winner = true;
                    }
                }

                Winner = true;
                CallRpc(CustomRPC.WinLose, CustomGameOptions.NoSolo == NoSolo.SameNKs ? WinLoseRPC.SameNKWins : WinLoseRPC.SoloNKWins, this);
                EndGame();
            }
            else if (CustomGameOptions.NeutralEvilsEndGame && role.Alignment == Alignment.NeutralEvil)
            {
                if (Type == LayerEnum.Executioner && ((Executioner)role).TargetVotedOut)
                {
                    Role.ExecutionerWins = true;
                    Winner = true;
                    EndGame();
                }
                else if (Type == LayerEnum.BountyHunter && ((BountyHunter)role).TargetKilled)
                {
                    Role.BountyHunterWins = true;
                    Winner = true;
                    EndGame();
                }
                else if (Type == LayerEnum.Cannibal && ((Cannibal)role).Eaten)
                {
                    Role.CannibalWins = true;
                    Winner = true;
                    EndGame();
                }
                else if (Type == LayerEnum.Guesser && ((Guesser)role).TargetGuessed)
                {
                    Role.GuesserWins = true;
                    Winner = true;
                    EndGame();
                }
            }
        }
    }

    public static bool operator ==(PlayerLayer a, PlayerLayer b)
    {
        if (a is null && b is null)
            return true;

        if (a is null || b is null)
            return false;

        return a.Type == b.Type && a.LayerType == b.LayerType;
    }

    public static bool operator !=(PlayerLayer a, PlayerLayer b) => !(a == b);

    public static implicit operator bool(PlayerLayer exists) => exists != null;

    private bool Equals(PlayerLayer other) => Equals(Player, other.Player) && Type == other.Type && GetHashCode() == other.GetHashCode();

    public override bool Equals(object obj)
    {
        if (obj is null)
            return false;

        if (ReferenceEquals(this, obj))
            return true;

        if (obj.GetType() != typeof(PlayerLayer))
            return false;

        return Equals((PlayerLayer)obj);
    }

    public override int GetHashCode() => HashCode.Combine(Player, Type, LayerType);

    public override string ToString() => Name;

    public void Delete()
    {
        OnLobby();
        Player = null;
    }

    public static void DeleteAll()
    {
        AllLayers.ForEach(x => x.Delete());
        Role.AllRoles.Clear();
        Objectifier.AllObjectifiers.Clear();
        Modifier.AllModifiers.Clear();
        Ability.AllAbilities.Clear();
        AllLayers.Clear();
    }

    public static List<PlayerLayer> GetLayers(PlayerControl player) => AllLayers.Where(x => x.Player == player).ToList();

    public static List<PlayerLayer> GetLayers(LayerEnum type) => AllLayers.Where(x => x.Type == type).ToList();

    public static List<T> GetLayers<T>(LayerEnum type) where T : PlayerLayer => GetLayers(type).Cast<T>().ToList();
}