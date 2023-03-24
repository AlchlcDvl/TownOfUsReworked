using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.BetterMaps.Airship
{
    public static class SpawnInMinigamePatch
    {
        private static bool GameStarted;

        #pragma warning disable
        public static List<byte> SpawnPoints = new();
        #pragma warning restore

        [HarmonyPatch(typeof(SpawnInMinigame), nameof(SpawnInMinigame.Begin))]
        public static class SpawnInMiningameBeginPatch
        {
            public static bool Prefix(SpawnInMinigame __instance)
            {
                if (CustomGameOptions.MeetingSpawnChoice || !GameStarted)
                {
                    GameStarted = true;
                    var Spawn = __instance.Locations.ToArray();

                    if (CustomGameOptions.NewSpawns)
                    {
                        Spawn = AddSpawn(Location: new Vector3(-8.808f, 12.710f, 0.013f), name: StringNames.VaultRoom, Sprite: AssetManager.Vault, Rollover: AssetManager.VaultAnim,
                            RolloverSfx: Spawn[0].RolloverSfx, array: Spawn);
                        Spawn = AddSpawn(Location: new Vector3(-19.278f, -1.033f, 0), name: StringNames.Cockpit, Sprite: AssetManager.Cokpit, Rollover: AssetManager.CokpitAnim,
                            RolloverSfx: Spawn[0].RolloverSfx, array: Spawn);
                        Spawn = AddSpawn(Location: new Vector3(29.041f, -6.336f, 0), name: StringNames.Medical, Sprite: AssetManager.Medical, Rollover: AssetManager.MedicalAnim,
                            RolloverSfx: Spawn[0].RolloverSfx, array: Spawn);

                        __instance.Locations = Spawn;
                    }

                    if ((byte)CustomGameOptions.SpawnType != 0)
                    {
                        if ((byte)CustomGameOptions.SpawnType == 1)
                            __instance.Locations = new SpawnInMinigame.SpawnLocation[3] {CustomGameOptions.NewSpawns ? Spawn[7] : Spawn[3], Spawn[2], Spawn[5]};
                        else if ((byte)CustomGameOptions.SpawnType == 2)
                            __instance.Locations = new SpawnInMinigame.SpawnLocation[3] {Spawn[SpawnPoints[0]], Spawn[SpawnPoints[1]], Spawn[SpawnPoints[2]]};
                    }

                    return true;
                }

                __instance.Close();
                PlayerControl.LocalPlayer.moveable = true;
                PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(GetMeetingPosition(PlayerControl.LocalPlayer.PlayerId));
                return false;
            }

            public static SpawnInMinigame.SpawnLocation[] AddSpawn(Vector3 Location, StringNames name, Sprite Sprite, AnimationClip Rollover, AudioClip RolloverSfx,
                SpawnInMinigame.SpawnLocation[] array)
            {
                Array.Resize(ref array, array.Length + 1);

                array[^1] = new SpawnInMinigame.SpawnLocation
                {
                    Location = Location,
                    Name = name,
                    Image = Sprite,
                    Rollover = Rollover,
                    RolloverSfx = RolloverSfx
                };

                return array;
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
            public static void Postfix() => GameStarted = false;
        }

        public static void ResetGlobalVariable() => GameStarted = false;
    }
}