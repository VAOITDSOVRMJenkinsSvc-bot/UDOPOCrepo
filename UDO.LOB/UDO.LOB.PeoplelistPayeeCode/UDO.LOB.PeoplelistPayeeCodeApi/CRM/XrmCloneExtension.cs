using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using VRM.Integration.UDO.Common;

namespace UDO.LOB.PeoplelistPayeeCode.CRM
{
    public static class XrmCloneExtension
    {
        public static Entity Clone(this Entity source, Entity target = null, bool includeRelatedEntities = false)
        {
            if (target == null)
            {
                target = new Entity(source.LogicalName);
            }

            target.Id = source.Id;
            target.EntityState = source.EntityState;
            foreach (var formattedValue in source.FormattedValues)
            {
                target.FormattedValues[formattedValue.Key] = formattedValue.Value;
            }

            foreach (var attribute in source.Attributes)
            {
                var attributeValue = attribute.Value;
                if (attributeValue is string)
                {
                    // This is the most common, so do it first
                }
                else if (attributeValue is EntityReference)
                {
                    var er = (EntityReference)attributeValue;
                    attributeValue = new EntityReference(er.LogicalName, er.Id) { Name = er.Name };
                }
                else if (attributeValue is OptionSetValue)
                {
                    attributeValue = new OptionSetValue(((OptionSetValue)attributeValue).Value);
                }
                else if (attributeValue is Money)
                {
                    attributeValue = new Money(((Money)attributeValue).Value);
                }
                else if (attributeValue is EntityCollection && includeRelatedEntities)
                {
                    var cloneCollection = Clone((EntityCollection)attributeValue, null, true);
                    attributeValue = cloneCollection;
                }

                target.Attributes[attribute.Key] = attributeValue;
            }

            if (!includeRelatedEntities) return target;

            foreach (var relatedEntity in source.RelatedEntities)
            {
                target.RelatedEntities[relatedEntity.Key] = Clone(relatedEntity.Value, null, true);
            }

            return target;
        }

        public static EntityCollection Clone(
            this EntityCollection source,
            EntityCollection target = null,
            bool includeRelatedEntities = false)
        {
            if (source == null) return null;
            if (target != null && !target.EntityName.Equals(source.EntityName, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException(
                    "Target Entity Collection must be of the same type of Entity as the Source Entity Collection");
            }

            if (target == null)
            {
                return
                    new EntityCollection(
                        source.Entities.Select(sourceEntity => Clone(sourceEntity, null, includeRelatedEntities))
                            .ToList())
                    {
                        EntityName = source.EntityName,
                        MinActiveRowVersion = source.MinActiveRowVersion,
                        MoreRecords = source.MoreRecords,
                        PagingCookie = source.PagingCookie,
                        TotalRecordCount = source.TotalRecordCount,
                        TotalRecordCountLimitExceeded = source.TotalRecordCountLimitExceeded
                    };
            }

            var newRecords = new List<Entity>(source.Entities.Count);
            foreach (var sourceEntity in source.Entities)
            {
                if (sourceEntity.Id == Guid.Empty)
                {
                    newRecords.Add(Clone(sourceEntity, null, includeRelatedEntities));
                    continue;
                }

                var matchFound = false;
                foreach (var targetEntity in target.Entities)
                {
                    if (!targetEntity.LogicalName.Equals(sourceEntity.LogicalName, StringComparison.OrdinalIgnoreCase)) continue;
                    if (!targetEntity.Id.Equals(sourceEntity.Id)) continue;
                    Clone(sourceEntity, targetEntity, includeRelatedEntities);
                    matchFound = true;
                }

                if (!matchFound)
                {
                    newRecords.Add(Clone(sourceEntity, null, includeRelatedEntities));
                }
            }

            target.Entities.AddRange(newRecords);
            return target;
        }

        
    }
}
