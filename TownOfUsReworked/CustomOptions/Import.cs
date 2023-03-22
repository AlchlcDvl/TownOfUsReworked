using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;
using Object = UnityEngine.Object;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.CustomOptions
{
    public class Import : CustomButtonOption
    {
        public CustomButtonOption Loading;
        public List<OptionBehaviour> OldButtons;
        public List<CustomButtonOption> SlotButtons = new List<CustomButtonOption>();

        protected internal Import(int id) : base(id, MultiMenu.main, "Load Custom Settings") => Do = ToDo;

        private List<OptionBehaviour> CreateOptions()
        {
            var options = new List<OptionBehaviour>();
            var togglePrefab = Object.FindObjectOfType<ToggleOption>();

            foreach (var button in SlotButtons)
            {
                if (button.Setting != null)
                {
                    button.Setting.gameObject.SetActive(true);
                    options.Add(button.Setting);
                }
                else
                {
                    var toggle = Object.Instantiate(togglePrefab, togglePrefab.transform.parent);
                    toggle.transform.GetChild(2).gameObject.SetActive(false);
                    toggle.transform.GetChild(0).localPosition += new Vector3(1f, 0f, 0f);
                    button.Setting = toggle;
                    button.OptionCreated();
                    options.Add(toggle);
                }
            }

            return options;
        }

        protected internal void Cancel(Func<IEnumerator> flashCoro) => Coroutines.Start(CancelCoro(flashCoro));

        protected internal IEnumerator CancelCoro(Func<IEnumerator> flashCoro)
        {
            var __instance = Object.FindObjectOfType<GameOptionsMenu>();

            foreach (var option in SlotButtons.Skip(1))
                option.Setting.gameObject.Destroy();

            Loading = SlotButtons[0];
            Loading.Do = () => { };
            Loading.Setting.Cast<ToggleOption>().TitleText.text = "Loading...";

            __instance.Children = new[] {Loading.Setting};

            yield return new WaitForSeconds(0.5f);

            Loading.Setting.gameObject.Destroy();

            foreach (var option in OldButtons)
                option.gameObject.SetActive(true);

            __instance.Children = OldButtons.ToArray();

            yield return new WaitForEndOfFrame();
            yield return flashCoro();
        }

        protected internal void ToDo()
        {
            SlotButtons.Clear();
            SlotButtons.Add(new CustomButtonOption(1, MultiMenu.external, "Slot 1", delegate { ImportSlot(1); }));
            SlotButtons.Add(new CustomButtonOption(1, MultiMenu.external, "Slot 2", delegate { ImportSlot(2); }));
            SlotButtons.Add(new CustomButtonOption(1, MultiMenu.external, "Slot 3", delegate { ImportSlot(3); }));
            SlotButtons.Add(new CustomButtonOption(1, MultiMenu.external, "Slot 4", delegate { ImportSlot(4); }));
            SlotButtons.Add(new CustomButtonOption(1, MultiMenu.external, "Slot 5", delegate { ImportSlot(5); }));
            SlotButtons.Add(new CustomButtonOption(1, MultiMenu.external, "Slot 6", delegate { ImportSlot(6); }));
            SlotButtons.Add(new CustomButtonOption(1, MultiMenu.external, "Slot 7", delegate { ImportSlot(7); }));
            SlotButtons.Add(new CustomButtonOption(1, MultiMenu.external, "Slot 8", delegate { ImportSlot(8); }));
            SlotButtons.Add(new CustomButtonOption(1, MultiMenu.external, "Slot 9", delegate { ImportSlot(9); }));
            SlotButtons.Add(new CustomButtonOption(1, MultiMenu.external, "Slot 10", delegate { ImportSlot(10); }));
            SlotButtons.Add(new CustomButtonOption(1, MultiMenu.external, "Cancel", delegate { Cancel(FlashWhite); }));

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
                option.transform.localPosition = new Vector3(x, y - i++ * 0.5f, z);

            __instance.Children = new Il2CppReferenceArray<OptionBehaviour>(options.ToArray());
        }

        private void ImportSlot(int slotId)
        {
            Utils.LogSomething($"Loading Slot - {slotId}");
            string text;

            try
            {
                var path = Path.Combine(Application.persistentDataPath, $"GameSettings-Slot{slotId}-ToU-Rew");
                text = File.ReadAllText(path);
            }
            catch
            {
                Cancel(FlashRed);
                return;
            }

            var splitText = text.Split("\n").ToList();

            while (splitText.Count > 0)
            {
                var name = splitText[0].Trim();
                splitText.RemoveAt(0);
                var option = AllOptions.FirstOrDefault(o => o.Name.Equals(name, StringComparison.Ordinal));

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

                switch (option.Type)
                {
                    case CustomOptionType.Number:
                        option.Set(float.Parse(value), false);
                        break;
                    case CustomOptionType.Toggle:
                        option.Set(bool.Parse(value), false);
                        break;
                    case CustomOptionType.String:
                        option.Set(int.Parse(value), false);
                        break;
                }
            }

            RPC.SendRPC();
            Cancel(FlashGreen);
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

        private IEnumerator FlashWhite()
        {
            yield return null;
        }
    }
}