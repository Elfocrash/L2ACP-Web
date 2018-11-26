/*
 * Copyright (C) 2018 Petr Jasicek
 * This program is free software: you can redistribute it and/or modify it under
 * the terms of the GNU General Public License as published by the Free Software
 * Foundation, either version 2 of the License, or (at your option) any later
 * version.
 * 
 * L2ACP is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE. See the GNU General Public License for more
 * details.
 * 
 * You should have received a copy of the GNU General Public License along with
 * this program. If not, see <http://www.gnu.org/licenses/>.
 */

using L2ACP.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace L2ACP.Services
{
    public class DatabaseServiceHelper
    {
        private static string LIN2WORLD_CONN_STRING = Startup.Configuration.GetValue<string>("ConnectionString_lin2world");
        private static string LIN2USER_CONN_STRING = Startup.Configuration.GetValue<string>("ConnectionString_lin2user");
        private static string LIN2DB_CONN_STRING = Startup.Configuration.GetValue<string>("ConnectionString_lin2db");

        public static async Task<int> GetNumDonatePoints(string accountName)
        {
            int numDonatePoints = -1;
            try
            {
                using (SqlConnection lin2dbDbConn = new SqlConnection(LIN2DB_CONN_STRING))
                {
                    lin2dbDbConn.Open();

                    using (SqlCommand cmd = lin2dbDbConn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT donatepoints FROM user_auth WHERE account = @USER";
                        cmd.Parameters.AddWithValue("@USER", accountName);

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                numDonatePoints = reader.GetInt32(reader.GetOrdinal("donatepoints"));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to execute query. Exception: " + ex.Message);
                numDonatePoints = -1;
            }

            return numDonatePoints;
        }

        public static bool AddDonatePoints(string accountName, int numPoints)
        {
            try
            {
                using (SqlConnection lin2dbDbConn = new SqlConnection(LIN2DB_CONN_STRING))
                {
                    lin2dbDbConn.Open();

                    using (SqlCommand cmd = lin2dbDbConn.CreateCommand())
                    {
                        cmd.CommandText = "UPDATE user_auth SET donatepoints=(donatepoints + @NUM_POINTS) WHERE account=@ACCOUNT_NAME";
                        cmd.Parameters.AddWithValue("@NUM_POINTS", numPoints);
                        cmd.Parameters.AddWithValue("@ACCOUNT_NAME", accountName);
                        if (cmd.ExecuteNonQuery() != 1)
                        {
                            return false;
                        }

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("AddDonatePoints: Failed to execute query. Exception: " + ex.Message);
                return false;
            }
        }

        public static async Task<string> GetAccountNameFromCharName(string charName)
        {
            string accountName = "";
            try
            {
                using (SqlConnection lin2worldDbConn = new SqlConnection(LIN2WORLD_CONN_STRING))
                {
                    lin2worldDbConn.Open();

                    using (SqlCommand cmd = lin2worldDbConn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT account_name FROM user_data WHERE char_name = @CHAR_NAME";
                        cmd.Parameters.AddWithValue("@CHAR_NAME", charName);

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                accountName = reader["account_name"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to execute query. Exception: " + ex.Message);
                accountName = "";
            }

            return accountName;
        }

        public static async Task<int> GetCharIdFromCharName(string charName)
        {
            int charId = -1;
            try
            {
                using (SqlConnection lin2worldDbConn = new SqlConnection(LIN2WORLD_CONN_STRING))
                {
                    lin2worldDbConn.Open();

                    using (SqlCommand cmd = lin2worldDbConn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT char_id FROM user_data WHERE char_name = @CHAR_NAME";
                        cmd.Parameters.AddWithValue("@CHAR_NAME", charName);

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                charId = reader.GetInt32(reader.GetOrdinal("char_id"));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to execute query. Exception: " + ex.Message);
                charId = -1;
            }

            return charId;
        }

        public static async Task<string> GetCharNameFromCharId(int charId)
        {
            string charName = "";
            try
            {
                using (SqlConnection lin2worldDbConn = new SqlConnection(LIN2WORLD_CONN_STRING))
                {
                    lin2worldDbConn.Open();

                    using (SqlCommand cmd = lin2worldDbConn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT char_name FROM user_data WHERE char_id = @CHAR_ID";
                        cmd.Parameters.AddWithValue("@CHAR_ID", charId);

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                charName = reader["char_name"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to execute query. Exception: " + ex.Message);
                charName = "";
            }

            return charName;
        }

        public static async Task<List<DonateService>> GetDonateServicesList()
        {
            List<DonateService> donateServices = new List<DonateService>();
            try
            {
                using (SqlConnection lin2worldDbConn = new SqlConnection(LIN2WORLD_CONN_STRING))
                {
                    lin2worldDbConn.Open();


                    using (SqlCommand cmd = lin2worldDbConn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT * FROM l2acp_donateservices";

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                DonateService donateService = new DonateService();
                                donateService.ServiceId = reader.GetInt32(reader.GetOrdinal("serviceid"));
                                donateService.ServiceName = reader["servicename"].ToString();
                                donateService.Price = reader.GetInt32(reader.GetOrdinal("price"));

                                donateServices.Add(donateService);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                donateServices = null;
            }

            return donateServices;
        }

        public static async Task<DonateService> GetDonateServiceById(int id)
        {
            DonateService donateService = null;
            try
            {
                using (SqlConnection lin2worldDbConn = new SqlConnection(LIN2WORLD_CONN_STRING))
                {
                    lin2worldDbConn.Open();

                    using (SqlCommand cmd = lin2worldDbConn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT * FROM l2acp_donateservices WHERE serviceid=@SERVICE_ID";
                        cmd.Parameters.AddWithValue("@SERVICE_ID", id);

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                donateService = new DonateService();
                                donateService.ServiceId = reader.GetInt32(reader.GetOrdinal("serviceid"));
                                donateService.ServiceName = reader["servicename"].ToString();
                                donateService.Price = reader.GetInt32(reader.GetOrdinal("price"));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                donateService = null;
            }

            return donateService;
        }

        public static async Task<bool> GiveItemToCharacter(string username, int itemId, int itemCount, int enchant)
        {
            int charId = await GetCharIdFromCharName(username);
            if (charId == -1)
            {
                return false;
            }

            // 2) Give character the item
            try
            {
                using (SqlConnection lin2worldDbConn = new SqlConnection(LIN2WORLD_CONN_STRING))
                {
                    lin2worldDbConn.Open();

                    using (SqlCommand cmd = lin2worldDbConn.CreateCommand())
                    {
                        cmd.CommandText = "INSERT INTO ItemDelivery (char_id, item_id, item_amount, enchant)" +
                            "VALUES (@CHAR_ID, @ITEM_ID, @ITEM_AMOUNT, @ENCHANT)";
                        cmd.Parameters.AddWithValue("@CHAR_ID", charId);
                        cmd.Parameters.AddWithValue("@ITEM_ID", itemId);
                        cmd.Parameters.AddWithValue("@ITEM_AMOUNT", itemCount);
                        cmd.Parameters.AddWithValue("@ENCHANT", enchant);
                        if (cmd.ExecuteNonQuery() != 1)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static async Task<List<TradeItemAcp>> GetPrivateStoreItems(bool isBuyList)
        {
            List<TradeItemAcp> items = new List<TradeItemAcp>();

            try
            {
                using (SqlConnection lin2worldDbConn = new SqlConnection(LIN2WORLD_CONN_STRING))
                {
                    lin2worldDbConn.Open();

                    using (SqlCommand cmd = lin2worldDbConn.CreateCommand())
                    {
                        int storeType = isBuyList ? 3 : 1;
                        cmd.CommandText = "SELECT * FROM PrivateStore WHERE store_type=@STORE_TYPE";
                        cmd.Parameters.AddWithValue("@STORE_TYPE", storeType);

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                int charId = reader.GetInt32(reader.GetOrdinal("char_id"));
                                string charName = await GetCharNameFromCharId(charId);
                                for (int itemIdx = 1; itemIdx <= 8; itemIdx++)
                                {
                                    int itemId = reader.GetInt32(reader.GetOrdinal("item" + itemIdx.ToString() + "_id"));
                                    if (itemId > 0)
                                    {
                                        TradeItemAcp item = new TradeItemAcp();
                                        item.ItemId = itemId;
                                        item.Count = reader.GetInt32(reader.GetOrdinal("item" + itemIdx.ToString() +  "_count"));
                                        item.Price = reader.GetInt32(reader.GetOrdinal("item" + itemIdx.ToString() +  "_price"));
                                        item.Enchant = reader.GetInt32(reader.GetOrdinal("item" + itemIdx.ToString() +  "_enchant"));
                                        item.PlayerId = charId;
                                        item.PlayerName = charName;
                                        items.Add(item);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

            return items;
        }

        private async Task<int> GetCharacterTotalOnline(string charName)
        {
            int totalOnline = -1;
            try
            {
                using (SqlConnection lin2worldDbConn = new SqlConnection(LIN2WORLD_CONN_STRING))
                {
                    lin2worldDbConn.Open();

                    using (SqlCommand cmd = lin2worldDbConn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT use_time FROM user_data WHERE char_name=@CHAR_NAME";
                        cmd.Parameters.AddWithValue("@CHAR_NAME", charName);

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                totalOnline = reader.GetInt32(reader.GetOrdinal("use_time"));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to execute query. Exception: " + ex.Message);
                totalOnline = -1;
            }

            return totalOnline;
        }

        public static bool IsCharacterOnline(string charName)
        {
            bool isOnline = false;
            try
            {
                using (SqlConnection lin2worldDbConn = new SqlConnection(LIN2WORLD_CONN_STRING))
                {
                    lin2worldDbConn.Open();

                    using (SqlCommand cmd = lin2worldDbConn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT count(*) FROM user_data with (nolock) WHERE char_name=@CHAR_NAME AND login>logout";
                        cmd.Parameters.AddWithValue("@CHAR_NAME", charName);

                        int count = (int)cmd.ExecuteScalar();
                        if (count == 1)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to execute query. Exception: " + ex.Message);
                isOnline = false;
            }

            return isOnline;
        }

        public static async Task<List<LuckyWheelItem>> GetLuckyWheelListHelper()
        {
            List<LuckyWheelItem> items = new List<LuckyWheelItem>();
            try
            {
                using (SqlConnection lin2worldDbConn = new SqlConnection(LIN2WORLD_CONN_STRING))
                {
                    lin2worldDbConn.Open();

                    using (SqlCommand cmd = lin2worldDbConn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT * FROM l2acp_luckywheelitems";

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                LuckyWheelItem item = new LuckyWheelItem();
                                item.ItemId = reader.GetInt32(reader.GetOrdinal("itemid"));
                                item.Count = reader.GetInt32(reader.GetOrdinal("itemcount"));
                                item.Chance = reader.GetDouble(reader.GetOrdinal("chance"));
                                items.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to execute query. Exception: " + ex.Message);
                items = null;
            }

            return items;
        }

        public static async Task<string> GetClanNameFromPledgeId(int pledgeId)
        {
            if (pledgeId <= 0)
            {
                return "";
            }

            string clanName = "";
            try
            {
                using (SqlConnection lin2worldDbConn = new SqlConnection(LIN2WORLD_CONN_STRING))
                {
                    lin2worldDbConn.Open();

                    using (SqlCommand cmd = lin2worldDbConn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT name FROM Pledge WHERE pledge_id=@PLEDGE_ID";
                        cmd.Parameters.AddWithValue("@PLEDGE_ID", pledgeId);

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                clanName = reader["name"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                clanName = "";
            }

            return clanName;
        }

        public static async Task<string> GetAllianceNameFromPledgeId(int pledgeId)
        {
            if (pledgeId <= 0)
            {
                return "";
            }

            string allianceName = "";
            try
            {
                using (SqlConnection lin2worldDbConn = new SqlConnection(LIN2WORLD_CONN_STRING))
                {
                    lin2worldDbConn.Open();

                    int allianceId = int.MinValue;
                    using (SqlCommand cmd = lin2worldDbConn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT alliance_id FROM Pledge WHERE pledge_id=@PLEDGE_ID";
                        cmd.Parameters.AddWithValue("@PLEDGE_ID", pledgeId);

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                allianceId = reader.GetInt32(reader.GetOrdinal("alliance_id"));
                            }
                        }
                    }

                    if (allianceId != int.MinValue)
                    {
                        using (SqlCommand cmd = lin2worldDbConn.CreateCommand())
                        {
                            cmd.CommandText = "SELECT name FROM Alliance WHERE id=@ALLIANCE_ID";
                            cmd.Parameters.AddWithValue("@ALLIANCE_ID", allianceId);

                            using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                            {
                                if (reader.Read())
                                {
                                    allianceName = reader["name"].ToString();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                allianceName = "";
            }

            return allianceName;
        }
    }
}
