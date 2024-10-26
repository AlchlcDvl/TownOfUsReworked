namespace TownOfUsReworked.PlayerLayers;

public abstract class PlayerLayer
{
    public virtual UColor Color => CustomColorManager.Layer;
    public virtual string Name => "None";
    public string Short => Modules.Info.AllInfo.Find(x => x.Name == Name)?.Short;
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

    public bool Dead => Data?.IsDead ?? true;
    public bool Disconnected => Data?.Disconnected ?? true;
    public bool Alive => !Disconnected && !Dead;
    public bool Local => Player?.AmOwner == true;

    public NetworkedPlayerInfo Data => Player?.Data;
    public string PlayerName => Data?.PlayerName ?? "";
    public byte PlayerId => Player?.PlayerId ?? 255;
    public int TasksLeft => Data?.Tasks?.Count(x => !x.Complete) ?? -1;
    public int TasksCompleted => Data?.Tasks?.Count(x => x.Complete) ?? -1;
    public int TotalTasks => Data?.Tasks?.Count ?? -1;
    public bool TasksDone => Player.CanDoTasks() && (TasksLeft == 0 || TasksCompleted >= TotalTasks);

    public string ColorString => $"<color=#{Color.ToHtmlStringRGBA()}>";

    public static readonly List<PlayerLayer> AllLayers = [];
    // public static readonly Dictionary<byte, List<PlayerLayer>> LayerLookup = [];
    public static List<PlayerLayer> LocalLayers() => CustomPlayer.Local.GetLayers();

    protected PlayerLayer() => AllLayers.Add(this);

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
        Player = null;
        Ignore = true;
        Deinit();

        if (Local)
            ExitingLayer();

