using System;
using System.Collections.Generic;
using System.Linq;
using Reactor.Utilities.Extensions;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Abilities.Abilities;
using UnityEngine;
using HarmonyLib;

namespace TownOfUsReworked.PlayerLayers.Abilities
{
    public abstract class Ability
    {
        public static readonly Dictionary<byte, Ability> AbilityDictionary = new Dictionary<byte, Ability>();
        public static readonly List<KeyValuePair<byte, AbilityEnum>> AbilityHistory = new List<KeyValuePair<byte, AbilityEnum>>();
        public Func<string> TaskText;

        protected Ability(PlayerControl player)
        {
            Player = player;
            AbilityDictionary.Add(player.PlayerId, this);
        }

        public static IEnumerable<Ability> AllAbilities => AbilityDictionary.Values.ToList();
        protected internal string Name { get; set; }
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
        
        protected internal Color Color { get; set; }
        protected internal AbilityEnum AbilityType { get; set; }
        public string ColorString => "<color=#" + Color.ToHtmlStringRGBA() + ">";
        public bool Local => PlayerControl.LocalPlayer.PlayerId == Player.PlayerId;

        private bool Equals(Ability other)
        {
            return Equals(Player, other.Player) && AbilityType == other.AbilityType;
        }

        internal virtual bool EABBNOODFGL(ShipStatus __instance)
        {
            return true;
        }

        public void AddToAbilityHistory(AbilityEnum ability)
        {
            AbilityHistory.Add(KeyValuePair.Create(_player.PlayerId, ability));
        }

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

        public override int GetHashCode()
        {
            return HashCode.Combine(Player, (int)AbilityType);
        }

        public static bool operator ==(Ability a, Ability b)
        {
            if (a is null && b is null)
                return true;

            if (a is null || b is null)
                return false;

            return a.AbilityType == b.AbilityType && a.Player.PlayerId == b.Player.PlayerId;
        }

        public static bool operator !=(Ability a, Ability b)
        {
            return !(a == b);
        }

        public static Ability GetAbility(PlayerControl player)
        {
            return (from entry in AbilityDictionary where entry.Key == player.PlayerId select entry.Value).FirstOrDefault();
        }

        public static T GetAbility<T>(PlayerControl player) where T : Ability
        {
            return GetAbility(player) as T;
        }

        public static Ability GetAbilityValue(AbilityEnum abilityEnum)
        {
            foreach (var ability in AllAbilities)
            {
                if (ability.AbilityType == abilityEnum)
                    return ability;
            }

            return null;
        }

        public static T GetAbilityValue<T>(AbilityEnum abilityEnum) where T : Ability
        {
           return GetAbilityValue(abilityEnum) as T;
        }

        public static Ability GetAbility(PlayerVoteArea area)
        {
            var player = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(x => x.PlayerId == area.TargetPlayerId);
            return player == null ? null : GetAbility(player);
        }

        public static IEnumerable<Ability> GetAbilities(AbilityEnum abilitytype)
        {
            return AllAbilities.Where(x => x.AbilityType == abilitytype);
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
                task.Text = $"{ColorString}Role: {Name}\n{TaskText()}</color>";
                Player.myTasks.Insert(0, task);
                return;
            }

            Player.myTasks.ToArray()[0].Cast<ImportantTextTask>().Text = $"{ColorString}Role: {Name}\n{TaskText()}</color>";
        }

        [HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.Start))]
        public static class LobbyBehaviour_Start
        {
            private static void Postfix(LobbyBehaviour __instance)
            {
                foreach (var ability in AllAbilities.Where(x => x.AbilityType == AbilityEnum.Snitch))
                {
                    var snitch = (Snitch)ability;
                    snitch.ImpArrows.DestroyAll();
                    snitch.SnitchArrows.Values.DestroyAll();
                    snitch.SnitchArrows.Clear();
                }

                AbilityDictionary.Clear();
            }
        }
    }
}