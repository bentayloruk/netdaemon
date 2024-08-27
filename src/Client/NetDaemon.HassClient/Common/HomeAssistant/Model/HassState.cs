using NetDaemon.Client.Common.HomeAssistant.Model;

namespace NetDaemon.Client.HomeAssistant.Model;

public record HassState
{
    private static readonly string[] NumericDomains = ["input_number", "number", "proximity"];

    public HassState()
    {
        _haasEntityId = new Lazy<HaasEntityId>(() => HaasEntityId.Parse(EntityId));
    }

    [JsonPropertyName("attributes")] public JsonElement? AttributesJson { get; init; }

    public IReadOnlyDictionary<string, object>? Attributes
    {
        get => AttributesJson?.Deserialize<Dictionary<string, object>>() ?? [];
        init => AttributesJson = value.ToJsonElement();
    }

    [JsonPropertyName("entity_id")] public string EntityId { get; init; } = "";

    [JsonPropertyName("last_changed")] public DateTime LastChanged { get; init; } = DateTime.MinValue;
    [JsonPropertyName("last_updated")] public DateTime LastUpdated { get; init; } = DateTime.MinValue;
    [JsonPropertyName("state")] public string? State { get; init; } = "";
    [JsonPropertyName("context")] public HassContext? Context { get; init; }

    public T? AttributesAs<T>()
    {
        return AttributesJson.HasValue ? AttributesJson.Value.Deserialize<T>() : default;
    }

    private Lazy<HaasEntityId> _haasEntityId { get; }

    [JsonIgnore] public string Domain => _haasEntityId.Value.Domain;

    [JsonIgnore] public string ObjectId => _haasEntityId.Value.ObjectId;

    public bool IsNumeric =>
        // Mixed domains have both numeric and non-numeric entities, if it has a 'unit_of_measurement' we treat it as numeric
        NumericDomains.Contains(Domain)
        || (_haasEntityId.Value.IsMixedDomain && Attributes?.ContainsKey("unit_of_measurement") == true);

    [JsonIgnore] public bool IsMixedDomain => _haasEntityId.Value.IsMixedDomain;

    private record AttributeString(string friendly_name);

    [JsonIgnore]
    public string? FriendlyName => AttributesAs<AttributeString>()?.friendly_name;
}
