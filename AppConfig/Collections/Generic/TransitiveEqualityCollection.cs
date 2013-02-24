using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppConfig.Collections.Generic
{
    public class TransitiveEqualityCollection<T> : List<IdentifierPair<T>>
    {
        private Dictionary<T, List<T>> outputDictionaryByFinalID;
        public Dictionary<T, List<T>> OutputDictionaryByFinalID
        {
            get
            {
                if (outputDictionaryByFinalID != null)
                    return outputDictionaryByFinalID;

                CreateOutputLists();
                return outputDictionaryByFinalID;
            }
        }

        private Dictionary<T, T> outputDictionaryByReplacedID;
        public Dictionary<T, T> OutputDictionaryByReplacedID
        {
            get
            {
                if (outputDictionaryByReplacedID != null)
                    return outputDictionaryByReplacedID;

                CreateOutputLists();
                return outputDictionaryByReplacedID;
            }
        }

        public void Add(T ID1, T ID2)
        {
            base.Add(new IdentifierPair<T>(ID1, ID2));
            BaseListChanged();
        }

        private void CreateOutputLists()
        {
            outputDictionaryByFinalID = new Dictionary<T, List<T>>();
            outputDictionaryByReplacedID = new Dictionary<T, T>();

            foreach (IdentifierPair<T> item in this)
            {
                //Get the first id unless it is in the replaced list then take the replaced with value
                T id1 = (outputDictionaryByReplacedID.Keys.Contains(item.ID1))
                    ? outputDictionaryByReplacedID[item.ID1]
                    : item.ID1;

                //If we are mapping the same id then lets skip this record
                if (id1.Equals(item.ID2))
                    continue;

                //If this id has already been replaced then continue.
                if (outputDictionaryByReplacedID.Keys.Contains(item.ID2))
                    continue;

                //Add the second id to the replaced list and use id1 as the id that replaces it
                outputDictionaryByReplacedID.Add(item.ID2, id1);

                //Find the list of ids for the id1 value and add the replaced id to the list
                //if the list doesn't exist then create it first.
                List<T> replacedIDList;
                if (!outputDictionaryByFinalID.TryGetValue(id1, out replacedIDList))
                {
                    replacedIDList = new List<T>();
                    outputDictionaryByFinalID.Add(id1, replacedIDList);
                }
                replacedIDList.Add(item.ID2);
            }
        }

        private void BaseListChanged()
        {
            outputDictionaryByFinalID = null;
            outputDictionaryByReplacedID = null;
        }
    }
}
