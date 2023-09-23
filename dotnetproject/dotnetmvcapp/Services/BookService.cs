using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using BookStoreApp.Models;

namespace BookStoreApp.Services
{
    public interface IBookService
    {
        bool AddBook(Book book);
        List<Book> GetAllBooks();
        Book GetBookById(int id);
        bool DeleteBook(int id);
    }
    public class BookService : IBookService
    {
        private readonly HttpClient _httpClient;
        public BookService(HttpClient httpClient, IConfiguration configuration)
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            _httpClient = new HttpClient(clientHandler);
            var apiSettings = configuration.GetSection("ApiSettings").Get<ApiSettings>();
            _httpClient.BaseAddress = new Uri(apiSettings.BaseUrl);
        }

        public bool AddBook(Book book)
        {
            try
            {
                var json = JsonConvert.SerializeObject(book);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + $"/Book", content).Result;

                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }

        public List<Book> GetAllBooks()
        {
            try
            {
                HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "/Book").Result;

                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<List<Book>>(data);
                }

                return new List<Book>();
            }
            catch (HttpRequestException)
            {
                return new List<Book>();
            }
        }

        public Book GetBookById(int id)
        {
            try
            {
                HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + $"/Book/{id}").Result;

                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<Book>(data);
                }

                return null;
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        public bool DeleteBook(int id)
        {
            try
            {
                HttpResponseMessage response = _httpClient.DeleteAsync(_httpClient.BaseAddress + $"/Book/{id}").Result;

                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }
    }
}
