namespace TownOfUsReworked.PlayerLayers;

public abstract class PlayerLayer
{
    public virtual UColor Color => CustomColorManager.Layer;
    public virtual string Name => "None";
    public virtual PlayerLayerEnum LayerType => PlayerLayerEnum.None;
    public virtual LayerEnum Type => LayerEnum.None;
    public virtual Func<string> Description => () => "- None";
    public virtual Func<string> Attributes => () => "- None";
    public virtual AttackEnum AttackVal => AttackEnum.None;
    public virtual DefenseEnum DefenseVal => DefenseEnum.None;
    public virtual bool Hidden => false;

    public PlayerControl Player { get; set; }
    public bool IsBlocked { get; set; }
    public bool Winner { get; set; }
    public bool Ignore { get; set; }

    // public string Short => Modules.Info.AllInfo.Find(x => x.Name == Name)?.Short;

    public bool Dead => Data?.IsDead ?? true;
    public bool Disconnected => Data?.Disconnected ?? true;
    public bool Alive => !Disconnected && !Dead;
    public bool Local => Player?.AmOwner ?? false;

    public NetworkedPlayerInfo Data => Player?.Data;
    public string PlayerName => Data?.PlayerName ?? "";
    public byte PlayerId => Player?.PlayerId ?? 255;
    public int TasksLeft => Data?.Tasks?.Count(x => !x.Complete) ?? -1;
    public int TasksCompleted => Data?.Tasks?.Count(x => x.Complete) ?? -1;
    public int TotalTasks => Data?.Tasks?.Count ?? -1;
    public bool TasksDone => Player.CanDoTasks() && (TasksLeft == 0 || TasksCompleted >= TotalTasks);

    public string ColorString => $"<#{Color.ToHtmlStringRGBA()}>";

    public static readonly List<PlayerLayer> AllLayers = [];

    protected PlayerLayer() => AllLayers.Add(this);

    ~PlayerLayer() => End();

    // Idk why, but the code for some reason fails to set the player in the constructor, so I was forced to make this and it sorta works
    public T Start<T>(PlayerControl player) where T : PlayerLayer => Start(player) as T;

    public PlayerLayer Start(PlayerControl player)
    {
        Player = player;
        Ignore = false;
        Init();

        if (Local)
            EnteringLayer();

        return this;
    }

    public T End<T>() where T : PlayerLayer => End() as T;

    public PlayerLayer End()
    {
        Deinit();

        if (Local)
            ExitingLayer();

        Ignore = true;
        Player = null;
        return this;
    }

    public virtual void Init() {}

    public virtual void Deinit() {}

    public virtual void OnIntroEnd() {}

    public virtual void UpdatePlayer() {}

    public virtual void UpdatePlayer(PlayerControl __instance) {}

    public virtual void UpdateVoteArea() {}

    public virtual void UpdateVoteArea(PlayerVoteArea voteArea) {}

    public virtual void UpdateHud(HudManager __instance) {}

    public virtual void UpdateMeeting(MeetingHud __instance) {}

    public virtual void VoteComplete(MeetingHud __instance) {}

    public virtual void ConfirmVotePrefix(MeetingHud __instance) {}

    public virtual void ConfirmVotePostfix(MeetingHud __instance) {}

    public virtual void ClearVote(MeetingHud __instance) {}

    public virtual void SelectVote(MeetingHud __instance, int id) {}

    public virtual void UpdateMap(MapBehaviour __instance) {}

    public virtual void ExitingLayer() {}

    public virtual void EnteringLayer() {}

    public virtual void BeforeMeeting() {}

    public virtual void OnMeetingStart(MeetingHud __instance) {}

    public virtual void OnMeetingEnd(MeetingHud __instance) {}

    public virtual void OnBodyReport(NetworkedPlayerInfo info) {}

    public virtual void UponTaskComplete(uint taskId) {}

    public virtual void ReadRPC(MessageReader reader) {}

    public virtual void OnDeath(DeathReason reason, DeathReasonEnum reason2, PlayerControl killer) {}

    public virtual void OnRevive() {}

    public virtual void PostAssignment() {}

    public virtual void OnKill(PlayerControl victim) {}

