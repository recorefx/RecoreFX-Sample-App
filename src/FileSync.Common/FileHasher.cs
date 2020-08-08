﻿using System;
using System.IO;
using System.Security.Cryptography;
using Recore.Security.Cryptography;

namespace FileSync.Common
{
    public sealed class FileHasher : IFileHasher
    {
        public Ciphertext<SHA1> HashFile(Filepath filepath)
        {
            var fileContents = File.ReadAllText(filepath.Value);
            return Ciphertext.SHA1(fileContents, salt: Array.Empty<byte>());
        }
    }
}