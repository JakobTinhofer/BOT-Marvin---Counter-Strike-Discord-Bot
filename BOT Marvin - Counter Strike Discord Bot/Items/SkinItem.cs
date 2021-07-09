using Discord;
using Discord.Rest;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

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
        /// <summary>
        /// How Valuable this skin is. From 0 to 6, higher -> more valuable.
        /// </summary>
        public int Value { get; private set; }

        /// <summary>
        /// The Color corresponding to the rarity.
        /// </summary>
        public Color Color { get; private set; }

        /// <summary>
        /// Enum class constructor
        /// </summary>
        /// <param name="val"><see cref="Value"/></param>
        /// <param name="col"><see cref="Color"/></param>
        private SkinRarity(int val, Color col)
        {
            Value = val;
            Color = col;
        }

        public static readonly SkinRarity ConsumerGrade = new SkinRarity(0, new Color(65, 60, 243));
        public static readonly SkinRarity IndustrialGrade = new SkinRarity(1, new Color(69, 141, 217));
        public static readonly SkinRarity Mil_SpecGrade = new SkinRarity(2, new Color(65, 60, 243));
        public static readonly SkinRarity Restricted = new SkinRarity(3, new Color(65, 60, 243));
        public static readonly SkinRarity Classified = new SkinRarity(4, new Color(211, 43, 159));
        public static readonly SkinRarity Covert = new SkinRarity(5, new Color(170, 64, 65));
        public static readonly SkinRarity Contraband = new SkinRarity(6, new Color(228, 146, 53));


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
                        byString.Add(field.Name, (SkinRarity)field.GetValue(null));
                    }
                }
            }


            return byString[input];

            
            
        }
    }
}
