using System;
using System.Collections.Generic;
using System.Linq;
using Reactor.Utilities.Extensions;
using TownOfUsReworked.Classes;
using UnityEngine;
using TownOfUsReworked.Enums;
using Hazel;
using HarmonyLib;

namespace TownOfUsReworked.PlayerLayers.Objectifiers
{
    public abstract class Objectifier
    {
        public static readonly Dictionary<byte, Objectifier> ObjectifierDictionary = new Dictionary<byte, Objectifier>();
        public static IEnumerable<Objectifier> AllObjectifiers => ObjectifierDictionary.Values.ToList();

        protected Objectifier(PlayerControl player)
        {
            Player = player;

            if (ObjectifierDictionary.ContainsKey(player.PlayerId))
                ObjectifierDictionary.Remove(player.PlayerId);

            ObjectifierDictionary.Add(player.PlayerId, this);
        }

        protected internal Color Color { get; set; } = Colors.Objectifier;
        protected internal ObjectifierEnum ObjectifierType { get; set; } = ObjectifierEnum.None;
        protected internal string Name { get; set; } = "Objectifierless";
        protected internal string SymbolName { get; set; } = ":";
        protected internal string ObjectifierDescription { get; set; } = "You are an objectifier!";
        protected internal string TaskText { get; set; } = "- None.";
        protected internal bool Hidden { get; set; } = false;

        protected internal string GetColoredSymbol() => $"{ColorString}{SymbolName}</color>";

        public string PlayerName { get; set; }
        private PlayerControl _player { get; set; }

        public PlayerControl Player
        {
            get => _player;
            set
            {
                if (_player != null)
                    _player.NameText().color = Color.white;

                _player = value;
                PlayerName = value.Data.PlayerName;
            }
        }

        public string ColorString => "<color=#" + Color.ToHtmlStringRGBA() + ">";

        private bool Equals(Objectifier other) => Equals(Player, other.Player) && ObjectifierType == other.ObjectifierType;

        internal virtual bool GameEnd(LogicGameFlowNormal __instance) => true;

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