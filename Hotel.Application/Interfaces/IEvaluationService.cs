

using Hotel.Application.DTOs.EvaluationDTO;
using Hotel.Application.PaggingItems;
using Microsoft.AspNetCore.Http;

namespace Hotel.Application.Interfaces
{
    public interface IEvaluationService
    {
        Task<PaginatedList<GetEvaluationDTO>> GetPageAsync(int index, int pageSize, string idSearch, string customerID, string roomTypeDetailId);
        Task<List<GetEvaluationDTO>> GetEvaluationAsync(string roomTypeDetailId);
        Task CreateEvaluationAsync(ICollection<IFormFile>? images, PostEvaluationDTO model);
        Task UpdateEvaluationAsync(string id);
        Task PutEvaluationAsync( PostEvaluationDTO model);
    }
}
