using System.Linq;
using HarmonyLib;
using Reactor.Utilities;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Abilities.Abilities;

namespace TownOfUsReworked.PlayerLayers.Abilities.SnitchMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CompleteTask))]
    public class CompleteTask
    {
        public static Sprite Sprite => TownOfUsReworked.Arrow;

        public static void Postfix(PlayerControl __instance)
        {
            if (!__instance.Is(AbilityEnum.Snitch))
                return;
            if (__instance.Data.IsDead)
                return;

            var taskinfos = __instance.Data.Tasks.ToArray();
            var tasksLeft = taskinfos.Count(x => !x.Complete);
            var role = Ability.GetAbility<Snitch>(__instance);
            var localRole = Ability.GetAbility(PlayerControl.LocalPlayer);

            switch (tasksLeft)
            {
                case 1:
                    if (tasksLeft == CustomGameOptions.SnitchTasksRemaining)
                    {
                        role.RegenTask();

                        if (PlayerControl.LocalPlayer.Is(AbilityEnum.Snitch))
                            Coroutines.Start(Utils.FlashCoroutine(role.Color));
                        else if (PlayerControl.LocalPlayer.Is(Faction.Intruder) | (PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralKill) &&
                            CustomGameOptions.SnitchSeesNeutrals) | PlayerControl.LocalPlayer.Is(Faction.Syndicate))
                        {
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

                    break;
                case 0:
                    role.RegenTask();

                    if (PlayerControl.LocalPlayer.Is(AbilityEnum.Snitch))
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
                            role.SnitchArrows.Add(imp.PlayerId, arrow);
                        }
                    }
                    else if (PlayerControl.LocalPlayer.Is(Faction.Intruder) | (PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralKill) &&
                            CustomGameOptions.SnitchSeesNeutrals) | PlayerControl.LocalPlayer.Is(Faction.Syndicate))
                        Coroutines.Start(Utils.FlashCoroutine(Color.green));

                    break;
            }
        }
    }
}