import { Injectable } from '@angular/core';
import { EncryptionService } from 'angular-encryption-service';

@Injectable({
    providedIn: 'root'
})
export class EncryptionLocalService {
    constructor(private _encryptionService: EncryptionService) { }
    demoEncrypt(userId: string, passPhrase: string): Promise<string> {
        return this.generateKey(userId).then(key => {
            return this._encryptionService.encrypt(passPhrase, key);
        });
    }
    decrypt(userId: string, phrase: string): Promise<string> {
        return this.generateKey(userId).then(key => {
            return this._encryptionService.decrypt(phrase, key);
        });
    }
    generateKey(phrase: string): Promise<CryptoKey> {
        return this._encryptionService.generateKey(phrase);
    }
}
