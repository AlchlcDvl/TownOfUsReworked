namespace TownOfUsReworked.PlayerLayers;

public abstract class PlayerLayer : IPlayerLayer, IDisposable
{
    public abstract UColor MainColor { get; }
    public abstract UColor LayerColor { get; }
    public abstract PlayerLayerEnum LayerType { get; }
    public abstract LayerEnum Type { get; }
    public abstract bool UseMainColor { get; }

    public virtual Func<string> Description => () => "- None";
    public virtual AttackEnum AttackVal => AttackEnum.None;
    public virtual DefenseEnum DefenseVal => DefenseEnum.None;
    public virtual bool Hidden => false;
    public virtual bool CanVent => false;

    public string Name { get; set; }
    // public LayerHandler Handler { get; set; }
    public PlayerControl Player { get; set; }
    public bool Winner { get; set; }
    protected bool Ignore { get; private set; }

    public bool Dead => Data?.IsDead ?? true;
    public bool Disconnected => Data?.Disconnected ?? true;
    public bool Alive => !Disconnected && !Dead;
    public bool Local => Player?.AmOwner ?? false;
    public UColor Color => UseMainColor ? MainColor : LayerColor;

    public NetworkedPlayerInfo Data => Player?.Data;
    public string PlayerName => Player?.name ?? Data?.PlayerName ?? "Playerless";
    public byte PlayerId => Player?.PlayerId ?? 255;
    public int TasksLeft => Data?.Tasks?.Count(x => !x.Complete) ?? -1;
    public int TasksCompleted => Data?.Tasks?.Count(x => x.Complete) ?? -1;
    public int TotalTasks => Data?.Tasks?.Count ?? -1;
    public bool TasksDone => (Player?.CanDoTasks() ?? false) && (TasksLeft == 0 || TasksCompleted >= TotalTasks);

    public string ColorString => $"<#{Color.ToHtmlStringRGBA()}>";

    public static readonly List<PlayerLayer> AllLayers = [];

    protected PlayerLayer() => AllLayers.Add(this);

    ~PlayerLayer() => End();

    /// <summary>
    /// Starts the layer, ensuring that it's valid and attaches it to a player.
    /// </summary>
    /// <param name="player">The player to attach the layer to.</param>
    /// <remarks>Idk why, but the code for some reason fails to set the player in the constructor, so I was forced to make this and it works.</remarks>
    public void Start(PlayerControl player)
    {
        Name = TranslationManager.Translate($"Layer.{Type}");
        Player = player;
        Ignore = false;
        Init();

        if (Local)
            EnteringLayer();
    }

    /// <summary>
    /// Invalids and ends the layer, detaching it from the player it was originally attached to.
    /// </summary>
    public void End()
    {
        ClearArrows();

        if (Local)
            ExitingLayer();

        Deinit();
        Ignore = true;
        Player = null;
    }

    /// <summary>
    /// Initialises the layer.
    /// </summary>
    protected virtual void Init() {}

    /// <summary>
    /// Denitialises the layer.
    /// </summary>
    protected virtual void Deinit() {}

    /// <summary>
    /// Performs an action once the intro cutscene ends (called in <see cref="IntroCutscene.OnDestroy"/>). Runs locally.
    /// </summary>
    public virtual void OnIntroEnd() {}

    /// <summary>
    /// Performs an action on <see cref="Player"/> (called in <see cref="PlayerControlHandler.Update"/>). Runs for everyone.
    /// </summary>
    public virtual void UpdatePlayer() {}

    /// <summary>
    /// Performs an action on a player (called in <see cref="PlayerControlHandler.Update"/>). Runs locally.
    /// </summary>
    public virtual void UpdatePlayer(PlayerControl __instance) {}

    /// <summary>
    /// Performs an action on the <see cref="PlayerVoteArea"/> that references <see cref="Player"/> (called in <see cref="VoteAreaHandler.Update"/>). Runs for everyone.
    /// </summary>
    public virtual void UpdateVoteArea() {}

    /// <summary>
    /// Performs an action on the <see cref="PlayerVoteArea"/> that references a player (called in <see cref="VoteAreaHandler.Update"/>). Runs locally.
    /// </summary>
    public virtual void UpdateVoteArea(PlayerVoteArea __instance) {}

    /// <summary>
    /// Performs an action that updates the hud (called in <see cref="HudManager.Update"/>). Runs locally.
    /// </summary>
    public virtual void UpdateHud(HudManager __instance) {}

    /// <summary>
    /// Performs an action that updates the meeting phone (called in <see cref="MeetingHud.Update"/>). Runs locally.
    /// </summary>
    public virtual void UpdateMeeting(MeetingHud __instance) {}

    /// <summary>
    /// Performs an action after vote calculation is complete (called in <see cref="VoteAreaHandler.Update"/>). Runs locally.
    /// </summary>
    public virtual void VoteComplete(MeetingHud __instance) {}

    /// <summary>
    /// Performs an action before your vote is confirmed (called in <see cref="MeetingHud.VotingComplete"/>). Runs locally.
    /// </summary>
    public virtual void ConfirmVotePrefix(MeetingHud __instance) {}

    /// <summary>
    /// Performs an action after your vote is confirmed (called in <see cref="MeetingHud.VotingComplete"/>). Runs locally.
    /// </summary>
    public virtual void ConfirmVotePostfix(MeetingHud __instance) {}

