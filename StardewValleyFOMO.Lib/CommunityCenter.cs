using System;
using System.Collections.Generic;
using System.Text;

namespace StardewValleyFOMO.Lib
{
    public class CommunityCenter
    {

    }

    public class Bundle
    {
        public int AreaId { get; } // Area ID of the bundle
        public string BundleKey { get; } // Bundle key (e.g., "Pantry/0")
        public string BundleDefinition { get; } // Bundle definition string
        public List<BundleItem> Items { get; } = new(); // List of items in the bundle
        public int ItemCountToComplete { get; } // Number of items needed to complete the bundle
        public Bundle(int areaId, string bundleKey, string bundleDefinition, int itemCountToComplete)
        {
            AreaId = areaId;
            BundleKey = bundleKey;
            BundleDefinition = bundleDefinition;
            ItemCountToComplete = itemCountToComplete;
        }

        public Bundle(KeyValuePair<string, string> bundleData)
        {
            // Bundle Key: "Boiler Room/22" -> Definition: "Adventurer's/R 518 1/766 99 0 767 10 0 768 1 0 769 10/1/2//Adventurer's"
            var keyParts = bundleData.Key.Split('/');
            BundleKey = keyParts[0];
            AreaId = int.Parse(keyParts[1]);
            BundleDefinition = bundleData.Value;

            var definitionParts = bundleData.Value.Split('/');
            ItemCountToComplete = int.Parse(definitionParts[4]);

            var items = definitionParts[2].Split(' ');
            for (int i = 0; i < items.Length; i += 3)
            {
                int id = int.Parse(items[i]);
                int count = int.Parse(items[i + 1]);
                int minQuality = (i + 2 < items.Length) ? int.Parse(items[i + 2]) : 0;

                Items.Add(new BundleItem(id, count, minQuality));
            }

        }
    }

    public class BundleItem
    {
        public int Id { get; } 
        public int Count { get; } 
        public int MinQuality { get; } 
        public BundleItem(int id, int count, int minQuality)
        {
            Id = id;
            Count = count;
            MinQuality = minQuality;
        }
    }
}
