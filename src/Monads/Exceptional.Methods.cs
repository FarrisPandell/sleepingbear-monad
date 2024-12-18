using System.Diagnostics.CodeAnalysis;
using SleepingBear.Monad.Core;
using SleepingBear.Monad.Errors;

namespace SleepingBear.Monad.Monads;

/// <summary>
///     Extension and helper methods for <see cref="Exceptional{TValue}" />.
/// </summary>
public static class Exceptional
{
    /// <summary>
    ///     Lifts a value to a <see cref="Exceptional{TValue}" />.
    /// </summary>
    /// <param name="value">The value being lifted.</param>
    /// <typeparam name="TValue">The type of the lifted value.</typeparam>
    /// <returns>A <see cref="Exceptional{TValue}" /> containing the lifted value.</returns>
    public static Exceptional<TValue> Success<TValue>(TValue value) where TValue : notnull
    {
        return new Exceptional<TValue>(value);
    }

    /// <summary>
    ///     Converts a <see cref="Exception" /> to a <see cref="Exceptional{TValue}" />.
    /// </summary>
    /// <param name="exception">The exception.></param>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <returns>A <see cref="Exceptional{TValue}" />.</returns>
    public static Exceptional<TValue> ToExceptional<TValue>(this Exception exception) where TValue : notnull
    {
        return new Exceptional<TValue>(exception);
    }

