using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace SplitGzipFile
{
    class Program
    {
        private const string InputFolderName = "input";
        private const string InputGzipFile = @"C:\bin3";
        private const int LinesToSplitFiles = 1000000;

        static void Main(string[] args)
        {
            Console.Write("Processing split... ");
            using var progress = new ProgressBar();
            ProcessSplit(progress);

            Console.WriteLine("Done.");
        }

        static void ProcessSplit(ProgressBar progressBar)
        {
            var memoryStream = new MemoryStream();
            var newData = new StringBuilder();
            var totalLine = 0;
            
            if (!Directory.Exists(InputFolderName))
            {
                Directory.CreateDirectory(InputFolderName);
            }
            
            using var input = File.OpenRead(InputGzipFile);
            
            input.CopyTo(memoryStream);
            
            if (memoryStream.CanSeek)
            {   
                memoryStream.Position = 0;
            }
            
            using var gzip = new GZipStream(memoryStream, CompressionMode.Decompress,true);
            using var reader = new StreamReader(gzip, Encoding.UTF8);
            while (reader.ReadLine() is { } line)
            {
                newData.AppendLine(line);
                
                progressBar.Report((double) totalLine / LinesToSplitFiles);

                if (totalLine == LinesToSplitFiles || (reader.EndOfStream && totalLine < LinesToSplitFiles))
                {
                    using var fs = File.Create($"{InputFolderName}\\{DateTime.UtcNow:ddMMyyyyss}");
                    using var zipStream = new GZipStream(fs, CompressionMode.Compress, false);
                    zipStream.Write(Encoding.ASCII.GetBytes(newData.ToString()), 0, newData.Length);

                    
                    totalLine = 0;
                    newData.Clear();
                }
                
                totalLine += 1;
            }
        }
    }
}