using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace TownOfUsReworked.Cosmetics.CustomNameplates
{
    //Thanks to Las Monjas for this code
    public class CustomNameplatesPatch
    {
        public static List<NameplateMetadataElement> AuthorDatas = Loader.LoadCustomNameplateData();
        private static bool _customNamePlatesLoaded = false;

        [HarmonyPatch(typeof(HatManager), nameof(HatManager.GetNamePlateById))]
        public static class AddCustomNamePlates
        {
            public static int NamePlateID = 0;

            public static void Postfix(HatManager __instance)
            {
                if (!_customNamePlatesLoaded)
                {
                    var allPlates = __instance.allNamePlates.ToList();

                    foreach (var data in AuthorDatas)
                    {
                        allPlates.Add(CreateNamePlate(GetSprite(data.Id), data.Name, data.Artist));
                        _customNamePlatesLoaded = true;
                        NamePlateID++;
                    }

                    _customNamePlatesLoaded = true;
                    __instance.allNamePlates = allPlates.ToArray();
                }
            }

            private static Sprite GetSprite(string id) => TownOfUsReworked.CreateSprite($"{TownOfUsReworked.Nameplates}{id}.png");

            private static NamePlateData CreateNamePlate(Sprite sprite, string nameplateName, string author)
            {
                var newPlate = ScriptableObject.CreateInstance<NamePlateData>();
                newPlate.viewData.viewData = ScriptableObject.CreateInstance<NamePlateViewData>();
                newPlate.name = $"{nameplateName} By {author}";
                newPlate.viewData.viewData.Image = sprite;
                newPlate.ProductId = "nameplate_" + sprite.name.Replace(' ', '_');
                newPlate.BundleId = "nameplate_" + sprite.name.Replace(' ', '_');
                newPlate.displayOrder = 99 + NamePlateID;
                newPlate.Free = true;
                newPlate.ChipOffset = new Vector2(0f, 0.2f);

                return newPlate;
            }
        }
    }
}
