using HarmonyLib;
using Hazel;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.PlayerLayers.Abilities;
using TownOfUsReworked.Classes;
using UnityEngine;
using Reactor.Utilities;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Enums;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Objectifiers.TraitorMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CompleteTask))]
    public class SetTraitor
    {
        public static Sprite Sprite => TownOfUsReworked.Arrow;

        public static void Postfix(PlayerControl __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(ObjectifierEnum.Traitor))
                return;

            if (PlayerControl.LocalPlayer.Data.IsDead)
                return;

            var traitor = Role.GetRole(PlayerControl.LocalPlayer);

            if (traitor.TasksDone)
            {
                TurnTraitor(PlayerControl.LocalPlayer);
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable);
                writer.Write((byte)TurnRPC.TurnTraitor);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
        }

        public static void TurnTraitor(PlayerControl traitor)
        {
            var traitorObj = Objectifier.GetObjectifier<Traitor>(traitor);
            var traitorRole = Role.GetRole(traitor);

            var IntrudersAlive = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Intruder) && !x.Data.IsDead && !x.Data.Disconnected).Count();
            var SyndicateAlive = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Syndicate) && !x.Data.IsDead && !x.Data.Disconnected).Count();

            var turnIntruder = false;
            var turnSyndicate = false;

            if (IntrudersAlive > 0 && SyndicateAlive > 0)
            {
                var random = Random.RandomRangeInt(0, 100);

                if (IntrudersAlive == SyndicateAlive)
                {
                    if (random < 50)
                        turnIntruder = true;
                    else if (random >= 50)
                        turnSyndicate = true;
                }
                else if (IntrudersAlive > SyndicateAlive)
                {
                    if (random < 25)
                        turnIntruder = true;
                    else if (random >= 25)
                        turnSyndicate = true;
                }
                else if (IntrudersAlive < SyndicateAlive)
                {
                    if (random < 75)
                        turnIntruder = true;
                    else if (random >= 75)
                        turnSyndicate = true;
                }
            }
            else if (IntrudersAlive > 0 && SyndicateAlive == 0)
                turnIntruder = true;
            else if (SyndicateAlive > 0 && IntrudersAlive == 0)
                turnSyndicate = true;
            else
                return;
            
            if (turnIntruder)
            {
                traitorRole.Faction = Faction.Intruder;
                traitorObj.Color = Colors.Intruder;
                traitorObj.Side = "Intruders";
                traitorRole.IsIntTraitor = true;
                traitorObj.former = traitorRole;
                traitorRole.FactionColor = Colors.Intruder;
                traitorObj.Objective = Role.IntrudersWinCon;
            }
            else if (turnSyndicate)
            {
                traitorRole.Faction = Faction.Syndicate;
                traitorRole.IsSynTraitor = true;
                traitorObj.Side = "Syndicate";
                traitorObj.former = traitorRole;
                traitorObj.Color = Colors.Syndicate;
                traitorObj.Objective = Role.SyndicateWinCon;
                traitorRole.FactionColor = Colors.Syndicate;
            }

            traitorObj.former = traitorRole;
            traitorObj.Side2 = traitorRole.Faction;
            traitorObj.Turned = true;
            traitor.RegenTask();

            if (CustomGameOptions.TraitorCanAssassin)
                Ability.GenAbility<Ability>(typeof(Assassin), traitor, 0);

            foreach (var snitch in Ability.GetAbilities(AbilityEnum.Snitch))
            {
                var snitchAbility = (Snitch)snitch;
                var role3 = Role.GetRole(snitchAbility.Player);

                if (role3.TasksLeft <= CustomGameOptions.SnitchTasksRemaining && traitor.Is(ObjectifierEnum.Traitor) && CustomGameOptions.SnitchSeesTraitor)
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

            foreach (var revealer in Role.GetRoles(RoleEnum.Revealer))
            {
                var revealerRole = (Revealer)revealer;

                if (revealerRole.Revealed && traitor.Is(ObjectifierEnum.Traitor) && traitorRole.TasksDone && CustomGameOptions.RevealerRevealsTraitor)
                {
                    var gameObj = new GameObject();
                    var arrow = gameObj.AddComponent<ArrowBehaviour>();
                    gameObj.transform.parent = traitor.gameObject.transform;
                    var renderer = gameObj.AddComponent<SpriteRenderer>();
                    renderer.sprite = Sprite;
                    arrow.image = renderer;
                    gameObj.layer = 5;
                    revealerRole.ImpArrows.Add(arrow);
                }
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Mystic) && traitor != PlayerControl.LocalPlayer)
                Coroutines.Start(Utils.FlashCoroutine(Colors.Traitor));

            if (traitor == PlayerControl.LocalPlayer || PlayerControl.LocalPlayer.Is(traitorRole.Faction))
                Coroutines.Start(Utils.FlashCoroutine(Colors.Traitor));
        }
    }
}