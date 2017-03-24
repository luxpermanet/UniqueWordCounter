using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UniqueWordCounter
{
    public class WordCounter
    {
        private static readonly Regex WordRegex = new Regex(@"\w+", RegexOptions.Compiled);
        
        private WordStore wordStore = new WordStore();
        private string filePath = null;
        private Encoding fileEncoding = Encoding.UTF8;
        private Stopwatch chrono = new Stopwatch();
        private int fileReadParallelism = 1;
        private ParallelOptions parallelOptions = null;
        private long executionMillis = 0;

        public WordCounter(string filePath, Encoding fileEncoding, int fileReadParallelism, int lineProcessParallelism)
        {
            this.filePath = filePath;
            this.fileEncoding = fileEncoding;
            this.fileReadParallelism = fileReadParallelism;
            parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = lineProcessParallelism };
        }

        public void Run()
        {
            chrono.Start();
            Parallel.ForEach(File.ReadLines(filePath, fileEncoding).AsParallel().WithDegreeOfParallelism(fileReadParallelism), parallelOptions, line =>
            {
                // Slower
                ////WordRegex
                ////    .Matches(ignoreCase ? line.ToLowerInvariant() : line)
                ////    .Cast<Match>()
                ////    .GroupBy(x => x.Value)
                ////    .Select(x => new Word(x.Key, x.Count()))
                ////    .ToList()
                ////    .ForEach(x => wordStore.AddWord(x));

                foreach (Match m in WordRegex.Matches(line))
                {
                    wordStore.AddWord(m.Value);
                }
            });
            chrono.Stop();
            executionMillis = chrono.ElapsedMilliseconds;
        }

        public void RunVerbose()
        {
            chrono.Start();
            Parallel.ForEach(GenerateLines(filePath, fileEncoding).AsParallel().WithDegreeOfParallelism(fileReadParallelism), parallelOptions, pair =>
            {
                int linePos = 1;
                foreach (Match m in WordRegex.Matches(pair.Item1))
                {
                    wordStore.AddWord(m.Value, pair.Item2, linePos++, 1);
                }
            });
            chrono.Stop();
            executionMillis = chrono.ElapsedMilliseconds;
        }

        private IEnumerable<Tuple<string, int>> GenerateLines(string filePath, Encoding fileEncoding)
        {
            string line = null;
            int lineNumber = 1;
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (var sr = new StreamReader(fs, fileEncoding))
                while ((line = sr.ReadLine()) != null)
                    yield return Tuple.Create(line, lineNumber++);
        }

        public Dictionary<string, Word> ReportRaw()
        {
            return wordStore.GetWords();
        }

        public string ReportText()
        {
            StringBuilder report = new StringBuilder();
            report.AppendLine("Word occurrences");
            report.AppendLine();
            report.AppendLine(wordStore.ToString());
            report.AppendLine($"Elapsed seconds: {executionMillis / 1000} ms.");

            return report.ToString();
        }
    }
}
