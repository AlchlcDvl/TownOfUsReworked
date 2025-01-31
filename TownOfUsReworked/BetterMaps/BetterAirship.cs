namespace TownOfUsReworked.BetterMaps;

[HeaderOption(MultiMenu.Main)]
public static class BetterAirship
{
    [ToggleOption]
    public static bool EnableBetterAirship = true;

    [StringOption<AirshipSpawnType>]
    public static AirshipSpawnType SpawnType = AirshipSpawnType.Normal;

    [ToggleOption]
    public static bool MoveVitals = false;

    [ToggleOption]
    public static bool MoveFuel = false;

    [ToggleOption]
    public static bool MoveDivert = false;

    [StringOption<MoveAdmin>]
    public static MoveAdmin MoveAdmin = MoveAdmin.DontMove;

    [StringOption<MoveElectrical>]
    public static MoveElectrical MoveElectrical = MoveElectrical.DontMove;

    [NumberOption(0f, 10f, 0.1f)]
    public static Number MinDoorSwipeTime = 0.4f;

    [NumberOption(30f, 100f, 5f, Format.Time)]
    public static Number CrashTimer = 90;

    [NumberOption(1f, 20f, 1f, Format.Time)]
    public static Number CrashCodeResetTimer = 10;

    [ToggleOption]
    public static bool EnableCustomSpawns = true;

    public static readonly List<byte> SpawnPoints = [];

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

                if ((int)MoveAdmin == 1)
                {
                    adminTable.transform.position = new(-17.269f, 1.375f, 0f);
                    adminTable.transform.rotation = Quaternion.Euler(new(0, 0, 350.316f));
                    adminTable.transform.localScale = new(1, 1, 1);

                    mapFloating.transform.position = new(-17.736f, 2.36f, 0f);
                    mapFloating.transform.rotation = Quaternion.Euler(new(0, 0, 350));
                    mapFloating.transform.localScale = new(1, 1, 1);
                }
                else if ((int)MoveAdmin == 2)
                {
                    // New Admin
                    adminTable.transform.position = new(5.078f, 3.4f, 1);
                    adminTable.transform.rotation = Quaternion.Euler(new(0, 0, 76.1f));
                    adminTable.transform.localScale = new(1.200f, 1.700f, 1);
                    mapFloating.transform.localScale = new(0, 0, 0);
                }
            }

            // Moving the electrical panel
            if (MoveElectrical != 0)
            {
                var electrical = GameObject.Find("GapRoom/task_lightssabotage (gap)");

                if ((int)MoveElectrical == 1)
                {
                    electrical.transform.position = new(-8.818f, 13.184f, 0f);
                    electrical.transform.localScale = new(0.909f, 0.818f, 1);

                    var originalSupport = GameObject.Find("Vault/cockpit_comms");
                    var supportElectrical = UObject.Instantiate(originalSupport, originalSupport.transform);

                    supportElectrical.transform.position = new(-8.792f, 13.242f);
                    supportElectrical.transform.localScale = new(1, 1, 1);
                }
                else if ((int)MoveElectrical == 2)
                    electrical.transform.position = new(19.339f, -3.665f, 0f);
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
                CustomPlayer.Local.moveable = true;
                CustomPlayer.Local.RpcCustomSnapTo(GetMeetingPosition(CustomPlayer.Local.PlayerId));
                return false;
            }

            var spawn = __instance.Locations.ToArray(); // Getting an array of the default spawn locations
            AddAsset("RolloverDefault", spawn[0].RolloverSfx); // Caching the default rollover sound effect

            // Adding custom spawn locations
            if (EnableCustomSpawns)
            {
                AddSpawn(new Vector3(-8.808f, 12.710f, 0.013f), StringNames.VaultRoom, GetSprite("Vault"), GetAnim("Vault"), GetAudio("RolloverDefault"), ref spawn);
                AddSpawn(new Vector3(-19.278f, -1.033f, 0f), StringNames.Cockpit, GetSprite("Cockpit"), GetAnim("Cockpit"), GetAudio("RolloverDefault"), ref spawn);
                AddSpawn(new Vector3(29.041f, -6.336f, 0f), StringNames.Medical, GetSprite("Medical"), GetAnim("Medical"), GetAudio("RolloverDefault"), ref spawn);
            }

            if (SpawnType == AirshipSpawnType.Fixed) // If the spawn type is fixed, set the spawn locations specific locations | TODO: Allow users to change fixed spawn locations
                __instance.Locations = new[] { spawn[3], spawn[2], spawn[5] };
            else if (SpawnType == AirshipSpawnType.RandomSynchronized) // Use the randomised locations set by the host
            {
                try
                {
                    __instance.Locations = new[] { spawn[SpawnPoints[0]], spawn[SpawnPoints[1]], spawn[SpawnPoints[2]] };
                }
                catch
                {
                    __instance.Locations = new[] { spawn[3], spawn[2], spawn[5] };
                }
            }
            else if (SpawnType == AirshipSpawnType.Random) // Use the randomised locations chosen by the client
                __instance.Locations = spawn.GetRandomRange(3).ToArray();

            return true;
        }

        // Getting a meeting position based on the player id
        private static Vector3 GetMeetingPosition(byte playerId)
        {
            var position = new Vector3(9f, 16f, 0);

            var yIndex = playerId % 2;
            var xIndex = (playerId - yIndex) / 2;

            position.x += xIndex * (13f - 9f) * 2 / GameData.Instance.PlayerCount;

            if (yIndex == 1)
                position.y = 14.4f;

            return position;
        }

        // Adding a new spawn location, a util for adding custom spawn locations
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

    [HarmonyPatch(typeof(HeliSabotageSystem), nameof(HeliSabotageSystem.UpdateSystem))]
    public static class HeliCountdownPatch
    {
        // Setting the timers to the crash, best method I could do was to completely replace the original method
        public static bool Prefix(HeliSabotageSystem __instance, PlayerControl player, MessageReader msgReader)
        {
            if (!EnableBetterAirship || MapPatches.CurrentMap != 4)
                return true;

            var b = msgReader.ReadByte();
            var b2 = (byte)(b & 15);
            var tags = (HeliSabotageSystem.Tags)(b & 240);

            if (tags == HeliSabotageSystem.Tags.FixBit)
            {
                __instance.codeResetTimer = CrashCodeResetTimer;
                __instance.CompletedConsoles.Add(b2);
            }
            else if (tags == HeliSabotageSystem.Tags.DeactiveBit)
                __instance.ActiveConsoles.Remove(new(player.PlayerId, b2));
            else if (tags == HeliSabotageSystem.Tags.ActiveBit)
                __instance.ActiveConsoles.Add(new(player.PlayerId, b2));
            else if (tags == HeliSabotageSystem.Tags.DamageBit)
            {
                __instance.codeResetTimer = -1f;
                __instance.Countdown = CrashTimer;
                __instance.CompletedConsoles.Clear();
                __instance.ActiveConsoles.Clear();
            }

            __instance.IsDirty = true;
            return false;
        }
    }
}