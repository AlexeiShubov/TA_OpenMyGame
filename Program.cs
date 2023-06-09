using System;
using System.Collections.Generic;
using System.Linq;

namespace TaskExam
{
    internal class TaskSolver
    {
        public enum Figure
        {
            Horse,
            Queen
        }

        public const char INCORRECT_SIMVOL = '0';
        public const char BLOCKED_POSITION = 'x';
        public const char START_POSITION = 's';
        public const char FINISH_POSITION = 'e';

        public static readonly (int X, int Y)[] moveDirectionsHorse =
        {
                (-2, -1),
                (-1, -2),
                (-2, 1),
                (-1, 2),
                (1, -2),
                (2, -1),
                (1, 2),
                (2, 1)
        };
        public static readonly (int X, int Y)[] moveDirectionsQueen =
        {
                (-1, -1),
                (-1, 1),
                (1, -1),
                (1, 1),
                (-1, 0),
                (1, 0),
                (0, -1),
                (0, 1)
        };

        public static void Main(string[] args)
        {
            TestGenerateWordsFromWord();
            TestMaxLengthTwoChar();
            TestGetPreviousMaxDigital();
            TestSearchQueenOrHorse();
            TestCalculateMaxCoins();

            Console.WriteLine("All Test completed!");

            Console.ReadLine();
        }

        /// задание 1) Слова из слова
        public static List<string> GenerateWordsFromWord(string word, List<string> wordDictionary)
        {
            List<string> resultList = new List<string>();

            Dictionary<char, int> wordMap = GetLettersMap(word);

            foreach (var currentCompareWord in wordDictionary)
            {
                if(word.Length >= currentCompareWord.Length && CompareLettersMap(wordMap, GetLettersMap(currentCompareWord)))
                {
                    resultList.Add(currentCompareWord);
                }
            }

            resultList.Sort();

            return resultList;
        }

        public static bool CompareLettersMap(Dictionary<char, int> startWordMap, Dictionary<char, int> compareWordMap)
        {
            foreach (var letter in compareWordMap)
            {
                if (!startWordMap.ContainsKey(letter.Key) || startWordMap[letter.Key] < compareWordMap[letter.Key])
                {
                    return false;
                }
            }

            return true;
        }

        public static Dictionary<char, int> GetLettersMap(string word)
        {
            Dictionary<char, int> map = new Dictionary<char, int>();

            foreach (var letter in word)
            {
                if (map.ContainsKey(letter))
                {
                    map[letter]++;
                }
                else
                {
                    map.Add(letter, 1);
                }
            }

            return map;
        }

        /// задание 2) Два уникальных символа
        public static int MaxLengthTwoChar(string word)
        {
            HashSet<char> uniqueSimvols = new HashSet<char>();

            int result = 0;

            for (int i = 0; i < word.Length; i++)
            {
                uniqueSimvols.Add(word[i]);
            }

            List<char> uniqueSimvolsList = new List<char>(uniqueSimvols);

            for (int i = 0; i < uniqueSimvolsList.Count - 1; i++)
            {
                for (int j = i + 1; j < uniqueSimvolsList.Count; j++)
                {
                    (char, char) pair = (uniqueSimvolsList[i], uniqueSimvolsList[j]);

                    int tempLenth = GetLength(pair.Item1, pair.Item2, word);

                    result = tempLenth > result ? tempLenth : result;
                }
            }

            return result;
        }

        public static int GetLength(char ferstSimvol, char secondSimvol, string word)
        {
            int result = 0;

            for (int i = 0; i < word.Length; i++)
            {
                if (word[i] == ferstSimvol)
                {
                    (ferstSimvol, secondSimvol) = (secondSimvol, ferstSimvol);

                    result++;
                }
                else if (word[i] == secondSimvol)
                {
                    return 0;
                }
            }

            return result;
        }

        /// задание 3) Предидущее число
        public static long GetPreviousMaxDigital(long value)
        {
            if (value.ToString().Length <= 2)
            {
                char[] temp = SpecialCaseCheck(value.ToString().ToArray());

                return temp == null ? -1 : long.Parse(new string(temp));
            }

            char[] result = GeneratingASmallerNumber(value.ToString().ToArray());

            if (result == null || result[0] == INCORRECT_SIMVOL)
            {
                return -1;
            }

            return long.Parse(new string(result));
        }

