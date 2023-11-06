class ValidatorInfo(List<IValidator> validators, bool hasValidators)
{
    public bool HasValidators { get; } = hasValidators;
    public List<IValidator> Validators { get; } = validators;
}