using System;
using System.Collections.Generic;
using System.Linq;
using Reactor.Utilities.Extensions;
using TownOfUsReworked.Extensions;
using UnityEngine;
using HarmonyLib;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Modifiers
{
    public abstract class Modifier
    {
        public static readonly Dictionary<byte, Modifier> ModifierDictionary = new Dictionary<byte, Modifier>();
        public static readonly List<KeyValuePair<byte, ModifierEnum>> ModifierHistory = new List<KeyValuePair<byte, ModifierEnum>>();

        protected Modifier(PlayerControl player)
        {
            Player = player;
            ModifierDictionary.Remove(player.PlayerId);
            ModifierDictionary.Add(player.PlayerId, this);
        }

        public static IEnumerable<Modifier> AllModifiers => ModifierDictionary.Values.ToList();

        protected internal Color Color { get; set; }
        protected internal ModifierEnum ModifierType { get; set; }
        protected internal string Name { get; set; }
        protected internal string ModifierDescription { get; set; }
        protected internal string TaskText { get; set; }
        
        protected internal int TasksLeft => Player.Data.Tasks.ToArray().Count(x => !x.Complete);
        protected internal int TotalTasks => Player.Data.Tasks.Count;

        public string PlayerName { get; set; }
        private PlayerControl _player { get; set; }

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

        public bool Local => PlayerControl.LocalPlayer.PlayerId == Player.PlayerId;

        public string ColorString => "<color=#" + Color.ToHtmlStringRGBA() + ">";

        private bool Equals(Modifier other)
        {
            return Equals(Player, other.Player) && ModifierType == other.ModifierType;
        }

        public void AddToModifierHistory(ModifierEnum modifier)
        {
            ModifierHistory.Add(KeyValuePair.Create(_player.PlayerId, modifier));
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

            if (obj.GetType() != typeof(Modifier))
                return false;

            return Equals((Modifier)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Player, (int)ModifierType);
        }

        public static bool operator ==(Modifier a, Modifier b)
        {
            if (a is null && b is null)
                return true;

            if (a is null || b is null)
                return false;

            return a.ModifierType == b.ModifierType && a.Player.PlayerId == b.Player.PlayerId;
        }

        public static bool operator !=(Modifier a, Modifier b)
        {
            return !(a == b);
        }

        public static Modifier GetModifier(PlayerControl player)
        {
            return (from entry in ModifierDictionary where entry.Key == player.PlayerId select entry.Value).FirstOrDefault();
        }

        public static T GetModifier<T>(PlayerControl player) where T : Modifier
        {
            return GetModifier(player) as T;
        }

        public virtual List<PlayerControl> GetTeammates()
        {
            var team = new List<PlayerControl>();
            return team;
        }

        public static Modifier GetModifier(PlayerVoteArea area)
        {
            var player = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(x => x.PlayerId == area.TargetPlayerId);
            return player == null ? null : GetModifier(player);
        }

        [HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.Start))]
        public static class LobbyBehaviour_Start
        {
            private static void Postfix(LobbyBehaviour __instance)
            {
                ModifierDictionary.Clear();
            }
        }
    }
}