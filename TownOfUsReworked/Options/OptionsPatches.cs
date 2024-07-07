// namespace TownOfUsReworked.Options;

// public static class SettingsPatches
// {
//     private static readonly string[] Menus = ["Global", "Crew", "Neutral", "Intruder", "Syndicate", "Modifier", "Objectifier", "Ability", "Role List", "Client"];
//     private static readonly List<int> CreatedPages = [];

//     public static Preset PresetButton;
//     public static CustomButtonOption SaveSettings;

//     public static int SettingsPage;

//     [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.Start))]
//     public static class OptionsMenuBehaviour_Start
//     {
//         public static void Postfix(GameSettingMenu __instance)
//         {
//             __instance.GamePresetsButton.gameObject.SetActive(false);
//             __instance.PresetsTab.gameObject.SetActive(false);

//             if (SettingsPage == 9)
//             {
//                 __instance.GameSettingsButton.gameObject.SetActive(false);
//                 __instance.RoleSettingsButton.gameObject.SetActive(false);
//             }

//             if (SettingsPage is 0 or 9)
//                 __instance.GameSettingsTab.gameObject.SetActive(true);
//             else
//                 __instance.RoleSettingsTab.gameObject.SetActive(true);
//         }
//     }

//     [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.ChangeTab))]
//     public static class OnChangingTabs
//     {
//         public static void Postfix(ref int tabNum)
//         {
//             if (tabNum == 1)
//                 SettingsPage = 0;
//             else if (tabNum == 2 && SettingsPage == 0)
//                 SettingsPage = 1;
//         }
//     }

//     [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.Close))]
//     public static class OptionsMenuBehaviour_Close
//     {
//         public static void Prefix()
//         {
//             if (SettingsPage is 9 or 10 || (SettingsPage == 8 && !IsRoleList) || (SettingsPage == 7 && !CustomGameOptions.EnableAbilities) || (SettingsPage == 6 &&
//                 !CustomGameOptions.EnableObjectifiers) || (SettingsPage == 5 && !CustomGameOptions.EnableModifiers))
//             {
//                 SettingsPage = 0;
//             }

//             PresetButton.SlotButtons.Clear();
//             LobbyConsole.ClientOptionsActive = false;
//             CreatedPages.Clear();
//         }
//     }

//     public static RoleOptionSetting LayersPrefab;
//     public static NumberOption NumberPrefab;
//     public static ToggleOption TogglePrefab;
//     public static ToggleOption ButtonPrefab;
//     public static StringOption StringPrefab;
//     public static CategoryHeaderMasked BaseHeaderPrefab;
//     public static CategoryHeaderEditRole LayerHeaderPrefab;
//     private const float Y = 0.713f;

//     private static Dictionary<CustomOption, MonoBehaviour> CreateOptions(Transform parent = null)
//     {
//         var options = new Dictionary<CustomOption, MonoBehaviour>();

//         if (SettingsPage == 10)
//             return options;

//         var type = (MultiMenu)SettingsPage;
//         parent ??= GameObject.Find($"Main Camera/{(type == MultiMenu.Client ? "ClientOptionsMenu" : "PlayerOptionsMenu(Clone)")}/MainArea/{(type is MultiMenu.Main or MultiMenu.Client ? "GAME SETTINGS" : "ROLES")} TAB/Scroller/SliderInner").transform;

//         if (type == MultiMenu.Main)
//         {
//             if (!SaveSettings.Setting)
//             {
//                 SaveSettings.Setting = UObject.Instantiate(ButtonPrefab, parent);
//                 SaveSettings.OptionCreated();
//             }

//             if (!PresetButton.Setting)
//             {
//                 PresetButton.Setting = UObject.Instantiate(ButtonPrefab, parent);
//                 PresetButton.OptionCreated();
//             }

//             options.Add(SaveSettings, SaveSettings.Setting);
//             options.Add(PresetButton, PresetButton.Setting);
//         }

//         foreach (var option in CustomOption.AllOptions.Where(x => x.Menu == type))
//         {
//             if (!option.Setting)
//             {
//                 MonoBehaviour setting = null;

//                 switch (option.Type)
//                 {
//                     case CustomOptionType.Number:
//                         setting = UObject.Instantiate(NumberPrefab, parent);
//                         break;

//                     case CustomOptionType.String:
//                         setting = UObject.Instantiate(StringPrefab, parent);
//                         break;

