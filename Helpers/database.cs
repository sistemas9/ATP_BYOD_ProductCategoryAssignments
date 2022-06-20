using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ATP_BYOD_ProductCategoryAssignments
{
    class Database
    {
        IDataReader rd = null;
        int rowsAfected = 0;
        public SqlConnection conn;

        public Database()
        {
            var AppSettings = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();

            String conectionString = AppSettings["DB_AYTPROD"];
            conn = new SqlConnection(conectionString);
            conn.Close();
        }

        public async Task<IDataReader> Query(String queryStr)
        {
            SqlCommand command = new SqlCommand(queryStr, conn);
            if (conn.State == ConnectionState.Closed)
               await conn.OpenAsync();
            try
            {
                rd = command.ExecuteReader();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return rd;
        }

        public async Task<int> queryInsert(String queryStr)
        {
            SqlCommand command = new SqlCommand(queryStr, conn);
            if (conn.State == ConnectionState.Closed)
                await conn.OpenAsync();
            try
            {
                rowsAfected = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return rowsAfected;
        }

        public async Task<int> QueryStroredProcedure(String storedProcedure, List<parametrosSP> parametros)
        {
            using (conn)
            {
                using (SqlCommand cmd = new SqlCommand(storedProcedure, conn))
                {
                    if (conn.State == ConnectionState.Closed)
                        await conn.OpenAsync();
                    cmd.CommandType = CommandType.StoredProcedure;
                    foreach (parametrosSP param in parametros)
                    {
                        cmd.Parameters.AddWithValue(param.nombre, param.valor);
                    }
                    try
                    {
                        rowsAfected = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
            return rowsAfected;
        }
  }
}
