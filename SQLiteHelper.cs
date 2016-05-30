/*
This file is part of SQLite.Helper.

SQLite.Helper is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

SQLite.Helper is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with SQLite.Helper.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace SQLite.Helper
{
    /// <summary>
    /// Class to simplify connections to a SQLite database
    /// </summary>
    public class SQLiteHelper
    {
        private readonly string ConnectionString;

        /// <summary>
        /// Connection to the SQLite database
        /// </summary>
        [Obsolete("Connections are created when needed", true)]
        public SQLiteConnection Connection { get; private set; }

        /// <summary>
        /// If and general error has occurred 
        /// </summary>
        [Obsolete("Use the exception that the methods throw", true)]
        public bool GeneralError { get; private set; }

        /// <summary>
        /// If and sql error has occurred 
        /// </summary>
        [Obsolete("Use the exception that the methods throw", true)]
        public bool SqlError { get; private set; }

        /// <summary>
        /// Error message
        /// </summary>
        [Obsolete("Use the exception that the methods throw", true)]
        public string SqlErrorMessage { get; private set; }

        /// <summary>
        /// SQLite error number 
        /// </summary>
        [Obsolete("Use the exception that the methods throw", true)]
        public int SqlErrorNum { get; private set; }

        /// <summary>
        /// Doesn't create a connection
        /// </summary>
        [Obsolete("You must pass the connection, or the file", true)]
        public SQLiteHelper()
        {
        }

        /// <summary>
        /// Checks if the database exists and creates the connnection to the database
        /// </summary>
        /// <param name="DatabasePath">Database Path</param>
        public SQLiteHelper(string DatabasePath)
        {
            // Checks if the database exists
            if (!File.Exists(DatabasePath))
            {
                throw new FileNotFoundException("Database not found", DatabasePath);
            }
            else
            {
                // Creates the connection
                ConnectionString = "Data Source=" + DatabasePath + "; Version=3";
            }
        }

        /// <summary>
        /// Creates a database
        /// </summary>
        /// <param name="Query">SQL statement</param>
        public void CreateDb(string Query)
        {
            this.SendQuery(Query);
        }

        /// <summary>
        /// Creates a database
        /// </summary>
        /// <param name="Query">SQL statement</param>
        /// <param name="DropQuery">SQL to drop current tables</param>
        [Obsolete("The query to drop tables no longer exists", false)]
        public void CreateDb(string Query, string DropQuery = null)
        {
            this.CreateDb(Query);
        }

        /// <summary>
        /// Sends a SQL query that doesn't return any value
        /// It can also be used to create a table, but it's recomended to use CreateDb instead
        /// </summary>
        /// <param name="Query">SQL statement</param>
        public void SendQuery(string Query)
        {
            using (SQLiteConnection Connection = new SQLiteConnection(this.ConnectionString))
            {
                using (SQLiteCommand SqlCmd = new SQLiteCommand())
                {
                    SqlCmd.Connection = Connection;
                    SqlCmd.CommandText = Query;
                    Connection.Open();
                    SqlCmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Gets the first result from the query
        /// </summary>
        /// <param name="Query">SQL statement</param>
        /// <returns>Return the value inside a System.Object and must be converted. If no result, returns null</returns>
        public object Get(string Query)
        {
            object result = null;
            using (SQLiteConnection Connection = new SQLiteConnection(this.ConnectionString))
            {
                using (SQLiteCommand SqlCmd = new SQLiteCommand())
                {
                    SqlCmd.Connection = Connection;
                    SqlCmd.CommandText = Query;
                    Connection.Open();
                    using (SQLiteDataReader Reader = SqlCmd.ExecuteReader())
                    {
                        if (Reader.NextResult())
                        {
                            result = Reader;
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets table values inside a dataset
        /// </summary>
        /// <param name="Query">SQL statement</param>
        /// <param name="TableName">Name of the table</param>
        /// <returns>All the returned values into a dataset</returns>
        public DataSet GetTable(string Query, string TableName)
        {
            DataSet data = null;

            using (SQLiteConnection Connection = new SQLiteConnection(this.ConnectionString))
            {
                using (SQLiteDataAdapter Adapter = new SQLiteDataAdapter(Query, Connection))
                {
                    Adapter.Fill(data, TableName);
                }
            }


            return data;
        }
    }
}
