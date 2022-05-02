using FluentValidation;

record CacheResult(IReadOnlyList<IValidator> Validators)
{
    public bool HasValidator => Validators.Any();
}