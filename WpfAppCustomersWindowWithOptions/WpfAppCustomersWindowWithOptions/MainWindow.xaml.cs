using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Http;
using Newtonsoft.Json;
using ASP.NET_Core_Web_API_project.Models;
using ASP.NET_Core_Web_API_project;
using ASP.NET_Core_Web_API_project.Controllers;

namespace WpfAppCustomersWindowWithOptions
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async Task<int> CustomerCurrentIDcount()
        {
            int currentID = 0;
            string url = "/customers";
            string method = "GET";
            string result = await SendHttpRequestAsync(url, method);
            List<Customer> customers = JsonConvert.DeserializeObject<List<Customer>>(result);
            foreach (Customer customer in customers)
            {
                if (currentID < customer.CustomerID)
                {
                    currentID = customer.CustomerID;
                }
            }
            return currentID;
        }


        private async Task<string> SendHttpRequestAsync(string url, string method, string data = null)
        {
            string result = "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5000/");
                    HttpRequestMessage request = new HttpRequestMessage(new HttpMethod(method), url);
                    if (data != null)
                    {
                        request.Content = new StringContent(data, Encoding.UTF8, "application/json");
                    }
                    HttpResponseMessage response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    result = await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
            return result;
        }

        private async void GetAllCustomers_Click(object sender, RoutedEventArgs e)
        {
            string url = "/customers";
            string method = "GET";
            string result = await SendHttpRequestAsync(url, method);
            List<Customer> customers = JsonConvert.DeserializeObject<List<Customer>>(result);
            foreach (Customer customer in customers)
            {
                CustomersListBox.Items.Add(JsonConvert.SerializeObject(customer));

            }
        }

        private async void GetSpecificCustomer_Click(object sender, RoutedEventArgs e)
        {
            string customerId = CustomerIdTextBox.Text;
            string url = $"/customers/{customerId}";
            string method = "GET";
            string result = await SendHttpRequestAsync(url, method);
            Customer customer = JsonConvert.DeserializeObject<Customer>(result);
            CustomersListBox.Items.Add(JsonConvert.SerializeObject(customer));
        }


        private async void GetCustomersWithNumericField_Click(object sender, RoutedEventArgs e)
        {
            string numericFieldValueString = CustomerNumericFieldTextBox.Text;
            int numericFieldValue;
            string url = "/customers";
            string method = "GET";
            string result = await SendHttpRequestAsync(url, method);
            List<Customer> customers = JsonConvert.DeserializeObject<List<Customer>>(result);

            if (!(int.TryParse(numericFieldValueString, out numericFieldValue)))
            {
                MessageBox.Show("Not a valid ID number");
                return;

            }

            foreach (Customer customer in customers)
            {
                if (customer.CustomerID > numericFieldValue)
                {
                    CustomersListBox.Items.Add(JsonConvert.SerializeObject(customer));
                }
            }
        }

        private async void AddNewCustomer_Click(object sender, RoutedEventArgs e)
        {
            int newCustomerID = (await CustomerCurrentIDcount()) + 1;
            string customerName = NewCustomerNameTextBox.Text;
            string customerAddress = NewCustomerAddressTextBox.Text;
            string customerPhone = NewCustomerPhoneTextBox.Text;

            string url = "/customers";
            string method = "POST";
            string data = "{\"CustomerID\": \"" + newCustomerID + "\",\"Name\": \"" + customerName + "\", \"Address\": \"" + customerAddress + "\", \"Phone\": \"" + customerPhone + "\"}";
            string result = await SendHttpRequestAsync(url, method, data);
            Customer customer = JsonConvert.DeserializeObject<Customer>(result);
            CustomersListBox.Items.Add("Created a new Customer:   " + JsonConvert.SerializeObject(customer));

        }

        private async void UpdateCustomer_Click(object sender, RoutedEventArgs e)
        {
            int costumerId;
            string customerIdString = UpdateCustomerIdTextBox.Text;
            string customerName = UpdateCustomerNameTextBox.Text;
            string customerAddress = UpdateCustomerAddressTextBox.Text;
            string customerPhone = UpdateCustomerPhoneTextBox.Text;


            if (!(int.TryParse(customerIdString, out costumerId)))
            {
                MessageBox.Show("Not a valid ID number");
                return;
            }

            string url = "/customers/" + customerIdString;
            string method = "PUT";
            string data = "{\"Name\": \"" + customerName + "\", \"Address\": \"" + customerAddress + "\", \"Phone\": \"" + customerPhone + "\"}";
            string result = await SendHttpRequestAsync(url, method, data);
            Customer customer = JsonConvert.DeserializeObject<Customer>(result);
            CustomersListBox.Items.Add("Updated customer with ID: " + customerIdString + " in case it existed before. if wasnt found, nothing happened");

        }

        private async void DeleteCustomer_Click(object sender, RoutedEventArgs e)
        {
            int customerId;
            string customerIdString = DeleteCustomerIdTextBox.Text;

            if (!(int.TryParse(customerIdString, out customerId)))
            {
                MessageBox.Show("Not a valid ID number");
                return;
            }

            string url = "/customers/" + customerIdString;
            string method = "DELETE";
            string result = await SendHttpRequestAsync(url, method);
            CustomersListBox.Items.Add("Deleted customer with ID: " + customerIdString + " in case it existed before. if wasnt found, nothing happened");

        }

    }
}