        return this;
    }

    public virtual void Init() {}

    public virtual void Deinit() {}

    public virtual void OnIntroEnd() {}

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

    public virtual void OnMeetingStart(MeetingHud __instance) {}

    public virtual void OnMeetingEnd(MeetingHud __instance) {}

    public virtual void OnBodyReport(NetworkedPlayerInfo info) {}

    public virtual void UponTaskComplete(uint taskId) {}

    public virtual void ReadRPC(MessageReader reader) {}

    public virtual void OnDeath() {}

    public virtual void OnRevive() {}

    public void GameEnd()
    {
        if (WinState != WinLose.None)
        {
            EndGame();
            return;
        }

        if (!Player || Disconnected || LayerType is PlayerLayerEnum.Ability or PlayerLayerEnum.Modifier)
            return;
        else if (Dead)
        {
            if (Type == LayerEnum.Phantom && TasksDone && ((Role)this).Faithful)
            {
                WinState = WinLose.PhantomWins;
                CallRpc(CustomRPC.WinLose, WinLose.PhantomWins);
            }
            else if (LayerType == PlayerLayerEnum.Role && ((Role)this).Alignment == Alignment.NeutralEvil && NeutralEvilSettings.NeutralEvilsEndGame)
            {
                if (Type == LayerEnum.Jester && ((Jester)this).VotedOut)
                {
                    WinState = WinLose.JesterWins;
                    Winner = true;
                }
                else if (Type == LayerEnum.Executioner && ((Executioner)this).TargetVotedOut)
                {
                    WinState = WinLose.ExecutionerWins;
                    Winner = true;
                }
                else if (Type == LayerEnum.Actor && ((Actor)this).Guessed)
                {
                    WinState = WinLose.ActorWins;
                    Winner = true;
                }
                else if (Type == LayerEnum.Troll && ((Troll)this).Killed)
                {
                    WinState = WinLose.TrollWins;
                    Winner = true;
                }
            }
        }
        else if (LayerType == PlayerLayerEnum.Disposition)
        {
            if (Type == LayerEnum.Corrupted && CorruptedWin(Player))
            {
                WinState = WinLose.CorruptedWins;

                if (Corrupted.AllCorruptedWin)
                    GetLayers<Corrupted>().ForEach(x => x.Winner = true);

                Winner = true;
                CallRpc(CustomRPC.WinLose, WinLose.CorruptedWins, this);
            }
            else if (Type == LayerEnum.Lovers && LoversWin(Player))
            {
                WinState = WinLose.LoveWins;
                Winner = true;
                ((Lovers)this).OtherLover.GetDisposition().Winner = true;
                CallRpc(CustomRPC.WinLose, WinLose.LoveWins, this);
            }
            else if (Type == LayerEnum.Rivals && RivalsWin(Player))
            {
                WinState = WinLose.RivalWins;
                Winner = true;
                CallRpc(CustomRPC.WinLose, WinLose.RivalWins, this);
            }
            else if (Type == LayerEnum.Taskmaster && TasksDone)
            {
                WinState = WinLose.TaskmasterWins;
                Winner = true;
                CallRpc(CustomRPC.WinLose, WinLose.TaskmasterWins, this);
            }
            else if (Type == LayerEnum.Mafia && MafiaWin())
            {
                WinState = WinLose.MafiaWins;
                CallRpc(CustomRPC.WinLose, WinLose.MafiaWins);
            }
            else if (this is Defector defector)
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

                if (WinState == WinLose.None && DefectorWins())
                    WinState = WinLose.DefectorWins;

                CallRpc(CustomRPC.WinLose, WinState);
            }
            else if (Type == LayerEnum.Overlord && OverlordWins() && Alive)
            {
                WinState = WinLose.OverlordWins;
                GetLayers<Overlord>().Where(ov => ov.Alive).ForEach(x => x.Winner = true);
                CallRpc(CustomRPC.WinLose, WinLose.OverlordWins);
            }
        }
        else if (this is Role role)
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
            else if (role.Faction == Faction.Syndicate && (role.Faithful || Type == LayerEnum.Betrayer || role.IsSynAlly || role.IsSynDefect || role.IsSynFanatic || role.IsSynTraitor) &&
                SyndicateWins())
            {
                WinState = WinLose.SyndicateWins;
                CallRpc(CustomRPC.WinLose, WinLose.SyndicateWins);
            }
            else if (role.Faction == Faction.Intruder && (role.Faithful || Type == LayerEnum.Betrayer || role.IsIntDefect || role.IsIntAlly || role.IsIntFanatic || role.IsIntTraitor) &&
                IntrudersWin())
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
            else if (AllNeutralsWin() && role.Faithful)
            {
                WinState = WinLose.AllNeutralsWin;
                CallRpc(CustomRPC.WinLose, WinLose.AllNeutralsWin);
            }
            else if (AllNKsWin() && role.Faithful && role.Alignment == Alignment.NeutralKill)
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
            else if (NeutralEvilSettings.NeutralEvilsEndGame && role.Alignment == Alignment.NeutralEvil)
            {
                if (Type == LayerEnum.Executioner && ((Executioner)role).TargetVotedOut)
                {
                    WinState = WinLose.ExecutionerWins;
                    Winner = true;
                }
                else if (Type == LayerEnum.BountyHunter && ((BountyHunter)role).TargetKilled)
                {
                    WinState = WinLose.BountyHunterWins;
                    Winner = true;
                }
                else if (Type == LayerEnum.Cannibal && ((Cannibal)role).Eaten)
                {
                    WinState = WinLose.CannibalWins;
                    Winner = true;
                }
                else if (Type == LayerEnum.Guesser && ((Guesser)role).TargetGuessed)
                {
                    WinState = WinLose.GuesserWins;
                    Winner = true;
                }
            }
            else if (Type == LayerEnum.Runner && TasksDone)
            {
                WinState = WinLose.TaskRunnerWins;
                Winner = true;
                CallRpc(CustomRPC.WinLose, WinLose.TaskRunnerWins, this);
            }
            else if (Type == LayerEnum.Hunter && HunterWins())
            {
                WinState = WinLose.HunterWins;
                CallRpc(CustomRPC.WinLose, WinLose.HunterWins);
            }
            else if (Type == LayerEnum.Hunted && HuntedWins())
            {
                WinState = WinLose.HuntedWins;
                CallRpc(CustomRPC.WinLose, WinLose.HuntedWins);
            }
            else if (Type == LayerEnum.Betrayer && role.Faction == Faction.Neutral)
            {
                WinState = WinLose.BetrayerWins;
                CallRpc(CustomRPC.WinLose, WinLose.BetrayerWins);
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

    public bool Equals(PlayerLayer other) => other.Player == Player && other.LayerType == LayerType && Type == other.Type && GetHashCode() == other.GetHashCode();

    public override bool Equals(object obj)
    {
        if (obj is null)
            return false;

        if (ReferenceEquals(this, obj))
            return true;

        var type = obj.GetType();
        var plType = typeof(PlayerLayer);

        if (!type.IsAssignableTo(plType) && type != plType && !plType.IsAssignableFrom(type))
            return false;

        return Equals((PlayerLayer)obj);
    }

    public override int GetHashCode() => HashCode.Combine(Player, Type, LayerType);

    public override string ToString() => Name;

    public static void DeleteAll()
    {
        AllLayers.ForEach(x => x.Deinit());
        AllLayers.Clear();
        /*LayerLookup.Values.ForEach(x => x.Clear());
        LayerLookup.Clear();
        Role.RoleLookup.Clear();
        Disposition.DispositionLookup.Clear();
        Modifier.ModifierLookup.Clear();
        Ability.AbilityLookup.Clear();*/
    }

    public static List<T> GetLayers<T>(bool includeIgnored = false) where T : PlayerLayer => [ .. AllLayers.Where(x => x is T && (!x.Ignore || includeIgnored) && x.Player).Cast<T>() ];

    public static List<PlayerLayer> GetLayers(LayerEnum type, bool includeIgnored = false) => [ .. AllLayers.Where(x => x.Type == type && (!x.Ignore || includeIgnored) && x.Player) ];
}