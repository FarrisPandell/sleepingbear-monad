﻿namespace SleepingBear.Monad.Errors.Test;

/// <summary>
///     Tests for <see cref="Error" />.
/// </summary>
internal static class ErrorTests
{
    [Test]
    public static void Ctor_ValidatesBehavior()
    {
        var error = new GenericError<int>(1234);
        Assert.That(error.Value, Is.EqualTo(1234));
    }
}