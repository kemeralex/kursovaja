using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // Initialize HttpClient and CookieContainer
        var cookies = new CookieContainer();
        var handler = new HttpClientHandler { CookieContainer = cookies };
        var client = new HttpClient(handler);

        // Start the application
        await StartApplication(client);
    }

    static async Task StartApplication(HttpClient client)
    {
        Console.WriteLine("====================================");
        Console.WriteLine("Welcome to the array manipulation application!");
        Console.WriteLine("====================================");

        // Connect to the server
        bool isConnected = await ConnectToServer(client);
        if (!isConnected)
        {
            Console.WriteLine("Failed to connect to the server. Exiting...");
            return;
        }

        // Authenticate the user (login or register)
        bool isAuthenticated = await AuthenticateUser(client);
        if (!isAuthenticated)
        {
            Console.WriteLine("Failed to authenticate. Exiting...");
            return;
        }

        // Main menu
        await MainMenu(client);
    }

    static async Task<bool> ConnectToServer(HttpClient client)
    {
        Console.WriteLine("Enter the server URL (default is http://localhost:5000):");
        string serverUrl = Console.ReadLine();
        if (string.IsNullOrEmpty(serverUrl))
        {
            serverUrl = "http://localhost:5000";
        }

        try
        {
            client.BaseAddress = new Uri(serverUrl);
            var response = await client.GetAsync("test");
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Successfully connected to the server.");
                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error connecting to the server: {ex.Message}");
        }
        return false;
    }

    static async Task<bool> AuthenticateUser(HttpClient client)
    {
        Console.WriteLine("------------------------------------");
        Console.WriteLine("1. Login");
        Console.WriteLine("2. Register");
        Console.WriteLine("------------------------------------");
        int choice = GetValidIntInput("Choose an option:", 1, 2);

        if (choice == 1)
        {
            return await LoginUser(client);
        }
        else
        {
            return await RegisterUser(client);
        }
    }

    static async Task<bool> LoginUser(HttpClient client)
    {
        Console.WriteLine("------------------------------------");
        Console.WriteLine("Enter your username:");
        string username = Console.ReadLine();
        Console.WriteLine("Enter your password:");
        string password = Console.ReadLine();

        var response = await client.PostAsync($"/login?login={username}&password={password}", null);
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("Login successful.");
            return true;
        }
        else
        {
            Console.WriteLine("Invalid username or password.");
            return false;
        }
    }

    static async Task<bool> RegisterUser(HttpClient client)
    {
        Console.WriteLine("------------------------------------");
        Console.WriteLine("Enter a username:");
        string username = Console.ReadLine();
        Console.WriteLine("Enter a password:");
        string password = Console.ReadLine();

        var response = await client.PostAsync($"/SignUp?login={username}&password={password}", null);
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("Registration successful. Please log in.");
            return await LoginUser(client);
        }
        else
        {
            Console.WriteLine("Registration failed. Please check your input.");
            return false;
        }
    }

    static async Task MainMenu(HttpClient client)
    {
        bool exit = false;
        while (!exit)
        {
            Console.WriteLine("====================================");
            Console.WriteLine("Main Menu:");
            Console.WriteLine("1. Generate array automatically");
            Console.WriteLine("2. Create array manually");
            Console.WriteLine("3. Sort array");
            Console.WriteLine("4. Display array");
            Console.WriteLine("5. Get a subarray");
            Console.WriteLine("6. Get an array element");
            Console.WriteLine("7. Edit an array element");
            Console.WriteLine("8. Add an element to the array");
            Console.WriteLine("9. Delete the array");
            Console.WriteLine("10. Request history");
            Console.WriteLine("11. Change password");
            Console.WriteLine("12. Exit");
            Console.WriteLine("====================================");

            int choice = GetValidIntInput("Choose an option:", 1, 12);

            switch (choice)
            {
                case 1:
                    await GenerateArrayAutomatically(client);
                    break;
                case 2:
                    await CreateArrayManually(client);
                    break;
                case 3:
                    await SortArray(client);
                    break;
                case 4:
                    await DisplayArray(client);
                    break;
                case 5:
                    await GetSubArray(client);
                    break;
                case 6:
                    await GetArrayElement(client);
                    break;
                case 7:
                    await EditArrayElement(client);
                    break;
                case 8:
                    await AddElementToArray(client);
                    break;
                case 9:
                    await DeleteArray(client);
                    break;
                case 10:
                    await ManageRequestHistory(client);
                    break;
                case 11:
                    await ChangePassword(client);
                    break;
                case 12:
                    exit = true;
                    Console.WriteLine("Exiting the program...");
                    break;
            }
        }
    }

    static async Task GenerateArrayAutomatically(HttpClient client)
    {
        Console.WriteLine("------------------------------------");
        Console.WriteLine("Automatic array generation:");
        int length = GetValidIntInput("Enter the array length:", 1);
        int lowerBound = GetValidIntInput("Enter the lower bound:");
        int upperBound = GetValidIntInput("Enter the upper bound:", lowerBound + 1);

        string request = $"/Generate_array?len={length}&lb={lowerBound}&ub={upperBound}";
        var (response, success) = await SendRequestAsync(client, request, "POST", "message");
        if (success)
        {
            Console.WriteLine(response);
        }
        else
        {
            Console.WriteLine("Error generating the array.");
        }
    }

    static async Task CreateArrayManually(HttpClient client)
    {
        Console.WriteLine("------------------------------------");
        Console.WriteLine("Manual array creation:");
        int length = GetValidIntInput("Enter the array length:", 1);
        int[] array = new int[length];
        for (int i = 0; i < length; i++)
        {
            array[i] = GetValidIntInput($"Enter element {i + 1}:");
        }
        string arrayString = string.Join(",", array);
        string request = $"/Create_array?array={arrayString}";
        var (response, success) = await SendRequestAsync(client, request, "POST", "message");
        if (success)
        {
            Console.WriteLine(response);
        }
        else
        {
            Console.WriteLine("Error creating the array.");
        }
    }

    static async Task SortArray(HttpClient client)
    {
        Console.WriteLine("------------------------------------");
        Console.WriteLine("Sorting the array:");
        var (response, success) = await SendRequestAsync(client, "/Sort_array", "POST", "message");
        if (success)
        {
            Console.WriteLine(response);
        }
        else
        {
            Console.WriteLine("Error sorting the array.");
        }
    }

    static async Task DisplayArray(HttpClient client)
    {
        Console.WriteLine("------------------------------------");
        Console.WriteLine("Displaying the array:");
        var (response, success) = await SendRequestAsync(client, "/Get_array", "GET", "message&array");
        if (success)
        {
            Console.WriteLine(response);
        }
        else
        {
            Console.WriteLine("Error retrieving the array.");
        }
    }

    static async Task GetSubArray(HttpClient client)
    {
        Console.WriteLine("------------------------------------");
        Console.WriteLine("Getting a subarray:");
        int lowerIndex = GetValidIntInput("Enter the lower index:");
        int upperIndex = GetValidIntInput("Enter the upper index:", lowerIndex);

        string request = $"/Get_part_array?low_ind={lowerIndex}&up_ind={upperIndex}";
        var (response, success) = await SendRequestAsync(client, request, "GET", "message&array");
        if (success)
        {
            Console.WriteLine(response);
        }
        else
        {
            Console.WriteLine("Error retrieving the subarray.");
        }
    }

    static async Task GetArrayElement(HttpClient client)
    {
        Console.WriteLine("------------------------------------");
        Console.WriteLine("Getting an array element:");
        int index = GetValidIntInput("Enter the element index:");
        string request = $"/Get_element?element={index}";
        var (response, success) = await SendRequestAsync(client, request, "GET", "message&singlevalue");
        if (success)
        {
            Console.WriteLine(response);
        }
        else
        {
            Console.WriteLine("Error retrieving the array element.");
        }
    }

    static async Task EditArrayElement(HttpClient client)
    {
        Console.WriteLine("------------------------------------");
        Console.WriteLine("Editing an array element:");
        int index = GetValidIntInput("Enter the element index:");
        int value = GetValidIntInput("Enter the new value:");
        string request = $"/Edit_array?index={index}&value={value}";
        var (response, success) = await SendRequestAsync(client, request, "PATCH", "message");
        if (success)
        {
            Console.WriteLine(response);
        }
        else
        {
            Console.WriteLine("Error editing the array element.");
        }
    }

    static async Task AddElementToArray(HttpClient client)
    {
        Console.WriteLine("------------------------------------");
        Console.WriteLine("Adding an element to the array:");
        int value = GetValidIntInput("Enter the element value:");
        Console.WriteLine("1. At the beginning");
        Console.WriteLine("2. At the end");
        Console.WriteLine("3. After a specified index");
        int positionChoice = GetValidIntInput("Choose the position:", 1, 3);

        string position = positionChoice switch
        {
            1 => "beginning",
            2 => "end",
            3 => "after",
            _ => "beginning"
        };

        int index = -1;
        if (positionChoice == 3)
        {
            index = GetValidIntInput("Enter the index:");
        }

        string request = $"/Add_element?element={value}&position={position}&index={index}";
        var (response, success) = await SendRequestAsync(client, request, "PATCH", "message");
        if (success)
        {
            Console.WriteLine(response);
        }
        else
        {
            Console.WriteLine("Error adding the element to the array.");
        }
    }

    static async Task DeleteArray(HttpClient client)
    {
        Console.WriteLine("------------------------------------");
        Console.WriteLine("Deleting the array:");
        var (response, success) = await SendRequestAsync(client, "/Delete_array", "DELETE", "message");
        if (success)
        {
            Console.WriteLine(response);
        }
        else
        {
            Console.WriteLine("Error deleting the array.");
        }
    }

    static async Task ManageRequestHistory(HttpClient client)
    {
        Console.WriteLine("------------------------------------");
        Console.WriteLine("Request history management:");
        Console.WriteLine("1. Get history");
        Console.WriteLine("2. Clear history");
        int choice = GetValidIntInput("Choose an option:", 1, 2);

        if (choice == 1)
        {
            var (response, success) = await SendRequestAsync(client, "/Get_history", "GET", "history");
            if (success)
            {
                Console.WriteLine(response);
            }
            else
            {
                Console.WriteLine("Error retrieving the request history.");
            }
        }
        else
        {
            var (response, success) = await SendRequestAsync(client, "/Clear_history", "DELETE", "message");
            if (success)
            {
                Console.WriteLine(response);
            }
            else
            {
                Console.WriteLine("Error clearing the request history.");
            }
        }
    }

    static async Task ChangePassword(HttpClient client)
    {
        Console.WriteLine("------------------------------------");
        Console.WriteLine("Changing password:");
        Console.WriteLine("Enter your current password:");
        string oldPassword = Console.ReadLine();
        Console.WriteLine("Enter your new password:");
        string newPassword = Console.ReadLine();

        var (username, _) = await SendRequestAsync(client, "/current_user", "GET", "message");
        string request = $"/Change_password?login={username}&oldPassword={oldPassword}&newPassword={newPassword}";
        var (response, success) = await SendRequestAsync(client, request, "PATCH", "message");
        if (success)
        {
            Console.WriteLine(response);
            Console.WriteLine("Please log in with your new password.");
            await LoginUser(client);
        }
        else
        {
            Console.WriteLine("Error changing the password.");
        }
    }

    static int GetValidIntInput(string prompt, int minValue = int.MinValue, int maxValue = int.MaxValue)
    {
        int result;
        while (true)
        {
            Console.WriteLine(prompt);
            if (int.TryParse(Console.ReadLine(), out result) && result >= minValue && result <= maxValue)
            {
                return result;
            }
            Console.WriteLine($"Invalid input. Please enter a number between {minValue} and {maxValue}.");
        }
    }

    static async Task<(string, bool)> SendRequestAsync(HttpClient client, string request, string method, string responseType)
    {
        HttpResponseMessage response;
        switch (method.ToUpper())
        {
            case "POST":
                response = await client.PostAsync(request, null);
                break;
            case "GET":
                response = await client.GetAsync(request);
                break;
            case "PATCH":
                var patchRequest = new HttpRequestMessage(new HttpMethod("PATCH"), request);
                response = await client.SendAsync(patchRequest);
                break;
            case "DELETE":
                response = await client.DeleteAsync(request);
                break;
            default:
                throw new ArgumentException("Invalid request method.");
        }

        string responseText = await response.Content.ReadAsStringAsync();
        if (responseType == "history")
        {
            if (responseText == "[]")
            {
                Console.WriteLine("History is empty.");
                return (responseText, response.IsSuccessStatusCode);
            }
            var history = JsonSerializer.Deserialize<List<ResponseForHistory>>(responseText);
            foreach (var item in history)
            {
                Console.WriteLine($"Operation: {item.operation}");
                Console.WriteLine($"Parameters: {item.parameters}");
                Console.WriteLine($"Result: {item.result}");
                Console.WriteLine();
            }
            return ("", true);
        }
        else
        {
            var serverResponse = JsonSerializer.Deserialize<ServerResponse>(responseText);
            string output = serverResponse.message;
            if (serverResponse.values != null)
            {
                output += " " + string.Join(", ", serverResponse.values);
            }
            if (serverResponse.singleValue != null)
            {
                output += " " + serverResponse.singleValue;
            }
            return (output, response.IsSuccessStatusCode);
        }
    }
}