using NetDaemon.Client.HomeAssistant.Model;

namespace NetDaemon.HassModel.CodeGenerator;

internal static class EntityMetaDataGenerator
{
    /// <summary>
    /// Creates metadata describing entities and their attributes based on all the states from HA
    /// </summary>
    public static EntitiesMetaData GetEntityDomainMetaData(HassStateCollection haasStates)
    {
        var haasStateGroups = haasStates.GroupForEntityMetadataGeneration();

        return new EntitiesMetaData{Domains = haasStateGroups
            .Select(mapEntityDomainMetadata)
            .ToList()};
    }

    private static EntityDomainMetadata mapEntityDomainMetadata(HassStateGroup hassStateGroup) =>
        new (
            Domain: hassStateGroup.Domain,
            IsNumeric: hassStateGroup.IsNumeric,
            IsMixedDomain: hassStateGroup.Any(state => state.IsMixedDomain),
            Entities: MapToEntityMetaData(hassStateGroup),
            Attributes: AttributeMetaDataGenerator.GetMetaDataFromEntityStates(hassStateGroup).ToList());

    private static List<EntityMetaData> MapToEntityMetaData(HassStateGroup hassStateGroup)
    {
        var entityMetaDatas = hassStateGroup.Select(state => new EntityMetaData(
            Id: state.EntityId,
            FriendlyName: state.FriendlyName,
            CSharpName: state.ObjectId.ToValidCSharpPascalCase()));

        entityMetaDatas = DeDuplicateCSharpNames(entityMetaDatas);

        return entityMetaDatas.OrderBy(e => e.Id).ToList();
    }

    private static IEnumerable<EntityMetaData> DeDuplicateCSharpNames(IEnumerable<EntityMetaData> entityMetaDatas)
    {
        // The PascalCased EntityId might not be unique because we removed all underscores
        // If we have duplicates we will use the original ID instead and only make sure it is a Valid C# identifier
        // HA entity ID's can only contain [a-z0-9_]. Which are all also valid in Csharp identifiers.
        // HA does allow the id to begin with a digit which is not valid for C#. In those cases it will be prefixed with
        return entityMetaDatas
            .ToLookup(e => e.CSharpName)
            .SelectMany(e => e.Count() == 1
                ? e
                : e.Select(i => i with { CSharpName = i.ObjectId.ToValidCSharpPascalCase() }));
    }
}
