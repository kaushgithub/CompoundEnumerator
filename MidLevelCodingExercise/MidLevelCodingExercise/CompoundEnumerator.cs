using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MidLevelCodingExercise
{
    public class CompoundEnumerator : IEnumerator
    {
        private IEnumerator[] ceEnumerators;
        private int currentEnumeratorIndex; //tracks the current enumerator
        private int enumeratorIndex;
        private IEnumerator currentEnumerator;
        object currentObject = null;
        public CompoundEnumerator(IEnumerator[] enumerators)
        {
            ceEnumerators = enumerators;
            currentEnumeratorIndex = 1;
            enumeratorIndex = 0;
            if (ceEnumerators != null && ceEnumerators.Count() > 0)
            {
                currentEnumerator = ceEnumerators[enumeratorIndex];
            }
        }

        public object Current
        {
            get
            {
                return currentObject;
            }
        }

        public bool MoveNext()
        {
            if (ceEnumerators != null && currentEnumeratorIndex <= ceEnumerators.Count()) //if the current enumerator index count is less than enumerators go inside
            {
                if (currentEnumerator != null && currentEnumerator.MoveNext()) //No more elements, increment the current enumerator index
                {
                    currentObject = currentEnumerator.Current;
                    return true;
                }
                else
                {
                    currentEnumeratorIndex++;
                    while (currentEnumeratorIndex <= ceEnumerators.Count()) //loop so that some enumerators may not have anything
                    {
                        enumeratorIndex++;
                        currentEnumerator = ceEnumerators[enumeratorIndex];
                        if (currentEnumerator != null && currentEnumerator.MoveNext())
                        {
                            currentObject = currentEnumerator.Current;
                            return true;
                        }
                        else
                        {
                            currentEnumeratorIndex++;
                        }
                    }
                }
            }
            return false;
        }

        public void Reset()
        {
            currentEnumeratorIndex = 1;
            enumeratorIndex = 0;
        }
    }
}
