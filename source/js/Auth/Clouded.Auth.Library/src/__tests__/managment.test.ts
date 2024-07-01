import { AuthManagementClient } from '../auth-management.client';
import { ErrorOutput } from '../dto/output/error.output';
import testDataFnc from './data/data';

const client = new AuthManagementClient(
  'http://localhost:8001',
  'GXvxVmRpZouoT5sQMews7mpP9duBAkGGmrr2jsUJ8JbF3w7pDqmEbFwQ3f5qAa73ATdfmEKiJjDq6qdr2fmQ8p2gthDg4RujapA9atKTjhxQ888taYkkgZ3fP5KSB5nh',
);

describe('managment entities', () => {
  const testsData = testDataFnc(client);
  for (const testData of testsData) {
    describe('Entity: ' + testData.entityName, () => {
      it('should create', async function () {
        for (const entity of testData.data) {
          const { data: result } = await testData.createFnc.call(client, entity);

          console.log({ result });
          expect(result).toBeDefined();
          expect(result.id).toBeDefined();

          testData.createdIds.push(result.id);
        }
      });

      it('should update', async function () {
        for (const entity of testData.data) {
          const index = testData.data.indexOf(entity);
          console.log(testData.createdIds[index], entity);
          const { data: result } = await testData.updateFnc.call(
            client,
            testData.createdIds[index],
            entity,
          );

          expect(result).toBeDefined();
          expect(result.id).toBeDefined();
        }
      });

      it('should read', async function () {
        for (const entity of testData.data) {
          const index = testData.data.indexOf(entity);
          const { data: result } = await testData.readFnc.call(client, testData.createdIds[index]);

          expect(result).toBeDefined();
          expect(result.id).toBe(testData.createdIds[index]);
        }
      });

      it('should read list', async function () {
        const { data: result } = await testData.listFnc.call(client);

        expect(result).toBeDefined();
        expect(result.length).toBeGreaterThan(0);
      });

      it('should delete', async function () {
        for (const entity of testData.data) {
          const index = testData.data.indexOf(entity);
          const result = await testData.deleteFnc.call(client, testData.createdIds[index]);

          expect(result).toBeDefined();
          expect(result).toBe(true);
        }
      });

      it('should throw not found on', async function () {
        for (const entity of testData.data) {
          let err = null;
          try {
            const index = testData.data.indexOf(entity);
            await testData.readFnc.call(client, testData.createdIds[index]);
          } catch (error) {
            err = error as ErrorOutput;
          }

          expect(err?.errors).toBeDefined();
          expect(err?.errors[0].type).toBe('NotFoundException');
        }
      });
    });
  }
});