//                     case CustomOptionType.Layers:
//                         setting = UObject.Instantiate(LayersPrefab, parent);
//                         break;

//                     case CustomOptionType.Toggle:
//                         setting = UObject.Instantiate(TogglePrefab, parent);
//                         break;

//                     case CustomOptionType.Header:
//                         setting = UObject.Instantiate(((CustomHeaderOption)option).HeaderType == HeaderType.General ? BaseHeaderPrefab : LayerHeaderPrefab, parent);
//                         break;

//                     case CustomOptionType.Entry:
//                         setting = UObject.Instantiate(ButtonPrefab, parent);
//                         break;
//                 }

//                 option.Setting = setting;
//                 option.OptionCreated();
//             }

//             options.Add(option, option.Setting);
//         }

//         return options;
//     }

//     [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Awake))]
//     public static class DefinePrefabs1
//     {
//         public static void Postfix(GameOptionsMenu __instance)
//         {
//             if (!NumberPrefab)
//             {
//                 // Background = 0, Value Text = 1, Title = 2, - = 3, + = 4, Value Box = 5
//                 NumberPrefab = UObject.Instantiate(__instance.numberOptionOrigin, null).DontUnload().DontDestroy();
//                 NumberPrefab.name = "CustomNumbersOptionPrefab";

//                 var background = NumberPrefab.transform.GetChild(0);
//                 background.localPosition += new Vector3(-0.8f, 0f, 0f);
//                 background.localScale += new Vector3(1f, 0f, 0f);

//                 NumberPrefab.transform.GetChild(1).localPosition += new Vector3(1.05f, 0f, 0f);

//                 var title = NumberPrefab.transform.GetChild(2);
//                 title.localPosition += new Vector3(-3.1f, 0f, 0f);
//                 title.GetComponent<RectTransform>().sizeDelta = new(10f, 0.458f);

//                 NumberPrefab.transform.GetChild(3).localPosition += new Vector3(0.6f, 0f, 0f);
//                 NumberPrefab.transform.GetChild(4).localPosition += new Vector3(1.5f, 0f, 0f);

//                 var valueBox = NumberPrefab.transform.GetChild(5);
//                 valueBox.localPosition += new Vector3(1.05f, 0f, 0f);
//                 valueBox.localScale += new Vector3(0.2f, 0f, 0f);

//                 NumberPrefab.GetComponentsInChildren<SpriteRenderer>(true).ForEach(x => x.material.SetInt(PlayerMaterial.MaskLayer, 20));

//                 foreach (var obj in NumberPrefab.GetComponentsInChildren<TextMeshPro>(true))
//                 {
//                     obj.fontMaterial.SetFloat("_StencilComp", 3f);
//                     obj.fontMaterial.SetFloat("_Stencil", 20);
//                 }

//                 NumberPrefab.gameObject.SetActive(false);
//             }

//             if (!StringPrefab)
//             {
//                 // Background = 0, Value Text = 1, Title = 2, - = 3, + = 4, Value Box = 5
//                 StringPrefab = UObject.Instantiate(__instance.stringOptionOrigin, null).DontUnload().DontDestroy();
//                 StringPrefab.name = "CustomStringOptionPrefab";

//                 var background = StringPrefab.transform.GetChild(0);
//                 background.localPosition += new Vector3(-0.8f, 0f, 0f);
//                 background.localScale += new Vector3(1f, 0f, 0f);

//                 StringPrefab.transform.GetChild(1).localPosition += new Vector3(1.05f, 0f, 0f);

//                 var title = StringPrefab.transform.GetChild(2);
//                 title.localPosition += new Vector3(-3.1f, 0f, 0f);
//                 title.GetComponent<RectTransform>().sizeDelta = new(10f, 0.458f);

//                 var minus = StringPrefab.transform.GetChild(3);
//                 minus.GetComponentInChildren<TextMeshPro>().text = "<";
//                 minus.localPosition += new Vector3(0.6f, 0f, 0f);

//                 var plus = StringPrefab.transform.GetChild(4);
//                 plus.GetComponentInChildren<TextMeshPro>().text = ">";
//                 plus.localPosition += new Vector3(1.5f, 0f, 0f);

//                 var valueBox = StringPrefab.transform.GetChild(5);
//                 valueBox.localPosition += new Vector3(1.05f, 0f, 0f);
//                 valueBox.localScale += new Vector3(0.2f, 0f, 0f);

