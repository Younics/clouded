import fetch from 'node-fetch';

export class HttpClient {
  private readonly _baseUrl: string;
  private readonly _timeout: number;
  private readonly _apiKey: string;
  private readonly _secretKey: string;
  private readonly _cloudedKey: string | undefined;

  constructor(
    baseUrl: string,
    timeout: number,
    apiKey: string,
    secretKey: string,
    cloudedKey?: string,
  ) {
    this._baseUrl = baseUrl;
    this._timeout = timeout;
    this._apiKey = apiKey;
    this._secretKey = secretKey;
    this._cloudedKey = cloudedKey;
  }

  public async get(url: string, options = {} as any) {
    this.apiKeysCheck();

    return new Promise<{ status: number; data: any }>((resolve, reject) => {
      const { timeout = this._timeout, headers } = options;
      const controller = new AbortController();
      const id = setTimeout(() => controller.abort(), timeout);
      fetch(`${this._baseUrl}${url}`, {
        ...options,
        headers: {
          ...headers,
          'Content-type': 'application/json',
          accept: 'application/json, text/plain, */*',
          ...(this._apiKey && this._secretKey
            ? { 'X-CLOUDED-MACHINE-KEY': btoa(`${this._apiKey}:${this._secretKey}`) }
            : {}),
          ...(this._cloudedKey ? { 'X-CLOUDED-KEY': this._cloudedKey } : {}),
        },
        signal: controller.signal,
      })
        .then(async (response) => {
          await this.handleResponse(response, reject, resolve);
        })
        .finally(() => clearTimeout(id));
    });
  }

  public async delete(url: string, options = {} as any) {
    this.apiKeysCheck();

    return new Promise<{ status: number; data: any }>((resolve, reject) => {
      const { timeout = this._timeout, headers } = options;
      const controller = new AbortController();
      const id = setTimeout(() => controller.abort(), timeout);
      fetch(`${this._baseUrl}${url}`, {
        ...options,
        method: 'DELETE',
        headers: {
          ...headers,
          'Content-type': 'application/json',
          accept: 'application/json, text/plain, */*',
          ...(this._apiKey && this._secretKey
            ? { 'X-CLOUDED-MACHINE-KEY': btoa(`${this._apiKey}:${this._secretKey}`) }
            : {}),
          ...(this._cloudedKey ? { 'X-CLOUDED-KEY': this._cloudedKey } : {}),
        },
        signal: controller.signal,
      })
        .then(async (response) => {
          await this.handleResponse(response, reject, resolve);
        })
        .finally(() => clearTimeout(id));
    });
  }

  public async post(url: string, data: any, options = {} as any) {
    this.apiKeysCheck();

    return new Promise<{ status: number; data: any }>((resolve, reject) => {
      const { timeout = this._timeout, headers } = options;
      const controller = new AbortController();
      const id = setTimeout(() => controller.abort(), timeout);
      fetch(`${this._baseUrl}${url}`, {
        ...options,
        method: 'POST',
        headers: {
          ...headers,
          'Content-type': 'application/json',
          accept: 'application/json, text/plain, */*',
          ...(this._apiKey && this._secretKey
            ? { 'X-CLOUDED-MACHINE-KEY': btoa(`${this._apiKey}:${this._secretKey}`) }
            : {}),
          ...(this._cloudedKey ? { 'X-CLOUDED-KEY': this._cloudedKey } : {}),
        },
        signal: controller.signal,
        body: JSON.stringify(data),
      })
        .then(async (response) => {
          console.log({ response });
          await this.handleResponse(response, reject, resolve);
        })
        .finally(() => clearTimeout(id));
    });
  }

  public async patch(url: string, data: any, options = {} as any) {
    this.apiKeysCheck();

    return new Promise<{ status: number; data: any }>((resolve, reject) => {
      const { timeout = this._timeout, headers } = options;
      const controller = new AbortController();
      const id = setTimeout(() => controller.abort(), timeout);
      fetch(`${this._baseUrl}${url}`, {
        ...options,
        method: 'PATCH',
        headers: {
          ...headers,
          'Content-type': 'application/json',
          accept: 'application/json, text/plain, */*',
          ...(this._apiKey && this._secretKey
            ? { 'X-CLOUDED-MACHINE-KEY': btoa(`${this._apiKey}:${this._secretKey}`) }
            : {}),
          ...(this._cloudedKey ? { 'X-CLOUDED-KEY': this._cloudedKey } : {}),
        },
        signal: controller.signal,
        body: JSON.stringify(data),
      })
        .then(async (response) => {
          await this.handleResponse(response, reject, resolve);
        })
        .finally(() => clearTimeout(id));
    });
  }

  public async put(url: string, data: any, options = {} as any) {
    this.apiKeysCheck();

    return new Promise<{ status: number; data: any }>((resolve, reject) => {
      const { timeout = this._timeout, headers } = options;
      const controller = new AbortController();
      const id = setTimeout(() => controller.abort(), timeout);
      fetch(`${this._baseUrl}${url}`, {
        ...options,
        method: 'PUT',
        headers: {
          ...headers,
          'Content-type': 'application/json',
          accept: 'application/json, text/plain, */*',
          ...(this._apiKey && this._secretKey
            ? { 'X-CLOUDED-MACHINE-KEY': btoa(`${this._apiKey}:${this._secretKey}`) }
            : {}),
          ...(this._cloudedKey ? { 'X-CLOUDED-KEY': this._cloudedKey } : {}),
        },
        signal: controller.signal,
        body: JSON.stringify(data),
      })
        .then(async (response) => {
          await this.handleResponse(response, reject, resolve);
        })
        .finally(() => clearTimeout(id));
    });
  }

  private async handleResponse(
    response: any,
    reject: (reason?: any) => void,
    resolve: (
      value:
        | PromiseLike<{
            status: number;
            data: any;
          }>
        | { status: number; data: any },
    ) => void,
  ) {
    const text = await response.text();
    let json = null;
    try {
      json = JSON.parse(text);
    } catch (err) {
      console.error(err);
    }

    if (!response.ok) {
      reject(json);
      return;
    }

    resolve({
      status: response.status,
      data: json,
    });
  }

  protected apiKeysCheck() {
    if (!this._cloudedKey && !this._apiKey && !this._secretKey)
      throw new Error('Api keys are not set!');
  }
}
