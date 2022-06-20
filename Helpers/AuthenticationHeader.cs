using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ATP_BYOD_ProductCategoryAssignments;

namespace ATP_BYOD_ProductCategoryAssignments
{
  public class AuthenticationHeader
    {
        public static String clientId;
        public static String clientSecretId;
        public static String urlBase;

    public async Task<String> getAuthenticationHeader()
    {
      try
      {
        ////////////Obtener datos de configuracion////////////////////////////////////////////////////////////////////////////////////
        var configs = new GetConfigsData();
        String config = await configs.GetConfigurationData("Config");
        String company = await configs.GetConfigurationData("Company");
        String urlToken = "";
        if (company == "atp")
        {
          if (config == "DESARROLLO")
            urlToken = await configs.GetConfigurationData("URL_TOKEN_ATP_TES");
          else
            urlToken = await configs.GetConfigurationData("URL_TOKEN_ATP_PROD");
        }
        /////////////////////////////////////generar token/////////////////////////////////////////////////////////////////////////////
        token authenticationHeader = new token();
        System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;
        HttpClient token = new HttpClient();
        HttpResponseMessage response = await token.GetAsync(urlToken);
        response.EnsureSuccessStatusCode();
        String responseBody = await response.Content.ReadAsStringAsync();
        responseBody = responseBody.Substring(1, responseBody.Length - 2);
        authenticationHeader = JsonConvert.DeserializeObject<token>(responseBody);
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        return authenticationHeader.Token;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        return ex.Message;
      }
    }
  }
  public class token
  {
    public String Token { get; set; }
  }
}