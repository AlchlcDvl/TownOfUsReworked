using HarmonyLib;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using AmongUs.Data;
using TownOfUsReworked.Classes;
using System;
using TMPro;
using Object = UnityEngine.Object;
using Reactor.Utilities.Extensions;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace TownOfUsReworked.Cosmetics
{
	[HarmonyPatch]
	public static class CustomNameplates
	{
		private static bool Loaded;
		private static bool Running;
		private static Material Shader;
		public readonly static Dictionary<string, NameplateExtension> CustomNameplateRegistry = new();

        #pragma warning disable
        public static NameplateExtension TestExt;
        #pragma warning restore

		private static Sprite CreateNameplateSprite(string path, bool fromDisk = false)
		{
            var texture = fromDisk ? AssetManager.LoadDiskTexture(path) : AssetManager.LoadResourceTexture(path);

			if (texture == null)
				return null;

			var sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), texture.width * 0.375f);

			if (sprite == null)
				return null;

			texture.hideFlags |= HideFlags.HideAndDontSave;
			sprite.hideFlags |= HideFlags.HideAndDontSave;
			return sprite;
		}

		private static NamePlateData CreateNameplateBehaviour(CustomNameplate cn, bool fromDisk = false, bool testOnly = false)
		{
			if (Shader == null)
				Shader = HatManager.Instance.PlayerMaterial;

			var nameplate = ScriptableObject.CreateInstance<NamePlateData>();
			nameplate.viewData.viewData = ScriptableObject.CreateInstance<NamePlateViewData>();
			nameplate.viewData.viewData.Image = CreateNameplateSprite(cn.ID, fromDisk);
			nameplate.name = cn.Name;
			nameplate.displayOrder = 99;
			nameplate.ProductId = "nameplate_" + cn.Name.Replace(' ', '_');
			nameplate.ChipOffset = new Vector2(0f, 0.2f);
			nameplate.Free = true;

            var extend = new NameplateExtension
            {
                Artist = cn.Artist ?? "Unknown",
                Package = cn.Package ?? "Misc",
                Condition = cn.Condition ?? "none"
            };

			if (testOnly)
			{
				TestExt = extend;
				TestExt.Condition = nameplate.name;
			}
			else
				CustomNameplateRegistry.Add(nameplate.name, extend);

			return nameplate;
		}

		private static NamePlateData CreateNameplateBehaviour(CosmeticsLoader.CustomNameplateOnline chd)
		{
			string filePath = Path.GetDirectoryName(Application.dataPath) + "\\CustomNameplates\\";
			chd.ID = filePath + chd.ID;
			return CreateNameplateBehaviour(chd, true, false);
		}

		public class NameplateExtension
		{
			public string Artist { get; set; }
			public string Package { get; set; }
			public string Condition { get; set; }
		}

		public class CustomNameplate
		{
			public string Artist { get; set; }
			public string Package { get; set; }
			public string Condition { get; set; }
			public string Name { get; set; }
			public string ID { get; set; }
		}

		// Token: 0x02000237 RID: 567
		[HarmonyPatch(typeof(HatManager), nameof(HatManager.GetNamePlateById))]
		public static class HatManagerPatch
		{
			private static List<NamePlateData> allPlates = new();

			public static void Prefix(HatManager __instance)
			{
				if (Running)
					return;

				Running = true;
				allPlates = __instance.allNamePlates.ToList();

				try
				{
					while (CosmeticsLoader.NameplateDetails.Count > 0)
					{
						var nameplateData = CreateNameplateBehaviour(CosmeticsLoader.NameplateDetails[0]);
						allPlates.Add(nameplateData);
						CosmeticsLoader.NameplateDetails.RemoveAt(0);
					}

					__instance.allNamePlates = allPlates.ToArray();
				}
				catch (Exception e)
				{
                    if (!Loaded)
                        Utils.LogSomething("Unable to add Custom Nameplates\n" + e);
				}

				Loaded = true;
			}

			public static void Postfix() => Running = false;
		}

		[HarmonyPatch(typeof(NameplatesTab), nameof(NameplatesTab.OnEnable))]
		public static class NameplatesTabOnEnablePatch
		{
			public const string InnerslothPackageName = "Innersloth Nameplates";
			private static TMP_Text Template;

			public static float CreateNameplatePackage(List<Tuple<NamePlateData, NameplateExtension>> nameplates, string packageName, float YStart, NameplatesTab __instance)
			{
				if (packageName != InnerslothPackageName)
					nameplates = nameplates.OrderBy(x => x.Item1.name).ToList();

				var offset = YStart;

				if (Template != null)
				{
					var title = Object.Instantiate(Template, __instance.scroller.Inner);
					var material = title.GetComponent<MeshRenderer>().material;
					material.SetFloat("_StencilComp", 4f);
					material.SetFloat("_Stencil", 1f);
					title.transform.localPosition = new Vector3(2.25f, YStart, -1f);
					title.transform.localScale = Vector3.one * 1.5f;
					title.fontSize *= 0.5f;
					title.enableAutoSizing = false;
                    __instance.StartCoroutine(Effects.Lerp(0.1f, new Action<float>(_ => title.SetText(packageName, true))));
					offset -= 0.8f * __instance.YOffset;
				}

				for (var i = 0; i < nameplates.Count; i++)
				{
					var nameplate = nameplates[i].Item1;
					var ext = nameplates[i].Item2;
					var xpos = __instance.XRange.Lerp(i % __instance.NumPerRow / (__instance.NumPerRow - 1f));
					var ypos = offset - (i / __instance.NumPerRow * __instance.YOffset);
					var colorChip = Object.Instantiate(__instance.ColorTabPrefab, __instance.scroller.Inner);

					if (ActiveInputManager.currentControlType == ActiveInputManager.InputType.Keyboard)
					{
                        colorChip.Button.OnMouseOver.AddListener((Action)(() => __instance.SelectNameplate(nameplate)));
                        colorChip.Button.OnMouseOut.AddListener((Action)(() =>
                            __instance.SelectNameplate(HatManager.Instance.GetNamePlateById(DataManager.Player.Customization.NamePlate))));
                        colorChip.Button.OnClick.AddListener((Action)(() => __instance.ClickEquip()));
					}
					else
                        colorChip.Button.OnClick.AddListener((Action)(() => __instance.SelectNameplate(nameplate)));

					colorChip.Button.ClickMask = __instance.scroller.Hitbox;
					colorChip.transform.localPosition = new Vector3(xpos, ypos, -1f);
					__instance.StartCoroutine(nameplate.CoLoadViewData(new Action<NamePlateViewData>(x =>
					{
						colorChip.gameObject.GetComponent<NameplateChip>().image.sprite = x.Image;
						colorChip.gameObject.GetComponent<NameplateChip>().ProductId = nameplate.ProductId;
					})));
					__instance.ColorChips.Add(colorChip);
				}
				return offset - ((nameplates.Count - 1) / __instance.NumPerRow * __instance.YOffset) - 1.5f;
			}

			public static bool Prefix(NameplatesTab __instance)
			{
                for (var i = 0; i < __instance.scroller.Inner.childCount; i++)
                    __instance.scroller.Inner.GetChild(i).gameObject.Destroy();

				__instance.ColorChips = new();
				var array = HatManager.Instance.GetUnlockedNamePlates();
				var packages = new Dictionary<string, List<Tuple<NamePlateData, NameplateExtension>>>();

				foreach (var data in array)
				{
					var ext = data.GetNameplateExtension();

					if (ext != null)
					{
						if (!packages.ContainsKey(ext.Package))
							packages[ext.Package] = new();

						packages[ext.Package].Add(new(data, ext));
					}
					else
					{
						if (!packages.ContainsKey(InnerslothPackageName))
							packages[InnerslothPackageName] = new();

						packages[InnerslothPackageName].Add(new(data, null));
					}
				}

				var YOffset = __instance.YStart;
				Template = __instance.transform.FindChild("Text").gameObject.GetComponent<TMP_Text>();
                var keys = packages.Keys.OrderBy(x =>
                {
                    if (x == InnerslothPackageName)
                        return 1000;

                    if (x == "Developer Visors")
                        return 0;

                    return 500;
                });

				foreach (string key in keys)
					YOffset = CreateNameplatePackage(packages[key], key, YOffset, __instance);

				__instance.scroller.ContentYBounds.max = -(YOffset + 3.8f);
				return false;
			}
		}

        [HarmonyPatch(typeof(HatManager), nameof(HatManager.GetUnlockedNamePlates))]
        public static class UnlockNameplates
        {
            public static bool Prefix(HatManager __instance, ref Il2CppReferenceArray<NamePlateData> __result)
            {
                __result =
                (
                    from n
                    in __instance.allNamePlates.ToArray()
                    where n.Free || DataManager.Player.Purchases.GetPurchase(n.ProductId, n.BundleId)
                    select n
                    into o
                    orderby o.displayOrder descending,
                    o.name
                    select o
                ).ToArray();
                return false;
            }
        }

        public static NameplateExtension GetNameplateExtension(this NamePlateData Nameplate)
        {
            if (TestExt?.Condition.Equals(Nameplate.name) == true)
                return TestExt;

            CustomNameplateRegistry.TryGetValue(Nameplate.name, out var ret);
            return ret;
        }
	}
}