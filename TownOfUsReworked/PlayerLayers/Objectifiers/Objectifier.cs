using System;
using System.Collections.Generic;
using System.Linq;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Enums;
using Hazel;
using HarmonyLib;

namespace TownOfUsReworked.PlayerLayers.Objectifiers
{
    public class Objectifier : PlayerLayer
    {
        public static readonly Dictionary<byte, Objectifier> ObjectifierDictionary = new Dictionary<byte, Objectifier>();
        public static IEnumerable<Objectifier> AllObjectifiers => ObjectifierDictionary.Values.ToList();

        protected Objectifier(PlayerControl player) : base(player)
        {
            Player = player;

            if (ObjectifierDictionary.ContainsKey(player.PlayerId))
                ObjectifierDictionary.Remove(player.PlayerId);

            ObjectifierDictionary.Add(player.PlayerId, this);
            Color = Colors.Objectifier;
        }

        protected internal ObjectifierEnum ObjectifierType = ObjectifierEnum.None;
        protected internal string SymbolName = ":";
        protected internal string TaskText = "- None.";
        protected internal bool Hidden = false;

        protected internal string GetColoredSymbol() => $"{ColorString}{SymbolName}</color>";

        private bool Equals(Objectifier other) => Equals(Player, other.Player) && ObjectifierType == other.ObjectifierType;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj.GetType() != typeof(Objectifier))
                return false;

            return Equals((Objectifier)obj);
        }

        public override int GetHashCode() => HashCode.Combine(Player, (int)ObjectifierType);

        public static bool operator == (Objectifier a, Objectifier b)
        {
            if (a is null && b is null)
                return true;

            if (a is null || b is null)
                return false;

            return a.ObjectifierType == b.ObjectifierType && a.Player.PlayerId == b.Player.PlayerId;
        }
        
        public static Objectifier GetObjectifierValue(ObjectifierEnum objEnum)
        {
            foreach (var obj in AllObjectifiers)
            {
                if (obj.ObjectifierType == objEnum)
                    return obj;
            }

            return null;
        }
        
        public static T GetObjectifierValue<T>(ObjectifierEnum objEnum) where T : Objectifier => GetObjectifierValue(objEnum) as T;

        public static bool operator != (Objectifier a, Objectifier b) => !(a == b);

        public static Objectifier GetObjectifier(PlayerControl player) => AllObjectifiers.FirstOrDefault(x => x.Player == player);

        public static T GetObjectifier<T>(PlayerControl player) where T : Objectifier => GetObjectifier(player) as T;

        public static Objectifier GetObjectifier(PlayerVoteArea area) => GetObjectifier(Utils.PlayerByVoteArea(area));

        public static IEnumerable<Objectifier> GetObjectifiers(ObjectifierEnum objectifiertype) => AllObjectifiers.Where(x => x.ObjectifierType == objectifiertype);

        public static T GenObjectifier<T>(Type type, PlayerControl player, int id, List<PlayerControl> players = null)
        {
            var objectifier = (T)((object)Activator.CreateInstance(type, new object[] { player }));
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetObjectifier, SendOption.Reliable);
            writer.Write(player.PlayerId);
            writer.Write(id);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            return objectifier;
        }
        
        [HarmonyPatch]
        public static class CheckEndGame
        {
            [HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.CheckEndCriteria))]
            public static bool Prefix(LogicGameFlowNormal __instance)
            {
                if (GameStates.IsHnS)
                    return true;

                if (!AmongUsClient.Instance.AmHost)
                    return false;

                foreach (var obj in AllObjectifiers)
                {
                    if (!obj.GameEnd(__instance))
                        return false;
                }

                return Utils.GameHasEnded();
            }
        }
    }
}