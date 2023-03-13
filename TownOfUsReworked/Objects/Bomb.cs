using System.Collections;
using Il2CppSystem.Collections.Generic;
using TownOfUsReworked.CustomOptions;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.Objects
{
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

            Players = Utils.GetClosestPlayers(Transform.position, (CustomGameOptions.BombRange + (Role.SyndicateHasChaosDrive ? CustomGameOptions.ChaosDriveBombRange : 0f)));
        }
    }
}