using Discord;
using Discord.Rest;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using LightBlueFox.Util.Logging;

namespace BOT_Marvin___Counter_Strike_Discord_Bot.Items
{
    public class SkinItem : Item, IEconomyItem, IDropableItem
    {

        /// <summary>
        /// Creates a new SkinItem from the values in this BsonDocument.<br/>
        /// PLEASE NOTE THAT THIS IS RESOURCE INTENSIVE, SINCE IT REQUIRES A DATABASE LOOKUP. Look if the object is cached in <see cref="Item.cachedItems"/>. 
        /// </summary>
        /// <param name="b">A bson document from the 'items' collection.</param>
        public SkinItem(BsonDocument b) : base(b)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("item_id", ItemID);
            var skin = MongoInterface.db.GetCollection<BsonDocument>("skins").Find(filter).FirstOrDefault();

            isActivityDropping = skin["isActivityDropping"].AsBoolean;
            dropValueMultiplier = 1;
            price = skin["price"].AsInt32;
            ParseModifiedPrices(skin);
            Rarity = SkinRarity.FromString((skin["rarity"].AsString));
            WeaponName = skin["weapon-name"].AsString;
            InspectLink = skin["inspect-link"].AsString;
            isActivityDropping = skin["isActivityDropping"].AsBoolean;
            IsCaseDropable = skin["isDropable"].AsBoolean;
        }

        /// <summary>
        /// Parses the prices of the weapon with <see cref="SkinModifier"/>s.
        /// </summary>
        private void ParseModifiedPrices(BsonDocument skin)
        {
            var mp = skin.GetValue("modifiedPrices", null);
            if (mp == null)
            {
                return;
            }
            var modPrices = mp.AsBsonDocument;
            foreach (SkinModifier s in Enum.GetValues(typeof(SkinModifier)))
            {
                var res = modPrices.GetValue(s.ToString(), -1).AsInt32;
                if (res != -1)
                    ModifiedPrice.Add(s, res);
            }
        }
        private bool isActivityDropping;
        private double dropValueMultiplier;
        private int price;

        #region Public Properties
        /// <summary>
        /// Inherited from <see cref="Item"/>.
        /// </summary>
        /// <returns><see cref="ItemType.Skin"/></returns>
        public override ItemType getType() => ItemType.Skin;
        /// <summary>
        /// Inherited from <see cref="IDropableItem"/>.
        /// </summary>
        /// <returns>Whether or not this is randomly dropped to active players.</returns>
        public bool IsActivityDropping() => isActivityDropping;
        /// <summary>
        /// Inherited from <see cref="IDropableItem"/>. The value (and therefore the probability) of a drop is determined by <see cref="GetPrice"/> * <see cref="GetDropValueMultiplier"/>
        /// </summary>
        public double GetDropValueMultiplier() => dropValueMultiplier;
        /// <summary>
        /// How much this skin costs in coins. 100 Coins = 1 Euro
        /// </summary>
        public int GetPrice() => price;
        /// <summary>
        /// The value of the skins depending on the <see cref="SkinModifier"/> it has.
        /// </summary>
        public Dictionary<SkinModifier, int> ModifiedPrice { get; private set; } = new Dictionary<SkinModifier, int>();
        /// <summary>
        /// How rare this skin is.
        /// </summary>
        public SkinRarity Rarity { get; private set; }
        /// <summary>
        /// The name of the weapon, for example "AK-47", "Desert Eagle"
        /// </summary>
        public string WeaponName { get; private set; }
        /// <summary>
        /// A link to inspect the item in CSGO
        /// </summary>
        public string InspectLink { get; private set; }
        /// <summary>
        /// Whether or not this item is allowed to drop from cases.
        /// </summary>
        public bool IsCaseDropable { get; private set; }
        #endregion
    }

    /// <summary>
    /// Skins can have specialities, like StatTrak, about them. This enum is a flags enum, so modifiers can be combined.
    /// </summary>
    [Flags()]
    public enum SkinModifier
    {
        Normal = 0,
        Souvenir = 1,
        StatTrak = 2,
    }

    /// <summary>
    /// Different rarities of skins.
    /// </summary>
    public class SkinRarity
    {
        public static readonly Color UnknownRarityColor = Color.LightGrey;
        public static readonly int UnkownRarityProbabilityValue = 1;
        /// <summary>
        /// How Valuable this skin is. From 0 to 6, higher -> more valuable.
        /// </summary>
        public int Value { get; private set; }

        public int ProbabilityValue { get; private set; }

        public string Name { get; private set; }

        /// <summary>
        /// The Color corresponding to the rarity.
        /// </summary>
        public Color Color { get; private set; }

        /// <summary>
        /// Enum class constructor
        /// </summary>
        /// <param name="val"><see cref="Value"/></param>
        /// <param name="col"><see cref="Color"/></param>
        private SkinRarity(string name, int val, Color col, int probabilityValue)
        {
            Name = name;
            Value = val;
            Color = col;
            ProbabilityValue = probabilityValue;
        }

        private static int rarityIndex = 0;

        public static readonly SkinRarity ConsumerGrade = new SkinRarity("Consumer Grade", rarityIndex++, new Color(65, 60, 243), 260);
        public static readonly SkinRarity IndustrialGrade = new SkinRarity("Industrial Grade", rarityIndex++, new Color(69, 141, 217), 260);
        public static readonly SkinRarity Mil_Spec = new SkinRarity("Mil-Spec", rarityIndex++, new Color(65, 60, 243), 260);
        public static readonly SkinRarity Restricted = new SkinRarity("Restricted", rarityIndex++, new Color(65, 60, 243), 160);
        public static readonly SkinRarity Exceptional = new SkinRarity("Exceptional", rarityIndex++, new Color(65, 60, 243), 160);
        public static readonly SkinRarity Classified = new SkinRarity("Classified", rarityIndex++, new Color(211, 43, 159), 32);
        public static readonly SkinRarity Covert = new SkinRarity("Covert", rarityIndex++, new Color(170, 64, 65), 6);
        public static readonly SkinRarity Extraordinary = new SkinRarity("Extraordinary", rarityIndex++, new Color(170, 64, 65), 6);
        public static readonly SkinRarity Contraband = new SkinRarity("Contraband", rarityIndex++, new Color(228, 146, 53), 3);


        private static Dictionary<string, SkinRarity> byString;

        /// <summary>
        /// Returns the rarity corresponding to the string. Possible values are all of the public static fields.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static SkinRarity FromString(string input)
        {
            if(byString == null)
            {
                byString = new Dictionary<string, SkinRarity>();
                Type t = typeof(SkinRarity);
                foreach (var field in t.GetFields())
                {
                    if(field.FieldType == typeof(SkinRarity) && field.IsStatic)
                    {
                        var r = (SkinRarity)field.GetValue(null);
                        byString.Add(r.Name, r);
                    }
                }
            }

            if(byString.ContainsKey(input))
                return byString[input];
            Logger.Log(LogLevel.ERROR, "Unknown rarity '{0}' ecountered while trying to parse object.", input);
            Logger.Log(LogLevel.INFO, "Adding new rarity '{0}' with default color and probability.", input);
            return new SkinRarity(input, rarityIndex++, UnknownRarityColor, UnkownRarityProbabilityValue);
            
            
        }
    }
}
