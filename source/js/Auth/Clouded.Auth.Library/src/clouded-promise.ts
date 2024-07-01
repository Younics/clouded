import { ErrorOutput } from '@/dto/output/error.output';
import { CloudedOutput } from '@/dto/output/clouded.output';

export class CloudedPromise<TSuccess> extends Promise<CloudedOutput<TSuccess>> {
  constructor(
    executor: (
      resolve: (value: CloudedOutput<TSuccess> | PromiseLike<CloudedOutput<TSuccess>>) => void,
      reject: (reason: ErrorOutput) => void,
    ) => void,
  ) {
    super(executor);
    Object.setPrototypeOf(this, new.target.prototype); // restore prototype chain
  }
}
