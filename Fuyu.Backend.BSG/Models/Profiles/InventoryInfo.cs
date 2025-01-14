using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization;
using System.Text;
using Fuyu.Backend.BSG.ItemTemplates;
using Fuyu.Backend.BSG.Models.Items;
using Fuyu.Backend.BSG.Services;
using Fuyu.Common.Hashing;
using Fuyu.Common.IO;

namespace Fuyu.Backend.BSG.Models.Profiles;

[DataContract]
public class InventoryInfo
{
    private readonly ItemFactoryService _itemFactoryService;
    private readonly InventoryService _inventoryService;
    private readonly ItemService _itemService;

    public InventoryInfo()
    {
        _itemFactoryService = ItemFactoryService.Instance;
        _inventoryService = InventoryService.Instance;
        _itemService = ItemService.Instance;
    }

    [DataMember(Name = "items")]
    public List<ItemInstance> Items { get; set; }

    [DataMember(Name = "equipment")]
    public MongoId Equipment { get; set; }

    [DataMember(Name = "stash")]
    public MongoId? Stash { get; set; }

    [DataMember(Name = "sortingTable")]
    public MongoId? SortingTable { get; set; }

    [DataMember(Name = "questRaidItems")]
    public MongoId? QuestRaidItems { get; set; }

    [DataMember(Name = "questStashItems")]
    public MongoId? QuestStashItems { get; set; }

    [DataMember(Name = "fastPanel")]
    public Dictionary<string, MongoId> FastPanel { get; set; }

    [DataMember(Name = "hideoutAreaStashes")]
    public Dictionary<string, MongoId> HideoutAreaStashes { get; set; }

    [DataMember(Name = "favoriteItems")]
    public MongoId[] FavoriteItems { get; set; }

    [DataMember(Name = "hideoutCustomizationStashId")]
    public MongoId? HideoutCustomizationStashId { get; set; }

    public List<ItemInstance> RemoveItem(ItemInstance item)
    {
        return _inventoryService.RemoveItem(this, item);
    }

    public ItemInstance FindItem(MongoId id)
    {
        return Items.Find(x => x.Id == id);
    }

    public IEnumerable<ItemInstance> FindItems(List<MongoId> id)
    {
        int amount = id.Count;
        for (int i = 0; i < amount; i++)
        {
            var item = FindItem(id[i]);
            if (item != null)
            {
                yield return item;
            }
        }
    }

    public List<ItemInstance> RemoveItem(MongoId id)
    {
        var item = Items.Find(i => i.Id == id);
        if (item == null)
        {
            throw new Exception($"Failed to find item with id {id} in inventory");
        }

        return _inventoryService.RemoveItem(this, item);
    }

    public List<ItemInstance> GetItemsByTemplate(MongoId tpl)
    {
        return Items.Where(i => i.TemplateId == tpl).ToList();
    }

    public ItemInstance GetStock(ItemInstance root)
    {
        var rootItemTemplate = _itemFactoryService.ItemTemplates[root.TemplateId];
        if (!rootItemTemplate.Props.ContainsKey("FoldedSlot"))
        {
            return null;
        }

        var weaponItemProperties = _itemFactoryService.GetItemProperties<WeaponItemProperties>(root.TemplateId);
        var subItems = _itemService.GetItemAndChildren(Items, root).Skip(1);

        return subItems.FirstOrDefault(i => i.SlotId == weaponItemProperties.FoldedSlot);
    }

