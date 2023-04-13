using HarmonyLib;
using Reactor.Utilities;
using TownOfUsReworked.CustomOptions;
using UnityEngine;
using TownOfUsReworked.Objects;
using System.Collections.Generic;
using TownOfUsReworked.Classes;
using System.Linq;

namespace TownOfUsReworked.Extensions
{
    [HarmonyPatch]
    public static class BombExtensions
    {
        public static void ClearBombs(this List<Bomb> obj)
        {
            foreach (var t in obj)
                t.Stop();

            obj.Clear();
        }

        public static void DetonateBombs(this List<Bomb> obj)
        {
            if (obj.Any(x => x.Drived))
            {
                foreach (var t in obj)
                {
                    if (t.Players.Count == 0)
                        continue;

                    t.Detonate();
                }

                obj.ClearBombs();
            }
            else
            {
                var bomb = obj[^1];
                bomb.Detonate();
                obj.Remove(bomb);
            }
        }

        public static Bomb CreateBomb(this Vector3 location, bool drived)
        {
            var BombPref = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            BombPref.name = "Bomb";
            var range = CustomGameOptions.BombRange + (drived ? CustomGameOptions.ChaosDriveBombRange : 0f);
            BombPref.transform.localScale = new Vector3(range * ShipStatus.Instance.MaxLightRadius * 2f, range * ShipStatus.Instance.MaxLightRadius * 2f, range * 2f *
                ShipStatus.Instance.MaxLightRadius);
            Object.Destroy(BombPref.GetComponent<SphereCollider>());
            BombPref.GetComponent<MeshRenderer>().material = AssetManager.BombMaterial;
            BombPref.transform.position = location;

            var BombScript = new Bomb
            {
                Transform = BombPref.transform,
                Drived = drived
            };

            Coroutines.Start(BombScript.BombTimer());
            return BombScript;
        }
    }
}