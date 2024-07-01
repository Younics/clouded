import { HttpClient } from '@/http.client';

export abstract class BaseClient {
  protected readonly apiKey: string;
  protected readonly secretKey: string;
  protected readonly httpClient: HttpClient;

  protected constructor(apiUrl: string, apiKey: string, secretKey: string, cloudedKey?: string) {
    this.apiKey = apiKey;
    this.secretKey = secretKey;
    this.httpClient = new HttpClient(apiUrl, 8000, apiKey, secretKey, cloudedKey);
  }
}
