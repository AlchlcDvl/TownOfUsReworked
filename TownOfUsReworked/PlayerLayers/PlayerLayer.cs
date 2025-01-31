namespace TownOfUsReworked.PlayerLayers;

public abstract class PlayerLayer : IPlayerLayer
{
    public virtual UColor Color => CustomColorManager.Layer;
    public virtual PlayerLayerEnum LayerType => PlayerLayerEnum.None;
    public virtual LayerEnum Type => LayerEnum.None;
    public virtual Func<string> Description => () => "- None";
    public virtual Func<string> Attributes => () => "- None";
    public virtual AttackEnum AttackVal => AttackEnum.None;
    public virtual DefenseEnum DefenseVal => DefenseEnum.None;
    public virtual bool Hidden => false;

    public string Name { get; set; }
    public PlayerControl Player { get; set; }
    public bool Winner { get; set; }
    public bool Ignore { get; set; }

    // public string Short => Modules.Info.AllAllInfo.Find(x => x.Layer == Type)?.Short;

    public bool Dead => Data?.IsDead ?? true;
    public bool Disconnected => Data?.Disconnected ?? true;
    public bool Alive => !Disconnected && !Dead;
    public bool Local => Player?.AmOwner ?? false;

    public NetworkedPlayerInfo Data => Player?.Data;
    public string PlayerName => Player?.name ?? Data?.PlayerName ?? "";
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
    public void Start(PlayerControl player)
    {
        Name = TranslationManager.Translate($"Layer.{Type}");
        Player = player;
        Ignore = false;
        Init();

        if (Local)
            EnteringLayer();
    }

    public void End()
    {
        ClearArrows();

        if (Local)
            ExitingLayer();

        Deinit();
        Ignore = true;
        Player = null;
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

    public virtual void ClearArrows() {}

    public virtual void CheckWin() {}

    // public virtual void UpdateSelfName(ref string name, ref UColor color, ref bool revealed, ref bool removeFromConsig) {}

    // public virtual void UpdatePlayerName(LayerHandler playerHandler, bool deadSeeEverything, ref string name, ref UColor color, ref bool revealed, ref bool removeFromConsig) {}

    public void GameEnd()
    {
        if (!Player || !Player.Data || Disconnected || LayerType is PlayerLayerEnum.Ability or PlayerLayerEnum.Modifier || Ignore)
            return;

        CheckWin();
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

    public override int GetHashCode() => HashCode.Combine(PlayerId, Type, LayerType);

    public override string ToString() => Name;

    public static IEnumerable<T> GetLayers<T>(bool includeIgnored = false) where T : PlayerLayer => AllLayers.Where(x => !x.Ignore || includeIgnored).OfType<T>();

    public static IEnumerable<T> GetILayers<T>(bool includeIgnored = false) where T : IPlayerLayer => AllLayers.Where(x => !x.Ignore || includeIgnored).OfType<T>();

    public static IEnumerable<PlayerLayer> LocalLayers() => CustomPlayer.Local.GetLayers();
}