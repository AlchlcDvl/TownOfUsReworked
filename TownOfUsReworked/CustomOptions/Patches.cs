using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Reactor.Utilities.Extensions;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using static UnityEngine.UI.Button;

namespace TownOfUsReworked.CustomOptions
{
    [HarmonyPatch]
    public static class Patches
    {
        private static readonly string[] Menus = { "Game", "Crew", "Neutral", "Intruder", "Syndicate", "Modifier", "Objectifier", "Ability" };

        #pragma warning disable
        public static Export ExportButton;
        public static Import ImportButton;
        public static Presets PresetButton;
        #pragma warning restore

        private static List<OptionBehaviour> CreateOptions(GameOptionsMenu __instance, MultiMenu type)
        {
            var options = new List<OptionBehaviour>();
            var togglePrefab = Object.FindObjectOfType<ToggleOption>();
            var numberPrefab = Object.FindObjectOfType<NumberOption>();
            var stringPrefab = Object.FindObjectOfType<StringOption>();

            if (type == MultiMenu.main)
            {
                if (ExportButton.Setting != null)
                {
                    ExportButton.Setting.gameObject.SetActive(true);
                    options.Add(ExportButton.Setting);
                }
                else
                {
                    var toggle = Object.Instantiate(togglePrefab, togglePrefab.transform.parent);
                    toggle.transform.GetChild(2).gameObject.SetActive(false);
                    toggle.transform.GetChild(0).localPosition += new Vector3(1f, 0f, 0f);
                    ExportButton.Setting = toggle;
                    ExportButton.OptionCreated();
                    options.Add(toggle);
                }

                if (ImportButton.Setting != null)
                {
                    ImportButton.Setting.gameObject.SetActive(true);
                    options.Add(ImportButton.Setting);
                }
                else
                {
                    var toggle = Object.Instantiate(togglePrefab, togglePrefab.transform.parent);
                    toggle.transform.GetChild(2).gameObject.SetActive(false);
                    toggle.transform.GetChild(0).localPosition += new Vector3(1f, 0f, 0f);
                    ImportButton.Setting = toggle;
                    ImportButton.OptionCreated();
                    options.Add(toggle);
                }

                if (PresetButton.Setting != null)
                {
                    PresetButton.Setting.gameObject.SetActive(true);
                    options.Add(PresetButton.Setting);
                }
                else
                {
                    var toggle = Object.Instantiate(togglePrefab, togglePrefab.transform.parent);
                    toggle.transform.GetChild(2).gameObject.SetActive(false);
                    toggle.transform.GetChild(0).localPosition += new Vector3(1f, 0f, 0f);
                    PresetButton.Setting = toggle;
                    PresetButton.OptionCreated();
                    options.Add(toggle);
                }
            }

            options.AddRange(__instance.Children);

            foreach (var option in CustomOption.AllOptions.Where(x => x.Menu == type))
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

        private static bool OnEnable(OptionBehaviour opt)
        {
            if (opt == ExportButton.Setting)
            {
                ExportButton.OptionCreated();
                return false;
            }

            if (opt == ImportButton.Setting)
            {
                ImportButton.OptionCreated();
                return false;
            }

            if (opt == PresetButton.Setting)
            {
                PresetButton.OptionCreated();
                return false;
            }

            //Works but may need to change to gameObject.name check
            var customOption = CustomOption.AllOptions.Find(option => option.Setting == opt);

            if (customOption == null)
            {
                customOption = ExportButton.SlotButtons.Find(option => option.Setting == opt);

                if (customOption == null)
                {
                    customOption = ImportButton.SlotButtons.Find(option => option.Setting == opt);

                    if (customOption == null)
                    {
                        customOption = PresetButton.SlotButtons.Find(option => option.Setting == opt);

                        if (customOption == null)
                        {
                            customOption = CustomNestedOption.AllCancelButtons.Find(option => option.Setting == opt);

                            if (customOption == null)
                            {
                                customOption = CustomNestedOption.AllInternalOptions.Find(option => option.Setting == opt);

                                if (customOption == null)
                                    return true;
                            }
                        }
                    }
                }
            }

            customOption.OptionCreated();
            return false;
        }

        [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.Start))]
        public static class OptionsMenuBehaviour_Start
        {
            public static void Postfix(GameSettingMenu __instance)
            {
                var obj = __instance.RolesSettingsHightlight.gameObject.transform.parent.parent;
                var diff = (0.7f * Menus.Length) - 2;
                obj.transform.localPosition = new Vector3(obj.transform.localPosition.x - diff, obj.transform.localPosition.y, obj.transform.localPosition.z);
                __instance.GameSettingsHightlight.gameObject.transform.parent.localPosition = new Vector3(obj.transform.localPosition.x, obj.transform.localPosition.y,
                    obj.transform.localPosition.z);
                List<GameObject> menug = new();
                List<SpriteRenderer> menugs = new ();

                for (var index = 0; index < Menus.Length; index++)
                {
                    var touSettings = Object.Instantiate(__instance.RegularGameSettings, __instance.RegularGameSettings.transform.parent);
                    touSettings.SetActive(false);
                    touSettings.name = "ToU-RewSettings" + Menus[index];

                    //Fix for scrollbar (bug in among us)
                    touSettings.GetComponentInChildren<Scrollbar>().parent = touSettings.GetComponentInChildren<Scroller>();

                    var gameGroup = touSettings.transform.FindChild("GameGroup");
                    var title = gameGroup?.FindChild("Text");

                    if (title != null)
                    {
                        title.GetComponent<TextTranslatorTMP>().Destroy();
                        title.GetComponent<TMPro.TextMeshPro>().m_text = $"Town Of Us Reworked {Menus[index]} Settings";
                    }

                    var sliderInner = gameGroup?.FindChild("SliderInner");

                    if (sliderInner != null)
                        sliderInner.GetComponent<GameOptionsMenu>().name = $"ToU-Rew{Menus[index]}OptionsMenu";

                    var ourSettingsButton = Object.Instantiate(obj.gameObject, obj.transform.parent);
                    ourSettingsButton.transform.localPosition = new Vector3(obj.localPosition.x + (0.7f * (index + 1)), obj.localPosition.y, obj.localPosition.z);
                    ourSettingsButton.name = $"ToU-Rew{Menus[index]}Tab";

                    var hatButton = ourSettingsButton.transform.GetChild(0); //TODO: Change to FindChild I guess to be sure
                    var hatIcon = hatButton.GetChild(0);
                    var tabBackground = hatButton.GetChild(1);

                    var renderer = hatIcon.GetComponent<SpriteRenderer>();
                    renderer.sprite = GetSettingSprite(index);
                    var touSettingsHighlight = tabBackground.GetComponent<SpriteRenderer>();
                    menug.Add(touSettings);
                    menugs.Add(touSettingsHighlight);

                    var passiveButton = tabBackground.GetComponent<PassiveButton>();
                    passiveButton.OnClick = new ButtonClickedEvent();
                    passiveButton.OnClick.AddListener(ToggleButton(menug, menugs, index));
                }

                ToggleButtonVoid(menug, menugs, 0);
            }

