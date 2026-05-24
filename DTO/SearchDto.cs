namespace Testing3.DTO;
public class UserSearchRequest
{
    public required UserFilterTarget Values { get; set; }
}
public class UserFilterTarget
{
    public RangeFilter<int>? Age { get; set; } = null;
    public int? AgeOut { get; set; }
    public string? Name { get; set; } = null;
    public RangeFilter<int>? PostCount { get; set; } = null;
    public int PostCountOut { get; set; }
    public string? Username { get; set; } = null;
    public string? City { get; set; } = null;
    public string? Hobby { get; set; } = null;
    public string? Interests { get; set; } = null;
    public RangeFilter<DateTime>? Date { get; set; } = null;
    public DateTime DateOut { get; set; }
}

public enum StringFilterMode
{
    Equals,
    Contains,
    StartsWith,
    EndsWith
}
public class RangeFilter<T> where T : struct, IComparable
{
    public T? Min { get; set; } = null;
    public T? Max { get; set; } = null;
    public bool Contains(T value)
    {
        if (Min != null && value.CompareTo(Min) < 0) return false;
        if (Max != null && value.CompareTo(Max) > 0) return false;
        return true;
    }
}
public class StringFilter //позже
{
    public required string Value { get; set; }
    public StringFilterMode Mode { get; set; }
}

// public class UserDto
// {
//     public string Username { get; set; }
//     public int Age { get; set; }
//     public DateTime Date { get; set; }
//     public int PostCount { get; set; }
//     public string City { get; set; }
//     public string? Hobby { get; set; } = null;
//     public string? Interests { get; set; } = null;
// }