using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.BetterMaps.Airship
{
    class SpawnInMinigamePatch
    {
        public static bool GameStarted = false;
        public static List<byte> SpawnPoints = new List<byte>();

        [HarmonyPatch(typeof(SpawnInMinigame), nameof(SpawnInMinigame.Begin))]
        class SpawnInMiningameBeginPatch
        {
            static bool Prefix(SpawnInMinigame __instance)
            {
                if (CustomGameOptions.MeetingSpawnChoice || !GameStarted)
                {
                    GameStarted = true;
                    var Spawn = __instance.Locations.ToArray();

                    if (CustomGameOptions.NewSpawns)
                    {
                        Spawn = AddSpawn(Location: new Vector3(-8.808f, 12.710f, 0.013f), name: StringNames.VaultRoom,
                            Sprite: TownOfUsReworked.VaultSprite, Rollover: TownOfUsReworked.VaultAnim, RolloverSfx: Spawn[0].RolloverSfx, array: Spawn);
                        Spawn = AddSpawn(Location: new Vector3(-19.278f, -1.033f, 0), name: StringNames.Cockpit,
                            Sprite: TownOfUsReworked.CokpitSprite, Rollover: TownOfUsReworked.CokpitAnim, RolloverSfx: Spawn[0].RolloverSfx, array: Spawn);
                        Spawn = AddSpawn(Location: new Vector3(29.041f, -6.336f, 0), name: StringNames.Medical,
                            Sprite: TownOfUsReworked.MedicalSprite, Rollover: TownOfUsReworked.MedicalAnim, RolloverSfx: Spawn[0].RolloverSfx, array: Spawn);

                        __instance.Locations = Spawn;
                    }

                    if ((byte)CustomGameOptions.SpawnType != 0)
                    {
                        if ((byte)CustomGameOptions.SpawnType == 1)
                        {
                            __instance.Locations = new SpawnInMinigame.SpawnLocation[3] {CustomGameOptions.NewSpawns ? Spawn[7] : Spawn[3],
                                Spawn[2], Spawn[5]};
                        }
                        else if ((byte)CustomGameOptions.SpawnType == 2)
                        {
                            __instance.Locations = new SpawnInMinigame.SpawnLocation[3] {Spawn[SpawnPoints[0]], Spawn[SpawnPoints[1]],
                                Spawn[SpawnPoints[2]]};
                        }
                    }

                    return true;
                }

                __instance.Close();
                PlayerControl.LocalPlayer.moveable = true;
                PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(GetMeetingPosition(PlayerControl.LocalPlayer.PlayerId));
                return false;
            }

            public static SpawnInMinigame.SpawnLocation[] AddSpawn(Vector3 Location, StringNames name, Sprite Sprite, AnimationClip Rollover,
                AudioClip RolloverSfx, SpawnInMinigame.SpawnLocation[] array)
            {
                Array.Resize(ref array, array.Length + 1);

                array[array.Length - 1] = new SpawnInMinigame.SpawnLocation
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
                int halfPlayerValue = (int) Mathf.Round(PlayerControl.AllPlayerControls.Count / 2);
                Vector3 Position = new Vector3(9f, 16f, 0);

                float xIndex = ((PlayerId - (PlayerId % 2)) / 2);
                float yIndex = (PlayerId % 2);

                float marge = (13f - 9f) / halfPlayerValue;
                Position.x += marge * xIndex;

                if (yIndex == 1)
                    Position.y = 14.4f;

                return Position;
            }
        }
    }
}
