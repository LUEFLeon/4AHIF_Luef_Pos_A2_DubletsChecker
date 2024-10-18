namespace DuplicateFinderAppConsole

{
    using _4AHIF_Luef_Dateidubletten_Filefinder;
    using System.Diagnostics;
    using System.Security;
    

    class Program
    {
        static void Main()
        {
            

            IDuplicateCheck duplicateChecker = new DuplicateCheck();

            string path = "Testfiles";

           
            var duplicates = duplicateChecker.Collect(path, CollectMode.SizeAndNameAndFirstThree);
            Console.WriteLine($"Duplicate files: {duplicates.Count()}");
            foreach (var duplicate in duplicates)
            {
                FileInfo fileInfo = new FileInfo(duplicate.GetType().Name);
                Console.WriteLine($"Duplicate.Filepath:{ fileInfo.Directory}");
                /*
                foreach (var file in duplicate.FilePaths)
                {
                   Console.WriteLine($"File: {file}");    
                    FileInfo fileInfo1 = new FileInfo(file);
                    Console.WriteLine($"path: {fileInfo1}");
                }
               */
            }
            /*
            string filePath = @"C:\Users\miq45\Desktop\Schuljahr\4AHIF\POS\wawa\";

            try
            {
                string[] files = Directory.GetFiles(filePath);
                
                foreach (string file in files)
                {
                    try
                    {
                        FileInfo fileInfo = new FileInfo(file);
                        byte[] firstThreeBytes = new byte[3];  // Array to store the first 3 bytes

                        
                        using (FileStream fs = new FileStream(filePath + fileInfo.Name, FileMode.Open, FileAccess.Read))
                        {
                            fs.Read(firstThreeBytes, 0, 3);  // Read 3 bytes starting at position 0
                        }

                        // Output the first 3 bytes to the console
                        Console.WriteLine("First 3 bytes of the file:");

                        foreach (var b in firstThreeBytes)
                        {
                            Console.WriteLine(b);  // Print each byte value
                        }

                       
                    }
                    catch (UnauthorizedAccessException)
                    {
                        Console.WriteLine($"Access denied to file: {file}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing file '{file}': {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error accessing folder: {ex.Message}");
            }
            */
        }
    
}
}
