using Rewards.Matching;
using Rewards.Models;

namespace Rewards.Processing;

public static class RewardCounter
{
	public static List<RelicRewardItem> Count(string ocrText)
	{
		return RewardMatcher.Match(ocrText)
			.GroupBy(item => item.CanonicalName)
			.Select(g => g.First())
			.ToList();
	}
}