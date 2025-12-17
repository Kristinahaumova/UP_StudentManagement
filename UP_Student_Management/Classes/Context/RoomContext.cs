using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using UP_Student_Management.Classes.Common;
using UP_Student_Management.Classes.Interfaces;
using UP_Student_Management.Classes.Models;

namespace UP_Student_Management.Classes.Context
{
    public class RoomContext : Room, IRoom
    {
        public List<RoomContext> AllRooms()
        {
            List<RoomContext> allRooms = new List<RoomContext>();

            string query = "SELECT * FROM `Rooms` ORDER BY Name";

            using (MySqlConnection connection = Connection.OpenConnection())
            {
                if (connection == null) throw new Exception("Не удалось установить соединение с базой данных");

                using (MySqlDataReader data = Connection.Query(query, connection))
                {
                    while (data.Read())
                    {
                        RoomContext room = new RoomContext
                        {
                            Id = data.GetInt32(0),
                            Name = data.GetString(1),
                            Capacity = data.GetInt32(2)
                        };
                        allRooms.Add(room);
                    }
                }
            }
            return allRooms;
        }

        public RoomContext GetById(int id)
        {
            string query = "SELECT * FROM `Rooms` WHERE Id = @id";

            using (MySqlConnection connection = Connection.OpenConnection())
            {
                if (connection == null) throw new Exception("Не удалось установить соединение с базой данных");

                try
                {
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);

                        using (MySqlDataReader data = command.ExecuteReader())
                        {
                            if (data.Read())
                            {
                                return new RoomContext
                                {
                                    Id = data.GetInt32(0),
                                    Name = data.GetString(1),
                                    Capacity = data.GetInt32(2)
                                };
                            }
                        }
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Ошибка получения аудитории по ID: {ex.Message}");
                }
            }
        }

        public List<RoomContext> SearchByName(string name)
        {
            List<RoomContext> rooms = new List<RoomContext>();

            string query = "SELECT * FROM `Rooms` WHERE Name LIKE @name ORDER BY Name";

            using (MySqlConnection connection = Connection.OpenConnection())
            {
                if (connection == null) throw new Exception("Не удалось установить соединение с базой данных");

                try
                {
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@name", $"%{name}%");

                        using (MySqlDataReader data = command.ExecuteReader())
                        {
                            while (data.Read())
                            {
                                rooms.Add(new RoomContext
                                {
                                    Id = data.GetInt32(0),
                                    Name = data.GetString(1),
                                    Capacity = data.GetInt32(2)
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Ошибка поиска аудиторий: {ex.Message}");
                }
            }

            return rooms;
        }

        public void Save(bool Update = false)
        {
            using (MySqlConnection connection = Connection.OpenConnection())
            {
                if (connection == null) throw new Exception("Не удалось установить соединение с базой данных");

                try
                {
                    if (Update)
                    {
                        string query = "UPDATE `Rooms` SET `Name` = @name, `Capacity` = @capacity WHERE `Id` = @id";
                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@name", this.Name);
                            command.Parameters.AddWithValue("@capacity", this.Capacity);
                            command.Parameters.AddWithValue("@id", this.Id);
                            command.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        string query = "INSERT INTO `Rooms` (`Name`, `Capacity`) VALUES (@name, @capacity)";
                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@name", this.Name);
                            command.Parameters.AddWithValue("@capacity", this.Capacity);
                            command.ExecuteNonQuery();
                            this.Id = (int)command.LastInsertedId;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Ошибка сохранения аудитории: {ex.Message}");
                }
            }
        }

        public void Delete()
        {
            using (MySqlConnection connection = Connection.OpenConnection())
            {
                if (connection == null) throw new Exception("Не удалось установить соединение с базой данных");

                try
                {
                    string query = "DELETE FROM `Rooms` WHERE `Id` = @id";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", this.Id);
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Ошибка удаления аудитории: {ex.Message}");
                }
            }
        }

        public bool Exists(int id)
        {
            string query = "SELECT COUNT(*) FROM `Rooms` WHERE Id = @id";

            using (MySqlConnection connection = Connection.OpenConnection())
            {
                if (connection == null) return false;

                try
                {
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        int count = Convert.ToInt32(command.ExecuteScalar());
                        return count > 0;
                    }
                }
                catch
                {
                    return false;
                }
            }
        }

        public bool ExistsByName(string name, int? excludeId = null)
        {
            string query = "SELECT COUNT(*) FROM `Rooms` WHERE Name = @name";

            using (MySqlConnection connection = Connection.OpenConnection())
            {
                if (connection == null) return false;

                try
                {
                    if (excludeId.HasValue)
                    {
                        query += " AND Id != @excludeId";
                    }

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@name", name);

                        if (excludeId.HasValue)
                        {
                            command.Parameters.AddWithValue("@excludeId", excludeId.Value);
                        }

                        int count = Convert.ToInt32(command.ExecuteScalar());
                        return count > 0;
                    }
                }
                catch
                {
                    return false;
                }
            }
        }

        // Получить количество занятых мест в аудитории
        public int GetOccupiedSeats()
        {
            string query = @"
                SELECT COUNT(*) 
                FROM Schedule s 
                JOIN Lessons l ON s.LessonId = l.Id 
                WHERE s.RoomId = @roomId 
                AND l.DateTimeStart <= NOW() 
                AND l.DateTimeEnd >= NOW()";

            using (MySqlConnection connection = Connection.OpenConnection())
            {
                if (connection == null) return 0;

                try
                {
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@roomId", this.Id);
                        return Convert.ToInt32(command.ExecuteScalar());
                    }
                }
                catch
                {
                    return 0;
                }
            }
        }

        // Получить процент занятости аудитории
        public double GetOccupancyPercentage()
        {
            int occupied = GetOccupiedSeats();
            return this.Capacity > 0 ? Math.Round((double)occupied / this.Capacity * 100, 1) : 0;
        }
    }
}
