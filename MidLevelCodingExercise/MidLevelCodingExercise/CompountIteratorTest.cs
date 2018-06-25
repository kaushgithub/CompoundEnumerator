using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MidLevelCodingExercise
{
    public class CompoundEnumeratorTest
    {
        private const bool DEBUG_TEST = true;

        private static Random rnd = new Random((int)DateTime.Now.Ticks);

        public static readonly int[][] SIMPLE_TEST_DATA ={
        new []{0, 1, 2, 3, 4},
        new []{5, 6, 7},
        new []{8},
        new []{9, 10, 11, 12}
    };

        public static readonly int[][] SPARSE_TEST_DATA = {
        new []{0, 1, 2, 3, 4},
        new int[]{},
        new []{5, 6, 7},
        new []{8},
        new int[]{},
        new int[]{},
        new []{9},
        new int[]{},
        new []{10, 11},
        new int[]{},
        new []{12}
    };

        public static readonly int[][] MULTIPLE_EMPTY_TEST_DATA = {
        new int[]{}, new int[]{}, new int[]{}, new int[]{}, new int[]{}, new int[]{}
    };

        public static readonly int NUM_TEST_LOOPS = DEBUG_TEST ? 50 : 100;
        public static readonly int MAX_ITERATORS_FOR_RANDOM_TEST = 100;
        public static readonly int MAX_ELEMENTS_PER_ITERATOR = 5;

        public static readonly String EOL = System.Environment.NewLine;

        public void TestSimpleData()
        {
            try
            {
                int[][] testData = SIMPLE_TEST_DATA;
                doSequentialIntegerTest(testData);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception caught within test");
                Console.WriteLine(e.ToString());
                throw e;
            }
        }

        public void TestSparseData()
        {
            try
            {
                int[][] testData = SPARSE_TEST_DATA;
                doSequentialIntegerTest(testData);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception caught within test");
                Console.WriteLine(e.ToString());
                throw e;
            }
        }

        public void TestEmptyData()
        {
            try
            {
                int[][] testData = MULTIPLE_EMPTY_TEST_DATA;
                doSequentialIntegerTest(testData);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception caught within test");
                Console.WriteLine(e.ToString());
                throw e;
            }

        }

        public void TestRandomNonSparseData()
        {

            try
            {
                for (int i = 0; i < NUM_TEST_LOOPS; i++)
                {
                    int[][] testData = constructRandomTestMatrix(MAX_ITERATORS_FOR_RANDOM_TEST,
                        MAX_ELEMENTS_PER_ITERATOR,
                        false, false);
                    doSequentialIntegerTest(testData);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception caught within test");
                Console.WriteLine(e.ToString());
                throw e;
            }
        }

        public void TestRandomSparseData()
        {

            try
            {
                for (int i = 0; i < NUM_TEST_LOOPS; i++)
                {
                    int[][] testData = constructRandomTestMatrix(MAX_ITERATORS_FOR_RANDOM_TEST,
                        MAX_ELEMENTS_PER_ITERATOR,
                        true, false);
                    doSequentialIntegerTest(testData);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception caught within test");
                Console.WriteLine(e.ToString());
                throw e;
            }
        }

        public void TestRandomSparseDataWithNulls()
        {

            try
            {
                for (int i = 0; i < NUM_TEST_LOOPS; i++)
                {
                    int[][] testData = constructRandomTestMatrix(MAX_ITERATORS_FOR_RANDOM_TEST,
                        MAX_ELEMENTS_PER_ITERATOR,
                        true, true);
                    doSequentialIntegerTest(testData);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception caught within test");
                Console.WriteLine(e.ToString());
                throw e;
            }
        }

        private void doSequentialIntegerTest(int[][] testData)
        {
            // Test the mode of use where you call hasNext()
            doSequentialIntegerTest(testData, true);

            // Test the mode of use where you call next() n times without ever calling hasNext()
            doSequentialIntegerTest(testData, false);
        }

        private void doSequentialIntegerTest(int[][] testData, bool shouldCallHasNext)
        {
            IEnumerator[] iterators = new IEnumerator[testData.Length];
            int numInts = 0;
            for (int i = 0; i < testData.Length; i++)
            {
                if (testData[i] == null)
                {
                    iterators[i] = null;
                }
                else
                {
                    var intList = intArrayToList(testData[i]);
                    iterators[i] = intList.GetEnumerator();
                    numInts += intList.Count;
                }
            }

            Console.WriteLine(intMatrixToString(testData));

            CompoundEnumerator iter = new CompoundEnumerator(iterators);
            int count = 0;
            while (iter.MoveNext())
            {
                int integerFromIterator = (int)iter.Current;
                if (count++ != integerFromIterator)
                {
                    throw new Exception("Unexpected value returned from CompoundIterator; " +
                    "test data was: " + intMatrixToString(testData) + ". ");
                }
            }

            if (numInts != count)
            {
                throw new Exception("Expected number of elements in compound iterator " +
                "to be the sum of the number of elements " +
                "in the individual iterators");
            }

            var ret = iter.MoveNext();
            if (ret)
            {
                throw new Exception("Move Next should continue to return false");
            }
        }

        private List<int> intArrayToList(int[] arr)
        {
            return arr.ToList();
        }

        private int[][] constructRandomTestMatrix(int maxIterators,
                                                  int maxElementsPerIterator,
                                                  bool isSparse,
                                                  bool canContainNullArrays)
        {
            int numIterators = randomIntBetween(1, maxIterators + 1);
            int[][] target = new int[numIterators][];

            int count = 0;
            for (int i = 0; i < target.Length; i++)
            {
                int minElementsPerIterator = 1;
                if (isSparse)
                {
                    if (canContainNullArrays)
                    {
                        minElementsPerIterator = -1;
                    }
                    else
                    {
                        minElementsPerIterator = 0;
                    }
                }
                int numElements =
                    randomIntBetween(minElementsPerIterator,
                        maxElementsPerIterator + 1);
                int[] subArray = null;
                if (numElements >= 0)
                {
                    subArray = new int[numElements];
                    for (int j = 0; j < subArray.Length; j++)
                    {
                        subArray[j] = count++;
                    }
                }
                if (DEBUG_TEST && (subArray == null))
                {
                    Console.WriteLine("subArray is null.");
                }
                target[i] = subArray;
            }

            if (DEBUG_TEST)
            {
                Console.WriteLine(intMatrixToString(target));
            }

            return target;
        }

        private int randomIntBetween(int inclusiveLowerBound, int exclusiveUpperBound)
        {
            if (inclusiveLowerBound >= exclusiveUpperBound)
            {
                throw new ArgumentException(
                    "Bad range for randomIntBetween();" +
                        " expected inclusiveLowerBound of " + inclusiveLowerBound +
                        " to be stricly less than exclusiveUpperBound of " + exclusiveUpperBound);
            }
            int range = exclusiveUpperBound - inclusiveLowerBound;
            return inclusiveLowerBound + rnd.Next(range);
        }

        private String intMatrixToString(int[][] matrix)
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("{").Append(EOL);
            for (int i = 0; i < matrix.Length; i++)
            {
                buf.Append("    ");
                int[] ints = matrix[i];
                if (ints == null)
                {
                    buf.Append("null");
                }
                else
                {
                    buf.Append("{ ");
                    for (int j = 0; j < ints.Length; j++)
                    {
                        int anInt = ints[j];
                        buf.Append(anInt);
                        if (j < ints.Length - 1)
                        {
                            buf.Append(",");
                        }
                        buf.Append(" ");
                    }
                    buf.Append("}");
                }
                if (i < matrix.Length - 1)
                {
                    buf.Append(",");
                }
                buf.Append(EOL);
            }
            buf.Append("}");
            return buf.ToString();
        }

        public void TestInfiniteIterator()
        {
            try
            {
                var tokenSource = new CancellationTokenSource();
                Exception caughtException = null;
                var task = Task.Run(() =>
                {
                    try
                    {
                        InfiniteRandomIntegerEnumerator it = new InfiniteRandomIntegerEnumerator();
                        IEnumerator[] iterators = new IEnumerator[] { it };
                        CompoundEnumerator ci = new CompoundEnumerator(iterators);
                        for (int i = 0; i < 10 && ci.MoveNext(); i++)
                        {
                            var current = ci.Current;
                        }
                    }
                    catch (Exception t)
                    {
                        caughtException = t;
                    }
                }, tokenSource.Token);

                const int timeout = 10 * 1000;
                task.Wait(timeout, tokenSource.Token);

                if (task.Status != TaskStatus.RanToCompletion)
                {
                    tokenSource.Cancel();
                    Console.WriteLine("Improper handling of infinite Iterator; had to wait more than " + timeout + "ms to construct and make 10 calls to MoveNext().");
                }

                if (caughtException != null)
                {
                    throw caughtException;
                }

                if (task.Exception != null)
                {
                    throw task.Exception;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception caught within test");
                Console.WriteLine(e.ToString());
                throw e;
            }
        }

        private class InfiniteRandomIntegerEnumerator : IEnumerator
        {
            private static Random rnd = new Random((int)DateTime.Now.Ticks);

            private int value;
            public object Current
            {
                get { return this.value; }
            }

            public bool MoveNext()
            {
                this.value = rnd.Next();
                return true;
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }
        }

    }
}
