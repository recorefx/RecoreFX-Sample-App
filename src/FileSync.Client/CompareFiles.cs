﻿using System.Collections.Generic;
using System.Linq;

using FileSync.Common;
using FileSync.Common.ApiModels;
using FileSync.Common.Filesystem;

namespace FileSync.Client
{
    sealed class CompareFiles
    {
        private readonly Dictionary<ForwardSlashFilepath, FileSyncFile> clientFiles;
        private readonly Dictionary<ForwardSlashFilepath, FileSyncFile> serviceFiles;
        private readonly IFileHasher fileHasher;

        public CompareFiles(IEnumerable<FileSyncFile> clientFiles, IEnumerable<FileSyncFile> serviceFiles, IFileHasher fileHasher)
        {
            this.clientFiles = clientFiles.ToDictionary(x => x.RelativePath);
            this.serviceFiles = serviceFiles.ToDictionary(x => x.RelativePath);
            this.fileHasher = fileHasher;
        }

        public IEnumerable<FileSyncFile> FilesToDownload()
            => OnlyOnService().Concat(
                Conflicts()
                    .Where(conflict => conflict.ChosenVersion == ChosenVersion.Service)
                    .Select(conflict => conflict.ServiceFile));

        public IEnumerable<FileSyncFile> FilesToUpload()
            => OnlyOnClient().Concat(
                Conflicts()
                    .Where(conflict => conflict.ChosenVersion == ChosenVersion.Client)
                    .Select(conflict => conflict.ClientFile));

        public IEnumerable<Conflict> Conflicts()
            => clientFiles.Keys
                .Intersect(serviceFiles.Keys)
                .Where(VersionsAreDifferent)
                .Select(key => new Conflict(
                    clientFile: clientFiles[key],
                    serviceFile: serviceFiles[key]));

        private IEnumerable<FileSyncFile> OnlyOnService()
            => serviceFiles.Keys
                .Except(clientFiles.Keys)
                .Select(key => serviceFiles[key]);

        private IEnumerable<FileSyncFile> OnlyOnClient()
            => clientFiles.Keys
                .Except(serviceFiles.Keys)
                .Select(key => clientFiles[key]);

        private bool VersionsAreDifferent(ForwardSlashFilepath path)
        {
            var clientFile = clientFiles[path];
            var serviceFile = serviceFiles[path];

            // In practice, clientFile's SHA should always be empty,
            // but it doesn't hurt to check.
            if (clientFile.Sha1 is null)
            {
                var systemPath = path.ToSystemFilepath();
                clientFile = clientFile with { Sha1 = fileHasher.HashFile(systemPath).ToString() };
            }

            return serviceFile.Sha1 != clientFile.Sha1;
        }
    }
}