    /// <summary>
    ///     Converts a <see cref="Exceptional{TValue}" /> to a <see cref="Result{TOk}" />.
    /// </summary>
    /// <param name="exceptional">The exceptional.</param>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <returns>A <see cref="Result{TOk}" />.</returns>
    [SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
    public static Result<TValue> ToResult<TValue>(this Exceptional<TValue> exceptional) where TValue : notnull
    {
        var (isSuccess, value, exception) = exceptional;
        return isSuccess
            ? value!
            : exception!.ToGenericError().ToResult<TValue>();
    }

    /// <summary>
    ///     Maps a <see cref="Exceptional{TValue}" />.
    /// </summary>
    /// <param name="exceptional">The exceptional being mapped.</param>
    /// <param name="mapFunc">The mapping function.</param>
    /// <typeparam name="TValueOut">The output value type.</typeparam>
    /// <typeparam name="TValue">The type of the lifted value.</typeparam>
    /// <returns>A <see cref="Exceptional{TValue}" />.</returns>
    /// <remarks>
    ///     The <paramref name="mapFunc" /> should NOT throw any exceptions.
    /// </remarks>
    [SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
    public static Exceptional<TValueOut> Map<TValue, TValueOut>(
        this Exceptional<TValue> exceptional,
        Func<TValue, TValueOut> mapFunc) where TValue : notnull where TValueOut : notnull
    {
        ArgumentNullException.ThrowIfNull(mapFunc);

        var (isSuccess, value, exception) = exceptional;
        return isSuccess
            ? new Exceptional<TValueOut>(mapFunc(value!))
            : new Exceptional<TValueOut>(exception!);
    }

    /// <summary>
    ///     Taps a <see cref="Exceptional{TValue}" />.
    /// </summary>
    /// <param name="exceptional">The exceptional being tapped.</param>
    /// <param name="valueAction">The 'Value' action.</param>
    /// <param name="exceptionAction">The 'Exception' action.</param>
    /// <returns>The <see cref="Exceptional{TValue}" /> instance.</returns>
    /// <remarks>
    ///     The <paramref name="valueAction" /> and <paramref name="exceptionAction" /> should NOT
    ///     throw any exceptions.
    /// </remarks>
    [SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
    public static Exceptional<TValue> Tap<TValue>(
        this Exceptional<TValue> exceptional,
        Action<TValue> valueAction,
        Action<Exception> exceptionAction) where TValue : notnull
    {
        ArgumentNullException.ThrowIfNull(valueAction);
        ArgumentNullException.ThrowIfNull(exceptionAction);

        var (isSuccess, value, exception) = exceptional;
        if (isSuccess)
        {
            valueAction(value!);
        }
        else
        {
            exceptionAction(exception!);
        }

        return exceptional;
    }

    /// <summary>
    ///     Binds a <see cref="Exceptional{TValue}" />.
    /// </summary>
    /// <param name="exceptional">The exceptional being bound.</param>
    /// <param name="bindFunc">The binding function.</param>
    /// <typeparam name="TValueOut">The output value type.</typeparam>
    /// <typeparam name="TValue">The type of the lifted value.</typeparam>
    /// <returns>A <see cref="Exceptional{TValue}" />.</returns>
    /// <remarks>
    ///     The <paramref name="bindFunc" /> should NOT throw any exceptions.
    /// </remarks>
    [SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
    public static Exceptional<TValueOut> Bind<TValue, TValueOut>(
        this Exceptional<TValue> exceptional,
        Func<TValue, Exceptional<TValueOut>> bindFunc)
        where TValue : notnull
        where TValueOut : notnull
    {
        ArgumentNullException.ThrowIfNull(bindFunc);

        var (isSuccess, value, exception) = exceptional;
        return isSuccess
            ? bindFunc(value!)
            : new Exceptional<TValueOut>(exception!);
    }

    /// <summary>
    ///     Try to get the value from <see cref="Exceptional{TValue}" />.
    /// </summary>
    /// <param name="exceptional">The exceptional being checked.</param>
    /// <param name="value">The value.</param>
    /// <returns>True if state is 'Value', false otherwise.</returns>
    [SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
    public static bool Try<TValue>(
        this Exceptional<TValue> exceptional,
        [NotNullWhen(true)] out TValue? value) where TValue : notnull
    {
        var (isSuccess, value2, _) = exceptional;
        value = isSuccess
            ? value2!
            : default;
        return isSuccess;
    }

    /// <summary>
    ///     Matches a <see cref="Exceptional{TValue}" />.
    /// </summary>
    /// <param name="exceptional">The exceptional being matched.</param>
    /// <param name="valueFunc">The 'Value' function.</param>
    /// <param name="exceptionFunc">The 'Exception' function.</param>
    /// <typeparam name="TValueOut">The output value type.</typeparam>
    /// <typeparam name="TValue">The type of the lifted value.</typeparam>
    /// <returns>The matched value.</returns>
    [SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
    public static TValueOut Match<TValue, TValueOut>(
        this Exceptional<TValue> exceptional,
        Func<TValue, TValueOut> valueFunc, Func<Exception, TValueOut> exceptionFunc) where TValue : notnull
    {
        ArgumentNullException.ThrowIfNull(valueFunc);
        ArgumentNullException.ThrowIfNull(exceptionFunc);

        var (isSuccess, value, exception) = exceptional;
        return isSuccess
            ? valueFunc(value!)
            : exceptionFunc(exception!);
    }

    /// <summary>
    ///     Tries to execute a function and return an <see cref="Exceptional{TValue}" />.
    /// </summary>
    /// <param name="func">The function.</param>
    /// <typeparam name="TValue">The return value type of function.</typeparam>
    /// <returns>A <see cref="Exceptional{TValue}" /> containing the return value of the function.</returns>
    [SuppressMessage("Design", "CA1031:Do not catch general exception types")]
    public static Exceptional<TValue> TryCatch<TValue>(Func<TValue> func) where TValue : notnull
    {
        ArgumentNullException.ThrowIfNull(func);

        try
        {
            return func();
        }
        catch (Exception ex)
        {
            ex.FailFastIfCritical("SleepingBear.Monad.Monads.ExceptionalExtensions.Try");
            return ex;
        }
    }

    /// <summary>
    ///     Tries to execute a function and return an <see cref="Exceptional{TValue}" />.
    /// </summary>
    /// <param name="func">The function.</param>
    /// <typeparam name="TValue">The type of the function's return value.</typeparam>
    /// <typeparam name="T1">The type of exception to catch.</typeparam>
    /// <returns>A <see cref="Exceptional{TValue}" /> containing the return value of the function.</returns>
    public static Exceptional<TValue> TryCatch<TValue, T1>(Func<TValue> func)
        where TValue : notnull where T1 : Exception
    {
        ArgumentNullException.ThrowIfNull(func);

        try
        {
            return func();
        }
        catch (T1 ex)
        {
            return ex;
        }
    }

    /// <summary>
    ///     Tries to execute a function and return an <see cref="Exceptional{TValue}" />.
    /// </summary>
    /// <param name="func">The function.</param>
    /// <typeparam name="TValue">The type of the function's return value.</typeparam>
    /// <typeparam name="T1">The type of exception to catch.</typeparam>
    /// <typeparam name="T2">The type of exception to catch.</typeparam>
    /// <returns>A <see cref="Exceptional{TValue}" /> containing the return value of the function.</returns>
    public static Exceptional<TValue> TryCatch<TValue, T1, T2>(Func<TValue> func)
        where TValue : notnull
        where T1 : Exception
        where T2 : Exception
    {
        ArgumentNullException.ThrowIfNull(func);

        try
        {
            return func();
        }
        catch (T1 ex)
        {
            return ex;
        }
        catch (T2 ex)
        {
            return ex;
        }
    }

    /// <summary>
    ///     Tries to execute a function and return an <see cref="Exceptional{TValue}" />.
    /// </summary>
    /// <param name="func">The function.</param>
    /// <typeparam name="TValue">The type of the function's return value.</typeparam>
    /// <typeparam name="T1">The type of exception to catch.</typeparam>
    /// <typeparam name="T2">The type of exception to catch.</typeparam>
    /// <typeparam name="T3">The type of exception to catch.</typeparam>
    /// <returns>A <see cref="Exceptional{TValue}" /> containing the return value of the function.</returns>
    public static Exceptional<TValue> TryCatch<TValue, T1, T2, T3>(Func<TValue> func)
        where TValue : notnull
        where T1 : Exception
        where T2 : Exception
        where T3 : Exception
    {
        ArgumentNullException.ThrowIfNull(func);

        try
        {
            return func();
        }
        catch (T1 ex)
        {
            return ex;
        }
        catch (T2 ex)
        {
            return ex;
        }
        catch (T3 ex)
        {
            return ex;
        }
    }
}