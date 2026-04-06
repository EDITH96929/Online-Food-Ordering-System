using FoodOrderingSystem.Models;
using System;
using System.Data.SqlClient;

namespace FoodOrderingSystem.DAL
{
    public class UserDAL
    {
        public bool RegisterUser(User user)
        {
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = @"INSERT INTO Users (Name, Email, Password, Phone, Role) 
                                 VALUES (@Name, @Email, @Password, @Phone, 'User')";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Name", user.Name);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@Password", user.Password);
                cmd.Parameters.AddWithValue("@Phone", user.Phone ?? "");
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public User LoginUser(string email, string password)
        {
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = "SELECT * FROM Users WHERE Email=@Email AND Password=@Password";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Password", password);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new User
                    {
                        UserId = (int)reader["UserId"],
                        Name = reader["Name"].ToString(),
                        Email = reader["Email"].ToString(),
                        Password = reader["Password"].ToString(),
                        Role = reader["Role"].ToString(),
                        Phone = reader["Phone"] == DBNull.Value ? "" : reader["Phone"].ToString()
                    };
                }
                return null;
            }
        }

        public bool EmailExists(string email)
        {
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = "SELECT COUNT(*) FROM Users WHERE Email=@Email";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Email", email);
                con.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }
    }
}