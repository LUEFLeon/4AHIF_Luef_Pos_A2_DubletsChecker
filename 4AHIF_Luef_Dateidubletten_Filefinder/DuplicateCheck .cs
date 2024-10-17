using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace _4AHIF_Luef_Dateidubletten_Filefinder
{
    public class DuplicateCheck : IDuplicateCheck
    {
        public IEnumerable<IDuplicate> Collect(string path)
        {
            return Collect(path, CollectMode.SizeOnly);
        }


        public IEnumerable<IDuplicate> Collect(string path, CollectMode mode)
        {
            var potentialDuplicates = GetPotentialDuplicates(path, mode);

            // Step 2: Refine potential duplicates by calculating and comparing MD5 hashes
            var verifiedDuplicates = VerifyDuplicatesWithMD5(potentialDuplicates);
            return verifiedDuplicates;
        }
        private IEnumerable<Duplicate> GetPotentialDuplicates(string path, CollectMode mode)
        {
            var fileGroups = Directory
                .EnumerateFiles(path, "*", SearchOption.AllDirectories)
                .AsParallel()
                .GroupBy(filePath => 
                {
                    var fileInfo = new FileInfo(filePath);
                    return mode switch
                    {
                        CollectMode.SizeOnly => new
                        {
                            Size = fileInfo.Length,
                            Name = (string)null,  
                            FirstThreeBytes = (byte[])null  
                        },

                        CollectMode.SizeAndName => new
                        {
                            Size = fileInfo.Length,
                            Name = Path.GetFileName(filePath),  
                            FirstThreeBytes = (byte[])null  
                        },

                        CollectMode.SizeAndNameAndFirstThree => new
                        {
                            Size = fileInfo.Length,
                            Name = Path.GetFileName(filePath),  
                            FirstThreeBytes = GetFirstThreeBytes(filePath)  
                        },

                        _ => throw new ArgumentOutOfRangeException(nameof(mode), "Unknown CollectMode")  // Handle invalid mode
                    };
        })
                .Where(group => group.Count() > 1);  // Only keep potential duplicates

            foreach (var group in fileGroups)
            {
                var duplicate = new Duplicate();
                duplicate.FilePaths.AddRange(group);
                yield return duplicate;
            }
        }

        private byte[] GetFirstThreeBytes(string filePath)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                var buffer = new byte[3];
                fileStream.Read(buffer, 0, 3);
                return buffer;
            }
        }

        private IEnumerable<IDuplicate> VerifyDuplicatesWithMD5(IEnumerable<Duplicate> potentialDuplicates)
        {
            foreach (var duplicate in potentialDuplicates)
            {
                var fileGroupsByHash = duplicate.FilePaths
                    .AsParallel()
                    .GroupBy(filePath => ComputeMD5(filePath))
                    .Where(group => group.Count() > 1);  // Only keep actual duplicates

                foreach (var group in fileGroupsByHash)
                {
                    var verifiedDuplicate = new Duplicate();
                    verifiedDuplicate.FilePaths.AddRange(group);
                    yield return verifiedDuplicate;
                }
            }
        }
        private string ComputeMD5(string filePath)
        {
            using (var md5 = MD5.Create())
            using (var stream = File.OpenRead(filePath))
            {
                var hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }

    }

}
