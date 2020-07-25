﻿using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

using ApiModels = FileSync.Common.ApiModels;

namespace FileSync.Client
{
    public sealed class FileServiceHttpClient : IFileServiceHttpClient
    {
        private readonly HttpClient httpClient;
        private static readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public FileServiceHttpClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<IEnumerable<ApiModels.File>> GetAllFileInfoAsync()
        {
            var body = await httpClient.GetStreamAsync("api/v1/files");
            return await JsonSerializer.DeserializeAsync<IEnumerable<ApiModels.File>>(body, jsonOptions);
        }

        public async Task<Stream> GetFileContentAsync(ApiModels.File file)
        {
            return await httpClient.GetStreamAsync(file.Links["content"].Href);
        }

        public Task<ApiModels.File> PutFileContentAsync(Stream content)
        {
            // TODO
            throw new System.NotImplementedException();
        }
    }
}