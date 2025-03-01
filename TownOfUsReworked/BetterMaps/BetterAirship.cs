// ReSharper disable UnusedMember.Local
namespace TownOfUsReworked.BetterMaps;

/// <summary>
/// Provides enhanced functionality and customization options for the Airship map.
/// </summary>
[HeaderOption(MultiMenu.Main)]
public static class BetterAirship
{
    /// <summary>
    /// Enables or disables all BetterAirship modifications.
    /// </summary>
    [ToggleOption]
    [Sorted(0)]
    private static bool EnableBetterAirship = true;

    /// <summary>
    /// Controls how players spawn on the Airship map.<br/>
    /// Options: <c>Normal</c>, <c>Meeting</c>, <c>Fixed</c>, <c>RandomSynchronized</c>, <c>Random</c>, <c>Custom</c>
    /// </summary>
    [StringOption<AirshipSpawnType>]
    [Sorted(1)]
    public static AirshipSpawnType SpawnType = AirshipSpawnType.Normal;

    private static AirshipSpawnLocation Location1Priv = AirshipSpawnLocation.Engine;
    /// <summary>
    /// Sets the first location when selecting the custom spawn type.<br/>
    /// Options: <c>Brig</c>, <c>Engine</c>, <c>MainHall</c>, <c>Kitchen</c>, <c>Records</c>, <c>CargoBay</c>, <c>VaultRoom</c>, <c>Medical</c>, <c>Cockpit</c>
    /// </summary>
    [StringOption<AirshipSpawnLocation>]
    [Sorted(2)]
    private static AirshipSpawnLocation Location1
    {
        get => Location1Priv;
        set
        {
            if (value < Location1Priv)
            {
                while (value.IsAny(Location2Priv, Location3Priv))
                    value = (AirshipSpawnLocation)CycleByte(8, 0, (byte)value, false);
            }
            else if (value > Location1Priv)
            {
                while (value.IsAny(Location2Priv, Location3Priv))
                    value = (AirshipSpawnLocation)CycleByte(8, 0, (byte)value, true);
            }

            Location1Priv = value;
        }
    }

    private static AirshipSpawnLocation Location2Priv = AirshipSpawnLocation.Kitchen;
    /// <summary>
    /// Sets the second location when selecting the custom spawn type.<br/>
    /// Options: <c>Brig</c>, <c>Engine</c>, <c>MainHall</c>, <c>Kitchen</c>, <c>Records</c>, <c>CargoBay</c>, <c>VaultRoom</c>, <c>Medical</c>, <c>Cockpit</c>
    /// </summary>
    [StringOption<AirshipSpawnLocation>]
    [Sorted(3)]
    private static AirshipSpawnLocation Location2
    {
        get => Location2Priv;
        set
        {
            if (value < Location2Priv)
            {
                while (value.IsAny(Location1Priv, Location3Priv))
                    value = (AirshipSpawnLocation)CycleByte(8, 0, (byte)value, false);
            }
            else if (value > Location2Priv)
            {
                while (value.IsAny(Location1Priv, Location3Priv))
                    value = (AirshipSpawnLocation)CycleByte(8, 0, (byte)value, true);
            }

            Location2Priv = value;
        }
    }

    private static AirshipSpawnLocation Location3Priv = AirshipSpawnLocation.VaultRoom;
    /// <summary>
    /// Sets the third location when selecting the custom spawn type.<br/>
    /// Options: <c>Brig</c>, <c>Engine</c>, <c>MainHall</c>, <c>Kitchen</c>, <c>Records</c>, <c>CargoBay</c>, <c>VaultRoom</c>, <c>Medical</c>, <c>Cockpit</c>
    /// </summary>
    [StringOption<AirshipSpawnLocation>]
    [Sorted(4)]
    private static AirshipSpawnLocation Location3
    {
        get => Location3Priv;
        set
        {
            if (value < Location3Priv)
            {
                while (value.IsAny(Location2Priv, Location1Priv))
                    value = (AirshipSpawnLocation)CycleByte(8, 0, (byte)value, false);
            }
            else if (value > Location3Priv)
            {
                while (value.IsAny(Location2Priv, Location1Priv))
                    value = (AirshipSpawnLocation)CycleByte(8, 0, (byte)value, true);
            }

            Location3Priv = value;
        }
    }

