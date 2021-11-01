using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;

namespace Holoone.Api.Helpers.Extensions
{
    public static class GetPropertyInfo
    {
        public static IEnumerable<NameValueCollection> GetProperties<T>(this T obj) where T : class
        {
            Type objType = obj.GetType();
            PropertyInfo[] properties = objType.GetProperties();

            IContractResolver resolver = JsonSerializer.CreateDefault().ContractResolver;
            int i = 0;

            foreach (PropertyInfo pi in properties)
            {
                var contract = resolver.ResolveContract(objType) as JsonObjectContract;
                if (contract == null)
                    yield return new NameValueCollection { { "", "" } };

                var propertyName = contract.Properties[i++].PropertyName;
                var propertyValue = pi.GetValue(obj, null)?.ToString();

                if(string.IsNullOrEmpty(propertyValue))
                    yield return new NameValueCollection { { "", "" } };

                yield return new NameValueCollection { { propertyName, propertyValue } };
            }
        }
    }
}
