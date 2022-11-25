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

namespace TownOfUsReworked.PlayerLayers.Objectifiers.TraitorMod
{
    [HarmonyPatch(typeof(AirshipExileController), nameof(AirshipExileController.WrapUpAndSpawn))]
    public static class AirshipExileController_WrapUpAndSpawn
    {
        public static void Postfix(AirshipExileController __instance) => SetTraitor.ExileControllerPostfix(__instance);
    }
    
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    public class SetTraitor
    {
        public static Sprite Sprite => TownOfUsReworked.Arrow;

        public static void Postfix(ExileController __instance) => ExileControllerPostfix(__instance);

        public static void ExileControllerPostfix(ExileController __instance)
        {
            var exiled = __instance.exiled?.Object;
            var traitorObj = Objectifier.GetObjectifier<Traitor>(exiled);

            if (PlayerControl.LocalPlayer.Data.IsDead | exiled == PlayerControl.LocalPlayer | exiled == traitorObj.Player)
                return;

            if (PlayerControl.LocalPlayer.Is(ObjectifierEnum.Traitor) && traitorObj.Turned)
            {
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Investigator))
                    Footprint.DestroyAll(Role.GetRole<Investigator>(PlayerControl.LocalPlayer));

                if (PlayerControl.LocalPlayer.Is(RoleEnum.TimeLord))
                {
                    var timeLordRole = Role.GetRole<TimeLord>(PlayerControl.LocalPlayer);
                    Object.Destroy(timeLordRole.UsesText);
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Tracker))
                {
                    var trackerRole = Role.GetRole<Tracker>(PlayerControl.LocalPlayer);
                    trackerRole.TrackerArrows.Values.DestroyAll();
                    trackerRole.TrackerArrows.Clear();
                    Object.Destroy(trackerRole.UsesText);
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Transporter))
                {
                    var transporterRole = Role.GetRole<Transporter>(PlayerControl.LocalPlayer);
                    Object.Destroy(transporterRole.UsesText);
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Veteran))
                {
                    var veteranRole = Role.GetRole<Veteran>(PlayerControl.LocalPlayer);
                    Object.Destroy(veteranRole.UsesText);
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Medium))
                {
                    var medRole = Role.GetRole<Medium>(PlayerControl.LocalPlayer);
                    medRole.MediatedPlayers.Values.DestroyAll();
                    medRole.MediatedPlayers.Clear();
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Operative))
                {
                    var opRole = Role.GetRole<Operative>(PlayerControl.LocalPlayer);
                    Object.Destroy(opRole.UsesText);
                }
                
                var traitor = Objectifier.GetObjectifier<Traitor>(PlayerControl.LocalPlayer);
                traitor.former = Role.GetRole(PlayerControl.LocalPlayer);
                traitor.formerRole = traitor.former.RoleType;
                traitor.RegenTask();

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.TraitorSpawn,
                    SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(writer);

                TurnTraitor(PlayerControl.LocalPlayer);
            }
        }

        public static void TurnTraitor(PlayerControl traitor)
        {
            var traitorObj = Objectifier.GetObjectifier<Traitor>(traitor);
            var random = Random.RandomRangeInt(0, 100);

            if (random <= 50)
                traitorObj.Side = Faction.Intruders;
            else if (random > 50)
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
                var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetAssassin,
                    SendOption.Reliable, -1);
                writer2.Write(traitor.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer2);
            }
            
            if (traitorObj.Turned)
                Coroutines.Start(Utils.FlashCoroutine(Colors.Traitor));

            foreach (var snitch in Ability.GetAbilities(AbilityEnum.Snitch))
            {
                var snitchRole = (Snitch)snitch;

                if (snitchRole.TasksDone && PlayerControl.LocalPlayer.Is(AbilityEnum.Snitch) && CustomGameOptions.SnitchSeesTraitor)
                {
                    var gameObj = new GameObject();
                    var arrow = gameObj.AddComponent<ArrowBehaviour>();
                    gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                    var renderer = gameObj.AddComponent<SpriteRenderer>();
                    renderer.sprite = Sprite;
                    arrow.image = renderer;
                    gameObj.layer = 5;
                    snitchRole.SnitchArrows.Add(traitor.PlayerId, arrow);
                }
                else if (snitchRole.Revealed && PlayerControl.LocalPlayer.Is(ObjectifierEnum.Traitor) && CustomGameOptions.SnitchSeesTraitor)
                {
                    var gameObj = new GameObject();
                    var arrow = gameObj.AddComponent<ArrowBehaviour>();
                    gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                    var renderer = gameObj.AddComponent<SpriteRenderer>();
                    renderer.sprite = Sprite;
                    arrow.image = renderer;
                    gameObj.layer = 5;
                    snitchRole.ImpArrows.Add(arrow);
                }
            }

            foreach (var haunter in Ability.GetAbilities(AbilityEnum.Revealer))
            {
                var haunterRole = (Revealer)haunter;

                if (haunterRole.Revealed && PlayerControl.LocalPlayer.Is(ObjectifierEnum.Traitor))
                {
                    var gameObj = new GameObject();
                    var arrow = gameObj.AddComponent<ArrowBehaviour>();
                    gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                    var renderer = gameObj.AddComponent<SpriteRenderer>();
                    renderer.sprite = Sprite;
                    arrow.image = renderer;
                    gameObj.layer = 5;
                    haunterRole.ImpArrows.Add(arrow);
                }
            }
        }
    }
}