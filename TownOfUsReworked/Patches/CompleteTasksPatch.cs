using System.Linq;
using HarmonyLib;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using UnityEngine;
using TownOfUsReworked.Data;
using Hazel;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.PlayerLayers.Objectifiers;
using TownOfUsReworked.PlayerLayers.Abilities;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CompleteTask))]
    public static class CompleteTasksPatch
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (__instance.Is(AbilityEnum.Snitch) && !__instance.Data.IsDead)
            {
                var role = Ability.GetAbility<Snitch>(__instance);

                if (role.TasksLeft == CustomGameOptions.SnitchTasksRemaining)
                {
                    if (PlayerControl.LocalPlayer == __instance)
                        Utils.Flash(role.Color);
                    else if (PlayerControl.LocalPlayer.Is(Faction.Intruder) || (PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralKill) && CustomGameOptions.SnitchSeesNeutrals) ||
                        PlayerControl.LocalPlayer.Is(Faction.Syndicate))
                    {
                        Utils.Flash(role.Color);
                        var gameObj = new GameObject("SnitchArrow") { layer = 5 };
                        var arrow = gameObj.AddComponent<ArrowBehaviour>();
                        gameObj.transform.parent = __instance.gameObject.transform;
                        var renderer = gameObj.AddComponent<SpriteRenderer>();
                        renderer.sprite = AssetManager.GetSprite("Arrow");
                        arrow.image = renderer;
                        gameObj.layer = 5;
                        Role.LocalRole.AllArrows.Add(role.PlayerId, arrow);
                    }
                }
                else if (PlayerControl.LocalPlayer.Is(AbilityEnum.Snitch) && role.TasksDone)
                {
                    if (PlayerControl.LocalPlayer == __instance)
                    {
                        Utils.Flash(Color.green);

                        foreach (var imp in PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Intruder) || x.Is(Faction.Syndicate) || (x.Is(RoleAlignment.NeutralKill) &&
                            CustomGameOptions.SnitchSeesNeutrals)))
                        {
                            var gameObj = new GameObject("SnitchEvilArrow") { layer = 5 };
                            var arrow = gameObj.AddComponent<ArrowBehaviour>();
                            gameObj.transform.parent = imp.gameObject.transform;
                            var renderer = gameObj.AddComponent<SpriteRenderer>();
                            renderer.sprite = AssetManager.GetSprite("Arrow");
                            arrow.image = renderer;
                            Role.LocalRole.AllArrows.Add(imp.PlayerId, arrow);
                        }
                    }
                    else if (PlayerControl.LocalPlayer.Is(Faction.Intruder) || (PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralKill) && CustomGameOptions.SnitchSeesNeutrals) ||
                        PlayerControl.LocalPlayer.Is(Faction.Syndicate))
                    {
                        Utils.Flash(Color.red);
                    }
                }
            }

            if (__instance.Is(ObjectifierEnum.Traitor) && !__instance.Data.IsDead)
            {
                var traitor = Objectifier.GetObjectifier<Traitor>(PlayerControl.LocalPlayer);

                if (traitor.TasksDone)
                {
                    TurnTraitor(__instance);
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable);
                    writer.Write((byte)TurnRPC.TurnTraitor);
                    writer.Write(__instance.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }
            else if (__instance.Is(ObjectifierEnum.Taskmaster) && !__instance.Data.IsDead)
            {
                var role = Objectifier.GetObjectifier<Taskmaster>(__instance);

                if (role.TasksLeft == CustomGameOptions.TMTasksRemaining)
                {
                    if (PlayerControl.LocalPlayer == __instance || PlayerControl.LocalPlayer.Is(Faction.Crew) || PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralBen) ||
                        PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralEvil))
                    {
                        Utils.Flash(role.Color);
                    }
                    else if (PlayerControl.LocalPlayer.Is(Faction.Intruder) || PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralKill) || PlayerControl.LocalPlayer.Is(Faction.Syndicate) ||
                        PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralNeo) || PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralPros))
                    {
                        Utils.Flash(role.Color);
                        var gameObj = new GameObject("TMArrow") { layer = 5 };
                        var arrow = gameObj.AddComponent<ArrowBehaviour>();
                        gameObj.transform.parent = __instance.gameObject.transform;
                        var renderer = gameObj.AddComponent<SpriteRenderer>();
                        renderer.sprite = AssetManager.GetSprite("Arrow");
                        arrow.image = renderer;
                        Role.LocalRole.AllArrows.Add(role.PlayerId, arrow);
                    }
                }
                else if (role.TasksDone)
                {
                    if (PlayerControl.LocalPlayer.Is(ObjectifierEnum.Taskmaster))
                        Utils.Flash(role.Color);

                    role.WinTasksDone = true;
                }
            }

            if (__instance.Is(RoleEnum.Phantom))
            {
                var role = Role.GetRole<Phantom>(__instance);

                if (role.TasksLeft == CustomGameOptions.PhantomTasksRemaining && CustomGameOptions.PhantomPlayersAlerted && !role.Caught)
                    Utils.Flash(role.Color);

                if (role.TasksDone && !role.Caught)
                    role.CompletedTasks = true;
            }
            else if (__instance.Is(RoleEnum.Revealer))
            {
                var role = Role.GetRole<Revealer>(__instance);

                if (role.TasksLeft == CustomGameOptions.RevealerTasksRemainingAlert && !role.Caught)
                {
                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Revealer))
                        Utils.Flash(role.Color);
                    else if (PlayerControl.LocalPlayer.Is(Faction.Intruder) || PlayerControl.LocalPlayer.Is(Faction.Syndicate) || (PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralKill) &&
                        CustomGameOptions.RevealerRevealsNeutrals))
                    {
                        role.Revealed = true;
                        Utils.Flash(role.Color);
                        var gameObj = new GameObject("RevealerArrow") { layer = 5 };
                        var arrow = gameObj.AddComponent<ArrowBehaviour>();
                        gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                        var renderer = gameObj.AddComponent<SpriteRenderer>();
                        renderer.sprite = AssetManager.GetSprite("Arrow");
                        arrow.image = renderer;
                        Role.LocalRole.AllArrows.Add(role.PlayerId, arrow);
                    }
                }
                else if (role.TasksDone && !role.Caught)
                {
                    role.CompletedTasks = role.TasksDone;

                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Revealer) || PlayerControl.LocalPlayer.Is(Faction.Intruder) || PlayerControl.LocalPlayer.Is(Faction.Syndicate) ||
                        (PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralKill) && CustomGameOptions.RevealerRevealsNeutrals))
                    {
                        Utils.Flash(role.Color);
                    }
                }
            }

            var role1 = Role.GetRole(__instance);

            if (role1.TasksDone)
            {
                if (__instance.Is(RoleEnum.Tracker))
                    ((Tracker)role1).UsesLeft++;
                else if (__instance.Is(RoleEnum.Transporter))
                    ((Transporter)role1).UsesLeft++;
                else if (__instance.Is(RoleEnum.Vigilante))
                    ((Vigilante)role1).UsesLeft++;
                else if (__instance.Is(RoleEnum.Veteran))
                    ((Veteran)role1).UsesLeft++;
                else if (__instance.Is(RoleEnum.Altruist))
                    ((Altruist)role1).UsesLeft++;
                else if (__instance.Is(RoleEnum.Monarch))
                    ((Monarch)role1).UsesLeft++;
                else if (__instance.Is(RoleEnum.Chameleon))
                    ((Chameleon)role1).UsesLeft++;
                else if (__instance.Is(RoleEnum.Engineer))
                    ((Engineer)role1).UsesLeft++;
                else if (__instance.Is(RoleEnum.Retributionist))
                    ((Retributionist)role1).UsesLeft++;
                else if (__instance.Is(RoleEnum.Coroner))
                {
                    if (((Coroner)role1).ReferenceBody != null)
                        ((Coroner)role1).UsesLeft++;
                }
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
                traitorRole.FactionColor = Colors.Intruder;
                traitorRole.Objectives = Role.IntrudersWinCon;
            }
            else if (turnSyndicate)
            {
                traitorRole.Faction = Faction.Syndicate;
                traitorRole.IsSynTraitor = true;
                traitorObj.Color = Colors.Syndicate;
                traitorRole.FactionColor = Colors.Syndicate;
                traitorRole.Objectives = Role.SyndicateWinCon;
            }

            traitorObj.Side = traitorRole.Faction;
            traitorObj.Turned = true;
            traitorObj.Hidden = false;
            traitor.RegenTask();

            foreach (var snitch in Ability.GetAbilities<Snitch>(AbilityEnum.Snitch))
            {
                if (CustomGameOptions.SnitchSeesTraitor)
                {
                    if (snitch.TasksLeft <= CustomGameOptions.SnitchTasksRemaining && traitor == PlayerControl.LocalPlayer)
                    {
                        var gameObj = new GameObject("SnitchArrow") { layer = 5 };
                        var arrow = gameObj.AddComponent<ArrowBehaviour>();
                        gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                        var renderer = gameObj.AddComponent<SpriteRenderer>();
                        renderer.sprite = AssetManager.GetSprite("Arrow");
                        arrow.image = renderer;
                        Role.LocalRole.AllArrows.Add(snitch.PlayerId, arrow);
                    }
                    else if (snitch.TasksDone && snitch.Player == PlayerControl.LocalPlayer)
                    {
                        var gameObj = new GameObject("SnitchEvilArrow") { layer = 5 };
                        var arrow = gameObj.AddComponent<ArrowBehaviour>();
                        gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                        var renderer = gameObj.AddComponent<SpriteRenderer>();
                        renderer.sprite = AssetManager.GetSprite("Arrow");
                        arrow.image = renderer;
                        Role.LocalRole.AllArrows.Add(PlayerControl.LocalPlayer.PlayerId, arrow);
                    }
                }
            }

            foreach (var revealer in Role.GetRoles<Revealer>(RoleEnum.Revealer))
            {
                if (revealer.Revealed && CustomGameOptions.RevealerRevealsTraitor && traitor == PlayerControl.LocalPlayer)
                {
                    var gameObj = new GameObject("RevealerArrow") { layer = 5 };
                    var arrow = gameObj.AddComponent<ArrowBehaviour>();
                    gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                    var renderer = gameObj.AddComponent<SpriteRenderer>();
                    renderer.sprite = AssetManager.GetSprite("Arrow");
                    arrow.image = renderer;
                    Role.LocalRole.AllArrows.Add(revealer.PlayerId, arrow);
                }
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Mystic) && traitor != PlayerControl.LocalPlayer)
                Utils.Flash(Colors.Mystic);

            if (traitor == PlayerControl.LocalPlayer || PlayerControl.LocalPlayer.Is(traitorRole.Faction))
                Utils.Flash(Colors.Traitor);
        }
    }
}