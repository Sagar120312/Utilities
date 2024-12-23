namespace PaypalUtility.DTOs
{
    internal class PaypalDtos
    {
    }

    public class PaymentRequestDto
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string ReturnUrl { get; set; }
        public string CancelUrl { get; set; }
    }

    public class OrderDto
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public string ApprovalUrl { get; set; }
    }

    public class RefundRequestDto
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; }
    }

    public class RefundDto
    {
        public string Id { get; set; }
        public string Status { get; set; }
    }

    public class SubscriptionRequestDto
    {
        public string PlanId { get; set; }
        public string ReturnUrl { get; set; }
        public string CancelUrl { get; set; }
    }
    public class SubscriptionDto
    {
        public string Id { get; set; }
        public string Status { get; set; }
    }
}
