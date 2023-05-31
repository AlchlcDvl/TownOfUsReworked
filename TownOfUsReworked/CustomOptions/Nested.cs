namespace TownOfUsReworked.CustomOptions
{
    [HarmonyPatch]
    public class CustomNestedOption : CustomOption
    {
        private List<OptionBehaviour> OldButtons = new();
        public List<CustomOption> InternalOptions = new();
        public readonly static List<CustomOption> AllInternalOptions = new();
        public readonly static List<CustomButtonOption> AllCancelButtons = new();
        private readonly CustomButtonOption CancelButton;
        private readonly CustomHeaderOption Header;

        public CustomNestedOption(MultiMenu menu, string name) : base(-1, menu, name, CustomOptionType.Nested, 0)
        {
            InternalOptions = new();
            Header = new(MultiMenu.external, name);
            CancelButton = new(MultiMenu.external, "Cancel", delegate { Cancel(FlashWhite); });
            InternalOptions.Add(Header);
            InternalOptions.Add(CancelButton);
            AllCancelButtons.Add(CancelButton);
        }

        public void ToDo()
        {
            var options = CreateOptions();
            var __instance = UObject.FindObjectOfType<GameOptionsMenu>();
            var y = __instance.GetComponentsInChildren<OptionBehaviour>().Max(option => option.transform.localPosition.y);
            var x = __instance.Children[1].transform.localPosition.x;
            var z = __instance.Children[1].transform.localPosition.z;
            var i = 0;
            OldButtons = __instance.Children.ToList();

            foreach (var option in __instance.Children)
                option.gameObject.SetActive(false);

            foreach (var option in options)
                option.transform.localPosition = new(x, y - (i++ * 0.5f), z);

            __instance.Children = new(options.ToArray());
        }

        public void AddOptions(params CustomOption[] options)
        {
            AllInternalOptions.RemoveRange(InternalOptions);

            foreach (var option in options.Where(x => x.Type != CustomOptionType.Nested))
                InternalOptions.Insert(1, option);

            AllInternalOptions.AddRange(InternalOptions);
        }

        private List<OptionBehaviour> CreateOptions()
        {
            var options = new List<OptionBehaviour>();
            var togglePrefab = UObject.FindObjectOfType<ToggleOption>();
            var numberPrefab = UObject.FindObjectOfType<NumberOption>();
            var keyValPrefab = UObject.FindObjectOfType<KeyValueOption>();

            if (togglePrefab == null)
                Utils.LogSomething("Toggle DNE");

            if (keyValPrefab == null)
                Utils.LogSomething("Key Value DNE");

            if (numberPrefab == null)
                Utils.LogSomething("Number DNE");

            foreach (var option in InternalOptions)
            {
                if (option.Setting != null)
                {
                    option.Setting.gameObject.SetActive(true);
                    options.Add(option.Setting);
                    continue;
                }

                switch (option.Type)
                {
                    case CustomOptionType.Number:
                        var number = UObject.Instantiate(numberPrefab, numberPrefab.transform.parent);
                        option.Setting = number;
                        options.Add(number);
                        break;

                    case CustomOptionType.String:
                        var str = UObject.Instantiate(keyValPrefab, keyValPrefab.transform.parent);
                        option.Setting = str;
                        options.Add(str);
                        break;

                    case CustomOptionType.Toggle:
                    case CustomOptionType.Nested:
                    case CustomOptionType.Button:
                    case CustomOptionType.Header:
                        var toggle = UObject.Instantiate(togglePrefab, togglePrefab.transform.parent);

                        if (option.Type == CustomOptionType.Header)
                        {
                            toggle.transform.GetChild(1).gameObject.SetActive(false);
                            toggle.transform.GetChild(2).gameObject.SetActive(false);
                        }
                        else if (option.Type is CustomOptionType.Button or CustomOptionType.Nested)
                        {
                            toggle.transform.GetChild(2).gameObject.SetActive(false);
                            toggle.transform.GetChild(0).localPosition += new Vector3(1f, 0f, 0f);
                        }

                        option.Setting = toggle;
                        options.Add(toggle);
                        break;
                }

                option.OptionCreated();
            }

            return options;
        }

        public void Cancel(Func<IEnumerator> flashCoro) => Coroutines.Start(CancelCoro(flashCoro));

        private IEnumerator FlashWhite() => null;

        public IEnumerator CancelCoro(Func<IEnumerator> flashCoro)
        {
            var __instance = UObject.FindObjectOfType<GameOptionsMenu>();

            foreach (var option in InternalOptions.Skip(1))
                option.Setting.gameObject.Destroy();

            var Loading = InternalOptions[0];
            Loading.Setting.Cast<ToggleOption>().TitleText.text = "Loading...";
            __instance.Children = new[] { Loading.Setting };
            yield return new WaitForSeconds(0.5f);
            Loading.Setting.gameObject.Destroy();

            foreach (var option in OldButtons)
                option.gameObject.SetActive(true);

            __instance.Children = OldButtons.ToArray();
            yield return new WaitForEndOfFrame();
            yield return flashCoro();
        }

        public override void OptionCreated()
        {
            base.OptionCreated();
            Setting.Cast<ToggleOption>().TitleText.text = Name;
        }
    }
}