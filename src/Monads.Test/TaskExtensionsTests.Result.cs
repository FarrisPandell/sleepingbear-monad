﻿using System.Globalization;
using SleepingBear.Monad.Errors;
using SleepingBear.Monad.Tasks;

namespace SleepingBear.Monad.Monads.Test;

/// <summary>
///     Tests for <see cref="TaskExtensions" />.
/// </summary>
internal static partial class TaskExtensionsTests
{
    [Test]
    public static async Task MapAsync_AsyncMapFunc_ReturnsMappedValue()
    {
        _ = await Result
            .Ok(1234)
            .ToTask()
            .MapAsync(async ok =>
            {
                await Task.Delay(0).ConfigureAwait(false);
                return ok.ToString(CultureInfo.InvariantCulture);
            })
            .TapAsync(
                value => { Assert.That(value, Is.EqualTo("1234")); },
                _ => { Assert.Fail("Should not be called."); })
            .ConfigureAwait(false);
    }

    [Test]
    public static async Task MapAsync_SyncMapFunc_ReturnsMappedValue()
    {
        _ = await Result
            .Ok(1234)
            .ToTask()
            .MapAsync(ok => ok.ToString(CultureInfo.InvariantCulture))
            .TapAsync(
                value => { Assert.That(value, Is.EqualTo("1234")); },
                _ => { Assert.Fail("Should not be called."); })
            .ConfigureAwait(false);
    }

    [Test]
    public static async Task MapErrorAsync_AsyncMapErrorFunc_ReturnsMappedValue()
    {
        _ = await new GenericError<int>(1234)
            .ToResult<int>()
            .ToTask()
            .MapErrorAsync(async _ =>
            {
                await Task.Delay(0).ConfigureAwait(false);
                return "string".ToGenericError();
            })
            .TapAsync(
                _ => { Assert.Fail("Should not be called."); },
                error =>
                {
                    switch (error)
                    {
                        case GenericError<string> stringError:
                            Assert.That(stringError.Value, Is.EqualTo("string"));
                            break;
                        default:
                            Assert.Fail("Should not be called.");
                            break;
                    }
                })
            .ConfigureAwait(false);
    }

    [Test]
    public static async Task MapErrorAsync_SyncMapErrorFunc_ReturnsMappedValue()
    {
        _ = await new GenericError<int>(1234)
            .ToResult<int>()
            .ToTask()
            .MapErrorAsync(_ => "string".ToGenericError())
            .TapAsync(
                _ => { Assert.Fail("Should not be called."); },
                error =>
                {
                    switch (error)
                    {
                        case GenericError<string> stringError:
                            Assert.That(stringError.Value, Is.EqualTo("string"));
                            break;
                        default:
                            Assert.Fail("Should not be called.");
                            break;
                    }
                })
            .ConfigureAwait(false);
    }

    [Test]
    public static async Task BindAsync_AsyncBindFunc_ReturnsMappedValue()
    {
        _ = await Result
            .Ok(1234)
            .ToTask()
            .BindAsync(async ok =>
            {
                await Task.Delay(0).ConfigureAwait(false);
                return Result.Ok(ok.ToString(CultureInfo.InvariantCulture));
            })
            .TapAsync(
                value => { Assert.That(value, Is.EqualTo("1234")); },
                _ => { Assert.Fail("Should not be called."); })
            .ConfigureAwait(false);
    }

    [Test]
    public static async Task BindAsync_SyncBindFunc_ReturnsMappedValue()
    {
        _ = await Result
            .Ok(1234)
            .ToTask()
            .BindAsync(ok => Result.Ok(ok.ToString(CultureInfo.InvariantCulture)))
            .TapAsync(
                value => { Assert.That(value, Is.EqualTo("1234")); },
                _ => { Assert.Fail("Should not be called."); })
            .ConfigureAwait(false);
    }

    [Test]
    public static async Task BindErrorAsync_AsyncBindErrorFunc_ReturnsMappedValue()
    {
        _ = await new GenericError<int>(1234)
            .ToResult<int>()
            .ToTask()
            .BindErrorAsync(async _ =>
            {
                await Task.Delay(0).ConfigureAwait(false);
                return "string".ToGenericError().ToResult<int>();
            })
            .TapAsync(
                _ => { Assert.Fail("Should not be called."); },
                error =>
                {
                    switch (error)
                    {
                        case GenericError<string> stringError:
                            Assert.That(stringError.Value, Is.EqualTo("string"));
                            break;
                        default:
                            Assert.Fail("Should not be called.");
                            break;
                    }
                })
            .ConfigureAwait(false);
    }

    [Test]
    public static async Task BindErrorAsync_SyncBindErrorFunc_ReturnsMappedValue()
    {
        _ = await new GenericError<int>(1234)
            .ToResult<int>()
            .ToTask()
            .BindErrorAsync(_ => "string".ToGenericError().ToResult<int>())
            .TapAsync(
                _ => { Assert.Fail("Should not be called."); },
                error =>
                {
                    switch (error)
                    {
                        case GenericError<string> stringError:
                            Assert.That(stringError.Value, Is.EqualTo("string"));
                            break;
                        default:
                            Assert.Fail("Should not be called.");
                            break;
                    }
                })
            .ConfigureAwait(false);
    }

    [Test]
    public static async Task MatchAsync_SyncFunc_ReturnMatchedValue()
    {
        var matched = await Result
            .Ok(1234)
            .ToTask()
            .MatchAsync(value => value.ToString(CultureInfo.InvariantCulture), _ => "Error")
            .ConfigureAwait(true);
        Assert.That(matched, Is.EqualTo("1234"));
    }

    [Test]
    public static async Task MatchAsync_AsyncFunc_ReturnMatchedValue()
    {
        var matched = await Result
            .Ok(1234)
            .ToTask()
            .MatchAsync(
                async value =>
                {
                    await Task.Delay(0).ConfigureAwait(false);
                    return value.ToString(CultureInfo.InvariantCulture);
                }, async _ =>
                {
                    await Task.Delay(0).ConfigureAwait(false);
                    return "Error";
                })
            .ConfigureAwait(true);
        Assert.That(matched, Is.EqualTo("1234"));
    }

    [Test]
    public static async Task TapAsync_Ok_CallSynchronousOkFunc()
    {
        _ = await Result
            .Ok(1234)
            .ToTask()
            .TapAsync(
                ok => { Assert.That(ok, Is.EqualTo(1234)); },
                _ => { Assert.Fail("Should not be called."); })
            .ConfigureAwait(true);
    }

    [Test]
    public static async Task TapAsync_Ok_CallAsynchronousOkFunc()
    {
        _ = await Result
            .Ok(1234)
            .ToTask()
            .TapAsync(
                ok =>
                {
                    Assert.That(ok, Is.EqualTo(1234));
                    return Task.CompletedTask;
                },
                _ =>
                {
                    Assert.Fail("Should not be called.");
                    return Task.CompletedTask;
                })
            .ConfigureAwait(true);
    }
}