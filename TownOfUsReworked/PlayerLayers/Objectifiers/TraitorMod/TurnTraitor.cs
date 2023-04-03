using HarmonyLib;
using Hazel;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.PlayerLayers.Abilities;
using TownOfUsReworked.Classes;
using UnityEngine;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.CustomOptions;
using System.Linq;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Objectifiers.TraitorMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CompleteTask))]
    public static class SetTraitor
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, ObjectifierEnum.Traitor))
                return;

            var traitor = Objectifier.GetObjectifier<Traitor>(PlayerControl.LocalPlayer);

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
            if (!traitor.Is(ObjectifierEnum.Traitor))
                return;

            var traitorObj = Objectifier.GetObjectifier<Traitor>(traitor);
            var traitorRole = Role.GetRole(traitor);

            var IntrudersAlive = PlayerControl.AllPlayerControls.ToArray().Count(x => x.Is(Faction.Intruder) && !x.Data.IsDead && !x.Data.Disconnected);
            var SyndicateAlive = PlayerControl.AllPlayerControls.ToArray().Count(x => x.Is(Faction.Syndicate) && !x.Data.IsDead && !x.Data.Disconnected);

            var turnIntruder = false;
            var turnSyndicate = false;

            if (IntrudersAlive > 0 && SyndicateAlive > 0)
            {
                var random = Random.RandomRangeInt(0, 100);

                if (IntrudersAlive == SyndicateAlive)
                {
                    turnIntruder = random < 50;
                    turnSyndicate = random >= 50;
                }
                else if (IntrudersAlive > SyndicateAlive)
                {
                    turnIntruder = random < 25;
                    turnSyndicate = random >= 25;
                }
                else if (IntrudersAlive < SyndicateAlive)
                {
                    turnIntruder = random < 75;
                    turnSyndicate = random >= 75;
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
                traitorRole.IsIntTraitor = true;
                traitorObj.former = traitorRole;
                traitorRole.FactionColor = Colors.Intruder;
                traitorObj.Objective = Role.IntrudersWinCon;
                traitorRole.Objectives = Role.IntrudersWinCon;
            }
            else if (turnSyndicate)
            {
                traitorRole.Faction = Faction.Syndicate;
                traitorRole.IsSynTraitor = true;
                traitorObj.former = traitorRole;
                traitorObj.Color = Colors.Syndicate;
                traitorObj.Objective = Role.SyndicateWinCon;
                traitorRole.FactionColor = Colors.Syndicate;
                traitorRole.Objectives = Role.SyndicateWinCon;
            }

            traitorObj.former = traitorRole;
            traitorObj.Side = traitorRole.Faction;
            traitorObj.TaskText = "";
            traitorObj.Turned = true;
            traitor.RegenTask();

            foreach (var snitch in Ability.GetAbilities(AbilityEnum.Snitch).Cast<Snitch>())
            {
                if (snitch.TasksLeft <= CustomGameOptions.SnitchTasksRemaining && traitor.Is(ObjectifierEnum.Traitor) && CustomGameOptions.SnitchSeesTraitor)
                {
                    var gameObj = new GameObject();
                    var arrow = gameObj.AddComponent<ArrowBehaviour>();
                    gameObj.transform.parent = traitor.gameObject.transform;
                    var renderer = gameObj.AddComponent<SpriteRenderer>();
                    renderer.sprite = AssetManager.Arrow;
                    arrow.image = renderer;
                    gameObj.layer = 5;
                    snitch.ImpArrows.Add(arrow);
                }
            }

            foreach (var revealer in Role.GetRoles(RoleEnum.Revealer).Cast<Revealer>())
            {
                if (revealer.Revealed && traitor.Is(ObjectifierEnum.Traitor) && traitorRole.TasksDone && CustomGameOptions.RevealerRevealsTraitor)
                {
                    var gameObj = new GameObject();
                    var arrow = gameObj.AddComponent<ArrowBehaviour>();
                    gameObj.transform.parent = traitor.gameObject.transform;
                    var renderer = gameObj.AddComponent<SpriteRenderer>();
                    renderer.sprite = AssetManager.Arrow;
                    arrow.image = renderer;
                    gameObj.layer = 5;
                    revealer.ImpArrows.Add(arrow);
                }
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Mystic) && traitor != PlayerControl.LocalPlayer)
                Utils.Flash(Colors.Mystic, "Someone changed their allegience!");

            if (traitor == PlayerControl.LocalPlayer || PlayerControl.LocalPlayer.Is(traitorRole.Faction))
                Utils.Flash(Colors.Traitor, "A <color=#370D43FF>Traitor</color> has revealed themselves!");
        }
    }
}