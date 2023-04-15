using backend.Models;
using backend.Models.Cars;

namespace backend.Services.Cars
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