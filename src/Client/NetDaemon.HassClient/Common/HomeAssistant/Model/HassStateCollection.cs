using System.Collections;
using System.Collections.ObjectModel;

namespace NetDaemon.Client.HomeAssistant.Model;

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

    public IEnumerable<HassStateGroup> GroupForEntityMetadataGeneration()
    {
        // We need to group the entities by domain, because some domains (Sensor) have numeric and non numeric
        // entities that should be treated differently we also group by IsNumeric
        return _states.GroupBy(e => (domain: e.Domain, e.IsNumeric))
            .Select(g => new HassStateGroup(g.Key.domain, g.Key.IsNumeric, g))
            .OrderBy(g => g.Key);
    }

    public static implicit operator HassStateCollection(HassState [] states) => new HassStateCollection(states);

}
