using Newtonsoft.Json;

namespace Mollie.DTOs
{
    public class ResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Code { get; set; }
        public dynamic Data { get; set; }
    }
    public class AuthDataDto
    {
        public string AuthCode { get; set; }
        public string RedirectUri { get; set; }
    }
    public class MollieTokenResponseDto
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
    }

    public class MollieProfileResponseDto
    {
        [JsonProperty("resource")]
        public string Resource { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("mode")]
        public string Mode { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("website")]
        public string Website { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("businessCategory")]
        public string BusinessCategory { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("review")]
        public ReviewDto Review { get; set; }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("_links")]
        public LinksDto Links { get; set; }
    }

    public class LinksDto
    {
        [JsonProperty("self")]
        public LinkDetailDto Self { get; set; }

        [JsonProperty("dashboard")]
        public LinkDetailDto Dashboard { get; set; }

        [JsonProperty("documentation")]
        public LinkDetailDto Documentation { get; set; }
    }

    public class LinkDetailDto
    {
        [JsonProperty("href")]
        public string Href { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public class ReviewDto
    {
        [JsonProperty("status")]
        public string Status { get; set; }
    }

    public class MollieProfileRequestDto
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("website")]
        public string Website { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("countriesOfActivity")]
        public List<string> CountriesOfActivity { get; set; }

        [JsonProperty("businessCategory")]
        public string BusinessCategory { get; set; }
    }

    public class PaymentRequestDto
    {
        public decimal Amount { get; set; }
        public decimal ApplicationFee { get; set; }
        public string TransactionId { get; set; }
        public string RedirectUrl { get; set; }
        public string CancelUrl { get; set; }
    }

    public class PaymentMethodDto
    {
        public string resource { get; set; }
        public string id { get; set; }
        public string description { get; set; }
    }

    public class MollieCustomerDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
    public class MollieCustomerEmbeddedDto
    {
        public List<MollieCustomerDto> Customers { get; set; }
    }

    public class MollieCustomerResponseDto
    {
        [JsonProperty("_embedded")]
        public MollieCustomerEmbeddedDto Embedded { get; set; }
    }

    public class CreateMollieCustomerRequestDto
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
    }
    public class CreateMollieSubscriptionRequestDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public decimal SubscriptionAmount { get; set; }
        public string TransactionId { get; set; }
    }
    public class MolliePaymentResponseDto
    {
        public string Resource { get; set; }
        public string Id { get; set; }
        public string Mode { get; set; }
        public Amount Amount { get; set; }
        public string Description { get; set; }
        public string SequenceType { get; set; }
        public string RedirectUrl { get; set; }
        public string WebhookUrl { get; set; }
        public Metadata Metadata { get; set; }
        public string ProfileId { get; set; }
        public string Status { get; set; }
        public bool IsCancelable { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public Links _links { get; set; }
    }
    public class PaymentResponseDto
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public string RedirectUrl { get; set; }
    }

    public class MollieWebhookDto
    {
        public string Id { get; set; }
    }

    public class PaymentRetryDto
    {
        public decimal Amount { get; set; }
        public string TransactionId { get; set; }
        public string RedirectUrl { get; set; }
        public string CancelUrl { get; set; }

    }

    public class CancelOrderDto
    {
        public string PaymentId { get; set; }
        public decimal Amount { get; set; }
    }

    public class PaymentStatusDto
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public string Method { get; set; }
        public string SubscriptionId { get; set; }
        public Amount AmountRemaining { get; set; }
        public Amount AmountRefunded { get; set; }
    }
    public class Amount
    {
        [JsonProperty("value")]
        public string Value { get; set; }
        [JsonProperty("currency")]
        public string Currency { get; set; }
    }

    public class Metadata
    {
        public string OrderId { get; set; }
    }

    public class Links
    {
        public Link Self { get; set; }
        public Link Checkout { get; set; }
        public Link Dashboard { get; set; }
        public Link Documentation { get; set; }
    }

    public class Link
    {
        public string Href { get; set; }
        public string Type { get; set; }
    }

    public class MollieSubscriptionRequest
    {
        [JsonProperty("amount")]
        public Amount Amount { get; set; }
        [JsonProperty("interval")]
        public string Interval { get; set; }
        [JsonProperty("startDate")]
        public string StartDate { get; set; }
        [JsonProperty("mandateId")]
        public string MandateId { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("webhookUrl")]
        public string WebhookUrl { get; set; }
        [JsonProperty("profileId")]
        public string ProfileId { get; set; }
    }
    public class MollieSubscriptionResponse
    {
        public string Id { get; set; }
        public Amount Amount { get; set; }
        public string Interval { get; set; }
        public string StartDate { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public string Method { get; set; }
        public string MandateId { get; set; }
        public string CustomerId { get; set; }
    }

    public class MollieMandateResponse
    {
        public string Id { get; set; }
        public string Status { get; set; }
    }
    public class EmbeddedMandates
    {
        public List<MollieMandateResponse> Mandates { get; set; }
    }

    public class MollieSubscriptionDto
    {
        [JsonProperty("resource")]
        public string Resource { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("mode")]
        public string Mode { get; set; }

        [JsonProperty("amount")]
        public Amount Amount { get; set; }

        [JsonProperty("times")]
        public int Times { get; set; }

        [JsonProperty("timesRemaining")]
        public int TimesRemaining { get; set; }

        [JsonProperty("interval")]
        public string Interval { get; set; }

        [JsonProperty("startDate")]
        public string StartDate { get; set; }

        [JsonProperty("nextPaymentDate")]
        public string NextPaymentDate { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("webhookUrl")]
        public string WebhookUrl { get; set; }

        [JsonProperty("customerId")]
        public string CustomerId { get; set; }

        [JsonProperty("mandateId")]
        public string MandateId { get; set; }

        [JsonProperty("createdAt")]
        public string CreatedAt { get; set; }

        [JsonProperty("_links")]
        public Links Links { get; set; }
    }
    public class MollieMandateListResponse
    {
        [JsonProperty("_embedded")]
        public EmbeddedMandates Embedded { get; set; }

    }
    public class CancelRecurringOrderRequestDto
    {
        public string SubscriptionId { get; set; }
        public string CustomerEmail { get; set; }
    }

    public class CancelOrderResponseDto
    {
        public bool SubscriptionCancelled { get; set; }
        public bool MandateRevoked { get; set; }
    }
    public class SubscriptionDto
    {
        public string Id { get; set; }
        public string MandateId { get; set; }
        public string CreatedAt { get; set; }
    }

    public class MollieTransactionDto
    {
        public Guid TenantId { get; set; }
        public string TransactionId { get; set; }
        public bool? IsSubscriptionTransaction { get; set; }
#nullable enable
        public string? SubscriptionId { get; set; }
        public string? PaymentId { get; set; }
        public string? TransactionStatus { get; set; }
        public DateTime? RowCreated { get; set; }
        public DateTime? RowUpdated { get; set; }
#nullable disable
    }
}
