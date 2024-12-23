namespace PaypalUtility.Operations.Interfaces
{
    public interface IPayPalOperations
    {
        Task<string> GetAccessTokenAsync();
        Task<string> CreatePaymentAsync(decimal amount, string currency, string returnUrl, string cancelUrl);
        Task<string> ExecutePaymentAsync(string paymentId, string payerId);

        // Create mandate (pre-authorization) with PayPal
        Task<string> CreateMandateAsync(decimal amount, string currency, string returnUrl, string cancelUrl);

        // Create a subscription
        Task<string> CreateSubscriptionAsync(string planId, string returnUrl, string cancelUrl);

        // Execute subscription
        Task<string> ExecuteSubscriptionAsync(string subscriptionId, string payerId);

        // Refund a payment
        Task<string> RefundPaymentAsync(string paymentId, decimal refundAmount);

        // Get payment details
        Task<string> GetPaymentDetailsAsync(string paymentId);
    }


}
