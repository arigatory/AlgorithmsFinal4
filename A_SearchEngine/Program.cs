// https://contest.yandex.ru/contest/24414/run-report/67202455/

/* 
 * -- ПРИНЦИП РАБОТЫ --
 * Используем ассоциативный массив Dictionary для создание поисковых индексов. В каждом документе находим количество уникальных слов
 * и сохраняем {слово -> (№ документа : количество )} 
 * При обработке запроса берем множество уникальных слов из запроса и суммируем по всем документам из индекса. Тем самым мы просматриваем
 * не все n, а только те, в которых точно слово встречается.
 * Сортируем по убыванию суммы и берем первые 5, если документов больше 5. Иначе сколько есть.
 * 
 * 
 * -- ДОКАЗАТЕЛЬСТВО КОРРЕКТНОСТИ --
 * Мы заранее записали какое слово в каком документе и сколько раз встречается, а само слово при этом является ключом. 
 * Поэтому при очередном запросе мы получим весь список документов.
 * 
 * -- ВРЕМЕННАЯ СЛОЖНОСТЬ --
 * Основная сложность идет на формирование индекса. Но он создается единожды.
 * Доступ к информации об одном слове происходит за O(1).
 * Если запрос длинный, длина m, то сложность будет 0(m), так как для каждого слова мы будет делать O(1) операций.
 * 
 * 
 * -- ПРОСТРАНСТВЕННАЯ СЛОЖНОСТЬ --
 * Мы храним весь индекс. Если слов n, то O(n).
 * 
 * 
 */


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace A_SearchEngine
{
    class SearchEngine
    {
        private List<Document> _documents = new();
        private Dictionary<string, Dictionary<int, int>> _wordDocumentNumberCount = new();

        public void AddDocument(Document document)
        {
            _documents.Add(document);
            Dictionary<string, int> uniqueWordsCount = document.Text.Split(' ').GroupBy(s => s).ToDictionary(grp => grp.Key, grp => grp.Count());
            foreach (var word in uniqueWordsCount)
            {
                if (_wordDocumentNumberCount.ContainsKey(word.Key))
                {
                    _wordDocumentNumberCount[word.Key][document.Id] = word.Value;
                }
                else
                {
                    _wordDocumentNumberCount[word.Key] = new Dictionary<int, int>();
                    _wordDocumentNumberCount[word.Key].Add(document.Id, word.Value);
                }
            }
        }

        public List<int> GetBestIdsByRequest(string request)
        {
            var words = new HashSet<string>(request.Split(' '));

            Dictionary<int, int> documentIdOccurenceCount = new Dictionary<int, int>();

            foreach (var word in words)
            {
                if (!_wordDocumentNumberCount.ContainsKey(word))
                {
                    continue;
                }
                Dictionary<int, int> documentIdTotalWords = _wordDocumentNumberCount[word];
                foreach (var idCount in documentIdTotalWords)
                {
                    if (documentIdOccurenceCount.ContainsKey(idCount.Key))
                    {
                        documentIdOccurenceCount[idCount.Key] += idCount.Value;
                    }
                    else
                    {
                        documentIdOccurenceCount[idCount.Key] = idCount.Value;
                    }
                }
            }


            var result = documentIdOccurenceCount
                   .OrderByDescending(d => d.Value)
                   .ThenBy(d => d.Key)
                   .Select(d => d.Key)
                   .Take(Math.Min(5, documentIdOccurenceCount.Count))
                   .ToList();

            return result;
        }

    }

    class Document
    {
        private static int _currentId;

        public int Id { get; }
        public string Text { get; set; }
        public Document(string text)
        {
            Id = ++_currentId;
            Text = text;
        }
    }

    public class Solution
    {
        private static TextReader _reader;
        private static TextWriter _writer;

        public static void Main(string[] args)
        {
            InitialiseStreams();
            SearchEngine searchEngine = new SearchEngine();

            var n = ReadInt();
            for (int i = 0; i < n; i++)
            {
                searchEngine.AddDocument(new Document(_reader.ReadLine()));
            }

            var m = ReadInt();
            for (int i = 0; i < m; i++)
            {
                var request = _reader.ReadLine();
                var result = searchEngine.GetBestIdsByRequest(request);

                _writer.WriteLine(string.Join(" ", result));
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
