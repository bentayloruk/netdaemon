using System.Collections;
using System.Collections.ObjectModel;

namespace NetDaemon.Client.HomeAssistant.Model;

public record HassStateGroup : IGrouping<ValueTuple<string, bool>, HassState>
{
    public string Domain { get; }

    public bool IsNumeric { get; }

    public IReadOnlyCollection<HassState> States { get; }

    internal HassStateGroup(string domain, bool isNumeric, IEnumerable<HassState> states)
    {
        Domain = domain;
        IsNumeric = isNumeric;
        States = new ReadOnlyCollection<HassState>(states.ToArray());
    }

    public IEnumerator<HassState> GetEnumerator()
    {
        return States.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)States).GetEnumerator();
    }

    public (string, bool) Key => (Domain, IsNumeric);
}
