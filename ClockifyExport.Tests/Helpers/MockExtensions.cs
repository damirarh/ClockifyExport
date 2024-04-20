using Microsoft.Extensions.Logging;
using Moq;

namespace ClockifyExport.Tests.Helpers;

/// <summary>
/// Extension methods for working with Moq mocks.
/// </summary>
internal static class MockExtensions
{
    /// <summary>
    /// Verifies that the logger was called with the expected parameters.
    /// </summary>
    /// <typeparam name="T">Type argument for the logger.</typeparam>
    /// <param name="loggerMock">Logger mock to verify.</param>
    /// <param name="expectedLogLevel">Expected log level of the log entry.</param>
    /// <param name="expectedEventId">Expected id of the logged event id object.</param>
    /// <param name="expectedMessage">Expected substring to appear in the formatted log message.</param>
    /// <param name="expectedTimes">Number of times the log method is expected to have been called.</param>
    internal static void VerifyLog<T>(
        this Mock<ILogger<T>> loggerMock,
        LogLevel expectedLogLevel,
        int expectedEventId,
        string expectedMessage,
        Times expectedTimes
    )
    {
        loggerMock.Verify(
            logger =>
                logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == expectedLogLevel),
                    It.Is<EventId>(eventId => eventId.Id == expectedEventId),
                    It.Is<It.IsAnyType>((value, _) => value.ToString()!.Contains(expectedMessage)),
                    It.Is<Exception>(e => e == null),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ),
            expectedTimes
        );
    }
}
