using HarmonyLib;
using Hazel;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.PlayerLayers.Abilities;
using TownOfUsReworked.PlayerLayers.Abilities.Abilities;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Extensions;
using UnityEngine;
using Reactor.Utilities;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Objectifiers.Objectifiers;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Objectifiers.TraitorMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CompleteTask))]
    public class SetTraitor
    {
        public static Sprite Sprite => TownOfUsReworked.Arrow;

        public static void Postfix(PlayerControl __instance)
        {
            if (!__instance.Is(ObjectifierEnum.Traitor))
                return;

            if (__instance.Data.IsDead)
                return;
            
            var taskinfos = __instance.Data.Tasks.ToArray();
            var tasksLeft = taskinfos.Count(x => !x.Complete);
            var traitor = Objectifier.GetObjectifier<Traitor>(__instance);

            if (traitor == null)
                return;
            
            if (tasksLeft == 0)
                traitor.Turned = true;

            if (traitor.Turned)
            {
                TurnTraitor(__instance);

                var writer = AmongUsClient.Instance.StartRpcImmediately(__instance.NetId, (byte)CustomRPC.TurnTraitor,
                    SendOption.Reliable, -1);
                writer.Write(__instance.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
        }

        public static void TurnTraitor(PlayerControl traitor)
        {
            var traitorObj = Objectifier.GetObjectifier<Traitor>(traitor);
            var traitorRole = Role.GetRole(traitor);

            var IntrudersAlive = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Intruder) && !x.Data.IsDead && !x.Data.Disconnected).Count();
            var SyndicateAlive = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Syndicate) && !x.Data.IsDead && !x.Data.Disconnected).Count();

            if (IntrudersAlive > 0 && SyndicateAlive > 0)
            {
                var random = Random.RandomRangeInt(0, 100);

                if (IntrudersAlive == SyndicateAlive)
                {
                    if (random <= 50)
                    {
                        traitorRole.Faction = Faction.Intruder;
                        traitorRole.Color = Colors.Intruder;
                        traitorRole.IsIntTraitor = true;
                        traitorObj.Side = "Intruders";
                    }
                    else if (random >= 51)
                    {
                        traitorRole.Faction = Faction.Syndicate;
                        traitorRole.Color = Colors.Syndicate;
                        traitorObj.Side = "Syndicate";
                        traitorRole.IsSynTraitor = true;
                    }
                }
                else if (IntrudersAlive > SyndicateAlive)
                {
                    if (random <= 25)
                    {
                        traitorRole.Faction = Faction.Intruder;
                        traitorRole.Color = Colors.Intruder;
                        traitorRole.IsIntTraitor = true;
                        traitorObj.Side = "Intruders";
                    }
                    else if (random >= 26)
                    {
                        traitorRole.Faction = Faction.Syndicate;
                        traitorRole.Color = Colors.Syndicate;
                        traitorObj.Side = "Syndicate";
                        traitorRole.IsSynTraitor = true;
                    }
                }
                else if (IntrudersAlive < SyndicateAlive)
                {
                    if (random <= 75)
                    {
                        traitorRole.Faction = Faction.Intruder;
                        traitorRole.Color = Colors.Intruder;
                        traitorRole.IsIntTraitor = true;
                        traitorObj.Side = "Intruders";
                    }
                    else if (random >= 76)
                    {
                        traitorRole.Faction = Faction.Syndicate;
                        traitorRole.Color = Colors.Syndicate;
                        traitorObj.Side = "Syndicate";
                        traitorRole.IsSynTraitor = true;
                    }
                }

                traitorObj.former = traitorRole;
            }
            else if (IntrudersAlive > 0 && SyndicateAlive == 0)
            {
                traitorRole.Faction = Faction.Intruder;
                traitorRole.Color = Colors.Intruder;
                traitorObj.Side = "Intruders";
                traitorRole.IsIntTraitor = true;
                traitorObj.former = traitorRole;
            }
            else if (SyndicateAlive > 0 && IntrudersAlive == 0)
            {
                traitorRole.Faction = Faction.Syndicate;
                traitorRole.Color = Colors.Syndicate;
                traitorRole.IsSynTraitor = true;
                traitorObj.Side = "Syndicate";
                traitorObj.former = traitorRole;
            }
            else
                return;
            
            if (traitorObj.Turned)
                Coroutines.Start(Utils.FlashCoroutine(Colors.Traitor));

            if (CustomGameOptions.TraitorCanAssassin)
            {
                Ability.AbilityDictionary.Remove(traitor.PlayerId);
                new Assassin(traitor);
                var writer2 = AmongUsClient.Instance.StartRpcImmediately(traitor.NetId, (byte)CustomRPC.SetAssassin,
                    SendOption.Reliable, -1);
                writer2.Write(traitor.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer2);
            }

            foreach (var snitch in Ability.GetAbilities(AbilityEnum.Snitch))
            {
                var snitchAbility = (Snitch)snitch;

                if (snitchAbility.Revealed && traitor.Is(ObjectifierEnum.Traitor) && CustomGameOptions.SnitchSeesTraitor)
                {
                    var gameObj = new GameObject();
                    var arrow = gameObj.AddComponent<ArrowBehaviour>();
                    gameObj.transform.parent = traitor.gameObject.transform;
                    var renderer = gameObj.AddComponent<SpriteRenderer>();
                    renderer.sprite = Sprite;
                    arrow.image = renderer;
                    gameObj.layer = 5;
                    snitchAbility.ImpArrows.Add(arrow);
                }
            }

            foreach (var revealer in Ability.GetAbilities(AbilityEnum.Revealer))
            {
                var revealerAbility = (Revealer)revealer;

                if (revealerAbility.Revealed && traitor.Is(ObjectifierEnum.Traitor) && CustomGameOptions.RevealerRevealsTraitor)
                {
                    var gameObj = new GameObject();
                    var arrow = gameObj.AddComponent<ArrowBehaviour>();
                    gameObj.transform.parent = traitor.gameObject.transform;
                    var renderer = gameObj.AddComponent<SpriteRenderer>();
                    renderer.sprite = Sprite;
                    arrow.image = renderer;
                    gameObj.layer = 5;
                    revealerAbility.ImpArrows.Add(arrow);
                }
            }
        }
    }
}