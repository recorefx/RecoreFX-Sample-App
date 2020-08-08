using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Recore;

using FileSync.Common;
using FileSync.Common.ApiModels;

namespace FileSync.Service.Controllers
{
    [ApiController]
    [Route("api/v1/listing")]
    public sealed class DirectoryV1Controller : ControllerBase
    {
        private readonly IFileStoreFactory fileStoreFactory;
        private readonly IFileHasher fileHasher;

        public DirectoryV1Controller(IFileStoreFactory fileStoreFactory, IFileHasher fileHasher)
        {
            this.fileStoreFactory = fileStoreFactory;
            this.fileHasher = fileHasher;
        }

        [HttpGet]
        public IEnumerable<Either<FileSyncDirectory, FileSyncFile>> GetListing([FromQuery] string path = ".")
        {
            // Assume that `path` uses forward slashes
            var forwardSlashPath = new ForwardSlashFilepath(path);
            var systemPath = forwardSlashPath.ToFilepath();
            var fileStore = fileStoreFactory.Create(systemPath);
            foreach (var directoryInfo in fileStore.GetDirectories())
            {
                yield return FileSyncDirectory.FromDirectoryInfo(
                    directoryInfo,
                    parentDirectory: systemPath,
                    listingEndpoint: new RelativeUri($"api/v1/listing"));
            }

            foreach (var fileInfo in fileStore.GetFiles())
            {
                yield return FileSyncFile.FromFileInfo(
                    fileInfo,
                    parentDirectory: systemPath,
                    Optional.Of(fileHasher),
                    contentEndpoint: new RelativeUri("api/v1/content"));
            }
        }
    }
}