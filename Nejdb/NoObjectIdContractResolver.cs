using System;
using System.Collections.Generic;
using System.Linq;
using Nejdb.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Nejdb
{
	internal class NoObjectIdContractResolver : DefaultContractResolver
	{
		public static readonly NoObjectIdContractResolver Instance = new NoObjectIdContractResolver();
		
		protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
		{
			var jsonProperties = base.CreateProperties(type, memberSerialization);
			var idProperty = jsonProperties.SingleOrDefault(
				x => x.PropertyType == typeof (ObjectId) &&
				     (x.PropertyName.Equals("Id", StringComparison.OrdinalIgnoreCase) ||
				      (x.PropertyName.Equals("_Id", StringComparison.OrdinalIgnoreCase))));

			if (idProperty != null)
			{
				jsonProperties.Remove(idProperty);
			}

			return jsonProperties;
		}
	}
}