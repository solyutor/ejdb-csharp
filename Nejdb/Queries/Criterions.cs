namespace Nejdb.Queries
{
    /// <summary>
    /// Contains helper methods for building queries
    /// </summary>
    public static class Criterions
    {
        /// <summary>
        /// Creates new instance of <see cref="EqualsCriterion{TValue}"/> with specified value/>
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ICriterion Equals<TValue>(TValue value)
        {
            return new EqualsCriterion<TValue>(value);
        }

        /// <summary>
        /// Creates new instance of <see cref="NotEqualsCriterion{TValue}"/> with specified value/>
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ICriterion NotEquals<TValue>(TValue value)
        {
            return new NotEqualsCriterion<TValue>(value);
        }

        /// <summary>
        /// Creates new instance of <see cref="StartsWithCriterion"/> with specified string prefix
        /// </summary>
        /// <param name="value">Prefix of a string to search</param>
        public static ICriterion StartsWith(string value)
        {
            return new StartsWithCriterion(value);
        }
    }
}