//                 StringPrefab.GetComponentsInChildren<SpriteRenderer>(true).ForEach(x => x.material.SetInt(PlayerMaterial.MaskLayer, 20));

//                 foreach (var obj in StringPrefab.GetComponentsInChildren<TextMeshPro>(true))
//                 {
//                     obj.fontMaterial.SetFloat("_StencilComp", 3f);
//                     obj.fontMaterial.SetFloat("_Stencil", 20);
//                 }

//                 StringPrefab.gameObject.SetActive(false);
//             }

//             if (!TogglePrefab)
//             {
//                 // Title = 0, Toggle = 1, Background = 2
//                 TogglePrefab = UObject.Instantiate(__instance.checkboxOrigin, null).DontUnload().DontDestroy();
//                 TogglePrefab.name = "CustomToggleOptionPrefab";

//                 var title = TogglePrefab.transform.GetChild(0);
//                 title.localPosition += new Vector3(-3.1f, 0f, 0f);
//                 title.GetComponent<RectTransform>().sizeDelta = new(10f, 0.458f);

//                 TogglePrefab.transform.GetChild(1).localPosition += new Vector3(2.2f, 0f, 0f);

//                 var background = TogglePrefab.transform.GetChild(2);
//                 background.localPosition += new Vector3(-0.8f, 0f, 0f);
//                 background.localScale += new Vector3(1f, 0f, 0f);

//                 TogglePrefab.GetComponentsInChildren<SpriteRenderer>(true).ForEach(x => x.material.SetInt(PlayerMaterial.MaskLayer, 20));

//                 foreach (var obj in TogglePrefab.GetComponentsInChildren<TextMeshPro>(true))
//                 {
//                     obj.fontMaterial.SetFloat("_StencilComp", 3f);
//                     obj.fontMaterial.SetFloat("_Stencil", 20);
//                 }

//                 TogglePrefab.gameObject.SetActive(false);
//             }

//             if (!ButtonPrefab)
//             {
//                 // Title = 0, Toggle = 1, Background = 2
//                 ButtonPrefab = UObject.Instantiate(__instance.checkboxOrigin, null).DontUnload().DontDestroy();
//                 ButtonPrefab.name = "ButtonPrefab";

//                 ButtonPrefab.transform.GetChild(0).localPosition += new Vector3(0.3f, 0f, 0f);

//                 var click = ButtonPrefab.transform.GetChild(1);
//                 click.GetChild(2).gameObject.SetActive(false);
//                 click.localPosition += new Vector3(-15f, 0f, 0f);
//                 click.localScale += new Vector3(14f, 0f, 0f);

//                 ButtonPrefab.transform.GetChild(1).GetChild(2).gameObject.SetActive(false);
//                 ButtonPrefab.transform.GetChild(2).gameObject.SetActive(false);

//                 ButtonPrefab.GetComponentsInChildren<SpriteRenderer>(true).ForEach(x => x.material.SetInt(PlayerMaterial.MaskLayer, 20));

//                 foreach (var obj in ButtonPrefab.GetComponentsInChildren<TextMeshPro>(true))
//                 {
//                     obj.fontMaterial.SetFloat("_StencilComp", 3f);
//                     obj.fontMaterial.SetFloat("_Stencil", 20);
//                 }

//                 ButtonPrefab.gameObject.SetActive(false);
//             }

//             if (!BaseHeaderPrefab)
//             {
//                 BaseHeaderPrefab = UObject.Instantiate(__instance.categoryHeaderOrigin, null).DontUnload().DontDestroy();
//                 BaseHeaderPrefab.name = "CustomHeaderOptionBasePrefab";
//                 BaseHeaderPrefab.transform.localScale = new(0.63f, 0.63f, 0.63f);
//                 BaseHeaderPrefab.Background.transform.localScale += new Vector3(0.7f, 0f, 0f);
//                 BaseHeaderPrefab.Background.material.SetInt(PlayerMaterial.MaskLayer, 20);
//                 BaseHeaderPrefab.Divider?.material.SetInt(PlayerMaterial.MaskLayer, 20);
//                 BaseHeaderPrefab.Title.fontMaterial.SetFloat("_StencilComp", 3f);
//                 BaseHeaderPrefab.Title.fontMaterial.SetFloat("_Stencil", 20);

