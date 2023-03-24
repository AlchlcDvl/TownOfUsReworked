using HarmonyLib;
using Reactor.Utilities;
using TownOfUsReworked.CustomOptions;
using UnityEngine;
using Object = UnityEngine.Object;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Objects;
using System.Collections.Generic;

namespace TownOfUsReworked.Classes
{
    [HarmonyPatch]
    public static class BombExtentions
    {
        public static void ClearBombs(this List<Bomb> obj)
        {
            foreach (Bomb t in obj)
            {
                Object.Destroy(t.Transform.gameObject);
                Coroutines.Stop(t.BombTimer());
            }

            obj.Clear();
        }

        public static void DetonateBombs(this List<Bomb> obj, string name)
        {
            foreach (Bomb t in obj)
            {
                if (t.Players.Count == 0)
                    continue;

                foreach (var player in t.Players)
                {
                    Utils.RpcMurderPlayer(player, player, false);
                    var targetRole = Role.GetRole(player);
                    targetRole.KilledBy = " By " + name;
                    targetRole.DeathReason = DeathReasonEnum.Killed;
                }
            }

            obj.ClearBombs();
        }

        public static Bomb CreateBomb(this Vector3 location)
        {
            var BombPref = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            BombPref.name = "Bomb";
            var range = CustomGameOptions.BombRange + (Role.SyndicateHasChaosDrive ? CustomGameOptions.ChaosDriveBombRange : 0f);
            BombPref.transform.localScale = new Vector3(range * ShipStatus.Instance.MaxLightRadius * 2f, range * ShipStatus.Instance.MaxLightRadius * 2f, range * 2f *
                ShipStatus.Instance.MaxLightRadius);
            Object.Destroy(BombPref.GetComponent<SphereCollider>());
            BombPref.GetComponent<MeshRenderer>().material = AssetManager.BombMaterial;
            BombPref.transform.position = location;

            var BombScript = new Bomb
            {
                Transform = BombPref.transform
            };

            Coroutines.Start(BombScript.BombTimer());
            return BombScript;
        }
    }
}