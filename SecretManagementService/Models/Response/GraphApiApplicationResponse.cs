using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace SecretManagementService.Models.Response;


public record GraphApiApplicationResponse
{
    public string? id { get; set; }
    public object? deletedDateTime { get; set; }
    public string? appId { get; set; }
    public object? applicationTemplateId { get; set; }
    public object? disabledByMicrosoftStatus { get; set; }
    public DateTime createdDateTime { get; set; }
    public string? displayName { get; set; }
    public object? description { get; set; }
    public object? groupMembershipClaims { get; set; }
    public string[]? identifierUris { get; set; }
    public object? isDeviceOnlyAuthSupported { get; set; }
    public bool? isFallbackPublicClient { get; set; }
    public object? nativeAuthenticationApisEnabled { get; set; }
    public object? notes { get; set; }
    public string? publisherDomain { get; set; }
    public object? serviceManagementReference { get; set; }
    public string? signInAudience { get; set; }
    public object[]? tags { get; set; }
    public object? tokenEncryptionKeyId { get; set; }
    public object? uniqueName { get; set; }
    public object? samlMetadataUrl { get; set; }
    public object? defaultRedirectUri { get; set; }
    public object? certification { get; set; }
    public object? optionalClaims { get; set; }
    public object? servicePrincipalLockConfiguration { get; set; }
    public object? requestSignatureVerification { get; set; }
    public object[]? addIns { get; set; }
    public Api? api { get; set; }
    public object[]? appRoles { get; set; }
    public Info? info { get; set; }
    public object[]? keyCredentials { get; set; }
    public Parentalcontrolsettings? parentalControlSettings { get; set; }
    public Passwordcredential[]? passwordCredentials { get; set; }
    public Publicclient? publicClient { get; set; }
    public Requiredresourceaccess[]? requiredResourceAccess { get; set; }
    public Verifiedpublisher? verifiedPublisher { get; set; }
    public Web? web { get; set; }
    public Spa? spa { get; set; }
}

public record Api
{
    public object? acceptMappedClaims { get; set; }
    public object[]? knownClientApplications { get; set; }
    public int? requestedAccessTokenVersion { get; set; }
    public Oauth2permissionscopes[]? oauth2PermissionScopes { get; set; }
    public object[]? preAuthorizedApplications { get; set; }
}

public record Oauth2permissionscopes
{
    public string? adminConsentDescription { get; set; }
    public string? adminConsentDisplayName { get; set; }
    public string? id { get; set; }
    public bool isEnabled { get; set; }
    public string? type { get; set; }
    public string? userConsentDescription { get; set; }
    public string? userConsentDisplayName { get; set; }
    public string? value { get; set; }
}

public record Info
{
    public object? logoUrl { get; set; }
    public object? marketingUrl { get; set; }
    public object? privacyStatementUrl { get; set; }
    public object? supportUrl { get; set; }
    public object? termsOfServiceUrl { get; set; }
}

public record Parentalcontrolsettings
{
    public object[]? countriesBlockedForMinors { get; set; }
    public string? legalAgeGroupRule { get; set; }
}

public record Publicclient
{
    public string?[]? redirectUris { get; set; }
}

public record Verifiedpublisher
{
    public object? displayName { get; set; }
    public object? verifiedPublisherId { get; set; }
    public object? addedDateTime { get; set; }
}

public record Web
{
    public string? homePageUrl { get; set; }
    public object? logoutUrl { get; set; }
    public string?[]? redirectUris { get; set; }
    public Implicitgrantsettings? implicitGrantSettings { get; set; }
    public Redirecturisetting[]? redirectUriSettings { get; set; }
}

public record Implicitgrantsettings
{
    public bool enableAccessTokenIssuance { get; set; }
    public bool enableIdTokenIssuance { get; set; }
}

public record Redirecturisetting
{
    public string? uri { get; set; }
    public object? index { get; set; }
}

public record Spa
{
    public object[]? redirectUris { get; set; }
}

public record Passwordcredential
{
    public string? customKeyIdentifier { get; set; }
    public string? displayName { get; set; }
    public DateTime endDateTime { get; set; }
    public string? hint { get; set; }
    public string? keyId { get; set; }
    public object? secretText { get; set; }
    public DateTime startDateTime { get; set; }
}

public record Requiredresourceaccess
{
    public string? resourceAppId { get; set; }
    public Resourceaccess[]? resourceAccess { get; set; }
}

public record Resourceaccess
{
    public string? id { get; set; }
    public string? type { get; set; }
}
