using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;
using TownOfUsReworked.Data;
using TownOfUsReworked.Classes;
using HarmonyLib;
using System.IO;

namespace TownOfUsReworked.CustomOptions
{
    [HarmonyPatch]
    public class Presets : CustomButtonOption
    {
        public CustomButtonOption Loading;
        public List<OptionBehaviour> OldButtons;
        public List<CustomButtonOption> SlotButtons = new();

        public Presets() : base(-1, MultiMenu.main, "Load Preset Settings") => Do = ToDo;

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

        private void Cancel(Func<IEnumerator> flashCoro) => Coroutines.Start(CancelCoro(flashCoro));

        private IEnumerator CancelCoro(Func<IEnumerator> flashCoro)
        {
            var __instance = Object.FindObjectOfType<GameOptionsMenu>();

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
            SlotButtons.Add(new CustomButtonOption(-1, MultiMenu.external, "Casual", delegate { LoadPreset("Casual", false); }));
            SlotButtons.Add(new CustomButtonOption(-1, MultiMenu.external, "Chaos", delegate { LoadPreset("Chaos", false); }));
            SlotButtons.Add(new CustomButtonOption(-1, MultiMenu.external, "Default", delegate { LoadPreset("Default", true); }));
            SlotButtons.Add(new CustomButtonOption(-1, MultiMenu.external, "Last Used", delegate { LoadPreset("LastUsed", true); }));
            SlotButtons.Add(new CustomButtonOption(-1, MultiMenu.external, "Ranked", delegate { LoadPreset("Ranked", false); }));
            SlotButtons.Add(new CustomButtonOption(-1, MultiMenu.external, "Cancel", delegate { Cancel(FlashWhite); }));
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
                option.transform.localPosition = new(x, y - (i++ * 0.5f), z);

            __instance.Children = new(options.ToArray());
        }

        public void LoadPreset(string presetName, bool data, bool inLobby = false)
        {
            Utils.LogSomething($"Loading - {presetName}");
            string text = null;

            try
            {
                text = data ? File.ReadAllText(Path.Combine(Application.persistentDataPath, $"{presetName}Settings")) : Utils.CreateText(presetName, "Presets");
            }
            catch
            {
                text = "";
            }

            if (string.IsNullOrEmpty(text))
            {
                Cancel(FlashRed);
                Utils.LogSomething("Preset no exist");
                return;
            }

            var splitText = text.Split("\n").ToList();

            while (splitText.Count > 0)
            {
                var option = AllOptions.Find(o => o.Name.Equals(splitText[0].Trim(), StringComparison.Ordinal));
                splitText.RemoveAt(0);

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

            if (!inLobby)
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

        private IEnumerator FlashWhite() => null;
    }
}