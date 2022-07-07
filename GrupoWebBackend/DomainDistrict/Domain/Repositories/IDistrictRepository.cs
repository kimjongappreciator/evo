using System.Collections.Generic;
using System.Threading.Tasks;
using GrupoWebBackend.DomainDistrict.Domain.Models;
namespace GrupoWebBackend.DomainDistrict.Domain.Repositories
{
    public interface IDistrictRepository
    { 
        Task<IEnumerable<District>> ListAsync();
        Task<District> FindAsync(int id);
        Task AddAsync(District district);
        void UpdateAsync(District district);
        void Delete(District district);
        
    }
}