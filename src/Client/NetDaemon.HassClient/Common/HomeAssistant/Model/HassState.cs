using System.Collections;
using System.Collections.ObjectModel;

namespace NetDaemon.Client.HomeAssistant.Model;

public record HassState
{
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
}

public class HassStateCollection : IReadOnlyCollection<HassState>
{
    private readonly IReadOnlyCollection<HassState> _states;

    public HassStateCollection(IEnumerable<HassState> states)
    {
        ArgumentNullException.ThrowIfNull(states);
        _states = new ReadOnlyCollection<HassState>(states.ToArray());
    }

    public HassStateCollection()
    {
        _states = new ReadOnlyCollection<HassState>(Array.Empty<HassState>());
    }

    public IEnumerator<HassState> GetEnumerator()
    {
        return _states.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_states).GetEnumerator();
    }

    public int Count => _states.Count;

    public static HassStateCollection Empty { get; } = new();
}
