using Rewards.Data;
using Rewards.Models;
using Rewards.Processing;
using FuzzySharp;

namespace Rewards.Matching;

public static class RewardMatcher
{
	private const int MatchThreshold = 50; // adjust if needed

	public static IEnumerable<RelicRewardItem> Match(string ocrText)
	{
		string text = OcrNormalizer.Normalize(ocrText);
		var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);

		int i = 0;
		while (i < words.Length)
		{
			RelicRewardItem? bestMatch = null;
			int bestScore = 0;
			int bestLength = 0;

			// Try sequences of words from length 1 up to max (e.g., 5)
			for (int length = 1; length <= 5 && i + length <= words.Length; length++)
			{
				string segment = string.Join(' ', words[i..(i + length)]);

				foreach (var item in RelicRewardPool.All)
				{
					int score = Fuzz.TokenSetRatio(segment, item.CanonicalName);

					if (score > bestScore)
					{
						bestScore = score;
						bestMatch = item;
						bestLength = length; // how many words this match consumes
					}
				}
			}

			if (bestMatch != null && bestScore >= MatchThreshold)
			{
				yield return bestMatch;
				i += bestLength; // skip matched words
			}
			else
			{
				i++; // move to next word if no match
			}
		}
	}
}