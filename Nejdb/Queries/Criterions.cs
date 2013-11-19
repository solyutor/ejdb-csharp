namespace Nejdb.Queries
{
    /// <summary>
    /// Contains helper methods for building queries
    /// </summary>
    public static class Criterions
    {
        /// <summary>
        /// Creates new instance of <see cref="EqualsCriterion{TValue}"/> with specified criterion/>
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ICriterion Equals<TValue>(TValue value)
        {
            return new EqualsCriterion<TValue>(value);
        }

        /// <summary>
        /// Creates new instance of <see cref="NotCriterion"/> with specified criterion/>
        /// </summary>
        /// <param name="criterion">A Criterion to negate</param>
        public static ICriterion Not(ICriterion criterion)
        {
            return new NotCriterion(criterion);
        }

        /// <summary>
        /// Creates new instance of <see cref="StartsWithCriterion"/> with specified string prefix
        /// </summary>
        /// <param name="value">Prefix of a string to search</param>
        public static ICriterion StartsWith(string value)
        {
            return new StartsWithCriterion(value);
        }


        /// <summary>
        /// Creates new instance of <see cref="NumberComparisonCriterion{TValue}"/> 
        /// </summary>
        /// <typeparam name="TValue">Any numeric type</typeparam>
        /// <param name="value"></param>
        public static ICriterion GreaterThan<TValue>(TValue value)
        {
            return new NumberComparisonCriterion<TValue>(value, Comparsion.Greater);
        }

        /// <summary>
        /// Creates new instance of <see cref="NumberComparisonCriterion{TValue}"/> 
        /// </summary>
        /// <typeparam name="TValue">Any numeric type</typeparam>
        /// <param name="value"></param>
        public static ICriterion GreaterThanOrEqual<TValue>(TValue value)
        {
            return new NumberComparisonCriterion<TValue>(value, Comparsion.GreaterOrEqual);
        }

        /// <summary>
        /// Creates new instance of <see cref="NumberComparisonCriterion{TValue}"/> 
        /// </summary>
        /// <typeparam name="TValue">Any numeric type</typeparam>
        /// <param name="value"></param>
        public static ICriterion LowerThan<TValue>(TValue value)
        {
            return new NumberComparisonCriterion<TValue>(value, Comparsion.Lower);
        }

        /// <summary>
        /// Creates new instance of <see cref="NumberComparisonCriterion{TValue}"/> 
        /// </summary>
        /// <typeparam name="TValue">Any numeric type</typeparam>
        /// <param name="value"></param>
        public static ICriterion LowerThanOrEqual<TValue>(TValue value)
        {
            return new NumberComparisonCriterion<TValue>(value, Comparsion.LowerOrEqual);
        }

        /// <summary>
        /// Creates new instance of <see cref="BetweenCriterion{TValue}"/> 
        /// </summary>
        /// <typeparam name="TValue">Any numeric type</typeparam>
        /// <param name="lower">Lower limit</param>
        /// <param name="upper">Upper limit</param>
        public static ICriterion Between<TValue>(TValue lower, TValue upper)
        {
            return new BetweenCriterion<TValue>(lower, upper);
        }

        public static ICriterion In<TValue>(params TValue[] values)
        {
            return new InCriterion<TValue>(values);
        }

        public static ICriterion NotIn<TValue>(params TValue[] values)
        {
            return new NotInCriterion<TValue>(values);
        }

        /// <summary>
        /// Matches that field or array at lease one array element contains ALL specified tokens 
        /// </summary>
        public static ICriterion All(params string[] tokens)
        {
            return new StringAndOrAnyCriterion(tokens, StringAndOrAnyCriterion.Match.All);
        }

        /// <summary>
        /// Matches that field or array at lease one array element contains ANY specified token 
        /// </summary>
        public static ICriterion Any(params string[] tokens)
        {
            return new StringAndOrAnyCriterion(tokens, StringAndOrAnyCriterion.Match.Any);
        }

        /// <summary>
        /// Compares string in case insensetive mode
        /// </summary>
        public static ICriterion IgnoreCase(ICriterion subCriterion)
        {
            return new IgnoreCaseCriterion(subCriterion);
        }

        /// <summary>
        /// Match that field exists
        /// </summary>
        public static ICriterion FieldExists()
        {
            return new FieldExistsCriterion(true);
        }

        /// <summary>
        /// Match that field does not exist
        /// </summary>
        public static ICriterion FieldNotExists()
        {
            return new FieldExistsCriterion(false);
        }
    }
}