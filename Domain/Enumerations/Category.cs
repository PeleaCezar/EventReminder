using Domain.Core.Primitives;

namespace Domain.Enumerations;

public sealed class Category : Enumeration<Category>
{
    public static readonly Category None = new Category(0, "None");
    public static readonly Category Concert = new Category(0, "Concert");

    /// <summary>
    /// Initializes a new instance of the <see cref="Category"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="name">The name.</param>
    private Category(int value, string name)
        : base(value, name)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Category"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <remarks>
    /// Required by EF Core.
    /// </remarks>
    private Category(int value)
        : base(value, FromValue(value).Value.Name)
    {
    }
}
