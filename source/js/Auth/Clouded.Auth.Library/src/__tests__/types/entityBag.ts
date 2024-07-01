import { CloudedPromise } from '../../../lib/clouded-promise';

export class EntityBag {
  public entityName!: string;
  public createFnc!: (input: any) => CloudedPromise<any>;
  public updateFnc!: (id: string, input: any) => CloudedPromise<any>;
  public readFnc!: (id: string) => CloudedPromise<any>;
  public deleteFnc!: (id: string) => Promise<boolean>;
  public listFnc!: () => CloudedPromise<any[]>;
  public data: any[] = [];
  public createdIds: string[] = [];
}
