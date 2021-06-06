using System;
using System.Collections.Generic;
using System.Linq;

namespace IVM.Studio.Utils
{
    public class ChannelNameConverter
    {
        private readonly IDictionary<int, string> convertTable;

        private readonly IDictionary<string, int> convertBackTable;

        public bool IsFrozen { get; private set; }

        public ChannelNameConverter()
        {
            convertTable = new SortedDictionary<int, string>();
            convertBackTable = new SortedDictionary<string, int>();
            IsFrozen = false;
        }

        public void AddMatch(int ChannelNumber, string ChannelName)
        {
            if (IsFrozen) 
                throw new InvalidOperationException("This converter is frozen.");

            try
            {
                if (convertTable.ContainsKey(ChannelNumber) || convertBackTable.ContainsKey(ChannelName)) 
                    throw new ArgumentException("Some of specified match already exists.");

                convertTable.Add(ChannelNumber, ChannelName);
                convertBackTable.Add(ChannelName, ChannelNumber);
            }
            catch {}
        }

        public void Freeze()
        {
            if (IsFrozen) 
                return;

            SortedSet<int> channelNumbers = new SortedSet<int> { 0, 1, 2, 3 };
            SortedSet<string> channelNames = new SortedSet<string> { "A", "B", "C", "D" };
            channelNumbers.ExceptWith(convertTable.Keys);
            channelNames.ExceptWith(convertTable.Values);

            if (channelNumbers.Count != channelNames.Count) 
                throw new InvalidOperationException("This converter has invalid matches.");

            foreach (var (num, name) in channelNumbers.Zip(channelNames, (num, name) => (num, name)))
            {
                convertTable.Add(num, name);
                convertBackTable.Add(name, num);
            }

            IsFrozen = true;
        }

        public string ConvertNumberToName(int ChannelNumber)
        {
            if (!IsFrozen) 
                throw new InvalidOperationException("This converter needs to be frozen.");

            return 
                convertTable[ChannelNumber];
        }

        public int ConvertNameToNumber(string ChannelName)
        {
            if (!IsFrozen) 
                throw new InvalidOperationException("This converter needs to be frozen.");

            return 
                convertBackTable[ChannelName];
        }
    }
}
