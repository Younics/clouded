import { BaseClient } from '@/base/base.client';
import { PaginatedOutput } from '@/dto/output/paginated.output';
import { CloudedPromise } from '@/clouded-promise';
import { CloudedOutput } from '@/dto/output/clouded.output';

export class BaseManagementClient extends BaseClient {
  constructor(apiUrl: string, apiKey: string, secretKey: string, cloudedKey?: string) {
    super(apiUrl, apiKey, secretKey, cloudedKey);
  }

  protected getPaginated(
    urlPathSegment: string,
    page: number,
    size: number,
  ): CloudedPromise<PaginatedOutput> {
    return this.httpClient
      .get(urlPathSegment + '/paginated', {
        params: {
          page,
          size,
        },
      })
      .then((response) => response.data as CloudedOutput<PaginatedOutput>);
  }

  protected getAll(urlPathSegment: string): CloudedPromise<any[]> {
    return this.httpClient
      .get(urlPathSegment)
      .then((response) => response.data as CloudedOutput<any[]>);
  }

  protected getById(
    urlPathSegment: string,
    id: number | string,
    params?: any,
  ): CloudedPromise<any> {
    return this.httpClient
      .get(`${urlPathSegment}/${id}`, { params })
      .then((response) => response.data as CloudedOutput<any>);
  }

  protected create(urlPathSegment: string, input: any): CloudedPromise<any> {
    return this.httpClient.post(urlPathSegment, input).then((response) => {
      return response.data as CloudedOutput<any>;
    });
  }

  protected update(urlPathSegment: string, id: number | string, input: any): CloudedPromise<any> {
    return this.httpClient.patch(`${urlPathSegment}/${id}`, input).then((response) => {
      return response.data as CloudedOutput<any>;
    });
  }

  protected async delete(urlPathSegment: string, id: number | string): Promise<boolean> {
    const response = await this.httpClient.delete(`${urlPathSegment}/${id}`);
    return response.status == 200;
  }

  protected async addOrRemove(
    urlPathSegment: string,
    id: number | string,
    nextUrlPathSegment: string,
    input: any,
  ): Promise<boolean> {
    const response = await this.httpClient.put(
      `${urlPathSegment}/${id}/${nextUrlPathSegment}`,
      input,
    );

    return response.status == 200;
  }
}
