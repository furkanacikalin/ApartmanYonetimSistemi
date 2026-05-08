using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using ApartmanYonetimSistemi.Models;

namespace ApartmanYonetimSistemi.Services;

public class PaymentService
{
    private readonly IConfiguration _configuration;
    private readonly Options _iyzicoOptions;

    public PaymentService(IConfiguration configuration)
    {
        _configuration = configuration;

        
        _iyzicoOptions = new Options
        {
            ApiKey = _configuration["Iyzico:ApiKey"],
            SecretKey = _configuration["Iyzico:SecretKey"],
            BaseUrl = _configuration["Iyzico:BaseUrl"]
        };
    }

    public async Task<PaymentResponse> ProcessPaymentAsync(ApartmanYonetimSistemi.Models.Payment payment, User user, string cardHolderName, string cardNumber, string expireMonth, string expireYear, string cvc)
    {
        CreatePaymentRequest request = new CreatePaymentRequest
        {
            Locale = Locale.TR.ToString(),
            ConversationId = Guid.NewGuid().ToString(), 
            Price = payment.Amount.ToString().Replace(",", "."),
            PaidPrice = payment.Amount.ToString().Replace(",", "."),
            Currency = Currency.TRY.ToString(),
            Installment = 1,
            BasketId = "B" + payment.Id,
            PaymentChannel = PaymentChannel.WEB.ToString(),
            PaymentGroup = PaymentGroup.PRODUCT.ToString()
        };

        
        Buyer buyer = new Buyer
        {
            Id = user.Id.ToString(),
            Name = user.FirstName,
            Surname = user.LastName,
            GsmNumber = "+905555555555", 
            Email = user.Username + "@apartman.com",
            IdentityNumber = "11111111111",
            RegistrationAddress = "Apartman Adresi",
            Ip = "85.34.78.112",
            City = "Istanbul",
            Country = "Turkey"
        };
        request.Buyer = buyer;

        
        Address billingAddress = new Address
        {
            ContactName = user.FirstName + " " + user.LastName,
            City = "Istanbul",
            Country = "Turkey",
            Description = "Apartman No:1 Daire: " + payment.FlatId, 
            ZipCode = "34000"
        };
        request.BillingAddress = billingAddress;

      
        request.ShippingAddress = billingAddress;

       
        PaymentCard paymentCard = new PaymentCard
        {
            CardHolderName = cardHolderName,
            CardNumber = cardNumber,
            ExpireMonth = expireMonth,
            ExpireYear = expireYear,
            Cvc = cvc,
            RegisterCard = 0 
        };
        request.PaymentCard = paymentCard;

        
        List<BasketItem> basketItems = new List<BasketItem>();
        basketItems.Add(new BasketItem
        {
            Id = payment.Id.ToString(),
            Name = payment.Description,
            Category1 = payment.Category,
            ItemType = BasketItemType.VIRTUAL.ToString(),
            Price = payment.Amount.ToString().Replace(",", ".")
        });
        request.BasketItems = basketItems;

        
        Iyzipay.Model.Payment iyzicoResponse = await Task.Run(() => Iyzipay.Model.Payment.Create(request, _iyzicoOptions));

        return new PaymentResponse
        {
            IsSuccess = iyzicoResponse.Status == "success",
            ErrorMessage = iyzicoResponse.ErrorMessage,
            IyzicoPaymentId = iyzicoResponse.PaymentId
        };
    }
}


public class PaymentResponse
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public string? IyzicoPaymentId { get; set; }
}