using System.Collections.Generic;

public static class Flowers
{
    const int MAX_ZONE = 8;

    public enum FlowerRarity { 
        Common,
        Seasonal,
        Rare,
        Unique
    }

    public struct Flower {
        public string Name { get; private set; }
        public FlowerRarity Rarity { get; private set; }
        public IntRange Zones { get; private set; }
        public IntRange Resources { get; private set; }

        public Flower(string name, FlowerRarity rarity, IntRange zones, IntRange resources) {
            Name = name;
            Rarity = rarity;
            Zones = zones;
            Resources = resources;
        }
    }

    /*
     *         "Rose",
        "Aster",
        "Daisy",
        "Lilac",
        "Cherry-Blossom",
        "Sunflower",
        "Violet",
        "Lily",
        "Poppy",
        "Appricot",
        "Apple",
        "Walnut"
     */

    //static List<Flower> flowers = new List<Flower>()
    //{
    //    new Flower(
    //        name: "Lily",
    //        rarity: FlowerRarity.Common,
    //        zones: new IntRange(0, MAX_ZONE),
    //        resources: new IntRange(3, 5)
    //    )
    //};



}

