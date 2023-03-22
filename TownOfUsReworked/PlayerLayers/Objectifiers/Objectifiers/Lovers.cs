﻿using System.Collections.Generic;
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

        public Lovers(PlayerControl player) : base(player)
        {
            Name = "Lover";
            SymbolName = "♥";
            TaskText = $"- Live to the final 3 with your Lover";
            Color = CustomGameOptions.CustomObjectifierColors ? Colors.Lovers : Colors.Objectifier;
            ObjectifierType = ObjectifierEnum.Lovers;
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

        public bool LoverDead() => OtherLover == null || OtherLover.Data.IsDead || OtherLover.Data.Disconnected;

        public bool IsDeadLover() => Player == null || Player.Data.IsDead || Player.Data.Disconnected;

        public bool LoversLose() => LoverDead() && IsDeadLover();

        public bool LoversAlive() => OtherLover != null && Player != null && !OtherLover.Data.IsDead && !Player.Data.IsDead && !OtherLover.Data.Disconnected && !Player.Data.Disconnected;
    }
}