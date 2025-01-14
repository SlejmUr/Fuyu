using System;
using System.Collections.Generic;
using System.Linq;
using Fuyu.Backend.BSG.Models.Items;
using Fuyu.Backend.BSG.Models.Trading;
using Fuyu.Common.Hashing;

namespace Fuyu.Backend.EFTMain.Services;

public class RagfairService
{
    private static readonly Lazy<RagfairService> instance = new(() => new RagfairService());

    private readonly EftOrm _eftOrm;
    private readonly HandbookService _handbookService;
    public Dictionary<MongoId, int> CategoricalOffers { get; } = [];

    private RagfairService()
    {
        _eftOrm = EftOrm.Instance;
        _handbookService = HandbookService.Instance;
    }

    public static RagfairService Instance => instance.Value;

    public List<Offer> Offers { get; } = [];

    public Offer CreateAndAddOffer(IRagfairUser user, List<ItemInstance> items, bool isBatch,
        List<HandoverRequirement> requirements, TimeSpan lifetime, bool unlimitedCount, int loyaltyLevel = 1)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(requirements);

        if (items.Count == 0)
        {
            throw new Exception($"{nameof(items)} is empty");
        }

        if (requirements.Count == 0)
        {
            throw new Exception($"{nameof(requirements)} is empty");
        }

        var handbook = _eftOrm.GetHandbook();
        var handbookItem = handbook.Items.Find(i => i.Id == items[0].TemplateId);

        if (handbookItem == null)
        {
            return null;
        }

        if (!CategoricalOffers.TryAdd(handbookItem.ParentId, 1))
        {
            CategoricalOffers[handbookItem.ParentId]++;
        }

        if (!CategoricalOffers.TryAdd(items[0].TemplateId, 1))
        {
            CategoricalOffers[items[0].TemplateId]++;
        }

        var offer = new Offer()
        {
            Id = new MongoId(true),
            IntId = Offers.Count,
            User = user,
            Root = items[0].Id,
            Items = items,
            ItemsCost =
                (isBatch
                    ? items.Sum(i => _handbookService.GetPrice(i.TemplateId, 100))
                    : _handbookService.GetPrice(items[0].TemplateId, 1)).GetValueOrDefault(1),
            Requirements = requirements,
            RequirementsCost =
                requirements.Sum(i => _handbookService.GetPrice(i.TemplateId).GetValueOrDefault(1) * i.Count),
            SummaryCost = 0,
            SellInOnePiece = isBatch,
            StartTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000d,
            EndTime = (DateTimeOffset.UtcNow + lifetime).ToUnixTimeMilliseconds() / 1000d,
            UnlimitedCount = unlimitedCount,
            LoyaltyLevel = loyaltyLevel
        };

        Offers.Add(offer);

        return offer;
    }

    public Offer GetOffer(MongoId offerId)
    {
        return Offers.Find(o => o.Id == offerId);
    }

    public void RemoveOffer(Offer offer)
    {
        if (!Offers.Remove(offer))
        {
            throw new Exception($"Failed to remove offer {offer.Id}");
        }

        var handbook = _eftOrm.GetHandbook();
        var handbookItem = handbook.Items.Find(i => i.Id == offer.Items[0].TemplateId);

        if (handbookItem != null)
        {
            if (!CategoricalOffers.ContainsKey(handbookItem.ParentId))
            {
                CategoricalOffers[handbookItem.ParentId]--;
            }
        }

        if (!CategoricalOffers.ContainsKey(offer.Items[0].TemplateId))
        {
            CategoricalOffers[offer.Items[0].TemplateId]--;
        }
    }
}