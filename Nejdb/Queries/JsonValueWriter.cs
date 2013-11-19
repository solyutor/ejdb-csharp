using System;
using System.Linq.Expressions;
using Newtonsoft.Json;

namespace Nejdb.Queries
{
    /// <summary>
    /// Helps to writer a value to the <see cref="JsonWriter"/>
    /// </summary>
    /// <typeparam name="TValue">A type of value to writer</typeparam>
    public static class JsonValueWriter<TValue>
    {
        /// <summary>
        /// Delegate that writes a value to <see cref="JsonWriter"/>
        /// </summary>
        public static readonly Action<JsonWriter, TValue> Write;

        static JsonValueWriter()
        {
            var writeMethod = typeof (JsonWriter).GetMethod("WriteValue", new[] {typeof (TValue)});
            if (writeMethod == null)
            {
                throw new InvalidOperationException("Could not find method to write " + typeof (TValue));
            }
            var writerParameter = Expression.Parameter(typeof (JsonWriter));
            var valueParameter = Expression.Parameter(typeof (TValue));

            var call = Expression.Call(writerParameter, writeMethod, valueParameter);

            Write = Expression.Lambda<Action<JsonWriter, TValue>>(call, writerParameter, valueParameter).Compile();
        }
    }
}