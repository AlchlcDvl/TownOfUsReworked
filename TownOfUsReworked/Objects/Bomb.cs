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

            Players = Utils.GetClosestPlayers(Transform.position, CustomGameOptions.BombRange + (Role.SyndicateHasChaosDrive ? CustomGameOptions.ChaosDriveBombRange : 0f));
        }

        public void Detonate(string name)
        {
            foreach (var player in Players)
            {
                Utils.RpcMurderPlayer(player, player, false);
                var targetRole = Role.GetRole(player);
                targetRole.KilledBy = " By " + name;
                targetRole.DeathReason = DeathReasonEnum.Killed;
            }

            Stop();
        }

        public void Stop()
        {
            Object.Destroy(Transform.gameObject);
            Coroutines.Stop(BombTimer());
        }
    }
}