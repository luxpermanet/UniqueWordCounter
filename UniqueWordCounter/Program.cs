﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UniqueWordCounter
{
    class Program
    {
        private static Regex KeyValRegex = new Regex("/(?<key>\\S*?):(?<val>\"{1}[\\s\\S]*\"{1}|\\S*)", RegexOptions.Compiled);

        static void Main(string[] args)
        {
            var filePath = string.Empty;
            var encoding = Encoding.UTF8;
            var fileReadParallelism = 1;
            var lineProcessParallelism = 1;

            // arg is in /key:value or /key:"value" form
            foreach (var arg in args)
            {
                var m = KeyValRegex.Match(arg);
                var key = m.Groups["key"].Value;
                var val = m.Groups["val"].Value;

                if (!string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(val))
                {
                    switch (key.ToUpperInvariant())
                    {
                        case "FILEPATH": filePath = val; break;
                        case "ENCODING": encoding = GetEncoding(val); break;
                        case "FILEREADPARALLELISM": fileReadParallelism = int.Parse(val); break;
                        case "LINEPROCESSPARALLELISM": lineProcessParallelism = int.Parse(val); break;
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(filePath))
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("UniqueWordCounter /filePath:\"FilePath\" /encoding:utf-8 /fileReadParallelism:5 /lineProcessParallelism:10");
                Console.WriteLine("Parameters:");
                Console.WriteLine("filePath: file path of the test file");
                Console.WriteLine("encoding: encoding of the file provided");
                Console.WriteLine("fileReadParallelism: number of max threads to read the file");
                Console.WriteLine("lineProcessParallelism: number of max threads to process the read lines");

                return;
            }

            var wordCounter = new WordCounter(filePath, encoding, fileReadParallelism, lineProcessParallelism);
     
            try
            {
                wordCounter.RunVerbose();
                //wordCounter.Run();
            }
            catch (AggregateException ae)
            {
                foreach (var ex in ae.InnerExceptions)
                    Console.WriteLine(ex.Message);

                throw;
            }

            Console.Write(wordCounter.ReportText());
            Console.ReadKey();
        }

        private static Encoding GetEncoding(string encoding)
        {
            var enc = Encoding.GetEncoding(encoding);
            if (enc == null)
            {
                return Encoding.UTF8;
            }
            return enc;
        }
    }
}