//                 BaseHeaderPrefab.GetComponentsInChildren<SpriteRenderer>(true).ForEach(x => x.material.SetInt(PlayerMaterial.MaskLayer, 20));

//                 foreach (var obj in BaseHeaderPrefab.GetComponentsInChildren<TextMeshPro>(true))
//                 {
//                     obj.fontMaterial.SetFloat("_StencilComp", 3f);
//                     obj.fontMaterial.SetFloat("_Stencil", 20);
//                 }

//                 BaseHeaderPrefab.gameObject.SetActive(false);
//             }
//         }
//     }

//     [HarmonyPatch(typeof(RolesSettingsMenu), nameof(RolesSettingsMenu.Awake))]
//     public static class DefinePrefabs2
//     {
//         public static void Postfix(RolesSettingsMenu __instance)
//         {
//             if (!LayersPrefab)
//             {
//                 // Title = 0, Role # = 1, Chance % = 2, Background = 3, Divider = 4
//                 //            ┗------------┗----------- Value = 0, - = 1, + = 2, Value Box = 3
//                 LayersPrefab = UObject.Instantiate(__instance.roleOptionSettingOrigin, null).DontUnload().DontDestroy();
//                 LayersPrefab.name = "CustomLayersOptionPrefab";

//                 LayersPrefab.GetComponentsInChildren<SpriteRenderer>(true).ForEach(x => x.material.SetInt(PlayerMaterial.MaskLayer, 20));

//                 foreach (var obj in LayersPrefab.GetComponentsInChildren<TextMeshPro>(true))
//                 {
//                     obj.fontMaterial.SetFloat("_StencilComp", 3f);
//                     obj.fontMaterial.SetFloat("_Stencil", 20);
//                 }

//                 LayersPrefab.gameObject.SetActive(false);
//             }

//             if (!LayerHeaderPrefab)
//             {
//                 LayerHeaderPrefab = UObject.Instantiate(__instance.categoryHeaderEditRoleOrigin, null).DontUnload().DontDestroy();
//                 LayerHeaderPrefab.name = "CustomHeaderOptionLayerPrefab";
//                 LayerHeaderPrefab.Background.material.SetInt(PlayerMaterial.MaskLayer, 20);
//                 LayerHeaderPrefab.Divider?.material.SetInt(PlayerMaterial.MaskLayer, 20);
//                 LayerHeaderPrefab.Title.fontMaterial.SetFloat("_StencilComp", 3f);
//                 LayerHeaderPrefab.Title.fontMaterial.SetFloat("_Stencil", 20);

//                 LayerHeaderPrefab.GetComponentsInChildren<SpriteRenderer>(true).ForEach(x => x.material.SetInt(PlayerMaterial.MaskLayer, 20));

//                 foreach (var obj in LayerHeaderPrefab.GetComponentsInChildren<TextMeshPro>(true))
//                 {
//                     obj.fontMaterial.SetFloat("_StencilComp", 3f);
//                     obj.fontMaterial.SetFloat("_Stencil", 20);
//                 }

//                 LayerHeaderPrefab.gameObject.SetActive(false);
//             }
//         }
//     }

//     [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Initialize))]
//     public static class GameOptionsMenu_Initialize
//     {
//         public static bool Prefix(GameOptionsMenu __instance)
//         {
//             if (IsHnS)
//                 return true;

//             __instance.Children = new();
//             __instance.MapPicker.Initialize(20);
//                __instance.MapPicker.SetUpFromData(GameManager.Instance.GameSettingsList.MapNameSetting, 20);

//             // TODO: Make a better fix for this for example caching the options or creating it ourself.
//             // AD Says: Done, kinda.
//             var allOptions = CreateOptions();
//             var (customOptions, behaviours) = (allOptions.Keys.ToList(), allOptions.Values.ToList());
//             var y = Y;

//             for (var i = 0; i < allOptions.Count; i++)
//             {
//                 var isHeader = customOptions[i] is CustomHeaderOption;
//                 behaviours[i].transform.localPosition = new(isHeader ? -0.903f : 0.952f, y, -2f);
//                 behaviours[i].gameObject.SetActive(true);

//                 if (behaviours[i] is OptionBehaviour option)
//                     option.SetClickMask(__instance.ButtonClickMask);

//                 y -= isHeader ? 0.63f : 0.45f;
//             }

