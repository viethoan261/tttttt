namespace WebFilm.Core.Interfaces.Services
{
    public interface IBaseService<TKey, TEntity>
    {
        IEnumerable<TEntity> GetAll();

        TEntity GetByID(TKey id);

        int Edit(TKey id, TEntity entity);

        int Add(TEntity entity);

        int Delete(TKey id);
    }
}