    /// <summary>
    /// When enabled, moves the Vitals panel to a new location (<c>24.55</c>, <c>-4.780</c>).
    /// </summary>
    [ToggleOption]
    private static bool MoveVitals = false;

    /// <summary>
    /// When enabled, moves the Fuel task to a new location (<c>36.070</c>, <c>1.897</c>).
    /// </summary>
    [ToggleOption]
    private static bool MoveFuel = false;

    /// <summary>
    /// When enabled, moves the Divert task to a new location (<c>13.35</c>, <c>-1.659</c>).
    /// </summary>
    [ToggleOption]
    private static bool MoveDivert = false;

    /// <summary>
    /// Controls the Admin table location.<br/>
    /// Options: <c>DontMove</c>, <c>Cockpit</c>, <c>MainHall</c>
    /// </summary>
    [StringOption<MoveAdmin>]
    private static MoveAdmin MoveAdmin = MoveAdmin.DontMove;

    /// <summary>
    /// Controls the Electrical panel location.<br/>
    /// Options: <c>DontMove</c>, <c>Vault</c>, <c>Electrical</c>
    /// </summary>
    [StringOption<MoveElectrical>]
    private static MoveElectrical MoveElectrical = MoveElectrical.DontMove;

    /// <summary>
    /// Minimum time required for door swipe animations.
    /// </summary>
    /// <remarks>
    /// Default: <c>0.4</c>s<br/>
    /// Range: <c>0</c> to <c>10</c>s<br/>
    /// Increment: <c>0.1</c>s
    /// </remarks>
    [NumberOption(0f, 10f, 0.1f, Format.Time)]
    public static Number MinDoorSwipeTime = 0.4f;

    /// <summary>
    /// Time until crash code resets during sabotage.
    /// </summary>
    /// <remarks>
    /// Default: <c>10</c>s<br/>
    /// Range: <c>1</c> to <c>20</c>s<br/>
    /// Increment: <c>1</c>s
    /// </remarks>
    [NumberOption(30f, 100f, 5f, Format.Time)]
    private static Number CrashTimer = 90;

    /// <summary>
    /// Time until crash code resets during sabotage.
    /// </summary>
    /// <remarks>
    /// Default: <c>10</c> seconds<br/>
    /// Range: <c>1</c> to <c>20</c> seconds<br/>
    /// Increment: <c>1</c> seconds
    /// </remarks>
    [NumberOption(1f, 20f, 1f, Format.Time)]
    private static Number CrashCodeResetTimer = 10;

    /// <summary>
    /// Stores synchronized random spawn points for all players.
    /// </summary>
    public static readonly List<byte> SpawnPoints = [];

