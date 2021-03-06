﻿using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Recore;

using FileSync.Common.ApiModels;
using FileSync.Common.Filesystem;

namespace FileSync.Client
{
    interface IFileServiceApi
    {
        Task<IEnumerable<Either<FileSyncDirectory, FileSyncFile>>> GetDirectoryListingAsync(RelativeUri? listingUri);

        Task<Stream> GetFileContentAsync(FileSyncFile file);

        Task PutFileContentAsync(ForwardSlashFilepath path, Stream content);
    }
}
