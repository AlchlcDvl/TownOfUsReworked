using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.Patches
{
	internal class MedScan
	{
		[HarmonyPatch(typeof(MedScanMinigame))]
		private static class MedScanMinigamePatch
		{
			[HarmonyPatch(nameof(MedScanMinigame.Begin))]
			private static void Postfix(MedScanMinigame __instance)
			{
				var oldHeightFeet = 3f;
				var oldHeightInch = 6f;
				var oldWeight = 92f;
				var newHeightFeet = 0f;
				var newHeightInch = 0f;
				var newWeight = 0f;
				var weightString = "";
				var heightString = "";
				var scale = 1f;

				//Update medical details for Giant and Dwarf modifiers based on game options
				if (PlayerControl.LocalPlayer.Is(ModifierEnum.Giant))
					scale = CustomGameOptions.GiantScale;
				else if (PlayerControl.LocalPlayer.Is(ModifierEnum.Dwarf))
					scale = CustomGameOptions.DwarfScale;
					
				newHeightFeet = oldHeightFeet * scale;
				newHeightInch = oldHeightInch * scale;
				newWeight = oldWeight * scale;
					
				while (newHeightFeet <= 1 && newHeightFeet > 0)
				{
					newHeightInch = newHeightInch + (12 * newHeightFeet);
					newHeightFeet = 0;
				}

				while (newHeightInch >= 12)
				{
					newHeightFeet += 1;
					newHeightInch -= 12;
				}

				weightString = $"{newWeight}lb";
				heightString = $"{newHeightFeet}' {newHeightInch}\"";
					
				__instance.completeString = __instance.completeString.Replace("3' 6\"", heightString).Replace("92lb", weightString);
			}
		}

        [HarmonyPatch(typeof(MedScanMinigame), nameof(MedScanMinigame.FixedUpdate))]
        class MedScanMinigameFixedUpdatePatch
        {
            static void Prefix(MedScanMinigame __instance)
            {
                if (CustomGameOptions.ParallelMedScans)
                {
                    //Allows multiple medbay scans at once
                    __instance.medscan.CurrentUser = PlayerControl.LocalPlayer.PlayerId;
                    __instance.medscan.UsersList.Clear();
                }
            }
        }
	}
}