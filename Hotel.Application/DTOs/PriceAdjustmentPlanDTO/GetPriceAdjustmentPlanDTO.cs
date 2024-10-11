namespace Hotel.Application.DTOs.PriceAdjustmentPlanDTO
{
    public class GetPriceAdjustmentPlanDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal AdjustmentValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Description { get; set; } = string.Empty;
    }
}
