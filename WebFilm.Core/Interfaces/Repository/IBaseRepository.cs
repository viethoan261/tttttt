namespace WebFilm.Core.Interfaces.Repository
{
    public interface IBaseRepository<TKey, TEntity>
    {
        IEnumerable<TEntity> GetAll();

        TEntity GetByID(TKey id);

        int Edit(TKey id, TEntity entity);

        int Add(TEntity entity);

        int Delete(TKey id);
    }
}
