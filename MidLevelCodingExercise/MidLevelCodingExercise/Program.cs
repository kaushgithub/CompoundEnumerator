using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidLevelCodingExercise
{
    class Program
    {
        static void Main(string[] args)
        {
            var test = new CompoundEnumeratorTest();
            test.TestSimpleData();
            test.TestSparseData();
            test.TestEmptyData();
            test.TestRandomNonSparseData();
            test.TestRandomSparseData();
            test.TestRandomSparseDataWithNulls();
            test.TestInfiniteIterator();

            Console.WriteLine("Tests completed successfully.");
            Console.Read();
        }
    }
}
