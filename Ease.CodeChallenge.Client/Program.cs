using System.Text;
using System.Text.Json;

namespace Ease.CodeChallenge.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("This is the code challenge Client APP");
            Console.WriteLine("Author - Jonathan Estrada Betancur \n");

            //Waiting for API is up and running
            await Task.Delay(2000);
            await SeedInitialData();

            string running;
            do
            {
                Console.WriteLine("Select one of the following options:");
                Console.WriteLine("1. Retrieve all guids");
                Console.WriteLine("2. Retrieve a guid given an specific ID");
                Console.WriteLine("3. Create a new guid");
                Console.WriteLine("4. Update an existing guid");
                Console.WriteLine("5. Delete a record");
                Console.Write("\nEnter the option: ");
                var opt = Console.ReadLine();
                switch (opt)
                {
                    case "1":
                        await GetAllRecords();
                        break;

                    case "2":

                        Console.Write("Enter the GUID: ");
                        var guid = Console.ReadLine();
                        await GetRecordByGuid(guid);
                        break;

                    case "3":

                        Console.Write("Enter the GUID (optional): ");
                        var newGuid = Console.ReadLine();
                        Console.Write("Enter the Expires Date (optional) - (yyyy-MM-dd): ");
                        var newExpiresDate = Console.ReadLine();
                        Console.Write("Enter the user: ");
                        var newUser = Console.ReadLine();
                        await CreateGuid(newGuid, newExpiresDate, newUser);
                        break;

                    case "4":

                        Console.Write("Enter the GUID: ");
                        var updateGuid = Console.ReadLine();
                        Console.Write("Enter the Expires Date (yyyy-MM-dd): ");
                        var updateExpiresDate = Console.ReadLine();
                        await UpdateGuid(updateGuid, updateExpiresDate);
                        break;

                    case "5":

                        Console.Write("Enter the GUID: ");
                        var deleteGuid = Console.ReadLine();
                        await DeleteGuid(deleteGuid);
                        break;


                    default:
                        Console.WriteLine("Invalid option");
                        break;
                }

                Console.Write("Do you want to continue (Y/N): ");
                running = Console.ReadLine();
                Console.WriteLine("");
            } while (running.Equals("Y", StringComparison.OrdinalIgnoreCase));
        }


        static async Task SeedInitialData()
        {
            Console.WriteLine("Initial Seeding.... \n");
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://localhost:7258/");

            //Generates 10 records for testing
            for (int i = 1; i <= 10; i++)
            {
                var metadata = new UserMetadataDto
                {
                    Guid = Guid.NewGuid().ToString("N").ToUpper(),
                    Expires = DateTime.Now.AddMinutes(5),
                    User = $"User {i}"
                };

                var jsonObject = JsonSerializer.Serialize(metadata);
                var content = new StringContent(jsonObject, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("/guid", content);
                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(jsonContent);
                }
            }

            Console.WriteLine("\nSeeding Complete.... \n");
        }

        static async Task GetAllRecords()
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://localhost:7258/");

            var response = await httpClient.GetAsync("/guid");
            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(jsonContent);
                Console.WriteLine("-------------------------------------------------------------------\n");
            }
        }

        static async Task GetRecordByGuid(string guid)
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://localhost:7258/");

            var response = await httpClient.GetAsync($"/guid/{guid}");
            var jsonContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine(jsonContent);
            Console.WriteLine("-------------------------------------------------------------------\n");
        }

        static async Task CreateGuid(string guid, string expire, string user)
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://localhost:7258/");

            var isValidDate = DateTime.TryParseExact(expire, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime expiration);
            var metadata = new UserMetadataDto
            {
                Guid = guid,
                Expires = isValidDate ? expiration : null,
                User = user
            };

            var jsonObject = JsonSerializer.Serialize(metadata);
            var content = new StringContent(jsonObject, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("/guid", content);
            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(jsonContent);
                Console.WriteLine("-------------------------------------------------------------------\n");
            }
        }

        static async Task UpdateGuid(string guid, string expire)
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://localhost:7258/");

            var isValidDate = DateTime.TryParseExact(expire, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime expiration);
            var metadata = new UserMetadataDto
            {
                Expires = isValidDate ? expiration : null,
            };

            var jsonObject = JsonSerializer.Serialize(metadata);
            var content = new StringContent(jsonObject, Encoding.UTF8, "application/json");

            var response = await httpClient.PutAsync($"/guid/{guid}", content);
            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(jsonContent);
                Console.WriteLine("-------------------------------------------------------------------\n");
            }
        }

        static async Task DeleteGuid(string guid)
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://localhost:7258/");

            var response = await httpClient.DeleteAsync($"/guid/{guid}");
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Guid deleted succesfully!");
                Console.WriteLine("-------------------------------------------------------------------\n");
            }
        }
    }
}
