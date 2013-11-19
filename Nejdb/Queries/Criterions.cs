using System;
using System.Linq.Expressions;

namespace Nejdb.Queries
{
    /// <summary>
    /// Contains helper methods for building queries
    /// </summary>
    public static class Criterions
    {
        /// <summary>
        /// Matches that a field value is equals to supplied value.
        /// </summary>
        public static ICriterion Equals<TValue>(TValue value)
        {
            return new EqualsCriterion<TValue>(value);
        }

        /// <summary>
        /// Negates result of supplied criterion
        /// </summary>
        /// <param name="criterion">A Criterion to negate</param>
        public static ICriterion Not(ICriterion criterion)
        {
            return new NotCriterion(criterion);
        }

        /// <summary>
        /// Matches that a string field value starts with specified string 
        /// </summary>
        /// <param name="value">Prefix of a string to search</param>
        public static ICriterion StartsWith(string value)
        {
            return new StartsWithCriterion(value);
        }

        /// <summary>
        /// Matches that a numeric field value is greater than specified value
        /// </summary>
        public static ICriterion GreaterThan<TValue>(TValue value)
        {
            return new NumberComparisonCriterion<TValue>(value, Comparsion.Greater);
        }

        /// <summary>
        /// Matches that a numeric field value is greater than or equal to specified value
        /// </summary>
        public static ICriterion GreaterThanOrEqual<TValue>(TValue value)
        {
            return new NumberComparisonCriterion<TValue>(value, Comparsion.GreaterOrEqual);
        }

        /// <summary>
        /// Matches that a numeric field value is lower than or equal to specified value
        /// </summary>
        public static ICriterion LowerThan<TValue>(TValue value)
        {
            return new NumberComparisonCriterion<TValue>(value, Comparsion.Lower);
        }

        /// <summary>
        /// Matches that a numeric field value is lower than specified value
        /// </summary>
        public static ICriterion LowerThanOrEqual<TValue>(TValue value)
        {
            return new NumberComparisonCriterion<TValue>(value, Comparsion.LowerOrEqual);
        }

        /// <summary>
        /// Creates new instance of <see cref="BetweenCriterion{TValue}"/> 
        /// </summary>
        public static ICriterion Between<TValue>(TValue lower, TValue upper)
        {
            return new BetweenCriterion<TValue>(lower, upper);
        }

        /// <summary>
        /// Checks that a value of a field is equals to any of supplied values;
        /// </summary>
        public static ICriterion In<TValue>(params TValue[] values)
        {
            return new InCriterion<TValue>(values);
        }

        /// <summary>
        /// Checks that a value of a field is not equals to any of supplied values;
        /// </summary>
        public static ICriterion NotIn<TValue>(params TValue[] values)
        {
            return new NotInCriterion<TValue>(values);
        }

        /// <summary>
        /// Matches that field or at least one array element contains ALL specified tokens 
        /// </summary>
        public static ICriterion All(params string[] tokens)
        {
            return new StringAndOrAnyCriterion(tokens, StringAndOrAnyCriterion.Match.All);
        }

        /// <summary>
        /// Matches that a field or at least one array element contains ANY specified token 
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

        /// <summary>
        /// Applies specified critertion to sprecified field. Field path is determined using expression tree.
        /// </summary>
        public static ICriterion Field<TObject, TProperty>(Expression<Func<TObject, TProperty>> property, ICriterion criterion)
        {
            return FieldCriterion.For(property, criterion);
        }

        /// <summary>
        /// Applies specified critertion to sprecified field.
        /// </summary>
        public static ICriterion Field(string path, ICriterion criterion)
        {
            return new FieldCriterion(path, criterion);
        }

        /// <summary>
        /// Combines multiple criterias using logical AND operatero
        /// </summary>
        /// <param name="criterions">Critertions to combines</param>
        public static ICriterion And(params ICriterion[] criterions)
        {
            return new AndCriterion(criterions);
        }

        /// <summary>
        /// Combines multiple criterias using logical OR operatero
        /// </summary>
        /// <param name="criterions">Critertions to combines</param>
        public static ICriterion Or(params ICriterion[] criterions)
        {
            return new OrCriterion(criterions);
        }

        /// <summary>
        /// Match arrays of simple values by a value or array of objects by specified criterion on for fields. 
        /// Fields for object checked using logical AND.
        /// </summary>
        public static ICriterion MatchElement(ICriterion criterion)
        {
            return new MatchElementCriterion(criterion);
        }

        /// <summary>
        /// Encloses criterions to json-object
        /// </summary>
        public static ICriterion Object(params ICriterion[] criterions)
        {
            return new ObjectCriterion(criterions);
        }
    }
}