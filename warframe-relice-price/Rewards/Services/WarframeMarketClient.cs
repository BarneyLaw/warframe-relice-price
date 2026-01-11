using System.Buffers;
using System.Net.Http;
using System.Net.Http.Json;
using Rewards.Models.WarframeMarket;
using System.Text.Json;

namespace Rewards.Services;

public class WarframeMarketClient
{
	private static readonly HttpClient Http = new()
	{
		BaseAddress = new Uri("https://api.warframe.market/v2/")
	};

	public async Task<int?> GetLowestPriceAsync(string slug)
	{
		try
		{
			string rawJson = await Http.GetStringAsync($"orders/item/{slug}/top");

			var response = JsonSerializer.Deserialize<ItemResponse>(
				rawJson,
				new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
			);

			var lowest = response?.Data?.Sell?
				.OrderBy(o => o.Platinum)
				.FirstOrDefault();

			return lowest?.Platinum;
		}
		catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
		{
			Console.WriteLine($"Item {slug} not found");
			return null;
		}
	}
}