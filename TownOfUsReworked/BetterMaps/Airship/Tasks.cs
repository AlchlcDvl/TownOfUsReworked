using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using Reactor.Utilities.Attributes;
using Il2CppInterop.Runtime.Attributes;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.BetterMaps.Airship
{
    [HarmonyPatch]
    [RegisterInIl2Cpp]
    public class Tasks : MonoBehaviour
    {
        public Tasks(IntPtr ptr) : base(ptr) { }

        public readonly static List<GameObject> AllCustomPlateform = new();
        public byte Id;
        public Action OnClick;

        #pragma warning disable
        public static Tasks NearestTask;
        private readonly SpriteRenderer renderer = null;
        #pragma warning restore

        [HideFromIl2Cpp]
        public float CanUse(GameData.PlayerInfo PC, out bool CanUse)
        {
            var Player = PC.Object;
            var truePosition = Player.GetTruePosition();
            CanUse = !PC.IsDead && Player.CanMove && !CallPlateform.PlateformIsUsed && !UnityEngine.Object.FindObjectOfType<MovingPlatformBehaviour>().InUse;
            var Distance = float.MaxValue;

            if (CanUse)
            {
                Distance = Vector2.Distance(truePosition, transform.position);
                CanUse &= Distance <= CustomGameOptions.InteractionDistance;
            }

            return Distance;
        }

        public void Use() => OnClick();

        [HideFromIl2Cpp]
        public void SetOutline(bool On)
        {
            if (renderer)
            {
                renderer.material.SetFloat("_Outline", (float) (On ? 1 : 0f));
                renderer.material.SetColor("_OutlineColor", Color.white);
                renderer.material.SetColor("_AddColor", On ? Color.white : Color.clear);
            }
        }

        public static void CreateThisTask(Vector3 Position, Vector3 Rotation, Action OnClick)
        {
            var CallPlateform = new GameObject("CallPlateform");
            CallPlateform.transform.position = Position;
            CallPlateform.transform.localRotation = Quaternion.Euler(Rotation);
            CallPlateform.transform.localScale = new Vector3(1f, 1f, 2f);
            CallPlateform.layer = 12;
            CallPlateform.SetActive(true);

            var CallPlateformTasks = CallPlateform.AddComponent<Tasks>();
            CallPlateformTasks.Id = 1;
            CallPlateformTasks.OnClick = OnClick;
            AllCustomPlateform.Add(CallPlateform);
        }

        public static void ClosestTasks(PlayerControl Player)
        {
            NearestTask = null;

            foreach (var CustomElectrical in AllCustomPlateform)
            {
                var component = CustomElectrical.GetComponent<Tasks>();
                component.SetOutline(false);

                if (component != null && ((!Player.Data.IsDead && (!AmongUsClient.Instance || !AmongUsClient.Instance.IsGameOver) && Player.CanMove) || !Player.inVent))
                {
                    var Distance = component.CanUse(Player.Data, out bool CanUse);

                    if (CanUse && Distance <= CustomGameOptions.InteractionDistance)
                    {
                        NearestTask = component;
                        component.SetOutline(true);
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(UseButton), nameof(UseButton.SetTarget))]
    public static class UseButtonSetTargetPatch
    {
        public static bool Prefix(UseButton __instance)
        {
            Tasks.ClosestTasks(PlayerControl.LocalPlayer);

            if (__instance.isActiveAndEnabled && PlayerControl.LocalPlayer && Tasks.NearestTask != null && Tasks.AllCustomPlateform != null)
            {
                __instance.graphic.color = new Color(1f, 1f, 1f, 1f);
                return false;
            }

            return true;
        }
    }
}