        public static char[] GeneratingASmallerNumber(char[] word)
        {
            for (int index = word.Length - 3; index >= 0; index--)
            {
                char[] tempTailWord = new char[word.Length - index];

                Array.Copy(word, index, tempTailWord, 0, tempTailWord.Length);

                if (tempTailWord.Length == 3)
                {
                    tempTailWord = CheckLastThreeNumbers(tempTailWord);

                    if (tempTailWord == null)
                    {
                        continue;
                    }

                    Array.Copy(tempTailWord, 0, word, index, tempTailWord.Length);

                    return word;
                }

                tempTailWord = ShufleTailWord(tempTailWord);

                if (tempTailWord != null)
                {
                    Array.Copy(tempTailWord, 0, word, index, tempTailWord.Length);

                    return word;
                }
            }

            return null;
        }

        public static char[] ShufleTailWord(char[] word)
        {
            char[] result = null;

            if (word[0] > word[1])
            {
                char[] temp = new char[word.Length - 1];
                result = new char[word.Length];

                result[0] = word.Where(t => t < word[0]).Max();

                for (int i = 0; i < word.Length; i++)
                {
                    if (result[0] == word[i])
                    {
                        word[i] = word[0];

                        break;
                    }
                }

                Array.Copy(word, 1, temp, 0, temp.Length);
                Array.Sort(temp);
                Array.Reverse(temp);
                Array.Copy(temp, 0, result, 1, temp.Length);
            }

            return result;
        }

        public static char[] CheckLastThreeNumbers(char[] word)
        {
            char[] sortedWord = new char[3];

            Array.Copy(word, word.Length - 3, sortedWord, 0, sortedWord.Length);
            Array.Sort(sortedWord);

            if (!word.SequenceEqual(sortedWord))
            {
                return ShufleLastThreeNumbers(word, sortedWord);
            }

            return null;
        }

        public static char[] ShufleLastThreeNumbers(char[] startWord, char[] sortedWord)
        {
            Array.Copy(startWord, startWord.Length - 3, sortedWord, 0, sortedWord.Length);

            if (startWord.Max() == startWord[0] && startWord[0] != startWord[2])
            {
                if (startWord[1] > startWord[2])
                {
                    (sortedWord[1], sortedWord[2]) = (sortedWord[2], sortedWord[1]);
                }
                else if (startWord[1] != startWord[2])
                {
                    (sortedWord[0], sortedWord[1], sortedWord[2]) = (sortedWord[2], sortedWord[0], sortedWord[1]);
                }

                return sortedWord;
            }
            else
            {
                if (startWord[1] > startWord[2])
                {
                    (sortedWord[1], sortedWord[2]) = (sortedWord[2], sortedWord[1]);
                }
                else if (startWord[1] < startWord[2])
                {
                    (sortedWord[0], sortedWord[1], sortedWord[2]) = (sortedWord[1], sortedWord[2], sortedWord[0]);
                }

                return sortedWord;
            }
        }

        public static char[] SpecialCaseCheck(char[] word)
        {
            if (word.Length <= 1)
            {
                return null;
            }
            else if (word.Length == 2)
            {
                if (word[0] > word[1] && word[1] != INCORRECT_SIMVOL)
                {
                    (word[0], word[1]) = (word[1], word[0]);

                    return word;
                }
                else
                {
                    return null;
                }
            }

            return null;
        }

        /// задание 4) Конь и Королева
        public static List<int> SearchQueenOrHorse(char[][] gridMap)
        {
            List<int> result = new List<int>();

            ((int, int) Start, (int, int) Finish) = GetStartFinishPositions(gridMap);

            result.Add(FindPath(gridMap, Start, Finish, moveDirectionsHorse, moveDirectionsQueen, Figure.Horse));
            result.Add(FindPath(gridMap, Start, Finish, moveDirectionsHorse, moveDirectionsQueen, Figure.Queen));

            return result;
        }

        public static ((int, int), (int, int)) GetStartFinishPositions(char[][] gridMap)
        {
            (int, int) start = (0, 0);
            (int, int) finish = (0, 0);

            for (int x = 0; x < gridMap.Length; x++)
            {
                for (int y = 0; y < gridMap[x].Length; y++)
                {
                    if (gridMap[x][y] == START_POSITION)
                    {
                        start = (x, y);
                    }
                    else if (gridMap[x][y] == FINISH_POSITION)
                    {
                        finish = (x, y);
                    }
                }
            }

            return (start, finish);
        }