//             behaviours.RemoveAll(x => x is CategoryHeaderMasked);
//             behaviours.Insert(0, __instance.MapPicker);
//             __instance.Children = behaviours.Cast<OptionBehaviour>().ToList().ToIl2Cpp();
//             __instance.scrollBar.SetYBoundsMax(-1.65f - y);
//             __instance.InitializeControllerNavigation();
//             return false;
//         }
//     }

//     public static void ToggleTabs(RolesSettingsMenu __instance, int pos)
//     {
//         var previous = SettingsPage;
//         SettingsPage = pos;

//         if (__instance)
//         {
//             var color = GetSettingColor(pos);
//             __instance.quotaHeader.Background.color = color.Shadow();
//             __instance.roleTabs[pos - 1].SelectButton(true);

//             if (previous > 0)
//                 __instance.roleTabs[previous - 1].SelectButton(false);

//             __instance.scrollBar.ScrollToTop();

//             if (CreatedPages.Contains(pos))
//                 return;

//             var allOptions = CreateOptions(__instance.RoleChancesSettings.transform);
//             var (customOptions, behaviours) = (allOptions.Keys.ToList(), allOptions.Values.ToList());
//             var y = 0.62f;

//             for (var i = 0; i < allOptions.Count; i++)
//             {
//                 var isHeader = customOptions[i] is CustomHeaderOption;
//                 var header = isHeader ? (CustomHeaderOption)customOptions[i] : null;
//                 var isGen = header?.HeaderType == HeaderType.General;
//                 var isLayer = customOptions[i] is CustomLayersOption;
//                 behaviours[i].transform.localPosition = new(isLayer ? -0.15f : (isHeader ? (isGen ? -0.623f : 4.986f) : 1.232f), y, -2f);
//                 behaviours[i].gameObject.SetActive(true);

//                 if (behaviours[i] is OptionBehaviour option)
//                     option.SetClickMask(__instance.ButtonClickMask);

//                 y -= isHeader ? (isGen ? 0.63f : 0.6f) : (isLayer ? 0.47f : 0.45f);
//             }

//             behaviours.RemoveAll(x => x is CategoryHeaderMasked);
//             __instance.scrollBar.SetYBoundsMax(-1.65f - y);
//             __instance.InitializeControllerNavigation();
//             CreatedPages.Add(pos);
//         }
//     }

//     public static UColor GetSettingColor(int index) => index switch
//     {
//         1 => CustomColorManager.Crew,
//         2 => CustomColorManager.Neutral,
//         3 => CustomColorManager.Intruder,
//         4 => CustomColorManager.Syndicate,
//         5 => CustomColorManager.Modifier,
//         6 => CustomColorManager.Objectifier,
//         7 => CustomColorManager.Abilities,
//         8 => CustomColorManager.RoleList,
//         _ => UColor.white
//     };

//     [HarmonyPatch(typeof(RolesSettingsMenu), nameof(RolesSettingsMenu.SetQuotaTab))]
//     public static class RolesSettingsMenu_SetQuotaTab
//     {
//         public static bool Prefix(RolesSettingsMenu __instance)
//         {
//             // var num = 0.662f;
//             var tabXPos = -2.69f;
//             __instance.roleTabs = new();
//             __instance.AllButton.gameObject.SetActive(false);

//             for (var i = 1; i < 9; i++)
//             {
//                 var tab = UObject.Instantiate(i % 2 == 0 ? __instance.roleSettingsTabButtonOrigin : __instance.roleSettingsTabButtonOriginImpostor, __instance.tabParent);
//                 tab.transform.localPosition = new(tabXPos, 2.27f, -2f);
//                 var cache = i;
//                 tab.icon.sprite =  GameManager.Instance.GameSettingsList.AllRoles.ToSystem().Find(cat => cat.Role.Role == Icons(i)).Role.RoleIconWhite;
//                 tab.icon.color = GetSettingColor(i).Light();
//                 tab.button.OverrideOnClickListeners(() => ToggleTabs(__instance, cache));
//                 tabXPos += 0.762f;
//                 __instance.roleTabs.Add(tab.Button);
//             }

//             ToggleTabs(__instance, SettingsPage);
//             return false;
//         }

//         private static RoleTypes Icons(int index) => index switch
//         {
//             1 => RoleTypes.GuardianAngel,
//             2 => RoleTypes.Phantom,
//             3 => RoleTypes.Shapeshifter,
//             4 => RoleTypes.Tracker,
//             5 or 6 or 7 => RoleTypes.Noisemaker,
//             8 => RoleTypes.Engineer,
//             _ => RoleTypes.Crewmate
//         };
//     }

