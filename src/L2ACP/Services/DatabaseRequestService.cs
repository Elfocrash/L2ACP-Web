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

using System.Net.Http;
using System.Threading.Tasks;
using L2ACP.Extensions;
using L2ACP.Models;
using L2ACP.Requests;
using L2ACP.Responses;
using Newtonsoft.Json;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;

namespace L2ACP.Services
{
    public class DatabaseRequestService : IRequestService
    {
        private static string LIN2WORLD_CONN_STRING = Startup.Configuration.GetValue<string>("ConnectionString_lin2world");
        private static string LIN2USER_CONN_STRING = Startup.Configuration.GetValue<string>("ConnectionString_lin2user");
        private static string LIN2DB_CONN_STRING = Startup.Configuration.GetValue<string>("ConnectionString_lin2db");

        public async Task<L2Response> LoginUser(string username, string password)
        {
            L2Response response = new L2Response();
            try
            {
                using (SqlConnection lin2dbDbConn = new SqlConnection(LIN2DB_CONN_STRING))
                {
                    lin2dbDbConn.Open();

                    using (SqlCommand cmd = lin2dbDbConn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT * FROM user_auth WHERE account = @USER";
                        cmd.Parameters.AddWithValue("@USER", username);

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                string dbPasswordHash = GeneralExtensions.ByteArrayToString((byte[])reader["password"]);
                                if (password == dbPasswordHash)
                                {
                                    System.Diagnostics.Debug.WriteLine("Matching passwords !");
                                    response.ResponseCode = 200;
                                    response.ResponseMessage = "OK";
                                }
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine("NOT MATCHING PASSWORDS !");

                                    response.ResponseCode = 500;
                                    response.ResponseMessage = "INVALID PASSWORD";
                                }
                            }
                            else
                            {
                                response.ResponseCode = 500;
                                response.ResponseMessage = "USER NOT FOUND";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to execute query. Exception: " + ex.Message);
                response.ResponseCode = 500;
                response.ResponseMessage = ex.Message;
            }

            return response;
        }

        public static byte[] ConvertHexStringToByteArray(string hexString)
        {
            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "The binary key cannot have an odd number of digits: {0}", hexString));
            }

            byte[] HexAsBytes = new byte[hexString.Length / 2];
            for (int index = 0; index < HexAsBytes.Length; index++)
            {
                string byteValue = hexString.Substring(index * 2, 2);
                HexAsBytes[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return HexAsBytes;
        }

        public async Task<L2Response> RegisterUser(string username, string password)
        {
            L2Response response = new L2Response();
            try
            {
                using (SqlConnection lin2dbDbConn = new SqlConnection(LIN2DB_CONN_STRING))
                {
                    lin2dbDbConn.Open();

                    using (SqlCommand cmd = lin2dbDbConn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT * FROM user_auth WHERE account = @USER";
                        cmd.Parameters.AddWithValue("@USER", username);

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                response.ResponseCode = 1000;
                                response.ResponseMessage = "Account with username: " + username + " already exists";
                                return response;
                            }
                        }
                    }

                    using (SqlCommand cmd = lin2dbDbConn.CreateCommand())
                    {
                        cmd.CommandText = "INSERT INTO user_auth (account, password, quiz1, quiz2, answer1, answer2) values (@USERNAME, @PASSWORD, @QUIZ1, @QUIZ2, @ANSWER1, @ANSWER2)";
                        cmd.Parameters.AddWithValue("@USERNAME", username);
                        cmd.Parameters.AddWithValue("@PASSWORD", ConvertHexStringToByteArray(password));
                        cmd.Parameters.AddWithValue("@QUIZ1", "");
                        cmd.Parameters.AddWithValue("@QUIZ2", "");
                        cmd.Parameters.AddWithValue("@ANSWER1", ConvertHexStringToByteArray("00"));
                        cmd.Parameters.AddWithValue("@ANSWER2", ConvertHexStringToByteArray("00"));

                        if (cmd.ExecuteNonQuery() != 1)
                        {
                            response.ResponseCode = 500;
                            response.ResponseMessage = "Unsuccessful registration";
                            return response;
                        }
                    }

                    using (SqlCommand cmd = lin2dbDbConn.CreateCommand())
                    {
                        cmd.CommandText = "INSERT INTO user_account (account, pay_stat) values (@USERNAME, 1)";
                        cmd.Parameters.AddWithValue("@USERNAME", username);

                        cmd.ExecuteNonQuery();
                    }

                    response.ResponseCode = 200;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to execute query. Exception: " + ex.Message);
                response.ResponseCode = 500;
                response.ResponseMessage = ex.Message;
            }

            System.Diagnostics.Debug.WriteLine("Registration. Reponse code: " + response.ResponseCode + ", Response message: " + response.ResponseMessage);

            return response;
        }

        public async Task<L2Response> GetAccountInfo(string username)
        {
            GetAccountInfoResponse response = new GetAccountInfoResponse();
            response.DonatePoints = 0;
            response.AccessLevel = 0;
            try
            {
                using (SqlConnection lin2worldDbConn = new SqlConnection(LIN2WORLD_CONN_STRING))
                {
                    lin2worldDbConn.Open();

                    using (SqlCommand cmd = lin2worldDbConn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT * FROM user_data WHERE account_name = @USER";
                        cmd.Parameters.AddWithValue("@USER", username);

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            List<string> charNames = new List<string>();
                            while (reader.Read())
                            {
                                charNames.Add(reader["char_name"].ToString());
                            }
                            response.AccountNames = charNames.ToArray();
                        }
                    }

                    using (SqlCommand cmd = lin2worldDbConn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT * FROM builder_account WHERE account_name = @USER";
                        cmd.Parameters.AddWithValue("@USER", username);

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                response.AccessLevel = reader.GetInt32(reader.GetOrdinal("default_builder"));
                            }
                        }
                    }