            public static Sprite GetSettingSprite(int index)
            {
                if (index == 1)
                    return AssetManager.CrewSettingsButton;
                else if (index == 2)
                    return AssetManager.NeutralSettingsButton;
                else if (index == 3)
                    return AssetManager.IntruderSettingsButton;
                else if (index == 4)
                    return AssetManager.SyndicateSettingsButton;
                else if (index == 5)
                    return AssetManager.ModifierSettingsButton;
                else if (index == 6)
                    return AssetManager.ObjectifierSettingsButton;
                else if (index == 7)
                    return AssetManager.AbilitySettingsButton;

                return AssetManager.SettingsButton;
            }
        }

        [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.Update))]
        public static class OptionsMenuBehaviour_Update
        {
            public static void Postfix(GameSettingMenu __instance)
            {
                __instance.RegularGameSettings.SetActive(false);
                __instance.RolesSettings.gameObject.SetActive(false);
                __instance.GameSettingsHightlight.gameObject.SetActive(false);
                __instance.RolesSettingsHightlight.gameObject.SetActive(false);
                __instance.GameSettingsHightlight.enabled = false;
                __instance.RolesSettingsHightlight.enabled = false;
                __instance.RolesSettingsHightlight.gameObject.transform.parent.parent.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
                __instance.GameSettingsHightlight.gameObject.transform.parent.parent.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
                __instance.RolesSettingsHightlight.gameObject.transform.parent.parent.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().gameObject.SetActive(false);
                __instance.GameSettingsHightlight.gameObject.transform.parent.parent.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().gameObject.SetActive(false);
            }
        }

        public static System.Action ToggleButton(List<GameObject> settings, List<SpriteRenderer> highlight, int id) => new(() => ToggleButtonVoid(settings, highlight, id));

        public static void ToggleButtonVoid(List<GameObject> settings, List<SpriteRenderer> highlight, int id)
        {
            foreach (var g in settings)
            {
                g.SetActive(id == settings.IndexOf(g));
                highlight[settings.IndexOf(g)].enabled = id == settings.IndexOf(g);
            }
        }

        [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Start))]
        public static class GameOptionsMenu_Start
        {
            public static bool Prefix(GameOptionsMenu __instance)
            {
                for (var index = 0; index < Menus.Length; index++)
                {
                    if (__instance.name == $"ToU-Rew{Menus[index]}OptionsMenu")
                    {
                        __instance.Children = new Il2CppReferenceArray<OptionBehaviour>(System.Array.Empty<OptionBehaviour>());
                        var childeren = new Transform[__instance.gameObject.transform.childCount];

                        for (var k = 0; k < childeren.Length; k++)
                            childeren[k] = __instance.gameObject.transform.GetChild(k);

                        //TODO: Make a better fix for this for example caching the options or creating it ourself.
                        var startOption = __instance.gameObject.transform.GetChild(0);
                        var customOptions = CreateOptions(__instance, (MultiMenu)index);
                        var (x, y, z) = (startOption.localPosition.x, startOption.localPosition.y, startOption.localPosition.z);

                        for (var k = 0; k < childeren.Length; k++)
                            childeren[k].gameObject.Destroy();

                        var i = 0;

                        foreach (var option in customOptions)
                            option.transform.localPosition = new Vector3(x, y - (i++ * 0.5f), z);

                        __instance.Children = new Il2CppReferenceArray<OptionBehaviour>(customOptions.ToArray());
                        return false;
                    }
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Update))]
        public static class GameOptionsMenu_Update
        {
            public static void Postfix(GameOptionsMenu __instance)
            {
                if (__instance.Children == null || __instance.Children.Length == 0)
                    return;

                var y = __instance.GetComponentsInChildren<OptionBehaviour>().Max(option => option.transform.localPosition.y);
                float x, z;

                if (__instance.Children.Length == 1)
                    (x, z) = (__instance.Children[0].transform.localPosition.x, __instance.Children[0].transform.localPosition.z);
                else
                    (x, z) = (__instance.Children[1].transform.localPosition.x, __instance.Children[1].transform.localPosition.z);

                var i = 0;

                foreach (var option in __instance.Children)
                    option.transform.localPosition = new Vector3(x, y - (i++ * 0.5f), z);
            }
        }

        [HarmonyPatch(typeof(ToggleOption), nameof(ToggleOption.OnEnable))]
        public static class ToggleOption_OnEnable
        {
            public static bool Prefix(ToggleOption __instance) => OnEnable(__instance);
        }

        [HarmonyPatch(typeof(NumberOption), nameof(NumberOption.OnEnable))]
        public static class NumberOption_OnEnable
        {
            public static bool Prefix(NumberOption __instance) => OnEnable(__instance);
        }

        [HarmonyPatch(typeof(StringOption), nameof(StringOption.OnEnable))]
        public static class StringOption_OnEnable
        {
            public static bool Prefix(StringOption __instance) => OnEnable(__instance);
        }

        [HarmonyPatch(typeof(ToggleOption), nameof(ToggleOption.Toggle))]
        public static class ToggleButtonPatch
        {
            public static bool Prefix(ToggleOption __instance)
            {
                var option = CustomOption.AllOptions.Find(option => option.Setting == __instance);
                //Works but may need to change to gameObject.name check

                if (option is CustomToggleOption toggle)
                {
                    toggle.Toggle();
                    return false;
                }

                if (option is CustomNestedOption nested)
                {
                    if (!AmongUsClient.Instance.AmHost)
                        return false;

                    nested.ToDo();
                    return false;
                }

                if (option is CustomHeaderOption)
                    return false;

                if (__instance == ExportButton.Setting)
                {
                    if (!AmongUsClient.Instance.AmHost)
                        return false;

                    ExportButton.Do();
                    return false;
                }

                if (__instance == ImportButton.Setting)
                {
                    if (!AmongUsClient.Instance.AmHost)
                        return false;

                    ImportButton.Do();
                    return false;
                }

                if (__instance == PresetButton.Setting)
                {
                    if (!AmongUsClient.Instance.AmHost)
                        return false;

                    PresetButton.Do();
                    return false;
                }

                var option2 = ExportButton.SlotButtons.Find(option => option.Setting == __instance);

                if (option2 is CustomButtonOption button)
                {
                    if (!AmongUsClient.Instance.AmHost)
                        return false;

                    button.Do();
                    return false;
                }

                var option3 = ImportButton.SlotButtons.Find(option => option.Setting == __instance);

                if (option3 is CustomButtonOption button2)
                {
                    if (!AmongUsClient.Instance.AmHost)
                        return false;

                    button2.Do();
                    return false;
                }

                var option4 = PresetButton.SlotButtons.Find(option => option.Setting == __instance);

                if (option4 is CustomButtonOption button3)
                {
                    if (!AmongUsClient.Instance.AmHost)
                        return false;

                    button3.Do();
                    return false;
                }

                var option5 = CustomNestedOption.AllCancelButtons.Find(option => option.Setting == __instance);

                if (option5 is CustomButtonOption button4)
                {
                    if (!AmongUsClient.Instance.AmHost)
                        return false;

                    button4.Do();
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(NumberOption), nameof(NumberOption.Increase))]
        public static class NumberOptionPatchIncrease
        {
            public static bool Prefix(NumberOption __instance)
            {
                var option = CustomOption.AllOptions.Find(option => option.Setting == __instance);
                //Works but may need to change to gameObject.name check

                if (option is CustomNumberOption number)
                {
                    number.Increase();
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(NumberOption), nameof(NumberOption.Decrease))]
        public static class NumberOptionPatchDecrease
        {
            public static bool Prefix(NumberOption __instance)
            {
                var option = CustomOption.AllOptions.Find(option => option.Setting == __instance);
                //Works but may need to change to gameObject.name check

                if (option is CustomNumberOption number)
                {
                    number.Decrease();
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(StringOption), nameof(StringOption.Increase))]
        public static class StringOptionPatchIncrease
        {
            public static bool Prefix(StringOption __instance)
            {
                var option = CustomOption.AllOptions.Find(option => option.Setting == __instance);
                // Works but may need to change to gameObject.name check

                if (option is CustomStringOption str)
                {
                    str.Increase();
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(StringOption), nameof(StringOption.Decrease))]
        public static class StringOptionPatchDecrease
        {
            public static bool Prefix(StringOption __instance)
            {
                var option = CustomOption.AllOptions.Find(option => option.Setting == __instance);
                //Works but may need to change to gameObject.name check

                if (option is CustomStringOption str)
                {
                    str.Decrease();
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSyncSettings))]
        public static class PlayerControlPatch
        {
            public static void Postfix()
            {
                if (PlayerControl.AllPlayerControls.Count < 1 || !AmongUsClient.Instance || !PlayerControl.LocalPlayer || !AmongUsClient.Instance.AmHost)
                    return;

                RPC.SendRPC();
            }
        }

        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.CoSpawnPlayer))]
        public static class PlayerJoinPatch
        {
            public static void Postfix()
            {
                if (PlayerControl.AllPlayerControls.Count < 1 || !AmongUsClient.Instance || !PlayerControl.LocalPlayer || !AmongUsClient.Instance.AmHost)
                    return;

                RPC.SendRPC();
            }
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public static class HudManagerUpdate
        {
            private const float MinX = -5.233334f, OriginalY = 2.9f, MinY = 3f;
            //Differs to cause excess options to appear cut off to encourage scrolling

            private static Scroller Scroller;
            private static Vector3 LastPosition = new(MinX, MinY);

            public static void Prefix(HudManager __instance)
            {
                if (__instance.GameSettings?.transform == null)
                    return;

                //Scroller disabled
                if (!CustomOption.LobbyTextScroller)
                {
                    //Remove scroller if disabled late
                    if (Scroller != null)
                    {
                        __instance.GameSettings.transform.SetParent(Scroller.transform.parent);
                        __instance.GameSettings.transform.localPosition = new Vector3(MinX, OriginalY);

                        Object.Destroy(Scroller);
                    }

                    return;
                }

                CreateScroller(__instance);

                Scroller.gameObject.SetActive(__instance.GameSettings.gameObject.activeSelf);

                if (!Scroller.gameObject.active)
                    return;

                var rows = __instance.GameSettings.text.Count(c => c == '\n');
                var maxY = Mathf.Max(MinY, (rows * 0.081f) + ((rows - 38) * 0.081f));

                Scroller.ContentYBounds = new FloatRange(MinY, maxY);

                // Prevent scrolling when the player is interacting with a menu
                if (PlayerControl.LocalPlayer?.CanMove != true)
                {
                    __instance.GameSettings.transform.localPosition = LastPosition;
                    return;
                }

                if (__instance.GameSettings.transform.localPosition.x != MinX || __instance.GameSettings.transform.localPosition.y < MinY)
                    return;

                LastPosition = __instance.GameSettings.transform.localPosition;
            }

            private static void CreateScroller(HudManager __instance)
            {
                if (Scroller != null)
                    return;

                Scroller = new GameObject("SettingsScroller").AddComponent<Scroller>();
                Scroller.transform.SetParent(__instance.GameSettings.transform.parent);
                Scroller.gameObject.layer = 5;

                Scroller.transform.localScale = Vector3.one;
                Scroller.allowX = false;
                Scroller.allowY = true;
                Scroller.active = true;
                Scroller.velocity = new Vector2(0, 0);
                Scroller.ScrollbarYBounds = new FloatRange(0, 0);
                Scroller.ContentXBounds = new FloatRange(MinX, MinX);
                Scroller.enabled = true;

                Scroller.Inner = __instance.GameSettings.transform;
                __instance.GameSettings.transform.SetParent(Scroller.transform);
            }
        }
    }
}