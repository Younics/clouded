import usersData from './users';
import machinesData from './machines';
import rolesData from './roles';
import permissionsData from './permissions';
import { EntityBag } from '../types/entityBag';
import { AuthManagementClient } from '../../auth-management.client';

export default function (client: AuthManagementClient): EntityBag[] {
  return [
    {
      entityName: 'user',
      createFnc: client.userCreate,
      updateFnc: client.userUpdate,
      readFnc: client.userById,
      deleteFnc: client.userDelete,
      listFnc: client.users,
      data: usersData,
      createdIds: [],
    } as EntityBag,
    // {
    //     entityName: "organization",
    //     createFnc:client.organizationCreate,
    //     updateFnc:client.organizationUpdate,
    //     readFnc:client.organizationById,
    //     deleteFnc:client.organizationDelete,
    //     data:organizationsData,
    // } as EntityBag,
    {
      entityName: 'machine',
      createFnc: client.machineCreate,
      updateFnc: client.machineUpdate,
      readFnc: client.machineById,
      deleteFnc: client.machineDelete,
      listFnc: client.machines,
      data: machinesData,
      createdIds: [],
    } as EntityBag,
    {
      entityName: 'role',
      createFnc: client.roleCreate,
      updateFnc: client.roleUpdate,
      readFnc: client.roleById,
      deleteFnc: client.roleDelete,
      listFnc: client.roles,
      data: rolesData,
      createdIds: [],
    } as EntityBag,
    {
      entityName: 'permission',
      createFnc: client.permissionCreate,
      updateFnc: client.permissionUpdate,
      readFnc: client.permissionById,
      deleteFnc: client.permissionDelete,
      listFnc: client.permissions,
      data: permissionsData,
      createdIds: [],
    } as EntityBag,
  ];
}
