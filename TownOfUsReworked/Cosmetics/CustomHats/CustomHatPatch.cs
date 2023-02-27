using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using System.Linq;
using Reactor.Utilities.Extensions;
using TownOfUsReworked.Classes;

//Adapted from https://github.com/xxomega77xx/HatPack
namespace TownOfUsReworked.Cosmetics.CustomVisors
{
    //Thanks to Las Monjas for the code
    class CustomHats
    {
        public static Material MagicShader;
        public static List<HatMetadataElement> AuthorDatas = Loader.LoadCustomHatData();
        private static bool _customHatsLoaded = false;

        [HarmonyPatch(typeof(HatManager), nameof(HatManager.GetHatById))]
        public static class AddCustomHats
        {
            public static int HatID = 0;

            public static void Postfix(HatManager __instance)
            {
                if (!_customHatsLoaded)
                {
                    var allHats = __instance.allHats.ToList();

                    foreach (var data in AuthorDatas)
                    {
                        HatID++;
                        allHats.Add(CreateHat(data.Id, data.Name, data.Adaptive, data.Artist, data.NoBounce, null, null, null));
                        _customHatsLoaded = true;
                    }

                    _customHatsLoaded = true;
                    allHats.Reverse();
                    __instance.allHats = allHats.ToArray();
                }
            }

            private static HatData CreateHat(string id, string hatName, bool altshader, string author, bool bounce, Sprite climb = null, Sprite floor = null, Sprite leftimage = null)
            {
				//Borrowed from Other Roles to get hats alt shaders to work
                if (MagicShader == null)
                {
                    Material hatShader = DestroyableSingleton<HatManager>.Instance.PlayerMaterial;
                    MagicShader = hatShader;
                }

                var stream = TownOfUsReworked.Assembly.GetManifestResourceStream($"{TownOfUsReworked.Hats}{id}.png");
                var mainImg = stream.ReadFully();
                var tex2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                Utils.LoadImage(tex2D, mainImg, false);
                var sprite = Sprite.Create(tex2D, new Rect(0.0f, 0.0f, tex2D.width, tex2D.height), new Vector2(0.5f, 0.5f), 100);

                var a = ScriptableObject.CreateInstance<HatViewData>();
                var b = new AddressableLoadWrapper<HatViewData>();
                a.MainImage = sprite;
                a.FloorImage = floor;
                a.LeftMainImage = leftimage;
                a.ClimbImage = climb;

                if (altshader)
                    a.AltShader = MagicShader;

                b.viewData = a;

                var newHat = ScriptableObject.CreateInstance<HatData>();
                newHat.hatViewData = b;
                newHat.StoreName = author;
                newHat.name = $"{hatName} By {author}";
                newHat.ProductId = id;
                newHat.displayOrder = 99 + HatID;
                newHat.InFront = true;
                newHat.NoBounce = bounce;
                newHat.Free = true;
                newHat.ChipOffset = new Vector2(-0.1f, 0.4f);
                return newHat;
            }
        }
    }
}