using HarmonyLib;
using Reactor.Utilities;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using Hazel;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.RevealerMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CompleteTask))]
    public static class CompleteTask
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (!__instance.Is(RoleEnum.Revealer))
                return;

            var role = Role.GetRole<Revealer>(__instance);

            if (role.TasksLeft <= CustomGameOptions.RevealerTasksRemainingAlert && !role.Caught)
            {
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Revealer))
                    Utils.Flash(role.Color, "You are almost finished with tasks!");
                else if (PlayerControl.LocalPlayer.Is(Faction.Intruder) || PlayerControl.LocalPlayer.Is(Faction.Syndicate) || (PlayerControl.LocalPlayer.Is(Faction.Neutral) &&
                    CustomGameOptions.RevealerRevealsNeutrals))
                {
                    role.Revealed = true;
                    Utils.Flash(role.Color, "A <color=#D3D3D3FF>Revealer</color> nearly finished with their tasks!");
                    var gameObj = new GameObject();
                    var arrow = gameObj.AddComponent<ArrowBehaviour>();
                    gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                    var renderer = gameObj.AddComponent<SpriteRenderer>();
                    renderer.sprite = AssetManager.Arrow;
                    arrow.image = renderer;
                    gameObj.layer = 5;
                    role.ImpArrows.Add(arrow);
                }
            }
            else if (role.TasksDone && !role.Caught)
            {
                role.CompletedTasks = true;

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Revealer) || PlayerControl.LocalPlayer.Is(Faction.Intruder) || PlayerControl.LocalPlayer.Is(Faction.Syndicate) ||
                    (PlayerControl.LocalPlayer.Is(Faction.Neutral) && CustomGameOptions.RevealerRevealsNeutrals))
                {
                    Utils.Flash(role.Color, "The <color=#D3D3D3FF>Revealer</color> has finished their tasks!");
                }

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.RevealerFinished);
                writer.Write(role.Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
        }
    }
}