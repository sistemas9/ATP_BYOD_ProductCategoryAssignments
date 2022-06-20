using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ATP_BYOD_ProductCategoryAssignments
{
    public class DatabaseAYT03
    {
        IDataReader rd = null;
        int rowsAfected = 0;
        public SqlConnection conn;

        public DatabaseAYT03()
        {
            var AppSettings = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build();

            String conectionString = AppSettings["DB_AYT03"];
            conn = new SqlConnection(conectionString);
            conn.Close();
        }

        public async Task<IDataReader> Query(String queryStr)
        {
          SqlCommand command = new SqlCommand(queryStr, conn);
          command.CommandTimeout = 120;
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
            command.CommandTimeout = 120;
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            try
            {
                rowsAfected = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return -2;
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
                conn.Open();
                cmd.CommandTimeout = 120;
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
  public class parametrosSP
  {
    public String nombre { get; set; }
    public dynamic valor { get; set; }
  }
}
