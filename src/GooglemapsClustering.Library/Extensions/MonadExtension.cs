using System;

namespace GooglemapsClustering.Clustering.Extensions
{
    /// <summary>
    /// http://kunuk.wordpress.com/2014/06/04/monads-for-c-tool-for-cleaner-code/
    /// </summary>
    public static class MonadExtension
    {
        /// <summary>
        /// Maybe monad
        /// </summary>
        /// <typeparam name="TFrom">from type</typeparam>
        /// <typeparam name="TTo">to type</typeparam>
        /// <param name="input">input argument</param>
        /// <param name="evaluator">evaluate whether it is null</param>
        /// <returns>evaluated value</returns>
        public static TTo With<TFrom, TTo>(this TFrom input, Func<TFrom, TTo> evaluator) where TFrom : class
        {
            return input == null ? default(TTo) : evaluator(input);
        }

        /// <summary>
        /// Specify a return value if the chain is undefined
        /// </summary>
        /// <typeparam name="TFrom">from type</typeparam>
        /// <typeparam name="TTo">to type</typeparam>
        /// <param name="input">input argument</param>
        /// <param name="evaluator">evaluate whether it is null</param>
        /// <param name="failureValue">value to be return if chain is undefined</param>
        /// <returns>evaluated value</returns>
        public static TTo Return<TFrom, TTo>(this TFrom input, Func<TFrom, TTo> evaluator, TTo failureValue) where TFrom : class
        {
            return input == null ? failureValue : evaluator(input);
        }
    }
}