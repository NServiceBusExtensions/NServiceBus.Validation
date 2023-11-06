class ValidatorInfo
{
    public bool HasValidators { get; }
    public List<IValidator> Validators { get; }

    public ValidatorInfo(List<IValidator> validators, bool hasValidators)
    {
        Validators = validators;
        HasValidators = hasValidators;
    }
}