        public static (int, int) GetNextStepFigure(char[][] gridMap, (int X, int Y) currentPosition, (int X, int Y) directionMove)
        {
            (int X, int Y) nextPosition = (currentPosition.X + directionMove.X, currentPosition.Y + directionMove.Y);

            if (nextPosition.X >= 0 && nextPosition.Y >= 0 && nextPosition.X < gridMap.Length
                && nextPosition.Y < gridMap[currentPosition.X].Length && gridMap[nextPosition.X][nextPosition.Y] != BLOCKED_POSITION)
            {
                return nextPosition;
            }

            return (-1, -1);
        }

        public static Queue<(int, int)> GetNextPositionsHorse(char[][] gridMap, (int X, int Y) checkedPosition, Dictionary<(int, int), int> visitedPositions, Queue<(int, int)> moveDirections)
        {
            Queue<(int, int)> nextPositions = new Queue<(int, int)>();

            (int, int) currentPosition = checkedPosition;

            foreach (var direction in moveDirections)
            {
                currentPosition = GetNextStepFigure(gridMap, currentPosition, direction);

                if (currentPosition != (-1, -1) && !visitedPositions.ContainsKey(currentPosition))
                {
                    visitedPositions.Add(currentPosition, visitedPositions[checkedPosition] + 1);
                    nextPositions.Enqueue(currentPosition);
                }

                currentPosition = checkedPosition;
            }

            return nextPositions;
        }

        public static Queue<(int, int)> GetNextPositionsQueen(char[][] gridMap, (int, int) checkedPosition, Dictionary<(int, int), int> visitedPositions, Queue<(int, int)> moveDirections)
        {
            Queue<(int, int)> nextPositions = new Queue<(int, int)>();
            Queue<(int, int)> temp = new Queue<(int, int)>(moveDirections);

            (int, int) currentPosition;

            while (temp.Count > 0)
            {
                currentPosition = GetNextStepFigure(gridMap, checkedPosition, temp.Peek());

                while (true)
                {
                    if (currentPosition == (-1, -1))
                    {
                        temp.Dequeue();

                        break;
                    }

                    if (!visitedPositions.ContainsKey(currentPosition))
                    {
                        visitedPositions.Add(currentPosition, visitedPositions[checkedPosition] + 1);
                        nextPositions.Enqueue(currentPosition);
                    }
                    else if (visitedPositions[checkedPosition] < visitedPositions[currentPosition])
                    {
                        visitedPositions[currentPosition] = visitedPositions[checkedPosition] + 1;
                    }

                    currentPosition = GetNextStepFigure(gridMap, currentPosition, temp.Peek());
                }
            }

            return nextPositions;
        }

        public static int FindPath(char[][] gridMap, (int, int) start, (int X, int Y) finish, (int, int)[] directionsHorse, (int, int)[] directionsQueen, Figure figure)
        {
            Dictionary<(int, int), int> visitedPositions = new Dictionary<(int, int), int>();
            Queue<Queue<(int, int)>> allMoves = new Queue<Queue<(int, int)>>();
            Queue<(int, int)> moveDirections = null;

            visitedPositions.Add(start, 0);

            if (figure == Figure.Horse)
            {
                moveDirections = new Queue<(int, int)>(directionsHorse);
                allMoves.Enqueue(GetNextPositionsHorse(gridMap, start, visitedPositions, moveDirections));
            }
            else if (figure == Figure.Queen)
            {
                moveDirections = new Queue<(int, int)>(directionsQueen);
                allMoves.Enqueue(GetNextPositionsQueen(gridMap, start, visitedPositions, moveDirections));
            }

            if (allMoves.Peek().Contains((finish.X, finish.Y)))
            {
                return 1;
            }

            while (allMoves.Count > 0)
            {
                Queue<(int, int)> temp = allMoves.Dequeue();

                while (temp.Count > 0)
                {
                    if (figure == Figure.Horse)
                    {
                        allMoves.Enqueue(GetNextPositionsHorse(gridMap, temp.Dequeue(), visitedPositions, moveDirections));
                    }
                    else if (figure == Figure.Queen)
                    {
                        allMoves.Enqueue(GetNextPositionsQueen(gridMap, temp.Dequeue(), visitedPositions, moveDirections));
                    }
                }
            }

            if (visitedPositions.ContainsKey(finish))
            {
                return visitedPositions[finish];
            }

            return -1;
        }

