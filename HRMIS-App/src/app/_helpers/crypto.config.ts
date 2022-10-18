import { CryptConfigProvider } from 'angular-encryption-service';
 export const AppCryptConfigProvider: CryptConfigProvider = {
    getSalt(): Promise<string> {
      // TODO: implement providing a salt, which should be unique per user and
      // base64-encoded.
      return Promise.resolve('saltsalt');
    }
  };