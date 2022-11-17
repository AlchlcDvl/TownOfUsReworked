using System.Linq;
using HarmonyLib;
using Reactor.Utilities;
using UnityEngine;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Abilities.Abilities;

namespace TownOfUsReworked.PlayerLayers.Abilities.RevealerMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CompleteTask))]
    public class CompleteTask
    {
        public static Sprite Sprite => TownOfUsReworked.Arrow;

        public static void Postfix(PlayerControl __instance)
        {
            if (!__instance.Is(AbilityEnum.Revealer))
                return;

            var role = Ability.GetAbility<Revealer>(__instance);
            var taskinfos = __instance.Data.Tasks.ToArray();
            var tasksLeft = taskinfos.Count(x => !x.Complete);

            if (tasksLeft == CustomGameOptions.RevealerTasksRemainingAlert && !role.Caught)
            {
                if (PlayerControl.LocalPlayer.Is(AbilityEnum.Revealer))
                    Coroutines.Start(Utils.FlashCoroutine(role.Color));
                else if (PlayerControl.LocalPlayer.Data.IsImpostor() | (PlayerControl.LocalPlayer.Is(Faction.Neutral) &&
                    CustomGameOptions.RevealerRevealsNeutrals))
                {
                    role.Revealed = true;
                    Coroutines.Start(Utils.FlashCoroutine(role.Color));
                    var gameObj = new GameObject();
                    var arrow = gameObj.AddComponent<ArrowBehaviour>();
                    gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                    var renderer = gameObj.AddComponent<SpriteRenderer>();
                    renderer.sprite = Sprite;
                    arrow.image = renderer;
                    gameObj.layer = 5;
                    role.ImpArrows.Add(arrow);
                }
            }

            if (tasksLeft == 0 && !role.Caught)
            {
                role.CompletedTasks = true;

                if (PlayerControl.LocalPlayer.Is(AbilityEnum.Revealer))
                    Coroutines.Start(Utils.FlashCoroutine(role.Color));
                else if (PlayerControl.LocalPlayer.Data.IsImpostor() | (PlayerControl.LocalPlayer.Is(Faction.Neutral) &&
                    CustomGameOptions.RevealerRevealsNeutrals))
                    Coroutines.Start(Utils.FlashCoroutine(Color.white));
            }
        }
    }
}