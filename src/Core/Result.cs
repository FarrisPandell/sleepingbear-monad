﻿using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace SleepingBear.Monad.Core;

/// <summary>
///     Result state enumeration.
/// </summary>
public enum ResultState
{
    /// <summary>
    ///     Invalid state - indicates the Result struct was constructed with the default constructor.
    /// </summary>
    Invalid = 0,

    /// <summary>
    ///     OK state.
    /// </summary>
    Ok,

    /// <summary>
    ///     Failure state.
    /// </summary>
    Failure
}

/// <summary>
///     Result monad.
/// </summary>
/// <typeparam name="TOk">The OK type.</typeparam>
public readonly struct Result<TOk> : IEquatable<Result<TOk>>
{
    private readonly ResultState _state;
    private readonly TOk? _ok;
    private readonly Error? _error;

    internal Result(TOk ok)
    {
        this._ok = ok;
        this._error = null;
        this._state = ResultState.Ok;
    }

    internal Result(Error error)
    {
        this._ok = default;
        this._error = error;
        this._state = ResultState.Failure;
    }

    /// <summary>
    ///     Indicates the result is in the 'OK' state.
    /// </summary>
    public bool IsOk => this._state == ResultState.Ok;

    /// <summary>
    ///     Indicates the result in the 'Failure' state.
    /// </summary>
    public bool IsFailure => this._state == ResultState.Failure;

    /// <summary>
    ///     Deconstructs the result.
    /// </summary>
    /// <param name="state">The state.</param>
    /// <param name="ok">The OK value.</param>
    /// <param name="error">The failure value.</param>
    public void Deconstruct(out ResultState state, out TOk? ok, out Error? error)
    {
        state = this._state;
        ok = this._ok;
        error = this._error;
    }

    /// <inheritdoc cref="object" />
    public override bool Equals(object? obj)
    {
        return obj is Result<TOk> other && this.Equals(other);
    }

    /// <inheritdoc cref="object" />
    public override int GetHashCode()
    {
        return HashCode.Combine((int)this._state, this._ok, this._error);
    }

    /// <summary>
    ///     Equality operator.
    /// </summary>
    public static bool operator ==(Result<TOk> left, Result<TOk> right)
    {
        return left.Equals(right);
    }

    /// <summary>
    ///     Inequality operator.
    /// </summary>
    public static bool operator !=(Result<TOk> left, Result<TOk> right)
    {
        return !(left == right);
    }

    /// <inheritdoc cref="IEquatable{T}" />
    public bool Equals(Result<TOk> other)
    {
        return this._state == other._state &&
               EqualityComparer<TOk?>.Default.Equals(this._ok, other._ok) &&
               Equals(this._error, other._error);
    }

    /// <summary>
    ///     Map a <see cref="Result{TOk}" />.
    /// </summary>
    /// <param name="map">The mapping function.</param>
    /// <typeparam name="TOkOut">The output OK type.</typeparam>
    /// <returns>A <see cref="Result{TOk}" />.</returns>
    /// <exception cref="InvalidOperationException">Thrown if state is Invalid.</exception>
    /// <exception cref="UnreachableException">Thrown if the state is unknown.</exception>
    [SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
    public Result<TOkOut> Map<TOkOut>(Func<TOk, TOkOut> map)
    {
        ArgumentNullException.ThrowIfNull(map);

        return this._state switch
        {
            ResultState.Ok => new Result<TOkOut>(map(this._ok!)),
            ResultState.Failure => new Result<TOkOut>(this._error!),
            ResultState.Invalid => throw new InvalidOperationException(),
            _ => throw new UnreachableException()
        };
    }

    /// <summary>
    ///     Map a <see cref="Result{TOk}" />.
    /// </summary>
    /// <param name="map">The mapping function.</param>
    /// <returns>A <see cref="Result{TOk}" />.</returns>
    /// <exception cref="InvalidOperationException">Thrown if state is Invalid.</exception>
    /// <exception cref="UnreachableException">Thrown if the state is unknown.</exception>
    [SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
    public Result<TOk> MapFailure(Func<Error, Error> map)
    {
        ArgumentNullException.ThrowIfNull(map);

        return this._state switch
        {
            ResultState.Ok => this,
            ResultState.Failure => new Result<TOk>(map(this._error!)),
            ResultState.Invalid => throw new InvalidOperationException(),
            _ => throw new UnreachableException()
        };
    }

    /// <summary>
    ///     Binds a <see cref="Result{TOk}" />.
    /// </summary>
    /// <param name="bind">The binding function.</param>
    /// <typeparam name="TOkOut">The output OK type.</typeparam>
    /// <returns>A <see cref="Result{TOk}" />.</returns>
    /// <exception cref="InvalidOperationException">Thrown if state is Invalid.</exception>
    /// <exception cref="UnreachableException">Thrown if the state is unknown.</exception>
    [SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
    public Result<TOkOut> Bind<TOkOut>(Func<TOk, Result<TOkOut>> bind)
    {
        ArgumentNullException.ThrowIfNull(bind);

        return this._state switch
        {
            ResultState.Ok => bind(this._ok!),
            ResultState.Failure => new Result<TOkOut>(this._error!),
            ResultState.Invalid => throw new InvalidOperationException(),
            _ => throw new UnreachableException()
        };
    }

    /// <summary>
    ///     Bind a <see cref="Result{TOk}" />.
    /// </summary>
    /// <param name="bind">The binding function.</param>
    /// <returns>A <see cref="Result{TOk}" />.</returns>
    /// <exception cref="InvalidOperationException">Thrown if state is Invalid.</exception>
    /// <exception cref="UnreachableException">Thrown if the state is unknown.</exception>
    [SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
    public Result<TOk> BindFailure(Func<Error, Result<TOk>> bind)
    {
        ArgumentNullException.ThrowIfNull(bind);

        return this._state switch
        {
            ResultState.Ok => this,
            ResultState.Failure => bind(this._error!),
            ResultState.Invalid => throw new InvalidOperationException(),
            _ => throw new UnreachableException()
        };
    }

    /// <summary>
    ///     Matches the <see cref="Result{TOk}" />.
    /// </summary>
    /// <param name="ok">The OK function.</param>
    /// <param name="error">The error function.</param>
    /// <typeparam name="TOut">The output type.</typeparam>
    /// <returns>The matched value.</returns>
    /// <exception cref="InvalidOperationException">Thrown if state is Invalid.</exception>
    /// <exception cref="UnreachableException">Thrown if the state is unknown.</exception>
    [SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
    public TOut Match<TOut>(Func<TOk, TOut> ok, Func<Error, TOut> error)
    {
        ArgumentNullException.ThrowIfNull(ok);
        ArgumentNullException.ThrowIfNull(error);

        return this._state switch
        {
            ResultState.Ok => ok(this._ok!),
            ResultState.Failure => error(this._error!),
            ResultState.Invalid => throw new InvalidOperationException(),
            _ => throw new UnreachableException()
        };
    }
}