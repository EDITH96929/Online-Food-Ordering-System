using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using FoodOrderingSystem.Models;

namespace FoodOrderingSystem.DAL
{
    public class OrderDAL
    {
        // Generate unique order code like ORD-2024-7823
        private string GenerateOrderCode()
        {
            Random rnd = new Random();
            return "ORD-" + DateTime.Now.Year + "-" + rnd.Next(1000, 9999);
        }

        // Generate 4 digit OTP
        private string GenerateOTP()
        {
            Random rnd = new Random();
            return rnd.Next(1000, 9999).ToString();
        }

        public int PlaceOrder(Order order)
        {
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = @"INSERT INTO Orders 
                         (UserId, OrderDate, TotalAmount, Status, OTP, OrderCode,
                          CustomerPhone, DeliveryAddress)
                         VALUES (@UserId, @OrderDate, @TotalAmount, 'Pending', @OTP, @OrderCode,
                          @CustomerPhone, @DeliveryAddress);
                         SELECT SCOPE_IDENTITY();";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@UserId", order.UserId);
                cmd.Parameters.AddWithValue("@OrderDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@TotalAmount", order.TotalAmount);
                cmd.Parameters.AddWithValue("@OTP", GenerateOTP());
                cmd.Parameters.AddWithValue("@OrderCode", GenerateOrderCode());
                cmd.Parameters.AddWithValue("@CustomerPhone", order.CustomerPhone ?? "");
                cmd.Parameters.AddWithValue("@DeliveryAddress", order.DeliveryAddress ?? "");
                con.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public void AddOrderDetail(OrderDetail detail)
        {
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = @"INSERT INTO OrderDetails (OrderId, FoodId, Quantity, Price)
                                 VALUES (@OrderId, @FoodId, @Quantity, @Price)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@OrderId", detail.OrderId);
                cmd.Parameters.AddWithValue("@FoodId", detail.FoodId);
                cmd.Parameters.AddWithValue("@Quantity", detail.Quantity);
                cmd.Parameters.AddWithValue("@Price", detail.Price);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public Dictionary<string, object> GetDashboardStats()
        {
            var stats = new Dictionary<string, object>();
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                con.Open();

                // Total revenue
                string q1 = "SELECT ISNULL(SUM(TotalAmount),0) FROM Orders WHERE Status='Delivered'";
                stats["TotalRevenue"] = (decimal)new SqlCommand(q1, con).ExecuteScalar();

                // Total orders
                string q2 = "SELECT COUNT(*) FROM Orders";
                stats["TotalOrders"] = (int)new SqlCommand(q2, con).ExecuteScalar();

                // Pending orders
                string q3 = "SELECT COUNT(*) FROM Orders WHERE Status='Pending'";
                stats["PendingOrders"] = (int)new SqlCommand(q3, con).ExecuteScalar();

                // Out for delivery
                string q4 = "SELECT COUNT(*) FROM Orders WHERE Status='OutForDelivery'";
                stats["OutForDelivery"] = (int)new SqlCommand(q4, con).ExecuteScalar();

                // Delivered
                string q5 = "SELECT COUNT(*) FROM Orders WHERE Status='Delivered'";
                stats["Delivered"] = (int)new SqlCommand(q5, con).ExecuteScalar();

                // Total users
                string q6 = "SELECT COUNT(*) FROM Users WHERE Role='User'";
                stats["TotalUsers"] = (int)new SqlCommand(q6, con).ExecuteScalar();

                // Top 5 food items by order count
                string q7 = @"SELECT TOP 5 f.FoodName, 
                      SUM(od.Quantity) AS TotalOrdered
                      FROM OrderDetails od
                      JOIN FoodItems f ON od.FoodId = f.FoodId
                      GROUP BY f.FoodName
                      ORDER BY TotalOrdered DESC";
                SqlDataReader r7 = new SqlCommand(q7, con).ExecuteReader();
                var topFoods = new List<string>();
                var topCounts = new List<int>();
                while (r7.Read())
                {
                    topFoods.Add(r7["FoodName"].ToString());
                    topCounts.Add((int)r7["TotalOrdered"]);
                }
                r7.Close();
                stats["TopFoods"] = topFoods;
                stats["TopCounts"] = topCounts;

                // Revenue last 7 days
                string q8 = @"SELECT CONVERT(DATE, OrderDate) AS Day,
                      ISNULL(SUM(TotalAmount),0) AS Revenue
                      FROM Orders
                      WHERE OrderDate >= DATEADD(DAY,-6,CAST(GETDATE() AS DATE))
                      AND Status='Delivered'
                      GROUP BY CONVERT(DATE, OrderDate)
                      ORDER BY Day ASC";
                SqlDataReader r8 = new SqlCommand(q8, con).ExecuteReader();
                var days = new List<string>();
                var revenues = new List<decimal>();
                while (r8.Read())
                {
                    days.Add(((DateTime)r8["Day"]).ToString("dd MMM"));
                    revenues.Add((decimal)r8["Revenue"]);
                }
                r8.Close();
                stats["RevenueDays"] = days;
                stats["RevenueAmounts"] = revenues;
            }
            return stats;
        }

