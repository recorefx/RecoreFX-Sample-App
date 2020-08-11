using System.IO;
using System.Linq;
using Recore;
using Xunit;

using FileSync.Common.ApiModels;
using FileSync.Service.Controllers;
using FileSync.Tests.SharedMocks;

namespace FileSync.Service.Tests
{
    using DirectoryListing = Either<FileSyncDirectory, FileSyncFile>;

    public class DirectoryControllerTests
    {
        [Fact]
        public void GetListingRootDirectory()
        {
            var fileStore = FileStoreMock.Mock(
                new[]
                {
                    new DirectoryInfo("directory")
                },
                new[]
                {
                    new FileInfo("hello.txt"),
                    new FileInfo("world.txt")
                });

            var controller = new DirectoryV1Controller(
                FileStoreMock.MockFactory(fileStore).Object,
                FileHasherMock.Mock().Object);

            var actual = controller.GetListing().ToArray();

            var expected = new DirectoryListing[]
            {
                new FileSyncDirectory
                {
                    RelativePath = new ForwardSlashFilepath("./directory"),
                    ListingUrl = "api/v1/listing?path=./directory"
                },
                new FileSyncFile
                {
                    RelativePath = new ForwardSlashFilepath("./hello.txt"),
                    LastWriteTimeUtc = FileStoreMock.DefaultFileTimestamp,
                    Sha1 = FileHasherMock.EmptySha1Hash,
                    ContentUrl = "api/v1/content?path=./hello.txt"
                },
                new FileSyncFile
                {
                    RelativePath = new ForwardSlashFilepath("./world.txt"),
                    LastWriteTimeUtc = FileStoreMock.DefaultFileTimestamp,
                    Sha1 = FileHasherMock.EmptySha1Hash,
                    ContentUrl = "api/v1/content?path=./world.txt"
                }
            };

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetListingSubdirectory()
        {
            var fileStore = FileStoreMock.Mock(
                new[]
                {
                    new DirectoryInfo("directory")
                },
                new[]
                {
                    new FileInfo("hello.txt"),
                    new FileInfo("world.txt")
                });

            var controller = new DirectoryV1Controller(
                FileStoreMock.MockFactory(fileStore).Object,
                FileHasherMock.Mock().Object);

            var actual = controller.GetListing("./subdirectory").ToArray();

            var expected = new DirectoryListing[]
            {
                new FileSyncDirectory
                {
                    RelativePath = new ForwardSlashFilepath("./subdirectory/directory"),
                    ListingUrl = "api/v1/listing?path=./subdirectory/directory"
                },
                new FileSyncFile
                {
                    RelativePath = new ForwardSlashFilepath("./subdirectory/hello.txt"),
                    LastWriteTimeUtc = FileStoreMock.DefaultFileTimestamp,
                    Sha1 = FileHasherMock.EmptySha1Hash,
                    ContentUrl = "api/v1/content?path=./subdirectory/hello.txt"
                },
                new FileSyncFile
                {
                    RelativePath = new ForwardSlashFilepath("./subdirectory/world.txt"),
                    LastWriteTimeUtc = FileStoreMock.DefaultFileTimestamp,
                    Sha1 = FileHasherMock.EmptySha1Hash,
                    ContentUrl = "api/v1/content?path=./subdirectory/world.txt"
                }
            };

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetListingEmptyDirectory()
        {
            var fileStore = FileStoreMock.Mock(
                Enumerable.Empty<DirectoryInfo>(),
                Enumerable.Empty<FileInfo>());

            var controller = new DirectoryV1Controller(
                FileStoreMock.MockFactory(fileStore).Object,
                FileHasherMock.Mock().Object);

            var actual = controller.GetListing("./empty-directory");
            var expected = Enumerable.Empty<DirectoryListing>();

            Assert.Equal(expected, actual);
        }
    }
}
