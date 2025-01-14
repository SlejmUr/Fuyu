using System;
using System.Collections.Generic;
using Fuyu.Common.IO;
using Fuyu.DependencyInjection;
using Fuyu.Modding;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.ItemTemplates;
using Fuyu.Backend.BSG.Models.Profiles.Info;
using Fuyu.Backend.BSG.Models.Trading;
using Fuyu.Backend.BSG.Services;
using Fuyu.Backend.EFTMain;
using Fuyu.Backend.EFTMain.Services;
using Fuyu.Common.Hashing;

public class Mod : AbstractMod
{
    public override string Id { get; } = "Fuyu.DevTools.GenerateFleamarketOffers";

    public override string Name { get; } = "GenerateFleaMarketOffers";

    public override Task OnLoad(DependencyContainer container)
    {
        var player = new RagfairPlayerUser(new MongoId(true), 344, EMemberCategory.Developer, EMemberCategory.Developer,
            "b1gdeveloper", 69.420f, true);
        new Thread((() =>
        {
            Terminal.WriteLine("Generating offers...");
            var sw = Stopwatch.StartNew();
            var templates = EftOrm.Instance.GetItemTemplates()["data"]!.ToObject<Dictionary<MongoId, ItemTemplate>>();
            var handbook = EftOrm.Instance.GetHandbook();
            var success = 0;
            var failed = 0;
            
            foreach (var (tid, template) in templates)
            {
                var entry = handbook.Items.Find(i => i.Id == tid);
                int price = entry?.Price ?? 100;
                
                if (price <= 0)
                {
                    price = 100;
                }
                
                try
                {
                    var item = ItemFactoryService.Instance.CreateItem(template.Id);

                    var x = RagfairService.Instance.CreateAndAddOffer(player,
                        [item],
                        false,
                        [new HandoverRequirement() { Count = price, TemplateId = "5449016a4bdc2d6f028b456f" }],
                        TimeSpan.FromMinutes(30d),
                        false
                    );

                    if (x == null)
                    {
                        failed++;
                    }
                    else
                    {
                        success++;
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            Terminal.WriteLine($"Done generating offers: {sw.ElapsedMilliseconds}ms, {success} succeeded and {failed} failed");
        })).Start();

        return Task.CompletedTask;
    }
}