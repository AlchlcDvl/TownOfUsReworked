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

            var snitch = Ability.GetAbility<Snitch>(__instance);

            switch (snitch.TasksLeft())
            {
                case 1:
                    if (snitch.TasksLeft() == CustomGameOptions.SnitchTasksRemaining)
                    {
                        if (PlayerControl.LocalPlayer.Is(AbilityEnum.Snitch))
                            Coroutines.Start(Utils.FlashCoroutine(snitch.Color));
                        else if (PlayerControl.LocalPlayer.Is(Faction.Intruder) || (PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralKill) &&
                            CustomGameOptions.SnitchSeesNeutrals) || PlayerControl.LocalPlayer.Is(Faction.Syndicate))
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

                    break;
                case 0:
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
                            snitch.SnitchArrows.Add(imp.PlayerId, arrow);
                        }
                    }
                    else if (PlayerControl.LocalPlayer.Is(Faction.Intruder) || (PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralKill) && CustomGameOptions.SnitchSeesNeutrals) ||
                        PlayerControl.LocalPlayer.Is(Faction.Syndicate))
                        Coroutines.Start(Utils.FlashCoroutine(Color.green));

                    break;
            }
        }
    }
}