using System.Text.Json.Serialization;

namespace Rewards.Models.WarframeMarket;

public class ItemResponse
{
    [JsonPropertyName("apiVersion")]
    public string? ApiVersion { get; set; }

    [JsonPropertyName("data")]
    public ItemData? Data { get; set; }

    [JsonPropertyName("error")]
    public string? Error { get; set; }
}

public class ItemData
{
    [JsonPropertyName("buy")]
    public List<OrderData>? Buy { get; set; }

    [JsonPropertyName("sell")]
    public List<OrderData>? Sell { get; set; }
}

public class OrderData
{
    [JsonPropertyName("order_type")]
    public string? OrderType { get; set; }

    [JsonPropertyName("platinum")]
    public int Platinum { get; set; }

    [JsonPropertyName("user")]
    public UserData? User { get; set; }
}

public class UserData
{
    [JsonPropertyName("status")]
    public string? Status { get; set; }
}