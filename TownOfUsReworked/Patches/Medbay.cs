using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.Patches
{
	internal class MedScan
	{
		[HarmonyPatch(typeof(MedScanMinigame))]
		private static class MedScanMinigamePatch
		{
			[HarmonyPatch(nameof(MedScanMinigame.Begin))]
			[HarmonyPostfix]
			private static void BeginPostfix(MedScanMinigame __instance)
			{
				var oldHeightFeet = 3f;
				var oldHeightInch = 6f;
				var oldWeight = 92f;
				var newHeightFeet = 0f;
				var newHeightInch = 0f;
				var newWeight = 0f;
				var weightString = "";
				var heightString = "";

				//Update medical details for Giant and Dwarf modifiers
				if (PlayerControl.LocalPlayer.Is(ModifierEnum.Giant))
				{
					var scale = CustomGameOptions.GiantScale;

					newHeightFeet = oldHeightFeet * scale;
					newHeightInch = oldHeightInch * scale;
					newWeight = oldWeight * scale;

					if (newHeightInch >= 12)
					{
						do
						{
							newHeightFeet += 1;
							newHeightInch -= 12;
						} while (newHeightInch >= 12);
					}

					weightString = $"{newWeight}lb";
					heightString = $"{newHeightFeet}' {newHeightInch}\"";
					
					__instance.completeString = __instance.completeString.Replace("3' 6\"", heightString).Replace("92lb", weightString);
				}
				else if (PlayerControl.LocalPlayer.Is(ModifierEnum.Dwarf))
				{
					var scale = CustomGameOptions.DwarfScale;

					newHeightFeet = oldHeightFeet * scale;
					newHeightInch = oldHeightInch * scale;
					newWeight = oldWeight * scale;

					if (newHeightFeet <= 1 && newHeightFeet > 0)
					{
						do
						{
							newHeightInch = newHeightInch + (12 * newHeightFeet);
							newHeightFeet = 0;

							if (newHeightInch >= 12)
							{
								do
								{
									newHeightFeet += 1;
									newHeightInch -= 12;
								} while (newHeightInch >= 12);
							}
						} while (newHeightFeet <= 1 && newHeightFeet > 0);
					}

					weightString = $"{newWeight}lb";
					heightString = $"{newHeightFeet}' {newHeightInch}\"";
					
					__instance.completeString = __instance.completeString.Replace("3' 6\"", heightString).Replace("92lb", weightString);
				}
			}
		}
	}
}