using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Modules;
using Reactor.Utilities;
using System;
using System.Collections;
using Reactor.Utilities.Extensions;
using TownOfUsReworked.Patches;
using UnityEngine;
using Object = UnityEngine.Object;
using TownOfUsReworked.PlayerLayers.Objectifiers;
using AmongUs.GameOptions;
using TownOfUsReworked.Extensions;
using Hazel;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Altruist : CrewRole
    {
        public bool CurrentlyReviving;
        public DeadBody CurrentTarget;
        public bool ReviveUsed;
        public AbilityButton ReviveButton;

        #pragma warning disable
        public static ArrowBehaviour Arrow;
        public static PlayerControl Target;
        #pragma warning restore

        public Altruist(PlayerControl player) : base(player)
        {
            Name = "Altruist";
            StartText = "Sacrifice Yourself To Save Another";
            AbilitiesText = $"- You can revive a dead body at the cost of your own life\n- Reviving someone takes {CustomGameOptions.AltReviveDuration}s\n- If a meeting is called " +
                "during your revive, both you and your target will be pronounced dead";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Altruist : Colors.Crew;
            RoleType = RoleEnum.Altruist;
            RoleAlignment = RoleAlignment.CrewProt;
            AlignmentName = CP;
            InspectorResults = InspectorResults.MeddlesWithDead;
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);

            if (ReviveButton == null)
                ReviveButton = CustomButtons.InstantiateButton();

            ReviveButton.UpdateButton(this, "REVIVE", 0, 1, AssetManager.Revive, AbilityTypes.Dead, "ActionSecondary", !ReviveUsed);

            if (Arrow != null)
            {
                if (LobbyBehaviour.Instance || MeetingHud.Instance || PlayerControl.LocalPlayer.Data.IsDead || Target.Data.IsDead)
                {
                    Arrow.gameObject.Destroy();
                    Target = null;
                }
                else
                    Arrow.target = Target.transform.position;
            }
        }

        public override void ButtonClick(AbilityButton __instance, out bool clicked)
        {
            clicked = false;

            if (__instance == ReviveButton)
            {
                if (Utils.IsTooFar(Player, CurrentTarget))
                    return;

                var playerId = CurrentTarget.ParentId;
                var player = Utils.PlayerById(playerId);
                Utils.Spread(Player, player);
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.AltruistRevive);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(playerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Coroutines.Start(Revive(CurrentTarget, this));
                clicked = true;
            }
        }

        public static IEnumerator Revive(DeadBody target, Altruist role)
        {
            var parentId = target.ParentId;
            var position = target.TruePosition;

            if (AmongUsClient.Instance.AmHost)
                Utils.RpcMurderPlayer(role.Player, role.Player);

            if (PlayerControl.LocalPlayer.PlayerId == parentId)
                Utils.Flash(Colors.Reanimated, "You are being resurrected!");

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

            Utils.BodyById(role.Player.PlayerId).gameObject.Destroy();
            var player = Utils.PlayerById(parentId);
            var targetRole = GetRole(player);
            targetRole.DeathReason = DeathReasonEnum.Revived;
            targetRole.KilledBy = " By " + role.PlayerName;

            foreach (var poisoner in GetRoles<Poisoner>(RoleEnum.Poisoner))
            {
                if (poisoner.PoisonedPlayer == player)
                    poisoner.PoisonedPlayer = null;
            }

            player.Revive();

            if (player.Data.IsImpostor())
                RoleManager.Instance.SetRole(player, RoleTypes.Impostor);
            else
                RoleManager.Instance.SetRole(player, RoleTypes.Crewmate);

            Murder.KilledPlayers.Remove(Murder.KilledPlayers.Find(x => x.PlayerId == player.PlayerId));
            player.NetTransform.SnapTo(new Vector2(position.x, position.y + 0.3636f));

            if (PlayerControl.LocalPlayer == player)
            {
                try
                {
                    //SoundManager.Instance.PlaySound(TownOfUsReworked.ReviveSound, false, 1f);
                } catch {}
            }

            if (SubmergedCompatibility.IsSubmerged() && PlayerControl.LocalPlayer.PlayerId == player.PlayerId)
                SubmergedCompatibility.ChangeFloor(player.transform.position.y > -7);

            if (target != null)
                Object.Destroy(target.gameObject);

            if (player.Is(ObjectifierEnum.Lovers) && CustomGameOptions.BothLoversDie)
            {
                var lover = Objectifier.GetObjectifier<Lovers>(player).OtherLover;
                lover.Revive();
                Murder.KilledPlayers.Remove(Murder.KilledPlayers.Find(x => x.PlayerId == lover.PlayerId));
                Utils.BodyById(lover.PlayerId).gameObject.Destroy();
                var loverRole = GetRole(lover);
                loverRole.DeathReason = DeathReasonEnum.Revived;
                loverRole.KilledBy = " By " + role.PlayerName;
            }

            if (Minigame.Instance)
                Minigame.Instance.Close();

            role.ReviveUsed = true;
            Utils.Spread(role.Player, player);

            if (PlayerControl.LocalPlayer.Is(Faction.Intruder) || PlayerControl.LocalPlayer.Is(Faction.Syndicate) || PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralKill) ||
                PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralEvil) || PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralPros) || PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralNeo))
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