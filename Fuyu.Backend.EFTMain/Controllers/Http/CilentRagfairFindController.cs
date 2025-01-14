using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Requests;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.BSG.Models.Trading;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Services;
using Fuyu.Common.Hashing;
using Fuyu.Common.IO;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class CilentRagfairFindController : AbstractEftHttpController<RagfairFindRequest>
{
    private readonly EftOrm _eftOrm;
    private readonly RagfairService _ragfairService;
    private readonly HandbookService _handbookService;

    public CilentRagfairFindController() : base("/client/ragfair/find")
    {
        _eftOrm = EftOrm.Instance;
        _ragfairService = RagfairService.Instance;
        _handbookService = HandbookService.Instance;
    }

    public override Task RunAsync(EftHttpContext context, RagfairFindRequest body)
    {
        var sw = Stopwatch.StartNew();
        var handbook = _eftOrm.GetHandbook();
        HashSet<MongoId> templateIds = [];
        List<Offer> selectedOffers = [];

        var rootItemEntry = handbook.Items.Find(i => i.Id == body.HandbookId);

        if (rootItemEntry != null)
        {
            templateIds.Add(body.HandbookId);
        }
        else
        {
            HashSet<HandbookCategory> categories = _handbookService.GetHandbookTree(handbook.Categories, body.HandbookId);

            foreach (var handbookItem in handbook.Items)
            {
                if (categories.FirstOrDefault(i => i.Id == handbookItem.ParentId) != null)
                {
                    templateIds.Add(handbookItem.Id);
                }
            }
        }

        foreach (var offer in _ragfairService.Offers)
        {
            if (templateIds.Contains(offer.Items[0].TemplateId))
            {
                selectedOffers.Add(offer);
            }
        }

        ResponseBody<OffersListResponse> responseBody = new()
        {
            data = new OffersListResponse
            {
                Categories = _ragfairService.CategoricalOffers,
                Offers = selectedOffers.Skip(body.Page * body.Limit).Take(body.Limit).ToList(),
                OffersCount = selectedOffers.Count,
                SelectedCategory = body.HandbookId
            }
        };

        Terminal.WriteLine(sw.ElapsedMilliseconds);
        return context.SendResponseAsync(responseBody, true, true);
    }
}