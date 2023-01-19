using HarmonyLib;
using Hazel;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TownOfUsReworked.Patches;

namespace TownOfUsReworked.BetterMaps.Airship
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
    class HandleRpcPatch
    {
        public static bool Prefix([HarmonyArgument(0)] byte CallId, [HarmonyArgument(1)] MessageReader reader)
        {
            if (CallId == (byte)CustomRPC.SetSpawnAirship)
            {
                List<byte> spawnPoints = reader.ReadBytesAndSize().ToList();
                SpawnInMinigamePatch.SpawnPoints = spawnPoints;

                return false;
            }

            if (CallId == (byte)CustomRPC.DoorSyncToilet)
            {
                int Id = reader.ReadInt32();

                PlainDoor DoorToSync = Object.FindObjectsOfType<PlainDoor>().FirstOrDefault(door => door.Id == Id);
                DoorToSync.SetDoorway(true);

                return false;
            }

            unchecked
            {
                if (CallId == (byte)CustomRPC.SyncPlateform)
                {
                    bool isLeft = reader.ReadBoolean();
                    CallPlateform.SyncPlateform(isLeft);

                    return false;
                }
            }

            return true;
        }
    }
}