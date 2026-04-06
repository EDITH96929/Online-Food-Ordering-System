using FoodOrderingSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace FoodOrderingSystem.DAL
{
    public class DeliveryDAL
    {
        public DeliveryBoy LoginDeliveryBoy(string email, string password)
        {
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = "SELECT * FROM DeliveryBoys WHERE Email=@Email AND Password=@Password";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Password", password);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                    return MapDeliveryBoy(reader);
                return null;
            }
        }

        public List<DeliveryBoy> GetAllDeliveryBoys()
        {
            List<DeliveryBoy> list = new List<DeliveryBoy>();
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = "SELECT * FROM DeliveryBoys ORDER BY JoinedOn DESC";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    list.Add(MapDeliveryBoy(reader));
            }
            return list;
        }

        public List<DeliveryBoy> GetAvailableDeliveryBoys()
        {
            List<DeliveryBoy> list = new List<DeliveryBoy>();
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = "SELECT * FROM DeliveryBoys WHERE IsAvailable=1";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    list.Add(MapDeliveryBoy(reader));
            }
            return list;
        }

        public bool AddDeliveryBoy(DeliveryBoy boy)
        {
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = @"INSERT INTO DeliveryBoys 
                                 (Name, Email, Password, Phone, IsAvailable)
                                 VALUES 
                                 (@Name, @Email, @Password, @Phone, 1)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Name", boy.Name);
                cmd.Parameters.AddWithValue("@Email", boy.Email);
                cmd.Parameters.AddWithValue("@Password", boy.Password);
                cmd.Parameters.AddWithValue("@Phone", boy.Phone ?? "");
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool ToggleAvailability(int deliveryBoyId, bool isAvailable)
        {
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = @"UPDATE DeliveryBoys 
                                 SET IsAvailable=@IsAvailable 
                                 WHERE DeliveryBoyId=@DeliveryBoyId";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@IsAvailable", isAvailable ? 1 : 0);
                cmd.Parameters.AddWithValue("@DeliveryBoyId", deliveryBoyId);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool EmailExists(string email)
        {
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = "SELECT COUNT(*) FROM DeliveryBoys WHERE Email=@Email";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Email", email);
                con.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        public DeliveryBoy GetDeliveryBoyById(int id)
        {
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = "SELECT * FROM DeliveryBoys WHERE DeliveryBoyId=@Id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Id", id);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                    return MapDeliveryBoy(reader);
                return null;
            }
        }

        private DeliveryBoy MapDeliveryBoy(SqlDataReader reader)
        {
            return new DeliveryBoy
            {
                DeliveryBoyId = (int)reader["DeliveryBoyId"],
                Name = reader["Name"].ToString(),
                Email = reader["Email"].ToString(),
                Password = reader["Password"].ToString(),
                Phone = reader["Phone"] == DBNull.Value ? "" : reader["Phone"].ToString(),
                IsAvailable = (bool)reader["IsAvailable"],
                TotalDeliveries = reader["TotalDeliveries"] == DBNull.Value ? 0 : (int)reader["TotalDeliveries"],
                JoinedOn = reader["JoinedOn"] == DBNull.Value ? System.DateTime.Now : (System.DateTime)reader["JoinedOn"]
            };
        }
    }
}