//     [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.Update))]
//     public static class FixIssue
//     {
//         public static bool Prefix(GameSettingMenu __instance)
//         {
//             if (Controller.currentTouchType != 0)
//             {
//                 __instance.ToggleLeftSideDarkener(false);
//                 __instance.ToggleRightSideDarkener(false);
//             }

//             return false;
//         }
//     }

//     [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.ValueChanged))]
//     public static class DisableCustomNotify
//     {
//         public static bool Prefix(ref OptionBehaviour option)
//         {
//             var optionn = option;
//             return CustomOption.AllOptions.Any(x => x.Setting == optionn);
//         }
//     }

//     [HarmonyPatch(typeof(RolesSettingsMenu), nameof(RolesSettingsMenu.OpenChancesTab))]
//     public static class RandomNullRefBOOOOOOOO
//     {
//         public static bool Prefix() => false;
//     }

//     private static float Timer;

//     [HarmonyPatch(typeof(RolesSettingsMenu), nameof(RolesSettingsMenu.Update))]
//     public static class RolesSettingsMenu_Update
//     {
//         public static bool Prefix(RolesSettingsMenu __instance)
//         {
//             if (IsHnS || SettingsPage is 0 or 9)
//                 return true;

//             Timer += Time.deltaTime;

//             if (Timer < 0.1f)
//                 return false;

//             Timer = 0f;
//             __instance.quotaHeader.Title.text = $"{Menus[SettingsPage]} Settings";
//             __instance.quotaHeader.Title.color = __instance.quotaHeader.Background.color.Light().Light();
//             var y = 0.65f;

//             foreach (var option in CustomOption.AllOptions)
//             {
//                 if (option != null && option.Setting && option.Setting.gameObject)
//                 {
//                     if (option.Menu != (MultiMenu)SettingsPage || !option.Active)
//                     {
//                         option.Setting.gameObject.SetActive(false);
//                         continue;
//                     }

//                     var isHeader = option is CustomHeaderOption;
//                     var header = isHeader ? (CustomHeaderOption)option : null;
//                     var isGen = header?.HeaderType == HeaderType.General;
//                     var isLayer = option is CustomLayersOption;
//                     option.Setting.gameObject.SetActive(true);
//                     option.Setting.transform.localPosition = new(isLayer ? -0.15f : (isHeader ? (isGen ? -0.623f : 4.986f) : 1.232f), y, -2f);
//                     y -= isHeader ? (isGen ? 0.63f : 0.6f) : (isLayer ? 0.47f : 0.45f);

//                     if (option is CustomLayersOption layer)
//                         layer.UpdateParts();
//                 }
//             }

//             __instance.scrollBar.SetYBoundsMax(-1.65f - y);
//             __instance.InitializeControllerNavigation();
//             return false;
//         }
//     }

//     [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Update))]
//     public static class GameOptionsMenu_Update
//     {
//         public static bool Prefix(GameOptionsMenu __instance)
//         {
//             if (__instance.Children == null || __instance.Children.Count == 0 || IsHnS || SettingsPage is not (0 or 9))
//                 return true;

//             Timer += Time.deltaTime;

//             if (Timer < 0.1f)
//                 return false;

//             Timer = 0f;

//             var y = Y;
//             var list = new List<MonoBehaviour>();

//             if (SettingsPage == 0)
//             {
//                 SaveSettings.Setting.gameObject.SetActive(true);
//                 SaveSettings.Setting.transform.localPosition = new(0.952f, y, -2f);
//                 y -= 0.45f;
//                 list.Add(SaveSettings.Setting);

//                 PresetButton.Setting.gameObject.SetActive(true);
//                 PresetButton.Setting.transform.localPosition = new(0.952f, y, -2f);
//                 y -= 0.45f;
//                 list.Add(PresetButton.Setting);
//             }

//             foreach (var option in CustomOption.AllOptions)
//             {
//                 if (option != null && option.Setting && option.Setting.gameObject)
//                 {
//                     if (option.Menu != (MultiMenu)SettingsPage || !option.Active)
//                     {
//                         option.Setting.gameObject.SetActive(false);
//                         continue;
//                     }

