import { BaseManagementClient } from '@/base/base-management.client';
import { CloudedPromise } from '@/clouded-promise';

export class AuthManagementClient extends BaseManagementClient {
  private readonly organizationUrlSegment: string = '/v1/organizations';
  private readonly userUrlSegment: string = '/v1/users';
  private readonly roleUrlSegment: string = '/v1/roles';
  private readonly permissionUrlSegment: string = '/v1/permissions';
  private readonly machinesUrlSegment: string = '/v1/machines';

  constructor(apiUrl: string, apiKey: string, secretKey: string, cloudedKey?: string) {
    super(apiUrl, apiKey, secretKey, cloudedKey);
  }

  public machines(): CloudedPromise<any[]> {
    return this.getAll(this.machinesUrlSegment);
  }

  public machineById(id: number | string, withUsers = false): CloudedPromise<any> {
    return this.getById(this.machinesUrlSegment, id, { withUsers });
  }

  public machineCreate(input: any): CloudedPromise<any> {
    return this.create(this.machinesUrlSegment, input);
  }

  public machineUpdate(id: number | string, input: any): CloudedPromise<any> {
    return this.update(this.machinesUrlSegment, id, input);
  }

  public machineDelete(id: number | string): Promise<boolean> {
    return this.delete(this.machinesUrlSegment, id);
  }
  public organizations(): CloudedPromise<any[]> {
    return this.getAll(this.organizationUrlSegment);
  }

  public organizationById(id: number | string, withUsers = false): CloudedPromise<any> {
    return this.getById(this.organizationUrlSegment, id, { withUsers });
  }

  public organizationCreate(input: any): CloudedPromise<any> {
    return this.create(this.organizationUrlSegment, input);
  }

  public organizationUpdate(id: number | string, input: any): CloudedPromise<any> {
    return this.update(this.organizationUrlSegment, id, input);
  }

  public organizationDelete(id: number | string): Promise<boolean> {
    return this.delete(this.organizationUrlSegment, id);
  }

  public organizationUsersAdd(id: number | string, userIds: number[] | string[]): Promise<boolean> {
    return this.addOrRemove(this.organizationUrlSegment, id, 'users/add', userIds);
  }

  public organizationUsersRemove(
    id: number | string,
    userIds: number[] | string[],
  ): Promise<boolean> {
    return this.addOrRemove(this.organizationUrlSegment, id, 'users/remove', userIds);
  }

  public users(filter?: {
    search?: string;
    roleIds?: number[] | string[];
    permissionIds?: number[] | string[];
    organizationId?: number | string;
  }): CloudedPromise<any[]> {
    let queryParams = '';

    if (filter) {
      const { search, roleIds, permissionIds, organizationId } = filter;
      queryParams = [
        search && `search=${search}`,
        roleIds && roleIds.map((x) => `roleIds=${x}`).join('&'),
        permissionIds && permissionIds.map((x) => `permissionIds=${x}`).join('&'),
        organizationId && `organizationId=${organizationId}`,
      ]
        .filter((x) => x)
        .join('&');
    }

    return this.getAll(`${this.userUrlSegment}?${queryParams}`);
  }

  public usersFind(filter: {
    search?: string;
    roleIds?: number[] | string[];
    permissionIds?: number[] | string[];
    organizationId?: number | string;
  }): CloudedPromise<any[]> {
    const { search, roleIds, permissionIds, organizationId } = filter;
    const queryParams = [
      search && `search=${search}`,
      roleIds && roleIds.map((x) => `roleIds=${x}`).join('&'),
      permissionIds && permissionIds.map((x) => `permissionIds=${x}`).join('&'),
      organizationId && `organizationId=${organizationId}`,
    ]
      .filter((x) => x)
      .join('&');

    return this.httpClient
      .get(`${this.userUrlSegment}/find?${queryParams}`)
      .then((response) => response.data);
  }

  public userById(
    id: number | string,
    withRoles = false,
    withPermissions = false,
  ): CloudedPromise<any> {
    return this.getById(this.userUrlSegment, id, { withRoles, withPermissions });
  }

