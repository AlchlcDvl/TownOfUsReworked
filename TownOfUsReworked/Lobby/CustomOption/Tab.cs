using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using TownOfUsReworked.Enums;
using Reactor.Utilities;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace TownOfUsReworked.Lobby.CustomOption
{
    public class CustomTabOption : CustomButtonOption
    {
        public List<CustomOption> InternalOptions = new List<CustomOption>();
        public List<OptionBehaviour> OldButtons;
        private CustomHeaderOption Header;
        public CustomHeaderOption Loading;
        private float scrollPerc = 0f;
        private float scrollSize = 0f;

        protected internal CustomTabOption(int id, MultiMenu menu, string name) : base(id, menu, name)
        {
            Do = ToDo;
            Header = new CustomHeaderOption(-1, menu, name);
        }

        public void AddOption(CustomOption option)
        {
            InternalOptions.Add(option);
        }

        public void AddOptions(params CustomOption[] options)
        {
            foreach (var option in options)
                AddOption(option);
        }

        protected internal void ToDo() {}

        protected internal void Cancel(Func<IEnumerator> flashCoro)
        {
            Coroutines.Start(CancelCoro(flashCoro));
        }

        private List<OptionBehaviour> CreateOptions()
        {
            var options = new List<OptionBehaviour>();

            var togglePrefab = Object.FindObjectOfType<ToggleOption>();
            var numberPrefab = Object.FindObjectOfType<NumberOption>();
            var stringPrefab = Object.FindObjectOfType<StringOption>();

            foreach (var option in InternalOptions)
            {
                if (option.Setting != null)
                {
                    //option.Setting.gameObject.SetActive(true);
                    options.Add(option.Setting);
                    continue;
                }

                switch (option.Type)
                {
                    case CustomOptionType.Header:
                        var toggle = Object.Instantiate(togglePrefab, togglePrefab.transform.parent);
                        toggle.transform.GetChild(1).gameObject.SetActive(false);
                        toggle.transform.GetChild(2).gameObject.SetActive(false);
                        option.Setting = toggle;
                        options.Add(toggle);
                        break;
                    case CustomOptionType.Toggle:
                        var toggle2 = Object.Instantiate(togglePrefab, togglePrefab.transform.parent);
                        option.Setting = toggle2;
                        options.Add(toggle2);
                        break;
                    case CustomOptionType.Number:
                        var number = Object.Instantiate(numberPrefab, numberPrefab.transform.parent);
                        option.Setting = number;
                        options.Add(number);
                        break;
                    case CustomOptionType.String:
                        var str = Object.Instantiate(stringPrefab, stringPrefab.transform.parent);
                        option.Setting = str;
                        options.Add(str);
                        break;
                }

                option.OptionCreated();
            }

            return options;
        }

        private IEnumerator FlashWhite()
        {
            yield return null;
        }

        private IEnumerator FlashGreen()
        {
            Setting.Cast<ToggleOption>().TitleText.color = Color.green;
            yield return new WaitForSeconds(0.5f);
            Setting.Cast<ToggleOption>().TitleText.color = Color.white;
        }

        private IEnumerator FlashRed()
        {
            Setting.Cast<ToggleOption>().TitleText.color = Color.red;
            yield return new WaitForSeconds(0.5f);
            Setting.Cast<ToggleOption>().TitleText.color = Color.white;
        }

        protected internal IEnumerator CancelCoro(Func<IEnumerator> flashCoro)
        {
            var __instance = Object.FindObjectOfType<GameOptionsMenu>();

            foreach (var option in InternalOptions.Skip(1))
                option.Setting.gameObject.SetActive(false);

            Loading = (CustomHeaderOption)InternalOptions[0];
            Loading.Setting.Cast<ToggleOption>().TitleText.text = "Loading...";

            __instance.Children = new[] {Loading.Setting};
            yield return new WaitForSeconds(0.5f);

            Loading.Setting.gameObject.SetActive(false);

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
