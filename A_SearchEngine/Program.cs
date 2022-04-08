using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace A_SearchEngine
{

    class Document
    {
        private static int _currentId;
        private Dictionary<string, int> _uniqueWordsCount { get; set; }
        private Dictionary<string, int> _relevanceValueHistory = new Dictionary<string, int>();

        public string Text { get; }
        public int Id { get; }

        public Document(string text)
        {
            Id = ++_currentId;
            Text = text;
            _uniqueWordsCount = new Dictionary<string, int>();
            foreach (var item in text.Split(' '))
            {
                if (_uniqueWordsCount.ContainsKey(item))
                {
                    _uniqueWordsCount[item]++;
                }
                else
                {
                    _uniqueWordsCount.Add(item, 1);
                }
            }
        }

        public int CalculateRelevance(string request)
        {
            if (_relevanceValueHistory.ContainsKey(request))
            {
                return _relevanceValueHistory[request];
            }

            var uniqueWords = new HashSet<string>(request.Split(' '));
            int relevance = _uniqueWordsCount.Where(s => uniqueWords.Contains(s.Key)).Select(x => x.Value).Sum();

            _relevanceValueHistory[request] = relevance;
            return relevance;
        }
    }

    public class Solution
    {
        private static TextReader _reader;
        private static TextWriter _writer;

        public static void Main(string[] args)
        {
            InitialiseStreams();

            var Documents = new List<Document>();

            var n = ReadInt();
            for (int i = 0; i < n; i++)
            {
                Documents.Add(new Document(_reader.ReadLine()));
            }


            var m = ReadInt();
            for (int i = 0; i < m; i++)
            {
                var request = _reader.ReadLine();
                var result = Documents.Where(d => d.CalculateRelevance(request) != 0)
                    .OrderByDescending(d => d.CalculateRelevance(request)).ToList();
                var total = Math.Min(result.Count, 5);
                for (int j = 0; j < total && result[j].CalculateRelevance(request) != 0; j++)
                {
                    _writer.Write($"{result[j].Id} ");
                }
                _writer.WriteLine();
            }

            CloseStreams();
        }

        private static void CloseStreams()
        {
            _reader.Close();
            _writer.Close();
        }

        private static void InitialiseStreams()
        {
            _reader = new StreamReader(Console.OpenStandardInput());
            _writer = new StreamWriter(Console.OpenStandardOutput());
        }

        private static int ReadInt()
        {
            return int.Parse(_reader.ReadLine());
        }
    }
}
