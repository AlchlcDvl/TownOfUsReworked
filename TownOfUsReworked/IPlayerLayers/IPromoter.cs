namespace TownOfUsReworked.IPlayerLayers;

public interface IPromoter : IPlayerLayer
{
    LayerEnum UnderlingType { get; }
    LayerEnum PromoterType { get; }
    float PromotionModifier { get; }
    bool IsUnderling { get; }
    bool IsPromoted { get; }
}