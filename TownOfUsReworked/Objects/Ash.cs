using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using TownOfUsReworked.Classes;
using Reactor.Utilities.Extensions;

namespace TownOfUsReworked.Objects
{
    [HarmonyPatch]
    public class Ash
    {
        public readonly static List<Ash> AllPiles = new();
        public GameObject Pile;

        public Ash(Vector2 position)
        {
            Pile = new GameObject("AshPile") { layer = 11 };
            Pile.AddSubmergedComponent("ElevatorMover");
            Pile.transform.position = new Vector3(position.x, position.y, (position.y / 1000f) + 0.001f);
            Pile.transform.localScale = Vector3.one * 0.35f;
            Pile.AddComponent<SpriteRenderer>().sprite = AssetManager.GetSprite("AshPile");
            Pile.SetActive(true);
            AllPiles.Add(this);
        }

        public void Destroy()
        {
            if (Pile == null)
                return;

            Pile.SetActive(false);
            Pile.Destroy();
        }

        public static void DestroyAll()
        {
            foreach (var pile in AllPiles)
                pile.Destroy();

            AllPiles.Clear();
        }
    }
}