//                     var isHeader = option is CustomHeaderOption;
//                     option.Setting.gameObject.SetActive(true);
//                     option.Setting.transform.localPosition = new(isHeader ? -0.903f : 0.952f, y, -2f);
//                     list.Add(option.Setting);
//                     y -= isHeader ? 0.63f : 0.45f;
//                 }
//             }

//             list.RemoveAll(x => x is CategoryHeaderMasked);
//             list.Insert(0, __instance.MapPicker);
//             __instance.Children = list.Cast<OptionBehaviour>().ToList().ToIl2Cpp();
//             __instance.scrollBar.SetYBoundsMax(-1.65f - y);
//             __instance.InitializeControllerNavigation();
//             return false;
//         }
//     }

//     private static bool Initialize(OptionBehaviour opt)
//     {
//         if (opt == PresetButton.Setting)
//         {
//             PresetButton.OptionCreated();
//             return false;
//         }

//         if (opt == SaveSettings.Setting)
//         {
//             SaveSettings.OptionCreated();
//             return false;
//         }

//         var customOption = (CustomOption.AllOptions.Find(option => option.Setting == opt)
//             ?? PresetButton.SlotButtons.Find(option => option.Setting == opt))
//             ?? RoleListEntryOption.EntryButtons.Find(option => option.Setting == opt);

//         if (customOption == null)
//             return true;

//         customOption.OptionCreated();
//         return false;
//     }

//     [HarmonyPatch(typeof(ToggleOption), nameof(ToggleOption.Initialize))]
//     public static class ToggleOption_Initialize
//     {
//         public static bool Prefix(ToggleOption __instance) => Initialize(__instance);
//     }

//     [HarmonyPatch(typeof(NumberOption), nameof(NumberOption.Initialize))]
//     public static class NumberOption_Initialize
//     {
//         public static bool Prefix(NumberOption __instance) => Initialize(__instance);
//     }

//     [HarmonyPatch(typeof(StringOption), nameof(StringOption.Initialize))]
//     public static class StringOption_Initialize
//     {
//         public static bool Prefix(StringOption __instance) => Initialize(__instance);
//     }

//     [HarmonyPatch(typeof(ToggleOption), nameof(ToggleOption.Toggle))]
//     public static class ToggleButtonPatch
//     {
//         public static bool Prefix(ToggleOption __instance)
//         {
//             var option = CustomOption.AllOptions.Find(option => option.Setting == __instance);

//             if (option is CustomToggleOption toggle)
//             {
//                 toggle.Toggle();
//                 return false;
//             }

//             if (option is RoleListEntryOption role)
//             {
//                 if (!AmongUsClient.Instance.AmHost)
//                     return false;

//                 role.ToDo();
//                 return false;
//             }

//             if (__instance == PresetButton.Setting)
//             {
//                 if (!AmongUsClient.Instance.AmHost)
//                     return false;

//                 PresetButton.Do();
//                 return false;
//             }

//             if (__instance == SaveSettings.Setting)
//             {
//                 if (!AmongUsClient.Instance.AmHost)
//                     return false;

//                 SaveSettings.Do();
//                 return false;
//             }

//             var option1 = PresetButton.SlotButtons.Find(option => option.Setting == __instance);

//             if (option1 is CustomButtonOption button)
//             {
//                 if (!AmongUsClient.Instance.AmHost)
//                     return false;

//                 button.Do();
//                 return false;
//             }

//             var option2 = RoleListEntryOption.EntryButtons.Find(option => option.Setting == __instance);

//             if (option2 is CustomButtonOption button1)
//             {
//                 if (!AmongUsClient.Instance.AmHost)
//                     return false;

//                 button1.Do();
//                 return false;
//             }

//             return true;
//         }
//     }

//     [HarmonyPatch(typeof(NumberOption), nameof(NumberOption.Increase))]
//     public static class NumberOptionPatchIncrease
//     {
//         public static bool Prefix(NumberOption __instance)
//         {
//             var option = CustomOption.AllOptions.Find(option => option.Setting == __instance);

//             if (option is CustomNumberOption number)
//             {
//                 number.Increase();
//                 return false;
//             }

//             return true;
//         }
//     }

//     [HarmonyPatch(typeof(NumberOption), nameof(NumberOption.Decrease))]
//     public static class NumberOptionPatchDecrease
//     {
//         public static bool Prefix(NumberOption __instance)
//         {
//             var option = CustomOption.AllOptions.Find(option => option.Setting == __instance);

