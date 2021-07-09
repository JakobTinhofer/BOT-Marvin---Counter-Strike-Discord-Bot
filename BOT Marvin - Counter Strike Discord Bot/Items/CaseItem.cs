using BOT_Marvin___Counter_Strike_Discord_Bot.Users;
using Discord;
using Discord.Rest;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace BOT_Marvin___Counter_Strike_Discord_Bot.Items
{

    /// <summary>
    /// All items that are containers that can be opened to receive other items
    /// </summary>
    public class CaseItem : Item, IEconomyItem, IDropableItem
    {
        

        

        #region Constructor and Parsing

        /// <summary>
        /// Creates a new CaseItem from the values in this BsonDocument.<br/>
        /// PLEASE NOTE THAT THIS IS RESOURCE INTENSIVE, SINCE IT REQUIRES A DATABASE LOOKUP. Look if the object is cached in <see cref="Item.cachedItems"/>
        /// </summary>
        /// <param name="b">An entry from the "items" collection.</param>
        public CaseItem(BsonDocument b) : base(b)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("item_id", ItemID);
            var caseBSON = MongoInterface.db.GetCollection<BsonDocument>("cases").Find(filter).FirstOrDefault();

            price = caseBSON["price"].AsInt32;
            isActivityDropping = caseBSON["isActivityDropping"].AsBoolean;
            dropValueMultiplier = 1;
            Enum.TryParse<CaseType>(caseBSON["case-type"].AsString, out CaseType t); Type = t;
            Content = ParseContent(caseBSON);
            IsActivated = caseBSON["isOpenable"].AsBoolean;
            RequiresKey = caseBSON["requiresKey"].AsBoolean;
            ModifiersDropped = ParseAvailableModifiers(caseBSON);
        }

        /// <summary>
        /// Parses the ObjectIds in the content field
        /// </summary>
        /// <param name="caseBSON">The BSON Document used to create this item</param>
        /// <returns>A list of <see cref="SkinItem"/>s that are in this case</returns>
        private List<ItemHolder<CaseItem>> ParseContent(BsonDocument caseBSON)
        {
            List<ItemHolder<CaseItem>> conLi = new List<ItemHolder<CaseItem>>();
            var li = caseBSON["content-list"].AsBsonArray.ToList();

            foreach (ObjectId id in li)
            {
                var filter = Builders<BsonDocument>.Filter.Eq("_id", id);
                var skin = MongoInterface.db.GetCollection<BsonDocument>("items").Find(filter).ToList()[0];
                conLi.Add(new ItemHolder<CaseItem>(skin));
            }
            return conLi;
        }

        /// <summary>
        /// Parses the mods like StatTrak, Souvenir, etc.
        /// </summary>
        /// <param name="caseBSON">The case BSON</param>
        /// <returns>A flag enum containing all the modifiers<br/>
        /// (use ParseAvailableModifiers(bson) & SkinModifiers.StatTrak to check if mod contained)</returns>
        private SkinModifier ParseAvailableModifiers(BsonDocument caseBSON)
        {
            SkinModifier mods = SkinModifier.Normal;

            var li = caseBSON["availableModifiers"].AsBsonArray.ToList();

            foreach(string mod in li)
            {
                Enum.TryParse<SkinModifier>(mod, out SkinModifier modEnum);
                mods = mods | modEnum;
            }

            return mods;
        }

        #endregion

        #region Private Members

        private double dropValueMultiplier;
        private bool isActivityDropping;
        private int price;

        #endregion

        #region Public Properties

        /// <summary>
        /// Inherited from <see cref="Item"/>
        /// </summary>
        /// <returns><see cref="ItemType.Case"/></returns>
        public override ItemType getType() { return ItemType.Case; }

        /// <summary>
        /// Inherited from <see cref="IEconomyItem"/>. The price of the case according to the data source. (1€ = 100)
        /// </summary>
        /// <returns>The price of the case</returns>
        public int GetPrice() { return price; }

        /// <summary>
        /// Inherited from <see cref="IDropableItem"/>. Cases only drop from activity when this is true.
        /// </summary>
        /// <returns></returns>
        public bool IsActivityDropping() { return isActivityDropping; }

        /// <summary>
        /// Inherited from <see cref="IDropableItem"/>. The value (and therefore the probability) of a drop is determined by <see cref="GetPrice"/> * <see cref="GetDropValueMultiplier"/>
        /// </summary>
        /// <returns></returns>
        public double GetDropValueMultiplier() { return dropValueMultiplier; }

        /// <summary>
        /// The type of case this item is.
        /// </summary>
        public CaseType Type { get; private set; }

        /// <summary>
        /// A list of all skin items contained.
        /// </summary>
        public List<ItemHolder<CaseItem>> Content { get; private set; } = new List<ItemHolder<CaseItem>>();

        /// <summary>
        /// Is openable.
        /// </summary>
        public bool IsActivated { get; private set; }
        /// <summary>
        /// If you need a key, it needs to be payed extra.
        /// </summary>
        public bool RequiresKey { get; private set; }

        /// <summary>
        /// All the types of modifiers that the skins dropped from this case can have
        /// </summary>
        public SkinModifier ModifiersDropped { get; private set; }
        #endregion
    }

    /// <summary>
    /// Different types of skin containers
    /// </summary>
    public enum CaseType
    {
        WeaponCase,
        SouvenirPackage
    }
}
