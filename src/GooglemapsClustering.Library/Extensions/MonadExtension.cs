using System;

namespace GooglemapsClustering.Clustering.Extensions
{
	/// <summary>
	/// http://kunuk.wordpress.com/2014/06/04/monads-for-c-tool-for-cleaner-code/
	/// </summary>
	public static class MonadExtension
	{
		/// <summary>
		/// Maybe Monad
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <param name="input"></param>
		/// <param name="evaluator"></param>
		/// <returns></returns>
		public static T2 _<T, T2>(this T input, Func<T, T2> evaluator)
			where T2 : class
			where T : class
		{
			return input == null ? null : evaluator(input);
		}
	}
}