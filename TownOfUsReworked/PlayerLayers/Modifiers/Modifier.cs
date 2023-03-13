using System;
using System.Collections.Generic;
using System.Linq;
using Reactor.Utilities.Extensions;
using TownOfUsReworked.Classes;
using UnityEngine;
using TownOfUsReworked.Enums;
using Hazel;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Modifiers
{
    public abstract class Modifier
    {
        public static readonly Dictionary<byte, Modifier> ModifierDictionary = new Dictionary<byte, Modifier>();

        protected Modifier(PlayerControl player)
        {
            Player = player;

            if (ModifierDictionary.ContainsKey(player.PlayerId))
                ModifierDictionary.Remove(player.PlayerId);

            ModifierDictionary.Add(player.PlayerId, this);
        }

        public static IEnumerable<Modifier> AllModifiers => ModifierDictionary.Values.ToList();

        protected internal Color Color { get; set; } = Colors.Modifier;
        protected internal ModifierEnum ModifierType { get; set; } = ModifierEnum.None;
        protected internal string Name { get; set; } = "Modifierless";
        protected internal string ModifierDescription { get; set; } = "You are a modifier!";
        protected internal string TaskText { get; set; } = "- None";
        protected internal bool Hidden { get; set; } = false;

        public string PlayerName { get; set; }
        private PlayerControl _player { get; set; }

        public PlayerControl Player
        {
            get => _player;
            set
            {
                if (_player != null) _player.NameText().color = Color.white;

                _player = value;
                PlayerName = value.Data.PlayerName;
            }
        }

        public string ColorString => "<color=#" + Color.ToHtmlStringRGBA() + ">";

        private bool Equals(Modifier other) => Equals(Player, other.Player) && ModifierType == other.ModifierType;

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

        public override int GetHashCode() => HashCode.Combine(Player, (int)ModifierType);

        public static bool operator == (Modifier a, Modifier b)
        {
            if (a is null && b is null)
                return true;

            if (a is null || b is null)
                return false;

            return a.ModifierType == b.ModifierType && a.Player.PlayerId == b.Player.PlayerId;
        }

        public static bool operator != (Modifier a, Modifier b) => !(a == b);

        public static Modifier GetModifier(PlayerControl player) => AllModifiers.FirstOrDefault(x => x.Player == player);

        public static T GetModifier<T>(PlayerControl player) where T : Modifier => GetModifier(player) as T;

        public static Modifier GetModifier(PlayerVoteArea area) => GetModifier(Utils.PlayerByVoteArea(area));

        public static T GenModifier<T>(Type type, PlayerControl player, int id)
        {
            var modifier = (T)((object)Activator.CreateInstance(type, new object[] { player }));
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetModifier, SendOption.Reliable);
            writer.Write(player.PlayerId);
            writer.Write(id);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            return modifier;
        }
    }
}