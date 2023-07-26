namespace TownOfUsReworked.CustomOptions
{
    public class Preset : CustomButtonOption
    {
        public CustomButtonOption Loading;
        public List<OptionBehaviour> OldButtons;
        public List<CustomButtonOption> SlotButtons = new();

        public Preset() : base(MultiMenu.main, "Load Preset Settings") => Do = ToDo;

        private List<OptionBehaviour> CreateOptions()
        {
            var options = new List<OptionBehaviour>();
            var togglePrefab = UObject.FindObjectOfType<ToggleOption>();

            foreach (var button in SlotButtons)
            {
                if (button.Setting != null)
                {
                    button.Setting.gameObject.SetActive(true);
                    options.Add(button.Setting);
                }
                else
                {
                    var toggle = UObject.Instantiate(togglePrefab, togglePrefab.transform.parent);
                    toggle.transform.GetChild(2).gameObject.SetActive(false);
                    toggle.transform.GetChild(0).localPosition += new Vector3(1f, 0f, 0f);
                    button.Setting = toggle;
                    button.OptionCreated();
                    options.Add(toggle);
                }
            }

            return options;
        }

        private void Cancel(Func<IEnumerator> flashCoro) => Coroutines.Start(CancelCoro(flashCoro));

        private IEnumerator CancelCoro(Func<IEnumerator> flashCoro)
        {
            if (SlotButtons.Count == 0)
                yield break;

            var __instance = UObject.FindObjectOfType<GameOptionsMenu>();

            foreach (var option in SlotButtons.Skip(1))
                option.Setting?.gameObject?.Destroy();

            Loading = SlotButtons[0];
            Loading.Do = () => {};
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

        public void ToDo()
        {
            SlotButtons.Clear();
            Presets.Keys.ToList().ForEach(x => SlotButtons.Add(new(MultiMenu.external, x, delegate { LoadPreset(x); })));
            SlotButtons.Add(new(MultiMenu.external, "Cancel", delegate { Cancel(FlashWhite); }));
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

        public void LoadPreset(string presetName, bool inLobby = false)
        {
            LogSomething($"Loading - {presetName}");
            string text = null;

            try
            {
                text = Presets[presetName];
            }
            catch
            {
                text = "";
            }

            if (string.IsNullOrEmpty(text))
            {
                Cancel(FlashRed);
                LogSomething("Preset no exist");
                return;
            }

            var splitText = text.Split("\n").ToList();

            while (splitText.Count > 0)
            {
                var name = splitText[0].Trim();
                splitText.RemoveAt(0);
                var option = AllOptions.Find(o => o.Name.Equals(name, StringComparison.Ordinal));

                if (option == null)
                {
                    try
                    {
                        splitText.RemoveAt(0);
                    } catch {}

                    continue;
                }

                var value = splitText[0];
                splitText.RemoveAt(0);

                try
                {
                    switch (option.Type)
                    {
                        case CustomOptionType.Number:
                            option.Set(float.Parse(value));
                            break;

                        case CustomOptionType.Toggle:
                            option.Set(bool.Parse(value));
                            break;

                        case CustomOptionType.String:
                            option.Set(int.Parse(value));
                            break;

                        case CustomOptionType.Entry:
                            option.Set((RoleEnum)int.Parse(value));
                            break;

                        case CustomOptionType.Layers:
                            var value2 = splitText[0];
                            splitText.RemoveAt(0);
                            option.Set(int.Parse(value), int.Parse(value2));
                            break;
                    }
                }
                catch (Exception e)
                {
                    LogSomething("Unable to set - " + option.Name + " : " + value + " " + splitText[0] + "\nException: " + e);
                }
            }

            SendOptionRPC();

            if (!inLobby)
                Cancel(FlashGreen);
        }

        private IEnumerator FlashGreen()
        {
            Setting.Cast<ToggleOption>().TitleText.color = UColor.green;
            yield return new WaitForSeconds(0.5f);
            Setting.Cast<ToggleOption>().TitleText.color = UColor.white;
        }

        private IEnumerator FlashRed()
        {
            Setting.Cast<ToggleOption>().TitleText.color = UColor.red;
            yield return new WaitForSeconds(0.5f);
            Setting.Cast<ToggleOption>().TitleText.color = UColor.white;
        }

        private IEnumerator FlashWhite() => null;
    }
}