using Rewards.Matching;
using Rewards.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warframe_relice_price.OCRVision;
using warframe_relice_price.Utils;

namespace Rewards.Processing;

    public static class RewardPrice
    {
        static readonly WarframeMarketClient wClient = new WarframeMarketClient();

        public static int? getPriceForItem(int index)
        {
        var ocrText = ImageToText.singleBoxOCR(index + 1);

        var item = RewardMatcher.matchSingle(ocrText);

        string urlName = WarframeMarketNaming.ToUrlName(item.CanonicalName);

        var price = wClient.GetLowestPrice(urlName);

        Logger.Log($"Fetched price for item {index}, {urlName} -> {price} plat");

        return price;


        }
    }

