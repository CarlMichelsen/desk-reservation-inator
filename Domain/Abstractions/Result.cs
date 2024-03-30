using Domain.Exception;

namespace Domain.Abstractions;

public sealed class Result<T>
{
    private readonly T? value;
    private readonly Error? error;

    public Result(T value)
    {
        this.IsError = false;
        this.value = value;
        this.error = default;
    }

    private Result(Error error)
    {
        this.IsError = true;
        this.value = default;
        this.error = error;
    }

    public bool IsError { get; private set; }

    public bool IsSuccess => !this.IsError;

    public Error? Error => this.error;

    public static implicit operator Result<T>(T value) => new(value);

    public static implicit operator Result<T>(Error error) => new(error);

    public TResult Match<TResult>(
        Func<T, TResult> success,
        Func<Error, TResult> failure) =>
        !this.IsError ? success(this.value!) : failure(this.error!);

    public T Unwrap()
    {
        return this.Match(
            (T value) => value,
            (Error error) => throw new UnhandledResultErrorException(error));
    }
}