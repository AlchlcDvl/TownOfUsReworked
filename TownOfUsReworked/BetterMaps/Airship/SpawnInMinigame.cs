namespace TownOfUsReworked.BetterMaps.Airship
{
    [HarmonyPatch]
    public static class SpawnInMinigamePatch
    {
        private static bool GameStarted;
        public readonly static List<byte> SpawnPoints = new();

        [HarmonyPatch(typeof(SpawnInMinigame), nameof(SpawnInMinigame.Begin))]
        public static class SpawnInMiningameBeginPatch
        {
            public static bool Prefix(SpawnInMinigame __instance)
            {
                if (PlayerControl.LocalPlayer.IsPostmortal() && !PlayerControl.LocalPlayer.Caught())
                {
                    __instance.Close();
                    return false;
                }

                if (TownOfUsReworked.MCIActive)
                {
                    foreach (var player in PlayerControl.AllPlayerControls)
                    {
                        if (!player.Data.PlayerName.Contains("Robot"))
                            continue;

                        var rand = new SRandom().Next(0, __instance.Locations.Count);
                        player.gameObject.SetActive(true);
                        player.NetTransform.RpcSnapTo(__instance.Locations[rand].Location);
                    }
                }

                if (CustomGameOptions.MeetingSpawnChoice || !GameStarted)
                {
                    GameStarted = true;
                    var Spawn = __instance.Locations.ToArray();
                    SpawnPoints.Clear();

                    if (AmongUsClient.Instance.AmHost)
                    {
                        var random = (byte)URandom.RandomRangeInt(0, Spawn.Length);
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetSpawnAirship, SendOption.Reliable);
                        writer.Write(PlayerControl.LocalPlayer.PlayerId);

                        while (SpawnPoints.Count < 3)
                        {
                            random = (byte)URandom.RandomRangeInt(0, Spawn.Length);

                            if (!SpawnPoints.Contains(random))
                                SpawnPoints.Add(random);
                        }

                        writer.WriteBytesAndSize(SpawnPoints.ToArray());
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }

                    if (CustomGameOptions.SpawnType == AirshipSpawnType.Fixed)
                        __instance.Locations = new[] { Spawn[3], Spawn[2], Spawn[5] };
                    else if (CustomGameOptions.SpawnType == AirshipSpawnType.RandomSynchronized)
                        __instance.Locations = new[] { Spawn[SpawnPoints[0]], Spawn[SpawnPoints[1]], Spawn[SpawnPoints[2]] };

                    return true;
                }

                __instance.Close();
                PlayerControl.LocalPlayer.moveable = true;
                PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(GetMeetingPosition(PlayerControl.LocalPlayer.PlayerId));
                return false;
            }

            public static Vector3 GetMeetingPosition(byte PlayerId)
            {
                var halfPlayerValue = (int) Mathf.Round(PlayerControl.AllPlayerControls.Count / 2);
                var Position = new Vector3(9f, 16f, 0);

                var xIndex = (PlayerId - (PlayerId % 2)) / 2;
                var yIndex = PlayerId % 2;

                var marge = (13f - 9f) / halfPlayerValue;
                Position.x += marge * xIndex;

                if (yIndex == 1)
                    Position.y = 14.4f;

                return Position;
            }
        }

        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Start))]
        static class GameEndedPatch
        {
            public static void Postfix() => ResetGlobalVariable();
        }

        public static void ResetGlobalVariable() => GameStarted = false;
    }
}