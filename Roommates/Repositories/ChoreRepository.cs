using Microsoft.Data.SqlClient;
using Roommates.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data;

namespace Roommates.Repositories
{
    internal class ChoreRepository : BaseRepository
    {
        public ChoreRepository(string connectionString) : base(connectionString) { }

        public List<Chore> GetAll()
        {
            using (SqlConnection conn = Connection)
            {

                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, Name FROM Chore";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Chore> chores = new List<Chore>();

                    while (reader.Read())
                    {
                        int idColumePosition = reader.GetOrdinal("Id");

                        int idValue = reader.GetInt32(idColumePosition);

                        int nameColumnPosition = reader.GetOrdinal("Name");
                        string nameValue = reader.GetString(nameColumnPosition);

                        Chore chore = new Chore
                        {
                            Id = idValue,
                            Name = nameValue,
                        };

                        chores.Add(chore);
                    }

                    reader.Close();

                    return chores;
                }
            }
        }


        public Chore? GetById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Name FROM Chore WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);
                    Chore? chore = null;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            chore = new Chore()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name"))
                            };
                        }
                    }
                    return chore;
                }
            }
        }

        public void Insert(Chore chore)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Chore (Name) 
                                         OUTPUT INSERTED.Id 
                                         VALUES (@name)";
                    cmd.Parameters.AddWithValue("@name", chore.Name);
                    chore.Id = (int)cmd.ExecuteScalar();
                }
            }
        }

        public List<Chore>? GetUnassignedChores()
        {
            using (SqlConnection conn = Connection)
            {
                var sql = @"SELECT rc.[Id]
                                      ,rc.[RoommateId]
                                      ,rc.[ChoreId]
	                                  ,c.[Name]
                                      FROM [Roommates].[dbo].[RoommateChore] rc
                                      JOIN [Roommates].[dbo].[Chore] c ON rc.ChoreId= c.id
                                      Where RoommateId is null";


                var chores = conn.Query<Chore>(sql).ToList();
                return chores;

            }
        }

        public void AssignChore (int roommateId, int choreId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO RoommateChore (RoommateId, ChoreId) 
                                        VALUES (@RoommateId,@ChoreId)";

                    cmd.Parameters.AddWithValue("@RoommateId", roommateId); 
                    cmd.Parameters.AddWithValue("@ChoreId", choreId);

                    cmd.ExecuteNonQuery();
                }
            }
        }



    }
}
