using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NServiceBus;

public class TestingTransport
{
    string fullPath;
    string auditPath;
    int endpointInstanceCount;

    public TestingTransport([CallerMemberName] string key = null)
    {
        fullPath = Path.GetFullPath(key);
        auditPath = Path.Combine(fullPath, "audit");
        if (Directory.Exists(fullPath))
        {
            Directory.Delete(fullPath, true);
        }
    }

    public void ApplyToEndpoint(EndpointConfiguration endpointConfiguration)
    {
        endpointInstanceCount++;
        var transport = endpointConfiguration.UseTransport<LearningTransport>();
        endpointConfiguration.AuditProcessedMessagesTo("audit");
        transport.StorageDirectory(fullPath);
    }

    public async Task<List<AuditedMessageData>> GetProcessedMessages(params IEndpointInstance[] endpointInstances)
    {
        if (endpointInstances.Length != endpointInstanceCount)
        {
            throw new Exception();
        }
        await Task.Delay(100);
        var breaker = 0;
        while (true)
        {
            await Task.Delay(50);
            if (!AreMessagesPending())
            {
                break;
            }

            breaker++;
            if (breaker > 100)
            {
                throw new Exception("Breaker hit before all pending messages were flushed.");
            }
        }

        await Task.WhenAll(endpointInstances.Select(x => x.Stop()));

        return GetMessages().ToList();
    }

    IEnumerable<AuditedMessageData> GetMessages()
    {
        if (!Directory.Exists(auditPath))
        {
            yield break;
        }
        foreach (var metadataFile in Directory.EnumerateFiles(auditPath, "*.metadata.txt"))
        {
            var metadata = DeserializeMetadata(metadataFile);
            yield return new AuditedMessageData
            {
                MessageId = Guid.Parse(metadata[Headers.MessageId]),
                ProcessingEndpoint = metadata[Headers.ProcessingEndpoint],
                OriginatingEndpoint = metadata[Headers.OriginatingEndpoint],
                MessageType = GetMessageType(metadata),
                Metadata = metadata,
                Body = GetBody(metadataFile)
            };
        }
    }

    string GetBody(string metadataFile)
    {
        var fileId = Path.GetFileName(metadataFile).Replace(".metadata.txt", "");
        var bodyPath = Path.Combine(auditPath, ".bodies", $"{fileId}.body.txt");
        return File.ReadAllText(bodyPath);
    }

    static Type GetMessageType(Dictionary<string, string> metadata)
    {
        if (metadata.TryGetValue(Headers.EnclosedMessageTypes, out var messageTypeName))
        {
            return Type.GetType(messageTypeName);
        }

        return null;
    }

    static Dictionary<string, string> DeserializeMetadata(string messageMetadata)
    {
        return JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(messageMetadata));
    }

    bool AreMessagesPending()
    {
        return EndpointDirectories().Any(HasPendingMessages);
    }

    static bool HasPendingMessages(string endpointDirectory)
    {
        return Directory.EnumerateDirectories(endpointDirectory, ".pending")
            .Any(pending => Directory.EnumerateFiles(pending).Any());
    }

    IEnumerable<string> EndpointDirectories()
    {
        return Directory.EnumerateDirectories(fullPath);
    }
}