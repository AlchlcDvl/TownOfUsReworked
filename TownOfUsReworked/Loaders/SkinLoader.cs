// namespace TownOfUsReworked.Loaders;

// public sealed class SkinLoader : BaseCosmeticLoader<SkinViewData, SkinData, CustomSkin>
// {
//     protected override string DirectoryInfo => TownOfUsReworked.Skins;
//     protected override string Manifest => "Skins";

//     protected override CosmeticsLayer.CosmeticKind CosmeticType => CosmeticsLayer.CosmeticKind.SKIN;

//     protected override void LoadData(CustomSkin item, string path, SkinViewData viewData, PreviewViewData preview, SkinData data)
//     {
//         viewData.EjectFrame = viewData.IdleFrame = CreateCosmeticSprite(path, item.MainID + "_spriteSheet", CosmeticType);
//         viewData.MatchPlayerColor = item.Adaptive;

//         preview.PreviewSprite = CreateCosmeticSprite(path, item.MainID + "_preview", CosmeticType);

//         data.PreviewCrewmateColor = item.Adaptive;
//         data.ViewDataRef = new CustomAddressable<SkinViewData>(viewData, data.ProductId).Ref;
//     }

//     protected override void GenerateHash(CustomSkin item, HashAlgorithm hasher)
//     {
//         item.MainHash = GenerateHash(Path.Combine(DirectoryInfo, $"{item.MainID}_spriteSheet.png"), hasher);
//         item.PreviewHash = GenerateHash(Path.Combine(DirectoryInfo, $"{item.MainID}_preview.png"), hasher);
//     }
// }