using System;
using System.Linq.Expressions;
using System.Security.Authentication;

namespace Benchmarks.Utils;

public static class Errors
{
    public static void AliasNullOrEmpty()
    {
        throw new Exception("Alias is null or empty");
    }
    
    public static void CantFormatToString(object value)
    {
        throw new Exception($"Can't format '{value}' to string");
    }

    public static void CommandPipelineIsNotActivated(Type request)
    {
        throw new Exception($"Command pipeline for {request} is not activated");
    }

    public static Exception ConnectionStringNotFound(string sourceName)
    {
        return new Exception($"Connection string for '{sourceName}' is not found");
    }

    public static Exception ConfigurationNotFound(string sectionKey)
    {
        return new Exception($"Configuration of '{sectionKey}' is not found");
    }

    public static Exception ConfigurationNotFound(string sectionKey, string value)
    {
        return new Exception($"Configuration value of '{sectionKey}:{value}' is not found");
    }

    public static Exception ConfigurationNotFound<T, TValue>(Expression<Func<T, TValue>> property)
    {
        const string typeName = nameof(T);
        return property.Body is MemberExpression member
            ? new Exception($"Configuration value of '{typeName}.{member.Member.Name}' is not found")
            : ConfigurationNotFound(typeName);
    }

    public static void KeyAlreadyExists(int key)
    {
        throw new Exception($"Key {key} already exists");
    }

    public static void KeyNotFound(int key)
    {
        throw new Exception($"Key {key} isn't found");
    }

    public static void MapperNotFound(Type input, Type output)
    {
        throw new Exception($"Mapper from {input} to {output} isn't found");
    }

    public static void MapperNotFound(Type input, Type output, Type arg)
    {
        throw new Exception($"Mapper from {input} to {output} with argument {arg} isn't found");
    }

    public static Exception MapperArgumentRequired(Type input, Type output, Type arg)
    {
        return new Exception($"Mapper from {input} to {output} should has argument {arg} for mapping");
    }

    public static void NotAuthorized()
    {
        throw new AuthenticationException("User authentication isn't found");
    }

    public static void RequestPipelineIsNotActivated(Type request)
    {
        throw new Exception($"Request pipeline for {request} is not activated");
    }


    public static Exception DependencyNotFound(Type dependencyType)
    {
        return new Exception($"Required dependency {dependencyType} isn't injected");
    }
}