namespace Rewards.Services;

public static class WarframeMarketNaming
{
	public static string ToUrlName(string canonicalName)
	{
		return canonicalName
			.ToLowerInvariant()
			.Replace(" blueprint", "")
			.Replace(" ", "_");
	}
}