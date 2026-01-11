namespace Rewards.Models;

public class RelicRewardItem
{
	public string CanonicalName { get; }
	public string MatchPattern { get; }

	public RelicRewardItem(string canonicalName, string matchPattern)
	{
		CanonicalName = canonicalName;
		MatchPattern = matchPattern.ToLowerInvariant();
	}
}