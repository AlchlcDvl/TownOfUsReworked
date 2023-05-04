using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;
using Object = UnityEngine.Object;
using TownOfUsReworked.Data;
using TownOfUsReworked.Classes;
using HarmonyLib;

namespace TownOfUsReworked.CustomOptions
{
    [HarmonyPatch]
    public class Export : CustomButtonOption
    {
        public CustomButtonOption Loading;
        public List<OptionBehaviour> OldButtons;
        public List<CustomButtonOption> SlotButtons = new();

        public Export(int id) : base(id, MultiMenu.main, "Save Custom Settings") => Do = ToDo;

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

        public void Cancel(Func<IEnumerator> flashCoro) => Coroutines.Start(CancelCoro(flashCoro));

        public IEnumerator CancelCoro(Func<IEnumerator> flashCoro)
        {
            var __instance = Object.FindObjectOfType<GameOptionsMenu>();

            foreach (var option in SlotButtons.Skip(1))
                option.Setting.gameObject.Destroy();

            Loading = SlotButtons[0];
            Loading.Do = () => {};
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

        public void ToDo()
        {
            SlotButtons.Clear();
            SlotButtons.Add(new CustomButtonOption(1, MultiMenu.external, "Slot 1", delegate { ExportSlot(1); }));
            SlotButtons.Add(new CustomButtonOption(1, MultiMenu.external, "Slot 2", delegate { ExportSlot(2); }));
            SlotButtons.Add(new CustomButtonOption(1, MultiMenu.external, "Slot 3", delegate { ExportSlot(3); }));
            SlotButtons.Add(new CustomButtonOption(1, MultiMenu.external, "Slot 4", delegate { ExportSlot(4); }));
            SlotButtons.Add(new CustomButtonOption(1, MultiMenu.external, "Slot 5", delegate { ExportSlot(5); }));
            SlotButtons.Add(new CustomButtonOption(1, MultiMenu.external, "Slot 6", delegate { ExportSlot(6); }));
            SlotButtons.Add(new CustomButtonOption(1, MultiMenu.external, "Slot 7", delegate { ExportSlot(7); }));
            SlotButtons.Add(new CustomButtonOption(1, MultiMenu.external, "Slot 8", delegate { ExportSlot(8); }));
            SlotButtons.Add(new CustomButtonOption(1, MultiMenu.external, "Slot 9", delegate { ExportSlot(9); }));
            SlotButtons.Add(new CustomButtonOption(1, MultiMenu.external, "Slot 10", delegate { ExportSlot(10); }));
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
                option.transform.localPosition = new Vector3(x, y - (i++ * 0.5f), z);

            __instance.Children = new Il2CppReferenceArray<OptionBehaviour>(options.ToArray());
        }

        private void ExportSlot(int slotId)
        {
            Utils.LogSomething($"Saving Slot - {slotId}");
            var builder = new StringBuilder();

            foreach (var option in AllOptions)
            {
                if (option.Type is CustomOptionType.Button or CustomOptionType.Header or CustomOptionType.Nested)
                    continue;

                builder.AppendLine(option.Name);
                builder.AppendLine(option.Value.ToString());
            }

            var text = Path.Combine(Application.persistentDataPath, $"GameSettings-Slot{slotId}-ToU-Rew-temp");

            try
            {
                File.WriteAllText(text, builder.ToString());
                var text2 = Path.Combine(Application.persistentDataPath, $"GameSettings-Slot{slotId}-ToU-Rew");
                File.Delete(text2);
                File.Move(text, text2);
                Cancel(FlashGreen);
            }
            catch
            {
                Cancel(FlashRed);
            }
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

        private IEnumerator FlashWhite() => null;
    }
}