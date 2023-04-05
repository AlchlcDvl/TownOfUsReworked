using HarmonyLib;
using Reactor.Utilities;
using System.Collections.Generic;
using TownOfUsReworked.CustomOptions;
using UnityEngine;
using Object = UnityEngine.Object;
using TownOfUsReworked.Objects;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.Extensions
{
    [HarmonyPatch]
    public static class BugExtensions
    {
        public static void ClearBugs(this List<Bug> obj)
        {
            foreach (var t in obj)
                t.Stop();

            obj.Clear();
        }

        public static Bug CreateBug(this Vector3 location)
        {
            var BugPref = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            BugPref.name = "Bug";
            BugPref.transform.localScale = new Vector3(CustomGameOptions.BugRange * ShipStatus.Instance.MaxLightRadius * 2f, CustomGameOptions.BugRange * 2f *
                ShipStatus.Instance.MaxLightRadius, CustomGameOptions.BugRange * ShipStatus.Instance.MaxLightRadius * 2f);
            Object.Destroy(BugPref.GetComponent<SphereCollider>());
            BugPref.GetComponent<MeshRenderer>().material = AssetManager.BugMaterial;
            BugPref.transform.position = location;
            var BugScript = new Bug { Transform = BugPref.transform };
            Coroutines.Start(BugScript.BugTimer());
            return BugScript;
        }
    }
}