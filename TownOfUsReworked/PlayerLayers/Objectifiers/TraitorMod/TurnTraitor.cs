using HarmonyLib;
using Hazel;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.PlayerLayers.Abilities;
using TownOfUsReworked.PlayerLayers.Abilities.Abilities;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.InvestigatorMod;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
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

            if (__instance.Is(ObjectifierEnum.Traitor) && traitor.Turned)
            {
                if (__instance.Is(RoleEnum.Investigator))
                    Footprint.DestroyAll(Role.GetRole<Investigator>(__instance));
                else if (__instance.Is(RoleEnum.TimeLord))
                {
                    var timeLordRole = Role.GetRole<TimeLord>(__instance);
                    Object.Destroy(timeLordRole.UsesText);
                }
                else if (__instance.Is(RoleEnum.Tracker))
                {
                    var trackerRole = Role.GetRole<Tracker>(__instance);
                    trackerRole.TrackerArrows.Values.DestroyAll();
                    trackerRole.TrackerArrows.Clear();
                    Object.Destroy(trackerRole.UsesText);
                }
                else if (__instance.Is(RoleEnum.Transporter))
                {
                    var transporterRole = Role.GetRole<Transporter>(__instance);
                    Object.Destroy(transporterRole.UsesText);
                }
                else if (__instance.Is(RoleEnum.Veteran))
                {
                    var veteranRole = Role.GetRole<Veteran>(__instance);
                    Object.Destroy(veteranRole.UsesText);
                }
                else if (__instance.Is(RoleEnum.Medium))
                {
                    var medRole = Role.GetRole<Medium>(__instance);
                    medRole.MediatedPlayers.Values.DestroyAll();
                    medRole.MediatedPlayers.Clear();
                }
                else if (__instance.Is(RoleEnum.Operative))
                {
                    var opRole = Role.GetRole<Operative>(__instance);
                    Object.Destroy(opRole.UsesText);
                }

                TurnTraitor(__instance);

                var writer = AmongUsClient.Instance.StartRpcImmediately(__instance.NetId, (byte)CustomRPC.TurnTraitor,
                    SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
        }

        public static void TurnTraitor(PlayerControl traitor)
        {
            var traitorObj = Objectifier.GetObjectifier<Traitor>(traitor);
            var random = Random.RandomRangeInt(0, 100);

            if (random <= 50)
                traitorObj.Side = Faction.Intruder;
            else if (random >= 51)
                 traitorObj.Side = Faction.Syndicate;

            var side = traitorObj.Side;

            foreach (var player2 in PlayerControl.AllPlayerControls)
            {
                var playerRole = Role.GetRole(player2);

                if (player2.Is(side) && traitor != player2)
                {
                    if (CustomGameOptions.FactionSeeRoles)
                        player2.nameText().color = playerRole.Color;
                    else
                        player2.nameText().color = playerRole.FactionColor;
                }
            }

            if (CustomGameOptions.TraitorCanAssassin)
            {
                var writer2 = AmongUsClient.Instance.StartRpcImmediately(traitor.NetId, (byte)CustomRPC.SetAssassin,
                    SendOption.Reliable, -1);
                writer2.Write(traitor.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer2);
            }
            
            if (traitorObj.Turned)
                Coroutines.Start(Utils.FlashCoroutine(Colors.Traitor));

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