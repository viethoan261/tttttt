using System.Security.Principal;
using WebFilm.Core.Interfaces.Repository;

namespace WebFilm.Core.Services
{
    public class BaseService<TKey, TEntity>
    {
        IBaseRepository<TKey, TEntity> _baseRepository;
        private IUserRepository userRepository;

        public BaseService(IBaseRepository<TKey, TEntity> baseRepository)
        {
            _baseRepository = baseRepository;
        }

        public BaseService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public IEnumerable<TEntity> GetAll()
        {
            var entity = _baseRepository.GetAll();
            return entity;
        }

        public TEntity GetByID(TKey id)
        {
            var entity = _baseRepository.GetByID(id);
            return entity;
        }

        public int Edit(TKey id, TEntity entity)
        {
            return _baseRepository.Edit(id, entity);
        }

        public int Add(TEntity entity)
        {
            return _baseRepository.Add(entity);
        }

        public int Delete(TKey id)
        {
            return _baseRepository.Delete(id);
        }

    }
}
