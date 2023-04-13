using System.Collections;
using Il2CppSystem.Collections.Generic;
using TownOfUsReworked.CustomOptions;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.Classes;
using HarmonyLib;
using TownOfUsReworked.Data;
using Reactor.Utilities;

namespace TownOfUsReworked.Objects
{
    [HarmonyPatch]
    public class Bomb
    {
        public List<PlayerControl> Players;
        public Transform Transform;
        public bool Drived;

        public IEnumerator BombTimer()
        {
            while (Transform != null)
            {
                yield return 0;
                Update();
            }
        }

        public void Update()
        {
            if (Transform == null)
                return;

            Players = Utils.GetClosestPlayers(Transform.position, CustomGameOptions.BombRange + (Drived ? CustomGameOptions.ChaosDriveBombRange : 0f) + 0.05f);
        }

        public void Detonate()
        {
            foreach (var player in Players)
                Utils.RpcMurderPlayer(player, player, DeathReasonEnum.Bombed, false);

            Stop();
        }

        public void Stop()
        {
            Object.Destroy(Transform.gameObject);
            Coroutines.Stop(BombTimer());
        }
    }
}