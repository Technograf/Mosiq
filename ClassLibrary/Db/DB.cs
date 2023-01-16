using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Db
{
    public class DB
    {
        public static void InitializeDatabase()
        {
            SqliteConnection db =
        new SqliteConnection("Filename=database.db");

            try
            {

                db.Open();

                String tableCommand = "CREATE TABLE IF NOT EXISTS songs" +
                    "(ID INTEGER PRIMARY KEY AUTOINCREMENT, " +
                    "Title VARCHAR(255) NULL, " +
                    "Artist VARCHAR(255) NULL, " +
                    //"AlbumArtist VARCHAR(255) NULL, " +
                    "Album VARCHAR(255) NULL, " +
                    "Genre VARCHAR(255) NULL, " +
                    "Year VARCHAR(255) NULL, " +
                    "Track VARCHAR(255) NULL, " +
                    "AlbumID VARCHAR(255) NULL, " +
                    "URI VARCHAR(255) NULL, " +
                    "HexColor VARCHAR(9) NULL, " +
                    "Star INTEGER NULL, " +
                    "DateAdded DateTime)";


                SqliteCommand createTable = new SqliteCommand(tableCommand, db);

                createTable.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERRO!!! " + ex.Message);
            }
            finally
            {
                db.Close();
            }
        }
    }
}
