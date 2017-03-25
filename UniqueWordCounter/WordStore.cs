using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace UniqueWordCounter
{
    /// <summary>
    /// Class responsible for holding words and their occurrences
    /// </summary>
    public class WordStore
    {
        private ConcurrentDictionary<string, Word> wordDict = new ConcurrentDictionary<string, Word>();

        /// <summary>
        /// Calls the AddWord(string content, int lineNumber, int linePos, int count) with default values of lineNumber=0, linePos=0, count=1
        /// </summary>
        /// <param name="content">word itself</param>
        public void AddWord(string content)
        {
            AddWord(content, 0, 0, 1);
        }

        /// <summary>
        /// Intantiates and adds/updates the word in the store
        /// If the word is already in store it only increments the occurrence count
        /// If the word is not yet in store it adds the word
        /// </summary>
        /// <param name="content">word itself</param>
        /// <param name="lineNumber">line number where the word seen</param>
        /// <param name="linePos">line position where the word seen</param>
        /// <param name="count">count of words of this content</param>
        public void AddWord(string content, int lineNumber, int linePos, int count)
        {
            wordDict.AddOrUpdate(content.ToLowerInvariant(), new Word(content, lineNumber, linePos, count), (key, oldValue) => 
            {
                var newValue = new Word(oldValue.Content, oldValue.FirstLineOfOccurrence, oldValue.FirstLinePosOfOccurrence, oldValue.OccurrenceCount + count);

                if ((lineNumber < oldValue.FirstLineOfOccurrence) ||
                    (lineNumber == oldValue.FirstLineOfOccurrence && linePos < oldValue.FirstLinePosOfOccurrence))
                {
                    newValue.Content = content;
                    newValue.FirstLinePosOfOccurrence = linePos;
                    
                    if (lineNumber < oldValue.FirstLineOfOccurrence)
                        newValue.FirstLineOfOccurrence = lineNumber;
                }

                return newValue;
            });
        }

        /// <summary>
        /// Get words and their occurrences
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, Word> GetWords()
        {
            return wordDict.ToDictionary(x => x.Key, x => x.Value);
        }

        /// <summary>
        /// Report words and their occurrences as text
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            
            var pairs = wordDict.OrderBy(x => x.Value.FirstLineOfOccurrence).ThenBy(x => x.Value.FirstLinePosOfOccurrence);
            foreach (var pair in pairs)
            {
                sb.AppendLine($"{pair.Value.OccurrenceCount}: {pair.Value.Content}");
            }

            return sb.ToString();
        }
    }
}
