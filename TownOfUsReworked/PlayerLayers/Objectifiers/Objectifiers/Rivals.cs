using System.Collections.Generic;
using Hazel;
using TownOfUsReworked.Patches;
using UnityEngine;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Objectifiers.Objectifiers
{
    public class Rivals : Objectifier
    {
        public PlayerControl OtherRival { get; set; }
        public bool RivalWins { get; set; }

        public Rivals(PlayerControl player) : base(player)
        {
            Name = "Rival";
            SymbolName = "α";
            TaskText = "You have a Rival!";
            Color = CustomGameOptions.CustomObjectifierColors ? Colors.Rivals : Colors.Objectifier;
            ObjectifierType = ObjectifierEnum.Rivals;
        }

        public static void Gen(List<PlayerControl> canHaveObjectifiers)
        {
            List<PlayerControl> all = new List<PlayerControl>();

            foreach (var player in canHaveObjectifiers)
                all.Add(player);

            if (all.Count < 3)
                return;

            PlayerControl firstRival = null;
            PlayerControl secondRival = null;
            
            while (firstRival == null || secondRival == null || firstRival == secondRival || (firstRival.GetFaction() == secondRival.GetFaction() && !CustomGameOptions.RivalsFaction))
            {
                all.Shuffle();

                var num = Random.RandomRangeInt(0, all.Count);
                firstRival = all[num];

                var num2 = Random.RandomRangeInt(0, all.Count);
                secondRival = all[num2];
            }

            canHaveObjectifiers.Remove(firstRival);
            canHaveObjectifiers.Remove(secondRival);

            var rival1 = new Rivals(firstRival);
            var rival2 = new Rivals(secondRival);

            rival1.OtherRival = rival2.Player;
            rival2.OtherRival = rival1.Player;

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetDuo, SendOption.Reliable, -1);
            writer.Write(firstRival.PlayerId);
            writer.Write(secondRival.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (Utils.RivalsWin(ObjectifierType))
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.RivalWin, SendOption.Reliable, -1);
                writer.Write(Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Wins();
                Utils.EndGame();
                return false;
            }

            return false;
        }

        public bool RivalDead()
        {
            PlayerControl rival1 = Player;
            PlayerControl rival2 = OtherRival;
            
            return !(rival1.Data.IsDead || rival1.Data.Disconnected) && (rival2.Data.IsDead || rival2.Data.Disconnected);
        }

        public override void Wins()
        {
            if (RivalDead())
                RivalWins = true;
        }
    }
}