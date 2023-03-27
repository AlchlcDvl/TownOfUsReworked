using System;
using System.Collections.Generic;
using System.Linq;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Enums;
using Hazel;
using HarmonyLib;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Abilities
{
    [HarmonyPatch]
    public abstract class Ability : PlayerLayer
    {
        public static readonly Dictionary<byte, Ability> AbilityDictionary = new();
        public static List<Ability> AllAbilities => AbilityDictionary.Values.ToList();

        protected Ability(PlayerControl player) : base(player)
        {
            if (AbilityDictionary.ContainsKey(player.PlayerId))
                AbilityDictionary.Remove(player.PlayerId);

            AbilityDictionary.Add(player.PlayerId, this);
            Color = Colors.Ability;
        }

        protected internal string TaskText = "- None.";
        protected internal AbilityEnum AbilityType = AbilityEnum.None;
        protected internal bool Hidden;

        private bool Equals(Ability other) => Equals(Player, other.Player) && AbilityType == other.AbilityType;

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj.GetType() != typeof(Ability))
                return false;

            return Equals((Ability)obj);
        }

        public override int GetHashCode() => HashCode.Combine(Player, (int)AbilityType);

        public static Ability GetAbility(PlayerControl player) => AllAbilities.Find(x => x.Player == player);

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
            var ability = (T)Activator.CreateInstance(type, new object[] { player });
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetAbility, SendOption.Reliable);
            writer.Write(player.PlayerId);
            writer.Write(id);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            return ability;
        }
    }
}