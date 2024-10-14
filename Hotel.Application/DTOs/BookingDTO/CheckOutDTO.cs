namespace Hotel.Application.DTOs.BookingDTO
{
    public class CheckOutDTO
    {
        public string BookingId {  get; set; } = string.Empty;
        public virtual ICollection<PostPunishesDTO> ? Punishes {  get; set; }
    }
}
