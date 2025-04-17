namespace TownOfUsReworked.PlayerLayers;

/// <summary>
/// The main class that identifies the aspects attached to a player.
/// </summary>
/// <remarks>Adopted from the role system of Town of Us but restructured to be better.</remarks>
public abstract class PlayerLayer : IPlayerLayer, IDisposable, INetSerializable
{
    /// <summary>
    /// Gets the main color for this layer.
    /// </summary>
    protected abstract UColor MainColor { get; }

    /// <summary>
    /// Gets the base layer color for this layer.
    /// </summary>
    protected abstract UColor LayerColor { get; }

    /// <summary>
    /// Gets the base type of the layer.
    /// </summary>
    public abstract PlayerLayerEnum LayerType { get; }

    /// <summary>
    /// Gets the type for this layer.
    /// </summary>
    public abstract LayerEnum Type { get; }

    /// <summary>
    /// Get a value indicating condition to check whether or not the layer's main should be used (if not, its base color is used).
    /// </summary>
    protected abstract bool UseMainColor { get; }

    /// <summary>
    /// Gets the description for the layer.
    /// </summary>
    public virtual Func<string> Description => () => "- None";

    /// <summary>
    /// Gets the attack value of the layer.
    /// </summary>
    public virtual AttackEnum AttackVal => AttackEnum.None;

    /// <summary>
    /// Gets the defense value of the layer.
    /// </summary>
    public virtual DefenseEnum DefenseVal => DefenseEnum.None;

    /// <summary>
    /// Gets a value indicating whether or not the player knows they have it.
    /// </summary>
    public virtual bool Hidden => false;

    /// <summary>
    /// Gets gets a value indicating whether or not the layer can allow the player to vent.
    /// </summary>
    public virtual bool CanVent => false;

    /// <summary>
    /// Gets or sets the name of the layer.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the handler for this layer.
    /// </summary>
    public LayerHandler Handler { get; set; }

