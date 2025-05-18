export type Payment = {
    billingAddress: string;
    billingAddress2: string;
    cardHash: string; // Use hashed value instead of raw card number
    country: string;
    cvvHash: string; // Use hashed value instead of raw cvv
    email: string;
    expirationDate: string;
    fullName: string;
    phone: string;
    ssnHash: string; // Use hashed value instead of raw ssn
}