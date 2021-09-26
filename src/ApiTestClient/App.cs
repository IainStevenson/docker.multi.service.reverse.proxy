using IdentityModel.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ApiTestClient
{
    public class App
    {
        private readonly ILogger<App> _logger;
        private readonly AppSettings _appSettings;
        private readonly IHttpClientFactory _clientFactory;
        private TokenResponse _tokenResponse;

        public App(IOptions<AppSettings> appSettings, ILogger<App> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _appSettings = appSettings?.Value ?? throw new ArgumentNullException(nameof(appSettings));
            _clientFactory = clientFactory;
        }

        public async Task Run(string[] args)
        {
            _logger.LogInformation("API Test Client Starting...");
            _logger.LogInformation("Initialising...");
            _logger.LogInformation($"Interacts with the {_appSettings.Authority} to obtain an access token");
            _logger.LogInformation("Then contacts the API directly using the token.");
            Thread.Sleep(100); // wait for logger to catch up
            Console.Write("\n\nEnsure the api and identity server end points are started and available, and then press any key: ");
            Console.ReadKey(true);
            Console.WriteLine();


            _logger.LogInformation("Obtaining Access Token");
            _logger.LogInformation("Requesting Token from the Identity Service: ...");


            _tokenResponse = null;
            using (var tokenClient = _clientFactory.CreateClient())
            {
                if (!await GetToken(tokenClient))
                {   // get token failed
                    if (_tokenResponse == null)
                    {
                        _logger.LogError($"Complete failure to aquire token! Is the token Endpoint {_appSettings.Authority} active?");
                    }
                    else
                    {
                        _logger.LogError("Invalid Token response recieved.");
                        _logger.LogError($"Token Error: {_tokenResponse.Error}");
                        _logger.LogError(JsonConvert.SerializeObject(_tokenResponse));
                    }
                }
                else
                {
                    _logger.LogInformation("Valid Token recieved.");

                    using var apiClient = _clientFactory.CreateClient();
                    {
                        _logger.LogInformation("Using Access Token in new Http Client");
                        apiClient.SetBearerToken(_tokenResponse.AccessToken);

                        Thread.Sleep(100); // wait for logger to catch up
                        Console.Write("\n\nPress any key to use the token to call the API: ");
                        Console.ReadKey(true);
                        Console.WriteLine();

                        _logger.LogInformation("Calling Identity API with token: ...");

                        if (!await CallApi(apiClient, new Uri(_appSettings.IdentityApiUri)))
                        {
                            // Identity call failed!
                            _logger.LogError($"Call to verify Identity from API has failed!");
                            _logger.LogError(JsonConvert.SerializeObject(_tokenResponse));
                        }
                        else
                        {
                            _logger.LogInformation("Identity verifided.");

                            using var apiClient2 = _clientFactory.CreateClient();
                            {
                                apiClient2.SetBearerToken(_tokenResponse.AccessToken);
                                if (!await CallApi(apiClient2, new Uri(_appSettings.DataApiUri)))
                                {
                                    _logger.LogError(
                                        $"Call to {_appSettings.DataApiUri} has unexpectedly failed!"
                                        );
                                }
                                else
                                {
                                    Thread.Sleep(100); // wait for logger to catch up
                                    Console.Write("\n\nPress any key to NEGLECT to use the token to call the API: ");
                                    Console.ReadKey(true);
                                    Console.WriteLine();
                                    using var apiClient3 = _clientFactory.CreateClient();
                                    {
                                        // NOTE: No token is set
                                        _logger.LogInformation("Calling API without the token: ...");
                                        _logger.LogInformation($"Expecting an error on next call");

                                        if (await CallApi(apiClient3, new Uri(_appSettings.DataApiUri)))
                                        {
                                            _logger.LogError(
                                            $"Call to Data has unexpectedly succeeded, Check Endpoint {_appSettings.DataApiUri} is secured!"
                                            );

                                        }
                                        else
                                        {
                                            // data call failed as expected
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Finished
            _logger.LogInformation("Finished!");
            Thread.Sleep(100); // wait for logger to catch up
            Console.Write("\n\nPress any key to terminate: ");
            Console.ReadKey(true);
            await Task.CompletedTask;
        }


        private async Task<bool> CallApi(HttpClient apiClient, Uri uri)
        {
            var response = await apiClient.GetAsync(uri);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"Bad response, code : [{response.StatusCode}]");
                return false;
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"API {uri.AbsoluteUri} Response recieved.");
                _logger.LogInformation($"[{JArray.Parse(content)}]");
                return true;
            }
        }


        private async Task<bool> GetToken(HttpClient client)
        {

            // discover endpoints from metadata            
            var disco = await client.GetDiscoveryDocumentAsync(_appSettings.Authority);

            if (disco.IsError)
            {
                _logger.LogInformation(disco.Error);
                return false;
            }
            // request token
            _tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = _appSettings.ClientId,
                ClientSecret = _appSettings.ClientSecret,
                Scope = _appSettings.Scope
            });

            return !_tokenResponse.IsError;


        }

    }
}
