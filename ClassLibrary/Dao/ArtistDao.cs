using ClassLibrary.Entities;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Dao
{
    public class ArtistDao
    {
        public static bool ArtistExists(Artist artist)
        {
            bool result;

            SqliteConnection db =
        new SqliteConnection("Filename=database.db");
            try
            {
                db.Open();

                SqliteCommand command = new SqliteCommand();
                command.Connection = db;

                command.CommandText = "SELECT count(Artist) FROM songs WHERE Artist = @ARTIST ";
                command.Parameters.AddWithValue("@ARTIST", artist.Name);

                int rowCount = Convert.ToInt32(command.ExecuteScalar());
                result = rowCount > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERRO!!! " + ex.Message);
                result = false;
            }
            finally
            {
                db.Close();
            }

            return result;
        }
    }
}
