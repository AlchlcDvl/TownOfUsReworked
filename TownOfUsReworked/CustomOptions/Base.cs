using System;
using System.Collections.Generic;
using TownOfUsReworked.Data;
using HarmonyLib;

namespace TownOfUsReworked.CustomOptions
{
    [HarmonyPatch]
    public class CustomOption
    {
        public readonly static List<CustomOption> AllOptions = new();
        public readonly int ID;
        public readonly MultiMenu Menu;
        public Func<object, string> Format;
        public string Name;
        protected internal object Value { get; set; }
        protected internal OptionBehaviour Setting { get; set; }
        protected internal CustomOptionType Type { get; set; }
        protected internal RoleEnum ParentRole { get; set; } = RoleEnum.None;
        protected internal ModifierEnum ParentModifier { get; set; } = ModifierEnum.None;
        protected internal AbilityEnum ParentAbility { get; set; } = AbilityEnum.None;
        protected internal ObjectifierEnum ParentObjectifier { get; set; } = ObjectifierEnum.None;
        protected internal List<Map> ParentMaps { get; set; } = new();
        protected internal bool Active { get; set; } = true;
        protected internal List<bool> Actives { get; set; } = new();
        public object DefaultValue { get; set; }
        public static bool LobbyTextScroller { get; set; } = true;

        protected internal CustomOption(int id, MultiMenu menu, string name, CustomOptionType type, object defaultValue, Func<object, string> format = null)
        {
            ID = id;
            Menu = menu;
            Name = name;
            Type = type;
            DefaultValue = Value = defaultValue;
            Format = format ?? (obj => $"{obj}");

            if (Type == CustomOptionType.Button)
                return;

            AllOptions.Add(this);
            Set(Value);
        }

        public override string ToString() => Format(Value);

        public virtual void OptionCreated() => Setting.name = Setting.gameObject.name = Name;

        protected internal void Set(object value, bool SendRpc = true)
        {
            Value = value;

            if (Setting != null && AmongUsClient.Instance.AmHost && SendRpc)
                RPC.SendRPC(this);

            if (Setting is ToggleOption toggle)
            {
                var newValue = (bool) Value;
                toggle.oldValue = newValue;

                if (toggle.CheckMark != null)
                    toggle.CheckMark.enabled = newValue;
            }
            else if (Setting is NumberOption number)
            {
                var newValue = (float) Value;

                number.Value = number.oldValue = newValue;
                number.ValueText.text = ToString();
            }
            else if (Setting is StringOption str)
            {
                var newValue = (int) Value;

                str.Value = str.oldValue = newValue;
                str.ValueText.text = ToString();
            }
        }

        protected internal void SetRole(RoleEnum role) => ParentRole = role;

        protected internal void SetAbility(AbilityEnum ability) => ParentAbility = ability;

        protected internal void SetObjectifier(ObjectifierEnum objectifier) => ParentObjectifier = objectifier;

        protected internal void SetModifier(ModifierEnum modifier) => ParentModifier = modifier;

        protected internal void SetMaps(params Map[] maps) => ParentMaps.AddRange(maps);
    }
}