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
        public void GivenABlobIsSmallerThan4MB_WhenRunIsCalled_AMessageIsPushedToTheImagesToProcessQueue()
        {
            // Arrange
            const string blobName = "test";
            Stream blobStream = GetMemoryStream(size: 0);
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
            imagesToProcessQueue.Verify(
                queue => queue.Add(It.IsAny<string>()),
                Times.Once);

        }

        [Fact]
        public void GivenABlobIsLargerThan4MB_WhenRunIsCalled_AMessageIsPushedToTheImagesTooLargeQueue()
        {
            // Arrange
            const string blobName = "test";
            Stream blobStream = GetMemoryStream(size: 4194304);
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
            imagesTooLargeQueue.Verify(
                queue => queue.Add(It.IsAny<string>()),
                Times.Once);

        }

        private static Mock<ICollector<string>> GetMockedQueue()
        {
            var mockQueue = new Mock<ICollector<string>>();

            return mockQueue;
        }

        private static Mock<ILogger> GetMockedLogger()
        {
            var mockLogger = new Mock<ILogger>();

            return mockLogger;
        }

        private static MemoryStream GetMemoryStream(int size)
        {
            return new MemoryStream(new byte[size]);
        }
    }
}
