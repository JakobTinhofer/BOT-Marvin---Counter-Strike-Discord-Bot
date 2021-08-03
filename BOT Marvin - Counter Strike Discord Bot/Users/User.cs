using BOT_Marvin___Counter_Strike_Discord_Bot.Items;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace BOT_Marvin___Counter_Strike_Discord_Bot.Users
{
    /// <summary>
    /// Represents a user in order to keep track of owned items, coins, etc. Can be found in the 'users' collection
    /// </summary>
    public class User
    {
        #region Constructors and Parsers
        /// <summary>
        /// Creates a User from a Bson document out of the 'users' collection. Faster than <see cref="User"/>(<see cref="ulong"/> id).
        /// </summary>
        /// <param name="b"></param>
        private User(BsonDocument b) => initFromBson(b);

        /// <summary>
        /// Creates a new user from an id. If the id already exists in the 'users' collection, data is read from there. Else, a new entry is created.
        /// </summary>
        /// <param name="id">The 'user_id' field of the 'users' collection. Also, the <see cref="SocketUser.Id"/> id.</param>
        private User(ulong id)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("user_id", id);
            var t = MongoInterface.db.GetCollection<BsonDocument>("users").Find(filter).ToList();
            if(t.Count > 0)
            {
                initFromBson(t[0]);
            }
            else
            {
                BsonDocument b = new BsonDocument();
                b["user_id"] = new BsonInt64((long)id);
                UserID = id;
                b["inventory"] = new BsonArray();
                Inventory = new Dictionary<ItemHolder<Item>, int>();
                b["coins"] = new BsonInt64(0);
                Coins = 0;
                MongoInterface.db.GetCollection<BsonDocument>("users").InsertOne(b);
            }
            
            
        }

        /// <summary>
        /// Parses the json data in the given document
        /// </summary>
        /// <param name="b"></param>
        private void initFromBson(BsonDocument b)
        {
            UserID = (ulong)b["user_id"].ToInt64();
            ParseInventory(b);
            Coins = b["coins"].AsInt64;
        }

        /// <summary>
        /// Parses the player inventory from the given item ids
        /// </summary>
        /// <param name="b"></param>
        private void ParseInventory(BsonDocument b)
        {
            var li = b["inventory"].AsBsonArray.ToList();

            
            foreach (ObjectId id in li)
            {
                var itm = new ItemHolder<Item>(id);
                if (!Inventory.ContainsKey(itm))
                    Inventory.Add(itm, 1);
                else
                    Inventory[itm] += 1;
            }
        }
        #endregion
        /// <summary>
        /// The user id. Same as the discord user id.
        /// </summary>
        public ulong UserID { get; private set; }

        #region Caching

        private static Dictionary<ulong, User> usersById = new Dictionary<ulong, User>();

        /// <summary>
        /// Creates a new user from the <see cref="UserID"/>. If the user has already been created, a reference to the already known instance is returned in order to save on database lookups.
        /// </summary>
        public static User FromID(ulong userID)
        {
            if (!usersById.ContainsKey(userID))
                usersById[userID] = new User(userID);
            
            return usersById[userID];
        }

        #endregion

        #region Manipulatable Properties

        private long _coins;
        /// <summary>
        /// The number of coins the player owns. 100 Coins = 1€. Changing this will also lead to a database update.
        /// </summary>
        public long Coins { get { return _coins; } set {
                _coins = value;
                UpdateDB();
            }
        
        }


        /// <summary>
        /// The Inventory with the item and the number of items owned. Corresponds to the field "user_id" in the users collection.
        /// </summary>
        private Dictionary<ItemHolder<Item>, int> Inventory { get; set; } = new Dictionary<ItemHolder<Item>, int>();

        /// <summary>
        /// Returns the number of times a user owns a item.
        /// </summary>
        /// <param name="itemId">The ID of the item to be counted.</param>
        /// <returns></returns>
        public int CountItem(ObjectId itemId)
        {
            var h = new ItemHolder<Item>(itemId);
            if (!Inventory.ContainsKey(h))
                return 0;
            return Inventory[h];
        }

        /// <summary>
        /// Adds one of the given item to the player inventory.
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(ObjectId itemID, int count = 1)
        {
            if (count < 1)
                throw new InvalidOperationException("Cannot add less than 1 item!");

            ItemHolder<Item> ih = new ItemHolder<Item>(itemID);
            if (Inventory.ContainsKey(ih))
                Inventory[ih] += count;
            else
                Inventory.Add(ih, count);
            UpdateDB();
        }

        public List<ItemHolder<Item>> GetItems()
        {
            List<ItemHolder<Item>> l = new List<ItemHolder<Item>>();
            foreach (var pair in Inventory)
            {
                for (int i = 0; i < pair.Value; i++)
                {
                    l.Add(pair.Key);
                }
            }
            return l;
        }


        /// <summary>
        /// Removes an item from the player inventory. 
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="count"></param>
        public void RemoveItem(ObjectId itemID, int count = 1)
        {
            if (count < 0)
                count *= -1;

            ItemHolder<Item> ih = new ItemHolder<Item>(itemID);
            if (!Inventory.ContainsKey(ih))
                throw new InvalidOperationException("There is no item with that objectID in the player inventory!");
            if (Inventory[ih] - count < 0)
                throw new InvalidOperationException("There are not enough items to remove " + count + " of them!");
            else if (Inventory[ih] - count == 0)
                Inventory.Remove(ih);
            else
                Inventory[ih] -= count;
        }

        #endregion

        #region Database updating
        private BsonArray getIdArray()
        {
            BsonArray arr = new BsonArray();
            foreach (var keyValue in Inventory)
            {
                for (int i = keyValue.Value; i > 0; i--)
                {
                    arr.Add(keyValue.Key.Get().ItemID);
                }
                
            }
            return arr;
        }

        private void UpdateDB()
        {
            var filter = Builders<BsonDocument>.Filter.Eq("user_id", UserID);
            var update = Builders<BsonDocument>.Update.Set("coins", Coins).Set("inventory", getIdArray());
            var col = MongoInterface.db.GetCollection<BsonDocument>("users");
            col.UpdateOneAsync(filter, update);
        }
        #endregion
    }
}
