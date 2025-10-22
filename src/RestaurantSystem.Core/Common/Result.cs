using System.Collections.Generic;

namespace RestaurantSystem.Core.Common;

/// <summary>
/// Generic result class for handling operation results with validation
/// </summary>
public class Result<T>
{
    private Result(bool succeeded, IEnumerable<string> errors, T value)
    {
    Succeeded = succeeded;
        Errors = errors;
        Value = value;
    }

    public bool Succeeded { get; }
    public IEnumerable<string> Errors { get; }
    public T Value { get; }
    public bool Failed => !Succeeded;

    public static Result<T> Success(T value) => new(true, Array.Empty<string>(), value);
    public static Result<T> Failure(IEnumerable<string> errors) => new(false, errors, default);
  public static Result<T> Failure(string error) => Failure(new[] { error });
}

/// <summary>
/// Non-generic result for operations without return value
/// </summary>
public class Result
{
  protected Result(bool succeeded, IEnumerable<string> errors)
    {
        Succeeded = succeeded;
        Errors = errors;
    }

    public bool Succeeded { get; }
    public IEnumerable<string> Errors { get; }
    public bool Failed => !Succeeded;

    public static Result Success() => new(true, Array.Empty<string>());
    public static Result Failure(IEnumerable<string> errors) => new(false, errors);
    public static Result Failure(string error) => Failure(new[] { error });
}