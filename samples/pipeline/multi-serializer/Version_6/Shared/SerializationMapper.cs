﻿using System;
using System.Collections.Generic;
using System.Linq;
using NServiceBus;
using NServiceBus.MessageInterfaces;
using NServiceBus.Serialization;
using NServiceBus.Serializers.Json;
using NServiceBus.Serializers.XML;
using NServiceBus.Settings;

#region serialization-mapper
public class SerializationMapper
{
    JsonMessageSerializer jsonSerializer;
    XmlMessageSerializer xmlSerializer;

    public SerializationMapper(IMessageMapper mapper,Conventions conventions, ReadOnlySettings settings)
    {
        jsonSerializer = new JsonMessageSerializer(mapper);
        xmlSerializer = new XmlMessageSerializer(mapper, conventions);
        List<Type> messageTypes = settings.GetAvailableTypes().Where(conventions.IsMessageType).ToList();
        xmlSerializer.Initialize(messageTypes);
    }
    
    public IMessageSerializer GetSerializer(Dictionary<string, string> headers)
    {
        string contentType;
        if (!headers.TryGetValue(Headers.ContentType, out contentType))
        {
            //default to Json
            return jsonSerializer;
        }
        if (contentType == jsonSerializer.ContentType)
        {
            return jsonSerializer;
        }
        if (contentType == xmlSerializer.ContentType)
        {
            return xmlSerializer;
        }
        throw new Exception($"Could not derive serializer for contentType='{contentType}'");
    }

    public IMessageSerializer GetSerializer(Type messageType)
    {
        bool isJsonMessage = messageType.ContainsAttribute<SerializeWithJsonAttribute>();
        bool isXmlMessage = messageType.ContainsAttribute<SerializeWithXmlAttribute>();
        if (isXmlMessage && isJsonMessage)
        {
            throw new Exception($"Choose either [SerializeWithXml] or [SerializeWithJson] for serialization of '{messageType.Name}'.");
        }
        if (isXmlMessage)
        {
            return xmlSerializer;
        }
        //default to json
        return jsonSerializer;
    }
}
#endregion