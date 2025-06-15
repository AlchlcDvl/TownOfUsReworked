namespace TownOfUsReworked.IPlayerLayers;

public interface IPromoter : IPlayerLayer
{
    Layer UnderlingType { get; }
    Layer PromoterType { get; }
    float PromotionModifier { get; }
    bool IsUnderling { get; }
    bool IsPromoted { get; }
}