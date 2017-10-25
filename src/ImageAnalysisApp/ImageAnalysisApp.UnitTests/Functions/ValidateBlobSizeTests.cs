using System.IO;
using ImageAnalysisApp.Functions;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ImageAnalysisApp.UnitTests.Functions
{
    public class ValidateBlobSizeTests
    {
        [Fact]
        public void UnitOfWork_StateUnderTest_ExpectedBehaviour()
        {
            // Arrange
            string blobName = "test";
            Stream blobStream = null;
            Mock<ICollector<string>> imagesTooLargeQueue = GetMockedQueue();
            Mock<ICollector<string>> imagesToProcessQueue = GetMockedQueue();
            Mock<ILogger> logger = GetMockedLogger();

            // Act
            ValidateBlobSize.Run(
                blobName,
                blobStream,
                imagesTooLargeQueue.Object,
                imagesToProcessQueue.Object,
                logger.Object);

            // Assert

        }

        private Mock<ICollector<string>> GetMockedQueue()
        {
            var mockQueue = new Mock<ICollector<string>>();

            return mockQueue;
        }

        private Mock<ILogger> GetMockedLogger()
        {
            var mockLogger = new Mock<ILogger>();

            return mockLogger;
        }
    }
}
