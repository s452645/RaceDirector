using backend.Models;
using backenend.Models;

namespace backend.Services
{
    public class CarService
    {
        private readonly BackendContext _context;

        public CarService(BackendContext context)
        {
            _context = context;
        }

        public List<Car> GetCarsByIds(List<Guid> ids)
        {
            return _context.Cars.Where(car => ids.Contains(car.Id)).ToList();
        }
    }
}
