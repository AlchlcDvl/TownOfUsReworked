using System;
using System.Collections.Generic;
using System.Linq;
using TownOfUsReworked.Classes;
using Hazel;
using HarmonyLib;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.Patches;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Objectifiers
{
    [HarmonyPatch]
    public abstract class Objectifier : PlayerLayer
    {
        public static readonly Dictionary<byte, Objectifier> ObjectifierDictionary = new();
        public static List<Objectifier> AllObjectifiers => ObjectifierDictionary.Values.ToList();

        #pragma warning disable
        public static bool NobodyWins;

        public static bool LoveWins;
        public static bool RivalWins;
        public static bool TaskmasterWins;
        public static bool CorruptedWins;
        public static bool OverlordWins;
        #pragma warning restore

        public static bool ObjectifierWins => LoveWins || RivalWins || TaskmasterWins || CorruptedWins || OverlordWins;

        protected Objectifier(PlayerControl player) : base(player)
        {
            if (ObjectifierDictionary.ContainsKey(player.PlayerId))
                ObjectifierDictionary.Remove(player.PlayerId);

            ObjectifierDictionary.Add(player.PlayerId, this);
            Color = Colors.Objectifier;
        }

        public ObjectifierEnum Type = ObjectifierEnum.None;
        public string SymbolName = ":";
        public string TaskText = "- None.";
        public bool Hidden;
        public bool Winner;

        public string GetColoredSymbol() => $"{ColorString}{SymbolName}</color>";

        private bool Equals(Objectifier other) => Equals(Player, other.Player) && Type == other.Type;

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj.GetType() != typeof(Objectifier))
                return false;

            return Equals((Objectifier)obj);
        }

        public override int GetHashCode() => HashCode.Combine(Player, (int)Type);

        public static bool operator == (Objectifier a, Objectifier b)
        {
            if (a is null && b is null)
                return true;

            if (a is null || b is null)
                return false;

            return a.Type == b.Type && a.Player.PlayerId == b.Player.PlayerId;
        }

        public static Objectifier GetObjectifierValue(ObjectifierEnum objEnum)
        {
            foreach (var obj in AllObjectifiers)
            {
                if (obj.Type == objEnum)
                    return obj;
            }

            return null;
        }

        public static T GetObjectifierValue<T>(ObjectifierEnum objEnum) where T : Objectifier => GetObjectifierValue(objEnum) as T;

        public static bool operator != (Objectifier a, Objectifier b) => !(a == b);

        public static Objectifier GetObjectifier(PlayerControl player) => AllObjectifiers.Find(x => x.Player == player);

        public static T GetObjectifier<T>(PlayerControl player) where T : Objectifier => GetObjectifier(player) as T;

        public static Objectifier GetObjectifier(PlayerVoteArea area) => GetObjectifier(Utils.PlayerByVoteArea(area));

        public static IEnumerable<Objectifier> GetObjectifiers(ObjectifierEnum objectifiertype) => AllObjectifiers.Where(x => x.Type == objectifiertype);

        public override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected)
                return true;

            if (ConstantVariables.CorruptedWin(Player))
            {
                CorruptedWins = true;

                if (CustomGameOptions.AllCorruptedWin)
                {
                    foreach (var corr in GetObjectifiers(ObjectifierEnum.Corrupted).Cast<Corrupted>())
                        corr.Winner = true;
                }
                else
                    Winner = true;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.CorruptedWin);
                writer.Write(CustomGameOptions.AllCorruptedWin);
                writer.Write(Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if (ConstantVariables.LoversWin(Player))
            {
                LoveWins = true;
                Winner = true;
                GetObjectifier(((Lovers)this).OtherLover).Winner = true;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.LoveWin);
                writer.Write(Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if (ConstantVariables.RivalsWin(Player))
            {
                RivalWins = true;
                Winner = true;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.RivalWin);
                writer.Write(Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if (TasksDone && Type == ObjectifierEnum.Taskmaster)
            {
                TaskmasterWins = true;
                Winner = true;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.TaskmasterWin);
                writer.Write(Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if (MeetingPatches.MeetingCount >= CustomGameOptions.OverlordMeetingWinCount && Type == ObjectifierEnum.Overlord && ((Overlord)this).IsAlive)
            {
                OverlordWins = true;

                foreach (var ov in GetObjectifiers(ObjectifierEnum.Overlord).Cast<Overlord>())
                {
                    if (ov.IsAlive)
                        ov.Winner = true;
                }

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.OverlordWin);
                writer.Write(Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }

            var flag = Player.Is(ObjectifierEnum.Corrupted) || Player.Is(ObjectifierEnum.Allied) || Player.Is(ObjectifierEnum.Overlord) || (Player.Is(ObjectifierEnum.Lovers) &&
                ((Lovers)this).LoversAlive()) || (Player.Is(ObjectifierEnum.Rivals) && ((Rivals)this).RivalDead());
            return !flag;
        }

        [HarmonyPatch]
        public static class CheckEndGame
        {
            [HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.CheckEndCriteria))]
            public static bool Prefix(LogicGameFlowNormal __instance)
            {
                if (ConstantVariables.IsHnS)
                    return true;

                if (!AmongUsClient.Instance.AmHost)
                    return false;

                if (ConstantVariables.NoOneWins)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                    writer.Write((byte)WinLoseRPC.NobodyWins);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    Role.NobodyWins = true;
                    NobodyWins = true;
                    return true;
                }
                else
                {
                    foreach (var obj in AllObjectifiers)
                    {
                        if (!obj.GameEnd(__instance))
                            return false;
                    }
                }

                return ConstantVariables.GameHasEnded;
            }
        }
    }
}