    /// <summary>
    /// Gets or sets the player.
    /// </summary>
    public PlayerControl Player { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether or not the layer is a winner.
    /// </summary>
    public bool Winner { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether or not the layer has been deinitialised.
    /// </summary>
    protected bool Deinitialised { get; private set; } = true; // Start uninitialised

    /// <summary>
    /// Gets a value indicating whether or not the player is dead.
    /// </summary>
    public bool Dead => Data?.IsDead ?? true;

    /// <summary>
    /// Gets a value indicating whether or not the player has disconnected.
    /// </summary>
    public bool Disconnected => Data?.Disconnected ?? true;

    /// <summary>
    /// Gets a value indicating whether or not the player is alive.
    /// </summary>
    public bool Alive => !Disconnected && !Dead;

    /// <summary>
    /// Gets a value indicating whether or not the player is the local player.
    /// </summary>
    public bool Local => Player?.AmOwner ?? false;

    /// <summary>
    /// Gets the color of the layer.
    /// </summary>
    public UColor Color => UseMainColor ? MainColor : LayerColor;

    /// <summary>
    /// Gets the player's data.
    /// </summary>
    public NetworkedPlayerInfo Data => Player?.Data;

    /// <summary>
    /// Gets the player's name.
    /// </summary>
    public string PlayerName => Player?.name ?? Data?.PlayerName ?? "Playerless";

    /// <summary>
    /// Gets the player's id.
    /// </summary>
    public byte PlayerId => Player?.PlayerId ?? 255;

    /// <summary>
    /// Gets the number of tasks left for the player.
    /// </summary>
    public int TasksLeft => Data?.Tasks?.Count(x => !x.Complete) ?? -1;

    /// <summary>
    /// Gets a count of how many tasks have been completed.
    /// </summary>
    public int TasksCompleted => Data?.Tasks?.Count(x => x.Complete) ?? -1;

    /// <summary>
    /// Gets the total number of the player's tasks.
    /// </summary>
    public int TotalTasks => Data?.Tasks?.Count ?? -1;

    /// <summary>
    /// Gets a value indicating whether or not the player's tasks have been completed.
    /// </summary>
    public bool TasksDone => (Player?.CanDoTasks() ?? false) && (TasksLeft == 0 || TasksCompleted >= TotalTasks);

    /// <summary>
    /// Gets the layer's color represented as a hex code for TextMeshPro.
    /// </summary>
    public string ColorString => $"<#{Color.ToHtmlStringRGBA()}>";

    /// <summary>
    /// A list that contains all layers for the current game, initialised or not.
    /// </summary>
    public static readonly List<PlayerLayer> AllLayers = [];

    /// <summary>
    /// Initialises a new instance of PlayerLayer and adds to the list of all layers.
    /// </summary>
    protected PlayerLayer() => AllLayers.Add(this);

    /// <summary>
    /// Destructor for the layer to free up resources.
    /// </summary>
    ~PlayerLayer() => End();

    /// <summary>
    /// Starts the layer, ensuring that it's valid and attaches it to a player.
    /// </summary>
    /// <param name="player">The player to attach the layer to.</param>
    /// <remarks>Idk why, but the code for some reason fails to set the player in the constructor, so I was forced to make this and it works.</remarks>
    public void Start(PlayerControl player)
    {
        if (!Deinitialised) // Already initialised
            return;

        Name = TranslationManager.Translate($"Layer.{Type}");
        Player = player;
        Deinitialised = false;
        Init();

        if (Local)
            EnteringLayer();
    }

    /// <summary>
    /// Invalids and ends the layer, detaching it from the player it was originally attached to.
    /// </summary>
    public void End()
    {
        if (Deinitialised) // Already deinitialised
            return;

        ClearArrows();

        if (Local)
            ExitingLayer();

        Deinit();
        Deinitialised = true;
        Player = null;
    }

    /// <summary>
    /// Initialises the layer.
    /// </summary>
    protected virtual void Init() {}

    /// <summary>
    /// Deinitialises the layer.
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
    /// <param name="__instance">The player to update.</param>
    public virtual void UpdatePlayer(PlayerControl __instance) {}

    /// <summary>
    /// Performs an action on the <see cref="PlayerVoteArea"/> that references <see cref="Player"/> (called in <see cref="VoteAreaHandler.Update"/>). Runs for everyone.
    /// </summary>
    public virtual void UpdateVoteArea() {}

    /// <summary>
    /// Performs an action on the <see cref="PlayerVoteArea"/> that references a player (called in <see cref="VoteAreaHandler.Update"/>). Runs locally.
    /// </summary>
    /// <param name="__instance">The player vote area to update.</param>
    public virtual void UpdateVoteArea(PlayerVoteArea __instance) {}

    /// <summary>
    /// Performs an action that updates the hud (called in <see cref="HudManager.Update"/>). Runs locally.
    /// </summary>
    /// <param name="__instance">The hud to update.</param>
    public virtual void UpdateHud(HudManager __instance) {}

    /// <summary>
    /// Performs an action that updates the meeting phone (called in <see cref="MeetingHud.Update"/>). Runs locally.
    /// </summary>
    /// <param name="__instance">The meeting to update.</param>
    public virtual void UpdateMeeting(MeetingHud __instance) {}

    /// <summary>
    /// Performs an action after vote calculation is complete (called in <see cref="VoteAreaHandler.Update"/>). Runs locally.
    /// </summary>
    /// <param name="__instance">The meeting to handle.</param>
    public virtual void VoteComplete(MeetingHud __instance) {}

    /// <summary>
    /// Performs an action before your vote is confirmed (called in <see cref="MeetingHud.VotingComplete"/>). Runs locally.
    /// </summary>
    /// <param name="__instance">The meeting to handle.</param>
    public virtual void ConfirmVotePrefix(MeetingHud __instance) {}

    /// <summary>
    /// Performs an action after your vote is confirmed (called in <see cref="MeetingHud.VotingComplete"/>). Runs locally.
    /// </summary>
    /// <param name="__instance">The meeting to handle.</param>
    public virtual void ConfirmVotePostfix(MeetingHud __instance) {}

    /// <summary>
    /// Performs an action when a vote is cleared (called in <see cref="MeetingHud.ClearVote"/>). Runs locally.
    /// </summary>
    /// <param name="__instance">The meeting to handle.</param>
    public virtual void ClearVote(MeetingHud __instance) {}

    /// <summary>
    /// Performs an action when a vote area is selected, making use of the id (called in <see cref="MeetingHud.Select"/>). Runs locally.
    /// </summary>
    /// <param name="__instance">The meeting to handle.</param>
    /// <param name="id">The vote that was selected.</param>
    public virtual void SelectVote(MeetingHud __instance, int id) {}

    /// <summary>
    /// Performs an action that updates the map (called in <see cref="MapBehaviour.FixedUpdate"/>). Runs locally.
    /// </summary>
    /// <param name="__instance">The minimap to update.</param>
    public virtual void UpdateMap(MapBehaviour __instance) {}

    /// <summary>
    /// Performs an action for the attached player when being detached (called in <see cref="Start"/>). Runs locally.
    /// </summary>
    public virtual void ExitingLayer() {}

    /// <summary>
    /// Performs an action for the attached player when being attached (called in <see cref="End"/>). Runs locally.
    /// </summary>
    public virtual void EnteringLayer() {}

    /// <summary>
    /// Performs an action before a meeting beings (called in <see cref="PlayerControl.StartMeeting"/>). Runs locally.
    /// </summary>
    public virtual void BeforeMeeting() {}

    /// <summary>
    /// Performs an action once a meeting begins (called in <see cref="MeetingHud.Start"/>). Runs locally.
    /// </summary>
    /// <param name="__instance">The meeting to handle.</param>
    public virtual void OnMeetingStart(MeetingHud __instance) {}

    /// <summary>
    /// Performs an action once a meeting ends (called in <see cref="MeetingHud.Close"/>). Runs locally.
    /// </summary>
    /// <param name="__instance">The meeting to handle.</param>
    public virtual void OnMeetingEnd(MeetingHud __instance) {}

    /// <summary>
    /// Performs an action when reporting a body (called in <see cref="PlayerControl.StartMeeting"/>). Runs locally.
    /// </summary>
    /// <param name="info">The player that's being reported.</param>
    public virtual void OnBodyReport(NetworkedPlayerInfo info) {}

    /// <summary>
    /// Performs an action when a specific task is completed (called in <see cref="PlayerControl.CompleteTask"/>). Runs for everyone.
    /// </summary>
    /// <param name="taskId">The id of that specific task.</param>
    public virtual void UponTaskComplete(uint taskId) {}

    /// <summary>
    /// Performs an action based on the received network message (called in <see cref="PlayerControl.HandleRpc"/>). Runs for everyone.
    /// </summary>
    /// <param name="reader">The network message to read from.</param>
    public virtual void ReadRPC(NetData reader) {}

    /// <summary>
    /// Performs an action upon the death of the player (called in <see cref="MiscUtils.CustomDie"/>). Runs for everyone.
    /// </summary>
    /// <param name="reason">The reason that the player died.</param>
    /// <param name="killer">The player's killer.</param>
    public virtual void OnDeath(DeathReasonEnum reason, PlayerControl killer) {}

    /// <summary>
    /// Performs an action when the player is revived (called in <see cref="PlayerControl.Revive"/>). Runs for everyone.
    /// </summary>
    public virtual void OnRevive() {}

    /// <summary>
    /// Performs an action after the player has been assigned this layer and role assignment as taken place (called in <see cref="LayerHandler.Initialize"/>). Runs for everyone.
    /// </summary>
    public virtual void PostAssignment() {}

    /// <summary>
    /// Performs an action when killing someone (called in <see cref="MiscUtils.CustomDie"/>). Runs for everyone.
    /// </summary>
    /// <param name="victim">The player's victim.</param>
    public virtual void OnKill(PlayerControl victim) {}

    /// <summary>
    /// Clears all arrows contained by this layer. Runs for everyone.
    /// </summary>
    public virtual void ClearArrows() {}

    /// <summary>
    /// Checks if this layer has achieved their win. Runs for the host.
    /// </summary>
    protected virtual void CheckWin(List<byte> winnerIds) {}

    /// <summary>
    /// Updates the player's name (called in <see cref="NameHandler.UpdateGameName"/>). Runs for everyone.
    /// </summary>
    /// <param name="name">The name to update.</param>
    /// <param name="color">The color to update.</param>
    /// <param name="revealed">Indicates whether or not the layer is revealed.</param>
    /// <param name="removeFromConsig">Indicates whether Consiglieres should remove the player from their investigated players list.</param>
    public virtual void UpdateSelfName(ref string name, ref UColor color, ref bool revealed, ref bool removeFromConsig) {}

    /// <summary>
    /// Updates a player's name (called in <see cref="NameHandler.UpdateGameName"/>). Runs for everyone.
    /// </summary>
    /// <param name="handler">The player's layer handler.</param>
    /// <param name="player">The player.</param>
    /// <param name="meeting">Indicates whether or not the game si currently in a meeting.</param>
    /// <param name="name">The name to update.</param>
    /// <param name="color">The color to update.</param>
    /// <param name="revealed">Indicates whether or not the layer is revealed.</param>
    /// <param name="removeFromConsig">Indicates whether Consiglieres should remove the player from their investigated players list.</param>
    public virtual void UpdatePlayerName(LayerHandler handler, PlayerControl player, bool meeting, ref string name, ref UColor color, ref bool revealed, ref bool removeFromConsig) {}

    /// <summary>
    /// Checks and ends the game as needed.
    /// </summary>
    public void GameEnd(List<byte> winnerIds)
    {
        if (!Player || !Player.Data || Disconnected || LayerType is PlayerLayerEnum.Ability or PlayerLayerEnum.Modifier || Deinitialised)
            return;

        CheckWin(winnerIds);
    }

    /// <summary>
    /// Equality check.
    /// </summary>
    /// <param name="a">Left layer.</param>
    /// <param name="b">Right layer.</param>
    /// <returns>true if a and b are the same layer.</returns>
    public static bool operator ==(PlayerLayer a, PlayerLayer b)
    {
        if (a is null && b is null)
            return true;

        if (a is null || b is null)
            return false;

        return a.Equals(b);
    }

    /// <inheritdoc cref="Equals(object)"/>
    private bool Equals(PlayerLayer obj) => obj.Player == Player && obj.LayerType == LayerType && Type == obj.Type;

    /// <inheritdoc/>
    public override bool Equals(object obj)
    {
        if (obj is not PlayerLayer pl)
            return false;

        return ReferenceEquals(this, obj) || Equals(pl);
    }

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(PlayerId, Type, LayerType);

    /// <inheritdoc/>
    public override string ToString() => Name;

    /// <inheritdoc/>
    public void Dispose()
    {
        End();
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public byte[] ToBytes() => [ PlayerId, (byte)Type ];

    /// <summary>
    /// Inequality check.
    /// </summary>
    /// <param name="a">Left layer.</param>
    /// <param name="b">Right layer.</param>
    /// <returns>true if a and b are the not same layer.</returns>
    public static bool operator !=(PlayerLayer a, PlayerLayer b) => !(a == b);

    /// <summary>
    /// Equality check.
    /// </summary>
    /// <param name="exists">Left layer.</param>
    /// <returns>true the layer exists.</returns>
    public static implicit operator bool(PlayerLayer exists) => exists != null;

    /// <summary>
    /// Gets all layers of a specific type.
    /// </summary>
    /// <typeparam name="T">The type of the layer.</typeparam>
    /// <param name="includeIgnored">Flag indicating whether or not deinitialises layers should also be fetched.</param>
    /// <returns>A collection of layers of the specified type.</returns>
    public static IEnumerable<T> GetLayers<T>(bool includeIgnored = false) where T : IPlayerLayer => (includeIgnored ? AllLayers : AllLayers.Where(x => !x.Deinitialised)).OfType<T>();

    /// <summary>
    /// Gets all layers attached to the local player.
    /// </summary>
    /// <returns>A collection of the local player's layers.</returns>
    public static IEnumerable<PlayerLayer> LocalLayers() => CustomPlayer.Local.GetLayers();
}