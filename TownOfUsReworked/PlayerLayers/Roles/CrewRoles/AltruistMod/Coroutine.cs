using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Reactor.Utilities.Extensions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.CoronerMod;
using TownOfUsReworked.Patches;
using UnityEngine;
using Object = UnityEngine.Object;
using TownOfUsReworked.PlayerLayers.Objectifiers;
using TownOfUsReworked.PlayerLayers.Objectifiers.Objectifiers;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.AltruistMod
{
    public class Coroutine
    {
        public static ArrowBehaviour Arrow;
        public static PlayerControl Target;
        public static Sprite Sprite => TownOfUsReworked.Arrow;

        public static IEnumerator AltruistRevive(DeadBody target, Altruist role)
        {
            var parentId = target.ParentId;
            var position = target.TruePosition;

            var revived = new List<PlayerControl>();

            if (AmongUsClient.Instance.AmHost)
                Utils.RpcMurderPlayer(role.Player, role.Player);

            if (CustomGameOptions.AltruistTargetBody)
            {
                if (target != null)
                {
                    foreach (DeadBody deadBody in GameObject.FindObjectsOfType<DeadBody>())
                    {
                        if (deadBody.ParentId == target.ParentId)
                            deadBody.gameObject.Destroy();
                    }
                }
            }

            var startTime = DateTime.UtcNow;

            while (true)
            {
                var now = DateTime.UtcNow;
                var seconds = (now - startTime).TotalSeconds;

                if (seconds < CustomGameOptions.ReviveDuration)
                    yield return null;
                else
                    break;

                if (MeetingHud.Instance)
                    yield break;
            }

            foreach (DeadBody deadBody in GameObject.FindObjectsOfType<DeadBody>())
            {
                if (deadBody.ParentId == role.Player.PlayerId)
                    deadBody.gameObject.Destroy();
            }

            var player = Utils.PlayerById(parentId);

            foreach (var poisoner in Role.GetRoles(RoleEnum.Poisoner))
            {
                var poisonerRole = (Poisoner)poisoner;

                if (poisonerRole.PoisonedPlayer == player)
                    poisonerRole.PoisonedPlayer = poisonerRole.Player;
            }

            player.Revive();
            Murder.KilledPlayers.Remove(Murder.KilledPlayers.FirstOrDefault(x => x.PlayerId == player.PlayerId));
            revived.Add(player);
            player.NetTransform.SnapTo(new Vector2(position.x, position.y + 0.3636f));

            if (PlayerControl.LocalPlayer == player)
            {
                try
                {
                    SoundManager.Instance.PlaySound(TownOfUsReworked.ReviveSound, false, 1f);
                } catch {}
            }

            if (SubmergedCompatibility.isSubmerged() && PlayerControl.LocalPlayer.PlayerId == player.PlayerId)
                SubmergedCompatibility.ChangeFloor(player.transform.position.y > -7);

            if (target != null)
                Object.Destroy(target.gameObject);

            if (player.Is(ObjectifierEnum.Lovers) && CustomGameOptions.BothLoversDie)
            {
                var lover = Objectifier.GetObjectifier<Lovers>(player).OtherLover.Player;

                lover.Revive();
                Murder.KilledPlayers.Remove(Murder.KilledPlayers.FirstOrDefault(x => x.PlayerId == lover.PlayerId));
                revived.Add(lover);

                foreach (DeadBody deadBody in GameObject.FindObjectsOfType<DeadBody>())
                {
                    if (deadBody.ParentId == lover.PlayerId)
                        deadBody.gameObject.Destroy();
                }
            }

            if (revived.Any(x => x.AmOwner))
            {
                Minigame.Instance.Close();
                Minigame.Instance.Close();
            }

            if (PlayerControl.LocalPlayer.Data.IsImpostor() | !(PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralBen) |
                PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralEvil)))
            {
                var gameObj = new GameObject();
                Arrow = gameObj.AddComponent<ArrowBehaviour>();
                gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                var renderer = gameObj.AddComponent<SpriteRenderer>();
                renderer.sprite = Sprite;
                Arrow.image = renderer;
                gameObj.layer = 5;
                Target = player;
                yield return Utils.FlashCoroutine(role.Color);
            }
        }
    }
}