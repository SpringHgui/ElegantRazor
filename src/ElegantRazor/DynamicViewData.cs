using System;
using System.Collections.Generic;

namespace ElegantRazor
{
    public class DynamicViewData
    {
        Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();

        public void AddValue(string key, object val)
        {
            keyValuePairs.Add(key, val);
        }

        public Dictionary<string, object> GetViewDataDictionary()
        {
            return keyValuePairs;
        }
    }


    public class DynamicViewBag : DynamicViewData
    {
    }
}
