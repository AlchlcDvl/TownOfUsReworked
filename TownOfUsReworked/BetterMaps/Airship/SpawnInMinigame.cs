namespace TownOfUsReworked.BetterMaps.Airship
{
    [HarmonyPatch]
    public static class SpawnInMinigamePatch
    {
        private static bool GameStarted;
        public static readonly List<byte> SpawnPoints = new();

        [HarmonyPatch(typeof(SpawnInMinigame), nameof(SpawnInMinigame.Begin))]
        public static class SpawnInMiningameBeginPatch
        {
            public static bool Prefix(SpawnInMinigame __instance)
            {
                if ((CustomPlayer.Local.IsPostmortal() && !CustomPlayer.Local.Caught()) || (CustomPlayer.Local.Is(ModifierEnum.Astral) &&
                    Modifier.GetModifier<Astral>(CustomPlayer.Local).LastPosition != Vector3.zero))
                {
                    __instance.Close();
                    return false;
                }

                if (TownOfUsReworked.MCIActive)
                {
                    foreach (var player in CustomPlayer.AllPlayers)
                    {
                        if (!player.Data.PlayerName.Contains("Robot"))
                            continue;

                        var rand = URandom.Range(0, __instance.Locations.Count);
                        player.gameObject.SetActive(true);
                        player.NetTransform.RpcSnapTo(__instance.Locations[rand].Location);
                    }
                }

                if (!GameStarted && CustomGameOptions.SpawnType != AirshipSpawnType.Meeting)
                {
                    GameStarted = true;
                    var spawn = __instance.Locations.ToArray();
                    SpawnPoints.Clear();

                    if (AmongUsClient.Instance.AmHost)
                    {
                        var random = (byte)URandom.RandomRangeInt(0, spawn.Length);

                        while (SpawnPoints.Count < 3)
                        {
                            random = (byte)URandom.RandomRangeInt(0, spawn.Length);

                            if (!SpawnPoints.Contains(random))
                                SpawnPoints.Add(random);
                        }

                        CallRpc(CustomRPC.Misc, MiscRPC.SetSpawnAirship, SpawnPoints.ToArray());
                    }

                    if (CustomGameOptions.SpawnType == AirshipSpawnType.Fixed)
                        __instance.Locations = new[] { spawn[3], spawn[2], spawn[5] };
                    else if (CustomGameOptions.SpawnType == AirshipSpawnType.RandomSynchronized)
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

                    return true;
                }

                __instance.Close();
                CustomPlayer.Local.moveable = true;
                CustomPlayer.Local.NetTransform.RpcSnapTo(GetMeetingPosition(CustomPlayer.Local.PlayerId));
                return false;
            }

            public static Vector3 GetMeetingPosition(byte PlayerId)
            {
                var halfPlayerValue = (int) Mathf.Round(CustomPlayer.AllPlayers.Count / 2);
                var position = new Vector3(9f, 16f, 0);

                var xIndex = (PlayerId - (PlayerId % 2)) / 2;
                var yIndex = PlayerId % 2;

                var marge = (13f - 9f) / halfPlayerValue;
                position.x += marge * xIndex;

                if (yIndex == 1)
                    position.y = 14.4f;

                return position;
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