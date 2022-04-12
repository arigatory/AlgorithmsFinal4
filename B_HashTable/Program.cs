// https://contest.yandex.ru/contest/24414/run-report/67215556/
// новая посылка: https://contest.yandex.ru/contest/24414/run-report/67286080/

/* 
 * -- ПРИНЦИП РАБОТЫ --
 * Заводим обычный массив _array, перед тем как к нему обратиться, подрезаем ключ с помощью побитового AND.
 * В массиве хранятся ссылки на односвязные списки Node. В них будем хранить значения при коллизиях.
 * 
 * 
 * -- ДОКАЗАТЕЛЬСТВО КОРРЕКТНОСТИ --
 * Алгоритм верен, поскольку на каждом этапе мы обращаемся стандартным образом к массиву, не выходим за границы,
 * так как индексы всегда подрезаются.
 *    при вставке если список пустой, то просто добавляем элемент. 
 *    Ксли там уже есть элементы, то возникает коллизия, сверям ключи.
 *    Если находится нужный, то обновляем значение, если не находится, то вставляем новый элемент в список.
 *    Удаление и получение элемента по ключу аналогично.
 * 
 * 
 * -- ВРЕМЕННАЯ СЛОЖНОСТЬ --
 * В среднем O(1+alfa), где alfa = количество уникальных элементов / SIZE. Я выбрал SIZE = 2^18-1
 * 
 * 
 * -- ПРОСТРАНСТВЕННАЯ СЛОЖНОСТЬ --
 * O(n), где n - количество предполагаемых запросов.
 */


using System;
using System.IO;

namespace B_HashTable
{

    class MyHashTable
    {

        private Node[] _array;
        private int _size;
        public MyHashTable(int size)
        {
            _size = size >> 3;
            _array = new Node[_size + 1];
        }

        class Node
        {
            public Node(int key, int value, Node next)
            {
                Key = key;
                Value = value;
                Next = next;
            }

            public int Value { get; set; }
            public int Key { get; set; }
            public Node Next { get; set; }
        }

        private int GetBucketIndex(int num)
        {
            return num.GetHashCode() % _size;
        }

        public void Put(int key, int value)
        {
            var node = new Node(key, value, null);
            int backetIndex = GetBucketIndex(node.Key);
            var placeToInsert = _array[backetIndex];

            if (placeToInsert == null)
            {
                _array[backetIndex] = node;
            }
            else
            {
                while (placeToInsert.Next != null)
                {
                    if (placeToInsert.Key == key)
                    {
                        placeToInsert.Value = value;
                        return;
                    }
                    placeToInsert = placeToInsert.Next;
                }
                if (placeToInsert.Key == key)
                {
                    placeToInsert.Value = value;
                    return;
                }
                node.Next = _array[backetIndex];
                _array[backetIndex] = node;
            }
        }

        public int Get(int key)
        {
            int backetIndex = GetBucketIndex(key);
            var placeToGetFrom = _array[backetIndex];

            if (placeToGetFrom == null)
            {
                return -1;
            }

            while (placeToGetFrom != null)
            {
                if (placeToGetFrom.Key == key)
                {
                    return placeToGetFrom.Value;
                }
                placeToGetFrom = placeToGetFrom.Next;
            }
            return -1;
        }

        public int Delete(int key)
        {
            var backerIndex = GetBucketIndex(key);
            var placeWhereDelete = _array[backerIndex];
            if (placeWhereDelete == null)
            {
                return -1;
            }

            if (placeWhereDelete.Next == null)
            {
                if (placeWhereDelete.Key != key)
                {
                    return -1;
                }
                var res = placeWhereDelete.Value;
                _array[backerIndex] = null;
                return res;
            }

            var priveousNode = placeWhereDelete;
            placeWhereDelete = placeWhereDelete.Next;

            if (priveousNode.Key == key)
            {
                _array[backerIndex] = priveousNode.Next;
                return priveousNode.Value;
            }

            while (placeWhereDelete != null)
            {
                if (placeWhereDelete.Key == key)
                {
                    priveousNode.Next = placeWhereDelete.Next;
                    return placeWhereDelete.Value;
                }
                priveousNode = placeWhereDelete;
                placeWhereDelete = placeWhereDelete.Next;
            }

            return -1;
        }

    }

    public class Solution
    {
        private static TextReader _reader;
        private static TextWriter _writer;

        public static void Main(string[] args)
        {
            InitialiseStreams();
            var n = ReadInt();

            MyHashTable hashTable = new MyHashTable(n);

            for (int i = 0; i < n; i++)
            {
                var items = _reader.ReadLine().Split();
                var command = items[0];
                // put (всю строку дольше сравнивать, чем только первый символ)
                if (command[0] == 'p')
                {
                    hashTable.Put(int.Parse(items[1]), int.Parse(items[2]));
                }
                // get
                else if (command[0] == 'g')
                {
                    var val = hashTable.Get(int.Parse(items[1]));
                    if (val == -1)
                    {
                        _writer.WriteLine("None");
                    }
                    else
                    {
                        _writer.WriteLine(val);
                    }
                }
                // delete
                else
                {
                    var val = hashTable.Delete(int.Parse(items[1]));
                    if (val == -1)
                    {
                        _writer.WriteLine("None");
                    }
                    else
                    {
                        _writer.WriteLine(val);
                    }
                }

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
