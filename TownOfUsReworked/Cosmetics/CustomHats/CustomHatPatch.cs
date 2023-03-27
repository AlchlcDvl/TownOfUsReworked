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
    public static class CustomHats
    {
        private static Material MagicShader;
        private readonly static List<HatMetadataElement> AuthorDatas = Loader.LoadCustomHatData();
        private static bool _customHatsLoaded;

        [HarmonyPatch(typeof(HatManager), nameof(HatManager.GetHatById))]
        public static class AddCustomHats
        {
            private static int HatID;

            public static void Postfix(HatManager __instance)
            {
                if (!_customHatsLoaded)
                {
                    var allHats = __instance.allHats.ToList();

                    foreach (var data in AuthorDatas)
                    {
                        HatID++;
                        allHats.Add(CreateHat(data.Id, data.Name, data.Adaptive, data.Artist, data.NoBounce, data.ClimbId, data.FlippedId));
                        _customHatsLoaded = true;
                    }

                    _customHatsLoaded = true;
                    allHats.Reverse();
                    __instance.allHats = allHats.ToArray();
                }
            }

            private static HatData CreateHat(string id, string hatName, bool altshader, string author, bool bounce, string climbid, string leftimageid)
            {
                //Borrowed from The Other Roles to get hats alt shaders to work
                if (MagicShader == null)
                    MagicShader = new(Shader.Find("Unlit/PlayerShader"));

                var sprite = GetSprite(id);
                var a = ScriptableObject.CreateInstance<HatViewData>();
                var b = new AddressableLoadWrapper<HatViewData>();
                a.MainImage = sprite;

                if (!string.IsNullOrEmpty(climbid))
                    a.ClimbImage = GetSprite(climbid);
                else
                    a.ClimbImage = null;

                if (!string.IsNullOrEmpty(leftimageid))
                    a.LeftMainImage = GetSprite(leftimageid);
                else
                    a.LeftMainImage = null;

                a.FloorImage = null;
                a.LeftFloorImage = null;

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

        private static Sprite GetSprite(string name)
        {
            var stream = TownOfUsReworked.Assembly.GetManifestResourceStream($"{TownOfUsReworked.Hats}{name}.png");
            var mainImg = stream.ReadFully();
            var tex2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            Utils.LoadImage(tex2D, mainImg, false);
            return Sprite.Create(tex2D, new Rect(0.0f, 0.0f, tex2D.width, tex2D.height), new Vector2(0.5f, 0.5f), 100f);
        }
    }
}