using System.Collections;
using System.Collections.Generic;
using TownOfUsReworked.CustomOptions;
using UnityEngine;
using TownOfUsReworked.Classes;
using HarmonyLib;
using TownOfUsReworked.Data;
using Reactor.Utilities;
using System.Linq;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.Objects
{
    [HarmonyPatch]
    public class Bomb : Range
    {
        public List<PlayerControl> Players;
        public bool Drived;

        public Bomb(Vector2 position, bool drived) : base(position, Colors.Bomber, CustomGameOptions.BombRange + (drived ? CustomGameOptions.ChaosDriveBombRange : 0f), "Bomb")
        {
            Drived = drived;
            Coroutines.Start(Timer());
        }

        public override IEnumerator Timer()
        {
            while (Transform != null)
            {
                yield return 0;
                Update();
            }
        }

        public override void Update()
        {
            if (Transform == null)
                return;

            Players = Utils.GetClosestPlayers(Transform.position, CustomGameOptions.BombRange + (Drived ? CustomGameOptions.ChaosDriveBombRange : 0f)).Il2CppToSystem();
        }

        public void Detonate()
        {
            foreach (var player in Players)
            {
                if (!player.Is(RoleEnum.Pestilence) && !player.IsOnAlert() && !player.IsProtected() && !player.IsShielded() && !player.IsRetShielded())
                    Utils.RpcMurderPlayer(player, player, DeathReasonEnum.Bombed, false);
            }

            Destroy();
        }

        public static void DetonateBombs(List<Bomb> obj)
        {
            if (obj.Any(x => x.Drived))
            {
                foreach (var t in obj)
                {
                    t.Detonate();
                    obj.Remove(t);
                }

                Clear(obj);
            }
            else
            {
                var bomb = obj[^1];
                bomb.Detonate();
                obj.Remove(bomb);
            }
        }

        public static void Clear(List<Bomb> obj)
        {
            foreach (var t in obj)
                t.Destroy();
        }
    }
}