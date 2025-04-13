namespace Business.Factories;

public interface IEntityFactory<TEntity, TModel, TAddViewModel, TEditViewModel>
{
    public TEntity MapModelToEntity(TModel model);
    public TEntity MapModelToEntity(TAddViewModel model);
    public TEntity MapModelToEntity(TEditViewModel model);
}
