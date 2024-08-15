import { AuthClient } from '../auth.client';
import { ErrorOutput } from '../dto/output/error.output';

const client = new AuthClient(
  'http://localhost:8001',
  'Q8zaz9MGErvY9mG7krKsV395dogXV77Y638560341163111220ddsLQplSgoSPkgS5Tu2rlhJdX5UcRLO5',
  'oDsxRBr7BTH33Ko0L5RCxm5eHvXLPZxBabTTOCClsTcvs0Zk',
);

describe('auth token', () => {
  it('should return tokens', async function () {
    const { data: result } = await client.token({
      identity: 'test@test.sk',
      password: 'qwerty',
    });

    expect(result).toBeDefined();
    expect(result.accessToken).toBeDefined();
    expect(result.refreshToken).toBeDefined();
  });

  it('should throw BadCredentialsException', async function () {
    let err = null;
    try {
      await client.token({
        identity: 'test@test.sk',
        password: 'qwertyk',
      });
    } catch (error) {
      err = error as ErrorOutput;
    }

    console.log({ err });

    expect(err?.errors).toBeDefined();
    expect(err?.errors[0].type).toBe('BadCredentialsException');
  });
});

describe('auth refresh token', () => {
  it('should refresh tokens', async () => {
    const { data: tokens } = await client.token({
      identity: 'test@test.sk',
      password: 'qwerty',
    });
    const { data: result } = await client.tokenRefresh({
      accessToken: tokens.accessToken.token,
      refreshToken: tokens.refreshToken.token,
    });

    expect(result).toBeDefined();
    expect(result.accessToken).toBeDefined();
    expect(result.refreshToken).toBeDefined();
  });

  it('should throw InvalidTokenException', async () => {
    const { data: tokens } = await client.token({
      identity: 'test@test.sk',
      password: 'qwerty',
    });

    let err = null;
    try {
      await client.tokenRefresh({
        accessToken: tokens.accessToken.token,
        refreshToken: tokens.refreshToken.token + 'asd',
      });
    } catch (error) {
      err = error as ErrorOutput;
    }

    expect(err?.errors).toBeDefined();
    expect(err?.errors[0].type).toBe('InvalidTokenException');
  });
});

describe('auth revoke token', () => {
  it('should revoke tokens', async () => {
    const { data: tokens } = await client.token({
      identity: 'test@test.sk',
      password: 'qwerty',
    });
    const result = await client.tokenRevoke({
      refreshToken: tokens.refreshToken.token,
      allOfUser: false,
    });

    expect(result).toBeDefined();
    expect(result).toBe(true);
  });

  it('should throw InvalidTokenException', async () => {
    const { data: tokens } = await client.token({
      identity: 'test@test.sk',
      password: 'qwerty',
    });

    let err = null;
    try {
      await client.tokenRevoke({
        refreshToken: tokens.refreshToken.token + 'asd',
        allOfUser: false,
      });
    } catch (error) {
      err = error as ErrorOutput;
    }

    expect(err?.errors).toBeDefined();
    expect(err?.errors[0].type).toBe('InvalidTokenException');
  });
});

describe('auth validate token', () => {
  it('should validate tokens', async () => {
    const { data: tokens } = await client.token({
      identity: 'test@test.sk',
      password: 'qwerty',
    });
    const { data: result } = await client.validate({ accessToken: tokens.accessToken.token });

    expect(result).toBeDefined();
    expect(result.userId).toBeTruthy();
    expect(result.expiresAt).toBeTruthy();
  });

  it('should throw InvalidTokenException', async () => {
    const { data: tokens } = await client.token({
      identity: 'test@test.sk',
      password: 'qwerty',
    });

    let err = null;
    try {
      await client.validate({ accessToken: tokens.accessToken.token + 'asd' });
    } catch (error) {
      err = error as ErrorOutput;
    }

    expect(err?.errors).toBeDefined();
    expect(err?.errors[0].type).toBe('InvalidTokenException');
  });
});

describe('auth forgot password', () => {
  it('should forgot password', async () => {
    const result = await client.forgotPassword('test@test.sk');

    expect(result).toBeDefined();
    expect(result).toBeTruthy();
  });

  it('should throw BadCredentialsException', async () => {
    let err = null;
    try {
      await client.forgotPassword('stringss');
    } catch (error) {
      err = error as ErrorOutput;
    }

    expect(err?.errors).toBeDefined();
    expect(err?.errors[0].type).toBe('BadCredentialsException');
  });
});
