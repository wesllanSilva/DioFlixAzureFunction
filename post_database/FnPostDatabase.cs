using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace PostDatabase
{
    public class FnPostDatabase
    {
        private readonly ILogger<FnPostDatabase> _logger;

        public FnPostDatabase(ILogger<FnPostDatabase> logger)
        {
            _logger = logger;
        }

        [Function("movie")]
        [CosmosDBOutput("%DatabaseName%", "movies", Connection = "CosmosDBConnection", CreateIfNotExists = true)]
        public async Task<object?> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            MovieRequest movie = null;            

            try
            {
                var content = await new StreamReader(req.Body).ReadToEndAsync();
                movie = JsonConvert.DeserializeObject<MovieRequest>(content);                
            }
            catch (Exception ex)
            {                
                return new BadRequestObjectResult("Erro ao deserializar o objeto: " + ex.Message);   
            }

            return JsonConvert.SerializeObject(movie);
        }
    }
}
