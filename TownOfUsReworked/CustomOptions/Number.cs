using static TownOfUsReworked.Languages.Language;
namespace TownOfUsReworked.CustomOptions
{
    public class CustomNumberOption : CustomOption
    {
        public CustomNumberOption(int id, MultiMenu menu, string name, float defaultValue, float min, float max, float increment, Func<object, object, string> format = null) : base(id,
            menu, name, CustomOptionType.Number, defaultValue)
        {
            Min = min;
            Max = max;
            Increment = increment;
            Format = format ?? Blank;
        }

        private static Func<object, object, string> Blank => (val, _) => $"{val}";

        protected float Min;
        protected float Max;
        protected float Increment;

        public float Get() => (float)Value;

        public void Increase()
        {
            var increment = Input.GetKeyInt(KeyCode.LeftShift) ? Increment / 2 : Increment;

            if (Get() + increment > Max)
                Set(Min);
            else
                Set(Get() + increment);
        }

        public void Decrease()
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
            number.TitleText.text = GetString(Name);
            number.ValidRange = new(Min, Max);
            number.Increment = Increment;
            number.Value = number.oldValue = Get();
            number.ValueText.text = ToString();
        }
    }
}