    public Vector2 GetItemSize(ItemInstance root)
    {
        var rootProperties = _itemFactoryService.GetItemProperties<ItemProperties>(root.TemplateId);
        var width = rootProperties.Width;
        var height = rootProperties.Height;

        var sizeUp = 0;
        var sizeDown = 0;
        var sizeLeft = 0;
        var sizeRight = 0;
        var forcedUp = 0;
        var forcedDown = 0;
        var forcedLeft = 0;
        var forcedRight = 0;

        var children = _itemService.GetItemAndChildren(Items, root).Skip(1);
        foreach (var child in children)
        {
            var itemProperties = _itemFactoryService.GetItemProperties<ItemProperties>(child.TemplateId);
            if (itemProperties.ExtraSizeForceAdd)
            {
                forcedUp += itemProperties.ExtraSizeUp;
                forcedDown += itemProperties.ExtraSizeDown;
                forcedLeft += itemProperties.ExtraSizeLeft;
                forcedRight += itemProperties.ExtraSizeRight;
            }
            else
            {
                sizeUp = Math.Max(sizeUp, itemProperties.ExtraSizeUp);
                sizeDown = Math.Max(sizeDown, itemProperties.ExtraSizeDown);
                sizeLeft = Math.Max(sizeLeft, itemProperties.ExtraSizeLeft);
                sizeRight = Math.Max(sizeRight, itemProperties.ExtraSizeRight);
            }
        }

        width += sizeLeft + sizeRight + forcedLeft + forcedRight;
        height += sizeUp + sizeDown + forcedUp + forcedDown;

        if (root.Location.IsValue1 && root.Location.Value1.r == EItemRotation.Vertical)
        {
            return new Vector2
            {
                X = height,
                Y = width
            };
        }

        return new Vector2
        {
            X = width,
            Y = height
        };
    }

    public LocationInGrid GetNextFreeSlot(int width, int height, out string gridName, EItemRotation desiredRotation = EItemRotation.Horizontal)
    {
        gridName = null;

        if (Stash == null)
        {
            return null;
        }

        var stashItem = Items.Find(i => i.Id == Stash.Value);

        if (stashItem == null)
        {
            return null;
        }

        var stashItemProps = _itemFactoryService.GetItemProperties<CompoundItemItemProperties>(stashItem.TemplateId);

        foreach (var grid in stashItemProps.Grids)
        {
            gridName = grid.Name;
            var gridWidth = grid.Properties.CellsHorizontal;
            var gridHeight = grid.Properties.CellsVertical;
            var cells = new bool[gridWidth * gridHeight];
            var itemsInThisGrid = _itemService.GetItemAndChildren(Items, stashItem).Skip(1);

            foreach (var itemInThisGrid in itemsInThisGrid)
            {
                if (!itemInThisGrid.Location.IsValue1)
                {
                    continue;
                }

                var itemLocation = itemInThisGrid.Location.Value1;
                var itemSize = GetItemSize(itemInThisGrid);
                var itemWidth = itemSize.X;
                var itemHeight = itemSize.Y;

                for (var y = 0; y < itemHeight; y++)
                {
                    for (var x = 0; x < itemWidth; x++)
                    {
                        var cellX = itemLocation.x + x;
                        var cellY = itemLocation.y + y;
                        if (cells[cellY * gridWidth + cellX])
                        {
                            throw new Exception("Overlap");
                        }

                        cells[cellY * gridWidth + cellX] = true;
                    }
                }
            }

            /*var builder = new StringBuilder(cells.Length);
            for (int i = 0; i < gridHeight; i++)
            {
                var line = cells.Skip(i * gridWidth).Take(gridWidth);
                builder.Append(line.Select(b => b ? 'X' : 'O').ToArray());
                if (i != gridHeight - 1)
                {
                    builder.AppendLine();
                }
            }

            Console.Clear();
            Terminal.WriteLine(builder.ToString());*/

            for (var idx = 0; idx < cells.Length; idx++)
            {
                if (cells[idx])
                {
                    continue;
                }

                var x = idx % gridWidth;
                var y = idx / gridWidth;
                var found = true;

                for (int dy = 0; found && dy < height; dy++)
                {
                    for (int dx = 0; dx < width; dx++)
                    {
                        var tempX = x + dx;
                        if (tempX > gridWidth)
                        {
                            found = false;
                            break;
                        }

                        var tempY = y + dy;
                        if (tempY > gridHeight)
                        {
                            found = false;
                            break;
                        }

                        var tempIdx = tempY * gridWidth + tempX;
                        if (cells[tempIdx])
                        {
                            found = false;
                            break;
                        }
                    }
                }

                if (found)
                {
                    return new LocationInGrid
                    {
                        x = x,
                        y = y,
                        r = desiredRotation
                    };
                }
            }
        }

        return null;
    }
}