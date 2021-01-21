using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ApiTestClient
{
    public class Program
    {
        private static async Task Main(string[] args)
        {
            Output(ConsoleColor.Green, () => { Console.WriteLine("API test client, initialising..."); });

            Output(ConsoleColor.Yellow, () =>
            {
                Console.Write("Please ensure api and identity server are started, and then press any key: ");
            });
            Console.ReadKey(true);

            Output(ConsoleColor.Green, () => { Console.WriteLine("\n\nObtain Access Token"); });

            Console.WriteLine("\n\nRequesting Token from Identity Server: ...");
            // discover endpoints from metadata
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("https://mystore.local/identity");
            if (disco.IsError)
            {
                Output(ConsoleColor.Red, () => { Console.WriteLine(disco.Error); });
                return;
            }
            // request token
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "client",
                ClientSecret = "secret",

                Scope = "api1"
            });

            if (tokenResponse.IsError)
            {
                Output(ConsoleColor.Red, () =>
                {
                    Console.WriteLine(tokenResponse.Error);
                });
                return;
            }

            Output(ConsoleColor.White, () => { Console.Write($"\n\n[{tokenResponse.AccessToken}]"); });
            Console.WriteLine("\n\nToken recieved.");


            Output(ConsoleColor.Green, () => { Console.WriteLine("\n\nUse Access Token"); });

            Output(ConsoleColor.Yellow, () => { Console.Write("\n\nPress any key to use the token to call the API: "); });

            Console.ReadKey(true);

            //Console.WriteLine("\n\nCalling API with token: ...");
            //// call api
            var apiClient = new HttpClient();
            apiClient.SetBearerToken(tokenResponse.AccessToken);
            //// api is not exposed directly
            //var response = await apiClient.GetAsync("https://mystore.local/api/weatherforecast");
            //if (!response.IsSuccessStatusCode)
            //{
            //    Output(ConsoleColor.Red, () => { Console.WriteLine(response.StatusCode); });
            //}
            //else
            //{
            //    var content = await response.Content.ReadAsStringAsync();
            //    Output(ConsoleColor.White, () =>
            //    {
            //        Output(ConsoleColor.White, () => { Console.WriteLine($"\n\n[{JArray.Parse(content)}]"); });
            //        Console.WriteLine($"\n\nSecure access Response recieved.");
            //    });
            //}

            var response = await apiClient.GetAsync("https://mystore.local/store/weatherforecast");
            if (!response.IsSuccessStatusCode)
            {
                Output(ConsoleColor.Red, () => { Console.WriteLine(response.StatusCode); });
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Output(ConsoleColor.White, () =>
                {
                    Output(ConsoleColor.White, () => { Console.WriteLine($"\n\n[{JArray.Parse(content)}]"); });
                    Console.WriteLine($"\n\nSite Weather Forecast Response recieved.");
                });
            }

            //Console.WriteLine("\n\nCalling API without the token: ...");
            //apiClient.SetBearerToken(null);
            
            //Console.WriteLine($"\n\nExpecting an error on next call");
 
            //response = await apiClient.GetAsync("https://localhost:6001/weatherforecast");
            //if (!response.IsSuccessStatusCode)
            //{
            //    Output(ConsoleColor.White, () => { Console.WriteLine($"\n\nAs expected the response was : [{response.StatusCode}]"); });
            //}
            //else
            //{
            //    var content = await response.Content.ReadAsStringAsync();
            //    Output(ConsoleColor.White, () =>
            //    {
            //        Output(ConsoleColor.Red, () => { Console.WriteLine($"\n\n[{JArray.Parse(content)}]"); });
            //        Console.WriteLine($"\n\nAPI Weather Forecast Response recieved.");
            //    });
            //}




            Output(ConsoleColor.Yellow, () => { Console.Write("\n\nPress any key to terminate: "); });
            Console.ReadKey(true);
            Console.WriteLine("API test client, completed.");

        }

        static void Output(ConsoleColor color, Action act)
        {
            var currentColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            act.Invoke();
            Console.ForegroundColor = currentColor;
        }
    }
}
