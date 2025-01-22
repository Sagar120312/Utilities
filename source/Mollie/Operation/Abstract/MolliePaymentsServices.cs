using Mollie.DTOs;
using Mollie.DTOs.Enum;
using Mollie.Operation.Interface;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace BroetchenTaxi.Domain.Operations
{
    public class MolliePaymentsServices : IMolliePaymentsServices
    {
        private static readonly HttpClient client = new HttpClient();
        private readonly string _redirectUri;
        private readonly string _webhookUri;
        private readonly AccessOption _accessType;

        public MolliePaymentsServices(string redirectUri, string webhookUri, AccessOption accessType, string accessKeyOrToken)
        {
            _redirectUri = redirectUri;
            _accessType = accessType;
            _webhookUri = webhookUri;
            if (string.IsNullOrEmpty(accessKeyOrToken))
            {
                throw new ArgumentNullException("ACCESS TOKEN is required with OAUTH ACCESS TYPE and API KEY is required with APIKey ACCESS TYPE");
            }
            InitiateClient(accessKeyOrToken);
        }
        public int InitiateClient(string accessKeyOrToken)
        {
            if (string.IsNullOrEmpty(accessKeyOrToken))
                return -1;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessKeyOrToken);
            return 0;
        }

        public async Task<HttpResponseMessage> CreatePayment(PaymentRequestDto paymentRequest, string email, string mollieProfileId)
        {
            try
            {
                var paymentMethods = await GetAvailablePaymentMethods(mollieProfileId);
                if (paymentMethods != null)
                {
                    object paymentData;
                    if (_accessType == AccessOption.OAuth)
                    {
                        paymentData = new
                        {
                            amount = new
                            {
                                currency = "EUR",
                                value = paymentRequest.Amount.ToString("F2")
                            },
                            applicationFee = new
                            {
                                amount = new
                                {
                                    currency = "EUR",
                                    value = paymentRequest.ApplicationFee.ToString("F2")
                                },
                                description = "Application Fee"
                            },
                            description = $"TransactionId #{paymentRequest.TransactionId}",
                            redirectUrl = paymentRequest.RedirectUrl,
                            cancelUrl = paymentRequest.CancelUrl,
                            webhookUrl = _webhookUri,
                            method = paymentMethods,
                            profileId = mollieProfileId,
                            metadata = new
                            {
                                order_id = paymentRequest.TransactionId
                            }
                        };
                    }
                    else
                    {
                        paymentData = new
                        {
                            amount = new
                            {
                                currency = "EUR",
                                value = paymentRequest.Amount.ToString("F2")
                            },
                            description = $"TransactionId #{paymentRequest.TransactionId}",
                            redirectUrl = paymentRequest.RedirectUrl,
                            cancelUrl = paymentRequest.CancelUrl,
                            webhookUrl = _webhookUri,
                            method = paymentMethods,
                            metadata = new
                            {
                                order_id = paymentRequest.TransactionId
                            }
                        };
                    }
                    var json = JsonConvert.SerializeObject(paymentData);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync("https://api.mollie.com/v2/payments", content);
                    return response;
                }
                else
                {
                    throw new Exception("No payment method is enabled");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in payment creation : " + e.Message);
                throw;
            }
        }

        public async Task<HttpResponseMessage> CreateRecurringPayment(PaymentRequestDto paymentRequest, string email, string mollieProfileId, string customerId)
        {
            try
            {

                var paymentMethods = await GetAvailablePaymentMethods(mollieProfileId);
                if (paymentMethods != null)
                {
                    object paymentData;
                    if (_accessType == AccessOption.OAuth)
                    {
                        paymentData = new
                        {
                            amount = new
                            {
                                currency = "EUR",
                                value = paymentRequest.Amount.ToString("F2")
                            },
                            applicationFee = new
                            {
                                amount = new
                                {
                                    currency = "EUR",
                                    value = paymentRequest.ApplicationFee.ToString("F2")
                                },
                                description = "Application Fee for Broetchen.Taxi"
                            },
                            description = $"TransactionId #{paymentRequest.TransactionId}",
                            redirectUrl = paymentRequest.RedirectUrl,
                            cancelUrl = paymentRequest.CancelUrl,
                            webhookUrl = _webhookUri,
                            sequenceType = "first",
                            customerId = customerId,
                            method = paymentMethods,
                            profileId = mollieProfileId,
                            metadata = new
                            {
                                order_id = paymentRequest.TransactionId
                            }
                        };
                    }
                    else
                    {
                        paymentData = new
                        {
                            amount = new
                            {
                                currency = "EUR",
                                value = paymentRequest.Amount.ToString("F2")
                            },
                            description = $"TransactionId #{paymentRequest.TransactionId}",
                            redirectUrl = paymentRequest.RedirectUrl,
                            cancelUrl = paymentRequest.CancelUrl,
                            webhookUrl = _webhookUri,
                            sequenceType = "first",
                            customerId = customerId,
                            method = paymentMethods,
                            metadata = new
                            {
                                order_id = paymentRequest.TransactionId
                            }
                        };
                    }

                    var json = JsonConvert.SerializeObject(paymentData);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync("https://api.mollie.com/v2/payments", content);
                    return response;
                }
                else
                {
                    throw new Exception("No payment method is enabled");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in payment creation : " + e.Message);
                throw;
            }
        }

        public async Task<HttpResponseMessage> CreateSubscription(CreateMollieSubscriptionRequestDto customerRequest, string mollieProfileId, string customerId)
        {
            CreateMollieCustomerRequestDto customerRequestDto = new CreateMollieCustomerRequestDto()
            {
                Name = customerRequest.Name,
                Email = customerRequest.Email
            };

            // List Mandates for Customer
            var mandates = await ListMandatesAsync(customerId);
            // Use the latest mandate
            var latestMandate = mandates[0];
            MollieSubscriptionRequest subscriptionRequest;
            if (_accessType == AccessOption.OAuth)
            {
                subscriptionRequest = new MollieSubscriptionRequest
                {
                    Amount = new Amount
                    {
                        Currency = "EUR",
                        Value = customerRequest.SubscriptionAmount.ToString("F2")
                    },
                    Interval = "1 week",
                    StartDate = DateTime.Now.AddDays(6).ToString("yyyy-MM-dd"),
                    MandateId = latestMandate.Id,
                    Description = "Weekly subscription on" + customerRequest.TransactionId,
                    WebhookUrl = _webhookUri,
                    ProfileId = mollieProfileId
                };
            }
            else
            {
                subscriptionRequest = new MollieSubscriptionRequest
                {
                    Amount = new Amount
                    {
                        Currency = "EUR",
                        Value = customerRequest.SubscriptionAmount.ToString("F2")
                    },
                    Interval = "1 week",
                    StartDate = DateTime.Now.AddDays(6).ToString("yyyy-MM-dd"),
                    MandateId = latestMandate.Id,
                    Description = "Weekly subscription on" + customerRequest.TransactionId,
                    WebhookUrl = _webhookUri
                };
            }

            var jsonContent = JsonConvert.SerializeObject(subscriptionRequest);
            
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"https://api.mollie.com/v2/customers/{customerId}/subscriptions", content);
            return response;
        }

        public async Task<MollieSubscriptionDto> GetSubscriptionData(string customerId, string subscriptionId)
        {
            var response = await client.GetAsync($"https://api.mollie.com/v2/customers/{customerId}/subscriptions/{subscriptionId}");
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                MollieSubscriptionDto subscriptoinData = JsonConvert.DeserializeObject<MollieSubscriptionDto>(responseString);

                return subscriptoinData;
            }
            else
                return null;
        }

        public async Task<List<MollieMandateResponse>> ListMandatesAsync(string customerId)
        {
            //var testmode = _configuration["mollieTestMode"] == "true" ? true : false;
            var response = await client.GetAsync($"https://api.mollie.com/v2/customers/{customerId}/mandates");
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var mandates = JsonConvert.DeserializeObject<MollieMandateListResponse>(responseString);
                return mandates.Embedded.Mandates;
            }

            return null;
        }

        public async Task<PaymentStatusDto> GetPaymentStatus(string paymentId)
        {
            try
            {
                var response = await client.GetAsync($"https://api.mollie.com/v2/payments/{paymentId}");

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var paymentResponse = JsonConvert.DeserializeObject<PaymentStatusDto>(responseString);
                    if (Convert.ToDecimal(paymentResponse.AmountRemaining?.Value) == 0 && Convert.ToDecimal(paymentResponse.AmountRefunded?.Value) > 0)
                        paymentResponse.Status = "refunded";
                    return paymentResponse;
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<string>> GetAvailablePaymentMethods(string? profileId)
        {
            try
            {
                HttpResponseMessage response;
                if (_accessType == AccessOption.OAuth)
                {
                    response = await client.GetAsync($"https://api.mollie.com/v2/methods?profileId={profileId}");
                }
                else
                {
                    response = await client.GetAsync($"https://api.mollie.com/v2/methods");
                }
                List<string> paymentMethodList = new List<string>();
                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    //var paymentMethodsResponse = JsonConvert.DeserializeObject<PaymentMethodDto>(responseString);
                    JObject parsedResponse = JObject.Parse(responseString);
                    var methodsJson = parsedResponse["_embedded"]?["methods"]?.ToString();

                    if (methodsJson != null)
                    {
                        List<PaymentMethodDto> methods = JsonConvert.DeserializeObject<List<PaymentMethodDto>>(methodsJson);

                        // Output the list of methods
                        foreach (var method in methods)
                        {
                            paymentMethodList.Add(method.id);
                            Console.WriteLine($"ID: {method.id}, Description: {method.description}");
                        }
                    }
                    return paymentMethodList;
                }
                var responseData = await response.Content.ReadAsStringAsync();
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<bool> RefundPayment(string paymentId, decimal amount)
        {
            var refundData = new
            {
                amount = new
                {
                    currency = "EUR",
                    value = amount.ToString("F2")
                }
            };

            var json = JsonConvert.SerializeObject(refundData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"https://api.mollie.com/v2/payments/{paymentId}/refunds", content);

            return response.IsSuccessStatusCode;
        }
        //Method to get the customer if already exists and create if it doesn't exist in Mollie
        public async Task<MollieCustomerDto> GetOrCreateCustomerAsync(CreateMollieCustomerRequestDto createCustomerRequest)
        {
            var existingCustomer = await GetCustomerByEmailAsync(createCustomerRequest.Email);
            if (existingCustomer != null)
            {
                return existingCustomer;
            }

            return await CreateCustomerAsync(createCustomerRequest);
        }

        //Find customer by mail
        public async Task<MollieCustomerDto> GetCustomerByEmailAsync(string email)
        {
            var customers = await GetCustomersAsync();
            return customers.FirstOrDefault(c => c.Email == email);
        }

        //get list of all customers in Mollie
        public async Task<List<MollieCustomerDto>> GetCustomersAsync()
        {
            try
            {
                var response = await client.GetAsync($"https://api.mollie.com/v2/customers");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var customersResponse = JsonConvert.DeserializeObject<MollieCustomerResponseDto>(content);
                return customersResponse.Embedded.Customers;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in GetCustomersAsync: " + ex.Message);
                throw;
            }
        }
        //CancelRecurringOrderRequest
        public async Task<int> CancelRecurringOrder(CancelRecurringOrderRequestDto request)
        {
            MollieCustomerDto customer = await GetCustomerByEmailAsync(request.CustomerEmail);
            if (customer == null)
            {
                return 1;
            }
            var subscriptionResponse = await GetSubscriptionAsync(customer.Id, request.SubscriptionId);
            if (subscriptionResponse == null)
            {
                return 2;
            }

            var cancelSubscriptionResponse = await CancelSubscriptionAsync(customer.Id, subscriptionResponse.Id);
            if (!cancelSubscriptionResponse.IsSuccessStatusCode)
            {
                return 3;
            }

            var revokeMandateResponse = await RevokeMandateAsync(customer.Id, subscriptionResponse.MandateId);
            if (!revokeMandateResponse.IsSuccessStatusCode)
            {
                return 4;
            }
            return 0;
        }
        //Create mollie customer
        public async Task<MollieCustomerDto> CreateCustomerAsync(CreateMollieCustomerRequestDto createCustomerRequest)
        {
            try
            {
                var jsonContent = JsonConvert.SerializeObject(createCustomerRequest);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://api.mollie.com/v2/customers", content);
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<MollieCustomerDto>(responseString);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in CreateCustomerAsync: " + ex.Message);
                throw;
            }
        }

        public async Task<HttpResponseMessage> CancelSubscriptionAsync(string customerId, string subscriptionId)
        {
            var url = $"https://api.mollie.com/v2/customers/{customerId}/subscriptions/{subscriptionId}";
            return await client.DeleteAsync(url);
        }

        public async Task<HttpResponseMessage> RevokeMandateAsync(string customerId, string mandateId)
        {
            var url = $"https://api.mollie.com/v2/customers/{customerId}/mandates/{mandateId}";
            return await client.DeleteAsync(url);
        }

        public async Task<SubscriptionDto> GetSubscriptionAsync(string customerId, string subscriptionId)
        {
            try
            {
                var url = $"https://api.mollie.com/v2/customers/{customerId}/subscriptions/{subscriptionId}";
                var response = await client.GetStringAsync(url);
                return JsonConvert.DeserializeObject<SubscriptionDto>(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while getting subscription : " + ex.Message);
                throw;
            }
        }


        /// <summary>
        /// This method is just for specific cases where Admin is allowing bussiness to create it's profile. Not generally used.
        /// </summary>
        /// <param name="mollieProfileWebsite"></param>
        /// <param name="mollieProfileEmail"></param>
        /// <param name="mollieProfilePhone"></param>
        /// <param name="serviceCategory"></param>
        /// <returns></returns>
        public async Task<string> GetMollieProfile(string mollieProfileWebsite, string mollieProfileEmail, string mollieProfilePhone, ServiceCategory serviceCategory)
        {
            var profileData = new MollieProfileRequestDto
            {
                Name = "Broetchen.Taxi",
                Website = mollieProfileWebsite,
                Email = mollieProfileEmail,
                Phone = mollieProfilePhone,
                Description = "Services for " + serviceCategory.ToString(),
                BusinessCategory = serviceCategory.ToString()
            };
            var jsonBody = JsonConvert.SerializeObject(profileData);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync("https://api.mollie.com/v2/profiles", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var profileResponse = JsonConvert.DeserializeObject<MollieProfileResponseDto>(responseContent);

                    Console.WriteLine($"Profile Created: {profileResponse.Id}");
                    return profileResponse.Id; // Return the profile ID
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to create profile. Error: {response.StatusCode} - {errorContent}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return null;
            }
        }

    }
}
