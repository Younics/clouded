import { BaseClient } from '@/base/base.client';
import { OAuthInput } from '@/dto/input/oauth.input';
import { OAuthOutput } from '@/dto/output/oauth.output';
import { TokenRefreshInput } from '@/dto/input/token-refresh.input';
import { AuthManagementClient } from '@/auth-management.client';
import { ResetPasswordInput } from '@/dto/input/reset-password.input';
import { CloudedPromise } from '@/clouded-promise';
import { CloudedOutput } from '@/dto/output/clouded.output';
import { TokenRevokeInput } from '@/dto/input/token-revoke.input';
import { ValidateOutput } from '@/dto/output/validate.output';
import { AccessTokenInput } from '@/dto/input/access-token.input';
import { OauthSocialInput } from '@/dto/input/oauth-social.input';

export class AuthClient extends BaseClient {
  public readonly management: AuthManagementClient;

  constructor(apiUrl: string, apiKey: string, secretKey: string, cloudedKey?: string) {
    super(apiUrl, apiKey, secretKey, cloudedKey);

    this.management = new AuthManagementClient(apiUrl, apiKey, secretKey, cloudedKey);
  }

  public appleLoginUrl(): CloudedPromise<string> {
    return this.httpClient
      .get('/v1/social/apple/login-url')
      .then((response) => response as CloudedOutput<string>);
  }

  public appleMe(code: string): CloudedPromise<any> {
    return this.httpClient
      .get(`/v1/social/apple/me/${code}`)
      .then((response) => response.data as CloudedOutput<any>);
  }

  public appleToken(input: OauthSocialInput): CloudedPromise<OAuthOutput> {
    return this.httpClient
      .post('/v1/social/apple/token', { ...input })
      .then((response) => response.data as CloudedOutput<OAuthOutput>);
  }

  public facebookLoginUrl(): CloudedPromise<string> {
    return this.httpClient
      .get('/v1/social/facebook/login-url')
      .then((response) => response as CloudedOutput<string>);
  }

  public facebookMe(code: string): CloudedPromise<any> {
    return this.httpClient
      .get(`/v1/social/facebook/me/${code}`)
      .then((response) => response.data as CloudedOutput<any>);
  }

  public facebookToken(input: OauthSocialInput): CloudedPromise<OAuthOutput> {
    return this.httpClient
      .post('/v1/social/facebook/token', { ...input })
      .then((response) => response.data as CloudedOutput<OAuthOutput>);
  }

  public googleLoginUrl(): CloudedPromise<string> {
    return this.httpClient
      .get('/v1/social/google/login-url')
      .then((response) => response as CloudedOutput<string>);
  }

  public googleMe(code: string): CloudedPromise<any> {
    return this.httpClient
      .get(`/v1/social/google/me/${code}`)
      .then((response) => response.data as CloudedOutput<any>);
  }

  public googleToken(input: OauthSocialInput): CloudedPromise<OAuthOutput> {
    return this.httpClient
      .post('/v1/social/google/token', { ...input })
      .then((response) => response.data as CloudedOutput<OAuthOutput>);
  }

  public validate(input: AccessTokenInput): CloudedPromise<ValidateOutput> {
    return this.httpClient
      .post('/v1/oauth/token/validate', { ...input })
      .then((response) => response.data as CloudedOutput<ValidateOutput>);
  }

  public token(input: OAuthInput): CloudedPromise<OAuthOutput> {
    return this.httpClient
      .post('/v1/oauth/token', { ...input, apiKey: this.apiKey, secretKey: this.secretKey })
      .then((response) => response.data as CloudedOutput<OAuthOutput>);
  }

  public tokenRefresh(input: TokenRefreshInput): CloudedPromise<OAuthOutput> {
    return this.httpClient
      .post('/v1/oauth/token/refresh', { ...input, apiKey: this.apiKey, secretKey: this.secretKey })
      .then((response) => response.data as CloudedOutput<OAuthOutput>);
  }

  public tokenRevoke(input: TokenRevokeInput): Promise<boolean> {
    return this.httpClient
      .post('/v1/oauth/token/revoke', { ...input })
      .then((response) => response.status === 200);
  }

  public permissions(input: AccessTokenInput): CloudedPromise<string[]> {
    return this.httpClient
      .post('/v1/oauth/permissions', { ...input })
      .then((response) => response.data as CloudedOutput<string[]>);
  }

  public forgotPassword(identity: string): Promise<boolean> {
    return this.httpClient
      .post('/v1/oauth/forgot-password', identity)
      .then((response) => response.status === 200);
  }

  public resetPassword(input: ResetPasswordInput): Promise<boolean> {
    return this.httpClient
      .post('/v1/oauth/reset-password', input)
      .then((response) => response.status === 200);
  }
}
