using System.Text.RegularExpressions;

namespace Rewards.Processing;

public static class OcrNormalizer
{
	public static string Normalize(string text)
	{
		string normalized = text
			.ToLowerInvariant()
			.Replace("blue print", "blueprint")
			.Replace("\n", " ")
			.Trim();

		// Keep letters, numbers, and spaces; remove everything else
		normalized = Regex.Replace(normalized, "[^a-zA-Z0-9 ]", "");

		// Optional: collapse multiple spaces into one
		normalized = Regex.Replace(normalized, @"\s+", " ");

		return normalized;
	}
}