    public void GameEnd()
    {
        if (WinState != WinLose.None)
        {
            EndGame();
            return;
        }

        if (!Player || !Player.Data || Disconnected || LayerType is PlayerLayerEnum.Ability or PlayerLayerEnum.Modifier || Ignore)
            return;

        switch (this)
        {
            case Disposition disp:
            {
                switch (disp)
                {
                    case Corrupted:
                    {
                        if (CorruptedWin(Player))
                        {
                            WinState = WinLose.CorruptedWins;

                            if (Corrupted.AllCorruptedWin)
                                GetLayers<Corrupted>().ForEach(x => x.Winner = true);
                            else
                                Winner = true;

                            CallRpc(CustomRPC.WinLose, WinLose.CorruptedWins, this);
                        }

                        break;
                    }
                    case Lovers lovers:
                    {
                        if (LoversWin(Player))
                        {
                            WinState = WinLose.LoveWins;
                            Winner = true;
                            lovers.OtherLover.GetDisposition().Winner = true;
                            CallRpc(CustomRPC.WinLose, WinLose.LoveWins, this);
                        }

                        break;
                    }
                    case Rivals:
                    {
                        if (RivalsWin(Player))
                        {
                            WinState = WinLose.RivalWins;
                            Winner = true;
                            CallRpc(CustomRPC.WinLose, WinLose.RivalWins, this);
                        }

                        break;
                    }
                    case Taskmaster:
                    {
                        if (TasksDone)
                        {
                            WinState = WinLose.TaskmasterWins;
                            Winner = true;
                            CallRpc(CustomRPC.WinLose, WinLose.TaskmasterWins, this);
                        }

                        break;
                    }
                    case Mafia:
                    {
                        if (MafiaWin())
                        {
                            WinState = WinLose.MafiaWins;
                            Winner = true;
                            CallRpc(CustomRPC.WinLose, WinLose.MafiaWins);
                        }

                        break;
                    }
                    case Defector defector:
                    {
                        if (DefectorWins())
                        {
                            if (defector.Side == Faction.Neutral)
                            {
                                WinState = NeutralSettings.NoSolo switch
                                {
                                    NoSolo.AllNKs => WinLose.AllNKsWin,
                                    NoSolo.AllNeutrals => WinLose.AllNeutralsWin,
                                    _ => WinLose.None
                                };
                            }

                            if (WinState == WinLose.None)
                                WinState = WinLose.DefectorWins;

                            CallRpc(CustomRPC.WinLose, WinState, this);
                        }

                        break;
                    }
                    case Overlord:
                    {
                        if (Alive && OverlordWins())
                        {
                            WinState = WinLose.OverlordWins;
                            GetLayers<Overlord>().Where(ov => ov.Alive).ForEach(x => x.Winner = true);
                            CallRpc(CustomRPC.WinLose, WinLose.OverlordWins);
                        }
                        break;
                    }
                }

                return;
            }
            case Role role:
            {
                switch (role)
                {
                    case Phantom phantom:
                    {
                        if (TasksDone && phantom.Faithful)
                        {
                            WinState = WinLose.PhantomWins;
                            CallRpc(CustomRPC.WinLose, WinLose.PhantomWins);
                        }

                        break;
                    }
                    case Evil evil:
                    {
                        if (NeutralEvilSettings.NeutralEvilsEndGame && evil.HasWon)
                        {
                            WinState = evil.EndState;
                            Winner = true;
                        }

                        break;
                    }
                    case Runner:
                    {
                        if (TasksDone)
                        {
                            WinState = WinLose.TaskRunnerWins;
                            Winner = true;
                            CallRpc(CustomRPC.WinLose, WinLose.TaskRunnerWins, this);
                        }

                        break;
                    }
                    case Hunter:
                    {
                        if (HunterWins())
                        {
                            WinState = WinLose.HunterWins;
                            CallRpc(CustomRPC.WinLose, WinLose.HunterWins);
                        }
                        break;
                    }
                    case Hunted:
                    {
                        if (HuntedWin())
                        {
                            WinState = WinLose.HuntedWin;
                            CallRpc(CustomRPC.WinLose, WinLose.HuntedWin);
                        }
                        break;
                    }
                    default:
                    {
                        if ((role.IsRecruit || Type == LayerEnum.Jackal) && CabalWin())
                        {
                            WinState = WinLose.CabalWins;
                            CallRpc(CustomRPC.WinLose, WinLose.CabalWins, this);
                        }
                        else if ((role.IsPersuaded || Type == LayerEnum.Whisperer) && SectWin())
                        {
                            WinState = WinLose.SectWins;
                            CallRpc(CustomRPC.WinLose, WinLose.SectWins);
                        }
                        else if ((role.IsBitten || Type == LayerEnum.Dracula) && UndeadWin())
                        {
                            WinState = WinLose.UndeadWins;
                            CallRpc(CustomRPC.WinLose, WinLose.UndeadWins);
                        }
                        else if ((role.IsResurrected || Type == LayerEnum.Necromancer) && ReanimatedWin())
                        {
                            WinState = WinLose.ReanimatedWins;
                            CallRpc(CustomRPC.WinLose, WinLose.ReanimatedWins);
                        }
                        else if (role.Faction == Faction.Syndicate && (role.Faithful || Type == LayerEnum.Betrayer || role.IsSynAlly || role.IsSynDefect || role.IsSynFanatic ||
                            role.IsSynTraitor) && SyndicateWins())
                        {
                            WinState = WinLose.SyndicateWins;
                            CallRpc(CustomRPC.WinLose, WinLose.SyndicateWins);
                        }
                        else if (role.Faction == Faction.Intruder && (role.Faithful || Type == LayerEnum.Betrayer || role.IsIntDefect || role.IsIntAlly || role.IsIntFanatic ||
                            role.IsIntTraitor) && IntrudersWin())
                        {
                            WinState = WinLose.IntrudersWin;
                            CallRpc(CustomRPC.WinLose, WinLose.IntrudersWin);
                        }
                        else if (role.Faction == Faction.Crew && (role.Faithful || role.IsCrewAlly || role.IsCrewDefect) && CrewWins())
                        {
                            WinState = WinLose.CrewWins;
                            CallRpc(CustomRPC.WinLose, WinLose.CrewWins);
                        }
                        else if (role.Faithful && ApocWins() && role.Alignment is Alignment.NeutralApoc or Alignment.NeutralHarb)
                        {
                            WinState = WinLose.ApocalypseWins;
                            CallRpc(CustomRPC.WinLose, WinLose.ApocalypseWins);
                        }
                        else if (role.Faithful && role.Faction == Faction.Neutral && AllNeutralsWin())
                        {
                            WinState = WinLose.AllNeutralsWin;
                            CallRpc(CustomRPC.WinLose, WinLose.AllNeutralsWin);
                        }
                        else if (role.Faithful && role.Alignment == Alignment.NeutralKill && AllNKsWin())
                        {
                            WinState = WinLose.AllNKsWin;
                            CallRpc(CustomRPC.WinLose, WinLose.AllNKsWin);
                        }
                        else if (role.Faithful && role.Alignment == Alignment.NeutralKill && (SameNKWins(role.Type) || SoloNKWins(Player)))
                        {
                            WinState = role.Type switch
                            {
                                LayerEnum.Arsonist => WinLose.ArsonistWins,
                                LayerEnum.Cryomaniac => WinLose.CryomaniacWins,
                                LayerEnum.Glitch => WinLose.GlitchWins,
                                LayerEnum.Juggernaut => WinLose.JuggernautWins,
                                LayerEnum.Murderer => WinLose.MurdererWins,
                                LayerEnum.SerialKiller => WinLose.SerialKillerWins,
                                LayerEnum.Werewolf => WinLose.WerewolfWins,
                                _ => WinLose.None,
                            };

                            if (NeutralSettings.NoSolo == NoSolo.SameNKs)
                            {
                                foreach (var role2 in GetLayers<Neutral>().Where(x => x.Type == role.Type))
                                {
                                    if (!role2.Disconnected && role2.Faithful)
                                        role2.Winner = true;
                                }
                            }

                            Winner = true;
                            CallRpc(CustomRPC.WinLose, WinState, this);
                        }
                        else if (Type == LayerEnum.Betrayer && role.Faction == Faction.Neutral)
                        {
                            WinState = WinLose.BetrayerWins;
                            CallRpc(CustomRPC.WinLose, WinLose.BetrayerWins);
                        }

                        break;
                    }
                }

                break;
            }
        }
    }

