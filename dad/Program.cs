//Elad_Abramov - 201620341

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace dad
{
    public class Program
    {
        //readonly string domain = "YOUR_DOMAIN";
        //readonly string toAddress = "YOUR_EMAIL";
        //readonly string subject = "testing one testing..";
        //readonly string text = "Hello everybody";
        //readonly string from = "YOUR_DOMAIN";
        //readonly HttpClient client = new HttpClient();

        public static async Task Main(string[] args)
        {
            string mailKey = "API_KEY";
            string con = "/v3/YOUR_DOMAIN/messages";

            Console.WriteLine("Please enter your Email:");
            string email = Console.ReadLine();
            Console.WriteLine("The user email is: " + email);
            Console.WriteLine("Now fetching a random joje from api:");
            DadJokeService dad = new DadJokeService();
            string j = dad.GetRandomJoke();
            Console.WriteLine("The joke is: " + "\n" + j);
            Program p = new Program();
            await p.sendEmailAsync(mailKey, j, email, con);
            Console.WriteLine("Sending joke to the given email..completed.");

        }

        public async Task sendEmailAsync(string _mailKey, string _j, string _email, string _con)
        {
            try
            {
                using (var client = new HttpClient { BaseAddress = new Uri($"https://api.eu.mailgun.net" + _con) })
                {

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(Encoding.ASCII.GetBytes($"api:{_mailKey}")));

                    var content = new FormUrlEncodedContent(new[] {
                      new KeyValuePair<string, string>("from", "<mailgun@YOUR_DOMAIN>"),
                      new KeyValuePair<string, string>("to", _email),
                      new KeyValuePair<string, string>("subject", "Dad Joke"),
                      new KeyValuePair<string, string>("text", _j)
                    });
                    var result = await client.PostAsync("messages", content);
                }

            }
            catch (Exception e)
            {
                var ex = e;
                Console.WriteLine("Exception: " + ex);
            }

        }





    }

    public class DadJokeService
        {
            private readonly HttpClient client;

            public DadJokeService()
            {
                client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.BaseAddress = new Uri("https://icanhazdadjoke.com");
            }

            public string GetRandomJoke()
            {
                HttpResponseMessage response = client.GetAsync("/").Result;
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = response.Content.ReadAsStringAsync().Result;
                    JokeResponse jokeResponse = JsonSerializer.Deserialize<JokeResponse>(jsonResponse);
                    return jokeResponse.Content;
                }
                else
                {
                    throw new HttpRequestException(string.Format("{0} ({1})", response.StatusCode, response.ReasonPhrase));
                }
            }


            private class JokeResponse
            {
                [JsonPropertyName("joke")]
                public string Content { get; set; }

                [JsonPropertyName("id")]
                public string Id { get; set; }

                [JsonPropertyName("status")]
                public int Status { get; set; }
            }
        }
}


   
