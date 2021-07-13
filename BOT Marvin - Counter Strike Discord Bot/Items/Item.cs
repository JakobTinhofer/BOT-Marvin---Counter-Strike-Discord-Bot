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
    /// The base for all types of items. Most of them are items also obtainable in csgo or on steam.
    /// </summary>
    public abstract class Item
    {
        #region Equality Operators and similar
        public static bool operator ==(Item a, Item b) {
            if (a is null && b is null)
                return true;
            if (a is null || b is null)
                return false;
            return a.ItemID == b.ItemID;
        }
        public static bool operator !=(Item a, Item b) {
            if (a is null && b is null)
                return false;
            if (a is null || b is null)
                return true;
            return a.ItemID == b.ItemID;
        }

        public override bool Equals(Object a)
        {
            var h = a as Item;
            if (h == null)
                return false;
            return h.ItemID == ItemID;
        }

        public override int GetHashCode()
        {
            return ItemID.GetHashCode();
        }
        #endregion



        /// <summary>
        /// If somehow the link to the item image gets lost in the database, this is the fallback image.
        /// </summary>
        private const string imagePlaceholder = "https://user-images.githubusercontent.com/24848110/33519396-7e56363c-d79d-11e7-969b-09782f5ccbab.png";

        /// <summary>
        /// Calls the constructor responding to the itemtype in the BsonDocument
        /// </summary>
        /// <param name="b">A BsonDocument from the 'items' collection.</param>
        /// <returns>A new, fully loaded Item.</returns>
        private static Item Parse(BsonDocument b)
        {
            switch (b["item-type"].AsString)
            {
                case "Case":
                    return new CaseItem(b);
                    
                case "Skin":
                    return new SkinItem(b);
                    
                default:
                    throw new InvalidOperationException("Unknown item type encountered!");
            }
        }
        
        /// <summary>
        /// Creates the item corresponding to the item object id.
        /// </summary>
        /// <param name="id">The id of the item in the 'items' collection.</param>
        /// <returns>The new Item.</returns>
        private static Item BuildFromID(ObjectId id)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", id);
            return Parse(MongoInterface.db.GetCollection<BsonDocument>("items").Find(filter).FirstOrDefault());
        }

        
        /// <summary>
        /// Fills in the basic information about the item.
        /// </summary>
        /// <param name="b">The BsonDocument from the 'items' collection.</param>
        protected Item(BsonDocument b)
        {
            Name = b["name"].AsString;
            ImageURL = b.GetValue("image", imagePlaceholder).AsString;
            ItemID = b["_id"].AsObjectId;
            Description = b["Description"].AsString;
            if (!cachedItems.ContainsKey(ItemID))
                cachedItems.Add(ItemID, this);
        }

        /// <summary>
        /// Returns whether or not this item is a case, skin or other.
        /// </summary>
        public abstract ItemType getType();

        
        /// <summary>
        /// The name. <see cref="SkinItem"/>s should be represented by <see cref="SkinItem.WeaponName"/> | <see cref="Name"/>
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// A link to the image for this item.
        /// </summary>
        public string ImageURL { get; private set; }

        /// <summary>
        /// This ID is the '_id' field from the 'items' collection and corresponds to the 'item_id' field in the 'skins', 'cases' collections.
        /// </summary>
        public ObjectId ItemID { get; private set; }

        /// <summary>
        /// A short text about the item.
        /// </summary>
        public string Description { get; private set; }

        #region Item Caching

        /// <summary>
        /// All items that have already been parsed from BSON are cached in order to save on unneccessary db lookups.
        /// </summary>
        private static Dictionary<ObjectId, Item> cachedItems = new Dictionary<ObjectId, Item>();

        /// <summary>
        /// All items in the cache are looked up and renewed from the DB data.
        /// </summary>
        public static void RenewAllInCache()
        {
            foreach (var item in cachedItems)
            {
                RenewItem(item.Key);
            }
        }

        /// <summary>
        /// Replaces this item with a newly loaded one from the DB.
        /// </summary>
        /// <param name="id">The '_id' field in the 'items' collection corresponding to the item to renew. Also, <see cref="Item.ItemID"/>.</param>
        public static void RenewItem(ObjectId id)
        {
            if (!cachedItems.ContainsKey(id))
                throw new ArgumentException("This object is not cached");

            cachedItems[id] = BuildFromID(id);
        }
        /// <summary>
        /// Clears all cached items. Does not renew any of them.
        /// </summary>
        public static void ClearCache() => cachedItems.Clear();

        /// <summary>
        /// Checks wether or not this item is cached and only looks up the item in the DB when not cached.
        /// </summary>
        /// <param name="id">The '_id' field in the 'items' collection corresponding to the item to look up. Also, <see cref="Item.ItemID"/>. </param>
        /// <returns>The Item with the id given.</returns>
        public static Item GetFromID(ObjectId id)
        {
            if (cachedItems.ContainsKey(id))
                return cachedItems[id];
            return BuildFromID(id);
        }


        /// <summary>
        /// Checks wether or not this item is cached and only looks up the item in the DB when not cached.
        /// </summary>
        /// <param name="b">A BSON document from the 'items' collection.</param>
        /// <returns>The Item with the id given.</returns>
        public static Item GetFromBSON(BsonDocument b)
        {
            var id = b["_id"].AsObjectId;
            if (cachedItems.ContainsKey(id))
                return cachedItems[id];
            return Parse(b);
        }

        #endregion

    }
}
