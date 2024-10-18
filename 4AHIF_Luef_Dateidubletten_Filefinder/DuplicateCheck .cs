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
                    var fileName = Path.GetFileNameWithoutExtension(filePath);
                    var processedName = PreprocessFileName(fileName);
                    var fileInfo = new FileInfo(filePath);
                    return mode switch
                    {
                        CollectMode.SizeOnly => new
                        {
                            Size = fileInfo.Length,
                            Name = (string)null,  
                            FirstThreeBytes = (string)null  
                        },

                        CollectMode.SizeAndName => new
                        {
    
                            Size = fileInfo.Length,
                            Name = processedName,  
                            FirstThreeBytes = (string)null  
                        },

                        CollectMode.SizeAndNameAndFirstThree => new
                        {
                            Size = fileInfo.Length,
                            Name = processedName,  
                            FirstThreeBytes = GetFirstThreeBytes(filePath)  
                        },

                        _ => throw new ArgumentOutOfRangeException(nameof(mode), "Unknown CollectMode")  // Handle invalid mode
                    };
        })
                .Where(group => group.Count() > 1)
                .ToList();
            
            foreach (var group in fileGroups)
            {
               // Console.WriteLine(fileGroups.Count());
                var duplicate = new Duplicate();
                duplicate.FilePaths.AddRange(group);

                yield return duplicate;
            }

        }

        private string PreprocessFileName(string fileName)
        {
            // Remove common duplicate markers like "Kopie" or "copy" (case insensitive)
            fileName = fileName.Replace("Kopie", "", StringComparison.OrdinalIgnoreCase)
                               .Replace("copy", "", StringComparison.OrdinalIgnoreCase)
                               .Trim();

            
            if (fileName.Length > 3)
            {
                
                return fileName.Substring(0, 3);
            }

            return fileName;  // If the file name is shorter than 3 characters, return it as-is
        }

        
        public void OutputPotentialDuplicates(string path, CollectMode mode)
        {
            var potentialDuplicates = GetPotentialDuplicates(path, mode);

            
            foreach (var group in potentialDuplicates)
            {
                Console.WriteLine($"Potential duplicate group:{group}" );
                
                
            }
        }   
        
        private string GetFirstThreeBytes(string filePath)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                var buffer = new byte[3];
                fileStream.Read(buffer, 0, buffer.Length);
                return BitConverter.ToString(buffer);
            }
        }

        private IEnumerable<IDuplicate> VerifyDuplicatesWithMD5(IEnumerable<Duplicate> potentialDuplicates)
        {
            foreach (var duplicate in potentialDuplicates)
            {
                var fileGroupsByHash = duplicate.FilePaths
                    .AsParallel()
                    .GroupBy(filePath => ComputeMD5(filePath))
                    .Where(group => group.Count() > 1);  

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
                byte[] hash = md5.ComputeHash(stream);
                //Console.WriteLine(hash.Length);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                
            }
        }

    }

}
