using System.Linq;
using HarmonyLib;
using Reactor.Utilities;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles;

namespace TownOfUsReworked.PlayerLayers.Abilities.SnitchMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CompleteTask))]
    public class CompleteTask
    {
        public static Sprite Sprite => TownOfUsReworked.Arrow;

        public static void Postfix(PlayerControl __instance)
        {
            if (!__instance.Is(AbilityEnum.Snitch) || __instance.Data.IsDead)
                return;

            var snitch = Ability.GetAbility<Snitch>(__instance);
            var role = Role.GetRole(snitch.Player);

            if (role.TasksLeft == CustomGameOptions.SnitchTasksRemaining)
            {
                if (PlayerControl.LocalPlayer.Is(AbilityEnum.Snitch))
                    Coroutines.Start(Utils.FlashCoroutine(snitch.Color));
                else if (PlayerControl.LocalPlayer.Is(Faction.Intruder) || (PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralKill) && CustomGameOptions.SnitchSeesNeutrals) ||
                    PlayerControl.LocalPlayer.Is(Faction.Syndicate))
                {
                    Coroutines.Start(Utils.FlashCoroutine(snitch.Color));
                    var gameObj = new GameObject();
                    var arrow = gameObj.AddComponent<ArrowBehaviour>();
                    gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                    var renderer = gameObj.AddComponent<SpriteRenderer>();
                    renderer.sprite = Sprite;
                    arrow.image = renderer;
                    gameObj.layer = 5;
                    snitch.ImpArrows.Add(arrow);
                }
            }
            else if (PlayerControl.LocalPlayer.Is(AbilityEnum.Snitch) && role.TasksDone)
            {
                Coroutines.Start(Utils.FlashCoroutine(Color.green));
                var impostors = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Intruder));

                foreach (var imp in impostors)
                {
                    var gameObj = new GameObject();
                    var arrow = gameObj.AddComponent<ArrowBehaviour>();
                    gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                    var renderer = gameObj.AddComponent<SpriteRenderer>();
                    renderer.sprite = Sprite;
                    arrow.image = renderer;
                    gameObj.layer = 5;
                    snitch.SnitchArrows.Add(imp.PlayerId, arrow);
                }
            }
            else if (PlayerControl.LocalPlayer.Is(Faction.Intruder) || (PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralKill) && CustomGameOptions.SnitchSeesNeutrals) ||
                PlayerControl.LocalPlayer.Is(Faction.Syndicate))
                Coroutines.Start(Utils.FlashCoroutine(Color.green));
        }
    }
}