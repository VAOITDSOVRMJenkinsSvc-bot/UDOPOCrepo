using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace VRM.CRME.Plugin.DependentMaintenance
{
    public static class EntityExtensions
    {
        public static void InjectValue(this Entity entity, 
            string targetAttribute, 
            object targetProperty, 
            ColumnSet columnSet)
        {
            if (columnSet.Columns.Contains(targetAttribute) && targetProperty != null)
                entity.Attributes.Add(targetAttribute, targetProperty);
        }

        public static void InjectAttribute(this Entity entity,
            string targetAttribute,
            object value,
            ColumnSet columnSet)
        {
            if (columnSet.Columns.Contains(targetAttribute))
            {
                if (!entity.Attributes.Contains(targetAttribute))
                {
                    entity.Attributes.Add(targetAttribute, value);
                }
                else
                {
                    entity.Attributes[targetAttribute] = value;
                }
            }

        }
    }
}