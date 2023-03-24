using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Reactor.Utilities;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;
using Object = UnityEngine.Object;
using TownOfUsReworked.Enums;
using Reactor.Utilities.Extensions;
using TownOfUsReworked.Classes;
using HarmonyLib;

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

        protected internal CustomNestedOption(int id, MultiMenu menu, string name) : base(id, menu, name, CustomOptionType.Nested, 0)
        {
            InternalOptions = new List<CustomOption>();
            Header = new CustomHeaderOption(-1, MultiMenu.external, name);
            CancelButton = new CustomButtonOption(-1, MultiMenu.external, "Cancel", delegate { Cancel(FlashWhite); });
            InternalOptions.Add(Header);
            InternalOptions.Add(CancelButton);
            AllCancelButtons.Add(CancelButton);
        }

        protected internal void ToDo()
        {
            var options = CreateOptions();
            var __instance = Object.FindObjectOfType<GameOptionsMenu>();
            var y = __instance.GetComponentsInChildren<OptionBehaviour>().Max(option => option.transform.localPosition.y);
            var x = __instance.Children[1].transform.localPosition.x;
            var z = __instance.Children[1].transform.localPosition.z;
            var i = 0;
            OldButtons = __instance.Children.ToList();

            foreach (var option in __instance.Children)
                option.gameObject.SetActive(false);

            foreach (var option in options)
                option.transform.localPosition = new Vector3(x, y - (i++ * 0.5f), z);

            __instance.Children = new Il2CppReferenceArray<OptionBehaviour>(options.ToArray());
        }

        public void AddOptions(params CustomOption[] options)
        {
            AllInternalOptions.RemoveRange(InternalOptions);

            foreach (var option in options)
                InternalOptions.Insert(1, option);

            AllInternalOptions.AddRange(InternalOptions);
        }

        private List<OptionBehaviour> CreateOptions()
        {
            var options = new List<OptionBehaviour>();
            var togglePrefab = Object.FindObjectOfType<ToggleOption>();
            var stringPrefab = Object.FindObjectOfType<StringOption>();
            var numberPrefab = Object.FindObjectOfType<NumberOption>();

            if (togglePrefab == null)
                Utils.LogSomething("Toggle DNE");

            if (stringPrefab == null)
                Utils.LogSomething("String DNE");

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
                        var number = Object.Instantiate(numberPrefab, numberPrefab.transform.parent);
                        option.Setting = number;
                        options.Add(number);
                        break;

                    case CustomOptionType.String:
                        var str = Object.Instantiate(stringPrefab, stringPrefab.transform.parent);
                        option.Setting = str;
                        options.Add(str);
                        break;

                    case CustomOptionType.Toggle:
                    case CustomOptionType.Nested:
                    case CustomOptionType.Button:
                    case CustomOptionType.Header:
                        var toggle = Object.Instantiate(togglePrefab, togglePrefab.transform.parent);

                        if (option.Type == CustomOptionType.Header)
                        {
                            toggle.transform.GetChild(1).gameObject.SetActive(false);
                            toggle.transform.GetChild(2).gameObject.SetActive(false);
                        }
                        else if (option.Type == CustomOptionType.Button || option.Type == CustomOptionType.Nested)
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

        protected internal void Cancel(Func<IEnumerator> flashCoro) => Coroutines.Start(CancelCoro(flashCoro));

        private IEnumerator FlashWhite()
        {
            yield return null;
        }

        protected internal IEnumerator CancelCoro(Func<IEnumerator> flashCoro)
        {
            var __instance = Object.FindObjectOfType<GameOptionsMenu>();

            foreach (var option in InternalOptions.Skip(1))
                option.Setting.gameObject.Destroy();

            var Loading = InternalOptions[0] as CustomHeaderOption;
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