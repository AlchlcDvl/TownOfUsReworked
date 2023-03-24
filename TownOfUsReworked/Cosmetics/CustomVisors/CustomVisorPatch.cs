using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using System.Linq;
using Reactor.Utilities.Extensions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.Cosmetics.CustomVisors
{
    //Thanks to Las Monjas for this code
    public static class CustomVisorPatch
    {
        private static Material MagicShader;
        private readonly static List<VisorMetadataElement> AuthorDatas = Loader.LoadCustomVisorData();
        private static bool _customVisorLoaded;

        [HarmonyPatch(typeof(HatManager), nameof(HatManager.GetVisorById))]
        public static class AddCustomVisors
        {
            private static int VisorID;

            public static void Postfix(HatManager __instance)
            {
                if (!_customVisorLoaded)
                {
                    var allVisors = __instance.allVisors.ToList();

                    foreach (var data in AuthorDatas)
                    {
                        VisorID++;
                        allVisors.Add(CreateVisor(data.Id, data.Name, data.Artist, data.Adaptive));
                        _customVisorLoaded = true;
                    }

                    _customVisorLoaded = true;
                    allVisors.Reverse();
                    __instance.allVisors = allVisors.ToArray();
                }
            }

            private static VisorData CreateVisor(string id, string visorName, string author, bool altshader)
            {
                //Borrowed from Other Roles to get hats alt shaders to work
                if (MagicShader == null)
                    MagicShader = DestroyableSingleton<HatManager>.Instance.PlayerMaterial;

                var a = ScriptableObject.CreateInstance<VisorViewData>();
                var b = new AddressableLoadWrapper<VisorViewData>();
                a.IdleFrame = GetSprite(id);

                if (altshader)
                    a.AltShader = MagicShader;

                b.viewData = a;

                var newVisor = ScriptableObject.CreateInstance<VisorData>();
                newVisor.viewData = b;
                newVisor.name = $"{visorName} By {author}";
                newVisor.ProductId = id;
                newVisor.displayOrder = 99 + VisorID;
                newVisor.Free = true;
                newVisor.ChipOffset = new Vector2(0f, 0.2f);
                return newVisor;
            }
        }

        private static Sprite GetSprite(string name)
        {
            var stream = TownOfUsReworked.Assembly.GetManifestResourceStream($"{TownOfUsReworked.Visors}{name}.png");
            var mainImg = stream.ReadFully();
            var tex2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            Utils.LoadImage(tex2D, mainImg, false);
            return Sprite.Create(tex2D, new Rect(0.0f, 0.0f, tex2D.width, tex2D.height), new Vector2(0.5f, 0.5f), 100f);
        }
    }
}