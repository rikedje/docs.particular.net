---
title: Upgrade encryption service configuration
summary: Instructions on how to upgrade the Rijndael property encryption configuration to use key identifiers
tags:
 - upgrade
related:
 - nservicebus/security/encryption
---

## Summary

This upgrade document explains what steps to take when upgrading NServiceBus endpoints to make use of the following hotfix releases:

- [NServiceBus 3.3.17](https://github.com/Particular/NServiceBus/releases/tag/3.3.17)
- [NServiceBus 4.7.8](https://github.com/Particular/NServiceBus/releases/tag/4.7.8)
- [NServiceBus 5.0.7](https://github.com/Particular/NServiceBus/releases/tag/5.0.7)
- [NServiceBus 5.1.5](https://github.com/Particular/NServiceBus/releases/tag/5.1.5)
- [NServiceBus 5.2.9](https://github.com/Particular/NServiceBus/releases/tag/5.2.9)


## Upgrade Steps

To deploy this fix throughout your system, you must first upgrade your endpoints with the latest versions of NServiceBus, then update endpoint configurations. Optionally, you may also want to generate new encryption keys.


### Upgrade endpoints

All endpoints using encryption need to be upgraded to the latest patch release.

- Version 3.x.x requires at least v3.3.17
- Version 4.x.x requires at least v4.7.8
- Version 5.x.x requires at least v5.0.7, v5.1.5 or v5.2.9

For example, if currently using NServiceBus 4.7.x, you must update to the latest patch release 4.7.8.


## Compatibility

It is not required to upgrade all endpoints simultaneously. There has not been a change in encryption or decryption. If a key identifier header is present in a message received by an endpoint that does not know this header then it will decrypt the message.


## Upgrade steps

Steps:

- Stop the endpoint
- Deploy the new version
- Edit the endpoint configuration, adding key identifiers to at least the current encryption key, if not the backup keys as well.
- Start the endpoint


NOTE: If only the new version is deployed without updating the configuration, then the endpoint will start but will not be able to encrypt messages or decrypt messages that have a key identifier.


### Update endpoint configurations

The following configuration examples demonstrate how to update existing endpoints while still using the same encryption key to not require all endpoints to be down at once and to allow keys to be backwards compatible.


#### XML Configuration

You can choose to keep the current encryption key and will still be able to decrypt messages already in flight:

```
<RijndaelEncryptionServiceConfig Key="do-not-use-this-encryption-key!!" KeyIdentifier="2015-10" />
  <ExpiredKeys>
    <add Key="an-expired-encryption-key-here-!" />
    <add Key="another-expired-key-!-----------" />
  </ExpiredKeys>
</RijndaelEncryptionServiceConfig>
```


#### Code API (v5+)

When recompiling your project, you will get an obsolete warning. Change the current method to the new one that has new arguments that allow to pass a dictionary to lookup keys and an optional list of keys that will be used to decrypt messages that do not have a key identifier header.

NOTE: Keys need to be available in the expired keys list to be backwards compatible.

**From**
```
busConfiguration.RijndaelEncryptionService("do-not-use-this-encryption-key!!")
```

**To**

Send encrypted messages with the existing key

Does not require all endpoints to be stopped, updated, configured and started simultaneously.

```
var key = Encoding.ASCII.GetBytes("do-not-use-this-encryption-key!!");

busConfiguration.RijndaelEncryptionService(
    "2015-10",
    new Dictionary<string,byte[]>{{ "2015-10", key}},
    new[]{ key } // Required for decrypting message without a key identifier
    );
```


## Generating new keys

You should generate a new encryption key if you are using ASCII keys with a key length of 16 characters to overcome the 7-bit limit that weakens the encryption key.

Switch to Base64 256 bits keys if possible, or to at least ASCII 24 character keys to be backwards compatible with pre v5.x.x endpoints and have stronger encryption.

NOTE: Base64 keys can only be configured for NServiceBus v5+ and are not compatible with earlier versions.

The following link contains samples for:

- Powershell
- LinqPad
- OpenSLL
- CryptoKeyGenerator

[Generating secure random strong encryption keys](/nservicebus/security/generating-encryption-keys.md)


## Locating possible corrupted data

Property encryption is often used for the following types of data:

- Credit card numbers
- Social security numbers
- Bank accounts

Investigate locations where such values are stored and check if these values are in an expected format by verifying that these values have a:

- correct length
- correct character set (credit card should only contain numbers)

Search in log files that indicate data validation issues. These could indicate that properties have been decrypted with an incorrect key.


## Recovering data

Recovering corrupted data is *only* possible when the original encrypted messages are moved to an audit queue.

We do not have a tool that helps in recovering. Please contact support if you suspect possible data corruption.
