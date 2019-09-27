using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DockerFunctionsDemo
{
    public static class ConvertToRoman
    {
        [FunctionName("ConvertToRoman")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            //value from get
            string parameter = req.Query["number"];
            
            //value from post
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            parameter = parameter ?? data?.number;

            int number;

            if (int.TryParse(parameter, out number)) //is numeric..
            {
                string result = "Your Roman number is: " + ConvertToRoman.ToRoman(number);
                return (ActionResult)new OkObjectResult(result);
            }

            return new BadRequestObjectResult("Please pass a number on the query string or in the request body");
        }

        public static string ToRoman(int number)
        {
            var result = string.Empty;

            var romanDictionary = new Dictionary<int, string>()
            {
                { 1000, "M" },
                { 900, "CM"},
                { 500, "D" },
                { 400, "CD" },
                { 100, "C"},
                { 90, "XC"},
                { 50, "L"},
                { 40, "XL"},
                { 10, "X" },
                { 9, "IX" },
                { 5, "V" },
                { 4, "IV" },
                { 1, "I" }
            };

            foreach (var roman in romanDictionary)
            {
                //calculate the rest
                while (number % roman.Key < number)
                {
                    //add the matching roman number to the result string
                    result += roman.Value;
                    //subtract the decimal value of the roman number 
                    number -= roman.Key;
                }
            }
            return result;
        }
    }
}
