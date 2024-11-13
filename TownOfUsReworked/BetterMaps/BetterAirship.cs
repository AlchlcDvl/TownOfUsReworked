namespace TownOfUsReworked.BetterMaps;

[HeaderOption(MultiMenu.Main)]
public static class BetterAirship
{
    [ToggleOption(MultiMenu.Main)]
    public static bool EnableBetterAirship { get; set; } = true;

    [StringOption(MultiMenu.Main)]
    public static AirshipSpawnType SpawnType { get; set; } = AirshipSpawnType.Normal;

    [ToggleOption(MultiMenu.Main)]
    public static bool MoveVitals { get; set; } = false;

    [ToggleOption(MultiMenu.Main)]
    public static bool MoveFuel { get; set; } = false;

    [ToggleOption(MultiMenu.Main)]
    public static bool MoveDivert { get; set; } = false;

    [StringOption(MultiMenu.Main)]
    public static MoveAdmin MoveAdmin { get; set; } = MoveAdmin.DontMove;

    [StringOption(MultiMenu.Main)]
    public static MoveElectrical MoveElectrical { get; set; } = MoveElectrical.DontMove;

    [NumberOption(MultiMenu.Main, 0f, 10f, 0.1f)]
    public static Number MinDoorSwipeTime { get; set; } = new(0.4f);

    [NumberOption(MultiMenu.Main, 30f, 100f, 5f, Format.Time)]
    public static Number CrashTimer { get; set; } = new(90);

    [ToggleOption(MultiMenu.Main)]
    public static bool EnableCustomSpawns { get; set; } = true;

    public static readonly List<byte> SpawnPoints = [];

    [HarmonyPatch(typeof(AirshipStatus), nameof(AirshipStatus.OnEnable))]
    public static class Repositioning
    {
        public static void Postfix()
        {
            if (!EnableBetterAirship)
                return;

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

            if (MoveVitals)
            {
                GameObject.Find("Medbay/panel_vitals").transform.position = new(24.55f, -4.780f, 0f);
                GameObject.Find("Medbay/panel_data").transform.position = new(25.240f, -7.938f, 0f);
            }

            if (MoveFuel)
                GameObject.Find("Storage/task_gas").transform.position = new(36.070f, 1.897f, 0f);

            if (MoveDivert)
                GameObject.Find("HallwayMain/DivertRecieve").transform.position = new(13.35f, -1.659f, 0f);
        }
    }

    [HarmonyPatch(typeof(SpawnInMinigame), nameof(SpawnInMinigame.Begin))]
    public static class SpawnInMinigamePatch
    {
        public static bool Prefix(SpawnInMinigame __instance)
        {
            if (TownOfUsReworked.MCIActive)
            {
                foreach (var player in AllPlayers())
                {
                    if (player.Data.PlayerName.Contains("Bot"))
                        player.RpcCustomSnapTo(__instance.Locations.Random().Location);
                }
            }

            if ((CustomPlayer.Local.IsPostmortal() && !CustomPlayer.Local.Caught()) || (CustomPlayer.Local.TryGetLayer<Astral>(out var astral) && astral.LastPosition != Vector3.zero))
            {
                __instance.Close();
                return false;
            }

            if (!EnableBetterAirship || IsSubmerged())
                return true;

            if (SpawnType == AirshipSpawnType.Meeting)
            {
                __instance.Close();
                CustomPlayer.Local.moveable = true;
                CustomPlayer.Local.RpcCustomSnapTo(GetMeetingPosition(CustomPlayer.Local.PlayerId));
                return false;
            }

            var spawn = __instance.Locations.ToArray();
            AddAsset("RolloverDefault", spawn[0].RolloverSfx);

            if (EnableCustomSpawns)
            {
                AddSpawn(new Vector3(-8.808f, 12.710f, 0.013f), StringNames.VaultRoom, GetSprite("Vault"), GetAnim("Vault"), GetAudio("RolloverDefault"), ref spawn);
                AddSpawn(new Vector3(-19.278f, -1.033f, 0f), StringNames.Cockpit, GetSprite("Cokpit"), GetAnim("Cokpit"), GetAudio("RolloverDefault"), ref spawn);
                AddSpawn(new Vector3(29.041f, -6.336f, 0f), StringNames.Medical, GetSprite("Medical"), GetAnim("Medical"), GetAudio("RolloverDefault"), ref spawn);
            }

            if (SpawnType == AirshipSpawnType.Fixed)
                __instance.Locations = new[] { spawn[3], spawn[2], spawn[5] };
            else if (SpawnType == AirshipSpawnType.RandomSynchronized)
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
            else if (SpawnType == AirshipSpawnType.Random)
                __instance.Locations = spawn.GetRandomRange(3).ToArray();

            return true;
        }

        public static Vector3 GetMeetingPosition(byte playerId)
        {
            var position = new Vector3(9f, 16f, 0);

            var yIndex = playerId % 2;
            var xIndex = (playerId - yIndex) / 2;

            position.x += xIndex * (13f - 9f) * 2 / AllPlayers().Count;

            if (yIndex == 1)
                position.y = 14.4f;

            return position;
        }

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
        public static bool Prefix(HeliSabotageSystem __instance, PlayerControl player, MessageReader msgReader)
        {
            if (!EnableBetterAirship || MapPatches.CurrentMap != 4)
                return true;

            var b = msgReader.ReadByte();
            var b2 = (byte)(b & 15);
            var tags = (HeliSabotageSystem.Tags)(b & 240);

            if (tags == HeliSabotageSystem.Tags.FixBit)
            {
                __instance.codeResetTimer = 10f;
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