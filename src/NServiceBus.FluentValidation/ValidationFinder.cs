using FluentValidation;
using Result = FluentValidation.AssemblyScanner.AssemblyScanResult;

namespace NServiceBus.FluentValidation;

public static class ValidationFinder
{
    public static IEnumerable<Result> FindValidatorsInAssemblyContaining<T>(
        bool throwForNonPublicValidators = true,
        bool throwForNoValidatorsFound = true) =>
        FindValidatorsInAssemblyContaining(typeof(T), throwForNonPublicValidators, throwForNoValidatorsFound);

    public static IEnumerable<Result> FindValidatorsInAssemblyContaining(
        Type type,
        bool throwForNonPublicValidators = true,
        bool throwForNoValidatorsFound = true) =>
        FindValidatorsInAssembly(type.Assembly, throwForNonPublicValidators, throwForNoValidatorsFound);

    public static IEnumerable<Result> FindValidatorsInAssembly(
        Assembly assembly,
        bool throwForNonPublicValidators = true,
        bool throwForNoValidatorsFound = true)
    {
        var assemblyName = assembly.GetName().Name;
        if (throwForNonPublicValidators)
        {
            var openGenericType = typeof(IValidator<>);
            var nonPublicValidators = assembly
                .GetTypes()
                .Where(
                    type =>
                        type is
                        {
                            IsPublic: false,
                            IsNestedPublic: false,
                            IsAbstract: false,
                            IsGenericTypeDefinition: false
                        } &&
                        type.GetInterfaces()
                            .Any(_ => _.IsGenericType && i.GetGenericTypeDefinition() == openGenericType)
                )
                .ToList();
            if (nonPublicValidators.Any())
            {
                throw new($"Found some non public validators were found in {assemblyName}:{Environment.NewLine}{string.Join(Environment.NewLine, nonPublicValidators.Select(_ => _.FullName))}.");
            }
        }

        var results = AssemblyScanner.FindValidatorsInAssembly(assembly).ToList();
        if (throwForNoValidatorsFound && !results.Any())
        {
            throw new($"No validators were found in {assemblyName}.");
        }

        return results;
    }

    /// <summary>
    /// Find all  assemblies matching *.Messages.dll that exist in AppDomain.CurrentDomain.BaseDirectory.
    /// Files starting with "System." or "Microsoft." are excluded.
    /// </summary>
    public static IEnumerable<Result> FindValidatorsInMessagesSuffixedAssemblies(
        bool throwForNonPublicValidators = true,
        bool throwForNoValidatorsFound = true)
    {
        var directory = AppDomain.CurrentDomain.BaseDirectory;
        var assemblies = Directory.EnumerateFiles(directory, "*.Messages.dll")
            .Where(file =>
            {
                var fileName = Path.GetFileName(file).ToLowerInvariant();
                return !fileName.StartsWith("microsoft.") &&
                       !fileName.StartsWith("system.");
            })
            .ToList();
        if (!assemblies.Any())
        {
            throw new($"Could not find any assemblies matching *.Messages.dll. Directory: {directory}");
        }

        return assemblies.SelectMany(file =>
            FindValidatorsInAssembly(
                Assembly.LoadFrom(file),
                throwForNonPublicValidators,
                throwForNoValidatorsFound));
    }
}