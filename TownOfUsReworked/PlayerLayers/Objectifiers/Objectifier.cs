using System;
using System.Collections.Generic;
using System.Linq;
using Reactor.Utilities.Extensions;
using TownOfUsReworked.Extensions;
using UnityEngine;
using TownOfUsReworked.Enums;
using HarmonyLib;

namespace TownOfUsReworked.PlayerLayers.Objectifiers
{
    public abstract class Objectifier
    {
        public static readonly Dictionary<byte, Objectifier> ObjectifierDictionary = new Dictionary<byte, Objectifier>();
        public static IEnumerable<Objectifier> AllObjectifiers => ObjectifierDictionary.Values.ToList();
        public List<KillButton> ExtraButtons = new List<KillButton>();

        protected Objectifier(PlayerControl player)
        {
            Player = player;
            ObjectifierDictionary.Remove(player.PlayerId);
            ObjectifierDictionary.Add(player.PlayerId, this);
        }

        protected internal Color Color { get; set; }
        protected internal ObjectifierEnum ObjectifierType { get; set; }
        protected internal string Name { get; set; }
        protected internal string SymbolName { get; set; }
        protected internal string ObjectifierDescription { get; set; }
        protected internal string TaskText { get; set; }
        protected internal bool Hidden { get; set; } = false;

        public virtual void Loses() {}
        public virtual void Wins() {}

        protected internal string GetColoredSymbol()
        {
            return $"{ColorString}{SymbolName}</color>";
        }

        public string PlayerName { get; set; }
        private PlayerControl _player { get; set; }
        public bool LostByRPC { get; protected set; }

        public PlayerControl Player
        {
            get => _player;
            set
            {
                if (_player != null)
                    _player.nameText().color = Color.white;

                _player = value;
                PlayerName = value.Data.PlayerName;
            }
        }
        

        protected internal int TasksLeft()
        {
            if (Player == null || Player.Data == null)
                return 0;
            
            return Player.Data.Tasks.ToArray().Count(x => !x.Complete);
        }

        protected internal bool TasksDone => TasksLeft() <= 0;

        public bool Local => PlayerControl.LocalPlayer.PlayerId == Player.PlayerId;
        public string ColorString => "<color=#" + Color.ToHtmlStringRGBA() + ">";

        private bool Equals(Objectifier other)
        {
            return Equals(Player, other.Player) && ObjectifierType == other.ObjectifierType;
        }

        internal virtual bool EABBNOODFGL(ShipStatus __instance)
        {
            return true;
        }

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

        public override int GetHashCode()
        {
            return HashCode.Combine(Player, (int)ObjectifierType);
        }

        public static bool operator ==(Objectifier a, Objectifier b)
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
        
        public static T GetObjectifierValue<T>(ObjectifierEnum objEnum) where T : Objectifier
        {
            return GetObjectifierValue(objEnum) as T;
        }

        public static bool operator !=(Objectifier a, Objectifier b)
        {
            return !(a == b);
        }

        public static Objectifier GetObjectifier(PlayerControl player)
        {
            return (from entry in ObjectifierDictionary where entry.Key == player.PlayerId select entry.Value).FirstOrDefault();
        }

        public static T GetObjectifier<T>(PlayerControl player) where T : Objectifier
        {
            return GetObjectifier(player) as T;
        }

        public static Objectifier GetObjectifier(PlayerVoteArea area)
        {
            var player = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(x => x.PlayerId == area.TargetPlayerId);
            return player == null ? null : GetObjectifier(player);
        }

        public static IEnumerable<Objectifier> GetObjectifiers(ObjectifierEnum objectifiertype)
        {
            return AllObjectifiers.Where(x => x.ObjectifierType == objectifiertype);
        }

        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CheckEndCriteria))]
        public static class ShipStatus_KMPKPPGPNIH
        {
            public static bool Prefix(ShipStatus __instance)
            {
                if (!AmongUsClient.Instance.AmHost)
                    return false;

                var result = true;

                foreach (var obj in AllObjectifiers)
                {
                    var objIsEnd = obj.EABBNOODFGL(__instance);

                    if (!objIsEnd)
                        result = false;
                }

                return result;
            }
        }
    }
}