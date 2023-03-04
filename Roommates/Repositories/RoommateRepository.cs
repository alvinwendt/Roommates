using Microsoft.Data.SqlClient;
using Roommates.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roommates.Repositories
{
    internal class RoommateRepository : BaseRepository
    {
        public RoommateRepository(string connectionString) : base(connectionString) { }
        public Roommate? GetById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT 
                                        rm.id,
                                        rm.FirstName,
                                        rm.RentPortion,
                                        r.id AS RoomId,
                                        r.Name AS RoomName,
                                        r.MaxOccupancy AS RoomMaxOccupancy
                                        FROM Roommate rm
                                        JOIN Room r ON rm.RoomId = r.Id
                                        WHERE rm.Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);
                    Roommate? roommate = null;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            roommate = new Roommate
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                RentPortion = reader.GetInt32(reader.GetOrdinal("RentPortion")),
                                Room = new Room
                                 {
                                     Id = reader.GetInt32(reader.GetOrdinal("RoomId")),
                                     Name = reader.GetString(reader.GetOrdinal("RoomName")),
                                     MaxOccupancy = reader.GetInt32(reader.GetOrdinal("RoomMaxOccupancy"))
                                 }
                            };
                        }
                    }
                    return roommate;
                }
            }
        }
    }
}
