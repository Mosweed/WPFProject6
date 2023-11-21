using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using kassasysteem.Models;
using  static System.Net.WebRequestMethods;

namespace kassasysteem.Helper
{
    public static class ApiHelper
    {
        private static HttpClient _client;

        public static HttpClient GetClient()
        {
            if (_client == null)
            {
                _client = new HttpClient()
                {
                    BaseAddress = new Uri("http://localhost:8000")


                };
            }

            return _client;
        }


        public static async Task<Access> GetAccess(string username, string password)
        {

            try
            {
                var response = await GetClient().PostAsJsonAsync("http://localhost:8000/api/login", new MyLoginRequest { email = username, password = password });
                if (response.IsSuccessStatusCode && response.Content.Headers.ContentType.MediaType == "application/json")
                {
                    Access access = await response.Content.ReadAsAsync<Access>();
                    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access.AccessToken);
                    return access;
                }
                else
                {
                    return null;
                }


            }
            catch (Exception)
            {

                return null;
            }
        }




        public static async Task<List<ShopCart>> GetProducts(int drawer_id)
        {

            var response = await GetClient().GetAsync($"http://localhost:8000/api/shopcart?drawer_id={drawer_id}");

            if (response.IsSuccessStatusCode && response.Content.Headers.ContentType.MediaType == "application/json")
            {

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsAsync<List<ShopCart>>();

            }
            else
            {
                return null;
            }





        }

        public static async Task<bool> Postitem(int drawer_id, int barCode, int quantity)
        {

            try
            {
                var response = await GetClient().PostAsJsonAsync("http://localhost:8000/api/shopcart", new ShopcartRequest { drawer_id = drawer_id, barcode = barCode, quantity = quantity });
                if (response.IsSuccessStatusCode && response.Content.Headers.ContentType.MediaType == "application/json")
                {


                    return true;
                }
                else
                {
                    return false;
                }


            }
            catch (Exception)
            {

                return false;
            }
        }


        public static async Task<bool> PostOrder(int drawer_id, decimal total , int customer_number)
        {

            try
            {
                var response = await GetClient().PostAsJsonAsync("http://localhost:8000/api/Neworder", new MyOrderRequest { drawer_id = drawer_id, total = total , customer_number = customer_number });
                if (response.IsSuccessStatusCode)
                {
                    // MessageBox.Show("Order is succesvol geplaatst");
                    return true;
                }
                else
                {
                    return false;
                }

            }

            catch (Exception)
            {
                return false;
            }

        }


        public static async Task<bool> DeleteItem(int id)
        {
            try
            {
                var response = await GetClient().DeleteAsync($"http://localhost:8000/api/shopcart/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
