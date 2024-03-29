﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Recore;
using Recore.Text.Json.Serialization.Converters;

using FileSync.Common.ApiModels;
using FileSync.Common.Filesystem;

namespace FileSync.Client
{
    using DirectoryListing = Either<FileSyncDirectory, FileSyncFile>;

    sealed class FileServiceHttpClient : IFileServiceApi
    {
        private readonly HttpClient httpClient;
        private readonly JsonSerializerOptions jsonOptions;

        public FileServiceHttpClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;

            jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            // FileSyncDirectory and FileSyncFile are both POCOs
            jsonOptions.Converters.Add(
                new OverrideEitherConverter<FileSyncDirectory, FileSyncFile>(
                    deserializeAsLeft: json => json.TryGetProperty("listingUrl", out JsonElement _)));
        }

        public async Task<IEnumerable<DirectoryListing>> GetDirectoryListingAsync(RelativeUri? listingUri)
        {
            listingUri ??= new RelativeUri("api/v1/listing");
            var result = await httpClient.GetFromJsonAsync<IEnumerable<DirectoryListing>>(listingUri, jsonOptions);
            return result ?? Enumerable.Empty<DirectoryListing>();
        }

        public async Task<Stream> GetFileContentAsync(FileSyncFile file)
            => await httpClient.GetStreamAsync(file.ContentUrl ?? throw new ArgumentNullException(nameof(file)));

        public async Task PutFileContentAsync(ForwardSlashFilepath path, Stream content)
        {
            var response = await httpClient.PutAsync($"api/v1/content?path={path}", new StreamContent(content));
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(response.ReasonPhrase);
            }
        }
    }
}
