import { TokenOutput } from '@/dto/output/token.output';

export class OAuthOutput {
  public accessToken!: TokenOutput;
  public refreshToken!: TokenOutput;
}
