using System.Collections.Generic;
using System.ComponentModel;

namespace XC2SaveNETThingy
{
    public static class BindingListExtensions
    {
        public static void AddRange<T>(this BindingList<T> list, IEnumerable<T> items)
        {
            bool raiseListChangedEvents = list.RaiseListChangedEvents;
            list.RaiseListChangedEvents = false;
            try
            {
                foreach (T item in items)
                    list.Add(item);
            }
            finally
            {
                list.RaiseListChangedEvents = raiseListChangedEvents;
            }

            if (raiseListChangedEvents)
                list.ResetBindings();
        }
    }
}