        public List<Order> GetOrdersByUser(int userId)
        {
            List<Order> list = new List<Order>();
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = @"SELECT o.*, u.Name AS UserName, u.Phone AS UserPhone,
                                 d.Name AS DeliveryBoyName
                                 FROM Orders o
                                 JOIN Users u ON o.UserId = u.UserId
                                 LEFT JOIN DeliveryBoys d ON o.DeliveryBoyId = d.DeliveryBoyId
                                 WHERE o.UserId = @UserId
                                 ORDER BY o.OrderDate DESC";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@UserId", userId);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    list.Add(MapOrder(reader));
            }
            return list;
        }

        public List<Order> GetAllOrders()
        {
            List<Order> list = new List<Order>();
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = @"SELECT o.*, u.Name AS UserName, u.Phone AS UserPhone,
                                 d.Name AS DeliveryBoyName
                                 FROM Orders o
                                 JOIN Users u ON o.UserId = u.UserId
                                 LEFT JOIN DeliveryBoys d ON o.DeliveryBoyId = d.DeliveryBoyId
                                 ORDER BY o.OrderDate DESC";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    list.Add(MapOrder(reader));
            }
            return list;
        }

        public Order GetOrderById(int orderId)
        {
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = @"SELECT o.*, u.Name AS UserName, u.Phone AS UserPhone,
                                 d.Name AS DeliveryBoyName
                                 FROM Orders o
                                 JOIN Users u ON o.UserId = u.UserId
                                 LEFT JOIN DeliveryBoys d ON o.DeliveryBoyId = d.DeliveryBoyId
                                 WHERE o.OrderId = @OrderId";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@OrderId", orderId);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                    return MapOrder(reader);
                return null;
            }
        }

        public List<OrderDetail> GetOrderDetails(int orderId)
        {
            List<OrderDetail> list = new List<OrderDetail>();
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = @"SELECT od.*, f.FoodName FROM OrderDetails od
                                 JOIN FoodItems f ON od.FoodId = f.FoodId
                                 WHERE od.OrderId = @OrderId";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@OrderId", orderId);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new OrderDetail
                    {
                        OrderDetailId = (int)reader["OrderDetailId"],
                        OrderId = (int)reader["OrderId"],
                        FoodId = (int)reader["FoodId"],
                        Quantity = (int)reader["Quantity"],
                        Price = (decimal)reader["Price"],
                        FoodName = reader["FoodName"].ToString()
                    });
                }
            }
            return list;
        }

        // Admin assigns delivery boy and sets time
        public bool AssignDelivery(int orderId, int deliveryBoyId, int deliveryMinutes)
        {
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = @"UPDATE Orders 
                                 SET DeliveryBoyId = @DeliveryBoyId,
                                     DeliveryTime = @DeliveryTime,
                                     DeliveryStartTime = @StartTime,
                                     Status = 'OutForDelivery'
                                 WHERE OrderId = @OrderId";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@DeliveryBoyId", deliveryBoyId);
                cmd.Parameters.AddWithValue("@DeliveryTime", deliveryMinutes);
                cmd.Parameters.AddWithValue("@StartTime", DateTime.Now);
                cmd.Parameters.AddWithValue("@OrderId", orderId);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public List<Order> GetDeliveryHistory(int deliveryBoyId)
        {
            List<Order> list = new List<Order>();
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = @"SELECT o.*, u.Name AS UserName, u.Phone AS UserPhone,
                         d.Name AS DeliveryBoyName
                         FROM Orders o
                         JOIN Users u ON o.UserId = u.UserId
                         LEFT JOIN DeliveryBoys d ON o.DeliveryBoyId = d.DeliveryBoyId
                         WHERE o.DeliveryBoyId = @DeliveryBoyId
                         AND o.Status = 'Delivered'
                         ORDER BY o.OrderDate DESC";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@DeliveryBoyId", deliveryBoyId);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    list.Add(MapOrder(reader));
            }
            return list;
        }

        // Delivery boy verifies OTP
        public string VerifyOTP(int orderId, string enteredOTP)
        {
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                // Check attempts first
                string checkQuery = "SELECT OTP, OTPAttempts FROM Orders WHERE OrderId=@OrderId";
                SqlCommand checkCmd = new SqlCommand(checkQuery, con);
                checkCmd.Parameters.AddWithValue("@OrderId", orderId);
                con.Open();
                SqlDataReader reader = checkCmd.ExecuteReader();
                if (reader.Read())
                {
                    string correctOTP = reader["OTP"].ToString();
                    int attempts = (int)reader["OTPAttempts"];
                    reader.Close();

                    if (attempts >= 3)
                        return "locked";

                    if (correctOTP == enteredOTP)
                    {
                        // Mark as delivered
                        string updateQuery = "UPDATE Orders SET Status='Delivered' WHERE OrderId=@OrderId";
                        SqlCommand updateCmd = new SqlCommand(updateQuery, con);
                        updateCmd.Parameters.AddWithValue("@OrderId", orderId);
                        updateCmd.ExecuteNonQuery();
                        return "success";
                    }
                    else
                    {
                        // Increment attempts
                        string attemptQuery = "UPDATE Orders SET OTPAttempts=OTPAttempts+1 WHERE OrderId=@OrderId";
                        SqlCommand attemptCmd = new SqlCommand(attemptQuery, con);
                        attemptCmd.Parameters.AddWithValue("@OrderId", orderId);
                        attemptCmd.ExecuteNonQuery();
                        return "wrong";
                    }
                }
                return "error";
            }
        }

        public bool UpdateOrderStatus(int orderId, string status)
        {
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = "UPDATE Orders SET Status=@Status WHERE OrderId=@OrderId";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@OrderId", orderId);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public List<Order> GetOrdersByDeliveryBoy(int deliveryBoyId)
        {
            List<Order> list = new List<Order>();
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = @"SELECT o.*, u.Name AS UserName, u.Phone AS UserPhone,
                                 d.Name AS DeliveryBoyName
                                 FROM Orders o
                                 JOIN Users u ON o.UserId = u.UserId
                                 LEFT JOIN DeliveryBoys d ON o.DeliveryBoyId = d.DeliveryBoyId
                                 WHERE o.DeliveryBoyId = @DeliveryBoyId
                                 AND o.Status = 'OutForDelivery'
                                 ORDER BY o.OrderDate DESC";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@DeliveryBoyId", deliveryBoyId);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    list.Add(MapOrder(reader));
            }
            return list;
        }

        private Order MapOrder(SqlDataReader reader)
        {
            return new Order
            {
                OrderId = (int)reader["OrderId"],
                UserId = (int)reader["UserId"],
                UserName = reader["UserName"].ToString(),
                UserPhone = reader["UserPhone"] == DBNull.Value ? "" : reader["UserPhone"].ToString(),
                CustomerPhone = reader["CustomerPhone"] == DBNull.Value ? "" : reader["CustomerPhone"].ToString(),
                DeliveryAddress = reader["DeliveryAddress"] == DBNull.Value ? "" : reader["DeliveryAddress"].ToString(),
                OrderDate = (DateTime)reader["OrderDate"],
                TotalAmount = (decimal)reader["TotalAmount"],
                Status = reader["Status"].ToString(),
                OTP = reader["OTP"].ToString(),
                OrderCode = reader["OrderCode"].ToString(),
                DeliveryTime = reader["DeliveryTime"] == DBNull.Value ? (int?)null : (int)reader["DeliveryTime"],
                DeliveryBoyId = reader["DeliveryBoyId"] == DBNull.Value ? (int?)null : (int)reader["DeliveryBoyId"],
                DeliveryBoyName = reader["DeliveryBoyName"] == DBNull.Value ? "" : reader["DeliveryBoyName"].ToString(),
                DeliveryStartTime = reader["DeliveryStartTime"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["DeliveryStartTime"]
            };
        }


    }
}