using Hotel.Application.DTOs.BookingDTO;
using Hotel.Application.Interfaces;
using Hotel.Domain.Entities;
using Hotel.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Text;

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
    public async Task SendEmailAsync(string recipientEmail, bool isConfirmation, GetBookingDTO model, int count)
    {
        var emailBody = CreateEmailBody(isConfirmation,model,count); // Tạo nội dung email từ DTO với kiểu gửi xác nhận
        var sender = _configuration["EmailSettings:Sender"];
        var password = _configuration["EmailSettings:Password"];
        var host = _configuration["EmailSettings:Host"];
        var port = int.Parse(_configuration["EmailSettings:Port"]);

        var mailMessage = new MailMessage
        {
            From = new MailAddress(sender),
            Subject = isConfirmation ? "Xác nhận đặt phòng thành công" : "Thông báo đặt phòng",
            Body = emailBody,
            IsBodyHtml = true
        };

        mailMessage.To.Add(recipientEmail); // Địa chỉ email người nhận

        using (var smtpClient = new SmtpClient(host, port))
        {
            smtpClient.Credentials = new NetworkCredential(sender, password);
            smtpClient.EnableSsl = true;

            try
            {
                await smtpClient.SendMailAsync(mailMessage);
                return ;
            }
            catch (SmtpException smtpEx)
            {
                throw new Exception("Lỗi gửi email: " + smtpEx.Message, smtpEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi gửi email: " + ex.Message, ex);
            }
        }
    }
    private string CreateEmailBody(bool isConfirmation, GetBookingDTO bookingDTO, int count)
    {
        var sb = new StringBuilder();

        if (isConfirmation)
        {
            sb.AppendLine("<h1 style='color: #2E86C1; font-family: Arial, sans-serif;'>Xác nhận đặt phòng thành công</h1>");
            sb.AppendLine("<table style='width: 100%; border: 1px solid #ddd; border-collapse: collapse; font-family: Arial, sans-serif;'>");
            sb.AppendLine("<tr><th colspan='2'>Thông tin đặt phòng</th></tr>");
            sb.AppendLine($"<tr><td><strong>Mã đặt phòng:</strong></td><td>{bookingDTO.Id}</td></tr>");
            sb.AppendLine($"<tr><td><strong>Tên khách hàng:</strong></td><td>{bookingDTO.CustomerName}</td></tr>");
            sb.AppendLine($"<tr><td><strong>Ngày nhận phòng:</strong></td><td>{bookingDTO.CheckInDate}</td></tr>");
            sb.AppendLine($"<tr><td><strong>Ngày trả phòng:</strong></td><td>{bookingDTO.CheckOutDate}</td></tr>");
            sb.AppendLine($"<tr><td><strong>Tổng tiền:</strong></td><td>{bookingDTO.TotalAmount:N0} VND</td></tr>");
            sb.AppendLine("</table>");
        }
        else
        {
            if(count>2)
            {
                sb.AppendLine("<h1 style='color: #C0392B; font-family: Arial, sans-serif;'>Thông báo từ khách sạn</h1>");
                sb.AppendLine("<p>Quý khách đã hủy đặt phòng hoặc có thay đổi đối với đơn đặt phòng của mình. Vui lòng liên hệ với chúng tôi để biết thêm chi tiết.</p>");
                sb.AppendLine("<b>Quý khách vui lòng liên hệ với khách sạn để được hoàn tiền cọc</b>");
                sb.AppendLine($"<p>Mã đặt phòng của quý khách: {bookingDTO.Id}</p>");
                sb.AppendLine($"<p>Tổng số tiền đã thanh toán: {bookingDTO.Deposit:N0} VND</p>");
            }   
            else
            {
                sb.AppendLine("<h1 style='color: #C0392B; font-family: Arial, sans-serif;'>Thông báo từ khách sạn</h1>");
                sb.AppendLine("<p>Quý khách đã hủy đặt phòng hoặc có thay đổi đối với đơn đặt phòng của mình. Vui lòng liên hệ với chúng tôi để biết thêm chi tiết.</p>");
                sb.AppendLine("<b>Vì quý khách huỷ phòng ngoài khoản thời gian quy định nên sẽ không được hoàn cọc</b>");
                sb.AppendLine($"<p>Mã đặt phòng của quý khách: {bookingDTO.Id}</p>");
                sb.AppendLine($"<p>Tổng số tiền đã thanh toán: {bookingDTO.Deposit:N0} VND</p>");
            }    
           
        }

        return sb.ToString();
    }
    private async Task<string> CreateEmailBody(GetBookingDTO bookingDTO)
    {
        Customer customer = await _unitOfWork.GetRepository<Customer>().GetByIdAsync(bookingDTO.CustomerId);

        var sb = new StringBuilder();
        sb.AppendLine("<h1 style='color: #2E86C1; font-family: Arial, sans-serif;'>Xác nhận đặt phòng thành công</h1>");

        // Bắt đầu bảng
        sb.AppendLine("<table style='width: 100%; border: 1px solid #ddd; border-collapse: collapse; font-family: Arial, sans-serif;'>");

        // Thông tin đặt phòng
        sb.AppendLine("<tr style='background-color: #f2f2f2; text-align: center; font-weight: bold;'>");
        sb.AppendLine("<th colspan='2' style='padding: 12px; border: 1px solid #ddd;'>Thông tin đặt phòng</th>");
        sb.AppendLine("</tr>");
        sb.AppendLine($"<tr><td style='padding: 10px; border: 1px solid #ddd;'><strong>Mã đặt phòng:</strong></td><td style='padding: 10px; border: 1px solid #ddd;'>{bookingDTO.Id}</td></tr>");
        sb.AppendLine($"<tr><td style='padding: 10px; border: 1px solid #ddd;'><strong>Tên khách hàng:</strong></td><td style='padding: 10px; border: 1px solid #ddd;'>{customer.Name}</td></tr>");
        sb.AppendLine($"<tr><td style='padding: 10px; border: 1px solid #ddd;'><strong>Số điện thoại:</strong></td><td style='padding: 10px; border: 1px solid #ddd;'>{bookingDTO.PhoneNumber}</td></tr>");
        sb.AppendLine($"<tr><td style='padding: 10px; border: 1px solid #ddd;'><strong>Ngày đặt:</strong></td><td style='padding: 10px; border: 1px solid #ddd;'>{bookingDTO.BookingDate}</td></tr>");
        sb.AppendLine($"<tr><td style='padding: 10px; border: 1px solid #ddd;'><strong>Ngày nhận phòng:</strong></td><td style='padding: 10px; border: 1px solid #ddd;'>{bookingDTO.CheckInDate}</td></tr>");
        sb.AppendLine($"<tr><td style='padding: 10px; border: 1px solid #ddd;'><strong>Ngày trả phòng:</strong></td><td style='padding: 10px; border: 1px solid #ddd;'>{bookingDTO.CheckOutDate}</td></tr>");

        // Chi tiết phòng
        sb.AppendLine("<tr style='background-color: #f2f2f2; text-align: center; font-weight: bold;'>");
        sb.AppendLine("<th colspan='2' style='padding: 12px; border: 1px solid #ddd;'>Chi tiết phòng</th>");
        sb.AppendLine("</tr>");
        foreach (var detail in bookingDTO.BookingDetail)
        {
            sb.AppendLine($"<tr><td style='padding: 10px; border: 1px solid #ddd;'><strong>Phòng:</strong></td><td style='padding: 10px; border: 1px solid #ddd;'>{detail.RoomName}</td></tr>");
        }

        // Dịch vụ
        if (bookingDTO.Services.Count > 0)
        {
            sb.AppendLine("<tr style='background-color: #f2f2f2; text-align: center; font-weight: bold;'>");
            sb.AppendLine("<th colspan='2' style='padding: 12px; border: 1px solid #ddd;'>Dịch vụ</th>");
            sb.AppendLine("</tr>");
            sb.AppendLine("<tr><td style='padding: 10px; border: 1px solid #ddd;'><strong>Tên dịch vụ:</strong></td><td style='padding: 10px; border: 1px solid #ddd;'>Số lượng</td></tr>");
            foreach (var service in bookingDTO.Services)
            {
                sb.AppendLine($"<tr><td style='padding: 10px; border: 1px solid #ddd;'><strong>{service.ServiceName}</strong></td><td style='padding: 10px; border: 1px solid #ddd;'>{service.Quantity}</td></tr>");
            }
        }

        // Thông tin thanh toán
        sb.AppendLine("<tr style='background-color: #f2f2f2; text-align: center; font-weight: bold;'>");
        sb.AppendLine("<th colspan='2' style='padding: 12px; border: 1px solid #ddd;'>Thông tin thanh toán</th>");
        sb.AppendLine("</tr>");
        sb.AppendLine($"<tr><td style='padding: 10px; border: 1px solid #ddd;'><strong>Tổng tiền:</strong></td><td style='padding: 10px; border: 1px solid #ddd;'>{bookingDTO.TotalAmount:N0} VND</td></tr>");
        sb.AppendLine($"<tr><td style='padding: 10px; border: 1px solid #ddd;'><strong>Tiền khuyến mãi:</strong></td><td style='padding: 10px; border: 1px solid #ddd;'>{bookingDTO.PromotionalPrice:N0} VND</td></tr>");
        sb.AppendLine($"<tr><td style='padding: 10px; border: 1px solid #ddd;'><strong>Số tiền đã đặt cọc:</strong></td><td style='padding: 10px; border: 1px solid #ddd;'>{bookingDTO.Deposit:N0} VND</td></tr>");
        sb.AppendLine($"<tr><td style='padding: 10px; border: 1px solid #ddd;'><strong>Số tiền chưa thanh toán:</strong></td><td style='padding: 10px; border: 1px solid #ddd;'>{bookingDTO.UnpaidAmount:N0} VND</td></tr>");

        // Kết thúc bảng
        sb.AppendLine("</table>");

        return sb.ToString();
    }
   


    private async Task<string> CreateEmailBodyFinal(GetBookingDTO bookingDTO)
    {
        Customer customer = await _unitOfWork.GetRepository<Customer>().GetByIdAsync(bookingDTO.CustomerId);

        var sb = new StringBuilder();
        sb.AppendLine("<h2 style='color: #2E86C1;'>Cảm ơn quý khách hàng đã sử dụng dịch vụ của khách sạn <br>Xin hân hạnh phục vụ quý khách lần kế tiếp</h2>");

        // Bắt đầu bảng
        sb.AppendLine("<table border='1' style='width: 100%; border-collapse: collapse; font-family: Arial, sans-serif;'>");

        // Phần tiêu đề chính
        sb.AppendLine("<tr style='background-color: #f2f2f2; text-align: center; font-weight: bold; font-size: 16px;'>");
        sb.AppendLine("<th colspan='3' style='padding: 12px;'>Thông tin dịch vụ đã sử dụng</th>");
        sb.AppendLine("</tr>");

        // Thông tin khách hàng
        sb.AppendLine($"<tr><td colspan='2' style='padding: 10px;'><strong>Tên khách hàng:</strong></td><td style='padding: 10px;'>{customer.Name}</td></tr>");
        sb.AppendLine($"<tr><td colspan='2' style='padding: 10px;'><strong>Số điện thoại:</strong></td><td style='padding: 10px;'>{bookingDTO.PhoneNumber}</td></tr>");
        sb.AppendLine($"<tr><td colspan='2' style='padding: 10px;'><strong>Ngày nhận phòng:</strong></td><td style='padding: 10px;'>{bookingDTO.CheckInDate}</td></tr>");
        sb.AppendLine($"<tr><td colspan='2' style='padding: 10px;'><strong>Ngày trả phòng:</strong></td><td style='padding: 10px;'>{bookingDTO.CheckOutDate}</td></tr>");

        // Chi tiết phòng
        sb.AppendLine("<tr style='background-color: #f2f2f2; text-align: center; font-weight: bold; font-size: 14px;'>");
        sb.AppendLine("<th colspan='2' style='padding: 12px;'>Chi tiết phòng</th>");
        sb.AppendLine("</tr>");

        foreach (var detail in bookingDTO.BookingDetail)
        {
            sb.AppendLine($"<tr><td colspan='2' style='padding: 10px;'><strong>Phòng:</strong></td><td style='padding: 10px;'>{detail.RoomName}</td></tr>");
        }

        // Dịch vụ
        if (bookingDTO.Services.Count > 0)
        {
            sb.AppendLine("<tr style='background-color: #f2f2f2; text-align: center; font-weight: bold; font-size: 14px;'>");
            sb.AppendLine("<th colspan='3' style='padding: 12px;'>Dịch vụ</th>");
            sb.AppendLine("</tr>");
            sb.AppendLine("<tr><td colspan='2' style='padding: 10px;'><strong>Tên dịch vụ:</strong></td><td style='padding: 10px;'>Số lượng</td></tr>");

            foreach (var service in bookingDTO.Services)
            {
                sb.AppendLine($"<tr><td colspan='2' style='padding: 10px;'><strong>{service.ServiceName}</strong></td><td style='padding: 10px;'>{service.Quantity}</td></tr>");
            }
        }

        // Phạt tiền
        if (bookingDTO.Punishes.Count > 0)
        {
            sb.AppendLine("<tr style='background-color: #f2f2f2; text-align: center; font-weight: bold; font-size: 14px;'>");
            sb.AppendLine("<th colspan='3' style='padding: 12px;'>Phạt tiền</th>");
            sb.AppendLine("</tr>");
            sb.AppendLine("<tr><td style='padding: 10px;'><strong>Tên:</strong></td><td style='padding: 10px;'>Số lượng</td><td style='padding: 10px;'>Tiền phạt</td></tr>");

            foreach (var item in bookingDTO.Punishes)
            {
                sb.AppendLine($"<tr><td style='padding: 10px;'><strong>{item.FacilitiesName}</strong></td><td style='padding: 10px;'>{item.Quantity}</td><td style='padding: 10px;'>{item.Fine:N0} VND</td></tr>");
            }
        }

        // Thông tin thanh toán
        sb.AppendLine("<tr style='background-color: #f2f2f2; text-align: center; font-weight: bold; font-size: 14px;'>");
        sb.AppendLine("<th colspan='3' style='padding: 12px;'>Thông tin thanh toán</th>");
        sb.AppendLine("</tr>");
        sb.AppendLine($"<tr><td colspan='2' style='padding: 10px;'><strong>Tiền khuyến mãi:</strong></td><td style='padding: 10px;'>{bookingDTO.PromotionalPrice:N0} VND</td></tr>");
        sb.AppendLine($"<tr><td colspan='2' style='padding: 10px;'><strong>Số tiền đã thanh toán:</strong></td><td style='padding: 10px;'>{bookingDTO.Deposit:N0} VND</td></tr>");
        sb.AppendLine($"<tr><td colspan='2' style='padding: 10px;'><strong>Tổng tiền:</strong></td><td style='padding: 10px;'>{bookingDTO.TotalAmount:N0} VND</td></tr>");
        sb.AppendLine($"<tr><td colspan='2' style='padding: 10px;'><strong>Số tiền chưa thanh toán:</strong></td><td style='padding: 10px;'>{0:N0} VND</td></tr>");

        sb.AppendLine("</table>");

        return sb.ToString();
    }
}

