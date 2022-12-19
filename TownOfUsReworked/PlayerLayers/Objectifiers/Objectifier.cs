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
        public static readonly List<KeyValuePair<byte, ObjectifierEnum>> ObjectifierHistory = new List<KeyValuePair<byte, ObjectifierEnum>>();
        public static IEnumerable<Objectifier> AllObjectifiers => ObjectifierDictionary.Values.ToList();

        protected Objectifier(PlayerControl player)
        {
            Player = player;
            ObjectifierDictionary.Add(player.PlayerId, this);
        }

        protected internal Color Color { get; set; }
        protected internal ObjectifierEnum ObjectifierType { get; set; }
        protected internal string Name { get; set; }
        protected internal string SymbolName { get; set; }
        protected internal string ObjectifierDescription { get; set; }
        protected internal string TaskText { get; set; }

        protected internal string GetColoredSymbol()
        {
            return $"{ColorString}{SymbolName}</color>";
        }

        public string PlayerName { get; set; }
        private PlayerControl _player { get; set; }
        public bool LostByRPC { get; protected set; }
        
        protected internal int TasksLeft => Player.Data.Tasks.ToArray().Count(x => !x.Complete);
        protected internal int TotalTasks => Player.Data.Tasks.Count;

        public PlayerControl Player
        {
            get => _player;
            set
            {
                if (_player != null) _player.nameText().color = Color.white;

                _player = value;
                PlayerName = value.Data.PlayerName;
            }
        }

        public void RegenTask()
        {
            bool createTask;

            try
            {
                var firstText = Player.myTasks.ToArray()[0].Cast<ImportantTextTask>();
                createTask = !firstText.Text.Contains("Role:");
            }
            catch (InvalidCastException)
            {
                createTask = true;
            }

            if (createTask)
            {
                var task = new GameObject(Name + "Task").AddComponent<ImportantTextTask>();
                task.transform.SetParent(Player.transform, false);
                task.Text = $"{ColorString}Role: {Name}\n{TaskText}</color>";
                Player.myTasks.Insert(0, task);
                return;
            }

            Player.myTasks.ToArray()[0].Cast<ImportantTextTask>().Text = $"{ColorString}Role: {Name}\n{TaskText}</color>";
        }

        public bool Local => PlayerControl.LocalPlayer.PlayerId == Player.PlayerId;
        public string ColorString => "<color=#" + Color.ToHtmlStringRGBA() + ">";

        private bool Equals(Objectifier other)
        {
            return Equals(Player, other.Player) && ObjectifierType == other.ObjectifierType;
        }

        public void AddToObjectifierHistory(ObjectifierEnum objectifier)
        {
            ObjectifierHistory.Add(KeyValuePair.Create(_player.PlayerId, objectifier));
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

            if (a is null | b is null)
                return false;

            return a.ObjectifierType == b.ObjectifierType && a.Player.PlayerId == b.Player.PlayerId;
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

        public static Objectifier GetObjectifierVote(PlayerVoteArea area)
        {
            var player = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(x => x.PlayerId == area.TargetPlayerId);
            return player == null ? null : GetObjectifier(player);
        }

        public static IEnumerable<Objectifier> GetObjectifiers(ObjectifierEnum objectifiertype)
        {
            return AllObjectifiers.Where(x => x.ObjectifierType == objectifiertype);
        }

        [HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.Start))]
        public static class LobbyBehaviour_Start
        {
            private static void Postfix(LobbyBehaviour __instance)
            {
                ObjectifierDictionary.Clear();
            }
        }
    }
}