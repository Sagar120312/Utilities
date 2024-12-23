using Newtonsoft.Json;
using PaypalUtility.Operations.Interfaces;
using System.Text;

namespace PaypalUtility.Operations.Abstracts
{
    public class PayPalOperations : IPayPalOperations
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _environment;

        private readonly string _baseUrl;
        private readonly HttpClient _httpClient;
        private string _accessToken;

        // Constructor to initialize the PayPal operations
        public PayPalOperations(string clientId, string clientSecret, string environment = "sandbox")
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
            _environment = environment;

            // Set base URL based on environment
            _baseUrl = environment.ToLower() == "live"
                ? "https://api.paypal.com"
                : "https://api.sandbox.paypal.com";

            _httpClient = new HttpClient();
        }

        // Method to get OAuth token from PayPal
        public async Task<string> GetAccessTokenAsync()
        {
            using (var client = new HttpClient())
            {
                var byteArray = Encoding.ASCII.GetBytes($"{_clientId}:{_clientSecret}");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                var content = new FormUrlEncodedContent(new[]
                {
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            });

                var response = await client.PostAsync($"{_baseUrl}/v1/oauth2/token", content);
                var responseContent = await response.Content.ReadAsStringAsync();
                dynamic responseObject = JsonConvert.DeserializeObject(responseContent);

                return responseObject.access_token;
            }
        }

        // Create payment
        public async Task<string> CreatePaymentAsync(decimal amount, string currency, string returnUrl, string cancelUrl)
        {
            var accessToken = await GetAccessTokenAsync();

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                var paymentData = new
                {
                    intent = "sale",
                    payer = new
                    {
                        payment_method = "paypal"
                    },
                    transactions = new[]
                    {
                    new
                    {
                        amount = new { total = amount.ToString("F2"), currency },
                        description = "Payment for order"
                    }
                },
                    redirect_urls = new
                    {
                        return_url = returnUrl,
                        cancel_url = cancelUrl
                    }
                };

                var json = JsonConvert.SerializeObject(paymentData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"{_baseUrl}/v1/payments/payment", content);
                var responseContent = await response.Content.ReadAsStringAsync();
                dynamic responseObject = JsonConvert.DeserializeObject(responseContent);

                // Extract approval URL for redirection
                var approvalUrl = responseObject.links[1].href;

                return approvalUrl;
            }
        }

        // Execute payment
        public async Task<string> ExecutePaymentAsync(string paymentId, string payerId)
        {
            var accessToken = await GetAccessTokenAsync();

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                var paymentExecutionData = new
                {
                    payer_id = payerId
                };

                var json = JsonConvert.SerializeObject(paymentExecutionData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"{_baseUrl}/v1/payments/payment/{paymentId}/execute", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                return responseContent; // Contains the details of the executed payment
            }
        }

        // Create mandate (pre-authorization)
        public async Task<string> CreateMandateAsync(decimal amount, string currency, string returnUrl, string cancelUrl)
        {
            var accessToken = await GetAccessTokenAsync();

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                var mandateData = new
                {
                    intent = "authorize",
                    payer = new
                    {
                        payment_method = "paypal"
                    },
                    transactions = new[]
                    {
                    new
                    {
                        amount = new { total = amount.ToString("F2"), currency },
                        description = "Mandate authorization"
                    }
                },
                    redirect_urls = new
                    {
                        return_url = returnUrl,
                        cancel_url = cancelUrl
                    }
                };

                var json = JsonConvert.SerializeObject(mandateData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"{_baseUrl}/v1/payments/payment", content);
                var responseContent = await response.Content.ReadAsStringAsync();
                dynamic responseObject = JsonConvert.DeserializeObject(responseContent);

                var approvalUrl = responseObject.links[1].href;

                return approvalUrl;
            }
        }

        // Create a subscription
        public async Task<string> CreateSubscriptionAsync(string planId, string returnUrl, string cancelUrl)
        {
            var accessToken = await GetAccessTokenAsync();

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                var subscriptionData = new
                {
                    plan = new { id = planId },
                    start_time = DateTime.UtcNow.AddMinutes(5).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    quantity = "1",
                    subscriber = new
                    {
                        name = new
                        {
                            given_name = "John",
                            surname = "Doe"
                        },
                        email_address = "user@example.com"
                    },
                    return_url = returnUrl,
                    cancel_url = cancelUrl
                };

                var json = JsonConvert.SerializeObject(subscriptionData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"{_baseUrl}/v1/billing/subscriptions", content);
                var responseContent = await response.Content.ReadAsStringAsync();
                dynamic responseObject = JsonConvert.DeserializeObject(responseContent);

                var approvalUrl = responseObject.links[1].href;

                return approvalUrl;
            }
        }

        // Execute subscription
        public async Task<string> ExecuteSubscriptionAsync(string subscriptionId, string payerId)
        {
            var accessToken = await GetAccessTokenAsync();

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                var subscriptionExecutionData = new
                {
                    payer_id = payerId
                };

                var json = JsonConvert.SerializeObject(subscriptionExecutionData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"{_baseUrl}/v1/billing/subscriptions/{subscriptionId}/activate", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                return responseContent;
            }
        }

        // Refund a payment
        public async Task<string> RefundPaymentAsync(string paymentId, decimal refundAmount)
        {
            var accessToken = await GetAccessTokenAsync();

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                var refundData = new
                {
                    amount = new
                    {
                        total = refundAmount.ToString("F2"),
                        currency = "USD"
                    }
                };

                var json = JsonConvert.SerializeObject(refundData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"{_baseUrl}/v1/payments/sale/{paymentId}/refund", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                return responseContent;
            }
        }

        // Get payment details
        public async Task<string> GetPaymentDetailsAsync(string paymentId)
        {
            var accessToken = await GetAccessTokenAsync();

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                var response = await client.GetAsync($"{_baseUrl}/v1/payments/payment/{paymentId}");
                var responseContent = await response.Content.ReadAsStringAsync();

                return responseContent;
            }
        }
    }

}
