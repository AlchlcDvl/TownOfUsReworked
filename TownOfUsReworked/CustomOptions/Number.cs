using System;
using UnityEngine;
using TownOfUsReworked.Data;
using HarmonyLib;

namespace TownOfUsReworked.CustomOptions
{
    [HarmonyPatch]
    public class CustomNumberOption : CustomOption
    {
        protected internal CustomNumberOption(int id, MultiMenu menu, string name, float defaultValue, float min, float max, float increment, Func<object, string> format = null) :
            base(id, menu, name, CustomOptionType.Number, defaultValue, format)
        {
            Min = min;
            Max = max;
            Increment = increment;
        }

        protected float Min { get; set; }
        protected float Max { get; set; }
        protected float Increment { get; set; }

        protected internal float Get() => (float)Value;

        protected internal void Increase()
        {
            var increment = Input.GetKeyInt(KeyCode.LeftShift) ? Increment / 2 : Increment;

            if (Get() + increment > Max)
                Set(Min);
            else
                Set(Get() + increment);
        }

        protected internal void Decrease()
        {
            var decrement = Input.GetKeyInt(KeyCode.LeftShift) ? Increment / 2 : Increment;

            if (Get() - decrement < Min)
                Set(Max);
            else
                Set(Get() - decrement);
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