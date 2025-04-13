namespace Data.Factories;

public interface IModelFactory<TEntity, TModel>
{
    public TModel MapEntityToModel(TEntity entity);
}
