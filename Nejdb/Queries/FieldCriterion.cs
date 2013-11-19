using System;
using System.Linq.Expressions;
using Newtonsoft.Json;

namespace Nejdb.Queries
{
    /// <summary>
    /// 
    /// </summary>
    public class FieldCriterion : ICriterion
    {
        private readonly string _path;
        private readonly ICriterion _criterion;

        public FieldCriterion(string path, ICriterion criterion)
        {
            _path = path;
            _criterion = criterion;
        }

        public void WriteTo(JsonWriter writer)
        {
            writer.WritePropertyName(_path);
            _criterion.WriteTo(writer);
        }

        public static FieldCriterion For<TObject, TProperty>(Expression<Func<TObject, TProperty>> path, ICriterion criterion)
        {
            var stringPath = path.ToMemberPath();
            return new FieldCriterion(stringPath, criterion);
        }
    }
}