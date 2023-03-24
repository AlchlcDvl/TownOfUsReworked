using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.Cosmetics.CustomNameplates
{
    //Thanks to Las Monjas for this code
    public static class CustomNameplatesPatch
    {
        private readonly static List<NameplateMetadataElement> AuthorDatas = Loader.LoadCustomNameplateData();
        private static bool _customNamePlatesLoaded;

        [HarmonyPatch(typeof(HatManager), nameof(HatManager.GetNamePlateById))]
        public static class AddCustomNamePlates
        {
            private static int NamePlateID;

            public static void Postfix(HatManager __instance)
            {
                if (!_customNamePlatesLoaded)
                {
                    var allPlates = __instance.allNamePlates.ToList();

                    foreach (var data in AuthorDatas)
                    {
                        allPlates.Add(CreateNamePlate(data.Id, data.Name, data.Artist));
                        _customNamePlatesLoaded = true;
                        NamePlateID++;
                    }

                    _customNamePlatesLoaded = true;
                    __instance.allNamePlates = allPlates.ToArray();
                }
            }

            private static NamePlateData CreateNamePlate(string id, string nameplateName, string author)
            {
                var sprite = Utils.CreateSprite(TownOfUsReworked.Nameplates + id);

                var a = ScriptableObject.CreateInstance<NamePlateViewData>();
                var b = new AddressableLoadWrapper<NamePlateViewData>();
                a.Image = sprite;
                b.viewData = a;

                var newPlate = ScriptableObject.CreateInstance<NamePlateData>();
                newPlate.viewData = b;
                newPlate.name = $"{nameplateName} By {author}";
                newPlate.ProductId = id;
                newPlate.displayOrder = 99 + NamePlateID;
                newPlate.Free = true;
                newPlate.ChipOffset = new Vector2(0f, 0.2f);

                return newPlate;
            }
        }
    }
}
