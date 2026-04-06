using FoodOrderingSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace FoodOrderingSystem.DAL
{
    public class RatingDAL
    {
        // Add food or delivery rating
        public bool AddRating(Rating rating)
        {
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = @"INSERT INTO Ratings 
                                 (OrderId, UserId, Stars, Comment, RatedOn, RatingType, DeliveryBoyId)
                                 VALUES 
                                 (@OrderId, @UserId, @Stars, @Comment, GETDATE(), @RatingType, @DeliveryBoyId)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@OrderId", rating.OrderId);
                cmd.Parameters.AddWithValue("@UserId", rating.UserId);
                cmd.Parameters.AddWithValue("@Stars", rating.Stars);
                cmd.Parameters.AddWithValue("@Comment", rating.Comment ?? "");
                cmd.Parameters.AddWithValue("@RatingType", rating.RatingType ?? "Food");
                cmd.Parameters.AddWithValue("@DeliveryBoyId",
                    rating.DeliveryBoyId.HasValue ? (object)rating.DeliveryBoyId.Value : DBNull.Value);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // Check if user already rated this order for a specific type
        public bool HasRated(int orderId, string ratingType = "Food")
        {
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = @"SELECT COUNT(*) FROM Ratings 
                                 WHERE OrderId=@OrderId AND RatingType=@RatingType";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@OrderId", orderId);
                cmd.Parameters.AddWithValue("@RatingType", ratingType);
                con.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        // Get all food ratings for a specific food item
        public List<Rating> GetFoodRatings(int foodId)
        {
            List<Rating> list = new List<Rating>();
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = @"SELECT r.*, u.Name AS UserName, o.OrderCode
                                 FROM Ratings r
                                 JOIN Users u ON r.UserId = u.UserId
                                 JOIN Orders o ON r.OrderId = o.OrderId
                                 JOIN OrderDetails od ON od.OrderId = o.OrderId
                                 WHERE od.FoodId = @FoodId
                                 AND r.RatingType = 'Food'
                                 ORDER BY r.RatedOn DESC";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@FoodId", foodId);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    list.Add(MapRating(reader));
            }
            return list;
        }

        // Get delivery ratings for a specific delivery boy
        public List<Rating> GetDeliveryRatings(int deliveryBoyId)
        {
            List<Rating> list = new List<Rating>();
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = @"SELECT r.*, u.Name AS UserName, o.OrderCode
                                 FROM Ratings r
                                 JOIN Users u ON r.UserId = u.UserId
                                 JOIN Orders o ON r.OrderId = o.OrderId
                                 WHERE r.DeliveryBoyId = @DeliveryBoyId
                                 AND r.RatingType = 'Delivery'
                                 ORDER BY r.RatedOn DESC";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@DeliveryBoyId", deliveryBoyId);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    list.Add(MapRating(reader));
            }
            return list;
        }

        // Get average delivery rating for a delivery boy
        public double GetDeliveryAverageRating(int deliveryBoyId)
        {
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = @"SELECT ISNULL(AVG(CAST(Stars AS FLOAT)), 0)
                                 FROM Ratings
                                 WHERE DeliveryBoyId = @DeliveryBoyId
                                 AND RatingType = 'Delivery'";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@DeliveryBoyId", deliveryBoyId);
                con.Open();
                return (double)cmd.ExecuteScalar();
            }
        }
        // Add this method to RatingDAL.cs
        public List<Rating> GetRatingsByUser(int userId)
        {
            List<Rating> list = new List<Rating>();
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = @"SELECT r.*, u.Name AS UserName, o.OrderCode
                         FROM Ratings r
                         JOIN Users u ON r.UserId = u.UserId
                         JOIN Orders o ON r.OrderId = o.OrderId
                         WHERE r.UserId = @UserId
                         ORDER BY r.RatedOn DESC";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@UserId", userId);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    list.Add(MapRating(reader));
            }
            return list;
        }

        private Rating MapRating(SqlDataReader reader)
        {
            return new Rating
            {
                RatingId = (int)reader["RatingId"],
                OrderId = (int)reader["OrderId"],
                UserId = (int)reader["UserId"],
                UserName = reader["UserName"].ToString(),
                Stars = (int)reader["Stars"],
                Comment = reader["Comment"].ToString(),
                RatedOn = (System.DateTime)reader["RatedOn"],
                OrderCode = reader["OrderCode"].ToString(),
                RatingType = reader["RatingType"].ToString(),
                DeliveryBoyId = reader["DeliveryBoyId"] == DBNull.Value
                                ? (int?)null
                                : (int)reader["DeliveryBoyId"]
            };
        }
    }
}