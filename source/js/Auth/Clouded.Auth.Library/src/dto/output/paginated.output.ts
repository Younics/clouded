import { PageInfoOutput } from '@/dto/output/page-info.output';

export class PaginatedOutput {
  public items: any[] = [];
  public pageInfo: PageInfoOutput = new PageInfoOutput();
}
