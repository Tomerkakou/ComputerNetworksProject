
using ComputerNetworksProject.Constants;
using ComputerNetworksProject.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Hosting.Server;

namespace ComputerNetworksProject.Services
{
    public class SendNotificationService:BackgroundService
    {
        private readonly ILogger<CartReleaseService> _logger;
        private readonly IServiceProvider _service;
        private readonly IEmailSender _emailSender;
        public SendNotificationService(ILogger<CartReleaseService> logger, IServiceProvider service, IEmailSender emailSender)
        {
            _logger = logger;
            _service = service;
            _emailSender = emailSender;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting SendNotificationService");
            while (!stoppingToken.IsCancellationRequested)
            {
                await SendNotifications();
                await Task.Delay(Constant.SentNotificationDelay, stoppingToken);
            }
            _logger.LogInformation("Stopping SendNotificationService");
        }
        private async Task SendNotifications()
        {
            using (var scope = _service.CreateScope()) 
            {
                var db = scope.ServiceProvider.GetService<ApplicationDbContext>();
                var products = await db.Notifications.Include(n => n.Product).Select(n => n.Product).Distinct().Where(p=>p.ProductStatus==Product.Status.ACTIVE).ToListAsync();

                foreach (var product in products)
                {
                    _logger.LogInformation($"sending notifications product {product.Id}");
                    var notifications=await db.Notifications.Where(n => n.ProductId==product.Id).ToListAsync();
                    foreach(var notfy in notifications)
                    {

                        string html = $@"<h2 style=""margin-top: 36px; font-size: 24px; font-weight: bold;"">Product {product.Name} back in stock!</h2>
                                    <p style=""margin-top: 8px;"">
                                        {product.Name} back in stock hurry up and buy it now!
                                    </p>
    
                                    <p style=""margin: 30px 0px; text-align: center"">
                                        <a href=""{HtmlEncoder.Default.Encode(Constant.WebUrl)}"" style=""background-color: #3781f0; white-space: nowrap; color: white; border-color: transparent; border-width: 1px; border-radius: 0.375rem; font-size: 18px; padding-left: 16px; padding-right: 16px; padding-top: 10px; padding-bottom: 10px; text-decoration: none; margin-top: 4px; margin-bottom: 4px;"">
                                            Buy it now!
                                        </a>
                                    </p>";
                        await _emailSender.SendEmailAsync(notfy.Email, "Product notification", html);
                    }
                    db.Notifications.RemoveRange(notifications);
                    await db.SaveChangesAsync();
                }
            }
        }
    }
}
