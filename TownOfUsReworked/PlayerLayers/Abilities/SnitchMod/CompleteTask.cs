using System.Linq;
using HarmonyLib;
using Reactor.Utilities;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Abilities.SnitchMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CompleteTask))]
    public static class CompleteTask
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (!__instance.Is(AbilityEnum.Snitch) || __instance.Data.IsDead)
                return;

            var role = Ability.GetAbility<Snitch>(__instance);

            if (role.TasksLeft == CustomGameOptions.SnitchTasksRemaining)
            {
                if (PlayerControl.LocalPlayer.Is(AbilityEnum.Snitch))
                {
                    Coroutines.Start(Utils.FlashCoroutine(role.Color));
                }
                else if (PlayerControl.LocalPlayer.Is(Faction.Intruder) || (PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralKill) && CustomGameOptions.SnitchSeesNeutrals) ||
                    PlayerControl.LocalPlayer.Is(Faction.Syndicate))
                {
                    Coroutines.Start(Utils.FlashCoroutine(role.Color));
                    var gameObj = new GameObject();
                    var arrow = gameObj.AddComponent<ArrowBehaviour>();
                    gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                    var renderer = gameObj.AddComponent<SpriteRenderer>();
                    renderer.sprite = AssetManager.Arrow;
                    arrow.image = renderer;
                    gameObj.layer = 5;
                    role.ImpArrows.Add(arrow);
                }
            }
            else if (PlayerControl.LocalPlayer.Is(AbilityEnum.Snitch) && role.TasksDone)
            {
                Coroutines.Start(Utils.FlashCoroutine(Color.green));

                foreach (var imp in PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Intruder)))
                {
                    var gameObj = new GameObject();
                    var arrow = gameObj.AddComponent<ArrowBehaviour>();
                    gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                    var renderer = gameObj.AddComponent<SpriteRenderer>();
                    renderer.sprite = AssetManager.Arrow;
                    arrow.image = renderer;
                    gameObj.layer = 5;
                    role.SnitchArrows.Add(imp.PlayerId, arrow);
                }
            }
            else if (PlayerControl.LocalPlayer.Is(Faction.Intruder) || (PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralKill) && CustomGameOptions.SnitchSeesNeutrals) ||
                PlayerControl.LocalPlayer.Is(Faction.Syndicate))
            {
                Coroutines.Start(Utils.FlashCoroutine(Color.green));
            }
        }
    }
}