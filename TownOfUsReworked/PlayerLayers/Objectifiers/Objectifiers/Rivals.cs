using System.Collections.Generic;
using Hazel;
using UnityEngine;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Objectifiers
{
    public class Rivals : Objectifier
    {
        public PlayerControl OtherRival { get; set; }
        public bool RivalWins { get; set; }

        public Rivals(PlayerControl player) : base(player)
        {
            Name = "Rival";
            SymbolName = "α";
            TaskText = $"- Get {OtherRival.name} killed and then live to the final 2.";
            Color = CustomGameOptions.CustomObjectifierColors ? Colors.Rivals : Colors.Objectifier;
            ObjectifierType = ObjectifierEnum.Rivals;
            ObjectifierDescription = $"You are a Rival! You are in competition with {OtherRival.name}! Kill {OtherRival.name} and survive to the final 2!";
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

            rival1.OtherRival = secondRival;
            rival2.OtherRival = firstRival;

            firstRival.RegenTask();
            secondRival.RegenTask();

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetDuo, SendOption.Reliable);
            writer.Write(firstRival.PlayerId);
            writer.Write(secondRival.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        internal override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (BothRivalsDead())
                return true;

            if (Utils.RivalsWin(Player))
            {
                Wins();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.RivalWin);
                writer.Write(Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            
            if (IsWinningRival())
                return false;

            return true;
        }

        public bool RivalDead()
        {
            PlayerControl rival = OtherRival;

            return !(rival.Data.IsDead || rival.Data.Disconnected);
        }

        public bool IsDeadRival()
        {
            PlayerControl rival = Player;
            
            return !(rival.Data.IsDead || rival.Data.Disconnected);
        }

        public bool BothRivalsDead()
        {
            return IsDeadRival() && RivalDead();
        }

        public bool IsWinningRival()
        {
            return RivalDead() && !IsDeadRival();
        }

        public override void Wins()
        {
            RivalWins = true;
        }
    }
}