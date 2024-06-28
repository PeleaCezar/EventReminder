using Domain.Core.Errors;
using Domain.Core.Primitives;
using Domain.Core.Primitives.Result;

namespace Domain.ValueObjects
{
    public sealed class FirstName : ValueObject
    {
        /// <summary>
        /// The first name maximum length.
        /// </summary>
        public const int MaxLength = 100;

        /// <summary>
        /// Initializes a new instance of the <see cref="FirstName"/> class.
        /// </summary>
        /// <param name="value">The first name value.</param>
        private FirstName(string value) => Value = value;

        /// <summary>
        /// Gets the first name value.
        /// </summary>
        public string Value { get; }

        public static implicit operator string(FirstName firstName) => firstName.Value;

        /// <summary>
        /// Creates a new <see cref="FirstName"/> instance based on the specified value.
        /// </summary>
        /// <param name="firstName">The first name value.</param>
        /// <returns>The result of the first name creation process containing the first name or an error.</returns>
        public static Result<FirstName> Create(string firstName) =>
            Result.Create(firstName, DomainErrors.FirstName.NullOrEmpty)
                .Ensure(f => !string.IsNullOrWhiteSpace(f), DomainErrors.FirstName.NullOrEmpty)
                .Ensure(f => f.Length <= MaxLength, DomainErrors.FirstName.LongerThanAllowed)
                .Map(f => new FirstName(f));

        public override string ToString() => Value;

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }
    }
}
