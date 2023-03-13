using System;
using System.Collections.Generic;
using System.Linq;
using Reactor.Utilities.Extensions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Enums;
using UnityEngine;
using Hazel;

namespace TownOfUsReworked.PlayerLayers.Abilities
{
    public abstract class Ability
    {
        public static readonly Dictionary<byte, Ability> AbilityDictionary = new Dictionary<byte, Ability>();

        protected Ability(PlayerControl player)
        {
            Player = player;

            if (AbilityDictionary.ContainsKey(player.PlayerId))
                AbilityDictionary.Remove(player.PlayerId);

            AbilityDictionary.Add(player.PlayerId, this);
        }

        public static IEnumerable<Ability> AllAbilities => AbilityDictionary.Values.ToList();

        protected internal string Name { get; set; }
        protected internal string AbilityDescription { get; set; }
        protected internal string TaskText { get; set; }
        protected internal Color Color { get; set; }
        protected internal AbilityEnum AbilityType { get; set; }
        protected internal bool Hidden { get; set; } = false;

        private PlayerControl _player { get; set; }
        public string PlayerName { get; set; }

        public PlayerControl Player
        {
            get => _player;
            set
            {
                if (_player != null)
                    _player.NameText().color = new Color32(255, 255, 255, 255);
                
                _player = value;
                PlayerName = value.Data.PlayerName;
            }
        }
        
        public string ColorString => "<color=#" + Color.ToHtmlStringRGBA() + ">";

        private bool Equals(Ability other) => Equals(Player, other.Player) && AbilityType == other.AbilityType;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj.GetType() != typeof(Ability))
                return false;

            return Equals((Ability)obj);
        }

        public override int GetHashCode() => HashCode.Combine(Player, (int)AbilityType);

        public static Ability GetAbility(PlayerControl player) => AllAbilities.FirstOrDefault(x => x.Player == player);

        public static T GetAbility<T>(PlayerControl player) where T : Ability => GetAbility(player) as T;

        public static Ability GetAbilityValue(AbilityEnum abilityEnum)
        {
            foreach (var ability in AllAbilities)
            {
                if (ability.AbilityType == abilityEnum)
                    return ability;
            }

            return null;
        }

        public static T GetAbilityValue<T>(AbilityEnum abilityEnum) where T : Ability => GetAbilityValue(abilityEnum) as T;

        public static Ability GetAbility(PlayerVoteArea area) => GetAbility(Utils.PlayerByVoteArea(area));

        public static IEnumerable<Ability> GetAbilities(AbilityEnum abilitytype) => AllAbilities.Where(x => x.AbilityType == abilitytype);

        public static bool operator == (Ability a, Ability b)
        {
            if (a is null && b is null)
                return true;

            if (a is null || b is null)
                return false;

            return a.AbilityType == b.AbilityType && a.Player.PlayerId == b.Player.PlayerId;
        }

        public static bool operator != (Ability a, Ability b) => !(a == b);

        public static T GenAbility<T>(Type type, PlayerControl player, int id)
        {
            var ability = (T)((object)Activator.CreateInstance(type, new object[] { player }));
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetAbility, SendOption.Reliable);
            writer.Write(player.PlayerId);
            writer.Write(id);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            return ability;
        }
    }
}