---
title: External Json Serializer
summary: How to use the external Json serializer in an endpoint.
related:
- nservicebus/serialization
---

## NServiceBus.Newtonsoft.Json

This samples uses the community run serializer [NServiceBus.Newtonsoft.Json](https://github.com/Particular/NServiceBus.Newtonsoft.Json) to provide full access to the [Newtonsoft Json.net](http://www.newtonsoft.com/json) API.


## Configuring to use NServiceBus.Newtonsoft.Json

snippet:config


## Diagnostic Mutator

A helper that will Write out the contents of any incoming message.

snippet:mutator


## The message send

snippet:message
  

## The Output

```
?{
  "OrderId": 9,
  "Date": "2015-09-15T10:23:44.9367871+10:00",
  "CustomerId": 12,
  "OrderItems": [
    {
      "ItemId": 6,
      "Quantity": 2
    },
    {
      "ItemId": 5,
      "Quantity": 4
    }
  ]
}
```