        /// задание 5) Жадина
        public static long CalculateMaxCoins(int[][] mapData, int idStart, int idFinish)
        {
            if (idStart == idFinish)
            {
                return -1;
            }

            Dictionary<int, int> lengthToPositions = new Dictionary<int, int>();

            int maxPath = -1;

            lengthToPositions.Add(idStart, 0);

            GetPath(SetGraph(mapData), idStart, idFinish, lengthToPositions, new HashSet<int>(), ref maxPath);

            return maxPath;
        }

        public static Dictionary<int, Queue<(int To, int Weight)>> SetGraph(int[][] mapdata)
        {
            Dictionary<int, Queue<(int To, int Weight)>> graph = new Dictionary<int, Queue<(int To, int Weight)>>();

            foreach (var item in mapdata)
            {
                AddEdgeToGraph(graph, item[0], item[1], item[2]);
                AddEdgeToGraph(graph, item[1], item[0], item[2]);
            }

            return graph;
        }

        public static void AddEdgeToGraph(Dictionary<int, Queue<(int To, int Weight)>> graph, int from, int to, int weight)
        {
            if (!graph.ContainsKey(from))
            {
                graph.Add(from, new Queue<(int To, int Weight)>());
            }

            graph[from].Enqueue((to, weight));
        }

        public static void GetPath(Dictionary<int, Queue<(int To, int Weight)>> graph, int from, int finish, Dictionary<int, int> lengthToPositions, HashSet<int> visitedPositions, ref int maxLengthToFinish)
        {
            Queue<(int To, int Weight)> q = new Queue<(int To, int Weight)>(graph[from]);

            visitedPositions.Add(from);

            while (q.Count > 0)
            {
                if (q.Peek().To == finish)
                {
                    maxLengthToFinish = maxLengthToFinish < lengthToPositions[from] + q.Peek().Weight ? lengthToPositions[from] + q.Peek().Weight : maxLengthToFinish;

                    q.Dequeue();
                }
                else if (!visitedPositions.Contains(q.Peek().To) && !lengthToPositions.ContainsKey(q.Peek().To))
                {
                    lengthToPositions.Add(q.Peek().To, lengthToPositions[from] + q.Peek().Weight);

                    GetPath(graph, q.Dequeue().To, finish, lengthToPositions, visitedPositions, ref maxLengthToFinish);
                }
                else
                {
                    q.Dequeue();
                }
            }
        }

        /// Тесты
        private static void TestGenerateWordsFromWord()
        {
            var wordsList = new List<string>
            {
                "кот", "ток", "око", "мимо", "гром", "ром", "мама",
                "рог", "морг", "огр", "мор", "порог", "бра", "раб", "зубр",
            };

            AssertSequenceEqual(GenerateWordsFromWord("арбуз", wordsList), new[] { "бра", "зубр", "раб" });
            AssertSequenceEqual(GenerateWordsFromWord("лист", wordsList), new List<string>());
            AssertSequenceEqual(GenerateWordsFromWord("маг", wordsList), new List<string>());
            AssertSequenceEqual(GenerateWordsFromWord("погром", wordsList), new List<string> { "гром", "мор", "морг", "огр", "порог", "рог", "ром" });
        }

        private static void TestMaxLengthTwoChar()
        {
            AssertEqual(MaxLengthTwoChar("a"), 0);
            AssertEqual(MaxLengthTwoChar("ab"), 2);
            AssertEqual(MaxLengthTwoChar("aa"), 0);
            AssertEqual(MaxLengthTwoChar("beabeefeab"), 5);
            AssertEqual(MaxLengthTwoChar("beabeeab"), 5);
            AssertEqual(MaxLengthTwoChar("а"), 0);
            AssertEqual(MaxLengthTwoChar("ab"), 2);
        }

        private static void TestGetPreviousMaxDigital()
        {
            AssertEqual(GetPreviousMaxDigital(21), 12l);
            AssertEqual(GetPreviousMaxDigital(531), 513l);
            AssertEqual(GetPreviousMaxDigital(1027), -1l);
            AssertEqual(GetPreviousMaxDigital(2071), 2017l);
            AssertEqual(GetPreviousMaxDigital(207034), 204730l);
            AssertEqual(GetPreviousMaxDigital(135), -1l);
        }

