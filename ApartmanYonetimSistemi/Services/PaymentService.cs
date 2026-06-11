using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using ApartmanYonetimSistemi.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

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
        // Kültür ayarlarından bağımsız olarak 150.50 formatında string üretmek için InvariantCulture kullanıyoruz
        string formattedPrice = payment.Amount.ToString("F2", CultureInfo.InvariantCulture);

        // Kullanıcı adı veya soyadı boşsa Iyzico'nun hata fırlatmasını önlemek için yedek değerler atıyoruz
        string buyerName = string.IsNullOrWhiteSpace(user.FirstName) ? "Sakin" : user.FirstName;
        string buyerSurname = string.IsNullOrWhiteSpace(user.LastName) ? "Kullanici" : user.LastName;

        CreatePaymentRequest request = new CreatePaymentRequest
        {
            Locale = Locale.TR.ToString(),
            ConversationId = Guid.NewGuid().ToString(),
            Price = formattedPrice,
            PaidPrice = formattedPrice,
            Currency = Currency.TRY.ToString(),
            Installment = 1,
            BasketId = "B" + payment.Id,
            PaymentChannel = PaymentChannel.WEB.ToString(),
            PaymentGroup = PaymentGroup.PRODUCT.ToString()
        };

        Buyer buyer = new Buyer
        {
            Id = user.Id.ToString(),
            Name = buyerName,
            Surname = buyerSurname,
            GsmNumber = "+905555555555",
            Email = (string.IsNullOrWhiteSpace(user.Username) ? "sakin" : user.Username) + "@apartman.com",
            IdentityNumber = "11111111111",
            RegistrationAddress = "Apartman Adresi",
            Ip = "85.34.78.112",
            City = "Istanbul",
            Country = "Turkey"
        };
        request.Buyer = buyer;

        Address billingAddress = new Address
        {
            ContactName = buyerName + " " + buyerSurname,
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
            Name = string.IsNullOrWhiteSpace(payment.Description) ? "Apartman Aidat Odemesi" : payment.Description,
            Category1 = string.IsNullOrWhiteSpace(payment.Category) ? "Aidat" : payment.Category,
            ItemType = BasketItemType.VIRTUAL.ToString(),
            Price = formattedPrice
        });
        request.BasketItems = basketItems;

        // Iyzico senkron bir metot olduğu için Blazor UI thread'ini bloklamamak adına Task.Run doğru bir tercihtir.
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