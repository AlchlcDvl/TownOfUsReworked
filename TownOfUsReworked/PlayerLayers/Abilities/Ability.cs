using System;
using System.Collections.Generic;
using System.Linq;
using Reactor.Utilities.Extensions;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Enums;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Abilities
{
    public abstract class Ability
    {
        public static readonly Dictionary<byte, Ability> AbilityDictionary = new Dictionary<byte, Ability>();

        protected Ability(PlayerControl player)
        {
            Player = player;
            AbilityDictionary.Remove(player.PlayerId);
            AbilityDictionary.Add(player.PlayerId, this);
        }

        public static IEnumerable<Ability> AllAbilities => AbilityDictionary.Values.ToList();

        protected internal string Name { get; set; }
        protected internal string AbilityDescription { get; set; }
        protected internal string TaskText { get; set; }
        protected internal string CommandInfo { get; set; }
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
                    _player.nameText().color = new Color32(255, 255, 255, 255);
                
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
        
        public string ColorString => "<color=#" + Color.ToHtmlStringRGBA() + ">";

        private bool Equals(Ability other)
        {
            return Equals(Player, other.Player) && AbilityType == other.AbilityType;
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

            if (obj.GetType() != typeof(Ability))
                return false;

            return Equals((Ability)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Player, (int)AbilityType);
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
                task.Text = $"{ColorString}Role: {Name}\n{TaskText}</color>";
                Player.myTasks.Insert(0, task);
                return;
            }

            Player.myTasks.ToArray()[0].Cast<ImportantTextTask>().Text = $"{ColorString}Role: {Name}\n{TaskText}</color>";
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
    }
}