using System;
using System.Collections;
using TownOfUsReworked.Extensions;
using Reactor.Utilities.Extensions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Patches;
using UnityEngine;
using Object = UnityEngine.Object;
using TownOfUsReworked.PlayerLayers.Objectifiers;
using AmongUs.GameOptions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.RetributionistMod
{
    public static class Coroutine
    {
        #pragma warning disable
        public static ArrowBehaviour Arrow;
        public static PlayerControl Target;
        #pragma warning restore

        public static IEnumerator RetributionistRevive(DeadBody target, Retributionist role)
        {
            if (role.RevivedRole?.RoleType != RoleEnum.Altruist)
                yield break;

            var parentId = target.ParentId;
            var position = target.TruePosition;

            if (AmongUsClient.Instance.AmHost)
                Utils.RpcMurderPlayer(role.Player, role.Player);

            if (CustomGameOptions.AltruistTargetBody)
                Utils.BodyById(parentId)?.gameObject.Destroy();

            var startTime = DateTime.UtcNow;

            while (true)
            {
                var now = DateTime.UtcNow;
                var seconds = (now - startTime).TotalSeconds;

                if (seconds < CustomGameOptions.AltReviveDuration)
                    yield return null;
                else
                    break;

                if (MeetingHud.Instance)
                    yield break;
            }

            Utils.BodyById(role.Player.PlayerId)?.gameObject.Destroy();
            var player = Utils.PlayerById(parentId);
            var targetRole = Role.GetRole(player);
            targetRole.DeathReason = DeathReasonEnum.Revived;
            targetRole.KilledBy = " By " + role.PlayerName;

            foreach (var poisoner in Role.GetRoles<Poisoner>(RoleEnum.Poisoner))
            {
                if (poisoner.PoisonedPlayer == player)
                    poisoner.PoisonedPlayer = null;
            }

            player.Revive();

            if (player.Is(Faction.Intruder))
                RoleManager.Instance.SetRole(player, RoleTypes.Impostor);
            else
                RoleManager.Instance.SetRole(player, RoleTypes.Crewmate);

            Murder.KilledPlayers.Remove(Murder.KilledPlayers.Find(x => x.PlayerId == player.PlayerId));
            player.NetTransform.SnapTo(new Vector2(position.x, position.y + 0.3636f));

            if (SubmergedCompatibility.IsSubmerged() && PlayerControl.LocalPlayer.PlayerId == player.PlayerId)
                SubmergedCompatibility.ChangeFloor(player.transform.position.y > -7);

            if (target != null)
                Object.Destroy(target.gameObject);

            if (player.Is(ObjectifierEnum.Lovers) && CustomGameOptions.BothLoversDie)
            {
                var lover = Objectifier.GetObjectifier<Lovers>(player).OtherLover;

                lover.Revive();
                Murder.KilledPlayers.Remove(Murder.KilledPlayers.Find(x => x.PlayerId == lover.PlayerId));
                Utils.BodyById(lover.PlayerId)?.gameObject.Destroy();
                var loverRole = Role.GetRole(lover);
                loverRole.DeathReason = DeathReasonEnum.Revived;
                loverRole.KilledBy = " By " + role.PlayerName;
            }

            if (Minigame.Instance)
                Minigame.Instance.Close();

            role.ReviveUsed = true;
            Utils.Spread(role.Player, player);

            if (PlayerControl.LocalPlayer.Is(Faction.Intruder) || PlayerControl.LocalPlayer.Is(Faction.Syndicate) || (PlayerControl.LocalPlayer.Is(Faction.Neutral) &&
                !PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralBen)))
            {
                var gameObj = new GameObject();
                Arrow = gameObj.AddComponent<ArrowBehaviour>();
                gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                var renderer = gameObj.AddComponent<SpriteRenderer>();
                renderer.sprite = AssetManager.Arrow;
                Arrow.image = renderer;
                gameObj.layer = 5;
                Target = player;
                yield return Utils.FlashCoro(role.Color, "Someone has been revived!");
            }
        }
    }
}