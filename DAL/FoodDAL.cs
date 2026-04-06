using FoodOrderingSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace FoodOrderingSystem.DAL
{
    public class FoodDAL
    {
        public List<FoodItem> GetAllFoodItems()
        {
            List<FoodItem> list = new List<FoodItem>();
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = @"SELECT f.*, 
                                 ISNULL(AVG(CAST(r.Stars AS FLOAT)), 0) AS AverageRating,
                                 COUNT(r.RatingId) AS TotalRatings
                                 FROM FoodItems f
                                 LEFT JOIN Ratings r ON r.OrderId IN (
                                     SELECT od.OrderId FROM OrderDetails od WHERE od.FoodId = f.FoodId
                                 )
                                 WHERE f.IsAvailable = 1
                                 GROUP BY f.FoodId, f.FoodName, f.Price, f.ImagePath, 
                                          f.Description, f.Category, f.IsAvailable";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(MapFoodItem(reader));
                }
            }
            return list;
        }

        public FoodItem GetFoodById(int foodId)
        {
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = @"SELECT f.*, 
                                 ISNULL(AVG(CAST(r.Stars AS FLOAT)), 0) AS AverageRating,
                                 COUNT(r.RatingId) AS TotalRatings
                                 FROM FoodItems f
                                 LEFT JOIN Ratings r ON r.OrderId IN (
                                     SELECT od.OrderId FROM OrderDetails od WHERE od.FoodId = f.FoodId
                                 )
                                 WHERE f.FoodId = @FoodId
                                 GROUP BY f.FoodId, f.FoodName, f.Price, f.ImagePath,
                                          f.Description, f.Category, f.IsAvailable";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@FoodId", foodId);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                    return MapFoodItem(reader);
                return null;
            }
        }

        public bool AddFoodItem(FoodItem item)
        {
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = @"INSERT INTO FoodItems 
                                 (FoodName, Price, ImagePath, Description, Category, IsAvailable)
                                 VALUES (@FoodName, @Price, @ImagePath, @Description, @Category, 1)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@FoodName", item.FoodName);
                cmd.Parameters.AddWithValue("@Price", item.Price);
                cmd.Parameters.AddWithValue("@ImagePath", item.ImagePath ?? "default.jpg");
                cmd.Parameters.AddWithValue("@Description", item.Description ?? "");
                cmd.Parameters.AddWithValue("@Category", item.Category ?? "Other");
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool UpdateFoodItem(FoodItem item)
        {
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = @"UPDATE FoodItems 
                                 SET FoodName=@FoodName, Price=@Price, ImagePath=@ImagePath,
                                     Description=@Description, Category=@Category
                                 WHERE FoodId=@FoodId";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@FoodName", item.FoodName);
                cmd.Parameters.AddWithValue("@Price", item.Price);
                cmd.Parameters.AddWithValue("@ImagePath", item.ImagePath ?? "default.jpg");
                cmd.Parameters.AddWithValue("@Description", item.Description ?? "");
                cmd.Parameters.AddWithValue("@Category", item.Category ?? "Other");
                cmd.Parameters.AddWithValue("@FoodId", item.FoodId);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool DeleteFoodItem(int foodId)
        {
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = "UPDATE FoodItems SET IsAvailable=0 WHERE FoodId=@FoodId";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@FoodId", foodId);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public List<FoodItem> GetAllFoodItemsForAdmin()
        {
            List<FoodItem> list = new List<FoodItem>();
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = @"SELECT f.*, 
                                 ISNULL(AVG(CAST(r.Stars AS FLOAT)), 0) AS AverageRating,
                                 COUNT(r.RatingId) AS TotalRatings
                                 FROM FoodItems f
                                 LEFT JOIN Ratings r ON r.OrderId IN (
                                     SELECT od.OrderId FROM OrderDetails od WHERE od.FoodId = f.FoodId
                                 )
                                 GROUP BY f.FoodId, f.FoodName, f.Price, f.ImagePath,
                                          f.Description, f.Category, f.IsAvailable";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    list.Add(MapFoodItem(reader));
            }
            return list;
        }

        private FoodItem MapFoodItem(SqlDataReader reader)
        {
            return new FoodItem
            {
                FoodId = (int)reader["FoodId"],
                FoodName = reader["FoodName"].ToString(),
                Price = (decimal)reader["Price"],
                ImagePath = reader["ImagePath"].ToString(),
                Description = reader["Description"] == DBNull.Value ? "" : reader["Description"].ToString(),
                Category = reader["Category"] == DBNull.Value ? "Other" : reader["Category"].ToString(),
                IsAvailable = (bool)reader["IsAvailable"],
                AverageRating = (double)reader["AverageRating"],
                TotalRatings = (int)reader["TotalRatings"]
            };
        }
    }
}