    /// <summary>
    /// Handles repositioning of various map elements when the Airship map loads.
    /// </summary>
    [HarmonyPatch(typeof(AirshipStatus), nameof(AirshipStatus.OnEnable))]
    public static class Repositioning
    {
        public static void Postfix()
        {
            if (!EnableBetterAirship)
                return;

            // Moving the admin table
            if (MoveAdmin != 0)
            {
                var adminTable = UObject.FindObjectOfType<MapConsole>();
                var mapFloating = GameObject.Find("Cockpit/cockpit_mapfloating");

                switch (MoveAdmin)
                {
                    case MoveAdmin.Cockpit:
                    {
                        adminTable.transform.SetPositionAndRotation(new(-17.269f, 1.375f, 0f), Quaternion.Euler(new(0, 0, 350.316f)));
                        adminTable.transform.localScale = new(1, 1, 1);

                        mapFloating.transform.SetPositionAndRotation(new(-17.736f, 2.36f, 0f), Quaternion.Euler(new(0, 0, 350)));
                        mapFloating.transform.localScale = new(1, 1, 1);
                        break;
                    }
                    case MoveAdmin.MainHall: // New Admin
                    {
                        adminTable.transform.SetPositionAndRotation(new(5.078f, 3.4f, 1), Quaternion.Euler(new(0, 0, 76.1f)));
                        adminTable.transform.localScale = new(1.200f, 1.700f, 1);

                        mapFloating.transform.localScale = new(0, 0, 0);
                        break;
                    }
                }
            }

            // Moving the electrical panel
            if (MoveElectrical != 0)
            {
                var electrical = GameObject.Find("GapRoom/task_lightssabotage (gap)");

                switch (MoveElectrical)
                {
                    case MoveElectrical.Vault:
                    {
                        electrical.transform.position = new(-8.818f, 13.184f, 0f);
                        electrical.transform.localScale = new(0.909f, 0.818f, 1);

                        var originalSupport = GameObject.Find("Vault/cockpit_comms");
                        var supportElectrical = UObject.Instantiate(originalSupport, originalSupport.transform);

                        supportElectrical.transform.position = new(-8.792f, 13.242f);
                        supportElectrical.transform.localScale = new(1, 1, 1);
                        break;
                    }
                    case MoveElectrical.Electrical:
                    {
                        electrical.transform.position = new(19.339f, -3.665f, 0f);
                        break;
                    }
                }
            }

            // Moving the vitals panel
            if (MoveVitals)
            {
                GameObject.Find("Medbay/panel_vitals").transform.position = new(24.55f, -4.780f, 0f);
                GameObject.Find("Medbay/panel_data").transform.position = new(25.240f, -7.938f, 0f);
            }

            // Moving the fuel task
            if (MoveFuel)
                GameObject.Find("Storage/task_gas").transform.position = new(36.070f, 1.897f, 0f);

            // Moving the divert task
            if (MoveDivert)
                GameObject.Find("HallwayMain/DivertRecieve").transform.position = new(13.35f, -1.659f, 0f);
        }
    }

    /// <summary>
    /// Manages custom spawn behavior and locations for players.<br/>
    /// Adds additional spawn points and implements different spawn modes.
    /// </summary>
    [HarmonyPatch(typeof(SpawnInMinigame), nameof(SpawnInMinigame.Begin))]
    public static class SpawnInMinigamePatch
    {
        public static bool Prefix(SpawnInMinigame __instance)
        {
            // Skip this if the local player has the Astral modifier or is a postmortal role
            if ((CustomPlayer.Local.IsPostmortal() && !CustomPlayer.Local.Caught()) || (CustomPlayer.Local.TryGetLayer<Astral>(out var astral) && astral.LastPosition != Vector3.zero))
            {
                __instance.Close();
                return false;
            }

            // Skip if better airship is disabled or if the map is submerged
            if (!EnableBetterAirship || IsSubmerged())
                return true;

            // If the spawn type is meeting, teleport the player to the meeting position
            if (SpawnType == AirshipSpawnType.Meeting)
            {
                __instance.Close();
                CustomPlayer.Local.RpcCustomSnapTo(GetMeetingPosition(CustomPlayer.Local.PlayerId));
                return false;
            }

            var spawn = __instance.Locations.ToArray(); // Getting an array of the default spawn locations
            var rollover = AddAsset("RolloverDefault", spawn[0].RolloverSfx); // Caching the default rollover sound effect

            // Adding custom spawn locations
            AddSpawn(new(-8.808f, 12.710f, 0.013f), StringNames.VaultRoom, GetSprite("Vault"), GetAnim("Vault"), rollover, ref spawn);
            AddSpawn(new(-19.278f, -1.033f, 0f), StringNames.Cockpit, GetSprite("Cockpit"), GetAnim("Cockpit"), rollover, ref spawn);
            AddSpawn(new(29.041f, -6.336f, 0f), StringNames.Medical, GetSprite("Medical"), GetAnim("Medical"), rollover, ref spawn);

            switch (SpawnType)
            {
                // If the spawn type is fixed, set the spawn locations specific (host-defined) locations | Done: Allow users to change fixed spawn locations
                case AirshipSpawnType.Fixed:
                {
                    __instance.Locations = new[] { spawn[(byte)Location1], spawn[(byte)Location2], spawn[(byte)Location3] };
                    break;
                }
                // Use the randomised locations set by the host
                case AirshipSpawnType.RandomSynchronized:
                {
                    try
                    {
                        __instance.Locations = new[] { spawn[SpawnPoints[0]], spawn[SpawnPoints[1]], spawn[SpawnPoints[2]] };
                    }
                    catch
                    {
                        __instance.Locations = new[] { spawn[3], spawn[2], spawn[5] };
                    }
                    break;
                }
                // Use the randomised locations chosen by the client
                case AirshipSpawnType.Random:
                {
                    __instance.Locations = spawn.GetRandomRange(3).ToArray();
                    break;
                }
            }

            return true;
        }