                    response.ResponseCode = 200;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to execute query. Exception: " + ex.Message);
                response.ResponseCode = 500;
                response.ResponseMessage = ex.Message;
            }

            response.DonatePoints = await DatabaseServiceHelper.GetNumDonatePoints(username);
            if (response.DonatePoints == -1)
            {
                response.ResponseCode = 500;
                response.ResponseMessage = "Failed get donate points from account: " + username;
                System.Diagnostics.Debug.WriteLine(response.ResponseMessage);
            }

            System.Diagnostics.Debug.WriteLine("ReponseCode: " + response.ResponseCode + ", AccessLevel: " + response.AccessLevel);

            return response;
        }

        public async Task<L2Response> GetInventory(string player)
        {
            GetInventoryResponse response = new GetInventoryResponse();
            try
            {
                using (SqlConnection lin2worldDbConn = new SqlConnection(LIN2WORLD_CONN_STRING))
                {
                    lin2worldDbConn.Open();

                    // First get character id from character (player) name
                    int charId = -1;
                    using (SqlCommand cmd = lin2worldDbConn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT char_id FROM user_data WHERE char_name = @CHARNAME";
                        cmd.Parameters.AddWithValue("@CHARNAME", player);

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                charId = reader.GetInt32(reader.GetOrdinal("char_id"));
                            }
                        }
                    }

                    if (charId == -1)
                    {
                        response.ResponseCode = 500;
                        response.ResponseMessage = "Could not find character ID for player: " + player;
                        return response;
                    }

                    using (SqlCommand cmd = lin2worldDbConn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT * FROM user_item WHERE char_id = @CHAR_ID";
                        cmd.Parameters.AddWithValue("@CHAR_ID", charId);

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            List<InventoryInfo> inventoryList = new List<InventoryInfo>();
                            while (reader.Read())
                            {
                                InventoryInfo item = new InventoryInfo();
                                item.ObjectId = reader.GetInt32(reader.GetOrdinal("item_id"));
                                item.ItemId = reader.GetInt32(reader.GetOrdinal("item_type"));
                                item.ItemCount = reader.GetInt32(reader.GetOrdinal("amount"));
                                item.Equipped = false; // This is not in L2Off database (?)
                                item.Enchant = reader.GetInt32(reader.GetOrdinal("enchant"));

                                inventoryList.Add(item);
                            }
                            response.InventoryInfo = inventoryList.ToArray();
                        }
                    }

                    response.ResponseCode = 200;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to execute query. Exception: " + ex.Message);
                response.ResponseCode = 500;
                response.ResponseMessage = ex.Message;
            }

            System.Diagnostics.Debug.WriteLine("GetInventory. ReponseCode: " + response.ResponseCode + ", InventoryItemCount: " + response.InventoryInfo.Length);

            return response;
        }

        public async Task<L2Response> GetPlayerInfo(string playerName)
        {
            GetPlayerInfoResponse response = new GetPlayerInfoResponse();
            try
            {
                using (SqlConnection lin2worldDbConn = new SqlConnection(LIN2WORLD_CONN_STRING))
                {
                    lin2worldDbConn.Open();
                    using (SqlCommand cmd = lin2worldDbConn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT * FROM user_data WHERE char_name = @CHAR_NAME";
                        cmd.Parameters.AddWithValue("@CHAR_NAME", playerName);

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            response.PlayerInfo = new PlayerInfo();
                            if (reader.Read())
                            {
                                response.PlayerInfo.Name = reader["char_name"].ToString();
                                response.PlayerInfo.Title = "";
                                response.PlayerInfo.Level = reader.GetByte(reader.GetOrdinal("Lev"));
                                response.PlayerInfo.Pvp = reader.GetInt32(reader.GetOrdinal("Duel"));
                                response.PlayerInfo.Pk = reader.GetInt32(reader.GetOrdinal("PK"));
                                response.PlayerInfo.Sex = (int)reader.GetByte(reader.GetOrdinal("gender"));
                                response.PlayerInfo.Race = (int)reader.GetByte(reader.GetOrdinal("class"));
                                int pledgeId = reader.GetInt32(reader.GetOrdinal("pledge_id"));
                                response.PlayerInfo.ClanName = await DatabaseServiceHelper.GetClanNameFromPledgeId(pledgeId); 
                                response.PlayerInfo.AllyName = await DatabaseServiceHelper.GetAllianceNameFromPledgeId(pledgeId);
                                response.PlayerInfo.Hero = false; // Can be probably retrieved from user_nobless
                                response.PlayerInfo.Nobless = false; // Can be probably retrieved from user_nobless
                                response.PlayerInfo.Time = reader.GetInt32(reader.GetOrdinal("use_time"));
                            }
                            else
                            {
                                response.ResponseCode = 500;
                                response.ResponseMessage = "Could not find character with name: " + playerName;
                                return response;
                            }
                            System.Diagnostics.Debug.WriteLine("5");
                        }
                    }
                    response.ResponseCode = 200;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to execute query. Exception: " + ex.Message);
                response.ResponseCode = 500;
                response.ResponseMessage = ex.Message;
            }

            System.Diagnostics.Debug.WriteLine("GetPlayerInfo ReponseCode: " + response.ResponseCode);

            return response;
        }

        public async Task<L2Response> EnchantItem(string playerName, int objId, int itemEnch)
        {
            var enchantRequest = new EnchantItemRequest
            {
                ObjectId = objId,
                Username = playerName,
                Enchant = itemEnch
            };
            var responseObject = await enchantRequest.SendPostRequest<L2Response>();

            return responseObject;
        }

        public async Task<L2Response> SendDonation(string accountName, int amount, string transactionId, string verifySign)
        {
            L2Response response = new L2Response();
            try
            {
                using (SqlConnection lin2worldDbConn = new SqlConnection(LIN2WORLD_CONN_STRING))
                {
                    lin2worldDbConn.Open();

                    // 1. Check if the entry is already there
                    using (SqlCommand cmd = lin2worldDbConn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT COUNT(*) FROM l2acp_donations " +
                            "WHERE accountName = @ACCOUNT_NAME and transactionId = @TRANSACTION_ID and verificationSign = @VERIFICATION_SIGN";
                        cmd.Parameters.AddWithValue("@ACCOUNT_NAME", accountName);
                        cmd.Parameters.AddWithValue("@TRANSACTION_ID", transactionId);
                        cmd.Parameters.AddWithValue("@VERIFICATION_SIGN", verifySign);

                        int count = (int)await cmd.ExecuteScalarAsync();
                        if (count > 0)
                        {
                            response.ResponseCode = 500;
                            response.ResponseMessage = "Donation already exists";
                            return response;
                        }
                    }

                    // 2. Create the donation entry
                    using (SqlCommand cmd = lin2worldDbConn.CreateCommand())
                    {
                        cmd.CommandText = "INSERT INTO l2acp_donations (accountName, amount, tranactionId, verificationSign) values (@ACCOUNT_NAME, @AMOUNT, @TRANSACTION_ID, @VERIFICATION_SIGN)";
                        cmd.Parameters.AddWithValue("@ACCOUNT_NAME", accountName);
                        cmd.Parameters.AddWithValue("@AMOUNT", amount);
                        cmd.Parameters.AddWithValue("@TRANSACTION_ID", transactionId);
                        cmd.Parameters.AddWithValue("@VERIFICATION_SIGN", verifySign);
                        if (cmd.ExecuteNonQuery() != 1)
                        {
                            response.ResponseCode = 500;
                            response.ResponseMessage = "Failed to insert donation entry for account name: " + accountName;
                            return response;
                        }
                    }

                    // 3. Add it to account's donation points total
                    if (!DatabaseServiceHelper.AddDonatePoints(accountName, amount))
                    {
                        response.ResponseCode = 500;
                        response.ResponseMessage = "Failed to update points for account: " + accountName;
                        return response;
                    }

                    response.ResponseCode = 200;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to execute query. Exception: " + ex.Message);
                response.ResponseCode = 500;
                response.ResponseMessage = ex.Message;
            }

            return response;
        }

        public async Task<L2Response> ChangePassword(string username, string currentPass, string newPass)
        {
            var response = new L2Response();
            try
            {
                using (SqlConnection lin2dbDbConn = new SqlConnection(LIN2DB_CONN_STRING))
                {
                    lin2dbDbConn.Open();

                    // 1. Verify that the account exists and that old password matches
                    using (SqlCommand cmd = lin2dbDbConn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT password FROM user_auth WHERE account = @USER";
                        cmd.Parameters.AddWithValue("@USER", username);

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                string currentDbPassword = GeneralExtensions.ByteArrayToString((byte[])reader["password"]);
                                if (currentPass != currentDbPassword)
                                {
                                    System.Diagnostics.Debug.WriteLine("Passwords do not match !");
                                    response.ResponseCode = 501;
                                    return response;
                                }
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("Failed to retrieve account's password for ChangePassword service. Username: " + username);
                                response.ResponseCode = 500;
                                return response;
                            }
                        }
                    }

                    // 2. Update the password

                    using (SqlCommand cmd = lin2dbDbConn.CreateCommand())
                    {
                        cmd.CommandText = "UPDATE user_auth SET password=@NEW_PASSWORD WHERE account=@ACCOUNT_NAME";
                        cmd.Parameters.AddWithValue("@NEW_PASSWORD", ConvertHexStringToByteArray(newPass));
                        cmd.Parameters.AddWithValue("@ACCOUNT_NAME", username);

                        if (cmd.ExecuteNonQuery() != 1)
                        {
                            System.Diagnostics.Debug.WriteLine("Failed to update password");
                            response.ResponseCode = 500;
                            return response;
                        }
                    }

                    response.ResponseCode = 200;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to execute query. Exception: " + ex.Message);
                response.ResponseCode = 500;
                response.ResponseMessage = ex.Message;
            }

            return response;
        }

        public async Task<L2Response> GetBuyList()
        {
            GetBuyListResponse response = new GetBuyListResponse();
            try
            {
                using (SqlConnection lin2worldDbConn = new SqlConnection(LIN2WORLD_CONN_STRING))
                {
                    lin2worldDbConn.Open();

                    using (SqlCommand cmd = lin2worldDbConn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT * FROM l2acp_donateitems";

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            List<BuyListItem> donateBuyList = new List<BuyListItem>();
                            while (reader.Read())
                            {
                                BuyListItem donateItem = new BuyListItem();
                                donateItem.ItemId = reader.GetInt32(reader.GetOrdinal("itemId"));
                                donateItem.ItemCount = reader.GetInt32(reader.GetOrdinal("itemCount"));
                                donateItem.Enchant = reader.GetInt32(reader.GetOrdinal("enchant"));
                                donateItem.Price = reader.GetInt32(reader.GetOrdinal("price"));

                                donateBuyList.Add(donateItem);
                            }
                            response.BuyList = donateBuyList.ToArray();
                        }
                    }

                    response.ResponseCode = 200;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to execute query. Exception: " + ex.Message);
                response.ResponseCode = 500;
                response.ResponseMessage = ex.Message;
            }

            return response;
        }

        public async Task<L2Response> GetBuyPrivateStoreList()
        {
            GetBuyPrivateStoreItemsResponse response = new GetBuyPrivateStoreItemsResponse();

            response.BuyList = (await DatabaseServiceHelper.GetPrivateStoreItems(true)).ToArray();

            return response;
        }

        public async Task<L2Response> GetSellPrivateStoreList()
        {
            GetSellPrivateStoreItemsResponse response = new GetSellPrivateStoreItemsResponse();

            response.SellList = (await DatabaseServiceHelper.GetPrivateStoreItems(false)).ToArray();

            return response;
        }

        public async Task<L2Response> BuyItem(string accountName, string modelUsername, int modelItemId, int modelItemCount, int modelEnchant,
            int modelPrice)
        {
            L2Response response = new L2Response();

            int numDonatePoints = await DatabaseServiceHelper.GetNumDonatePoints(accountName);
            if (numDonatePoints == -1)
            {
                response.ResponseCode = 500;
                return response;
            }

            if (numDonatePoints < modelPrice)
            {
                response.ResponseCode = 501;
                return response;
            }

            if (await DatabaseServiceHelper.GiveItemToCharacter(modelUsername, modelItemId, modelItemCount, modelEnchant))
            {
                response.ResponseCode = 200;

                DatabaseServiceHelper.AddDonatePoints(accountName, -1 * modelPrice);
            }
            else
            {
                response.ResponseCode = 500;
            }

            return response;
        }

        public async Task<L2Response> SellPrivateStoreItem(int objectId, int buyerId, int count, string sellerName)
        {
            var sellRequest = new SellPrivateStoreItemRequest
            {
                ObjectId = objectId,
                BuyerId = buyerId,
                Count = count,
                SellerName = sellerName
            };

            var responseObject = await sellRequest.SendPostRequest<L2Response>();
            return responseObject;
        }

        public async Task<L2Response> BuyPrivateStoreItem(int objectId, int sellerId, int count, string buyerName)
        {
            var sellRequest = new BuyPrivateStoreItemRequest
            {
                ObjectId = objectId,
                SellerId = sellerId,
                Count = count,
                BuyerName = buyerName
            };

            var responseObject = await sellRequest.SendPostRequest<L2Response>();
            return responseObject;
        }

        public async Task<L2Response> GiveItem(string username, int itemId, int itemCount, int enchant)
        {
            L2Response response = new L2Response();
            if (await DatabaseServiceHelper.GiveItemToCharacter(username, itemId, itemCount, enchant))
            {
                response.ResponseCode = 200;
            }
            else
            {
                response.ResponseCode = 500;
            }

            return response;
        }

        public async Task<L2Response> AnnounceTextAsync(string text)
        {
            // TODO
            var responseObject = await new AnnounceRequest(text).SendPostRequest<L2Response>();
            return responseObject;
        }

        public async Task<L2Response> GiveDonatePoints(string playerName, int donatePoints)
        {
            var response = new L2Response();
            try
            {
                string accountName = "";
                // 1. Get the account name for specified player
                using (SqlConnection lin2worldDbConn = new SqlConnection(LIN2WORLD_CONN_STRING))
                {
                    lin2worldDbConn.Open();
                    
                    using (SqlCommand cmd = lin2worldDbConn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT account_name FROM user_data WHERE char_name=@CHAR_NAME";
                        cmd.Parameters.AddWithValue("@CHAR_NAME", playerName);

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                accountName = reader["account_name"].ToString();
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("Failed to retrieve account name based on character name: " + playerName);
                                response.ResponseCode = 500;
                                return response;
                            }
                        }
                    }
                }

                // 2. Give donate points
                if (!DatabaseServiceHelper.AddDonatePoints(accountName, donatePoints))
                {
                    response.ResponseCode = 500;
                    response.ResponseMessage = "Failed to update donate points";
                    return response;
                }

                response.ResponseCode = 200;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to execute query. Exception: " + ex.Message);
                response.ResponseCode = 500;
                response.ResponseMessage = ex.Message;
            }

            return response;
        }

        public async Task<L2Response> SetPlayerLevel(string playerName, int level)
        {
            var response = new L2Response();
            try
            {
                using (SqlConnection lin2worldDbConn = new SqlConnection(LIN2WORLD_CONN_STRING))
                {
                    lin2worldDbConn.Open();

                    using (SqlCommand cmd = lin2worldDbConn.CreateCommand())
                    {
                        cmd.CommandText = "UPDATE user_data SET Lev=@LEVEL WHERE char_name=@CHAR_NAME";
                        cmd.Parameters.AddWithValue("@LEVEL", (byte)level);
                        cmd.Parameters.AddWithValue("@CHAR_NAME", playerName);
                        if (await cmd.ExecuteNonQueryAsync() != 1)
                        {
                            response.ResponseCode = 500;
                            return response;
                        }
                    }
                }

                response.ResponseCode = 200;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to execute query. Exception: " + ex.Message);
                response.ResponseCode = 500;
                response.ResponseMessage = ex.Message;
            }

            return response;
        }

        public async Task<L2Response> GetTopStats()
        {
            GetStatsResponse response = new GetStatsResponse();

            try
            {
                using (SqlConnection lin2worldDbConn = new SqlConnection(LIN2WORLD_CONN_STRING))
                {
                    lin2worldDbConn.Open();

                    // Top PvP
                    using (SqlCommand cmd = lin2worldDbConn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT TOP 20 char_name, Lev, daily_pvp, PK, use_time, class FROM user_data ORDER BY daily_pvp DESC";

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            List<TopPlayer> topPvpPlayers = new List<TopPlayer>();
                            while (reader.Read())
                            {
                                TopPlayer plr = new TopPlayer();
                                plr.CharName = reader["char_name"].ToString();
                                plr.Level = reader.GetByte(reader.GetOrdinal("Lev"));
                                plr.PvpKills = reader.GetInt32(reader.GetOrdinal("daily_pvp"));
                                plr.PkKills = reader.GetInt32(reader.GetOrdinal("PK"));
                                plr.OnlineTime = reader.GetInt32(reader.GetOrdinal("use_time"));
                                plr.Online = DatabaseServiceHelper.IsCharacterOnline(plr.CharName) ? 1 : 0;

                                topPvpPlayers.Add(plr);
                            }
                            response.TopPvp = topPvpPlayers.ToArray();
                        }
                    }

                    // Top PK
                    using (SqlCommand cmd = lin2worldDbConn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT TOP 20 char_name, Lev, daily_pvp, PK, use_time, class FROM user_data ORDER BY PK DESC";

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            List<TopPlayer> topPkPlayers = new List<TopPlayer>();
                            while (reader.Read())
                            {
                                TopPlayer plr = new TopPlayer();
                                plr.CharName = reader["char_name"].ToString();
                                plr.Level = reader.GetByte(reader.GetOrdinal("Lev"));
                                plr.PvpKills = reader.GetInt32(reader.GetOrdinal("daily_pvp"));
                                plr.PkKills = reader.GetInt32(reader.GetOrdinal("PK"));
                                plr.OnlineTime = reader.GetInt32(reader.GetOrdinal("use_time"));
                                plr.Online = DatabaseServiceHelper.IsCharacterOnline(plr.CharName) ? 1 : 0;

                                topPkPlayers.Add(plr);
                            }
                            response.TopPk = topPkPlayers.ToArray();
                        }
                    }

                    // Top Online
                    using (SqlCommand cmd = lin2worldDbConn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT TOP 20 char_name, Lev, daily_pvp, PK, use_time, class FROM user_data ORDER BY use_time DESC";

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            List<TopPlayer> topOnlinePlayers = new List<TopPlayer>();
                            while (reader.Read())
                            {
                                TopPlayer plr = new TopPlayer();
                                plr.CharName = reader["char_name"].ToString();
                                plr.Level = reader.GetByte(reader.GetOrdinal("Lev"));
                                plr.PvpKills = reader.GetInt32(reader.GetOrdinal("daily_pvp"));
                                plr.PkKills = reader.GetInt32(reader.GetOrdinal("PK"));
                                plr.OnlineTime = reader.GetInt32(reader.GetOrdinal("use_time"));
                                
                                plr.Online = DatabaseServiceHelper.IsCharacterOnline(plr.CharName) ? 1 : 0;

                                topOnlinePlayers.Add(plr);
                            }
                            response.TopOnline = topOnlinePlayers.ToArray();
                        }
                    }

                    System.Diagnostics.Debug.WriteLine("-------------- OK !!!");

                    response.ResponseCode = 200;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to execute query. Exception: " + ex.Message);
                response.ResponseCode = 500;
                response.ResponseMessage = ex.Message;
            }

            return response;
        }

        public async Task<L2Response> GetDonateServices()
        {
            GetDonateServicesResponse response = new GetDonateServicesResponse();

            List<DonateService> donateServicesList = await DatabaseServiceHelper.GetDonateServicesList();
            if (donateServicesList == null)
            {
                response.ResponseCode = 500;
            }
            else
            {
                response.DonateServices = donateServicesList.ToArray();
                response.ResponseCode = 200;
            }

            return response;
        }

        public async Task<L2Response> RenamePlayer(string playerName, string newName)
        {
            var response = new L2Response();

            if (playerName == newName)
            {
                response.ResponseCode = 501;
                return response;
            }
            if (await DatabaseServiceHelper.GetCharIdFromCharName(newName) != -1)
            {
                // Player already exists
                response.ResponseCode = 502;
                return response;
            }

            DonateService renameService = await DatabaseServiceHelper.GetDonateServiceById(1);
            if (renameService == null)
            {
                response.ResponseCode = 503;
                return response;
            }

            if (renameService.Price < 0)
            {
                // Service is disabled
                response.ResponseCode = 503;
                return response;
            }

            string accountName = await DatabaseServiceHelper.GetAccountNameFromCharName(playerName);
            int numDonatePoints = await DatabaseServiceHelper.GetNumDonatePoints(accountName);
            if (numDonatePoints == -1)
            {
                response.ResponseCode = 503;
                return response;
            }
            else if (numDonatePoints < renameService.Price)
            {
                response.ResponseCode = 504;
                return response;
            }

            // Check name validity
            if (newName.Length > 25 || newName.Length < 3)
            {
                response.ResponseCode = 500;
                return response;
            }
            else if (!char.IsLetter(newName[0]))
            {
                response.ResponseCode = 500;
                return response;
            }
            else if (!Regex.IsMatch(newName, @"^[a-zA-Z0-9]+$"))
            {
                response.ResponseCode = 500;
                return response;
            }

            try
            {
                using (SqlConnection lin2worldDbConn = new SqlConnection(LIN2WORLD_CONN_STRING))
                {
                    lin2worldDbConn.Open();

                    using (SqlCommand cmd = lin2worldDbConn.CreateCommand())
                    {
                        cmd.CommandText = "UPDATE user_data SET char_name=@NEW_CHAR_NAME WHERE char_name=@CHAR_NAME";
                        cmd.Parameters.AddWithValue("@NEW_CHAR_NAME", newName);
                        cmd.Parameters.AddWithValue("@CHAR_NAME", playerName);
                        if (cmd.ExecuteNonQuery() != 1)
                        {
                            response.ResponseCode = 503;
                            return response;
                        }
                    }
                }

                DatabaseServiceHelper.AddDonatePoints(accountName, -1 * renameService.Price);

                response.ResponseCode = 200;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to execute query. Exception: " + ex.Message);
                response.ResponseCode = 503;
                response.ResponseMessage = ex.Message;
            }

            return response;
        }

        public async Task<L2Response> SetNobless(string playerName)
        {
            L2Response response = new L2Response();

            // Disabled
            response.ResponseCode = 500;

            return response;
        }

        public async Task<L2Response> ChangeSex(string playerName)
        {
            var response = new L2Response();

            DonateService service = await DatabaseServiceHelper.GetDonateServiceById(4);
            if (service == null)
            {
                response.ResponseCode = 501;
                return response;
            }
            if (service.Price < 0)
            {
                // Service is disabled
                response.ResponseCode = 501;
                return response;
            }

            string accountName = await DatabaseServiceHelper.GetAccountNameFromCharName(playerName);
            int numDonatePoints = await DatabaseServiceHelper.GetNumDonatePoints(accountName);
            if (numDonatePoints == -1)
            {
                response.ResponseCode = 501;
                return response;
            }
            else if (numDonatePoints < service.Price)
            {
                response.ResponseCode = 500;
                return response;
            }

            try
            {
                using (SqlConnection lin2worldDbConn = new SqlConnection(LIN2WORLD_CONN_STRING))
                {
                    lin2worldDbConn.Open();

                    byte gender = 0;

                    // 1. Get current gender
                    using (SqlCommand cmd = lin2worldDbConn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT gender FROM user_data WHERE char_name=@CHAR_NAME";
                        cmd.Parameters.AddWithValue("@CHAR_NAME", playerName);

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                gender = reader.GetByte(reader.GetOrdinal("gender"));
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("Failed to retrieve account name based on character name: " + playerName);
                                response.ResponseCode = 501;
                                return response;
                            }
                        }
                    }

                    gender = gender == 0 ? gender = 1 : gender = 0;

                    // 2. Change it to opposite gender
                    using (SqlCommand cmd = lin2worldDbConn.CreateCommand())
                    {
                        cmd.CommandText = "UPDATE user_data SET gender=@NEW_GENDER WHERE char_name=@CHAR_NAME";
                        cmd.Parameters.AddWithValue("@NEW_GENDER", gender);
                        cmd.Parameters.AddWithValue("@CHAR_NAME", playerName);
                        if (cmd.ExecuteNonQuery() != 1)
                        {
                            response.ResponseCode = 501;
                            return response;
                        }
                    }
                }

                DatabaseServiceHelper.AddDonatePoints(accountName, -1 * service.Price);

                response.ResponseCode = 200;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to execute query. Exception: " + ex.Message);
                response.ResponseCode = 501;
                response.ResponseMessage = ex.Message;
            }

            return response;
        }

        public async Task<L2Response> ResetPk(string playerName)
        {
            var response = new L2Response();

            DonateService service = await DatabaseServiceHelper.GetDonateServiceById(3);
            if (service == null)
            {
                response.ResponseCode = 500;
                return response;
            }
            if (service.Price < 0)
            {
                // Service is disabled
                response.ResponseCode = 500;
                return response;
            }

            string accountName = await DatabaseServiceHelper.GetAccountNameFromCharName(playerName);
            int numDonatePoints = await DatabaseServiceHelper.GetNumDonatePoints(accountName);
            if (numDonatePoints == -1)
            {
                response.ResponseCode = 500;
                return response;
            }
            else if (numDonatePoints < service.Price)
            {
                response.ResponseCode = 501;
                return response;
            }

            try
            {
                using (SqlConnection lin2worldDbConn = new SqlConnection(LIN2WORLD_CONN_STRING))
                {
                    lin2worldDbConn.Open();

                    // Check if player already has 0 PKs
                    using (SqlCommand cmd = lin2worldDbConn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT PK FROM user_data WHERE char_name=@CHAR_NAME";
                        cmd.Parameters.AddWithValue("@CHAR_NAME", playerName);
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                int numPKs = reader.GetInt32(reader.GetOrdinal("PK"));
                                if (numPKs == 0)
                                {
                                    response.ResponseCode = 502;
                                    return response;
                                }
                            }
                            else
                            {
                                response.ResponseCode = 500;
                                return response;
                            }
                        }
                    }

                    using (SqlCommand cmd = lin2worldDbConn.CreateCommand())
                    {
                        cmd.CommandText = "UPDATE user_data SET PK=0 WHERE char_name=@CHAR_NAME";
                        cmd.Parameters.AddWithValue("@CHAR_NAME", playerName);
                        if (cmd.ExecuteNonQuery() != 1)
                        {
                            response.ResponseCode = 500;
                            return response;
                        }
                    }
                }

                DatabaseServiceHelper.AddDonatePoints(accountName, -1 * service.Price);

                response.ResponseCode = 200;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to execute query. Exception: " + ex.Message);
                response.ResponseCode = 500;
                response.ResponseMessage = ex.Message;
            }

            return response;
        }

        public async Task<L2Response> GetAllPlayers()
        {
            var response = new GetAllPlayerNamesResponse();
            try
            {
                using (SqlConnection lin2worldDbConn = new SqlConnection(LIN2WORLD_CONN_STRING))
                {
                    lin2worldDbConn.Open();

                    using (SqlCommand cmd = lin2worldDbConn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT char_name FROM user_data";

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            List<string> charNames = new List<string>();
                            while (reader.Read())
                            {
                                charNames.Add(reader["char_name"].ToString());
                            }
                            response.AllPlayerNames = charNames.ToArray();
                        }
                    }

                    response.ResponseCode = 200;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to execute query. Exception: " + ex.Message);
                response.ResponseCode = 500;
                response.ResponseMessage = ex.Message;
            }

            return response;
        }

        public async Task<L2Response> Punish(int punishId, string playerName, int time)
        {
            var response = new L2Response();
            try
            {
                using (SqlConnection lin2worldDbConn = new SqlConnection(LIN2WORLD_CONN_STRING))
                {
                    lin2worldDbConn.Open();

                    int charId = await DatabaseServiceHelper.GetCharIdFromCharName(playerName);
                    if (charId == -1)
                    {
                        System.Diagnostics.Debug.WriteLine("1");
                        response.ResponseCode = 502;
                        return response;
                    }

                    bool on = true;
                    if (punishId > 4)
                    {
                        punishId -= 4;
                        on = false;
                    }
                    if (punishId > 4)
                    {
                        System.Diagnostics.Debug.WriteLine("2");
                        response.ResponseCode = 502;
                        time = 0;
                        return response;
                    }

                    using (SqlCommand cmd = lin2worldDbConn.CreateCommand())
                    {
                        cmd.CommandText = "lin_SetPunish";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@char_id", charId);
                        cmd.Parameters.AddWithValue("@punish_id", punishId);
                        cmd.Parameters.AddWithValue("@punish_on", on ? 1 : 0);
                        cmd.Parameters.AddWithValue("@remain", time);

                        cmd.ExecuteNonQuery();
                    }

                    response.ResponseCode = 200;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to execute query. Exception: " + ex.Message);
                response.ResponseCode = 500;
                response.ResponseMessage = ex.Message;
            }

            return response;
        }

        public async Task<L2Response> GetAllOnlinePlayersForMap()
        {
            GetAllOnlinePlayersForMapResponse response = new GetAllOnlinePlayersForMapResponse();

            try
            {
                using (SqlConnection lin2worldDbConn = new SqlConnection(LIN2WORLD_CONN_STRING))
                {
                    lin2worldDbConn.Open();

                    using (SqlCommand cmd = lin2worldDbConn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT char_name, xloc, yloc, Lev, nickname FROM user_data with (nolock) WHERE login>logout";

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            List<MapPlayer> onlinePlayers = new List<MapPlayer>();
                            while (reader.Read())
                            {
                                MapPlayer plr = new MapPlayer();
                                plr.Name = reader["char_name"].ToString();
                                plr.Title = reader["nickname"].ToString();
                                plr.Level = reader.GetByte(reader.GetOrdinal("Lev"));
                                plr.X = reader.GetInt32(reader.GetOrdinal("xloc"));
                                plr.Y = reader.GetInt32(reader.GetOrdinal("yloc"));

                                onlinePlayers.Add(plr);
                            }
                            response.MapPlayers = onlinePlayers.ToArray();
                        }
                    }

                    response.ResponseCode = 200;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to execute query. Exception: " + ex.Message);
                response.ResponseCode = 500;
                response.ResponseMessage = ex.Message;
            }

            return response;
        }

        public async Task<L2Response> GetAllBossesForMap()
        {
            GetLiveRbsForMapResponse response = new GetLiveRbsForMapResponse();

            try
            {
                using (SqlConnection lin2worldDbConn = new SqlConnection(LIN2WORLD_CONN_STRING))
                {
                    lin2worldDbConn.Open();

                    using (SqlCommand cmd = lin2worldDbConn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT * FROM npc_boss WHERE alive=1";

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            List<MapMob> bossList = new List<MapMob>();
                            while (reader.Read())
                            {
                                MapMob boss = new MapMob();
                                boss.Name = reader["npc_db_name"].ToString();
                                boss.MaxHp = reader.GetInt32(reader.GetOrdinal("hp"));
                                boss.CurrentHp = boss.MaxHp;
                                boss.Level = 1;// reader.GetByte(reader.GetOrdinal("Lev"));
                                boss.X = reader.GetInt32(reader.GetOrdinal("pos_x"));
                                boss.Y = reader.GetInt32(reader.GetOrdinal("pos_y"));

                                bossList.Add(boss);
                            }
                            response.MapMobs = bossList.ToArray();
                        }
                    }

                    response.ResponseCode = 200;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to execute query. Exception: " + ex.Message);
                response.ResponseCode = 500;
                response.ResponseMessage = ex.Message;
            }

            return response;
        }

        public async Task<L2Response> SpawnNpc(int npcId, int x, int y)
        {
            var request = new SpawnNpcRequest
            {
                NpcId = npcId,
                X = x,
                Y = y
            };

            var responseObject = await request.SendPostRequest<L2Response>();
            return responseObject;
        }

        public async Task<L2Response> SetDonateList(AdminDonateListViewmodel[] items)
        {
            var response = new L2Response();
            try
            {
                using (SqlConnection lin2worldDbConn = new SqlConnection(LIN2WORLD_CONN_STRING))
                {
                    lin2worldDbConn.Open();

                    using (SqlCommand cmd = lin2worldDbConn.CreateCommand())
                    {
                        cmd.CommandText = "TRUNCATE TABLE l2acp_donateitems";
                        cmd.ExecuteNonQuery();
                    }

                    foreach (AdminDonateListViewmodel item in items)
                    {
                        using (SqlCommand cmd = lin2worldDbConn.CreateCommand())
                        {
                            cmd.CommandText = "INSERT INTO l2acp_donateitems (itemId, itemCount, enchant, price) " +
                                "VALUES (@ITEM_ID, @ITEM_COUNT, @ENCHANT, @PRICE)";
                            cmd.Parameters.AddWithValue("@ITEM_ID", item.itemid);
                            cmd.Parameters.AddWithValue("@ITEM_COUNT", item.itemcount);
                            cmd.Parameters.AddWithValue("@ENCHANT", item.itemenchant);
                            cmd.Parameters.AddWithValue("@PRICE", item.itemprice);
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }

                    response.ResponseCode = 200;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to execute query. Exception: " + ex.Message);
                response.ResponseCode = 500;
                response.ResponseMessage = ex.Message;
            }

            return response;
        }

        public async Task<L2Response> RestartServer(int seconds)
        {
            var responseObject = await new RestartServerRequest(seconds).SendPostRequest<L2Response>();
            return responseObject;
        }


        public async Task<L2Response> GetLuckyWheelList()
        {
            LuckyWheelListResponse response = new LuckyWheelListResponse();

            var items = await DatabaseServiceHelper.GetLuckyWheelListHelper();
            if (items != null)
            {
                response.Items = items.ToArray();
                response.ResponseCode = 200;
            }
            else
            {
                response.ResponseCode = 500;
            }

            return response;
        }

        public async Task<L2Response> GetAnalyticsPlayers()
        {
            // TODO
            GetAnalyticsPlayersResponse response = new GetAnalyticsPlayersResponse();

            List<AnalyticsPlayerData> list = new List<AnalyticsPlayerData>();
            var item = new AnalyticsPlayerData();
            item.Count = 500;
            item.Timestamp = 0;
            list.Add(item);

            response.PlayerData = list.ToArray();
            response.ResponseCode = 200;

            return response;
        }

        public async Task<L2Response> SpinLuckyWheel(string playername)
        {
            LuckyWheelSpinResponse response = new LuckyWheelSpinResponse();

            var items = await DatabaseServiceHelper.GetLuckyWheelListHelper();
            if (items != null)
            {
                Dictionary<int, double> itemIdToUpperLimitChance = new Dictionary<int, double>();

                double totalChance = 0.0;
                int itemIdx = 0;
                foreach (var item in items)
                {
                    totalChance += item.Chance;
                    itemIdToUpperLimitChance.Add(itemIdx, totalChance);
                    itemIdx++;
                }

                double random = (new Random()).NextDouble() * totalChance;
                response.Item = null;
                itemIdx = 0;
                foreach (var item in items)
                {
                    if (random <= itemIdToUpperLimitChance[itemIdx])
                    {
                        response.Item= item;
                        break;
                    }
                    itemIdx++;
                }

                if (response.Item == null)
                {
                    response.ResponseCode = 500;
                }
                else
                {
                    string accountName = await DatabaseServiceHelper.GetAccountNameFromCharName(playername);
                    DatabaseServiceHelper.AddDonatePoints(accountName, -5);
                    await DatabaseServiceHelper.GiveItemToCharacter(playername, response.Item.ItemId, response.Item.Count, 0);
                    response.ResponseCode = 200;
                }
            }
            else
            {
                response.ResponseCode = 500;
            }

            return response;
        }
    }
}
 
 