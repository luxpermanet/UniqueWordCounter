using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UniqueWordCounter
{
    /// <summary>
    /// Class responsible for counting words in a text file
    /// </summary>
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

        /// <summary>
        /// Instantiates class with the parameters
        /// </summary>
        /// <param name="filePath">file path to be tested</param>
        /// <param name="fileEncoding">encoding of the file</param>
        /// <param name="fileReadParallelism">number of max threads to read the file</param>
        /// <param name="lineProcessParallelism">number of max threads to process the read lines</param>
        public WordCounter(string filePath, Encoding fileEncoding, int fileReadParallelism, int lineProcessParallelism)
        {
            this.filePath = filePath;
            this.fileEncoding = fileEncoding;
            this.fileReadParallelism = fileReadParallelism;
            parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = lineProcessParallelism };
        }

        /// <summary>
        /// Runs the word counter by discarding the word positions (line number, line position)
        /// </summary>
        public void Run()
        {
            chrono.Start();

            var exceptions = new ConcurrentQueue<Exception>();
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

                try
                {
                    foreach (Match m in WordRegex.Matches(line))
                    {
                        wordStore.AddWord(m.Value);
                    }
                }
                catch (Exception e) { exceptions.Enqueue(e); }
            });

            if (exceptions.Count > 0)
                throw new AggregateException(exceptions);

            chrono.Stop();
            executionMillis = chrono.ElapsedMilliseconds;
        }

        /// <summary>
        /// Runs the word counter by taking word positions into account (line number, line position)
        /// </summary>
        public void RunVerbose()
        {
            chrono.Start();

            var exceptions = new ConcurrentQueue<Exception>();
            Parallel.ForEach(GenerateLines(filePath, fileEncoding).AsParallel().WithDegreeOfParallelism(fileReadParallelism), parallelOptions, pair =>
            {
                try
                {
                    int linePos = 1;
                    foreach (Match m in WordRegex.Matches(pair.Item1))
                    {
                        wordStore.AddWord(m.Value, pair.Item2, linePos++, 1);
                    }
                }
                catch (Exception e) { exceptions.Enqueue(e); }
            });

            if (exceptions.Count > 0)
                throw new AggregateException(exceptions);

            chrono.Stop();
            executionMillis = chrono.ElapsedMilliseconds;
        }

        /// <summary>
        /// Reads one line from file at a time
        /// </summary>
        /// <param name="filePath">file path to read lines</param>
        /// <param name="fileEncoding">encoding of the file provided</param>
        /// <returns></returns>
        private IEnumerable<Tuple<string, int>> GenerateLines(string filePath, Encoding fileEncoding)
        {
            string line = null;
            int lineNumber = 1;
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (var sr = new StreamReader(fs, fileEncoding))
                while ((line = sr.ReadLine()) != null)
                    yield return Tuple.Create(line, lineNumber++);
        }

        /// <summary>
        /// Returns words and their occurrences as dictionary
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, Word> ReportRaw()
        {
            return wordStore.GetWords();
        }

        /// <summary>
        /// Reports words and their occurrences as text
        /// </summary>
        /// <returns></returns>
        public string ReportText()
        {
            StringBuilder report = new StringBuilder();
            report.AppendLine("Word occurrences");
            report.AppendLine();
            report.AppendLine(wordStore.ToString());
            report.AppendLine($"Elapsed seconds: {executionMillis / 1000} seconds");

            return report.ToString();
        }
    }
}
