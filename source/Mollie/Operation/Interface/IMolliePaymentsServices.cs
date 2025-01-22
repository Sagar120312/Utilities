using Mollie.DTOs;

namespace Mollie.Operation.Interface
{
    public interface IMolliePaymentsServices
    {
        // Payment-related methods
        Task<HttpResponseMessage> CreatePayment(PaymentRequestDto paymentRequest, string email, string mollieProfileId);
        Task<HttpResponseMessage> CreateRecurringPayment(PaymentRequestDto paymentRequest, string email, string mollieProfileId, string customerId);
        Task<PaymentStatusDto> GetPaymentStatus(string paymentId);

        // Subscription-related methods
        Task<HttpResponseMessage> CreateSubscription(CreateMollieSubscriptionRequestDto customerRequest, string mollieProfileId, string customerId);
        Task<MollieSubscriptionDto> GetSubscriptionData(string customerId, string subscriptionId);
        Task<List<MollieMandateResponse>> ListMandatesAsync(string customerId);
        Task<HttpResponseMessage> CancelSubscriptionAsync(string customerId, string subscriptionId);
        Task<HttpResponseMessage> RevokeMandateAsync(string customerId, string mandateId);
        Task<SubscriptionDto> GetSubscriptionAsync(string customerId, string subscriptionId);

        // Refund-related methods
        Task<bool> RefundPayment(string paymentId, decimal amount);

        // Customer-related methods
        Task<MollieCustomerDto> GetOrCreateCustomerAsync(CreateMollieCustomerRequestDto createCustomerRequest);
        Task<MollieCustomerDto> GetCustomerByEmailAsync(string email);


        Task<int> CancelRecurringOrder(CancelRecurringOrderRequestDto request);
        Task<MollieCustomerDto> CreateCustomerAsync(CreateMollieCustomerRequestDto createCustomerRequest);

    }

}