    public static bool operator ==(PlayerLayer a, PlayerLayer b)
    {
        if (a is null && b is null)
            return true;

        if (a is null || b is null)
            return false;

        return a.Equals(b);
    }

    public static bool operator !=(PlayerLayer a, PlayerLayer b) => !(a == b);

    public static implicit operator bool(PlayerLayer exists) => exists != null && exists.Player;

    public bool Equals(PlayerLayer other) => other.Player == Player && other.LayerType == LayerType && Type == other.Type;

    public override bool Equals(object obj)
    {
        if (obj is null || obj is not PlayerLayer pl)
            return false;

        if (ReferenceEquals(this, obj))
            return true;

        return Equals(pl);
    }

    public override int GetHashCode() => HashCode.Combine(Player, Type, LayerType);

    public override string ToString() => Name;

    public static void DeleteAll()
    {
        AllLayers.ForEach(x => x.Deinit());
        AllLayers.Clear();
    }

    public static IEnumerable<T> GetLayers<T>(bool includeIgnored = false) where T : PlayerLayer => AllLayers.Where(x => (!x.Ignore || includeIgnored) && x.Player).OfType<T>();

    public static IEnumerable<T> GetILayers<T>(bool includeIgnored = false) where T : IPlayerLayer => AllLayers.Where(x => (!x.Ignore || includeIgnored) && x.Player).OfType<T>();

    public static List<PlayerLayer> LocalLayers() => CustomPlayer.Local.GetLayers();
}