//             if (option is CustomNumberOption number)
//             {
//                 number.Decrease();
//                 return false;
//             }

//             return true;
//         }
//     }

//     [HarmonyPatch(typeof(StringOption), nameof(StringOption.Increase))]
//     public static class KeyValueOptionPatchIncrease
//     {
//         public static bool Prefix(StringOption __instance)
//         {
//             var option = CustomOption.AllOptions.Find(option => option.Setting == __instance);

//             if (option is CustomStringOption str)
//             {
//                 str.Increase();
//                 return false;
//             }

//             return true;
//         }
//     }

//     [HarmonyPatch(typeof(StringOption), nameof(StringOption.Decrease))]
//     public static class KeyValueOptionOptionPatchDecrease
//     {
//         public static bool Prefix(StringOption __instance)
//         {
//             var option = CustomOption.AllOptions.Find(option => option.Setting == __instance);

//             if (option is CustomStringOption str)
//             {
//                 str.Decrease();
//                 return false;
//             }

//             return true;
//         }
//     }

//     [HarmonyPatch(typeof(RoleOptionSetting), nameof(RoleOptionSetting.IncreaseChance))]
//     public static class RoleOptionOptionPatchIncreaseChance
//     {
//         public static bool Prefix(RoleOptionSetting __instance)
//         {
//             var option = CustomOption.AllOptions.Find(option => option.Setting == __instance);

//             if (option is CustomLayersOption layer)
//             {
//                 layer.IncreaseChance();
//                 return false;
//             }

//             return true;
//         }
//     }

//     [HarmonyPatch(typeof(RoleOptionSetting), nameof(RoleOptionSetting.DecreaseChance))]
//     public static class RoleOptionOptionPatchDecreaseChance
//     {
//         public static bool Prefix(RoleOptionSetting __instance)
//         {
//             var option = CustomOption.AllOptions.Find(option => option.Setting == __instance);

//             if (option is CustomLayersOption layer)
//             {
//                 layer.DecreaseChance();
//                 return false;
//             }

//             return true;
//         }
//     }

//     [HarmonyPatch(typeof(RoleOptionSetting), nameof(RoleOptionSetting.IncreaseCount))]
//     public static class RoleOptionOptionPatchIncreaseCount
//     {
//         public static bool Prefix(RoleOptionSetting __instance)
//         {
//             var option = CustomOption.AllOptions.Find(option => option.Setting == __instance);

//             if (option is CustomLayersOption layer)
//             {
//                 layer.IncreaseCount();
//                 return false;
//             }

//             return true;
//         }
//     }

//     [HarmonyPatch(typeof(RoleOptionSetting), nameof(RoleOptionSetting.DecreaseCount))]
//     public static class RoleOptionOptionPatchDecreaseCount
//     {
//         public static bool Prefix(RoleOptionSetting __instance)
//         {
//             var option = CustomOption.AllOptions.Find(option => option.Setting == __instance);

//             if (option is CustomLayersOption layer)
//             {
//                 layer.DecreaseCount();
//                 return false;
//             }

//             return true;
//         }
//     }

//     [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.CoSpawnPlayer))]
//     public static class PlayerJoinPatch
//     {
//         private static bool SentOnce;

//         public static void Postfix(PlayerPhysics __instance)
//         {
//             if (!AmongUsClient.Instance || !CustomPlayer.Local || !__instance.myPlayer)
//                 return;

//             if (__instance.myPlayer == CustomPlayer.Local)
//             {
//                 if (!SentOnce)
//                 {
//                     Run("<color=#5411F8FF>人 Welcome! 人</color>", "Welcome to Town Of Us Reworked! Type /help to get started!");
//                     SentOnce = true;
//                 }

//                 return;
//             }

//             if (CustomPlayer.AllPlayers.Count <= 1 || !AmongUsClient.Instance.AmHost || TownOfUsReworked.MCIActive)
//                 return;

//             SendOptionRPC(setting: (CustomOption)null);
//             CallRpc(CustomRPC.Misc, MiscRPC.SyncSummary, ReadDiskText("Summary", TownOfUsReworked.Other));

//             if (CachedFirstDead != null)
//                 CallRpc(CustomRPC.Misc, MiscRPC.SetFirstKilled, CachedFirstDead);
//         }
//     }
// }