using System;
using System.Collections.Generic;
using System.Linq;
using Fuyu.Backend.BSG.Models.Trading;
using Fuyu.Common.Hashing;

namespace Fuyu.Backend.EFTMain.Services;

public class HandbookService
{
    public static HandbookService Instance => _instance.Value;

    private static readonly Lazy<HandbookService> _instance = new(() => new HandbookService());

    private readonly EftOrm _eftOrm;
    private readonly MongoId _generatedCategoryId;

    public HandbookService()
    {
        _eftOrm = EftOrm.Instance;
        _generatedCategoryId = new MongoId(true);
    }

    public HashSet<HandbookCategory> GetHandbookTree(List<HandbookCategory> categories, MongoId rootId)
    {
        var rootEntry = categories.Find(c => c.Id == rootId);

        if (rootEntry == null)
        {
            return [];
        }

        HashSet<HandbookCategory> result = [rootEntry];
        bool added = true;
        while (added)
        {
            added = false;
            foreach (var category in categories)
            {
                if (!result.Contains(category) && result.Any(c => c.Id == category.ParentId))
                {
                    result.Add(category);
                    added = true;
                }
            }
        }

        return result;
    }

    /// <param name="price">If null will not create a handbook entry in the event no entry was found</param>
    public int? GetPrice(MongoId templateId, int? price = null)
    {
        var handbook = _eftOrm.GetHandbook();
        var entry = handbook.Items.Find(i => i.Id == templateId);

        if (entry != null)
        {
            return entry.Price;
        }

        var generatedCategory = handbook.Categories.Find(i => i.Id == _generatedCategoryId);

        if (generatedCategory == null)
        {
            generatedCategory = new HandbookCategory
            {
                Id = _generatedCategoryId,
                ParentId = "",
                Icon = "dd",
                Color = "#ff0000"
            };

            handbook.Categories.Add(generatedCategory);
        }

        if (price.HasValue)
        {
            handbook.Items.Add(new HandbookItem
            {
                Id = templateId,
                ParentId = _generatedCategoryId,
                Price = price.Value
            });
        }

        return price;
    }
}