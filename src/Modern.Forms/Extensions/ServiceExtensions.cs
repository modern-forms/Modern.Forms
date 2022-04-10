using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modern.Forms
{
    internal static class ServiceExtensions
    {
        /// <summary>
        ///  Gets the service object of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the service object to get.</typeparam>
        /// <param name="provider">The <see cref="IServiceProvider"/> to retrieve the service object from.</param>
        /// <param name="service">If found, contains the service object when this method returns; otherwise, <see langword="null"/>.</param>
        /// <returns>A service object of type <typeparamref name="T"/> or <see langword="null"/> if there is no such service.</returns>
        public static bool TryGetService<T> (
            this IServiceProvider? provider,
            [NotNullWhen (true)] out T? service)
            where T : class
            => (service = provider?.GetService (typeof (T)) as T) is not null;
    }
}
