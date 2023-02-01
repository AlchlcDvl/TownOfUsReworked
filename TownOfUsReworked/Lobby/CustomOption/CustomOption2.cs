using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using BepInEx.Configuration;
using Hazel;
using Reactor.Utilities.Extensions;
using UnityEngine;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.Lobby.CustomOption
{
	public class CustomOption2
	{
		public static bool LobbyTextScroller { get; set; } = true;
		public float Min { get; set; }
		public float Max { get; set; }
		public float Increment { get; set; }
		public static List<CustomOption2> options = new List<CustomOption2>();
		public static ConfigEntry<string> baseSettings;
		public int id;
		public string name;
		public object[] selections;
		public float defaultSelection;
		public ConfigEntry<float> entry;
		public float selection;
		public OptionBehaviour optionBehaviour;
		public CustomOption2 parent;
		public bool isHeader;
		public MultiMenu Menu;
		public Func<float, string> Format;
		public CustomOptionType valueType;
		public Func<bool> Visible;

		public CustomOption2(int Id, MultiMenu menu, Color color, string name, CustomOptionType ValueType, object[] Selections, object defaultValue, Func<float, string> format,
            CustomOption2 Parent, bool IsHeader, Func<bool> visible = null)
		{
			id = Id;
			string localisedName = name;
			localisedName = CustomOption2.cs(color, localisedName);
			name = ((parent == null) ? localisedName : localisedName);
			selections = Selections;
			parent = Parent;
			isHeader = IsHeader;
			Menu = menu;
			valueType = ValueType;
			Visible = visible;
			Func<float, string> format2 = format;

			if (format == null && (format2 = Format) == null)
			{
				format2 = (Format = delegate(float obj)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(0, 1);
					defaultInterpolatedStringHandler2.AppendFormatted<float>(obj);
					return defaultInterpolatedStringHandler2.ToStringAndClear();
				});
			}

			Format = format2;

			switch (valueType)
			{
                case CustomOptionType.String:
                {
                    int index = Array.IndexOf<object>(selections, defaultValue);
                    defaultSelection = (float)((index >= 0) ? index : 0);
                    break;
                }
                case CustomOptionType.Toggle:
                {
                    int tindex = Array.IndexOf<object>(selections, defaultValue);
                    defaultSelection = (float)((tindex >= 0) ? tindex : 0);
                    break;
                }
                case CustomOptionType.Number:
                    defaultSelection = (float)defaultValue;
                    break;
                case CustomOptionType.Header:
                    defaultSelection = 1f;
                    break;
			}

			selection = defaultSelection;

			if (valueType == CustomOptionType.Header)
				selection = defaultSelection;

			CustomOption2.options.Add(this);
		}

		public static CustomOption2 Create(int id, MultiMenu menu, Color color, string name, string[] selections, CustomOption2 parent = null, bool isHeader = false)
		{
			Func<float, string> format = (float value) => selections[Math.Clamp((int)value, 0, selections.Length - 1)];
			CustomOptionType valueType = CustomOptionType.String;
			object[] array = selections;
			return new CustomOption2(id, menu, color, name, valueType, array, "", format, parent, isHeader, null);
		}

		public static CustomOption2 Create(int id, MultiMenu menu, Color color, string name, float defaultValue, float min, float max, float step, Func<float, string> format =
            null, CustomOption2 parent = null, bool isHeader = false)
		{
			List<object> selections = new List<object>();

			for (float s = min; s <= max; s += step)
				selections.Add(s);

			return new CustomOption2(id, menu, color, name, CustomOptionType.Number, selections.ToArray(), defaultValue, format, parent, isHeader, null)
			{
				Min = min,
				Max = max,
				Increment = step
			};
		}

		public static CustomOption2 Create(int id, MultiMenu menu, Color color, string name, bool defaultValue, CustomOption2 parent = null, bool isHeader = false)
		{
			Func<float, string> format = delegate(float val)
			{
				if ((int)val <= 0)
					return "Off";

				return "On";
			};

			CustomOptionType valueType = CustomOptionType.Toggle;

			object[] array = new string[] { "Off", "On" };

			return new CustomOption2(id, menu, color, name, valueType, array, defaultValue ? "On" : "Off", format, parent, isHeader, null);
		}

		public static CustomOption2 Create(int id, MultiMenu menu, Color color, string name, Func<bool> visible = null)
		{
			CustomOptionType valueType = CustomOptionType.Header;
			object[] array = new string[0];
			return new CustomOption2(id, menu, color, name, valueType, array, 1, null, null, true, visible);
		}

		public static string cs(Color c, string s)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(17, 2);
			defaultInterpolatedStringHandler.AppendLiteral("<color=#");
			defaultInterpolatedStringHandler.AppendFormatted(c.ToHtmlStringRGBA());
			defaultInterpolatedStringHandler.AppendLiteral(">");
			defaultInterpolatedStringHandler.AppendFormatted(s);
			defaultInterpolatedStringHandler.AppendLiteral("</color>");
			return defaultInterpolatedStringHandler.ToStringAndClear();
		}

		public static void Clear()
		{
			CustomOption2.options.Clear();
		}

		public static void SaveBaseOptions()
		{
			CustomOption2.baseSettings.Value = Convert.ToBase64String(GameOptionsManager.Instance.gameOptionsFactory.ToBytes(GameManager.Instance.LogicOptions.currentGameOptions));
		}

		public static void LoadBaseOptions()
		{
			string optionsString = CustomOption2.baseSettings.Value;

			if (optionsString == "")
				return;

			GameOptionsManager.Instance.GameHostOptions = GameOptionsManager.Instance.gameOptionsFactory.FromBytes(Convert.FromBase64String(optionsString));
			GameOptionsManager.Instance.CurrentGameOptions = GameOptionsManager.Instance.GameHostOptions;
			GameManager.Instance.LogicOptions.SetGameOptions(GameOptionsManager.Instance.CurrentGameOptions);
			GameManager.Instance.LogicOptions.SyncOptions();
		}

		public static void SendRpc(CustomOption2 option = null)
		{
			if (PlayerControl.AllPlayerControls.Count <= 1 || (!AmongUsClient.Instance.AmHost && PlayerControl.LocalPlayer == null))
				return;

			List<CustomOption2> optionsList;

			if (option != null)
				optionsList = new List<CustomOption2> { option };
			else
			{
				optionsList = new List<CustomOption2>(from x in CustomOption2.options
				where x.valueType != CustomOptionType.Header
				select x);
			}

			while (optionsList.Any<CustomOption2>())
			{
				byte amount = (byte)Math.Min(optionsList.Count, 20);
				MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, 120, SendOption.Reliable, -1);
				writer.Write(amount);

				for (int i = 0; i < (int)amount; i++)
				{
					CustomOption2 item = optionsList[0];
					optionsList.RemoveAt(0);
					writer.Write(item.id);
					writer.Write(item.selection);
				}

				AmongUsClient.Instance.FinishRpcImmediately(writer);
			}
		}

		public static void ReceiveRpc(byte numberOfOptions, MessageReader reader)
		{
			try
			{
				for (int i = 0; i < (int)numberOfOptions; i++)
				{
					int optionId = reader.ReadInt32();
					float selection = reader.ReadSingle();
					CustomOption2.options.First((CustomOption2 option) => option.id == optionId).Set(selection, true);
				}
			} catch (Exception) {}
		}

		public int Get()
		{
			return (int)selection;
		}

		public bool GetBool()
		{
			return Get() > 0;
		}

		public float GetFloat()
		{
			return selection;
		}

		public override string ToString()
		{
			return Format(selection);
		}

		protected void Set(float newSelection, bool SendRpc = true)
		{
			selection = newSelection;
			AmongUsClient instance = AmongUsClient.Instance;

			if (instance != null && instance.AmHost && PlayerControl.LocalPlayer)
			{
				if (entry != null)
					entry.Value = selection;
			}

			if (optionBehaviour != null && AmongUsClient.Instance.AmHost && PlayerControl.LocalPlayer && SendRpc)
			{
				if (id == 0)
					CustomOption2.SendRpc(null);
				else
					CustomOption2.SendRpc(this);
			}

			if (optionBehaviour == null)
				return;

			StringOption stringOption = optionBehaviour as StringOption;

			if (stringOption != null)
			{
				stringOption.oldValue = (stringOption.Value = Get());
				stringOption.ValueText.text = ToString();
				return;
			}

			ToggleOption toggleOption = optionBehaviour as ToggleOption;

			if (toggleOption != null)
			{
				toggleOption.CheckMark.enabled = GetBool();
				return;
			}

			NumberOption numberOption = optionBehaviour as NumberOption;

			if (numberOption != null)
			{
				numberOption.oldValue = (numberOption.Value = GetFloat());
				numberOption.ValueText.text = ToString();
			}
		}

		public void Toggle()
		{
			Set((float)(1 - Get()), true);
		}

		public void Increase()
		{
			if (valueType == CustomOptionType.String)
			{
				Set((float)Mathf.Clamp(Get() + 1, 0, selections.Length - 1), true);
				return;
			}

			if (valueType == CustomOptionType.Number)
			{
				float increment = (Increment > 5f && Input.GetKeyInt(KeyCode.LeftShift)) ? 5f : Increment;
				Set(Mathf.Clamp(GetFloat() + increment, Min, Max), true);
			}
		}

		public void Decrease()
		{
			if (valueType == CustomOptionType.String)
			{
				Set((float)Mathf.Clamp(Get() - 1, 0, selections.Length - 1), true);
				return;
			}

			if (valueType == CustomOptionType.Number)
			{
				float increment = (Increment > 5f && Input.GetKeyInt(KeyCode.LeftShift)) ? 5f : Increment;
				Set(Mathf.Clamp(GetFloat() - increment, Min, Max), true);
			}
		}
	}
}