  public userByIdentity(
    identity: number | string,
    withRoles = false,
    withPermissions = false,
  ): CloudedPromise<any> {
    return this.httpClient
      .get(`${this.userUrlSegment}/by_identity/${identity}`, {
        params: { withRoles, withPermissions },
      })
      .then((response) => response.data);
  }

  public userCreate(input: any): CloudedPromise<any> {
    return this.create(this.userUrlSegment, input);
  }

  public userUpdate(id: number | string, input: any): CloudedPromise<any> {
    return this.update(this.userUrlSegment, id, input);
  }

  public userDelete(id: number | string): Promise<boolean> {
    return this.delete(this.userUrlSegment, id);
  }

  public userRolesAdd(
    id: number | string,
    roleIds: number[] | string[],
    organizationId?: number | string,
  ): Promise<boolean> {
    return this.addOrRemove(this.userUrlSegment, id, 'roles/add', {
      roleIds,
      organizationId,
    });
  }

  public userRolesRemove(
    id: number | string,
    roleIds: number[] | string[],
    organizationId?: number | string,
  ): Promise<boolean> {
    return this.addOrRemove(this.userUrlSegment, id, 'roles/remove', {
      roleIds,
      organizationId,
    });
  }

  public userPermissionsAdd(
    id: number | string,
    permissionIds: number[] | string[],
    organizationId?: number | string,
  ): Promise<boolean> {
    return this.addOrRemove(this.userUrlSegment, id, 'permissions/add', {
      permissionIds,
      organizationId,
    });
  }

  public userPermissionsRemove(
    id: number | string,
    permissionIds: number[] | string[],
    organizationId?: number | string,
  ): Promise<boolean> {
    return this.addOrRemove(this.userUrlSegment, id, 'permissions/remove', {
      permissionIds,
      organizationId,
    });
  }

  public roles(): CloudedPromise<any[]> {
    return this.getAll(this.roleUrlSegment);
  }

  public roleById(id: number | string): CloudedPromise<any> {
    return this.getById(this.roleUrlSegment, id);
  }

  public roleByIdentity(identity: number | string, withPermissions = false): CloudedPromise<any> {
    return this.httpClient
      .get(`${this.roleUrlSegment}/by_identity/${identity}`, {
        params: { withPermissions },
      })
      .then((response) => response.data);
  }

  public roleCreate(input: number | string): CloudedPromise<any> {
    return this.create(this.roleUrlSegment, input);
  }

  public roleUpdate(id: number | string, input: any): CloudedPromise<any> {
    return this.update(this.roleUrlSegment, id, input);
  }

  public roleDelete(id: number | string): Promise<boolean> {
    return this.delete(this.roleUrlSegment, id);
  }

  public rolePermissionsAdd(
    id: number | string,
    permissionIds: number | string[],
  ): Promise<boolean> {
    return this.addOrRemove(this.roleUrlSegment, id, 'permissions/add', permissionIds);
  }

  public rolePermissionsRemove(
    id: number | string,
    permissionIds: number | string[],
  ): Promise<boolean> {
    return this.addOrRemove(this.roleUrlSegment, id, 'permissions/remove', permissionIds);
  }

  public permissions(): CloudedPromise<any[]> {
    return this.getAll(this.permissionUrlSegment);
  }

  public permissionById(id: number | string): CloudedPromise<any> {
    return this.getById(this.permissionUrlSegment, id);
  }

  public permissionByIdentity(identity: number | string): CloudedPromise<any> {
    return this.httpClient
      .get(`${this.roleUrlSegment}/by_identity/${identity}`)
      .then((response) => response.data);
  }

  public permissionCreate(input: number | string): CloudedPromise<any> {
    return this.create(this.permissionUrlSegment, input);
  }

  public permissionUpdate(id: number | string, input: number | string): CloudedPromise<any> {
    return this.update(this.permissionUrlSegment, id, input);
  }

  public permissionDelete(id: number | string): Promise<boolean> {
    return this.delete(this.permissionUrlSegment, id);
  }
}
