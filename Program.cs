using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace DataStreamGenerator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string token = "";
            while (token.Length == 0)
            {
                token = await LoginAsync();
            }

            var connection = new HubConnectionBuilder().WithUrl("http://localhost:12165/Hubs/MessageHub", options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(token);
            }).Build();
            await connection.StartAsync();

            await connection.SendAsync("SetOnline", "stream");

            var oprator = "";
            while(oprator != "n")
            {
                Console.WriteLine("Please input count of data");
                var count = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Please input speed of data generating(ms)");
                var delay = Convert.ToInt32(Console.ReadLine());
                await connection.SendAsync("UploadStream", clientStreamData(count,delay));
                oprator = Console.ReadLine();
            }      
        }

        static async Task<string> LoginAsync()
        {
            Console.WriteLine("Please input Email");
            var email = Console.ReadLine();
            Console.WriteLine("Please input Password");
            var password = Console.ReadLine();

            HttpClient client = new HttpClient();
            var data = "{\"email\":\"" + email + "\",\"password\":\"" + password + "\"}";
            //Console.WriteLine(data);
            StringContent stringContent = new StringContent(data);
            stringContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync("http://localhost:12165/Account/Login",stringContent);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return "";
            }
            return await response.Content.ReadAsStringAsync();
        }


        async static IAsyncEnumerable<string> clientStreamData(int count,int delay)
        {
            for (var i = 0; i < count; i++)
            {
                await Task.Delay(delay);
                //Random rand = new Random();
                //var data = (rand.Next() % 10000).ToString();
                var data = i.ToString();
                Console.WriteLine("send: " + data);
                yield return data;  
            }
            Console.WriteLine("Continue to send(y/n)");
        }
    }
}
