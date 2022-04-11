// https://contest.yandex.ru/contest/24414/run-report/67215556/

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
 * O(1), но константа C в данном случае большая, C = 2^18, так как мы выделяем много памяти под массив,
 * даже если нужно будет хранить всего 1 элемент.
 */


using System;
using System.IO;

namespace B_HashTable
{

    class MyHashTable
    {
        const int SIZE = 0b1111111111111111;

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


        private int MyHash(int num)
        {
            return num & SIZE;
        }

        Node[] _array = new Node[SIZE + 1];


        public void Put(int key, int value)
        {
            var node = new Node(key, value, null);
            int hash = MyHash(node.Key);
            var placeToInsert = _array[hash];

            if (placeToInsert == null)
            {
                _array[hash] = node;
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
                node.Next = _array[hash];
                _array[hash] = node;
            }
        }

        public int Get(int key)
        {
            int hash = MyHash(key);
            var placeToGet = _array[hash];

            if (placeToGet == null)
            {
                return -1;
            }

            while (placeToGet != null)
            {
                if (placeToGet.Key == key)
                {
                    return placeToGet.Value;
                }
                placeToGet = placeToGet.Next;
            }
            return -1;
        }

        public int Delete(int key)
        {
            var hash = MyHash(key);
            var placeToDelete = _array[hash];
            if (placeToDelete == null)
            {
                return -1;
            }

            if (placeToDelete.Next == null)
            {
                if (placeToDelete.Key != key)
                {
                    return -1;
                }
                var res = placeToDelete.Value;
                _array[hash] = null;
                return res;
            }

            var priveousNode = placeToDelete;
            placeToDelete = placeToDelete.Next;

            if (priveousNode.Key == key)
            {
                _array[hash] = priveousNode.Next;
                return priveousNode.Value;
            }

            while (placeToDelete != null)
            {
                if (placeToDelete.Key == key)
                {
                    priveousNode.Next = placeToDelete.Next;
                    return placeToDelete.Value;
                }
                priveousNode = placeToDelete;
                placeToDelete = placeToDelete.Next;
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
            MyHashTable hashTable = new MyHashTable();


            var n = ReadInt();



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
