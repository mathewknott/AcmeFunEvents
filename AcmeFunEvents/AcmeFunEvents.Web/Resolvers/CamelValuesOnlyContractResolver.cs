using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AcmeFunEvents.Web.Resolvers
{
    public class CamelValuesOnlyContractResolver : CamelCasePropertyNamesContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (property.PropertyType == typeof (string))
            {
                property.ShouldSerialize = instance =>! string.IsNullOrEmpty(Convert.ToString(property.ValueProvider.GetValue(instance)));
            }
            else if (property.PropertyType == typeof (bool))
            {
                property.ShouldSerialize = instance => Convert.ToBoolean(property.ValueProvider.GetValue(instance));
            }
            else if (property.PropertyType == typeof (int))
            {
                property.ShouldSerialize = instance => !Convert.ToInt32(property.ValueProvider.GetValue(instance)).Equals(0);
            }
            else if (property.PropertyType == typeof (DateTime))
            {
                property.ShouldSerialize = instance => !Convert.ToDateTime(property.ValueProvider.GetValue(instance)).Equals(DateTime.MinValue);
            }
            else if (property.PropertyType == typeof (float))
            {
                property.ShouldSerialize = instance => Math.Abs(Convert.ToSingle(property.ValueProvider.GetValue(instance))) > 0;
            }
            else
            {
                property.ShouldSerialize = instance => property.ValueProvider.GetValue(instance) != null;
            }

            return property;
        }
    }
}
