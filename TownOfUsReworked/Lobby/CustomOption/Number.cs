using System;
using UnityEngine;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.Lobby.CustomOption
{
    public class CustomNumberOption : CustomOption
    {
        protected internal CustomNumberOption(int id, MultiMenu menu, string name, float defaultValue, float min, float max, float increment, Func<object, string> format = null) :
            base(id, menu, name, CustomOptionType.Number, defaultValue, format)
        {
            Min = min;
            Max = max;
            Increment = increment;
        }

        protected internal CustomNumberOption(bool indent, int id, MultiMenu menu, string name, float defaultValue, float min, float max, float increment, Func<object, string> format =
            null) : this(id, menu, name, defaultValue, min, max, increment, format)
        {
            Indent = indent;
        }

        protected float Min { get; set; }
        protected float Max { get; set; }
        protected float Increment { get; set; }

        protected internal float Get()
        {
            return (float)Value;
        }
       
        protected internal void Increase()
        {
            var increment = Increment > 5 && Input.GetKeyInt(KeyCode.LeftShift) ? 5 : Increment;
            Set(Mathf.Clamp(Get() + increment, Min, Max));
        }

        protected internal void Decrease()
        {
            var increment = Increment > 5 && Input.GetKeyInt(KeyCode.LeftShift) ? 5 : Increment;
            Set(Mathf.Clamp(Get() - increment, Min, Max));
        }

        public override void OptionCreated()
        {
            base.OptionCreated();
            var number = Setting.Cast<NumberOption>();

            number.TitleText.text = Name;
            number.ValidRange = new FloatRange(Min, Max);
            number.Increment = Increment;
            number.Value = number.oldValue = Get();
            number.ValueText.text = ToString();
        }
    }
}