    /// <summary>
    /// Performs an action when a vote is cleared (called in <see cref="MeetingHud.ClearVote"/>). Runs locally.
    /// </summary>
    public virtual void ClearVote(MeetingHud __instance) {}

    /// <summary>
    /// Performs an action when a vote area is selected, making use of the id (called in <see cref="MeetingHud.Select"/>). Runs locally.
    /// </summary>
    public virtual void SelectVote(MeetingHud __instance, int id) {}

    /// <summary>
    /// Performs an action that updates the map (called in <see cref="MapBehaviour.FixedUpdate"/>). Runs locally.
    /// </summary>
    public virtual void UpdateMap(MapBehaviour __instance) {}

    /// <summary>
    /// Performs an action that updates the hud (called in <see cref="VoteAreaHandler.Update"/>). Runs locally.
    /// </summary>
    public virtual void ExitingLayer() {}

    /// <summary>
    /// Performs an action that updates the hud (called in <see cref="VoteAreaHandler.Update"/>). Runs locally.
    /// </summary>
    public virtual void EnteringLayer() {}

    /// <summary>
    /// Performs an action that updates the hud (called in <see cref="VoteAreaHandler.Update"/>). Runs locally.
    /// </summary>
    public virtual void BeforeMeeting() {}

    /// <summary>
    /// Performs an action that updates the hud (called in <see cref="VoteAreaHandler.Update"/>). Runs locally.
    /// </summary>
    public virtual void OnMeetingStart(MeetingHud __instance) {}

    /// <summary>
    /// Performs an action that updates the hud (called in <see cref="VoteAreaHandler.Update"/>). Runs locally.
    /// </summary>
    public virtual void OnMeetingEnd(MeetingHud __instance) {}

    /// <summary>
    /// Performs an action that updates the hud (called in <see cref="VoteAreaHandler.Update"/>). Runs locally.
    /// </summary>
    public virtual void OnBodyReport(NetworkedPlayerInfo info) {}

    /// <summary>
    /// Performs an action that updates the hud (called in <see cref="VoteAreaHandler.Update"/>). Runs locally.
    /// </summary>
    public virtual void UponTaskComplete(uint taskId) {}

    /// <summary>
    /// Performs an action that updates the hud (called in <see cref="VoteAreaHandler.Update"/>). Runs locally.
    /// </summary>
    public virtual void ReadRPC(MessageReader reader) {}

    /// <summary>
    /// Performs an action that updates the hud (called in <see cref="VoteAreaHandler.Update"/>). Runs locally.
    /// </summary>
    public virtual void OnDeath(DeathReason reason, DeathReasonEnum reason2, PlayerControl killer) {}

    /// <summary>
    /// Performs an action that updates the hud (called in <see cref="VoteAreaHandler.Update"/>). Runs locally.
    /// </summary>
    public virtual void OnRevive() {}

    /// <summary>
    /// Performs an action that updates the hud (called in <see cref="VoteAreaHandler.Update"/>). Runs locally.
    /// </summary>
    public virtual void PostAssignment() {}

    /// <summary>
    /// Performs an action that updates the hud (called in <see cref="VoteAreaHandler.Update"/>). Runs locally.
    /// </summary>
    public virtual void OnKill(PlayerControl victim) {}

    /// <summary>
    /// Performs an action that updates the hud (called in <see cref="VoteAreaHandler.Update"/>). Runs locally.
    /// </summary>
    public virtual void ClearArrows() {}

    /// <summary>
    /// Performs an action that updates the hud (called in <see cref="VoteAreaHandler.Update"/>). Runs locally.
    /// </summary>
    protected virtual void CheckWin() {}

    /// <summary>
    /// Performs an action that updates the hud (called in <see cref="VoteAreaHandler.Update"/>). Runs locally.
    /// </summary>
    public virtual void UpdateSelfName(ref string name, ref UColor color, ref bool revealed, ref bool removeFromConsig) {}

    /// <summary>
    /// Performs an action that updates the hud (called in <see cref="VoteAreaHandler.Update"/>). Runs locally.
    /// </summary>
    public virtual void UpdatePlayerName(LayerHandler handler, PlayerControl player, bool meeting, ref string name, ref UColor color, ref bool revealed, ref bool removeFromConsig) {}

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

    public static implicit operator bool(PlayerLayer exists) => exists != null;

    private bool Equals(PlayerLayer other) => other.Player == Player && other.LayerType == LayerType && Type == other.Type;

    public override bool Equals(object obj)
    {
        if (obj is not PlayerLayer pl)
            return false;

        return ReferenceEquals(this, obj) || Equals(pl);
    }

    public override int GetHashCode() => HashCode.Combine(PlayerId, Type, LayerType);

    public override string ToString() => Name;

    public static IEnumerable<T> GetLayers<T>(bool includeIgnored = false) where T : IPlayerLayer => (includeIgnored ? AllLayers : AllLayers.Where(x => !x.Ignore)).OfType<T>();

    public static IEnumerable<PlayerLayer> LocalLayers() => CustomPlayer.Local.GetLayers();

    public void Dispose()
    {
        if (!Ignore)
           End();

        GC.SuppressFinalize(this);
    }
}