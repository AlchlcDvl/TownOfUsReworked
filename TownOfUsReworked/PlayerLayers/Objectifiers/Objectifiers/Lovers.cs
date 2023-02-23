using System.Collections.Generic;
using Hazel;
using UnityEngine;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Objectifiers
{
    public class Lovers : Objectifier
    {
        public PlayerControl OtherLover { get; set; }
        public bool LoveWins { get; set; }

        public Lovers(PlayerControl player) : base(player)
        {
            Name = "Lover";
            SymbolName = "♥";
            TaskText = $"- Live to the final 3 with {OtherLover.name}";
            Color = CustomGameOptions.CustomObjectifierColors ? Colors.Lovers : Colors.Objectifier;
            ObjectifierType = ObjectifierEnum.Lovers;
            ObjectifierDescription = $"You are a Lover! You are in love with {OtherLover.name}! Survive to the final 3 with your loved one!";
        }

        public static void Gen(List<PlayerControl> canHaveObjectifiers)
        {
            List<PlayerControl> all = new List<PlayerControl>();

            foreach (var player in canHaveObjectifiers)
                all.Add(player);

            if (all.Count < 5)
                return;

            PlayerControl firstLover = null;
            PlayerControl secondLover = null;
            
            while (firstLover == null || secondLover == null || firstLover == secondLover || (firstLover.GetFaction() == secondLover.GetFaction() && !CustomGameOptions.LoversFaction))
            {
                all.Shuffle();

                var num = Random.RandomRangeInt(0, all.Count);
                firstLover = all[num];

                var num2 = Random.RandomRangeInt(0, all.Count);
                secondLover = all[num2];
            }

            canHaveObjectifiers.Remove(firstLover);
            canHaveObjectifiers.Remove(secondLover);

            var lover1 = new Lovers(firstLover);
            var lover2 = new Lovers(secondLover);

            lover1.OtherLover = secondLover;
            lover2.OtherLover = firstLover;

            firstLover.RegenTask();
            secondLover.RegenTask();

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetCouple, SendOption.Reliable);
            writer.Write(firstLover.PlayerId);
            writer.Write(secondLover.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        internal override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (LoverDead())
                return true;

            if (Utils.LoversWin(Player))
            {
                Wins();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.LoveWin);
                writer.Write(Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }

            return false;
        }

        public bool LoverDead()
        {
            PlayerControl lover1 = Player;
            PlayerControl lover2 = OtherLover;
            
            return lover1 != null && lover2 != null && ((lover1.Data.IsDead && lover1.Data.Disconnected) || (lover2.Data.Disconnected || lover2.Data.IsDead));
        }

        public override void Wins()
        {
            LoveWins = true;
        }
    }
}