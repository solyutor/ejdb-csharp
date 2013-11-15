using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Nejdb.Bson;

namespace Nejdb
{
    internal static class IdHelper<TDocument>
    {
        private static readonly Func<TDocument, ObjectId> _getter;
        private static readonly Action<TDocument, ObjectId> _setter;

        static IdHelper()
        {
            var property = typeof(TDocument)
                .GetProperties()
                .SingleOrDefault(p => p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase) && p.PropertyType == typeof(ObjectId));

            if (property == null)
            {
                _getter = document => new ObjectId();
                _setter = delegate { };
            }
            else
            {
                _getter = GenerateGetter(property);
                _setter = GenerateSetter(property);
            }
        }

        private static Action<TDocument, ObjectId> GenerateSetter(PropertyInfo property)
        {
            var documentParameter = Expression.Parameter(typeof(TDocument));
            var idParameter = Expression.Parameter(typeof(ObjectId));

            var idProperty = Expression.Property(documentParameter, property);
            var propertyAssign = Expression.Assign(idProperty, idParameter);


            var lambda = Expression.Lambda<Action<TDocument, ObjectId>>(propertyAssign, documentParameter, idParameter);

            return lambda.Compile();
        }

        private static Func<TDocument, ObjectId> GenerateGetter(PropertyInfo property)
        {
            var documentParameter = Expression.Parameter(typeof(TDocument));
            var idProperty = Expression.Property(documentParameter, property);
            var lambda = Expression.Lambda<Func<TDocument, ObjectId>>(idProperty, documentParameter);

            return lambda.Compile();
        }

        public static ObjectId GetId(TDocument document)
        {
            return _getter(document);
        }

        public static void SetId(TDocument document, ObjectId id)
        {
            _setter(document, id);
        }

    }
}