using HarmonyLib;
using Reactor.Utilities;
using System.Collections;
using Il2CppSystem.Collections.Generic;
using TownOfUsReworked.CustomOptions;
using UnityEngine;
using Object = UnityEngine.Object;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.Objects
{
    public class Bomb
    {
        public List<PlayerControl> players = new List<PlayerControl>();
        public Transform transform;

        public IEnumerator BombTimer()
        {
            while (transform != null)
            {
                yield return 0;
                Update();
            }
        }

        public void Update()
        {
            if (transform == null)
                return;

            players = Utils.GetClosestPlayers(transform.position, CustomGameOptions.BombRange + (Role.SyndicateHasChaosDrive ? CustomGameOptions.ChaosDriveBombRange : 0f));
        }
    }

    [HarmonyPatch]
    public static class BombExtentions
    {
        public static void ClearBombs(this System.Collections.Generic.List<Bomb> obj)
        {
            foreach (Bomb t in obj)
            {
                Object.Destroy(t.transform.gameObject);
                Coroutines.Stop(t.BombTimer());
            }

            obj.Clear();
        }

        public static void DetonateBombs(this System.Collections.Generic.List<Bomb> obj, string name)
        {
            foreach (Bomb t in obj)
            {
                if (t.players.Count == 0)
                    continue;

                foreach (var player in t.players)
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
            BombPref.transform.localScale = new Vector3(range * ShipStatus.Instance.MaxLightRadius * 2f, range * ShipStatus.Instance.MaxLightRadius * 2f, range *
                ShipStatus.Instance.MaxLightRadius * 2f);
            GameObject.Destroy(BombPref.GetComponent<SphereCollider>());
            BombPref.GetComponent<MeshRenderer>().material = Bomber.BombMaterial;
            BombPref.transform.position = location;
            var BombScript = new Bomb();
            BombScript.transform = BombPref.transform;
            Coroutines.Start(BombScript.BombTimer());
            return BombScript;
        }
    }
}