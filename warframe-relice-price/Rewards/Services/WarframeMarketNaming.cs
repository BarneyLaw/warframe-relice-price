using System.Text.RegularExpressions;

namespace Rewards.Services;

public static class WarframeMarketNaming
{

    private static readonly string[] ComponentKeywords = new[]
{
            "chassis", "systems", "neuroptics", "stock", "blade", "handle",
            "receiver", "barrel", "link", "girp", "forma", "cerebrum", "carapace",
        };

    // Better way using regex
    public static string ToUrlName(string canonicalName)
    {
        string lower = canonicalName.ToLowerInvariant().Trim();

        // Regex: remove " blueprint" only if preceded by a component keyword
        string pattern = $@"\b({string.Join("|", ComponentKeywords)}) blueprint$";
        lower = Regex.Replace(lower, pattern, "$1");

        return lower.Replace(" ", "_");
    }

	/*
    public static string ToUrlName(string canonicalName)
	{
		string lower = canonicalName.ToLowerInvariant();

		if (isComponent(lower))
		{
			lower = lower.Replace(" blueprint", "");
        }

		return lower.Replace(" ", "_"); ;
    }
	*/


	public static bool isComponent(string canonicalName)
	{
		string lower = canonicalName
			.Trim()
			.ToLowerInvariant();

		return (lower.Contains(" chassis") ||
				lower.Contains(" systems") ||
				lower.Contains(" neuroptics") ||
				lower.Contains(" stock") ||
				lower.Contains(" blade") ||
				lower.Contains(" handle") ||
				lower.Contains(" receiver") ||
				lower.Contains(" barrel") ||
				lower.Contains(" link") ||
				lower.Contains("forma")
				);
    }
}