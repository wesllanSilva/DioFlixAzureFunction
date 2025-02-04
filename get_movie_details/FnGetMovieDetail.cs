using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace GetMovieDetail
{
    public class FnGetMovieDetail
    {
        private readonly ILogger<FnGetMovieDetail> _logger;
        private readonly CosmosClient _cosmosClient;

        public FnGetMovieDetail(ILogger<FnGetMovieDetail> logger, CosmosClient cosmosClient)
        {
            _logger = logger;
            _cosmosClient =  cosmosClient;
        }

        [Function("detail")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

             var container = _cosmosClient.GetContainer("DioFlixDB", "movies");
             var id = req.Query["id"];
             var query = $"SELECT * FROM c WHERE c.id = @id";
             var queryDefinition = new QueryDefinition(query).WithParameter("@id", id);
             var result = container.GetItemQueryIterator<MovieResult>(queryDefinition);
             var results = new List<MovieResult>(); 
             while(result.HasMoreResults) 
             {
                foreach(var item in await result.ReadNextAsync())
                {
                    results.Add(item);
                }
             }

             var responseMessage = req.CreateResponse(System.Net.HttpStatusCode.OK);
             await responseMessage.WriteAsJsonAsync(results.FirstOrDefault());

             return responseMessage;            
        }
    }
}
