using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.AuditFilter;
using Xunit;

public class Tests
{
    [Fact]
    public async Task Skip_with_attribute_and_default_to_include()
    {
        var message = new MessageWithExcludeFromAudit();
        var result = await Send(message, _ => _.FilterAuditQueue(FilterResult.IncludeInAudit));
        Assert.Empty(result);
    }
    [Fact]
    public async Task Skip_with_attribute_and_default_to_exclude()
    {
        var message = new MessageWithExcludeFromAudit();
        var result = await Send(message, _ => _.FilterAuditQueue(FilterResult.ExcludeFromAudit));
        Assert.Empty(result);
    }

    [Fact]
    public async Task Audit_with_attribute_and_default_to_include()
    {
        var message = new MessageWithIncludeInAudit();
        var result = await Send(message, _ => _.FilterAuditQueue(FilterResult.IncludeInAudit));
        Assert.True(result.Count == 1);
    }

    [Fact]
    public async Task Audit_with_attribute_and_default_to_exclude()
    {
        var message = new MessageWithIncludeInAudit();
        var result = await Send(message, _ => _.FilterAuditQueue(FilterResult.ExcludeFromAudit));
        Assert.True(result.Count == 1);
    }
    [Fact]
    public async Task Simple_message_and_default_to_include()
    {
        var message = new SimpleMessage();
        var result = await Send(message, _ => _.FilterAuditQueue(FilterResult.IncludeInAudit));
        Assert.True(result.Count == 1);
    }

    [Fact]
    public async Task Simple_message_and_default_to_exclude()
    {
        var message = new SimpleMessage();
        var result = await Send(message, _ => _.FilterAuditQueue(FilterResult.ExcludeFromAudit));
        Assert.True(result.Count == 0);
    }
    [Fact]
    public async Task Simple_message_and_delegate_to_include()
    {
        var message = new SimpleMessage();
        var result = await Send(message, _ => _.FilterAuditQueue(
            (instance, headers) => FilterResult.IncludeInAudit));
        Assert.True(result.Count == 1);
    }

    [Fact]
    public async Task Simple_message_and_delegate_to_exclude()
    {
        var message = new SimpleMessage();
        var result = await Send(message, _ => _.FilterAuditQueue(
            (instance, headers) => FilterResult.ExcludeFromAudit));
        Assert.True(result.Count == 0);
    }

    static async Task<List<AuditedMessageData>> Send(object message, Action<EndpointConfiguration> addAuditFilter, [CallerMemberName] string key = null)
    {
        var testingTransport = new TestingTransport(key);
        var configuration = new EndpointConfiguration("AuditFilterSample");
        configuration.UsePersistence<LearningPersistence>();
        testingTransport.ApplyToEndpoint(configuration);
        addAuditFilter(configuration);

        var endpoint = await Endpoint.Start(configuration);
        await endpoint.SendLocal(message);
        return await testingTransport.GetProcessedMessages(endpoint);
    }
}