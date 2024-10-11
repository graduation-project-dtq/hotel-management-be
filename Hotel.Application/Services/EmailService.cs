using Hotel.Application.DTOs.BookingDTO;
using Hotel.Application.Interfaces;
using Hotel.Domain.Entities;
using Hotel.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWork _unitOfWork;
    public EmailService(IConfiguration configuration, IUnitOfWork unitOfWork)
    {
        _configuration = configuration;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> SendBookingConfirmationEmailAsync(Booking booking, string customerId, GetBookingDTO bookingDTO)
    {
        var emailBody = await CreateEmailBody(bookingDTO); // Tạo nội dung email từ DTO
        var sender = _configuration["EmailSettings:Sender"];
        var password = _configuration["EmailSettings:Password"];
        var host = _configuration["EmailSettings:Host"];
        var port = int.Parse(_configuration["EmailSettings:Port"]);

        var mailMessage = new MailMessage
        {
            From = new MailAddress(sender),
            Subject = "Xác nhận đặt phòng thành công",
            Body = emailBody,
            IsBodyHtml = true
        };

        mailMessage.To.Add(customerId); // Địa chỉ email của khách hàng

        using (var smtpClient = new SmtpClient(host, port))
        {
            smtpClient.Credentials = new NetworkCredential(sender, password);
            smtpClient.EnableSsl = true;

            try
            {
                await smtpClient.SendMailAsync(mailMessage);
                return true;
            }
            catch (SmtpException smtpEx)
            {

                throw new Exception("Lỗi gửi email xác nhận: " + smtpEx.Message, smtpEx);
            }
            catch (Exception ex)
            {

                throw new Exception("Lỗi gửi email xác nhận: " + ex.Message, ex);
            }
        }
    }

    private async Task<string> CreateEmailBody(GetBookingDTO bookingDTO)
    {
        // Lấy thông tin khách hàng bất đồng bộ
        Customer customer = await _unitOfWork.GetRepository<Customer>().GetByIdAsync(bookingDTO.CustomerId);

        var sb = new StringBuilder();
        sb.AppendLine("<h1>Xác nhận đặt phòng thành công</h1>");

        // Bắt đầu bảng
        sb.AppendLine("<table border='1' style='width: 100%; border-collapse: collapse;'>");
        sb.AppendLine("<tr><th colspan='2'>Thông tin đặt phòng</th></tr>");
        sb.AppendLine($"<tr><td><strong>Mã đặt phòng:</strong></td><td>{bookingDTO.Id}</td></tr>");
        sb.AppendLine($"<tr><td><strong>Tên khách hàng:</strong></td><td>{customer.Name}</td></tr>");
        sb.AppendLine($"<tr><td><strong>Ngày đặt:</strong></td><td>{bookingDTO.BookingDate}</td></tr>");
        sb.AppendLine($"<tr><td><strong>Ngày nhận phòng:</strong></td><td>{bookingDTO.CheckInDate}</td></tr>");
        sb.AppendLine($"<tr><td><strong>Ngày trả phòng:</strong></td><td>{bookingDTO.CheckOutDate}</td></tr>");

        // Thêm chi tiết phòng
        sb.AppendLine("<tr><th colspan='2'>Chi tiết phòng</th></tr>");
        foreach (var detail in bookingDTO.BookingDetail)
        {
            sb.AppendLine($"<tr><td><strong>Phòng:</strong></td><td>{detail.RoomName}</td></tr>");
        }

        // Thêm dịch vụ
        sb.AppendLine("<tr><th colspan='2'>Dịch vụ</th></tr>");
        foreach (var service in bookingDTO.Services)
        {
            sb.AppendLine($"<tr><td><strong>Dịch vụ:</strong></td><td>{service.ServiceName}</td></tr>");
        }
        sb.AppendLine("<tr><th colspan='2'>Thông tin thanh toán</th></tr>");
        sb.AppendLine($"<tr><td><strong>Tổng tiền:</strong></td><td>{bookingDTO.TotalAmount:N0} VND</td></tr>");
        sb.AppendLine($"<tr><td><strong>Giá khuyến mãi:</strong></td><td>{bookingDTO.PromotionalPrice:N0} VND</td></tr>");
        sb.AppendLine($"<tr><td><strong>Số tiền đã đặt cọc:</strong></td><td>{bookingDTO.Deposit:N0} VND</td></tr>");
        sb.AppendLine($"<tr><td><strong>Số tiền chưa thanh toán:</strong></td><td>{bookingDTO.UnpaidAmount:N0} VND</td></tr>");
        // Kết thúc bảng
        sb.AppendLine("</table>");

        return sb.ToString();
    }


}

