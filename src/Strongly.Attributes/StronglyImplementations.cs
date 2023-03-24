using System;

namespace Strongly
{
    /// <summary>
    /// Interfaces and patterns the strongly typed id should implement
    /// </summary>
    [Flags]
    public enum StronglyImplementations
    {
        // Used with HasFlag, so needs to be 1, 2, 4 etc

        /// <summary>
        /// Don't implement any additional members for the strongly typed ID
        /// </summary>
        None = 0,

        /// <summary>
        /// Use the default implementations for the strongly typed Id.
        /// This will be the value provided in the <see cref="StronglyDefaultsAttribute"/>, which falls back to
        /// <see cref="IEquatable"/> and <see cref="IComparable"/>
        /// </summary>
        Default = 1,

        // ReSharper disable once InconsistentNaming
        /// <summary>
        /// Implement the <see cref="IEquatable{T}"/> interface
        /// </summary>
        IEquatable = 2,

        // ReSharper disable once InconsistentNaming
        /// <summary>
        /// Implement the <see cref="IComparable{T}"/> interface
        /// </summary>
        IComparable = 4,
    }
}