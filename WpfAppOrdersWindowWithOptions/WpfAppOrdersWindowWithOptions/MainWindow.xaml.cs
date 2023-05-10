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

namespace WpfAppOrdersWindowWithOptions
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


        private async Task<int> OrderCurrentIDcount()
        {
            int currentID = 0;
            string url = "/orders";
            string method = "GET";
            string result = await SendHttpRequestAsync(url, method);
            List<Order> orders = JsonConvert.DeserializeObject<List<Order>>(result);
            foreach (Order order in orders)
            {
                if (currentID < order.OrderID)
                {
                    currentID = order.OrderID;
                }
            }
            return currentID;
        }


        private async Task<bool> isCustomerwithIDexist(int ordersCostumerID)
        {            
            string url = "/customers";
            string method = "GET";
            string result = await SendHttpRequestAsync(url, method);
            List<Customer> customers = JsonConvert.DeserializeObject<List<Customer>>(result);
            foreach (Customer customer in customers)
            {
                if (customer.CustomerID == ordersCostumerID)
                {
                    return true;
                }
            }
            return false;
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

        private async void GetAllOrders_Click(object sender, RoutedEventArgs e)
        {
            string url = "/orders";
            string method = "GET";
            string result = await SendHttpRequestAsync(url, method);
            List<Order> orders = JsonConvert.DeserializeObject<List<Order>>(result);
            foreach (Order order in orders)
            {
                OrdersListBox.Items.Add(JsonConvert.SerializeObject(order));

            }
        }

        private async void GetSpecificOrder_Click(object sender, RoutedEventArgs e)
        {
            string orderId = OrderIdTextBox.Text;
            string url = $"/orders/{orderId}";
            string method = "GET";
            string result = await SendHttpRequestAsync(url, method);
            Order order = JsonConvert.DeserializeObject<Order>(result);
            OrdersListBox.Items.Add(JsonConvert.SerializeObject(order));
        }


        private async void GetOrdersWithNumericField_Click(object sender, RoutedEventArgs e)
        {
            string numericFieldValueString = OrderNumericFieldTextBox.Text;
            int numericFieldValue;
            string url = "/orders";
            string method = "GET";
            string result = await SendHttpRequestAsync(url, method);
            List<Order> orders = JsonConvert.DeserializeObject<List<Order>>(result);

            if (!(int.TryParse(numericFieldValueString, out numericFieldValue)))
            {
                MessageBox.Show("Not a valid TotalAmount number");
                return;

            }

            foreach (Order order in orders)
            {
                if (order.TotalAmount > numericFieldValue)
                {
                    OrdersListBox.Items.Add(JsonConvert.SerializeObject(order));
                }
            }
        }

        private async void AddNewOrder_Click(object sender, RoutedEventArgs e)
        {
            int newOrderID = (await OrderCurrentIDcount()) + 1;

            int ordersCustomerID;
            string ordersCustomerIDstring = NewOrdersCustomerIDTextBox.Text;

            DateTime orderDate;
            string orderDateString = NewOrderDateTextBox.Text;

            int totalAmount;
            string totalAmountString = NewTotalAmountTextBox.Text;


            if (int.TryParse(ordersCustomerIDstring, out ordersCustomerID))
            {
                if(!(await isCustomerwithIDexist(ordersCustomerID)))
                {
                    MessageBox.Show("this CustomerID does not exist right now");
                    return;
                }
            }
            else
            {
                MessageBox.Show("Not a valid CustomerID number");
                return;
            }

            if (!(DateTime.TryParse(orderDateString, out orderDate)))
            {
                MessageBox.Show("Not a valid Date");
                return;
            }

            if (!(int.TryParse(totalAmountString, out totalAmount)))
            {
                MessageBox.Show("Not a valid totalAmount number");
                return;              
            }


            string url = "/orders";
            string method = "POST";
            string data = "{\"OrderID\": \"" + newOrderID + "\",\"CustomerID\": \"" + ordersCustomerIDstring + "\", \"OrderDate\": \"" + orderDateString + "\", \"TotalAmount\": \"" + totalAmountString + "\"}";
            string result = await SendHttpRequestAsync(url, method, data);
            Order order = JsonConvert.DeserializeObject<Order>(result);
            OrdersListBox.Items.Add("Created a new Order:   " + JsonConvert.SerializeObject(order));

        }

        private async void UpdateOrder_Click(object sender, RoutedEventArgs e)
        {
            int orderId;
            string orderIdString = UpdateOrderIdTextBox.Text;

            int ordersCustomerID;
            string ordersCustomerIDString = UpdateOrdersCustomerIDTextBox.Text;

            DateTime orderDate;
            string orderDateString = UpdateOrderDateTextBox.Text;

            int totalAmount;
            string totalAmountString = UpdateTotalAmountTextBox.Text;


            if (!(int.TryParse(orderIdString, out orderId)))
            {
                MessageBox.Show("Not a valid ID number");
                return;
            }


            if (int.TryParse(ordersCustomerIDString, out ordersCustomerID))
            {
                if (!(await isCustomerwithIDexist(ordersCustomerID)))
                {
                    MessageBox.Show("this CustomerID does not exist right now");
                    return;
                }
            }
            else
            {
                MessageBox.Show("Not a valid CustomerID number");
                return;
            }


            if (!(DateTime.TryParse(orderDateString, out orderDate)))
            {
                MessageBox.Show("Not a valid Date");
                return;
            }

            if (!(int.TryParse(totalAmountString, out totalAmount)))
            {
                MessageBox.Show("Not a valid totalAmount number");
                return;
            }

            string url = "/orders/" + orderIdString;
            string method = "PUT";
            string data = "{\"CustomerID\": \"" + ordersCustomerIDString + "\", \"OrderDate\": \"" + orderDateString + "\", \"TotalAmount\": \"" + totalAmountString + "\"}";
            string result = await SendHttpRequestAsync(url, method, data);
            Order order = JsonConvert.DeserializeObject<Order>(result);
            OrdersListBox.Items.Add("Updated order with ID: " + orderIdString + " in case it existed before. if wasnt found, nothing happened");

        }

        private async void DeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            int orderId;
            string orderIdString = DeleteOrderIdTextBox.Text;

            if (!(int.TryParse(orderIdString, out orderId)))
            {
                MessageBox.Show("Not a valid ID number");
                return;
            }

            string url = "/orders/" + orderIdString;
            string method = "DELETE";
            string result = await SendHttpRequestAsync(url, method);
            OrdersListBox.Items.Add("Deleted order with ID: " + orderIdString + " in case it existed before. if wasnt found, nothing happened");

        }

    }
}
