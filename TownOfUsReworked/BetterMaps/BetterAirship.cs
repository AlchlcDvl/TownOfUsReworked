namespace TownOfUsReworked.BetterMaps;

[HarmonyPatch(typeof(AirshipStatus), nameof(AirshipStatus.OnEnable))]
static class Repositioning
{
    public static void Postfix()
    {
        var adminTable = UObject.FindObjectOfType<MapConsole>();

        if (CustomGameOptions.MoveAdmin != 0)
        {
            var mapFloating = GameObject.Find("Cockpit/cockpit_mapfloating");

            if ((int)CustomGameOptions.MoveAdmin == 1)
            {
                adminTable.transform.position = new(-17.269f, 1.375f);
                adminTable.transform.rotation = Quaternion.Euler(new(0, 0, 350.316f));
                adminTable.transform.localScale = new(1, 1, 1);

                mapFloating.transform.position = new(-17.736f, 2.36f);
                mapFloating.transform.rotation = Quaternion.Euler(new(0, 0, 350));
                mapFloating.transform.localScale = new(1, 1, 1);
            }
            else if ((int)CustomGameOptions.MoveAdmin == 2)
            {
                //New Admin
                adminTable.transform.position = new(5.078f, 3.4f, 1);
                adminTable.transform.rotation = Quaternion.Euler(new(0, 0, 76.1f));
                adminTable.transform.localScale = new(1.200f, 1.700f, 1);
                mapFloating.transform.localScale = new(0, 0, 0);
            }
        }

        if (CustomGameOptions.MoveElectrical != 0)
        {
            var electrical = GameObject.Find("GapRoom/task_lightssabotage (gap)");

            if ((int)CustomGameOptions.MoveElectrical == 1)
            {
                electrical.transform.position = new(-8.818f, 13.184f);
                electrical.transform.localScale = new(0.909f, 0.818f, 1);

                var originalSupport = GameObject.Find("Vault/cockpit_comms");
                var supportElectrical = UObject.Instantiate(originalSupport, originalSupport.transform);

                supportElectrical.transform.position = new(-8.792f, 13.242f);
                supportElectrical.transform.localScale = new(1, 1, 1);
            }
            else if ((int)CustomGameOptions.MoveElectrical == 2)
                electrical.transform.position = new(19.339f, -3.665f);
        }

        if (CustomGameOptions.MoveVitals)
        {
            GameObject.Find("Medbay/panel_vitals").transform.position = new(24.55f, -4.780f);
            GameObject.Find("Medbay/panel_data").transform.position = new(25.240f, -7.938f);
        }

        if (CustomGameOptions.MoveFuel)
            GameObject.Find("Storage/task_gas").transform.position = new(36.070f, 1.897f);

        if (CustomGameOptions.MoveDivert)
            GameObject.Find("HallwayMain/DivertRecieve").transform.position = new(13.35f, -1.659f);
    }
}

[HarmonyPatch(typeof(SpawnInMinigame), nameof(SpawnInMinigame.Begin))]
public static class SpawnInMinigamePatch
{
    public static bool GameStarted;
    public static readonly List<byte> SpawnPoints = new();

    public static bool Prefix(SpawnInMinigame __instance)
    {
        if ((CustomPlayer.Local.IsPostmortal() && !CustomPlayer.Local.Caught()) || (CustomPlayer.Local.Is(LayerEnum.Astral) && Modifier.GetModifier<Astral>(CustomPlayer.Local).LastPosition
            != Vector3.zero))
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

    public static Vector3 GetMeetingPosition(byte playerId)
    {
        var halfPlayerValue = (int)Mathf.Round(CustomPlayer.AllPlayers.Count / 2);
        var position = new Vector3(9f, 16f, 0);

        var xIndex = (playerId - (playerId % 2)) / 2;
        var yIndex = playerId % 2;

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
    public static void Prefix() => SpawnInMinigamePatch.GameStarted = false;
}

[HarmonyPatch(typeof(HeliSabotageSystem), nameof(HeliSabotageSystem.RepairDamage))]
public static class HeliCountdownPatch
{
    public static bool Prefix(HeliSabotageSystem __instance, ref byte amount)
    {
        if ((HeliSabotageSystem.Tags)(amount & 240) == HeliSabotageSystem.Tags.DamageBit)
        {
            __instance.Countdown = CustomGameOptions.CrashTimer;
            HeliSabotageSystem.CharlesDuration = CustomGameOptions.CrashTimer;
            __instance.CompletedConsoles.Clear();
            __instance.ActiveConsoles.Clear();
            __instance.codeResetTimer = -1f;
            __instance.IsDirty = true;
            return false;
        }

        return true;
    }
}