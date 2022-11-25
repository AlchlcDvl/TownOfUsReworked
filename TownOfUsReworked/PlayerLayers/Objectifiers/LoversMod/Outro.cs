using System.Linq;
using HarmonyLib;
using TMPro;
using TownOfUsReworked.Extensions;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.Patches;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.PlayerLayers.Objectifiers.Objectifiers;

namespace TownOfUsReworked.PlayerLayers.Objectifiers.LoversMod
{
    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
    internal class Outro
    {
        public static void Postfix(EndGameManager __instance)
        {
            TextMeshPro text;
            Vector3 pos;

            if (Role.GetRoles(RoleEnum.Taskmaster).Any(x => ((Taskmaster)x).WinTasksDone))
                return;

            if (Role.GetRoles(RoleEnum.Cannibal).Any(x => ((Cannibal)x).EatNeed == 0))
                return;

            if (Objectifier.GetObjectifiers(ObjectifierEnum.Phantom).Any(x => ((Phantom)x).CompletedTasks))
                return;

            if (!Objectifier.AllObjectifiers.Where(x => x.ObjectifierType == ObjectifierEnum.Lovers).Any(x => ((Lovers)x).LoveCoupleWins))
                return;

            PoolablePlayer[] array = Object.FindObjectsOfType<PoolablePlayer>();

            if (array[0] != null)
            {
                array[0].gameObject.transform.position -= new Vector3(1.5f, 0f, 0f);
                array[0].SetFlipX(true);
                array[0].NameText().color = Colors.Lovers;
            }

            if (array[1] != null)
            {
                array[1].SetFlipX(false);
                array[1].gameObject.transform.position = array[0].gameObject.transform.position + new Vector3(1.2f, 0f, 0f);
                array[1].gameObject.transform.localScale *= 0.92f;
                array[1].cosmetics.hat.transform.position += new Vector3(0.1f, 0f, 0f);
                array[1].NameText().color = Colors.Lovers;
            }

            __instance.BackgroundBar.material.color = Colors.Lovers;

            text = Object.Instantiate(__instance.WinText);
            text.text = "Love Wins!";
            text.color = Colors.Lovers;
            pos = __instance.WinText.transform.localPosition;
            pos.y = 1.5f;
            text.transform.position = pos;
            text.text = $"<size=4>{text.text}</size>";
        }
    }
}