        private static void TestSearchQueenOrHorse()
        {
            char[][] gridA =
            {
                new[] {'s', '#', '#', '#', '#', '#'},
                new[] {'#', 'x', 'x', 'x', 'x', '#'},
                new[] {'#', '#', '#', '#', 'x', '#'},
                new[] {'#', '#', '#', '#', 'x', '#'},
                new[] {'#', '#', '#', '#', '#', 'e'},
            };

            AssertSequenceEqual(SearchQueenOrHorse(gridA), new[] { 3, 2 });

            char[][] gridB =
            {
                new[] {'s', '#', '#', '#', '#', 'x'},
                new[] {'#', 'x', 'x', 'x', 'x', '#'},
                new[] {'#', 'x', '#', '#', 'x', '#'},
                new[] {'#', '#', '#', '#', 'x', '#'},
                new[] {'x', '#', '#', '#', '#', 'e'},
            };

            AssertSequenceEqual(SearchQueenOrHorse(gridB), new[] { -1, 3 });

            char[][] gridC =
            {
                new[] {'s', '#', '#', '#', '#', 'x'},
                new[] {'x', 'x', 'x', 'x', 'x', 'x'},
                new[] {'#', '#', '#', '#', 'x', '#'},
                new[] {'#', '#', '#', 'e', 'x', '#'},
                new[] {'x', '#', '#', '#', '#', '#'},
            };

            AssertSequenceEqual(SearchQueenOrHorse(gridC), new[] { 2, -1 });


            char[][] gridD =
            {
                 new[] {'e', '#'},
                 new[] {'x', 's'},
             };

            AssertSequenceEqual(SearchQueenOrHorse(gridD), new[] { -1, 1 });

            char[][] gridE =
            {
                new[] {'e', '#'},
                new[] {'x', 'x'},
                new[] {'#', 's'},
             };

            AssertSequenceEqual(SearchQueenOrHorse(gridE), new[] { 1, -1 });

            char[][] gridF =
            {
                new[] {'x', '#', '#', 'x'},
                new[] {'#', 'x', 'x', '#'},
                new[] {'#', 'x', '#', 'x'},
                new[] {'e', 'x', 'x', 's'},
                new[] {'#', 'x', 'x', '#'},
                new[] {'x', '#', '#', 'x'},
            };

            AssertSequenceEqual(SearchQueenOrHorse(gridF), new[] { -1, 5 });
        }

        private static void TestCalculateMaxCoins()
        {
            var mapA = new[]
            {
                new []{0, 1, 1},
                new []{0, 2, 4},
                new []{0, 3, 3},
                new []{1, 3, 10},
                new []{2, 3, 6},
            };

            AssertEqual(CalculateMaxCoins(mapA, 0, 3), 11l);

            var mapB = new[]
            {
                new []{0, 1, 1},
                new []{1, 2, 53},
                new []{2, 3, 5},
                new []{5, 4, 10}
            };

            AssertEqual(CalculateMaxCoins(mapB, 0, 5), -1l);

            var mapC = new[]
            {
                new []{0, 1, 1},
                new []{0, 3, 2},
                new []{0, 5, 10},
                new []{1, 2, 3},
                new []{2, 3, 2},
                new []{2, 4, 7},
                new []{3, 5, 3},
                new []{4, 5, 8}
            };

            AssertEqual(CalculateMaxCoins(mapC, 0, 5), 19l);
        }

        /// Тестирующая система
        private static void Assert(bool value)
        {
            if (value)
            {
                return;
            }

            throw new Exception("Assertion failed");
        }

        private static void AssertEqual(object value, object expectedValue)
        {
            if (value.Equals(expectedValue))
            {
                return;
            }

            throw new Exception($"Assertion failed expected = {expectedValue} actual = {value}");
        }

        private static void AssertSequenceEqual<T>(IEnumerable<T> value, IEnumerable<T> expectedValue)
        {
            if (ReferenceEquals(value, expectedValue))
            {
                return;
            }

            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (expectedValue is null)
            {
                throw new ArgumentNullException(nameof(expectedValue));
            }

            var valueList = value.ToList();
            var expectedValueList = expectedValue.ToList();

            if (valueList.Count != expectedValueList.Count)
            {
                throw new Exception($"Assertion failed expected count = {expectedValueList.Count} actual count = {valueList.Count}");
            }

            for (var i = 0; i < valueList.Count; i++)
            {
                if (!valueList[i].Equals(expectedValueList[i]))
                {
                    throw new Exception($"Assertion failed expected value at {i} = {expectedValueList[i]} actual = {valueList[i]}");
                }
            }
        }
    }
}
