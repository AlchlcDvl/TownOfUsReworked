using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using Reactor.Utilities;
using System;
using System.Collections;
using Reactor.Utilities.Extensions;
using TownOfUsReworked.Patches;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Objectifiers;
using TownOfUsReworked.Extensions;
using Hazel;
using TownOfUsReworked.Data;
using TownOfUsReworked.Custom;
using System.Linq;
using Random = UnityEngine.Random;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Altruist : CrewRole
    {
        public bool CurrentlyReviving;
        public DeadBody CurrentTarget;
        public bool ReviveUsed;
        public CustomButton ReviveButton;

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
            Type = LayerEnum.Altruist;
            ReviveButton = new(this, AssetManager.Revive, AbilityTypes.Dead, "ActionSecondary", Revive);
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            ReviveButton.Update("REVIVE", 0, 1, true, !ReviveUsed);
        }

        public override void OnLobby()
        {
            Arrow.gameObject.Destroy();
            Target = null;
        }

        public void Revive()
        {
            if (Utils.IsTooFar(Player, CurrentTarget))
                return;

            var playerId = CurrentTarget.ParentId;
            var player = Utils.PlayerById(playerId);
            Utils.Spread(Player, player);
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.AltruistRevive);
            writer.Write(Player.PlayerId);
            writer.Write(playerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            Coroutines.Start(Revive(CurrentTarget, this));
        }

        public static IEnumerator Revive(DeadBody target, Altruist role)
        {
            var parentId = target.ParentId;
            var position = target.TruePosition;

            if (AmongUsClient.Instance.AmHost)
                Utils.RpcMurderPlayer(role.Player, role.Player);

            if (PlayerControl.LocalPlayer.PlayerId == parentId)
                Utils.Flash(role.Color);

            if (CustomGameOptions.AltruistTargetBody)
                target?.gameObject.Destroy();

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
            var targetRole = GetRole(player);
            targetRole.DeathReason = DeathReasonEnum.Revived;
            targetRole.KilledBy = " By " + role.PlayerName;
            player.Revive();
            player.Data.SetImpostor(player.Data.IsImpostor());

            Murder.KilledPlayers.Remove(Murder.KilledPlayers.Find(x => x.PlayerId == player.PlayerId));
            player.NetTransform.SnapTo(new Vector2(position.x, position.y + 0.3636f));

            if (SubmergedCompatibility.IsSubmerged() && PlayerControl.LocalPlayer == player)
                SubmergedCompatibility.ChangeFloor(player.transform.position.y > -7);

            target?.gameObject.Destroy();

            if (player.Is(ObjectifierEnum.Lovers) && CustomGameOptions.BothLoversDie)
            {
                var lover = player.GetOtherLover();
                lover.Revive();
                Murder.KilledPlayers.Remove(Murder.KilledPlayers.Find(x => x.PlayerId == lover.PlayerId));
                Utils.BodyById(lover.PlayerId).gameObject.Destroy();
                var loverRole = GetRole(lover);
                loverRole.DeathReason = DeathReasonEnum.Revived;
                loverRole.KilledBy = " By " + role.PlayerName;
                Utils.RecentlyKilled.Remove(lover);
            }

            if (player == PlayerControl.LocalPlayer)
            {
                if (Minigame.Instance)
                    Minigame.Instance.Close();

                if (MapBehaviour.Instance)
                    MapBehaviour.Instance.Close();
            }

            if (AmongUsClient.Instance.AmHost)
            {
                if (SetPostmortals.WillBeRevealer == player)
                {
                    var toChooseFrom = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Crew) && x.Data.IsDead && !x.Data.Disconnected && x != player).ToList();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetRevealer, SendOption.Reliable, -1);

                    if (toChooseFrom.Count == 0)
                    {
                        SetPostmortals.WillBeRevealer = null;
                        writer.Write(byte.MaxValue);
                    }
                    else
                    {
                        var rand = Random.RandomRangeInt(0, toChooseFrom.Count);
                        var pc = toChooseFrom[rand];
                        SetPostmortals.WillBeRevealer = pc;
                        writer.Write(pc.PlayerId);
                    }

                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
                else if (SetPostmortals.WillBePhantom == player)
                {
                    var toChooseFrom = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Neutral) && x.Data.IsDead && !x.Data.Disconnected && x != player).ToList();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetPhantom, SendOption.Reliable, -1);

                    if (toChooseFrom.Count == 0)
                    {
                        SetPostmortals.WillBePhantom = null;
                        writer.Write(byte.MaxValue);
                    }
                    else
                    {
                        var rand = Random.RandomRangeInt(0, toChooseFrom.Count);
                        var pc = toChooseFrom[rand];
                        SetPostmortals.WillBePhantom = pc;
                        writer.Write(pc.PlayerId);
                    }

                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
                else if (SetPostmortals.WillBeBanshee == player)
                {
                    var toChooseFrom = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Syndicate) && x.Data.IsDead && !x.Data.Disconnected && x != player).ToList();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetBanshee, SendOption.Reliable, -1);

                    if (toChooseFrom.Count == 0)
                    {
                        SetPostmortals.WillBeBanshee = null;
                        writer.Write(byte.MaxValue);
                    }
                    else
                    {
                        var rand = Random.RandomRangeInt(0, toChooseFrom.Count);
                        var pc = toChooseFrom[rand];
                        SetPostmortals.WillBeBanshee = pc;
                        writer.Write(pc.PlayerId);
                    }

                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
                else if (SetPostmortals.WillBeGhoul == player)
                {
                    var toChooseFrom = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Neutral) && x.Data.IsDead && !x.Data.Disconnected && x != player).ToList();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetGhoul, SendOption.Reliable, -1);

                    if (toChooseFrom.Count == 0)
                    {
                        SetPostmortals.WillBeGhoul = null;
                        writer.Write(byte.MaxValue);
                    }
                    else
                    {
                        var rand = Random.RandomRangeInt(0, toChooseFrom.Count);
                        var pc = toChooseFrom[rand];
                        SetPostmortals.WillBeGhoul = pc;
                        writer.Write(pc.PlayerId);
                    }

                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }

            role.ReviveUsed = true;
            Utils.Spread(role.Player, player);
            Utils.RecentlyKilled.Remove(player);

            if (PlayerControl.LocalPlayer.Is(Faction.Intruder) || PlayerControl.LocalPlayer.Is(Faction.Syndicate) || PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralKill) ||
                PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralEvil) || PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralPros) || PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralNeo))
            {
                var gameObj = new GameObject();
                Arrow = gameObj.AddComponent<ArrowBehaviour>();
                gameObj.transform.parent = player.gameObject.transform;
                var renderer = gameObj.AddComponent<SpriteRenderer>();
                renderer.sprite = AssetManager.Arrow;
                Arrow.image = renderer;
                gameObj.layer = 5;
                Target = player;
                yield return Utils.FlashCoro(role.Color);
            }
        }
    }
}