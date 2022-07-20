using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringPath
{
    class Program
    {
        private const string InputFolderName = @"C:\Users\ClaudioCésarRuelaPar\RiderProjects\SplitGzipFile\SplitGzipFile\bin\Debug\net5.0\input";
        private const string ProcessedFolderName = "processed";
        
        static void Main(string[] args)
        {
            MonitorDirectory();
            Console.ReadKey();
        }
        
        private static void MonitorDirectory()
        {
            var fileSystemWatcher = new FileSystemWatcher();

            fileSystemWatcher.Path = InputFolderName;
            
            fileSystemWatcher.Created += FileSystemWatcher_Created;
            fileSystemWatcher.Renamed += FileSystemWatcher_Renamed;
            fileSystemWatcher.Deleted += FileSystemWatcher_Deleted;

            fileSystemWatcher.EnableRaisingEvents = true;
        }

        private static void FileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            var memoryStream = FileLocked(e.Name);
            
            using var gzip = new GZipStream(memoryStream, CompressionMode.Decompress,true);
            using var reader = new StreamReader(gzip, Encoding.UTF8);
            while (reader.ReadLine() is { } line)
            {
                
            }
            
            if (!Directory.Exists(ProcessedFolderName))
            {
                Directory.CreateDirectory(ProcessedFolderName);
            }
            
            // Moving the file file.txt to location C:\gfg.txt
            Console.WriteLine("Moved");
            File.Move( $@"{InputFolderName}\{e.Name}", $@"{ProcessedFolderName}\{e.Name}");
            
            
            Console.WriteLine("File created: {0}", e.Name);
        }

        private static void FileSystemWatcher_Renamed(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("File renamed: {0}", e.Name);
        }

        private static void FileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("File deleted: {0}", e.Name);
        }
        
        private static MemoryStream FileLocked(string fileName)
        {
            var memoryStream = new MemoryStream();
            
            var locked = true;
            do
            {
                try
                {
                    using var input = File.OpenRead($@"{InputFolderName}\{fileName}");
                    input.CopyTo(memoryStream);
            
                    if (memoryStream.CanSeek)
                    {   
                        memoryStream.Position = 0;
                    }
                    locked = false;
                    
                }
                catch (IOException)
                {
                    //the file is unavailable because it is:
                    //still being written to
                    //or being processed by another thread
                    //or does not exist (has already been processed)
                    locked = true;
                }
            } while (locked);
            
            return memoryStream;
        }
    }
}