        /// <summary>
        /// Gets the meeting position for a player based on their ID.
        /// </summary>
        /// <param name="playerId">The ID of the player to reposition</param>
        /// <returns>A Vector3 position where the player should spawn during meetings</returns>
        /// <remarks>
        /// Positions are calculated in a grid pattern:<br/>
        /// - Players are arranged in two rows<br/>
        /// - X position is calculated based on total player count<br/>
        /// - Y position alternates between 16.0 and 14.4 based on odd/even player IDs
        /// </remarks>
        private static Vector3 GetMeetingPosition(byte playerId)
        {
            var position = new Vector3(9f, 16f, 0);

            var yIndex = playerId % 2;
            var xIndex = (playerId - yIndex) / 2;

            position.x += xIndex * 8f / GameData.Instance.PlayerCount;

            if (yIndex == 1)
                position.y = 14.4f;

            return position;
        }

        /// <summary>
        /// Adds a new spawn location to the available spawn points array.
        /// </summary>
        /// <param name="location">The Vector3 position where players can spawn</param>
        /// <param name="name">The display name of the spawn location</param>
        /// <param name="sprite">The sprite to display for this spawn point</param>
        /// <param name="rollover">The animation to play when hovering over this spawn point</param>
        /// <param name="sfx">The sound effect to play when selecting this spawn point</param>
        /// <param name="array">Reference to the array of spawn locations to modify</param>
        private static void AddSpawn(Vector3 location, StringNames name, Sprite sprite, AnimationClip rollover, AudioClip sfx, ref SpawnInMinigame.SpawnLocation[] array)
        {
            Array.Resize(ref array, array.Length + 1);
            array[^1] = new()
            {
                Location = location,
                Name = name,
                Image = sprite,
                Rollover = rollover,
                RolloverSfx = sfx
            };
        }
    }

    /// <summary>
    /// Modifies the helicopter crash sabotage system timers and behavior.
    /// </summary>
    [HarmonyPatch(typeof(HeliSabotageSystem), nameof(HeliSabotageSystem.UpdateSystem))]
    public static class HeliCountdownPatch
    {
        public static bool Prefix(HeliSabotageSystem __instance, PlayerControl player, MessageReader msgReader)
        {
            // Skip modifications if disabled or not on the Airship map (map ID 4)
            if (!EnableBetterAirship || MapPatches.CurrentMap != 4)
                return true;

            // Read operation type and console ID from the network message
            var b = msgReader.ReadByte();
            var b2 = (byte)(b & 15); // Extract console ID (bits 0-3)

            // Handle different sabotage system operations based on tag type (bits 4-7)
            switch ((HeliSabotageSystem.Tags)(b & 240))
            {
                case HeliSabotageSystem.Tags.FixBit:
                {
                    // Reset code timer when a console is fixed
                    __instance.codeResetTimer = CrashCodeResetTimer;
                    __instance.CompletedConsoles.Add(b2);
                    break;
                }
                case HeliSabotageSystem.Tags.DeactiveBit:
                {
                    // Remove player from active console users
                    __instance.ActiveConsoles.Remove(new(player.PlayerId, b2));
                    break;
                }
                case HeliSabotageSystem.Tags.ActiveBit:
                {
                    // Add player to active console users
                    __instance.ActiveConsoles.Add(new(player.PlayerId, b2));
                    break;
                }
                case HeliSabotageSystem.Tags.DamageBit:
                {
                    // Initialize the crash sequence with a custom timer
                    __instance.codeResetTimer = -1f;
                    __instance.Countdown = CrashTimer;
                    __instance.CompletedConsoles.Clear();
                    __instance.ActiveConsoles.Clear();
                    break;
                }
            }

            // Mark the system as needing network sync
            __instance.IsDirty = true;
            return false; // Prevent original method execution
        }
    }
}