using System;
using System.Collections.Generic;
using System.Linq;
using TownOfUsReworked.Classes;
using Hazel;
using HarmonyLib;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Modifiers
{
    [HarmonyPatch]
    public abstract class Modifier : PlayerLayer
    {
        public static readonly Dictionary<byte, Modifier> ModifierDictionary = new();
        public static List<Modifier> AllModifiers => ModifierDictionary.Values.ToList();

        protected Modifier(PlayerControl player) : base(player)
        {
            if (ModifierDictionary.ContainsKey(player.PlayerId))
                ModifierDictionary.Remove(player.PlayerId);

            ModifierDictionary.Add(player.PlayerId, this);
            Color = Colors.Modifier;
        }

        public ModifierEnum Type = ModifierEnum.None;
        public string TaskText = "- None";
        public bool Hidden;

        private bool Equals(Modifier other) => Equals(Player, other.Player) && Type == other.Type;

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj.GetType() != typeof(Modifier))
                return false;

            return Equals((Modifier)obj);
        }

        public override int GetHashCode() => HashCode.Combine(Player, (int)Type);

        public static bool operator == (Modifier a, Modifier b)
        {
            if (a is null && b is null)
                return true;

            if (a is null || b is null)
                return false;

            return a.Type == b.Type && a.Player.PlayerId == b.Player.PlayerId;
        }

        public static bool operator != (Modifier a, Modifier b) => !(a == b);

        public static Modifier GetModifier(PlayerControl player) => AllModifiers.Find(x => x.Player == player);

        public static T GetModifier<T>(PlayerControl player) where T : Modifier => GetModifier(player) as T;

        public static Modifier GetModifier(PlayerVoteArea area) => GetModifier(Utils.PlayerByVoteArea(area));
    }
}