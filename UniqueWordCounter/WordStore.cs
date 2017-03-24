using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace UniqueWordCounter
{
    public class WordStore
    {
        private ConcurrentDictionary<string, Word> wordStore = new ConcurrentDictionary<string, Word>();

        public void AddWord(string content)
        {
            AddWord(content, 0, 0, 1);
        }

        public void AddWord(string content, int lineNumber, int linePos, int count)
        {
            wordStore.AddOrUpdate(content.ToLowerInvariant(), new Word(content, lineNumber, linePos, count), (key, oldValue) => 
            {
                var newValue = new Word(oldValue.Content, oldValue.FirstLineOfOccurrence, oldValue.FirstLinePosOfOccurrence, oldValue.OccurrenceCount + count);
                
                if (oldValue.FirstLineOfOccurrence > lineNumber)
                {
                    newValue.Content = content;
                    newValue.FirstLineOfOccurrence = lineNumber;
                    newValue.FirstLinePosOfOccurrence = linePos;
                }
                else if (oldValue.FirstLineOfOccurrence == lineNumber && oldValue.FirstLinePosOfOccurrence > linePos)
                {
                    newValue.Content = content;
                    newValue.FirstLinePosOfOccurrence = linePos;
                }
                return newValue;
            });
        }

        public Dictionary<string, Word> GetWords()
        {
            return wordStore.ToDictionary(x => x.Key, x => x.Value);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            
            var pairs = wordStore.OrderBy(x => x.Value.FirstLineOfOccurrence).ThenBy(x => x.Value.FirstLinePosOfOccurrence);
            foreach (var pair in pairs)
            {
                sb.AppendLine($"{pair.Value.OccurrenceCount}: {pair.Value.Content}");
            }

            return sb.ToString();
        }
    }
}
