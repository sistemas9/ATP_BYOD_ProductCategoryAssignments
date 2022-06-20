
using FastMember;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;

namespace ATP_BYOD_ProductCategoryAssignments
{
  class Program
  {
    private static IConfigurationRoot Appsettings;
    public static String companyName = String.Empty;
    public static String connStr = String.Empty;
    private static DatabaseAYT03 dbayt03 = new DatabaseAYT03();
    static void Main(string[] args)
    {
      DateTime inicio = DateTime.Now;
      Console.WriteLine("Hello World!");
      var AppSettings = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build();
      connStr = AppSettings["DB_AYT03"];
      int cuantos = CuantosRegistrosProductCategoryAssignments();
      List<String> queryUrls = GetQueryUrls(cuantos);
      List<ProductCategoryAssignments> productos = GetProductCategoryAssignments(queryUrls);
      InsertDataProductCategoryAssignments(productos);
      DateTime fin = DateTime.Now;
      Console.WriteLine("Termino");
      Console.WriteLine("Tardo: {0} segundos", Math.Round( ((fin - inicio).TotalSeconds), 2));
    }

    public static List<String> GetQueryUrls(int cuantas)
    {
      List<String> urls = new List<String>();
      String incremento = "1000";
      for (int i = 0; i <= cuantas; i += Convert.ToInt32(incremento))
      {
        String url = "https://ayt.operations.dynamics.com/data/ProductCategoryAssignments?$skip=" + i + "&$top=" + incremento;
        urls.Add(url);
      }
      return urls;
    }

    public static int CuantosRegistrosProductCategoryAssignments()
    {
      int cuantos = 0;

      String urlProductCategoryAssignments = "https://ayt.operations.dynamics.com/data/ProductCategoryAssignments?$top=1&$count=true";
      ConsultaEntity entity = new ConsultaEntity();
      var result = entity.QueryEntity(urlProductCategoryAssignments);
      var resultObj = JsonConvert.DeserializeObject<dynamic>(result.Result);
      cuantos = resultObj["@odata.count"];
      return cuantos;
    }

    public static List<ProductCategoryAssignments> GetProductCategoryAssignments(List<String> urls)
    {
      List<ProductCategoryAssignments> productos = new List<ProductCategoryAssignments>();
      Parallel.ForEach(urls, (url) => {
        ConsultaEntity entity = new ConsultaEntity();
        var result = entity.QueryEntity(url);
        EntityModel resultObj = JsonConvert.DeserializeObject<EntityModel>(result.Result);
        productos.AddRange(resultObj.value);
      });
      return productos;
    }

    public static Boolean InsertDataProductCategoryAssignments(List<ProductCategoryAssignments> products)
    {
      Boolean inserted = false;

      String truncate = "TRUNCATE TABLE AYT_ProductCategoryAsignments";
      int rows = dbayt03.queryInsert(truncate).Result;
      var copyParameters = new[]
      {
        nameof(ProductCategoryAssignments.ProductNumber),
        nameof(ProductCategoryAssignments.ProductCategoryHierarchyName),
        nameof(ProductCategoryAssignments.ProductCategoryName),
      };
      using (var sqlCopy = new SqlBulkCopy(connStr))
      {
        sqlCopy.DestinationTableName = "[AYT_ProductCategoryAsignments]";
        sqlCopy.BatchSize = 10000;
        using (var readerProducts = ObjectReader.Create(products, copyParameters))
        {
          sqlCopy.WriteToServer(readerProducts);
        }
      }
      inserted = true;
      return inserted;
    }
  }
}
