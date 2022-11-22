using Hazel;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using Object = UnityEngine.Object;
using Reactor.Networking.Extensions;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;
using System;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Warper : Role
    {
        public KillButton _warpButton;
        public DateTime LastWarped;
        public bool SyndicateWin;

        public Warper(PlayerControl player) : base(player)
        {
            Name = "Warper";
            ImpostorText = () => "Warp The Crew Away From Each Other";
            TaskText = () => "Separate the Crew";
            Color = CustomGameOptions.CustomSynColors ? Colors.Warper : Colors.Syndicate;
            SubFaction = SubFaction.None;
            RoleType = RoleEnum.Warper;
            Faction = Faction.Syndicate;
            FactionColor = Colors.Syndicate;
            FactionName = "Syndicate";
            AlignmentName = () => "Syndicate (Support)";
            Results = InspResults.TransWarpTeleTask;
            AddToRoleHistory(RoleType);
        }

        public void Warp()
        {
            Dictionary<byte, Vector2> coordinates = GenerateWarpCoordinates();

            unchecked
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Warp,
                    SendOption.Reliable, -1);
                writer.Write((byte)coordinates.Count);

                foreach ((byte key, Vector2 value) in coordinates)
                {
                    writer.Write(key);
                    writer.Write(value);
                }

                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }

            WarpPlayersToCoordinates(coordinates);
        }

        public static void WarpPlayersToCoordinates(Dictionary<byte, Vector2> coordinates)
        {
            if (coordinates.ContainsKey(PlayerControl.LocalPlayer.PlayerId))
            {
                Coroutines.Start(Utils.FlashCoroutine(Colors.Warper));

                if (Minigame.Instance)
                    Minigame.Instance.Close();

                if (PlayerControl.LocalPlayer.inVent)
                {
                    PlayerControl.LocalPlayer.MyPhysics.RpcExitVent(Vent.currentVent.Id);
                    PlayerControl.LocalPlayer.MyPhysics.ExitAllVents();
                }
            }

            foreach ((byte key, Vector2 value) in coordinates)
            {
                PlayerControl player = Utils.PlayerById(key);
                player.transform.position = value;
            }
        }

        private Dictionary<byte, Vector2> GenerateWarpCoordinates()
        {
            List<PlayerControl> targets = PlayerControl.AllPlayerControls.ToArray().Where(player => !player.Data.IsDead &&
                !player.Data.Disconnected).ToList();
            HashSet<Vent> vents = Object.FindObjectsOfType<Vent>().ToHashSet();
            Dictionary<byte, Vector2> coordinates = new Dictionary<byte, Vector2>(targets.Count);

            foreach (PlayerControl target in targets)
            {
                Vent vent = vents.Random();

                Vector3 destination = SendPlayerToVent(vent);
                coordinates.Add(target.PlayerId, destination);
            }

            return coordinates;
        }

        public static Vector3 SendPlayerToVent(Vent vent)
        {
            Vector2 size = vent.GetComponent<BoxCollider2D>().size;
            Vector3 destination = vent.transform.position;
            destination.y += 0.3636f;
            return destination;
        }

        public float WarpTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastWarped;
            var num = CustomGameOptions.WarpCooldown * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
        
        public KillButton WarpButton
        {
            get => _warpButton;
            set
            {
                _warpButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public void Wins()
        {
            SyndicateWin = true;
        }

        public void Loses()
        {
            LostByRPC = true;
        }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (Player.Data.IsDead | Player.Data.Disconnected)
                return true;

            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected &&
                (x.Data.IsImpostor() | x.Is(Faction.Crew) | x.Is(RoleAlignment.NeutralKill) | x.Is(RoleAlignment.NeutralNeo) |
                x.Is(RoleAlignment.NeutralPros))) == 0)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SyndicateWin,
                    SendOption.Reliable, -1);
                writer.Write(Player.PlayerId);
                Wins();
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }

            return false;
        }
    }
}