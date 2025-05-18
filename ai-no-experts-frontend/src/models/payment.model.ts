export type Payment = {
    billingAddress: string;
    billingAddress2: string;
    cardNumber: string; // It's advisable to encrypt this field.
    country: string;
    cvv: string; // This field should be encrypted and not stored.
    email: string;
    expirationDate: string;
    fullName: string;
    phone: string;
    ssn: string; // It's advisable to encrypt this field.
}